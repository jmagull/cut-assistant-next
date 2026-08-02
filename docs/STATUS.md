# Projektstatus

## Aktueller Stand

Stand: 02.08.2026

- GitHub-Repository `cut-assistant-next` ist angelegt und derzeit privat.
- Der Branch `main` ist sauber und mit `origin/main` synchronisiert.
- Die Repository-Grundstruktur und `AGENTS.md` sind vorhanden.
- Das Projekt verwendet C#, .NET 10 und WPF.
- Das .NET 10 SDK 10.0.302 ist installiert.
- ffprobe ist über eine eigene Schnittstelle eingebunden.
- MP4-Dateien können ausgewählt und analysiert werden.
- Container-, Video- und Audiodaten werden in der WPF-Oberfläche angezeigt.
- mpv/libmpv ist als eingebetteter MediaPlayer integriert.
- Play, Pause, Positionsanzeige und Zeitleiste mit Seeking sind umgesetzt.
- Einzelbildnavigation um `−10`, `−1`, `+1` und `+10` Frames ist umgesetzt.
- Die aktuelle Frame-Nummer und die geschätzte Gesamtzahl der Frames werden angezeigt.
- Eine Lautstärkeregelung von `0` bis `100 Prozent` ist umgesetzt.
- Die gewählte Lautstärke bleibt beim Laden einer anderen Datei erhalten.
- Release-Build und automatisierte Tests sind erfolgreich.
- Insgesamt sind 57 von 57 automatisierten Tests erfolgreich.
- Die vollständige Wiedergabesteuerung wurde unter Windows praktisch geprüft.
- Pull Request #11 wurde in `main` übernommen.
- Aktueller Merge-Commit von `main`: `57a071a`.
- Es bestehen derzeit keine lokalen Änderungen.

## Vorhandene Projekte

- `CutAssistantNext.App`
- `CutAssistantNext.Core`
- `CutAssistantNext.Media`
- `CutAssistantNext.App.Tests`
- `CutAssistantNext.Core.Tests`
- `CutAssistantNext.Media.Tests`

## Letzter abgeschlossener Bauabschnitt

Lautstärkeregelung für den eingebetteten mpv-Player:

- Lautstärkeregler von `0` bis `100 Prozent`
- Prozentanzeige in der WPF-Oberfläche
- Steuerung über `PlaybackViewModel`
- Übergabe an die mpv-Eigenschaft `volume`
- Begrenzung ungültiger Werte auf den erlaubten Bereich
- Aktivierung nur in geeigneten Player-Zuständen
- Beibehaltung der gewählten Lautstärke beim Dateiwechsel
- automatisierte Tests für Service, ViewModel und Ereignisbehandlung
- manueller Praxistest während Wiedergabe und Pause
- Release-Build mit 0 Fehlern
- 57 automatisierte Tests erfolgreich
- Übernahme mit Pull Request #11

## Nächster geplanter Bauabschnitt

**Verständliche Protokolldatei**

Geplante Themen:

- zentrale Protokollierung wichtiger Programmabläufe
- verständliche Meldungen für Start, Dateiauswahl, Analyse und Wiedergabe
- nachvollziehbare Fehlerprotokollierung
- geeigneter Speicherort für die Protokolldatei
- Begrenzung beziehungsweise Rotation älterer Protokolle
- testbare Kapselung hinter einer klaren Schnittstelle
- Dokumentation und manueller Praxistest

## Noch nicht umgesetzt

- verständliche Protokolldatei
- Cutlists
- Cutlist-Server
- MP4Box-Schnitt
- FFmpeg-Schnitt
- Renamer
- Stapelverarbeitung
