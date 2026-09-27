# Drittanbieterhinweise: CAN-libmpv 0.41.0

**Gilt ausschließlich für den in [README.md](README.md) mit vier SHA-256-Werten bezeichneten DLL-Kandidaten. Diese Hinweise allein sind keine Freigabe zur öffentlichen Verteilung.** Die Original-Lizenztexte liegen in diesem Verzeichnis unter `licenses/`. Das korrespondierende Quellen-Prüfpaket `CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip` (SHA-256 `01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B`) enthält Quellen, Paketrezepte, Patches und Bauprotokolle. Das Relink-Paket `CAN-libmpv-v0.41.0-relink-2026-09-26.zip` (SHA-256 `00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342`) enthält die Objektdateien, Archive und eine geprüfte Anleitung zum Ersetzen der statisch eingebundenen LGPL-Bibliotheken. Beide ZIP-Dateien müssen bei einer Binärverteilung zusammen mit diesen Hinweisen zugänglich sein.

Für CANs eigenen Quellcode ist GPL-3.0-or-later festgelegt; der vollständige Text liegt im Repository unter `LICENSE`. Die tatsächliche kombinierte Windows-Ausgabe bleibt bis zum Abschluss der übrigen Lizenz- und Quellenprüfung intern. Die hier genannten Komponenten behalten ihre jeweiligen Copyright- und Lizenzhinweise. Lizenzangaben von MSYS2-Paketen sind Bestandsdaten; die Originaltexte und gegebenenfalls abweichende Einzeldatei-Lizenzen gehen vor.

## Hauptkomponenten

| Komponente | Version/Build | Maßgebliche Hinweise |
| --- | --- | --- |
| mpv | 0.41.0; `gpl=true`, `libmpv=true` | [Copyright](licenses/mpv/Copyright), [LICENSE.GPL](licenses/mpv/LICENSE.GPL), [LICENSE.LGPL](licenses/mpv/LICENSE.LGPL) |
| FFmpeg | 9.0.2; `--enable-gpl --enable-version3`, statisch | [COPYING.GPLv3](licenses/FFmpeg/COPYING.GPLv3), [COPYING.GPLv2](licenses/FFmpeg/COPYING.GPLv2), [COPYING.LGPLv2.1](licenses/FFmpeg/COPYING.LGPLv2.1), [COPYING.LGPLv3](licenses/FFmpeg/COPYING.LGPLv3), [LICENSE.md](licenses/FFmpeg/LICENSE.md) |

mpv nennt für diesen Standard-Build GPLv2 oder später. Die konkrete FFmpeg-Konfiguration meldete GPLv3 oder später. Der resultierende kombinierte Build ist für eine GPLv3-Veröffentlichung vorgesehen; damit werden die eigenständigen Pflichten der weiteren Komponenten nicht aufgehoben.

## Drei mitgelieferte Begleit-DLLs

| Datei im Vier-DLL-Paket | Zuordnung und Lizenz | Originaltext |
| --- | --- | --- |
| `libc++.dll` | LLVM libc++ 22.1.8-1; Apache-2.0 mit LLVM-Exception | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-libc++/libc++/LICENSE) |
| `libshaderc_shared.dll` | shaderc 2026.3-1; Apache-2.0; glslang und SPIR-V Tools als Build-Abhängigkeiten | [shaderc](licenses/msys2/mingw-w64-clang-x86_64-shaderc/shaderc/LICENSE), [glslang](licenses/msys2/mingw-w64-clang-x86_64-glslang/glslang/LICENSE.txt), [SPIR-V Tools](licenses/msys2/mingw-w64-clang-x86_64-spirv-tools/spirv-tools/LICENSE) |
| `libspirv-cross-c-shared.dll` | SPIRV-Cross 1~1.4.357.0-1; Apache-2.0 | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-spirv-cross/spirv-cross/LICENSE) |

Diese Zuordnung bezieht sich auf die gesicherten Paketversionen und DLL-Hashes. Welche Teile der shaderc-Build-Abhängigkeiten in seiner DLL landen, ist damit noch nicht dateigenau geklärt; ihre Texte sind vorsorglich beigefügt.

## Beim Linken erfasste MSYS2-Pakete

