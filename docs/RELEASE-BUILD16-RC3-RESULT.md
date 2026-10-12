# CAN 0.2.1 Build 16 RC3 – Paketprüfbericht

Stand: 12.10.2026. Schritt 3 auf ausdrücklichen Nutzerauftrag abgeschlossen: Setup und Portable-ZIP erstellt. **Setup-/VM- und Portable-Prüfungen mit beiden Motoren vom Nutzer bestätigt; frische VM-Installation und Deinstallation geprüft. Anschließend auf Nutzerauftrag als Vorabversion veröffentlicht.** Der Paketbauschritt selbst änderte keine Tags; der spätere [Veröffentlichungsnachweis](RELEASE-PUBLICATION-BUILD16-RC3.md) dokumentiert Freigabe und Uploadprüfungen.

## Quellen und Inhalt

- CAN-Quellcommit **603d64d5c6374eea96ec5920719443fc6d399d81**, ohne Arbeitsbaumüberlagerung, Version 0.2.1 · Build 16 · RC3.
- Motorquellcommit **2a474629836d52023bf37cbbdb309ccd1c23acf3**, otr-can **0.1.5**. Beigefügte getestete EXE unverändert: SHA-256 **FC6ED6568880A995F3D141A9378AA2CE28ADE8DC0E257E93DC972799BB1324A6**.
- Paket enthält AVI-/MP3-Anbindung und den Motor mit einmaliger MP3→AAC-LC-Kodierung nach dem PCM-Schnitt, passende Quellen, Lizenz-/Autorenhinweise und Einrichtungstext. MP4Box bleibt Standard; externe FFmpeg/ffprobe, FFMS2 sowie .NET-/VC-Laufzeit werden nicht zusätzlich ausgeliefert.
- Geprüfter libmpv-0.41.0-Bestand samt unveränderten Quellen-/Relink-Archiven übernommen. Kein neuer libmpv- oder Rust-Build während der Paketierung.

## Paketbau

Aus dem eingefrorenen CAN-Quellordner mit dessen tools/package-release.ps1 gebaut, BuildNumber 16 und Candidate RC3 ausdrücklich übergeben. Gesperrter win-x64-Restore nur im exportierten Quellbaum, Release-Build, Tests und framework-dependent Publish erfolgreich. Build **0 Warnungen/0 Fehler**, **793/793 CAN-Tests**, 0 fehlgeschlagen/übersprungen (Core 74, Cutlists 65, Media 125, App 529). Letzter Rust-Quellprüfstand unverändert **44/44 Tests** aus Schritt 2.

Inno Setup 7 erstellt die Setup-EXE erfolgreich aus demselben Publish-Verzeichnis, aus dem das Portable-ZIP erzeugt wurde. Vollständige Installations-/Deinstallations- oder Setup-Payload-Abnahme daraus nicht abgeleitet.

## Automatische Inhaltsprüfung

- Portable-ZIP neu entpackt, **alle 249 Dateien** per SHA-256 mit PUBLISH-FILES.sha256 abgeglichen; vollständige Anzahl und Pfadgrenzen geprüft.
- Release-Provenienz bestätigt Quellcommit, Build 16, RC3, Quellenarchivhash, Motorversion 0.1.5 und dessen Herkunft. Keine Arbeitsbaumüberlagerung.
- Apphost und App-DLL tragen Dateiversion **0.2.1.16**; Assembly-Metadatum **CanCandidate=RC3** direkt aus dem PE gelesen, ohne Appstart. Keine sichtbare Versionsanzeige/GUI-Abnahme daraus abgeleitet.
- Tatsächlich kopierter Motor mit --version und capabilities gestartet: **otr_can 0.1.5**, **mp3-aac-v1**. EXE und Motorquellenarchiv bytegleich zum vorbereiteten Motorpaket.
- Keine FFmpeg-/ffprobe-/MP4Box-/ffmsindex-EXE, FFMS2-DLL oder private .NET-Laufzeit unbeabsichtigt im Portable-Inhalt.
- Alle sechs Binär-/Quellenartefakte anhand SHA256SUMS.txt erneut vollständig geprüft. Geteilte libmpv-Archive vor Übernahme gegen ihre bisherigen festen Hashes geprüft; alte Pakete nicht verändert.
- Alle acht lokalen und acht exportierten CAN-Lockdateien gegen Ausgangshashes unverändert. Sämtliche 464 manifestierten Quell-Snapshot-Dateien nach Paketbau unverändert. Lokaler Buildzähler bleibt **16**; die vier vorbestehenden Produktions-Lockänderungen bleiben erhalten.

## Dateien

Ablage: **artifacts/release-build16-RC3**.

