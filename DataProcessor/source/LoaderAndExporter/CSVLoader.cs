using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using System.Globalization;
using System.Text;
using DataProcessor.source.API.NonGenericsSeries;
using DataProcessor.source.API.DataFrame;

namespace DataProcessor.source.LoaderAndExporter
{
    public static class CSVLoader
    {
        /// <summary>
        /// Generates a column name string based on the specified index.
        /// </summary>
        /// <param name="index">The zero-based index used to create the column name. Must be greater than or equal to zero.</param>
        /// <returns>A string in the format "Column_{index}" representing the generated column name.</returns>
        private static string GenerateColumnName(int index)
        {
            return "Column_" + index.ToString();
        }
        
        /// <summary>
        /// Detects the delimiter character used in a CSV file by analyzing a sample of its lines.
        /// </summary>
        /// <remarks>The method examines up to five non-empty lines from the file and tests common
        /// delimiters (such as ',', ';', '\t', and '|'). The detected delimiter is the one that appears consistently
        /// across all sampled lines. On Windows, ';' is also considered as a candidate delimiter.</remarks>
        /// <param name="path">The file path to the CSV file to analyze. Must refer to an existing file.</param>
        /// <param name="encoding">The character encoding to use when reading the CSV file.</param>
        /// <returns>A character representing the detected delimiter. Returns ',' if no common delimiter is found.</returns>
        /// <exception cref="LoadCsvException">Thrown if the CSV file is empty or cannot be read.</exception>
        private static char DetectDelimiter(string path, Encoding encoding)
        {
            using var reader = new StreamReader(path, encoding);

            // Read sample lines
            var sampleLines = new List<string>();
            for (int i = 0; i < 5 && !reader.EndOfStream; i++)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                    sampleLines.Add(line);
            }

            if (sampleLines.Count == 0)
                throw new LoadCsvException("CSV file is empty");

            // Try common delimiters
            char[] candidates = OperatingSystem.IsWindows()
                ? new[] { ',', ';', '\t', '|' }
                : new[] { ',', '\t', '|' };
            foreach (var delim in candidates)
            {
                var counts = sampleLines
                    .Select(line => CountDelimiterOutsideQuotes(line, delim))
                    .ToArray();

                // Consistent count across all lines
                if (counts.Length > 0 &&
                    counts.Distinct().Count() == 1 &&
                    counts[0] > 0)
                {
                    return delim;
                }
            }

            // Fallback
            return ',';
        }

