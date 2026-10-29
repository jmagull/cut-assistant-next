# Klebecenter – zwei Audioverfahren: Machbarkeit und Architekturvorschlag

**Stand: 28.10.2026. Nur Untersuchung und Vorschlag; noch keine Produktimplementierung.** Stabil veröffentlicht: CAN 1.0.2 Build 41. Lokaler Entwicklungsstand: Build 42, otr-can unverändert 0.1.6. Keine neue Buildnummer, Pakete, Git-Aktionen oder Veröffentlichung.

## Ergebnis und vorgeschlagene Entscheidung

Zwei bewusst gewählte Schaltflächen sind sauber integrierbar. **Audio-Streamcopy ist als streng begrenztes Verfahren technisch tragfähig, nicht als allgemeiner Ersatz für die durchgehende Neukodierung.** Ein kleiner positiver Grenztest ist nachgewiesen. Für die beiden aktuellen John-Wick-Dateien ist der einfache AAC-Paketanschluss mit unverändertem FFmpeg-MP4-Verfahren nicht sicher möglich: An der inneren Naht wären Endpadding und neues Priming zu trimmen; der strenge Versuch bricht bereits wegen nicht monotoner DTS ab.

Empfohlen zur gesonderten Freigabe: ein konservativer erster Kopiermodus mit klarer Ablehnung solcher Fälle und zusätzlichem vollständigem PCM-Vergleich. Er verändert keine Pakete, entfernt keine Priming-Pakete, repariert keine Zeitstempel und kodiert auch keine kleine Nahtzone neu. Viele separat mit otr-can kodierte Filmteile können deshalb zunächst nur den bewährten Weg verwenden. Ob der verbleibende Nutzen diesen eng begrenzten Modus rechtfertigt, ist vor der Umsetzung ausdrücklich zu entscheiden.

Der Auftrag verlangt in Abschnitt 7: „Vor der eigentlichen Implementierung bitte einen kleinen Architekturvorschlag und eine fachliche Machbarkeitsbewertung für Audio-Streamcopy vorlegen.“ Die Implementierung folgt erst nach ausdrücklicher Freigabe.

## Vorhandener Build-42-Ablauf

Untersucht: Mp4JoinService.cs, FfprobeStreamCopyVerifier.cs, FfprobeStreamCopyVerifier.Join.cs, JoinPhaseTimer.cs, JoinDialog.xaml/.xaml.cs, MainWindow.Join.cs und BoundedJoinToolRunner.cs; vorhandene Klebecenter-Dokumentation und Tests.

Die Anfrage enthält bisher zwei Dateien, explizite Tonspurindizes und eine neue Ausgabe. Der Dienst prüft gemeinsame Bild-/Tonformate und jede Quellzeitachse. Gewählte Spuren werden streng als Float32-PCM dekodiert, direkt in geprüftem RF64 verbunden und einmal durchgehend als AAC-LC kodiert. Video wird je Teil unverändert vorbereitet; sämtliche Videozeitpunkte/-pakete, das finale AAC-Umpacken, Samples und Vorschau werden geprüft. Erst danach wird die neue Datei ohne Überschreiben übernommen. Eingaben bleiben gegen Schreiben gesperrt. Fehler/Abbruch behalten das Diagnoseprotokoll; Erfolg räumt bekannte Dateien auf.

Die heutige Quelltonprüfung erlaubt Priming ganz am Anfang und Endpadding ganz am Ende jeder einzelnen Quelle. Das ist für deren anschließende PCM-Zusammensetzung korrekt. Daraus folgt jedoch keine Erlaubnis, dieselben Trimmangaben mitten in einer verbundenen AAC-Paketfolge zu übernehmen. Die bestehende Prüfung der einmal neu kodierten durchgehenden Ausgabespur setzt gerade keine innere Priming-/Paddingstelle voraus.

## Reale Referenz John Wick 4

Beide vom Nutzer genannten Dateien sind jetzt vorhanden; gelesen wurden ausschließlich diese benannten Dateien. Explizit untersucht: **Spur 1, AAC-LC, Deutsch, Stereo, 48000 Hz**, Zeitbasis 1/48000, Präsentationsstart null.

