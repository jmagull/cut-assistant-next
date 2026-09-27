# CAN-libmpv 0.41.0: geprüfter Kandidat

**Status: lokaler Prüf- und Testkandidat, nicht zur öffentlichen Verteilung freigegeben.** Diese Unterlagen dokumentieren den selbst gebauten Vier-DLL-Kandidaten. CANs Build und Publish verwenden jetzt diesen Kandidaten. Ein internes Test-Setup und ein portables Test-ZIP wurden erzeugt; die öffentliche Freigabe steht noch aus.

## Herkunft und Binärdateien

| Bestandteil | Festgelegter Stand |
| --- | --- |
| mpv | Tag `v0.41.0`, Commit `41f6a645068483470267271e1d09966ca3b9f413`; Quell-ZIP SHA-256 `7A37DED4AE1655528525C008736E3ABD2F0F360E94140107D62185D056A20C46` |
| FFmpeg | Version `9.0.2`; Quellarchiv SHA-256 `8C3850283EB25FA026482078A04051E0BE17347B09EF81A0849BEC15A96E002E`; abgetrennte Signatur erfolgreich gegen den FFmpeg-Veröffentlichungsschlüssel `FCF986EA15E6E293A5644F10B4322F04D67658D8` geprüft |
| MSYS2 | CLANG64; 25 unmittelbar beim Linken erfasste, versionierte Binärpakete und 31 korrespondierende Quellarchive mit Build-Rezepten und Patches |
| libdovi | Version `3.4.0-1`; 89 Registry-Archive aus dem Cargo-Lockfile gesichert und gegen dessen Prüfsummen geprüft (einschließlich Entwicklungsabhängigkeiten) |

| Datei des getesteten Pakets | SHA-256 |
| --- | --- |
| `libmpv-2.dll` | `95A37097C6C7EADF7098A0247AC9028143DA26E14081A76A7F6575C1A5AB22EA` |
| `libc++.dll` | `7344DAED05388589E9BD691ED1D30C568C374DA4B8B6A12E1502185948C03CD4` |
| `libshaderc_shared.dll` | `A66696E62D2207B259E33BC3E00E02C55B77F74048872596805B8D82BCDABFB6` |
| `libspirv-cross-c-shared.dll` | `2AF800CBBD27CEA876227DD3903F7FB662652BBC86E2BA9F92F13E501F1E637A` |

Das gesicherte Quellen-, Lizenz- und Build-Prüfpaket heißt `CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip`, umfasst 405.926.830 Bytes und hat SHA-256 `01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B`. Es ist derzeit lokal gesichert und **noch kein öffentlich erreichbares Release-Asset**. Bei einer Veröffentlichung müssen die ausgelieferten Binärdateien und ihre korrespondierenden Quellen gemeinsam dauerhaft erreichbar sein. Dieses große Archiv gehört als Release-Asset neben die Binärdateien, nicht in die Git-Historie.

Das separate Relink-Paket `CAN-libmpv-v0.41.0-relink-2026-09-26.zip` umfasst 47.273.515 Bytes und hat SHA-256 `00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342`. Es enthält die 216 mpv-Objektdateien, 63 Link-Archive, einen ortsunabhängigen Link-Aufruf und PowerShell-5.1-Skripte. Es wurde nach dem Entpacken in einem anderen Ordner sowohl unverändert als auch mit einem geänderten `libfribidi.a` erfolgreich neu verknüpft. Der neue Marker wurde in der DLL nachgewiesen; beide DLLs initialisierten mpv erfolgreich. Details: [RELINK-VERIFICATION.md](RELINK-VERIFICATION.md). Dieses Paket muss bei einer öffentlichen Ausgabe neben dem Quellenpaket erreichbar sein.

## Tatsächlicher Build

FFmpeg wurde als statische Bibliotheken mit `--enable-gpl --enable-version3 --disable-autodetect --disable-shared` gebaut; die Konfiguration meldete `GPL version 3 or later`. Weitere Optionen, Konfigurationsprotokolle und die installierten Paketdateien befinden sich im Prüfpaket unter `build-records/`.

mpv wurde unter anderem mit `gpl=true`, `libmpv=true`, `cplayer=false`, `auto_features=disabled`, `prefer_static=true`, `buildtype=release`, D3D11/WASAPI/OpenGL sowie aktiviertem shaderc und SPIRV-Cross konfiguriert. Die statischen FFmpeg-Archive stammten aus dem privaten FFmpeg-Build; getrennte C++-Linkargumente `-lc++` und `-lc++abi` führten zum erfolgreichen Meson-Link. Die vollständigen finalen Meson-Optionen, Abhängigkeiten und Link-Unterlagen liegen im Prüfpaket unter `build-records/`. Dessen damalige absolute Build-Pfade sind nicht portabel und müssen für einen Neuaufbau angepasst werden.

