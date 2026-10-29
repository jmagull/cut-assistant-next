# Aktueller Projektstand

**29.10.2026 – Entwicklungsbuild 46:** Nutzerkorrektur umgesetzt: QUICK ist pragmatischer FFmpeg-Concat mit Streamcopy, bestätigter AAC-/AC-3-Mehrspurzuordnung und **Meckerecke – technische Hinweise**. Warnungen, H.264-Hashunterschiede und Sampleabweichungen allein verhindern den Erfolg nicht. Keine zusätzlichen Vollprüfungen. Mindestprüfung, Originalschutz, Protokoll, Phasenzeiten und Drei-Sekunden-Kontrolle bleiben; QUICK-Übergang ausdrücklich angenähert. AUDIO-ENTHUSIAST unverändert. **1576 Tests**, **27 native QUICK-Fälle**, **21 bisherige native Fälle** positiv, neun Neukodierungsausgaben bytegleich. Reale MP4Box-/otr-can-Furiosa-Mehrspurläufe und John-Wick-AAC technisch erfolgreich. Zwei eigene MP4Box-QUICK-Läufe mit AAC+AC-3 persönlich auf Bild/Ton/Klebestelle/Drei-Sekunden-Vorschau bestätigt: Furiosa 23,74 s und anschließend John Wick 16,44 s. Das doppelte Furiosa-Protokoll zählt weiterhin als ein Lauf. Stand jetzt auf Nutzerauftrag eingefroren: Setup/ZIP/Quellen vollständig erstellt, technisch geprüft und schreibgeschützt; Commit/Push nach Paketprüfung beauftragt. Veröffentlichung erst nach neuer Ansage. [Paketbericht](RELEASE-1.0.2-BUILD46-RESULT.md). [Aktueller Nachweis](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md).

**Historische Diagnose vor der Nutzerkorrektur:** Die [MP4Box-Furiosa-Diagnose](KLEBECENTER-MP4BOX-FURIOSA-DIAGNOSE-20261029.md) und [manuelle Referenzprüfung](KLEBECENTER-MP4BOX-REFERENZPRUEFUNG-20261029.md) erklären die Ablehnung in Build 45. Die nachfolgende verbindliche Korrektur hebt deren strenge QUICK-Abbruchregeln und Rücksprache-Sperre ausdrücklich auf. Eingefrorene Build-45-Pakete bleiben unverändert.

**29.10.2026 – Entwicklungsbuild 45:** Checkbox um „nur bei übereinstimmendem Tonformat“ ergänzt, Hinweis auf einmalige Zuordnung und Abwahl nicht benötigter Paare. Nur Beschriftung/Textumbruch; Logik und Motor unverändert. 1552 Tests und vier native Anzeigen-/Auswahlfälle positiv. Hinweis persönlich bestätigt; Stand und Pakete als Build 45 erstellt, technisch geprüft und schreibgeschützt eingefroren; neue persönliche Paketabnahme offen. [Paketbericht](RELEASE-1.0.2-BUILD45-RESULT.md). [Nachweis und laufende Setup-Prüfrunde](KLEBECENTER-TONSPURHINWEIS-20261029.md).

**Nachtrag 29.10.2026 – Entwicklungsbuild 44:** separaten Tonspurkopie-Menüeintrag aus dem Klebecenter entfernt; Auswahl weiterhin direkt beim Zusammenfügen. Nur Menüänderung, Kopierdienst und beide Klebeverfahren unverändert. Menü vom Nutzer persönlich bestätigt; Setup/Portable/Quellen für Build 44 jetzt erstellt und technisch geprüft; persönliche Paketabnahme offen. Build-43-Pakete bleiben erhalten. [Paketbericht](RELEASE-1.0.2-BUILD44-RESULT.md). Spätere Werkzeugrubrik mit Tonspurkopie, Martins Werbebildervorschau und Untertitelanzeige auf der [Merkliste](SETUP-MERKLISTE.md).

