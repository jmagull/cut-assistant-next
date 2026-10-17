# Paketsatz CAN 1.0.0 · Build 32

Stand: **16.10.2026**. Auf Nutzerauftrag Funktionsumfang eingefroren und **Setup, Portable-ZIP, passende Quellen-/Lizenzpakete und SHA-256-Prüfsummen** lokal erstellt und geprüft. Stabile Versionskennung **1.0.0 · Build 32**, kein RC-Zusatz. Sieben Auslieferungsdateien schreibgeschützt eingefroren unter `artifacts/release-1.0.0-build32`. Am **17.10.2026** nach Abschluss der heutigen Praxistests zusätzlich **Commit und Push** des Quell- und Dokumentationsstands beauftragt. Kein neuer Tag und keine GitHub-Veröffentlichung.

## Fester Quellstand

Gebauter lokaler Quellcommit **3e2ac03dfe4a701ecbc95c80cc06ff2527309b9f**, ohne Arbeitsbaumüberlagerung. Der erste freigegebene Freeze-Commit `a24520ce6630dda8b355e03719013ee655743bb6` wurde für die endgültigen Pakete um den bereinigten Voraussetzungenhinweis und beigelegte Release Notes ergänzt. Die endgültigen Pakete stammen ausschließlich aus dem oben genannten Folgecommit. Spätere Ergebnisdokumentation ändert diesen Quellpin nicht.

**557 manifestierte Quelldateien**, **558 Dateien im CAN-Quellen-ZIP** einschließlich Manifest; jede Archivdatei bytegenau mit dem Export verglichen. `SOURCE-SNAPSHOT.json` benennt Version, Build, feste Herkunft und den unveränderten Motor. Die vier vorbestehenden lokalen Produktions-Lockänderungen wurden nicht committet und sind nicht im Quellenarchiv enthalten; exportiert wurden alle acht Git-Lockdateien. Alle acht lokalen Lockdateien bleiben bytegleich zum Arbeitsbeginn erhalten.

## Tatsächlich geprüft