Der Windows-Ladertest mit nur diesen vier DLLs und Systembibliotheken gelang. `libmpv` meldete Client-API 2.5, initialisierte und spielte ein lokales MPEG-4/AAC-Testvideo bis zum regulären Ende. Der integrierte Publish-Ordner bestand erneut den Lade- und Initialisierungstest. Der Projektverantwortliche bestätigte am 27.09.2026 den HD-Schnitt einer großen Aufnahme sowie auf einer VM Installation, einen dreiteiligen Schnitt, Prüfung der fertigen Datei und saubere Deinstallation. Die älteren selbstenthaltenen Testpakete enthalten noch nicht CANs neuen Lizenzhinweis und die vollständige .NET-Lizenzzuordnung; die spätere frameworkabhängige interne Vorschau enthält die aktualisierten Hinweise und wurde auf der VM geprüft. Details: [INTEGRATION-TEST.md](INTEGRATION-TEST.md).

## Nachweise in diesem Verzeichnis

- `metadata/libmpv-linked-archives-2026-09-26.txt`: beim Linken erfasste statische Archive und zugehörige MSYS2-Pakete.
- `metadata/libmpv-msys2-package-lock-2026-09-26.csv`: 25 Binärpakete mit Version, Größe und SHA-256.
- `metadata/libmpv-msys2-source-lock-2026-09-26.csv`: 31 Quellarchive mit Größe, SHA-256 und Anzahl der enthaltenen Patches/Quellen.
- `metadata/libmpv-msys2-source-locations-2026-09-26.csv`: Zuordnung der Binärpakete zu den MSYS2-Quellarchiven und Downloadadressen.
- `metadata/libmpv-msys2-source-files-2026-09-26.csv`: abgeglichene Prüfsummen der in den Quellarchiven enthaltenen Quellen und Patches; sieben Signaturdateien waren in `.SRCINFO` mit `SKIP` angegeben.
- `metadata/libmpv-libdovi-crates-lock-2026-09-26.csv`: 89 Cargo-Registry-Archive mit SHA-256.
- `metadata/libmpv-libdovi-crates-licenses-2026-09-26.csv`: Lizenzangaben der Crates als Bestandsaufnahme, nicht als Nachweis, dass jede Crate im DLL-Code enthalten ist.
- `metadata/libdovi-production-license-closure-2026-09-26.csv`: konservative Produktions-/Build-Abhängigkeitsliste aus dem tatsächlichen `--all-features`-Baurezept; 28 Crates mit zugeordneten Lizenztexten.

Die Dateien in `metadata/` sind ein kleiner, versionierbarer Lock- und Prüfindex. Die zugehörigen Originalarchive, Lizenztexte und Bauprotokolle befinden sich im gesicherten Prüfpaket. Ein bitidentischer Neuaufbau aller MSYS2-Pakete wurde nicht durchgeführt.

[LICENSING-REVIEW.md](LICENSING-REVIEW.md) hält die Lizenzprüfung und die vor einer öffentlichen Ausgabe verbleibenden Schritte fest.
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) ordnet die Komponenten den hier gesicherten Originaltexten unter `licenses/` zu. Diese Hinweise gelten ausschließlich für die vier oben gehashten DLLs.

## Lizenz- und Release-Status

mpvs `Copyright` nennt GPLv2 oder später für den Standard-Build. FFmpeg wurde hier als GPLv3-oder-später-Build konfiguriert. Das mitgelinkte LittleCMS-`fast_float`-Modul trägt ebenfalls GPLv3-Bedingungen. Für CANs eigenen Quellcode ist GPL-3.0-or-later festgelegt; der vollständige Text steht im Repository unter `LICENSE`. Die selbstenthaltene Windows-Ausgabe enthält zusätzlich .NET-/WPF-Binärdateien mit eigenen Bedingungen; siehe [DOTNET-LICENSING.md](../../DOTNET-LICENSING.md).

Die statisch eingebundenen Bibliotheken libplacebo, GLib, FriBidi, libiconv und libintl bleiben unter LGPL; der Weg über ersetzbare Archive und Relink-Material nach LGPLv2.1 §6 wurde technisch geprüft. Graphite2 bietet ausdrücklich GPLv2 oder später als Alternative; für diesen Kandidaten ist GPLv3 gewählt. FreeTypes FTL-Credit und die Hinweise für die drei Begleit-DLLs stehen in den Drittanbieterhinweisen. Die 28 Crates der konservativen libdovi-Produktionsliste sind mit Originaltexten zugeordnet. Die technische Relink-Probe ist noch keine öffentliche Freigabe.

Vor einer öffentlichen CAN-Ausgabe sind deshalb noch nötig:

1. Den exakten freizugebenden Quellstand festhalten; die Herkunftsbestätigung für CAN-Code und Programmsymbol ist in der [Freigabecheckliste](../../RELEASE-CHECKLIST-BUILD6.md) dokumentiert.
2. Die in [DOTNET-LICENSING.md](../../DOTNET-LICENSING.md) begründet bewertete frameworkabhängige Paketform und ihre Voraussetzungen beim endgültigen Publish beibehalten.
3. Die beiden unveränderten Quellen-/Relink-ZIP-Pakete mit den vier DLLs dauerhaft öffentlich bereitstellen und die Hashes prüfen.
4. Aus dem festgehaltenen Quellstand ein endgültiges frameworkabhängiges Installer- und Portable-Paket mit allen aktuellen Hinweisen bauen und prüfen. Die VM-Tests vom 27.09.2026 gelten für die dokumentierten internen Pakete.

**Die dokumentierte Bewertung ist keine verbindliche Rechtsberatung oder Garantie. Die genannten Veröffentlichungsarbeiten bleiben erforderlich.**
