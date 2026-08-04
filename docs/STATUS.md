# Projektstatus

## Aktueller Stand

Stand: 04.08.2026

- GitHub-Repository `cut-assistant-next` ist angelegt und derzeit privat.
- Der aktuelle Arbeitsbranch ist `feature/understandable-logging`.
- `main` ist mit `origin/main` synchronisiert und steht auf Commit `cd4e021`.
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
- Eine verständliche UTF-8-Protokolldatei mit Größenbegrenzung und Rotation ist umgesetzt.
- Release-Build und automatisierte Tests sind erfolgreich.
- Insgesamt sind 70 von 70 automatisierten Tests erfolgreich.
- Die vollständige Wiedergabesteuerung und Protokollierung wurden unter Windows praktisch geprüft.
- Der aktuelle Feature-Stand ist im Arbeitsbranch vollständig umgesetzt, getestet und noch nicht in `main` übernommen.

## Vorhandene Projekte

- `CutAssistantNext.App`
- `CutAssistantNext.Core`
- `CutAssistantNext.Media`
- `CutAssistantNext.App.Tests`
- `CutAssistantNext.Core.Tests`
- `CutAssistantNext.Media.Tests`

## Aktueller Bauabschnitt

Verständliche Protokolldatei:

- zentrale, testbare Logging-Schnittstelle `IAppLogger`
- stille Standardimplementierung `NullAppLogger`
- konkrete Dateiimplementierung `FileAppLogger`
- Speicherort `%LOCALAPPDATA%\Cut Assistant Next\Logs\CutAssistantNext.log`
- UTF-8-Kodierung ohne BOM
- Größenbegrenzung auf 2 MiB
- Rotation mit bis zu drei älteren Protokolldateien
- verständliche Einträge für Programmstart und Programmende
- Protokollierung der Dateiauswahl und Medienanalyse
- Protokollierung von libmpv-Initialisierung und Ladevorgängen
- ausgewählte Einträge für Play, Pause, Seeking und Stop
- Fehlerprotokollierung mit vollständigen technischen Details
- keine Erfolgsprotokolle für Lautstärke- und Einzelbildschritte
- keine Logflut durch häufige Positions-, Frame- oder Pause-Ereignisse
- störungsfreies Verhalten bei nicht beschreibbarem Protokollpfad
- manueller Praxistest mit realer MP4-Datei
- Release-Build mit 0 Fehlern
- 70 automatisierte Tests erfolgreich

## Nächster geplanter Bauabschnitt

Der nächste Bauabschnitt wird nach Abschluss, Commit und Übernahme der verständlichen Protokolldatei festgelegt.

## Noch nicht umgesetzt

- Cutlists
- Cutlist-Server
- MP4Box-Schnitt
- FFmpeg-Schnitt
- Renamer
- Stapelverarbeitung
