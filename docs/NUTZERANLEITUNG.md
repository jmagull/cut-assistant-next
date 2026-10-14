# Nutzeranleitung – Cut Assistant Next

## Neu in Build 16 RC3: AVI/MP3 mit otr-can

Diese Ergänzung ist in **CAN Build 16 RC3 mit dem beigefügten Motor otr-can 0.1.5** enthalten. Unter **OTR-CAN-Werkzeuge …** wird die neue Motor-EXE ausgewählt. FFmpeg/ffprobe und FFMS2 bleiben separat eingerichtet, MP4Box bleibt Standard.

Bei geeigneten H.264-AVI-Dateien bietet **Schneiden otr-can** eine ausdrücklich experimentelle Vorbereitung an. CAN kopiert Video und Ton ohne Neukodierung in eine temporäre MP4-Datei. Anschließend wird MP3 erst nach dem Schnitt einmal nach AAC-LC kodiert (Stereo 192 kbit/s, Mono 96 kbit/s). Diese Tonkodierung ist verlustbehaftet, eine zusätzliche AAC-Vorkodierung entfällt. Bereits vorhandene AAC-/AC3-Spuren behalten ihren bisherigen Schnittablauf. Original, Schnittpunkte und Namensbildung bleiben erhalten; temporäre Arbeitsdateien werden aufgeräumt.

Ein alter Motor wird für MP3-Aufträge mit einem Upgrade-Hinweis abgelehnt. Ungeeignete Zeitstrukturen und fehlende Samples werden nicht automatisch repariert. Bitte jede Tonspur und alle Übergänge kontrollieren. Die neue Teufelsbrigade-Datei wurde am 12.10.2026 persönlich positiv abgenommen: Anfang, Ende, alle Schnittübergänge sowie Ton und Bild. Eine bereits im vorherigen Ergebnis vorhandene Video-Zeitstempelauffälligkeit bleibt offen. [Prüfstand und Grenzen](AVI-OTR-CAN-MP3-20261012.md).

Die folgenden Kapitel beschreiben die **0.3.0 · Build 30 · RC3**-Ausgabe mit Kontrollplayer, AVI-Frame-Lupe, Suchsprunghalbierung bei Richtungswechsel, FFmpeg-Versionskontrolle und gespeichertem Maskenwechsel ohne Neustart. Build 30 ergänzt die sichtbare Frame-Lupe, den Untersuchungsmodus, reine Filmnamen und die MP4-Keyframe-Navigation. Ältere Ausgaben bleiben unverändert verfügbar. [Veröffentlichungsnachweis](RELEASE-PUBLICATION-BUILD30-RC3.md).

Stand: 14.10.2026. Diese Anleitung beschreibt **0.3.0 · Build 30 · RC3** für Windows 11 x64. Die Vorbereitung von AVI und anderen Containern ist experimentell.

Im Fenstertitel und oben in der Oberfläche stehen Version und Buildnummer, bei einem Release-Kandidaten zusätzlich dessen Kennung, aktuell **Cut Assistant Next · 0.3.0 · Build 30 · RC3**. Die persönliche Produktiv-/VM-/Portable-/Deinstallationsabnahme gilt für die Vorgängerausgabe Build 29; eine erneute Installationsabnahme für Build 30 wird nicht behauptet. Bitte die tatsächlich angezeigten Angaben bei Fehlermeldungen mit angeben.

## 1. Einmalig einrichten

Starte CAN aus seinem vollständigen Ausgabeordner; kopiere nicht nur die EXE allein. Die [Build-30-RC3-Ausgabe](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build30-rc3) bietet Setup und Portable-ZIP mit libmpv und optionalem otr-can 0.1.5. Sie benötigt die separat installierte [.NET 10 Desktop Runtime für Windows x64](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). FFmpeg/ffprobe, GPAC/MP4Box und bei Bedarf FFMS2 werden separat eingerichtet. Build 29 wurde technisch sowie durch Produktivinstallation, VM-Setup/Portable mit beiden Motoren und Setup-Deinstallation geprüft und auf Nutzerauftrag veröffentlicht. Installationstyp und frisches Portable-Profil nicht gesondert unterschieden. [Nachweis](RELEASE-PUBLICATION-BUILD29-RC3.md).

Öffne die Einstellungen und hinterlege die benötigten Werkzeuge:

| Menü | Einstellung | Wofür? |
|---|---|---|
| Einstellungen → FFmpeg-Werkzeuge … | `ffprobe.exe` | Analyse der Original- und Arbeitsdatei |
| Einstellungen → FFmpeg-Werkzeuge … | `ffmpeg.exe` | Experimentelles verlustfreies Umpacken vor dem Schnitt |
| Einstellungen → Schnittanwendung … | Programmdatei `mp4box.exe` | Schneiden und Zusammenfügen |
| Einstellungen → OTR-CAN-Werkzeuge … | OTR-CAN-EXE und `ffmsindex.exe` | Pfade für den optionalen Motor einrichten und Werkzeuge prüfen |
| Einstellungen → Cutlist-Einstellungen … | Persönliche Server-URL | Cutlists suchen, herunterladen und hochladen |
| Einstellungen → Cutlist-Einstellungen … | Standardautor und Kommentarbausteine | Neue Cutlists vorbereiten |
| Einstellungen → Namensmaske … | Standard-Namensmaske | Namen der geschnittenen Ausgabe bilden |

Wähle jeweils die ausführbare Datei, nicht nur ihren Ordner. **Schnittanwendung …** richtet MP4Box ein. OTR-CAN hat eine eigene Auswahl; ein beliebiges anderes Programm wird durch Eintragen seines Pfades nicht zu einem unterstützten Schnittmotor.

Ohne Server-Konfiguration kannst du Videos weiterhin bearbeiten und lokale Cutlists verwenden. Ohne eingerichteten Schnittmotor kannst du Schnittmarken vorbereiten und Cutlists speichern.

Unter **Einstellungen → Cutlist-Einstellungen …** kannst du sechs Schnellbausteine bearbeiten. Der sechste ist mit **„Mit otr-can framegenau geschnitten.“** vorbelegt, auch bei älteren Einstellungen mit fünf eigenen Texten. Diese fünf Texte bleiben erhalten. Der Baustein wird in **Cutlist erzeugen** erst nach deinem Klick an den Kommentar angehängt; er wählt keinen Schnittmotor. Ein leeres sechstes Feld wird nach dem Speichern ausgeblendet und beim nächsten Start nicht neu befüllt.

### OTR-CAN-Werkzeuge einrichten

