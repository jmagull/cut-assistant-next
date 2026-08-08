# Projektstatus

## Aktueller Stand

Stand: 08.08.2026

- GitHub-Repository `cut-assistant-next` ist angelegt und derzeit privat.
- Der aktuelle Arbeitsbranch ist `feature/cut-markers`.
- `main` ist mit `origin/main` synchronisiert und steht auf Commit `d0f3ed1`.
- Pull Request **#13** mit der verständlichen Protokolldatei ist in `main` übernommen.
- Die Repository-Grundstruktur und `AGENTS.md` sind vorhanden.
- Das Projekt verwendet C#, .NET 10 und WPF.
- Das .NET 10 SDK 10.0.302 ist installiert.
- ffprobe ist über eine eigene Schnittstelle eingebunden.
- MP4-Dateien sowie OTR-Dateien mit tatsächlichem MP4-Inhalt und Dateiendung `.avi` können ausgewählt und analysiert werden.
- Container-, Video- und Audiodaten werden in der WPF-Oberfläche angezeigt.
- mpv/libmpv ist als eingebetteter MediaPlayer integriert.
- Geladene Medien starten zuverlässig im pausierten Zustand.
- Play/Pause ist als gemeinsamer Umschaltvorgang umgesetzt.
- Positionsanzeige und Zeitleiste mit Seeking sind umgesetzt.
- Einzelbildnavigation um `−10`, `−1`, `+1` und `+10` Frames ist umgesetzt.
- Die aktuelle Frame-Nummer und die geschätzte Gesamtzahl der Frames werden angezeigt.
- Tastaturkürzel sind umgesetzt: Leertaste für Play/Pause, `←`/`→` für ein Bild und `Strg+←`/`Strg+→` für zehn Bilder.
- Eine Lautstärkeregelung von `0` bis `100 Prozent` ist umgesetzt.
- Die gewählte Lautstärke bleibt beim Laden einer anderen Datei erhalten.
- Eine verständliche UTF-8-Protokolldatei mit Größenbegrenzung und Rotation ist umgesetzt.
- Die zuletzt verwendete Fenstergröße sowie der maximierte Zustand werden lokal gespeichert und beim nächsten Start wiederhergestellt.
- Release-Build und automatisierte Tests sind erfolgreich.
- Insgesamt sind 107 von 107 automatisierten Tests erfolgreich.
- Wiedergabe, Navigation, Schnittmarkierung, Schnittkorrektur und Fensterwiederherstellung wurden unter Windows praktisch geprüft.
- Der aktuelle Feature-Stand ist im Arbeitsbranch vollständig umgesetzt, getestet und noch nicht in `main` übernommen.

## Vorhandene Projekte

- `CutAssistantNext.App`
- `CutAssistantNext.Core`
- `CutAssistantNext.Media`
- `CutAssistantNext.App.Tests`
- `CutAssistantNext.Core.Tests`
- `CutAssistantNext.Media.Tests`

## Aktueller Bauabschnitt

Schnittbereiche und Cut-Markers:

- fachliches Modell `RemoveSegment` für Bereiche, die aus dem Video entfernt werden sollen
- `CutPlan` mit Mediendauer, chronologischer Sortierung und Validierung
- Schutz vor ungültigen Bereichen und Überschneidungen
- angrenzende Schnittbereiche sind zulässig
- Hinzufügen, Entfernen und Ersetzen bestehender Schnittbereiche
- testbares `CutPlanViewModel`
- Setzen von Schnittanfang und Schnittende an der aktuellen Videoposition
- mehrere zu entfernende Bereiche können nacheinander erfasst werden
- neu angelegte Bereiche bleiben zunächst unausgewählt, damit unmittelbar der nächste Bereich erfasst werden kann
- vorhandene Bereiche können über Tabelle oder Schnitt-Timeline ausgewählt werden
- ausgewählte Bereiche werden in der Schnitt-Timeline deutlich hervorgehoben
- Korrekturmodus für ausgewählte Bereiche
- Anfang eines ausgewählten Bereichs kann auf die aktuelle Videoposition korrigiert werden
- Ende eines ausgewählten Bereichs kann auf die aktuelle Videoposition korrigiert werden
- ausgewählte Bereiche können vollständig gelöscht werden
- Erfassungsmodus und Korrekturmodus schließen sich gegenseitig aus
- eine bereits vorgemerkte neue Schnittmarke wird beim Wechsel in den Korrekturmodus verworfen
- verständliche Hinweise bei ungültiger Position für Anfangs- oder Endkorrektur
- visuelle Schnitt-Timeline mit proportional dargestellten roten Entfernungsbereichen
- bidirektionale Auswahl zwischen Tabelle und Schnitt-Timeline
- Tastatursteuerung für Play/Pause und framegenaue Navigation
- gemeinsame Play/Pause-Schaltfläche
- Speicherung der zuletzt verwendeten Fenstergröße unter `%LOCALAPPDATA%\Cut Assistant Next\Settings\window-settings.json`
- Dateiauswahl berücksichtigt auch OTR-Dateien mit Dateiendung `.avi`
- libmpv behält den angeforderten Pausenzustand auch bei `pause=no`-Ereignissen während des Ladens bei
- vollständiger Solution-Testlauf mit 107 erfolgreichen Tests
- `git diff --check` ohne Beanstandungen

## Schnittsemantik

Die Benutzeroberfläche beschreibt Bereiche, die entfernt werden sollen.

Die spätere Cutlist-Ausgabe wird daraus die komplementären Behaltebereiche berechnen. Damit bleibt die Bedienung für das manuelle Schneiden intuitiv, während das klassische Cutlist-Format weiterhin seine Keep-Segmente erhält.

## Nächster geplanter Bauabschnitt

Nach Abschluss, Commit und Übernahme von `feature/cut-markers` wird der nächste Bauabschnitt festgelegt.

Naheliegende Folgeschritte sind die Erzeugung klassischer Cutlists aus den markierten Entfernungsbereichen und anschließend die Anbindung eines tatsächlichen Schnittverfahrens.

## Noch nicht umgesetzt

- Schreiben und Einlesen vollständiger Cutlists
- Cutlist-Server
- MP4Box-Schnitt
- FFmpeg-Schnitt
- Smart Rendering
- Renamer
- Stapelverarbeitung