| Datei | Byte | SHA-256 |
|---|---:|---|
| CutAssistantNext-0.2.1-Build16-win-x64-Setup-RC3.exe | 20221933 | ED0E5DB3ABD7AE98514BAC6DCC7BFC59D4182D992DFE0AA9E52FC971A7E5AC2D |
| CutAssistantNext-0.2.1-Build16-win-x64-RC3.zip | 24889401 | 07CA8B4D17725A50FA2B44499F96DEB9EDAB3A0FD0720D352DA59EF898CA64BE |
| CutAssistantNext-0.2.1-Build16-source-RC3.zip | 2841874 | 852DDC850B0F8B4DF0D1FF1BCA35E0FD3BAD5B992103FBDDC03D72317F1C697C |
| otr-can-0.1.5-source.zip | 63988 | 3A973278ABDF719B745405D9DFBB930C14A28CFEF40C7FB52A35CA17F7214E57 |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

SHA256SUMS.txt, PUBLISH-FILES.sha256 und RELEASE-PROVENANCE.json im Publish-/Portable-Inhalt dokumentieren die Zuordnung. Dieser Ergebnisbericht und RELEASE-NOTES.md entstehen nach dem Paketbau, gehören nicht zum eingefrorenen CAN-Quellenarchiv und verändern Setup/ZIP nicht.

## Persönlicher Setup-AVI-Test – 12.10.2026

Der Nutzer übermittelt einen neuen vollständigen CAN-Schnittlauf von „Die Teufelsbrigade“ und bestätigt ausdrücklich **„setup Build 16“** sowie **„Anfang, Ende, Schnitte, Bild, Ton alles top :-)“**. Damit ist der AVI/MP3-Schnitt aus der Setup-Anwendung mit positiver persönlicher Ergebnisbewertung bestätigt.

Das Protokoll zeigt Original-AVI mit H.264/720×576/25 fps und MP3-Stereo/48 kHz. Die Vorbereitung kopiert Video und MP3; Videopaket-/Stream-/Laufzeitprüfung bestanden. Genau eine FFMS2-Indexierung, sechs Behaltebereiche, Motor 0.1.5 und alle drei Stufen erfolgreich. 182.574 Videopakete, 350.542.080 decodierte Tonsamples, AAC-LC-Ziel 192 kbit/s; finale Motor- und CAN-Prüfungen abgeschlossen. Die fünf Übergangszeiten bleiben 00:24:48,200; 00:45:38,840; 01:05:41,600; 01:25:17,720; 01:50:13,080, Ende nominal 02:01:42,960.

FFmpeg meldet beim Umpacken weiterhin fehlende/unbesetzte PTS. Diese Warnungen bleiben dokumentiert; weder der erfolgreiche Lauf noch die persönliche Rückmeldung ersetzen den strengen Voll-Videodekodiernachweis. Der bereits bekannte technische Zeitstempelbefund wird nicht als behoben ausgegeben. Keine zusätzliche Datei-/Audiohashprüfung oder Laufzeitmessung des neuen Ergebnisses vorgenommen.

Anschließend die tatsächliche Installation lesend geprüft: App-Dateiversion 0.2.1.16; **alle 249 im Paketmanifest erklärten installierten Dateien** stimmen bytegenau mit dem Paketsatz überein. Daraus folgt kein Nachweis einer frischen VM-Installation oder der Abwesenheit zusätzlicher Altdateien.

Beim Pfadcheck stand der Motor noch auf unserem früheren Entwicklungsordner. Entwicklungs- und installierte Motor-EXE sind bytegleich (FC6ED656…1324A6). Den Pfad über CANs Einstellungsverwaltung auf **C:\Program Files\Cut Assistant Next\tools\otr-can\otr_can.exe** gestellt, vorherige Einstellung gesichert, Indexer und alte EXE erhalten, Versions-/MP3-Fähigkeit geprüft. Der übermittelte Film entstand vor dieser Pfadkorrektur; kein neuer Schnitt nach der Umstellung behauptet.

Nachweise: .build/release-build16-RC3/user-avi-dbeeb881.log, USER-SETUP-AVI-RESULT.json, USER-INSTALLED-CONTENTS.json und otr-can-settings-before-installed-path.json samt verification.json. Installationstyp (Überinstallation/frisch/VM) nicht ausdrücklich genannt; Portable, neuer MP4Box-Pakettest und Deinstallation bleiben offen.

## Frische VM-Installation und Deinstallation – 12.10.2026

Nutzer bestätigt ausdrücklich **„setup frisch bei VM / MP4Box check / ot-can check / Deinstallation check :-)“**. Frische Setup-Installation in einer VM, erfolgreiche Prüfungen beider Motoren und anschließende Deinstallation damit positiv zurückgemeldet. „ot-can“ im Projektkontext als otr-can zugeordnet.

Das beigefügte Protokoll betrifft **MP4Box**, nicht einen zusätzlichen otr-can-Lauf: Teufelsbrigade-AVI mit H.264/720×576/25 fps und MP3-Stereo/48 kHz, Stream-Copy-Vorbereitung und exakte Videopaketprüfung bestanden, sechs erfolgreiche -splitx-Aufrufe, vollständiges -cat-Zusammenfügen und Abschluss **„Fertig.“**. FFmpeg meldet weiterhin fehlende PTS; Warnungen bleiben dokumentiert.

