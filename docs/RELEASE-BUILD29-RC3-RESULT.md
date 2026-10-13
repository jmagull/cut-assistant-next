# CAN 0.3.0 · Build 29 · RC3 – eingefrorener Paketsatz

Stand: 13.10.2026. Auf Nutzerauftrag Commit/Push durchgeführt und Setup, Portable-ZIP sowie passende Quellen-/Prüfsummenartefakte erstellt. **Quellstand fest eingefroren; automatische Paketprüfungen erfolgreich. Produktivinstallation und Schnittprüfung am 13.10.2026 vom Nutzer positiv bestätigt. Separate VM-/Portable-Prüfungen und Deinstallation dieser neuen Ausgabe noch offen.** Kein neues GitHub-Release erstellt; öffentlicher Build 16 unverändert.

## Fester Quellstand

CAN-Quellcommit: **ce4bc15b118d8080a78d7db5396db3ccf830ca89**, auf `origin/main` gepusht und gegen den Remote-Hash geprüft. Der CAN-Quellen-Snapshot und die Paketprovenienz verweisen auf exakt diesen Commit, ohne Arbeitsbaumüberlagerung. Spätere Ergebnisdokumentation wird in einem eigenen Commit gesichert und verändert diesen eingefrorenen Quell-/Binärsatz nicht.

CAN **0.3.0 · Build 29 · RC3**, App-Dateiversion **0.3.0.29**, Assemblyversion **0.3.0.0**, Informationsversion **0.3.0+build.29** und Kandidat **RC3**. Vier vorbestehende Produktions-Lockdateiänderungen bleiben außerhalb von Commit und Quellarchiv erhalten; der Snapshot verwendet die versionierten Lockdateien.

Motor **otr-can 0.1.5**, Quellencommit **2a474629836d52023bf37cbbdb309ccd1c23acf3**. Getestete Motor-EXE bytegleich erhalten (**FC6ED6568880A995F3D141A9378AA2CE28ADE8DC0E257E93DC972799BB1324A6**). Quellen, MPL-/Autoren-/Abhängigkeitshinweise und bestehende Provenienz vollständig beigefügt. Letzter unveränderter nativer Quellenprüfstand **44/44 Rust-Tests**; kein neuer Rust-Build erforderlich oder behauptet, keine bytegenaue Reproduzierbarkeit der früheren Kontroll-EXE behauptet.

libmpv-0.41.0-DLLs und passende Quellen-/Relink-Archive unverändert übernommen. Keine externen FFmpeg-/ffprobe-/MP4Box-/FFMS2-Programme oder private .NET-Laufzeit gebündelt. Die .NET 10 Desktop Runtime und übrigen dokumentierten Voraussetzungen bleiben separat erforderlich.

## Build und automatische Prüfungen

