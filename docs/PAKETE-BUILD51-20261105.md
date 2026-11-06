# CAN 1.0.3 Build 51 – lokale Pakete

**Audio-Fehlerbehandlung-Entwicklung**, Windows 11 x64, Dateiversion **1.0.3.51**. Der Paketsatz gehört zur persönlich erprobten lokalen Prüffassung. [Audioablauf und Praxisnachweise](AUDIO-DREI-OPTIONEN-20261105.md)

## Dateien

| Datei | Inhalt |
|---|---|
| `CutAssistantNext-1.0.3-Build51-win-x64-Setup.exe` | Installation als **Cut Assistant Next** im bisherigen CAN-Programmordner |
| `CutAssistantNext-1.0.3-Build51-win-x64.zip` | Portable Anwendung. Vollständig in einen eigenen Ordner entpacken und `CutAssistantNext.App.exe` starten |
| `CutAssistantNext-1.0.3-Build51-source.zip` | CAN-Quellen und eingebundene Spotlight-CAN-Quellen einschließlich der eigenständigen Anwendung, Dokumentation und Tests |
| `otr-can-0.1.6-audio-diag.1-source.zip` | Quellen zum mitgelieferten Rust-Motor mit dem Merkmal `experimental-aac-gap` |
| `CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip` und `CAN-libmpv-v0.41.0-relink-2026-09-26.zip` | Zugehörige libmpv-Quellen, Nachweise und Material zum erneuten Linken |
| `SHA256SUMS.txt` und `PUBLISH-FILES.sha256` | Prüfsummen für Pakete und Dateien der Anwendung |

## Start und Werkzeuge

Installiere die **.NET 10 Desktop Runtime für Windows x64**. Für Analyse und Schnitt werden je nach gewähltem Motor **FFmpeg/ffprobe**, **GPAC/MP4Box** oder **FFMS2/ffmsindex** benötigt. MP4Box und die FFmpeg- sowie FFMS2-Pfade werden in den vorhandenen CAN-Einstellungen übernommen. Der interne Build-51-Motor liegt unter `runtime\otr-can\otr_can.exe`. CAN wählt ihn in dieser Fassung automatisch. Bei einer fehlenden VC++-Laufzeit verwende das x64-Paket von Microsoft. Bezugsquellen stehen in `VORAUSSETZUNGEN.txt`.

Das Setup verwendet die bisherige CAN-Installationskennung und kann eine vorhandene CAN-Installation aktualisieren. Setup und Portable-ZIP verwenden das vorhandene CAN-Benutzerprofil. „Portable“ bezeichnet die Ausführung aus einem entpackten Ordner. Lizenz- und Herkunftshinweise befinden sich im Anwendungsordner unter `LICENSE`, `third-party` und `runtime\otr-can`.

## Quellpaket bauen

Das CAN-Quellpaket enthält den für Build 51 verwendeten Spotlight-CAN-Stand bereits unter `external/spotlight-can`. Bei einem Git-Checkout zuerst `git submodule update --init --recursive` ausführen. Für die Wiedergabe vor dem Build `tools/setup-libmpv.ps1 -SourceDirectory <Ordner-mit-vier-libmpv-DLLs>` mit den geprüften DLLs für libmpv 0.41.0 aufrufen. Verwende für CAN das .NET 10 SDK und PowerShell.

~~~powershell
dotnet restore .\CutAssistantNext.sln -r win-x64 --locked-mode
dotnet build .\CutAssistantNext.sln -c Release --no-restore -p:CanBuildNumber=51 -p:CanCandidate=Audio-Fehlerbehandlung-Entwicklung
dotnet test .\CutAssistantNext.sln -c Release --no-build --no-restore
~~~

Der Rust-Motor ist separat baubar. Im entpackten Motor-Quellordner mit der Windows-MSVC-Toolchain ausführen.

~~~powershell
cargo build --release --locked --features experimental-aac-gap
~~~

## Prüfgrenzen

Der experimentelle AAC-Weg ist auf nachgewiesene geeignete innere Lücken beschränkt. Er ergänzt Stille und prüft den neuen Schnitt vollständig. Die geprüfte NCIS-Reparatur füllte 512 Samples bei 48 kHz. Die Bildprüfungen nahmen im Referenzlauf zusammen rund neun Minuten in Anspruch. Persönliche Positivmeldungen liegen auch für reguläre Schnitte vor. [Ablauf, Grenzen und Filmergebnisse](AUDIO-DREI-OPTIONEN-20261105.md)
