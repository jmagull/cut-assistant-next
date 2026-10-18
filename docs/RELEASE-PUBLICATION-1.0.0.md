# Veröffentlichung – CAN 1.0.0 · Build 32

Auf ausdrücklichen Nutzerauftrag am **18.10.2026** als [erste stabile Version und aktuelle Latest-Ausgabe](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.0) veröffentlicht. **Kein Entwurf und keine Vorabversion.** Die persönliche Setup-/Deinstallations-/Portable-Abnahme lag vor Veröffentlichung vor.

## Fester Quellstand und Abnahme

Annotierter Tag **v1.0.0** zeigt nachweislich auf den tatsächlich gebauten CAN-Quellcommit **3e2ac03dfe4a701ecbc95c80cc06ff2527309b9f**. Dateiversion **1.0.0.32**, Anzeige **Cut Assistant Next · 1.0.0 · Build 32**, kein RC. Spätere Abnahmen und Ergebnisdokumentation separat auf `main` gesichert; Quellpin und veröffentlichte Dateien unverändert.

Motor **otr-can 0.1.5** unverändert, Motorquellcommit **2a474629836d52023bf37cbbdb309ccd1c23acf3**, geprüfte Motor-EXE und native Bibliotheken erhalten. Kein neuer Build oder Neuverpacken im Veröffentlichungsschritt. **1347/1347 Pakettests**, keine übersprungen, Release **0 Warnungen/0 Fehler**, **250 Portable-Dateien**, **558 Quellen-ZIP-Dateien** und **13 native WPF/libmpv-Fallgruppen** gegen die endgültigen Bits geprüft.

Persönlich bestätigt: Produktiv-Setup, Kommandozeilenstart, Drag & Drop, beide Ansichten und Wechsel, Frame-Lupe/Frame-Liste, Halbierungssuche, Schutzdialog und reale Filmschnitte. Neues Setup und Deinstallation in der VM einschließlich dokumentierter AVI-/MP4-Prüfungen; Portable in der VM mit anderem otr-can-Pfad, MP4-Schnitten mit beiden Motoren samt Bild/Ton und Halbierungssprung sowie AVI-Schnitten mit beiden Motoren bestätigt. Der genaue Umfang ist im [Paketbericht](RELEASE-1.0.0-BUILD32-RESULT.md) getrennt dokumentiert; keine pauschale Qualitätsfreigabe sämtlicher Medien oder jeder Tonspur daraus abgeleitet.

Bekannte Grenzen stehen ausdrücklich in den veröffentlichten Release Notes: AVI experimentell mit Schutzsperren und möglichen Vorbereitungswarnungen, MP4Box-Präzisionsgrenzen samt konkreten Blitzerfällen, unterstützte otr-can-Bildzeiten und vollständige Audioabdeckung aller Behaltebereiche, externe Laufzeiten/Werkzeuge sowie weitere Codec-/Audio-/Decodergrenzen. Der Doc-Zwischenstand mit unfertiger Cutlist ist nach Fertigstellung mit beiden Motoren erfolgreich geprüft. Keine neue bitidentische Reproduzierbarkeit der unveränderten Motor-EXE behauptet.

## Öffentlich geprüfte Downloads

Alle **sieben Dateien vor Freigabe im Entwurf und nach Veröffentlichung** per GitHub REST geprüft: genau ein Asset je erwartetem Namen, Status **uploaded**, exakte Bytezahl und **SHA-256-Digest** identisch zum unveränderten lokalen schreibgeschützten Archiv. Release-Titel/Text entsprechen den vorbereiteten Hinweisen. Quelltag und stabiler **Latest-Status** unabhängig bestätigt.

