# Projektstatus

## 12.10.2026 – Build 16 RC3 veröffentlicht

Auf ausdrücklichen Nutzerauftrag **5) go :-)** um **12.10.2026 15:26:56 Uhr MESZ** als öffentliche Vorabversion veröffentlicht. CAN-Tag **v0.2.1-build16-rc3** verweist auf den gebauten Quellcommit **603d64d**; Motor **0.1.5 / 2a47462**. Alle **sieben Uploads**, Größen, GitHub-SHA-256-Digests, Release-Text und Quellziel vor und nach Freigabe geprüft. **793/793 CAN-Tests**, **44/44 Rust-Tests**, 249 Portable-Dateien geprüft; gemeldete persönliche Setup-/VM-/Portable-Prüfungen positiv. Bekannter PTS-Befund und weitere Prüfgrenzen bleiben erhalten. [Öffentliches Release](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.1-build16-rc3), [Veröffentlichungsnachweis](RELEASE-PUBLICATION-BUILD16-RC3.md). Pakete und Prüfsummen unverändert, vier vorbestehende Produktions-Lockdateiänderungen nicht übertragen.

## 12.10.2026 – Portable-Prüfung in der VM bestätigt

Nutzer bestätigt **„VM und zip: MP4Box check / otr-can check“**. Portable-ZIP mit beiden Motoren positiv geprüft; vorherige frische Setup-VM-Installation, beide Motoren und Deinstallation ebenfalls bestätigt. Beide neuen Anhänge sind identisch und enthalten MP4Box: eigene Arbeits-/Segmentdateien, sechs Bereiche, Zusammenfügen, „Fertig.“. otr-can aus ausdrücklicher Nutzerbestätigung übernommen, keine neuen nativen Messwerte behauptet. [Paketprüfbericht](RELEASE-BUILD16-RC3-RESULT.md). Praktische gemeldete Paketprüfungen positiv abgeschlossen; Überinstallation, frisches Portable-Profil und strenger Video-Nachweis nicht daraus abgeleitet. Bekannte Grenzen erhalten. Nächster separater Schritt: Veröffentlichung. Pakete/Prüfsummen und Build 16 unverändert.

## 12.10.2026 – Frische VM-Setup-Prüfung bestätigt

Nutzer bestätigt **frische Setup-Installation in der VM**, **MP4Box**, **otr-can** und **Deinstallation**. Beigefügtes MP4Box-Protokoll zeigt erfolgreiche Stream-Copy-Vorbereitung der Teufelsbrigade-AVI, sechs Bereichsschnitte, Zusammenfügen und „Fertig.“. otr-can und Deinstallation ausdrücklich als „check“ bestätigt; keine neuen nativen Messwerte oder zusätzliche detaillierte VM-Bild-/Tonabnahme daraus abgeleitet. [Paketprüfbericht](RELEASE-BUILD16-RC3-RESULT.md). Vorherige detaillierte Setup-Filmabnahme bleibt gültig, PTS-Befund erhalten. Nächster Pakettest: Portable-ZIP; Überinstallation nicht gesondert bestätigt. Kein neuer Build oder Release.

## 12.10.2026 – Setup Build 16: AVI/MP3 persönlich bestätigt

Nutzer bestätigt neuen Teufelsbrigade-Lauf aus **Setup Build 16**: **„Anfang, Ende, Schnitte, Bild, Ton alles top :-)“**. Protokoll: verlustfreie AVI-Vorbereitung, eine Indexierung, Motor 0.1.5, sechs Bereiche, 182.574 Videopakete und 350.542.080 Tonsamples/48 kHz, alle Motor-/CAN-Prüfungen abgeschlossen. Alle 249 erklärten installierten Dateien nachträglich gegen Paketmanifest bytegenau geprüft. Motorpfad vom verbliebenen Entwicklungspfad auf die bytegleiche installierte EXE korrigiert und gesichert; kein neuer Schnitt nach dieser Umstellung behauptet. [Aktualisierter Paketprüfbericht](RELEASE-BUILD16-RC3-RESULT.md). Installationstyp noch nicht ausdrücklich bestätigt; Portable, MP4Box-Paketlauf, weitere Installations-/Deinstallationsprüfungen und Veröffentlichung bleiben offen. Bekannte PTS-Warnungen bleiben dokumentiert.

## 12.10.2026 – Setup/Portable Build 16 RC3 erstellt

Schritt 3 abgeschlossen: fester CAN-Quellcommit **603d64d**, Motor **0.1.5 / 2a47462**. Gesperrter Restore ausschließlich im exportierten Quellbaum, Build/Publish erfolgreich, **0 Warnungen/Fehler**, **793/793 Tests**. Setup und Portable sowie vier passende Quellen-/Relink-Archive und Prüfsummen erstellt. Alle **249 Portable-Dateien**, App-Version 0.2.1.16, RC3-Assemblymetadaten, Motorversion/MP3-Fähigkeit und Artefakthashes geprüft. Getestete Motor-EXE erhalten; lokale und exportierte Lockdateien sowie Quellmanifest unverändert, Zähler bleibt 16. [Paketprüfbericht](RELEASE-BUILD16-RC3-RESULT.md). Noch keine Installation, persönliche Paketabnahme oder Veröffentlichung; nächster Schritt 4: Installations-/Portable- und Schnittprüfungen mit tatsächlichen Paketkomponenten.