| Eigenschaft | Teil 1 | Teil 2 |
|---|---:|---:|
| Dateiname | John Wick Kapitel 4 Teil 1.mp4 | John Wick Kapitel 4 Teil 2.mp4 |
| Dateigröße | 2419311231 Bytes | 709188070 Bytes |
| Bild-/Tonlaufzeit | 7057,120 s | 2472,040 s |
| Nutzsamples je Kanal | 338741760 | 118657920 |
| AAC-Pakete einschließlich Priming | 330804 | 115878 |
| Übersprungener Anfang | 1024 Samples | 1024 Samples |
| Verworfenes Ende | 512 Samples | 128 Samples |

Beide AAC-Konfigurationen haben denselben SHA-256 **b3108813350006ef07740046fab14af9c374938867d16db02fe1a8baa5e72f10**. Beide Videos sind progressives H.264 High, 1920×1080, 25 fps, Zeitbasis 1/12800 und haben dieselbe Konfiguration. Die vollständige AAC-Paketprüfung findet in beiden Quellen keine inneren Lücken oder Überlappungen. Jedes codierte AAC-Paket umfasst hier 1024 Samples; die Nutzlaufzeiten berücksichtigen die individuellen Randtrimmungen.

Die beiden AAC-Spuren wurden erneut vollständig mit Fehlerabbruch dekodiert: **338741760 + 118657920 = 457399680 Samples je Kanal**, **3659197440 Float32-PCM-Bytes**. Der SHA-256 der in Reihenfolge gelesenen PCM-Daten ist **44eb4d7f104fd64405528d8ac7cd52538fb0866af7434d4ea9a5faebf7132a10** – identisch zum erhaltenen PCM-PROOF.json der externen Referenz vom 26.10.2026. Damit ist der dekodierte gewählte Ton dieser neuen Dateinamen zur historischen PCM-Referenz nachgewiesen; eine vollständige Bytegleichheit der MP4-Dateien wird daraus nicht abgeleitet.

### Konkreter Kopierversuch und Hinderungsgrund

Nur als Diagnose wurde eine getrennte Audio-Datei im ignorierten Prüfverzeichnis versucht, mit ausdrücklich gesetzten Dauern 7057.1200000 und 2472.0400000, concat-Demuxer, auto_convert=0, copyts, genau der ausgewählten AAC-Spur, c:a=copy, avoid_negative_ts=disabled, xerror und ohne Überschreiben. FFmpeg lief im bestehenden Windows-Job mit **1 GiB / 25 % CPU**; keine Parallelisierung.

Der Prozess bricht nach **4,86 Sekunden** ab:

> Non-monotonic DTS; previous: 338741248, current: 338740736; Error submitting a packet to the muxer: Invalid argument

Das letzte AAC-Paket von Teil 1 beginnt bei Sample **338741248** und enthält noch **512 Endpadding-Samples**. Das Priming-Paket von Teil 2 liegt nach dem korrekten Filmversatz bei **338741760 − 1024 = 338740736** und damit vor dem vorherigen Paketanfang. An der inneren Naht müssen insgesamt **512 + 1024 = 1536 Samples** behandelt werden. Das beschreibt benötigte Randtrimmung, keinen nachgewiesenen hörbaren 32-ms-Aussetzer. Zusätzlich bleibt am Gesamtausgabeende die zulässige äußere Trimmung von 128 Samples nachzuweisen.

Die unvollständige Diagnose-Datei enthält nur die 330804 Pakete von Teil 1. Deren Nutzdaten stimmen mit Teil 1 überein; es gibt **keine erfolgreiche vollständige John-Wick-Kopierausgabe**. Die Abbruchzeit darf nicht als schnelle Gesamtverarbeitungszeit mit den persönlich gemessenen **300,54 Sekunden** des bewährten Weges verglichen werden. Ein gleichermaßen erfolgreicher A/B-Lauf beider Verfahren ist für diesen Fall nicht möglich.

Zeitstempel einfach vorwärts zu verschieben würde die beabsichtigte Bild-/Tonzuordnung verlassen. Das Verkürzen einer Paketdauer beweist keine Entfernung von Samples aus einem AAC-Block. Ein Decoder-/Samplefilter würde dekodieren und anschließend für eine AAC-Ausgabe erneut kodieren. Ein eigener MP4-Schreiber mit mehreren inneren Editlisten oder eine partielle Neukodierung wäre ein eigener Entwicklungsauftrag; die Untersuchung behauptet keine grundsätzliche Unmöglichkeit jeder denkbaren AAC-Verbindung.