| Paket | Version | Paket-Lizenzangabe | Originaltexte |
| --- | --- | --- | --- |
| `mingw-w64-clang-x86_64-brotli` | `1.2.0-1` | MIT | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-brotli/brotli/LICENSE) |
| `mingw-w64-clang-x86_64-bzip2` | `1.0.8-4` | custom | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-bzip2/bzip2/LICENSE) |
| `mingw-w64-clang-x86_64-crt` | `14.0.0.r426.g4564ee4b5-1` | ZPL-2.1 | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-crt/crt/COPYING), [COPYING.MinGW-w64-runtime.txt](licenses/msys2/mingw-w64-clang-x86_64-crt/crt/COPYING.MinGW-w64-runtime.txt), [COPYING.MinGW-w64.txt](licenses/msys2/mingw-w64-clang-x86_64-crt/crt/COPYING.MinGW-w64.txt) |
| `mingw-w64-clang-x86_64-expat` | `2.8.5-1` | MIT | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-expat/expat/COPYING) |
| `mingw-w64-clang-x86_64-fontconfig` | `2.18.3-1` | custom | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-fontconfig/fontconfig/COPYING) |
| `mingw-w64-clang-x86_64-freetype` | `2.14.3-1` | GPL-2.0-or-later OR FTL | [FTL.TXT](licenses/msys2/mingw-w64-clang-x86_64-freetype/freetype/FTL.TXT), [GPLv2.TXT](licenses/msys2/mingw-w64-clang-x86_64-freetype/freetype/GPLv2.TXT) |
| `mingw-w64-clang-x86_64-fribidi` | `1.0.17-1` | LGPL-2.1-or-later | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-fribidi/fribidi/COPYING) |
| `mingw-w64-clang-x86_64-gettext-runtime` | `1.0-1` | GPL-3.0-or-later AND LGPL-2.1-or-later | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-gettext-runtime/gettext-runtime/COPYING), [COPYING.LIB](licenses/msys2/mingw-w64-clang-x86_64-gettext-runtime/gettext-runtime/intl/COPYING.LIB), [COPYING](licenses/msys2/mingw-w64-clang-x86_64-gettext-runtime/gettext-runtime/libasprintf/COPYING), [COPYING.LIB](licenses/msys2/mingw-w64-clang-x86_64-gettext-runtime/gettext-runtime/libasprintf/COPYING.LIB) |
| `mingw-w64-clang-x86_64-glib2` | `2.90.0-1` | LGPL-2.1-or-later | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-glib2/glib2/COPYING) |
| `mingw-w64-clang-x86_64-graphite2` | `1.3.15-1` | LGPL-2.1-or-later | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-graphite2/graphite2/COPYING), [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-graphite2/graphite2/LICENSE) |
| `mingw-w64-clang-x86_64-harfbuzz` | `14.5.0-1` | MIT | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-harfbuzz/harfbuzz/COPYING) |
| `mingw-w64-clang-x86_64-lcms2` | `2.19.1-1` | MIT AND GPL-3.0-or-later | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-lcms2/lcms2/LICENSE), [LICENSE-fast_float](licenses/msys2/mingw-w64-clang-x86_64-lcms2/lcms2/LICENSE-fast_float) |
| `mingw-w64-clang-x86_64-libass` | `0.17.5-1` | ISC | [mingw-w64-clang-x86_64-libass-0.17.5-COPYING](licenses/msys2/mingw-w64-clang-x86_64-libass-0.17.5-COPYING) |
| `mingw-w64-clang-x86_64-libc++` | `22.1.8-1` | Apache-2.0 WITH LLVM-exception | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-libc++/libc++/LICENSE) |
| `mingw-w64-clang-x86_64-libdovi` | `3.4.0-1` | MIT | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-libdovi/libdovi/LICENSE) |
| `mingw-w64-clang-x86_64-libiconv` | `1.19-1` | LGPL-2.1-or-later  documentation:GPL-3.0-or-later | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-libiconv/libiconv/COPYING), [COPYING.LIB](licenses/msys2/mingw-w64-clang-x86_64-libiconv/libiconv/COPYING.LIB), [COPYING.LIB](licenses/msys2/mingw-w64-clang-x86_64-libiconv/libiconv/libcharset/COPYING.LIB), [README](licenses/msys2/mingw-w64-clang-x86_64-libiconv/libiconv/README) |
| `mingw-w64-clang-x86_64-libplacebo` | `7.360.1-2` | LGPL2.1 | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-libplacebo/libplacebo/LICENSE) |
| `mingw-w64-clang-x86_64-libpng` | `1.6.58-1` | custom | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-libpng/libpng/LICENSE) |
| `mingw-w64-clang-x86_64-libunibreak` | `7.0-1` | Zlib | [LICENCE](licenses/msys2/mingw-w64-clang-x86_64-libunibreak/libunibreak/LICENCE) |
| `mingw-w64-clang-x86_64-libunwind` | `22.1.8-1` | Apache-2.0 WITH LLVM-exception | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-libunwind/libunwind/LICENSE) |
| `mingw-w64-clang-x86_64-pcre2` | `10.48-3` | BSD-3-Clause | [COPYING](licenses/msys2/mingw-w64-clang-x86_64-pcre2/pcre2/COPYING), [LICENCE.md](licenses/msys2/mingw-w64-clang-x86_64-pcre2/pcre2/LICENCE.md) |
| `mingw-w64-clang-x86_64-shaderc` | `2026.3-1` | Apache-2.0 | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-shaderc/shaderc/LICENSE) |
| `mingw-w64-clang-x86_64-spirv-cross` | `1~1.4.357.0-1` | Apache-2.0 | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-spirv-cross/spirv-cross/LICENSE) |
| `mingw-w64-clang-x86_64-vulkan-loader` | `1~1.4.357.0-1` | Apache-2.0 | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-vulkan-loader/vulkan-loader/LICENSE) |
| `mingw-w64-clang-x86_64-zlib` | `1.3.2-2` | Zlib | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-zlib/zlib/LICENSE) |