- Build aus dem Git-Export mit dessen `tools/package-release.ps1`, ausdrücklich BuildNumber **29** und Candidate **RC3**; lokaler Zähler bleibt 29.
- Gesperrter Windows-x64-Restore, Release-Build, Tests, framework-dependent Publish und Inno-Setup-Kompilierung erfolgreich; **0 Warnungen/0 Fehler**, **1076/1076 CAN-Tests bestanden**, keine übersprungen (Core 127, Cutlists 72, App 726, Media 151).
- **249 Portable-Dateien** nach neuer Entpackung vollständig per SHA-256 gegen `PUBLISH-FILES.sha256` geprüft; Dateianzahl und Pfadgrenzen stimmen.
- **509 Quellen-ZIP-Dateien** gegen den exportierten Snapshot bytegenau geprüft (508 manifestierte Dateien plus Manifest). Alle manifestierten Quellen nach Build/Publish unverändert.
- Quell-/Build-/RC-/Motorzuordnung in `SOURCE-SNAPSHOT.json` und `RELEASE-PROVENANCE.json` geprüft. Apphost und alle vier Produktionsassemblies tragen die erwarteten Versionswerte; tatsächlicher kompilierter WPF-Fenstertitel und Hauptüberschrift **Cut Assistant Next · 0.3.0 · Build 29 · RC3** geprüft, Hauptfenster nicht angezeigt und keine persönlichen Einstellungen gespeichert.
- **Sechs WPF-/Speicherprüfungen mit bytegleichen Kopien der entpackten Paketassemblies**: echte Masken-Speichern-/Neuerzeugen-Schaltflächen, S/E↔x ohne Neustart, normale/kleine Cutlist-Ansicht, Vorschau-/Kontexterhaltung und isoliertes Speichern/Wiederladen mit unveränderten Schnittbereichen. Keine Bindungsfehler. Keine neue persönliche Film-/Ton-/Installationsabnahme daraus abgeleitet.
- Tatsächlich gepackte Motor-EXE mit `--version` und `capabilities` geprüft: **otr_can 0.1.5 / mp3-aac-v1**. Kopierte native DLLs, Quellen und Motorbestand per Hash erhalten.
- **Alle sechs Binär-/Quellenartefakte** erneut gegen `SHA256SUMS.txt` geprüft. Setup und ZIP stammen aus demselben Publish-Verzeichnis. Setup-Kompilierung erfolgreich; daraus keine Setup-Installation oder vollständige Installationspayload-Abnahme behauptet. Der Inno-Bootstrapper hat wie der vorherige Build-16-Installer leere Versionsstrings/numerisch 0.0.0.0; die Paket-/Anwendungszuordnung wird über Compilerparameter, Dateinamen, Herkunft und geprüfte App-Versionen belegt.
- Alle acht lokalen Lockdateien, erfasste persönliche Einstellungsdateien und bestehende Build-16-Artefakte samt Prüfsummen unverändert. Aktive Anwendung und Einstellungen nicht ersetzt oder installiert.

Der Prüfhelfer las zunächst TRX-Dateinamen mit eckigen Klammern als Wildcardpfade; auf literal gelesene Pfade korrigiert, vier vollständige Testresultate erfolgreich ausgewertet. Keine Pakete oder Tests dafür verändert; ursprüngliche Build-/Testlogs erhalten.

## Dateien und Prüfsummen

Ablage: **artifacts/release-build29-RC3**. Zusammengehörige Downloads:

| Datei | Byte | SHA-256 |
|---|---:|---|
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-0.3.0-Build29-source-RC3.zip | 2990238 | F719C821F25B10C5E875AFB82AC3AF1C8C3DBE782BE5314C976E1AB36DF9FE30 |
| CutAssistantNext-0.3.0-Build29-win-x64-RC3.zip | 24941763 | E98772971BA95983F75846023D9DE70AA2D72CB09DF591A85F26BB833CF4EBD2 |
| CutAssistantNext-0.3.0-Build29-win-x64-Setup-RC3.exe | 20270346 | A63F194772283735EAC7DBE0757430E9528316526DD6E4C7FBDAF99DB9D54FBE |
| otr-can-0.1.5-source.zip | 63988 | 3A973278ABDF719B745405D9DFBB930C14A28CFEF40C7FB52A35CA17F7214E57 |

`SHA256SUMS.txt` erfasst diese sechs Artefakte. `PUBLISH-FILES.sha256` und die im Portable enthaltene `RELEASE-PROVENANCE.json` dokumentieren die vollständige Payload und Quellenzuordnung. `RELEASE-NOTES.md` ist eine nachträgliche Beschreibung; sie gehört nicht zum eingefrorenen Quellenarchiv und verändert keine Paketdatei.

## Persönliche Produktivinstallation und Schnittprüfung – 13.10.2026

