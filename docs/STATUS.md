# Projektstatus

## Aktueller Stand

Stand: 27.07.2026

- GitHub-Repository `cut-assistant-next` ist angelegt.
- Das Repository ist derzeit privat.
- Der Branch `main` ist sauber und mit GitHub synchronisiert.
- Die Repository-Grundstruktur und `AGENTS.md` sind vorhanden.
- Das .NET 10 SDK 10.0.302 ist installiert.
- Eine minimale WPF-Solution wurde angelegt.
- Release-Build und automatisierte Tests waren erfolgreich.
- Die WPF-Anwendung wurde unter Windows manuell gestartet.
- Der Feature-Branch `feature/initial-wpf-solution` wurde per Pull Request nach `main` übernommen.
- Es bestehen derzeit keine lokalen Änderungen.

## Vorhandene Projekte

- `CutAssistantNext.App`
- `CutAssistantNext.Core`
- `CutAssistantNext.Media`
- `CutAssistantNext.Core.Tests`

## Letzter abgeschlossener Bauabschnitt

Minimales .NET-10-WPF-Grundgerüst:

- WPF-Hauptfenster
- Core-Klassenbibliothek
- Media-Klassenbibliothek
- xUnit-Testprojekt
- Build mit 0 Warnungen und 0 Fehlern
- 1 automatisierter Test erfolgreich

## Nächster geplanter Bauabschnitt

**ffprobe und Medienanalyse**

Geplante Themen:

- Einbindung beziehungsweise Auffinden von ffprobe
- Medieninformationen als JSON auslesen
- Dauer, Auflösung, Bildrate und Codecs erfassen
- ffprobe-Aufruf hinter einer klaren Schnittstelle kapseln
- Parser mit anonymisierten Testdaten testen
- Medieninformationen zunächst ohne Videowiedergabe in der WPF-Oberfläche darstellen

## Noch nicht umgesetzt

- mpv/libmpv
- Videowiedergabe
- Frame-Stepping
- Cutlists
- MP4Box
- FFmpeg-Schnitt
- Cutlist-Server
- Renamer
- Stapelverarbeitung