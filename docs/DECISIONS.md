# Architekturentscheidungen

## ADR-016 – Geprüfte Seek-Optimierung als regulärer otr-can-0.1.2-Stand

**Status:** am 10.10.2026 nach ausdrücklicher Nutzerfreigabe übernommen.

- Schneller Video-Eingangsseek mit kurzem Vorlauf und wiederhergestellter absoluter Zeit ist normales Verhalten des optionalen CPU-Motors 0.1.2. Ton und weitere Streams behalten den bisherigen Dekodierweg. Streamreihenfolge, Encoder-/Bitratenparameter, Schnittgrenzen, kopierte Mittelstücke und Ausgabe-/Indexverwaltung bleiben erhalten. Kein NVENC.
- Experimenteller Cargo-Schalter und Warnhinweis entfernt. Neuer eigener Buildordner; bisherige Referenz-/Versuchs-EXEs erhalten. Cargo.lock ändert nur die eigene Paketversion, keine Abhängigkeiten.
- CAN verwendet dieselbe geprüfte CLI und seinen konfigurierbaren Motorpfad. Lokale Pfadumstellung vom Nutzer beauftragt; Indexerpfad und übrige Einstellungen erhalten. Keine Änderung des C#-Schnittpfads, keine fest codierten Benutzerpfade, keine neue Cutlist-Pflichtabhängigkeit. MP4Box bleibt Vorgabe.
- 25 Rust-Tests, 743 CAN-Tests, synthetischer Auftrag über CANs Motorfabrik/Schnittdienst und vier reale bytegleiche Ausgaben. Bekannte Zeitstempel-/Framevorbehalte bleiben bestehen; Details im Seek-Prüfbericht.

## ADR-015 – Messbare Phasen und laufende Zeit statt geschätztem Gesamtfortschritt

**Status:** umgesetzt am 10.10.2026 für die bestehende otr-can-0.1.1-Schnittstelle.

- CAN übersetzt die vorhandenen FFMS2-/otr-can-Protokollzeilen in eine getrennte sichtbare Schrittanzeige. Originalausgaben bleiben im kopierbaren Protokoll. Unbekannte/ungültige Zeilen werden erhalten; nur erkannte Zeilen liefern eine Übersetzung.
- `CutProgressUpdate` bleibt mit seinem bisherigen Konstruktor kompatibel und erhält die additive Kategorie `Progress` sowie einen optionalen phasenbezogenen Prozentwert. Gültige gemessene Indexwerte werden angezeigt; beim Wechsel zu CPU-/Kopierarbeit verschwindet der Zahlenwert und der Balken wird unbestimmt. Keine Gesamtprozente oder Restzeit werden berechnet.
- Behaltebereich X von Y, Kopieren/CPU-Kodieren, Originalzeitbereich und Zusammenfügen werden aus dem aktuellen Motorprotokoll erkannt. Ein separater UI-Timer aktualisiert Gesamt-/Schrittzeit und Zeit seit der letzten Meldung, ohne jede Sekunde Protokolltext anzuhängen. Monotone Zeitquelle, eingefrorene Abschluss-/Fehlerzustände und kontrollierte Timerfreigabe.
- Die bestehende Rust-Variante sammelt FFmpegs stderr bis zum Ende eines Unteraufrufs. Deshalb liefert diese Änderung keinen echten FFmpeg-Prozentwert innerhalb langer HD-Dekodier-/Kodierphasen. Die CPU-Schnittparameter und geprüften Rust-EXEs bleiben erhalten; eine spätere native Fortschrittsschnittstelle ist separat zu entwickeln und zu vergleichen.

## ADR-014 – Motorwahl direkt beim Schnittauftrag

**Status:** umgesetzt am 10.10.2026 auf Benutzerwunsch; ersetzt die gespeicherte Motorwahl aus ADR-013.

