# OTR-CAN 0.1.6 – lokale Musikladen-Integration

Stand: **19.10.2026**. Vom Nutzer zur Integration und zum lokalen Test freigegeben. **Keine Aufnahme in Setup oder Portable, keine Veröffentlichung. Musikladen-Testfilm und eigener CAN-Schnitt mit otr-can 0.1.6 vom Nutzer persönlich positiv bestätigt.**

## Ergebnis und Aktivierung

Die zuvor isoliert erprobte AAC-Korrektur ist jetzt Bestandteil des regulären Rust-Motors **otr-can 0.1.6**. Nur der lokale CAN-Motorpfad ist auf `../otr-native-poc/target/can-engine-0.1.6-build/release/otr_can.exe` umgestellt. Der bisherige installierte Motor bleibt erhalten; FFMS2-Pfad und alle anderen Einstellungen unverändert. CAN zum Einlesen des neuen Pfads neu starten und beim Schnitt **OTR-CAN** auswählen.

Die Versionskennung des CAN-Programms bleibt **1.0.1 · Build 33**. An CANs C#-Produktcode war keine Anpassung erforderlich: Die vorhandene CLI, Werkzeugprüfung und Motorfabrik akzeptieren 0.1.6. `capabilities` meldet `mp3-aac-v1 aac-overlap-v1`; MP3-Unterstützung bleibt erhalten. Paketierungsskript, vorhandene 1.0.0-/1.0.1-Setup-/Portable-Dateien, Quellarchive und Prüfsummen wurden nicht geändert.

## Technische Prüfung

- Regulärer Rust-Motor: **53/53 Tests**, Formatcheck und Release-Build mit Compilerwarnungen als Fehler bestanden. Keine neue Produktionsabhängigkeit; Cargo.lock ändert ausschließlich die eigene Paketversion.
- **14/14 synthetische Medienfälle** mit 0.1.6: sechs gesunde Referenzen bytegleich, drei gezielte AAC-Konfliktfälle bytegleich zum geprüften Versuch, fünf Fehlerfälle weiter abgelehnt. Kein fertiges Ergebnis bei Ablehnung; Indizes und Originale unverändert, private Arbeitsdateien bereinigt.
- **Vier synthetische Aufträge durch die echten CAN-Build-33-Assemblies**: MP3, gemischte MP3/AAC/AC3-Spuren, korrigiertes Stereo-AAC und korrigiertes AAC neben MP3/AC3. Motorfabrik, Werkzeugprüfung, Fortschrittsmeldungen und abschließende CAN-Ausgabeprüfung erfolgreich. Exakt eine Indexierung und ein Motoraufruf; Original-/Indexerhaltung und Bereinigung bestätigt. Strukturierte Schnittresultate behalten geplante Dauer und Übergänge. Alle MP4-Ausgaben bytegleich zu den zuvor geprüften Referenzen.
- **Vollständiger Musikladen-Auftrag über dieselbe CAN-Anbindung** mit unverändertem Keep `[1073.640000,4589.480000)` erfolgreich: **175792 Videopakete, 168760320 Audiosamples bei 48 kHz, Dauer 00:58:35.840**. Strenge native Video-PTS/DTS-, Audio- und Mux-Prüfungen sowie CAN-Ausgabeprüfung bestanden.
- Gesamte neue Musikladen-MP4 **bytegleich** zum früheren Versuch mit ausdrücklich eingeschalteter vollständiger Videodekodierung. Der aktuelle CAN-Auftrag verwendete den regulären Aufruf ohne `--verify-video`; die vollständige Dekodierprüfung stammt vom bytegleichen Referenzfilm.
- PCM aus dem tatsächlichen CAN-Auftrag vor Kodierung erfasst: ursprüngliche **168760354**, korrigierte **168760320** Samples. Beide PCM-Nutzdatenfolgen bytegleich zur unabhängig geprüften Versuchsauswahl; korrigierter PCM-SHA-256 `d42eea486fc34d9eb676da03801165d4c5d51b8f82100e23b597f5d22aca62fc`. Exakt **34** nachgewiesene Konfliktvektoren entfernt.

Der Prüfer verwendet die unveränderten vier Produkt-Assemblies aus `artifacts/release-1.0.1-build33/publish`. Er läuft mit expliziten Werkzeugpfaden und ohne FFmpeg im Prozess-PATH. Sein eigener Build hat **0 Warnungen/0 Fehler**. Die früheren 1385 CAN-Tests wurden für diese reine Motoränderung nicht erneut als Suite ausgeführt; die neuen Nachweise betreffen den regulären Rust-Motor und dessen echte CAN-Anbindung.

## Persönlicher CAN-Vergleich am 19.10.2026

Der Nutzer hat Musikladen selbst über CAN mit beiden Motoren geschnitten und meldet:

