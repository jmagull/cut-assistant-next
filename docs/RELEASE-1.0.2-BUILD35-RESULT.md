# CAN 1.0.2 · Build 35 – eingefrorener lokaler Paketsatz

**Stand 22.10.2026: Pakete erstellt und technisch geprüft. Nicht veröffentlicht.** Nutzerauftrag umfasst Freeze, Setup/ZIP/Quellen/Prüfsummen sowie Commit und Push. **Veröffentlichung frühestens Sonntag, 25.10.2026, nach zusätzlicher ausdrücklicher Freigabe.** Keine automatische Veröffentlichung und kein neuer Release-Tag. Die persönliche Setup-/VM-/Deinstallations-/Portable-Abnahme des neuen Paketsatzes steht noch aus.

## Quellstand und Paketumfang

Gebauter Quellcommit: **`c9e6ebf1368a47211bc0ca22243d6e6138e20959`**. Version **1.0.2**, Build **35**, keine RC-Kennung. Neue Pakete unter `artifacts/release-1.0.2-build35`; alte Pakete nicht ersetzt.

Enthalten sind die eng begrenzte MP4Box-H.264-Überlappungszuordnung und die bestehenden Ausgabezeit-/Identitäts-/Rückfallprüfungen. **otr-can 0.1.6** bleibt unverändert: EXE-SHA-256 `73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E`. Geprüfte libmpv-0.41.0-Dateien, passende Lizenzen und Quellen unverändert übernommen. [Release Notes](RELEASE-NOTES-1.0.2.md), [Freeze](FREEZE-1.0.2-20261022.md).

Setup und Portable stammen aus demselben Publish-Verzeichnis; **250 Portable-Dateien** vollständig gegen das Publish-Dateimanifest geprüft. .NET-10-Desktop-Laufzeit und externe Medienwerkzeuge bleiben separat erforderlich. Diagnosewerkzeuge werden als Quellen mitgeliefert und nicht in die Programmverteilung eingebunden.

## Quellen und technische Prüfung

- **621 committierte Repositorydateien** gegen den Quellcommit unter den vorhandenen Git-Zeilenendenregeln geprüft. Der Export berücksichtigt `.gitattributes`/Windows-Zeilenenden. Das Quellenarchiv bewahrt die tatsächlichen Buildbytes, `SOURCE-SNAPSHOT.json` und `SOURCE-FILES.sha256`; **623 Dateien im Quellen-ZIP** bytegenau zur Quellkopie geprüft. Keine Arbeitsbaumänderungen im Quellsnapshot.
- Die vier vorbestehenden lokalen Produktions-Lockänderungen sind aus Commit und Paketquellen ausgeschlossen und unverändert erhalten. Gesperrter `win-x64`-Restore mit den vorhandenen committierten Lockdateien erfolgreich; keine Aktualisierung von Abhängigkeiten. NuGet-Audit für den Paketlauf deaktiviert, keine neue Online-Sicherheitsbewertung behauptet.
- Vollständiger Release-Build und Publish aus dem eingefrorenen Quellverzeichnis erfolgreich, **0 Warnungen/0 Fehler**. **1411/1411 Solution-Tests**: Core 200, Media 246, App 893, Cutlists 72; keine Fehler oder übersprungenen Tests.
- **40/40 isolierte Diagnose-/Übergabetests** aus demselben Quellstand und **893/893 App-Tests erneut gegen die finalen Portable-Produkt-DLLs** erfolgreich.
- **Elf Motor-Paketprüfungen** einschließlich falscher Herkunft, fehlender Quellen, abweichender Hashes und fehlender Fähigkeiten erfolgreich. Der enthaltene unveränderte Motor meldet `otr_can 0.1.6` sowie `mp3-aac-v1 aac-overlap-v1`. Kein neuer Rust-Motorbuild für diese Paketierung; die bestehenden Quellen-/Lizenznachweise bleiben enthalten.
- Kleiner künstlicher MP4Box-Schnitt über die finalen Produktklassen: vier Behalteabschnitte, drei bestätigte Übergänge und bereinigte Segmente. **Drei native Kontrollplayerprüfungen** mit realem HWND/libmpv bestanden, einschließlich Seek-Aufträgen mit Drei-Sekunden-Vorlauf beziehungsweise Begrenzung auf null, fortgesetzter Wiedergabe und passenden neuen Originalbildern. Versionsanzeige **„Cut Assistant Next · 1.0.2 · Build 35“** geprüft; beide Player freigegeben. Kein zusätzlicher Großdatei-/Furiosa-Schnitt für die Paketierung.
- Installer mit vorhandenen Inno-Setup-7-Einstellungen erfolgreich kompiliert. SHA-256 aller sechs Nutzdateien und Publish-Dateimanifest geprüft. Setup wurde nicht automatisch auf dem Arbeitsrechner installiert.

