# OTR-CAN – Audio-Prüfung und gemeinsame Testdatei

Historischer Versuchsstand vom 11.10.2026: Audio-Testvariante **0.1.2-audio.1**, zum Prüfzeitpunkt regulärer Motor **0.1.2**, CAN **0.2.1 · Build 11 · RC3**. Die anschließende Nutzerentscheidung und Übernahme als 0.1.3 stehen am Ende; kein Release veröffentlicht.

## Ergebnis und wesentlicher Unterschied

Die neue Audio-Timeline besteht die Prüfungen: **35 synthetische Aufträge / 70 Tonspuren**, je Spur korrekte Samplezahl und lückenlose rohe Sample-PTS. Die vor der Kodierung zusammengestellten Float-PCM-Samples sind bytegenau dieselben wie beim unabhängigen Auswählen der Keep-Samples aus dem vollständig decodierten Original.

Auch Pines ist vollständig geprüft: AAC-Stereo und AC-3-Stereo, jeweils **264.240.000 decodierte Samples / 5505,000 s bei 48 kHz**. Beide vollständigen PCM-Referenzen wurden unabhängig erneut anhand der Original-Samplepositionen verifiziert; Payload-SHA-256 und Bytezahl stimmen exakt.

**Diese Lösung kodiert jede komplette Tonspur einmal neu.** Sie kopiert den ursprünglichen komprimierten Ton nicht bitgleich. Codec, Kanäle/-layout, Samplerate und konfigurierte Zielbitrate bleiben erhalten; Sprache und Disposition werden geprüft. Zusätzliche Kodierverluste und zusätzliche Laufzeit sind ein bewusster Unterschied zum bisherigen Smart Rendering. Keine verlustfreie Tonübernahme oder allgemeine Hör-/Lippen-Synchronitätsabnahme behauptet.

## Umsetzung und Trennung

Neue Quellen ausschließlich im Rust-Repository unter experiments/audio-continuity. Bestehender regulärer Motor, Video-Testprojekt experiments/frame-boundaries, Einstellungen und Referenzen erhalten. Eigenständiges Cargo-Projekt mit MPL-Hinweisen, unverändertem Abhängigkeitsbestand und eigener EXE target/audio-continuity-build/release/otr_can_audio_test.exe. Der Schalter **--audio-experiment** sowie ein neuer absoluter **--reference-dir** sind verpflichtend.

Die Audio-EXE erzeugt eine Audio-MP4 und ausdrücklich angeforderte verlustfreie WAV/RF64-Belege. tools/mux-test.ps1 kombiniert sie diagnostisch mit der separat geprüften Videoreferenz. Das ist noch kein einheitlicher produktiver CAN-Schnittauftrag.

1. Keep-Grenzen über denselben Frame-/Zeitmapper wie im Video-Versuch bestimmen.
2. Originalton mit vollständiger Dekodiervorgeschichte einmal lesen. Pro Spur behaltene Samples schneiden und als PCM zusammenfügen.
3. PCM-Format und erwartete Samplezahl prüfen. Kanallayout am Encoder-Eingang ausdrücklich setzen.
4. Pro Spur genau eine durchgehende AAC-LC-/AC-3-Kodierung mit ursprünglicher Zielbitrate.
5. Encoder-Vorlauf und End-Padding über präzise MP4-Zeiten begrenzen. Tatsächlich decodierte Samplezahl und jeden Audio-PTS prüfen; keine gaps/overlaps durch nachträgliche Ausgabe-Zeitstempelnormalisierung verstecken.
6. Private Zwischenordner und Sperre bereinigen. Nur die ausdrücklich benannten Belegausgaben bleiben bestehen.

Begrenzter Versuch: Videozeitbasis weiterhin 25/50-fps-CFR mit Quellstart null; AAC-LC und AC-3. Zugelassen sind 32/44,1/48 kHz, tatsächlich geprüft 44,1/48 kHz. Andere Codecs/Profiles/Raten werden abgelehnt. Keine allgemeine Reparatur beschädigter Original-Timelines; nicht sämtliche AC-3-Bitstream-Metadaten als übernommen ausgeben.