| Download | Bytes | SHA-256 |
|---|---:|---|
| [CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.0/CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip) | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| [CAN-libmpv-v0.41.0-relink-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.0/CAN-libmpv-v0.41.0-relink-2026-09-26.zip) | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| [CutAssistantNext-1.0.0-Build32-source.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.0/CutAssistantNext-1.0.0-Build32-source.zip) | 3160830 | 8EF486F084A37C626E8A5F277D9F0510F84A7E4E912BA6C79DA30AEA139DBF92 |
| [CutAssistantNext-1.0.0-Build32-win-x64-Setup.exe](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.0/CutAssistantNext-1.0.0-Build32-win-x64-Setup.exe) | 20325270 | 015E9C814D0D747F7B55269F955DEC1977757086644B040E4D3A50CBBF1759AB |
| [CutAssistantNext-1.0.0-Build32-win-x64.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.0/CutAssistantNext-1.0.0-Build32-win-x64.zip) | 25005369 | 92229B0AFF563FD0D4D6203F84062BFE90B39C060301FC706BDFED14372DA60A |
| [otr-can-0.1.5-source.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.0/otr-can-0.1.5-source.zip) | 63988 | 3A973278ABDF719B745405D9DFBB930C14A28CFEF40C7FB52A35CA17F7214E57 |
| [SHA256SUMS.txt](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.0/SHA256SUMS.txt) | 647 | D5DB5970657B3DF582DF3EF34288980383DEDDE7FE0F13BD37C38C61F75B1777 |

`SHA256SUMS.txt` enthält die sechs Archive; seine eigene Prüfsumme steht in der Tabelle und im Ausführungsnachweis. Die ausdrücklich beigefügte CAN-Quellen-ZIP enthält den tatsächlich gebauten Snapshot samt Herkunft und Manifest; GitHubs automatisch erzeugte Quellarchive ersetzen diese Herkunftsdateien nicht.

Veröffentlicht: **18.10.2026 10:31:27 Europe/Berlin**, UTC **2026-10-18T08:31:27Z**. GitHub-Release-ID **416326970**.

## Erhaltung und Git-Sicherung

**Acht bestehende GitHub-Releases** vor/nach Freigabe vollständig verglichen: Metadaten, Texte, Asset-IDs, Namen, Größen und Digests unverändert. Bestehende Remote-Tags erhalten; ausschließlich **v1.0.0** hinzugefügt. **70 ältere lokale Release-Dateien** bytegleich erhalten. Sieben neue Dateien weiterhin schreibgeschützt und identisch zum persönlich abgenommenen Paketsatz.

Alle **acht lokalen Lockdateien** unverändert; vier vorbestehende Produktionsänderungen nicht committet. Buildzähler weiterhin **32**. Keine Videos, Buildausgaben oder lokalen Einstellungen eingecheckt. Aktuelle Abnahmen und Veröffentlichungshinweise separat committet und gepusht; stabiler Quelltag ausdrücklich am gebauten Commit.

## Lokale Nachweise

`.build/publication-1.0.0-20261018`: `BEFORE.json`, `RELEASES-BEFORE.json`, `TAGS-BEFORE.txt`, `OLD-LOCAL-ARTIFACTS.json`, `UPLOAD-EXPECTED.json`, `RELEASE-NOTES.md`, `GIT-PREPUBLICATION.json`, `TAG-MESSAGE.txt`, `UPLOAD.log`, `RELEASE-DRAFT.json`, `VERIFIED-DRAFT.json`, `PUBLISH.log`, `RELEASE-PUBLIC.json`, `LATEST.json`, `VERIFIED-PUBLIC.json`, vorher/nachher verglichene Release-/Tag-Listen und abschließender Git-Nachweis. Die Tag-Lese-API lieferte den unveröffentlichten Entwurf zunächst nicht; der bereits vorhandene Entwurf wurde über seine eindeutige Release-ID geprüft. Kein doppelter Release und keine ungeprüfte Veröffentlichung.

[Release Notes](RELEASE-NOTES-1.0.0.md), [Benutzeranleitung](NUTZERANLEITUNG.md), [Freeze-Umfang](FREEZE-1.0.0-20261016.md).
