using System;

namespace DataProcessor.source.EngineWrapper.QueryEngine.Nodes
{
    /// <summary>
    /// Represents a select operation in a series query plan.
    /// </summary>
    internal class SeriesSelectNode : QueryNode
    {
        /// <summary>
        /// Gets the delegate used to project each input value.
        /// </summary>
        public Delegate Selector { get; }

        /// <summary>
        /// Gets the expected input value type for the selector.
        /// </summary>
        public Type ValueType { get; }

        /// <summary>
        /// Gets the expected result type produced by the selector.
        /// </summary>
        public Type ResultType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesSelectNode"/> class.
        /// </summary>
        /// <param name="selector">The delegate used to project input values.</param>
        /// <param name="valueType">The selector input type.</param>
        /// <param name="resultType">The selector result type.</param>
        public SeriesSelectNode(Delegate selector, Type valueType, Type resultType)
        {
            Selector = selector;
            ValueType = valueType;
            ResultType = resultType;
        }
    }
}
