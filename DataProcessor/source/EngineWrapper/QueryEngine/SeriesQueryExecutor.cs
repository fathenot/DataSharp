using DataProcessor.source.API.NonGenericsSeries;

namespace DataProcessor.source.EngineWrapper.QueryEngine
{
    internal static class SeriesQueryExecutor
    {
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
