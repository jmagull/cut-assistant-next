# CAN 1.0.1 · Build 34 – lokaler Paketprüfbericht

Stand: **19.10.2026**. Vom Nutzer ausdrücklich beauftragt: otr-can 0.1.6 in Setup, ZIP und zugehörige Quellen integrieren. **Pakete lokal fertig; nicht veröffentlicht. Nachtrag 20.10.2026: Setup auf Arbeitsrechner und in der VM, VM-Deinstallation und Portable in der VM persönlich erfolgreich geprüft. Die vorgesehenen persönlichen Pakettests sind abgeschlossen.**

## Umfang und Ergebnis

**CAN 1.0.1 · Build 34 enthält otr-can 0.1.6**, einschließlich der technisch und persönlich geprüften Musikladen-AAC-Korrektur. Setup und Portable verwenden denselben geprüften Publish-Ordner. Die bestehende MP4Box-Ausgabeübergangsprüfung aus Build 33 ist erhalten. Die bekannten MP4Box-Blitzer bei Musikladen und Radical bleiben bestehen.

Die verteilte Motor-EXE ist bytegleich zur persönlich mit Musikladen und Navy CIS geprüften 0.1.6-EXE: SHA-256 `73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E`. Keine neue Kompilierung wird als persönlich geprüfte EXE ausgegeben. CLI-Version `otr_can 0.1.6`, Fähigkeiten `mp3-aac-v1 aac-overlap-v1` in Publish und entpacktem ZIP bestätigt.

[Release Notes](RELEASE-NOTES-1.0.1.md), [Motor-Integration und persönliche Filmprüfungen](OTR-CAN-0.1.6-INTEGRATION-20261019.md), [VM-/Portable-Übergabe](UEBERGABE-CAN-1.0.1-BUILD34-20261019.txt).

## Persönliche Setupprüfung auf dem Arbeitsrechner – 20.10.2026

Der Nutzer bestätigt den Test des normalen Setups von CAN 1.0.1 · Build 34 auf seinem Arbeitsrechner mit zwei Aufnahmen:

| Fall | Motor laut Protokoll | Persönliche Rückmeldung |
|---|---|---|
| Vera – Ein ganz spezieller Fall: Die steigende Flut | MP4Box, vier Abschnitte | Bild, Ton, Schnitt und 3-Sekunden-Vorschau erfolgreich geprüft |
| Musikladen | otr-can 0.1.6 | Bild, Ton, Schnitt und 3-Sekunden-Vorschau erfolgreich geprüft; kein NDR-Blitzer |

Das Vera-Protokoll endet nach Zusammenfügen und Schnittstellenprüfung mit „Fertig.“. Gleiche Bildzeiten an den ersten beiden Übergängen werden ausdrücklich durch die Prüfung des dekodierten neuen Abschnittsanfangs behandelt. Das Musikladen-Protokoll bestätigt Motor 0.1.6, die 34 gezielten AAC-Konfliktkorrekturen, 168760320 dekodierte Audiosamples mit kontinuierlichen PTS, die finale native MP4-Prüfung und den erfolgreichen CAN-Abschluss. Die Sicht-/Hör- und Vorschauabnahme beruht auf der ausdrücklichen persönlichen Rückmeldung des Nutzers; die Textprotokolle allein würden diese nicht belegen.

Beide Originalprotokolle sind lokal unter `.build/release-1.0.1-build34-20261019/personal-acceptance-20261020` gesichert. **Damit ist der normale Setup-Praxistest auf dem Arbeitsrechner für diese beiden Fälle positiv bestätigt.** Diese Arbeitsrechner-Rückmeldung allein bestätigt keine VM-, Portable-, Deinstallations- oder gesonderte Upgrade-Abnahme. Die anschließenden VM-Rückmeldungen sind im folgenden Abschnitt getrennt dokumentiert. Keine Freigabe für Commit, Push, Tag oder Veröffentlichung erteilt. Ausschließlich Dokumentation ergänzt; Programme, Quellenarchive und Paketdateien unverändert.

