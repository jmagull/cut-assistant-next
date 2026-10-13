# AVI-Frame-Lupe – 13.10.2026

## Problem und Änderung

Bei der Original-AVI „Die Teufelsbrigade“ fehlen die Original-PTS der decodierten Videobilder. ffprobe liefert jedoch `best_effort_timestamp` in der Video-Zeitbasis. Die bisherige Lupe konnte diese Zeiten in der Tabelle darstellen und zur Navigation verwenden, sperrte aber Vorschau und Kantenübernahme.

Der zusätzliche Weg gilt ausschließlich für den von der Medienanalyse erkannten AVI-Container, unabhängig von der Dateiendung. Vorhandene Original-PTS behalten Vorrang. Fehlen sie, darf der rekonstruierte Bildzeitstempel für den vorhandenen FFmpeg-Vorschauweg verwendet werden. FFmpeg wählt anhand dieses Zeitstempels und prüft seine Ausgabe mit `showinfo` auf genau ein Bild mit passender Zeitbasis und passendem Zeitstempel. Die fachlichen Prüfungen auf eindeutige, aufsteigende Analysezeiten, Dateigrenzen, gültige Schnittbereiche und Abbruch bleiben erhalten. Paket-DTS und eine Rechnung aus Bildrate/Framenummer werden nicht als Ersatz verwendet.

Status, Details und Übernahmeprotokoll kennzeichnen rekonstruierte AVI-Zeitstempel ausdrücklich; das Frame-Modell behält fehlende Original-PTS bei. Nach der Bildprüfung kann der gewählte Zeitpunkt als Schnittkante übernommen werden, mit derselben Containerstart-Normalisierung wie zuvor. Andere Container ohne Original-PTS bleiben gesperrt.

FFmpeg beschreibt `best_effort_timestamp` als aus Heuristiken geschätzten Framezeitstempel in der Stream-Zeitbasis: [AVFrame-Dokumentation](https://ffmpeg.org/doxygen/trunk/structAVFrame.html). Die Bildprüfung bestätigt die Zuordnung im FFmpeg-Vorschauweg, keine nachträglich vorhandenen Original-PTS oder allgemeine Reparatur von AVI-Zeitstempeln.

Geänderte Produktionsdateien: `FrameLoupeViewModel.cs` und `MainWindow.xaml.cs`. Keine Änderung an Schnittmotoren, Parser, Hauptplayer oder externer Werkzeugkonfiguration; keine zusätzliche Abhängigkeit.

## Prüfungen

Abschließender Stand **0.3.0 · Build 26 · RC3**: Restore ohne Schreiben der vorhandenen Lockdateien erfolgreich, nummerierter Release-Rebuild über `tools/build.ps1 -Configuration Release -NoRestore` mit **0 Warnungen/0 Fehlern**. **1018/1018 CAN-Tests bestanden** (Core 122, Cutlists 72, App 699, Media 125), keine übersprungen. Alle acht Lockdateien per SHA-256 bytegleich zum Aufgabenbeginn; vier lokale Produktionsänderungen unverändert erhalten. Geänderte C#-Dateien formatiert, Diffprüfung bestanden. Die 32 Integrationsfälle auch gegen den abschließenden Build 26 erfolgreich wiederholt, korrekte Prüfbilder gesichtet. Lupenvorprüfung: **40/40 Fälle bestanden**, darunter 19 neue Fälle für erkannte Container, Herkunft, PTS-Vorrang, Startversatz 0/+2/−2 s, variable Zeiten, Intervallerweiterung, mehrdeutige/fehlende Zeiten, Vorschaufehler, falsche Zeitbasis, leere Bilder und Abbruch.

Echte ffprobe-/FFmpeg-Prüfung mit nummerierten, kleinen synthetischen Quellen: AVI mit H.264-B-Frames/25 fps, AVI ohne B-Frames/25 fps, AVI mit B-Frames/50 fps und MP4 mit Original-PTS. Je sechs Positionen einschließlich Dateianfang, Keyframeumgebung, Mitte und Nähe zum Ende; nach jeder Position +1/−1 und Prüfung der tatsächlichen Bildnummer. Zusätzlich je eine Suche −64/+32 mit erweitertem Analyseabschnitt und erfolgreicher Halbierung. 28 Quellen-/Navigationsfälle erfolgreich. Keine realen Aufnahmen neu kodiert und keine Videos in Git aufgenommen.

Die Original-Teufelsbrigade-AVI bei **582,240 s** mit echtem Analyse-/Vorschauweg geprüft: 582,240 → 582,280 → 582,680 → 582,280 → 582,240 s, entsprechend +1/+10/−10/−1 Frame. Bildvorschau verfügbar, Original-PTS weiterhin leer, Übernahme freigegeben. Kompilierter WPF-Dialog lädt das tatsächliche PNG; normale und minimale Fenstergröße (1050×1000 / 780×620), erreichbarer Übernahmebutton und echter Übernahmeklick geprüft. Nur Startkante übernommen, Endkante 600 s erhalten. Keine Bindungsfehler; keine persönlichen Einstellungen gespeichert. Insgesamt 32 Integrationsfälle.

Ein erster lokaler Bildvergleich verwendete auch am Dateianfang `-ss 0` und erhielt dort leere Ausgabe. Der vorhandene Produktions-Vorschauweg verwendet für frühe Zeitstempel keinen Seek; dieser Weg besteht den nummerierten Dateianfangsvergleich. Ein Vorprüfbuild scheiterte an fehlenden Eigenschaften im erweiterten Testhelfer; diese ergänzt, alle Tests erhalten. Der isolierte Integrationshelfer benötigte einen zusätzlichen Namensraumimport. Prüfbilder zunächst wegen eines Renderursprungs ohne Hintergrund/mit abgeschnittenem Rand erzeugt; ausschließlich den Helfer korrigiert und die reale Buttonbegrenzung geprüft. Erstprotokolle und Vorprüfungen bleiben erhalten.

## Persönliche Abnahme – 13.10.2026

Der Nutzer bestätigt: **„Lupe funktioniert bei avi jetzt bestens :-), und schnitt geht auch, bei beiden schnittverfahren keine blitzer :-)“**. Damit sind die AVI-Lupenbedienung und der gemeldete Schnitt-/Bildvergleich der Teufelsbrigade für den geprüften lokalen Stand **0.3.0 · Build 26 · RC3** persönlich positiv bestätigt.