- Unter der Schnittliste stehen alle Cutlist-/Schnittbuttons einschließlich Namensmaske in einer gemeinsamen Zeile, sofern die Fensterbreite ausreicht. Die beiden Motorbuttons heißen **Schneiden MP4Box (schnell)** und **Schneiden otr-can (framegenau, langsam)**. Die Buttonwahl bestimmt den jeweiligen Auftrag; der Namensdialog zeigt den gewählten Motor im Fenstertitel. Bei schmalen Fenstern bricht die gemeinsame Leiste um.
- `CutEngineKind` wird ausdrücklich an Vorbereitung und Motorfabrik übergeben. MP4Box benötigt keine OTR-CAN-Einstellung und behält seine vorhandene MP4-Vorbereitung. OTR-CAN verwendet das Original und wird vor der Schnittausführung weiterhin vollständig geprüft. Bei Werkzeugfehlern gibt es keinen stillen Wechsel auf MP4Box.
- OTR-CAN-Einstellungen enthalten nur Motor-/Indexerpfade und die manuelle Werkzeugprüfung. Der Auswahlhaken und automatische Aktivierungsprüfung beim Speichern entfallen; leere/teilweise Pfade bleiben zulässig. Ein altes JSON-Feld `UseOtrCan` wird ohne automatische Dateiumschreibung ignoriert; vorhandene Pfade bleiben erhalten.
- Die angeforderte Beschriftung beschreibt die Bedienwahl. Bestehende Eingabe-/Abnahmegrenzen und Zeitstempelbefunde gelten weiterhin. Schnittparameter, Rust-EXEs, Cutlist-Logik, Versionskennung und Buildnummer werden nicht geändert.

## ADR-013 – Optionaler OTR-CAN-Ablauf unter CAN-Kontrolle

**Status:** Schritt 5 umgesetzt am 10.10.2026; ergänzt ADR-011/012. Reale Referenz-/Decoderprüfung in Schritt 6 abgeschlossen mit [Zeitstempelvorbehalt](OTR-CAN-SCHRITT6-PRUEFBERICHT.md); beide fertigen Testfilme persönlich positiv bestätigt.

- MP4Box bleibt Vorgabe. `UseOtrCan` fehlt in alten Einstellungen und ist dann false. Die ausdrückliche Auswahl wird nur nach Prüfung aller vier Werkzeuge gespeichert; vor einem nativen Schnitt werden sie erneut geprüft. Zurückschalten auf MP4Box erfordert keine nativen Werkzeuge.
- Der Auftrag ist ein Snapshot. OTR-CAN verwendet dessen Original; die MP4Box-Vorbereitung entfällt ausschließlich im nativen Modus. Namensmaske, Keep-Segmente, Ziel und Prozessablauf bleiben in CAN. Der Media-Prozessstarter verwendet ArgumentList, keine Shell, UTF-8-Ausgaben, parallele Leser und Prozessbaum-Abbruch. Dateiarbeit läuft außerhalb des UI-Threads.
- Ein GUID-Arbeitsordner und eine private Ergebnisdatei liegen als Geschwister im Ausgabeordner. CAN erzeugt einmal einen Index mit `ffmsindex -c -k`, prüft Index und track00-Begleitdateien und übergibt sie. Der Motor indexiert nicht. Nach erfolgreicher Container-/Video-/Audiostrukturprüfung veröffentlicht CAN durch Dateiverschiebung; Fehler/Abbruch erhalten das bestehende Ziel. Ein neu entstandenes Ziel wird nicht ohne ursprüngliche Ersetzungsfreigabe überschrieben. Bereinigung beschränkt sich auf den kontrollierten Arbeitsbereich; Probleme erscheinen im Protokoll.
- Rust `can-engine/` 0.1.1 erhält obligatorische CLI-Parameter `--ffmpeg`/`--ffprobe` mit absoluten vorhandenen Dateien. Sämtliche native Werkzeugaufrufe verwenden diese Pfade. Die bisherige Bibliotheksfunktion bleibt als Kompatibilitätseinstieg erhalten; CAN verwendet den expliziten Einstieg. Separate Build-Ausgabe bewahrt CAN 04 und 0.1.0. Seek-Reihenfolge, CPU-Codecs, vollständige Streamzuordnung und Audiobitratenparameter bleiben erhalten. Ein einzelner Teil mit anderer Ausgabeendung wird jetzt tatsächlich umgepackt.
- CAN übergibt Zeitgrenzen ohne Frameversatz mit sieben Nachkommastellen. Die native Syntax akzeptiert jetzt CAN-Ticks; die vorhandene Mikrosekundenquantisierung bleibt erhalten. Grenzen: genau eine Videospur an Position 0, Zeiten unter 24 Stunden, MP4-kompatible Streams. Die Metadatenprüfung beweist keine Framegenauigkeit oder Synchronität für beliebige Dateien; Navy CIS und Kimi werden anschließend geprüft.
- Keine neuen Produktionsabhängigkeiten, Werkzeugpakete, globale PATH-/OTR-Konfiguration, WSL, Datei-/Archivverwaltung, neue Cutlist-Pflichtwerkzeuge oder Repository-/Git-Veröffentlichung.