Für FreeType wird in diesem Entwurf die FreeType-Lizenz (FTL) gewählt: **Diese Software enthält Bestandteile des FreeType Project (https://freetype.org).** Der vollständige FTL-Text liegt oben beim FreeType-Paket. Graphite2 bietet in seinem `COPYING` ausdrücklich GPLv2 oder später als Alternative zur LGPL; für diesen GPLv3-Kandidaten wird diese GPLv3-Option gewählt. LittleCMS enthält hier zusätzlich `liblcms2_fast_float.a`; dessen `LICENSE-fast_float` ist der GPLv3-Text. Bei `gettext-runtime` stammt die gelinkte `libintl.a` aus dem LGPL-Teil des Pakets; die übrigen Lizenztexte des Pakets sind ebenfalls beigefügt.

## Weitere gesicherte Quellen und mögliche eingebundene Hilfskomponenten

Der shaderc-Build verwendet glslang und SPIR-V Tools; libplacebo verwendet unter anderem die eigenständige Bibliothek fast_float und xxHash. Diese fast_float-Bibliothek ist nicht dasselbe wie das GPLv3-Modul `liblcms2_fast_float.a`. Auch Build- und Headerpakete werden hier vorsorglich genannt, ohne zu behaupten, dass jede Quelldatei im DLL-Code enthalten ist.

| Quellpaket | Version | Originaltexte |
| --- | --- | --- |
| `mingw-w64-fast_float` | `8.2.10-1` | [LICENSE-APACHE](licenses/msys2/mingw-w64-clang-x86_64-fast_float/fast_float/LICENSE-APACHE), [LICENSE-BOOST](licenses/msys2/mingw-w64-clang-x86_64-fast_float/fast_float/LICENSE-BOOST), [LICENSE-MIT](licenses/msys2/mingw-w64-clang-x86_64-fast_float/fast_float/LICENSE-MIT) |
| `mingw-w64-glslang` | `16.3.0-1` | [LICENSE.txt](licenses/msys2/mingw-w64-clang-x86_64-glslang/glslang/LICENSE.txt) |
| `mingw-w64-python-glad` | `2.0.8-3` | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-python-glad/python-glad/LICENSE) |
| `mingw-w64-spirv-headers` | `2~1.4.357.0-1` | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-spirv-headers/spirv-headers/LICENSE) |
| `mingw-w64-spirv-tools` | `3~1.4.357.0-1` | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-spirv-tools/spirv-tools/LICENSE) |
| `mingw-w64-vulkan-headers` | `1~1.4.350.1-1` | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-vulkan-headers/vulkan-headers/LICENSE) |
| `mingw-w64-xxhash` | `0.8.3-2` | [LICENSE](licenses/msys2/mingw-w64-clang-x86_64-xxhash/xxhash/LICENSE) |