Die frühere persönliche positive Kontrolle bleibt als Praxisbeobachtung bestehen. Sie ersetzt keinen aktuellen technischen Nachweis für Priming, Endpadding und Decoderzustand dieser konkreten Dateien.

## Zusätzliche Grenztests

### Positiv: ganze AAC-Blöcke ohne innere Trimmung

Zwei kleine künstliche MP4s: progressive H.264-Bilder, je 32 Frames bei 25 fps, je 60 AAC-LC-Pakete bei 48 kHz, Stereo. Je **1,28 Sekunden**, ohne Skip/Discard an der inneren Grenze. Die AAC-Testdaten enthalten Stille.

Kopierte Gesamtausgabe: **64 unveränderte Videopakete**, **120 unveränderte AAC-Pakete**, 2,56 Sekunden, Klebestelle 1,28 Sekunden. Alle Paketnutzdaten, Größen, Flags, Dauern und um den exakten Teilversatz verschobenen PTS/DTS stimmen. Beide Quellen und Ausgabe streng vollständig dekodiert: **122880 Samples je Kanal**, identische gesamte PCM-Bytefolge. SHA-256 **ca02793076b8845454d3cc4d949bda5bc49fe16d063e310331f86318ed644ba7**.

Dies beweist einen technisch möglichen Fall; es ist kein allgemeiner Spielfilm-Kompatibilitätsnachweis.

### Negativ: gleiche Pakete, gleiche Samplezahl, trotzdem anderer Ton

Gleiche Dauer und AAC-Blockgrenzen mit einem künstlichen **997-Hz-Ton**, dessen AAC-Paketfolge auf zwei Dateien verteilt wurde. Kopierprozess und alle vollständigen Dekodierungen laufen warnungsfrei. Paketnutzdaten und erwartete Zeitstempel stimmen; beide Vergleichsseiten enthalten exakt **122880 Samples je Kanal**.

Trotzdem unterscheiden sich bei unabhängiger Dekodierung der Quellen gegenüber der durchgehenden Dekodierung der verbundenen Ausgabe die Samples **61440 bis 62463**, also genau die ersten **1024 Samples hinter der Naht**. Referenz-PCM-SHA **cd4bc0c89b54bc3c1f1770754c46b317c0906775ed927eb5ae80da6a093e8ce0**, Ausgabe-PCM-SHA **eb6b07caf96caf477a5070e1b10e8ae8b0297c4a6f9cf20adafd481b4e494553**.

AAC benötigt Decoderzustand aus vorherigen Blöcken. Der einzelne neu gestartete Decoder von Teil 2 und der durchlaufende Ausgabedecoder können daher trotz identischer Pakete unterschiedliche Samples liefern. Der vorgeschlagene vollständige PCM-Vergleich muss diesen Fall ablehnen. Nur Samplezahl, Packet-Hashes und Prozess-Erfolg zu prüfen wäre zu schwach.

Die künstliche Dateierstellung aus rohem ADTS protokolliert eine bekannte Dauerschätzungswarnung. Diese bleibt in den Diagnoseunterlagen erhalten; exakte MP4-Paketanzahl und Zeitachse werden anschließend unabhängig ermittelt. Das erlaubt keine Warnungen im eigentlichen Klebeversuch oder in dessen Decoderprüfungen. Ein erster Prüfhilfenlauf stoppte korrekt an dieser Erstellungswarnung; die Fortsetzung wurde ausschließlich für diese bekannte Testdatenerstellung präzisiert.

## Zulassung für einen ersten schnellen Modus

Empfohlener enger Umfang, zusätzlich zu allen bestehenden Videoanforderungen:

