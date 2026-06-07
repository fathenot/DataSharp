using System;

namespace DataProcessor.source.EngineWrapper.QueryEngine
{
    /// <summary>
    /// Builds a series query plan by collecting filter and projection operations.
    /// </summary>
    public class SeriesQueryBuilder
    {
        private readonly SeriesQueryWorks _plan;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesQueryBuilder"/> class for the specified query plan.
        /// </summary>
        /// <param name="plan">The query plan that receives operations added by the builder.</param>
        internal SeriesQueryBuilder(SeriesQueryWorks plan)
        {
            _plan = plan;
        }

        /// <summary>
        /// Adds a filter condition to the query that selects elements matching the specified predicate.
        /// </summary>
        /// <remarks>Multiple calls to <c>Where</c> can be chained to combine multiple filter conditions.
        /// The predicate is not executed immediately; it is stored as part of the query definition and evaluated when
        /// the query is executed.</remarks>
        /// <typeparam name="T">The type of elements to which the predicate is applied.</typeparam>
        /// <param name="predicate">A function that defines the condition each element must satisfy to be included in the query results.</param>
        /// <returns>The current instance of <see cref="SeriesQueryBuilder"/>, enabling method chaining.</returns>
        public SeriesQueryBuilder Where<T>(Func<T, bool> predicate)
        {
            _plan.Add(
                new SeriesWhereNode(predicate, typeof(T))
            );

            return this;
        }

        /// <summary>
        /// Projects each element of the series into a new form by applying the specified selector function.
        /// </summary>
        /// <remarks>Use this method to shape or transform the data in the series. The selector function
        /// is applied to each element in the sequence. This method supports method chaining for building complex
        /// queries.</remarks>
        /// <typeparam name="T">The type of the elements in the series.</typeparam>
        /// <param name="selector">A function to transform each element of type T. The function receives an element and returns the projected
        /// element.</param>
        /// <returns>The current instance of the SeriesQueryBuilder with the select operation added.</returns>
        public SeriesQueryBuilder Select<T>(Func<T, T> selector)
        {
            _plan.Add(
                new SeriesSelectNode(selector, typeof(T), typeof(T))
            );

            return this;
        }

        /// <summary>
        /// Specifies a projection function to transform each element in the series query.
        /// </summary>
        /// <remarks>Use this method to shape the results of the series query by selecting specific fields
        /// or computing new values from each element. The selector function is applied to each element when the query
        /// is executed.</remarks>
        /// <param name="selector">A function that defines the transformation to apply to each element. The function receives an element as
        /// input and returns the projected value.</param>
        /// <returns>The current <see cref="SeriesQueryBuilder"/> instance with the projection applied. This enables method
        /// chaining.</returns>
        public SeriesQueryBuilder Select(Func<dynamic, dynamic> selector)
        {
            _plan.Add(
                new SeriesSelectNode(selector, typeof(object), typeof(object))
            );

            return this;
        }

        /// <summary>
        /// Projects each element of the input series into a new form using the specified selector function.
        /// </summary>
        /// <remarks>Use this method to shape or transform the data in the series before applying further
        /// query operations. This method can be called multiple times to apply successive transformations.</remarks>
        /// <typeparam name="TIn">The type of the elements in the input series.</typeparam>
        /// <typeparam name="TOut">The type of the elements returned by the selector function.</typeparam>
        /// <param name="selector">A function to transform each element of the input series. Cannot be null.</param>
        /// <returns>The current instance of <see cref="SeriesQueryBuilder"/> to allow method chaining.</returns>
        public SeriesQueryBuilder Select<TIn, TOut>(Func<TIn, TOut> selector)
        {
            _plan.Add(
                new SeriesSelectNode(selector, typeof(TIn), typeof(TOut))
            );

            return this;
        }
    }
}