- Im endgültigen exportierten Baum: gesperrter win-x64-Restore, Release-Build **0 Warnungen/0 Fehler**, **1347/1347 Tests**, 0 übersprungen. Core 196, Cutlists 72, Media 194, App 885. Tests erst nach beendetem Build; vollständige TRX und Logs vorhanden.
- Framework-dependent win-x64-Publish. Dateiversion **1.0.0.32**; tatsächliches Hauptfenster und Anwendungsanzeige **Cut Assistant Next · 1.0.0 · Build 32**, ohne RC. Externe .NET-Laufzeit, FFmpeg/ffprobe, MP4Box und FFMS2 nicht versehentlich gebündelt. Geprüfter Vier-DLL-libmpv-Satz und Lizenzhinweise vorhanden.
- **250 Portable-Dateien** aus dem endgültigen ZIP extrahiert und jede SHA-256 gegen das Publish-Manifest geprüft, keine zusätzlichen Dateien. Benutzeranleitung, bereinigte Voraussetzungen und **RELEASE-NOTES.md** beiliegend. Setup mit **Inno Setup 7** aus genau diesem Publish-Verzeichnis erfolgreich kompiliert. Keine Installation ins laufende Nutzersystem durchgeführt.
- **13 native Fallgruppen** gegen die endgültigen entpackten Programmbits: stabile Versionsanzeige; Layout bei drei Größen mit demselben HWND und erhaltenem Schnittzustand/Position; Splitter, schmale Fenster und laufender Ansichtswechsel; OLE-Drop samt Ablehnungen und bestätigtem AVI-Laden; abgebrochener Medienwechsel ohne Zustandsverlust; AVI-Schutz in beiden Ansichten und Rückkehr zu MP4; Keyframe-Navigation, Halbierung und Lupenvorschau in beiden Ansichten; leere/40-zeilige Liste, volle Zeitwerte und feste Lupe; tatsächliche Schnitt-/Löschklicks und Werkzeugdialoge.
- Ausgabe-, Erzeugungs- und Namensmaskendialoge sowie lokale Dateiauswahl abgebrochen; Upload-/Server-Schutz geprüft. Kein echter Upload, erneuter Onlineabruf, gespeicherter Schnitt oder vollständiger Filmschnitt aus dem Prüfhelfer. **Keine WPF-Bindungsfehler**; rechte Werkzeugdarstellung gerendert und visuell kontrolliert. Alle **13 persönlichen Einstellungsdateien** vor/nach der abschließenden Prüfung bytegleich.
- Vier CAN-Assemblies und vier native DLLs des Helfers per SHA-256 mit dem endgültigen Portable verglichen. Nach dem letzten Quellenbau wurde der Helfer gegen die neuen DLLs neu gebaut und die vollständige native Prüfung wiederholt; frühere Prüf-DLLs wurden nicht als Nachweis übernommen. Der erste Helfer-Build hatte noch unvollständig umgestellte Referenzpfade; diese wurden ausschließlich im isolierten Prüfprojekt korrigiert. Produktbuild und Tests waren erfolgreich.
- Stabile Setupnamen ohne leeren Suffix geprüft. Der RC-Zweig der aktualisierten Setupdefinition wurde zusätzlich mit dem echten Compiler und vorhandenen alten Publish-Dateien in einem isolierten Testordner erfolgreich kompiliert; alte Release-Dateien nicht überschrieben.
- Motor **otr-can 0.1.5** unverändert; EXE-SHA-256 **FC6ED6568880A995F3D141A9378AA2CE28ADE8DC0E257E93DC972799BB1324A6**, Version und Fähigkeit `mp3-aac-v1` geprüft. Passende Quellen aus **2a474629836d52023bf37cbbdb309ccd1c23acf3** sowie Lizenz-/Provenienzpaket wiederverwendet. Keine neue bitidentische Reproduzierbarkeit der Motor-EXE behauptet.
- Sechs Archive und `SHA256SUMS.txt` vollständig nachgerechnet und schreibgeschützt eingefroren. **70 Dateien älterer lokaler Release-Sätze** unverändert; alle acht lokalen Lockdateien erhalten. Lokaler erfolgreicher Buildzähler nach bestandenem endgültigem Paketlauf auf **32** fortgeschrieben.

## Auslieferungsdateien und SHA-256

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-1.0.0-Build32-source.zip | 3160830 | 8EF486F084A37C626E8A5F277D9F0510F84A7E4E912BA6C79DA30AEA139DBF92 |
| CutAssistantNext-1.0.0-Build32-win-x64-Setup.exe | 20325270 | 015E9C814D0D747F7B55269F955DEC1977757086644B040E4D3A50CBBF1759AB |
| CutAssistantNext-1.0.0-Build32-win-x64.zip | 25005369 | 92229B0AFF563FD0D4D6203F84062BFE90B39C060301FC706BDFED14372DA60A |
| otr-can-0.1.5-source.zip | 63988 | 3A973278ABDF719B745405D9DFBB930C14A28CFEF40C7FB52A35CA17F7214E57 |
| SHA256SUMS.txt | 647 | D5DB5970657B3DF582DF3EF34288980383DEDDE7FE0F13BD37C38C61F75B1777 |

`SHA256SUMS.txt` enthält die sechs Archive; seine eigene Prüfsumme steht hier und im lokalen Artefaktnachweis. Das Manifest enthält aus technischen Gründen keinen Selbsthash.

## 17.10.2026 – Brilliant Minds: drei Abschnitte mit beiden Motoren persönlich erfolgreich