## Persönliche VM-Setupprüfung und Deinstallation – 20.10.2026

Der Nutzer bestätigt das Setup in seiner VM mit **MP4Box und otr-can**: Bild, Ton, Schnitte und 3-Sekunden-Vorschau erfolgreich geprüft. Die zwei übermittelten Beispiele sind:

| Fall | MP4Box | otr-can 0.1.6 |
|---|---|---|
| Grey’s Anatomy – Durchs Feuer, HQ-MP4 vom 30.09.2026 | Drei Abschnitte, finale Schnittstellenprüfung und „Fertig.“ | Drei Abschnitte, 60651 geprüfte Videopakete, 116449920 Audiosamples bei 48 kHz mit kontinuierlichen PTS, finale MP4- und CAN-Ausgabeprüfung erfolgreich |
| Die Teufelsbrigade, ursprüngliche HQ-AVI vom 21.01.2021 | Stream-Copy-Vorbereitung nach MP4, sechs Abschnitte, finale Schnittstellenprüfung und „Fertig.“ | Stream-Copy-Vorbereitung mit erhaltenem MP3-Ton, sechs Abschnitte, 182574 geprüfte Videopakete, 350542080 Audiosamples bei 48 kHz; MP3 nach PCM-Schnitt einmal nach AAC-LC kodiert, native MP4- und CAN-Ausgabeprüfung erfolgreich |

Das im Chat übermittelte Grey’s-Anatomy-OTR-Protokoll weist ausdrücklich Motor 0.1.6 aus. Seine Keeps `[592.56,1759.16)`, `[2314.20,3005.48)` und `[3565.52,4133.68)` ergeben unabhängig nachgerechnet **2426.040 s = 00:40:26.040**, exakt 60651 Bilder bei 25 fps und 116449920 Samples bei 48 kHz. Auch die Teufelsbrigade-Zahlen stimmen überein: **7302.960 s = 02:01:42.960**. Diese Zahlen belegen nicht von sich aus eine vollständige Videodekodierung; die regulären Protokolle nennen Paket-/PTS-/DTS-Prüfungen.

Beide AVI-Vorbereitungsprotokolle enthalten die bekannten FFmpeg-Hinweise `Timestamps are unset` / `pts has no value`, einschließlich Wiederholungen. Die Warnungen bleiben als Befund erhalten; die Vorbereitung wird nicht als warnungsfrei bezeichnet. Die positiven Sicht-/Hör-/Schnitt-/Vorschauprüfungen stammen aus der ausdrücklichen Nutzerbestätigung. AVI bleibt weiterhin experimentell.

Anschließend bestätigt der Nutzer ausdrücklich: **„VM: setup: deinstallation check“**. Damit sind **VM-Setup und VM-Deinstallation persönlich positiv bestätigt**. Eine separate Upgrade-Abnahme wurde nicht ausdrücklich genannt. Zum Zeitpunkt dieser VM-Setup-Rückmeldung war Portable noch offen; die anschließende positive Portable-Abnahme ist im nächsten Abschnitt dokumentiert.

Die drei Anhänge sind bytegleich unter `.build/release-1.0.1-build34-20261019/personal-acceptance-20261020` gesichert: `vm-greys-mp4box.log`, `vm-teufelsbrigade-mp4box.log`, `vm-teufelsbrigade-otr-can.log`. Das zusätzliche Grey’s-Anatomy-OTR-Protokoll stammt aus dem Nachrichtentext; `VM-ACCEPTANCE.json` nennt diese Herkunft und die nachgerechneten Werte. `CURRENT-ACCEPTANCE-STATUS.json` führt den Gesamtstand zusammen. Programme und Pakete unverändert; keine Veröffentlichung beauftragt.

## Persönliche Portable-Abnahme in der VM – 20.10.2026