## 12.10.2026 – Quellstand Build 16 RC3 gesichert

Schritt 2 abgeschlossen: CAN-Quellcommit **603d64d**, Motor-Quellcommit **2a47462** lokal gesichert. Feste CAN-/Motorquellenarchive ohne Arbeitsbaumüberlagerung und ein vollständiges 0.1.5-Motorpaket mit Quellen, Lizenzbeständen und Provenienz vorbereitet. Exportierter Motor offline gebaut, **44/44 Tests**, Archive/Manifeste vollständig geprüft; echter Motorblock des eingefrorenen CAN-Paketskripts akzeptiert das Paket. Getestete EXE erhalten; Kontrollbuild nicht bytegleich, keine bytegenaue Reproduzierbarkeit behauptet. Alle acht lokalen CAN-Lockdateien unverändert; vier vorbestehende Produktionsänderungen bleiben offen. CAN-Prüfstand weiterhin Build 16, 793/793 Tests. [Quellen-/Prüfbericht](QUELLSTAND-BUILD16-RC3-20261012.md). Kein Setup/Portable, Push, Tag oder Release. Nächster Schritt: Paketbau aus genau diesen Quellen.

## 12.10.2026 – Paketbau-Vorbereitung für otr-can 0.1.5

Schritt 1 abgeschlossen: Paketskript erwartet 0.1.5 samt passendem Snapshot/Quellenarchiv und prüft zusätzlich die MP3-Fähigkeit der kopierten EXE. Installationshinweise aktualisiert. Je 10 PowerShell-Prüffälle unter PowerShell 7 und Windows PowerShell 5.1 bestanden. Validierungsbuild **0.2.1 · Build 16 · RC3**, ohne Warnungen/Fehler, **793/793 CAN-Tests**. Anwendungscode/Motor unverändert; Filmabnahme betrifft den zuvor geprüften Build-15-Ablauf. Kein neuer Paketsatz erstellt. [Prüfbericht](PAKETBAU-OTR-CAN-015-20261012.md). Nächster Schritt: Quellstand sichern und passende feste Quellen vorbereiten; lokale Lockdateiänderungen erhalten.

## 12.10.2026 – Experimentelle AVI-/MP3-Unterstützung für otr-can

Lokaler Stand **CAN 0.2.1 · Build 15 · RC3 / otr-can 0.1.5**. Vorhandenes verlustfreies Umpacken wiederverwendet, Arbeitsquelle statt Original indexiert/geschnitten, MP3 erst nach PCM-Schnitt einmal nach AAC-LC kodiert. 793/793 CAN- und 44/44 Rust-Tests; vier synthetische Aufträge mit sechs bytegenau geprüften PCM-Tonspuren sowie bytegleicher AAC-/AC3-Regressionsausgabe bestanden. MP4Box-AVI und MP4-Multispurablauf weiterhin erfolgreich.

Teufelsbrigade: sechs unveränderte Keep-Bereiche, 182.574 Videopakete, 350.542.080 Audio-Samples; unabhängiges PCM vor Kodierung bytegleich zur Originalauswahl. Persönliche Filmabnahme am 12.10.2026 bestätigt: Anfang, Ende, alle Schnittübergänge, Ton und Bild einwandfrei. Strenger zusätzlicher Videodekodierlauf zeigt vorbestehende PTS-Auffälligkeit; Videostream neuer und früherer Ausgabe bytegleich. Keine allgemeine AVI-Freigabe oder Video-Reparatur daraus abgeleitet. [Prüfbericht](AVI-OTR-CAN-MP3-20261012.md). Vier vorbestehende src-Lockänderungen erhalten; kein Commit oder Release. Lokaler Motorpfad auf die separat geprüfte 0.1.5-EXE gestellt; frühere Einstellung gesichert, Indexer und alte EXE unverändert.

Die folgenden Angaben dokumentieren den vorherigen, veröffentlichten Build-13-Stand.

Stand: 11.10.2026. Dieser Überblick beschreibt den aktuellen Feature-Stand; ältere Projektdefinitionen und Architekturentscheidungen dokumentieren teilweise frühere Entwicklungsphasen.

