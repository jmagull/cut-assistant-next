# Cut Assistant Next – Proof of Concept

Moderner Nachfolger des Cut Assistant für Windows 11 – ohne DirectShow und ohne Abhängigkeit von installierten Windows-Codecs.

## Ziel des Proof of Concept

**Proof of Concept 0.1: Medienwiedergabe und Navigation**

Der erste Prototyp soll nachweisen, dass typische OTR-MP4-Dateien zuverlässig geöffnet, analysiert, abgespielt und präzise navigiert werden können.

## Aktueller Entwicklungsstand

Die .NET-Solution und die grundlegenden Projekte sind eingerichtet:

- `CutAssistantNext.Core`
- `CutAssistantNext.Media`
- `CutAssistantNext.App`
- `CutAssistantNext.Core.Tests`
- `CutAssistantNext.Media.Tests`

Die erste funktionsfähige Medienkomponente ist umgesetzt: Cut Assistant Next kann `ffprobe.exe` tatsächlich starten, die JSON-Ausgabe lesen und in strukturierte .NET-Objekte überführen.

## ffprobe-Medienanalyse

Der `FfprobeRunner` startet `ffprobe.exe` als externen Prozess.

Standardpfad:

```text
C:\Tools\ffmpeg\bin\ffprobe.exe
```

Der Pfad kann beim Erzeugen des `FfprobeRunner` durch einen anderen Pfad ersetzt werden.

Für die Analyse wird sinngemäß folgender Aufruf verwendet:

```text
ffprobe.exe
-v error
-show_streams
-show_format
-of json
<Media-Datei>
```

Der technische Ablauf:

```text
Mediendatei
    ↓
FfprobeRunner
    ↓
ffprobe.exe
    ↓
JSON-Ausgabe
    ↓
FfprobeJsonParser
    ↓
MediaAnalysisResult
```

Ermittelt werden unter anderem:

- Containerformat
- Dateigröße
- Laufzeit
- Video- und Audiostreams
- Video- und Audiocodecs
- Auflösung
- Sample Aspect Ratio
- Display Aspect Ratio
- Bildrate
- Field Order
- Samplerate
- Kanalanzahl
- Kanallayout

Der Runner behandelt außerdem:

- fehlende `ffprobe.exe`
- fehlende Mediendateien
- fehlerhafte ffprobe-Prozessaufrufe
- leere JSON-Ausgaben
- Prozessabbrüche über `CancellationToken`

Dateipfade mit Leerzeichen werden über `ProcessStartInfo.ArgumentList` sicher übergeben.

## Manueller Praxistest

Der vollständige Ablauf wurde mit einer realen MP4-Datei erfolgreich geprüft.

```text
Format:       QuickTime / MOV
Dateigröße:   973.028.644 Byte
Dauer:        00:28:53,5
Videostreams: 1
Audiostreams: 1

Video:
  Codec:       H.264
  Auflösung:   1920 × 1080
  SAR:         1:1
  DAR:         16:9
  Bildrate:    50 fps
  Field Order: progressive

Audio:
  Codec:       AAC
  Samplerate:  48.000 Hz
  Kanäle:      2
  Layout:      stereo
```

Damit ist nachgewiesen, dass Cut Assistant Next nicht nur vorbereitetes Test-JSON verarbeitet, sondern `ffprobe.exe` tatsächlich ausführt.

## Tests

Die Parser- und Runner-Funktionen sind durch automatisierte xUnit-Tests abgesichert.

```text
Tests insgesamt:  7
Erfolgreich:      7
Fehlgeschlagen:   0
Übersprungen:     0
```

Tests ausführen:

```powershell
dotnet test .\CutAssistantNext.sln
```

Release-Build ausführen:

```powershell
dotnet build .\CutAssistantNext.sln --configuration Release
```

## Technische Grundlage

- C# und .NET 10
- WPF
- ffprobe für Medieninformationen
- mpv/libmpv für die geplante Videowiedergabe
- xUnit für automatisierte Tests
- Git und GitHub für Versionsverwaltung
- Codex als Programmierwerkstatt
- ChatGPT für Architektur, Planung, Testauswertung und Dokumentation

## POC-0.1-Funktionsumfang

Bereits umgesetzt:

- Visual-Studio-Solution und Projektstruktur
- Datenmodell für Medieninformationen
- Parser für ffprobe-JSON
- Behandlung unvollständiger und ungültiger JSON-Daten
- tatsächliche Ausführung von `ffprobe.exe`
- Übergabe der JSON-Ausgabe an den vorhandenen Parser
- automatisierte Tests für Parser und Runner
- erfolgreicher Praxistest mit einer realen MP4-Datei

Als Nächstes geplant:

- MP4-Datei über die WPF-Oberfläche auswählen
- Medieninformationen in der Oberfläche anzeigen
- mpv/libmpv einbinden
- eingebettete Videowiedergabe
- Play/Pause
- Positionsanzeige und Zeitleiste
- Einzelbild vorwärts und rückwärts
- Lautstärkeregelung
- verständliche Protokolldatei

Noch nicht enthalten sind Cutlists, Cutlist-Server, MP4Box-/FFmpeg-Schnitt, Stapelverarbeitung und automatische Umbenennung.

## Repository-Struktur

```text
cut-assistant-next/
├── CutAssistantNext.sln
├── AGENTS.md
├── README.md
├── docs/
├── src/
│   ├── CutAssistantNext.App/
│   ├── CutAssistantNext.Core/
│   └── CutAssistantNext.Media/
├── tests/
│   ├── CutAssistantNext.Core.Tests/
│   └── CutAssistantNext.Media.Tests/
├── samples/
│   └── cutlists/
└── tools/
```

## Entwicklungsstand

Die tatsächliche ffprobe-Ausführung wurde mit Pull Request **#3** in `main` übernommen.

## Arbeitsgrundsatz

> Eine Funktion planen, umsetzen, testen, dokumentieren und erst danach den nächsten Schritt beginnen.