Der Nutzer bestätigt **„VM zip: otr-can Pfad umgetragen“**, die erfolgreiche Prüfung von **Bild, Ton, Schnitt und 3-Sekunden-Vorschau** sowie abschließend **„zip insgesamt check“**. Damit ist die vorgesehene persönliche Portable-Prüfung ebenfalls positiv abgeschlossen.

- **Grey’s Anatomy – Durchs Feuer mit otr-can:** Das im Nachrichtentext übermittelte Protokoll bestätigt Motor **0.1.6**, drei Keeps, **60651 Videopakete** mit geprüften PTS/DTS, **116449920 dekodierte Audiosamples bei 48 kHz** mit kontinuierlichen PTS sowie erfolgreiche native MP4- und CAN-Ausgabeprüfungen. Die persönliche Bild-/Ton-/Schnitt-/Vorschauprüfung wird ausdrücklich bestätigt.
- **Teufelsbrigade-AVI mit MP4Box:** Der angehängte Lauf zeigt verlustfreie MP4-Vorbereitung, sechs Schnittabschnitte, Zusammenfügen und vollständige Schnittstellenprüfung mit Abschluss **„Fertig.“**. Die bekannten FFmpeg-Zeitstempelhinweise der AVI-Vorbereitung bleiben im Originalprotokoll erhalten. Der Nutzer bestätigt den ZIP-Test insgesamt positiv.

Originalanhang bytegleich lokal gesichert als `.build/release-1.0.1-build34-20261019/personal-acceptance-20261020/portable-teufelsbrigade-mp4box.log`. `PORTABLE-ACCEPTANCE.json` unterscheidet den Inline-OTR-Nachweis und den MP4Box-Anhang; `CURRENT-ACCEPTANCE-STATUS.json` führt den Gesamtstand zusammen.

**Gesamtstand: normales Setup auf dem Arbeitsrechner, VM-Setup mit beiden Motoren, VM-Deinstallation und Portable in der VM persönlich positiv bestätigt.** Die vorgesehenen persönlichen Pakettests sind abgeschlossen. Eine separate Upgrade-Abnahme wird weiterhin nicht zusätzlich behauptet. Keine neue Paketierung und keine Änderung an Programm, Motor, Filmdateien oder vorhandenen Archiven. **Noch kein Auftrag für Commit, Push, Tag oder Veröffentlichung.**

## Quellen und reproduzierbarer Paketweg

CAN-Basiscommit **`fdc7e335acdaba5a9089bdc3429faebb433767b5`**, ergänzt um die dokumentierten lokalen Änderungen. Kein Commit, Push, Tag oder Release. Das Quellenarchiv ist ein ausdrücklich nicht committierter Arbeitsbaum-Snapshot mit eigener Datei-/SHA-256-Liste, kein behaupteter neuer Release-Commit.

- Neue CAN-Quelle: `artifacts/source-1.0.1-build34/CutAssistantNext-1.0.1-Build34-source`.
- **594 Quelldateien** im Manifest; **595 ZIP-Einträge** einschließlich des Manifests bytegenau zur Quellkopie geprüft. Snapshot nennt Build 34, Motor 0.1.6, EXE-/Motorquellen-Prüfsummen und 48 ergänzte/geänderte Arbeitsbaumdateien.
- Alle acht Lockdateien der Quellkopie aus dem Basiscommit exportiert. Vier schon vorher vorhandene lokale Produktions-Lockänderungen wurden ausdrücklich ausgeschlossen; alle acht Original-Lockdateien bleiben unverändert. Restore mit `win-x64` und `--locked-mode` bestanden.
- Neue Motorquelle: `artifacts/source-otr-can-0.1.6`, **26 Manifestdateien / 27 ZIP-Einträge**. Regulärer Motorquellstand vollständig mit der Quellkopie verglichen. Motorbasiscommit `2a474629836d52023bf37cbbdb309ccd1c23acf3` plus ausdrücklich dokumentierte lokale Änderungen; die Versionsänderung ist kein neuer Commit.
- Motorquellen separat mit **53 Tests**, Formatcheck und Release-Build mit Compilerwarnungen als Fehler geprüft. Der Quellen-Neubau ist nicht bytegleich zur zuvor persönlich geprüften EXE; beide Prüfsummen und die bewusste Verteilung der erhaltenen getesteten EXE sind in `ENGINE-PROVENANCE.json` genannt. Keine Zusage binärer Reproduzierbarkeit.
- Alle **40 Windows-Crates** und der gesamte übrige Cargo-Lock-Abhängigkeitsbestand unverändert gegenüber 0.1.5. Bestehende Original-Lizenztexte, Rust-Standardbibliothekshinweise, MPL-2.0, Herkunft und Danksagung erhalten. Passende Motorquellen sowohl im Programm unter `tools/otr-can/sources` als auch als separates ZIP verfügbar.
- Paketierung über die eingefrorene Fassung von `tools/package-release.ps1`, Inno Setup 7 und die unveränderten geprüften libmpv-Dateien. Das Skript verlangt 0.1.6 und beide Fähigkeiten; alte/falsche Motoren und fehlende Quellen werden abgelehnt.

