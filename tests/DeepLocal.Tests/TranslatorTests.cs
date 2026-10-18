using DeepLocal.Services;
using Xunit;

namespace DeepLocal.Tests
{
    public class TranslatorTests
    {
        private static Language It => Languages.ByCode("it")!;
        private static Language En => Languages.ByCode("en")!;

        [Fact]
        public void TranslateGemma_prompt_names_both_languages()
        {
            var prompt = Translator.BuildPrompt("translategemma:12b", "Ciao", It, En);

            Assert.Contains("professional Italian (it) to English (en) translator", prompt);
            Assert.Contains("Please translate the following Italian text into English:", prompt);
            Assert.EndsWith("\n\n\nCiao", prompt);
        }

        [Fact]
        public void TranslateGemma_prompt_auto_source_omits_source()
        {
            var prompt = Translator.BuildPrompt("translategemma:12b", "Ciao", null, En);

            Assert.Contains("professional translator into English (en)", prompt);
            Assert.DoesNotContain("Italian", prompt);
            Assert.EndsWith("\n\n\nCiao", prompt);
        }

        [Fact]
        public void Generic_prompt_mentions_source_and_appends_text()
        {
            var prompt = Translator.BuildPrompt("llama3", "Ciao", It, En);

            Assert.Contains("from Italian", prompt);
            Assert.Contains("Output ONLY the English translation", prompt);
            Assert.EndsWith("Text:\nCiao", prompt);
        }

        [Theory]
        [InlineData("こんにちは世界", "ja")]
        [InlineData("안녕하세요", "ko")]
        [InlineData("你好世界", "zh")]
        [InlineData("Привіт", "uk")]
        [InlineData("Привет", "ru")]
        [InlineData("مرحبا", "ar")]
        [InlineData("پدر", "fa")]
        [InlineData("Γειά σου", "el")]
        [InlineData("สวัสดี", "th")]
        [InlineData("नमस्ते", "hi")]
        [InlineData("שלום", "he")]
        public void DetectByScript_maps_scripts(string text, string code)
            => Assert.Equal(code, Translator.DetectByScript(text)?.Code);

        [Fact]
        public void DetectByScript_plain_latin_is_unknown()
            => Assert.Null(Translator.DetectByScript("Hello world"));

        [Fact]
        public void DetectByScript_latin_with_few_han_stays_unknown()
            => Assert.Null(Translator.DetectByScript("Hello world 世界"));

        [Fact]
        public void CleanOutput_strips_wrapping_quotes()
            => Assert.Equal("Ciao", Translator.CleanOutput("\"Ciao\"", "Ciao"));

        [Fact]
        public void CleanOutput_removes_translation_label()
            => Assert.Equal("Ciao", Translator.CleanOutput("Translation: Ciao", "ciao"));

        [Fact]
        public void CleanOutput_removes_trailing_end_marker()
            => Assert.Equal("Ciao", Translator.CleanOutput("Ciao\nEND", "ciao"));

        [Fact]
        public void CleanOutput_unwraps_code_fence()
            => Assert.Equal("Ciao", Translator.CleanOutput("```\nCiao\n```", "ciao"));

        [Fact]
        public void CleanOutput_strips_thinking_block()
            => Assert.Equal("Ciao", Translator.CleanOutput("<think>hmm</think>Ciao", "ciao"));

        [Fact]
        public void CleanOutput_empty_input_is_empty()
            => Assert.Equal("", Translator.CleanOutput("", "x"));

        [Fact]
        public void StripThinking_removes_reasoning_block()
            => Assert.Equal("ab", Translator.StripThinking("a<think>x</think>b"));
    }
}
