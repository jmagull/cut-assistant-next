# CAN 1.0.1 · Build 33 – lokaler Paketprüfbericht

Stand: **19.10.2026**. Versionswechsel und lokale Paketierung vom Nutzer ausdrücklich freigegeben. **Nicht veröffentlicht. Persönliche Setup-/Portable-/Windows-11-VM-Abnahme noch offen.**

## Ergebnis und Umfang

Setup, Portable und passende Quellen liegen unter `artifacts/release-1.0.1-build33`. Die neue MP4Box-Übergangsprüfung ist enthalten. Sie verwendet bestätigte Ausgabezeiten für Abschlussfenster und Kontrollplayer; bei nicht bestätigbarer Analyse bleibt der geschnittene Film erhalten und die Prüfstellen sind ausdrücklich berechnet. Schnittparameter und Cutlists bleiben erhalten.

**otr-can 0.1.5 ist unverändert.** Die Musikladen-Audiokorrektur ist nicht enthalten. Der bekannte MP4Box-Blitzer bei Radical und die experimentellen AVI-Grenzen bleiben bestehen. [Release Notes](RELEASE-NOTES-1.0.1.md), [MP4Box-Integrationsbericht](UEBERGANGSPRUEFUNG-INTEGRATION-20261018.md), [Musikladen-Diagnose](OTR-CAN-MUSIKLADEN-PCM-DIAGNOSE-20261019.md).

## Quellstand und reproduzierbarer Paketweg

Basis: Git-Commit **`fdc7e335acdaba5a9089bdc3429faebb433767b5`**, ergänzt um die freigegebenen lokalen Änderungen einschließlich der bereits integrierten MP4Box-Verbesserung. **Kein neuer Commit und kein neuer Tag.** Das Quellenarchiv ist ausdrücklich ein nicht committierter Arbeitsbaum-Snapshot, kein nachträglich behaupteter Release-Commit.

- Neue Quellkopie: `artifacts/source-1.0.1-build33/CutAssistantNext-1.0.1-Build33-source`.
- `SOURCE-SNAPSHOT.json` nennt Basiscommit, 43 lokale Ergänzungen/Änderungen, Motorversion und ausgeschlossene Lockdatei-Änderungen.
- **590 Quelldateien** im SHA-256-Manifest; **591 ZIP-Einträge** einschließlich des Manifests vollständig gegen die Quellkopie geprüft.
- Alle acht Lockdateien der Quellkopie stammen aus dem angegebenen Basiscommit. Die vier lokalen Produktions-Lockänderungen werden nicht übernommen; die Originaldateien bleiben vollständig erhalten.
- Restore mit `win-x64` und `--locked-mode` erfolgreich, ohne Lockdatei-Aktualisierung. Build, Tests und Publish laufen im selben eingefrorenen Quellstand.
- Setup und Portable verwenden denselben geprüften Publish-Ordner. Quellenmanifest vor/nach Paketierung und abschließend erneut geprüft.
- Das lokale `tools/build.ps1` wurde wegen seines automatischen Zählers nicht aufgerufen. Der Paketbuild erhält `CanBuildNumber=33` ausdrücklich; der lokale Zähler wurde erst nach bestandenem Build, Tests und nativer Paketprüfung von 32 auf **33** übernommen.

Der Kern des Paketlaufs aus der Quellkopie:

```powershell
dotnet restore .\CutAssistantNext.sln -r win-x64 --locked-mode
dotnet build .\CutAssistantNext.sln -c Release --no-restore -p:CanBuildNumber=33
dotnet test .\CutAssistantNext.sln -c Release --no-build --no-restore
dotnet publish .\src\CutAssistantNext.App\CutAssistantNext.App.csproj -c Release -r win-x64 --self-contained false -p:CanBuildNumber=33 -p:RestoreLockedMode=true -o <neuer Publish-Ordner>
```

Die vollständigen konkreten Paketparameter, Werkzeugpfade und der Quellen-ZIP-Pfad stehen in `.build/release-1.0.1-build33-20261019/PACKAGE-INPUTS.json`. Paketierung über die dort eingefrorene Fassung von `tools/package-release.ps1`, Inno Setup 7 und den vorhandenen geprüften libmpv-/Motorpaketsatz. Keine neue Produktionsabhängigkeit. Build-Reproduzierbarkeit bedeutet hier nachvollziehbarer Quell-/Abhängigkeits-/Paketstand; keine zusätzliche Zusage bitidentischer neu kompilierter EXEs.

## Technische Prüfungen

