# CAN 1.0.2 · Build 38 – lokaler Paketbericht

**Stand 26.10.2026: erstellt und technisch geprüft, nicht veröffentlicht.** Setup nicht automatisch installiert. Die manuelle Tonspurauswahl wurde nach Bereitstellung der Pakete persönlich erfolgreich durchlaufen; separate neue Portable-/VM-/Deinstallationsfälle und persönliche Abbruch-/Fehlerfallprüfungen bleiben offen.

## Inhalt und Herkunft

Neues Menü **Tonspuren → Tonspur für Filmteile auswählen …**. Ein bereits geschnittener MP4-Filmteil kann als zusätzliche Datei mit einer bewusst gewählten AAC- oder AC-3-Spur gespeichert werden. Anzeige aller entfallenden Tonspuren, keine Vorauswahl, keine Anwendung der Schnittliste, keine Neukodierung und kein Überschreiben vorhandener Dateien.

Die gemeinsame Kopiervorbereitung prüft die gewählte Spur streng und vergleicht sämtliche Bild-/Tonpakete sowie ursprüngliche Wiedergabezeiträume. Veränderte Wiedergabebeschneidung wird abgelehnt. Der automatische AAC-Notfallweg aus Build 37 bleibt enthalten und wurde mitgeprüft.

Unverändert: **otr-can 0.1.6**, **libmpv 0.41.0** sowie ihre passenden Quellenarchive. Keine neue Produktionsabhängigkeit und keine Änderung an der Installation oder persönlichen Werkzeugpfaden.

Exakter gebauter Quellsnapshot aus Basiscommit **1631aa7367a328bf86fd291a54a6c62f8731626f** plus den in SOURCE-SNAPSHOT.json genannten Arbeitsänderungen. **IncludesWorkingTreeChanges=true**: Die neue Produktweiterentwicklung ist noch nicht committed oder gepusht. Vier vorbestehende lokale Produktions-Lockänderungen wurden erhalten und ausgeschlossen; verwendet wurden die gesperrten Git-Lockdateien.

## Technische Prüfung

- Gesperrter Restore, Release-Build und Publish aus dem festen Quellsnapshot erfolgreich; **0 Warnungen, 0 Fehler**.
- **1446/1446 Solution-Tests**, keine übersprungenen Tests.
- Zusätzlich **925/925 App-Tests und 249/249 Medientests** gegen bytegleich verifizierte endgültige Produkt-DLLs des Portable-Pakets.
- **Zehn native Fälle** durch endgültige Paketbibliotheken und beigefügten Motor bestanden: manuelle AAC-/AC-3-Kopie, Ablehnung geänderter Wiedergabedauer, Ablehnung beschädigter gewählter Spuren, AAC-Kopie bei entfallendem defektem AC-3 sowie alle bisherigen vier automatischen Normal-/Notfallfälle.
- Beide Dialoge aus den finalen Paketbibliotheken gerendert; neue manuelle Auswahl visuell geprüft. Speichern startet deaktiviert und wird erst durch explizite Auswahl möglich.
- Entwicklungsprüfung zusätzlich: manuelle vollständige John-Wick-Teil-1-AAC-Kopie durch denselben Dienst erfolgreich. Vollständig dekodierter Ton zweier künstlicher Kopierfälle bytegleich zur Quelle. [Details und Grenzen](TONSPURAUSWAHL-FILMTEILE-20261026.md).
- **645 Quelldateien** im Manifest, **646 Quellen-ZIP-Dateien** bytegleich zum Snapshot.
- **250 Portable-Dateien** vollständig gegen Publish-Manifest und ZIP geprüft.
- Installer mit Inno Setup 7 erfolgreich kompiliert. Apphost-Dateiversion **1.0.2.38**.
- **29 vorbestehende Dateien** unverändert erhalten, darunter Build-35-/36-/37-Pakete, vier lokale Produktions-Lockdateien und der bisherige Motor. Der Buildzähler wurde erst nach erfolgreicher Paketprüfung von 37 auf 38 erhöht.
- Sechs Nutzdateien in SHA256SUMS.txt; acht oberste Paketdateien schreibgeschützt.

## Dateien

Ordner **artifacts/release-1.0.2-build38**.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build38-source.zip | 3433899 | C17CB15172F1DB1143100E94DF4FF9DB14214E3B631BA0FC9E96B8D9AA1C5A00 |
| CutAssistantNext-1.0.2-Build38-win-x64.zip | 25122979 | 295658D53B5D0FBE57EA01950647A1BBE7153F1ADFAC5A270F16E271F2E2E443 |
| CutAssistantNext-1.0.2-Build38-win-x64-Setup.exe | 20410629 | 304ACD2D5170E1A2B9E5E8912A2986AE0720F7C7CFAF0CC61B40FAF7E1791AEC |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

## Persönliche Abnahme und Grenzen

Der Nutzer hat den bereits geschnittenen Teil 1 über die neue manuelle Auswahl als **John Wick Kapitel 4 Teil 1 otr-can - AAC.mp4** gespeichert. Vollständiges Erfolgsprotokoll und Screenshot der Abschlussmeldung belegen Auswahl, strenge Tonprüfung, Kopie ohne Neukodierung, exakten Bild-/Tonpaketvergleich und korrekt genannten Spurverlust. Separate neue Portable-/VM-/Deinstallationsfälle sowie persönliche Prüfungen von Abbruch und vorhandenen Zieldateien bleiben offen. Die technischen Fehlerfallprüfungen sind davon unabhängig bestanden.

Eine neue Kopie wird bei Decoderfehlern, nicht bestätigbaren Daten oder veränderten Wiedergabezeiten verworfen. Keine automatische Neukodierung, kein automatisches Zusammenfügen und keine Reparatur defekter Spuren. Die bekannte Ursache früherer vollständiger Rechnerstillstände bleibt ungeklärt.

Automatischer Notfallablauf und manuelle Tonspurkopie sind jeweils persönlich durchlaufen; daraus wird keine vollständige Abnahme aller Paket-/Installations- und Fehlerfallvarianten abgeleitet. Keine Veröffentlichung, kein Tag oder Upload ohne ausdrückliche Freigabe.

Lokale Nachweise: **.build/release-1.0.2-build38-20261026/** mit PACKAGE-INPUTS.json, PACKAGE-PROOF.json, FINAL.json, FINAL-NATIVE.json, final-package-results/, native-cases/ sowie beiden Dialogdarstellungen. Paketlogs und Solution-TRX liegen beim neuen Paketsatz. Reale Medien bleiben außerhalb von Git.

[Release Notes](RELEASE-NOTES-1.0.2.md), [Umsetzung und Filmprüfung](TONSPURAUSWAHL-FILMTEILE-20261026.md), [Paketumfang](FREEZE-1.0.2-BUILD38-20261026.md).

## Nachtrag: bestätigter Referenzfilm

Die vom Nutzer erzeugte Teil-1-AAC-Kopie und Teil 2 mit Audio Korrektur wurden anschließend separat mit FFmpeg zusammengefügt. Der Nutzer bestätigt den sauberen Übergang. Dieser zusätzliche Medienauftrag änderte weder CAN-Code noch Build-38-Pakete und ist keine bereits integrierte Klebecenter-Funktion. [Technischer Referenzfall](JOHN-WICK-4-KLEBECENTER-REFERENZ-20261026.md).

Der heutige Dokumentationsabgleich ergänzt die Arbeitsdokumentation; die Dokumente in den eingefrorenen Paketen/Quellarchiven bleiben bei ihrem damaligen Erstellungsstand.
