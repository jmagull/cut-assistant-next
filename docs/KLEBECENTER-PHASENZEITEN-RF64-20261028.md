# Klebecenter – Phasenzeiten und direkte RF64-Zusammensetzung

**Stand 28.10.2026: lokal implementiert und geprüft, Entwicklungsbuild 42.** Veröffentlichter stabiler Stand unverändert CAN 1.0.2 Build 41 mit otr-can 0.1.6. Keine neuen Pakete, kein Commit/Push und keine Veröffentlichung.

## Auftrag und Umsetzung

Nach dem [Untersuchungsbericht](KLEBECENTER-PERFORMANCE-ANALYSE-20261028.md) hat der Nutzer zuerst Phasenzeiten im unveränderten Ablauf, danach die Beseitigung der zusätzlichen PCM-Kopie freigegeben. Kein GPU-/Encoderwechsel und keine Parallelisierung.

**Phasenzeiten:** Mp4JoinService misst mit monotoner Zeitbasis. Protokollzeilen mit Präfix Phasenzeit: enthalten JSON-Felder Phase, Elapsed, ElapsedMilliseconds, Succeeded. Pro Aufruf mit measurePhases: false abschaltbar; optionaler IProgress<JoinPhaseTiming> für strukturierte Auswertung. Keine neue globale Einstellung oder zusätzliche Bedienfrage. Erfasst werden Eingabeanalysen, Video-/Tonzeitachsen, Tondekodierung, Videokopie und deren Vergleich, PCM-Zusammensetzung/Readback, AAC, Mux, Ausgabeanalyse und alle abschließenden Paket-/Tonprüfungen, Vorschau, SHA-256, Übernahme und Bereinigung. total umfasst den Dienst ab angelegtem Arbeitsprotokoll samt Bereinigung, nicht die Dateiauswahl. Cleanup-/Total-Zeiten gehen nach Schließen des Arbeitsprotokolls noch an Fortschrittsdialog/Beobachter. Succeeded bezeichnet normale Rückkehr des gemessenen Aufrufs; Bereinigungswarnungen bleiben eigene Meldungen.

Werkzeugbefehle, Codecoptionen und Grenzen bleiben gleich. CPU-/I/O-Zähler und Werkzeugidentität werden bei Vergleichsläufen außerhalb der Produktpipeline erfasst. Die instrumentierte Variante wurde vor jeder PCM-Optimierung getrennt als Baseline gesichert: 296 Medientests und 21 native Fälle bestanden.

**Direktes RF64:** Bisher zwei dekodierte PCM-Dateien → joined.pcm → vollständiger Readback → Kopie nach joined.wav. Jetzt dieselben PCM-Dateien direkt nach joined.wav. Erwarteter Payload-SHA entsteht beim Lesen in Eingabereihenfolge. Nach Schließen des Writers wird die fertige Datei unabhängig neu gelesen: Header mit Kanalmaske und 64-Bit-Samplezahl, exakte Dateilänge und kompletter PCM-Payload-SHA werden geprüft. Fehlende/zusätzliche/unvollständige Samples werden abgelehnt, CreateNew schützt vorhandene Dateien und CancellationToken wird weitergereicht.

Unverändert: ausgewählte Tonspuren, einmaliger AAC-LC-Encode und Bitrate, Video-Streamcopy/Zeitbasis, alle nachfolgenden Paket-/PTS-/DTS-/Paddingprüfungen, vollständiger Ausgabe-Tondecode, Drei-Sekunden-Vorschau und Erfolgs-/Fehlerbereinigung. Keine weitere Pipe- oder Null-Ausgabe-Optimierung.

Für John Wicks dokumentierte PCM-Menge B = 3659197440 Bytes entfallen B Bytes Lesen plus B Bytes Schreiben: **7,32 GB weniger logische PCM-I/O**. Maximaler Platzbedarf wird nicht pauschal halbiert; konservativer Speicherplatzschutz bleibt.

## Prüfungen