Hintergrund: [FFmpeg atrim](https://ffmpeg.org/ffmpeg-filters.html#atrim), [Audio-Codecoptionen](https://ffmpeg.org/ffmpeg-codecs.html), [MP4/Edit-Lists](https://ffmpeg.org/ffmpeg-formats.html#mov_002c-mp4_002c-ismv), [setts](https://ffmpeg.org/ffmpeg-bitstream-filters.html#setts). Der Nachweis beruht auf echten Samples, Decoderzeitstempeln und Paketdaten.

## Prüfungen

**31/31 Rust-Tests**, Formatprüfung und separater Release-Build ohne Compilerwarnungen oder Fehler. Native Gap-/Overlap-/Vorlauf-/Padding-Prüfungen auch mit absichtlich fehlerhaften Daten getestet.

Die 27 ersten Aufträge prüfen die ursprünglichen Grenzfälle mit/ohne B-Frames, einzelne Bilder, Anfang, Dateiende, späte Keeps und Mehrbereichslisten: je AAC und AC-3, Mono sowie Stereo/5.1(side), 25/50 fps, 44,1/48 kHz. Alle 54 PCM-Referenzen stimmen vor Kodierung bytegenau mit unabhängiger Original-Sampleauswahl überein. Alle decodierten Ergebnislängen sind exakt.

Zusätzlich acht Aufträge mit unregelmäßigen, bandbegrenzten Prüfsignalen und geeignetem Tiefbasston: weitere 16 bytegenaue PCM-Referenzen, korrekte Ergebnis-Samplezahlen und Kanäle.

Signalvergleich nach verlustbehafteter Kodierung:

- 160 Kanal-/Zeitfenstervergleiche; 155 Korrelationsmaxima bei null Samples Versatz.
- Fünf Maxima im LFE-Kanal bei minus zwei bzw. plus einem Sample; höchstens **0,0417 ms** bei 48 kHz, kein gemessener globaler Spurversatz.
- Keine Kanalvertauschung; kleinster gemessener Korrelationskoeffizient 0,991528.
- Kleinster gemessener SNR-Wert 18,813 dB, insbesondere kurze Kodierfenster. Kein aus diesen Zahlen abgeleiteter pauschaler Hörqualitäts-Grenzwert.

Die erste reine Sinus-/Chirp-Messung liefert 19 nichtnullige Korrelationsmaxima, darunter mehrdeutige Periodenverschiebungen und ein schwaches, ungeeignet hochfrequentes LFE-Signal; kleinster SNR dort 5,127 dB. Die Werte bleiben erhalten und werden nicht als Nachweis exakter zeitlicher Verschiebung oder unveränderter Tonqualität ausgegeben. Die zusätzlichen unregelmäßigen Signale prüfen die Zuordnung zuverlässiger; die Kodierverluste werden dadurch nicht aufgehoben.

## Pines: Audio und gemeinsame MP4

Audio-Prüflauf **168,6 Sekunden / rund 2:49 Minuten**, einschließlich Originaldekodierung, großer PCM-Zwischenausgaben, kompletter Neukodierung und vollständiger Decoderprüfung. Kein direkt mit den früheren 57,2 Sekunden für Indexierung plus regulärem Schnitt vergleichbarer Produktionsbenchmark.

| Stream | Format | Ergebnis |
|---|---|---|
| 0 | H.264 / 25 fps | 137625 Bilder, 5505,000 s |
| 1 | AAC-LC / Stereo / 48 kHz | 264240000 Samples, 5505,000 s |
| 2 | AC-3 / Stereo / 48 kHz | 264240000 Samples, 5505,000 s |

AAC-Zielbitrate 163535 bit/s wie im Original; gemessener Durchschnitt im Ergebnis 163499 bit/s. AC-3 exakt 384000 bit/s. Beide Sprachen deu und ursprüngliche Default-/Disposition-Werte erhalten.

Der erste reine Copy-Mux verlängerte AC-3 am Ende um **896 Samples / 18,667 ms**. Ursache im Prüfschritt: letzte Codec-Paketlänge ohne ursprüngliche Präsentations-Endbegrenzung. Eine zweite Fassung begrenzt die letzte Paketdauer ausdrücklich, erhält PTS/DTS und präzise Edit-Lists; die dritte setzt zusätzlich AC-3s Rahmenlänge 1536 und arbeitet ohne FFmpeg-Warnung.

**Alle Video-/Audio-Pakete der endgültigen gemeinsamen Datei** wurden vor und nach diesem Mux auf Payload-SHA-256, PTS, DTS, Dauer, Flags und Skip-/Padding-Sidedata verglichen. Identisch; Decoder-Konfigurationen, Zeitbasen und die zusätzlich abgefragten Stream-Metadaten ebenfalls identisch. Damit bleiben die vollständig geprüften Einzelreferenzen im gemeinsamen Container erhalten. Kein erneutes vollständiges Video-Decoding allein wegen dieses nachgewiesen unveränderten Muxes.

Gemeinsame Testdatei:

.build/audio-fix-20261011/pines/Pines-frame-audio-test-v3.mp4

Berechnete Übergänge im Ergebnis: **18:16.880**, **30:49.080**, **49:25.520**, **1:21:47.200**; Ende **1:31:45.000**. Der Nutzer bestätigt am 11.10.2026 zum gemeinsamen Pines-Testfilm v3: „ok bild und ton passen einwandfrei zusammen :-)“. Damit ist die persönliche Bild-/Ton-Synchronitätsbewertung dieses Films positiv. Eine ausdrücklich getrennte Abnahme von AAC und AC-3 wurde nicht mitgeteilt; keine allgemeine Datei-/Codec- oder Veröffentlichungsfreigabe daraus abgeleitet. Frühere verworfene Muxdateien bleiben ausschließlich als Diagnosebelege erhalten.