Unter **Einstellungen → OTR-CAN-Werkzeuge …** wählst du die OTR-CAN-Schnittmotor-EXE und `ffmsindex.exe` aus. Im veröffentlichten Build-16-RC3-Paket liegt der Motor bereits unter `tools/otr-can/otr_can.exe` im CAN-Ordner; wähle diese Datei aus. Die Motorquellen und Lizenztexte sind dort ebenfalls enthalten. FFMS2 / `ffmsindex.exe` einschließlich seiner DLLs bleibt separat zu installieren. Der Windows-Motor benötigt außerdem die Microsoft Visual C++ v14-Laufzeit für x64; bei fehlender `VCRUNTIME140.dll` das [aktuelle Microsoft-Paket](https://aka.ms/vc14/vc_redist.x64.exe) installieren. Den Motor wählst du für jeden Auftrag direkt unter der Schnittliste: **Schneiden MP4Box (schnell)** oder **Schneiden otr-can (framegenau, langsam)**. Der Werkzeugdialog hat keinen Auswahlhaken. Der reguläre Motor **0.1.5** enthält die korrigierte Frame-/Zeitstempelplanung und kodiert jede vollständige Tonspur neu. Bei AAC/AC-3 bleiben Codec, Kanallayout, Samplerate und Zielbitrate erhalten. MP3 wird nach PCM-Schnitt einmal nach AAC-LC kodiert (Stereo 192 kbit/s, Mono 96 kbit/s). Zusätzliche Kodierverluste und Laufzeit sind damit verbunden. 0.1.1 bleibt zur Schnittstelle kompatibel; die erhaltenen CAN-04- und 0.1.0-Referenzen unterstützen noch nicht die vollständige Werkzeugpfadübergabe.

FFMS2 kannst du aus dem [offiziellen Downloadbereich](https://github.com/ffms/ffms2/releases) beziehen. Entpacke das vollständige Windows-64-Bit-Paket mit seinen DLLs und wähle die darin enthaltene `ffmsindex.exe`. FFmpeg und ffprobe werden ebenfalls von dir installiert und bleiben unter **FFmpeg-Werkzeuge …** eingerichtet. Die Programme dürfen in unterschiedlichen Ordnern liegen; WSL und eine globale OTR-Konfiguration sind nicht erforderlich.

**Werkzeuge prüfen** verwendet die bearbeiteten OTR-CAN-/ffmsindex-Pfade und die bereits gespeicherten FFmpeg-/ffprobe-Pfade. Es werden ausschließlich Hilfe- oder Versionsabfragen ausgeführt: kein Video wird indexiert oder geschnitten. Fehlende Programme, unpassende Schnittstellen und Zeitüberschreitungen erscheinen im Dialog. Die Prüfung läuft im Hintergrund; **Abbrechen** oder Schließen beendet sie, ohne die Bearbeitung zu speichern.

Die beiden Pfade sind optional und dürfen leer bleiben. Du kannst vollständige EXE-Pfade bereits vor der Installation vorbereiten. **Speichern** schreibt ausschließlich diese Pfade. Vor einem otr-can-Schnitt prüft CAN alle benötigten Werkzeuge erneut; bei fehlenden Werkzeugen zeigt es einen Fehler. Ein früher gespeicherter Auswahlhaken hat keine Wirkung mehr. Bei einem Speicherfehler bleibt der Dialog geöffnet; eine gestartete Speicherung wird vor dem Schließen abgeschlossen.

Fehlende OTR-CAN-Werkzeuge haben keine Auswirkungen auf MP4Box oder die Cutlist-Bearbeitung. Der bisherige ffprobe-Bedarf beim Laden und Analysieren einer Videodatei bleibt bestehen. FFMS2 wird von CAN nicht mitgeliefert und ist für die reine Cutlist-Erstellung keine neue Pflichtabhängigkeit.

### Standardordner für Videos und Cutlists

Unter **Einstellungen → Standardordner …** kannst Du drei Ordner unabhängig voneinander festlegen:

- **Originalvideos:** Startordner bei **Videodatei laden**.
- **Geschnittene Videos:** Verbindlicher Ausgabeordner beim Schneiden mit beiden Motoren; ab Build 18 erscheint kein zusätzlicher Film-Speicherdialog.
- **Eigene Cutlists:** Verbindlicher Speicherordner für eigene Cutlists; beim Laden lokaler Cutlists weiterhin Startordner der Dateiauswahl.

Wähle vorhandene Ordner über **Ordner wählen …** oder trage vollständige Pfade ein. **Speichern** übernimmt die Auswahl; sie bleibt nach einem Neustart erhalten. Beim Laden von Originalen und lokalen Cutlists kannst Du weiterhin einen anderen Ordner im Dateidialog auswählen. Film- und Cutlist-Speicherung verwenden unmittelbar den jeweils eingestellten Zielordner.

Die Felder dürfen leer bleiben und denselben Ordner enthalten. Zum Filmschneiden ist ein vorhandener, beschreibbarer Ordner für **Geschnittene Videos** erforderlich; zum Speichern eigener Cutlists entsprechend ein Ordner für **Eigene Cutlists**. Fehlt der benötigte Ordner oder ist er nicht nutzbar, weist CAN auf die Einstellungen hin und verwendet keinen Ersatzordner. Der Filmname wird direkt im Schneidedialog festgelegt.

Über **×** oder durch Leeren eines Felds entfernst Du eine Vorgabe. Beim Laden bleibt die bisherige Dateiauswahl erhalten. Beim Speichern einer Cutlist wird dagegen niemals auf den Originalordner zurückgefallen. Vorhandene Einstellungen bleiben erhalten; keine Ordner werden automatisch angelegt und keine Dateien verschoben. **Abbrechen** verwirft die Bearbeitung.

### Persönliche Cutlist-Server-URL eintragen

Trage unter **Einstellungen → Cutlist-Einstellungen …** deine persönliche URL in der Form `http://cutlist.at/<dein FRED>/` ein. Ersetze den Platzhalter einschließlich der spitzen Klammern durch deinen persönlichen FRED. Der Einstellungsdialog öffnet sich höher und nahe dem oberen Rand des aktuellen Bildschirms. Seine Höhe wird auf den verfügbaren Arbeitsbereich begrenzt; auf kleinen Bildschirmen bleibt der Inhalt scrollbar. Die ausführlichen Hinweise zur Schreibweise blendet CAN unter dem URL-Eingabefeld ein, wenn „Verbindung testen“ wegen einer leeren oder ungültigen URL oder eines Verbindungsfehlers scheitert. Bei einem erneuten Test werden sie zunächst ausgeblendet; nach erfolgreicher Verbindung bleiben sie verborgen.

- **Ohne `www`:** Verwende `cutlist.at`, nicht `www.cutlist.at`.
- Für `cutlist.at` muss die URL in der aktuellen CAN-Version mit `http://` beginnen; `https://cutlist.at/…` wird nicht akzeptiert.
- Der FRED besteht aus genau 64 Hexadezimalzeichen (0–9 und a–f).
- Der abschließende Schrägstrich `/` ist erforderlich.
- Entferne angehängte Parameter oder Sprungmarken wie `?…` oder `#`.

### Namensmaske bearbeiten und zurücksetzen

Unter **Einstellungen → Namensmaske …** legst du fest, wie CAN vorgeschlagene Dateinamen bildet. Die CAN-Standardmaske trennt Staffel/Folge und Folgentitel mit einem Leerzeichen, zum Beispiel `Hunting Party S02E13 Xander Wax [25.08.2026]`.

Ab Build 20 stehen **S**, **Staffel**, **E**, **Folge** und **x** als einzelne Bausteine zur Verfügung. Damit lassen sich beispielsweise **S01E01**, **4x01**, **04x01** oder mit einem Bindestrich **02-13** zusammensetzen. Eine Eingabe **1** bleibt **1**, **01** bleibt **01**; CAN ergänzt keine führenden Nullen.

**S** und **E** erscheinen nur bei vorhandenem zugehörigem Wert. **x** erscheint nur, wenn Staffel und Folge vorhanden sind. Die neue CAN-Standardmaske erzeugt **Ghostbusters S01E01 Hexenprobleme [19.09.2026]**. Ab Build 30 RC3 ergibt ein Film ohne Serienangaben ausschließlich **Die Teufelsbrigade**. Bei vorhandenen Serienangaben bleibt die vollständige Maske mit Zusatzfeldern aktiv.

**Build 30 RC3** beendet den automatisch erzeugten Namensvorschlag nach dem **Name**-Baustein, wenn **Staffel, Folge und Folgentitel alle leer** sind. Hinter dem Filmnamen entfällt dann alles: Serienbausteine, Trennzeichen, Datum, Sender und weitere Zusätze. So entsteht **Die Hart 2** statt **Die Hart 2- [12.10.2026]**. Der Text im Namen selbst bleibt erhalten.

Sobald mindestens ein Serienfeld ausgefüllt ist, verwendet CAN weiterhin die vollständige Maske. Die gespeicherte Maske und Datums-/Serienmetadaten werden durch die Filmregel nicht verändert. Ein Präfix, das die Maske ausdrücklich vor dem Namen vorsieht, bleibt erhalten.

Die Regel gilt für neu erzeugte Namensvorschläge in Cutlists und beim Schneiden mit beiden Motoren. Ein manuell bearbeiteter Ausgabename oder geschützter übernommener Cutlist-Vorschlag bleibt erhalten. Einen bestehenden Vorschlag ersetzt du bei Bedarf bewusst mit **Aus obigen Eingaben neu erzeugen**.

Das Editorbeispiel verwendet **Ghostbusters__Hexenprobleme_26.09.19_10-50_pro7maxx_25_TVOON_DE.HQ.mp4**. Das OTR-Datum **26.09.19** steht für **19.09.2026**; Name, Folgentitel und die Serienwerte **01/01** sind ergänzte Beispieldaten. Der Ausgabedateiname erhält beim Schneiden zusätzlich **.mp4**.

Deine bereits gespeicherte Maske wird nicht automatisch ersetzt. Die bisherigen S-/E-Präfixbausteine erscheinen im Editor getrennt, unveränderte Paare behalten beim Speichern ihre bisherige Syntax. Andere eigene Präfixe bleiben ebenfalls erhalten. **CAN-Standardmaske** lädt bewusst die neue Standardmaske; erst **Speichern** übernimmt sie dauerhaft. Unabhängig davon kannst du den endgültigen Filmnamen im Schneidedialog frei bearbeiten.

Ziehe verfügbare Elemente und Trennzeichen mit gedrückter linker Maustaste in die Namensmaske. Vorhandene Bausteine kannst du durch Ziehen verschieben. Zum Entfernen wählst du einen Baustein aus und drückst **Entf**. Die Vorschau aktualisiert sich bei jeder Änderung.

| Schaltfläche | Wirkung |
|---|---|
| **Änderungen verwerfen** | Stellt die Maske wieder her, die beim Öffnen des Editors geladen war. Der Editor bleibt geöffnet. |
| **CAN-Standardmaske** | Lädt die aktuelle CAN-Standardmaske mit getrennten Serienbausteinen und Ghostbusters-Beispiel. Dauerhafte Übernahme erst mit Speichern. |
| **Speichern** | Übernimmt die angezeigte Maske dauerhaft und schließt den Editor. Beim nächsten Öffnen ist sie der Ausgangsstand. |
| **Abbrechen** | Schließt den Editor, ohne die Änderungen zu übernehmen. Die bisher gespeicherte Maske bleibt erhalten. |

Auch nach **CAN-Standardmaske** kannst du die Bausteine weiter bearbeiten. Erst **Speichern** übernimmt das Ergebnis dauerhaft. **Änderungen verwerfen** führt weiterhin zum Stand beim Öffnen zurück.

### Portable-Version aus dem ZIP verwenden

Das Portable-Paket vollständig in einen beliebigen Ordner entpacken und
`CutAssistantNext.App.exe` dort starten. Der bisherige selbstenthaltene
Build-6-Teststand bringt die .NET-Desktop-Laufzeit mit; die neue kleinere
laufzeitabhängige Vorschau benötigt eine separate Installation.

Für Wiedergabe und Schnitt werden die benötigten Programme so eingerichtet:

1. **Nur für die laufzeitabhängige Ausgabe:** Die
   [.NET 10 Desktop Runtime für Windows x64](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
   installieren. Die einfache ".NET Runtime" ohne Desktop-Komponenten genügt nicht.
2. FFmpeg einschließlich ffprobe von
   https://www.gyan.dev/ffmpeg/builds/ herunterladen und entpacken.
   Die bisher getestete Version ist 8.1.2 (Essentials Build).
3. GPAC einschließlich MP4Box von
   https://gpac.io/downloads/gpac-nightly-builds/ herunterladen und
   installieren. Getestet wurde GPAC 26.07.
4. Unter **Einstellungen → FFmpeg-Werkzeuge …** die beiden ausführbaren
   Dateien `ffprobe.exe` und `ffmpeg.exe` auswählen. Sie liegen
   normalerweise im Unterordner `bin` des entpackten FFmpeg-Pakets.
5. CAN erkennt eine regulär installierte GPAC-Version automatisch,
   solange kein eigener MP4Box-Pfad eingetragen ist. Falls die Erkennung
   nicht gelingt, unter **Einstellungen → Schnittanwendung …** die
   `MP4Box.exe` selbst auswählen.

Ab **0.3.0 · Build 27 · RC3** führt **Prüfen** im Dialog **FFmpeg-Werkzeuge** eine Versionskontrolle für beide Programme aus: **größer als 5.1** erforderlich (5.1/5.1.0 abgewiesen, 5.1.1 zulässig), **ab 9.0.2** empfohlen. Unterstützte ältere Releases erhalten einen Empfehlungshinweis. Unbekannte Versionen werden nicht als erfolgreich geprüft gemeldet. Git-Entwicklungsbuilds mit nachweislich neuerem Bibliotheksstand werden als solche gekennzeichnet; eine konkrete Releaseversion ab 9.0.2 wird nicht behauptet. Die Prüfung speichert keine Einstellungen, bleibt im Hintergrund und wird beim Schließen abgebrochen. Dieselbe Versionsregel gilt in der OTR-CAN-Werkzeugkontrolle.

Die Einstellungen werden weiterhin im Benutzerprofil gespeichert.
„Portable“ bezeichnet hier die entpackbare Programmausgabe, nicht eine
vom Benutzerprofil unabhängige Speicherung aller Einstellungen.

Der bisherige selbstenthaltene Build-6-Teststand wurde unter Windows 11
praktisch geprüft. Der VM-Test der neuen laufzeitabhängigen Ausgabe steht
noch aus.

## 2. Video laden

Klicke oben auf **Videodatei laden** und wähle das Originalvideo. Der Dateidialog bietet unter anderem MP4, AVI, MKV, MOV, TS, MPEG und WMV an. „Alle Dateien“ erlaubt die Auswahl weiterer Endungen. Die Auswahlmöglichkeit ist keine Zusage, dass jeder enthaltene Codec verarbeitet werden kann.

Nach erfolgreicher Analyse erscheint bei einem tatsächlich erkannten **AVI-Container** direkt unter dem Dateinamen der gelbe **Hinweis zum AVI-Original**. Er erklärt, dass Bildfehler, ungenaue Bildsprünge oder Tonversatz bei älteren Aufnahmen bereits im Original auftreten können. Eine abweichende Endung wie `.avi.mp4` wird ausdrücklich genannt. Prüfe Originalwiedergabe und Schnittergebnis; die Bearbeitung bleibt möglich. Der Hinweis verschwindet beim Laden einer anderen Datei, sofern diese kein AVI enthält. Entscheidend ist der erkannte Container: Eine echte MP4 mit `.avi` im Namen erhält keinen AVI-Hinweis.

CAN analysiert die Datei und lädt sie in den Player. Unter **Video-Informationen …** findest du Container, Video- und Audiowerte. Maßgeblich ist der erkannte Inhalt: Eine Datei mit `.avi` am Ende kann bereits MP4-Inhalt enthalten.

Beim Laden wird noch nichts umgewandelt. Die Originaldatei bleibt Grundlage der Bearbeitung.

## 3. Eine Cutlist wählen oder selbst beginnen

Ist eine persönliche Server-URL eingerichtet, sucht CAN nach dem Laden automatisch mit dem unveränderten Originaldateinamen. Der Treffer-Dialog zeigt die verfügbaren Cutlists sowie Informationen zur Auswahl, darunter Autor, Bewertung, Format, vorgeschlagener Name und Kommentar.

- **Cutlist laden** übernimmt die ausgewählte Server-Cutlist.
- **Ohne Cutlist fortfahren** schließt den Dialog. Du kannst eigene Schnittmarken setzen.
- **Cutlist vom Server laden** in der Hauptoberfläche öffnet die Suche später erneut. So kannst du eine andere Cutlist ausprobieren, ohne das Video neu zu laden.
- **Lokale Cutlist laden** öffnet eine bereits vorhandene `.cutlist`-Datei auf deinem Rechner.

Das Laden einer anderen Cutlist übernimmt deren Schnittplan. Sichere eigene Änderungen vorher mit „Cutlist erzeugen“, wenn du sie behalten möchtest.

### Warnungen beim Einlesen

Eine andere Dateigröße kann auf eine andere Aufnahme oder Qualitätsstufe hinweisen. CAN zeigt die Abweichung und fragt, ob du trotzdem laden möchtest. Bestätige bewusst und kontrolliere die Schnittstellen; eine passende Laufzeit allein beweist nicht, dass jede Schnittmarke passt.

Cutlists aus anderen Formaten können dieselbe Timeline beschreiben. Deshalb werden sie nicht allein aufgrund des Formats ausgefiltert.

Leere Bereiche mit exakt null Dauer werden beim Import ignoriert. Die übrigen gültigen Bereiche bleiben erhalten; die Cutlist-Datei auf der Festplatte wird dabei nicht verändert. Widersprüchliche oder negative Angaben bleiben Fehler. Schnittpositionen hinter dem tatsächlichen Videoende werden geprüft; erkannte kurze Endfragmente können über den vorhandenen Korrekturdialog behandelt werden.

## 4. Schnittmarken bearbeiten

CAN markiert **Bereiche, die entfernt werden sollen**. Die roten Abschnitte der Schnitt-Timeline stehen für Vorlauf, Werbung, Nachlauf oder andere unerwünschte Teile. Direkt hinter **Schnittliste** erklärt ein kleinerer, dezenter Klammertext die roten Bereiche und die Buttons **Schnittanfang setzen** und **Schnittende setzen**; bei schmalen Fenstern bricht der Text um.

1. Gehe im Player an den Anfang eines zu entfernenden Abschnitts.
2. Klicke **Schnittanfang setzen**.
3. Gehe an das Ende dieses Abschnitts und klicke **Schnittende setzen**.
4. Wiederhole das für weitere Abschnitte.

Mit **Videoanfang** bzw. **Videoende** kannst du die entsprechende Dateigrenze verwenden. Bestehende Bereiche lassen sich in der Tabelle oder Timeline auswählen und korrigieren. **Bereich löschen** entfernt die ausgewählte Markierung aus dem Schnittplan; es löscht keine Videodatei.

Ein Klick auf einen Schnittbereich wählt ihn aus und hebt ihn hervor. Ein erneuter Klick auf denselben Bereich hebt die Auswahl wieder auf – sowohl in der Timeline als auch in jeder Tabellenzelle. Ein Klick auf eine andere Zeile wechselt die Auswahl; Timeline und Tabelle bleiben synchron. In der Schnittliste werden Anfang, Ende und Dauer einheitlich als Stunden:Minuten:Sekunden mit drei Nachkommastellen dargestellt. Die interne Genauigkeit der Schnittmarken wird dadurch nicht verändert.

Ein Klick auf **Anfang** oder **Ende** springt zusätzlich weiterhin an die jeweilige Grenze im Original, auch wenn damit gerade abgewählt wird. Nach einer Abwahl ist keine Kante mehr für die Frame-Lupe ausgewählt. Klicke erneut auf die gewünschte Grenze, um sie wieder auszuwählen. Zeitkorrektur und Löschen verwenden wie bisher den aktuell ausgewählten Bereich.

Ein Klick auf die Zeitleiste oder die farbige Schnitt-Timeline setzt die Wiedergabeposition und den Slider direkt auf die gewählte Stelle. Bei roten Bereichen wird zusätzlich wie bisher die Auswahl umgeschaltet. Das Ziehen des Reglers bleibt möglich.

Play/Pause, Zeitleiste, **−10 Bilder**, **Bild zurück**, **Bild vor** und **+10 Bilder** helfen bei der Positionierung. Einzelbildschritte sind im pausierten Zustand verfügbar. Tastatur: Leertaste für Play/Pause, Pfeil links/rechts für ein Bild, Strg+Pfeil links/rechts für zwanzig Bilder. Mit Strg verwenden auch die größeren Bildschritt-Buttons zwanzig Bilder.

Die Anzeige **Geschnitten** ist eine Vorschau der geplanten Ausgabelaufzeit. Prüfe das tatsächliche Schnittergebnis im Player; Schnittmotor und Videostruktur können die exakten Grenzen beeinflussen.

### Schnittkanten vor dem Schneiden prüfen

Ab Build 22 kannst du die Filmszenen vor und nach einem roten Schnittbereich schnell im Originalplayer kontrollieren:

- **Rechtsklick auf Anfang:** drei Sekunden vor den eingetragenen Anfang springen.
- **Rechtsklick auf Ende:** drei Sekunden nach das eingetragene Ende springen.

Ein Bereich von **10:00 bis 12:00** führt damit zu **09:57** beziehungsweise **12:03**. Playerposition und Slider folgen; die Wiedergabe startet nicht automatisch. Auswahl, aktuell gewählte Lupenkante und Schnittwerte bleiben erhalten. Linksklick verhält sich weiterhin wie oben beschrieben.

An Dateianfang/-ende begrenzt CAN den Sprung auf die Originaldatei. Ist ein benachbarter Filmabschnitt kürzer als drei Sekunden, bleibt die errechnete Position innerhalb dieses Abschnitts. Berühren sich zwei rote Bereiche im Inneren direkt, erscheint ein Hinweis, weil dazwischen kein Filmstück zum Prüfen vorhanden ist.

Dauerzelle und Texteingaben erhalten keine neue Rechtsklick-Navigation. Zur genauen Korrektur verwende anschließend die vorhandene Frame-Lupe.

![Schnittkontrolle drei Sekunden vor dem Anfang und nach dem Ende](images/schnittkontrolle-3sek.svg)

Rechtsklick auf Anfang: 3 Sekunden zurück. Rechtsklick auf Ende: 3 Sekunden vorwärts. So lassen sich die Filmszenen unmittelbar vor und nach einem Schnittbereich kontrollieren.

### Keyframes im Hauptfenster – ab Build 30 RC3

Nach dem Laden einer MP4 analysiert CAN Keyframes im Hintergrund. Bei langen Aufnahmen kann das einige Zeit dauern; die normale Wiedergabe bleibt verfügbar. **◀ K** und **K ▶** neben den Bildschritten sind bis zum Abschluss deaktiviert; ihr Tooltip zeigt den Zustand. Anschließend springen sie zum vorherigen/nächsten analysierten Keyframe und pausieren die Wiedergabe. Die blauen Bereiche am oberen Rand der Schnitt-Timeline markieren diese Positionen; die roten Entfernbereiche bleiben erhalten. Mehrere Keyframes können bei langen Filmen in derselben Bildspalte liegen.

**(K)** neben der Framenummer erscheint nur auf einem zugeordneten Keyframe im pausierten Zustand. Nach einem Bildschritt oder während eines Sprungs verschwindet es. Die Navigation verändert keine Schnittmarken. Fehlende oder unsichere Zeitinformationen geben keine Keyframe-Navigation frei.

**Bei AVI werden diese Keyframe-Elemente ausgeblendet und es findet keine zusätzliche Keyframe-Analyse statt.** Der vorhandene AVI-Infoblock erklärt dies. Die Frame-Lupe einschließlich Untersuchungsmodus und Kantenbearbeitung bleibt für AVI nutzbar. [Prüfbericht und separate Fassung](KEYFRAME-NAVIGATION-20261014.md).

### Frame-Lupe im Entwicklungsbuild ab Build 10

Klicke in der Schnitttabelle auf die **Startzeit oder Endzeit** und drücke **Shift+F**. Die Frame-Lupe öffnet diese Kante in einem eigenen Fenster. FFprobe und FFmpeg müssen unter **Einstellungen → FFmpeg-Werkzeuge** verfügbar sein.

**Ab Build 30 RC3:** Mit **Frame-Lupe** in der Schnittliste oder **Shift+F** kannst du auch ohne ausgewählte Kante untersuchen. Navigiere im Hauptplayer zur gewünschten Stelle. Die Lupe öffnet im **Untersuchungsmodus** an der vom Player gemeldeten erreichten Position; der Sliderwert allein ist nicht maßgeblich. Vorschau, Bildschritte, Suche und Frame-Details bleiben verfügbar. **Schnittkante übernehmen** ist deaktiviert; die Schnittliste und ein vorgemerkter Anfang bleiben unverändert. Das gilt auch bei vorhandenen, aber abgewählten Bereichen oder einer ausgewählten Zeile ohne Start-/Endkante. Mit ausgewählter Kante öffnet weiterhin die bisherige Kantenbearbeitung. [Entwicklungsprüfung](FRAME-LUPE-UNTERSUCHUNGSMODUS-20261014.md), [veröffentlichte Pakete](RELEASE-PUBLICATION-BUILD30-RC3.md).

- **Links/Rechts** und die Buttons **−1/+1**: ein tatsächlich analysierter Frame.
- **−10/+10**: zehn Frames; mit Strg zwanzig. **Strg+Links/Rechts**: zwanzig Frames.
- **Umschalt+Links/Rechts** oder die beiden Suchbuttons: richtungsabhängige Suche ab **Build 27**. Der erste Sprung verwendet den eingestellten Startwert, standardmäßig 2000 Frames. Gleiche Richtung behält den Wert; erst ein Richtungswechsel halbiert ihn, bereits für den Sprung in die Gegenrichtung. Beispiel: **+2000 → +2000 → −1000 → −1000 → +500**. Minimum 1 Frame; die beiden Buttons zeigen ihre jeweilige nächste Sprungweite an.
- **Suche neu starten** setzt Suchweite und gemerkte Suchrichtung zurück. Normale Bildschritte verändern beides nicht. Fehlgeschlagene, abgebrochene oder an der Dateigrenze bewegungslose Suchsprünge verbrauchen keine Halbierung.
- Unter **Einstellungen → Frame-Lupe** lässt sich der Startwert ändern; er gilt beim nächsten Öffnen.
- **Frame-Details** zeigt Zeitstempel, Keyframe und Bildtyp. Die Detailtabelle bietet elf sichtbare Framezeilen; bei ±10-Schritten wird der gewählte Frame weiterhin automatisch in den sichtbaren Tabellenbereich gescrollt. Ein Klick auf einen Frame in der Detailtabelle lädt dessen Vorschau. Bei kleinen Fenstern bleibt der Detailbereich scrollbar.
- Ab **Build 11** zeigt **Neue Schnittkante** die Zeit des gewählten Frames. **Schnittkante übernehmen** trägt diese Zeit in den Schnittplan ein und schließt die Lupe. Der Button ist erst mit bestätigter Bildvorschau innerhalb der Datei aktiv.
- CAN bearbeitet Entfernbereiche: An einer **Startkante** wählst du das erste zu entfernende Bild. An einer **Endkante** wählst du das erste Bild, das danach erhalten bleiben soll. Die Grenze liegt jeweils unmittelbar vor dem gewählten Bild; es wird kein zusätzlicher Frameversatz angewendet und nicht automatisch zu einem Keyframe gesprungen.
- **Abbrechen**, Escape oder das Fensterschließen lassen die Kante unverändert. Würde die neue Kante einen leeren/umgekehrten Bereich oder eine Überschneidung erzeugen, erscheint eine Meldung; die Lupe bleibt zum Korrigieren geöffnet.

Beim Laden eines größeren Abschnitts kann die Analyse etwas dauern. **Schließen** beendet auch laufende Vorgänge. Fehlen eindeutige Zeitstempel, zeigt die Lupe keine bestätigte Bildzuordnung an; Fehlerdetails stehen im aufklappbaren Detailbereich.

Die Übernahme in den Schnittplan speichert noch keine Datei. Prüfe anschließend die übrigen Kanten und verwende **Cutlist erzeugen**, um die korrigierte Fassung im eingerichteten Cutlist-Ordner zu speichern. Der Dateiname bleibt an die Originalaufnahme gebunden. Bei einer vorhandenen Fassung kannst du das Ersetzen ablehnen; wenn du beide Fassungen behalten möchtest, sichere die alte Datei zuvor als Kopie. Beim Export werden die korrigierten Entfernbereiche wie bisher in zu behaltende Abschnitte umgerechnet.

Die Übernahme verwendet Original-PTS und Zeitbasis und zieht den bekannten Containerstart ab. Ab **0.3.0 · Build 26 · RC3** kann die Lupe bei tatsächlich erkanntem AVI ohne Original-PTS den von FFmpeg rekonstruierten Bildzeitstempel verwenden. Status und Frame-Details kennzeichnen diesen Weg ausdrücklich. Eine Übernahme ist weiterhin nur mit bestätigter Bildvorschau und passender Zeitbasis möglich; fehlen auch rekonstruierte Zeitstempel oder ist die Zuordnung mehrdeutig, bleibt sie gesperrt. Sie verwendet weder die geschätzte Framenummer des Hauptplayers noch eine Rechnung aus Bildrate und lokaler Lupennummer. Die interne Zeitauflösung beträgt 100 Nanosekunden; die Anzeige mit Millisekunden verringert die interne Genauigkeit nicht. Die Framenummer zählt nur im geladenen Abschnitt; ein absoluter Datei-Frameindex und der unabhängige Abgleich zum sichtbaren Hauptplayerbild folgen. Dateien mit mehreren Videostreams benötigen zuvor die noch ausstehende Zuordnung zur aktiven Player-Videospur.

Die Wahl einer genauen Schnittzeit garantiert nicht, dass jeder Schnittmotor sie identisch ausführt. CANs MP4Box-Verfahren bleibt unverändert. Für den vorgesehenen Vergleich mit CutlistDude lade die korrigierte Liste dort und kontrolliere Filmstart, Übergänge, Filmende und Ton-Synchronität im Ergebnis.

## 5. Nur eine Cutlist erstellen

Mit **Cutlist erzeugen** öffnest du den Erstellungsdialog. Prüfe Metadaten, vorgeschlagenen Filmnamen, Bewertung und Kommentar. **Cutlist speichern** schreibt die Datei direkt in den eingestellten Ordner **Eigene Cutlists**. Ein zusätzlicher Dateispeicherdialog erscheint nicht.

Der Cutlist-Dateiname wird fest aus dem vollständigen Originaldateinamen und der Endung **.cutlist** gebildet, beispielsweise **Aufnahme.HQ.mp4.cutlist**. Ein manuell geänderter Name für den geschnittenen Film verändert diesen Cutlist-Dateinamen nicht.

Fehlt der Cutlist-Ordner oder ist er nicht nutzbar, erscheint **Cutlist konnte nicht gespeichert werden** mit dem Hinweis, den Pfad unter Einstellungen zu überprüfen. CAN verwendet keinen anderen Ordner. Existiert die Cutlist bereits, fragt CAN vor dem Ersetzen nach; **Nein** ist vorausgewählt und erhält die Datei. Nach erfolgreichem Speichern folgt die bisherige Bestätigung.

Dafür muss das Video nicht geschnitten oder nach MP4 vorbereitet werden. CAN kann somit auch ausschließlich zum Erstellen und Korrigieren von Cutlists verwendet werden.

Das klassische Cutlist-Format beschreibt die zu behaltenden Abschnitte. CAN rechnet seine Entfernbereiche beim Erzeugen entsprechend um. Der Bezug zur Originaldatei bleibt bestehen.

### Fremde Cutlists als Vorlage verwenden

Cut Assistant Next ermöglicht es, vorhandene Schnittpunkte schnell zu kontrollieren und bei Bedarf framegenau nach den eigenen Vorstellungen nachzuarbeiten.

Wenn du aus einer fremden Cutlist eine eigene Fassung erzeugst, bleibt deren vorgeschlagener Filmname zunächst in der Namensvorschau erhalten. Änderungen an den Namensfeldern überschreiben diesen Vorschlag nicht automatisch. Erst mit **Aus obigen Eingaben neu erzeugen** wechselst du bewusst zur eigenen Namensmaske. Anschließend berücksichtigt die Vorschau die aktuellen Eingaben. Ab **Build 29** wird dabei die beim Öffnen des Dialogs aktuell gespeicherte Standardmaske verwendet, auch wenn die Datei und deren Namenszustand schon vor dem Maskenwechsel geladen waren. Ein Neustart oder erneutes Videoladen ist nicht erforderlich. Bereits übernommene Namen bleiben bis zum bewussten Neuerzeugen erhalten.

Die Namensvorschau lässt sich markieren und mit **Strg+C** kopieren.

Bei einer Vorlage mit einem anderen Autor ergänzt CAN den Kommentar der neu erzeugten Cutlist um einen Herkunftshinweis, beispielsweise „Vorlage von KukiDent“. Dein eigener Autor bleibt erhalten. Bei fehlendem oder identischem Vorlagenautor wird kein Herkunftshinweis ergänzt; doppelte Angaben werden vermieden.

Prüfe Schnittpunkte, Filmnamen, Bewertung und Kommentar vor dem Speichern. Wenn du deine Fassung veröffentlichen möchtest, speichere sie zunächst lokal und lade anschließend diese eigene Version hoch.

## 6. Video schneiden

Klicke **Schneiden** in der Hauptoberfläche.

### Wenn bereits MP4-Inhalt vorliegt

Mit **Schneiden MP4Box (schnell)** verwendet CAN den bestehenden Ablauf. Das gilt auch für MP4-Inhalt mit einer anderen Dateiendung.

### Wenn AVI oder ein anderer Container erkannt wurde

Bei **Schneiden MP4Box (schnell)** zeigt CAN zuerst einen Hinweis mit dem erkannten Container und fragt, ob es die experimentelle Vorbereitung durchführen soll:

- **Ja:** Weiter zum Schnittdialog; vor dem eigentlichen Schnitt wird eine temporäre MP4 erzeugt.
- **Nein:** Zurück zur Bearbeitung. Originalvideo und Schnittmarken bleiben geladen.

Die Vorbereitung kopiert die Streams mit FFmpeg ohne Neukodierung. Das Original bleibt unverändert. Falls ein Stream nicht in MP4 übernommen werden kann, erscheint ein Fehler; CAN startet nicht selbstständig eine verlustbehaftete Konvertierung.

### Namen und Ziel wählen

Im Schnittdialog prüfst du Name, gegebenenfalls Staffel, Folge und Folgentitel. Im Feld **Ausgabedateiname** kannst du den gesamten vorgeschlagenen Namen ändern: etwa **Ghosts - 4x01 - Patience**, **Ghostbusters** oder **Die Teufelsbrigade [21.01.2021]**. Auch Datum und Trennzeichen sind frei editierbar.

**Du kannst den gesamten Dateinamen abändern.**

Änderungen an den oberen Namensfeldern überschreiben deinen manuell gewählten Namen nicht. **Aus obigen Eingaben neu erzeugen** ersetzt ihn bewusst durch einen neuen Vorschlag aus den oberen Feldern und der gespeicherten Standardmaske. Ein aus einer Cutlist übernommener Vorschlag kann weiterhin als Ausgangswert dienen. Die manuelle Filmnamensänderung verändert weder Standardmaske noch Cutlist-Namensdaten.

Klicke auf **Schneiden …**. CAN verwendet unmittelbar den eingestellten Ordner **Geschnittene Videos** und ergänzt **.mp4** einmal. Ein weiterer Speicherdialog erscheint für beide Motoren nicht. Gib nur den Dateinamen ein, keinen Ordnerpfad. Ungültige Windows-Namen werden verständlich abgelehnt und nicht automatisch umbenannt.

Ist der Ausgabeordner nicht eingerichtet oder nicht nutzbar, überprüfe ihn unter **Einstellungen → Standardordner …**. CAN verwendet keinen Ausweichordner. Bestehende Ausgaben werden nur nach ausdrücklichem **Ja** in der Überschreibfrage ersetzt; **Nein** ist vorausgewählt und führt zurück zur Namensbearbeitung. Das Original darf auch nach einer Bestätigung nicht ersetzt werden.

### Mit OTR-CAN schneiden

Mit **Schneiden otr-can (framegenau, langsam)** werden geeignete MP4-Dateien direkt verwendet. Für geeignete H.264-AVI-Dateien bietet CAN eine experimentelle MP4-Vorbereitung an: Video und Ton werden ohne Neukodierung kopiert; das Original bleibt unverändert. Die vorbereitete Datei wird an den Motor übergeben, die Schnittpunkte bleiben gleich. CAN erstellt genau einen FFMS2-Index pro Schnittvorgang, übergibt die Behaltebereiche und startet den nativen CPU-Motor. Geeignete Abschnitte zwischen IDR-Bildern werden kopiert, Video-Schnittgrenzen neu kodiert. Alle unterstützten Tonspuren werden einmal vollständig neu kodiert. Bei AAC/AC-3 bleiben Codec, Kanallayout, Samplerate und Zielbitrate erhalten; MP3 wird erst nach dem PCM-Schnitt nach AAC-LC kodiert. Die Tonübernahme ist damit verlustbehaftet. CAN prüft die erzeugte MP4 und ihre Video-/Audiostruktur, bevor es die gewählte Ausgabe anlegt oder eine bestätigte vorhandene Datei ersetzt.

Nach Erfolg, Fehler oder Abbruch entfernt CAN seine Arbeitsdateien im Ausgabeordner. Abbruch beendet auch laufende FFmpeg-Kindprozesse. Falls Aufräumen scheitert, zeigt das Protokoll den verbliebenen Pfad. Das Original wird nicht verschoben. Motor 0.1.5 unterstützt eine Videospur an Streamposition 0, H.264 mit 25/50 fps und Quellstart null sowie AAC-LC/AC-3 und geeignete MP3-Spuren; Schnittzeiten müssen unter 24 Stunden liegen. Andere Codecs oder Zeitstrukturen werden mit einer Meldung abgelehnt. Der gemeinsame Pines-Film wurde persönlich als synchron bestätigt; der reguläre CAN-Auftrag liefert denselben Dateiinhalt in rund 3:33 Minuten. Weitere reale Formate und getrennte Tonspurkontrollen bleiben vor Version 1.0 offen. Prüfe Anfang, Übergänge, Ende und jede Tonspur persönlich. Die Prüfgrenzen stehen im [0.1.3-Prüfbericht](OTR-CAN-0.1.3-PRUEFBERICHT.md).

### Vorbereitung und Fortschritt

Der Dialog **Video vorbereiten und schneiden** zeigt den aktuellen Schritt und darunter das standardmäßig geöffnete **Protokoll**. Dieses lässt sich einklappen und über **In Zwischenablage** kopieren.

Bei sehr großen Protokollen zeigt das Fenster die jüngsten Meldungen und einen Hinweis auf ältere Einträge. **In Zwischenablage** kopiert weiterhin das vollständige Protokoll. Die Anzeige wird gebündelt aktualisiert, damit Laufzeit, Fehleranzeige und Bedienung reagieren.

Bei otr-can zeigt CAN die Indexierung mit den tatsächlichen Prozentwerten. Danach siehst du den aktuellen Behaltebereich, Kopieren oder CPU-Neukodierung der Schnittkante und den betreffenden Zeitbereich im Original. Hinzu kommen Laufzeit, Dauer des aktuellen Schritts und Zeit seit der letzten Programmmeldung. Der bewegte Balken bei CPU-Arbeit zeigt den laufenden Vorgang; er ist kein Gesamtprozentwert und keine Restzeitprognose. Motor 0.1.5 zeigt zusätzlich Originalton vorbereiten, alle Tonspuren neu kodieren sowie Bild/Ton zusammenführen und prüfen. Einzelne Decoder-/Encoderphasen können trotzdem ohne neue Meldung laufen. Die Zeitansicht läuft weiter; Abbrechen bleibt verfügbar. Nach Ende, Fehler oder Abbruch bleibt die gemessene Laufzeit stehen.

Bei experimenteller Vorbereitung geschieht Folgendes:

1. FFmpeg kopiert die Streams in eine temporäre MP4.
2. CAN prüft, ob die Medieninformationen im Wesentlichen zum Original passen.
3. Kleine Unterschiede, beispielsweise zwei Frames, werden im Protokoll vermerkt. Deutliche Abweichungen stoppen den Ablauf.
4. MP4Box schneidet die bereits gewählten Bereiche und erstellt die Ausgabe.
5. Die temporäre Arbeitsdatei wird nach dem Vorgang entfernt. Bei einem weiteren Schnittversuch wird sie erneut erzeugt.

Die Prüfungen sind keine vollständige Kontrolle aller Bildzeitstempel. Kontrolliere Anfang, Ende, Werbegrenzen und Ton-Synchronität der fertigen Datei.

Enthält eine HD-Aufnahme mehrere Tonspuren, bleiben sie beim Schneiden mit MP4Box erhalten. Öffne die geschnittene Datei anschließend in deinem eigenen Player und wähle dort bei Bedarf die gewünschte Tonspur aus. CANs Player bietet derzeit keine Tonspurauswahl.

**Abbrechen** fordert den Abbruch an. Warte, bis er abgeschlossen ist. Nach einem Fehler bleibt das Fenster offen und zeigt den Grund direkt oberhalb des Protokolls.

Die technische Gesamtabnahme von Build 24 enthält einen offenen Befund: Bei einer erzeugten Testdatei mit zwei gleichartigen AAC-Spuren scheitert MP4Box 26.07 beim Zusammenfügen. CAN zeigt den Fehler im Protokoll und veröffentlicht keine fertige Ausgabe. AAC/AC3 sowie die geeignete AVI/MP3-Testaufnahme wurden mit beiden Motoren erfolgreich geprüft. Dies ersetzt keine persönliche Prüfung realer Aufnahmen. [Prüfergebnisse und Grenzen](GESAMTABNAHME-BEDIENUNG-20261012.md).

### Abschluss und spätere Kontrolle

Nach einem erfolgreichen Schnitt und dem Aufräumen der temporären Arbeitsdatei erscheint **Schneiden abgeschlossen**. Das Fenster zeigt die fertige Datei sowie Anfang, Übergänge und Ende zur Kontrolle. Alle Zeiten beziehen sich auf den **geschnittenen Film**: Sie werden aus den Laufzeiten der behaltenen Abschnitte berechnet und dienen zur Orientierung; die tatsächlichen Schnittgrenzen können geringfügig abweichen.

Mit **Hinweise kopieren** übernimmst du Dateipfad, Kontrollstellen und Hinweise in die Zwischenablage, etwa für eine spätere Prüfung. Das Schnittprotokoll bleibt bei Bedarf aufklappbar und lässt sich markieren und mit **Strg+C** kopieren. **Schließen** beendet das Abschlussfenster; es gibt keinen Countdown.

Ab dem lokalen **Build 25** besitzen **Anfang, alle Übergänge und Ende** eine Schaltfläche **Prüfen**, auch bei nur einem Behalteabschnitt. Sie öffnet die fertige Datei in einem eigenen CAN-Kontrollfenster. Nach dem Laden beginnt die Wiedergabe am Anfang **bei 0:00**, für einen Übergang **drei Sekunden davor** und am Ende **drei Sekunden vor der tatsächlichen Filmdauer**. Ist der Film kürzer als drei Sekunden, beginnt die Endprüfung bei 0:00. **Vorherige Prüfstelle** und **Nächste Prüfstelle** wechseln in der Reihenfolge Anfang → Übergänge → Ende und starten dort erneut. Play/Pause, Positionsregler und eine eigene Lautstärke stehen zur Verfügung; die Wiedergabe läuft über den Übergang hinaus weiter. Am Anfang beziehungsweise Ende ist der unmögliche Wechsel deaktiviert. Nach dem natürlichen Filmende kannst du erneut einen Übergang prüfen oder über den Positionsregler zurückgehen.

CAN verwendet immer ein Kontrollfenster und übernimmt beim Wechsel die Datei und Übergänge des angeklickten Abschlussfensters. Nach dem Schließen kannst du es erneut öffnen. **Schließen** im Abschlussfenster beendet das Kontrollfenster nicht; beim Beenden von CAN wird es mit geschlossen. Originalplayer und Schnittliste bleiben für Korrekturen geöffnet. Die ursprüngliche Wiedergabe läuft gegebenenfalls weiter; zum getrennten Anhören kannst du sie selbst pausieren.

Anfang und Ende lassen sich immer prüfen. Die Zeiten im Abschlussfenster bleiben berechnete Planwerte; für den Start der Endprüfung verwendet und zeigt der Kontrollplayer die tatsächliche geladene Dauer. Bitte Bild, Ton, Tonsynchronität und sämtliche Übergänge persönlich kontrollieren. Die Tonspurauswahl bleibt dem externen Player vorbehalten. Fehlende oder defekte Ausgaben werden im Kontrollfenster verständlich gemeldet. [Prüfbericht für Anfang/Ende und verbleibende Abnahme](KONTROLLPLAYER-ANFANG-ENDE-20261012.md).

## 7. Cutlist auf den Server hochladen

Speichere eine selbst erstellte oder überarbeitete Cutlist zunächst lokal. Danach kannst du **Cutlist auf Server hochladen** verwenden. Auch eine bewusst lokal geladene Cutlist kann als Upload-Kandidat dienen. Prüfe vor der Bestätigung, ob sie zur aktuell geladenen Originaldatei gehört und sinnvoll kommentiert ist.

Eine frisch vom Server heruntergeladene temporäre Cutlist wird nicht unmittelbar als eigener Upload-Kandidat behandelt. Bearbeite und speichere sie zuerst als eigene Fassung. Beim Laden eines anderen Videos wird der bisherige Upload-Kandidat verworfen.

Teile Cutlists für die regulär verfügbaren Originalaufnahmen. Die von CAN intern erzeugte MP4-Arbeitsdatei dient nur dem lokalen Schnitt; ihre temporäre Identität wird nicht zur Grundlage einer neuen Cutlist gemacht. Eine allgemeine Herkunftserkennung für jede extern erstellte und manuell geladene Remux-Datei ist damit nicht zugesichert.

## 8. Fenster, Einstellungen und Protokolle

Das Hauptfenster merkt Größe und Maximierung beim Schließen und stellt sie beim nächsten Start wieder her, auch ohne zuvor ein Video zu laden. Die Lautstärke wird ebenfalls gespeichert.

Auch **Schneiden**, **Video vorbereiten und schneiden**, **Schneiden abgeschlossen** und **Cutlist erzeugen** merken ihre Größe und Maximierung jeweils getrennt. Vergrößere beispielsweise das Fortschrittsfenster für mehr Platz im Protokoll: Nach dem Schließen öffnet es beim nächsten Schnitt und nach einem Programmneustart wieder in dieser Größe. Sehr große gespeicherte Maße werden an die verfügbare Bildschirmfläche angepasst.

Bei geringer Breite umbrechen Bediengruppen. Bei geringer Höhe erreichst du die unteren Bereiche über die rechte Scrollleiste.

Die lokalen Einstellungen liegen unter `%LOCALAPPDATA%\Cut Assistant Next\Settings`. Das allgemeine Anwendungsprotokoll liegt unter `%LOCALAPPDATA%\Cut Assistant Next\Logs\CutAssistantNext.log`. Das technische Schnittprotokoll kannst du im Fortschrittsdialog kopieren, bevor du ihn schließt.

Über **Hilfe → Nutzeranleitung (GitHub)** öffnest du diese Anleitung im Standardbrowser. **Hilfe → GitHub-Projekt** führt zur Projektseite. Auch **Update/GitHub** öffnet die Projektseite im Browser. Diese Links benötigen eine Internetverbindung.

Unter **Credits** findest du die Danksagung mit Links zu den beteiligten Projekten und Informationsseiten. Der Text lässt sich scrollen; die Links öffnen sich im Standardbrowser.

## 9. Häufige Probleme

| Meldung oder Beobachtung | Nächster Schritt |
|---|---|
| Gültigen Pfad zu ffprobe.exe einstellen | Unter FFmpeg-Werkzeuge die vorhandene Programmdatei auswählen; ffprobe wird auch zur Kontrolle der Arbeitsdatei benötigt. |
| ffmpeg.exe nicht gefunden | FFmpeg-Pfad prüfen. Für direkte MP4-Schnitte ist kein Remux nötig. |
| Keine Schnittanwendung konfiguriert | Unter Schnittanwendung den Pfad zu MP4Box eintragen. |
| Persönliche Server-URL lässt sich nicht speichern | `http://cutlist.at/<dein FRED>/` verwenden: ohne `www`, mit 64-stelligem FRED und abschließendem `/`, ohne Parameter oder `#`. |
| Keine Server-Treffer | Originaldateiname und persönliche Server-URL prüfen; lokale Cutlist laden oder selbst markieren. |
| Cutlist enthält überlappende oder falsch sortierte Schnittbereiche | Die Cutlist wurde nicht geladen. Bitte eine andere Cutlist wählen. |
| Dateigröße laut Cutlist weicht ab | Aufnahme/Qualitätsstufe vergleichen; nur bewusst fortfahren und Grenzen kontrollieren. |
| Vorbereitung nicht möglich oder Prüfung gestoppt | Konkreten Fehler lesen und das Protokoll kopieren. Nicht jeder Codec lässt sich verlustfrei nach MP4 übernehmen. |
| Temporäre Datei konnte nicht gelöscht werden | Den im Protokoll genannten Pfad notieren; nach Ende des Vorgangs prüfen, ob die Datei noch von einem Programm verwendet wird. |
| Schnittlauf abgebrochen oder fehlgeschlagen | Die Ausgabe wird erst nach erfolgreichem Zusammenfügen übernommen. Bei Fehler oder Abbruch wird die temporäre Ausgabe aufgeräumt; eine bereits vorhandene Zieldatei bleibt erhalten. |

## 10. Experimenteller Umfang und Testbasis

Der integrierte AVI-Ablauf wurde mit **Die Diplomatin – Tod einer Nonne** und **Rubikon**, jeweils OTR-HD-AVI, mit jeweils zwei Cutlists erfolgreich geprüft. Die ausgegebenen MP4-Dateien enthielten H.264 mit 1280×720 bei 50 fps sowie MP3-Stereoton. Der Nutzer bestätigte Schnittgrenzen und Ton-Synchronität.

Das sind konkrete Beispiele, keine Garantie für alle historischen AVI-, MKV- oder sonstigen Codec-Kombinationen. Weitere technische Prüfergebnisse stehen im [Testplan](TESTPLAN.md), die genauen aktuellen Toleranzen im [Projektstatus](STATUS.md).

Ein weiterer Praxistest mit „Enigma – Das Geheimnis“ (AVI, H.264/MP3, 25 fps) führte erfolgreich über FFmpeg-Vorbereitung und MP4Box-Schnitt zur fertigen MP4. Anfang, Mitte, Ende, Abspann und Ton wurden kontrolliert. Dabei aufgetretene Zeitstempel-Auffälligkeiten sind im Testplan dokumentiert.

Bei einer anderen Full-HD-Aufnahme („Wo die Liebe hinfällt“) wurden dagegen bereits bei der Wiedergabe und Frame-Navigation Auffälligkeiten festgestellt. Die Datei besitzt einen AVI-Container, obwohl ihr Name auf `.avi.mp4` endet. Für diese Aufnahme liegt keine erfolgreiche Schnittabnahme vor. Am 11.10.2026 bestätigt der Nutzer das Original als bereits untauglich; diese Aufnahme wird auf seinen Wunsch nicht weiter untersucht. Er wartet auf einen neuen Sendetermin.

Besonders bei solchen älteren AVI-Aufnahmen können Bild-, Ton- oder Navigationsprobleme schon in der unveränderten Originaldatei vorliegen. CAN verändert das Original beim Einlesen nicht. Kontrolliere vor dem Schneiden die Wiedergabe und anschließend das tatsächliche Schnittergebnis sorgfältig.