- Vollständiger lokaler Release-Build **42**, **0 Warnungen/Fehler**. Version.props weiterhin 1.0.2, Motor 0.1.6.
- **1505 Tests bestanden:** 200 Core, 72 Cutlists, 308 Media, 925 App; nichts übersprungen.
- Tests für monotone Zeit, Ergebnis/Fehler, Abschalten und Cancellation; RF64-Nutzdatenreihenfolge, erhaltene Eingaben, Mono/Stereo/5.1/5.1(side), Größen über 4 GiB, kurze/zusätzliche/unvollständige/fehlende Daten, Header-/Payload-Manipulation, Dateilänge, vorhandene Ziele, Abbruch und Überlauf.
- **21 native Fälle je Variante**, also 42 Vergleichsläufe, bestanden. Alle **neun erfolgreichen MP4-Ausgaben vollständig byteidentisch** zwischen Baseline und Optimierung.
- Zwei zusätzliche native Läufe: Zeitmessung abgeschaltet ergibt identische MP4 ohne Messzeilen; Abbruch nach fertig geprüftem RF64 ergibt kein Ziel und lässt nur protocol.txt, keine PCM/WAV-Dateien zurück.
- RF64 über 4 GiB in allen acht Aufwärm-/Messläufen byteidentisch. Beide synthetischen Eingaben danach erneut vollständig gehasht und unverändert.

Drei erste neue Testfehler betrafen die falsche Erwartung IOException statt InvalidDataException bei ungültigen PCM-Daten. Präzise Fehlertypen korrigiert, Fehlerfälle nicht entfernt; erster Lauf lokal erhalten.

## A/B-Leistungsvergleich

Gleiche Messhilfe und zwei identische synthetische Float32-PCM-Dateien: **4300000000 Bytes**, Stereo/48 kHz, **537500000 Samples je Kanal**. Keine nachgebildeten John-Wick-Originalteile. Vollständige RF64-Datei jeweils **4300000104 Bytes**, tatsächlich über 4 GiB.

Windows-Job mit 1 GiB und 25 % CPU, ein Aufwärmlauf und drei Messungen pro Variante, Reihenfolge abgewechselt, kein absichtlich geleerter Cache. Die anschließende vollständige Vergleichsdatei-Hashbildung liegt außerhalb der Funktionszeit, aber innerhalb der gesonderten Prozess-I/O-Zähler. Gemessen wird nur PCM-Zusammensetzung samt jeweiligem Readback.

| Variante | Drei Zeiten | Median | Bereich |
|---|---|---:|---|
| joined.pcm + Readback + RF64-Kopie | 14,23 / 13,61 / 14,19 s | **14,19 s** | 13,61–14,23 s |
| Direkt RF64 + vollständiger Readback | 11,34 / 11,54 / 11,54 s | **11,54 s** | 11,34–11,54 s |

**2,65 s bzw. 18,7 % weniger Zeit für diesen Schritt.** Pro Lauf entfallen **4,3 GB Lesen plus 4,3 GB Schreiben**. Alle vollständigen RF64-Dateien haben denselben SHA-256:

**92DA143B070A20E3F0812044D88DAD92966C2668246A22C26FAC6645C3B435F1**

Das ist **keine Aussage über 18,7 % Beschleunigung des ganzen Klebecenters**. AAC, Paketprüfungen und Muxing bleiben unverändert.

## Persönlicher John-Wick-Praxistest vom 28.10.2026

Der Nutzer bestätigt für den lokalen Entwicklungsstand Build 42: **Bild, Ton, Schnitt und Drei-Sekunden-Vorschau positiv**. Das eingereichte Klebecenter-Protokoll enthält alle 25 Phasen mit Succeeded=true und endet einschließlich Bereinigung erfolgreich; keine Warnungen oder Fehler im eingereichten Text.

Eingaben: **John Wick Kapitel 4 Teil 1.mp4** und **John Wick Kapitel 4 Teil 2.mp4**, jeweils Spur 1 (AAC, Deutsch, Stereo, 48000 Hz). Diese Namen unterscheiden sich von den früher dokumentierten Referenzpfaden; eine Bytegleichheit mit den früheren Originalteilen ist damit nicht nachgewiesen.

