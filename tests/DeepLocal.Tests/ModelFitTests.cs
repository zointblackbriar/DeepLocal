using System.Collections.Generic;
using DeepLocal.Services;
using Xunit;

namespace DeepLocal.Tests
{
    public class ModelFitTests
    {
        private const long Vram6Gb = 6_439_305_216L;   // RTX 4050 Laptop
        private const long Translategemma12b = 8_100_000_000L;
        private const long Translategemma4b = 3_300_000_000L;

        private static ModelItem Gg(string name, long size) =>
            new(name) { Size = size, Family = "gemma3", IsInstalled = true };

        [Fact]
        public void Budget_is_85_percent()
            => Assert.Equal(5_100_000_000L, ModelFit.Budget(6_000_000_000L));

        [Fact]
        public void Fits_true_for_cloud_models()
            => Assert.True(ModelFit.Fits(isCloud: true, Translategemma12b, Vram6Gb));

        [Fact]
        public void Fits_true_when_size_unknown()
            => Assert.True(ModelFit.Fits(isCloud: false, modelSizeBytes: 0, Vram6Gb));

        [Fact]
        public void Fits_true_when_vram_unknown()
            => Assert.True(ModelFit.Fits(isCloud: false, Translategemma12b, vramBytes: null));

        [Fact]
        public void Fits_false_when_model_exceeds_budget()
            => Assert.False(ModelFit.Fits(isCloud: false, Translategemma12b, Vram6Gb));

        [Fact]
        public void Fits_true_when_model_fits()
            => Assert.True(ModelFit.Fits(isCloud: false, Translategemma4b, Vram6Gb));

        [Theory]
        [InlineData("translategemma:12b", "translategemma")]
        [InlineData("qwen2.5-coder:7b", "qwen2.5-coder")]
        [InlineData("plain", "plain")]
        public void BaseName_strips_tag(string name, string expected)
            => Assert.Equal(expected, ModelFit.BaseName(name));

        [Fact]
        public void SmallerThatFits_picks_largest_same_family_that_fits()
        {
            var twelve = Gg("translategemma:12b", Translategemma12b);
            var four = Gg("translategemma:4b", Translategemma4b);
            var qwen = Gg("qwen2.5-coder:7b", 4_700_000_000L);

            var chosen = ModelFit.SmallerThatFits(twelve, new List<ModelItem> { twelve, four, qwen }, Vram6Gb);

            Assert.Same(four, chosen);
        }

        [Fact]
        public void SmallerThatFits_null_when_current_already_fits()
        {
            var four = Gg("translategemma:4b", Translategemma4b);
            var twelve = Gg("translategemma:12b", Translategemma12b);

            Assert.Null(ModelFit.SmallerThatFits(four, new List<ModelItem> { twelve, four }, Vram6Gb));
        }

        [Fact]
        public void SmallerThatFits_null_when_no_same_family_candidate()
        {
            var twelve = Gg("translategemma:12b", Translategemma12b);
            var qwen = Gg("qwen2.5-coder:7b", 4_700_000_000L);

            Assert.Null(ModelFit.SmallerThatFits(twelve, new List<ModelItem> { qwen }, Vram6Gb));
        }

        [Fact]
        public void FormatVramGb_rounds_to_one_decimal()
        {
            Assert.Equal("6", ModelFit.FormatVramGb(Vram6Gb));
            Assert.Equal("?", ModelFit.FormatVramGb(null));
        }
    }
}
