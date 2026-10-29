# MUCH MORE QUICK AND COPY – Referenzprüfung und Rücksprache

**Historischer Nachweis:** Die anschließend verbindlich korrigierte Nutzeranweisung ist ab [Build 46](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md) umgesetzt. QUICK arbeitet nun pragmatisch; die hier beschriebenen strengen Abbruchregeln und eine damalige Rücksprache-Sperre gelten nicht mehr für den aktuellen QUICK-Produktweg. Messungen und der eingefrorene Build-45-Stand bleiben erhalten.

**29.10.2026. Entwicklungsauftrag vollständig eingelesen; technische Referenzprüfung durchgeführt. Noch keine Produktimplementierung oder neue Buildnummer.** CAN Build 45 und alle bisherigen Pakete unverändert erhalten. otr-can bleibt 0.1.6; veröffentlichter Stand Build 41.

## Auftrag und vereinbarter Prüfpunkt

Aktuelle vollständige Fassung: Anhang **24123c6d-bf9f-4ae7-801a-cce262f29a06**, inhaltlich identisch zum vollständigen Anhang 6bded502-6942-45be-9423-754e93a37955. Die erste Fassung 5079e530-298b-4040-a40a-913f0d33d137 war am Satzende abgeschnitten. Vollständiger Auftrag unter **.build/much-more-quick-copy-20261029/REQUEST.md** gesichert.

Abschnitt 9 verlangt ausdrücklich: **„Falls die manuelle MP4Box-Ausgabe bei einer sicherheitsrelevanten technischen Prüfung scheitert, bitte nicht die Prüfung entfernen, sondern den konkreten Konflikt dokumentieren und vor einer weitergehenden Änderung Rücksprache halten.“** Dieser Punkt ist jetzt erreicht. Keine Prüfungen gelockert oder Produktdateien geändert.

## Geprüfte Dateien

Aus **C:\Users\joerg\Desktop\down**:

- Furiosa  A Mad Max Saga  Teil 1 MP4Box.mp4
- Furiosa  A Mad Max Saga  Teil 2 MP4Box.mp4
- Furiosa A Mad Max Saga - MP4Box Streamcopy Test.mp4

Der Nutzer meldet für den manuellen Concat-Aufruf **10,42 s** und positive persönliche Bild-/Tonprüfung an der Klebestelle. Dieser Befund bleibt gültig; die technischen Prüfungen gehen darüber hinaus.

## Positive Nachweise

- Referenzausgabe vollständig mit Bild und beiden Tonspuren decodiert: **201566 Bilder**, keine Decoderfehler, kein künstliches Duplizieren oder Entfernen von Bildern. Laufzeit der begrenzten Vollprüfung **847 s = 14:07 min**, Spitzenbelegung rund 96 MB; 1 GiB / 25 % CPU, jeweils ein FFmpeg-Prozess. Der Null-Prüfausgang meldete eine Zeitstempelwarnung; sie wurde nicht ignoriert, sondern mit gesonderter Decoderprüfung untersucht.
- **Alle 201566 Videopakete nachgewiesen.** Vergleich normalisiert ausschließlich AVCC-NAL-Längenverpackung und wiederholte SPS/PPS. Sämtliche übrigen NAL-Nutzdaten einschließlich SEI/AUD bleiben identisch; zu jedem VCL-Slice stimmen die tatsächlich referenzierten PPS-/SPS-Bytes mit der Quelle überein. Unterschiedliche initiale extradata_hash-Werte allein bedeuten hier keine Decoderinkompatibilität.
- **377950 AAC- und 251960 AC-3-Pakete** vollständig in Quellreihenfolge und mit unveränderten komprimierten Nutzdaten erhalten.
- Alle Paket-PTS und -DTS sind gegenüber den Quellen einschließlich Concat-Dateiversatz um einheitlich **2048 Ticks bei 48000 Hz = 42,667 ms** verschoben. Keine Non-monotonic-DTS-Korrektur an den komprimierten Videopaketen. Eine Bildpaketdauer an der Naht wird von 1920 auf 35 Ticks verkürzt; das entspricht genau dem Abstand bis zum ersten Bild von Teil 2. Zwei AAC-Paketdauern ändern sich ebenfalls und sind einzeln dokumentiert.
- Die sechs AAC-/AC-3-Quell- und Ausgabespuren vollständig streng zu PCM decodiert, ohne Decoderwarnungen; Samplezahlen am tatsächlichen PCM-Dateiumfang gemessen. Temporäre PCM-Dateien anschließend gezielt entfernt.

## Konflikt 1: rückläufige Präsentationszeit in tatsächlicher Decoder-Ausgabereihenfolge

Die Null-Ausgabe der Vollprüfung meldet bei Bildwert 56518 eine nicht aufsteigende Zeit. Ein zunächst möglicher reiner Rundungseffekt wurde durch zusätzliche Prüfung **nicht bestätigt**: Auch bei erhaltener Demux-Zeitbasis bleibt ein Rücklauf sichtbar. Anschließend direkte ffprobe-Frameprüfung **ohne Null-Muxer**, über ein ausreichend großes Fenster einschließlich Keyframe-Vorlauf:

| Decoder-Ausgabe | PTS bei 48000 Hz | Zeit |
|---|---:|---|
| vorheriges P-Bild | 108514688 | 00:37:40.722667 |
| folgendes I-Bild | 108514291 | 00:37:40.714396 |

