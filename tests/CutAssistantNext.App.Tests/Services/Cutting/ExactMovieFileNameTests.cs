using CutAssistantNext.App.Services.Cutting;

namespace CutAssistantNext.App.Tests.Services.Cutting;

public sealed class ExactMovieFileNameTests
{
    [Theory]
    [InlineData("Ghosts - S04E01 - Patience", "Ghosts - S04E01 - Patience.mp4")]
    [InlineData("Ghosts - 4x01 - Patience", "Ghosts - 4x01 - Patience.mp4")]
    [InlineData("Ghostbusters", "Ghostbusters.mp4")]
    [InlineData("Die Teufelsbrigade [21.01.2021]", "Die Teufelsbrigade [21.01.2021].mp4")]
    [InlineData("Die Teufelsbrigade [22.01.2021]", "Die Teufelsbrigade [22.01.2021].mp4")]
    [InlineData("Ärger  mit  Leerzeichen - (Teil 1)", "Ärger  mit  Leerzeichen - (Teil 1).mp4")]
    [InlineData("Film.mp4", "Film.mp4")]
    [InlineData("Film.MP4", "Film.MP4")]
    [InlineData("Film.avi", "Film.avi.mp4")]
    public void PreservesChosenSpellingAndAddsExtensionOnce(string name, string expected)
    {
        Assert.Equal(expected, OutputFileNameBuilder.BuildExactMovieFileName(name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(".mp4")]
    [InlineData(".")]
    [InlineData("..")]
    [InlineData("Film ")]
    [InlineData("Film.")]
    [InlineData("Film: Titel")]
    [InlineData("Film/Titel")]
    [InlineData("Film\\Titel")]
    [InlineData("Film\nTitel")]
    [InlineData("Film\rTitel")]
    [InlineData("Film\tTitel")]
    [InlineData("Film<Titel>")]
    [InlineData("Film|Titel")]
    [InlineData("Film?")]
    [InlineData("Film*")]
    [InlineData("Film\"Titel")]
    [InlineData("CON")]
    [InlineData("con.mp4")]
    [InlineData("CON .mp4")]
    [InlineData("NUL.Film")]
    [InlineData("AUX")]
    [InlineData("PRN")]
    [InlineData("COM1")]
    [InlineData("LPT9")]
    [InlineData("COM¹")]
    [InlineData("LPT².mp4")]
    [InlineData("CONIN$")]
    [InlineData("CONOUT$")]
    public void RejectsInvalidNamesInsteadOfSilentlyChangingThem(string name)
    {
        Assert.Throws<ArgumentException>(() => OutputFileNameBuilder.BuildExactMovieFileName(name));
    }

    [Fact]
    public void ChecksComponentLengthIncludingAutomaticallyAddedExtension()
    {
        Assert.Equal(255, OutputFileNameBuilder.BuildExactMovieFileName(new string('a', 251)).Length);
        Assert.Throws<ArgumentException>(() => OutputFileNameBuilder.BuildExactMovieFileName(new string('a', 252)));
    }

    [Fact]
    public void DoesNotChangeLegacySuggestionSanitizing()
    {
        Assert.Equal("Film_ Titel.mp4", OutputFileNameBuilder.Build("Film: Titel", ".mp4"));
        Assert.Equal("_CON.mp4", OutputFileNameBuilder.Build("CON", ".mp4"));
    }
}
