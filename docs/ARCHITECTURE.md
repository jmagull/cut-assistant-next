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

## Offene Entscheidungen

- konkrete libmpv-.NET-Anbindung oder eigener schlanker Wrapper
- Verteilung der nativen mpv-Dateien
- Logging-Bibliothek
- MVVM-Hilfsbibliothek oder möglichst wenige externe Pakete
- Installer-/Portable-Konzept
