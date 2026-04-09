using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProcessor.source.API.NonGenericsSeries;
namespace test.TestEngine
{
    public class TestQuery
    {
        [Fact]
        public void Query_Where_IntValues_ReturnsExpected()
        {
            var series = new Series(new object?[] { 1, 2, 3, 4, 5 });

            var result = series.Query(q => q.Where<int>(v => v % 2 == 1));
            Assert.Equal(typeof(int), series.dataType);
            Assert.Equal(new object?[] { 1, 3, 5 }, result.Values);
            Assert.Equal(new object[] { 0, 2, 4 }, result.Index);
        }

        [Fact]
        public void Query_Where_WithNulls_SkipsNulls()
        {
            var series = new Series(new object?[] { 1, null, 3, null, 5 });

            var result = series.Query(q => q.Where<int>(v => v > 1).Where<int>(v => v < 5));

            Assert.Equal(new object?[] { 3 }, result.Values);
            Assert.Equal(new object[] { 2 }, result.Index);
        }

        [Fact]
        public void Query_Where_StringValues_ReturnsExpected()
        {
            var series = new Series(new object?[] { "a", "bb", "ccc", "dd" });

            var result = series.Query(q => q.Where<string?>(v => v != null && v.Length >= 2));

            Assert.Equal(new object?[] { "bb", "ccc", "dd" }, result.Values);
            Assert.Equal(new object[] { 1, 2, 3 }, result.Index);
        }

        [Fact] // stress test
        public void Query_Where_MillionsData()
        {
            int count = 2_000_000;
            var random = new Random();

            List<int> numbers = new List<int>(count);

            for (int i = 0; i < count; i++)
            {
                numbers.Add(random.Next()); // từ 0 → int.MaxValue
            }
            var series = new Series(numbers);
            var result = series.Query(q => q.Where<int>(v => v > 1).Where<int>(v => v < 5));
            // Ensure no exception
            Assert.NotNull(result);
        }

        [Fact]
        public void applySelect()
        {
            int count = 200;
            var random = new Random();

            List<int> numbers = new List<int>(count);

            for (int i = 0; i < count; i++)
            {
                numbers.Add(random.Next()); // từ 0 → int.MaxValue
            }
            var series = new Series(numbers);
            var index = series.Index;
            var c = series.Count;
            Assert.Equal(count, index.Count);
            var result = series.Query(q => q.Select(v => (int)v + 1));
            for (int i = 0;i < count; i++)
            {
                Assert.Equal((int)result.Values[i] - 1, numbers[i]);
            }
        }
    }
}
