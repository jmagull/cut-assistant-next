# Öffentliche RC1-Veröffentlichung

Die Vorabversion **0.2.0 Build 6 – RC1** wurde am 27. September 2026 im öffentlichen Repository veröffentlicht:

- [GitHub-Release `v0.2.0-build6-rc1`](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.0-build6-rc1)
- [Quellstand `0e54453878bd995debe9206bf55ad5f628210f58`](https://github.com/jmagull/cut-assistant-next/commit/0e54453878bd995debe9206bf55ad5f628210f58), auf den der Release-Tag zeigt
- [Prüfsummendatei `SHA256SUMS.txt`](https://github.com/jmagull/cut-assistant-next/releases/download/v0.2.0-build6-rc1/SHA256SUMS.txt)

Der Release enthält Setup, Portable-ZIP, das für diesen Build verwendete CAN-Quellpaket sowie das Quell- und Relink-Paket für den selbst gebauten libmpv-Stack. Diese fünf Pakete und die Prüfsummendatei wurden nach dem Upload anhand von Dateigröße und SHA-256 gegen die lokalen RC1-Artefakte abgeglichen. Repository, Release-Seite und alle sechs Downloads waren anschließend ohne Anmeldung erreichbar.

Die Quelländerungen des RC1 wurden über [Pull Request #18](https://github.com/jmagull/cut-assistant-next/pull/18) in `main` übernommen. Spätere Änderungen an dieser README und an diesem Veröffentlichungsnachweis betreffen nur die Dokumentation; der RC1-Tag und die geprüften Release-Pakete bleiben beim eingefrorenen Quellstand. Ältere Checklisten und Prüfprotokolle dokumentieren den Zustand *vor* der öffentlichen Freigabe.

RC1 ist eine **Vorabversion**. Für die Nutzung unter Windows 11 x64 werden die [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0), [FFmpeg/ffprobe](https://ffmpeg.org/download.html) und [GPAC/MP4Box](https://gpac.io/downloads/) entsprechend der [Nutzeranleitung](NUTZERANLEITUNG.md) benötigt. Die Lizenz- und Herkunftshinweise stehen in [LICENSE](../LICENSE), der [libmpv-Dokumentation](libmpv/v0.41.0/README.md) und den [Third-Party-Notices](libmpv/v0.41.0/THIRD-PARTY-NOTICES.md).
