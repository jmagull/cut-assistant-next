# CAN 1.0.2 Build 45 – eingefrorener Paketsatz

**29.10.2026: Setup, Portable und passende Quellen vollständig erstellt, technisch geprüft und auf Nutzerwunsch eingefroren. Keine Veröffentlichung.** Dateiversion **1.0.2.45**, unverändert **otr-can 0.1.6 / libmpv 0.41.0**. Öffentlich bleibt Build 41. [Freeze-Auftrag](FREEZE-1.0.2-BUILD45-20261029.md), [Bedienung](NUTZERANLEITUNG.md), [Releasehinweise](RELEASE-NOTES-1.0.2.md).

## Eingefrorener Umfang

Klebecenter-Menü nur mit **Filmteile zusammenfügen …**; Tonspuren direkt dort wählen. Neuer Haken **„Dieses Tonspurpaar übernehmen (nur bei übereinstimmendem Tonformat)“**, mit Textumbruch und Hinweis auf einmalige Spurzuordnung und Abwahl nicht benötigter Paare. Nutzer hat den Build-45-Screenshot persönlich bestätigt und anschließend Setup/ZIP/Quellen sowie Freeze beauftragt. QUICK AND COPY, AUDIO-ENTHUSIAST, Paarvorschläge, Bestätigung und Doppelbelegungsschutz unverändert. Kein Produktcode bei der Paketierung geändert.

Exakter Quellsnapshot aus Basiscommit **d200ebf7567bceeab012ee9a2bb6b564635382a4** plus 43 manifestierten Arbeitsänderungen. **690 manifestierte Quelldateien / 691 Dateien im Quellen-ZIP**, **250 Portable-Dateien**. Vier vorbestehende Produktions-Lockänderungen bleiben im Arbeitsrepository unverändert und sind aus dem Snapshot ausgeschlossen; dieser verwendet committed Locks. Keine neue Produktionsabhängigkeit, keine Filmdateien oder privaten Einstellungen im Quellen-ZIP. Zähler bleibt 45.

Ausgabe **artifacts/release-1.0.2-build45**: Setup, Portable-ZIP, exakte CAN-Quellen, passende Motorquellen, libmpv-Prüfquellen/Relink und zwei Prüfsummenmanifeste. **Alle acht obersten Auslieferungsdateien schreibgeschützt.** Zusätzlich die **690 manifestierten Snapshot-Dateien plus SOURCE-FILES.sha256** schreibgeschützt; Hashes weiterhin identisch. Alle 371 Produkt-/Testquellen gegenüber Freeze-Auftragseingang unverändert. Quellsnapshot, Archive und Nachweise bleiben für diesen Stand erhalten.

## Prüfungen

- Snapshot-Restore im Locked Mode, Release-Build und Publish erfolgreich; **0 Warnungen / 0 Fehler**. App-Dateiversion **1.0.2.45** bestätigt. Inno Setup 7 aus dem geprüften Publish-Verzeichnis erfolgreich kompiliert.
- **1552 Pakettests:** 200 Core, 72 Cutlists, 351 Media, 929 App; keine Fehler oder übersprungenen Tests.
- Zusätzlich **1280 Tests gegen exakt finale Paket-DLLs:** 929 App und 351 Media; alle vier Produktbibliotheken bytegleich zu den ausgelieferten Dateien.
- **28 native QUICK- und 21 native AUDIO-ENTHUSIAST-Fälle** gegen finale Paketbibliotheken positiv. Mehrspur AAC+AC-3 ausdrücklich automatisch vorgeschlagen, bestätigt und in beiden Reihenfolgen erhalten; Einzelwahl, beschädigte gewählte/entfallende Spur, Fehler-/Warnungsablehnung, Abbruch, vorhandene/rivalisierende Ausgaben und Bereinigung geprüft. Negative Fälle nur bei korrekter Ablehnung positiv gezählt.
- Tatsächliches kompiliertes MainWindow-Menü mit genau einem Zusammenfügen-Eintrag geprüft; Dialog ohne Standardverfahren, Mehrspurvorschläge, Bestätigungspflicht, erneute Bestätigung nach Änderung/Tausch und sichtbare entfallende John-Wick-Spur. Nutzereinstellungen nicht gespeichert.
- **Vier native Hinweis-/Auswahlfälle** bei 680 und 900 DIP, John Wick nur AAC sowie Furiosa AAC+AC-3. Checkboxtext und Textumbruch sichtbar geprüft, korrekte Spurzahl erhalten, doppelte AAC-Zuordnung weiter gesperrt; Abwahl verlangt erneute Bestätigung.
- Echte libmpv-Prüfung aus dem Paket: AAC-ID 1 und AC-3-ID 2 gelesen, beide umgeschaltet und zurückgelesen; vorhandene Furiosa-Referenz im unsichtbaren und stummgeschalteten Prüfplayer. Keine zusätzliche persönliche Filmabnahme daraus abgeleitet.
- Quellenmanifest und Quellen-ZIP vollständig verglichen; alle 250 Portable-Nutzdateien gegen Manifest und ZIP geprüft. Fünf Prüfhilfen verwenden bytegleich die finalen Produkt-DLLs. Native DLLs, Drittquellen, Lizenz-/Herkunftshinweise und Voraussetzungen kontrolliert; kein unerwünschtes .NET-Runtime-/FFmpeg-/MP4Box-/FFMS-Bündel.
- Motor direkt **otr_can 0.1.6**, Fähigkeiten **mp3-aac-v1 / aac-overlap-v1**. EXE-SHA **73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E**; passende unveränderte Quellen enthalten.
- **42 geschützte Dateien** unverändert: acht ursprüngliche Locks, Version.props, Buildzähler und 32 oberste Auslieferungsdateien der Builds 40/41/43/44. Kein Commit, Push, Tag, Upload oder Release.

