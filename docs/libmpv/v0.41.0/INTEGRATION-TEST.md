# CAN-Integration des Vier-DLL-Kandidaten (26.09.2026)

**Interner Teststand, keine öffentliche Freigabe.** Die Buildnummer blieb bei 6. Es wurde weder committet noch gepusht.

## Einbindung

- `tools/setup-libmpv.ps1 -SourceDirectory <Ordner mit vier DLLs>` prüft die vier SHA-256-Werte aus [README.md](README.md) vor und nach dem Kopieren nach `external/libmpv/can-v0.41.0/`. Der frühere Ordner `external/libmpv/win-x64/` bleibt bestehen und wird nicht mehr vom App-Projekt verwendet.
- Das App-Projekt kopiert alle vier DLLs und `docs/libmpv/v0.41.0/` nach `third-party/libmpv/v0.41.0/` in Build und Publish. Vor dem Build werden alle vier DLLs auf Anwesenheit geprüft.
- `installer/CutAssistantNext-CAN-libmpv-v041-TEST.iss` nimmt den vollständigen Test-Publish-Ordner auf. Der Inno-Compiler meldete die vier DLLs und die Lizenzunterlagen beim Packen; eine tatsächliche Installation auf einem sauberen Rechner steht noch aus.

## Lokale Prüfergebnisse

| Prüfung | Ergebnis |
| --- | --- |
| `dotnet build` (Release, Build 6) | erfolgreich, 0 Fehler, 0 Warnungen |
| `dotnet test` (Release, ohne Neubau) | 484 bestanden, 0 fehlgeschlagen |
| Self-contained Publish `win-x64` | erfolgreich, 532 Dateien; Programm-Dateiversion `0.2.0.6` |
| Publish-DLLs und portable ZIP | alle vier SHA-256-Werte stimmen mit [README.md](README.md) überein |
| Drittanbieterhinweise | `THIRD-PARTY-NOTICES.md` und 105 Original-Lizenztexte in Publish und portablem ZIP vorhanden |
| `libmpv`-Ladetest aus Publish | Client-API 2.5; Erzeugen, Initialisieren und Freigeben erfolgreich |
| Inno-Test-Setup | erfolgreich kompiliert; Aufnahme der vier DLLs und Hinweise im Compiler-Protokoll sichtbar |
| FFmpeg-Werkzeug im portablen ZIP | nicht enthalten; externe Werkzeuge bleiben separat zu konfigurieren |

Die beiden lokalen Testartefakte liegen unter `artifacts/` und werden von Git ignoriert:

| Artefakt | Bytes | SHA-256 |
| --- | ---: | --- |
| `CutAssistantNext-0.2.0-Build6-CAN-libmpv-v041-PORTABLE-TEST.zip` | 85.385.395 | `6E07E531BABD4CB582FD4C03CCFF2ACB60FF085B443196F6C55A0EAB693BBD15` |
| `installer-output/CutAssistantNext-0.2.0-Build6-CAN-libmpv-v041-TEST.exe` | 63.516.325 | `E385CC44269675B1C4188A9C1AA9D277020C01A76BF19653E05A97A75950BE7B` |

## HD-Praxistest am 27.09.2026

CAN 0.2.0 Build 6 lief aus einem separaten Testordner. Die dortige `libmpv-2.dll` hatte SHA-256 `95A37097C6C7EADF7098A0247AC9028143DA26E14081A76A7F6575C1A5AB22EA` und entspricht damit dem selbst gebauten Vier-DLL-Kandidaten. Der Projektverantwortliche berichtete von geschmeidiger Bedienung und Wiedergabe einer 6.928.885.595 Byte (6,45 GiB) großen HD-Aufnahme mit H.264, 1280 × 720 Pixeln, 50 fps und zwei Tonspuren.