- **otr-can 0.1.6:** „ohne ndr-blitzer, alles prima, ton und bild“. Damit ist zusätzlich zum vorab geprüften Testfilm der selbst ausgeführte CAN-Schnitt persönlich positiv bestätigt.
- **MP4Box:** „wieder mit ndr-Blitzer am Anfang“, obwohl der Auftrag technisch mit „Fertig.“ endet und die Schnittstellenprüfung durchläuft. Dieser Anfangsblitzer bleibt ein offener MP4Box-Befund; die Positionsprüfung entfernt keine unerwünschten Quellbilder.

Das angehängte otr-can-Protokoll bestätigt Motor **0.1.6**, **34** gezielt korrigierte AAC-Konflikte, **175792** geprüfte Videopakete, **168760320** Audiosamples mit kontinuierlichen PTS, finale MP4-Prüfung und erfolgreichen CAN-Abschluss. Lokal archiviert als `.build/otr-can-0.1.6-integration-20261019/personal-can-cut-20261019.log`.

Die übermittelten Bereiche sind nicht vollständig gleich: MP4Box verwendet `1073.64:4589.46`, otr-can endet bei `4589.48`; beide beginnen bei `1073.64`. Die um 20 ms abweichende Endkante wird dokumentiert, ohne daraus eine Ursache für den Anfangsblitzer abzuleiten. Der MP4Box-Blitzer ist hier durch die persönliche Beobachtung belegt; eine neue bildweise Ursachenanalyse wurde nicht durchgeführt. Keine Änderung an Schnittparametern, Motoren oder Paketen in diesem Dokumentationsschritt.

## Abgleich der vom Nutzer benannten Ausgaben

Am 19.10.2026 ausschließlich lesend anhand von Dateigröße und SHA-256 verglichen:

- Nutzerdatei mit Endung `otr-can 016 2026-10-20.mp4`: **852624236 Bytes**, SHA-256 `5d454657649a7ac1f925171f381999b278f49ff2f8db2dd9aefb629e9429e71d`. Damit entspricht auch die tatsächlich vom Nutzer erzeugte fertige Datei exakt dem vollständig geprüften Versuchsfilm und der technischen CAN-Integrationsausgabe. Das Datum 2026-10-20 ist Bestandteil des vergebenen Dateinamens; die Prüfung erfolgte am 19.10.2026.
- Nutzerdatei mit Endung `MP4Box 2026-10-19.mp4`: **851463671 Bytes**, SHA-256 `f905c5f6138c80c936dd9322be31a669e03eef50fa83503c9ddd638ff8a82917`.
- Ältere Ausgabe ohne Motor-/Datumszusatz nach `[20.09.2026]`: ebenfalls **851463671 Bytes**, aber SHA-256 `8b6c9c4f6613620a8cef7aacf1fab15c51232e1cd2717b4267f087f79f8c7aba`. Die beiden gleich großen Dateien sind somit nicht bytegleich. Ob der Unterschied nur Metadaten oder auch Mediendaten betrifft, wurde hier nicht untersucht.

Vollständige Dateinamen und Prüfsummen lokal unter `.build/otr-can-0.1.6-integration-20261019/personal-output-hashes.json`. Keine Original- oder Ausgabedatei verändert; keine neue Paketierung oder Programmanpassung.

## Weiterer Realfilm: Navy CIS vom 18.10.2026

Am 19.10.2026 meldet der Nutzer einen erfolgreichen eigenen CAN-Schnitt von `Navy_CIS_26.10.18_20-15_5plus_50_TVOON_DE.HD.mp4` mit **otr-can 0.1.6**: „Bild und Ton und Übergänge top“. Damit liegt neben Musikladen eine weitere persönliche positive Realfilmprüfung vor.

Die vom Nutzer genannte Cutlist wurde ausschließlich als Daten gelesen und mit dem übermittelten Laufprotokoll abgeglichen:

| Keep | Start | Ende exklusiv | Dauer |
|---|---:|---:|---:|
| 1 | 583.160 s | 1938.720 s | 1355.560 s |
| 2 | 2587.320 s | 3643.280 s | 1055.960 s |

Die Cutlist stimmt exakt mit den protokollierten Framebereichen `[14579,48468)` und `[64683,91082)` bei **25 fps** überein. Summe **2411.520 s = 00:40:11.520**, entsprechend **60288 Videobildern** und **115752960 Samples pro Tonspur bei 48 kHz**; diese Werte wurden aus der Cutlist unabhängig nachgerechnet.

