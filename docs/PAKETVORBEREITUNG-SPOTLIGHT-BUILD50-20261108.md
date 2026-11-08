# CAN 1.0.3 Build 50 – Spotlight-Paketvorbereitung

**Paketprüfung 08.11.2026 abgeschlossen:** Quellcommit `185e8b3`, Setup/Portable/Quellen lokal geprüft und Quellstand gepusht. Build 50 unverändert; persönliche Setup-/Portable-Abnahme offen, keine Veröffentlichung. [Ergebnisse, Paketpfad und SHA-256](RELEASE-1.0.3-SPOTLIGHT-BUILD50-RESULT.md).

Stand 08.11.2026. Beauftragter Kandidat: **1.0.3 · Build 50 · Spotlight-Entwicklung**, Branch `codex/spotlight-integration`. Die Fensterverwaltung wurde am 07.11.2026 persönlich positiv bestätigt. Bisheriger Entwicklungsnachweis: 1773 CAN-Tests, 78 Spotlight-Tests, 32 native Integrationsprüfungen und vier getrennte Fensterprozesse erfolgreich; Release-Build ohne Warnungen/Fehler. Diese Zahlen ersetzen nicht die erneute Paketprüfung.

## Verbindlicher Umfang

Die Pakete enthalten nur den Spotlight-Stand dieses Zweigs einschließlich der Fensterpersistenz. Die separate Audio-Entwicklung Build 51 wird nicht übernommen. otr-can bleibt 0.1.6 mit unverändertem Normalmodus; MP4Box, CAN-Player und Cutlist-Logik werden für die Paketierung nicht geändert. Öffentlich stabil bleibt CAN 1.0.2 Build 48. Windows 11 x64 ist die persönlich geprüfte Zielplattform; Windows 10 ist nicht persönlich validiert.

## Paketverfahren

- Paketordner `artifacts/release-1.0.3-build50`; bisherige Pakete erhalten. Auf Nutzerwunsch heißen die Dateien wie gewohnt `CutAssistantNext-1.0.3-Build50-win-x64-Setup.exe`, `CutAssistantNext-1.0.3-Build50-win-x64.zip` und `CutAssistantNext-1.0.3-Build50-source.zip`, ohne Spotlight-Zusatz.
- CAN-Quellarchiv mit kompletter, fest versionierter Spotlight-Unterkomponente; zusätzlicher Spotlight-Quell-ZIP.
- Source-Snapshot und SHA-256-Dateimanifest; keine persönlichen Einstellungen, Filme, Git-Metadaten, Buildausgaben oder Zugangsdaten im Quellarchiv.
- Vorhandenes `tools/package-release.ps1` aus dem entpackten Snapshot verwenden: `-BuildNumber 50 -Candidate Spotlight-Entwicklung`. Das nummerierende `tools/build.ps1` wird für diesen Auftrag nicht ausgeführt. Build/Publish setzen die Nummer ausdrücklich und verändern den lokalen Zähler nicht.
- Frameworkabhängiger Windows-x64-Publish, Portable-ZIP und Inno-Setup-7-Installer aus genau demselben Publish-Ordner. Die technische Kandidatenkennung bleibt in Anwendung und Setup; die fertig geprüften Dateien werden ohne Inhaltsänderung nach dem üblichen CAN-Schema benannt und die Prüfsummenliste wird aktualisiert. Anwendung: Dateiversion 1.0.3.50.
- Unverändertes otr-can-0.1.6-Paket mit Quellen/Lizenzen und die bisherigen libmpv-0.41.0-Prüf-/Relinkarchive beilegen. Vier native libmpv-DLLs per SHA-256 verifizieren. Spotlight-Herkunft und Quellcommit in der Laufzeit-Provenienz führen.
- .NET 10 Desktop Runtime x64, FFmpeg/ffprobe, GPAC/MP4Box und gegebenenfalls FFMS2 sowie Visual-C++-Laufzeit bleiben separat einzurichten. Kein solches Werkzeug wird zusätzlich gebündelt.
- Alle CAN-/Spotlight-Suiten, native Integration und Kontrollplayer prüfen; Suiten nochmals mit finalen Publish-DLLs. Fensterpersistenz einschließlich getrennter Prozesse prüfen. Alle Archive, Quell-/Lizenzdateien, Portable-Einträge, Installer-Eingaben und SHA-256-Werte kontrollieren.

## Installation und persönliche Abnahme

Das Setup verwendet die bestehende CAN-Installationsidentität und kann eine vorhandene Installation aktualisieren. Deshalb wird es in diesem Auftrag nicht auf dem produktiven Arbeitsrechner installiert. Neue Installation, Start, Beenden, Deinstallation und Portable auf einem sauberen Windows-11-Testsystem bleiben offen, wenn eine solche Umgebung nicht bereitsteht. Portable in einen neuen Ordner entpacken, den kompletten Inhalt zusammenhalten und die Werkzeugpfade prüfen. Die persönliche Prüfung von Build 50 am 07.11.2026 betrifft die Entwicklungsfassung, nicht automatisch dieses neue Setup oder ZIP.

Spotlight-Quellen haben im derzeit eingebundenen Commit keine eigene LICENSE-Datei. [Herkunft und Abgrenzung](SPOTLIGHT-QUELLEN.md). Öffentliche Lizenzzuordnung bleibt vor einer Veröffentlichung zu klären; die lokale Paketvorbereitung und Branch-Sicherung sind beauftragt.

Commit und Push sind nach erfolgreicher Prüfung ausdrücklich freigegeben. Keine Tags, kein GitHub-Release, kein Merge nach main. Der abschließende Paketbericht ergänzt nach dem Bau Commit, Ergebnisse, Größen, Hashes und offene persönliche Prüfungen. Das Quellarchiv bildet seinen angegebenen Quellcommit ab; ein späterer reiner Ergebnisbericht ändert den paketierten Produktstand nicht.
