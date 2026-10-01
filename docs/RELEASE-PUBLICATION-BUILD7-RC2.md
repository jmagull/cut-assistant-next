# Veröffentlichung: CAN 0.2.1 Build 7 – RC2

Die Vorabversion wurde am **01.10.2026 um 20:03 Uhr MESZ** veröffentlicht.

- [GitHub-Release v0.2.1-build7-rc2](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.1-build7-rc2)
- [Prüfsummen](https://github.com/jmagull/cut-assistant-next/releases/download/v0.2.1-build7-rc2/SHA256SUMS.txt)
- [Prüfbericht](RELEASE-BUILD7-RC2-RESULT.md)

Der Tag verweist auf den gebauten Commit `dcf4123bd663aa7a61cb22b35764716471e7624b`. Die Binärpakete stammen aus dem frisch entpackten CAN-Quellarchiv dieses Commits. Der Prüfbericht und dieser Veröffentlichungsnachweis wurden nach dem Paketbau ergänzt; Tag und Quellarchiv bleiben unverändert.

Veröffentlicht wurden Setup, Portable-ZIP, das passende CAN-Quellarchiv, das unveränderte libmpv-Quellen-Prüfpaket, das unveränderte Relink-Paket und SHA256SUMS.txt. Alle sechs Uploads wurden vor der Freigabe anhand ihrer Größe und der von GitHub gemeldeten SHA-256-Digests mit den lokalen Dateien abgeglichen. GitHub bestätigte anschließend `isDraft: false`, `isPrerelease: true` und die Tag-Zuordnung zum Quellcommit.

Der Release-Build war fehler- und warnungsfrei; 496/496 Tests bestanden. Die 148 Dateien des Portable-ZIPs stimmen mit dem geprüften Publish überein. Der Portable-Start mit richtiger Versionsanzeige und das reguläre Beenden wurden geprüft. Nach der Veröffentlichung bestätigte der Benutzer die Installation in einer VM mit GPAC, .NET 10 und FFmpeg sowie einen vollständigen Schnitt von „Grey’s Anatomy – Durchs Feuer“ mit drei Behaltebereichen. Bild und Ton der fertigen Datei wurden als einwandfrei bestätigt. Die anschließende Setup-Deinstallation in der VM wurde ebenfalls als erfolgreich bestätigt. Ein separater Videoschnitt mit dem Portable-ZIP steht noch aus. Der Prüfbericht und die Release-Hinweise wurden entsprechend ergänzt.

Voraussetzungen bleiben Windows 11 x64, eine separat installierte .NET 10 Desktop Runtime und separat eingerichtete FFmpeg/ffprobe- sowie GPAC/MP4Box-Werkzeuge. Der geprüfte libmpv-Stack ist enthalten. Die vorherige Veröffentlichung 0.2.0 Build 6 RC2 bleibt unverändert verfügbar.
