# Furiosa / MP4Box – AUDIO-ENTHUSIAST-Vorprüfung, 31.10.2026

**Späterer Nachtrag: Entwicklungsbuild 47** setzt die anschließende Nutzerentscheidung um, AUDIO-ENTHUSIAST für erkannte MP4Box-Filmteile zu sperren. [Umsetzung und Prüfungen](MP4BOX-AUDIO-ENTHUSIAST-SPERRE-20261031.md). Die folgende Diagnose beschreibt die vorherige Ablehnung in Build 46; dessen Pakete bleiben erhalten.

Der Nutzer zeigt eine weitere Ablehnung im Klebecenter: **„Diese erste Fassung benötigt progressive H.264-MP4-Dateien mit durchgehenden 25 oder 50 Bildern/s ab Zeit null.“** Der Screenshot nennt lokale Dateien auf dem Arbeitsrechner, nicht die zuvor verwendeten VM-Pfade.

## Geprüfte Eingaben

- `C:\Users\joerg\Desktop\down\Furiosa A Mad Max Saga Teil 1 MP4Box 22_09.mp4`
- `C:\Users\joerg\Desktop\down\Furiosa  A Mad Max Saga  Teil 2 MP4Box.mp4`

Die zweite Datei wurde anhand des vorhandenen Namens mit doppelten Leerzeichen aufgelöst. Ausgewählt ist laut Screenshot jeweils AAC-Spur 1, Deutsch/Stereo/48000 Hz. Die QUICK-Paare sind abgewählt; das sperrt QUICK, ist aber nicht der Grund der AUDIO-ENTHUSIAST-Ablehnung.

## Aktuelle lesende Metadatenprüfung

| Angabe | Teil 1 (22_09) | Teil 2 |
|---|---|---|
| Video | H.264, progressiv | H.264, progressiv |
| Video-Start-PTS | 0 | 0 |
| Nominelle Bildrate | 25/1 | 25/1 |
| Mittlere Bildrate | 647398006/25896375 ≈ 24,999561 | 364176000/14567497 ≈ 24,999216 |
| Videozeitbasis | 1/48000 | 1/48000 |
| Videodauer in Ticks | 328744333 | 58269988 |
| AAC-Dauer in Ticks (1/48000) | 328743312 | 58269312 |
| AAC kürzer als Bild | 1021 Samples ≈ 21,271 ms | 676 Samples ≈ 14,083 ms |

Die aktuellen Metadaten treffen die Bildvorprüfung in `FfprobeStreamCopyVerifier.ParseJoinPart`: `avg_frame_rate` muss exakt mit `r_frame_rate` übereinstimmen. Das scheitert bei beiden Dateien und erklärt die sichtbare Meldung bereits vor dem Verbindungsstart. H.264, progressives Bild und Start null sind hier vorhanden.

Weitere strenge Voraussetzungen wären ebenfalls zu untersuchen: Die Videodauern liegen nicht auf vollständigen 40-ms-Schritten, die AAC-/Videodauern sind nicht identisch, und die H.264-Konfigurationshashes beider Teile unterscheiden sich. Unterschiedliche Hashes allein beweisen keine Decoderinkompatibilität. Die Angabe „Samples“ in der Tabelle ist das Zeitäquivalent der Dauerabweichung bei 48000 Hz, keine neu gemessene vollständige PCM-Samplezahl.

## Einordnung und Lösungsumfang

Die vorher vorgeschlagene gezielte Tonauffüllung allein löst diesen Furiosa-Fall nicht. AUDIO-ENTHUSIAST müsste zusätzlich die tatsächlich vorhandenen Bildzeitachsen und gegebenenfalls wechselnde H.264-Parameter nachweisbar unverändert übernehmen können. Dafür sind Paket-/Decoderprüfungen und eine zur Bildzeit passende PCM-Zusammensetzung erforderlich. Eine pauschale Änderung der Headerbildrate oder alleinige Lockerung der Vorprüfung genügt nicht; nachgelagerte strenge Zeit-/Konfigurationsprüfungen würden weiterhin greifen.

Der frühere [Furiosa-Befund vom 29.10.2026](KLEBECENTER-MP4BOX-FURIOSA-DIAGNOSE-20261029.md) enthält Paketzeitabweichungen für die damalige Teil-1-Datei ohne `22_09` und dieselbe Teil-2-Datei. Die neu gemessenen ausgewählten Metadaten stimmen mit den damaligen Werten überein; daraus wird keine Byteidentität der anders benannten Teil-1-Dateien abgeleitet. Kein neuer vollständiger Paket-/Decoderlauf oder Synchronitätsnachweis durchgeführt.

**Offener Kompatibilitätsbefund:** Kein weiterer bestandener VM-Funktionsfall, keine Produktkorrektur umgesetzt. Die drei positiven VM-Funktionsfälle bleiben erhalten. Aktuell ist QUICK AND COPY der bereits erfolgreich persönlich geprüfte pragmatische Kopierweg; seine Prüfgrenzen bleiben davon unberührt.

## Erhaltung und Nachweise

Nur ffprobe-Metadaten gelesen; keine Schnitte, Neukodierung, Zeitkorrektur oder Änderung der Filmdateien. Dateigrößen und Änderungszeiten vor/nach der Analyse gleich. Lokale Rohdaten unter `.build/furiosa-enthusiast-20261031/part-1.json`, `part-2.json` und `SUMMARY.json`. Produktcode, Buildnummer 46 und eingefrorene Pakete unverändert.

[Laufende VM-Prüfung](VM-SETUP-BUILD46-20261031.md), [Testplan](TESTPLAN.md), [Projektstand](AKTUELLER-STAND.md).
