using Common.Library.Type;

using Xunit;

using System.IO;
using System.Linq;

namespace Common.Library.Tests.Type
{
    public class VarInt64Tests
    {
        [Theory]
        [InlineData(0, new byte[] { 0 })]
        [InlineData(1, new byte[] { 1 })]
        [InlineData(300, new byte[] { 0xAC, 0x02 })]
        [InlineData(0xFA1ADFADFAA, new byte[] { 0xAA, 0xBF, 0xEB, 0xEF, 0x9A, 0xF4, 0x03 })]
        public void GetBytesTest(long value, byte[] expected)
        {
            var varint = new Varint64(value);

            var bytes = varint.GetBytes();

            Assert.True(Enumerable.SequenceEqual(expected, bytes));
        }

        [Theory]
        [InlineData(53, "5")]
        public void GetStringTest(long value, string expected)
        {
            var varint = new Varint64(value);

            var str = varint.GetString();

            Assert.Equal(expected, str);
        }

        [Theory]
        [InlineData(new byte[] { 0xAC, 0x02 }, 300)]
        [InlineData(new byte[] { 53 }, 53)]
        [InlineData(new byte[] { 0xAA, 0xBF, 0xEB, 0xEF, 0x9A, 0xF4, 0x03 }, 0xFA1ADFADFAA)]
        [InlineData(new byte[] { 0xEF, 0xB3, 0x87, 0xBE, 0x01 }, 0x17C1D9EF)]
        public void GetTest(byte[] target, long expected)
        {
            using (var stream = new MemoryStream(target))
            {
                var varint = Varint64.Get(stream, 0);

                Assert.Equal(expected, varint.Value);
            }
        }
    }
}