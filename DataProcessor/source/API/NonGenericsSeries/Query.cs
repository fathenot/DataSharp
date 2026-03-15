using DataProcessor.source.EngineWrapper.QueryEngine;

namespace DataProcessor.source.API.NonGenericsSeries
{
    public partial class Series
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="build"></param>
        /// <returns></returns>
        public Series Query(Func<SeriesQueryBuilder, SeriesQueryBuilder> build)
        {
            var plan = new SeriesQueryWorks();

            var builder = new SeriesQueryBuilder(plan);

            build(builder);

            return SeriesQueryExecutor.Execute(this, plan);
        }
    }
}

