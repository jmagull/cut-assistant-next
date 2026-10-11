# Projektstatus

Stand: 11.10.2026. Dieser Überblick beschreibt den aktuellen Feature-Stand; ältere Projektdefinitionen und Architekturentscheidungen dokumentieren teilweise frühere Entwicklungsphasen.

## Aktueller Umfang

CAN verwendet C#, .NET 10 und WPF unter Windows 11 x64. ffprobe liefert Medieninformationen, mpv/libmpv übernimmt die Wiedergabe, MP4Box den Schnitt. FFmpeg bereitet bei Bedarf eine MP4-Arbeitsdatei vor.

Umgesetzt sind Medienanalyse und Wiedergabe, Frame-Navigation, Frame-Lupe mit Schnittkantenübernahme und Halbierungssuche, manuelle Schnittplanung, lokale Cutlists, Server-Suche/-Download/-Upload, Namensmasken, Herkunftsübernahme bei Fremd-Cutlists, MP4Box-Schnitt und experimentelle Vorbereitung anderer Container. Die Bedienung ist in der [Nutzeranleitung](NUTZERANLEITUNG.md) beschrieben.

Die optionale OTR-CAN-Integration ist als Schritt 5 umgesetzt: der gemeinsame unveränderliche Schnittauftrag hält Original-/Zielpfade und Keep-Segmente fest. Die Motorwahl erfolgt auf Benutzerwunsch über **Schneiden MP4Box (schnell)** oder **Schneiden otr-can (framegenau, langsam)** direkt unter der Schnittliste, jeweils für diesen Auftrag. **OTR-CAN-Werkzeuge …** bietet nur Pfade und Prüfungen; ein älterer gespeicherter Motorhaken wird ignoriert. OTR-CAN 0.1.4 erhält die konfigurierten FFmpeg-/ffprobe-Pfade. CAN indexiert einmal im eigenen Arbeitsordner, startet den CPU-Motor am Original, prüft die MP4-/Streamstruktur und übernimmt erst danach das Ergebnis. Abbruch beendet den Prozessbaum; nach Erfolg, Fehler oder Abbruch werden eigene Arbeitsdateien bereinigt. Die lokale Entwicklungskennung bleibt **0.2.1 · Build 11 · RC3**; die öffentliche Build-7-RC2-Ausgabe bleibt unverändert.

Aktueller regulärer Motor **0.1.4**: Frame-/Zeitstempelkorrektur und vollständige Audio-Neukodierung aus 0.1.3, zusätzlich stabiler Audiofilterzustand bei geänderten Downmix-Begleitdaten. CAN verarbeitet große Warnprotokolle gebündelt mit begrenzter Anzeige und vollständiger Kopiermöglichkeit. **40 Rust- und 753 CAN-Tests**, 24 gemeinsame Grenzfälle, echter WPF-Großprotokolltest und unabhängiger Radical-PCM-Vergleich beider Tonspuren bestanden. Radical-Korrektur technisch geprüft und vom Nutzer persönlich bestätigt: sauberer Übergang, synchroner Ton. Stereo wird als lauter als 5.1 wahrgenommen; der Nutzer bestätigt denselben Unterschied bereits im Original. Keine quantitative Pegelmessung vorgenommen. Details und regulärer Abschluss im [Radical-Korrekturbericht](OTR-CAN-RADICAL-KORREKTUR-20261011.md). Pines und die ursprüngliche 0.1.3-Übernahme bleiben im [0.1.3-Prüfbericht](OTR-CAN-0.1.3-PRUEFBERICHT.md) dokumentiert. MP4Box bleibt Standard, ältere EXEs erhalten. Weitere reale Formate und getrennte Tonspurkontrollen vor 1.0; kein Paket veröffentlicht.

Zusätzlicher persönlicher 0.1.3-Praxistest am 11.10.2026: **Torchwood / Vergiftet (HQ)** mit drei Behaltebereichen. Nutzer bestätigt **„kein Blitzer, ton ist top :-)“**. Das übermittelte Protokoll zeigt eine Indexierung, 77956 Video-Pakete / 25 fps sowie eine AAC-Stereospur / 48 kHz mit 149675520 decodierten Samples; jeweils 51:58.240, erfolgreicher nativer und CAN-Abschluss. Keine Laufzeit mitgeteilt. Ein-Tonspur-Test; weitere getrennte Mehrtonspurkontrollen bleiben offen. Details im [0.1.3-Prüfbericht](OTR-CAN-0.1.3-PRUEFBERICHT.md).

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

- „Videodatei laden“ bietet MP4, AVI, MKV und weitere Dateiendungen an. Entscheidend ist die Containeranalyse, nicht der Dateiname.
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

## Hilfe, Credits und Versionsanzeige

- Hilfe verlinkt Nutzeranleitung und GitHub-Projekt; Update/GitHub öffnet ebenfalls die Projektseite.
- Credits zeigt die abgestimmte Danksagung in einem scrollbaren Dialog mit Projektlinks.
- Version und lokale Buildnummer stehen im Fenstertitel und in der Hauptüberschrift. Aktueller Entwicklungsstand: 0.2.1 Build 11 RC3; öffentliche Vorabversion: 0.2.1 Build 7 RC2.
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

