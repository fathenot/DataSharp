using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.source.EngineWrapper
{
    internal class EngineException: Exception
    {
        string message;
        public EngineException(string message) { this.message = message; }
        public new string Message => message;
    }
}