Lokaler Entwicklungsstand nach Credits-Ergänzung und Dokumentationsabgleich: **0.2.1 · Build 13 · RC3**, Release-Build ohne Warnungen/Fehler und **785/785 CAN-Tests**. Der KI-Absatz wurde im echten WPF-Dialog bei normaler und kleiner Ansicht geprüft. Der persönlich abgenommene Paketsatz bleibt **Build 12 RC3**; dessen eingefrorene Quellen-/Binärpakete enthalten diese Ergänzung noch nicht. Der aktualisierte Build-13-Paketsatz ist erstellt und technisch geprüft; persönliche Paketbestätigung noch offen. Motor otr-can 0.1.4 unverändert, letzter nativer Prüfstand 40 Tests.

## Aktueller Umfang

Der aktualisierte **Build-13-RC3-Paketsatz ist erstellt, automatisch geprüft und veröffentlicht**: fester Quellcommit c98041a, 785/785 Tests, 249 vollständig geprüfte Portable-Dateien, Credits-/WPF-Prüfung mit entpackten Komponenten und künstlicher Zwei-Tonspur-Schnitt bestanden. Die Schnittausgabe ist bytegleich zum Build-12-Pakettest. [Neuer Prüfbericht](RELEASE-BUILD13-RC3-RESULT.md). Eine neue persönliche Installations-/Portable-Bestätigung für Build 13 ist nicht dokumentiert; der Nutzer beauftragte die sofortige Veröffentlichung. [Öffentliches RC3-Release](RELEASE-PUBLICATION-BUILD13-RC3.md).

CAN verwendet C#, .NET 10 und WPF unter Windows 11 x64. ffprobe liefert Medieninformationen, mpv/libmpv übernimmt die Wiedergabe, MP4Box den Schnitt. FFmpeg bereitet bei Bedarf eine MP4-Arbeitsdatei vor.

Umgesetzt sind Medienanalyse und Wiedergabe, Frame-Navigation, Frame-Lupe mit Schnittkantenübernahme und Halbierungssuche, manuelle Schnittplanung, lokale Cutlists, Server-Suche/-Download/-Upload, Namensmasken, Herkunftsübernahme bei Fremd-Cutlists, MP4Box-Schnitt und experimentelle Vorbereitung anderer Container. Die Bedienung ist in der [Nutzeranleitung](NUTZERANLEITUNG.md) beschrieben.

Die optionale OTR-CAN-Integration ist als Schritt 5 umgesetzt: der gemeinsame unveränderliche Schnittauftrag hält Original-/Zielpfade und Keep-Segmente fest. Die Motorwahl erfolgt auf Benutzerwunsch über **Schneiden MP4Box (schnell)** oder **Schneiden otr-can (framegenau, langsam)** direkt unter der Schnittliste, jeweils für diesen Auftrag. **OTR-CAN-Werkzeuge …** bietet nur Pfade und Prüfungen; ein älterer gespeicherter Motorhaken wird ignoriert. OTR-CAN 0.1.4 erhält die konfigurierten FFmpeg-/ffprobe-Pfade. CAN indexiert einmal im eigenen Arbeitsordner, startet den CPU-Motor am Original, prüft die MP4-/Streamstruktur und übernimmt erst danach das Ergebnis. Abbruch beendet den Prozessbaum; nach Erfolg, Fehler oder Abbruch werden eigene Arbeitsdateien bereinigt. Die lokale Entwicklungskennung ist **0.2.1 · Build 13 · RC3**; die öffentliche Build-7-RC2-Ausgabe bleibt unverändert.

Aktueller regulärer Motor **0.1.4**: Frame-/Zeitstempelkorrektur und vollständige Audio-Neukodierung aus 0.1.3, zusätzlich stabiler Audiofilterzustand bei geänderten Downmix-Begleitdaten. CAN verarbeitet große Warnprotokolle gebündelt mit begrenzter Anzeige und vollständiger Kopiermöglichkeit. **40 Rust- und 785 CAN-Tests**, 24 gemeinsame Grenzfälle, echter WPF-Großprotokolltest und unabhängiger Radical-PCM-Vergleich beider Tonspuren bestanden. Radical-Korrektur technisch geprüft und vom Nutzer persönlich bestätigt: sauberer Übergang, synchroner Ton. Stereo wird als lauter als 5.1 wahrgenommen; der Nutzer bestätigt denselben Unterschied bereits im Original. Keine quantitative Pegelmessung vorgenommen. Details und regulärer Abschluss im [Radical-Korrekturbericht](OTR-CAN-RADICAL-KORREKTUR-20261011.md). Pines und die ursprüngliche 0.1.3-Übernahme bleiben im [0.1.3-Prüfbericht](OTR-CAN-0.1.3-PRUEFBERICHT.md) dokumentiert. MP4Box bleibt Standard, ältere EXEs erhalten. Den aktuellen Praxistest-Punkt hat der Nutzer am 11.10.2026 als abgeschlossen bestätigt; Paketvorbereitung und persönliche Installationsabnahme von Build 12 sind anschließend abgeschlossen. Lokaler Build-12-RC3-Paketsatz mit Setup, Portable-ZIP und passendem Quell-/Lizenzbestand vorbereitet; persönliche Überinstallation, Windows-11-VM- und separate Portable-Paketprüfung einschließlich Bild/Ton und Setup-Deinstallation positiv bestätigt. [Paketprüfbericht](RELEASE-BUILD12-RC3-RESULT.md). Build 13 RC3 anschließend auf Nutzerauftrag veröffentlicht; siehe Veröffentlichungsnachweis.

