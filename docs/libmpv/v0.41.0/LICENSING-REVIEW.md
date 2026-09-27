# Lizenzprüfung des CAN-libmpv-0.41.0-Kandidaten

**Technische Lizenzprüfung für genau diesen Vier-DLL-Kandidaten; noch keine öffentliche Freigabe.** Maßgeblich sind die vier SHA-256-Werte in [README.md](README.md). Die zugeordneten [Drittanbieterhinweise](THIRD-PARTY-NOTICES.md), Original-Lizenztexte und ein geprüftes Relink-Paket liegen vor. Quellen- und Relink-Paket sind noch nicht öffentlich erreichbar. Für CANs eigenen Quellcode ist GPL-3.0-or-later gewählt. Setup und Publish verwenden diesen Kandidaten bereits; ein VM-Installations-, Schnitt- und Deinstallationstest wurde erfolgreich berichtet. Für die öffentliche Ausgabe ist ein frameworkabhängiges Paket mit separat installierter .NET Desktop Runtime gewählt. Die begründete Bewertung dieser Paketform und ihre Voraussetzungen sind in [DOTNET-LICENSING.md](../../DOTNET-LICENSING.md) dokumentiert.

## GPLv3-Weg

- mpvs `Copyright` nennt für den Standard-Build GPLv2 oder später; der tatsächliche Build verwendete `gpl=true`.
- FFmpegs Build meldete nach `--enable-gpl --enable-version3` GPLv3 oder später. Seine FFmpeg-Archive wurden statisch eingebunden.
- In der Linkliste stehen `liblcms2.a` **und** `liblcms2_fast_float.a`. Das gesicherte LittleCMS-Baurezept aktiviert `fastfloat`; die Paketlizenz erfasst für dieses Modul GPLv3 oder später. Dies ist daher kein bloßer Metadatenhinweis.
- Die [GNU-Lizenzübersicht](https://www.gnu.org/licenses/gpl-faq.en.html#AllCompatibility) erläutert die Kombination von GPLv2-oder-später-Code mit GPLv3-Code. CANs eigene Lizenzvariante ist GPL-3.0-or-later; der Projektverantwortliche bestätigte die eigene Erstellung des Symbols und die Neuimplementierung des CAN-Codes ohne Übernahme aus anderen Projekten.

Diese Befunde zeigen einen nachvollziehbaren GPLv3-Weg. Sie ersetzen keinen Abgleich sämtlicher tatsächlich kompilierten Dateien, Lizenzoptionen und Copyright-Vermerke.

## Statisch eingebundene LGPL-Teile

Die Linkliste enthält unter anderem `libplacebo.a`, `libglib-2.0.a`, `libfribidi.a`, `libgraphite2.a`, `libiconv.a` und `libintl.a`. Graphite2 bietet laut dem gesicherten `COPYING` GPLv2 oder später als ausdrückliche Alternative; für diesen Kandidaten wird GPLv3 gewählt. Die übrigen fünf Bibliotheken bleiben unter LGPLv2.1; für sie ist der Weg über ersetzbare Archive und Relink-Material nach §6 vorgesehen:

| Link-Eingang | Paketversion | Beleg für die Bibliothekslizenz | Relink-Eingang |
| --- | --- | --- | --- |
| `libplacebo.a` | 7.360.1-2 | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-libplacebo/libplacebo/LICENSE) | Im Relink-Paket ersetzbar |
| `libglib-2.0.a` | 2.90.0-1 | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-glib2/glib2/COPYING) | Im Relink-Paket ersetzbar |
| `libfribidi.a` | 1.0.17-1 | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-fribidi/fribidi/COPYING) | Im Relink-Paket ersetzt und geprüft |
| `libiconv.a` | 1.19-1 | [COPYING.LIB](licenses/msys2/mingw-w64-clang-x86_64-libiconv/libiconv/COPYING.LIB) | Im Relink-Paket ersetzbar |
| `libintl.a` | 1.0-1 | [intl/COPYING.LIB](licenses/msys2/mingw-w64-clang-x86_64-gettext-runtime/gettext-runtime/intl/COPYING.LIB) | Im Relink-Paket ersetzbar |

