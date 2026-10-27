# CAN 1.0.2 Build 41 – Veröffentlichung

Am **27.10.2026** nach ausdrücklichem Nutzerauftrag veröffentlicht: **[v1.0.2 / Latest](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2)**. GitHub-Veröffentlichungszeit: **2026-10-27T21:02:07Z**. Kein Entwurf, keine Vorabversion.

## Code und Herkunft

- Release-Commit und Tagziel: **6f4c3dcae54e9c8ecad6e03df667ca7c6f33da00** ([Commit](https://github.com/jmagull/cut-assistant-next/commit/6f4c3dcae54e9c8ecad6e03df667ca7c6f33da00)). Main und annotierter Tag **v1.0.2** gepusht.
- **361 Programm-/Testquelldateien** des Commits stimmen mit dem geprüften Build-41-Snapshot überein; Dokumentation ergänzt spätere Abnahme und Freigabe.
- CAN **1.0.2**, App-Dateiversion **1.0.2.41**, Motor **otr-can 0.1.6**, libmpv **0.41.0**. Die Motor-EXE meldet tatsächlich 0.1.6; EXE in Portable und Publish hashgleich geprüft.
- Motor-SHA-256: **73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E**. Bei Überinstallation den Werkzeugpfad auf **tools/otr-can/otr_can.exe** im neuen Programmordner prüfen.
- Vier vorbestehende lokale Produktions-Lockdateiänderungen unverändert erhalten und nicht committed. Buildausgaben und private Filme nicht im Commit.

## Prüfung und Abnahme

1489 Pakettests, zusätzlich 1217 Tests gegen finale Produktbibliotheken und 21 native Klebecenter-Fälle bestanden. Build ohne Warnungen/Fehler. Build-40-Setup/ZIP in der VM persönlich bestätigt; Build-41-Setup auf dem Arbeitsrechner mit John Wick einschließlich entfernter temporärer Datei und Bild/Ton/Schnitt/Drei-Sekunden-Vorschau/Klebecenter persönlich bestätigt. Zusätzliche Build-41-ZIP-/VM-Prüfung bleibt als separater Umfang offen.

Die acht Assets wurden zunächst als Entwurf hochgeladen. Vor dem Freischalten wurden alle Größen und von GitHub berechneten SHA-256-Digests gegen die eingefrorenen lokalen Dateien geprüft. Danach öffentliche Latest-Version und unveränderte Assetdigests erneut geprüft. Release-ID: **425978946**. Keine früheren Pakete ersetzt; der zurückgenommene 1.0.1-Entwurf bleibt erhalten.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-1.0.2-Build41-source.zip | 3519382 | 591324E1AB879491BAB76D13FE6954B7AC6372CE5BB5A691609CD82049B50915 |
| CutAssistantNext-1.0.2-Build41-win-x64-Setup.exe | 20453526 | 0A00B4040B985532CB8A08D0181ADFB0B8AF71F6DC488176678DD880D5645A0C |
| CutAssistantNext-1.0.2-Build41-win-x64.zip | 25170942 | 48696B86715F8167B49DFA9DBE1864BBD3334AD3F0BB4A46FDCFF734E2D20E1A |
| PUBLISH-FILES.sha256 | 32911 | 1A28C106D357D03EB4FBA3767ECFAC4F5181AAF0BCE3DC9CE233A6EA2FF78798 |
| SHA256SUMS.txt | 647 | 38D0D581FC50FEAD6D21A66CF05315BA7D31FF37BF6A9A64E00E70E4B802E8EE |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |

Die Quellenarchive gehören exakt zu den persönlich/technisch geprüften Binärpaketen. Dokumentation im Archiv behält den Erstellungsstand vor Freigabe; aktuelle Repository-Unterlagen und Releasebeschreibung ergänzen spätere Rückmeldungen. Keine nachträgliche Veränderung der geprüften Pakete.

Lokale Nachweise: **.build/publication-1.0.2-build41-20261027/** (COMMIT-PROOF, LOCAL-PUBLICATION-PROOF, REMOTE-PROOF und PUBLISHED.json) sowie **.build/release-1.0.2-build41-20261027/**. [Paketbericht](RELEASE-1.0.2-BUILD41-RESULT.md), [Anleitung](NUTZERANLEITUNG.md), [Klebecenter](KLEBECENTER-20261027.md).
