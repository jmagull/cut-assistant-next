# Projektstatus

## Aktueller Stand

Stand: 11.08.2026

- GitHub-Repository `cut-assistant-next` ist angelegt und derzeit privat.
- Der aktuelle Arbeitsbranch ist `feature/cutlist-naming-metadata`.
- `main` und `origin/main` sind synchron und stehen auf Commit `4754462`.
- Die Repository-Grundstruktur und `AGENTS.md` sind vorhanden.
- Das Projekt verwendet C#, .NET 10 und WPF.
- ffprobe ist über eine eigene Schnittstelle eingebunden.
- MP4-Dateien sowie OTR-Dateien mit tatsächlichem MP4-Inhalt und Dateiendung `.avi` können ausgewählt und analysiert werden.
- Container-, Video- und Audiodaten werden in der WPF-Oberfläche angezeigt.
- mpv/libmpv ist als eingebetteter MediaPlayer integriert.
- Play/Pause, Seeking und Einzelbildnavigation um `−10`, `−1`, `+1` und `+10` Frames sind umgesetzt.
- Positionsanzeige, geschätzte Framezahl, Zeitleiste und Lautstärkeregelung sind umgesetzt.
- Eine verständliche UTF-8-Protokolldatei mit Größenbegrenzung und Rotation ist umgesetzt.
- Fenstergröße und maximierter Zustand werden lokal gespeichert und wiederhergestellt.
- Manuelle Schnittplanung mit Remove-Bereichen ist umgesetzt.
- Schnittbereiche können gesetzt, ausgewählt, korrigiert und gelöscht werden.
- Eine eigene Schnitt-Timeline visualisiert die markierten Bereiche.
- Die technische Grundlage zur Erzeugung klassischer Cutlists ist umgesetzt.
- Konfigurierbare Namensmasken, Standardautor und Schnelltexte sind als Cutlist-Einstellungen vorbereitet.
- Automatische technische Hinweise können aus der Medienanalyse abgeleitet werden, unter anderem bei einer `.avi`-Dateiendung und tatsächlich erkanntem MP4/ISO-BMFF-Container.
- Remove-Bereiche werden für die Cutlist-Ausgabe in komplementäre Keep-Bereiche umgerechnet.
- `NoOfCuts` wird aus der tatsächlichen Anzahl der Keep-Bereiche abgeleitet.
- Cutlist-Metadaten für `[General]` und `[Info]` sowie ein typisiertes `CutlistDocument` sind umgesetzt.
- Der Cutlist-Serializer erzeugt klassische `[General]`-, `[CutN]`- und `[Info]`-Bereiche.
- Die klassischen Kompatibilitätsfelder `comment1` und `comment2` werden ausgegeben.
- Zeit- und Zahlenwerte werden kulturunabhängig mit Punkt als Dezimaltrenner geschrieben.
- Der `CutlistFileWriter` schreibt `.cutlist`-Dateien als UTF-8 ohne BOM mit CRLF-Zeilenenden.
- Ein vollständiger Smoke-Test hat eine reale `.cutlist`-Datei erfolgreich erzeugt.
- Umlaute wurden in der erzeugten Datei explizit als UTF-8 erfolgreich geprüft.
- Die erzeugte Struktur wurde mit eigenen und fremden historischen HD-/HQ-Cutlists verschiedener Programme abgeglichen.
- Historische Null- oder Negativsegmente werden bewusst nicht nachgebildet.
- `StartFrame` und `DurationFrames` bleiben optionale spätere Kompatibilitätsfelder.
- Aktueller vollständiger Solution-Testlauf: **162 von 162 Tests erfolgreich**.
- Der aktuelle Feature-Stand ist noch nicht in `main` übernommen.

## Vorhandene Projekte

- `CutAssistantNext.App`
- `CutAssistantNext.Core`
- `CutAssistantNext.Media`
- `CutAssistantNext.Cutlists`
- `CutAssistantNext.App.Tests`
- `CutAssistantNext.Core.Tests`
- `CutAssistantNext.Media.Tests`
- `CutAssistantNext.Cutlists.Tests`

## Aktueller Bauabschnitt

Cutlist-Namensbildung, Metadaten und lokale Cutlist-Erzeugung:

- allgemeine, testbare Namensbildung in `CutAssistantNext.Core.Naming`
- frei konfigurierbare Namensmaske
- optionale Präfixsyntax wie `%Staffel:S%` und `%Folge:E%`
- fehlende bekannte Werte werden tolerant behandelt
- unbekannte Variablen werden als Fehler erkannt
- derselbe erzeugte Basisname soll für `SuggestedMovieName` und später für den Ausgabedateinamen verwendet werden
- persistente Cutlist-Einstellungen für Namensmaske, Standardautor und Schnelltexte
- technische Hinweise getrennt von frei editierbaren Benutzerkommentaren
- automatische Erkennung des Sonderfalls `.avi` mit tatsächlich erkanntem MP4/ISO-BMFF-Container
- Umrechnung von `CutPlan`-Remove-Bereichen in positive Keep-Bereiche
- typisierte Metadatenmodelle für `[General]` und `[Info]`
- abstrakte Beschreibung des vorgesehenen Schnittprogramms über `CutApplicationInfo`
- kein fest codierter lokaler Installationspfad eines Schnittmotors in den Cutlist-Metadaten
- `CutlistDocument` als typisiertes Gesamtmodell
- `CutlistSerializer` unabhängig vom Dateisystem
- `CutlistFileWriter` für lokale `.cutlist`-Dateien
- UTF-8 ohne BOM und CRLF als Ausgabeformat
- Golden-Master-Test für das vollständige erzeugte Cutlist-Format
- Abgleich gegen reale historische Bestands-Cutlists mit 25 und 50 fps
- optionale Framefelder und Legacy-Encoding als dokumentierte spätere Kompatibilitätsthemen
- ADR-007 dokumentiert die Regeln für kompatible Cutlist-Erzeugung

## Schnittsemantik

Die Benutzeroberfläche beschreibt Bereiche, die entfernt werden sollen.

`RemoveSegment` und `CutPlan` bilden diese Semantik in `CutAssistantNext.Core` ab. Für klassische Cutlists erzeugt `CutlistKeepSegmentBuilder` daraus die komplementären Behaltebereiche.

Das Bedienmodell bleibt damit auf das Entfernen von Werbung, Vorlauf, Nachlauf oder anderen unerwünschten Abschnitten ausgerichtet, während das klassische Cutlist-Format weiterhin seine Keep-Segmente erhält.

## Nächster geplanter Bauabschnitt

Der aktuelle Bauabschnitt wird zunächst vollständig dokumentiert, geprüft und anschließend versioniert.

Danach stehen insbesondere die Integration der Cutlist-Erzeugung in die WPF-Oberfläche und die Vorbereitung der späteren Anbindung eines tatsächlichen Schnittmotors an.

## Noch nicht umgesetzt

- Cutlist-Reader/Parser
- Unterstützung historischer Windows-1252-/ANSI-Cutlists beim Einlesen
- Cutlist-Server und Upload
- WPF-Dialog zum Erzeugen und Bearbeiten der Cutlist-Metadaten
- MP4Box-Schnitt
- FFmpeg-Schnitt
- Smart Rendering
- Renamer
- Stapelverarbeitung