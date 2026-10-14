# Veröffentlichung – CAN 0.3.0 · Build 30 · RC3

Auf ausdrücklichen Nutzerauftrag am 14.10.2026 eingefroren, gebaut, committet, gepusht und als [öffentliche Vorabversion](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build30-rc3) freigegeben. Kein Entwurf mehr; nicht als stabile Latest-Ausgabe markiert.

## Fester Quellstand und Prüfungen

Annotierter Tag **v0.3.0-build30-rc3** zeigt auf **57878bd90b3e044218f8f157b6922552a170f1d4**. Gebaute App **0.3.0 · Build 30 · RC3**, Dateiversion **0.3.0.30**. Unveränderter Motor **otr-can 0.1.5**, Motorquellcommit **2a474629836d52023bf37cbbdb309ccd1c23acf3**. Spätere Dokumentationscommits ändern diesen Quellpin und die Pakete nicht.

Release-Build **0 Warnungen/0 Fehler**, **1215/1215 CAN-Tests**, keine übersprungen. **249 Portable-Dateien**, **527 Quellen-ZIP-Dateien**, **18 native Integrationsprüfungen** gegen das entpackte ZIP und ergänzende kompilierte K-Anzeige-/Legendenprüfung erfolgreich. Setup mit Inno Setup 7 gebaut. [Paketprüfbericht und Prüfgrenzen](RELEASE-BUILD30-RC3-RESULT.md).

Enthält die vier Verbesserungen aus den Shoutbox-Rückmeldungen: sichtbare Frame-Lupe, reine Filmnamen bei leeren Serienfeldern, Untersuchungsmodus ohne Kante und MP4-Keyframes im Hauptfenster. AVI von Hauptfenster-Keyframes ausgenommen; Hinweis und AVI-Lupe erhalten. K direkt hinter der aktuellen Framenummer, Status **„Blaue Bereiche: Keyframes“**.

## Öffentliche Downloads

Alle sieben Uploads im Entwurf und nach Freigabe über GitHub REST geprüft: eindeutige Namen, Status uploaded, exakte Bytezahl und SHA-256-Digest. Release-Text stimmt mit lokalen Freigabehinweisen überein; sechs Archive und Prüfsummendatei lokal unverändert und schreibgeschützt.

| Download | Bytes | SHA-256 |
|---|---:|---|
| [CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build30-rc3/CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip) | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| [CAN-libmpv-v0.41.0-relink-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build30-rc3/CAN-libmpv-v0.41.0-relink-2026-09-26.zip) | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| [CutAssistantNext-0.3.0-Build30-source-RC3.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build30-rc3/CutAssistantNext-0.3.0-Build30-source-RC3.zip) | 3053572 | 3C9A0CF3E0B05D6F71B65B06E9ADB5F8C0106196F9ACDADD927266D7D9F89C1A |
| [CutAssistantNext-0.3.0-Build30-win-x64-RC3.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build30-rc3/CutAssistantNext-0.3.0-Build30-win-x64-RC3.zip) | 24959505 | B3F7E40272FA8A95925CA980FAF6EF00C7D009C88746BCDB11F66C87C4D704CF |
| [CutAssistantNext-0.3.0-Build30-win-x64-Setup-RC3.exe](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build30-rc3/CutAssistantNext-0.3.0-Build30-win-x64-Setup-RC3.exe) | 20277651 | 2E929BE1B1CFFE4E96997F3A4CAC3C272971D7B42C7613CC9A144F371ECE689C |
| [otr-can-0.1.5-source.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build30-rc3/otr-can-0.1.5-source.zip) | 63988 | 3A973278ABDF719B745405D9DFBB930C14A28CFEF40C7FB52A35CA17F7214E57 |
| [SHA256SUMS.txt](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build30-rc3/SHA256SUMS.txt) | 659 | 7F5A528DF49C32D30761A664463A6747DD0DA649B46F244F9FB1B7D477BCDDEF |

Veröffentlicht: **14.10.2026 22:57:30 Europe/Berlin**, UTC **2026-10-14T20:57:30Z**. GitHub-Release-ID **413175316**.

## Erhaltung und verbleibende Grenzen

Build 29 und Build 16: Release-ID, Tagname, Zielcommit, Text, Status, Zeitangaben und sämtliche Asset-IDs/Namen/Größen/Digests vor/nach Freigabe verglichen – unverändert. Build-29-Remote-Tag weiterhin Quellcommit **ce4bc15b118d8080a78d7db5396db3ccf830ca89**; lokale eingefrorene Build-29-Archive weiterhin bytegleich. Alle acht lokalen Lockdateien unverändert. Die vier vorbestehenden Produktions-Lockänderungen sind nicht committet und nicht im Quell-Snapshot enthalten. Buildzähler nun 30.

Die persönliche Rückmeldung gibt den Gesamtstand zum Veröffentlichen frei. Keine frische Build-30-Setup-/VM-/Deinstallationsprüfung oder neue reale Schnitt-/Tonspurabnahme daraus abgeleitet. AVI-Zeitstempelgrenzen und dokumentierter MP4Box-Zwei-AAC-Befund bleiben bestehen; externe Laufzeiten und Werkzeuge erforderlich. AVI bleibt experimentell.

Nachweise unter `.build/freeze-build30-20261014`; Dokumentation separat auf `origin/main`. [Übergabetext für den nächsten Chat](UEBERGABE-BUILD30-RC3-20261014.txt).
