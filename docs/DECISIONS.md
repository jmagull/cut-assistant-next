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

**Status:** entschieden

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

Play/Pause, Seeking, Stoppen und Einzelbildschritte um `−10`, `−1`, `+1` und `+10` Frames sind über den testbaren Service abgebildet und praktisch über die WPF-Oberfläche bestätigt. Auch die Tastatursteuerung, der zuverlässige Start geladener Medien im pausierten Zustand und der kontrollierte Shutdown wurden erfolgreich geprüft.

Die libmpv-Render-API bleibt eine mögliche spätere Alternative, falls die HWND-Einbettung wegen WPF-Airspace, Overlays oder anderer Einschränkungen nicht ausreicht.

## ADR-005 – ffprobe für Medienanalyse

**Status:** entschieden

Metadaten werden über ffprobe strukturiert ausgelesen und anhand anonymisierter JSON-Testdaten testbar gemacht.

## ADR-006 – Schnittbearbeitung verwendet Remove-Bereiche

**Status:** entschieden

Die Benutzeroberfläche beschreibt Schnittbereiche als Abschnitte, die aus dem Video entfernt werden sollen.

Begründung:

- Beim manuellen Schneiden markiert der Benutzer typischerweise Werbung, Vorlauf, Nachlauf oder Wiederholungen, die entfernt werden sollen.
- Dieses Bedienmodell ist intuitiver als das direkte Erfassen aller Behaltebereiche.
- `RemoveSegment` und `CutPlan` bilden diese Semantik in der testbaren Core-Schicht ab.
- Die klassische Cutlist-Spezifikation beschreibt dagegen die Teile, die im fertigen Film erhalten bleiben.
- Die Cutlist-Ausgabe berechnet deshalb aus den Remove-Bereichen die komplementären Keep-Bereiche.
- Die unterschiedliche Semantik von Benutzeroberfläche und Cutlist-Dateiformat ist damit beabsichtigt und keine Umkehrung durch die Oberfläche.

## ADR-007 – Kompatible Cutlist-Erzeugung auf Basis realer Bestandsdateien

**Status:** entschieden

Die von Cut Assistant Next erzeugten Cutlists orientieren sich nicht nur an einer theoretischen Spezifikation, sondern zusätzlich an realen eigenen und fremden Bestands-Cutlists verschiedener Programme und Jahrgänge.

Begründung und Regeln:

- Das interne Schnittmodell der Benutzeroberfläche verwendet weiterhin Remove-Bereiche gemäß ADR-006.
- Für die Cutlist-Ausgabe werden daraus ausschließlich positive Keep-Bereiche mit `Start` und `Duration` in Sekunden erzeugt.
- `NoOfCuts` entspricht der tatsächlichen Anzahl dieser Keep-Bereiche.
- Bereiche mit einer Dauer von `0` oder einer negativen Dauer werden von Cut Assistant Next nicht erzeugt. Solche Werte wurden zwar in historischen Bestandsdateien gefunden, werden aber als Artefakte älterer Werkzeuge betrachtet und nicht nachgebildet.
- `StartFrame` und `DurationFrames` sind optionale Kompatibilitätsfelder. Sie werden nicht zwingend ausgegeben und erst ergänzt, wenn ein Schnittmotor oder ein konkreter Kompatibilitätsfall sie benötigt.
- Zeit- und Zahlenwerte werden kulturunabhängig mit einem Punkt als Dezimaltrenner geschrieben.
- Der Writer speichert neu erzeugte Cutlists als UTF-8 ohne BOM mit CRLF-Zeilenenden.
- Ein späterer Cutlist-Reader soll zusätzlich historische Windows-1252-/ANSI-Dateien lesen können, da solche Bestandsdateien nachgewiesen wurden.
- Die klassischen Kompatibilitätszeilen werden in der historisch verbreiteten Form geschrieben:
  - `comment1=The following parts of the movie will be kept, the rest will be cut out.`
  - `comment2=All values are given in seconds.`
- Lokal erzeugte Cutlists enthalten keine künstlich erzeugten `[Meta]`- oder `[Server]`-Informationen. Solche Bereiche gehören zu einem späteren Server- bzw. Upload-Kontext.
- Die Reihenfolge von Bereichen in vorhandenen Cutlists ist historisch nicht einheitlich. Der eigene Serializer erzeugt deshalb eine stabile definierte Reihenfolge, ein späterer Reader darf sich jedoch nicht auf eine bestimmte Abschnittsreihenfolge verlassen.
- Optionale Angaben wie `DisplayAspectRatio` dürfen ausgegeben werden, wenn sie aus der Medienanalyse zuverlässig vorliegen.

Die Kompatibilitätsregeln werden durch automatisierte Serializer-, Datei- und Golden-Master-Tests abgesichert.

## ADR-008 – Namensbildung und Cutlist-Metadaten bleiben von UI und Schnittmotor getrennt

**Status:** entschieden

Die Bildung von Filmnamen sowie die Verwaltung von Cutlist-Metadaten werden unabhängig von der WPF-Oberfläche und von einem konkreten Schnittmotor umgesetzt.

Begründung und Regeln:

- Die allgemeine Namensbildung liegt in `CutAssistantNext.Core.Naming` und kann unabhängig von WPF, Cutlist-Dateiausgabe und Schnittmotor verwendet und getestet werden.
- Die Namensmaske ist konfigurierbar und nicht fest im Programmcode vorgegeben.
- Staffel und Folge werden als Textwerte behandelt und nicht auf numerische Werte beschränkt. Dadurch bleiben auch abweichende Benennungsschemata wie beispielsweise `S2026E01` möglich.
- Optionale Präfixe können direkt in der Namensmaske angegeben werden, beispielsweise `%Staffel:S%` oder `%Folge:E%`.
- Fehlt der Wert einer bekannten Variablen, wird dieser Teil der Namensmaske tolerant weggelassen. Unbekannte Variablen werden dagegen als Fehler erkannt.
- Derselbe erzeugte Basisname soll sowohl für `SuggestedMovieName` in der Cutlist als auch später für den Namen der erzeugten Mediendatei verwendet werden.
- Namensmaske, Standardautor und Schnelltexte sind Benutzereinstellungen. Sie gehören nicht als fest codierte Werte in die Cutlist-Erzeugung.
- Der Standardautor dient als Vorgabe für neue Cutlists. Der konkrete `Author` gehört dagegen zu den Metadaten der jeweils erzeugten Cutlist.
- Schnelltexte sind vom Benutzer auswählbare bzw. editierbare Kommentare. Automatisch erkannte technische Hinweise werden davon getrennt behandelt und bei Bedarf auf technische Fehlerfelder der Cutlist abgebildet.
- Die Identität eines vorgesehenen Schnittprogramms wird über `CutApplicationInfo` beschrieben. Dazu gehören Name, ausführbare Datei, Version und Optionen.
- Ein lokaler Installationspfad eines Schnittprogramms ist eine Anwendungseinstellung und wird nicht in den Cutlist-Metadaten gespeichert.
- Die Architektur legt keinen bestimmten Schnittmotor fest. MP4Box, FFmpeg oder spätere Smart-Rendering-Verfahren können auf derselben Cutlist- und Namensgrundlage aufbauen.

Damit bleiben Namensbildung, Benutzereinstellungen, Cutlist-Dateiformat und tatsächliche Medienverarbeitung voneinander getrennt und können unabhängig weiterentwickelt werden.