**Abgleich 28.10.2026: CAN 1.0.2 Build 41 · otr-can 0.1.6 · libmpv 0.41.0 · .NET 10/WPF · Windows 11 x64.** [Versionen](VERSIONEN.md), [Nutzeranleitung](NUTZERANLEITUNG.md).

## Versionen und Auslieferung

| Bereich | Aktueller Stand |
|---|---|
| Öffentlich dokumentierter stabiler Stand | CAN 1.0.2 Build 41, veröffentlicht als v1.0.2 / Latest; Rücknahme von 1.0.1 bleibt dokumentiert |
| Veröffentlichter Paketsatz | CAN 1.0.2 Build 41; App-Dateiversion 1.0.2.41 |
| Lokaler Freeze/Paketauftrag | Build 46: bestätigtes pragmatisches QUICK und Meckerecke eingefroren; Setup/ZIP/Quellen technisch geprüft und schreibgeschützt, danach Commit/Push. Veröffentlichung gesperrt bis erneuter Ansage; Build 45 erhalten |
| Schnittmotor / Wiedergabe | otr-can 0.1.6 / libmpv 0.41.0 |
| Build-41-Paketprüfung | 1489/1489 Tests, zusätzlich 925 App-/292 Medientests gegen finale DLLs; 0 Warnungen/Fehler |
| Native Laufzeitnachweise | Build 41: 21 Klebecenter-Fälle einschließlich Bereinigung; frühere Notfall-/Kopiefälle aus Build 39 bleiben separat dokumentiert |
| Änderung Build 41 | nur Klebecenter-Bereinigung und vier Regressionstests; Medienverarbeitung und Motor unverändert |
| Persönliche Klebecenter-Erfolge | John Wick AC-3/AAC und AAC/AAC; zusätzlich Furiosa nach zwei eigenen otr-can-Schnitten, alle Prüfstellen positiv |
| Build-40-Setup auf Arbeits-PC | Hinweis 0.1.6 sichtbar bestätigt; nach angekündigtem Umstieg beide Furiosa-otr-can-Schnitte und Klebecenter persönlich positiv; keine pauschale VM-/Portable-Abnahme |
| VM-Setup Build 40 | acht Schnittfälle plus Furiosa-Klebecenter: neun Funktionstests positiv; anschließend Deinstallation bestätigt und Wechsel zur ZIP-Prüfung |
| VM-Deinstallation | persönlich „deinstallation check“; Klebecenter-Diagnoseordner mit protocol.txt verbleibt gemäß bisheriger Aufräumregel im Filmordner |
| VM-Portable Build 40 | drei Fälle: Grey’s Anatomy/MP4Box, Teufelsbrigade AVI/otr-can und Furiosa-Klebecenter AAC/AAC positiv; persönliche ZIP-Prüfung mit „damit zip check“ abgeschlossen |
| Build-41-Setup auf Arbeits-PC | John Wick: temporäre Datei weg, Bild/Ton/Schnitt/Drei-Sekunden-Vorschau und Klebecenter persönlich positiv |
| Weitere Aufgaben | neue Build-41-ZIP-/VM-Prüfung getrennt offen; weitere Varianten nach Bedarf |
| Veröffentlichung | [v1.0.2 / Latest](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2), 27.10.2026; acht Assets vollständig gegen SHA-256 geprüft |

Pakete unter **artifacts/release-1.0.2-build41**: Setup, Portable, exakte CAN-Quellen, passende Motor-/libmpv-Quellen, Relink und Prüfsummen. Frühere Pakete erhalten. [Paketbericht](RELEASE-1.0.2-BUILD41-RESULT.md), [Release Notes](RELEASE-NOTES-1.0.2.md).

## Umgesetzte Funktionen

