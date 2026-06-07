using CsvHelper;
using CsvHelper.Configuration;
using DataProcessor.source.API.NonGenericsSeries;
using System.Globalization;
using System.Runtime.Versioning;
using DataProcessor.source.API.DataFrame;
namespace DataProcessor.source.LoaderAndExporter
{
    internal static class DataLoader
    {
        /// <summary>
        /// Loads a CSV file into a <see cref="DataFrame"/>.
        /// </summary>
        /// <param name="path">The path to the CSV file to load.</param>
        /// <param name="hasHeader">A value indicating whether the first row contains column names.</param>
        /// <param name="delim">The delimiter used to separate fields, or <see langword="null"/> to auto-detect the delimiter.</param>
        /// <returns>A <see cref="DataFrame"/> containing the data read from the CSV file.</returns>
        public static DataFrame LoadFromCSV(string path, bool hasHeader, char? delim = null)
        {
            return CSVLoader.LoadFromCSV(path, hasHeader: hasHeader, delim: delim);
        }
    }
}