Das Nutzerprotokoll bestätigt eindeutige aufsteigende Quellbildzeiten, 60288 geprüfte Videopakete mit PTS/DTS, eine AAC-Stereo- und eine AC3-Stereo-Spur sowie je 115752960 dekodierte Samples mit kontinuierlichen PTS. Finale native MP4-/Paketprüfung und CAN-Ausgabeprüfung erfolgreich. Für diesen Lauf wird keine AAC-Überlappkorrektur protokolliert: Die ursprüngliche PCM-Auswahl passt bereits exakt. Damit ist auch der reguläre Audiopfad mit AAC/AC3 unter 0.1.6 an einem weiteren Realfilm erfolgreich durchlaufen. Eine zusätzliche vollständige Videodekodierung wurde im vorgelegten regulären Aufruf nicht nachgewiesen.

Die Cutlist nennt in ihren beschreibenden Anwendungs-/Kommentarfeldern MP4Box und CAN 1.0.0. Das aktuelle Laufprotokoll weist ausdrücklich **Motor 0.1.6** aus; die gespeicherten Beschreibungsfelder sind kein Gegenbeleg zum tatsächlich ausgeführten Schnitt. Cutlist, Filme und diese Metadaten wurden nicht geändert. Kein neuer Schnitt oder Build in diesem Dokumentationsschritt, Setup-/Portable-Pakete weiter unverändert.

## Korrekturregel und Grenzen

Nur eine zu große PCM-Menge einer AAC-LC-Spur aktiviert den Zusatzpfad. Jede Entfernung muss einzeln durch 1024 dekodierte Samples bei 1023 ausgewiesenen Sample-Ticks und genau einen Tick Überschneidung mit dem folgenden 1024-Sample-Frame bewiesen sein. Der folgende Frame besitzt die betroffene Originalzeit; sämtliche Kanäle des überzähligen Vorgänger-Samples werden gemeinsam entfernt. Vollständige Keep-Abdeckung, passende Formate/Zeitbasis und exakte Erklärung der gesamten Differenz sind Pflicht. Danach erneute Prüfung des korrigierten WAV/RF64 und alle bisherigen Ausgabeprüfungen.

Keine pauschale Sampletoleranz oder globale Zeitverschiebung. Echte Lücken, größere/unbelegte Überschneidungen, fehlender Ton und AC3-Konflikte werden nicht hierdurch freigegeben. Bei bereits passender PCM-Zahl erfolgt weiterhin keine zusätzliche allgemeine AAC-Quellzeitprüfung. Der Zusatzpfad benötigt eine weitere Quellprüfung durch ffprobe und temporären Platz für eine zweite PCM-Datei. Der Nutzer bestätigt am 19.10.2026: „testfilm ist ok“. Damit ist die persönliche Prüfung des Musikladen-Testfilms positiv abgeschlossen. Anschließend bestätigt der Nutzer auch seinen eigenen CAN-Schnitt mit otr-can 0.1.6: ohne NDR-Blitzer, Bild und Ton einwandfrei. Setup-/Portable-Abnahme für Motor 0.1.6 weiterhin nicht erfolgt.

## Dateien und Rückweg

Geändert im regulären Motor: `Cargo.toml`, `Cargo.lock`, `src/main.rs`, `src/cutting/audio.rs`, `src/cutting/mod.rs`, `tests/cli.rs`, `README.md`; neu `src/cutting/audio_overlap.rs`. Zusätzlich dieser CAN-Bericht und ausschließlich die lokale Motorpfadeinstellung. Die schon vorher vorhandenen CAN-Änderungen und Lockfiles bleiben erhalten.

Lokale Nachweise unter `.build/otr-can-0.1.6-integration-20261019`: Rust-Test-/Buildprotokolle, `NATIVE-MATRIX.json`, fünf `can/*/RESULT.json`, Musikladen-`protocol.txt`, `PCM-PROOF.json`, `PRESERVATION.json` sowie Prüferquellen. Der vollständige ursprüngliche Motorquellstand ist unter `original-can-engine-0.1.5` gesichert und gegen den Ausgangszustand geprüft. Der getrennte Korrekturversuch und beide älteren Referenz-EXEs bleiben unverändert.

Der vorherige lokale Motorpfad ist in `otr-can-settings-before.json` sowie `settings-activation-before.json` gesichert. Rückwechsel bei Bedarf unter Einstellungen → OTR-CAN auf den vorherigen Pfad; der Indexerpfad bleibt gleich. Keine automatische Rückstellung oder Änderung an der installierten EXE.

[Testfilm aus der CAN-Integration](../.build/otr-can-0.1.6-integration-20261019/can/Musikladen/CAN%20Ergebnis%20-%20%C3%A4.mp4): **852624236 Bytes**, SHA-256 `5d454657649a7ac1f925171f381999b278f49ff2f8db2dd9aefb629e9429e71d`.

Hintergrund und unabhängiger PCM-Nachweis: [Diagnose](OTR-CAN-MUSIKLADEN-PCM-DIAGNOSE-20261019.md), [isolierter Korrekturversuch](OTR-CAN-MUSIKLADEN-KORREKTURVERSUCH-20261019.md). Diese älteren Berichte dokumentieren den damaligen Stand ohne Integration.
