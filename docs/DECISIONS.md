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

**Status:** technischer Spike erfolgreich; für den POC vorläufig bestätigt

Der erste Wiedergabe-POC verwendet libmpv.

Umgesetzte technische Lösung:

- WPF-Einbettung über einen eigenen `HwndHost` und die mpv-Option `wid`
- `HanumanInstitute.LibMpv` 0.10.1 als .NET-Anbindung
- vollständige Kapselung hinter eigenen Schnittstellen
- `IMediaPlayerService` als testbare Schnittstelle für die Anwendung
- `MpvMediaPlayerService` für Zustände, Steuerung und Ereignisverarbeitung
- `HanumanLibMpvClient` als interne Kapselung der konkreten libmpv-Aufrufe
- reproduzierbare Bereitstellung einer fest versionierten `libmpv-2.dll` für Windows x64
- SHA-256-Prüfung des heruntergeladenen Archivs
- automatische Übernahme der nativen DLL in Build- und Publish-Ausgaben
- kontrollierte Freigabe von libmpv vor dem Zerstören des nativen Videofensters
- keine direkte Abhängigkeit des ViewModels von WPF-, mpv- oder nativen Typen

Einbettung, Laden und Wiedergabe wurden mit einer realen MP4-Datei praktisch bestätigt. Video und Ton wurden korrekt innerhalb der WPF-Anwendung wiedergegeben. Auch der vollständige Shutdown ohne zurückbleibenden Prozess wurde nachgewiesen.

Play, Pause, Seeking und Stoppen sind bereits über den testbaren Service abgebildet. Die praktische Bestätigung über Bedienelemente sowie Einzelbildschritte stehen noch aus. Nach diesen Tests wird die technische Entscheidung endgültig bestätigt.

Die libmpv-Render-API bleibt eine mögliche spätere Alternative, falls die HWND-Einbettung wegen WPF-Airspace, Overlays oder anderer Einschränkungen nicht ausreicht.

## ADR-005 – ffprobe für Medienanalyse

**Status:** entschieden

Metadaten werden über ffprobe strukturiert ausgelesen und anhand anonymisierter JSON-Testdaten testbar gemacht.