SHA-256:

- Audio-Test-EXE: E87D8A42750F24EEC314BD0974A0727477ACA1900674DA8BA4567DD7F0C75120
- gemeinsame Pines-Testdatei v3: A1467DDBB5D9B90C5FF3B6D9B3DB745ABF73D8928756E57AF6C2E29DBCBE3111

## Belege und Erhaltung

Ignoriert unter .build/audio-fix-20261011: matrix/RESULT.json, matrix-noise/RESULT.json, SIGNAL-RESULT.json, SIGNAL-NOISE-RESULT.json, pines/SOURCE-PCM-COMPARISON.json, pines/MUX-COMPARISON.json, Paket-/Konfigurationsproben, Audio- und Muxprotokolle, PCM-Belege, Prüfhilfen, build.log, tests.log und FINAL-RESULT.json.

Ein erster nativer Versuch wurde wegen geratenem WAV-Kanallayout korrekt abgelehnt; Eingangs-Layout anschließend ausdrücklich gesetzt. Eigene PowerShell-Prüfhelferfehler bei Auswertung, Komma-Escapierung und Argumentübergabe korrigiert; Erstprotokolle erhalten. Keine Produktions-/App-Fehler daraus abgeleitet.

CAN-C#-Quellen, acht Lockdateien, aktive Motor-EXE und alle vorbestehenden Rust-Dateien per Prüfsumme erhalten. Kein regulärer CAN-Rebuild erforderlich; letzter regulärer Stand 743 CAN-/25 Rust-Tests. Keine Einstellungen, Version, Build 11 oder RC3 geändert. Kein Commit, Push oder Branchwechsel.

## Anschließende Entscheidung und reguläre Übernahme

Der Nutzer wählt nach positiver persönlicher Pines-Bild-/Tonbewertung ausdrücklich die vollständige Audio-Neukodierung: „ok dann machen wir die neukodierung beim audio, wir hatten ja zeit gewonnen“. Dieser Weg ist mit der Framekorrektur in einem gemeinsamen Auftrag geprüft und als regulärer Motor **0.1.3** in CAN übernommen. 40 Rust- und 750 CAN-Tests; regulärer Pines-Auftrag über CAN 213,24 s, Ausgabe vollständig bytegleich zum persönlich geprüften Film. Zusätzliche Kodierverluste und Laufzeit bleiben ausdrücklich dokumentiert. Aktueller Umfang und weitere Freigabeschritte im [0.1.3-Prüfbericht](OTR-CAN-0.1.3-PRUEFBERICHT.md).