- Letzter bestätigter vollständiger Testlauf am 10.10.2026: **743/743 Tests bestanden**, Release-Rebuild unter Buildnummer 11 ohne Warnungen oder Fehler; nativer Release-Build mit **25/25 Rust-Tests**. Ein künstliches H.264-Video mit zwei AAC-Tonspuren wurde über CAN geschnitten: genau eine Indexierung, 70 Videoframes, 348 identische decodierte Bild-/Ton-Datensätze einschließlich Zeitstempeln und bytegleiche Ausgabe zu CAN 04. Ein einzelner AVI-Abschnitt wurde als tatsächliches MP4 ausgegeben. Die integrierte Werkzeugpfadübergabe funktioniert auch ohne FFmpeg/Indexer im Prozess-PATH. Motorwahl in normaler/kleiner WPF-Ansicht und isolierter Einstellungsdatei geprüft. Die folgenden realen Navy-CIS-/Kimi-Prüfungen bestätigen Referenztreue und decodierte Bildanzahl mit den dokumentierten Zeitstempelvorbehalten.
- P1 abgeschlossen: Schutz übernommener Namensvorschläge, bewusste Neuberechnung aus der Namensmaske, automatische Herkunftsangabe bei Fremd-Cutlists, einheitliche Zeitdarstellung und kopierbare Namensvorschau. Die Herkunftsübernahme ist durch sieben neue Testfälle und praktische Prüfungen abgesichert.
- Diplomatin und Rubikon: integrierter AVI-Ablauf mit jeweils zwei Cutlists erfolgreich; Wiedergabe einschließlich Anfang, Ende und Ton-Synchronität vom Nutzer bestätigt.
- Zusätzlich bestätigter Diplomatin-Schnitt mit HQ-Cutlist trotz unterschiedlicher Quelldateigröße.
- Enigma: AVI mit H.264/MP3 und 25 fps erfolgreich verlustfrei vorbereitet und mit MP4Box geschnitten. Die fertige MP4 enthält 162.246 Videopakete. Anfang, Mitte, Ende, Abspann und Ton wurden praktisch geprüft. Ein vollständiger Decoderlauf mit normalisierten Ausgabezeitstempeln endete ohne Fehlermeldungen.
- Bei Enigma traten beim Remux fehlende PTS-Werte und beim ersten Decoder-Prüflauf zwei DTS-Meldungen auf. Die genaue Ursache ist nicht abschließend geklärt; die praktische Qualitätskontrolle war erfolgreich.
- „Wo die Liebe hinfällt“: großer Full-HD-AVI-Container trotz Endung `.avi.mp4`, durchschnittlich 50 fps und zwei Tonspuren. Bildfehler und Probleme beim framegenauen Zurückspringen in CAN beobachtet; Tonversatz in MPC-HC berichtet. Keine erfolgreiche Schnittabnahme für diese Datei.
- Fehleranzeige mit absichtlich ungültigem ffprobe-Pfad live bestätigt.
- Kleine künstliche AVI-/MKV-Dateien: Prozessaufruf und Ergebnisprüfung, Ablehnung existierender Arbeitsdateien und vorab abgebrochener Aufträge geprüft.
- Leere Cutlist-Endbereiche sind automatisiert mit den Diplomatin-Schnittwerten abgesichert. Ein separater ausdrücklicher Live-Nachweis für genau den zuvor fehlerhaften Originaleintrag ist nicht protokolliert.

Weitere Prüfschritte stehen im [Testplan](TESTPLAN.md). Aus diesen Beispielen folgt keine allgemeine Unterstützung aller AVI-/Codec-Kombinationen.

## Nächste Schritte

1. Regulären Motor 0.1.3 mit weiteren Realfilmen und getrennten Tonspuren prüfen; anschließend Paket-/Lizenz- und Installationsabnahme für Version 1.0 vorbereiten.
2. Historische Player-/AVI-Sonderfälle bei gezielten Tests berücksichtigen. Am 11.10.2026 meldet der Nutzer aktuell keine Bildnavigation-Auffälligkeit; daraus folgt keine pauschale Abnahme aller Dateien.
3. UX-Merkliste: Fenstergrößen der Schnittdialoge und besonders von „Video vorbereiten und schneiden“ mit ausreichend großem Protokollbereich beim nächsten Aufruf/Programmstart wiederherstellen; ebenso die bereits vorgemerkte Größe von „Cutlist erzeugen“. Klick auf die Zeitleiste zum Positionieren und weitere Bedienungsdetails bleiben vorgemerkt. Siehe [Merkliste](SETUP-MERKLISTE.md).
4. Für AVI-Container mit irreführenden Dateiendungen wie `.mpg.HD.avi.mp4` einen deutlich hervorgehobenen Warnhinweis ergänzen. Der Hinweis soll erklären, dass Probleme bereits in der unveränderten Originaldatei vorliegen können. Die Verarbeitung bleibt möglich.

Für V2 vorgemerkt: Klebezentrum (experimentell) für mehrteilige Aufnahmen und Feinabstimmung der Schnittbereiche gemäß chrisdudes Hinweisen. Stapelverarbeitung und Smart Rendering bleiben spätere Wünsche. Eine automatische Neukodierung ist nicht Teil des aktuellen Vorbereitungsablaufs.

Das abschließende Code-Review ist erledigt. Beide Befunde wurden behoben und getestet: widersprüchliche Cutlist-Bereiche werden mit einer verständlichen Meldung abgelehnt (`fde1970`); MP4-Ausgaben werden erst nach erfolgreichem Zusammenfügen übernommen (`cdcff86`). Bei Fehler oder Abbruch wird die temporäre Ausgabe gelöscht, eine vorhandene Zieldatei bleibt erhalten.