| Prüfung | Ergebnis |
|---|---|
| Gesperrter Restore | Alle acht Projekte erfolgreich |
| Release-Build | **0 Warnungen, 0 Fehler** |
| Core / Cutlists / Media / App | **199 / 72 / 224 / 890** |
| Gesamte Lösung | **1385/1385 bestanden**, keine fehlgeschlagenen/übersprungenen Tests |
| Finale Publish-App | **890/890 App-Tests erneut bestanden** mit den tatsächlich gepackten DLLs |
| Übrige 495 Tests | Core-/Cutlists-/Media-DLLs bytegleich zwischen Testlauf und Paket |
| Motor-Paketprüfungen | **10/10 bestanden** einschließlich Ablehnungen falscher Versionen, Quellen und Prüfsummen |
| Portable | **250 Dateien**, alle einzeln gegen Publish-Manifest und entpacktes ZIP geprüft; keine fehlenden/zusätzlichen Dateien |
| Quellen-ZIP | Alle 591 Einträge bytegleich zur eingefrorenen Quelle |
| Versionsanzeige | **Cut Assistant Next · 1.0.1 · Build 33**, keine RC-Kennung |
| Programmdateiversion | **1.0.1.33**, Produktversion **1.0.1+build.33** |
| Setup | Erfolgreich kompiliert, Produktversion 1.0.1; Installation/Deinstallation noch nicht persönlich geprüft |
| Native Bibliotheken | Vier geprüfte libmpv-DLLs; Prüfharness verwendet dieselben Paketbytes |
| Motor | EXE bytegleich zum bestehenden 0.1.5-Paket; Version und `mp3-aac-v1` bestätigt |
| Laufzeit/Lizenzen | Frameworkabhängige .NET-10-Desktop-Ausgabe, erforderliche Lizenztexte und Herkunftsnachweise vorhanden; keine externen Medienwerkzeuge eingebündelt |

### Echter synthetischer Schnitt und Kontrollplayer

Mit den entpackten finalen Programmdateien wurde der vorhandene synthetische Vier-Abschnitt-Film erneut über den regulären MP4Box-Dienst geschnitten. Ausschließlich neue Diagnoseausgabe unter `.build/`; keine Originalaufnahme oder vorhandene Filmausgabe ersetzt.

- Gemessene Übergänge **2.200 / 5.400 / 8.400 s** korrekt bestätigt.
- Kontrollplayer fordert **0 / 2.400 / 5.400 s** an: drei Sekunden Vorlauf, beim ersten Übergang auf den Dateianfang begrenzt.
- Drei reale HWND-/libmpv-Prüfungen mit stummer, versteckter Wiedergabe bestanden. Die neuen Abschnittsanfänge stimmen pixelweise mit den Quell-Referenzbildern überein, einschließlich doppelter PTS an Übergängen 2 und 3.
- Ausgabedateiidentität bestätigt, temporäre Segmente bereinigt und Player freigegeben.
- Das reguläre Abschlussfenster mit geprüften Zeiten und Bildverlust-Hinweis gerendert und visuell kontrolliert; Inhalt lesbar und scrollbar.
- Die 890 App-Tests umfassen die bestehenden Fehler-/Abbruch-/Dateiaustausch- und Ersatzzeitpfade. Keine neue persönliche Film-/Audio-/VM-Abnahme aus diesen technischen Prüfungen abgeleitet.

### Formatierung und transparente Abweichungen

Die Formatprüfung der geänderten C#-Integrationsdateien ist erfolgreich; `git diff --check` und die PowerShell-Syntaxprüfung bestehen. Der breite Formatcheck der gesamten Lösung meldet **bestehende Leerraum-/Encoding-Abweichungen in 46 unveränderten Dateien** (Exit 2). Keine davon gehört zu den lokalen Änderungen. Sie wurden für diesen Release-Schritt nicht großflächig umformatiert. Einzelheiten: `FORMAT-VERIFY.log`, `FORMAT-SCOPE.json`, `FORMAT-CHANGED-RESULT.json`.

Der erste Quellkopie-Helfer unterschied rohe Git-/Arbeitsbaumbytes und wertete normale CRLF-/LF-Unterschiede bei vier unveränderten Test-Lockdateien zunächst als zusätzliche lokale Änderungen. Die Prüfung stoppte vor der Paketierung. Der unvollständige Erstversuch liegt unter `source-attempt1`; anschließend wurden lokale Änderungen anhand des Git-Diffs klassifiziert und sämtliche Lockdateien aus Git exportiert. Der erfolgreiche zweite Snapshot enthält genau die vier ausdrücklich ausgeschlossenen lokalen Produktions-Lockänderungen. Kein ursprünglicher Lockinhalt wurde verändert und kein Build-/Testfehler übergangen.