        /// <summary>
        /// Counts the number of occurrences of the specified delimiter character in the input string that are not
        /// enclosed within double quotes.
        /// </summary>
        /// <remarks>This method treats pairs of double quotes (") as quote boundaries. Delimiters inside
        /// quoted sections are not counted. This is useful for parsing delimited text formats, such as CSV, where
        /// delimiters inside quotes should be ignored.</remarks>
        /// <param name="line">The input string to search for delimiter characters. Delimiters within quoted sections are ignored.</param>
        /// <param name="delimiter">The character to count in the input string, excluding those found inside double-quoted sections.</param>
        /// <returns>The number of delimiter characters found outside of quoted sections in the input string.</returns>
        private static int CountDelimiterOutsideQuotes(string line, char delimiter)
        {
            int count = 0;
            bool inQuotes = false;

            foreach (char c in line)
            {
                if (c == '"')
                    inQuotes = !inQuotes; // reverse the state of in quote
                else if (c == delimiter && !inQuotes)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Aligns a row of column values to the specified number of columns, optionally enforcing strict validation of
        /// column count.
        /// </summary>
        /// <remarks>When strict validation is disabled, the method ensures the returned array always has
        /// the specified number of columns by truncating or padding as needed. This can be useful when processing
        /// tabular data with inconsistent row lengths.</remarks>
        /// <param name="row">The array of column values to align. Each element represents a column in the row.</param>
        /// <param name="expectedNumColumns">The expected number of columns for the aligned row. Must be greater than zero.</param>
        /// <param name="rowNumber">The zero-based index of the row being aligned. Used for error reporting in exception messages.</param>
        /// <param name="strict">If <see langword="true"/>, the method throws an exception when the row has too many or too few columns. If
        /// <see langword="false"/>, extra columns are truncated and missing columns are filled with empty strings.</param>
        /// <returns>An array of strings containing the aligned column values. The array will have exactly the specified number
        /// of columns. Extra columns are truncated and missing columns are filled with empty strings when strict
        /// validation is disabled.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="strict"/> is <see langword="true"/> and the row does not have exactly the expected
        /// number of columns.</exception>
        private static string[] AlignRow(string[] row, int expectedNumColumns, int rowNumber, bool strict = true)
        {
            // no operation needed if perfect
            if (row.Length == expectedNumColumns)
            {
                return row.ToArray();
            }

            var aligned = new string[expectedNumColumns];

            if (row.Length > expectedNumColumns)
            {
                if (strict)
                {
                    throw new InvalidOperationException($"Row {rowNumber}: Too many columns  (expected {expectedNumColumns}, got {row.Length})");
                }
                // Truncate extra columns
                Array.Copy(row, aligned, expectedNumColumns);
            }
            else
            {
                if (strict)
                {
                    throw new InvalidOperationException(
                        $"Row {rowNumber}: Not enough columns " +
                        $"(expected {expectedNumColumns}, got {row.Length})");
                }

                // Copy available columns
                Array.Copy(row, aligned, row.Length);

                // Fill missing columns with empty string
                for (int i = row.Length; i < expectedNumColumns; i++)
                    aligned[i] = string.Empty;
            }
            return aligned;
        }

        /// <summary>
        /// Loads a CSV file into a DataFrame, using the specified delimiter, header option, and encoding.
        /// </summary>
        /// <remarks>All columns are loaded as string data. Empty lines in the CSV file are skipped. If
        /// <paramref name="hasHeader"/> is <see langword="false"/>, column names are generated in the format "Column0",
        /// "Column1", etc. The method does not fail on missing or malformed fields; such data is collected
        /// as-is.</remarks>
        /// <param name="path">The file path to the CSV file to load. Must refer to a readable file.</param>
        /// <param name="hasHeader">A value indicating whether the first row of the CSV file contains column headers. If <see langword="true"/>,
        /// the first row is used as column names; otherwise, column names are auto-generated.</param>
        /// <param name="delim">The delimiter character used to separate fields in the CSV file. If <see langword="null"/>, the delimiter is
        /// auto-detected.</param>
        /// <param name="encoding">The text encoding to use when reading the CSV file. If <see langword="null"/>, UTF-8 encoding is used.</param>
        /// <returns>A DataFrame containing the data from the CSV file, with columns represented as string series. The DataFrame
        /// will have column names from the header row if <paramref name="hasHeader"/> is <see langword="true"/>;
        /// otherwise, column names are auto-generated.</returns>
        /// <exception cref="LoadCsvException">Thrown if the CSV file cannot be read or if the header row is missing when <paramref name="hasHeader"/> is
        /// <see langword="true"/>.</exception>
        public static DataFrame LoadFromCSV(string path, bool hasHeader, char? delim = null, Encoding? encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;


            // Auto-detect delimiter if not specified
            char effectiveDelimiter = delim
                ?? DetectDelimiter(path, encoding);

            // set configuration for csv reader
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = effectiveDelimiter.ToString(),
                HasHeaderRecord = true,

                // Your design: don't fail on bad data, collect it
                MissingFieldFound = null,
                BadDataFound = null,

                // Trim handling
                TrimOptions = TrimOptions.None,  // Keep raw strings!

                // Quote handling
                Mode = CsvMode.RFC4180,  // Standard CSV

                // Buffer size for performance
                BufferSize = 2048,
            };

            // read the csv file
            using var reader = new StreamReader(path, encoding);
            using var csv = new CsvReader(reader, config);

            // Step 1: Read and validate header
            ColumnRegistry columnRegistry;
            if (hasHeader)
            {
                csv.Read();
                csv.ReadHeader();

                if (csv.HeaderRecord == null || csv.HeaderRecord.Length == 0)
                    throw new LoadCsvException("Cannot read header from CSV file");

                columnRegistry = new ColumnRegistry(csv.HeaderRecord);
            }
            else {
                csv.Read();
                int columnCount = csv.Parser.Count;
                var generatedHeaders = Enumerable.Range(0, columnCount)
                    .Select(i => GenerateColumnName(i))
                    .ToArray();
                columnRegistry = new ColumnRegistry(generatedHeaders);

                // Reset to read first row as data
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.DiscardBufferedData();
                csv.Read();
            }

            // Step 2: Initialize buffer to store
            var buffers = new List<List<string>>(columnRegistry.GetNumColumns());
            for (int i = 0; i < columnRegistry.GetNumColumns(); i++)
                buffers.Add(new List<string>());

            // Step 3: read rows
            int rowNumber = hasHeader ? 1 : 0;
            while (csv.Read())
            {
                rowNumber++;

                var record = csv.Parser.Record;

                if (record == null || record.Length == 0)
                    continue;  // Skip empty lines

                // Align row to expected columns
                var aligned = AlignRow(
                    record,
                    columnRegistry.GetNumColumns(),
                    rowNumber
                    );

                for (int i = 0; i < columnRegistry.GetNumColumns(); i++)
                {
                    buffers[i].Add(aligned[i]);
                }
            }

            // Step 4: Create Series<string> for each column
            var series = new List<Series>();

            for (int i = 0; i < columnRegistry.GetNumColumns(); i++)
            {
                var columnName = columnRegistry.GetColumnName(i);
                var columnData = buffers[i];

                // All Series are string type - your design!
                series.Add(new Series(
                    columnData,
                    dtype: typeof(string),
                    name: columnName));
            }

            return new DataFrame (
                columnRegistry.Columns.ToList(),
                series);
        }
    }
}

