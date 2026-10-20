# Veröffentlichung – CAN 1.0.1 · Build 34

Auf ausdrücklichen Nutzerauftrag am **20.10.2026** als [stabile Version und aktuelle Latest-Ausgabe](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.1) veröffentlicht. **Kein Entwurf und keine Vorabversion.** Sämtliche vorgesehenen persönlichen Pakettests waren vor Veröffentlichung positiv bestätigt.

## Quellstand, Pakete und Abnahmen

Annotierter Tag **v1.0.1** zeigt auf den tatsächlich gebauten CAN-Quellcommit **`a6b7e52bddb72a2517631df2a8c5b8fb68c06ea5`**. Alle **593 Repositorydateien** wurden gegen den Build-Snapshot unter regulärer Git-Zeilenendennormalisierung geprüft. Die ausdrücklich beigefügte CAN-Quellen-ZIP enthält unverändert die exakten Buildbytes einschließlich Snapshot-Metadaten und Manifest. Spätere persönliche Abnahmen und Veröffentlichungshinweise sind getrennte Folgecommits auf `main`. GitHubs automatisch erzeugtes Quellenarchiv ersetzt nicht den beigefügten nachvollziehbaren Build-Snapshot.

Motorquellcommit **`bbc3a5104b75e7bcfc4f257ea5b14e9a1961f6db`** entspricht den ausgelieferten 0.1.6-Quellen. Regulärer Motor und isolierter Korrekturversuch wurden im privaten Motor-Repository committet und gepusht. Spätere README-Abnahmen sind getrennt vom gebauten Quellcommit. Die persönlich geprüfte Motor-EXE bleibt bytegleich: **`73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E`**. Keine neue binäre Reproduzierbarkeit behauptet; der separate Quellneubau und seine abweichende EXE-Prüfsumme sind im Motor-Herkunftsnachweis dokumentiert.

**Kein Neubau oder Neuverpacken im Veröffentlichungsschritt.** Anzeige **Cut Assistant Next · 1.0.1 · Build 34**, Dateiversion **1.0.1.34**, Motor **otr-can 0.1.6**. **1385/1385 CAN-Tests**, zusätzlich **890/890 App-Tests mit finalen Paket-DLLs**, **53/53 Rust-Quelltests**, **11 Motor-Paketprüfungen**, **250 Portable-Dateien**, **595 CAN- und 27 Motorquellen-ZIP-Einträge**, echte Schnitt- und native Kontrollplayer-Prüfungen bestanden. Release-Build **0 Warnungen/0 Fehler**. Keine Produktcodeänderung seit diesem geprüften Build.

Persönlich bestätigt: normales Setup auf dem Arbeitsrechner (Vera/MP4Box, Musikladen/otr-can ohne NDR-Blitzer), VM-Setup mit MP4Box und otr-can (Grey’s Anatomy und Teufelsbrigade-AVI), VM-Deinstallation und Portable in der VM mit ausdrücklich umgestelltem otr-can-Pfad und beiden Motoren. Bild, Ton, Schnitte und 3-Sekunden-Vorschau bestätigt. Eine separate Upgradeprüfung wird nicht zusätzlich behauptet. [Detaillierter Abnahmestand](RELEASE-1.0.1-BUILD34-RESULT.md).

Bekannte Grenzen sind in den öffentlichen Release Notes genannt: MP4Box-Blitzer bei Musikladen und Radical bleiben bestehen; AVI ist experimentell und kann bekannte Vorbereitungswarnungen erzeugen. Die AAC-Korrektur ist eng begrenzt und keine allgemeine Reparatur beschädigter Quellen. Externe Werkzeuge und Laufzeiten bleiben separat erforderlich.

## Verifizierte öffentliche Downloads

Alle **sieben Dateien vor Freigabe im Entwurf und nach Veröffentlichung** über die GitHub REST API geprüft: eindeutiger erwarteter Name, Status **uploaded**, exakte Bytezahl und **SHA-256-Digest** stimmen mit dem unveränderten lokalen schreibgeschützten Paketsatz überein. Titel und Release-Text unter normalisierten Zeilenenden identisch zur vorbereiteten Fassung. Quelltag und stabiler Latest-Status unabhängig geprüft.