Quelle: `Brilliant_Minds_26.10.16_21-05_5plus_65_TVOON_DE.HQ.mp4`. MP4Box schneidet **739.08–1597.56 s**, **2221.60–3474.20 s** und **4143.60–4436.52 s**, fügt die drei Abschnitte zusammen und meldet „Fertig.“. Persönliche Rückmeldung **„ohne blitzer“**. Auch otr-can wird nach erfolgreichem Protokollabschluss mit **„auc alle prima“** positiv bestätigt.

otr-can **0.1.5**: eine FFMS2-Indexierung, **179998** eindeutige aufsteigende Quellframes und bestätigtes 25-/50-fps-Raster. Behaltebereiche **739.080–1597.600 s**, **2221.600–3474.240 s**, **4143.600–4436.560 s**, Frames **[18477,39940)**, **[55540,86856)** und **[103590,110914)**. Insgesamt **60103** Bilder: **59741** kopiert, **362** an den Kanten CPU-kodiert. Verifikation der 60103 Videopakete samt Roh-PTS/DTS erfolgreich.

Eine Stereo-AAC-Spur mit **48000 Hz**, Zielbitrate **160386 bit/s**, **115397760** verifizierten dekodierten Samples und kontinuierlichen Roh-PTS. Abschließende MP4-Verifikation bestätigt Video-/Audiopakete, PTS/DTS, Padding und Decoderkonfiguration. Aus Behaltezeiten, 25-fps-Framezahl und Audiosamples konsistente Sollausgabe **2404,120 s = 00:40:04.120**. Keine neue unabhängige Messung der fertigen Datei oder tatsächliche optionale vollständige Videodekodierung daraus abgeleitet. Die OTR-Endzeiten liegen jeweils **0,040 s** nach den MP4Box-Anforderungen; kein vollständig identischer A/B-Plan behauptet.

Persönlicher positiver Gesamttest dokumentiert; einzelne Tonspuren/Übergänge wurden in dieser Rückmeldung nicht gesondert aufgeschlüsselt. Keine neue VM-/Portable-/Deinstallationsabnahme gemeldet. Nur Abnahmedokumentation ergänzt; Programmcode, Motor, Quellpin, eingefrorene Pakete, Version/Buildzähler und Lockdateien unverändert. Kein neuer CAN-Build/Testlauf; technischer Paketnachweis weiterhin 1347/1347 Tests und Release ohne Warnungen/Fehler.

## 17.10.2026 – Doc S02E16: fertige Cutlist mit beiden Motoren persönlich erfolgreich

Quelle: `Doc_S02E16_26.10.16_20-15_5plus_50_TVOON_DE.HQ.mp4`. Nach dem zunächst fehlgeschlagenen otr-can-Versuch klärt der Nutzer **„die cutlist ist ja garnicht fertig“**. Der erste Plan reichte bis ans Ende der kompletten Aufnahme; dieser Lauf wird als **historischer Zwischenstand mit unfertiger Cutlist** geführt.

Mit fertig gesetztem Schluss-Schnitt laufen **beide Motoren erfolgreich**; persönliche Bestätigung **„Beide ohne probleme“**. MP4Box schneidet `582.96:2179.96` und `2842.36:3737.12`, fügt die Abschnitte zusammen und meldet „Fertig.“. otr-can 0.1.5 verwendet **582.960–2180.000 s** und **2842.360–3737.160 s**, Frames **[14574,54500)** und **[71059,93429)**. Die MP4Box-Endanforderungen liegen jeweils 0,040 s früher; kein vollständig identischer A/B-Plan behauptet.

OTR-Protokoll: eine FFMS2-Indexierung, **157446** gültige Quellframes, **62296** verifizierte Videopakete (62093 kopierte und 203 CPU-kodierte Bilder), eine Stereo-AAC-Spur mit **48000 Hz**, Zielbitrate **160281 bit/s**, **119608320** verifizierte dekodierte Samples und kontinuierliche Roh-PTS. Abschließende MP4-Prüfung für Video-/Audiopakete, PTS/DTS, Padding und Decoderkonfiguration erfolgreich. Aus Behaltebereichen, 25-fps-Framezahl und Audiosamples konsistente Sollausgabe **2491,840 s = 00:41:31.840**; keine neue unabhängige Messung der fertigen Datei. Die generische Meldung zur optionalen Voll-Dekodierung wird nicht als tatsächliche vollständige Videodekodierung gewertet.

