# AVI/MP3 mit otr-can – Umsetzung und Prüfung

Stand: 12.10.2026. Lokaler Stand CAN 0.2.1 · Build 15 · RC3 und otr-can 0.1.5. Kein Commit, keine Veröffentlichung und keine neuen Setup-Pakete. Öffentliches RC3-Paket und ältere Motor-EXEs bleiben erhalten.

## Ergebnis

Variante B mit vorhandener CAN-Vorbereitung umgesetzt: geeignete H.264-AVI-Dateien werden nach ausdrücklicher experimenteller Bestätigung mit `-map 0 -c copy` nach MP4 umgepackt. Video und MP3 werden dabei nicht kodiert. otr-can dekodiert MP3, bildet die Keep-Auswahl als PCM und kodiert die fertige Tonspur einmal nach AAC-LC (Stereo 192 kbit/s, Mono 96 kbit/s). AAC-/AC3-Einstellungen bleiben erhalten.

Die technische Audio- und Integrationsprüfung ist erfolgreich. Die neue Teufelsbrigade-Datei ist erzeugt und am 12.10.2026 persönlich positiv abgenommen: Anfang, Ende, alle Schnittübergänge sowie Ton und Bild. Eine zusätzlich gefundene Video-PTS-Auffälligkeit ist bereits in der vorherigen persönlich geprüften Ausgabe vorhanden. Kein uneingeschränkt grüner Voll-Videodekodiernachweis und keine allgemeine Freigabe beliebiger AVI-Dateien.

## Schritte 1–3: Motor und CAN

- Motor audio.rs: Eingangs- und Zielcodec getrennt; MP3 Mono/Stereo bei bisherigen 32/44,1/48 kHz zugelassen. AAC-LC-Profil explizit gewählt. MP3 benötigt für die feste Zielbitrate keine Eingangsbitrate; AAC/AC3 behalten ihre bisherigen Bitratenprüfungen. Ausgabe wird gegen Zielcodec, Layout, Rate, Samplezahlen und kontinuierliche PTS geprüft.
- Neue CLI-Abfrage `capabilities` meldet `mp3-aac-v1` ohne Datei-/Werkzeugzugriff. CAN benötigt diese Fähigkeit ausschließlich für MP3-Aufträge. Alte Motoren bleiben bei bestehenden AAC-/AC3-Aufträgen verwendbar.
- Cargo-Paketversion und eigener Lockeintrag auf 0.1.5; keine neuen Produktionsabhängigkeiten. Eigener Ausgabeordner `target/can-engine-0.1.5-build`, bisherige EXEs erhalten. Kein Eingriff in Video-Smart-Rendering oder Frame-/Keep-Zuordnung.
- ConfiguredCutEngineFactory bietet Vorbereitung für erkannten AVI-Container mit einer H.264-Videospur an Position 0. FfmpegRemuxRunner und VideoPreparation einschließlich exakter Videopaketprüfung wiederverwendet.
- OtrCanCutService analysiert/indexiert/schneidet SourceFilePath; OriginalFilePath bleibt die Originalidentität. Ein vorbereiteter MP3-Eingang darf nicht bereits AAC enthalten. Keep-Snapshot, Ziel, Namensmaske und Ausgabeverzeichnis bleiben erhalten.
- Hinweise erklären Experiment, einmalige verlustbehaftete Audiokodierung und nötige Kontrolle jeder Tonspur/aller Übergänge. Fortschritt unterstützt die bisherigen und neuen Motorphasen. Abbruch und private Arbeitsdateibereinigung bleiben erhalten.
- FFmpeg/ffprobe bleiben separat konfiguriert und werden nicht zusätzlich mit CAN ausgeliefert.

Geändert: sechs CAN-Produktionsdateien (MainWindow, ConfiguredCutEngineFactory, OtrCanCutService, OtrCanProgressReporter, VideoPreparation, OtrCanToolChecker), drei CAN-Testdateien, Dokumentation; im Motor audio.rs, main.rs, CLI-Tests, Cargo-Version/Lockeintrag und Dokumentation.

## Schritt 4: Build, Tests und Regressionen

