using DeepLocal.Services;
using Xunit;

namespace DeepLocal.Tests
{
    public class LanguagesTests
    {
        [Theory]
        [InlineData("EN", "en")]
        [InlineData("it", "it")]
        [InlineData("zh-TW", "zh-TW")]
        public void ByCode_is_case_insensitive(string input, string expected)
            => Assert.Equal(expected, Languages.ByCode(input)?.Code);

        [Fact]
        public void ByCode_auto_is_auto()
            => Assert.True(Languages.ByCode("auto")!.IsAuto);

        [Fact]
        public void ByCode_unknown_is_null()
            => Assert.Null(Languages.ByCode("xx"));

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ByCode_blank_is_null(string? code)
            => Assert.Null(Languages.ByCode(code));

        [Theory]
        [InlineData("Italian", "it")]
        [InlineData("italiano", "it")]
        [InlineData("Italian.", "it")]
        [InlineData("Chinese", "zh")]
        [InlineData("Mandarin", "zh")]
        [InlineData("Farsi", "fa")]
        [InlineData("Portuguese", "pt")]
        [InlineData("Brazilian Portuguese", "pt")]
        public void ByName_maps_common_names(string raw, string expected)
            => Assert.Equal(expected, Languages.ByName(raw)?.Code);

        [Fact]
        public void ByName_blank_is_null()
            => Assert.Null(Languages.ByName("   "));
    }
}
