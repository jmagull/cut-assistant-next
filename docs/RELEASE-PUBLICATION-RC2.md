# Öffentliche RC2-Veröffentlichung

Die Vorabversion **0.2.0 Build 6 – RC2** wurde am 29. September 2026 um 17:56 Uhr MESZ veröffentlicht.

- [GitHub-Release v0.2.0-build6-rc2](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.0-build6-rc2)
- [Prüfsummen](https://github.com/jmagull/cut-assistant-next/releases/download/v0.2.0-build6-rc2/SHA256SUMS.txt)
- [RC2-Prüfbericht](RELEASE-BUILD6-RC2-RESULT.md)

## Quellstand und Pakete

Der Tag verweist auf den gebauten Commit `178ba0a3c41f17800648636eb10a76961b3d96b4`. Der Prüfbericht wurde anschließend mit Commit `9c35969` ergänzt. Spätere Dokumentationsänderungen verändern weder den Tag noch die geprüften Pakete.

Veröffentlicht wurden Setup, Portable-ZIP, das zugehörige CAN-Quellarchiv, das libmpv-Quellen-Prüfpaket, das libmpv-Relink-Paket und SHA256SUMS.txt.

Alle sechs Uploads wurden vor der Veröffentlichung anhand ihrer Dateigröße und der von GitHub gemeldeten SHA-256-Digests mit den lokalen Dateien abgeglichen. GitHub bestätigte anschließend `isDraft: false` und `isPrerelease: true`.

## Änderungen und Prüfung

RC2 trennt das Verwerfen aktueller Namensmaskenänderungen vom Wiederherstellen der CAN-Standardmaske. Die Anleitung erklärt beide Funktionen. Die RC-Kennung erscheint in Fenstertitel, Hauptüberschrift und Setup-Versionsbezeichnung.

Der Release-Build war fehler- und warnungsfrei; 484/484 automatisierte Tests bestanden. Setup und Portable-ZIP wurden separat mit einem HD-Schnitt von „Black Adam“ praktisch geprüft; Bild und Ton waren einwandfrei. Drüberinstallation, Namensmasken-Reset und Deinstallation des Setups wurden bestätigt. Der Prüfbericht beschreibt den genauen Prüfumfang.

## Voraussetzungen

RC2 ist eine Vorabversion für Windows 11 x64. Die .NET 10 Desktop Runtime sowie FFmpeg/ffprobe und GPAC/MP4Box werden separat installiert und eingerichtet. Der geprüfte libmpv-Stack ist enthalten.

Siehe [Nutzeranleitung](NUTZERANLEITUNG.md), [Lizenz](../LICENSE) und [libmpv-Hinweise](libmpv/v0.41.0/THIRD-PARTY-NOTICES.md). Die Unterstützung weiterer Eingangscontainer bleibt experimentell.

Die historischen RC1-Berichte bleiben als Nachweise der vorherigen Veröffentlichung erhalten.