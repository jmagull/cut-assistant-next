# CAN 1.0.2 Build 47 – Veröffentlichung

Nach ausdrücklichem Nutzerauftrag am **31.10.2026** als stabile **[v1.0.2-build47 / Latest](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2-build47)** veröffentlicht. GitHub-Zeit: **2026-10-31T16:54:06Z**. Release-ID **430061255**. Kein Entwurf, keine Vorabversion.

## Code, Freeze und Push

- Release-Commit und Ziel des annotierten Tags **v1.0.2-build47**: **ae6cc93d263a6d70eb16b3102960e3d094ab38f0** ([Commit](https://github.com/jmagull/cut-assistant-next/commit/ae6cc93d263a6d70eb16b3102960e3d094ab38f0)). Main und Tag erfolgreich gepusht; Remote-Ziele geprüft.
- **376 Produkt-/Testdateien** des Release-Commits stimmen inhaltlich mit dem manifestierten Build-47-Quellsnapshot überein. Beim Git-Vergleich werden ausschließlich CRLF/LF-Zeilenenden normalisiert; keine fachlichen Quellenabweichungen. Separate Quellen-/ZIP-Manifeste prüfen die exakten Archivbytes.
- CAN **1.0.2**, App-Dateiversion **1.0.2.47**, otr-can **0.1.6**, libmpv **0.41.0**, .NET 10/WPF, Windows 11 x64. Motor-SHA **73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E**.
- Vier vorbestehende lokale Produktions-Lockänderungen bleiben erhalten und außerhalb des Commits. Keine Videos, Buildausgaben oder lokalen Einstellungen committed. Der anschließende reine Dokumentationsnachtrag dokumentiert die abgeschlossene Veröffentlichung auf main; Release-Tag und Pakete werden nicht verschoben oder ersetzt.

## Prüfung und tatsächliche persönliche Nachweise

**1593 Pakettests** plus **1321 Tests gegen finale Produktbibliotheken**, Release ohne Warnungen/Fehler. Vor Commit nochmals Restore, Build mit unveränderter Nummer 47 und alle 1593 Tests erfolgreich. 250 Portable-Dateien, 705 manifestierte Quelldateien/706 Quellen-ZIP-Dateien, alle Pakete und Prüfsummen geprüft.

Build-47-VM-Setup: MP4Box → QUICK, otr-can → QUICK und AUDIO-ENTHUSIAST persönlich positiv; Sperr-/Dateiwechselchecks bestätigt. Portable: beide QUICK-Wege persönlich positiv, otr-can-AUDIO-ENTHUSIAST technisch erfolgreich; persönliche Kontrolle dieses letzten Laufs nicht separat zugeordnet. Deinstallation Build 47 nicht gesondert bestätigt. Rocky Horror, Doc und Brilliant Minds sind positive **Build-46-Setup-/Arbeitsrechnerfälle**, keine nachträglich umgewidmeten Build-47-Tests. Diese Grenzen bleiben trotz ausdrücklicher Veröffentlichungsfreigabe erhalten.

## Release und Uploads

Alle acht unveränderten Dateien wurden zunächst als Entwurf hochgeladen. Vor Freischaltung Größen und GitHub-SHA-256-Digests gegen die lokalen Dateien verglichen, danach erneut geprüft. Stabile Veröffentlichung und Latest-Zuordnung per API bestätigt. Die Tag-Abfrage war beim Entwurf noch nicht verfügbar; die vollständige Vorprüfung erfolgte erfolgreich über die Release-ID. Der frühere **v1.0.2 / Build 41** (Release-ID 425978946, 27.10.2026) samt allen acht Assets ist weiterhin unverändert vorhanden; weder Tag noch Dateien ersetzt.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-1.0.2-Build47-source.zip | 3670880 | E6514B83521C8EAC52AB92DA758F4B3A01435D5E067D0EC307615D2ED5FA6772 |
| CutAssistantNext-1.0.2-Build47-win-x64-Setup.exe | 20508494 | E7AA9C6E61FF7333D9BE94AA631050389D9BAE3BBCB80B77015EBE16453D5712 |
| CutAssistantNext-1.0.2-Build47-win-x64.zip | 25239558 | 394DBE9B09727E3D9E9C3FC9E31927500DFA2C5D27BA6AEDDF9A84A9887DFAA4 |
| PUBLISH-FILES.sha256 | 32911 | 68937306250F1B7F9C9A5B9D939D993B4E6B57DE541E6BB4BDB71F9DFC103413 |
| SHA256SUMS.txt | 647 | A7676FD516813773805CD040731A556DC9202677CD97BC452CE4806FFC5BE066 |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |

Die Quellenarchive gehören exakt zu den geprüften Binärpaketen. Ihre Dokumentation behält den Stand bei Snapshot-Erstellung vor Freigabe. Aktuelle Repository-Unterlagen und die Releasebeschreibung ergänzen spätere Rückmeldungen und Veröffentlichung; kein eingefrorenes Archiv wurde dafür geändert.

Nachweise unter **.build/publication-1.0.2-build47-20261031/**: COMMIT-PROOF.json, RELEASE-COMMIT.txt, PACKAGE-RECHECK.json, DRAFT.json, REMOTE-DRAFT-PROOF.json, PUBLISHED.json, LATEST.json, REMOTE-PROOF.json und PREVIOUS-RELEASE-AFTER.json. Paketprüfung unter **.build/release-1.0.2-build47-20261031/**.

[Paketbericht](RELEASE-1.0.2-BUILD47-RESULT.md), [Projektstand](AKTUELLER-STAND.md), [Release Notes](RELEASE-NOTES-1.0.2.md), [Setup](VM-SETUP-BUILD47-20261031.md), [Portable](VM-PORTABLE-BUILD47-20261031.md), [Build-46-Praxistests](PRAXISTESTS-20261031.md).
