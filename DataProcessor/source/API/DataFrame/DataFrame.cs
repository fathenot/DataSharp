using System;
using System.Linq;
using System.Text;
using System.IO;
using DataProcessor.source.API.NonGenericsSeries;
using DataProcessor.source.Core.IndexTypes;
using System.Runtime.CompilerServices;
using DataProcessor.source.Core.IndexTypes;
using DataProcessor.source.API.DataFrame;

namespace DataProcessor.source.API.DataFrame
{
    /// <summary>
    /// Represents a two-dimensional, tabular data structure with labeled columns and rows, supporting data manipulation
    /// and analysis operations.
    /// </summary>
    /// <remarks>The DataFrame provides a flexible container for heterogeneous data, allowing access by column
    /// name and row index. It is commonly used for data processing, statistical analysis, and machine learning tasks.
    /// Thread safety is not guaranteed; concurrent access should be managed externally. The DataFrame is immutable with
    /// respect to its schema and index after creation, but the underlying data can be modified through exposed
    /// methods.</remarks>
    public sealed partial class DataFrame
    {
        private List<Series> frame;
        private ColumnRegistry columnRegistry;
        private DataIndex index;
        private Schema schema;
        internal DataFrame(List<string> columnName, List<Series> series, DataIndex? index = null)
        {
            this.columnRegistry = new ColumnRegistry(columnName);
            this.frame = new List<Series> (series);
            int rowCount = series.Count == 0 ? 0 : series[0].Count;

            this.index = index == null
                ? new RangeIndex(0, rowCount)
                : index.Clone();

            // create schema
            List<Type> types = new List<Type> ();
            foreach (var serie in series)
            {
                types.Add(serie.DataType);
            }
            var schema = new List<(string, Type)> ();
            foreach (var t in columnName.Zip(types))
            {
                schema.Add(t);
            }
            this.schema = new Schema(schema);
        }

    }
}