- **Klebecenter → Filmteile zusammenfügen …**: Dateien und Reihenfolge direkt wählen. Lokal ab Build 43 ausdrücklich QUICK AND COPY oder AUDIO-ENTHUSIAST wählen; mehrere bestätigte AAC-/AC-3-Paare nur im Kopierweg. Keine Vorbereitungskopie erforderlich. Ab Build 44 entfällt der zusätzliche Tonspurkopie-Menüeintrag; der Dienst bleibt für später erhalten.
- Passendes H.264-Video bleibt in beiden Verfahren unverändert. AUDIO-ENTHUSIAST verwendet unverändert eine Spur je Teil und durchgehende AAC-LC-Neukodierung; hier dürfen die Quellcodecs verschieden sein. QUICK AND COPY benötigt kompatible Codecpaare und übernimmt bestätigte AAC-/AC-3-Spuren ohne Neukodierung; technische Auffälligkeiten stehen ab Build 46 als Hinweise in der Meckerecke. [Umsetzung und Grenzen](KLEBECENTER-QUICK-COPY-20261028.md).
- Verfahrensabhängige Prüfung, Abbruch und Schutz vorhandener Dateien. Abschlussdialog mit Anfang, bei QUICK angenäherter Klebestelle und Ende; Kontrollplayer mit drei Sekunden Vorlauf.
- Ausdrücklicher AAC-Notfallweg beim otr-can-Schnitt bleibt enthalten. Gesunde Aufträge behalten alle Spuren; Spurverlust nur nach bestätigter Auswahl.
- Cutlist-Metadaten weisen den vorgesehenen Schnittmotor samt ermittelter Version aus.

[Klebecenter](KLEBECENTER-20261027.md), [AAC-Notfallweg](OTR-CAN-AAC-NOTFALLWEG-20261026.md), [historische Einzelkopie Build 38](TONSPURAUSWAHL-FILMTEILE-20261026.md).

## Bestätigte John-Wick-Läufe

Teil 1 mit zwei Tonspuren, Teil 2 mit Audio-Korrektur: Teil 1 wahlweise AC-3-Spur 2 oder AAC-Spur 1; Teil 2 AAC-Spur 1. Jeweils Deutsch/Stereo/48000 Hz. Beide eigenen Läufe technisch und persönlich positiv: **457399680 Tonsamples**, Dauer **02:38:49.160**, Klebestelle **01:57:37.120**, Vorlauf **01:57:34.120**. Die technische Referenz belegt zusätzlich **238229 unveränderte Videopakete**.

Die zweite AAC/AAC-Rückmeldung erfolgte in der angekündigten Arbeits-PC-Setuprunde vor dem Wechsel zu Build 40. Ihr Protokoll enthält keine CAN-Buildanzeige; daraus wird keine pauschale Installationsabnahme abgeleitet. Für Build 40 ist der korrigierte Hinweis sichtbar bestätigt.


## Furiosa: großer Praxislauf in der Build-40-Setuprunde

Nach dem angekündigten Wechsel zu Setup Build 40 bestätigt der Nutzer beide otr-can-Schnitte von Furiosa Teil 1/2 sowie das Klebecenter und alle Prüfstellen. Beide gewählten Spuren AAC/Deutsch/Stereo/48000 Hz. Vollständiges Klebeprotokoll erfolgreich: **387448320 Tonsamples**, Dauer **02:14:31.840**, Klebestelle **01:54:18.320**, Vorlauf **01:54:15.320**. Damit ist der Arbeits-PC-Funktionslauf der Setup-Prüfrunde positiv gemeldet; zusätzliche VM-/Portable-/Deinstallationsvarianten werden daraus nicht abgeleitet. [Praxisbericht](FURIOSA-KLEBECENTER-PRAXIS-20261027.md).

## Versionshinweis, Quellen und Dokumentation

Build 39 enthielt bereits Motor 0.1.6, nannte im Voraussetzungstext aber 0.1.5. Build 40 berichtigt die Zeile und prüft sie gegen den tatsächlichen Motor. Die Mindestangabe „MP3-Unterstützung ab 0.1.5“ bleibt richtig.