Zusätzlicher persönlicher 0.1.3-Praxistest am 11.10.2026: **Torchwood / Vergiftet (HQ)** mit drei Behaltebereichen. Nutzer bestätigt **„kein Blitzer, ton ist top :-)“**. Das übermittelte Protokoll zeigt eine Indexierung, 77956 Video-Pakete / 25 fps sowie eine AAC-Stereospur / 48 kHz mit 149675520 decodierten Samples; jeweils 51:58.240, erfolgreicher nativer und CAN-Abschluss. Keine Laufzeit mitgeteilt. Ein-Tonspur-Test; weitere getrennte Mehrtonspurkontrollen bleiben offen. Details im [0.1.3-Prüfbericht](OTR-CAN-0.1.3-PRUEFBERICHT.md).

Weiterer direkter Nutzerlauf am 11.10.2026: **Chicago Fire / Ein komischer Vogel (HD)** mit Motor 0.1.4 und einem Behaltebereich erfolgreich abgeschlossen; AAC-Stereo und AC-3/6 Kanäle, beide Samplezahlen/-PTS und finaler Mux geprüft laut übermitteltem Protokoll, Ergebnisdauer 39:37.160. Nutzer bestätigt anschließend den otr-can-Bildanfang als ohne Blitzer; im gezeigten MP4Box-Vergleich steht noch TV-SPONSORING. Nutzer bestätigt abschließend **„ton und bild beide ok :-)“**. **Fenstergrößenübernahme vom Nutzer als „check“ bestätigt**. Details im [Motor-Prüfbericht](OTR-CAN-RADICAL-KORREKTUR-20261011.md) und [Fenstergrößenbericht](DIALOG-FENSTERGROESSEN-PRUEFBERICHT-20261011.md).

Weiterer persönlicher MP4Box-Praxistest am 11.10.2026: **Brilliant Minds (HQ)**, Aufnahme vom 09.10.2026, mit drei Behaltebereichen erfolgreich geschnitten und zusammengefügt laut übermitteltem Protokoll. Nutzer bestätigt **„Einwand frei ohne Blitzer :-)“**. Positive persönliche Schnitt-/Bildbewertung des MP4Box-Ergebnisses; keine gesonderte Tonspurprüfung oder Laufzeit mitgeteilt. Details im [Tagebuch](TAGEBUCH.md).

## Frühere Prüfungen und Entwicklungsschritte

Die folgenden Befunde dokumentieren die jeweiligen älteren Motor- und Versuchsstände. Aktuelle Übernahme und Grenzen stehen oben im 0.1.3-Prüfbericht.

Der neue Motor unterstützt zunächst genau eine Videospur an Streamposition 0 und Schnittzeiten unter 24 Stunden. CAN übergibt seine Zeitangaben mit sieben Nachkommastellen; die native Mikrosekundenberechnung bleibt erhalten. Die reale Referenz-/Decoderprüfung von Navy CIS und Kimi ist abgeschlossen: jeweils zwei Tonspuren, korrekte Soll-Bildanzahl und bytegleiche Ausgabe zu einem frischen CAN-04-Schnitt. Der Nutzer hat beide fertigen Filme persönlich als perfekt bewertet. Geerbte Zeitstempelauffälligkeiten an Teilübergängen bleiben offen; siehe [Prüfbericht](OTR-CAN-SCHRITT6-PRUEFBERICHT.md). Kein Paket veröffentlicht. Die Cutlist-Erstellung erhält keine neue Werkzeugpflicht; der bisherige ffprobe-Bedarf beim Videoladen bleibt bestehen.

Zusätzlicher direkter CAN-Bedientest am 10.10.2026: Radical mit derselben unveränderten Cutlist über beide Schnittbuttons. Der bei MP4Box sichtbare SRF-zwei-Blitzer am Übergang um 57:26 ist laut Nutzer im otr-can-Ergebnis verschwunden, ohne Lupenkorrektur. Abschlussprotokoll bestätigt eine Indexierung und erfolgreichen nativen Schnitt; die Ausgabe enthält AAC-Stereo und AC-3 mit sechs Kanälen. Details stehen im [Prüfbericht](OTR-CAN-SCHRITT6-PRUEFBERICHT.md).

Weiterer direkter HD-Bedientest am 10.10.2026: The Place Beyond the Pines mit fünf Behaltebereichen. Der Nutzer bestätigt die Schnittstellen beider Ergebnisse als „super“, ohne Blitzer und ohne Verwendung der Lupe. otr-can benötigte laut Nutzer knapp 15 Minuten. Beide Ausgaben enthalten AAC-Stereo und AC-3-Stereo; der native Metadatenbefund von einem zusätzlichen Videoframe/40 ms gegenüber der nominalen Cutlist bleibt im Prüfbericht festgehalten.