Build-/Test-/Publish-Kern im selben CAN-Snapshot:

```powershell
dotnet restore .\CutAssistantNext.sln -r win-x64 --locked-mode
dotnet build .\CutAssistantNext.sln -c Release --no-restore -p:CanBuildNumber=34
dotnet test .\CutAssistantNext.sln -c Release --no-build --no-restore
dotnet publish .\src\CutAssistantNext.App\CutAssistantNext.App.csproj -c Release -r win-x64 --self-contained false -p:CanBuildNumber=34 -p:RestoreLockedMode=true -o <neuer Publish-Ordner>
```

`PACKAGE-INPUTS.json` unter `.build/release-1.0.1-build34-20261019` enthält die konkreten Eingaben. `tools/build.ps1` wurde wegen seines automatischen Zählers nicht zusätzlich aufgerufen. Alle Paketaufrufe erhalten explizit Build 34; der ursprüngliche lokale Zähler blieb bis nach den erfolgreichen Paket-/Testprüfungen auf 33 und wurde dann auf **34** übernommen.

## Prüfungen der finalen Paketbytes

| Prüfung | Ergebnis |
|---|---|
| Gesperrter Restore | Alle acht Projekte erfolgreich |
| CAN-Release-Build | **0 Warnungen / 0 Fehler** |
| Core / Cutlists / Media / App | **199 / 72 / 224 / 890** |
| Gesamte Lösung | **1385/1385 bestanden**, keine fehlgeschlagenen oder übersprungenen Tests |
| Finale Publish-App | **890/890 App-Tests erneut bestanden** mit den tatsächlich gepackten DLLs |
| Übrige 495 Tests | Core-/Cutlists-/Media-DLLs bytegleich zwischen Testlauf und Paket |
| Rust-Quellneubau | **53/53 Tests**, Release-Build mit Warnungen als Fehler |
| Motor-Paketprüfung | **11/11**, einschließlich falscher Version, fehlender/veränderter Quellen, EXE-Manipulation und fehlender neuer AAC-Fähigkeit |
| Portable | **250 Dateien**, jede bytegenau gegen Publish-Manifest und entpacktes ZIP; keine zusätzlichen/fehlenden Dateien |
| CAN-/Motor-Quellenarchive | Alle **595 / 27 ZIP-Einträge** bytegleich zur jeweils eingefrorenen Quelle |
| Versionsanzeige | **Cut Assistant Next · 1.0.1 · Build 34**, keine RC-Kennung |
| Windows-Programmversion | **1.0.1.34**, Produktversion **1.0.1+build.34** |
| Setup | Erfolgreich kompiliert, Produktversion 1.0.1, Paketname Build34 |
| Laufzeit/Lizenzen | Frameworkabhängige .NET-10-Desktop-Ausgabe, geprüfte libmpv-DLLs und erforderliche Hinweise; externe Medienwerkzeuge separat |

### Echte Schnitt- und Kontrollplayer-Aufträge

