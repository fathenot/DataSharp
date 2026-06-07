using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProcessor.source.EngineWrapper.ComputationEngine;
namespace DataProcessor.source.EngineWrapper.ComputationEngine
{
    internal class CalculateMean
    {
        /// <summary>
        /// Computes the arithmetic mean of a sequence of double-precision floating-point values.
        /// </summary>
        /// <param name="data">The values to average.</param>
        /// <param name="nullIndicies">The positions that should be excluded from the divisor.</param>
        /// <returns>The arithmetic mean of the non-null values in <paramref name="data"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="data"/> is null or empty, or when there are no valid values to average.</exception>
        internal static double Mean(double[] data, int[]? nullIndicies = null)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Data cannot be null or empty.");
            double sum = CalculateSum.ComputeSum(data);
            if (nullIndicies != null)
            {
                int count = data.Length - nullIndicies.Length;
                if (count <= 0)
                    throw new ArgumentException("No valid data points to calculate mean.");
                return sum / count;
            }

            return sum / data.Length;
        }

        internal static long Mean(long[] data, int[]? nullIndicies = null)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Data cannot be null or empty.");
            long sum = CalculateSum.ComputeSum(data);
            if (nullIndicies != null)
            {
                int count = data.Length - nullIndicies.Length;
                if (count <= 0)
                    throw new ArgumentException("No valid data points to calculate mean.");
                return sum / count;
            }
            return sum / data.Length;
        }

    }
}
