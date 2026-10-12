# Quellstand für CAN Build 16 RC3 gesichert – Schritt 2

Stand: 12.10.2026. Zwei feste lokale Quellcommits, vollständige Quellenarchive und ein vorbereitetes Motorpaket erstellt. Keine Git-Übertragung, Tags, Veröffentlichung, Setup- oder Portable-Pakete. Vorhandene Releases und ältere EXEs erhalten.

## Feste Quellen

| Bestandteil | Commit |
|---|---|
| CAN 0.2.1, Build 16, RC3 | 603d64d5c6374eea96ec5920719443fc6d399d81 |
| otr-can 0.1.5, nur can-engine | 2a474629836d52023bf37cbbdb309ccd1c23acf3 |

CAN-Commit umfasst gezielt 18 Dateien für AVI/MP3, Tests, Paketbau und Dokumentation. Die vier vorhandenen Produktions-Lockdateiänderungen sind ausgeschlossen. Motor-Commit umfasst sieben Dateien; Rust-Arbeitsbaum sauber. Keine Buildausgaben, Medien, Zugangsdaten oder lokalen Einstellungen eingecheckt.

Archive wurden direkt mit git archive aus diesen IDs exportiert, ohne Arbeitsbaumüberlagerung. SOURCE-SNAPSHOT.json erklärt Version, Commit, Build-/RC-Kennung und die vier ausgeschlossenen lokalen Lockdateien. SOURCE-FILES.sha256 erfasst alle Snapshot-Dateien. CAN enthält 464 manifestierte Dateien plus Manifest, Motor 25 plus Manifest. Im Motorarchiv sind ausschließlich die eigenständigen can-engine-Quellen enthalten; kein privater alter Decoder-/Vendor-/Referenzbaum.

Dieser Ergebnisbericht und die abschließenden Statusänderungen werden nach der Archivprüfung in einem zusätzlichen Dokumentationscommit gesichert. Sie gehören nicht zum bereits eingefrorenen CAN-Quellenarchiv. Für den Paketbau bleibt ausdrücklich der CAN-Quellcommit 603d64d maßgeblich; nicht automatisch der spätere Dokumentations-HEAD.

## Artefakte und Prüfsummen

| Archiv | Byte | SHA-256 |
|---|---:|---|
| CutAssistantNext-0.2.1-Build16-source-RC3.zip | 2841874 | 852DDC850B0F8B4DF0D1FF1BCA35E0FD3BAD5B992103FBDDC03D72317F1C697C |
| otr-can-0.1.5-source.zip | 63988 | 3A973278ABDF719B745405D9DFBB930C14A28CFEF40C7FB52A35CA17F7214E57 |

Lokale Ablage:

- CAN-Archiv: .build/quellstand-build16-20261012/CutAssistantNext-0.2.1-Build16-source-RC3.zip
- Exportierter CAN-Quellordner: artifacts/source-build16-RC3/CutAssistantNext-0.2.1-Build16-source-RC3
- Exportierte Motorquellen: artifacts/otr-can-0.1.5-source-RC3/otr-can-0.1.5
- Motorpaket für Schritt 3: artifacts/otr-can-0.1.5-package-RC3
- Motorquellenarchiv direkt darin: sources/otr-can-0.1.5-source.zip

Das Motorpaket enthält die zuvor getestete EXE unverändert, vollständige Quellen, MPL-/Autoren-/Herkunftshinweise, Abhängigkeits-Lizenztexte, Einrichtungshinweise sowie ENGINE-PROVENANCE.json und ENGINE-FILES.sha256. 100 manifestierte Dateien plus Manifest. Alle 40 Windows-Crate-Versionen und Lizenzdeklarationen stimmen mit dem bisherigen 0.1.4-Bestand überein; Rust-Standardbibliotheks-Hinweise beigelegt. FFmpeg, ffprobe, FFMS2 und Microsoft-VC-Laufzeit nicht beigefügt.

## Build- und Herkunftsnachweis