- CAN-Release-Build 15 mit tools/build.ps1 -Configuration Release -NoRestore: 0 Warnungen, 0 Fehler.
- 793/793 CAN-Tests: Core 74, Cutlists 65, Media 125, App 529; 0 fehlgeschlagen/übersprungen.
- 44/44 Rust-Tests: 38 Bibliotheks- und 6 CLI-Tests; 0 fehlgeschlagen/ignoriert. Formatprüfung und separater Release-Build erfolgreich.
- Neue Tests für MP3-Zielwahl, Erhaltung bestehender Audioziele, ungültige MP3-Formate, gezielte Fähigkeitserkennung, Arbeitsdateiübergabe, fehlende Arbeitsdatei, Vermeidung einer AAC-Vorkodierung sowie Benutzer-/Fortschrittstexte. Vorhandene Fehler-/Abbruch-/Werkzeug-/Bereinigungstests weiterhin bestanden.
- Kein Produktions-Restore. Alle acht CAN-Lockdateien per SHA-256 gegen den Aufgabenbeginn geprüft und unverändert; vier vorbestehende Produktionsänderungen erhalten.

Vier echte synthetische CAN-Aufträge mit Unicode-/Leerzeichenpfaden und ohne FFmpeg im Prozess-PATH:

| Video | Ton | Ergebnis |
|---|---|---|
| 25 fps | MP3 Stereo 48 kHz | bestanden |
| 25 fps | MP3 Stereo 44,1 kHz | bestanden |
| 50 fps | MP3 Mono 32 kHz | bestanden |
| 50 fps | MP3 Stereo 48 kHz + AAC Stereo 44,1 kHz + AC3/5.1 48 kHz | bestanden |

Jeweils vier Keep-Bereiche inklusive einzelner Frames, 96 exakte nummerierte Videobilder, eine Indexierung, unveränderte Original-/Indexdaten und bereinigte Arbeitsordner. Alle sechs PCM-Auswahlen vor Kodierung bytegenau zu einer unabhängigen vollständigen Originaldekodierung mit sampleweiser Keep-Auswahl. Die verlustbehaftete AAC-Ausgabe selbst wird nicht als PCM-bytegleich bezeichnet. Sprach-/Dispositionsdaten zusätzlich vom Motor geprüft.

AAC-/AC3-Regressionsauftrag mit 0.1.4 und 0.1.5: vollständige Ausgaben bytegleich, SHA-256 E35F2E362254922D03FA9A3EE033177573F7998BFB8CCE01911B085243B2EDD3.

MP4Box über tatsächlichen CAN-Dienst: MP4 mit drei Tonspuren und AVI mit bestehender Vorbereitung erfolgreich; Originale und Streamcodecs erhalten. Kein neuer MP4Box-Framegenauigkeitsnachweis daraus abgeleitet.

Negativtest: um 120 ms verzögerter MP3-Ton bei Keep ab Null wird wegen falscher PCM-Samplezahl abgelehnt. Keine fertige Ausgabe, Original erhalten, Arbeitsbereich bereinigt; keine automatische Audio-Reparatur eingeführt.

## Schritt 5: Die Teufelsbrigade

Original-AVI und zugehörige Cutlist bleiben unverändert. Die sechs Starts/Dauern werden über vorhandene CAN-Lese-/Planfunktionen unverändert übernommen; bisherige native Mikrosekunden-/Framezuordnung bleibt erhalten.

- CAN-Ablauf mit Vorbereitung, genau einer Indexierung und Ausgabeprüfung: 235,18 s, inklusive PCM-Prüfinstrumentierung; kein isolierter Motorbenchmark.
- 182.574 Videopakete, nominal 7.302,960 s; 350.542.080 decodierte Audio-Samples bei 48 kHz, Stereo; AAC-LC-Ziel 192 kbit/s.
- PCM vor Kodierung bytegenau zur unabhängigen Auswahl aus dem ursprünglichen AVI: SHA-256 D61B6A508F43EDFF3DBC634BC86917F7BB4912144D76D728F6F2C229B5E1C409.
- Alle drei Motorstufen und finale Paket-/Zeitstempel-/Mux- sowie CAN-Ausgabeprüfung erfolgreich. Private Motorarbeitsdaten und umgepackte Quelle gelöscht. Bewusst kopierte PCM-Prüfreferenz bleibt als Nachweis erhalten.
- Ausgabe: `.build/avi-mp3-20261012/real-film/Die Teufelsbrigade otr-can 0.1.5 [21.01.2021].mp4`, 959.777.146 Byte; SHA-256 E13DDE91BAFB78BE4E8B32BA5C22619BB35AB07E0EF9090E37A243B951871076.

### Offener, vorbestehender Video-Befund