Drei neue synthetische Aufträge durch die reguläre CAN-Motorfabrik mit **Build-34-Paket-DLLs und dem aus dem Portable-ZIP entpackten Motor** erfolgreich: gesunder MP3-Fall, Stereo-AAC mit zwei nachgewiesenen Ein-Sample-Konflikten sowie gemischte MP3/AAC/AC3-Spuren mit AAC-Konflikten. Ergebnisse bytegleich zu den zuvor unabhängig geprüften Referenzen. Jeweils eine Indexierung, korrekte Quelle, unveränderte Originale/Indizes, vollständige Bereinigung und erwartete geplante Ergebnisdauer/-übergänge bestätigt. Vor Kodierung erfasste PCM-Dateien im lokalen Nachweisordner erhalten. Keine persönlichen Werkzeugpfade umgestellt.

Zusätzlich echter MP4Box-Vier-Abschnitt-Schnitt mit denselben Paket-DLLs: gemessene Übergänge **2.200 / 5.400 / 8.400 s**, Kontrollplayer fordert **0 / 2.400 / 5.400 s** an. Drei reale HWND-/libmpv-Prüfungen vergleichen die angezeigten Abschnittsanfänge pixelweise mit den Referenzbildern; doppelte PTS an Übergängen 2 und 3 berücksichtigt. Dateizuordnung, Bereinigung und Playerfreigabe bestanden. Abschlussfenster mit geprüften Zeiten und dem bestehenden Bildverlusthinweis gerendert und visuell kontrolliert.

Die persönlichen Musikladen-/Navy-CIS-Abnahmen beziehen sich auf die identische Motor-EXE und lokale CAN-Schnitte. Eine persönliche Installation, Deinstallation oder Portable-Abnahme von Build 34 wird daraus nicht abgeleitet.

### Formatierung und Prüfhelfer

PowerShell-Syntaxprüfung, `git diff --check` und Formatprüfung der 21 geänderten C#-Dateien bestanden. Der breite Formatcheck wurde nicht wiederholt; die 46 bekannten, unveränderten Altdateien aus dem Build-33-Bericht wurden nicht umformatiert.

Die Abhängigkeitsprüfung unterschied zunächst nicht zwischen allen Cargo-Lock-Einträgen und Windows-Crates. Der Nachweis wurde mit `--filter-platform x86_64-pc-windows-msvc` präzisiert: 40 identische Windows-Crates, zusätzlich fünf unveränderte Unix-Einträge im Lockfile. Prüfhelfer wurden nach einem falschen UI-Aufrufmodus und einem alten Build-33-Hilfspfad berichtigt; der erfolgreiche native Lauf und die Formatprüfung sind separat protokolliert. Keine Produktprüfung abgeschwächt oder entfernt. Der erste UI-Aufruf brach vor einem Schnitt mit „Unknown mode“ ab; `NATIVE-QA-attempt1.log` bleibt erhalten.

## Pakete und Prüfsummen

Sechs Nutzdateien plus `SHA256SUMS.txt` unter `artifacts/release-1.0.1-build34`. Die beiden libmpv-Archive sind unverändert aus dem bisherigen Paketsatz übernommen.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| `CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip` | 405926830 | `01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B` |
| `CAN-libmpv-v0.41.0-relink-2026-09-26.zip` | 47273515 | `00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342` |
| `CutAssistantNext-1.0.1-Build34-source.zip` | 3284055 | `E1B068CEAE5492B6DE4E0E9F991D56DED95698A5355E6AC2C5A0519C3BEC079F` |
| `CutAssistantNext-1.0.1-Build34-win-x64-Setup.exe` | 20356048 | `9C6A8793ED2C31104D0305D1857FA66EB7B6D900C940E4A545F29CC30C17F961` |
| `CutAssistantNext-1.0.1-Build34-win-x64.zip` | 25062619 | `FBE8EAF5E4993F384194DDB6811169504FE5299B77AEB6E36F11A4D23DC492D4` |
| `otr-can-0.1.6-source.zip` | 71900 | `B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F` |

