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

Die ffprobe-Medienanalyse und die erste eingebettete libmpv-Wiedergabe sind inzwischen bis in die WPF-Anwendung integriert.

Eine MP4-Datei kann über einen Dateiauswahldialog ausgewählt, asynchron analysiert und anschließend direkt im eingebetteten Videofenster mit Bild und Ton wiedergegeben werden.

Die Analyse ist über `IMediaAnalysisRunner` abstrahiert. Das testbare `MainWindowViewModel` verwaltet Status, Fehleranzeige, Auslastungszustand und die formatierten Medieninformationen.

Die Wiedergabe ist über `IMediaPlayerService` und `MpvMediaPlayerService` gekapselt. Ein eigener WPF-Host auf Basis von `HwndHost` stellt das native Fensterhandle für libmpv bereit.

Die native Laufzeitbibliothek wird fest versioniert, per SHA-256 kontrolliert und beim Build automatisch in den Ausgabeordner kopiert. Beim Schließen der Anwendung wird libmpv vollständig freigegeben, bevor das native Videofenster zerstört wird.

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

Der Status wechselte nach Abschluss auf `Analyse erfolgreich abgeschlossen.` Die Oberfläche blieb während der asynchronen Analyse reaktionsfähig.

Zusätzlich wurde die eingebettete Wiedergabe mit der realen Datei `2068756_60422686.mp4` geprüft. libmpv zeigte das Video innerhalb des WPF-Fensters an und gab den Ton korrekt aus.

Nach dem normalen Schließen der Anwendung wurde der Prozess vollständig beendet. Damit ist die vollständige Kette von der Dateiauswahl über ffprobe bis zur eingebetteten Bild- und Tonwiedergabe praktisch nachgewiesen.

## Tests

Parser, Runner, ViewModel und MediaPlayer-Service sind durch automatisierte xUnit-Tests abgesichert.

Die Tests prüfen unter anderem:

- Übernahme und Formatierung erfolgreicher Analyseergebnisse
- verständliche Fehleranzeige
- `IsAnalyzing` und `CanAnalyze` während einer laufenden Analyse
- Ersatzanzeige `Nicht verfügbar` bei fehlenden Werten
- Initialisierung und Zustandswechsel des MediaPlayer-Service
- Laden, Wiedergabe, Pause, Seeking und Stoppen
- Verarbeitung von libmpv-Ereignissen und Fehlern

```text
Tests insgesamt:  17
Erfolgreich:      17
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
- mpv/libmpv für die eingebettete Videowiedergabe
- `HanumanInstitute.LibMpv` 0.10.1 als .NET-Anbindung
- eigener `HwndHost` für die Windows-HWND-Einbettung
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
- eigene Wiedergabeschnittstelle `IMediaPlayerService`
- testbarer `MpvMediaPlayerService`
- interne Kapselung der libmpv-Aufrufe
- `HanumanInstitute.LibMpv` als .NET-Wrapper
- reproduzierbares Setup der fest versionierten `libmpv-2.dll`
- SHA-256-Prüfung des heruntergeladenen libmpv-Archivs
- automatische Übernahme der nativen DLL in Build- und Publish-Ordner
- eigener WPF-Video-Host auf Basis von `HwndHost`
- Einbettung über die mpv-Option `wid`
- kontrollierte Initialisierung und Freigabe von libmpv
- eingebettete Videowiedergabe mit Bild und Ton
- automatisierte Tests für Parser, Runner, ViewModel und MediaPlayer-Service
- erfolgreicher Praxistest mit realen MP4-Dateien in der WPF-Anwendung

Als Nächstes geplant:

- Bedienelemente für Play und Pause
- Positionsanzeige und Zeitleiste
- Seeking über die Oberfläche
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

Die WPF-Integration der Medienanalyse wurde mit Pull Request **#5** übernommen.

Die eingebettete libmpv-Wiedergabe ist im aktuellen Entwicklungsstand umgesetzt. Bild und Ton, native HWND-Einbettung, reproduzierbare Bereitstellung der Laufzeitbibliothek sowie der kontrollierte Anwendungs-Shutdown wurden praktisch geprüft.

Der nächste Entwicklungsschritt ist die Wiedergabesteuerung mit Play, Pause, Positionsanzeige und Zeitleiste.

## Arbeitsgrundsatz

> Eine Funktion planen, umsetzen, testen, dokumentieren und erst danach den nächsten Schritt beginnen.
