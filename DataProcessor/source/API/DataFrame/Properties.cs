using DataProcessor.source.Core.IndexTypes;
using DataProcessor.source.API.NonGenericsSeries;

namespace DataProcessor.source.API.DataFrame
{
    public sealed partial class DataFrame
    {
       
        /// <summary>
        /// Gets the dimensions of the data frame as a tuple containing the number of rows and columns.
        /// </summary>
        /// <remarks>The returned tuple provides the current shape of the data frame, where the first
        /// element represents the number of rows and the second element represents the number of columns. This property
        /// is useful for determining the size of the data frame before performing operations that depend on its
        /// structure.</remarks>
        public (int row, int column) Shape { get => (this.frame[0].Count, this.columnRegistry.GetNumColumns()); }

        /// <summary>
        /// Gets the number of rows in the data frame.
        /// </summary>
        public int RowCount { get => this.frame[0].Count; }

        /// <summary>
        /// Gets number of columns in the dataframe
        /// </summary>
        public int ColumnCount { get => this.columnRegistry.GetNumColumns(); }

        /// <summary>
        /// Gets the collection of column names defined in the registry.
        /// </summary>
        /// <remarks>The returned list is read-only and reflects the current set of columns available.
        /// Changes to the underlying registry will be reflected in this collection. The order of column names
        /// corresponds to their registration sequence.</remarks>
        public IReadOnlyList<string> Columns { get => this.columnRegistry.Columns; }

        /// <summary>
        /// Gets a cloned copy of the underlying data index associated with this instance.
        /// </summary>
        /// <remarks>The returned index is a deep copy, ensuring that modifications to the clone do not
        /// affect the original index. This property is intended for internal use and may not be accessible outside the
        /// assembly.</remarks>
        internal DataIndex Index { get => index.Clone(); }

        /// <summary>
        /// Gets a read-only list of the data types for each series in the frame.
        /// </summary>
        /// <remarks>The order of types in the list corresponds to the order of series in the frame. The
        /// returned list is a snapshot and will not reflect subsequent changes to the frame's series.</remarks>
        public IReadOnlyList<Type> DTypes
        {
            get
            {
                List<Type> types = new List<Type>();
                foreach (Series series in this.frame)
                {
                    types.Add(series.DataType);
                }
                return types;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains no rows or columns.
        /// </summary>
        public bool IsEmpty { get => RowCount == 0 || ColumnCount == 0; }

        /// <summary>
        /// Gets the collection of series contained in the frame.
        /// </summary>
        internal IReadOnlyList<Series> Series { get => frame; }

    }
}