Quellsnapshots: Basiscommit **1631aa7367a328bf86fd291a54a6c62f8731626f** plus dokumentierte uncommittete Arbeitsänderungen (**IncludesWorkingTreeChanges=true**). Vier lokale Produktions-Lockänderungen sind erhalten und aus den Snapshots ausgeschlossen. Der lokale Buildzähler steht auf **46**. Die aktuelle QUICK-Weiterentwicklung ist separat von den eingefrorenen Build-45-Paketen.

Die gezielte Bereinigung wird als eigener Paketsatz Build 41 ausgeliefert. Frühere Pakete, Quellenarchive und Prüfsummen bleiben erhalten. Paketdokumente zeigen ihren Erstellungsstand; datierte Berichte und frühere Motorversionen sind historische Nachweise.

## Nächster Schritt und Grenzen

Arbeits-PC- und neun VM-Funktionsfälle sowie VM-Deinstallation persönlich positiv gemeldet. Build-40-Portable-Prüfung persönlich abgeschlossen. Die daraufhin beauftragte Bereinigung ist in Build 41 technisch geprüft; persönliche Setup-Prüfung der Korrektur auf dem Arbeitsrechner mit John Wick positiv; neue ZIP-/VM-Prüfung getrennt offen. Ursache früherer Furiosa-Rechnerstillstände ungeklärt; spätere Erfolge belegen keine gezielte Behebung. AVI experimentell; MP4Box-/Vera-/Werbeblitzer-Grenzen bestehen fort. Veröffentlichung, Commit und Push sind am 27.10.2026 nach ausdrücklicher Freigabe erfolgt.

## VM-Setuptest begonnen

Erster Fall **Die Teufelsbrigade AVI → MP4-Arbeitsdatei → MP4Box, sechs Abschnitte** technisch abgeschlossen und persönlich für Ton/Schnitt/Bild/Drei-Sekunden-Vorschau bestätigt. Nutzer testet weiter; keine vollständige VM-Endabnahme behauptet. [Laufender VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## VM: zweiter Motorweg bestätigt

Die Teufelsbrigade AVI auch mit otr-can 0.1.6 persönlich positiv: sechs Abschnitte, 182574 Videopakete und 350542080 Tonsamples technisch verifiziert; MP3 einmal nach AAC-LC kodiert. Nutzer bestätigt Ton/Bild/Schnitt/Drei-Sekunden-Vorschau und testet weiter. [VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## Portable-ZIP in der VM begonnen

Nach bestätigter Setup-Deinstallation zur ZIP-Prüfung gewechselt. Grey’s Anatomy MP4 mit MP4Box, drei Abschnitte und alle Schnittstellenprüfungen erfolgreich; Bild/Ton/Schnitt/Drei-Sekunden-Vorschau persönlich positiv. Getrennter Portable-Nachweis, kein daraus abgeleiteter Gesamtabschluss. [Portable-Bericht](VM-PORTABLE-BUILD40-20261027.md).

## Bereinigung ab Build 41

Nach Erfolg Arbeitsordner entfernen; bei Fehler/Abbruch Diagnose erhalten. Gesperrte oder fremde Inhalte bleiben mit Meldung erhalten. Keine automatische Löschung alter Arbeitsordner. [Umsetzung und Nachweise](KLEBECENTER-BEREINIGUNG-20261027.md).

## Veröffentlichungsabschluss

Release-Tag v1.0.2 zeigt auf den gepushten Programmstand; acht Assets öffentlich mit identischen Prüfsummen. [Nachweis](RELEASE-1.0.2-PUBLISHED-20261027.md).

## Lokale Weiterentwicklung am 28.10.2026

Phasenzeiten und direkte RF64-Zusammensetzung nach Freigabe umgesetzt und geprüft; Veröffentlichung bleibt Build 41. [Messung, Tests und Grenzen](KLEBECENTER-PHASENZEITEN-RF64-20261028.md).