Nutzer bestätigt **„1) Installation Produktiv klappt check / 2) Schnitt check siehe Bild :-)“** für den aktuellen Build-29-Paketsatz. Installation in der produktiv genutzten Umgebung und anschließende Schnittprüfung damit persönlich positiv bestätigt. Frische Installation oder Überinstallation nicht eigens unterschieden; keine zusätzliche unabhängige Installation-Datei-/Versionsprüfung behauptet.

Das bereitgestellte Bild der eigenen Cutlist-Uploads dokumentiert:

- **Torchwood / Sünden der Vergangenheit / S4E7:** mit CAN geschnitten, Werbung vollständig entfernt, **MP4Box aus GPAC 26.07** verwendet. Vorgeschlagener Dateiname **Torchwood S04E07 Sünden der Vergangenheit [12.10.2026]**. Erfolgreicher MP4Box-Praxisschnitt laut Nutzer und Cutlist-Vermerk; kein sichtbarer zusätzlicher Fehlerhinweis für diesen Eintrag.
- **Torchwood / Befreiungsaktion / S4E6:** **doppelte Szene bei 15:05 mit MP4Box** berichtet. Übergang mit der **Frame-Lupe framegenau bestimmt**, mit **otr-can 0.1.5 gelingt der Schnitt**. Vorgeschlagener Dateiname **Torchwood S04E06 Befreiungsaktion [12.10.2026]**. Positive otr-can-/Lupenprüfung; MP4Box-Bildbefund separat erhalten.
- Der bereits dokumentierte **Chicago-Fire-/Unantastbar**-Vergleich bleibt sichtbar: MP4Box-Werbeblitzer am Anfang auch nach Lupenkorrektur, otr-can blitzerfrei, Bild/Ton/alle Übergänge geprüft. Diese Werte bleiben dem bisherigen Fall zugeordnet.

Kein neues natives Schnittprotokoll, keine Sample-/Paket-/Laufzeitwerte und keine gesonderte Tonspur-/Synchronitätsabnahme für die beiden Torchwood-Dateien in dieser Nachricht. Keine neuen Messungen aus dem Screenshot abgeleitet. Bild lokal unter `.build/freeze-build29-20261013/user-acceptance/productive-install-and-torchwood.png` gesichert. Keine Serveraktion ausgeführt.

Produktiv-Setup-/Schnittabnahme damit ergänzt. **Offen:** separate Portable-/VM-Prüfungen, Installationsart/Überinstallation nach Bedarf, Deinstallation in geeigneter Testumgebung und ggf. frisches Benutzerprofil. Eingefrorene Quellen-/Binärarchive und Prüfsummen unverändert.

## Grenzen und nächste Prüfung

Die Entwicklungsabnahmen gelten weiter: AVI-Lupe, Richtungswechsel-Halbsprung, Versionskontrolle, Benennung und gespeicherter Maskenwechsel ohne Neustart persönlich bestätigt. Chicago Fire mit otr-can bild-/ton-/übergangsgeprüft blitzerfrei; MP4Box-Blitzer am Anfang auch nach Lupenkorrektur dokumentiert. Künstlicher Zwei-AAC-MP4Box-Befund und AVI-Zeitstempelgrenzen bleiben separat erhalten.

Produktivinstallation und beide Schnittverfahren sind oben persönlich bestätigt. Nächster Schritt: separate Portable-/VM-Paketprüfung und ggf. Überinstallation/Deinstallation in geeigneter Testumgebung; dabei Versionsanzeige, Pfade, Motoren und Ergebnis kontrollieren. Keine Aussage über frisches Benutzerprofil oder VM dieser Ausgabe bisher bestätigt. GitHub-Veröffentlichung bleibt ein gesonderter Auftrag.

Lokale Nachweise: `.build/freeze-build29-20261013` mit Commit-/Remote-Nachweis, Paket-/Quellen-/Erhaltungsprüfungen und WPF-Belegen; Build-/Restore-/Publish-/Installer-Logs und TRX-Ergebnisse unter `artifacts/release-build29-RC3`.
