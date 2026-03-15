using System.Text;
namespace DataProcessor.source.API.DataFrame
{
    /// <summary>
    /// Represents a schema definition consisting of column names and their associated data types.
    /// </summary>
    /// <remarks>The Schema class provides a way to describe the structure of tabular data by associating each
    /// column name with its corresponding .NET type. This class is intended for internal use and is not
    /// thread-safe.</remarks>
    internal sealed class Schema
    {
        private IReadOnlyList<(string Name, Type dataType)> schema; // string is for column name and type is for data type of column

        /// <summary>
        /// Initializes a new instance of the Schema class with the specified column definitions.
        /// </summary>
        /// <param name="schema">A sequence of tuples, each containing the column name and its associated type. Must contain at least one
        /// column.</param>
        /// <exception cref="ArgumentException">Thrown if the schema sequence contains no columns.</exception>
        internal Schema(IEnumerable<(string, Type)> schema)
        {
            // validate input
            if (schema.Count() == 0)
            {
                throw new ArgumentException("Schema must contain at least 1 column");
            }
            this.schema = schema.ToList().AsReadOnly();
        }

        /// <summary>
        /// Returns a read-only list describing the schema, where each element contains the name and type of a field.
        /// </summary>
        /// <remarks>The returned list provides an overview of the fields available in the schema. The
        /// list is immutable; modifications to the schema require using appropriate methods on the containing
        /// class.</remarks>
        /// <returns>A read-only list of tuples, each containing the field name as a string and the field type as a <see
        /// cref="Type"/> object. The list reflects the current schema configuration.</returns>
        internal IReadOnlyList<ValueTuple<string, Type>> ShowSchema()
        {
            return schema;
        }

        /// <summary>
        /// return the string to describe the schema
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (schema == null || schema.Count == 0)
                return "Schema: <empty>";

            var sb = new StringBuilder();
            sb.AppendLine("Schema:");
            sb.AppendLine("--------------------------------");
            sb.AppendLine(string.Format("{0,-20} | {1}", "Column", "Type"));
            sb.AppendLine("--------------------------------");

            foreach (var (name, type) in schema)
            {
                sb.AppendLine(string.Format("{0,-20} | {1}", name, type.Name));
            }

            sb.AppendLine("--------------------------------");
            return sb.ToString();
        }

        /// <summary>
        /// Return number of columns
        /// </summary>
        internal int ColumnCount => schema.Count();

        /// <summary>
        /// Retrieves the data type of the column with the specified name.
        /// </summary>
        /// <remarks>Column names are case-sensitive. Ensure that the provided name matches the column
        /// definition exactly.</remarks>
        /// <param name="name">The name of the column for which to obtain the data type. Cannot be null or empty.</param>
        /// <returns>The <see cref="Type"/> representing the data type of the specified column.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a column with the specified <paramref name="name"/> does not exist in the schema.</exception>
        internal Type getColumnType(string name)
        {
            var columnData = schema.FirstOrDefault(c => c.Name == name);
            if (columnData == default)
            {
                throw new KeyNotFoundException($"Column '{name}' not found");
            }
            return columnData.dataType;
        }

        /// <summary>
        /// Determines whether the schema contains a column with the specified name.
        /// </summary>
        /// <param name="columnName">The name of the column to search for. Cannot be null or empty.</param>
        /// <returns>true if a column with the specified name exists in the schema; otherwise, false.</returns>
        internal bool HasColumn(string columnName)
        {
            return schema.Any(c => c.Name == columnName);
        }
        /// <summary>
        /// Retrieves the name of the column at the specified index within the schema.
        /// </summary>
        /// <param name="index">The zero-based index of the column whose name is to be retrieved. Must be greater than or equal to 0 and
        /// less than the total number of columns.</param>
        /// <returns>The name of the column at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal to the number of columns in the
        /// schema.</exception>
        internal string GetColumnName(int index)
        {
            if (index < 0 || index >= schema.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return schema[index].Name;
        }

        /// <summary>
        /// Gets a read-only list of the data types for each column in the schema.
        /// </summary>
        internal IReadOnlyList<Type> ColumnDataTypes
        {
            get
            {
                return schema.Select(c => c.dataType).ToList().AsReadOnly();
            }
        }
    }
}

