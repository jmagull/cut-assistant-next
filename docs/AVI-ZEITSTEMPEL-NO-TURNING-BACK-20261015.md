# AVI-Diagnose No Turning Back – 15.10.2026

Reiner Untersuchungsauftrag; keine Produktkorrektur. Halbierungssuche persönlich positiv bestätigt, Produktcode und Schnittmotoren unverändert.

## Gesichertes Ergebnis

Die Lupe und otr-can verweigern diese Aufnahme technisch begründet. **1149** Rücksprünge der decodierten PTS und **1149 doppelte FFMS2-Bildzeiten**, periodisch nach I-Bildern alle 250 Bilder/5 s. Die reguläre MP4-Arbeitskopie hat eine **bytegleiche FFMS2-Zeitcodedatei**. Die vorhandene erfolgreiche MP4Box-Ausgabe enthält noch **978 Doppelzeiten**. MP4Box verarbeitet die Aufnahme, repariert diese Zuordnung aber nicht. Technischer Erfolg und persönliche Bild-/Ton-/Übergangsabnahme bleiben getrennt.

## Identität und Eigenschaften

Original: `No_Turning_Back_26.03.29_23-15_3sat_80_TVOON_DE.mpg.HD.avi`, **1638090194 Byte**, SHA-256 vor/nach und für Diagnosekopie **404b840af7cac136b35ca788013171b39fe8fbb06d7afc8cc0dcb1a3041bf3f3**. H.264 Main, 1280×720 progressiv, 50/1 fps, Zeitbasis 1/50, Dauer 5747,840 s. MP3 Stereo/48 kHz, Zeitbasis 3/125, ausgewiesene Dauer 5746,992 s. Keine Synchronitätsaussage allein aus diesen Metadaten.

Container nennt 287392 Frames; vollständig decodiert **287390**, davon **1150 I und 286240 P**, keine als B ausgewiesenen Bilder. Deshalb keine ungeprüfte Zeitachse aus nomineller FPS und Frameindex.

Log belegt dieselbe AVI ab 16:23:19, Kante 425,080 s, erfolgreiche MP4Box-Aufrufe 16:27/16:28 auf temporären **MP4-Dateien** mit `-splitx 425.08:5310.88`. Vorhandene Ausgabe `No Turning Back.mp4`: 1371919537 Byte, 4885,840 s, H.264/MP3.

Die historische otr-can-Tempdatei aus dem Screenshot ist nicht mehr vorhanden; ihre damalige Byteidentität ist rückwirkend nicht direkt beweisbar. Unabhängig davon wurden jetzt **beide Fehler an derselben gehashten Aufnahme** reproduziert: aktuelle Lupe am Original und installierter otr-can **0.1.5** an dessen regulärer CAN-MP4-Arbeitskopie.

## Konkrete Fehlerstellen

Lupenintervall 423,080–427,080 s enthält durch normalen Seek-Vorlauf 351 Bilder ab etwa 420 s, mit Rücksprüngen **420,180→420,080** und **425,180→425,080 s**. Vollständige Analyse ab Dateianfang (268,6 s) bestätigt die wiederkehrende Störung, kein bloßes Seek-Artefakt.

| Decode-Index, nullbasiert | Typ | Frame-pkt_dts (s) | PTS (s) |
|---:|:---:|---:|---:|
| 21250 | I | 425,040 | 425,180 |
| 21251 | P | 425,060 | 425,080 |
| 21256 | P | 425,160 | 425,180 |

DTS streng aufsteigend; PTS nach dem I-Bild um 100 ms rückwärts, sechs Bilder später doppelt. Decode-Indizes sind keine zugesicherten Player-Framezahlen. Best-Effort ist eine andere Folge und enthält selbst einen Rücksprung; keine sichere pauschale Ersatzquelle.

Lupe nutzt `Pts ?? BestEffortTimestamp`; `FrameLoupeViewModel.SetWindow` sperrt bei `time <= previousTime`. Echtes aktuelles Modell: gemeldete Fehlermeldung, Tabelle leer, keine Auswahl/Vorschau, CanApply=false. Schutz korrekt.

FFMS2 `-c -k` indexiert erfolgreich. Erste Doppelzeit: FFMS2-Indizes **255/256 = 5180 ms**, Dateizeilen 257/258 mit Kopfzeile; danach 10180,15180…ms. FFMS2-Indizes und Decode-Indizes unterscheiden sich. `can-engine/src/cutting/info.rs`, `retrieve_existing_indexes`, verwirft bei `pair[0] >= pair[1]`. Doppelwerte bereits in externer Datei, kein Rust-Rundungsfehler. Installierter Motor reproduziert **Retrieving metadata → workspace lock released → FFMS2 frame timestamps must be strictly increasing**, Exit 1, keine Ausgabe. Ein anfänglicher Diagnoseaufruf wurde an die korrekte Vorgabe angepasst, den Index im privaten Motorarbeitsordner abzulegen.

