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
