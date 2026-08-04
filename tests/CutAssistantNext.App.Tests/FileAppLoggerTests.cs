using System.Text;
using CutAssistantNext.App.Logging;

namespace CutAssistantNext.App.Tests;

public sealed class FileAppLoggerTests
{
    [Fact]
    public void WriteMethods_CreateUtf8EntriesWithTechnicalDetails()
    {
        var testDirectory = CreateTestDirectory();
        var logFilePath = Path.Combine(
            testDirectory,
            "CutAssistantNext.log");

        try
        {
            var logger = new FileAppLogger(logFilePath);

            logger.Information(
                "Lautstärke wurde geändert.");

            logger.Warning(
                "Grüße mit Umlauten und ß.");

            logger.Error(
                "Die Testaktion ist fehlgeschlagen.",
                new InvalidOperationException(
                    "Technischer Testfehler."));

            var content = File.ReadAllText(
                logFilePath,
                Encoding.UTF8);

            Assert.Contains(
                "[Information] Lautstärke wurde geändert.",
                content);

            Assert.Contains(
                "[Warnung] Grüße mit Umlauten und ß.",
                content);

            Assert.Contains(
                "[Fehler] Die Testaktion ist fehlgeschlagen.",
                content);

            Assert.Contains(
                "Technische Details:",
                content);

            Assert.Contains(
                "InvalidOperationException",
                content);

            Assert.Contains(
                "Technischer Testfehler.",
                content);

            var bytes = File.ReadAllBytes(logFilePath);

            Assert.False(
                bytes.Length >= 3 &&
                bytes[0] == 0xEF &&
                bytes[1] == 0xBB &&
                bytes[2] == 0xBF);
        }
        finally
        {
            Directory.Delete(
                testDirectory,
                recursive: true);
        }
    }

    [Fact]
    public void Information_RotatesFilesAtConfiguredLimit()
    {
        var testDirectory = CreateTestDirectory();
        var logFilePath = Path.Combine(
            testDirectory,
            "CutAssistantNext.log");

        try
        {
            var logger = new FileAppLogger(
                logFilePath,
                maximumFileSizeBytes: 100,
                retainedFileCount: 2);

            logger.Information(
                new string('A', 80));

            logger.Information(
                new string('B', 80));

            logger.Information(
                new string('C', 80));

            Assert.True(File.Exists(logFilePath));

            Assert.True(
                File.Exists(
                    Path.Combine(
                        testDirectory,
                        "CutAssistantNext.1.log")));

            Assert.True(
                File.Exists(
                    Path.Combine(
                        testDirectory,
                        "CutAssistantNext.2.log")));

            Assert.Contains(
                new string('C', 80),
                File.ReadAllText(
                    logFilePath,
                    Encoding.UTF8));
        }
        finally
        {
            Directory.Delete(
                testDirectory,
                recursive: true);
        }
    }

    [Fact]
    public void WriteMethods_DoNotThrowWhenLogCannotBeWritten()
    {
        var testDirectory = CreateTestDirectory();
        var blockingFilePath = Path.Combine(
            testDirectory,
            "KeinOrdner");

        File.WriteAllText(
            blockingFilePath,
            "Diese Datei verhindert einen Unterordner.");

        try
        {
            var logFilePath = Path.Combine(
                blockingFilePath,
                "CutAssistantNext.log");

            var logger = new FileAppLogger(logFilePath);

            var exception = Record.Exception(
                () => logger.Information(
                    "Dieser Eintrag kann nicht geschrieben werden."));

            Assert.Null(exception);
            Assert.False(File.Exists(logFilePath));
        }
        finally
        {
            Directory.Delete(
                testDirectory,
                recursive: true);
        }
    }

    [Fact]
    public void Information_SupportsConcurrentWrites()
    {
        var testDirectory = CreateTestDirectory();
        var logFilePath = Path.Combine(
            testDirectory,
            "CutAssistantNext.log");

        try
        {
            var logger = new FileAppLogger(logFilePath);

            Parallel.For(
                0,
                100,
                index => logger.Information(
                    $"Paralleler Eintrag {index:D3}."));

            var lines = File.ReadAllLines(
                logFilePath,
                Encoding.UTF8);

            Assert.Equal(100, lines.Length);

            for (var index = 0; index < 100; index++)
            {
                Assert.Contains(
                    lines,
                    line => line.Contains(
                        $"Paralleler Eintrag {index:D3}.",
                        StringComparison.Ordinal));
            }
        }
        finally
        {
            Directory.Delete(
                testDirectory,
                recursive: true);
        }
    }

    [Fact]
    public void GetDefaultLogFilePath_UsesLocalApplicationData()
    {
        var expectedDirectory = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "Cut Assistant Next",
            "Logs");

        var logFilePath =
            FileAppLogger.GetDefaultLogFilePath();

        Assert.Equal(
            expectedDirectory,
            Path.GetDirectoryName(logFilePath));

        Assert.Equal(
            "CutAssistantNext.log",
            Path.GetFileName(logFilePath));
    }

    private static string CreateTestDirectory()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            $"CutAssistantNext-Logging-{Guid.NewGuid():N}");

        Directory.CreateDirectory(path);

        return path;
    }
}