CAN erzeugte eine Cutlist mit einem Behaltebereich ab 420,376 Sekunden. MP4Box meldete den Schnitt als abgeschlossen. Die ausgegebene MP4-Datei umfasste 5.509.923.112 Byte und 5.621,16 Sekunden; die technische Prüfung bestätigte 720p/50 fps und beide Tonspuren. Der Projektverantwortliche bestätigte, dass keine Tonprobleme auftraten. Welche der beiden Tonspuren zu hören ist, hängt vom jeweiligen Player ab; die Wahl im Player nach dem Schnitt ist in der Nutzeranleitung beschrieben.

Dieser Test belegt einen erfolgreichen großen HD-Fall mit dem konkreten CAN-libmpv-Build. Er ist keine allgemeine Zusage für alle HD-Dateien oder Codecs.

## VM-Praxistest am 27.09.2026

Der Projektverantwortliche installierte den neuen CAN-Teststand auf einer Windows-VM, nachdem er dort das alte CAN deinstalliert hatte. Das übermittelte Schnittprotokoll zeigt drei mit MP4Box fertiggestellte Segmente (`1044.44:1698.04`, `2364.56:3398.36`, `4034.64:4746.96`) und das erfolgreiche Zusammenfügen aller drei Segmente mit abschließendem „Fertig.“. Die addierten Behaltebereiche umfassen 2.399,72 Sekunden. Die MP4Box-Meldung „No suitable destination track found - creating new one“ trat beim Anlegen der ersten Video- und Tonspur der neuen Zieldatei auf; der anschließende Zusammenfügevorgang lief vollständig durch.

Der Projektverantwortliche prüfte die fertige Datei auf der VM und bestätigte ein einwandfreies Ergebnis. Anschließend verlief auch die Deinstallation des neuen CAN-Teststands sauber. Für die öffentliche Freigabe fehlen außerdem die in [README.md](README.md) beschriebenen öffentlichen Quellen-/Relink-Assets, die Bestätigung der Rechte an CAN-Code und Symbol sowie die Prüfung der .NET-/WPF-Lizenzbedingungen. CANs eigene Lizenzvariante ist inzwischen GPL-3.0-or-later.

## Neuer VM-Test der laufzeitabhängigen Ausgabe am 27.09.2026

Der Projektverantwortliche prüfte den Start des neuen laufzeitabhängigen CAN-Pakets zunächst ohne installierte .NET-Desktop-Laufzeit. Windows zeigte den erwarteten .NET-Startdialog für fehlendes `Microsoft.NETCore.App` 10.0.0. Dieser Dialog allein prüft noch nicht die CAN-Wiedergabe; der erforderliche Download für die endgültige Nutzung ist die .NET 10 Desktop Runtime für Windows x64.

Das anschließend übermittelte neue Schnittprotokoll für `Brilliant_Minds_26.09.18_21-05_5plus_65_TVOON_DE.HQ.mp4` enthält dieselben drei Behaltebereiche wie beim früheren VM-Test, aber neue temporäre Dateinamen. MP4Box meldete bei jedem Segment `file 1 done`, beim Anhängen jedes Segments 100 %, beim abschließenden MP4-Schreiben 100 % und zum Schluss „Fertig.“. Der Projektverantwortliche bestätigte ausdrücklich, dass das Protokoll vom neuen laufzeitabhängigen Paket stammt und die fertige MP4 im Player geprüft und in Ordnung ist. Im Anschluss bestätigte der Projektverantwortliche, dass sich die Setup-Version gut deinstallieren ließ. Nach Installation der .NET 10 Desktop Runtime startete CAN aus dem separaten ZIP ohne erneute Laufzeitabfrage. Ein neues, vom Setup-Test verschiedenes Schnittprotokoll für dieselbe Datei zeigt bei allen drei Segmenten `file 1 done`, beim Zusammenfügen 100 %, beim abschließenden MP4-Schreiben 100 % und „Fertig.“. Der Projektverantwortliche prüfte die mit der ZIP-Ausgabe erzeugte MP4 im Player und bestätigte Bild und Ton als einwandfrei. Auch die Wiedergabe im CAN-Player wurde erfolgreich geprüft.