| Download | Bytes | SHA-256 |
|---|---:|---|
| [CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.1/CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip) | 405926830 | `01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B` |
| [CAN-libmpv-v0.41.0-relink-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.1/CAN-libmpv-v0.41.0-relink-2026-09-26.zip) | 47273515 | `00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342` |
| [CutAssistantNext-1.0.1-Build34-source.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.1/CutAssistantNext-1.0.1-Build34-source.zip) | 3284055 | `E1B068CEAE5492B6DE4E0E9F991D56DED95698A5355E6AC2C5A0519C3BEC079F` |
| [CutAssistantNext-1.0.1-Build34-win-x64-Setup.exe](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.1/CutAssistantNext-1.0.1-Build34-win-x64-Setup.exe) | 20356048 | `9C6A8793ED2C31104D0305D1857FA66EB7B6D900C940E4A545F29CC30C17F961` |
| [CutAssistantNext-1.0.1-Build34-win-x64.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.1/CutAssistantNext-1.0.1-Build34-win-x64.zip) | 25062619 | `FBE8EAF5E4993F384194DDB6811169504FE5299B77AEB6E36F11A4D23DC492D4` |
| [otr-can-0.1.6-source.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.1/otr-can-0.1.6-source.zip) | 71900 | `B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F` |
| [SHA256SUMS.txt](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.1/SHA256SUMS.txt) | 647 | `934E1DDBCEB5BA78B496C5FECBFE609D0617A28149BFF9959F828D37282226FE` |

`SHA256SUMS.txt` enthält die sechs Nutzdateien; seine eigene Prüfsumme steht oben und im Ausführungsnachweis. Die Paket-Release-Notes dokumentieren den eingefrorenen Paketstand vom 19.10.2026; der aktuelle öffentliche Release-Text und die aktuelle Dokumentation ergänzen die nachfolgende vollständige Abnahme und Veröffentlichung. Keine Paketbytes nachträglich geändert.

Veröffentlicht: **20.10.2026 20:24:21 Europe/Berlin**, UTC **2026-10-20T18:24:21Z**. GitHub-Release-ID **418710813**.

## Erhaltung und Git-Sicherung

**Neun bereits bestehende GitHub-Releases** vor/nach Veröffentlichung verglichen: IDs, Tags, Quellziele, Titel, Texte, Veröffentlichungsstatus sowie Asset-IDs, Namen, Größen, Status und Digests unverändert. Bestehende Remote-Tags erhalten; ausschließlich **v1.0.1** hinzugefügt. Alle **120 erfassten lokalen Release-Dateien** einschließlich früherer Versionen bleiben unverändert; die endgültige Abschlussprüfung kontrolliert dies erneut.

Alle acht ursprünglichen lokalen Lockdateien bleiben erhalten. Vier vorbestehende Produktionsänderungen wurden ausdrücklich nicht committet. Buildzähler bleibt **34**. Keine Videodateien, Buildausgaben, Zugangsdaten oder persönlichen Einstellungen eingecheckt. Beide Repository-Branches wurden ohne Force-Push gesichert. Der Release-Tag bleibt am gebauten Quellcommit; aktuelle Dokumentation folgt separat auf `main`.

## Lokale Nachweise

`.build/publication-1.0.1-20261020`: Ausgangsstände, ursprüngliche Release-/Tag-Listen, lokale Artefaktprüfsummen, Quellcommit-Zuordnung, Commit-/Pushlogs, Tagtext, Upload-Erwartungen, ursprünglicher und fortgesetzter Upload, eindeutige Entwurfs-ID, verifizierter Entwurf, Veröffentlichungsantwort, verifizierte öffentliche Release-Daten und Latest-Nachweis. Die Tag-Lese-API zeigte den Entwurf zunächst nicht; er wurde über die eindeutig gefundene Release-ID geprüft. Kein zweiter Entwurf angelegt. Ein unterbrochener Upload wurde anhand bereits verifizierter Assets ohne Duplikate fortgesetzt. Textvergleich mit explizitem UTF-8 und normalen CRLF-/LF-Zeilenenden; keine inhaltliche Abweichung übergangen.

[Release Notes](RELEASE-NOTES-1.0.1.md), [Benutzeranleitung](NUTZERANLEITUNG.md), [Paketbericht und persönliche Abnahmen](RELEASE-1.0.1-BUILD34-RESULT.md).
