using DataProcessor.source.Core.ValueStorage;

namespace DataProcessor.source.EngineWrapper.QueryEngine
{
    /// <summary>
    /// Provides static methods for projecting and transforming elements from value storage or spans, supporting null
    /// value handling and type-specific selection logic.
    /// </summary>
    /// <remarks>The SelectExecutor class is intended for internal use to efficiently apply transformation
    /// functions to collections of values, such as those stored in custom value storage types or spans. It supports
    /// handling of null values via bitmaps and dispatches to optimized routines based on the underlying storage type.
    /// This class is not thread-safe and is designed for use within the data processing pipeline.</remarks>
    internal static class SelectExecutor
    {
        /// <summary>
        /// Applies a transformation function to each element in the specified span and returns a list of the results.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the input span.</typeparam>
        /// <param name="values">The span of input values to transform.</param>
        /// <param name="nullPositions">A bitmap indicating which positions in the input span are considered null. This parameter is reserved for
        /// future use and is not currently utilized.</param>
        /// <param name="selector">A function to apply to each element of the input span.</param>
        /// <returns>A list of objects containing the results of applying the selector function to each element in the input
        /// span.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the selector function is null.</exception>
        public static List<object?> Execute<T>(
            ReadOnlySpan<T> values,
            NullBitMap nullPositions,
            Func<T, T> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var result = new List<object?>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                result.Add(selector(values[i]));
            }

            return result;
        }

        /// <summary>
        /// Projects each element of the input span into a new form, inserting null values at positions indicated by the
        /// specified null bitmap.
        /// </summary>
        /// <typeparam name="TIn">The type of the elements in the input span.</typeparam>
        /// <typeparam name="TOut">The type of the elements returned by the selector function.</typeparam>
        /// <param name="values">The span of input values to process.</param>
        /// <param name="nullPositions">A bitmap indicating which positions in the input span should be treated as null. If a position is marked as
        /// null, the corresponding output element will be null.</param>
        /// <param name="selector">A function to transform each non-null input value to an output value.</param>
        /// <returns>A list of objects containing the results of applying the selector to each non-null input value, with nulls
        /// inserted at positions where the null bitmap indicates a null value.</returns>
        /// <exception cref="ArgumentNullException">Thrown if selector is null.</exception>
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

        /// <summary>
        /// Projects each value in the specified storage into a new form and returns a list of the results.
        /// </summary>
        /// <param name="storage">The value storage to enumerate and project values from.</param>
        /// <param name="selector">A function to transform each value retrieved from the storage. The function receives the value at each index
        /// and returns the projected result.</param>
        /// <returns>A list containing the results of applying the selector function to each value in the storage, in order.</returns>
        /// <exception cref="ArgumentNullException">Thrown if selector is null.</exception>
        private static List<object?> ExecuteObject(
            AbstractValueStorage storage,
            Func<object?, object?> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var result = new List<object?>(storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                result.Add(selector(storage.GetValue(i)));
            }
            return result;
        }

        /// <summary>
        /// Applies a dynamic selector to every value in the storage and returns the projected values.
        /// </summary>
        /// <param name="storage">The value storage to enumerate.</param>
        /// <param name="selector">A dynamic selector that receives each stored value and returns the projected value.</param>
        /// <returns>A list containing the projected values in storage order.</returns>
        /// <exception cref="ArgumentNullException">Thrown if selector is null.</exception>
        public static List<dynamic> Execute(
            AbstractValueStorage storage,
            Func<dynamic, dynamic> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var result = new List<dynamic>(storage.Count);
            for (int i = 0; i < storage.Count; i++)
            {
                result.Add(selector(storage.GetValue(i)!));
            }

            return result;
        }

        /// <summary>
        /// Executes the select node against the specified storage, dispatching to the typed storage path when possible.
        /// </summary>
        /// <param name="storage">The value storage that provides the input values.</param>
        /// <param name="node">The select query node containing the selector and its input/output type metadata.</param>
        /// <returns>A list containing the projected values, preserving null positions for typed storage paths.</returns>
        /// <exception cref="NotSupportedException">Thrown when the storage kind is not supported by the select executor.</exception>
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