Nach persönlicher positiver Prüfung des schnellen Seek-Versuchs direkt in CAN wurde die Optimierung als regulärer Motor **0.1.2** übernommen. Video verwendet kurzen Vorlauf, der Ton behält den bisherigen Dekodierweg. Keine Experimental-Kennzeichnung oder Build-Schalter mehr; der lokale CAN-Motorpfad ist auf den neuen regulären Build gestellt. Vier erneute reale 0.1.2-Ausgaben sind bytegleich: Pines 57,2 s, Navy CIS 23,1 s, Kimi 31,0 s und HQ 24,8 s, jeweils Indexierung plus Motorlauf. 25 Rust- und 743 CAN-Tests bestanden; MP4Box bleibt Vorgabe. Aufbau und Messgrenzen stehen im [Seek-Prüfbericht](OTR-CAN-SEEK-PRUEFBERICHT.md).

Vorbereitung von Version 1.0 am 11.10.2026: Eine nummerierte Bilddiagnose am unveränderten 0.1.2-Motor reproduziert falsche Segmentgrenzen in 12 von 16 Grenzfällen. Gleiche Gesamtbildzahl kann ein fehlendes und ein doppeltes Bild verdecken; rohe PTS und Audio-Übergänge sind weiterhin nicht freigegeben. Die Lupen-Vorschaukette besteht 30 echte Bild-/PTS-/Zeitprüfungen, der Nutzer meldet aktuell keine neue Navigationsauffälligkeit. Vor Übernahme einer Korrektur ist eine separate Versuchsvariante erforderlich; siehe [Diagnose und Vorschlag](OTR-CAN-FRAME-DIAGNOSE-20261011.md).

Die freigegebene getrennte Video-Testvariante korrigiert inzwischen alle 16 ursprünglichen und sieben zusätzlichen nummerierten Bildfälle. Pines hat 137.625 tatsächlich decodierte Bilder mit korrekten Video-PTS/DTS; 84 Originalbildvergleiche ohne Abweichungsbefund, davon 39 wegen ähnlicher Nachbarbilder nicht eindeutig. 31 Rust-Experimenttests bestanden. Ausschließlich Video ohne Ton, keine CAN-Übernahme; Audio-Übergänge bleiben vor Version 1.0 offen. Quellen im Rust-Repository experiments/frame-boundaries, Details im [Korrekturbericht](OTR-CAN-FRAME-KORREKTUR-20261011.md).

Audio-Versuch am 11.10.2026: 35 synthetische Aufträge / 70 Tonspuren und beide Pines-Spuren mit exakten PCM-/Ergebnis-Samplezahlen sowie lückenlosen rohen PTS geprüft. Gemeinsame Pines-Testdatei hat 137.625 Bilder und alle drei Streams bei 5505,000 s; Paket-/Konfigurationsvergleich vor/nach Mux identisch. **Vollständige Audio-Neukodierung**, zusätzliche Verluste und Laufzeit. Der Nutzer bestätigt den gemeinsamen Pines-Testfilm persönlich als einwandfrei synchron; keine ausdrücklich getrennte Tonspur-Abnahme oder CAN-Übernahme daraus abgeleitet. Nachweise und Entscheidung vor Integration im [Audio-Prüfbericht](OTR-CAN-AUDIO-PRUEFBERICHT-20261011.md).

## Frame-Lupe und Schnittkantenkorrektur

- Umschalt+F öffnet die Lupe für eine ausdrücklich angeklickte Anfangs- oder Endkante. Eine unabhängige FFmpeg-Vorschau wird anhand des Original-PTS und der Zeitbasis geprüft. Tabellenwahl, Keyframe-Anzeige und normale Schritte mit ±1/±10 sowie Strg für ±20 sind verfügbar.
- Eigene Suchbuttons und Umschalt+Links/Rechts beginnen standardmäßig mit 2000 Frames und halbieren nach jedem erfolgreich angezeigten Suchsprung bis auf 1. Der Startwert ist einstellbar; normale Schritte verändern die Suchweite nicht.
- **Schnittkante übernehmen** setzt die ausgewählte Grenze vor den angezeigten Frame. Bei einer Startkante ist dieser das erste entfernte Bild, bei einer Endkante das erste anschließend behaltene Bild. Abbrechen verändert den Plan nicht. **Cutlist erzeugen** speichert anschließend die korrigierten Behaltebereiche.
- Derzeit wird eine Datei mit genau einem Videostream vorausgesetzt. Die Frame-Nummer gilt nur für den geladenen Analyseabschnitt; ein absoluter Dateiindex und der unabhängige Abgleich zum Hauptplayerbild bleiben offen.
- Navy-CIS-Praxistests bestätigten Navigation, Keyframe-Auswahl und das Speichern einer korrigierten Cutlist. Der externe CutlistDude-Test zeigte einen Ein-Frame-Versatz bei der Zuordnung der Zeiten, einschließlich eines Werbebildes am zweiten Filmstart. CutlistDude/CL_OFFSET und CANs MP4Box-Modus wurden nicht geändert; die Zuordnung muss vor einer Integration gesondert geklärt werden.

