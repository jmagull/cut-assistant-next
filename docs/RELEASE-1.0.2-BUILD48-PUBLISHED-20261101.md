# CAN 1.0.2 Build 48 – Veröffentlichung

Am **01.11.2026** nach ausdrücklicher Freigabe als stabile **[v1.0.2-build48 / Latest](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2-build48)** veröffentlicht. GitHub-Zeit **2026-11-01T19:37:40Z** (20:37:40 Uhr Europe/Berlin), Release-ID **430808774**. Kein Entwurf, keine Vorabversion.

## Commit, Tag und Push

- Endgültiger Release-Quellcommit **5afa5ff099d3b5e2f0586ce5899aafde6feabf58** ([Commit](https://github.com/jmagull/cut-assistant-next/commit/5afa5ff099d3b5e2f0586ce5899aafde6feabf58)).
- Annotierter Tag **v1.0.2-build48** zeigt exakt auf diesen tatsächlich gebauten Commit; Remote-Referenz und annotiertes Tagobjekt vor Freischaltung über GitHub-API geprüft.
- main erfolgreich von 42d72fc4074acf20e60dc10aea50865519e6ff08 auf den Release-Commit gepusht; neuer Tag erfolgreich gepusht. Keine Force-Pushes und keine Änderung bestehender Tags.
- Nach der Veröffentlichung folgt ausschließlich ein Dokumentationscommit auf main. Der Release-Tag und die bereits hochgeladenen Pakete bleiben beim gebauten Release-Commit.
- CAN **1.0.2 Build 48**, App-Dateiversion **1.0.2.48**, unverändert otr-can **0.1.6**, libmpv **0.41.0**, .NET 10/WPF, Windows 11 x64.

## Qualitätssicherung

**1634/1634 Pakettests** (Core 200, Media 390, App 972, Cutlists 72), zusätzlich **1362/1362 Tests gegen finale Produktbibliotheken** (App 972, Media 390), jeweils 0 Fehler/übersprungen. **32/32 native/WPF-Kontrollen in sechs separaten Prozessen**, alle Exitcode 0. Vollständiger nummerierter Build und Snapshot-/Paketbuild jeweils ohne Warnungen/Fehler. **250 Portable-Dateien**, **716 manifestierte Quelldateien / 717 Quellen-ZIP-Dateien** und fünf ZIP-CRCs vollständig geprüft. Setup enthält laut geprüftem Compilerprotokoll exakt die 250 Publish-Dateien; native DLLs, Lizenzen, Motorfähigkeiten und Versionsressourcen bestätigt. [Vollständiger Paketbericht einschließlich der aufgeklärten Vorbereitungsfehler](RELEASE-1.0.2-BUILD48-RESULT.md).

Persönliche Praxisabnahme der korrigierten Entwicklungsfassung mit Ghosts – Weihnachtsgeister Teil 1 und Teil 2 durch den Nutzer bestätigt: ursprünglicher EOF-Absturzfall, regulärer Tonspurwechsel und Navigation einwandfrei. Keine daraus abgeleitete neue Setup-/Portable-/VM-Abnahme.

## Upload und Freischaltung

Alle acht Assets zunächst in einen Entwurf hochgeladen. Vor Freischaltung wurden bei jedem Asset Status uploaded, Dateiname, Byteanzahl und GitHub-SHA-256-Digest gegen den lokalen geprüften Paketsatz verglichen. Erst danach Freischaltung mit draft=false, prerelease=false und make_latest=true. Anschließend veröffentlichte Assets erneut identisch geprüft und Latest-Zuordnung über releases/latest bestätigt.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-1.0.2-Build48-source.zip | 3726259 | 902A4C9A0287A05A824DB4119157B1B6A883CA7E86678FF10F11C1AB980C4A17 |
| CutAssistantNext-1.0.2-Build48-win-x64-Setup.exe | 20518042 | 0FB98F8F2555413669ABCA9233A240CDA2BA4D340286D695B3E8B799486D3B9C |
| CutAssistantNext-1.0.2-Build48-win-x64.zip | 24555623 | B6C6C2D23DF7B48CDAD2FC1CD3197D3C1B6F82E54B9EFBD0311ED44C07B254E2 |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| PUBLISH-FILES.sha256 | 32911 | 6CA21506C4C285A61A24FD52D3C1E602473FC9ACA812F4D4DADC606C6E466252 |
| SHA256SUMS.txt | 647 | B2F82062AF061E737927F180C8E52D92119493A108F494B362AD9DFF9E3D3D5E |

## Vorgänger und abschließender Arbeitsstand

**Build 47 bleibt unverändert verfügbar:** [v1.0.2-build47](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2-build47), Release-ID 430061255, alter Tag unverändert. Vor-/Nachvergleich bestätigt identische Veröffentlichung, Beschreibung, Titel, Tagzuordnung und alle acht Assets einschließlich Asset-IDs, Größen und Digests. Alte lokale Dateien stimmen mit ihren vorhandenen Prüfsummen überein.

Die vier schon vor Entwicklung vorhandenen Änderungen an src/CutAssistantNext.{App,Core,Cutlists,Media}/packages.lock.json sind weder committet noch im Quellenarchiv übernommen und bleiben lokal erhalten. Nach dem reinen Dokumentationsnachtrag sind sie die einzigen verbleibenden Arbeitsbaumänderungen. Buildzähler **48**, Version.props unverändert. Keine Diagnosemedien, Logs, Buildausgaben oder lokalen Einstellungen committed.

Lokale Nachweise: **.build/release-1.0.2-build48-20261101/** mit RELEASE-COMMIT.txt, TAG-REF.json, TAG-OBJECT.json, PACKAGE-PROOF.json, QA-SUMMARY.json, DRAFT.json, REMOTE-DRAFT-PROOF.json, PUBLISHED.json, LATEST.json, PREVIOUS-RELEASE-AFTER.json und REMOTE-PROOF.json. Der finale main-Commit und Git-Status werden dort in FINAL.json festgehalten.

[Release und Downloads](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2-build48) · [Paketprüfung](RELEASE-1.0.2-BUILD48-RESULT.md) · [Korrekturbericht](KONTROLLPLAYER-TONSPUR-KORREKTUR-20261101.md) · [Aktueller Stand](AKTUELLER-STAND.md).
