# CAN 1.0.2 Build 43 – Paketprüfung

**28.10.2026: lokaler Paketsatz vollständig erstellt und technisch geprüft. Keine Veröffentlichung.** Dateiversion **1.0.2.43**, unveränderte Komponenten **otr-can 0.1.6 / libmpv 0.41.0**. Öffentlich bleibt CAN 1.0.2 Build 41. [Umfang](FREEZE-1.0.2-BUILD43-20261028.md), [Funktion und Grenzen](KLEBECENTER-QUICK-COPY-20261028.md), [Releasehinweise](RELEASE-NOTES-1.0.2.md).

## Enthalten

Setup, Portable-ZIP und exakte CAN-Quellen; dazu passende unveränderte Motorquellen, libmpv-Prüfquellen/Relink und SHA-256-Prüfsummen. Ausgabe **artifacts/release-1.0.2-build43**. QUICK AND COPY mit bestätigter AAC-/AC-3-Mehrspurzuordnung, AUDIO-ENTHUSIAST mit unverändertem Build-42-Verfahren, Phasenzeiten, Tonspurwahl im Kontrollfenster und zusätzlicher unabhängiger Tonspurkopie. [Anleitung](NUTZERANLEITUNG.md).

Gebaut ausschließlich aus dem manifestierten Quellsnapshot: Basiscommit **d200ebf7567bceeab012ee9a2bb6b564635382a4** plus dokumentierte Arbeitsänderungen. **685 manifestierte Quelldateien**, **686 Dateien im Quellen-ZIP**, **250 Portable-Dateien** einschließlich Lizenz-/Herkunftshinweisen, Anleitungen und unverändertem Motor. Keine echten Filmdateien oder Buildausgaben im Quellen-ZIP. Vier vorbestehende Produktions-Lockänderungen unverändert erhalten und aus dem Snapshot ausgeschlossen; dieser verwendet committed Locks. Keine neue Produktionsabhängigkeit. Lokaler Zähler bleibt 43; bisherige Build-40-/41-Pakete erhalten.

## Technische Prüfung

- Snapshot-Restore im Locked Mode, Release-Build und Publish ohne Warnungen/Fehler. App-Dateiversion 1.0.2.43; laufzeitabhängig, .NET 10 Desktop Runtime x64 separat. Inno Setup 7 kompiliert erfolgreich aus dem nachgewiesenen Publish-Verzeichnis.
- **1552 Pakettests:** 200 Core, 72 Cutlists, 351 Media, 929 App; keine Fehler oder übersprungenen Tests.
- Zusätzlich **1280 Tests gegen finale ausgelieferte Produkt-DLLs:** 929 App + 351 Media. Deren vier Produktbibliotheken bytegleich zur Portable-Ausgabe.
- **28 QUICK- und 21 AUDIO-ENTHUSIAST-Nativfälle** gegen die finalen Core-/Media-DLLs bestanden. Mehrspur AAC+AC-3 ausdrücklich automatisch vorgeschlagen, bestätigt, erhalten und in beiden Reihenfolgen geprüft; Einzelspuren, Decoder-/Paketfehler, falsche Zuordnung, Abbruch, Ausgabeschutz und gezielte Bereinigung abgedeckt. Negative Fälle nur bei korrekter Ablehnung positiv.
- Paketoberfläche nativ geprüft: zwei Aktionen ohne Standardverfahren, Mehrspurzuordnung, Bestätigungspflicht, erneute Bestätigung nach Änderung/Tausch und sichtbare entfallende John-Wick-Spur. Echte libmpv-Paketprüfung erkennt AAC-ID 1 und AC-3-ID 2, schaltet beide und bestätigt die Auswahl durch Rücklesen; Testfenster unsichtbar und stummgeschaltet.
- Alle Quellmanifest-/Quellen-ZIP-Dateien und Portable-ZIP-Nutzdaten vollständig gegen ihre Manifeste geprüft. Paket- und Motorherkunft, vier native DLL-Hashes, Hinweise/Versionen sowie bekannte verbotene Runtime-/Werkzeugbeigaben kontrolliert. Motor direkt **otr_can 0.1.6**, Fähigkeiten **mp3-aac-v1** und **aac-overlap-v1** bestätigt; EXE-SHA **73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E**.
- 26 geschützte Dateien (bestehende Locks, Version.props, Zähler, frühere Pakete) vollständig unverändert. Produkt-/Testquellen gegenüber dem Auftragseingang nicht geändert. Kein Commit, Push, Tag, Upload oder Release.

Die Schutzprüfung stoppte den ersten Start vor jeder Paketaktion, weil die lokale Prüfhilfe noch den alten Ausgabeordner referenzierte. Zielpfad auf den neuen Build-43-Ordner berichtigt; alte Pakete wurden nicht überschrieben. Ein alter Datumsverweis in der schreibgeschützten Prüfhilfe wurde ebenfalls korrigiert. Danach sämtliche Paketprüfungen erfolgreich; keine fehlgeschlagenen Produkt-/Pakettests verschwiegen oder entfernt.

## Persönliche Abnahme und Grenzen

Eigener Furiosa-QUICK-Lauf des Nutzers vor der Paketierung auf **Bild, Ton und Klebeübergang im Drei-Sekunden-Fenster positiv** bestätigt. Beide Spuren technisch erhalten, je +1280 Samples / 26,667 ms vollständig erklärt. Die persönliche Bestätigung wird nicht als gesonderte Einzelprüfung jedes Codecs oder Anfang/Ende ausgeweitet. Reale technische John-Wick-/Furiosa-Referenzen und der Vergleich beider Verfahren stehen im Funktionsbericht.

**Neue Setup-/Portable-/VM-Abnahme bleibt offen.** Keine automatische Installation auf dem Arbeitsrechner. Im Setup und ZIP den mitgelieferten Motor **tools/otr-can/otr_can.exe** unter Einstellungen auswählen; alte gespeicherte Pfade können auf eine frühere Kopie zeigen. Erlaubte QUICK-Tonabweichungen sind keine PCM-Identitätszusage; unbekannte Warnungen/Decoderfehler bleiben Abbruchgründe. Kein allgemeiner Nachweis für beliebige Video-/Audiokombinationen.

Die Archive behalten den Dokumentationsstand bei Erstellung vor Abschluss der Paketprüfung. Dieser Arbeitsbericht ergänzt die späteren vollständigen Prüfergebnisse und Prüfsummen; die Archive werden dafür nicht nachträglich verändert.

## Paketdateien

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build43-source.zip | 3590661 | 4707FB040A338BB6535839BA316F25DEABB6B92BD3867D94B215C17DA6DAF7E1 |
| CutAssistantNext-1.0.2-Build43-win-x64.zip | 25227918 | 08E315B0AF69455BD84E7BD1A6F9E01124FEA82F75DC06C6E8D796700FFB7B5B |
| CutAssistantNext-1.0.2-Build43-win-x64-Setup.exe | 20498582 | DFE7379CC869675AAD80264C30357ED8D29C48455FB2C7758AE4671262F0A8AE |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

Alle sechs Auslieferungsdateien stehen zusätzlich in SHA256SUMS.txt; PUBLISH-FILES.sha256 beschreibt die 250 Portable-Nutzdateien. Die acht obersten Auslieferungsdateien werden nach erfolgreicher Endprüfung schreibgeschützt.

Nachweise: **.build/release-1.0.2-build43-20261028/** – PACKAGE-INPUTS.json, PACKAGE-PROOF.json, endgültige Tests und Nativ-/Dialog-/Playernachweise, Sicherung geschützter Dateien. Keine neue Veröffentlichungsfreigabe.