## libdovi und Rust-Abhängigkeiten

`libdovi.a` stammt aus dem `dolby_vision`-C-API-Build 3.4.0 (`cargo cbuild --all-features`). Die folgende aus `Cargo.toml` und `Cargo.lock` berechnete Liste umfasst **28 Crates als konservative Obergrenze** für Produktions- und Build-Abhängigkeiten; Makros und plattformspezifische Anteile müssen nicht alle im fertigen DLL-Code enthalten sein. Die vollständigen 89 Lockfile-Crates sind im Quellen-Prüfpaket erhalten. Die beiden `winapi-*-pc-windows-gnu`-Crates sind nicht in dieser Produktions-Abhängigkeitsliste.

| Crate | Version | Lizenzangabe | Originaltexte |
| --- | --- | --- | --- |
| `anyhow` | `1.0.102` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/anyhow-1.0.102/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/anyhow-1.0.102/LICENSE-MIT) |
| `bitstream-io` | `4.10.0` | MIT/Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/bitstream-io-4.10.0/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/bitstream-io-4.10.0/LICENSE-MIT) |
| `bitvec` | `1.0.1` | MIT | [LICENSE.txt](licenses/libdovi-crates/bitvec-1.0.1/LICENSE.txt) |
| `bitvec_helpers` | `4.0.2` | MIT | [LICENSE](licenses/libdovi-crates/bitvec_helpers-4.0.2/LICENSE) |
| `crc` | `3.4.0` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/crc-3.4.0/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/crc-3.4.0/LICENSE-MIT) |
| `crc-catalog` | `2.5.0` | MIT OR Apache-2.0 | [LICENSE-Apache-2.0](licenses/libdovi-crates/crc-catalog-2.5.0/LICENSE-Apache-2.0), [LICENSE-MIT](licenses/libdovi-crates/crc-catalog-2.5.0/LICENSE-MIT) |
| `equivalent` | `1.0.2` | Apache-2.0 OR MIT | [LICENSE-APACHE](licenses/libdovi-crates/equivalent-1.0.2/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/equivalent-1.0.2/LICENSE-MIT) |
| `funty` | `2.0.0` | MIT | [LICENSE.txt](licenses/libdovi-crates/funty-2.0.0/LICENSE.txt) |
| `hashbrown` | `0.17.1` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/hashbrown-0.17.1/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/hashbrown-0.17.1/LICENSE-MIT) |
| `indexmap` | `2.14.0` | Apache-2.0 OR MIT | [LICENSE-APACHE](licenses/libdovi-crates/indexmap-2.14.0/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/indexmap-2.14.0/LICENSE-MIT) |
| `itoa` | `1.0.18` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/itoa-1.0.18/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/itoa-1.0.18/LICENSE-MIT) |
| `libc` | `0.2.186` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/libc-0.2.186/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/libc-0.2.186/LICENSE-MIT) |
| `memchr` | `2.8.0` | Unlicense OR MIT | [COPYING](licenses/libdovi-crates/memchr-2.8.0/COPYING), [LICENSE-MIT](licenses/libdovi-crates/memchr-2.8.0/LICENSE-MIT) |
| `no_std_io2` | `0.9.4` | Apache-2.0 OR MIT | [LICENSE-APACHE](licenses/libdovi-crates/no_std_io2-0.9.4/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/no_std_io2-0.9.4/LICENSE-MIT) |
| `proc-macro2` | `1.0.106` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/proc-macro2-1.0.106/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/proc-macro2-1.0.106/LICENSE-MIT) |
| `quote` | `1.0.45` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/quote-1.0.45/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/quote-1.0.45/LICENSE-MIT) |
| `radium` | `0.7.0` | MIT | [LICENSE.txt](licenses/libdovi-crates/radium-0.7.0/LICENSE.txt) |
| `roxmltree` | `0.21.1` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/roxmltree-0.21.1/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/roxmltree-0.21.1/LICENSE-MIT) |
| `serde` | `1.0.228` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/serde-1.0.228/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/serde-1.0.228/LICENSE-MIT) |
| `serde_core` | `1.0.228` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/serde_core-1.0.228/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/serde_core-1.0.228/LICENSE-MIT) |
| `serde_derive` | `1.0.228` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/serde_derive-1.0.228/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/serde_derive-1.0.228/LICENSE-MIT) |
| `serde_json` | `1.0.149` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/serde_json-1.0.149/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/serde_json-1.0.149/LICENSE-MIT) |
| `syn` | `2.0.117` | MIT OR Apache-2.0 | [LICENSE-APACHE](licenses/libdovi-crates/syn-2.0.117/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/syn-2.0.117/LICENSE-MIT) |
| `tap` | `1.0.1` | MIT | [LICENSE.txt](licenses/libdovi-crates/tap-1.0.1/LICENSE.txt) |
| `tinyvec` | `1.11.0` | Zlib OR Apache-2.0 OR MIT | [LICENSE-APACHE.md](licenses/libdovi-crates/tinyvec-1.11.0/LICENSE-APACHE.md), [LICENSE-MIT.md](licenses/libdovi-crates/tinyvec-1.11.0/LICENSE-MIT.md), [LICENSE-ZLIB.md](licenses/libdovi-crates/tinyvec-1.11.0/LICENSE-ZLIB.md) |
| `unicode-ident` | `1.0.24` | (MIT OR Apache-2.0) AND Unicode-3.0 | [LICENSE-APACHE](licenses/libdovi-crates/unicode-ident-1.0.24/LICENSE-APACHE), [LICENSE-MIT](licenses/libdovi-crates/unicode-ident-1.0.24/LICENSE-MIT), [LICENSE-UNICODE](licenses/libdovi-crates/unicode-ident-1.0.24/LICENSE-UNICODE) |
| `wyz` | `0.5.1` | MIT | [LICENSE.txt](licenses/libdovi-crates/wyz-0.5.1/LICENSE.txt) |
| `zmij` | `1.0.21` | MIT | [LICENSE-MIT](licenses/libdovi-crates/zmij-1.0.21/LICENSE-MIT) |