Die Paketrezepte und Lizenztexte belegen die LGPL-Zuordnung dieser Archive; die Link-Eingänge stehen in [der gesicherten Linkliste](metadata/libmpv-linked-archives-2026-09-26.txt). Das separate Relink-Paket `CAN-libmpv-v0.41.0-relink-2026-09-26.zip` hat SHA-256 `00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342` und umfasst die 216 mpv-Objektdateien sowie 63 Link-Archive. Es wurde aus einem frisch entpackten Ordner mit unveränderten Eingaben und anschließend mit einem geänderten `libfribidi.a` erfolgreich verknüpft. Der Marker aus dem Ersatzarchiv war in der neuen DLL exportiert und aufrufbar; `mpv_create()` und `mpv_initialize()` gelangen in beiden Proben. [RELINK-VERIFICATION.md](RELINK-VERIFICATION.md) protokolliert den Nachweis.

Damit ist die technische Relink-Möglichkeit für diesen Kandidaten belegt. Die LGPLv2.1-[§6-Bedingungen](https://www.gnu.org/licenses/old-licenses/lgpl-2.1.en.html) verlangen bei der tatsächlichen Weitergabe zusätzlich, dass korrespondierende Bibliotheksquellen, Objekt-/Relink-Material, Lizenztexte und prominente Hinweise die Empfänger erreichen. Das Quellen-Prüfpaket und das Relink-Paket müssen daher zusammen mit den vier DLLs verfügbar sein. Eine Umstellung der LGPL-Kopien nach §3 wurde nicht vorgenommen und wird für diesen Weg nicht behauptet.

## Weitere Bestandteile

- FreeType 2.14.3 bietet `GPL-2.0-or-later OR FTL`. In den Drittanbieterhinweisen wird FTL gewählt und der FreeType-Credit genannt; die Originaltexte liegen bei.
- `libc++.dll`/libunwind tragen `Apache-2.0 WITH LLVM-exception`; `libshaderc_shared.dll` und `libspirv-cross-c-shared.dll` sind als eigene ausgelieferte Komponenten zu behandeln. Der shaderc-Build verwendet externe glslang- und SPIRV-Tools-Pakete; deren Texte und Quellen liegen im Prüfpaket.
- Für die übrigen in `metadata/libmpv-linked-archives-2026-09-26.txt` genannten Pakete liegen Lizenztexte und Quellarchive vor. Die Paketmetadaten lauten je nach Paket MIT, ISC, BSD, Zlib, ZPL, Apache oder `custom`; die letzte Gruppe erfordert die konkreten Originaltexte statt einer pauschalen Lizenzbezeichnung.
- Die 89 gesicherten Rust-Crates umfassen auch Entwicklungsabhängigkeiten. Aus dem tatsächlichen `libdovi`-Baurezept (`--all-features`) und dem Lockfile ist eine konservative Produktions-Abhängigkeitsliste von 28 Crates errechnet und in [der Metadatentabelle](metadata/libdovi-production-license-closure-2026-09-26.csv) festgehalten; sie behauptet nicht, dass alle 28 im DLL-Code enthalten sind. Die zwei `winapi-*-pc-windows-gnu`-Crates gehören nicht zu dieser Liste. Für `crc-catalog` wurde der exakte upstream Commit aus dem Registry-Archiv rekonstruiert, um die dort fehlenden Lizenztexte zuzuordnen.

## Vor einer öffentlichen Ausgabe

1. Der Projektverantwortliche bestätigte die eigene Erstellung des Symbols und die Neuimplementierung des CAN-Codes ohne Übernahme aus dem alten Cut Assistant oder anderen Projekten. Den tatsächlich veröffentlichten Quellstand noch eindeutig festhalten.
2. Beim endgültigen Publish die Voraussetzungen der begründeten .NET-Bewertung beibehalten: unveränderte separat installierte Desktop Runtime, keine gebündelten besonders lizenzierten Laufzeitdateien und vollständige Hinweise für mitgelieferte Bestandteile; siehe [DOTNET-LICENSING.md](../../DOTNET-LICENSING.md).
3. Vier DLLs, korrespondierendes Quellenpaket, Relink-Paket und [Drittanbieterhinweise](THIRD-PARTY-NOTICES.md) gemeinsam dauerhaft bereitstellen und die Hashes gegen [README.md](README.md) prüfen. Die [GPLv3 §6](https://www.gnu.org/licenses/gpl.en.html) beschreibt die Quellcodebereitstellung bei Binärdownloads.
4. Aus dem festgehaltenen Quellstand ein endgültiges frameworkabhängiges Installer- und Portable-Paket mit sämtlichen aktuellen Hinweisen bauen und beide Ausgaben prüfen. Die internen Build-6-Testpakete sind kein fertiges öffentliches Release.
