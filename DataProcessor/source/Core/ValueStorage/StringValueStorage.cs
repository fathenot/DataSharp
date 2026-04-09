using System.Collections;
using System.Text;
using DataProcessor.source.UserSettings;

namespace DataProcessor.source.Core.ValueStorage
{
    /// <summary>
    /// Provides storage for an array of nullable strings, with functionality to manage and access the data.
    /// </summary>
    internal class StringStorage : AbstractValueStorage, IEnumerable<object?>
    {
        private readonly string?[] strings;
        private readonly NullBitMap nullBitMap;

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= strings.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
        }

        internal StringStorage(string?[] strings, bool copy = true)
        {
            nullBitMap = new NullBitMap(strings.Length);
            if (!copy)
            {
                this.strings = strings;
                for (int i = 0; i < strings.Length; i++)
                {
                    this.strings[i] = UserSettings.UserConfig.NormalizeUnicode ? strings[i]?.Normalize(NormalizationForm.FormC) : strings[i];
                    nullBitMap.SetNull(i, this.strings[i] == null);
                }
            }
            else
            {
                this.strings = new string?[strings.Length];
                for (int i = 0; i < strings.Length; i++)
                {
                    string? s = strings[i];
                    this.strings[i] = UserSettings.UserConfig.NormalizeUnicode ? s?.Normalize(NormalizationForm.FormC) : s;
                    nullBitMap.SetNull(i, this.strings[i] == null);
                }
            }
        }

        internal override StorageKind storageKind => StorageKind.String;

        internal string?[] Strings
        {
            get { return strings; }
        }

        internal ReadOnlySpan<string?> ValuesSpan => strings;

        internal NullBitMap NullBitmap => nullBitMap;

        internal string[] NonNullValues
        {
            get
            {
                string[] result = new string[strings.Length - NullIndices.Count()];
                int current_idx = 0;
                for (int i = 0; i < strings.Length; i++)
                {
                    if (strings[i] != null)
                    {
                        result[current_idx] = strings[i]!;
                        current_idx++;
                    }
                }
                return result;
            }
        }

        internal override Type ElementType => typeof(string);

        internal override nint GetNativeBufferPointer()
        {
            throw new NotSupportedException("StringStorage does not support native buffer pointer access.");
        }

        internal override object? GetValue(int index)
        {
            ValidateIndex(index);
            return strings[index];
        }

        internal override void SetValue(int index, object? value)
        {
            if (index < 0 || index >= strings.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            if (value != null && value is not string)
            {
                throw new ArgumentException("Value must be a string or null.", nameof(value));
            }
            strings[index] = ((string?)value)?.Normalize(NormalizationForm.FormC);
            nullBitMap.SetNull(index, value == null);
        }

        internal override int Count => strings.Length;

        internal override IEnumerable<int> NullIndices
        {
            get
            {
                for (int i = 0; i < strings.Length; i++)
                {
                    if (nullBitMap.IsNull(i))
                    {
                        yield return i;
                    }
                }
            }
        }

        public override IEnumerator<object?> GetEnumerator()
        {
            for (int i = 0; i < strings.Length; i++)
            {
                yield return strings[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