**Merksatz:** Cutlist einschließlich Schlussbereich vollständig erstellen. Die Originaltonspuren müssen alle behaltenen Bereiche vollständig abdecken. Bei dieser Aufnahme war der Schluss-Schnitt entscheidend, da die unfertige Liste bis über das Originaltonende reichte. Ein Schluss-Schnitt ist keine allgemeine technische Pflicht, wenn Bild und alle Tonspuren den gewünschten Behaltebereich bereits vollständig abdecken.

Die frühere Audio-Enddifferenz von 0,784 s und die reproduzierte PCM-Abweichung bleiben als technische Erklärung des verworfenen Zwischenplans dokumentiert; sie sind **kein offener otr-can-Fehler des fertigen Doc-Schnitts**. [Historischer Befund und erfolgreiche Auflösung](DOC-S02E16-AUDIOENDE-20261017.md). Keine zusätzliche VM-/Portable-/Deinstallationsabnahme gemeldet. Nur Dokumentation berichtigt; Anwendung, Motor, eingefrorene Pakete und Quellpin unverändert. Kein neuer CAN-Build/Testlauf; technischer Paketnachweis weiterhin 1347/1347 Tests.

## 17.10.2026 – Toxic Folge 3: persönlicher MP4Box-Erfolg und erfolgreicher otr-can-Lauf

Quelle laut Nutzerprotokollen: `Toxic__Folge_3_S1E3_26.10.16_21-40_sf2_55_TVOON_DE.HQ.mp4`. MP4Box schneidet einen Behalteabschnitt mit **`-splitx 747.3:3222.76`**, beendet den Lauf mit **„Fertig.“**; der Nutzer bestätigt **„MP4Box prima“**. Der anschließende otr-can-Lauf ist technisch erfolgreich protokolliert. Eine gesonderte persönliche Beurteilung der otr-can-Ausgabe ist in dieser Nachricht noch nicht ausdrücklich enthalten; der automatische Abschlusshinweis ist keine persönliche Sicht-/Tonabnahme.

otr-can **0.1.5** bestätigt eine FFMS2-Indexierung mit **239975** eindeutigen aufsteigenden Quellframes und geeignetem 25-/50-fps-Zeitraster. Behalteabschnitt **747.300–3222.780 s**, Frames **[37365,161139)**: **177** Kantenbilder CPU-kodiert (40 + 137), **123597** kopiert, insgesamt **123774** verifizierte Videopakete samt Roh-PTS/DTS. Eine Stereo-AAC-Spur mit **48000 Hz**, Zielbitrate **162241 bit/s**, **118823040** verifizierten dekodierten Samples und kontinuierlichen Roh-PTS. Abschließende MP4-Verifikation für Video-/Audiopakete, Zeitstempel, Padding und Decoderkonfiguration erfolgreich.

Protokollwerte ergeben konsistent **2475,480 s = 00:41:15.480** Sollausgabe über 50-fps-Framebereiche, Behaltezeit und Audiosamples; keine neue unabhängige Messung der fertigen Datei. Die OTR-Endzeit liegt **0,020 s** hinter der im MP4Box-Aufruf übergebenen Endzeit; kein vollständig identischer A/B-Schnittplan behauptet. Aus der generischen Meldung zur optionalen Voll-Dekodierung wird keine tatsächlich ausgeführte vollständige Videodekodierung abgeleitet.

