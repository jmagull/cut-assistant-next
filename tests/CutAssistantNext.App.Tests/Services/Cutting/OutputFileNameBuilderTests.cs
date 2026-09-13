using CutAssistantNext.App.Services.Cutting;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class OutputFileNameBuilderTests
{
    [Fact]
    public void Build_WithNormalName_AppendsExtension()
    {
        var result =
            OutputFileNameBuilder.Build(
                "Tatort [18.08.2026]",
                ".mp4");

        Assert.Equal(
            "Tatort [18.08.2026].mp4",
            result);
    }

    [Fact]
    public void Build_WithInvalidWindowsCharacters_ReplacesThem()
    {
        var result =
            OutputFileNameBuilder.Build(
                "Tatort: Wer/was? <Test>|*",
                ".mp4");

        Assert.Equal(
            "Tatort_ Wer_was_ _Test___",
            Path.GetFileNameWithoutExtension(result));
    }

    [Fact]
    public void Build_WithTrailingDotsAndSpaces_RemovesThem()
    {
        var result =
            OutputFileNameBuilder.Build(
                "Tatort...   ",
                ".mp4");

        Assert.Equal(
            "Tatort.mp4",
            result);
    }

    [Fact]
    public void Build_WithReservedWindowsDeviceName_PrefixesName()
    {
        var result =
            OutputFileNameBuilder.Build(
                "CON",
                ".mp4");

        Assert.Equal(
            "_CON.mp4",
            result);
    }

    [Fact]
    public void Build_WithExtensionWithoutDot_NormalizesExtension()
    {
        var result =
            OutputFileNameBuilder.Build(
                "Tatort",
                "mp4");

        Assert.Equal(
            "Tatort.mp4",
            result);
    }

    [Theory]
    [InlineData("Tatort.mp4")]
    [InlineData("Tatort.MP4")]
    public void Build_WhenNameAlreadyHasRequestedExtension_DoesNotDuplicateIt(
        string suggestedMovieName)
    {
        var result =
            OutputFileNameBuilder.Build(
                suggestedMovieName,
                ".mp4");

        Assert.Equal(
            suggestedMovieName,
            result);
    }
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("...")]
    public void Build_WhenNameBecomesEmpty_Throws(
        string suggestedMovieName)
    {
        Assert.Throws<ArgumentException>(
            () =>
                OutputFileNameBuilder.Build(
                    suggestedMovieName,
                    ".mp4"));
    }
}