| Gemessener Schritt | Zeit |
|---|---:|
| Gesamt einschließlich Bereinigung | **300,54 s (05:00,54)** |
| Durchgehende AAC-Kodierung | **215,84 s (03:35,84)** |
| Direkte RF64-Zusammensetzung samt vollständiger Prüfung | **10,88 s** |
| Zusammenführen zur finalen MP4 | **9,45 s** |
| Vollständiges Dekodieren des fertigen Tons | **7,39 s** |
| Sechs-Sekunden-Prüfung der Klebestelle | **0,76 s** |
| Bereinigung | **0,91 s** |

Die AAC-Kodierung belegt **71,8 %** der gemessenen Gesamtzeit und ist der größte Einzelposten. Das Protokoll bestätigt **457399680 Tonsamples je Kanal**, Klebestelle **01:57:37.120**, Gesamtdauer **02:38:49.160** und Vorschau ab **01:57:34.120**. Diese Zeit-/Samplewerte entsprechen der dokumentierten John-Wick-Referenz.

Dies ist ein erfolgreicher echter Filmlauf samt persönlicher Kontrolle, kein A/B-Gesamtzeitvergleich: Für diese beiden konkreten Eingaben liegt noch kein gemessener Lauf des unveränderten früheren Ablaufs vor. Keine prozentuale Gesamtbeschleunigung ableiten. Die erfolgreiche Cleanup-Phase meldet normale Rückkehr; eine gesonderte persönliche Sichtkontrolle des entfernten Arbeitsordners wurde in dieser Rückmeldung nicht genannt.

Quelle: vom Nutzer eingereichtes Protokoll, Anhang **50267686-d14a-4994-a2ac-384a2847c07e/Eingefügter Text.txt**, und persönliche Rückmeldung „BIld, Ton, Schnitt; 3 Sekunden Vorschau check“. Keine Produktänderung durch diese Dokumentation; bestehende 1505 Tests unverändert, keine erneute Buildnummer und keine neuen Pakete.

## Grenzen und weiterer Ablauf

Bei der technischen Umsetzung fehlten die ursprünglichen John-Wick-Teile an den früher dokumentierten Pfaden; ein vollständiger A/B-Vergleich mit dieser Originalreferenz wird weiterhin nicht behauptet. Der oben ergänzte persönliche Filmlauf verwendet die neu gemeldeten Eingaben. Neun bitidentische native Erfolgsausgaben und der große synthetische PCM-Test belegen den vorhandenen Prüfumfang. Persönliche Prüfungen weiterer Filme bleiben getrennt. Der erste persönliche Lauf mit kopierten Phasenzeiten ist oben dokumentiert; weitere Vergleichsläufe können dieselben Messzeilen verwenden. Keine neuen Setup-/ZIP-Pakete oder automatische Veröffentlichung.

## Dateien und Nachweise

Geändert: Mp4JoinService.cs, neu JoinPhaseTimer.cs, angepasst Mp4JoinTests.cs, neu JoinPhaseTimerTests.cs. Keine neue Produktionsabhängigkeit, UI- oder Motoränderung.

Nachweis unter **.build/klebecenter-optimization-20261028/**: INSTRUMENTED-SOURCE.cs, getrennte Bibliotheken, final-tests/, baseline-/candidate-NATIVE.json, NATIVE-COMPARISON.json, TARGETED-NATIVE.json, Phasenbeispiele und pcm-benchmark/SUMMARY.json. Große eigene PCM-Dateien nach Abschluss entfernt; Zeiten, Befehle und Hashnachweise bleiben.

Alle acht vorhandenen Produktions-/Test-Lockdateien und Version.props bleiben bytegleich. Isolierte Restore-Locks liegen nur unter .build; externe Paketauflösungen entsprechen den bisherigen Basisframework-Abhängigkeiten. Veraltete interne Projektversionsangaben der Nutzer-Lockdateien bleiben erhalten. Erfolgreicher Vollbuild erhöht lokalen Zähler **41 → 42**. Eingefrorene Build-41-Pakete unverändert.
