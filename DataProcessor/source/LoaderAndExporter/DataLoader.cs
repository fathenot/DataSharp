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
        /// CSV loader semantics not finalized yet (header / missing / extra columns)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="delim"></param>
        /// <returns></returns>

        public static DataFrame LoadFromCSV(string path, bool hasHeader, char? delim = null)
        {
            return CSVLoader.LoadFromCSV(path, hasHeader: hasHeader, delim: delim);
        }
    }
}

