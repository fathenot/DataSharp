using System;

namespace DataProcessor.source.EngineWrapper.QueryEngine
{
    public class SeriesQueryBuilder
    {
        private readonly SeriesQueryWorks _plan;

        internal SeriesQueryBuilder(SeriesQueryWorks plan)
        {
            _plan = plan;
        }

        public SeriesQueryBuilder Where<T>(Func<T, bool> predicate)
        {
            _plan.Add(
                new SeriesWhereNode(predicate, typeof(T))
            );

            return this;
        }

        public SeriesQueryBuilder Select<T>(Func<T, T> selector)
        {
            _plan.Add(
                new SeriesSelectNode(selector, typeof(T), typeof(T))
            );

            return this;
        }

        public SeriesQueryBuilder Select(Func<object?, object?> selector)
        {
            _plan.Add(
                new SeriesSelectNode(selector, typeof(object), typeof(object))
            );

            return this;
        }

        public SeriesQueryBuilder Select<TIn, TOut>(Func<TIn, TOut> selector)
        {
            _plan.Add(
                new SeriesSelectNode(selector, typeof(TIn), typeof(TOut))
            );

            return this;
        }
    }
}