Nur Abnahmedokumentation ergänzt; keine Programm-/Motoränderung, kein erneuter Build/Testlauf und keine Änderung an eingefrorenen Paketen, Quellpin, Version/Buildzähler oder Lockdateien. Persönliche Qualitätsabnahme jeder Tonspur/jedes Übergangs sowie VM-/Portable-/Deinstallationsabnahme werden aus dem Protokoll nicht abgeleitet. Technischer Paketnachweis weiterhin 1347/1347 Tests und Release ohne Warnungen/Fehler.

## 17.10.2026 – Toxic Folge 2: beide Motoren persönlich positiv geprüft

Quelle laut Nutzerprotokollen: `Toxic__Folge_2_S1E2_26.10.16_21-00_sf2_40_TVOON_DE.HQ.mp4`. MP4Box schneidet einen Behalteabschnitt mit **`-splitx 511.84:2931.26`**, beendet den Auftrag mit **„Fertig.“** und ist vom Nutzer mit **„Keine Blitzer“** bestätigt. Zu otr-can nach erfolgreichem Lauf: **„auch alles prima“**. Damit ist **Folge 2** positiv geprüft; der bekannte MP4Box-Anfangsblitzer bei **Folge 1** bleibt ein separater Befund.

Das otr-can-0.1.5-Protokoll bestätigt eine FFMS2-Indexierung mit **194968** eindeutigen aufsteigenden Quellframes und geeignetem 25-/50-fps-Zeitraster. Ein Behalteabschnitt **511.840–2931.280 s**, Frames **[25592,146564)**: **177** Bilder an den Kanten CPU-kodiert (40 + 137), **120795** Bilder kopiert, insgesamt **120972** verifizierte Videopakete samt Roh-PTS/DTS. Audio: eine Stereo-AAC-Spur, **48000 Hz**, Zielbitrate **160707 bit/s**, **116133120** verifizierte dekodierte Samples und kontinuierliche Roh-PTS. Abschließende MP4-Paket-/Zeitstempel-/Padding-/Decoderkonfigurationsprüfung erfolgreich.

Aus Framebereichen mit 50 fps, Behaltezeiten und Audiosamples konsistent berechnete Sollausgabe: **2419,440 s = 00:40:19.440**; keine neue unabhängige Messung der fertigen Datei. Die protokollierte OTR-Endzeit liegt **0,020 s** hinter der im MP4Box-Aufruf übergebenen Endzeit; daher kein vollständig identischer A/B-Plan behauptet. Die generische Meldung zur optionalen vollständigen Videodekodierung belegt nicht, dass diese tatsächlich angefordert oder ausgeführt wurde.

Persönlicher positiver Gesamttest dokumentiert; eine gesonderte Prüfung jeder Tonspur oder jedes Übergangs wurde hier nicht aufgeschlüsselt. Keine Zuordnung zu VM oder Portable gemeldet. Nur Abnahmedokumentation ergänzt; keine Änderung an Programm/Motoren, Quellpin, eingefrorenen Archiven, Version, Buildzähler oder Lockdateien. Kein neuer Build/Testlauf; technischer Paketnachweis weiterhin 1347/1347 Tests und Release ohne Warnungen/Fehler.

## 17.10.2026 – Standard-/Power-User-Ansicht und problemloses Umschalten bestätigt

Mit **„sieht beides gut aus, umschalten klappt ohne probleme“** bestätigt der Nutzer beide Darstellungen und den Wechsel im realen Praxislauf von **CAN 1.0.0 · Build 32**. Die Screenshots zeigen in beiden Ansichten dieselbe Aufnahme **Winter Spring Summer or Fall**, denselben Frame **47.524**, Position **00:31:40**, sechs identische Schnittbereiche und die angezeigte Schnittdauer **01:28:50.160**. Sichtbar ebenfalls gleich: Lautstärke **74 %** und Halbierungssuchweite **1 Frame**. Damit sind Darstellung und problemloser Wechsel mit erhaltenem sichtbarem Bearbeitungsstand persönlich positiv bestätigt.

