using DataProcessor.source.Core.ValueStorage;

namespace DataProcessor.source.EngineWrapper.QueryEngine
{
    internal static class SelectExecutor
    {
        public static List<object?> Execute<T>(
            ReadOnlySpan<T> values,
            NullBitMap nullPositions,
            Func<T, T> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var result = new List<object?>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                if (nullPositions.IsNull(i))
                {
                    result.Add(null);
                    continue;
                }

                result.Add(selector(values[i]));
            }

            return result;
        }

        public static List<object?> Execute<TIn, TOut>(
            ReadOnlySpan<TIn> values,
            NullBitMap nullPositions,
            Func<TIn, TOut> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var result = new List<object?>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                if (nullPositions.IsNull(i))
                {
                    result.Add(null);
                    continue;
                }

                result.Add(selector(values[i]));
            }

            return result;
        }

        private static List<object?> ExecuteObject(
            AbstractValueStorage storage,
            Func<object?, object?> selector)
        {
            var result = new List<object?>(storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                var value = storage.GetValue(i);
                if (value == null)
                {
                    result.Add(null);
                }
                else
                {
                    result.Add(selector(value));
                }
            }
            return result;
        }

        public static List<object?> Execute(AbstractValueStorage storage, SeriesSelectNode node)
        {
            if (node.ValueType == typeof(object) && node.ResultType == typeof(object))
            {
                return ExecuteObject(storage, (Func<object?, object?>)node.Selector);
            }

            switch (storage.storageKind)
            {
                case StorageKind.Int32:
                {
                    var int32ValuesStorage = (Int32ValuesStorage)storage;
                    if (node.ValueType == typeof(int) && node.ResultType == typeof(int))
                    {
                        return Execute<int, int>(int32ValuesStorage.ValuesSpan, int32ValuesStorage.NullBitmap, (Func<int, int>)node.Selector);
                    }
                    return Execute<int, object?>(int32ValuesStorage.ValuesSpan, int32ValuesStorage.NullBitmap, (Func<int, object?>)node.Selector);
                }
                case StorageKind.Int64:
                {
                    var int64ValuesStorage = (Int64ValuesStorage)storage;
                    if (node.ValueType == typeof(long) && node.ResultType == typeof(long))
                    {
                        return Execute<long, long>(int64ValuesStorage.ValuesSpan, int64ValuesStorage.NullBitmap, (Func<long, long>)node.Selector);
                    }
                    return Execute<long, object?>(int64ValuesStorage.ValuesSpan, int64ValuesStorage.NullBitmap, (Func<long, object?>)node.Selector);
                }
                case StorageKind.Double:
                {
                    var doubleStorage = (DoubleValueStorage)storage;
                    if (node.ValueType == typeof(double) && node.ResultType == typeof(double))
                    {
                        return Execute<double, double>(doubleStorage.ValuesSpan, doubleStorage.NullBitmap, (Func<double, double>)node.Selector);
                    }
                    return Execute<double, object?>(doubleStorage.ValuesSpan, doubleStorage.NullBitmap, (Func<double, object?>)node.Selector);
                }
                case StorageKind.Decimal:
                {
                    var decimalStorage = (DecimalStorage)storage;
                    if (node.ValueType == typeof(decimal) && node.ResultType == typeof(decimal))
                    {
                        return Execute<decimal, decimal>(decimalStorage.ValuesSpan, decimalStorage.NullBitmap, (Func<decimal, decimal>)node.Selector);
                    }
                    return Execute<decimal, object?>(decimalStorage.ValuesSpan, decimalStorage.NullBitmap, (Func<decimal, object?>)node.Selector);
                }
                case StorageKind.Char:
                {
                    var charStorage = (CharStorage)storage;
                    if (node.ValueType == typeof(char) && node.ResultType == typeof(char))
                    {
                        return Execute<char, char>(charStorage.ValuesSpan, charStorage.NullBitmap, (Func<char, char>)node.Selector);
                    }
                    return Execute<char, object?>(charStorage.ValuesSpan, charStorage.NullBitmap, (Func<char, object?>)node.Selector);
                }
                case StorageKind.Boolean:
                {
                    var boolStorage = (BoolStorage)storage;
                    if (node.ValueType == typeof(bool) && node.ResultType == typeof(bool))
                    {
                        return Execute<bool, bool>(boolStorage.ValuesSpan, boolStorage.NullBitmap, (Func<bool, bool>)node.Selector);
                    }
                    return Execute<bool, object?>(boolStorage.ValuesSpan, boolStorage.NullBitmap, (Func<bool, object?>)node.Selector);
                }
                case StorageKind.String:
                {
                    var stringStorage = (StringStorage)storage;
                    if (node.ValueType == typeof(string) && node.ResultType == typeof(string))
                    {
                        return Execute<string?, string?>(stringStorage.ValuesSpan, stringStorage.NullBitmap, (Func<string?, string?>)node.Selector);
                    }
                    return Execute<string?, object?>(stringStorage.ValuesSpan, stringStorage.NullBitmap, (Func<string?, object?>)node.Selector);
                }
                case StorageKind.Object:
                {
                    var objectStorage = (ObjectValueStorage)storage;
                    return Execute<object?, object?>(objectStorage.ValuesSpan, objectStorage.NullBitmap, (Func<object?, object?>)node.Selector);
                }
                case StorageKind.DateTime:
                {
                    var dateStorage = (DateTimeStorage)storage;
                    if (node.ValueType == typeof(DateTime) && node.ResultType == typeof(DateTime))
                    {
                        var selector = (Func<DateTime, DateTime>)node.Selector;
                        var result = new List<object?>(dateStorage.Count);
                        for (int i = 0; i < dateStorage.Count; i++)
                        {
                            if (dateStorage.NullBitmap.IsNull(i))
                            {
                                result.Add(null);
                                continue;
                            }
                            var value = new DateTime(dateStorage.TicksSpan[i], dateStorage.KindsSpan[i]);
                            result.Add(selector(value));
                        }
                        return result;
                    }

                    var objectSelector = (Func<DateTime, object?>)node.Selector;
                    var objectResult = new List<object?>(dateStorage.Count);
                    for (int i = 0; i < dateStorage.Count; i++)
                    {
                        if (dateStorage.NullBitmap.IsNull(i))
                        {
                            objectResult.Add(null);
                            continue;
                        }
                        var value = new DateTime(dateStorage.TicksSpan[i], dateStorage.KindsSpan[i]);
                        objectResult.Add(objectSelector(value));
                    }
                    return objectResult;
                }
                default:
                    throw new NotSupportedException($"Unsupported storage kind: {storage.storageKind}");
            }
        }
    }
}
