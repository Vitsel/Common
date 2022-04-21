using Common.Library.Type;

using Xunit;

using System;
using System.IO;
using System.Linq;

namespace Common.Library.Tests.Type
{
    public class VarintParserTests
    {
        [Fact]
        public void ReadBytesTest()
        {
            var input = new byte[] { 0xAA, 0xBF, 0xEB, 0xEF, 0x9A, 0xF4, 0x03, 0xFF, 0xFF };
            var expected = new byte[] { 0xAA, 0xBF, 0xEB, 0xEF, 0x9A, 0xF4, 0x03 };

            var stream = new MemoryStream();
            stream.Write(input, 0, input.Length);
            stream.Seek(0, SeekOrigin.Begin);

            var parser = new VarintParser();
            var bytes = parser.ReadBytes(stream, sizeof(long));

            Assert.True(Enumerable.SequenceEqual(expected, bytes));
        }

        [Fact]
        public void ToBytesTest()
        {
            var value = 0xFA1ADFADFAA;
            var expected = new byte[] { 0xAA, 0xBF, 0xEB, 0xEF, 0x9A, 0xF4, 0x03 };

            var parser = new VarintParser();
            var bytes = parser.ToBytes(value, sizeof(long));

            Assert.True(Enumerable.SequenceEqual(expected, bytes));
        }

        [Theory]
        [InlineData(new byte[] { 0xAC, 0x02 }, 300)]
        public void ToInt16Test(byte[] bytes, short expected)
        {
            var parser = new VarintParser();
            var value = parser.ToInt16(bytes);

            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(new byte[] { 0xAC, 0x02 }, 300)]
        public void ToInt32Test(byte[] bytes, int expected)
        {
            var parser = new VarintParser();
            var value = parser.ToInt32(bytes);

            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(new byte[] { 0xAC, 0x02 }, 300)]
        [InlineData(new byte[] { 0xAA, 0xBF, 0xEB, 0xEF, 0x9A, 0xF4, 0x03 }, 0xFA1ADFADFAA)]
        public void ToInt64Test(byte[] bytes, long expected)
        {
            var parser = new VarintParser();
            var value = parser.ToInt64(bytes);

            Assert.Equal(expected, value);
        }

        [Fact]
        public void ConvertExceptionTest()
        {
            var bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            var parser = new VarintParser();

            Assert.Throws<IndexOutOfRangeException>(() => parser.ToInt16(bytes));
            Assert.Throws<IndexOutOfRangeException>(() => parser.ToInt32(bytes));
            Assert.Throws<IndexOutOfRangeException>(() => parser.ToInt64(bytes));
        }

        [Fact]
        public void ReadBytesEofExceptionTest()
        {
            var bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

            var stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            var parser = new VarintParser();
            Assert.Throws<EndOfStreamException>(() => parser.ReadBytes(stream, sizeof(long)));
        }

        [Fact]
        public void ReadBytesInvalidInputExceptionTest()
        {
            var bytes = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            var stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            var parser = new VarintParser();
            Assert.Throws<IndexOutOfRangeException>(() => parser.ReadBytes(stream, sizeof(long)));
        }
    }
}