Die vorhandenen technischen Prüfungen zum gemeinsamen Player/HWND ergänzen diese persönliche Rückmeldung. Die Screenshots ersetzen keine gesonderte DPI-/Mehrmonitor- oder Windows-11-VM-/Portable-/Deinstallationsprüfung. Nur Abnahmedokumentation ergänzt; kein neuer Build/Testlauf und keine Änderung an eingefrorenen Paketen, Quellpin oder Programmcode.

## 17.10.2026 – Halbierungssuche und Frame-Lupe persönlich geprüft

Der Nutzer bestätigt **„halbierungssuche check“** und anschließend **„Framelupe check“**. **Halbierungssuche und Frame-Lupe** sind damit im aktuellen CAN-1.0.0-Build-32-Praxislauf persönlich positiv gemeldet. Dies ergänzt die bestehenden automatisierten und nativen Navigations-/Lupenprüfungen. Die gesonderten VM-/Portable-/Deinstallationsprüfpunkte bleiben bestehen.

Ausschließlich Abnahmedokumentation ergänzt; eingefrorene Pakete, Quellpin, Programmcode, Version/Buildzähler und Lockdateien unverändert. Kein neuer Build oder Testlauf; technischer Paketnachweis weiterhin 1347/1347 Tests und Release ohne Warnungen/Fehler.

## 17.10.2026 – Winter Spring Summer or Fall: fünf Abschnitte mit beiden Motoren erfolgreich

Quelle laut Nutzerprotokollen: `Winter_Spring_Summer_or_Fall_26.10.16_20-15_swiss1_105_TVOON_DE.HQ.mp4`. MP4Box schneidet fünf Abschnitte und fügt sie anschließend mit `-cat` zusammen; Lauf endet mit **„Fertig.“**. Persönliche Rückmeldung: **„Alles in Ordnung keine Blitzer“**; zu otr-can zusätzlich **„auch alles ok“**. Der Nutzer setzt die Praxistests fort.

| Abschnitt | MP4Box-Anforderung in Sekunden | otr-can-Protokoll in Sekunden |
|---|---|---|
| 1 | 606.24–1900.92 | 606.24–1900.96 |
| 2 | 2288.28–3293.16 | 2288.28–3293.20 |
| 3 | 3668.24–4963.24 | 3668.24–4963.28 |
| 4 | 5343.04–6798.04 | 5343.04–6798.08 |
| 5 | 7180.76–7461.16 | 7180.76–7461.20 |

Die angeforderten Anfangszeiten stimmen überein. Die protokollierten OTR-Endzeiten liegen jeweils **0,040 s** später; damit wird kein vollständig identischer A/B-Schnittplan und keine gemessene identische MP4Box-Ausgabedauer behauptet. Ursache oder tatsächliche Frame-Übernahme des MP4Box-Ergebnisses wurden hier nicht zusätzlich untersucht.

Das bereitgestellte otr-can-0.1.5-Protokoll belegt **eine** FFMS2-Indexierung mit **194972** eindeutigen aufsteigenden Frames und bestätigtem 25-/50-fps-Raster, fünf Behalteabschnitte und **133254** verifizierte Videopakete samt PTS/DTS. Eine Stereo-AAC-Spur mit **48000 Hz**, Zielbitrate **162462 bit/s**, **255847680** dekodierten Samples und kontinuierlichen Roh-PTS ist geprüft. Abschließende MP4-Verifikation bestätigt Video-/Audiopakete, PTS/DTS, Padding und Decoderkonfiguration. Aus den angegebenen 25-fps-Framebereichen, Behaltezeiten und Audiosamples konsistent berechnete Sollausgabe: **5330,160 s = 01:28:50.160**. Dies ist eine Protokollberechnung, keine neue unabhängige Messung der fertigen Datei. Die generische Meldung zur optionalen Voll-Dekodierung wird nicht als Nachweis einer tatsächlich ausgeführten vollständigen Videodekodierung gewertet.

