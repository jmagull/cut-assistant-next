# Veröffentlichung – CAN 0.3.0 · Build 31 · RC3

Auf ausdrücklichen Nutzerauftrag am **15.10.2026** unverändert als [öffentliche Vorabversion](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build31-rc3) veröffentlicht. Kein Entwurf mehr und keine stabile Latest-Ausgabe.

## Fester Quellstand und Abnahme

Annotierter Tag **v0.3.0-build31-rc3** zeigt auf **de8ec0d598b095d85e4559e7fd83012534959637**. Gebaute Dateiversion **0.3.0.31**, Kennung **RC3**. Motor **otr-can 0.1.5**, Motorquellcommit **2a474629836d52023bf37cbbdb309ccd1c23acf3**, unveränderte geprüfte Motor-EXE und native Bibliotheken.

Release-Build ohne Warnungen/Fehler, **1308/1308 Tests**, keine übersprungen. **249 Portable-Dateien** und **539 Quellen-ZIP-Dateien** vollständig gegen Manifeste geprüft; vier native UI-Wechselfälle, sechs reale Indexvergleiche und drei Schnittdienst-/FFMS2-Fälle gegen entpackten Paketcode erfolgreich. [Paketnachweis und Grenzen](RELEASE-BUILD31-RC3-RESULT.md).

Vor Veröffentlichung persönlich vom Nutzer bestätigt: **Setup in der VM, Deinstallation, Portable-ZIP in der VM sowie Videoschnitt mit MP4Box und otr-can**. AVI-Ausblendung/MP4-Wiedereinblendung und die gezielten Vorprüfungs-Sperren ebenfalls bestätigt. Kein pauschaler Qualitätsnachweis jeder Aufnahme/Tonspur aus diesen Rückmeldungen abgeleitet.

Neu: Hauptplayer-Halbierungssuche, für erkannte AVI ausgeblendete Suchzeile und gesperrte Frame-Lupe, sowie Monotonie-/Nullstart-/CFR-Vorprüfung vor otr-can. MP4Box und Motor bleiben unverändert. AVI experimentell; keine Timingreparatur oder automatische Bildratenumrechnung. Externe Laufzeiten/Werkzeuge und bekannte weitere Codec-/Audio-/MP4Box-Grenzen bleiben bestehen.

## Geprüfte öffentliche Downloads

Alle sieben Uploads **vor Freigabe im Entwurf und nach Veröffentlichung** über GitHub REST geprüft: genau ein Asset je Name, Status uploaded, exakte Bytezahl und SHA-256-Digest. Release-Text identisch mit den vorbereiteten Freigabehinweisen. Quellpin und unveränderte schreibgeschützte lokale Archive geprüft.

| Download | Bytes | SHA-256 |
|---|---:|---|
| [CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build31-rc3/CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip) | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| [CAN-libmpv-v0.41.0-relink-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build31-rc3/CAN-libmpv-v0.41.0-relink-2026-09-26.zip) | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| [CutAssistantNext-0.3.0-Build31-source-RC3.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build31-rc3/CutAssistantNext-0.3.0-Build31-source-RC3.zip) | 3104947 | 74F2E40A1E807220774CE0C0047D7AA20FCEB509605B898828648C163DDF0242 |
| [CutAssistantNext-0.3.0-Build31-win-x64-RC3.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build31-rc3/CutAssistantNext-0.3.0-Build31-win-x64-RC3.zip) | 24973680 | 9CCD38087D3C19A4A5111A2261C41A028E4F7DD880AA01DEA59FA72A2858A248 |
| [CutAssistantNext-0.3.0-Build31-win-x64-Setup-RC3.exe](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build31-rc3/CutAssistantNext-0.3.0-Build31-win-x64-Setup-RC3.exe) | 20290615 | 3625AE24BEDAD8A35A130783A2A6C94467970E2E6C0196A4AB161FC88747FCCC |
| [otr-can-0.1.5-source.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build31-rc3/otr-can-0.1.5-source.zip) | 63988 | 3A973278ABDF719B745405D9DFBB930C14A28CFEF40C7FB52A35CA17F7214E57 |
| [SHA256SUMS.txt](https://github.com/jmagull/cut-assistant-next/releases/download/v0.3.0-build31-rc3/SHA256SUMS.txt) | 659 | 37BBEAD5C669A5407738706273A30BB51394B7B19960312181C26D42D31EE333 |

Veröffentlicht: **15.10.2026 21:30:46 Europe/Berlin**, UTC **2026-10-15T19:30:46Z**. GitHub-Release-ID **414191680**.

## Erhaltung und Dokumentation

Öffentliche Build-30-, Build-29- und Build-16-Releases vor/nach Freigabe verglichen: Release-Metadaten, Inhalte und sämtliche Asset-IDs/Namen/Größen/Digests unverändert. Lokale Build-30-Archive weiterhin bytegleich; alle acht lokalen Lockdateien unverändert, vier vorbestehende Produktionsänderungen nicht committet. Buildzähler bleibt **31**. Kein neuer Build, Versionswechsel, Motorbuild oder Neuverpacken im Veröffentlichungsschritt.

Nachweise unter `.build/freeze-build31-20261015`: `RELEASE31-DRAFT.json`, `RELEASE31-PUBLIC.json`, `UPLOAD-EXPECTED.json`, `VERIFIED-DRAFT.json`, `VERIFIED-PUBLIC.json`, `RELEASE-NOTES.md` und die vorher/nachher verglichenen Alt-Releases. Die Repository-Dokumentation wird separat gesichert; der veröffentlichte Quellpin und die Pakete bleiben unverändert. [Übergabe](UEBERGABE-BUILD31-RC3-20261015.txt).