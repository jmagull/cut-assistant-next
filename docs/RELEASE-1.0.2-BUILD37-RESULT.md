# CAN 1.0.2 · Build 37 – lokaler Paketbericht

**Stand 26.10.2026: erstellt und technisch geprüft, nicht veröffentlicht.** Keine automatische Installation. Der automatische AAC-Notfallablauf ist nach Bereitstellung der Pakete persönlich bestätigt; separate neue Portable-/VM-/Deinstallationsfälle und persönliche Abbruch-/Fehlerfallprüfungen bleiben offen. Kein Tag, Upload oder Release wurde ausgeführt.

## Inhalt und Herkunft

Build 37 enthält den AAC-Notfallweg nach erkannter otr-can-Tonaufbereitungsstörung: ausdrückliche Spurauswahl, vollständige strenge AAC-Prüfung, Arbeitskopie ohne Neukodierung mit exaktem Paket-/Zeitstempelvergleich, kontrollierter erneuter Schnitt und sichtbarer Hinweis auf entfallene Tonspuren. Gesunde Aufträge behalten alle Tonspuren.

Motor **otr-can 0.1.6**, libmpv **0.41.0** und deren Quellenarchive bleiben unverändert. Keine neue Produktionsabhängigkeit. Die ursprüngliche Entwicklungsprüfung des John-Wick-Teil-2-AAC-Films wurde anschließend durch einen erfolgreichen vollständigen CAN-Bedienlauf des Nutzers ergänzt; Installationstyp und zusätzliche Paketvarianten sind davon getrennt zu betrachten.

Fester Quellsnapshot aus Basiscommit **1631aa7367a328bf86fd291a54a6c62f8731626f** plus den in SOURCE-SNAPSHOT.json aufgeführten aktuellen Arbeitsänderungen. **IncludesWorkingTreeChanges=true**: Der neue Produktcode ist zum Paketierungszeitpunkt noch nicht committed oder gepusht. Vier vorbestehende lokale Produktions-Lockänderungen wurden erhalten und aus dem Snapshot ausgeschlossen; gebaut wurde mit den gesperrten Lockdateien aus Git. Das Quellen-ZIP ist der genaue gebaute Stand, geprüft über SOURCE-FILES.sha256.

## Technischer Nachweis

- Restore mit gesperrten Abhängigkeiten, Release-Build und Publish aus dem festen Snapshot erfolgreich; **0 Warnungen, 0 Fehler**.
- **1429/1429 Solution-Tests** erfolgreich, keine übersprungenen Tests.
- Zusätzlich **908/908 App-Tests und 249/249 Medientests** gegen bytegleich verifizierte endgültige Produkt-DLLs des Portable-Pakets erfolgreich.
- Vier native Normal-/Fehlerfälle durch endgültige CAN-Paketbibliotheken und beigefügten Motor erfolgreich: beide Spuren bei gesundem Material; bestätigter AAC-Notfallweg; Ablehnung ohne Ausgabe; beschädigtes AAC weiter abgelehnt.
- Finaler WPF-Rückfragedialog aus den Paketbibliotheken gerendert und auf vollständige Texte/Schaltflächen geprüft. Die ursprüngliche automatische Dialogprüfung wurde später durch den persönlich bestätigten vollständigen Rückfrage-/Schnittablauf ergänzt.
- **636 Quelldateien** im Manifest, **637 Dateien** im Quellen-ZIP bytegleich zum Snapshot geprüft.
- **250 Portable-Dateien** vollständig gegen Publish-Manifest und ZIP geprüft.
- Installer mit Inno Setup 7 erfolgreich kompiliert; Apphost-Dateiversion **1.0.2.37**. Setup wurde nicht automatisch installiert.
- **21 vorbestehende Dateien** bytegleich erhalten, darunter Build-35-/Build-36-Pakete, vier lokale Produktions-Lockdateien und bisheriger Motor. Buildzähler erst nach erfolgreicher Paketprüfung von 36 auf 37 übernommen.
- Acht oberste Paketdateien einschließlich Prüfsummen sind schreibgeschützt. Die sechs Nutzdateien sind in SHA256SUMS.txt erfasst.

## Dateien

Ordner: **artifacts/release-1.0.2-build37**.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build37-source.zip | 3414357 | A6BFB7E05B912A06270572200F9189255EFC0805D998691E129E37361A753853 |
| CutAssistantNext-1.0.2-Build37-win-x64.zip | 25106836 | 5E3AE678BEADCC0DB3E39CC15B67E1FFD02B76293006A8A70AFADF5C73FE1546 |
| CutAssistantNext-1.0.2-Build37-win-x64-Setup.exe | 20399018 | 8D5B432EEEB719215B59328F71A6F62F06B0D3ECC37D1094F230CBD2C6A16AC3 |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

## Persönliche Abnahme und verbleibende Prüfungen

Nach Bereitstellung von Build 37 hat der Nutzer den John-Wick-Teil-2-Fehlerfall in CAN vollständig durchlaufen und mit **„alles top“** bestätigt. Das Protokoll belegt ursprünglichen AC-3-Abbruch, ausdrückliche AAC-Auswahl, vollständige Prüfungen und erfolgreichen erneuten Schnitt. Der Screenshot zeigt den korrekten Hinweis auf die entfallene AC-3-Spur sowie Anfang, Übergang und Ende. Separate neue Portable-/VM-/Deinstallationsfälle und persönliche Abbruch-/Fehlerfallprüfungen bleiben offen. Die Rückmeldung belegt nicht sämtliche Installationsvarianten.

Die Auswahl im nativen Pakettest wurde ausdrücklich simuliert. Der Prüfweg repariert keine defekte Tonspur; auch Fehler in entfernten Bereichen der gewählten AAC-Spur führen weiterhin zum Abbruch. Die Ursache früherer vollständiger Rechnerstillstände bleibt ungeklärt; Build 37 enthält dafür keine behauptete Korrektur.

Keine Veröffentlichung ohne ausdrückliche Freigabe. Die bisherigen Installationsdateien und Nutzereinstellungen wurden nicht geändert.

Lokale Detailnachweise unter **.build/release-1.0.2-build37-20261026/**: PACKAGE-INPUTS.json, PACKAGE-PROOF.json, PRESERVED-BEFORE.json, PRESERVED-AFTER.json, FINAL.json, FINAL-NATIVE.json, final-package-results/ und native-cases/. Paketlogs und Solution-TRX befinden sich beim neuen Paketsatz.

[Release Notes](RELEASE-NOTES-1.0.2.md), [Notfallweg und Filmprüfung](OTR-CAN-AAC-NOTFALLWEG-20261026.md), [Paketauftrag](FREEZE-1.0.2-BUILD37-20261026.md).

## Dokumentationsnachtrag

Dieser Nachtrag vom 26.10.2026 aktualisiert die Arbeitsdokumentation nach der persönlichen Rückmeldung. Eingefrorene Build-37-Pakete, Quellsnapshot und Prüfsummen bleiben unverändert. [Aktueller Stand](AKTUELLER-STAND.md).
