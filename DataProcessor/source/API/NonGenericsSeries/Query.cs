using DataProcessor.source.EngineWrapper.QueryEngine;

namespace DataProcessor.source.API.NonGenericsSeries
{
    public partial class Series
    {
        /// <summary>
        /// Executes a query against the current series using a builder function to define query parameters and
        /// operations.
        /// </summary>
        /// <remarks>The builder function is invoked with a new <see cref="SeriesQueryBuilder"/> instance,
        /// allowing you to fluently specify filters, projections, or other query operations. The query is executed
        /// immediately after the builder function completes.</remarks>
        /// <param name="build">A function that configures a <see cref="SeriesQueryBuilder"/> by specifying query criteria and operations.
        /// The function should return the configured builder instance.</param>
        /// <returns>A <see cref="Series"/> containing the results of the executed query. The returned series reflects the
        /// criteria and operations defined in the builder function.</returns>
        public Series Query(Func<SeriesQueryBuilder, SeriesQueryBuilder> build)
        {
            var plan = new SeriesQueryWorks();

            var builder = new SeriesQueryBuilder(plan);

            build(builder);

            return SeriesQueryExecutor.Execute(this, plan);
        }
    }
}