## Oberfläche und Cutlist-Ablauf

- „Videodatei laden“ bietet MP4, AVI, MKV und weitere Dateiendungen an. Entscheidend ist die Containeranalyse, nicht der Dateiname. Tatsächlich erkanntes AVI erhält nach dem Laden einen gelben, nicht blockierenden Hinweis auf mögliche Bild-, Navigations- und Tonprobleme im Original. Abweichende Endungen wie `.avi.mp4` werden erklärt; bei Dateiwechsel, Analysefehler oder Abbruch wird ein vorheriger Hinweis entfernt. Echte MP4-Dateien erhalten keine AVI-Warnung.
- Nach dem Laden wird bei konfiguriertem Server automatisch gesucht. „Cutlist vom Server laden“ öffnet die Suche erneut, ohne die Originaldatei neu laden zu müssen.
- Lokale und heruntergeladene Cutlists verwenden denselben Importweg. Dateigrößenabweichungen lösen eine Warnung aus und können bewusst akzeptiert werden.
- Null-Längen-Einträge werden beim Import ignoriert und NoOfCuts wird intern angepasst. Negative Dauern, widersprüchliche Frame-Dauern und ausschließlich leere Bereiche bleiben Fehler.
- Die Oberfläche zeigt Entfernbereiche; klassische Cutlists speichern die komplementären Behaltebereiche.
- Fremde Cutlists können als Vorlage dienen. Ein vorhandener Namensvorschlag bleibt zunächst erhalten und kann bewusst aus den eigenen Eingaben neu erzeugt werden.
- Bei abweichendem Vorlagenautor ergänzt CAN den Benutzerkommentar automatisch um einen Herkunftshinweis. Fehlende oder eigene Autoren erzeugen keinen Hinweis; doppelte Einträge werden vermieden.
- Anfang, Ende und Dauer erscheinen in der Schnittliste einheitlich mit drei Nachkommastellen. Die Namensvorschau im Dialog „Cutlist erzeugen“ ist markierbar und mit Strg+C kopierbar.
- Technische Medieninformationen stehen im Dialog „Video-Informationen“.
- Bei geringer Breite umbrechen die Bediengruppen, bei geringer Höhe scrollt der Hauptbereich. Das native Videofenster wird auf den sichtbaren Scrollbereich begrenzt.
- Fenstergröße wird auch ohne Videoladen gespeichert; Erfolg oder Fehler wird protokolliert.
- Schneiden, Fortschritt/Protokoll, Abschluss und Cutlist erzeugen merken getrennt Größe und Maximierung über Aufrufe und Neustarts. Bestehende Cutlist-Einstellungen bleiben kompatibel; 767 CAN-Tests und echter Vier-Dialog-Neustarttest bestanden. [Prüfbericht](DIALOG-FENSTERGROESSEN-PRUEFBERICHT-20261011.md).

## Hilfe, Credits und Versionsanzeige

- Hilfe verlinkt Nutzeranleitung und GitHub-Projekt; Update/GitHub öffnet ebenfalls die Projektseite.
- Credits zeigt die abgestimmte Danksagung in einem scrollbaren Dialog mit Projektlinks. Der KI-Hinweis aus der Dokumentation wurde nach der Build-12-Paketabnahme auch im aktuellen Dialog ergänzt; die eingefrorenen Build-12-Pakete enthalten diese Ergänzung noch nicht.
- Version und lokale Buildnummer stehen im Fenstertitel und in der Hauptüberschrift. Aktueller Entwicklungsstand: 0.2.1 Build 16 RC3; öffentliche Vorabversion: 0.2.1 Build 16 RC3.
- Vollständige Builds über `tools/build.ps1` erhöhen den Zähler nur nach Erfolg. Fehlgeschlagener Build und anschließendes Weiterzählen wurden geprüft.
- Hilfe-Links, Credits-Dialog und Update/GitHub wurden vom Nutzer live bestätigt.

## Experimentelle Video-Vorbereitung

Originaldatei → Analyse → Server-/lokale Cutlist oder eigene Marken → „Schneiden“ → gegebenenfalls Ja/Nein-Abfrage → FFmpeg-Arbeitsdatei → Plausibilitätsprüfung → MP4Box → MP4-Ausgabe.

Die Originaldatei bleibt geladen und unverändert. Cutlist-Namen und Dateigrößen beziehen sich weiterhin auf das Original. Die temporäre MP4 wird nicht als neue Nutzerdatei geladen und nicht Grundlage einer automatisch erzeugten Server-Cutlist.

