using DeepLocal.Services;
using Xunit;

namespace DeepLocal.Tests
{
    public class GpuInfoTests
    {
        [Fact]
        public void DedicatedVram_is_null_or_positive()
        {
            var vram = GpuInfo.DedicatedVramBytes();
            Assert.True(vram is null || vram > 0);
        }
    }
}
