using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.source.EngineWrapper.QueryEngine
{
    /// <summary>
    /// Represents a collection of query nodes used to build a series query.
    /// </summary>
    internal class SeriesQueryWorks
    {
        private readonly List<QueryNode> _nodes = new List<QueryNode>();
        public IReadOnlyList<QueryNode> Nodes => _nodes;

        internal void Add(QueryNode node)
        {
            _nodes.Add(node);
        }
    }
}

