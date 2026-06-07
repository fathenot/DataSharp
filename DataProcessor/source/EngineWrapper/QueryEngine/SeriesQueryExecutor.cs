using DataProcessor.source.API.NonGenericsSeries;

namespace DataProcessor.source.EngineWrapper.QueryEngine
{
    internal static class SeriesQueryExecutor
    {
        /// <summary>
        /// Executes a sequence of query operations on the specified series according to the provided query plan.
        /// </summary>
        /// <remarks>The method processes the query plan in order, applying each operation to the series.
        /// The returned series may reference the same data as the input or a transformed subset, depending on the
        /// operations specified in the plan. This method does not modify the original series.</remarks>
        /// <param name="series">The input series to be queried. Cannot be null.</param>
        /// <param name="plan">The query plan that defines the sequence of operations to apply to the series. Cannot be null.</param>
        /// <returns>A new Series instance containing the result of applying the query plan to the input series.</returns>
        public static Series Execute(
            Series series,
            SeriesQueryWorks plan)
        {
            List<int>? mask = null;
            var currentSeries = series;

            foreach (var node in plan.Nodes)
            {
                switch (node)
                {
                    case SeriesWhereNode where:
                        mask = WhereExecutor.Execute(currentSeries.Storage, where, mask);
                        break;
                    case SeriesSelectNode select:
                        if (mask != null)
                        {
                            currentSeries = currentSeries.Take(mask);
                            mask = null;
                        }

                        var values = SelectExecutor.Execute(currentSeries.Storage, select);
                        currentSeries = new Series(values, currentSeries.Index);
                        break;
                }
            }

            if (mask != null)
                return currentSeries.Take(mask);

            return currentSeries;
        }
    }
}