1. Genau die beiden bewusst ausgewählten Spuren verwenden. Beide AAC-LC, identische geprüfte AudioSpecificConfig, Abtastrate und bekannte Kanalbelegung; tatsächlich nachgewiesenes 1024-Sample-Raster. HE-AAC, 960-Sample-Frames, dynamische Konfigurationswechsel oder unbekannte Zusatzdaten zunächst ablehnen. Unterschiedliche Bitraten allein sind kein Ausschlussgrund.
2. Beide Nutzzeitachsen starten wie bisher exakt bei null und decken den jeweiligen Bildzeitraum vollständig ab. Andere Starts nicht automatisch normalisieren. Integer-/rationale Zeitrechnung, keine Millisekundentoleranz an der Naht.
3. Kein Endpadding oder verkürzter AAC-Block am Ende von Teil 1; kein Priming/Skip, negativer Vorlauf oder Trimmungsbedarf am Anfang von Teil 2. Ganze unveränderte Pakete müssen ohne Lücke oder Überlappung aufeinanderpassen. Auch ganze Priming-Pakete in dieser ersten Fassung nicht automatisch löschen.
4. Ein äußerer Vorlauf ganz am Anfang von Teil 1 oder Endpadding ganz am Ende von Teil 2 darf nur übernommen werden, wenn Konfiguration, Wiedergabedauer, genaue Skip-/Discard-Angaben, vollständige Ausgabezeitachse und Dekodierung ihre unveränderte Wirkung beweisen. Andernfalls ablehnen.
5. Beide gewählten Quellspuren weiterhin strikt vollständig dekodieren und deren exakte Samplezahlen prüfen. Erwarteten PCM-SHA in der festgelegten Reihenfolge aus diesen Daten bilden. Die fertige Ausgabe ebenfalls streng vollständig in dasselbe Float32-Format dekodieren: **Samplezahl und gesamte PCM-Bytefolge müssen übereinstimmen**. Ein bloßer Vergleich von Samplezahlen genügt nicht. Festgelegter Decoder, ein Thread und unveränderte Dekodieroptionen für beide Seiten.
6. Vollständiger komprimierter Paketvergleich gegen die Quellen: AAC-Hash, Größe, Reihenfolge, Flags, Konfiguration, vollständige exakte Zeitachsen und äußere Trimmwirkung. Kein Hashvergleich allein, keine nur kurze Nahtprobe. Die vollständigen Videovergleiche bleiben erforderlich.
7. Jedes nicht beweisbare Ergebnis verwerfen. Meldung mit Grund und Hinweis auf die andere Schaltfläche; keine automatische Umstellung. Technischer Integritätsnachweis gilt für den geprüften Decoderweg; persönliche Prüfung von Anfang, Naht und Ende bleibt erforderlich.

Damit kann der Encoder vollständig entfallen. Quell-/Ausgabedekodierung und Integritätsprüfungen bleiben; „schnell“ bedeutet nicht „ungeprüft“ und auch kein garantiertes Laufzeitverhältnis. Häufigkeit geeigneter echter Filmteile und tatsächliche Gesamtzeit sind noch offen.

## Kleiner Architekturvorschlag

### Bewusste Bedienung

Nach Dateien, Reihenfolge und Tonspuren zwei Aktionen:

- **Audio kopieren (schnell)**
- **Audio neu kodieren (bewährt)**

Keine Aktion als IsDefault/Enter-Standard, keine gespeicherte Verfahrensvoreinstellung, kein automatischer Wechsel. Beide Aktionen erst bei gültiger Dateiauswahl und je gewählter Tonspur nutzbar; die Kopieraktion kann bei offensichtlich unpassendem Codec bereits einen erklärenden Hinweis geben. Eine allein vorhandene Tonspur darf wie bisher angezeigt/ausgewählt bleiben; das ist keine Verfahrenswahl.

Hinweis darunter: „Schnell: Ton ohne Neukodierung verbinden, sofern technisch sicher möglich. Bewährt: Ton samplegenau zusammensetzen und durchgehend neu kodieren.“ Der Verlust der übrigen Spuren bleibt sichtbar. Nach Fehler zurück zur bewussten Wahl; die bewährte Variante nie selbständig starten.

### Getrennte Audioverarbeitung, gemeinsame Schutzregeln

