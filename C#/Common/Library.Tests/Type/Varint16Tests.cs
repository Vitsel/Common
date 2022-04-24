using Common.Library.Type;

using Xunit;

using System.IO;
using System.Linq;

namespace Common.Library.Tests.Type
{
    public class VarInt16Tests
    {
        [Theory]
        [InlineData(0, new byte[] { 0 })]
        [InlineData(1, new byte[] { 1 })]
        [InlineData(300, new byte[] { 0xAC, 0x02 })]
        public void GetBytesTest(short value, byte[] expected)
        {
            var varint = new Varint16(value);

            var bytes = varint.GetBytes();

            Assert.True(Enumerable.SequenceEqual(expected, bytes));
        }

        [Theory]
        [InlineData(53, "5")]
        public void GetStringTest(short value, string expected)
        {
            var varint = new Varint16(value);

            var str = varint.GetString();

            Assert.Equal(expected, str);
        }

        [Theory]
        [InlineData(new byte[] {0xAC, 0x02}, 300)]
        [InlineData(new byte[] {53}, 53)]
        public void GetTest(byte[] target, short expected)
        {
            using (var stream = new MemoryStream(target))
            {
                var varint = Varint16.Get(stream, 0);

                Assert.Equal(expected, varint.Value);
            }
        }
    }
}