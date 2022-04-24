using Common.Library.Type;

using Xunit;

using System.IO;
using System.Linq;

namespace Common.Library.Tests.Type
{
    public class VarInt32Tests
    {
        [Theory]
        [InlineData(0, new byte[] { 0 })]
        [InlineData(1, new byte[] { 1 })]
        [InlineData(300, new byte[] { 0xAC, 0x02 })]
        [InlineData(0xFE8, new byte[] { 0xE8, 0x1F })]
        [InlineData(0x1FF8, new byte[] {0xF8, 0x3F})]
        [InlineData(0x1015, new byte[] { 0x00 })]
        public void GetBytesTest(int value, byte[] expected)
        {
            var varint = new Varint32(value);

            var bytes = varint.GetBytes();

            Assert.True(Enumerable.SequenceEqual(expected, bytes));
        }

        [Theory]
        [InlineData(53, "5")]
        public void GetStringTest(int value, string expected)
        {
            var varint = new Varint32(value);

            var str = varint.GetString();

            Assert.Equal(expected, str);
        }

        [Theory]
        [InlineData(new byte[] { 0xAC, 0x02 }, 300)]
        [InlineData(new byte[] { 53 }, 53)]
        [InlineData(new byte[] {0x90, 0x20}, 0)]
        public void GetTest(byte[] target, int expected)
        {
            using (var stream = new MemoryStream(target))
            {
                var varint = Varint32.Get(stream, 0);

                Assert.Equal(expected, varint.Value);
            }
        }
    }
}