## ADR-012 – Optionale OTR-CAN-Werkzeugkonfiguration und begrenzte Prüfung

**Status:** Vorbereitung umgesetzt am 10.10.2026; der Schnittablauf folgt separat.

- OTR-CAN-EXE und `ffmsindex.exe` erhalten eigene optionale Einstellungen in `Settings/otr-can-settings.json`. Die vorhandenen FFmpeg-/ffprobe- und MP4Box-Einstellungen werden nicht migriert oder dupliziert. Das Speichern aktiviert keinen Schnittmotor; MP4Box bleibt Standard und derzeit alleiniger aktiver Motor.
- Der separate Dialog erlaubt auch eine leere oder teilweise vorbereitete Konfiguration. Nichtleere Werte müssen vollständige EXE-Pfade sein. Die Verfügbarkeit und erwartete Schnittstelle werden bei **Werkzeuge prüfen** geprüft; dabei dürfen die vier Programme in unterschiedlichen Ordnern liegen. Fehlende OTR-CAN-Werkzeuge sind keine neue Start-, MP4Box- oder Cutlist-Voraussetzung.
- Die Prüfung startet OTR-CAN mit `cut-can --help`, ffmsindex ohne Eingabedatei und FFmpeg/ffprobe mit `-version`. Es wird kein Video geöffnet, indexiert oder geschnitten. Pfadzugriffe und Prozessstarts laufen außerhalb des WPF-Threads; maximal zehn Sekunden pro Programm, Abbruch beendet den Prozessbaum, Ausgabeströme werden parallel gelesen und Ressourcen freigegeben.
- Eine Prüfung speichert keine Einstellungen. Abbrechen/Schließen beendet die Prüfung; die Bearbeitung bleibt ungespeichert. Speichern schreibt die separate Einstellung über eine temporäre Datei und anschließende Übernahme; Fehler bleiben sichtbar. Eine ausdrücklich gestartete Speicherung wird vor dem Schließen beendet.
- FFmpeg, ffprobe und FFMS2 bleiben Benutzerinstallationen. Der FFMS2-Downloadlink steht im Dialog, in der Anleitung und in den Setup-Hinweisen. Das Paketbauskript lehnt versehentlich enthaltene `ffmsindex.exe`/`ffms2.dll` ab. Es werden keine Werkzeuge installiert, mitgeliefert oder globalen PATH-Einstellungen verändert.
- Die spätere Schnittausführung muss die konfigurierten FFmpeg-/ffprobe-Dateien tatsächlich verwenden. Die derzeitige Rust-PATH-Suche wird durch eine erfolgreiche Hilfeprüfung nicht umgestellt; diese Anbindung und die einmalige jobbezogene FFMS2-Indexierung gehören zu Schritt 5. Keine globale `otr.json`, WSL oder OTR-Dateiverwaltung.

## ADR-011 – Gemeinsamer Schnittauftrag für optionale Schnittmotoren

**Status:** Schnittvertrag und MP4Box-Anbindung umgesetzt am 10.10.2026; OTR-CAN-Ausführung folgt separat.

