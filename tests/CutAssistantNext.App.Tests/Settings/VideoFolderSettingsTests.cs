using System.IO;
using CutAssistantNext.App.Settings;
using CutAssistantNext.App.ViewModels;

namespace CutAssistantNext.App.Tests.Settings;

public sealed class VideoFolderSettingsTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"can-video-folders-{Guid.NewGuid():N}");
    private string SettingsPath => Path.Combine(_directory, "Settings", "folders.json");

    [Fact]
    public void TwoFoldersRoundTripAndRemainIndependentAfterRestart()
    {
        var original = Directory.CreateDirectory(Path.Combine(_directory, "Originale – Übersee")).FullName;
        var output = Directory.CreateDirectory(Path.Combine(_directory, "Geschnitten")).FullName;
        var store = new VideoFolderSettingsStore(SettingsPath);
        Assert.True(store.Save(new VideoFolderSettings
        {
            OriginalVideosDirectory = original,
            CutVideosDirectory = output
        }));

        var loaded = new VideoFolderSettingsStore(SettingsPath).Load();
        Assert.Equal(original, VideoFolderPathResolver.Resolve(loaded.OriginalVideosDirectory));
        Assert.Equal(output, VideoFolderPathResolver.Resolve(loaded.CutVideosDirectory));
        Assert.False(File.ReadAllBytes(SettingsPath).Take(3).SequenceEqual(new byte[] { 239, 187, 191 }));

        Assert.True(store.Save(loaded with { CutVideosDirectory = string.Empty }));
        var reset = new VideoFolderSettingsStore(SettingsPath).Load();
        Assert.Equal(original, reset.OriginalVideosDirectory);
        Assert.Equal(string.Empty, reset.CutVideosDirectory);
    }

    [Fact]
    public void MissingSettingsLeaveBothDefaultsEmpty()
    {
        Assert.Equal(new VideoFolderSettings(), new VideoFolderSettingsStore(SettingsPath).Load());
    }

    [Theory]
    [InlineData("{invalid")]
    [InlineData("null")]
    [InlineData("{\"OriginalVideosDirectory\":123}")]
    public void CorruptSettingsFallBackWithoutChangingTheFile(string json)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, json);
        Assert.Equal(new VideoFolderSettings(), new VideoFolderSettingsStore(SettingsPath).Load());
        Assert.Equal(json, File.ReadAllText(SettingsPath));
    }

    [Fact]
    public void NullAndMissingJsonFieldsAreHandledIndependently()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        var output = Path.Combine(_directory, "Schnitte");
        File.WriteAllText(SettingsPath, System.Text.Json.JsonSerializer.Serialize(new
        {
            OriginalVideosDirectory = (string?)null,
            CutVideosDirectory = output
        }));
        var loaded = new VideoFolderSettingsStore(SettingsPath).Load();
        Assert.Equal(string.Empty, loaded.OriginalVideosDirectory);
        Assert.Equal(output, loaded.CutVideosDirectory);
    }

    [Fact]
    public void UnavailableFolderFallsBackWithoutErasingThePreference()
    {
        var configured = Path.Combine(_directory, "Offline-Laufwerk");
        var store = new VideoFolderSettingsStore(SettingsPath);
        Assert.True(store.Save(new VideoFolderSettings { OriginalVideosDirectory = configured }));
        var loaded = store.Load();
        Assert.Equal(string.Empty, VideoFolderPathResolver.Resolve(loaded.OriginalVideosDirectory));
        Assert.Equal(configured, store.Load().OriginalVideosDirectory);
    }

    [Fact]
    public void SaveFailureIsReportedAndTheBlockingFileIsPreserved()
    {
        Directory.CreateDirectory(_directory);
        var blockingFile = Path.Combine(_directory, "file");
        File.WriteAllText(blockingFile, "keep this file");
        var store = new VideoFolderSettingsStore(Path.Combine(blockingFile, "settings.json"));
        Assert.False(store.Save(new VideoFolderSettings()));
        Assert.Equal("keep this file", File.ReadAllText(blockingFile));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("relative-folder")]
    [InlineData(@"\relative-to-drive")]
    [InlineData(@"C:relative-to-drive")]
    [InlineData("C:\\invalid\0folder")]
    public void InvalidOrEmptyInitialDirectoryNeverUsesTheWorkingDirectory(string? directory)
    {
        Assert.Equal(string.Empty, VideoFolderPathResolver.Resolve(directory,
            _ => throw new InvalidOperationException("Invalid paths must not be probed.")));
    }

    [Fact]
    public void AFileCannotBeUsedAsAnOutputFolder()
    {
        Directory.CreateDirectory(_directory);
        var file = Path.Combine(_directory, "film.mp4");
        File.WriteAllText(file, "test");
        var settings = new VideoFolderSettings { CutVideosDirectory = file };
        Assert.Contains("Geschnittene Videos", VideoFolderPathResolver.GetValidationError(settings)!);
        Assert.Equal(string.Empty, VideoFolderPathResolver.Resolve(file));
    }

    [Fact]
    public void RelativeConfiguredPathHasAnUnderstandableValidationError()
    {
        var settings = new VideoFolderSettings { OriginalVideosDirectory = "videos" };
        var error = VideoFolderPathResolver.GetValidationError(settings);
        Assert.Contains("Originalvideos", error!);
        Assert.Contains("vollständigen Ordnerpfad", error!);
    }

    [Fact]
    public void OptionalFoldersAndTheSameFolderForBothAreAllowed()
    {
        var folder = Directory.CreateDirectory(_directory).FullName;
        Assert.Null(VideoFolderPathResolver.GetValidationError(new VideoFolderSettings()));
        Assert.Null(VideoFolderPathResolver.GetValidationError(new VideoFolderSettings
        {
            OriginalVideosDirectory = folder,
            CutVideosDirectory = folder
        }));
        Assert.Null(VideoFolderPathResolver.GetValidationError(new VideoFolderSettings { CutVideosDirectory = folder }));
    }

    [Fact]
    public void ViewModelNormalizesPathsAndDoesNotMutateLoadedSettingsBeforeSave()
    {
        var original = Path.Combine(_directory, "Originale");
        var loaded = new VideoFolderSettings { OriginalVideosDirectory = original };
        var viewModel = new VideoFolderSettingsViewModel(loaded)
        {
            CutVideosDirectory = "  " + Path.Combine(_directory, "Schnitte") + "  "
        };
        var edited = viewModel.CreateSettings();
        Assert.Equal(original, edited.OriginalVideosDirectory);
        Assert.Equal(Path.Combine(_directory, "Schnitte"), edited.CutVideosDirectory);
        Assert.Equal(string.Empty, loaded.CutVideosDirectory);
    }

    [Fact]
    public void ExistingVideoSettingsLoadWithoutACutlistFolderAndRetainTheirValues()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        var original = Path.Combine(_directory, "Originale");
        var output = Path.Combine(_directory, "Schnitte");
        File.WriteAllText(SettingsPath, System.Text.Json.JsonSerializer.Serialize(new
        {
            OriginalVideosDirectory = original,
            CutVideosDirectory = output
        }));
        var store = new VideoFolderSettingsStore(SettingsPath);
        var loaded = store.Load();
        Assert.Equal(original, loaded.OriginalVideosDirectory);
        Assert.Equal(output, loaded.CutVideosDirectory);
        Assert.Equal(string.Empty, loaded.OwnCutlistsDirectory);
        var viewModel = new VideoFolderSettingsViewModel(loaded)
        {
            OwnCutlistsDirectory = "  " + Path.Combine(_directory, "Eigene Cutlists") + "  "
        };
        Assert.True(store.Save(viewModel.CreateSettings()));
        var reloaded = new VideoFolderSettingsStore(SettingsPath).Load();
        Assert.Equal(original, reloaded.OriginalVideosDirectory);
        Assert.Equal(output, reloaded.CutVideosDirectory);
        Assert.Equal(Path.Combine(_directory, "Eigene Cutlists"), reloaded.OwnCutlistsDirectory);
        viewModel.OwnCutlistsDirectory = string.Empty;
        Assert.True(store.Save(viewModel.CreateSettings()));
        Assert.Equal(loaded, store.Load());
    }

    [Fact]
    public void ConfiguredCutlistFolderWinsForOpeningAndSaving()
    {
        var cutlists = Directory.CreateDirectory(Path.Combine(_directory, "Cutlists")).FullName;
        var original = Directory.CreateDirectory(Path.Combine(_directory, "Originale")).FullName;
        var settings = new VideoFolderSettings { OwnCutlistsDirectory = cutlists };
        Assert.Equal(cutlists, VideoFolderPathResolver.ResolveCutlists(settings));
        Assert.Equal(cutlists, VideoFolderPathResolver.ResolveCutlists(settings, original));
    }

    [Theory]
    [InlineData("")]
    [InlineData("missing")]
    [InlineData("relative-path")]
    public void AbsentCutlistFolderPreservesPreviousOpenAndSaveDefaults(string input)
    {
        var original = Directory.CreateDirectory(Path.Combine(_directory, "Originale")).FullName;
        var configured = input == "missing" ? Path.Combine(_directory, input) : input;
        var settings = new VideoFolderSettings { OwnCutlistsDirectory = configured };
        Assert.Equal(string.Empty, VideoFolderPathResolver.ResolveCutlists(settings));
        Assert.Equal(original, VideoFolderPathResolver.ResolveCutlists(settings, original));
        Assert.Equal(configured, settings.OwnCutlistsDirectory);
    }

    [Fact]
    public void MissingFallbackAlsoLeavesTheFileDialogUsable()
    {
        Assert.Equal(string.Empty, VideoFolderPathResolver.ResolveCutlists(new VideoFolderSettings(),
            Path.Combine(_directory, "missing")));
    }

    [Theory]
    [InlineData("relative")]
    [InlineData("missing")]
    [InlineData("file")]
    public void InvalidCutlistFolderHasItsOwnValidationMessage(string kind)
    {
        Directory.CreateDirectory(_directory);
        var path = kind == "relative" ? "relative" : Path.Combine(_directory, kind);
        if (kind == "file") File.WriteAllText(path, "keep");
        var error = VideoFolderPathResolver.GetValidationError(new VideoFolderSettings { OwnCutlistsDirectory = path });
        Assert.Contains("Eigene Cutlists", error!);
    }

    [Fact]
    public void NullCutlistJsonValueIsTreatedAsOptional()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, "{\"OwnCutlistsDirectory\":null}");
        var settings = new VideoFolderSettingsStore(SettingsPath).Load();
        Assert.Equal(string.Empty, settings.OwnCutlistsDirectory);
        Assert.Null(VideoFolderPathResolver.GetValidationError(settings));
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true);
    }
}
