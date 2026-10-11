# CAN 0.2.1 Build 13 – lokaler RC3-Paketsatz

Stand: 11.10.2026. **Setup, Portable-ZIP und passende Quellen erstellt und automatisch geprüft. Noch nicht veröffentlicht; keine neue persönliche Paketabnahme für Build 13.** Die vollständige persönliche [Build-12-Abnahme](RELEASE-BUILD12-RC3-RESULT.md) bleibt ihrem unveränderten Paketsatz zugeordnet. Öffentliche Vorabversion bleibt Build 7 RC2.

## Quellstand und Änderung gegenüber Build 12

Gebaut aus dem lokalen Commit **c98041a066685414481ac46a3a6af8d105b085bd**, ohne Arbeitsbaumüberlagerung. SOURCE-SNAPSHOT.json nennt Commit, Version 0.2.1, Buildnummer 13 und RC3; SOURCE-FILES.sha256 umfasst **459 Dateien**, einschließlich dieser Snapshot-Metadaten. Das Manifest selbst wird durch den SHA-256-Wert des gesamten Quellarchivs erfasst.

Die neue funktionale Änderung gegenüber dem Build-12-Paketsatz ist der statische Abschnitt **KI-unterstützte Entwicklung** im Credits-Dialog, mit dem unveränderten Wortlaut aus DANKSAGUNG.md. Hinzu kommen der Abgleich alter Aufgabenlisten und die Einordnung historischer Release-/Lizenzunterlagen. AVI-Hinweis und Motorpaketierung sind jetzt ebenfalls im festen Commit gesichert; sie waren bereits im Build-12-Quellarchiv enthalten.

Vier vorbestehende lokale src-Lockdateiänderungen bleiben außerhalb des Commits und des Quellarchivs. Gesperrter Paketbau verwendet die eingecheckten Fassungen. Alle acht ursprünglichen Lockdateien sowie sämtliche Dateien auf oberster Ebene des Build-12-Releaseordners wurden vor/nach der Arbeit per SHA-256 verglichen und bleiben unverändert. Lokaler Buildzähler weiterhin 13; keine neue funktionale Versionsnummer.

Dieser Prüfbericht und die darauf verweisenden Statusaktualisierungen entstanden nach dem Paketbau. Sie sind ergänzende Nachweise und nicht Teil des unveränderten Quellarchivs. Maßgeblich für die Pakete ist der oben genannte Quellcommit; sein Dokumentationsstand beschreibt die damals noch laufende Vorbereitung.

## Paketform und unveränderte Bestandteile

Frameworkabhängige Windows-11-x64-Ausgabe: .NET 10 Desktop Runtime separat installieren; vier verifizierte libmpv-0.41.0-DLLs enthalten. FFmpeg, ffprobe, GPAC/MP4Box und FFMS2/ffmsindex werden separat eingerichtet. Keine gebündelte .NET-Laufzeit, kein DirectShow oder Windows-Codec-Paket.

Optionaler Motor **otr-can 0.1.4** in beiden Binärpaketen unter tools/otr-can/otr_can.exe. Original-EXE unverändert: SHA-256 **CDC9E9E3317F8CC4E9512249F28399DE4CCDFCE73491C4DF109A8E5333CD8C2D**. Motorquellen aus Commit **8831021ae0e412b122b7db85583a79039f642343**, Quell-ZIP direkt im Motorunterordner und zusätzlich im Releaseordner. MPL-/Autorenhinweise, Lizenzbestände für 40 Windows-Fremd-Crates und Rust-Standardbibliotheks-Hinweise unverändert beigelegt. Für den Motor ist Microsoft Visual C++ v14 Redistributable x64 separat erforderlich. Kein Rust-Neubau in dieser Aufgabe; letzter bestätigter nativer Prüfstand **40/40 Tests**.

Unveränderte libmpv-Quellen-/Relink-Archive aus RC2 im Releaseordner enthalten und erneut gegen die bekannten Prüfsummen geprüft. CANs LICENSE, NuGet-/Desktop-Runtime-Hinweise und libmpv-Drittanbieterhinweise liegen bei. Die Komponentenbedingungen werden durch diese Paketprüfung nicht neu bewertet.

## Automatische und lokale Prüfungen

