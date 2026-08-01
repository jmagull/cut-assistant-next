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

Die ffprobe-Medienanalyse, die eingebettete libmpv-Wiedergabe und die grundlegende Player-Steuerung sind inzwischen bis in die WPF-Anwendung integriert.

Eine MP4-Datei kann über einen Dateiauswahldialog ausgewählt, asynchron analysiert und anschließend direkt im eingebetteten Videofenster mit Bild und Ton wiedergegeben werden.

Die Analyse ist über `IMediaAnalysisRunner` abstrahiert. Das testbare `MainWindowViewModel` verwaltet Status, Fehleranzeige, Auslastungszustand und die formatierten Medieninformationen.

Die Wiedergabe ist über `IMediaPlayerService` und `MpvMediaPlayerService` gekapselt. Ein eigener WPF-Host auf Basis von `HwndHost` stellt das native Fensterhandle für libmpv bereit.

Das testbare `PlaybackViewModel` bildet Player-Zustand, aktuelle Position, Gesamtdauer, Frameinformationen und die Verfügbarkeit der Bedienelemente ab. Die Oberfläche bietet Play, Pause, eine formatierte Zeit- und Frameanzeige, eine automatisch mitlaufende Zeitleiste mit Seeking sowie die Einzelbildnavigation um `−10`, `−1`, `+1` und `+10` Frames. Die Frame-Schaltflächen sind nur im pausierten Zustand verfügbar.

Während der Benutzer den Slider bewegt, überschreiben automatische Positionsmeldungen von libmpv nicht den gewählten Vorschauwert. Erst beim Loslassen wird die neue Position an den MediaPlayer-Service übergeben.

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

## Einzelbildnavigation und Frameanzeige

Im pausierten Zustand stehen vier Schaltflächen für die Navigation zur Verfügung:

- `−10 Frames`
- `−1 Frame`
- `+1 Frame`
- `+10 Frames`

Die Vorwärtsnavigation verwendet die Frame-Step-Funktion von mpv. Für die Rückwärtsnavigation wird die Zielposition über einen relativen Seek-Befehl angesteuert.

Unterhalb der Zeitanzeige zeigt die Oberfläche die aktuelle Frame-Nummer und die von mpv geschätzte Gesamtzahl der Frames an. Die aktuelle Frame-Nummer wird aus Wiedergabeposition, Gesamtdauer und geschätzter Frameanzahl berechnet und auf den gültigen Bereich begrenzt.

Beispiel:

```text
Frame 89 / ca. 121.211
```

Solange noch keine ausreichenden Werte vorliegen, erscheint ein Gedankenstrich als Ersatzanzeige.

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

Auch die Wiedergabesteuerung wurde praktisch geprüft:

- Play und Pause funktionieren zuverlässig
- aktuelle Position und Gesamtdauer werden korrekt angezeigt
- die Zeitleiste läuft während der Wiedergabe automatisch mit
- Seeking funktioniert während laufender und pausierter Wiedergabe
- während des manuellen Ziehens springt der Slider nicht zur Player-Position zurück
- der Ablauf `Pause → Seeking → Play → Pause → Play` funktioniert stabil
- die Frame-Schaltflächen sind während der Wiedergabe deaktiviert
- im pausierten Zustand funktionieren Schritte um `−10`, `−1`, `+1` und `+10` Frames
- die Frameanzeige ändert sich bei jedem Schritt um die erwartete Anzahl
- die aktuelle Frame-Nummer läuft während der normalen Wiedergabe automatisch mit

Nach dem normalen Schließen der Anwendung wurde der Prozess vollständig beendet. Damit ist die vollständige Kette von der Dateiauswahl über ffprobe bis zur eingebetteten Bild- und Tonwiedergabe einschließlich Play/Pause, Seeking, Einzelbildnavigation und Frameanzeige praktisch nachgewiesen.

## Tests

Parser, Runner, `MainWindowViewModel`, `PlaybackViewModel` und MediaPlayer-Service sind durch automatisierte xUnit-Tests abgesichert.

Die Tests prüfen unter anderem:

- Übernahme und Formatierung erfolgreicher Analyseergebnisse
- verständliche Fehleranzeige
- `IsAnalyzing` und `CanAnalyze` während einer laufenden Analyse
- Ersatzanzeige `Nicht verfügbar` bei fehlenden Werten
- Initialisierung und Zustandswechsel des MediaPlayer-Service
- Laden, Wiedergabe, Pause, Seeking und Stoppen
- Verarbeitung von libmpv-Ereignissen und Fehlern
- Play-/Pause-Zustände bei unterschiedlichen libmpv-Ereignisreihenfolgen
- Positions-, Dauer- und Slider-Verhalten des `PlaybackViewModel`
- Seeking-Vorschau, Begrenzung und Abbruch
- Freigabe der Frame-Schaltflächen nur im pausierten Zustand
- Vorwärts- und Rückwärtsschritte um ein und zehn Frames
- Berechnung, Begrenzung und Formatierung der Frameanzeige
- stabile Beibehaltung des Pausenzustands nach Frame-Schritten

```text
Tests insgesamt:  48
Erfolgreich:      48
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
- testbares `PlaybackViewModel` für Player-Zustand, Position und Dauer
- Bedienelemente für Play und Pause
- Positionsanzeige und automatisch mitlaufende Zeitleiste
- Seeking während laufender und pausierter Wiedergabe
- Schutz vor zurückspringendem Slider während des manuellen Seeking
- Einzelbildnavigation um `−10`, `−1`, `+1` und `+10` Frames
- Frame-Schaltflächen nur im pausierten Zustand
- Anzeige der aktuellen Frame-Nummer und der geschätzten Gesamtzahl
- automatische Aktualisierung der Frameanzeige während der Wiedergabe
- automatisierte Tests für Parser, Runner, ViewModels und MediaPlayer-Service
- erfolgreicher Praxistest mit realen MP4-Dateien in der WPF-Anwendung

Als Nächstes geplant:

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

Die eingebettete libmpv-Wiedergabe mit Bild, Ton, eigener HWND-Einbettung und kontrolliertem Shutdown wurde mit Pull Request **#6** integriert.

Play, Pause, Positionsanzeige, Gesamtdauer und Zeitleiste mit Seeking wurden mit Pull Request **#7** in `main` übernommen.

Die zugehörige README-Dokumentation wurde mit Pull Request **#8** aktualisiert.

Die Einzelbildnavigation um `−10`, `−1`, `+1` und `+10` Frames sowie die aktuelle Frameanzeige wurden mit Pull Request **#9** in `main` übernommen.

Aktueller Merge-Commit: `18222b4`.

Der Proof of Concept ist damit ein funktionsfähiger eingebetteter MP4-Player mit ffprobe-Medienanalyse, Play/Pause-Steuerung, präzisem Seeking, Einzelbildnavigation und aktueller Frameanzeige.

Die nächsten Entwicklungsschritte sind die Lautstärkeregelung und eine verständliche Protokolldatei.

## Arbeitsgrundsatz

> Eine Funktion planen, umsetzen, testen, dokumentieren und erst danach den nächsten Schritt beginnen.