MP4Box arbeitet hier mit MP4-Paketen/Sampletabellen und Schnittbereichen, ohne die streng eindeutige FFMS2-Zeitliste zu verlangen. [GPAC-Dokumentation](https://wiki.gpac.io/MP4Box/mp4box-dump-opts/). Nicht monotone Paket-PTS allein wären bei Bildumordnung normal; hier beweisen zusätzlich decodierte I/P-Folge und verschiedene Bilder mit gleichen Zeiten die Mehrdeutigkeit. Kein neuer MP4Box-Produktauftrag, keine Änderung des funktionierenden Verfahrens.

## Eingegrenzter Auslöser und Kopierversuche

H.264-Header: pic_order_cnt_type=2, 50-fps-Takt aus time_scale=100000 und num_units_in_tick=1000. Picture-Timing-SEI: problematisches I-Bild **dpb_output_delay=14**, P-Bilder **2**, entsprechend 140/20 ms. Passend zu beobachteten DTS→PTS-Abständen. [FFmpeg-H.264-Parser](https://github.com/FFmpeg/FFmpeg/blob/master/libavcodec/h264_parser.c) nutzt diese Angaben zur PTS-Ableitung. Ein befülltes AVI-ffprobe-PTS-Feld ist deshalb keine Garantie ursprünglicher Container-PTS; auch das Oberflächenwort Original-PTS beweist diese Herkunft nicht. [Best-Effort ist heuristisch](https://ffmpeg.org/doxygen/trunk/structAVFrame.html).

Zwei Bilder bei **5,180 s** sind verschieden: I-MD5 `59e131dde43f652c1c2b5eee9dfc9c80`, P-MD5 `e8c8d3492c9e301c8f67045f64dd4109`. Nach Zeit zu deduplizieren würde echte Bilder entfernen.

| Versuch auf separater Kopie | Ergebnis |
|---|---|
| reguläres vollständiges AVI→MP4, Stream-Copy | 287390 FFMS2-Zeiten, 1149 Doppelzeiten |
| 12-s-MP4 zusätzlich +genpts | 598 Bilder, weiterhin 2 Doppelzeiten |
| 12-s-AVI-Kontrolle, Stream-Copy | 598 Bilder, 2 Rücksprünge/Doppelzeiten |
| gleiche 12-s-Kopie, zusätzlich SEI entfernen | 598 Bilder, keine PTS im Probe-Ergebnis, eindeutige Best-Effort-/FFMS2-Folge; alle **598 decodierten Bildhashes gleich** zur Kontrolle |

SEI-Entfernung nur Diagnose mit `filter_units=remove_types=6`: entfernt sämtliche SEI, möglicherweise auch Untertitel/HDR-/Darstellungsinformationen. Keine Anwendungsempfehlung, kein vollständiger Film-/Audiotest, keine wiederhergestellten Originalzeiten. Welche ursprüngliche Encoder-/Aufnahmestufe die widersprüchlichen Angaben erzeugte, bleibt offen. Kein allgemeiner AVI-/FFMS2-Defekt behauptet.

Reguläre vollständige MP4-Kopie zusätzlich geprüft: **287390 normalisierte VCL-NALs**, **1481332943 VCL-Byte**, SHA-256 **bdab64df714bb294aab5aca9b0a288f3af0ed9b93a8cb7320a028202f7bcee8f**, gleich zum Original; MP3-Payload ebenfalls hashgleich. Unmittelbare Video-/Extradata-Hashes unterscheiden sich, daher separater normalisierter Nutzdatenvergleich. Keine Bytegleichheit kompletter Container oder sämtlicher Metadaten behauptet.

## Vergleich und Empfehlung

Teufelsbrigade-AVI: vollständiger FFMS2-Index **263694 Bilder, 0 Monotoniefehler**; reales aktuelles Lupenmodell bei 582,240 s mit ±1 erfolgreich. Die Hart 2 MP4: 127 Bilder im Lupenintervall bei 425,080 s, eindeutige PTS und ±1 erfolgreich; keine vollständige Referenzfilmprüfung behauptet.

Schutzprüfungen beibehalten; nicht sortieren, Bilder löschen oder pauschal Frameindex/FPS einsetzen ([FFMS2-API](https://github.com/FFMS/ffms2/blob/master/doc/ffms2-api.md)). Erst nach Abstimmung gezielt widersprüchliches Picture-Timing auf einer Kopie behandeln und andere SEI-Inhalte schützen. Vor möglicher Übernahme vollständige Bildanzahl/-reihenfolge, Bild-/Keyframe-Identitäten, Audiozuordnung, Anfang/Ende/Übergänge und 25-/50-fps-/B-Frame-/VFR-Regressionen nachweisen. Noch **keine allgemein sichere alternative AVI-Zuordnung freigegeben oder implementiert**. MP4Box bleibt technisch erfolgreicher Weg mit gesonderter Ergebnisprüfung.

## Prüfstand

Unveränderter Produktcode separat Release gebaut, ohne Restore, Buildnummer 30: **0 Warnungen/0 Fehler**, **1244/1244 Tests**, 0 übersprungen (Core 196, Cutlists 72, App 823, Media 153). Drei zusätzliche reale Lupenmodell-Fälle bestanden. Original-SHA-256 und Kopie gleich; Größen/Änderungszeiten aller vier Medien einschließlich MP4Box-Ausgabe identisch. Acht Lockdateien, vorhandener Produktdiff, neue Halbierungssuche-Dateien, Version/Buildzähler erhalten. Nur Dokumentation und Diagnosehilfen ergänzt, keine Originalaufnahme/Cutlist geändert, keine Prüfung abgeschaltet; kein Restore, Commit, Push, Paket oder Release.

Belege ausschließlich im ignorierten `.build/avi-timestamps-20261015`: `identities-before.json`, `media-preservation.json`, `source-session.log`, `problem-full-summary.json`, `full-pattern-summary.json`, `ffms-summary.json`, `motor-reproduction.*`, `loupe-reproduction.json`, `h264-headers.log`, `sei-intervention.json`, `duplicate-time-different-images.json`, `genpts-result.json`, `normalized-vcl-comparison.json`, `build.log`, `tests.log`, `can-tests/`. Diagnosemedien/Indizes werden nicht in Git aufgenommen.
