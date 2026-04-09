using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.source.EngineWrapper.QueryEngine
{
    internal class SeriesWhereNode: QueryNode
    {
        public Delegate Predicate { get; }

        public Type ValueType { get; }

        public SeriesWhereNode(Delegate predicate, Type valueType)
        {
            Predicate = predicate;
            ValueType = valueType;
        }
    }
}

