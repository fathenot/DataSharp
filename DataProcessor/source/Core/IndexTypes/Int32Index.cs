namespace DataProcessor.source.Core.IndexTypes
{
    public class Int32Index : DataIndex
    {
        private readonly List<int> indexList;
        private readonly Dictionary<int, List<int>> indexMap;

        private int ConvertToInt(object key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            try
            {
                return Convert.ToInt32(key);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid index: cannot convert {key} to int.", ex);
            }
        }

        public Int32Index(List<int> indexList)
            : base(indexList.Cast<object>().ToList())
        {
            this.indexList = indexList;
            indexMap = new Dictionary<int, List<int>>();

            for (int i = 0; i < indexList.Count; i++)
            {
                int key = indexList[i];
                if (!indexMap.ContainsKey(key))
                {
                    indexMap[key] = new List<int>();
                }
                indexMap[key].Add(i);
            }
        }

        public override int Count => indexList.Count;

        public override IReadOnlyList<object> IndexList => indexList.Cast<object>().ToList().AsReadOnly();

        public override object GetIndex(int idx)
        {
            return indexList[idx];
        }

        public override int FirstPositionOf(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var tmp = ConvertToInt(key);
            if (indexMap.TryGetValue(tmp, out var positions))
                return positions[0];
            return -1;
        }

        public override bool Contains(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var tmp = ConvertToInt(key);
            return indexMap.ContainsKey(tmp);
        }

        public override List<int> GetIndexPosition(object index)
        {
            var tmp = ConvertToInt(index);
            if (indexMap.ContainsKey(tmp))
            {
                return indexMap[tmp];
            }
            throw new ArgumentException($"Index {index} not found.");
        }

        public override DataIndex Slice(int start, int end, int step)
        {
            List<object> slicedIndex = new List<object>();
            if (step == 0)
            {
                throw new ArgumentException("step must not be 0");
            }
            if (step > 0)
            {
                for (int i = start; i < end; i += step)
                {
                    slicedIndex.Add(indexList[i]);
                }
            }
            else
            {
                for (int i = start; i > end; i += step)
                {
                    slicedIndex.Add(indexList[i]);
                }
            }

            return new Int32Index(slicedIndex.Cast<int>().ToList());
        }

        public override DataIndex TakeKeys(List<object> indexList)
        {
            var slicedIndex = new List<int>();
            foreach (var item in indexList)
            {
                var tmp = ConvertToInt(item);
                if (indexMap.ContainsKey(tmp))
                {
                    foreach (var position in indexMap[tmp])
                    {
                        slicedIndex.Add(tmp);
                    }
                }
                else
                {
                    throw new ArgumentException($"Item {item} is not of type int.");
                }
            }
            return new Int32Index(slicedIndex);
        }

        public override IEnumerable<object> DistinctIndices()
        {
            return indexList.Distinct().Cast<object>();
        }

        public override IEnumerator<object> GetEnumerator()
        {
            foreach (var item in indexList)
                yield return item;
        }

        public override object this[int index]
        {
            protected set
            {
                if (index < 0 || index >= indexList.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                }
                int oldValue = indexList[index];
                indexList[index] = (int)value;

                if (indexMap.ContainsKey(oldValue))
                {
                    indexMap[oldValue].Remove(index);
                    if (indexMap[oldValue].Count == 0)
                    {
                        indexMap.Remove(oldValue);
                    }
                }

                if (!indexMap.ContainsKey((int)value))
                {
                    indexMap[(int)value] = new List<int>();
                }
                indexMap[(int)value].Add(index);
            }
        }

        public override DataIndex Clone()
        {
            return new Int32Index(indexList);
        }
    }
}
