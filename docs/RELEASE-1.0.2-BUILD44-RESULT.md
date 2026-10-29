# CAN 1.0.2 Build 44 – Paketprüfung

**29.10.2026: lokaler Paketsatz vollständig erstellt und technisch geprüft. Keine Veröffentlichung.** Dateiversion **1.0.2.44**, unveränderte Komponenten **otr-can 0.1.6 / libmpv 0.41.0**. Öffentlich bleibt Build 41. [Quellumfang](FREEZE-1.0.2-BUILD44-20261029.md), [Bedienung](NUTZERANLEITUNG.md), [Releasehinweise](RELEASE-NOTES-1.0.2.md).

## Inhalt und Herkunft

Setup, Portable-ZIP, exakte CAN-Quellen, passende Motorquellen und libmpv-Prüfquellen/Relink mit Prüfsummen unter **artifacts/release-1.0.2-build44**. Klebecenter-Menü nur mit **Filmteile zusammenfügen …**; Tonspuren direkt dort wählen. Nutzer hat die Menüvereinfachung mit „tip top gecheckt“ persönlich bestätigt. QUICK AND COPY, AUDIO-ENTHUSIAST und eigenständiger Kopierdienst gegenüber Build 43 unverändert. Letzterer bleibt im Code für eine mögliche spätere Werkzeugrubrik erhalten.

Manifestierter Snapshot aus Basiscommit **d200ebf7567bceeab012ee9a2bb6b564635382a4** plus 40 dokumentierten Arbeitsänderungen. **687 manifestierte Quelldateien / 688 Dateien im Quellen-ZIP**, **250 Portable-Dateien**. Vier vorbestehende Produktions-Lockänderungen unverändert erhalten und aus dem Snapshot ausgeschlossen; dieser verwendet committed Locks. Keine neue Produktionsabhängigkeit, keine Filme oder privaten Einstellungen im Quellenarchiv. Zähler unverändert 44; frühere Pakete erhalten.

## Prüfungen

- Snapshot-Restore im Locked Mode, Release-Build und Publish erfolgreich; **0 Warnungen / 0 Fehler**. Inno Setup 7 aus dem geprüften Publish-Verzeichnis erfolgreich kompiliert. App-Dateiversion **1.0.2.44**.
- **1552 Pakettests:** 200 Core, 72 Cutlists, 351 Media, 929 App; keine Fehler oder übersprungenen Tests.
- Zusätzlich **1280 Tests gegen exakt finale Paket-DLLs:** 929 App und 351 Media. Alle vier Produktbibliotheken bytegleich zur ausgelieferten Portable-Ausgabe.
- **28 native QUICK-Fälle und 21 native AUDIO-ENTHUSIAST-Fälle** gegen finale Paketbibliotheken erfolgreich. Automatische AAC-/AC-3-Paarvorschläge, bestätigter Erhalt beider Spuren in beiden Reihenfolgen, bewusste Einzelwahl, Fehler-/Warnungsablehnung, beschädigte gewählte/entfallende Spur, Abbruch, Ausgabeschutz und Bereinigung geprüft. Negative Fälle nur bei korrekter Ablehnung als positiv gezählt.
- Tatsächliches kompiliertes MainWindow-Menü nativ geprüft: genau ein Eintrag zum Zusammenfügen, keine separate Tonspurkopie. Zusammenfügen-Dialog: zwei Verfahren ohne Standardauswahl, beide Tonspuren vorgeschlagen, Bestätigungspflicht, erneute Bestätigung nach Änderung/Tausch und sichtbare John-Wick-Auslassung. Kein Speichern der Nutzereinstellungen.
- Native libmpv-Prüfung mit den Paketdateien: AAC-ID 1 und AC-3-ID 2 erkannt, beide gewählt und zurückgelesen; vorhandene Furiosa-Referenz im unsichtbaren und stummgeschalteten Fenster verwendet. Ein erster Start der isolierten Prüfhilfe über `dotnet` konnte abhängige native DLLs nicht laden; anschließend Start über ihren Windows-Apphost erfolgreich. Erstes Fehlerprotokoll behalten; keine Änderung an Produkt oder Paketen erforderlich.
- Quellenmanifest und Quellen-ZIP vollständig abgeglichen, sämtliche 250 Portable-Nutzdateien gegen Manifest und ZIP geprüft. Native DLLs, Drittquellen, Lizenzen/Herkunft und Voraussetzungen kontrolliert; kein unbeabsichtigtes .NET-Runtime-/FFmpeg-/MP4Box-/FFMS-Bündel.
- Mitgelieferter Motor meldet **otr_can 0.1.6**, Fähigkeiten **mp3-aac-v1** und **aac-overlap-v1**. EXE-SHA **73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E**. Unveränderte passende Motorquellen enthalten.
- **34 geschützte Dateien** unverändert: acht ursprüngliche Locks, Version.props, Zähler und 24 oberste Auslieferungsdateien der Builds 40/41/43. Alle 371 Produkt-/Testdateien gegenüber Auftragseingang unverändert. Kein Commit, Push, Tag, Upload oder Release.

## Persönliche Abnahme und Grenzen

Menüänderung persönlich bestätigt; **neue Setup-/Portable-/VM-Abnahme bleibt offen**. Frühere Furiosa-QUICK-Prüfung auf Bild, Ton und Drei-Sekunden-Klebeübergang bleibt separat dokumentiert. Technische Paketprüfungen ersetzen keine persönliche Bild-/Ton-/Synchronitätskontrolle.

Paket ist laufzeitabhängig: .NET 10 Desktop Runtime x64 separat erforderlich. Unter Einstellungen → OTR-CAN-Werkzeuge den mitgelieferten Motor **tools/otr-can/otr_can.exe** wählen; ältere gespeicherte Pfade können auf andere Kopien zeigen. QUICK bewahrt komprimierte Nutzdaten mit den dokumentierten begrenzten Tonabweichungen, keine PCM-Identitätszusage. Unbekannte Warnungen und Decoderfehler bleiben Abbruchgründe. Keine neue allgemeine Formatfreigabe.

Archive behalten den Dokumentationsstand bei Quellsnapshot-Erstellung vor Abschluss der Paketprüfung. Dieser Arbeitsbericht ergänzt nachträglich die Ergebnisse und Prüfsummen, ohne Archive zu verändern.

## Auslieferungsdateien

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build44-source.zip | 3597865 | 3F4F625033CA271A39836D8A78D9C8C5B8908A692F0308AB54668B08D89545E8 |
| CutAssistantNext-1.0.2-Build44-win-x64.zip | 25227904 | 8B4866A480FB0CDE98D8A37A5355F78723E720483ECDBD6BB03F1E7DA483D959 |
| CutAssistantNext-1.0.2-Build44-win-x64-Setup.exe | 20502768 | 35C8D8F0F1C6AE60708CC7BF7395178389A7FCB98C0FAE817D8B1EE89C12DAFE |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

Alle sechs Assets stehen in **SHA256SUMS.txt**; **PUBLISH-FILES.sha256** beschreibt die 250 Portable-Nutzdateien. Die acht obersten Auslieferungsdateien werden nach erfolgreicher Endprüfung schreibgeschützt.

Nachweise: **.build/release-1.0.2-build44-20261029/** mit PACKAGE-INPUTS.json, PACKAGE-PROOF.json, TRX-Ergebnissen, nativen Fallnachweisen, Dialog-/Menü-/Playerprüfung und Sicherung geschützter Dateien.
