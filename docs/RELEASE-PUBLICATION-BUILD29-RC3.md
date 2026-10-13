# Veröffentlichung – CAN 0.3.0 · Build 29 · RC3

Auf ausdrücklichen Nutzerauftrag „also dann veröffentlichen :-)“ am 13.10.2026 als [öffentliche Vorabversion](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build29-rc3) freigegeben. Kein Entwurf mehr; nicht als stabile „Latest“-Ausgabe markiert.

## Fester Stand

- Annotierter Tag **v0.3.0-build29-rc3** zeigt auf den tatsächlich gebauten CAN-Quellcommit **ce4bc15b118d8080a78d7db5396db3ccf830ca89**. Remote-Tag vor und nach Veröffentlichung geprüft. Spätere Dokumentationscommits verändern diesen Quellpin nicht.
- **0.3.0 · Build 29 · RC3**, App-Dateiversion **0.3.0.29**, unveränderter **otr-can 0.1.5** aus Motorquellcommit **2a474629836d52023bf37cbbdb309ccd1c23acf3**.
- Genau der eingefrorene und persönlich abgenommene Paketsatz hochgeladen; keine neuen Binärdateien, Pakete, Motoränderungen oder Buildnummer erzeugt.
- [Paketprüfbericht](RELEASE-BUILD29-RC3-RESULT.md): Release/Publish/Setup ohne Warnungen oder Fehler, **1076/1076 CAN-Tests**, **249 Portable-Dateien** und **509 Quellen-ZIP-Dateien** geprüft. Letzter unveränderter nativer Quellenprüfstand **44/44 Rust-Tests**, kein neuer Rust-Build.

## Öffentliche Downloads

Alle sieben Dateien vor Freigabe im Entwurf und danach im öffentlichen Release über GitHub REST geprüft: genau eine Datei je Name, Status uploaded, richtige Größe und SHA-256-Digest. Release-Text entspricht den lokalen Freigabehinweisen. Die sechs Archive und die Prüfsummendatei sind lokal weiterhin bytegleich und schreibgeschützt.

| Download | Bytes | SHA-256 |
|---|---:|---|
| [CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build29-rc3/CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip) | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| [CAN-libmpv-v0.41.0-relink-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build29-rc3/CAN-libmpv-v0.41.0-relink-2026-09-26.zip) | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| [CutAssistantNext-0.3.0-Build29-source-RC3.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build29-rc3/CutAssistantNext-0.3.0-Build29-source-RC3.zip) | 2990238 | F719C821F25B10C5E875AFB82AC3AF1C8C3DBE782BE5314C976E1AB36DF9FE30 |
| [CutAssistantNext-0.3.0-Build29-win-x64-RC3.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build29-rc3/CutAssistantNext-0.3.0-Build29-win-x64-RC3.zip) | 24941763 | E98772971BA95983F75846023D9DE70AA2D72CB09DF591A85F26BB833CF4EBD2 |
| [CutAssistantNext-0.3.0-Build29-win-x64-Setup-RC3.exe](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build29-rc3/CutAssistantNext-0.3.0-Build29-win-x64-Setup-RC3.exe) | 20270346 | A63F194772283735EAC7DBE0757430E9528316526DD6E4C7FBDAF99DB9D54FBE |
| [otr-can-0.1.5-source.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build29-rc3/otr-can-0.1.5-source.zip) | 63988 | 3A973278ABDF719B745405D9DFBB930C14A28CFEF40C7FB52A35CA17F7214E57 |
| [SHA256SUMS.txt](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build29-rc3/SHA256SUMS.txt) | 659 | B6184C3AB414A538D605E609BAAD86A2150A319F64D11852294A51E0B5CD863B |

Veröffentlicht: **13.10.2026 20:56:20 Europe/Berlin**; UTC **2026-10-13T18:56:20Z**; GitHub-Release-ID **412002070**.

## Abnahme und erhaltene Grenzen

Produktivinstallation und reale Torchwood-Schnitte persönlich bestätigt. In der VM Setup-Installation, Greys Anatomy / Durchs Feuer (MP4) mit MP4Box und otr-can sowie Deinstallation erfolgreich; Portable-Appstart, Pfade und Teufelsbrigade (AVI) mit beiden Motoren ebenfalls erfolgreich. AVI-Lupe, Richtungswechsel-Halbsprung, FFmpeg-Versionsprüfung und Maskenwechsel ohne Neustart zuvor persönlich bestätigt.

Bekannte AVI-Zeitstempelauffälligkeiten, der synthetische Zwei-AAC-MP4Box-Befund sowie die konkreten Chicago-Fire-Blitzer und Torchwood-Doppelszenen bei MP4Box bleiben dokumentiert. Keine zusätzliche getrennte Tonspur-/Synchronitätsmessung oder Prüfung eines frischen Portable-Profils aus der VM-Rückmeldung abgeleitet. Externe Laufzeiten/Werkzeuge bleiben erforderlich; AVI bleibt experimentell. [Details und Prüfgrenzen](RELEASE-BUILD29-RC3-RESULT.md).

Die bisherige öffentliche **0.2.1 · Build 16 · RC3**-Ausgabe bleibt unverändert: Release-ID, Tag, Zielcommit, Text, Veröffentlichungszeit und sämtliche Asset-IDs, Namen, Größen und Digests vor/nach dieser Veröffentlichung verglichen. Alle acht lokalen Lockdateien per Hash unverändert; die vier vorbestehenden Produktions-Lockänderungen bleiben außerhalb von Commit, Quellarchiv und Veröffentlichung. Lokaler Buildzähler weiterhin **29**.

Lokale Prüfbelege unter .build/publication-build29-20261013: erwartete Uploads, REST-/Prüfnachweise vor/nach Freigabe, Tagprüfung und Erhaltungsnachweise. Dokumentation separat auf origin/main gesichert.
