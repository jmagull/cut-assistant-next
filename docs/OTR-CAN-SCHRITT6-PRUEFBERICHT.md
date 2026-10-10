# OTR-CAN – reale Referenzprüfung, Schritt 6

Stand: 10.10.2026. CAN **0.2.1 · Build 11 · RC3**, nativer Motor **0.1.1**.

Die beiden realen HD-Aufnahmen wurden mit ihren vorhandenen CAN-Cutlists über CANs Import, Schnittauftrag, Motorwahl und Schnittdienst verarbeitet. Beide neuen MP4-Ausgaben sind **bytegleich mit einem frischen CAN-04-Schnitt** aus demselben Original und denselben einmal erzeugten Indexdaten. Originale, Cutlists, bisherige Ausgaben, Referenz-EXEs und produktive Einstellungen bleiben erhalten.

Die Referenztreue und Decoderprüfung sind bestätigt. Der Nutzer hat anschließend beide neuen Ausgaben persönlich positiv bewertet: **„navy perfekt :-)“** und **„kimi perfekt“**. Die Rückmeldungen nennen die beiden Tonspuren nicht einzeln; deren getrennte Hörprüfung wird daraus nicht zusätzlich abgeleitet. Eine uneingeschränkte Qualitäts-/Releasefreigabe folgt daraus nicht: geerbte Zeitstempelauffälligkeiten an Teilübergängen bleiben offen.

## Daten und Ergebnis

### Ergänzung: direkter CAN-Bedientest mit Wo dein Herz schlägt

Am 10.10.2026 bestätigt der Nutzer die HQ-Aufnahme mit fünf Behaltebereichen für beide Motoren als **„MP4Box und otr-can beide sauber :-)“**. Die otr-can-Laufzeit empfindet er hier als in Ordnung, anders als zuvor beim HD-Film The Place Beyond the Pines. Das übermittelte Motorprotokoll zeigt eine Indexierung, fünf abgeschlossene Bereiche, CPU-Endkanten und erfolgreichen Schnitt einschließlich Ausgabeprüfung und Bereinigung. Die Rückmeldung betrifft den bisherigen Motor 0.1.1 und wird nicht als persönliche Abnahme der anschließend gestarteten Seek-Versuchsvariante ausgegeben.

### Ergänzung: direkter CAN-Bedientest mit The Place Beyond the Pines

Am 10.10.2026 hat der Nutzer die HD-Aufnahme mit derselben Fünf-Bereiche-Cutlist über MP4Box und otr-can geschnitten, ohne die Frame-Lupe zu verwenden. Nach persönlicher Kontrolle bestätigt er **„die schniite MP4Box und otr-can beide super, in beiden fällen keine Blitzer, lupe wurde nicht benutzt“**. Anders als bei Radical sind in diesem Vergleich beide Ergebnisse an den geprüften Schnittstellen positiv bewertet. Die native Laufzeit betrug laut Nutzer knapp 15 Minuten; das Nutzerprotokoll bestätigt eine Indexierung, CPU-Kantenbearbeitung, Zusammenfügen, Bereinigung und erfolgreiche Ausgabeprüfung.

Die nominalen Behaltebereiche bei 25 fps sind 1509,320–2606,200 s, 3295,440–4047,640 s, 4671,520–5787,960 s, 6052,120–7993,800 s und 8680,160–9277,9599999 s. Daraus berechnete Übergänge im geschnittenen Film: 18:16,880, 30:49,080, 49:25,520 und 1:21:47,200; Sollende rund 1:31:45,000.

Eine reine ffprobe-Metadatenprüfung bestätigt in beiden Ausgaben H.264/1920 × 1080 sowie AAC-Stereo und AC-3-Stereo mit 48 kHz, AC-3 bei 384.000 bit/s. MP4Box meldet 137.613 Videoframes und 5504,520 s Videodauer; otr-can meldet 137.626 und 5505,040 s, also ein Bild/40 ms mehr als die nominalen 137.625 Bilder. Native Containerdauer wegen Audioende 5505,154667 s. Dieser Unterschied bleibt als technischer Befund offen; die positive persönliche Schnittbewertung ersetzt keinen vollständigen Decoder-/Zeitstempelvergleich oder eine getrennte Tonspurprüfung. Filme und Cutlist wurden durch den Assistenten nicht verändert oder erneut geschnitten. Lokale Metadatenbelege liegen im ignorierten `.build/mp4box-completion/`.

### Ergänzung: direkter CAN-Bedientest mit Radical

Am 10.10.2026 hat der Nutzer Radical über beide Schnittbuttons in CAN geschnitten. Verwendet wurde dieselbe unveränderte Cutlist, ohne Frame-Lupenbearbeitung: 950,780–4397,640 s und 4814,560–8376,780 s, jeweils bei 50 fps. Der MP4Box-Schnitt zeigt am Übergang bei etwa 57:26,873 einen kurzen SRF-zwei-Einblender. Für den otr-can-Schnitt bestätigt der Nutzer ausdrücklich **„Blitzer bei 57:26 ist weg :-)“**. Dies belegt den Erfolg an dieser konkreten Schnittstelle direkt über die normale CAN-Oberfläche.

