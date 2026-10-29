# Versionen und Nachweise

**Stand 29.10.2026.** CAN-Anwendung, Buildnummer, Schnittmotor und Wiedergabebibliothek haben getrennte Versionen. [Aktueller Stand](AKTUELLER-STAND.md).

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
| 1.0.2 | 42 | 0.1.6 | Lokaler Entwicklungsbuild: Phasenzeiten und direkte RF64-Zusammensetzung; nicht paketiert/veröffentlicht |
| 1.0.2 | 43 | 0.1.6 | Lokale Prüffassung: QUICK AND COPY mit AAC-/AC-3-Mehrspurzuordnung, unveränderter AUDIO-ENTHUSIAST, Tonspurwahl im Kontrollplayer; Setup/Portable/Quellen lokal geprüft, nicht veröffentlicht |
| 1.0.2 | 44 | 0.1.6 | Lokale Menüvereinfachung: separate Tonspurkopie im Klebecenter ausgeblendet; Auswahl beim Zusammenfügen bleibt. Menü persönlich bestätigt; Setup/Portable/Quellen lokal technisch geprüft; persönliche neue Paketabnahme offen, nicht veröffentlicht |
| 1.0.2 | 45 | 0.1.6 | Lokaler Hinweis am Tonspurpaar-Haken und Erklärung doppelter Zuordnung; nur XAML-Text/Textumbruch, persönlich bestätigt und eingefroren; Setup/ZIP/Quellen technisch geprüft und schreibgeschützt eingefroren; neue persönliche Paketabnahme offen, nicht veröffentlicht |
| 1.0.2 | 46 | 0.1.6 | Bestätigtes pragmatisches QUICK mit Meckerecke eingefroren; Setup/ZIP/Quellen technisch geprüft und schreibgeschützt; Quellstand committet/gepusht, Veröffentlichung erst nach Ansage |

Veröffentlichte App-Dateiversion **1.0.2.41**, lokale Weiterentwicklung **1.0.2.46** (geprüfter Paketsatz 1.0.2.46 schreibgeschützt eingefroren; frühere Pakete erhalten), libmpv **0.41.0**, Framework **.NET 10**, Ziel **Windows 11 x64/WPF**. CAN 1.0.2 Build 41 wurde am 27.10.2026 nach ausdrücklicher Freigabe veröffentlicht. [Veröffentlichungsnachweis](RELEASE-1.0.2-PUBLISHED-20261027.md).

## Technische Nachweise

- Build 46: 1576 Tests, 27 native QUICK- und 21 bisherige Fälle; neun bytegleiche Enthusiast-Ausgaben; drei reale Referenzen erfolgreich. [Ablauf und Grenzen](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md).

- Build 43: 28 native QUICK-Fälle, 21 bisherige Klebecenter-Fälle und neun bytegleiche Neukodierungsausgaben gegen Build 42; echte John-Wick-/Furiosa-Mehrspurprüfung und native Player-Umschaltung. [Nachweis und Grenzen](KLEBECENTER-QUICK-COPY-20261028.md).

- Build 41: 1489 Pakettests, zusätzlich 1217 Tests gegen finale DLLs und 21 native Klebecenter-Fälle einschließlich Erfolgs-/Fehler-/Abbruchbereinigung. Persönliche Setup-Prüfung auf dem Arbeitsrechner mit John Wick positiv, einschließlich verschwundener temporärer Datei; neue ZIP-/VM-Prüfung getrennt offen. [Paketbericht](RELEASE-1.0.2-BUILD41-RESULT.md).

- Build 38: 1446 Pakettests, zusätzlich 1174 Tests gegen finale DLLs, zehn native Fälle.
- Build 39: 1485 Pakettests, zusätzlich 1213 Tests gegen finale DLLs, 21 Klebecenter- und zehn bisherige native Fälle.
- Build 40: 1485 Pakettests und zusätzlich 1213 Tests gegen finale DLLs erneut bestanden. 361 Anwendungs-/Testquellen bytegleich zu Build 39; dessen 31 native Fälle wurden für die Textkorrektur nicht wiederholt.
- Persönlich: John Wick AC-3/AAC und AAC/AAC mit Bild/Ton/Übergang bestätigt. Nach Umstieg auf Setup Build 40 auch beide Furiosa-otr-can-Schnitte und Klebecenter samt allen Prüfstellen positiv. Screenshot bestätigt Hinweis 0.1.6. Arbeits-PC-Funktionslauf der Setup-Prüfrunde positiv; zusätzliche Portable-/VM-/Deinstallationsvarianten bleiben offen. [Furiosa-Praxisbericht](FURIOSA-KLEBECENTER-PRAXIS-20261027.md).

Ein neuer CAN-Build bedeutet keinen neuen Motor. Historische 0.1.3-/0.1.4-/0.1.5-Angaben werden nicht auf 0.1.6 umgeschrieben. „MP3-Unterstützung ab 0.1.5“ bleibt richtig.

## Quellen und Pakete

**artifacts/release-1.0.2-build40** mit **CutAssistantNext-1.0.2-Build40-source.zip**, SOURCE-SNAPSHOT.json/SOURCE-FILES.sha256. Passend dazu unverändert **otr-can-0.1.6-source.zip**, **CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip**, Relink-Paket und SHA256SUMS.txt. Keine echten Filme im Repository.

[Paketbericht 40](RELEASE-1.0.2-BUILD40-RESULT.md), [Paketbericht 39](RELEASE-1.0.2-BUILD39-RESULT.md), [Rücknahme 1.0.1](RELEASE-WITHDRAWAL-1.0.1-20261020.md), [stabiler Build 32](RELEASE-1.0.0-BUILD32-RESULT.md).
