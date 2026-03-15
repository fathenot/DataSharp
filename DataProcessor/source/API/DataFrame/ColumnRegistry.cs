using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessor.source.API.DataFrame
{
    /// <summary>
    /// Provides an internal registry for managing and resolving column names and their corresponding indices within a
    /// collection.
    /// </summary>
    /// <remarks>This class is intended for internal use to efficiently map column names to their positions,
    /// including support for duplicate column names. It enables fast lookups and retrieval of column metadata by name
    /// or index.</remarks>
    internal sealed class ColumnRegistry
    {
        private readonly IReadOnlyList<string> _columns;
        private readonly Dictionary<string, List<int>> _nameToIndices; // improve efficient mapping aka columnname -> position;

        /// <summary>
        /// Initializes a new instance of the ColumnRegistry class with the specified collection of column names.
        /// </summary>
        /// <remarks>Duplicate column names are supported; each occurrence is tracked by its index in the
        /// list. The registry allows efficient lookup of column indices by name.</remarks>
        /// <param name="columns">A read-only list containing the names of the columns to be registered. Each name may appear multiple times
        /// to represent duplicate columns.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="columns"/> is <see langword="null"/>.</exception>
        internal ColumnRegistry(IReadOnlyList<string> columns)
        {
            _columns = columns ?? throw new ArgumentNullException(nameof(columns));
            _nameToIndices = new Dictionary<string, List<int>>();
            for(int i = 0; i< _columns.Count; i++)
            {
                var name = _columns[i];
                if (!_nameToIndices.TryGetValue(name, out var indices))
                {
                    indices = new List<int>();
                    _nameToIndices[name] = indices;
                }
                indices.Add(i);
            }
        }

        /// <summary>
        /// Retrieves the list of indices associated with the specified column name.
        /// </summary>
        /// <param name="columnName">The name of the column for which to retrieve indices. Cannot be null.</param>
        /// <returns>A read-only list of integers containing the indices for the specified column name. Returns an empty list if
        /// the column name is not found.</returns>
        internal IReadOnlyList<int> Resolve(string columnName)
        {
            return _nameToIndices.TryGetValue(columnName, out var indices)
            ? indices
            : Array.Empty<int>();  // Not found
        }

        /// <summary>
        /// Retrieves the name of the column at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the column whose name is to be retrieved. Must be within the valid range of column
        /// indices.</param>
        /// <returns>The name of the column at the specified index, or null if the column does not have a name.</returns>
        internal string? GetColumnName(int index)
        {
            return _columns[index];
        }
        /// <summary>
        /// Returns the number of columns currently defined in the collection.
        /// </summary>
        /// <returns>The total count of columns. Returns 0 if no columns are present.</returns>
        internal int GetNumColumns() => _columns.Count;

        internal bool Contains(string columnName)
        {
            return _nameToIndices.ContainsKey(columnName);
        }

        // Check for duplicates:
        internal bool HasDuplicates()
        {
            return _columns.GroupBy(c => c).Any(g => g.Count() > 1);
        }

        
        /// <summary>
        /// Returns the index of the first occurrence of the specified column name.
        /// </summary>
        /// <param name="columnName">The name of the column to locate. Cannot be null or empty.</param>
        /// <returns>The zero-based index of the first occurrence of the specified column.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified column name does not exist.</exception>

        internal int GetFirstOccurence(string columnName)
        {
            var indices = Resolve(columnName);
            if (indices.Count == 0)
                throw new ArgumentException($"Column '{columnName}' not found");
            return indices[0];
        }

        /// <summary>
        /// Gets a collection of column names that are unique within the current context.
        /// </summary>
        internal IEnumerable<string> UniqueColumns => _nameToIndices.Keys;

        /// <summary>
        /// Gets the collection of column names associated with the current instance.
        /// </summary>
        internal IReadOnlyList<string> Columns => _columns;
    }
}