Das Nutzerprotokoll zeigt Werkzeugprüfung, einmalige FFMS2-Indexierung, CPU-Schnitt beider Bereiche, Ausgabe-/Tonspurprüfung und erfolgreichen Abschluss. Die gemeldeten Positionen im Ergebnis sind Anfang 0:00, Übergang 57:26,860 und berechnetes Ende 1:56:49,080.

Eine anschließende reine Metadatenprüfung der fertigen Datei bestätigt H.264/1280 × 720, gemeldete 350.454 Videoframes und 7009,080 s Videodauer, AAC-Stereo/48 kHz sowie AC-3/48 kHz mit sechs Kanälen und 384.000 bit/s. Die Containerdauer beträgt 7009,165334 s. Für Radical wurde durch den Assistenten kein weiterer Schnitt oder vollständiger Decodervergleich gestartet; separate persönliche Tonspurprüfungen werden nicht aus der Blitzer-Rückmeldung abgeleitet. Die früher dokumentierten Zeitstempelgrenzen bleiben bestehen.

### Ursprüngliche Referenzprüfung: Navy CIS und Kimi

| Merkmal | Navy CIS, 27.09.2026 | Kimi, 04.10.2026 |
|---|---|---|
| Original | HD.mp4, 2.274.178.477 Bytes | HD.mp4, 1.638.255.708 Bytes |
| Video | H.264, 1920 × 1080, 25 fps | H.264, 1280 × 720, 50 fps |
| Behaltebereiche | 592,600–2036,520 s; 2692,520–3650,880 s | 908,880–3634,520 s; 4057,980–6293,540 s |
| Soll-Videodauer | 2402,280 s | 4961,200 s |
| Decodierte Videoframes | **60.057**, exakt Soll | **248.060**, exakt Soll |
| Tonspur 1 | AAC, Stereo, 48 kHz | AAC, Stereo, 48 kHz |
| Tonspur 2 | AC-3, Stereo, 48 kHz | AC-3, **6 Kanäle**, 48 kHz |
| Gemeldete AAC-Ausgaberate | 165.884 bit/s | 160.394 bit/s |
| AC-3-Ausgaberate | **384.000 bit/s** | **384.000 bit/s** |
| MP4-Containerdauer | 2402,328 s | 4961,240 s |
| Ausgabegröße | 767.497.957 Bytes | 901.354.313 Bytes |
| FFMS2-Indexierungen | **1** | **1** |
| Original/Index unverändert, Arbeitsdateien bereinigt | bestätigt | bestätigt |

Die gemeldete mittlere AAC-Bitrate eines geschnittenen VBR-Streams muss nicht exakt der mittleren Rate des Originals entsprechen. CPU-Encoding, korrigierte Seek-Reihenfolge, vollständiges `-map 0` und die übergebenen Audiobitratenparameter wurden nicht verändert; die Bytegleichheit mit CAN 04 umfasst auch die Tonspuren.

SHA-256 der neuen Ausgabe und der jeweils frischen CAN-04-Referenz:

- Navy CIS: `7aa2c16cdf35ddc25d76be920a6ef83db6d6bf4cd5e8a58699a1a7c4ca44509d`
- Kimi: `b127b0d9dff0058d91547b3b9165ff6f6fca51308ab7bb12e23e3665b3858726`

Navy CIS ist zusätzlich bytegleich mit der vorhandenen, früher abgenommenen `otr-native-navy-ffmpeg9-cpu`-Ausgabe. Die ältere Kimi-CPU-Ausgabe ist nicht bytegleich: sie enthält 248.061 statt 248.060 Videoframes und 20 ms mehr Videodauer. Ihr Hash ist `b550773a3b44d79e2329c3cd8aa9d9a86099fc43235d7da48ef91f4e64fb8e83`. Die früheren vollständigen Aufruf-/Cutlistparameter liegen nicht als belastbarer Nachweis vor; die Ursache wird deshalb nicht erfunden. Die aktuelle Ausgabe trifft die Soll-Bildanzahl und ist bytegleich mit CAN 04 bei identischem aktuellem Auftrag.

## Prüfablauf und praktische Grenzen

