using System.Collections;
using System.Runtime.InteropServices;

namespace DataProcessor.source.Core.ValueStorage
{
    /// <summary>
    /// this class provides storage for nullable double values, allowing for efficient memory usage and null tracking.
    /// </summary>
    internal class DoubleValueStorage : AbstractValueStorage, IEnumerable<object?>
    {
        private readonly double[] values;
        private readonly NullBitMap nullBitMap;
        private readonly GCHandle handle;

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= values.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
        }

        internal DoubleValueStorage(double?[] array)
        {
            values = new double[array.Length];
            nullBitMap = new NullBitMap(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                {
                    nullBitMap.SetNull(i, true);
                    values[i] = default;
                }
                else
                {
                    values[i] = Convert.ToDouble(array[i]);
                    nullBitMap.SetNull(i, false);
                }
            }
            handle = GCHandle.Alloc(values, GCHandleType.Pinned);
        }

        internal DoubleValueStorage(double[] array, bool copy = true)
        {
            if (copy)
            {
                values = new double[array.Length];
                Array.Copy(array, values, array.Length);
            }
            else
            {
                values = array;
            }
            nullBitMap = new NullBitMap(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                nullBitMap.SetNull(i, false);
            }
            handle = GCHandle.Alloc(values, GCHandleType.Pinned);
        }

        internal override Type ElementType => typeof(double);

        internal override StorageKind storageKind => StorageKind.Double;
        internal override int Count => values.Length;

        internal double[] NonNullValues
        {
            get
            {
                double[] result = new double[values.Length - NullIndices.Count()];
                int current_idx = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    if (!nullBitMap.IsNull(i))
                    {
                        result[current_idx] = values[i];
                        current_idx++;
                    }
                }
                return result;
            }
        }

        internal ReadOnlySpan<double> ValuesSpan => values;

        internal NullBitMap NullBitmap => nullBitMap;

        internal override nint GetNativeBufferPointer()
        {
            return handle.AddrOfPinnedObject();
        }

        internal override object? GetValue(int index)
        {
            ValidateIndex(index);
            return nullBitMap.IsNull(index) ? null : values[index];
        }

        internal override IEnumerable<int> NullIndices
        {
            get
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (nullBitMap.IsNull(i))
                        yield return i;
                }
            }
        }

        internal override void SetValue(int index, object? value)
        {
            ValidateIndex(index);
            if (value == null)
            {
                nullBitMap.SetNull(index, true);
                values[index] = default;
                return;
            }

            if (value is IConvertible convertible)
            {
                try
                {
                    values[index] = Convert.ToDouble(convertible);
                    nullBitMap.SetNull(index, false);
                    return;
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Cannot convert value to double: {e.Message}", e);
                }
            }

            throw new ArgumentException("Value must be a numeric type or null.");
        }

        public override IEnumerator<object?> GetEnumerator()
        {
            for (int i = 0; i < values.Length; i++)
            {
                yield return nullBitMap.IsNull(i) ? null : values[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