- Gesperrter Windows-x64-Restore aus dem Quellarchiv erfolgreich. Quellmanifest vor/nach dem Paketbau unverändert.
- Release-Build und Publish erfolgreich, Build **0 Warnungen/0 Fehler**; **785/785 CAN-Tests**, kein übersprungener Test: Core 74, Cutlists 65, Media 125, App 521. .NET SDK 10.0.303; Inno Setup 7 erstellt das Setup erfolgreich.
- **249 Dateien** im vollständig neu entpackten Portable-ZIP gegen PUBLISH-FILES.sha256 geprüft, keine Zusatzdateien. RELEASE-PROVENANCE.json bestätigt Quellarchivhash, festen Commit, Build 13 und RC3. Vier native DLLs, separat zu installierende Werkzeuggrenzen, Runtime-Konfiguration und notwendige Lizenzhinweise vom Paketierungsskript geprüft.
- Sechs Release-Archive/-Programme in SHA256SUMS.txt erfasst: Setup, Portable, CAN-Quellen, Motorquellen, libmpv-Quellen und Relink-Paket. Motor-EXE, Motorquellen und beide libmpv-Archive bytegleich zu den bisherigen geprüften Beständen.
- Echter Credits-Dialog aus entpackter CAN-Assembly bei **780 × 720 und 540 × 400** gerendert und visuell geprüft. KI-Absatz exakt gleich DANKSAGUNG.md; vollständiger Textumbruch, scrollbar, Schließen erreichbar, keine Bindungsfehler. Bestehende OTR-Projektlinks und Navigationshandler weiterhin geprüft.
- Isolierter WPF-Hauptfenstertest mit entpackten CAN-Assemblies und mitgelieferten echten libmpv-DLLs: Titel **0.2.1 · Build 13 · RC3**, erfolgreiche native Playerinitialisierung, sichtbarer AVI-Hinweis und reguläres asynchrones Schließen. Nur isolierte Fensterdaten geschrieben; Prüfbild gesichtet. Dieser Helfertest bestätigt die Paketkomponenten, nicht den unveränderten CAN-Apphost-Start oder eine VM-Installation.
- Kurzer künstlicher Schnitt mit zwei Behaltebereichen über entpackte CAN-Services und beigelegten Motor: **90 tatsächlich decodierte Videobilder**, AAC-Stereo und AC-3/5.1, erfolgreiche vollständige Decoderprüfung. Genau eine Indexierung und ein Motoraufruf; Original-/Indexdaten unverändert, Arbeitsordner bereinigt. Pfade mit Leerzeichen/Umlauten und Ausführung ohne FFmpeg im Prozess-PATH geprüft; keine persönlichen Einstellungen geschrieben.
- Künstliche Schnittausgabe bytegleich zum Build-12-Pakettest: SHA-256 **E7BABCB25806D93714FE2B97931D4A4783E9F89BA109CD4B7F031A45781579EB**. Lauf rund **2,9 s**; kein Realfilm-Benchmark und keine neue persönliche Tonprüfung.

Prüfbelege: artifacts/release-build13-RC3/logs, test-results, PUBLISH-FILES.sha256 und .build/release-build13-RC3 (Snapshot, vollständige ZIP-Prüfung, Credits-/Hauptfensterbilder, Schnittprotokoll, Medien-/Decoderprüfung, Ausgangshashes). Ein fehlerhaft geschriebener erster Aufruf des lokalen Quellvorbereitungshelfers und ein erster Medienwerkzeugaufruf mit falschem Pfad wurden korrigiert; dabei wurde jeweils kein Programm ausgeführt. Kein Produktcode oder Test zur Umgehung eines Fehlers geändert.

## Persönlicher Abschluss und Veröffentlichung

Für diesen neuen Paketsatz liegt noch kein persönlicher Setup-/Portable-Test vor. Ein kurzer Bestätigungslauf kann sich auf Überinstallation/Versionsanzeige **Build 13 RC3**, den neuen KI-Abschnitt unter **Credits**, regulären Start/Schließen und einen Schnitt konzentrieren. Die dokumentierte vollständige Build-12-Abnahme wird dadurch nicht umetikettiert.

Kein Push, Tag, GitHub-Release oder Versionssprung auf 1.0 vorgenommen. Nächster Schritt: persönlichen Paketcheck festhalten und anschließend die Veröffentlichung dieses konkreten Paketsatzes beauftragen.

## Reproduzierbarer lokaler Paketbau

CAN-Quellarchiv vollständig entpacken. Vier-DLL-libmpv-Ordner gemäß dessen Dokumentation bereitstellen; Motorpaket aus tools/otr-can des Portable-ZIPs verwenden; Inno Setup bereitstellen. Aus dem entpackten Quellordner:

```powershell
.\tools\package-release.ps1 -BuildNumber 13 -Candidate RC3 `
    -NativeSourceDirectory '<vier-DLL-Ordner>' `
    -SourceArchivePath '<CAN-Quell-ZIP>' `
    -OutputDirectory '<neuer Ausgabeordner>' `
    -InnoCompilerPath '<ISCC.exe>' `
    -OtrCanPackageDirectory '<tools\otr-can aus dem Portable-ZIP>'
```

Das Skript prüft Quellen-/Motor-Manifeste, führt Restore, Build, Tests und Publish durch und erstellt Setup und ZIP. Die unveränderten libmpv-Quellen-/Relink-Archive sowie das zusätzliche Motorquell-ZIP anschließend dem Releaseordner beilegen und den vollständigen SHA256SUMS.txt-Satz erstellen. Keine bitidentische Reproduzierbarkeit beliebiger Neubauten behauptet.

## Pakete und Prüfsummen

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-0.2.1-Build13-source-RC3.zip | 2817027 | 7D3092942E65D89CE7A616A81367E6704504D963512D1510D96B1C3C990CCD54 |
| CutAssistantNext-0.2.1-Build13-win-x64-RC3.zip | 24881639 | 658956EE1BB91664AA81998A75801454E9FF3A67C2FAA62AEC4CDBF7A43431A7 |
| CutAssistantNext-0.2.1-Build13-win-x64-Setup-RC3.exe | 20215316 | EC59B7401FFE3AC1ABAFF11B5959C967A2C294AE64B1D29510CE90642893E80D |
| otr-can-0.1.4-source.zip | 61927 | B92DE748E1C5469AF2C28E5C789BDF417A333C0FC9129D73927C79DAF369508B |