Die persönliche Rückmeldung bestätigt diesen Filmtest positiv. Eine gesonderte Einzelabnahme von Bild, jeder Tonspur und jedem Übergang wurde hier nicht aufgeschlüsselt. Keine Zuordnung zu VM oder Portable gemeldet; diese Paketprüfpunkte bleiben offen. Die bekannten Radical-/Toxic-MP4Box-Blitzer gelten weiterhin für jene Dateien. Nur Abnahmedokumentation ergänzt, keine Motor-/Programmänderung und kein erneuter Build/Testlauf. Technischer Paketnachweis weiterhin **1347/1347 Tests**, Release ohne Warnungen/Fehler. Eingefrorene Archive, Prüfsummen und Quellpin unverändert.

## 17.10.2026 – Schutzrückfrage beim Videowechsel persönlich bestätigt

Der Nutzer meldet **„Prüfdialog check“** und zeigt den Dialog **„Videodatei wechseln“** in **CAN 1.0.0 · Build 32**. Der sichtbare Text fragt nach dem Laden einer anderen Videodatei und dem Verwerfen der aktuellen Schnittarbeit, mit Hinweis, bei Bedarf zuvor die Cutlist zu speichern. **Ja** und **Nein** sind vorhanden. Diese Rückmeldung bestätigt die angezeigte Schutzrückfrage; eine betätigte Antwort und das anschließende Ergebnis sind damit nicht getrennt persönlich bestätigt. Der vorhandene native Abbruch-/Zustandserhaltungsnachweis bleibt bestehen. Dieser Screenshot zeigt den Dateiwechseldialog, nicht den Kontrollplayer für das Schnittergebnis.

Nur Abnahmedokumentation ergänzt; eingefrorene Pakete und Quellpin unverändert. Kein neuer Build oder Testlauf für diesen Nachtrag. Keine weitere VM-, Deinstallations- oder Portable-Abnahme daraus abgeleitet.

## 17.10.2026 – Toxic-Praxistest in der Power-User-Ansicht und Drag & Drop

Der Nutzer prüft **Toxic Folge 1 S1E1 (HQ)** in der Profi-/Power-User-Darstellung. Quelle laut Protokoll: `Toxic__Folge_1_S1E1_26.10.16_20-10_sf2_50_TVOON_DE.HQ.mp4`. MP4Box schneidet einen Behalteabschnitt mit **`-splitx 746.3:3104.8`**, entsprechend **00:12:26.300 bis 00:51:44.800**, und beendet den Auftrag mit „Fertig.“. Der Nutzer meldet **einen Blitzer am Filmanfang**; erfolgreicher Prozessabschluss ist somit keine Bestätigung blitzfreier Schnittgrenzen.

Für **otr-can 0.1.5** meldet der Nutzer **keinen Blitzer am Anfang, alles prima**. Sein Cutlist-Server-Screenshot bestätigt die Ausgabe als blitzfrei und **Bild, Ton und alle Schnittübergänge geprüft**. Keine getrennte persönliche Prüfung jeder Tonspur daraus abgeleitet. Der Screenshot dokumentiert den vorhandenen Servereintrag; diese Arbeit hat keinen Upload vorgenommen. Anders als beim zuvor technisch untersuchten Radical-Fall liegt für Toxic bislang eine persönliche Rückmeldung mit Schnittprotokoll vor, kein zusätzlicher Vergleich dekodierter Bilder oder Stream-Pakete.

Anschließend ausdrücklich **„video in Player gezogen check :-)“**: Drag & Drop erneut persönlich bestätigt. Keine Zuordnung dieses neuen Tests zu VM oder Portable gemeldet; die noch offenen Paketprüfpunkte bleiben bestehen. Nur Repository-Dokumentation ergänzt. Eingefrorene Archive, Quellpin, Version, Buildzähler und Lockdateien unverändert; kein erneuter Build oder Testlauf erforderlich, technischer Paketnachweis weiterhin 1347/1347 Tests.

