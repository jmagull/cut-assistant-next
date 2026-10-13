# Eingefrorener Entwicklungskandidat 0.3.0 · Build 29 · RC3

Stand: 13.10.2026. Nutzerauftrag: **Commit, Push, Setup, ZIP und zugehörige Artefakte erstellen, dann den jetzigen Stand einfrieren**. Die abgeschlossenen persönlichen AVI-/Lupen-/Masken-/Bedienprüfungen gelten für den Entwicklungsstand; die neue Setup-/Portable-Installationsabnahme ist anschließend separat durchzuführen.

## Fester Umfang

- CAN **0.3.0 · Build 29 · RC3**, Dateiversion **0.3.0.29**. Buildnummer und RC-Kennung für den gesamten Paketsatz festhalten.
- Unveränderter Motor **otr-can 0.1.5**, Quellencommit **2a474629836d52023bf37cbbdb309ccd1c23acf3**. Bereits getestete EXE mit SHA-256 **FC6ED6568880A995F3D141A9378AA2CE28ADE8DC0E257E93DC972799BB1324A6**, matching sources/licensing/provenance aus dem geprüften Motorpaket wiederverwenden.
- Geprüfter libmpv-0.41.0-Stack, unveränderte Quellen-/Relink-Archive und Lizenztexte. .NET 10 Desktop Runtime, FFmpeg/ffprobe, GPAC/MP4Box, FFMS2 und VC-Laufzeit weiterhin externe Voraussetzungen; keine zusätzliche private Runtime/Toolbündelung.
- FFmpeg/ffprobe **größer als 5.1** erforderlich, **ab 9.0.2** empfohlen. Paketvoraussetzungen an die bestehende Versionskontrolle angeglichen.

Vier vorbestehende lokale Änderungen an den Produktions-`packages.lock.json` in App, Core, Cutlists und Media bleiben ausdrücklich außerhalb des Commits und des Quellenarchivs erhalten. Alle Quellen werden aus einem festen Git-Commit exportiert, ohne Arbeitsbaumüberlagerung. Der genaue Quellcommit steht im erzeugten `SOURCE-SNAPSHOT.json` und in der Paketprovenienz; spätere Ergebnisberichte werden gesondert dokumentiert.

## Prüfstand vor Einfrieren

Entwicklungsbuild 29: **0 Warnungen/0 Fehler**, **1076/1076 CAN-Tests**, keine übersprungen. AVI-Lupe, Richtungswechsel-Halbsprung, FFmpeg-Prüfung und gespeicherter Maskenwechsel ohne Neustart persönlich bestätigt; Quellen und persönliche Nachweise in den datierten Berichten. Vorprüfung des gesperrten Windows-x64-Restores mit den unveränderten versionierten Lockdateien und Version 0.3.0 erfolgreich. Keine Produktions-/Testabhängigkeit neu hinzugefügt.

## Paketbau und Prüfplan

1. Änderungen gezielt ohne die vier Lockdateien committen und auf `origin/main` sichern.
2. CAN aus genau diesem Commit exportieren; `SOURCE-SNAPSHOT.json`, vollständiges SHA-256-Quellenmanifest und Quellen-ZIP erzeugen. Motor-/libmpv-Bestand bytegleich erhalten.
3. Im exportierten Baum `tools/package-release.ps1` mit Buildnummer **29** und Candidate **RC3** ausführen: gesperrter Restore, Release, Tests, framework-dependent Publish, Inno-Setup und Portable-ZIP.
4. Sämtliche ZIP-Dateien gegen Publish-Manifest, App-/Motorversionen, Quellenzuordnung, native DLLs und sechs Binär-/Quellenartefakte samt SHA-256 prüfen. Neue Pakete unter `artifacts/release-build29-RC3`, lokale Nachweise unter `.build/freeze-build29-20261013`.
5. Paket-Prüfergebnisse dokumentieren und sichern. Danach persönliche Setup-/Portable-Prüfung; eine GitHub-Release-Veröffentlichung ist ein gesonderter Schritt.

Die öffentliche **0.2.1 · Build 16 · RC3**-Ausgabe bleibt unverändert. Der bekannte AVI-Zeitstempelbefund, gesperrte Frames ohne bestätigbare Zeitstempel und die dokumentierten MP4Box-Bild-/Zwei-AAC-Befunde bleiben erhalten. Beim Motor-Kontrollbuild wurde zuvor keine Byteidentität zur getesteten EXE nachgewiesen; bestehende Provenienz nicht als streng reproduzierbarer Binärbuild ausgeben.

## Abschluss

Quellcommit **ce4bc15b118d8080a78d7db5396db3ccf830ca89** auf origin/main gesichert. Setup, Portable, vier passende Quellen-/Relink-Archive und Prüfsummen erstellt; **1076/1076 Pakettests**, **249 Portable-Dateien** und **509 Quellen-ZIP-Dateien** geprüft. Versionsanzeige und sechs Masken-/Cutlist-Prüfungen mit entpacktem Paketcode erfolgreich. Vier lokale Produktions-Lockänderungen und Altartefakte erhalten. Der feste Quell-/Binärsatz bleibt unverändert; [abschließender Paketprüfbericht](RELEASE-BUILD29-RC3-RESULT.md). Persönliche Produktivinstallation sowie VM-Setup mit beiden Motoren/Deinstallation und Portable-Start/Pfade/AVI-Schnitt mit beiden Motoren am 13.10.2026 positiv bestätigt; gemeldete Paketabnahme abgeschlossen.

Am 13.10.2026 auf gesonderten Nutzerauftrag unverändert als öffentliche **0.3.0 · Build 29 · RC3**-Vorabversion veröffentlicht. Quelltag und alle sieben Uploads vor/nach Freigabe geprüft; [Nachweis](RELEASE-PUBLICATION-BUILD29-RC3.md). Build 16 und lokale Lockänderungen erhalten.
