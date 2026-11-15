# Cut Assistant Next 1.0.3 Build 60

[Setup für Windows x64](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build60/CutAssistantNext-1.0.3-Build60-win-x64-Setup.exe) · [Portable-ZIP](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build60/CutAssistantNext-1.0.3-Build60-win-x64.zip) · [Vollständige Quellen](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build60/CutAssistantNext-1.0.3-Build60-source.zip) · [SHA-256-Prüfsummen](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build60/SHA256SUMS.txt) · [Nutzeranleitung](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build60/NUTZERANLEITUNG.md) · [Paket- und Testbericht](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build60/PAKETBERICHT.md)

## Hinweis zu FFmpeg

Für Cut Assistant Next 1.0.3 Build 60 empfehlen wir vorerst den erfolgreich geprüften **FFmpeg-Git-Build vom 12.10.2026 (Gyan.dev, `615b07170e`)**. FFmpeg und ffprobe müssen aus demselben Paket stammen.

Der Gyan.dev-Entwicklungsbuild vom **11.11.2026 (`23c4540053`)** kann beim otr-can-Videokopieren durch ein geändertes Seek-Verhalten unvollständige Segmente erzeugen. otr-can erkennt das und bricht den Schnitt ab. Für diesen Schnittweg bitte den empfohlenen Build verwenden.

Das Seek-Verhalten wurde im FFmpeg-Entwicklungszweig korrigiert. Ein Build eines anderen Anbieters vom **14.11.2026 (`604ef3784d`)** bestand den Praxistest mit dem vorhandenen otr-can-Motor. Ein aktualisierter Gyan.dev-Build kann nach erfolgreicher praktischer Prüfung als neue Referenz dienen.

[Download und Versionsinformationen bei Gyan.dev](https://www.gyan.dev/ffmpeg/builds/)

## Kurze Behaltebereiche am Videoende

- CAN prüft zeitbasierte Cutlists bei fehlender oder ungültiger Bildrate anhand der geladenen Aufnahme. Verlässliche Bildzeitstempel haben Vorrang. Der direkte Import und die lokale/Server-Cutlist-Tabelle nutzen dieselbe fachliche Prüfung.
- Bei einem auffälligen kurzen Endbereich zeigt CAN die Zeiten und, wenn gemessen, die Anzahl der Bildanfänge. Gültige Bereiche lassen sich unverändert laden. Entfernt wird der letzte von mehreren Bereichen erst nach Bestätigung. Bereiche hinter dem Videoende werden abgewiesen.
- Beim direkten MP4Box-Schnitt kann CAN einen auffälligen Endbereich vorab prüfen. Fehlt trotz Exitcode 0 eine Segmentdatei, nennt CAN Segmentnummer, Quellbereich und die verwendeten Werkzeuggrenzen. Die Keyframe-Grenzen von MP4Box gelten weiterhin.

## Usability aus Build 59

- **Sekundensprünge:** Frei einstellbare Sprungweite. `S` halten und `←` oder `→` drücken. Ein kurzer Druck springt einmal. Beim Halten folgen weitere Sprünge nach etwa 350 ms und danach etwa alle 200 ms. Die vorhandenen Sprungbuttons wiederholen bei gehaltener Maustaste genauso. Texteingaben, Einzelbildnavigation und `Strg + ←/→` für Keyframes behalten ihre Bedienung.
- **Schnittshuffle:** Der aktive rote oder grüne Bereich bleibt nach einer Korrektur ausgewählt. Für einen neuen Bereich zuerst die Auswahl aufheben.
- **Lautstärke:** Das Hauptfenster regelt auch den Kontrollplayer. Beim Öffnen und bei späteren Änderungen übernimmt der Kontrollplayer den zentralen Wert.
- **Tooltips:** Belegte Schaltflächen zeigen das jeweilige Tastenkürzel zusätzlich zur Funktionsbeschreibung.

**Aus Build 58 übernommen:** Spotlight-CAN mit „Schnitt bis Videoende abschließen“, Rot-/Grün-Schnittmodus, gemeinsame Suche nach lokalen und Server-Cutlists, Cutlist-Bewertung, Bild- und Schnittvorschau sowie Kontrollplayer. otr-can behält die Audioprüfung, Tonspur-Abwahl und die ausdrücklich zu bestätigende experimentelle AAC-Zeitlückenreparatur. MP4Box fügt Segmente mit gleichartigen AAC-Tonspuren weiterhin über `-add`, `-cat` und `-new` zusammen. Keyframe-bedingte Werbeblitzer können bei MP4Box weiterhin auftreten.

## Installation

Windows 11 x64 und .NET 10 Desktop Runtime x64 vorausgesetzt. Für otr-can wird außerdem Microsoft Visual C++ v14 Redistributable x64 benötigt. Das Setup kann über eine vorhandene Installation installiert werden. Für die portable Ausgabe das ZIP vollständig in ein frisches Verzeichnis entpacken und `CutAssistantNext.App.exe` starten.

Enthalten sind libmpv 0.41.0 und otr-can 0.1.6-audio-diag.1 unter `tools/otr-can/otr_can.exe`. FFmpeg, ffprobe, MP4Box und FFMS2 bleiben externe Werkzeuge. Bezugsquellen und Pfadeinstellungen stehen in `VORAUSSETZUNGEN.txt`. Die vollständigen Quellen, das libmpv-Relink-Archiv und SHA-256-Prüfsummen werden separat bereitgestellt.

## Prüfbasis

Die persönliche Abnahme unter Windows 11 umfasste das Setup auf dem Arbeitsrechner sowie Setup und frisch entpacktes Portable-ZIP in der VM. Bestätigt sind Schnitte mit MP4Box und otr-can, der AVI-Test „Teufelsbrigade“, die Cutlist-Bewertung, der Kontrollplayer mit zentraler Lautstärke und die Sekundensprünge. Bei der Deinstallation in der VM blieben die Benutzereinstellungen erhalten. Die otr-can-Werkzeugpfade mussten anschließend erneut gesetzt werden. Für Windows 10 liegt keine persönliche Prüfung von Build 60 vor.

Der erneut geprüfte Release-Quellstand bestand **2.114 CAN-Tests** und **78 Spotlight-Tests**. Das vollständige Quellarchiv enthält auch die angehefteten Spotlight-Quellen.