Ein zusätzlicher unabhängiger FFmpeg-Null-Ausgabelauf meldet nicht monoton steigende Video-DTS. Gezielte ffprobe-Prüfung: erste Abweichung bei decodiertem Frame 192, PTS 7,760 statt nominal 7,680 s. Die ersten zehn Sekunden zeigen dieselbe decodierte PTS-Folge in der neuen und der zuvor persönlich positiv geprüften Datei „Die Teufelsbrigade otr-can [21.01.2021].mp4“.

Der vollständige komprimierte Videostream beider Dateien ist bytegleich: SHA-256 1550C894CAE68ADD7297C55B48E6A09E5A5A5904B8DE6D27896689707B127AD5. Dieser Befund ist somit keine neue Videoänderung der MP3-Erweiterung. Ursache und Player-Auswirkungen hier nicht abschließend untersucht. Die bisherige positive persönliche Bewertung bleibt erhalten, ersetzt aber keine bestandene strenge Dekodierprüfung. Keine Warnung unterdrückt, keine Video-Schutzprüfung abgeschwächt.

Weitere künstliche AVI-Versuche werden wegen unpassender CFR-/Präsentationszeiten abgelehnt; eine synthetische MP4 mit Audio-Titel erzeugt eine FFmpeg-UDTA-Warnung und wird ebenfalls abgelehnt. Der Prüfhelfer wurde zunächst korrigiert: AVI-avg_frame_rate 600 statt der erzeugten 25 fps sowie ein falsches FPS-Argument hatten ungeeignete Test-Keep-Zeiten verursacht. Produktschnittpunkte wurden dafür nicht verändert. Erstversuche und Diagnoseprotokolle bleiben erhalten.

Der lokale Motorpfad wurde nach Prüfung über CANs eigene Einstellungsverwaltung auf die separate 0.1.5-EXE gestellt. Vorherige Einstellung: .build/avi-mp3-20261012/otr-can-settings-before.json. Indexerpfad und alte installierte EXE unverändert; SHA-256-Erhaltung in der zugehörigen verification.json bestätigt. Die installierte CAN-Ausgabe wird dadurch nicht aktualisiert.

## Persönliche Filmabnahme – 12.10.2026

Nutzer bestätigt wörtlich: **„anfang ende, alle schnitte, ton und bild prima :-)“**. Positive persönliche Abnahme der neuen Teufelsbrigade-Testdatei mit otr-can 0.1.5; keine neue Messung oder erneute Motor-/GUI-Ausführung aus dieser Nachricht abgeleitet.

Die zuvor zur Kontrolle angegebenen Positionen im Ergebnis sind aus den tatsächlich verarbeiteten Framezahlen bei 25 fps berechnet:

| Bereichsübergang | Position im Ergebnis |
|---|---|
| 1 → 2 | 00:24:48,200 |
| 2 → 3 | 00:45:38,840 |
| 3 → 4 | 01:05:41,600 |
| 4 → 5 | 01:25:17,720 |
| 5 → 6 | 01:50:13,080 |

Anfang 00:00:00, Ende nominal 02:01:42,960. Die vorbestehende technische PTS-/DTS-Auffälligkeit bleibt dokumentiert; die persönliche positive Bewertung wird nicht als bestandene strenge Videodekodierprüfung ausgegeben.

## Nachweise und nächster Schritt

Lokale Nachweise ausschließlich unter `.build/avi-mp3-20261012`: BUILD-FINAL.log, TESTS-FINAL.log, RUST-TESTS.log, LOCKS-FINAL.json, matrix-positive-v2/*/can/{RESULT,AUDIT}.json, regression/RESULT.json, mp4box-*/RESULT.json, negative-audio-offset/RESULT.json und real-film/{RESULT,AUDIT-PCM}.json. REAL-AUDIT.log und Vergleichsdateien dokumentieren den fehlgeschlagenen strengen Video-Dekodierlauf. Prüfhelfer/Medien bleiben ignoriert und werden nicht in Git aufgenommen.

Persönliche Abnahme der neuen Datei am 12.10.2026 abgeschlossen: Anfang/Ende, alle fünf Bereichsübergänge, Ton und Bild vom Nutzer als einwandfrei bestätigt. Die Rückmeldung betrifft den Film; daraus wird keine zusätzliche GUI-, Installations- oder quantitative Synchronitätsmessung abgeleitet. Eine Reparatur der vorbestehenden Video-PTS-Auffälligkeit wäre ein eigener Schritt; keine pauschale Videoneukodierung eingeführt. AVI-Anbindung erfordert den neuen lokalen CAN-Code; die installierte öffentliche Build-13-Ausgabe enthält sie nicht.