[Setup](../artifacts/release-1.0.1-build34/CutAssistantNext-1.0.1-Build34-win-x64-Setup.exe), [Portable-ZIP](../artifacts/release-1.0.1-build34/CutAssistantNext-1.0.1-Build34-win-x64.zip), [CAN-Quellen](../artifacts/release-1.0.1-build34/CutAssistantNext-1.0.1-Build34-source.zip), [Motorquellen](../artifacts/release-1.0.1-build34/otr-can-0.1.6-source.zip), [Prüfsummen](../artifacts/release-1.0.1-build34/SHA256SUMS.txt).

## Erhaltung und nächster Schritt

**745 von 750 erfassten Bestandsdateien unverändert.** Erwartete Änderungen: README, Release Notes 1.0.1, Paketierungsskript, dessen Motor-Pakettests und lokaler Buildzähler auf 34. Die bestehenden C#-Änderungen, alle acht Original-Lockdateien, Motorquellen, getestete Motor-EXE und persönlichen Einstellungen sind erhalten. Alle zwölf Nutzdateien aus Build 32 und 33 erneut gegen ihre Manifeste geprüft; auch übrige erfasste alte Paketdateien unverändert. HEAD und Tag `v1.0.0` erhalten. Keine Originalmedien oder vorhandenen Filmausgaben ersetzt.

Neu als nachträgliche separate Prüfnachweise: dieser Bericht und die Build-34-VM-Übergabe. Sie ändern die bereits eingefrorenen Quellenarchive oder Pakete nicht. Die aktuellen Release Notes sind in Setup und Portable bytegleich enthalten.

Die vorgesehenen persönlichen [Setup-/VM-/Portable-Prüfungen einschließlich VM-Deinstallation](UEBERGABE-CAN-1.0.1-BUILD34-20261019.txt) sind am 20.10.2026 erfolgreich abgeschlossen. Auch der umgestellte Motorpfad und otr-can **0.1.6** im Portable-Test sind bestätigt. Alle Werkzeugpfade bleiben bewusst konfigurierbar; eine alte persönliche Motorwahl wird durch die Paketierung nicht überschrieben. Commit, Push, Tag oder Veröffentlichung erst auf gesonderten Auftrag.

Lokale Detailnachweise unter `.build/release-1.0.1-build34-20261019`: `PACKAGE-INPUTS.json`, `PACKAGE-PROOF.json`, `FINAL.json`, `VERSION-PROOF.json`, `CAN-CUT-PROOF.json`, `DEPENDENCY-PROOF.json`, Motor-Pakettests, Rust-Quellbuild-/Testlogs, finale App-Tests, Format-/QA-Logs und native Zeit-/Bildnachweise. Vollständige Build-/Test-/Publish-/Installerlogs und TRX zusätzlich im Paketausgabeordner.

## Veröffentlichungsfreigabe – 20.10.2026

Nach Abschluss sämtlicher vorgesehener persönlicher Paketprüfungen beauftragt der Nutzer ausdrücklich Commit, Push und Veröffentlichung. Die früheren Hinweise auf fehlende Freigabe dokumentieren den damaligen Stand. Gebauter CAN-Quellcommit **`a6b7e52bddb72a2517631df2a8c5b8fb68c06ea5`**, passender Motorquellcommit **`bbc3a5104b75e7bcfc4f257ea5b14e9a1961f6db`**; alle 593 CAN-Repositorydateien und sämtliche Motorquelldateien gegen die gebauten Snapshots unter regulärer Git-Zeilenendennormalisierung geprüft. Die mitgelieferten Quellenarchive enthalten unverändert die exakten Buildbytes und ursprünglichen Snapshot-Metadaten. Spätere Abnahmen und Veröffentlichungsnachweise sind separate Folgecommits. Pakete werden nicht neu gebaut oder umgepackt. [Veröffentlichungsnachweis](RELEASE-PUBLICATION-1.0.1.md).