Die beiden beigefügten Protokolle betreffen „Die Teufelsbrigade“, AVI/H.264/25 fps mit MP3-Stereo/48 kHz, und zeigen erfolgreiches verlustfreies Umpacken der Videoquelle sowie sechs Behaltebereiche:

- MP4Box: sechs Segmente geschnitten, zusammengefügt und mit „Fertig.“ abgeschlossen.
- otr-can **0.1.5**: eine FFMS2-Indexierung, sechs Behaltebereiche, MP3 nach PCM-Schnitt einmal nach AAC-LC kodiert; **350.549.760 decodierte Tonsamples** und kontinuierliche Audio-PTS laut Motorprotokoll geprüft, abschließende Motor-/CAN-Prüfung erfolgreich.

Dies dokumentiert die persönliche Rückmeldung und die vom Nutzer übermittelten Protokollwerte. Keine neue unabhängige Ergebnisdatei-/Hash-/Voll-Dekoderprüfung, gesonderte Tonspur-/Synchronitätsabnahme oder bestätigte Cutlist-Speicherung daraus abgeleitet. Die vorhandenen AVI-PTS-Warnungen beim Umpacken sind weiterhin im Protokoll enthalten; sie verhindern in diesen Läufen den erfolgreichen Abschluss nicht und werden nicht als repariert eingeordnet.

Rohprotokolle lokal im ignorierten `.build/avi-loupe-20261013/personal-acceptance` samt SHA-256 gesichert. **Offen bleibt der gesonderte Bedienwunsch:** Suchsprung bei gleicher Richtung beibehalten und erst beim Richtungswechsel halbieren; [Merkliste](SETUP-MERKLISTE.md#offener-bedienwunsch--frame-lupe-13102026).

## Grenzen und nächste Prüfung

Rekonstruierte Zeitstempel sind weiterhin Schätzwerte. Die Zuordnung wird gegen die FFmpeg-Bildausgabe geprüft; ein allgemeiner unabhängiger Abgleich aller AVI-Bilder mit mpv, CutlistDude oder einem Schnittmotor wird nicht behauptet. Ein zusätzlicher Frame-/Keyframeversatz wird nicht eingeführt.

Frames, für die auch `best_effort_timestamp` fehlt, und mehrdeutige oder nicht aufsteigende Zeiten bleiben gesperrt; solche fehlenden Zeiten können beispielsweise an den letzten Decoderbildern eines Probeabschnitts auftreten. Defekte Originaldateien werden nicht repariert. Die zuvor als untauglich bestätigte Aufnahme „Wo die Liebe hinfällt“ wurde nicht untersucht.

Persönliche AVI-Lupen-/Schnittabnahme oben bestätigt. Folgeänderung „Suchsprung ausschließlich beim Richtungswechsel halbieren“ ist in **Build 27** umgesetzt und technisch geprüft; [neuer Prüfbericht](LUPEN-SUCHRICHTUNG-FFMPEG-20261013.md). Deren persönliche Bedienprüfung folgt. Bekannter MP4Box-Zwei-AAC-Befund bleibt separat. Öffentlicher Build 16 RC3 mit otr-can 0.1.5 unverändert; kein Paket, Release, Commit oder Push.

Lokale Belege im ignorierten `.build/avi-loupe-20261013`: Analysebefunde, nummerierte Quellen, Unit-Test-/Buildprotokolle, Integrationsnachweis, WPF-Prüfbilder und Erhaltungsprüfsummen.