Die sechs an MP4Box übergebenen Bereiche lauten 357,0413125–1845,24; 2258,68–3509,28; 3947,32–5150,04; 5585,2–6761,28; 7145,72–8641,04; 9015,64–9705,4641875 s. Dies sind die bestehenden MP4Box-Motorgrenzen; keine Änderung an Schnittlogik oder Cutlist daraus abgeleitet.

otr-can-Erfolg und Deinstallation beruhen auf der ausdrücklichen Nutzerbestätigung. Kein separates neues otr-can-Protokoll, keine zusätzlichen Samplezahlen, Laufzeiten, VM-Dateihashes oder gesonderte detaillierte Bild-/Tonbewertung dieser beiden VM-Ausgaben mitgeteilt. Die frühere detaillierte positive Setup-Bild-/Tonabnahme bleibt separat gültig. Keine eigenständige Inspektion der VM vorgenommen.

Nachweise: .build/release-build16-RC3/user-vm-mp4box-50c4345f.log und USER-VM-RESULT.json. Die Art der Installation ist für diesen VM-Lauf jetzt ausdrücklich als frisch bestätigt. Portable-Abnahme weiterhin offen; Überinstallation nicht gesondert als eigener Test bestätigt.

## Portable-ZIP in der VM – 12.10.2026

Nutzer bestätigt anschließend ausdrücklich **„VM und zip: / MP4Box check / otr-can check“**. Positive persönliche Funktionsprüfung des Portable-ZIPs in der VM mit beiden Schnittmotoren damit dokumentiert. Zusammen mit der vorherigen frischen Setup-VM-Installation und Deinstallation sind die gemeldeten praktischen Setup-/Portable-Prüfungen positiv abgeschlossen.

Die beiden neuen Anhänge sind byteidentische Kopien desselben **MP4Box-Protokolls** (SHA-256 6994A592887C88B01B4303348A362F027F01E662B662CEADCCAAE0FC165E4D54). Sie zeigen einen eigenen Lauf mit neuer temporärer Arbeitsdatei und anderen Segmentdateien als der vorangegangene Setup-VM-Test: Stream-Copy von H.264 und MP3, exakte Videopaketprüfung, sechs -splitx-Bereiche, vollständiges Zusammenfügen und „Fertig.“. Der Erfolg von otr-can wird aus der ausdrücklichen Nutzerbestätigung übernommen; kein neues natives Protokoll oder neue Sample-/Laufzeitwerte daraus abgeleitet.

Keine neue detaillierte Anfang-/Ende-/Übergangs-/Bild-/Tonmessung, keine VM-Dateihashes und kein Nachweis eines unveränderten frischen Portable-Einstellungsprofils mitgeteilt. Die vorherige ausführliche Setup-Filmabnahme bleibt separat gültig. Die Remux-PTS-Warnungen und der bekannte technische Video-Befund bleiben dokumentiert. Überinstallation weiterhin nicht gesondert als eigener Test bestätigt.

Nachweise: .build/release-build16-RC3/user-portable-154b56ab.log, user-portable-4c39c9b1.log und USER-PORTABLE-RESULT.json. Keine Paketdatei oder ausführbare Komponente geändert.

## Prüfgrenzen und nächster Schritt

Die persönliche Teufelsbrigade-Prüfung ist für einen neuen Setup-Build-16-Lauf ausführlich bestätigt. Frische VM-Installation, beide Motoren, Deinstallation sowie Portable mit beiden Motoren sind zusätzlich positiv zurückgemeldet; der jeweilige Umfang ist oben getrennt dokumentiert. Der vorbestehende Video-PTS-Befund bleibt dokumentiert. Die Kontroll-Rust-EXE aus dem Quellenarchiv ist nicht bytegleich zur getesteten EXE; keine bytegenaue Reproduzierbarkeit behauptet, getestete EXE beibehalten.

**Praktische Paketabnahme:** Frische VM-Setup-Installation, beide Motoren und Deinstallation sowie Portable mit beiden Motoren sind vom Nutzer bestätigt. Der Setup-AVI-Lauf ist zusätzlich detailliert für Anfang/Ende, Schnitte, Bild und Ton abgenommen. Überinstallation, ein frisches Portable-Einstellungsprofil und zusätzliche quantitative/strenge Video-Prüfungen sind nicht gesondert bestätigt; bekannte Grenzen bleiben dokumentiert. Schritt 5 anschließend auf ausdrücklichen Nutzerauftrag abgeschlossen: [Veröffentlichungsnachweis](RELEASE-PUBLICATION-BUILD16-RC3.md).

Nachweise: .build/release-build16-RC3/{PACKAGE.log,PACKAGE-CONTENTS.json,PRESERVATION.json,ARTIFACTS-VERIFIED.json}, portable-extracted und artifacts/release-build16-RC3/{logs,test-results}. Keine realen Medien eingecheckt.