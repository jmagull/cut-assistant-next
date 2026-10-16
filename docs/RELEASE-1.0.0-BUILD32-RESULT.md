# Paketsatz CAN 1.0.0 · Build 32

Stand: **16.10.2026**. Auf Nutzerauftrag Funktionsumfang eingefroren und **Setup, Portable-ZIP, passende Quellen-/Lizenzpakete und SHA-256-Prüfsummen** lokal erstellt und geprüft. Stabile Versionskennung **1.0.0 · Build 32**, kein RC-Zusatz. Sieben Auslieferungsdateien schreibgeschützt eingefroren unter `artifacts/release-1.0.0-build32`. Keine GitHub-Veröffentlichung, kein Push und kein neuer Tag.

## Fester Quellstand

Gebauter lokaler Quellcommit **3e2ac03dfe4a701ecbc95c80cc06ff2527309b9f**, ohne Arbeitsbaumüberlagerung. Der erste freigegebene Freeze-Commit `a24520ce6630dda8b355e03719013ee655743bb6` wurde für die endgültigen Pakete um den bereinigten Voraussetzungenhinweis und beigelegte Release Notes ergänzt. Die endgültigen Pakete stammen ausschließlich aus dem oben genannten Folgecommit. Spätere Ergebnisdokumentation ändert diesen Quellpin nicht.

**557 manifestierte Quelldateien**, **558 Dateien im CAN-Quellen-ZIP** einschließlich Manifest; jede Archivdatei bytegenau mit dem Export verglichen. `SOURCE-SNAPSHOT.json` benennt Version, Build, feste Herkunft und den unveränderten Motor. Die vier vorbestehenden lokalen Produktions-Lockänderungen wurden nicht committet und sind nicht im Quellenarchiv enthalten; exportiert wurden alle acht Git-Lockdateien. Alle acht lokalen Lockdateien bleiben bytegleich zum Arbeitsbeginn erhalten.

## Tatsächlich geprüft

- Im endgültigen exportierten Baum: gesperrter win-x64-Restore, Release-Build **0 Warnungen/0 Fehler**, **1347/1347 Tests**, 0 übersprungen. Core 196, Cutlists 72, Media 194, App 885. Tests erst nach beendetem Build; vollständige TRX und Logs vorhanden.
- Framework-dependent win-x64-Publish. Dateiversion **1.0.0.32**; tatsächliches Hauptfenster und Anwendungsanzeige **Cut Assistant Next · 1.0.0 · Build 32**, ohne RC. Externe .NET-Laufzeit, FFmpeg/ffprobe, MP4Box und FFMS2 nicht versehentlich gebündelt. Geprüfter Vier-DLL-libmpv-Satz und Lizenzhinweise vorhanden.
- **250 Portable-Dateien** aus dem endgültigen ZIP extrahiert und jede SHA-256 gegen das Publish-Manifest geprüft, keine zusätzlichen Dateien. Benutzeranleitung, bereinigte Voraussetzungen und **RELEASE-NOTES.md** beiliegend. Setup mit **Inno Setup 7** aus genau diesem Publish-Verzeichnis erfolgreich kompiliert. Keine Installation ins laufende Nutzersystem durchgeführt.
- **13 native Fallgruppen** gegen die endgültigen entpackten Programmbits: stabile Versionsanzeige; Layout bei drei Größen mit demselben HWND und erhaltenem Schnittzustand/Position; Splitter, schmale Fenster und laufender Ansichtswechsel; OLE-Drop samt Ablehnungen und bestätigtem AVI-Laden; abgebrochener Medienwechsel ohne Zustandsverlust; AVI-Schutz in beiden Ansichten und Rückkehr zu MP4; Keyframe-Navigation, Halbierung und Lupenvorschau in beiden Ansichten; leere/40-zeilige Liste, volle Zeitwerte und feste Lupe; tatsächliche Schnitt-/Löschklicks und Werkzeugdialoge.
- Ausgabe-, Erzeugungs- und Namensmaskendialoge sowie lokale Dateiauswahl abgebrochen; Upload-/Server-Schutz geprüft. Kein echter Upload, erneuter Onlineabruf, gespeicherter Schnitt oder vollständiger Filmschnitt aus dem Prüfhelfer. **Keine WPF-Bindungsfehler**; rechte Werkzeugdarstellung gerendert und visuell kontrolliert. Alle **13 persönlichen Einstellungsdateien** vor/nach der abschließenden Prüfung bytegleich.
- Vier CAN-Assemblies und vier native DLLs des Helfers per SHA-256 mit dem endgültigen Portable verglichen. Nach dem letzten Quellenbau wurde der Helfer gegen die neuen DLLs neu gebaut und die vollständige native Prüfung wiederholt; frühere Prüf-DLLs wurden nicht als Nachweis übernommen. Der erste Helfer-Build hatte noch unvollständig umgestellte Referenzpfade; diese wurden ausschließlich im isolierten Prüfprojekt korrigiert. Produktbuild und Tests waren erfolgreich.
- Stabile Setupnamen ohne leeren Suffix geprüft. Der RC-Zweig der aktualisierten Setupdefinition wurde zusätzlich mit dem echten Compiler und vorhandenen alten Publish-Dateien in einem isolierten Testordner erfolgreich kompiliert; alte Release-Dateien nicht überschrieben.
- Motor **otr-can 0.1.5** unverändert; EXE-SHA-256 **FC6ED6568880A995F3D141A9378AA2CE28ADE8DC0E257E93DC972799BB1324A6**, Version und Fähigkeit `mp3-aac-v1` geprüft. Passende Quellen aus **2a474629836d52023bf37cbbdb309ccd1c23acf3** sowie Lizenz-/Provenienzpaket wiederverwendet. Keine neue bitidentische Reproduzierbarkeit der Motor-EXE behauptet.
- Sechs Archive und `SHA256SUMS.txt` vollständig nachgerechnet und schreibgeschützt eingefroren. **70 Dateien älterer lokaler Release-Sätze** unverändert; alle acht lokalen Lockdateien erhalten. Lokaler erfolgreicher Buildzähler nach bestandenem endgültigem Paketlauf auf **32** fortgeschrieben.

