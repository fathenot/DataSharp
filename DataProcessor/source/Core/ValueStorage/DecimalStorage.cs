using System.Collections;
using System.Runtime.InteropServices;

namespace DataProcessor.source.Core.ValueStorage
{
    internal class DecimalStorage : AbstractValueStorage, IEnumerable<object?>
    {
        private readonly decimal[] values;
        private readonly NullBitMap nullBitMap;
        private readonly GCHandle handle;

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= values.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
        }

        internal DecimalStorage(decimal?[] decimals)
        {
            values = new decimal[decimals.Length];
            nullBitMap = new NullBitMap(decimals.Length);
            handle = GCHandle.Alloc(values, GCHandleType.Pinned);
            for (int i = 0; i < decimals.Length; i++)
            {
                if (decimals[i] == null)
                {
                    nullBitMap.SetNull(i, true);
                    values[i] = default;
                }
                else
                {
                    values[i] = Convert.ToDecimal(decimals[i]);
                    nullBitMap.SetNull(i, false);
                }
            }
        }

        internal DecimalStorage(decimal[] decimals, bool copy = true)
        {
            if (copy)
            {
                values = new decimal[decimals.Length];
                Array.Copy(decimals, values, decimals.Length);
            }
            else
            {
                values = decimals;
            }
            nullBitMap = new NullBitMap(decimals.Length);
            for (int i = 0; i < decimals.Length; i++)
            {
                nullBitMap.SetNull(i, false);
            }
            handle = GCHandle.Alloc(values, GCHandleType.Pinned);
        }

        internal override int Count => values.Length;

        internal override StorageKind storageKind => StorageKind.Decimal;

        internal override IEnumerable<int> NullIndices
        {
            get
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (nullBitMap.IsNull(i))
                    {
                        yield return i;
                    }
                }
            }
        }

        internal override Type ElementType => typeof(decimal);

        internal decimal[] NonNullValues
        {
            get
            {
                decimal[] result = new decimal[values.Length - nullBitMap.CountNulls()];
                int current_idx = 0;
                for (int i = 0; i < Count; i++)
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

        internal ReadOnlySpan<decimal> ValuesSpan => values;

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
                    values[index] = Convert.ToDecimal(convertible);
                    nullBitMap.SetNull(index, false);
                    return;
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Cannot convert value to decimal: {e.Message}", e);
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