## Nachträgliche persönliche Setup-Abnahme auf dem Produktivrechner

Am **16.10.2026** bestätigt der Nutzer **„setup produktiv rechner check“** für **CAN 1.0.0 · Build 32**. Das Setup auf dem Produktivrechner ist damit persönlich erfolgreich geprüft. Die vorherigen Screenshots zeigen Standard- und Power-User-Ansicht mit stabiler Versionsanzeige, geladenem Film und gleicher Position/Schnittliste.

Die neue Windows-11-VM-Prüfung von Setup, Deinstallation und Portable bleibt offen. Ausschließlich Repository-Dokumentation ergänzt; die eingefrorenen Archive, Prüfsummen und der gebaute Quellcommit bleiben unverändert. Technischer Paketnachweis weiterhin 1347/1347 Tests und Build ohne Warnungen/Fehler; kein neuer Build oder Testlauf für diesen Nachtrag. Keine neue Veröffentlichungsfreigabe daraus abgeleitet.

## Persönliche Abnahme und verbleibende Schritte

Persönlich bestätigt: Kommandozeilenstart mit realer HD-Aufnahme, Drag & Drop, Power-User-Layout, Frame-Lupe, Frame-Liste und erfolgreiche Schnitte mit beiden Motoren. Radical mit otr-can 0.1.5 nach Bild, Ton und allen Übergängen blitzfrei. Der bekannte **MP4Box-Werbeblitzer bei 57:27.314** bleibt offen und dokumentiert; ebenso der ältere MP4Box-Befund bei zwei gleichartigen AAC-Spuren. Kein pauschaler Qualitätsnachweis aller Formate oder aller Tonspuren daraus abgeleitet.

**Noch erforderlich:** persönliche Prüfung dieses neuen Setup-Pakets, Deinstallation und Portable-Ausgabe in einer Windows-11-VM. Frühere Build-31-Paketabnahmen gelten für die früheren Archive. Echte DPI-/Mehrmonitorwechsel sind nicht persönlich bestätigt. Commit und Push wurden am 17.10.2026 gesondert beauftragt. Git-Tag und stabile GitHub-Veröffentlichung benötigen weiterhin einen gesonderten Nutzerauftrag; noch keine Release-Uploads ausgeführt.

AVI bleibt experimentell; bestehende Bedienungssperren und Zeitstrukturgrenzen bleiben erhalten. Externe .NET-10-Desktop-/VC++-Laufzeiten und Medienwerkzeuge sind erforderlich. [Freeze-Umfang](FREEZE-1.0.0-20261016.md), [Release Notes](RELEASE-NOTES-1.0.0.md), [VM-Übergabe](UEBERGABE-CAN-1.0.0-BUILD32-20261016.txt).

## Lokale Nachweise

`.build/freeze-1.0.0-build32-20261016`: `PACKAGE-FINAL.log`, `PACKAGE-INPUTS.json`, `SOURCE-COMMIT.txt`, `SOURCE-ZIP-PROOF.json`, `PACKAGE-TESTS.json`, `PORTABLE-PROOF.json`, `UI-QA-FINAL-ASSEMBLIES-BUILD.log`, `UI-QA-FINAL.log`, `proof/PROOF.json`, leeres `proof/BINDINGS.log`, Werkzeug-/Fensteraufnahmen, `SETTINGS-FINAL-BEFORE.json`, `PRESERVED.json`, `ARTIFACTS.json` und `FINAL.json`. Finale Build-/Test-/Publish-/Installerlogs und TRX außerdem im Paketausgabeordner. Der vorausgegangene Paketlauf ist separat im Nachweisordner erhalten; maßgeblich sind ausschließlich die finalen Archive und die oben aufgeführten Prüfsummen.