- Getestete und für das Paket erhaltene EXE: SHA-256 FC6ED6568880A995F3D141A9378AA2CE28ADE8DC0E257E93DC972799BB1324A6.
- Exportierte Motorquellen separat offline mit --locked gebaut, formatiert und getestet: Release-Build erfolgreich, **44/44 Tests**, 0 fehlgeschlagen/ignoriert. Kein Eingriff in die aktive EXE oder ältere Builds.
- Kontrollbuild-EXE: SHA-256 00BE8D879E008DA5EB680931C0F8FBEE49FF6286767113D7ADC83C39BAAD2BA6. Nicht bytegleich zur getesteten EXE; keine bytegenaue Reproduzierbarkeit behauptet. Das Motorpaket behält die getestete EXE. Beide Hashes und der erfolgreiche Quellbuild stehen in ENGINE-PROVENANCE.json.
- Alle 24 exportierten, ursprünglich versionierten Motor-Dateien stimmen nach Zeilenendennormalisierung mit dem sauberen, gesicherten Motorbaum überein.
- Letzter CAN-Build weiterhin **0.2.1 · Build 16 · RC3**, ohne Warnungen/Fehler, **793/793 Tests**. CAN-Anwendungscode in diesem Schritt unverändert; kein neuer CAN-Build und kein Produktions-Restore. Vollständiger Build aus dem CAN-Archiv erfolgt beim Paketbau in Schritt 3.

## Archiv- und Paketprüfung

- Beide ZIPs geöffnet, alle 465 beziehungsweise 26 Dateieinträge mit dem exportierten Snapshot bytegenau verglichen; vollständige Anzahl und Pfadgrenzen geprüft.
- Alle Quell- und Motorpaket-Manifeste verifiziert. Quellen- und EXE-Hashes stimmen mit ihrer Provenienz überein.
- Alle acht exportierten CAN-Lockdateien stimmen inhaltlich mit den Dateien des festen CAN-Commits überein. Git exportiert hier Windows-Zeilenenden; der rohe Git-Blob speichert LF. Diese reine Zeilenendendifferenz wurde beim Textvergleich normalisiert. Die eigentlichen Archiv-/Manifestprüfungen bleiben bytegenau.
- Alle acht lokalen CAN-Lockdateien per Ausgangs-SHA-256 unverändert. Genau vier vorbestehende src-Lockänderungen bleiben im Arbeitsverzeichnis erhalten; kein Reset/Stash/Restore dieser Dateien.
- Der tatsächliche Motorblock des eingefrorenen CAN-Paketskripts mit dem echten vorbereiteten Motorpaket ausgeführt: Version 0.1.5, MP3-Fähigkeit, Quell-/EXE-Provenienz und sämtliche Kopien erfolgreich geprüft. Dabei keine .NET-Wiederherstellung, Publish-Ausgabe oder Inno-Setup-Erstellung.

Der Prüfhelfer verglich die exportierten JSON-Lockdateien zunächst bytegenau mit LF-Git-Blobs; die legitime CRLF-Exportkonvertierung verursachte einen Fehlbefund. Nach gezielter Zeilenendennormalisierung war die Inhaltsgleichheit nachgewiesen. Ein nachfolgender Helfer prüfte irrtümlich LASTEXITCODE nach einem reinen PowerShell-Skript; dieser ungeeignete Exitcode-Check wurde entfernt. Keine Quelldatei, Prüfsumme oder funktionale Schutzprüfung dafür abgeschwächt.

Nachweise: .build/quellstand-build16-20261012/{BEFORE,PREPARED,SOURCE-VERIFY,MOTOR-SOURCE-VERIFY,PACKAGE-INPUTS}.json, SOURCE-ARCHIVES.sha256, MOTOR-SOURCE-BUILD.log, MOTOR-SOURCE-TESTS.log und actual-motor-check/RESULT.json.

## Nächster Schritt

Schritt 3: Setup und Portable aus dem vorbereiteten CAN-Quellordner bauen, BuildNumber 16 und Candidate RC3 explizit übergeben, das vorbereitete 0.1.5-Motorpaket und den vorhandenen geprüften libmpv-Bestand verwenden. PACKAGE-INPUTS.json hält die lokalen Eingabepfade bereit. Der spätere Paket-Ausgabeordner artifacts/release-build16-RC3 ist noch nicht erstellt.

Die positive persönliche Teufelsbrigade-Abnahme bleibt gültig. Der vorbestehende technische Video-PTS-Befund bleibt dokumentiert. Neue Installations-/Portable-Abnahme und Veröffentlichung sind separate Schritte.