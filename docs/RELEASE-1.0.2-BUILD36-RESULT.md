# CAN 1.0.2 · Build 36 – lokaler Paketbericht

**Stand 25.10.2026: erstellt und technisch geprüft, nicht veröffentlicht.** Kein Tag, kein GitHub-Upload und keine Release-Freigabe. Der Quellcommit ist lokal gesichert; der Push steht aus.

## Änderung und Herkunft

Quellcommit: fedf3b3ab0ef1470c202ddbceb8c029fc3f3cecb. Die Cutlist-Erstellung bietet eine sichtbare Motorwahl. CAN schlägt den zuletzt erfolgreich für dieselbe Aufnahme verwendeten Motor vor, sonst den Motor einer geladenen Cutlist. Für otr-can werden dessen EXE-Dateiname und die per Versionsabfrage ermittelte Version eingetragen. Die vier vorbestehenden lokalen Änderungen an Produktions-Lockdateien sind nicht im Commit oder Quellen-ZIP.

Version 1.0.2, Build 36; keine neue Produktionsabhängigkeit. Der unveränderte otr-can-Motor ist 0.1.6, libmpv bleibt 0.41.0. Build 35 und alle zuvor erfassten Dateien blieben bytegleich. Der lokale Buildzähler wurde erst nach erfolgreicher Paketprüfung von 35 auf 36 erhöht.

## Technische Prüfung

- Restore mit gesperrten Abhängigkeiten, Release-Build und Publish aus dem festen Quellsnapshot erfolgreich; Build mit 0 Warnungen und 0 Fehlern.
- 1416/1416 Solution-Tests bestanden, keine Fehler oder übersprungenen Tests.
- 898/898 App-Tests zusätzlich gegen die Produkt-DLLs des finalen Portable-Pakets bestanden.
- 626 Quelldateien im SHA-256-Manifest, 627 Dateien im Quellen-ZIP gegen den Snapshot bytegleich geprüft.
- 250 Portable-Dateien vollständig gegen das Publish-Manifest und ZIP geprüft.
- Installer mit Inno Setup 7 erfolgreich kompiliert; Apphost meldet Dateiversion 1.0.2.36. Der Installer wurde nicht automatisch installiert.
- 19 vorbestehende Dateien vor der Zählerübernahme hashgleich, darunter Build-35-Pakete und die vier lokalen Produktions-Lockdateien. Danach wurde nur der lokale Buildzähler absichtlich geändert.
- otr-can-EXE und alle drei übernommenen Quellenarchive haben dieselben SHA-256-Werte wie im vorherigen Paket.

## Lokale Auslieferungsdateien

Alle Dateien liegen unter artifacts/release-1.0.2-build36. SHA256SUMS.txt erfasst die sechs Nutzdateien; PUBLISH-FILES.sha256 bleibt der zusätzliche lokale Detailnachweis.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build36-source.zip | 3391122 | C661260486DEF4F744B6D60F0E94D704B39133C26128D0BE72DB8A045EC3E62C |
| CutAssistantNext-1.0.2-Build36-win-x64.zip | 25077136 | 4C31DD6FFA32CC281B7C12BD61D81DBDE8E27BF8737458C0D572D7F552BC5734 |
| CutAssistantNext-1.0.2-Build36-win-x64-Setup.exe | 20380285 | 29BB90F818751EEB75F76899973AC91B9DE64AA0E072AF53D9AF49FAA90714BE |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

## Offene Abnahme

Die persönlichen Bild-/Ton-/Schnitt-/Vorschauprüfungen früherer Peppermint-Läufe gelten für deren damals verwendete Programmdateien. Build 36 wurde noch nicht persönlich als Setup oder Portable gestartet. Vor einer späteren Freigabe sind Installation/Start/Versionsanzeige, eine neu gespeicherte otr-can-Cutlist mit den drei korrekten Motorfeldern und bei Bedarf VM-Deinstallation sowie Portable-Pfade an den tatsächlichen Build-36-Dateien zu prüfen.

Die Ursache der früheren vollständigen Rechnerstillstände ist weiterhin ungeklärt. Build 36 enthält dafür keine Änderung. Keine Veröffentlichung ohne ausdrückliche Freigabe.

Lokale Detailnachweise: .build/release-1.0.2-build36-20261025/PACKAGE-INPUTS.json, PACKAGE-PROOF.json, FINAL.json, PRESERVED-BEFORE.json, PRESERVED-AFTER.json und final-app-results/final-app.trx. Paketlogs und Solution-TRX liegen unter artifacts/release-1.0.2-build36.
