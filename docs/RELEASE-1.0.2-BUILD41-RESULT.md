# CAN 1.0.2 Build 41 – Klebecenter-Bereinigung

Stand 27.10.2026: als v1.0.2 / Latest veröffentlicht, Commit/Push abgeschlossen. Nach Erfolg Arbeitsordner entfernen, bei Fehler/Abbruch Diagnose erhalten. Medienverarbeitung, Motor otr-can 0.1.6 und libmpv 0.41.0 unverändert. [Änderung und Prüfung](KLEBECENTER-BEREINIGUNG-20261027.md), [Umfang](FREEZE-1.0.2-BUILD41-20261027.md).

## Technische Nachweise

Release-Build ohne Warnungen/Fehler; 1489 Pakettests (200 Core, 72 Cutlists, 292 Media, 925 App). Zusätzlich 1217 Tests gegen die finalen Produktbibliotheken und 21 native Klebecenter-Fälle mit Erfolgs-/Fehler-/Abbruchbereinigung bestanden. Vier neue Regressionstests decken Erfolg, Fehler, gesperrte Arbeitsdatei und fremde Ordnerinhalte ab. Quell-/Portable-Inhalte, Manifeste und Motorhash geprüft; App-Dateiversion 1.0.2.41.

Basiscommit 1631aa7367a328bf86fd291a54a6c62f8731626f plus dokumentierte Arbeitsänderungen. Vier vorbestehende Produktions-Lockänderungen erhalten, aus dem Snapshot ausgeschlossen. Anfangsfehler beim gesperrten Restore erkannt und nach Rücknahme ausschließlich selbst erzeugter Test-Lockänderungen behoben; Fehlversuch bleibt lokal dokumentiert.

## Pakete und Abnahme

Setup, Portable-ZIP, CAN-Quellen und unveränderte Motor-/libmpv-Quellen samt Relink/Prüfsummen unter **artifacts/release-1.0.2-build41**. Frühere Pakete einschließlich Build 40 erhalten. Dieser Bericht ergänzt nach endgültigem Dokumentationsabgleich die Paketdateien und Prüfsummen; der Quellsnapshot zeigt seinen Erstellungsstand.

Build-40-Setup und -ZIP persönlich positiv geprüft; persönliche Setup-Abnahme der neuen Build-41-Bereinigung auf dem Arbeitsrechner mit John Wick nun positiv: temporäre Datei weg, Bild/Ton/Schnitt/Drei-Sekunden-Vorschau und Klebecenter bestätigt. Neue ZIP-/VM-Prüfung bleibt getrennt offen. Keine automatische Installation; Commit/Push und Veröffentlichung sind nach ausdrücklicher Freigabe abgeschlossen. Lokale Nachweise unter **.build/release-1.0.2-build41-20261027/**.

## Abschließender Paketnachweis

**CAN 1.0.2 Build 41 erstellt und vollständig technisch geprüft.** 1489 Pakettests, zusätzlich 1217 Tests gegen finale Produktbibliotheken, 21 native Klebecenter-Fälle. Native Prüfbibliotheken sind bytegleich zu den final ausgelieferten Core-/Media-DLLs. **668 Quelldateien** im Manifest, **669 Dateien** im Quellen-ZIP und **250 Portable-Dateien** geprüft; 55 geschützte Dateien einschließlich früherer Pakete, Nutzer-Lockdateien und Zähler unverändert. Buildzähler 41. Acht oberste Paketdateien schreibgeschützt; keine Veröffentlichung.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build41-source.zip | 3519382 | 591324E1AB879491BAB76D13FE6954B7AC6372CE5BB5A691609CD82049B50915 |
| CutAssistantNext-1.0.2-Build41-win-x64.zip | 25170942 | 48696B86715F8167B49DFA9DBE1864BBD3334AD3F0BB4A46FDCFF734E2D20E1A |
| CutAssistantNext-1.0.2-Build41-win-x64-Setup.exe | 20453526 | 0A00B4040B985532CB8A08D0181ADFB0B8AF71F6DC488176678DD880D5645A0C |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

## Veröffentlichungsfreigabe

Am 27.10.2026 beauftragt der Nutzer ausdrücklich Commit, Push und Veröffentlichung dieses unveränderten Build-41-Paketsatzes. README, Releasebeschreibung und Klebecenter-Anleitung ergänzen aktuelle Abnahme, Motorversion 0.1.6 und Pfadprüfung nach Überinstallation. Archivdokumente behalten ihren damaligen Erstellungsstand; nach Upload folgt ein eigener Veröffentlichungsnachweis.

## Veröffentlichung abgeschlossen

Nach ausdrücklicher Freigabe am 27.10.2026 als **v1.0.2 / Latest** veröffentlicht. Alle acht Assetgrößen und GitHub-SHA-256-Prüfsummen stimmen mit dem unveränderten geprüften Paketsatz überein. [Veröffentlichungsnachweis](RELEASE-1.0.2-PUBLISHED-20261027.md).