- `CutAssistantNext.Core.Cutting` enthält den unveränderlichen `CutRequest`, positive `KeepSegment`-Bereiche, `ICutEngine` und gemeinsame Status-/Protokollmeldungen über `CutProgressUpdate`. Der Vertrag benötigt weder WPF noch konkrete Werkzeuge oder Cutlist-Dateiformate.
- CAN erzeugt die Keep-Segmente aus seiner bestehenden Remove-Schnittplanung. Der Auftrag enthält vollständige Original-/Zielpfade, unveränderte Zeiten, eine optionale Medienbildrate und die ausdrückliche Überschreiberlaubnis. Er ist eine Momentaufnahme; spätere Änderungen am Schnittplan beeinflussen laufende Aufträge nicht.
- Eine für MP4Box vorbereitete Arbeitsdatei wird separat als `SourceFilePath` zugeordnet. Der ursprüngliche `OriginalFilePath`, Zielname und Keep-Zeiten bleiben erhalten. Ein späterer OTR-CAN-Aufruf verwendet die Originaldatei und keine automatisch erzeugte MP4-Arbeitsdatei.
- Der gemeinsame Auftrag führt keine Framekorrektur aus. Die bestehende MP4Box-Endgrenzenanpassung bleibt ausschließlich in dessen Adapter. Eine Bildrate ist für MP4Box erforderlich, aber keine allgemeine Pflicht des Schnittvertrags.
- `ConfiguredCutEngineFactory` liefert derzeit ausschließlich MP4Box. Der Hauptablauf verwendet `ICutEngine`; Namensmaske, Dateidialoge, Vorbereitung, Fortschrittsfenster, Abbruch und Aufräumen bleiben unter CAN-Kontrolle. Die bisherigen MP4Box-Prozessargumente und Schnitt-/Veröffentlichungsregeln bleiben erhalten.
- OTR-CAN bleibt ein zukünftiger optionaler Motor. Dieser Schritt führt weder eine Auswahl in der Oberfläche noch OTR-CAN-/ffmsindex-Aufrufe, Werkzeugpfade, Installationspakete oder neue Produktionsabhängigkeiten ein. MP4Box bleibt Standard.
- Cutlist-Erzeugung und -Speicherung erhalten keine zusätzlichen Werkzeugabhängigkeiten. Die vorhandene ffprobe-Voraussetzung des Videolade-/Analyseablaufs wird hier nicht verändert; ein vollständig ffprobe-freier Bedienablauf ist damit nicht umgesetzt.

Die Vorbereitung wird weiterhin mit Buildnummer 11 geprüft. Die lokale Kandidatenkennung wurde anschließend am 10.10.2026 auf Benutzerwunsch auf **0.2.1 · Build 11 · RC3** erhöht. Ein direkter Release-Rebuild mit `CanBuildNumber=11` bewahrt den lokalen Zähler; das nummernerhöhende Buildskript bleibt unverändert. Git- und Referenzstände werden nicht veröffentlicht.

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
- Der Cutlist-Reader unterstützt neben UTF-8 auch historische Windows-1252-/ANSI-Dateien, da solche Bestandsdateien nachgewiesen wurden.
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

## ADR-009 – Cutlist-Server-Suche erfolgt automatisch und formatneutral

**Status:** entschieden

Nach dem erfolgreichen Laden einer Mediendatei sucht Cut Assistant Next automatisch auf dem konfigurierten persönlichen Cutlist-Server nach passenden Cutlists.

Begründung und Regeln:

