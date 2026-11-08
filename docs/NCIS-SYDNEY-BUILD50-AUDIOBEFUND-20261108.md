# NCIS Sydney – bekannter Audiofall in Spotlight Build 50

Stand **08.11.2026**. Der Nutzer meldet den gleichen otr-can-Abbruch in der VM und auf dem Arbeitsrechner für **NCIS Sydney – Breathless, S02E07**. Screenshot: **115580160 erwartete / 115579648 vorhandene PCM-Samples**, Differenz **512 Samples** in Tonspur 1. Die Videoverarbeitung und Verifikation von **60198 Videopaketen** sind bereits erfolgreich; der Abbruch liegt in der anschließenden Audioaufbereitung.

## Abgleich mit dem früher gelösten Fall

Die getrennte Audio-Entwicklung dokumentiert exakt dieselbe Datei, dieselben drei Behaltebereiche und dieselben Soll-/Ist-Samplezahlen. Die innere AAC-Lücke liegt im dritten Behaltebereich bei Originalzeit rund **01:04:18.645** und beträgt **512/48000 s = 10,667 ms**. Der bereits untersuchte kürzere Ton am Gesamtdateiende ist für diesen konkreten Schnitt nicht die Ursache.

Die aktuelle persönliche Cutlist wurde ausschließlich gelesen. Sie nennt CAN 1.0.3, 25 fps und die Behaltebereiche ab **430,76 / 1916,04 / 3449,52 s** mit **992,28 / 1006,48 / 409,16 s** Dauer. Summe **2407,920 s = 00:40:07.920**. Das stimmt exakt mit dem früheren Audio-Referenzfall überein. Die Zuordnung von VM/Arbeitsrechner folgt der Nutzerangabe; die aktuelle Installation auf dem Arbeitsrechner wurde zusätzlich geprüft.

Frühere Nachweise im getrennten Checkout `cut-assistant-next-aac`:

- `docs/AUDIO-DREI-OPTIONEN-20261105.md`: persönlicher vollständiger NCIS-Schnitt und persönlicher Setup-Test mit **Build 51** erfolgreich nach ausdrücklicher Auswahl **AAC-Zeitlücken reparieren (experimentell)**. 512 positionsgenau ergänzte Stillevektoren, 115580160 Nutzsamples; Bild, Ton, Schnitt und Drei-Sekunden-Vorschau persönlich bestätigt.
- `docs/FREEZE-1.0.3-BUILD52.md`: derselbe Reparaturweg auch mit installiertem **Build 52** persönlich bestätigt. Zusätzlich Tonspur-Abwahl für andere Fehlerfälle.
- Die ursprüngliche Diagnose liegt separat unter `otr-can-audio/docs/NCIS-SYDNEY-AAC-DIAGNOSE-20261104.md`.

Der reguläre erste Schnittversuch bleibt auch dort streng und bricht bei dieser Lücke ab. Die Lösung besteht aus Diagnose, ausdrücklicher Anwenderentscheidung und gesondert geprüfter Reparatur, nicht aus einer pauschalen Toleranz für fehlende Samples.

## Tatsächlich installierter und paketierter Stand

Installierte CAN-Dateiversion am Arbeitsrechner: **1.0.3.50**. Installierte Motor-EXE unter `tools/otr-can`, Motor im Spotlight-Publish und Motor aus dem stabilen CAN-1.0.2-Build-48-Paket haben denselben SHA-256:

`73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E`

Damit sind die bisherigen regulären 0.1.6-Korrekturen enthalten. Der gesonderte Diagnose-/Reparaturmotor **0.1.6-audio-diag.1** und CANs Audio-Reparaturdialog/-Ablauf aus Build 51/52 sind dagegen **nicht Bestandteil des Spotlight-Build-50-Pakets**.

Der Paketauftrag für Build 50 schloss die ungeprüfte Übernahme der getrennten Audio-Entwicklung ausdrücklich aus. Diese Branch-Trennung erklärt die fehlende Funktion; es wurde nicht versehentlich eine ältere stabile Motor-EXE eingepackt. Beim anschließenden NCIS-Test hätte auf diese bereits bekannte Einschränkung hingewiesen werden müssen. Die positive Abnahme von Grey’s Anatomy und Buchladen bleibt gültig, ist aber keine Abdeckung dieses AAC-Lückenfalls.

## Konsequenz für den gemeinsamen Entwicklungsstand

Für einen gemeinsamen Nachfolger müssen die persönlich geprüfte Audio-Fehlerbehandlung aus Build 52 und die neuere Spotlight-Fensterpersistenz kontrolliert zusammengeführt und gemeinsam gebaut/geprüft werden. Ein Austausch nur der Motor-EXE integriert den fehlenden CAN-Dialog und Reparaturablauf nicht. Fehlende Samples werden in Build 50 nicht stillschweigend ergänzt und Prüfungen nicht gelockert.

Dieser Schritt ist noch nicht umgesetzt. In dieser Untersuchung wurden nur Quellstände, Dokumentation, Cutlist und Motorhashes verglichen; kein erneuter Filmschnitt, kein Austausch der installierten Dateien, kein neuer Build und keine Änderung der vorhandenen Pakete. Der aktuelle NCIS-otr-can-Test ist **fehlgeschlagen**, nicht bestanden. Kein Release, Tag oder Merge nach main.
