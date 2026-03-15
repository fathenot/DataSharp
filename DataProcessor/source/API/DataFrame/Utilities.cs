using System.Text;

namespace DataProcessor.source.API.DataFrame
{
    public partial class DataFrame
    {
        public override string ToString() => Show(5);

        /// <summary>
        /// Returns a string representation of the current schema.
        /// </summary>
        /// <returns>A string containing the schema definition. The format and content depend on the implementation of the schema
        /// object.</returns>
        public string ShowSchema()
        {
           return schema.ToString();
        }

        /// <summary>
        /// Returns a formatted string representation of the DataFrame, displaying up to the specified number of rows.
        /// </summary>
        /// <remarks>The output includes column names and row indices. If the DataFrame contains more rows
        /// than the specified limit, only the top rows are shown and a note is appended. Long cell values are truncated
        /// for readability.</remarks>
        /// <param name="n">The maximum number of rows to display. Defaults to 20. Must be non-negative.</param>
        /// <returns>A string containing the DataFrame's column headers and up to the specified number of rows. If the DataFrame
        /// is empty, returns a message indicating that it is empty.</returns>
        public string Show(int n = 20)
        {
            var sb = new StringBuilder();

            int rowCount = frame.Count == 0 ? 0 : frame[0].Count;
            int colCount = frame.Count;

            sb.AppendLine($"DataFrame [{rowCount} rows x {colCount} columns]");
            sb.AppendLine();

            if (rowCount == 0 || colCount == 0)
            {
                sb.AppendLine("<empty DataFrame>");
                return sb.ToString();
            }

            int rowsToShow = Math.Min(n, rowCount);

            const int colWidth = 15;

            // Header
            sb.Append("| Index | ");
            foreach (var col in columnRegistry.Columns)
                sb.Append($"{col,-colWidth} |");
            sb.AppendLine();

            sb.AppendLine(new string('-', 8 + (colWidth + 3) * colCount));

            // Rows
            for (int i = 0; i < rowsToShow; i++)
            {
                sb.Append($"| {index[i],5} | ");

                foreach (var series in frame)
                {
                    var value = series.GetValueIntloc(i) ;
                    var text = value?.ToString() ?? "null";

                    if (text.Length > colWidth)
                        text = text.Substring(0, colWidth - 3) + "...";

                    sb.Append($"{text,-colWidth} |");
                }

                sb.AppendLine();
            }

            if (rowCount > rowsToShow)
                sb.AppendLine($"only showing top {rowsToShow} rows");

            return sb.ToString();
        }

    }
}