## Auslieferungsdateien und SHA-256

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-1.0.0-Build32-source.zip | 3160830 | 8EF486F084A37C626E8A5F277D9F0510F84A7E4E912BA6C79DA30AEA139DBF92 |
| CutAssistantNext-1.0.0-Build32-win-x64-Setup.exe | 20325270 | 015E9C814D0D747F7B55269F955DEC1977757086644B040E4D3A50CBBF1759AB |
| CutAssistantNext-1.0.0-Build32-win-x64.zip | 25005369 | 92229B0AFF563FD0D4D6203F84062BFE90B39C060301FC706BDFED14372DA60A |
| otr-can-0.1.5-source.zip | 63988 | 3A973278ABDF719B745405D9DFBB930C14A28CFEF40C7FB52A35CA17F7214E57 |
| SHA256SUMS.txt | 647 | D5DB5970657B3DF582DF3EF34288980383DEDDE7FE0F13BD37C38C61F75B1777 |

`SHA256SUMS.txt` enthält die sechs Archive; seine eigene Prüfsumme steht hier und im lokalen Artefaktnachweis. Das Manifest enthält aus technischen Gründen keinen Selbsthash.

## Persönliche Abnahme und verbleibende Schritte

Persönlich bestätigt: Kommandozeilenstart mit realer HD-Aufnahme, Drag & Drop, Power-User-Layout, Frame-Lupe, Frame-Liste und erfolgreiche Schnitte mit beiden Motoren. Radical mit otr-can 0.1.5 nach Bild, Ton und allen Übergängen blitzfrei. Der bekannte **MP4Box-Werbeblitzer bei 57:27.314** bleibt offen und dokumentiert; ebenso der ältere MP4Box-Befund bei zwei gleichartigen AAC-Spuren. Kein pauschaler Qualitätsnachweis aller Formate oder aller Tonspuren daraus abgeleitet.

**Noch erforderlich:** persönliche Prüfung dieses neuen Setup-Pakets, Deinstallation und Portable-Ausgabe in einer Windows-11-VM. Frühere Build-31-Paketabnahmen gelten für die früheren Archive. Echte DPI-/Mehrmonitorwechsel sind nicht persönlich bestätigt. Push, Git-Tag und stabile GitHub-Veröffentlichung benötigen den gesonderten Nutzerauftrag; noch keine Uploads ausgeführt.

AVI bleibt experimentell; bestehende Bedienungssperren und Zeitstrukturgrenzen bleiben erhalten. Externe .NET-10-Desktop-/VC++-Laufzeiten und Medienwerkzeuge sind erforderlich. [Freeze-Umfang](FREEZE-1.0.0-20261016.md), [Release Notes](RELEASE-NOTES-1.0.0.md), [VM-Übergabe](UEBERGABE-CAN-1.0.0-BUILD32-20261016.txt).

## Lokale Nachweise

`.build/freeze-1.0.0-build32-20261016`: `PACKAGE-FINAL.log`, `PACKAGE-INPUTS.json`, `SOURCE-COMMIT.txt`, `SOURCE-ZIP-PROOF.json`, `PACKAGE-TESTS.json`, `PORTABLE-PROOF.json`, `UI-QA-FINAL-ASSEMBLIES-BUILD.log`, `UI-QA-FINAL.log`, `proof/PROOF.json`, leeres `proof/BINDINGS.log`, Werkzeug-/Fensteraufnahmen, `SETTINGS-FINAL-BEFORE.json`, `PRESERVED.json`, `ARTIFACTS.json` und `FINAL.json`. Finale Build-/Test-/Publish-/Installerlogs und TRX außerdem im Paketausgabeordner. Der vorausgegangene Paketlauf ist separat im Nachweisordner erhalten; maßgeblich sind ausschließlich die finalen Archive und die oben aufgeführten Prüfsummen.