FFmpeg verwendet Stream-Copy für alle Streams. Nicht kompatible Streams werden nicht still entfernt oder neu kodiert. Nach Abschluss, Fehler oder Abbruch versucht CAN die temporäre MP4 zu löschen; Fehler beim Aufräumen erscheinen im Protokoll. MP4Box legt seine Schnittsegmente im Ausgabeordner ab und räumt sie am Ende auf.

Geprüft werden Container, Streamanzahl, Codecs, Auflösung, Bildrate, Dauer und verfügbare Start-/Frame-Angaben. Kleine Abweichungen werden nur im Protokoll vermerkt:

| Merkmal | Aktuelle Toleranz |
|---|---|
| Gemeldete Frame-Anzahl | bis zu zwei Frames |
| Mittlere Bildrate | rechnerisch höchstens zwei Frames über die Original-Laufzeit |
| Containerlaufzeit | zwei Frames, mindestens 100 ms |
| Relativer Videostart | zwei Frames |
| Relativer Audiostart | 100 ms |
| Fehlende Start-/Frame-Anzahl-Metadaten | Detailhinweis; übrige Prüfungen gelten weiter |

Dies ist eine Metadaten-Plausibilitätsprüfung, kein vollständiger Nachweis der Timeline oder Ton-Synchronität. Die Diplomatin zeigte 317399 gegenüber 317397 Frames und 6347,98 gegenüber 6348 Sekunden. Dieser akzeptierte Grenzfall ist durch einen Regressionstest abgesichert.

## Fortschritt und Fehler

Das Protokoll ist standardmäßig aufgeklappt, kann eingeklappt und in die Zwischenablage kopiert werden. Es zeigt sowohl FFmpeg als auch MP4Box. Ein konkreter Fehlergrund bleibt im Statusbereich sichtbar, selbst wenn nachgereichte Fortschrittsmeldungen eintreffen. Bei Fehler gibt es kein automatisches Schließen.

## Verifikation

- Historischer vollständiger Testlauf am 10.10.2026: **743/743 Tests bestanden**, Release-Rebuild unter Buildnummer 11 ohne Warnungen oder Fehler; damaliger nativer Release-Build mit **25/25 Rust-Tests**. Aktueller bestätigter Paketprüfstand: 785 CAN-/40 Rust-Tests, siehe oben. Ein künstliches H.264-Video mit zwei AAC-Tonspuren wurde über CAN geschnitten: genau eine Indexierung, 70 Videoframes, 348 identische decodierte Bild-/Ton-Datensätze einschließlich Zeitstempeln und bytegleiche Ausgabe zu CAN 04. Ein einzelner AVI-Abschnitt wurde als tatsächliches MP4 ausgegeben. Die integrierte Werkzeugpfadübergabe funktioniert auch ohne FFmpeg/Indexer im Prozess-PATH. Motorwahl in normaler/kleiner WPF-Ansicht und isolierter Einstellungsdatei geprüft. Die folgenden realen Navy-CIS-/Kimi-Prüfungen bestätigen Referenztreue und decodierte Bildanzahl mit den dokumentierten Zeitstempelvorbehalten.
- P1 abgeschlossen: Schutz übernommener Namensvorschläge, bewusste Neuberechnung aus der Namensmaske, automatische Herkunftsangabe bei Fremd-Cutlists, einheitliche Zeitdarstellung und kopierbare Namensvorschau. Die Herkunftsübernahme ist durch sieben neue Testfälle und praktische Prüfungen abgesichert.
- Diplomatin und Rubikon: integrierter AVI-Ablauf mit jeweils zwei Cutlists erfolgreich; Wiedergabe einschließlich Anfang, Ende und Ton-Synchronität vom Nutzer bestätigt.
- Zusätzlich bestätigter Diplomatin-Schnitt mit HQ-Cutlist trotz unterschiedlicher Quelldateigröße.
- Enigma: AVI mit H.264/MP3 und 25 fps erfolgreich verlustfrei vorbereitet und mit MP4Box geschnitten. Die fertige MP4 enthält 162.246 Videopakete. Anfang, Mitte, Ende, Abspann und Ton wurden praktisch geprüft. Ein vollständiger Decoderlauf mit normalisierten Ausgabezeitstempeln endete ohne Fehlermeldungen.
- Bei Enigma traten beim Remux fehlende PTS-Werte und beim ersten Decoder-Prüflauf zwei DTS-Meldungen auf. Die genaue Ursache ist nicht abschließend geklärt; die praktische Qualitätskontrolle war erfolgreich.
- „Wo die Liebe hinfällt“: großer Full-HD-AVI-Container trotz Endung `.avi.mp4`, durchschnittlich 50 fps und zwei Tonspuren. Bildfehler und Probleme beim framegenauen Zurückspringen in CAN beobachtet; Tonversatz in MPC-HC berichtet. Keine erfolgreiche Schnittabnahme für diese Datei. Am 11.10.2026 bestätigt der Nutzer das Original als bereits untauglich und beauftragt ausdrücklich keine weitere Untersuchung dieser Aufnahme; er wartet auf einen neuen Sendetermin.
- Fehleranzeige mit absichtlich ungültigem ffprobe-Pfad live bestätigt.
- Kleine künstliche AVI-/MKV-Dateien: Prozessaufruf und Ergebnisprüfung, Ablehnung existierender Arbeitsdateien und vorab abgebrochener Aufträge geprüft.
- Leere Cutlist-Endbereiche sind automatisiert mit den Diplomatin-Schnittwerten abgesichert. Ein separater ausdrücklicher Live-Nachweis für genau den zuvor fehlerhaften Originaleintrag ist nicht protokolliert.