**Rücklauf: 397 Ticks = 8,271 ms.** `best_effort_timestamp` bestätigt dieselben Werte. Im ursprünglichen MP4Box-Teil 1 ist derselbe Rücklauf bereits vorhanden, mit beiden Werten um 2048 Ticks kleiner. Er wurde nicht durch den neuen Concat-Versuch erzeugt.

Die frühere Sortierung aller Paket-PTS zeigte zwar lückenlose Intervalle, bewies aber nicht deren tatsächliche Decoder-Ausgabereihenfolge. Hier scheitert die im Auftrag ausdrücklich verlangte aufsteigende Präsentationsfolge. Das ist kein Decoderabsturz und kein Beleg fehlender Bilddaten; die Zeitachse erfüllt aber diese Akzeptanzregel nicht. Der persönliche positive Check an der späteren Klebestelle widerspricht diesem früher im Film liegenden Befund nicht.

## Konflikt 2: AAC-Sampleabweichung außerhalb der bisherigen Grenzen

| Spur | getrennte Quellen, Samples/Kanal | manuelle Ausgabe | zusätzliche Samples | Zeitäquivalent |
|---|---:|---:|---:|---:|
| AAC | 387015903 | 387019996 | **4093** | **85,271 ms** |
| AC-3 | 387008239 | 387010560 | 2321 | 48,354 ms |

AAC-Zusatz exakt aus entfernten Randdaten erklärt: **2048 Samples Anfangsvorlauf Teil 1 + 125 Samples Endtrimmung Teil 1 + 1920 Samples Anfangsvorlauf Teil 2 = 4093**. In der manuellen Ausgabe fehlt die ursprüngliche äußere AAC-Anfangstrimmung; das steht zusätzlich zur Behandlung der inneren Naht. Finale AAC-Endtrimmung 804 Samples bleibt erhalten. AC-3 verliert 1117 Samples innere Endtrimmung, 384 Samples inneren Vorlauf und 820 Samples finale Endtrimmung, zusammen 2321.

Bestehende QUICK-Regel: maximal **3072 zusätzliche Samples und höchstens 80 ms**. Bei 48000 Hz greift die Samplegrenze bereits bei 64 ms. AAC überschreitet beide Grenzen, AC-3 bleibt innerhalb. Diese Zahlen sind zusätzliche decodierte Samples, **keine gemessene gleich große Lippensynchronitätsabweichung**. Der uniforme Zeitversatz aller Spuren und die Codec-Randdaten sind separat dokumentiert.

## Nächster Schritt – erst nach Rücksprache

Nicht einfach die bisherigen Grenzen erhöhen oder den Rücklauf übergehen. Vorgeschlagen ist ein **separater Untersuchungs-/Korrekturversuch ohne Neukodierung**, der

1. die rückläufigen Bildzeitmarken gezielt behandelt, ohne Bildpakete zu entfernen, zu duplizieren oder die gesamte Zeitachse pauschal zu glätten,
2. äußeren AAC-Vorlauf und Randmetadaten möglichst erhält,
3. anschließend erneut vollständige NAL-/Parameter-, Zeit-, Sample- und Decoderprüfungen besteht.

Ob das technisch sauber möglich ist, ist noch nicht bewiesen. Alternativ kann QUICK später nur auf bereits passende MP4Box-Zeitachsen erweitert werden, während dieser konkrete Furiosa-Fall weiter abgewiesen bleibt. Kein pauschales Verbot aller MP4Box-Dateien.

## Unveränderte Basis und Nachweise

**371 Produkt-/Testdateien, acht vorbestehende Lockdateien, Version.props, Zähler 45 und 40 oberste Auslieferungsdateien der Builds 40/41/43/44/45 per SHA-256 unverändert.** Keine neue Produktionsabhängigkeit, kein Commit/Push, keine Setup-/Portable-Paketierung oder Veröffentlichung. Keine realen Filmdateien in Git. Die bisherige Produkt-Regression wird nicht als neu durchgeführt ausgegeben, da der Produktcode unverändert ist.

Nachweise unter **.build/much-more-quick-copy-20261029/**: REQUEST.md, PRESERVED-BEFORE.json, CODE-BEFORE.json, MANUAL-FULL-DECODE.json, MANUAL-PACKET-PROOF.json, MANUAL-TIMESTAMP-TRANSLATION.json, MANUAL-AUDIO-SAMPLES.json, DECODED-FRAME-ORDER-WIDE.json, DECODED-FRAME-ORDER-SOURCE1.json und REFERENCE-FINDINGS.json. Erste zu kurze Decoderfenster erfassten nur den Keyframe-Vorlauf; das erweiterte Fenster umfasst die tatsächliche Auffälligkeit. Ein langsamer Diagnoseprototyp wurde für die PPS-Headerauswertung optimiert; die vorhandenen Paketnachweise weiterverwendet und dieselben Ergebnisse erneut erhalten.

Primärquellen zur Einordnung: [FFmpeg-Concat-Demuxer](https://ffmpeg.org/ffmpeg-formats.html#concat-1) beschreibt die versetzte Paketzeitachse, Dateidauern und automatische H.264-Konvertierung; [h264_mp4toannexb](https://ffmpeg.org/ffmpeg-bitstream-filters.html#h264_005fmp4toannexb) beschreibt die Umverpackung längenpräfigierter H.264-NALs in Startcodeform ohne Decodierung. Die hier genannten Integritäts- und Zeitbefunde stammen aus den lokalen Messungen, nicht allein aus der Dokumentation.
