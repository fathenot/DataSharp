using System;

namespace DataProcessor.source.EngineWrapper.QueryEngine
{
    internal class SeriesSelectNode : QueryNode
    {
        public Delegate Selector { get; }

        public Type ValueType { get; }

        public Type ResultType { get; }

        public SeriesSelectNode(Delegate selector, Type valueType, Type resultType)
        {
            Selector = selector;
            ValueType = valueType;
            ResultType = resultType;
        }
    }
}