Weitere Prüfschritte stehen im [Testplan](TESTPLAN.md). Aus diesen Beispielen folgt keine allgemeine Unterstützung aller AVI-/Codec-Kombinationen.

## Abnahme und nächste Schritte

Am 11.10.2026 bestätigt der Nutzer die Punkte **„1) check / 2) check“** der gemeinsam besprochenen Aufgabenliste.

- [x] **1. Weitere Praxistests:** vom Nutzer für den aktuellen Stand mit otr-can 0.1.4 als abgeschlossen bestätigt. Die einzelnen Filmbeobachtungen und technischen Prüfgrenzen bleiben in den jeweiligen Berichten dokumentiert.
- [x] **2. AVI-Sonderfälle und Warnhinweis:** gelber Originalhinweis umgesetzt und geprüft, vom Nutzer bestätigt. Die als untauglich bestätigte alte Aufnahme „Wo die Liebe hinfällt“ wird ausdrücklich nicht weiter untersucht; der Nutzer wartet auf einen neuen Sendetermin.
- [x] **3. Nächste Veröffentlichung vorbereiten:** lokaler 0.2.1-Build-12-RC3-Paketsatz samt otr-can 0.1.4, Quellen, Lizenztexten und Prüfsummen erstellt und automatisch geprüft. Überinstallation/Versionsanzeige sowie Windows-11-VM-Installation, beide Schnittmotoren, Bild/Ton, Deinstallation und separater Portable-ZIP-Test vom Nutzer bestätigt. Persönliche Paketabnahme abgeschlossen; [Prüfbericht](RELEASE-BUILD12-RC3-RESULT.md). Noch nicht veröffentlicht, Version 1.0 weiterhin nicht festgelegt.
- [x] **4. Ältere Aufgabenlisten abgleichen:** Merkliste und Testplan auf den bestätigten Stand gebracht; ältere Build-6-Releaseunterlagen und libmpv-Lizenzprüfung als historische Nachweise eingeordnet. Timeline-Klick, Fenstergrößen, GPL-3.0-or-later-Lizenzwahl, KI-Dokumentation und Paketprüfung sind erledigt. Der KI-Absatz im Credits-Dialog wurde zusätzlich ergänzt. Der alte Juli-libmpv-Kandidat ist durch den geprüften September-Stack abgelöst, dessen Quellen-/Relink-Pakete bereits mit RC2 veröffentlicht wurden.
- [x] **5. Aktualisierten Build 13 RC3 sichern und paketieren:** Commit c98041a, neues CAN-Quellarchiv, Setup, Portable-ZIP und vollständiger Quellen-/Prüfsummensatz erstellt und technisch geprüft. Vorbestehende Lockdateiänderungen und Build-12-Pakete erhalten; [Prüfbericht](RELEASE-BUILD13-RC3-RESULT.md).

- [x] **6. Build 13 RC3 veröffentlichen:** auf ausdrücklichen Nutzerauftrag am 11.10.2026 um 18:07:42 Uhr MESZ als öffentliche Vorabversion freigegeben. Sieben Uploads, Größen und GitHub-Digests vor/nach Freigabe geprüft; Tag verweist auf c98041a. [Veröffentlichungsnachweis](RELEASE-PUBLICATION-BUILD13-RC3.md). Eine zusätzliche persönliche Build-13-Paketprüfung ist bislang nicht dokumentiert und bleibt von der Veröffentlichung getrennt.

Für V2 vorgemerkt: Klebezentrum (experimentell) für mehrteilige Aufnahmen und Feinabstimmung der Schnittbereiche gemäß chrisdudes Hinweisen. Stapelverarbeitung und Smart Rendering bleiben spätere Wünsche. Eine automatische Neukodierung ist nicht Teil des aktuellen Vorbereitungsablaufs.

Das abschließende Code-Review ist erledigt. Beide Befunde wurden behoben und getestet: widersprüchliche Cutlist-Bereiche werden mit einer verständlichen Meldung abgelehnt (`fde1970`); MP4-Ausgaben werden erst nach erfolgreichem Zusammenfügen übernommen (`cdcff86`). Bei Fehler oder Abbruch wird die temporäre Ausgabe gelöscht, eine vorhandene Zieldatei bleibt erhalten.
