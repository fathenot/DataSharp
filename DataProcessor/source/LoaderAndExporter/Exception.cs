using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.source.LoaderAndExporter
{
    /// <summary>
    /// Represents an exception that is thrown when an error occurs while loading a CSV file.
    /// </summary>
    /// <remarks>Use this exception to indicate failures related to CSV parsing or loading operations. This
    /// exception may be thrown when the CSV format is invalid, required data is missing, or other errors are
    /// encountered during the loading process.</remarks>
    public class LoadCsvException: Exception
    {
        private string message;
        public LoadCsvException(string message)
        {
            this.message = message;
        }
    }
}

