# Architektur – Arbeitsstand

## Zielbild

```text
WPF-Oberfläche
      |
Anwendungslogik / ViewModels
      |
+----------------------+----------------------+
| Medienwiedergabe     | Medienanalyse        |
| mpv/libmpv           | ffprobe              |
+----------------------+----------------------+
      |
Später: Cutlists | Schnittmotoren | Server | Renamer | Stapel
```

## Vorgesehene Projekte

- `CutAssistantNext.App` – WPF-Oberfläche und Startpunkt
- `CutAssistantNext.Core` – Modelle, Verträge und Anwendungslogik
- `CutAssistantNext.Media` – mpv/libmpv und ffprobe
- `CutAssistantNext.Cutlists` – späterer Cutlist-Parser
- passende Testprojekte unter `tests/`

## Architekturregeln

- Die Oberfläche kennt keine DirectShow- oder ffprobe-Details.
- Externe Prozesse und native Bibliotheken werden hinter Schnittstellen gekapselt.
- Fachlogik bleibt unabhängig von WPF testbar.
- Pfade und Laufzeitabhängigkeiten werden nicht fest codiert.
- Fehler werden in verständliche Anwendungsmeldungen und technische Logs getrennt.

## Wiedergabe-POC – technische Richtung

Für den libmpv-Spike ist folgende Aufteilung vorgesehen:

```text
CutAssistantNext.App
├── MainWindow und ViewModel
└── MpvVideoHost auf Basis von HwndHost
          |
          | stellt ein Windows-HWND bereit
          v
CutAssistantNext.Media
├── IMediaPlayerService
├── MpvMediaPlayerService
└── interne Kapselung der libmpv-Aufrufe
          |
          v
libmpv-2.dll
```

- Der WPF-spezifische Video-Host verbleibt in `CutAssistantNext.App`.
- Player-Zustand und Wiedergabesteuerung werden hinter `IMediaPlayerService` gekapselt.
- ViewModels kennen weder WPF-Fensterklassen noch mpv- oder native Typen.
- `HanumanInstitute.LibMpv` wird als erster .NET-Wrapper im Spike geprüft.
- Die native `libmpv-2.dll` wird für Windows x64 fest versioniert und reproduzierbar bereitgestellt.
- Die Einbettung erfolgt zunächst über die mpv-Option `wid`.
- Die libmpv-Render-API bleibt eine spätere Alternative.

## Offene Entscheidungen

- Bestätigung von `HanumanInstitute.LibMpv` oder Wechsel auf einen eigenen schlanken Wrapper
- genaue Quelle, Version, Prüfsumme und Lizenzvariante von `libmpv-2.dll`
- Lebenszyklus, Ereignisschleife und Threading des MediaPlayer-Service
- Logging-Bibliothek
- MVVM-Hilfsbibliothek oder möglichst wenige externe Pakete
- Installer-/Portable-Konzept