- **Anfrage:** Mp4JoinRequest erhält eine ausdrücklich erforderliche Verfahrensangabe, etwa JoinAudioMode.AacStreamCopy / ContinuousAacEncode. Fehlender/ungültiger Wert führt vor Verarbeitung zum Fehler. Alle Aufrufer werden explizit zugeordnet; keine versteckte Wahl durch einen optionalen Defaultparameter.
- **Orchestrierung:** Mp4JoinService dispatcht bewusst nach Verfahren. Den heutigen Neukodierungslauf möglichst als eigenen bestehenden Ablauf erhalten. Gemeinsame Datei-/Kompatibilitäts-/Video-/Übernahme-/Bereinigungshelfer wiederverwenden; keine neue universelle Strategie-Infrastruktur und keine Änderung von Videoargumenten oder Prüfstärke.
- **Kopierplanung:** neuer kleiner AacCopyJoinPlan mit Konfiguration, Framegrenzen, exakten Offsets, Paketanzahlen, äußeren Trimmangaben und nachvollziehbarem Ablehnungsgrund. Analyse und Fachentscheidung ohne WPF. Vor großer Zwischenverarbeitung problematische innere Trimmung ablehnen.
- **Kopierweg:** nur gewählte Audioindizes separat per Streamcopy in M4A-Arbeitsdateien vorbereiten und vollständig gegen die Quelle prüfen. Dadurch bleiben zusätzliche, nicht ausgewählte Spuren und unterschiedliche Streamreihenfolgen aus der Concat-Zuordnung heraus. Nur kompatible vorbereitete AAC-Tracks mit expliziten Dauern und kontrollierten Optionen verbinden; deren Paketfolge gegen beide Originale prüfen. Das entstandene AAC wiederum mit dem unverändert vorbereiteten Video per Streamcopy in die finale MP4 führen. Kein ADTS-Umweg im Produkt, kein automatischer Bitstreamfilter und keine Zeitstempelreparatur.
- **Ausgabeprüfung:** neuer modebezogener AAC-Paketvergleich ergänzt FfprobeStreamCopyVerifier; bisheriger Bildvergleich bleibt unverändert. Vollständiger PCM-SHA-Vergleich für Copy, bisheriger RF64-/Encode-/AAC-Remux-/Sample-Nachweis für Reencode. Gemeinsame Endkontrolle: Laufzeit, Vorschau, Ausgabeidentität und sichere Veröffentlichung der Datei.
- **Protokoll/Abschluss:** Verfahren ausdrücklich nennen, Tonverlust/übernommene Spurwahl zeigen; Copy-Erfolg nur nach allen Nachweisen. Beispiel bei Ablehnung: „Die beiden AAC-Tonspuren können nicht zuverlässig ohne Neukodierung verbunden werden: An der Klebestelle ist eine innere Trimmung erforderlich. Bitte Audio neu kodieren (bewährt) verwenden.“
- **Zeitmessung:** bestehender JoinPhaseTimer bleibt; gemeinsame Phasennamen behalten. Copy bekommt zusätzlich beispielsweise audio-copy-preflight, audio-copy-1/2, audio-copy-proof-1/2, audio-concat, joined-aac-proof und final-pcm-proof. Kein aac-encode-Eintrag für den Kopiermodus. total/cleanup weiterhin erfassen.
- **Ressourcen/Dateien:** derselbe begrenzte Runner, ein Thread, kein Parallelbetrieb. Originale sperren, CreateNew/kein Überschreiben, CancellationToken und Zeitlimits beibehalten. Neue bekannte M4A-/Concat-Prüfdateien in die gezielte Bereinigung aufnehmen; Diagnose bleibt bei Fehler/Abbruch. Eigene Speicherplatzrechnung des Copy-Weges muss die beibehaltenen PCM-Prüfdateien berücksichtigen. Keine neuen Produktionsabhängigkeiten.

Voraussichtlich betroffene Produktdateien nach Freigabe: Mp4JoinService.cs und Anfrage, eine neue begrenzte AAC-Plan-/Copy-Komponente, neue modebezogene Verifier-Teilklasse, JoinDialog.xaml/.xaml.cs und MainWindow.Join.cs; passende Media-/App-Tests und Bedienungsdokumentation. Die genaue Aufteilung ist noch kein implementierter Vertrag.

## Prüfplan nach ausdrücklicher Freigabe

