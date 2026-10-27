# Versionen und Nachweise

**Stand 27.10.2026.** CAN-Anwendung, Buildnummer, Schnittmotor und Wiedergabebibliothek haben getrennte Versionen. [Aktueller Stand](AKTUELLER-STAND.md).

| CAN | Build | otr-can | Einordnung |
|---|---:|---|---|
| 1.0.0 | 32 | 0.1.5 | Früherer stabiler Stand |
| 1.0.1 | 34 | 0.1.6 | Veröffentlichung zurückgenommen; datierter Nachweis |
| 1.0.2 | 35 | 0.1.6 | MP4Box-Übergangszuordnung |
| 1.0.2 | 36 | 0.1.6 | Schnittmotor-Cutlist-Metadaten |
| 1.0.2 | 37 | 0.1.6 | Bestätigter AAC-Notfallweg |
| 1.0.2 | 38 | 0.1.6 | Damalige Einzelkopie für externes Kleben |
| 1.0.2 | 39 | 0.1.6 | Integriertes Klebecenter; Voraussetzungstext noch fälschlich 0.1.5 |
| 1.0.2 | 40 | 0.1.6 | Hinweis berichtigt; Setup-/ZIP-Prüfung persönlich abgeschlossen |
| 1.0.2 | 41 | 0.1.6 | Veröffentlicht als v1.0.2 / Latest; Klebecenter-Arbeitsordner nach Erfolg entfernen |

Aktuelle App-Dateiversion **1.0.2.41**, libmpv **0.41.0**, Framework **.NET 10**, Ziel **Windows 11 x64/WPF**. CAN 1.0.2 Build 41 wurde am 27.10.2026 nach ausdrücklicher Freigabe veröffentlicht. [Veröffentlichungsnachweis](RELEASE-1.0.2-PUBLISHED-20261027.md).

## Technische Nachweise

- Build 41: 1489 Pakettests, zusätzlich 1217 Tests gegen finale DLLs und 21 native Klebecenter-Fälle einschließlich Erfolgs-/Fehler-/Abbruchbereinigung. Persönliche Setup-Prüfung auf dem Arbeitsrechner mit John Wick positiv, einschließlich verschwundener temporärer Datei; neue ZIP-/VM-Prüfung getrennt offen. [Paketbericht](RELEASE-1.0.2-BUILD41-RESULT.md).

- Build 38: 1446 Pakettests, zusätzlich 1174 Tests gegen finale DLLs, zehn native Fälle.
- Build 39: 1485 Pakettests, zusätzlich 1213 Tests gegen finale DLLs, 21 Klebecenter- und zehn bisherige native Fälle.
- Build 40: 1485 Pakettests und zusätzlich 1213 Tests gegen finale DLLs erneut bestanden. 361 Anwendungs-/Testquellen bytegleich zu Build 39; dessen 31 native Fälle wurden für die Textkorrektur nicht wiederholt.
- Persönlich: John Wick AC-3/AAC und AAC/AAC mit Bild/Ton/Übergang bestätigt. Nach Umstieg auf Setup Build 40 auch beide Furiosa-otr-can-Schnitte und Klebecenter samt allen Prüfstellen positiv. Screenshot bestätigt Hinweis 0.1.6. Arbeits-PC-Funktionslauf der Setup-Prüfrunde positiv; zusätzliche Portable-/VM-/Deinstallationsvarianten bleiben offen. [Furiosa-Praxisbericht](FURIOSA-KLEBECENTER-PRAXIS-20261027.md).

Ein neuer CAN-Build bedeutet keinen neuen Motor. Historische 0.1.3-/0.1.4-/0.1.5-Angaben werden nicht auf 0.1.6 umgeschrieben. „MP3-Unterstützung ab 0.1.5“ bleibt richtig.

## Quellen und Pakete

**artifacts/release-1.0.2-build40** mit **CutAssistantNext-1.0.2-Build40-source.zip**, SOURCE-SNAPSHOT.json/SOURCE-FILES.sha256. Passend dazu unverändert **otr-can-0.1.6-source.zip**, **CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip**, Relink-Paket und SHA256SUMS.txt. Keine echten Filme im Repository.

[Paketbericht 40](RELEASE-1.0.2-BUILD40-RESULT.md), [Paketbericht 39](RELEASE-1.0.2-BUILD39-RESULT.md), [Rücknahme 1.0.1](RELEASE-WITHDRAWAL-1.0.1-20261020.md), [stabiler Build 32](RELEASE-1.0.0-BUILD32-RESULT.md).
