using DataProcessor.source.Core.ValueStorage;

namespace DataProcessor.source.EngineWrapper.QueryEngine
{
    internal static class WhereExecutor
    {
        public static List<int> Execute<T>(
            ReadOnlySpan<T> values,
            NullBitMap nullPositions,
            Func<T, bool> predicate,
            List<int>? previousResult)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            List<int> result = new(previousResult?.Count ?? values.Length / 2);

            if (previousResult == null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (nullPositions.IsNull(i)) continue;
                    if (predicate(values[i])) result.Add(i);
                }
                return result;
            }

            foreach (var i in previousResult)
            {
                if (i < 0 || i >= values.Length) continue;
                if (nullPositions.IsNull(i)) continue;
                if (predicate(values[i])) result.Add(i);
            }

            return result;
        }

        private static Func<T, bool> CastPredicate<T>(StorageKind kind, object predicate)
        {
            return predicate as Func<T, bool>
                ?? throw new InvalidOperationException(
                    $"Predicate type mismatch for StorageKind.{kind}: " +
                    $"expected Func<{typeof(T).Name}, bool>, " +
                    $"got {predicate?.GetType().Name ?? "null"}");
        }

        public static List<int> Execute(AbstractValueStorage storage, SeriesWhereNode node, List<int>? previousResult)
        {
            switch (storage.storageKind)
            {
                case StorageKind.Int32:
                    {
                        var s = (Int32ValuesStorage)storage;
                        return Execute(s.ValuesSpan, s.NullBitmap, CastPredicate<int>(storage.storageKind, node.Predicate), previousResult);
                    }
                case StorageKind.Int64:
                    {
                        var s = (Int64ValuesStorage)storage;
                        return Execute(s.ValuesSpan, s.NullBitmap, CastPredicate<long>(storage.storageKind, node.Predicate), previousResult);
                    }
                case StorageKind.Double:
                    {
                        var s = (DoubleValueStorage)storage;
                        return Execute(s.ValuesSpan, s.NullBitmap, CastPredicate<double>(storage.storageKind, node.Predicate), previousResult);
                    }
                case StorageKind.Decimal:
                    {
                        var s = (DecimalStorage)storage;
                        return Execute(s.ValuesSpan, s.NullBitmap, CastPredicate<decimal>(storage.storageKind, node.Predicate), previousResult);
                    }
                case StorageKind.Char:
                    {
                        var s = (CharStorage)storage;
                        return Execute(s.ValuesSpan, s.NullBitmap, CastPredicate<char>(storage.storageKind, node.Predicate), previousResult);
                    }
                case StorageKind.Boolean:
                    {
                        var s = (BoolStorage)storage;
                        return Execute(s.ValuesSpan, s.NullBitmap, CastPredicate<bool>(storage.storageKind, node.Predicate), previousResult);
                    }
                case StorageKind.String:
                    {
                        var s = (StringStorage)storage;
                        return Execute(s.ValuesSpan, s.NullBitmap, CastPredicate<string?>(storage.storageKind, node.Predicate), previousResult);
                    }
                case StorageKind.Object:
                    {
                        var s = (ObjectValueStorage)storage;
                        return Execute(s.ValuesSpan, s.NullBitmap, CastPredicate<object?>(storage.storageKind, node.Predicate), previousResult);
                    }
                case StorageKind.DateTime:
                    {
                        var s = (DateTimeStorage)storage;
                        var predicate = CastPredicate<DateTime>(storage.storageKind, node.Predicate);
                        var result = new List<int>(previousResult?.Count ?? s.Count / 2);

                        if (previousResult == null)
                        {
                            for (int i = 0; i < s.Count; i++)
                            {
                                if (s.NullBitmap.IsNull(i)) continue;
                                if (predicate(new DateTime(s.TicksSpan[i], s.KindsSpan[i]))) result.Add(i);
                            }
                            return result;
                        }

                        foreach (var i in previousResult)
                        {
                            if (i < 0 || i >= s.Count) continue;
                            if (s.NullBitmap.IsNull(i)) continue;
                            if (predicate(new DateTime(s.TicksSpan[i], s.KindsSpan[i]))) result.Add(i);
                        }

                        return result;
                    }
                default:
                    throw new NotSupportedException($"Unsupported storage kind: {storage.storageKind}");
            }
        }
    }
}