**146 erfasste Bestandsdateien** nach Paketbau unverändert: acht lokale Lockdateien, erfasste Build-32/33/34-Auslieferungsdateien, bisheriges Motorpaket und persönliche Einstellungen. Lokalen Buildzähler erst nach erfolgreicher technischer Prüfung von 34 auf 35 übernommen. Keine Änderung an Medien oder Werkzeugpfaden.

## Eingefrorene Auslieferungsdateien

Die sechs Nutzdateien und `SHA256SUMS.txt` bilden die sieben vorgesehenen Auslieferungsdateien. `PUBLISH-FILES.sha256` bleibt zusätzlicher lokaler Detailnachweis. Pakete und beide Prüfsummenlisten werden schreibgeschützt aufbewahrt.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build35-win-x64-Setup.exe | 20369596 | `2906467216EC018D366482BF18D4CFE38A59CBE07913689C40F6BDCBFF1A9920` |
| CutAssistantNext-1.0.2-Build35-win-x64.zip | 25073686 | `860A8F584CFC2DEEAE77BE6A1BA1CCA07E4D38566F2685654D137A49E02FE307` |
| CutAssistantNext-1.0.2-Build35-source.zip | 3381616 | `39F193987FD44456882B83A103AFDCFBA4E2BD34A84FC22DD45C8CE57884F200` |
| otr-can-0.1.6-source.zip | 71900 | `B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F` |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | `01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B` |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | `00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342` |

## Verbleibende persönliche Abnahme und Freigabe

Mit diesen neuen Dateien: Setup/Start und Versionsanzeige, vorhandene Werkzeugpfade, MP4Box-/otr-can-Schnittkontrolle, Setup/Deinstallation in der Windows-11-VM und entpacktes Portable/ZIP prüfen. Bild, Ton, Übergänge und Drei-Sekunden-Sprung jeweils im tatsächlich geprüften Umfang bestätigen. Frühere Film- oder Build-34-Paketabnahmen werden nicht auf diese Pakete übertragen. [Prüfübergabe](UEBERGABE-CAN-1.0.2-BUILD35-20261022.txt).

Die früheren Systemhänger bleiben ursächlich ungeklärt; keine Behebung durch diese MP4Box-Änderung behauptet. Bekannte MP4Box-Bildverluste/Blitzer, der erste alte Furiosa-Bildankerbefund und AVI-Grenzen bleiben dokumentiert. Keine neue Großdatei-/Belastungsprüfung angeordnet.

Vor Veröffentlichung Quellpin, eingefrorene Paketbytes und sieben Uploads erneut prüfen. **Erst ab 25.10.2026 und nach ausdrücklicher Freigabe veröffentlichen.** Bis dahin bleibt die zuletzt bestätigte öffentliche Version 1.0.0 Build 32 Latest, 1.0.1 Build 34 bleibt zurückgenommener Entwurf. Kein Draft/Upload/Tag für 1.0.2 durch den Paketauftrag angelegt.

Detailnachweise unter `.build/release-1.0.2-build35-20261022`: `PACKAGE-INPUTS.json`, `PACKAGE-PROOF.json`, `FINAL.json`, `PRESERVED-BEFORE.json`, `PRESERVED-AFTER.json`, finale App-/Prototyp-TRX, elf Motor-Paketprüfungen und native künstliche Prüfungen. Vollständige Restore-/Build-/Test-/Publish-/Installerlogs im Paketausgabeordner. Spätere Abnahme-/Pushnachträge sind separate Dokumentationscommits; gebaute Quellen und Paketbytes bleiben eingefroren.
