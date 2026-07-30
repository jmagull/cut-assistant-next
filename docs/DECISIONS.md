# Architekturentscheidungen

## ADR-001 – Neuaufbau statt Delphi-Modernisierung

**Status:** entschieden

Der neue Cut Assistant wird eigenständig in C#/.NET aufgebaut. Der alte Delphi-Code dient als Referenz, wird aber nicht als technische Basis fortgeführt.

## ADR-002 – WPF für den ersten Windows-POC

**Status:** entschieden

Der erste POC verwendet WPF und zielt auf Windows 11 x64.

## ADR-003 – Kein DirectShow

**Status:** entschieden

DirectShow, DSPack und installierte Windows-Codecs werden nicht verwendet.

## ADR-004 – mpv/libmpv für Wiedergabe

**Status:** technischer Spike in Vorbereitung; im POC praktisch zu bestätigen

Für den ersten Wiedergabe-POC wird libmpv untersucht.

Vorgesehene technische Richtung:

- WPF-Einbettung zunächst über einen eigenen `HwndHost` und die mpv-Option `wid`
- `HanumanInstitute.LibMpv` als erster Kandidat für die .NET-Anbindung
- vollständige Kapselung hinter eigenen Schnittstellen
- `IMediaPlayerService` als testbare Schnittstelle für die Anwendung
- separate interne Kapselung der konkreten libmpv-Aufrufe
- Bereitstellung einer fest versionierten `libmpv-2.dll` für Windows x64
- keine direkte Abhängigkeit des ViewModels von WPF-, mpv- oder nativen Typen

Die libmpv-Render-API bleibt eine mögliche spätere Alternative, falls die HWND-Einbettung wegen WPF-Airspace, Overlays oder anderer Einschränkungen nicht ausreicht.

Der POC muss mindestens Einbettung, Laden, Wiedergabe, Pause, Seeking und Frame-Schritte mit realen OTR-Dateien nachweisen. Erst danach wird die technische Richtung endgültig entschieden.

## ADR-005 – ffprobe für Medienanalyse

**Status:** entschieden

Metadaten werden über ffprobe strukturiert ausgelesen und anhand anonymisierter JSON-Testdaten testbar gemacht.
