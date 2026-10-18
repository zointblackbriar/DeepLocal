using DeepLocal.Services;
using Xunit;

namespace DeepLocal.Tests
{
    public class ModelItemTests
    {
        [Theory]
        [InlineData(0, "")]
        [InlineData(500_000_000, "500 MB")]
        [InlineData(1_500_000_000, "1.5 GB")]
        [InlineData(3_300_000_000, "3.3 GB")]
        public void FormatSize_formats_bytes(long bytes, string expected)
            => Assert.Equal(expected, ModelItem.FormatSize(bytes));

        [Fact]
        public void Detail_lists_size_family_and_params()
        {
            var item = new ModelItem("translategemma:12b")
            {
                Size = 8_100_000_000,
                Family = "gemma3",
                ParameterSize = "12.2B",
                IsInstalled = true,
            };

            Assert.Equal("8.1 GB · gemma3 · 12.2B", item.Detail);
        }

        [Fact]
        public void Detail_for_cloud_is_badge()
        {
            var item = new ModelItem("foo-cloud") { IsCloud = true, IsInstalled = true };
            Assert.Equal(Loc.T("CloudBadge"), item.Detail);
        }

        [Fact]
        public void Detail_for_missing_model_is_download_size()
        {
            var item = new ModelItem("translategemma:4b") { DownloadSize = "TranslateGemma 4B · 3.3 GB" };
            Assert.Equal("TranslateGemma 4B · 3.3 GB", item.Detail);
        }
    }
}
