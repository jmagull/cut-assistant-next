# Cut Assistant Next 1.0.3 Build 59

**Neu gegenüber Build 58**

- **Sekundensprünge:** Frei einstellbare Sprungweite. `S` halten und `←` oder `→` drücken. Ein kurzer Druck springt einmal. Beim Halten folgen weitere Sprünge nach etwa 350 ms und danach etwa alle 200 ms. Die vorhandenen Sprungbuttons wiederholen bei gehaltener Maustaste genauso. Texteingaben, Einzelbildnavigation und `Strg + ←/→` für Keyframes behalten ihre Bedienung.
- **Schnittshuffle:** Der aktive rote oder grüne Bereich bleibt nach einer Korrektur ausgewählt. Für einen neuen Bereich zuerst die Auswahl aufheben.
- **Lautstärke:** Das Hauptfenster regelt auch den Kontrollplayer. Beim Öffnen und bei späteren Änderungen übernimmt der Kontrollplayer den zentralen Wert.
- **Tooltips:** Belegte Schaltflächen zeigen das jeweilige Tastenkürzel zusätzlich zur Funktionsbeschreibung.

**Aus Build 58 übernommen:** Spotlight-CAN mit „Schnitt bis Videoende abschließen“, Rot-/Grün-Schnittmodus, gemeinsame Suche nach lokalen und Server-Cutlists, Cutlist-Bewertung, Bild- und Schnittvorschau sowie Kontrollplayer. otr-can behält die Audioprüfung, Tonspur-Abwahl und die ausdrücklich zu bestätigende experimentelle AAC-Zeitlückenreparatur. MP4Box fügt Segmente mit gleichartigen AAC-Tonspuren weiterhin über `-add`, `-cat` und `-new` zusammen. Keyframe-bedingte Werbeblitzer können bei MP4Box weiterhin auftreten.

## Installation

Windows 11 x64 und .NET 10 Desktop Runtime x64 vorausgesetzt. Für otr-can wird außerdem Microsoft Visual C++ v14 Redistributable x64 benötigt. Das Setup aktualisiert die vorhandene Installation. Für die portable Ausgabe das ZIP vollständig in ein frisches Verzeichnis entpacken und `CutAssistantNext.App.exe` starten.

Enthalten sind libmpv 0.41.0 und otr-can 0.1.6-audio-diag.1 unter `tools/otr-can/otr_can.exe`. FFmpeg, ffprobe, MP4Box und FFMS2 bleiben externe Werkzeuge. Bezugsquellen und Pfadeinstellungen stehen in `VORAUSSETZUNGEN.txt`. Die vollständigen Quellen, das libmpv-Relink-Archiv und SHA-256-Prüfsummen werden separat bereitgestellt.

## Prüfbasis

Jörg hat Tastatur- und Mauswiederholung, die erhaltene Schnittauswahl, den zentralen Lautstärkeregler, die Lautstärkeübernahme und die Tooltips mit dem Usability-Entwicklungsstand persönlich unter Windows 11 geprüft. Die persönliche Prüfung der neuen Build-59-Pakete steht aus. Windows 10 wurde nicht persönlich geprüft.