## Pakete und SHA-256

Sechs Nutzdateien plus `SHA256SUMS.txt` bilden den lokalen Paketsatz. Die drei unveränderten Motor-/libmpv-Quellenarchive wurden aus dem bisherigen Release übernommen und sind bytegleich.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| `CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip` | 405926830 | `01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B` |
| `CAN-libmpv-v0.41.0-relink-2026-09-26.zip` | 47273515 | `00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342` |
| `CutAssistantNext-1.0.1-Build33-source.zip` | 3265778 | `8F31DAC223839A1FFAF68879AA6E45942C29430712D9D8528E299ACDD17D2E6F` |
| `CutAssistantNext-1.0.1-Build33-win-x64-Setup.exe` | 20335236 | `175FADBE1818131570C094CC352F69E64A45D5B348DD732802513D511EE5B50F` |
| `CutAssistantNext-1.0.1-Build33-win-x64.zip` | 25034010 | `514BE3A394BF18F8B633388353C753C24CD123B1727DE525D62A99CB7661D02E` |
| `otr-can-0.1.5-source.zip` | 63988 | `3A973278ABDF719B745405D9DFBB930C14A28CFEF40C7FB52A35CA17F7214E57` |

[Setup](../artifacts/release-1.0.1-build33/CutAssistantNext-1.0.1-Build33-win-x64-Setup.exe), [Portable](../artifacts/release-1.0.1-build33/CutAssistantNext-1.0.1-Build33-win-x64.zip), [CAN-Quellen](../artifacts/release-1.0.1-build33/CutAssistantNext-1.0.1-Build33-source.zip), [Prüfsummen](../artifacts/release-1.0.1-build33/SHA256SUMS.txt).

## Erhaltung und Änderungen dieses Paketschritts

Geändert: `Version.props` auf 1.0.1, README-Hinweis zur lokalen Paketvorbereitung und `tools/package-release.ps1` zur versionsabhängigen Auswahl der Release Notes mit früher Fehlermeldung bei fehlender Datei. Neu: `docs/RELEASE-NOTES-1.0.1.md`, dieser Ergebnisbericht und die separate VM-Übergabe.

**585 von 588 zuvor erfassten Bestandsdateien bytegleich**; nur die drei gerade genannten Bestandsdateien sind erwartungsgemäß geändert. Damit bleiben insbesondere die vorhandene MP4Box-Implementierung und alle acht ursprünglichen Lockdateien erhalten. Zählerstand nach erfolgreicher Prüfung ausdrücklich 33. Alte sechs Release-Nutzdateien erneut gegen ihr bestehendes Manifest geprüft, Tag `v1.0.0` und HEAD unverändert; Motorquellen und bestehendes Motorpaket nicht ersetzt. Keine Originalmedien, Cutlists, vorhandenen Filmausgaben oder persönlichen Einstellungen angepasst. Die Oberflächenprüfung verwendet einen eigenen Einstellungsstore unter `.build/`.

Kein Commit, Push, Tag oder Release. Dieser nachträgliche Paketbericht und die VM-Übergabe sind separate Prüfnachweise; sie ändern das bereits eingefrorene Quellenarchiv oder die Pakete nicht.

## Persönlich noch zu prüfen

[VM-/Portable-Übergabe](UEBERGABE-CAN-1.0.1-BUILD33-20261019.txt): Setup in Windows 11 x64, gegebenenfalls Upgrade über 1.0.0, Versionsanzeige, Standard-/Power-User-Ansicht, MP4Box-Übergänge samt Vorlauf und Ersatzzeithinweis, ein bestehender erfolgreicher otr-can-Fall, Deinstallation und Portable aus einem anderen Verzeichnis. Die Musikladen-Korrektur wird hier nicht erwartet.

Erst nach persönlicher Paketabnahme und gesonderter Freigabe werden Commit, Push und Veröffentlichung entschieden.

Lokale Detailnachweise: `.build/release-1.0.1-build33-20261019/` mit `PACKAGE.log`, `PACKAGE-INPUTS.json`, `PACKAGE-PROOF.json`, `VERSION-PROOF.json`, `MOTOR-PACKAGE-TESTS.log`, `FINAL-APP-TESTS.log`, nativen Bild-/Zeitnachweisen unter `synthetic-cut/`, Formatprotokollen und `FINAL.json`. Die Build-/Test-/Publish-/Installerlogs und TRX liegen auch im Paketausgabeordner.
