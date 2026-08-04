using System.Globalization;
using System.IO;
using System.Text;
using CutAssistantNext.Core;
using CutAssistantNext.Core.Logging;

namespace CutAssistantNext.App.Logging;

public sealed class FileAppLogger : IAppLogger
{
    public const long DefaultMaximumFileSizeBytes =
        2 * 1024 * 1024;

    public const int DefaultRetainedFileCount = 3;

    private const string DefaultLogFileName =
        "CutAssistantNext.log";

    private static readonly UTF8Encoding Utf8WithoutBom =
        new(encoderShouldEmitUTF8Identifier: false);

    private readonly object _syncRoot = new();
    private readonly string _logFilePath;
    private readonly long _maximumFileSizeBytes;
    private readonly int _retainedFileCount;
    private readonly TimeProvider _timeProvider;

    public FileAppLogger()
        : this(GetDefaultLogFilePath())
    {
    }

    public FileAppLogger(
        string logFilePath,
        long maximumFileSizeBytes =
            DefaultMaximumFileSizeBytes,
        int retainedFileCount =
            DefaultRetainedFileCount,
        TimeProvider? timeProvider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logFilePath);

        if (maximumFileSizeBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumFileSizeBytes),
                "Die maximale Protokolldateigröße muss größer als 0 sein.");
        }

        if (retainedFileCount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(retainedFileCount),
                "Die Anzahl älterer Protokolldateien darf nicht negativ sein.");
        }

        _logFilePath = Path.GetFullPath(logFilePath);
        _maximumFileSizeBytes = maximumFileSizeBytes;
        _retainedFileCount = retainedFileCount;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public string LogFilePath => _logFilePath;

    public void Information(string message)
    {
        WriteEntry(
            "Information",
            message,
            exception: null);
    }

    public void Warning(string message)
    {
        WriteEntry(
            "Warnung",
            message,
            exception: null);
    }

    public void Error(
        string message,
        Exception? exception = null)
    {
        WriteEntry(
            "Fehler",
            message,
            exception);
    }

    public static string GetDefaultLogFilePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            ApplicationInfo.ProductName,
            "Logs",
            DefaultLogFileName);
    }

    private void WriteEntry(
        string level,
        string message,
        Exception? exception)
    {
        try
        {
            var entry = CreateEntry(
                level,
                message,
                exception);

            var entryByteCount =
                Utf8WithoutBom.GetByteCount(entry);

            lock (_syncRoot)
            {
                var directoryPath =
                    Path.GetDirectoryName(_logFilePath);

                if (!string.IsNullOrWhiteSpace(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                RotateIfRequired(entryByteCount);

                File.AppendAllText(
                    _logFilePath,
                    entry,
                    Utf8WithoutBom);
            }
        }
        catch
        {
            // Protokollierungsfehler dürfen die Anwendung
            // nicht beeinträchtigen.
        }
    }

    private string CreateEntry(
        string level,
        string message,
        Exception? exception)
    {
        var timestamp = _timeProvider
            .GetLocalNow()
            .ToString(
                "yyyy-MM-dd HH:mm:ss.fff zzz",
                CultureInfo.InvariantCulture);

        var builder = new StringBuilder();

        builder
            .Append(timestamp)
            .Append(" [")
            .Append(level)
            .Append("] ")
            .AppendLine(
                string.IsNullOrWhiteSpace(message)
                    ? "Keine Meldung angegeben."
                    : message.Trim());

        if (exception is not null)
        {
            builder.AppendLine("Technische Details:");
            builder.AppendLine(exception.ToString());
        }

        return builder.ToString();
    }

    private void RotateIfRequired(
        int upcomingEntryByteCount)
    {
        if (!File.Exists(_logFilePath))
        {
            return;
        }

        var currentLength =
            new FileInfo(_logFilePath).Length;

        if (currentLength == 0 ||
            currentLength + upcomingEntryByteCount <=
                _maximumFileSizeBytes)
        {
            return;
        }

        RotateFiles();
    }

    private void RotateFiles()
    {
        if (_retainedFileCount == 0)
        {
            File.Delete(_logFilePath);
            return;
        }

        var oldestFilePath =
            GetRotatedFilePath(_retainedFileCount);

        if (File.Exists(oldestFilePath))
        {
            File.Delete(oldestFilePath);
        }

        for (var index = _retainedFileCount - 1;
             index >= 1;
             index--)
        {
            var sourceFilePath =
                GetRotatedFilePath(index);

            if (!File.Exists(sourceFilePath))
            {
                continue;
            }

            File.Move(
                sourceFilePath,
                GetRotatedFilePath(index + 1),
                overwrite: true);
        }

        File.Move(
            _logFilePath,
            GetRotatedFilePath(1),
            overwrite: true);
    }

    private string GetRotatedFilePath(int index)
    {
        var directoryPath =
            Path.GetDirectoryName(_logFilePath)
            ?? string.Empty;

        var fileNameWithoutExtension =
            Path.GetFileNameWithoutExtension(
                _logFilePath);

        var extension =
            Path.GetExtension(_logFilePath);

        return Path.Combine(
            directoryPath,
            $"{fileNameWithoutExtension}.{index}{extension}");
    }
}
