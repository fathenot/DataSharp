using System.Collections;

namespace DataProcessor.source.Core.ValueStorage
{
    internal class BoolStorage : AbstractValueStorage, IEnumerable<object?>
    {
        private readonly BitArray _values;
        private readonly NullBitMap _nullBitMap;

        internal BoolStorage(bool?[] bools)
        {
            var length = bools.Length;
            _values = new BitArray(length);
            _nullBitMap = new NullBitMap(length);

            for (var i = 0; i < length; i++)
            {
                if (bools[i].HasValue)
                {
                    _values[i] = bools[i].Value;
                }
                else
                {
                    _values[i] = false;
                    _nullBitMap.SetNull(i, true);
                }
            }
        }

        internal BoolStorage(bool[] bools)
        {
            _values = new BitArray(bools);
            _nullBitMap = new NullBitMap(bools.Length);
            for (int i = 0; i < bools.Length; i++)
            {
                _nullBitMap.SetNull(i, false);
            }
        }

        internal override int Count => _values.Count;

        internal override Type ElementType => typeof(bool);

        internal override IEnumerable<int> NullIndices
        {
            get
            {
                for (int i = 0; i < _values.Length; i++)
                {
                    if (_nullBitMap.IsNull(i))
                        yield return i;
                }
            }
        }

        internal bool[] NonNullValues
        {
            get
            {
                var result = new bool[_values.Length - _nullBitMap.CountNulls()];
                var currentIndex = 0;

                for (var i = 0; i < Count; i++)
                {
                    if (!_nullBitMap.IsNull(i))
                    {
                        result[currentIndex] = _values[i];
                        currentIndex++;
                    }
                }

                return result;
            }
        }

        internal ReadOnlySpan<bool> ValuesSpan
        {
            get
            {
                var result = new bool[_values.Length];
                _values.CopyTo(result, 0);
                return result;
            }
        }

        internal NullBitMap NullBitmap => _nullBitMap;

        internal override StorageKind storageKind => StorageKind.Boolean;

        internal override nint GetNativeBufferPointer()
        {
            throw new NotImplementedException();
        }

        internal override object? GetValue(int index)
        {
            return _values[index];
        }

        internal override void SetValue(int index, object? value)
        {
            if (value == null)
            {
                _nullBitMap.SetNull(index, true);
                _values[index] = false;
                return;
            }

            if (value is bool b)
            {
                _values[index] = b;
                _nullBitMap.SetNull(index, false);
                return;
            }

            throw new ArgumentException("Value must be a boolean or null.", nameof(value));
        }

        public override IEnumerator<object?> GetEnumerator()
        {
            for (var i = 0; i < _values.Length; i++)
            {
                yield return _nullBitMap.IsNull(i) ? null : _values[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }
    }
}