- Vor und nach dem Schnitt wurden Original, Cutlist, ältere CPU-Ausgabe und verwendete Motor-EXEs gehasht. CANs Cutlist-Import und anschließende Keep-Erzeugung erhalten Start und Dauer exakt. Die bestehende MP4Box-Einstellung in einer Cutlist bestimmt nicht den gewählten Motor.
- Die isolierte Prüfhilfe verwendet die gebauten CAN-Komponenten einschließlich Motorfabrik, Werkzeugprüfung, Prozessstarter und Schnittdienst. Sie liest/speichert keine produktive Motorwahl. Der Prozess-PATH enthält für die neue Variante kein FFmpeg/ffprobe/ffmsindex; deren konfigurierte vollständige Pfade werden verwendet. Keine WPF-Bedienabnahme eines neuen Live-Schnitts behauptet.
- CAN erzeugt genau einen neuen Index pro Film. CAN 04 liest anschließend dieselben Indexdateien unverändert, bevor CAN seinen Arbeitsbereich bereinigt. Die Vergleichsreferenz erhält lediglich die sechsstellige Zeitdarstellung ihrer bisherigen CLI; hier sind die siebten CAN-Tickstellen jeweils null.
- Beide kompletten Ausgaben wurden mit Softwaredecodern, `-err_detect explode` und `-xerror` auf decodierbare Bild-/Tondaten geprüft. Alle Videoframes und alle Audioframes werden exportiert; keine Bilder dupliziert oder entfernt. Prüfbilder von Anfang, Übergang und Ende sowie weiteren Filmszenen wurden gesichtet. Kurze Vorschauen liegen für die persönliche Prüfung bereit; deren Stereo-AAC-Vorschau ersetzt nicht das Prüfen beider Tonspuren in der vollständigen MP4.
- PCM-Vergleiche mit dem Original an drei aussagekräftigen Stellen je Tonspur dienen als lokale Zeitplausibilität: Navy CIS etwa +18,5 bis +26 ms, Kimi etwa −21,5 bis −1,5 ms gegenüber der nominalen Keep-Timeline. Das sind Inhaltskorrelationen, keine vollständige Lippen-Synchronitätsmessung. Der frühe Kimi-Logo-/Musikabschnitt ist für diese Korrelation mehrdeutig; die zusätzliche Dialogstelle bei 60 s liefert verwertbare Übereinstimmung. Die anfängliche Audio-Prüfmethode mit Seek-/Fensterproblemen wurde korrigiert; vorläufige Werte werden nicht als Tonversatz ausgegeben.

## Zeitstempelvorbehalt

Ein erster unnormalisierter Navy-CIS-Framehash-Lauf brach am Übergang mit `Non-monotonic DTS` ab. Der Fehler wurde erhalten und untersucht. Die MP4-Videopakete haben in beiden Dateien durchgehend steigende DTS; die ursprünglichen decodierten Video-PTS sind jedoch nicht durchgehend streng steigend:

- Navy CIS: ein Rücksprung von 40 ms um 1443,8 s und ein doppelter PTS nahe dem Ende.
- Kimi: ein doppelter PTS um 2722,7 s innerhalb des Übergangs vom kopierten zum neu kodierten Teil.

Die vollständige anschließende Decoderprüfung protokolliert diese ursprünglichen PTS mit `showinfo` vor dem Filter und verwendet **nur für den Prüfexport** fortlaufende Video-PTS mit `setpts`. Das beseitigt die Anforderung des Framehash-Muxers, ohne decodierte Bilder zu entfernen oder zu ergänzen. Die fertigen Filme werden nicht umgeschrieben. Die erfolgreiche Decoderprüfung darf deshalb nicht als bestandene Prüfung unveränderter Originalzeitstempel dargestellt werden.

Auch die unverändert exportierten Audio-PTS haben einzelne Abstands-/Überlappungsabweichungen: Navy AAC 15 Stellen, maximal absolut 40 ms; AC-3 10 Stellen, maximal etwa 66,7 ms. Kimi AAC 10 Stellen, maximal etwa 21,3 ms; AC-3 9 Stellen, maximal etwa 32 ms. Decoderfehler wurden im vollständigen Prüflauf nicht gefunden. Aufgrund der Bytegleichheit bestehen diese Merkmale auch in den frischen CAN-04-Referenzen; bei Navy CIS ebenso in der früher abgenommenen CPU-Ausgabe.

Vor einer Veröffentlichung sollen Teilübergänge und Zeitstempel gesondert untersucht und die tatsächliche Wiedergabe mit beiden Tonspuren persönlich bestätigt werden. In Schritt 6 wurde kein stiller Frame-/Zeitversatz, keine neue Schnittkorrektur und keine Änderung am Referenzmotor eingeführt.

## Belege und Erhaltung

Private Daten liegen ausschließlich unter dem ignorierten `.build/otr-can-step6/`: `navy/` und `kimi/` mit Ausgaben, Original-/Referenzhashes, Schnittprotokollen, Metadaten, vollständigen Framehashes, Rohzeitstempelauswertung, Audiovergleichen, Prüfbildern und Start-/Übergangs-/Endvorschauen. Reale Filme, Cutlists und Vorschaubilder gehören nicht in Git oder ein Releasepaket.

Produktionscode, Rust-Code, Referenz-EXEs, Nutzer-Lockdateien und Buildzähler wurden nicht verändert. Dokumentation und lokale Prüfhilfen wurden ergänzt. Abschlussprüfung: Release-Rebuild ohne Warnungen/Fehler und **724/724 CAN-Tests bestanden**. Version bleibt **0.2.1 · Build 11 · RC3**. Keine Commits, Pushes, neuen Repositories, Installationspakete oder Veröffentlichungen.