- Vorhandene 1505 Tests und native bewährte Fälle unverändert weiterführen. AAC/AAC und AC-3/AAC im bewährten Weg bleiben funktionsgleich; Werkzeugargumente und geprüfte Ergebnisse gegen Build 42 vergleichen.
- Fachtests für Konfiguration, falschen Codec, fehlende Auswahl, Samplegrenzen, äußere/innere Trimmung, ungleiche Starts, Überlauf, monotone DTS/PTS, zusätzliche/fehlende/veränderte Pakete und die Kopier-PCM-Prüfung.
- Native positive Fälle mit mehreren tatsächlichen Signalen/Kanalbelegungen und erlaubten äußeren Trimmungen; negativer Decoderzustandsfall, John-Wick-Ablehnung, Decoderwarnungen, falsche Samplezahl und manipulierte Paketdaten.
- WPF-Prüfung beider Aktionen, Reihenfolgetausch samt Spurauswahl, kein Standardverfahren, keine stille Umstellung, Abbruch und nachvollziehbare Fehlermeldung.
- Ausgabe-Rennen, vorhandenes Ziel, Originalschutz, Diagnoseerhalt und gezielte Bereinigung für beide Varianten prüfen.
- Erst für nachweislich geeignete echte Dateien eine vollständige Laufzeitmessung beider Verfahren unter gleichen Bedingungen. John Wick im bewährten Weg ist bereits persönlich positiv geprüft; kein künstlich erfolgreicher Copy-Vergleich.
- Persönliche Kontrolle einer neuen akzeptierten Kopierausgabe steht aus: Anfang, Klebestelle, Ende, Ton/Bild/Synchronität und Vorschau. Sie kann nicht durch diesen Untersuchungsbericht vorweggenommen werden.

## Nachweise, Quellen und Grenzen

Lokale Belege: **.build/klebecenter-audio-copy-analysis-20261028/** mit Werkzeugidentität, vollständigen Eingabe-/Paketanalysen, Quellen-Hashes, copy-monitor/, john-wick-decode-0/ und john-wick-decode-1/, JOHN-WICK-PCM.json, POSITIVE.json und DECODER-STATE-NEGATIVE.json. Die beiden eigenen großen PCM-Prüfdateien und die unvollständige John-Wick-Diagnosekopie wurden nach gesicherten Hashnachweisen gezielt entfernt (REMOVED-OWN-LARGE-FILES.json). Kleine synthetische Dateien und alle Diagnosebelege bleiben. Vorhandene Nutzerdateien werden nicht bereinigt.

Verwendet: konfigurierte FFmpeg-/ffprobe-EXEs **2026-10-12-git-615b07170e-essentials_build-www.gyan.dev**, mit erneut gesicherten SHA-256-Werten. Die Online-Dokumentation des Entwicklungszweigs ist ergänzende technische Erklärung; die konkreten Befunde stammen von genau diesen lokal geprüften EXEs.

- [Offizielle FFmpeg-Formatdokumentation: concat](https://ffmpeg.org/ffmpeg-formats.html#concat): Dateidauern bestimmen die Versätze; zusätzliche Pakete vor Inpoints können überlappen. Ein In-/Outpoint garantiert keine samplegenaue Trimmung komprimierter Blöcke.
- [Offizielle FFmpeg-Codec-API: initial_padding / trailing_padding](https://www.ffmpeg.org/doxygen/9.0/structAVCodecContext.html): Anfangs-/Endauffüllung sind zu verwerfende Samples, Encoderpakete können negativ beginnen.
- [FFmpeg concatdec.c](https://ffmpeg.org/doxygen/trunk/concatdec_8c_source.html): concat_read_packet verschiebt PTS/DTS um den Dateiversatz; dies ist kein samplebasierter AAC-Neuaufbau.
- [FFmpeg movenc.c](https://www.ffmpeg.org/doxygen/trunk/movenc_8c_source.html): mov_write_edts_tag schreibt für normale Tracks eine Inhalts-Editliste mit optionalem anfänglichem Leereintrag; hieraus ergibt sich kein allgemeiner CLI-Nachweis für mehrere innere AAC-Trimmstellen. Keine pauschale Aussage über alle MP4-Writer.
- [FFmpeg AAC-Decoderdaten](https://ffmpeg.org/doxygen/trunk/structAACDecContext.html): gespeicherter Überlappungszustand vorheriger Frames erklärt die zusätzlich erforderliche PCM-Prüfung.

Produktcode, vier vorbestehend geänderte Nutzer-Produktions-Locks, übrige Locks, Version.props und Buildnummer bleiben unverändert gegenüber Beginn dieser Untersuchung. Kein erneuter Produktbuild erforderlich; die vorhandenen 1505 Tests werden nicht als neu ausgeführt dargestellt. Diese Aufgabe enthält native Untersuchungsversuche, keine freigegebene Produktänderung. Implementierung und neue Pakete sind weiterhin offen.