## Persönliche Abnahme und Grenzen

Neuer Hinweis persönlich bestätigt. Zuvor zehn Build-44-Setup-Funktionsfälle auf dem Arbeitsrechner positiv: vier HD-Aufnahmen jeweils mit beiden Motoren sowie John Wick in beiden Klebeverfahren. QUICK dort auf Ton/Übergang, AUDIO-ENTHUSIAST auf Bild/Ton/Übergang bestätigt; doppelte Anhänge als jeweils einen Lauf gezählt. **Neue Build-45-Setup-/Portable-/VM-Abnahme bleibt getrennt offen.** [Praxisnachweise](KLEBECENTER-TONSPURHINWEIS-20261029.md).

Laufzeitabhängiges Paket: .NET 10 Desktop Runtime x64 separat. Unter Einstellungen → OTR-CAN-Werkzeuge die mitgelieferte **tools/otr-can/otr_can.exe** wählen; ältere gespeicherte Pfade können auf andere Kopien zeigen. QUICK erhält komprimierte Paketnutzdaten mit den dokumentierten begrenzten Tonabweichungen; keine PCM-Identitätszusage. Unbekannte Warnungen und Decoderfehler bleiben Abbruchgründe. Keine allgemeine Freigabe beliebiger Medienformate.

Archive bewahren den Dokumentationsstand bei Quellsnapshot-Erstellung vor Abschluss der Paketprüfung. Dieser Arbeitsbericht ergänzt die späteren vollständigen Ergebnisse und Prüfsummen; eingefrorene Archive und Quellsnapshot werden dafür nicht nachträglich verändert.

## Auslieferungsdateien

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build45-source.zip | 3608596 | 70635D55F04FC5D86E562E625B634867A68F1A6B5ED6E34AAAAA3B239C8180EC |
| CutAssistantNext-1.0.2-Build45-win-x64.zip | 25228451 | FB587AC5733A9DC7BAF71F6F35055EBBFD781736F3FE3DA1D70FCED95CDC7807 |
| CutAssistantNext-1.0.2-Build45-win-x64-Setup.exe | 20501968 | 38B0F79525D399A8A2C73A4A07C80DC452D0FDB9F372EABB8301184451A7D579 |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

**SHA256SUMS.txt** beschreibt diese sechs Assets; **PUBLISH-FILES.sha256** die 250 Portable-Dateien. Nachweise unter **.build/release-1.0.2-build45-20261029/**: PACKAGE-INPUTS.json, PACKAGE-PROOF.json, FINAL.json, SOURCE-CODE-FROZEN.json, TRX-Ergebnisse, native Fall-/Menü-/Dialog-/Hinweis-/Playernachweise und Sicherung geschützter Dateien.