Die beiden Texte für `crc-catalog` wurden aus dem in den Crate-Metadaten bezeichneten upstream-Repository-Commit `ed4ad631f22b05055c21a3a4127eb7cf6d75bb62` ergänzt, weil das Registry-Archiv keinen Lizenztext enthält. Bei `unicode-ident` ist zusätzlich zum MIT- bzw. Apache-Text auch der Unicode-Lizenztext enthalten.

## Statisch eingebundene LGPL-Bibliotheken und Relink-Material

`libplacebo`, GLib, FriBidi, libiconv und libintl sind statisch eingebunden und bleiben unter ihrer LGPL. Graphite2 ist ebenfalls statisch; für diesen Kandidaten wird seine ausdrücklich angebotene GPLv3-Option gewählt. Die Original-LGPL-Texte sind beigefügt. Für die fünf LGPL-Bibliotheken wird [LGPLv2.1 §6a](https://www.gnu.org/licenses/old-licenses/lgpl-2.1.en.html) mit dem oben bezeichneten Quellen- und Relink-Paket vorgesehen. Das Relink-Paket enthält 216 mpv-Objektdateien und 63 Link-Archive. Eine neue DLL wurde mit unveränderten Archiven erfolgreich verknüpft und initialisiert. In einer zweiten Probe wurde ein geändertes `libfribidi.a` eingesetzt; dessen zusätzlicher Marker erschien in der neu verknüpften und ebenfalls initialisierten DLL. Die Einzelheiten stehen in [RELINK-VERIFICATION.md](RELINK-VERIFICATION.md). Es wurde **keine** LGPL-Bibliothekskopie nach §3 auf GPLv3 umgestellt.

**Vor der öffentlichen CAN-Ausgabe:** Binärdateien, korrespondierendes Quellen-Prüfpaket, Relink-Paket und diese Hinweise gemeinsam dauerhaft bereitstellen; die vier DLL-Hashes mit [README.md](README.md) abgleichen; CANs eigene Projektlizenz festlegen und die Hinweise samt `licenses/` in Installer und portable Ausgabe aufnehmen. Ein Austausch der DLLs gegen einen anderen Build erfordert eine erneute Zuordnung der Hinweise.
