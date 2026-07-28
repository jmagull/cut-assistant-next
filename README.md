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
- `CutAssistantNext.App.Tests`

Die ffprobe-Medienanalyse ist inzwischen bis in die WPF-Anwendung integriert.

Eine MP4-Datei kann über einen Dateiauswahldialog ausgewählt, asynchron analysiert und ohne Blockierung der Oberfläche ausgewertet werden.

Die Verarbeitung ist über `IMediaAnalysisRunner` abstrahiert. Das testbare `MainWindowViewModel` verwaltet Status, Fehleranzeige, Auslastungszustand und die formatierten Medieninformationen.

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

## WPF-Medienanalyse

Eine MP4-Datei kann direkt über die WPF-Oberfläche ausgewählt und analysiert werden.

Der Ablauf:

```text
MainWindow
    ↓
MainWindowViewModel
    ↓
IMediaAnalysisRunner
    ↓
FfprobeRunner
    ↓
FfprobeJsonParser
    ↓
MediaAnalysisResult
```

Während der Analyse:

- bleibt die Oberfläche reaktionsfähig
- wird die Dateiauswahl vorübergehend deaktiviert
- erscheint der Status `Datei wird analysiert …`
- wird ein unbestimmter Fortschrittsbalken angezeigt

Nach erfolgreicher Analyse werden dargestellt:

- Dateiname und vollständiger Pfad
- Containerformat
- Dateigröße
- Laufzeit
- Video-Codec
- Auflösung
- SAR und DAR
- Bildrate
- Field Order
- Audio-Codec
- Samplerate
- Kanalanzahl
- Kanallayout

Für den Proof of Concept werden jeweils der erste Video- und Audiostream angezeigt.
Fehlende Werte erscheinen als `Nicht verfügbar`.
Fehler des Runners werden verständlich in der Oberfläche dargestellt.

## Manueller Praxistest

Der vollständige ffprobe-Ablauf wurde mit einer realen MP4-Datei erfolgreich über die WPF-Oberfläche geprüft.

```text
Datei:
  Frieren Nach dem Ende der Reise S02E05
  Ein ganz normaler Kerl [26.07.2026].mp4

Container:
  QuickTime / MOV
  Dateigröße: 1,10 GiB
  Laufzeit:   00:25:13.035

Video:
  Codec:       H.264 / AVC
  Auflösung:   1920 × 1080
  SAR:         1:1
  DAR:         16:9
  Bildrate:    25 fps
  Field Order: progressive

Audio:
  Codec:       AAC
  Samplerate:  44.100 Hz
  Kanäle:      2
  Layout:      stereo
```

Der Status wechselte nach Abschluss auf `Analyse erfolgreich abgeschlossen.`

Die Oberfläche blieb während der asynchronen Analyse reaktionsfähig. Damit ist nachgewiesen, dass die vollständige Kette von der Dateiauswahl über `ffprobe.exe` bis zur Ergebnisanzeige funktioniert.

## Tests

Parser, Runner und ViewModel sind durch automatisierte xUnit-Tests abgesichert.

Die ViewModel-Tests prüfen unter anderem:

- Übernahme und Formatierung erfolgreicher Analyseergebnisse
- verständliche Fehleranzeige
- `IsAnalyzing` und `CanAnalyze` während einer laufenden Analyse
- Ersatzanzeige `Nicht verfügbar` bei fehlenden Werten

```text
Tests insgesamt:  11
Erfolgreich:      11
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
- Abstraktion über `IMediaAnalysisRunner`
- MP4-Dateiauswahl über die WPF-Oberfläche
- asynchrone Analyse ohne Blockierung der Oberfläche
- Status- und Fortschrittsanzeige während der Analyse
- verständliche Fehleranzeige
- Anzeige der wichtigsten Datei-, Container-, Video- und Audiowerte
- testbares `MainWindowViewModel`
- automatisierte Tests für Parser, Runner und ViewModel
- erfolgreicher Praxistest mit einer realen MP4-Datei in der WPF-Anwendung

Als Nächstes geplant:

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
│   │   └── ViewModels/
│   ├── CutAssistantNext.Core/
│   └── CutAssistantNext.Media/
├── tests/
│   ├── CutAssistantNext.App.Tests/
│   ├── CutAssistantNext.Core.Tests/
│   └── CutAssistantNext.Media.Tests/
├── samples/
│   └── cutlists/
└── tools/
```

## Entwicklungsstand

Die tatsächliche ffprobe-Ausführung wurde mit Pull Request **#3** in `main` übernommen.

Die zugehörige Dokumentation folgte mit Pull Request **#4**.

Die WPF-Integration der Medienanalyse ist umgesetzt, durch automatisierte Tests abgesichert und mit einer realen MP4-Datei erfolgreich geprüft.

Die WPF-Integration ist für die abschließende Git-Kontrolle und einen Pull Request vorbereitet.

## Arbeitsgrundsatz

> Eine Funktion planen, umsetzen, testen, dokumentieren und erst danach den nächsten Schritt beginnen.
