# Furiosa / MP4Box – Klebecenter-Vorprüfung, 29.10.2026

**Historischer Nachweis:** Die anschließend verbindlich korrigierte Nutzeranweisung ist ab [Build 46](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md) umgesetzt. QUICK arbeitet nun pragmatisch; die hier beschriebenen strengen Abbruchregeln und eine damalige Rücksprache-Sperre gelten nicht mehr für den aktuellen QUICK-Produktweg. Messungen und der eingefrorene Build-45-Stand bleiben erhalten.

## Zuordnung und Ergebnis

Nach VM-Ablehnung meldet der Nutzer denselben Effekt auf dem Arbeitsrechner und zeigt im Screenshot **8713de49-84c9-40a9-8b23-0da55eb0eefc** die lokalen Eingaben:

- **C:\Users\joerg\Desktop\down\Furiosa  A Mad Max Saga  Teil 1 MP4Box.mp4**
- **C:\Users\joerg\Desktop\down\Furiosa  A Mad Max Saga  Teil 2 MP4Box.mp4**

Diese beiden lokalen Dateien wurden untersucht. Eine bytegleiche Identität mit den laut Nutzer in der VM neu erzeugten Dateien wurde nicht bewiesen. Die vorangegangene Rückmeldung „andere Dateien“ wird durch den später gezeigten lokalen Pfad präzisiert; der Befund bezieht sich ausdrücklich auf die Screenshot-Pfade.

**Ablehnung mit unveränderten finalen Build-45-Bibliotheken nachgestellt.** Beide MP4Box-Teile scheitern an der Bildvorprüfung; beide daneben liegenden otr-can-Vergleichsdateien bestehen sie. Kein allgemeines MP4Box-Verbot, kein Nachweis eines VM-spezifischen Fehlers.

## Konkrete Befunde

Die MP4Box-Header nennen **25/1** als Bildrate, aber **647398006/25896375** (Teil 1) bzw. **364176000/14567497** (Teil 2) als mittlere Bildrate. Build 45 fordert hier bereits identische Angaben. H.264, progressives Bild und Startzeit null sind bei beiden vorhanden; sie sind nicht der Ablehnungsgrund.

Die vollständigen Videopaket-Zeitlisten zeigen tatsächliche Abweichungen vom 40-ms-Raster, nicht nur anders gerundete Headerwerte:

| Fassung | Videopakete | Abweichende Abstände zwischen sortierten Bildzeiten |
|---|---:|---:|
| MP4Box Teil 1 | 171218 | 9 |
| MP4Box Teil 2 | 30348 | 3 |
| otr-can Teil 1 | 172041 | 0 |
| otr-can Teil 2 | 30338 | 0 |

Beispiel Teil 1: aufeinanderfolgende Präsentationszeiten mit **151,729 ms**, **8,271 ms**, **31,729 ms** statt stets 40 ms. Teil 2 unter anderem **80 ms** und **38,083 ms**. Die Dekodierzeitabstände und einzelne Paketdauern weichen ebenfalls ab. Nach einem kleinen Zeitversatz können viele folgende Bilder außerhalb des ursprünglichen 40-ms-Rasters liegen; das bedeutet nicht, dass entsprechend viele Bilder beschädigt sind. Es gibt keine doppelten Präsentationszeiten.

Weitere Grenzen der aktuellen Klebefassung: Bei den MP4Box-Dateien sind die ausgewiesenen Video-/AAC-/AC-3-Endzeiten nicht exakt gleich; außerdem unterscheiden sich die H.264-Decoderkonfigurationen der beiden Teile (unterschiedliche extradata_hash-Werte). Nur das Überspringen des Headervergleichs würde daher keine nachgewiesen sichere Freigabe ergeben. Die otr-can-Vergleichsdateien haben durchgehend exakte 40-ms-Bildschritte, deckungsgleiche ausgewiesene Bild-/Tonzeiten und passende Decoderkonfigurationen.

Unterschiedliche Framezahlen und Gesamtlängen der Vergleichsfassungen belegen **keine verlorenen Filmbilder**: Identische Schnittpläne oder inhaltliche Deckung wurden hier nicht geprüft. Ebenso kein Vollvideodekodier- oder subjektiver Bild-/Tonvergleich dieser MP4Box-Dateien behauptet.

## Bedeutung und weiteres Vorgehen

Build 45 unterstützt beim Kleben eine exakt geprüfte, durchgehende 25/50-fps-Zeitstruktur und vollständige deckungsgleiche Tonachsen. Die geprüften MP4Box-Dateien erfüllen das nicht. Sie können trotzdem einzeln normal abspielbar sein; ihre grundsätzliche Schnittqualität wird hier nicht bewertet.

Für den aktuellen Furiosa-Fall ist der bereits persönlich auf Bild/Ton/Übergang bestätigte otr-can-/QUICK-Weg der geprüfte Arbeitsweg. Eine Erweiterung für diese MP4Box-Zeitstrukturen wäre ein **separater Entwicklungsauftrag** mit Nachweis unveränderter Bildpakete, richtiger Zeitachsenbehandlung, Decoderkonfigurationen und Ton-Synchronität. Nicht einfach Prüfungen abschalten oder pauschal neue Zeitstempel erzeugen. Die zu allgemeine Vorprüfungsfehlermeldung sollte bei einer späteren Überarbeitung den konkreten Grund nennen.

## Durchführung und Erhaltung

Lesende ffprobe-Metadaten-/Paketprüfung; vier Paketprozesse nacheinander, jeweils maximal **1 GiB / 25 % CPU**. Kein Schnitt, Remux, Encoding oder Vollvideodekodieren. Dateigrößen und Änderungszeiten der Eingaben unverändert. 371 Produkt-/Testdateien und alle acht eingefrorenen Build-45-Auslieferungsdateien per Hash unverändert. Keine neuen Pakete, kein Commit/Push oder Release.

Nachweise: **.build/mp4box-join-diagnosis-20261029/** – METADATA.json, FROZEN-BUILD45-RESULT.json, PACKET-ANALYSIS.json, FINDINGS.json, rohe ffprobe- und Ressourcenprotokolle. Die zusätzlich vorbereitete Windows-PowerShell-5.1-Diagnosehilfe wurde auf lokalen Dateien erfolgreich lesend geprüft, aber nicht als VM-Nachweis ausgeführt.

## VM-Rückmeldungen separat

Nach Einrichtung der Werkzeugpfade läuft John Wick aus otr-can laut Nutzer in der VM positiv. Furiosa aus otr-can mit QUICK AND COPY technisch abgeschlossen und danach **Bild/Ton/Übergang** persönlich bestätigt; AAC und AC-3 erhalten, je erklärte +1280 Samples / 26,667 ms, Laufzeit 00:03:16.41. Die zwei Anhänge 16776d94-e749-4a7d-befe-12e70533ffcb und a31ff65b-c3f9-4c19-a791-2b20ab9d1268 sind identisch und zählen als ein Lauf. Keine pauschale VM-/Portable-Gesamtabnahme daraus abgeleitet.