- Die persönliche Server-URL ist eine lokale Benutzereinstellung und gehört nicht in Cutlist-Metadaten oder Programmcode.
- Zusätzlich zu `http://cutlist.at/<FRED>/` akzeptiert die URL-Prüfung `https://sniplist.mepaso.net/<FRED>/` mit demselben 64-stelligen FRED-Format. Diese zusätzliche Adresse wird im Einstellungsdialog nicht beworben. Such-, Download- und Upload-Anfragen behalten den konfigurierten Server bei; automatisierte Tests verwenden nachgebildete HTTP-Antworten. Am 01.10.2026 bestätigte der Benutzer mit dieser konfigurierten URL einen erfolgreichen Verbindungstest, Suchtreffer, das Laden einer Cutlist und einen erfolgreichen Upload mit Server-ID 2079449. Der Ablauf ist damit praktisch geprüft; aus den Screenshots wird keine Aussage über interne Weiterleitungen oder die Serverarchitektur abgeleitet.
- Ist keine persönliche Server-URL eingerichtet, wird die automatische Serversuche still übersprungen.
- Die Suche verwendet den vollständigen Originaldateinamen der geladenen Mediendatei.
- `ApplyToFile` und die Identität der Originaldatei werden durch die Serversuche nicht verändert.
- Ein erfolgreicher HTTP-Aufruf ohne Antwortinhalt bedeutet fachlich `0 Treffer` und ist kein technischer Fehler.
- Bei einem oder mehreren Treffern entscheidet der Benutzer selbst, ob und welche Cutlist verwendet wird.
- Mehrere verfügbare Cutlists werden neutral dargestellt. Eine Bevorzugung bestimmter Autoren findet nicht statt.
- Unterschiedliche Formate derselben Aufnahme, beispielsweise MP4 und AVI, werden nicht allein aufgrund ihres Formats ausgefiltert.
- Das Format wird für die Benutzeroberfläche aus dem Cutlist-Dateinamen abgeleitet und sichtbar gemacht.
- Kommentare, Bewertungen und andere Serverinformationen sollen dem Benutzer die Auswahl zwischen mehreren Schnittfassungen erleichtern.
- Die Cutlist-ID wird intern für Auswahl und Download benötigt, aber nicht als fachliche Information im Dialog hervorgehoben.
- Solange der Server kein verlässlich nutzbares Upload-Datum über die verwendete Suchschnittstelle liefert, werden numerische Cutlist-IDs absteigend sortiert. Eine höhere ID wird dabei ausschließlich als praktischer Näherungswert für eine neuere Serverfassung behandelt; aus der ID selbst wird kein Datum abgeleitet.
- Der Download einer ausgewählten Server-Cutlist verwendet keinen eigenen parallelen Ladealgorithmus. Die heruntergeladene Datei durchläuft den bestehenden lokalen Prüf- und Ladeweg.

Damit bleiben Serversuche, Benutzerauswahl, Download und fachliche Cutlist-Verarbeitung voneinander getrennt.

#### Praxisbestätigung: unterschiedliche Formate können dieselbe Timeline besitzen

Ein Praxistest am 05.09.2026 bestätigte die Entscheidung, Suchergebnisse nicht nach dem Containerformat der aktuell geladenen Mediendatei zu filtern.

Für dieselbe OTR-Aufnahme wurden sowohl eine MP4- als auch eine AVI-Cutlist vom Server geladen. Obwohl die AVI-Cutlist aufgrund einer Dateigrößenabweichung von 20,2 % die bestehende Plausibilitätswarnung auslöste, lag ihre Schnitt-Timeline praktisch auf derselben Zeitachse wie die MP4-Cutlist. Insbesondere war das Filmende mit `01:29:58.080` identisch. Die beobachteten Unterschiede von maximal etwa 1,35 Sekunden lagen ausschließlich an einzelnen Werbegrenzen und sind mit unterschiedlich gesetzten Schnittmarken vereinbar.

Daraus folgt:

- Containerformat und Dateigröße sind keine ausreichenden Kriterien zur Beurteilung der Timeline-Kompatibilität.
- Die Dateigrößenprüfung bleibt eine Warnung und kein automatischer Ausschluss.
- Server-Cutlists anderer Formate bleiben sichtbar und können nach Benutzerbestätigung geladen werden.
- Die tatsächliche fachliche Prüfung erfolgt weiterhin im gemeinsamen Cutlist-Ladeweg.
- Dieser Ansatz ist zugleich Grundlage für die spätere AVI-Vorbereitung für MP4Box, sofern die ursprüngliche Timeline beim Remux erhalten bleibt.

## ADR-010 – Direkter Cutlist-Upload basiert auf einer bewusst gespeicherten lokalen Fassung

**Status:** entschieden

Der direkte Upload auf den persönlichen Cutlist-Server erfolgt nicht aus einem flüchtigen Bearbeitungszustand, sondern aus einer zuvor lokal gespeicherten oder bewusst lokal wieder geladenen Cutlist.

Begründung und Regeln:

- Eine über `Cutlist erzeugen …` erfolgreich gespeicherte Cutlist wird als möglicher Upload-Kandidat gemerkt.
- Eine über `Cutlist laden …` bewusst vom lokalen Dateisystem geladene Cutlist kann ebenfalls als Upload-Kandidat verwendet werden.
- Beim Laden einer neuen Mediendatei wird ein zuvor gemerkter Upload-Kandidat verworfen.
- Vor dem Upload wird `ApplyToFile` erneut gegen den vollständigen Originaldateinamen der aktuell geladenen Mediendatei geprüft.
- Eine automatisch vom Server heruntergeladene temporäre Cutlist wird nicht unmittelbar als eigener Upload-Kandidat behandelt.
- Eine Server-Cutlist darf ausdrücklich als Vorlage dienen. Sie kann geladen, geprüft und verändert werden. Soll daraus eine eigene Fassung entstehen, wird diese zunächst lokal gespeichert und anschließend hochgeladen.
- Dadurch können beispielsweise korrigierte Schnittfassungen mit eigenem Autor und einem Hinweis wie `Vorlage von <Autor>, Ende korrigiert.` veröffentlicht werden.
- Der Upload verändert die lokale Cutlist nicht. Stattdessen wird im Speicher eine separate serverkompatible Kopie erzeugt.
- `Application`, `Version` und `Author` werden unverändert aus der gespeicherten Cutlist übernommen.
- Am 01.10.2026 wurde durch einen echten Upload nachgewiesen, dass der Server `Application=Cut Assistant Next` und `Version=0.2.0` akzeptiert und erhält (Cutlist-ID 2079433).
- Der Testupload vom 01.10.2026 mit `app=CutAssistantNext` und noch `version=0.26.5.6` war erfolgreich (vom Benutzer zurückgemeldete Cutlist-ID 2079437). Das HTTP-Feld `app` ist in der heruntergeladenen Cutlist nicht separat sichtbar.
- Das HTTP-Formular sendet nun `app=CutAssistantNext` und als `version` die dreiteilige numerische Version der laufenden CAN-Anwendung aus deren Assembly-Dateiversion (beim nachfolgenden Praxistest `0.2.0`, ohne Buildnummer oder RC-Zusatz). Diese Angabe ist unabhängig von der Version einer geladenen Cutlist. Der Testupload mit diesen beiden HTTP-Feldern wurde am 01.10.2026 vom Benutzer als erfolgreich zurückgemeldet (Cutlist-ID 2079440, GeneratedOn=2026-10-01 17:20:20). Die HTTP-Felder sind in der heruntergeladenen Cutlist nicht separat sichtbar; der Nachweis beruht auf dem Test der entsprechend gebauten Anwendung. Für diesen getesteten Ablauf sind die alten HTTP-Kennungen damit nicht erforderlich.
- Fachliche Inhalte wie Schnittbereiche, Bewertung, `SuggestedMovieName`, Fehlerangaben und Benutzerkommentar werden aus der gespeicherten Cutlist übernommen.
- Der anschließende Upload aus CAN 0.2.1 wurde am 01.10.2026 mit Cutlist-ID 2079448 bestätigt (`GeneratedOn=2026-10-01 20:40:32`). Die zurückgemeldete Serverfassung stimmt bei Programmname, Version, Autor und Schnittdaten mit der lokalen Fassung überein. Damit ist der Ablauf auch mit HTTP-Version 0.2.1 praktisch bestätigt; die HTTP-Felder sind weiterhin nicht separat im Cutlist-Inhalt sichtbar.
- Der Benutzerkommentar wird beim Upload nicht automatisch durch einen technischen Standardtext ersetzt.
- Der Upload erfolgt erst nach ausdrücklicher Bestätigung durch den Benutzer.
- Während eines laufenden Uploads wird ein zweiter paralleler Upload verhindert.
- Nur eine Serverantwort mit gültiger numerischer Cutlist-ID gilt als erfolgreicher Upload.

Damit bleiben Bearbeitung, lokale Speicherung, Serverkompatibilität und Veröffentlichung klar voneinander getrennt. Eine heruntergeladene Fremd-Cutlist kann als Arbeitsgrundlage dienen, wird aber nicht unbeabsichtigt als eigene Fassung veröffentlicht.
