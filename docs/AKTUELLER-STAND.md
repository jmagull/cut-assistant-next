# Aktueller Projektstand

**Stand 31.10.2026: CAN 1.0.2 Build 47 ist eingefroren und zur Veröffentlichung freigegeben.** App-Dateiversion **1.0.2.47**, Motor **otr-can 0.1.6**, Wiedergabe **libmpv 0.41.0**, .NET 10/WPF für Windows 11 x64. Die Veröffentlichung läuft unter **v1.0.2-build47**; der frühere Release v1.0.2 mit Build 41 bleibt erhalten.

## Versionen und Auslieferung

| Bereich | Stand |
|---|---|
| Freigegebener Release | [CAN 1.0.2 Build 47 / v1.0.2-build47](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2-build47), Veröffentlichung in Vorbereitung |
| App-Dateiversion / Buildzähler | 1.0.2.47 / 47 |
| Schnittmotor / Wiedergabe | otr-can 0.1.6 / libmpv 0.41.0 |
| Oberfläche / Plattform | .NET 10, WPF, Windows 11 x64 |
| Paketsatz | artifacts/release-1.0.2-build47: Setup, Portable, CAN-/Motor-/libmpv-Quellen, Relink, zwei Prüfsummenmanifeste |
| Paketprüfung | 1593 Tests, zusätzlich 1321 Tests gegen finale DLLs; 0 Warnungen/Fehler |
| Dateien | 250 Portable-Nutzdateien, 705 manifestierte Quellen / 706 Quellen-ZIP-Dateien; acht Auslieferungsdateien geprüft und schreibgeschützt |
| Herkunft | Snapshot-Basis 3aa00a138565c043b34fcb959f2c28d937f83b0b plus 17 manifestierte Änderungen; alle 376 Produkt-/Testdateien seit Paketauftrag unverändert |
| Lokale Nutzeränderungen | Vier Produktions-Lockänderungen erhalten und aus Quellenarchiv/Commit ausgeschlossen |
| Vorheriger öffentlicher Release | v1.0.2 / Build 41 vom 27.10.2026 bleibt erhalten |

## Klebecenter und bestätigte Nutzung

Erkannte GPAC/MP4Box-Filmteile sperren AUDIO-ENTHUSIAST samt Tonspurauswahl; ein dauerhaft sichtbarer Hinweis verweist auf QUICK. Erkennung unabhängig vom Dateinamen, bei gemischten Paaren, Tauschen und Ersetzen berücksichtigt. Der Dienst prüft nochmals vor Medienverarbeitung. Fehlende GPAC-Kennung erlaubt keine sichere Herkunftsaussage; die strengen Medienprüfungen bleiben aktiv.

QUICK verwendet FFmpeg-Streamcopy für Video und bestätigte AAC-/AC-3-Spurpaare, mit technischen Hinweisen in der Meckerecke. AUDIO-ENTHUSIAST verbindet genau eine Spur je Teil per RF64 und einmaliger AAC-LC-Neukodierung bei unverändertem Video. QUICK-Prüfzeiten angenähert, Kontrollplayer mit Tonspurwahl und drei Sekunden Vorlauf. Keine automatische Verfahrensumschaltung.

| Persönlicher Nachweis | Stand |
|---|---|
| Build-47-VM-Setup | MP4Box → QUICK; otr-can → QUICK und AUDIO-ENTHUSIAST positiv, Sperr-/Dateiwechselchecks bestätigt |
| Build-47-Portable in der VM | MP4Box-/otr-can-QUICK persönlich positiv, otr-can-AUDIO-ENTHUSIAST technisch erfolgreich; persönliche Kontrolle dieses letzten Laufs nicht separat zugeordnet |
| Deinstallation Build 47 | Nicht separat bestätigt |
| Aktuelle Arbeits-PC-Schnitte | Rocky Horror/50 fps/otr-can, Doc und Brilliant Minds/25 fps/MP4Box persönlich positiv – ausdrücklich Build-46-Setup |
| Nutzerentscheidung 31.10.2026 | Stand einfrieren, Commit, Push und Veröffentlichung ausdrücklich freigegeben |

[Paketbericht](RELEASE-1.0.2-BUILD47-RESULT.md), [Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md), [Setup](VM-SETUP-BUILD47-20261031.md), [Portable](VM-PORTABLE-BUILD47-20261031.md), [Praxistests Build 46](PRAXISTESTS-20261031.md), [Anleitung](NUTZERANLEITUNG.md).

## Grenzen und nächste Schritte

Jede Ausgabe auf Bild, Ton, Synchronität und Schnitt-/Klebestellen prüfen. QUICK garantiert keine sampleidentische Tonwellenform. Herkunft ohne GPAC-Kennung bleibt unbekannt. AVI experimentell, MP4Box-/Werbeblitzer-Grenzen bleiben bestehen. Ursache früherer Furiosa-Rechnerstillstände ungeklärt; spätere Erfolge belegen keine gezielte Behebung. Weitere persönliche Prüfvarianten bleiben nachvollziehbar dokumentiert; die Veröffentlichung ist vom Nutzer freigegeben.

## Datierte Entwicklungshistorie

Die folgenden Einträge bewahren ihren damaligen Stand. Ältere Angaben wie „öffentlich Build 41“, „Buildzähler 46“ oder noch fehlende Freigaben sind historische Aussagen und werden durch den aktuellen Überblick oben abgelöst.

<details>
<summary>Frühere Zwischenstände und datierte Nachträge</summary>

**31.10.2026 – Brilliant Minds mit Build 46 / Setup / Arbeitsrechner positiv:** S01E11 „Die andere Frau“, 5plus HQ vom 30.10.2026, 25 fps, laut Cutlist MP4Box 26.07, drei Behaltebereiche. Schnitt-/Zusammenfügungsprotokoll mit allen drei Abschnittsprüfungen bis „Fertig.“; an Übergang 1 dekodierter neuer Anfang wegen gleicher Bildzeit zusätzlich geprüft. Bild/Ton/Schnitt/Drei-Sekunden-Vorschau persönlich bestanden. Dritter aktueller Arbeits-PC-Praxisfall; Code und Pakete unverändert. [Nachweis](PRAXISTESTS-20261031.md#fall-3-brilliant-minds-s01e11--die-andere-frau-5plus-hq-25-fps).

**31.10.2026 – Doc S02E18 mit Build 46 / Setup / Arbeitsrechner positiv:** Laufende Testumgebung gemäß letzter Nutzerangabe fortgeführt. 5plus-HQ-Aufnahme vom 30.10.2026, Cutlist mit **25 fps**, **MP4Box 26.07** und zwei Behaltebereichen. Bild/Ton/Schnitt/Drei-Sekunden-Vorschau persönlich bestanden; laut Cutlist-Kommentar Werbung vollständig entfernt. Eigenständiger Praxisfall neben Rocky Horror, getrennt von Build-47-VM/Portable dokumentiert. [Nachweis](PRAXISTESTS-20261031.md#fall-2-doc-s02e18--orientierung-5plus-hq-25-fps).

**31.10.2026 – Rocky-Horror-Praxischeck mit Build 46 / Setup / Arbeitsrechner positiv:** Zuordnung vom Nutzer nachträglich präzisiert. arte-HD-Aufnahme vom 30.10.2026, Cutlist mit **50 fps**, einem Behaltebereich und vorgesehenem Motor **otr-can 0.1.6**. Ton/Bild/Schnitt/Drei-Sekunden-Vorschau persönlich bestanden. Cutlist-Kommentar nennt keine Werbung und keinen verbliebenen arte-Vorspann-Blitzer. Getrennt von Build-47-VM/Portable dokumentiert; keine vollständige Paket-Endabnahme. [Praxisnachweis](PRAXISTESTS-20261031.md#fall-1-the-rocky-horror-picture-show-arte-hd-50-fps).

**31.10.2026 – Build-47-Portable: Furiosa-Sperre und weitere Läufe positiv:** MP4Box-Sperre auch für Furiosa persönlich bestätigt. otr-can → QUICK AND COPY mit AAC und AC-3 in **41,93 s**, Ton/Bild/Schnitt/Drei-Sekunden-Vorschau persönlich bestanden; Herkunft vom Nutzer ausdrücklich geklärt. Eigener angehängter otr-can → AUDIO-ENTHUSIAST-Lauf technisch erfolgreich in **5:26,37 min**, **387448320 Samples**, Klebestelle **01:54:18.320**, Dauer **02:14:31.840**. Persönliche Kontrolle dieses letzten Laufs nicht separat zugeordnet. Drei Portable-Läufe dokumentiert, ZIP-Endabnahme offen. [Portable-Nachweise](VM-PORTABLE-BUILD47-20261031.md#abnahmestand).

**31.10.2026 – Portable-ZIP Build 47 in der VM begonnen:** Nach „jetzt zip“ eigener John-Wick-MP4Box-/QUICK-Lauf mit AAC und AC-3 erfolgreich in **28,61 s**. AUDIO-ENTHUSIAST-Sperrhinweis sichtbar; Ton, Bild und Drei-Sekunden-Vorschau persönlich bestanden. Ausgabeübernahme und Bereinigung laut Protokoll positiv, technische Hinweise erhalten. Erster Portable-Fall bestätigt; weitere Fälle/ZIP-Endabnahme offen. Keine Setup-Deinstallation daraus abgeleitet. [Portable-Bericht](VM-PORTABLE-BUILD47-20261031.md).

**31.10.2026 – VM-Setup Build 47, drei Wege bestätigt:** Nutzer hält fest: **MP4Box mit QUICK AND COPY; otr-can mit QUICK AND COPY und AUDIO-ENTHUSIAST**. Neuer otr-can-QUICK-Mehrspurlauf in **42,79 s**, Bild/Ton/Drei-Sekunden-Vorschau positiv. Furiosa-otr-can mit AUDIO-ENTHUSIAST in **5:37,95 min**, **387448320 Samples**, Klebestelle **01:54:18.320**, Dauer **02:14:31.840** technisch geprüft; Bild/Ton persönlich bestanden. Drei Verarbeitungsläufe und Sperr-/Dateiwechselchecks positiv; vollständige VM-Endabnahme, Deinstallation und Portable separat offen. [VM-Bericht](VM-SETUP-BUILD47-20261031.md#vom-nutzer-festgehaltene-verfahrenszuordnung).

**31.10.2026 – VM-Dialogwechsel bestätigt:** Furiosa-MP4Box-Paar zeigt die AUDIO-ENTHUSIAST-Sperrmeldung; nach Auswahl des otr-can-Paars verschwindet sie. Zwei Screenshots und persönliche Bestätigung positiv. Kein weiterer Verarbeitungslauf; AUDIO-ENTHUSIAST benötigt weiterhin seine beiden Tonspurauswahlen. [VM-Nachweis](VM-SETUP-BUILD47-20261031.md#zusätzlicher-dialogcheck-von-mp4box-zu-otr-can-wechseln).

**31.10.2026 – VM-Setup Build 47 persönlich begonnen:** Nutzer bestätigt die neue MP4Box-Sperre für AUDIO-ENTHUSIAST. QUICK AND COPY mit beiden deutschen AAC-/AC-3-Stereospuren (48000 Hz) erfolgreich in **27,83 s**, Bild/Ton/Drei-Sekunden-Vorschau persönlich positiv. Hinweise in der Meckerecke erhalten, Ausgabeübernahme und Bereinigung laut Protokoll erfolgreich. Weitere VM-Fälle, Endabnahme, Deinstallation und Portable-ZIP bleiben offen; Build-46-Nachweise getrennt erhalten. [VM-Bericht](VM-SETUP-BUILD47-20261031.md).

**31.10.2026 – Build-47-Pakete erstellt und geprüft:** Setup, Portable-ZIP, CAN-/Motor-/libmpv-Quellen und Prüfsummen unter **artifacts/release-1.0.2-build47**. 1593 Pakettests plus 1321 Tests gegen finale Produkt-DLLs, native Herkunfts-/Dialogprüfung und zwei Dialoggrößen positiv. 705 manifestierte Quellen/706 Quellen-ZIP-Dateien, 250 Portable-Dateien vollständig geprüft; acht Auslieferungsdateien und 706 Snapshot-Dateien schreibgeschützt. Alle 376 Produkt-/Testdateien seit Auftrag und 58 geschützte Ursprungsdateien unverändert. Dialog persönlich bestätigt, neue persönliche Paketabnahme offen. Build 46 erhalten, kein Commit/Push/Release. [Prüfbericht und Prüfsummen](RELEASE-1.0.2-BUILD47-RESULT.md).

**31.10.2026 – Build-47-Dialogcheck bestätigt, Pakete beauftragt:** Nutzer bestätigt die MP4Box-Sperre mit zwei John-Wick-Teilen und beauftragt Setup, Portable-ZIP und Quellen. Bestätigten Produktcode und Buildnummer 47 unverändert in separaten Snapshot übernehmen, daraus Pakete erstellen und prüfen; frühere Build-46-Auslieferung erhalten. Neue persönliche Paketabnahme offen, keine Veröffentlichung oder Git-Aktion beauftragt. [Paketbericht](RELEASE-1.0.2-BUILD47-RESULT.md).

**31.10.2026 – Entwicklungsbuild 47:** Nutzerentscheidung umgesetzt: AUDIO-ENTHUSIAST bei erkanntem GPAC/MP4Box-Filmteil gesperrt, einschließlich Tonspurauswahl; sichtbarer Hinweis bei den Startknöpfen verweist auf QUICK AND COPY. Container-Kennung unabhängig vom Dateinamen lesen; Sperre auch für gemischte Paare und im Dienst. Release-Rebuild ohne Warnungen/Fehler, **1593 Tests**, native Furiosa-/otr-can-Erkennung und zwei Dialoggrößen geprüft. Buildzähler jetzt **47**; neue persönliche Dialog-/Paketabnahme offen. Build-46-Pakete und deren laufende VM-Prüfung bleiben getrennt erhalten; keine neuen Pakete oder Veröffentlichung. [Umsetzung und Grenzen](MP4BOX-AUDIO-ENTHUSIAST-SPERRE-20261031.md).

**31.10.2026 – Furiosa / MP4Box auch lokal vor AUDIO-ENTHUSIAST abgewiesen:** Die jetzt im Screenshot benannten Dateien (Teil 1 mit `22_09`) lesend analysiert. Beide H.264-Videos sind progressiv und beginnen bei null, melden aber nominell 25 fps bei abweichender mittlerer Bildrate. Das verletzt die aktuelle strenge Bildvorprüfung. AAC zusätzlich etwa 21,271 bzw. 14,083 ms kürzer als Bild; H.264-Konfigurationshashes verschieden. Tonauffüllung allein löst diesen Fall nicht; keine Produktänderung umgesetzt. Separater lokaler Befund, kein weiterer VM-Erfolg. [Messwerte und Lösungsumfang](FURIOSA-MP4BOX-AUDIO-ENTHUSIAST-20261031.md).

**31.10.2026 – offener VM-Befund Build 46:** AUDIO-ENTHUSIAST weist zwei MP4Box-Teile bereits im Auswahldialog mit der blauen Meldung zu gleichen Bild-/Tonzeiträumen ab null ab. Nutzer bestätigt: kein Lauf startet. Strenge Metadaten-Vorprüfung im Code identifiziert; konkrete Start-/Endabweichung dieses Dateipaars noch nicht gemessen. Frühere MP4Box-Protokolle mit 40 ms kürzerem Ton sind ein Hinweis. Drei positive Funktionsfälle bleiben bestehen; Prüfrunde läuft, Produktcode unverändert. [Diagnosestand](VM-SETUP-BUILD46-20261031.md#offener-befund-zwei-mp4box-teile-mit-audio-enthusiast).

**31.10.2026 – VM-Setup Build 46, dritter Funktionsfall positiv:** QUICK AND COPY mit John Wick Kapitel 4 Teil 1/2 und jeweils einer AAC-Stereospur (Deutsch, 48000 Hz) erfolgreich in **28,88 s**. Ton, Bild und Drei-Sekunden-Vorschau persönlich bestätigt. Gewählte Spuren ohne Neukodierung vorbereitet und verbunden; Audio-DTS-Warnung und Laufzeitunterschied in der Meckerecke erhalten, Ausgabeübernahme und Bereinigung erfolgreich. Drei Funktionsfälle und Installation positiv; VM-Prüfrunde weiterhin laufend. [Nachweis](VM-SETUP-BUILD46-20261031.md#fall-3-quick-and-copy-mit-einer-aac-spur).

**31.10.2026 – VM-Setup Build 46, zweiter Funktionsfall positiv:** AUDIO-ENTHUSIAST mit John Wick Kapitel 4 Teil 1/2 und AAC/AAC erfolgreich in **00:05:54.41**; Bild, Ton und Drei-Sekunden-Vorschau persönlich bestätigt. **457399680 Tonsamples**, Klebestelle **01:57:37.120**, Gesamtdauer **02:38:49.160** technisch geprüft; Ausgabeübernahme und Bereinigung erfolgreich. Installation und beide Klebeverfahren damit positiv gemeldet; die VM-Prüfrunde läuft weiter. [Nachweis](VM-SETUP-BUILD46-20261031.md#fall-2-audio-enthusiast-mit-john-wick-kapitel-4).

**31.10.2026 – VM-Setuprunde Build 46 begonnen:** Installation von CAN 1.0.2 Build 46 persönlich bestätigt und im Screenshot sichtbar. QUICK AND COPY mit beiden bestätigten AAC-/AC-3-Stereopaaren (Deutsch, 48000 Hz) erfolgreich in **30,03 s**; Ton, Bild, Schnitt/Klebestelle und Drei-Sekunden-Vorschau persönlich positiv. Technische Hinweise in der Meckerecke erhalten, Ausgabeübernahme und Bereinigung laut Protokoll erfolgreich. Nutzer testet weiter; Deinstallation und Portable-ZIP Build 46 noch nicht bestätigt. [Laufender VM-Bericht](VM-SETUP-BUILD46-20261031.md).

**30.10.2026 – weiterer persönlicher Schnittcheck bestanden:** „John Wick – Kapitel 3“ (HD-MP4 vom 20.10.2026), sechs Behaltebereiche, mit MP4Box und otr-can 0.1.6 jeweils Schnitt/Ton/Bild/Drei-Sekunden-Vorschau positiv. Beide Erfolgsprotokolle liegen vor: MP4Box-Schnittstellenprüfung aller sechs Abschnitte abgeschlossen; otr-can verifiziert 183126 Videopakete und je 351601920 decodierte Samples für AAC und AC-3 (jeweils Stereo/48000 Hz) sowie die finale MP4. CAN-Buildnummer und Installationsvariante bleiben offen. [Testergebnis](TESTPLAN.md#weiterer-fall-john-wick--kapitel-3).

**30.10.2026 – persönliche Schnittprüfung bestätigt:** „Das Weihnachtsschloss“ mit otr-can 0.1.6: Bild, Ton, Schnittübergänge und Drei-Sekunden-Vorschau erfolgreich geprüft; der Nutzer bestätigt dieselben Prüfpunkte auch für den MP4Box-Schnitt. Das übermittelte otr-can-Protokoll bestätigt fünf Behaltebereiche, 134128 Videopakete und 257525760 verifizierte Tonsamples einer AAC-Stereospur mit 48000 Hz. CAN-Buildnummer und Installationsvariante sind nicht angegeben; der MP4Box-Lauf ist persönlich bestätigt, ohne eigenes technisches Protokoll. [Testergebnis](TESTPLAN.md#persönliche-schnittprüfung-am-30102026).

**29.10.2026 – Entwicklungsbuild 46:** Nutzerkorrektur umgesetzt: QUICK ist pragmatischer FFmpeg-Concat mit Streamcopy, bestätigter AAC-/AC-3-Mehrspurzuordnung und **Meckerecke – technische Hinweise**. Warnungen, H.264-Hashunterschiede und Sampleabweichungen allein verhindern den Erfolg nicht. Keine zusätzlichen Vollprüfungen. Mindestprüfung, Originalschutz, Protokoll, Phasenzeiten und Drei-Sekunden-Kontrolle bleiben; QUICK-Übergang ausdrücklich angenähert. AUDIO-ENTHUSIAST unverändert. **1576 Tests**, **27 native QUICK-Fälle**, **21 bisherige native Fälle** positiv, neun Neukodierungsausgaben bytegleich. Reale MP4Box-/otr-can-Furiosa-Mehrspurläufe und John-Wick-AAC technisch erfolgreich. Zwei eigene MP4Box-QUICK-Läufe mit AAC+AC-3 persönlich auf Bild/Ton/Klebestelle/Drei-Sekunden-Vorschau bestätigt: Furiosa 23,74 s und anschließend John Wick 16,44 s. Das doppelte Furiosa-Protokoll zählt weiterhin als ein Lauf. Stand jetzt auf Nutzerauftrag eingefroren: Setup/ZIP/Quellen vollständig erstellt, technisch geprüft und schreibgeschützt; Quellstand nach Paketprüfung committet und gepusht. Veröffentlichung erst nach neuer Ansage. [Paketbericht](RELEASE-1.0.2-BUILD46-RESULT.md). [Aktueller Nachweis](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md).

**Historische Diagnose vor der Nutzerkorrektur:** Die [MP4Box-Furiosa-Diagnose](KLEBECENTER-MP4BOX-FURIOSA-DIAGNOSE-20261029.md) und [manuelle Referenzprüfung](KLEBECENTER-MP4BOX-REFERENZPRUEFUNG-20261029.md) erklären die Ablehnung in Build 45. Die nachfolgende verbindliche Korrektur hebt deren strenge QUICK-Abbruchregeln und Rücksprache-Sperre ausdrücklich auf. Eingefrorene Build-45-Pakete bleiben unverändert.

**29.10.2026 – Entwicklungsbuild 45:** Checkbox um „nur bei übereinstimmendem Tonformat“ ergänzt, Hinweis auf einmalige Zuordnung und Abwahl nicht benötigter Paare. Nur Beschriftung/Textumbruch; Logik und Motor unverändert. 1552 Tests und vier native Anzeigen-/Auswahlfälle positiv. Hinweis persönlich bestätigt; Stand und Pakete als Build 45 erstellt, technisch geprüft und schreibgeschützt eingefroren; neue persönliche Paketabnahme offen. [Paketbericht](RELEASE-1.0.2-BUILD45-RESULT.md). [Nachweis und laufende Setup-Prüfrunde](KLEBECENTER-TONSPURHINWEIS-20261029.md).

**Nachtrag 29.10.2026 – Entwicklungsbuild 44:** separaten Tonspurkopie-Menüeintrag aus dem Klebecenter entfernt; Auswahl weiterhin direkt beim Zusammenfügen. Nur Menüänderung, Kopierdienst und beide Klebeverfahren unverändert. Menü vom Nutzer persönlich bestätigt; Setup/Portable/Quellen für Build 44 jetzt erstellt und technisch geprüft; persönliche Paketabnahme offen. Build-43-Pakete bleiben erhalten. [Paketbericht](RELEASE-1.0.2-BUILD44-RESULT.md). Spätere Werkzeugrubrik mit Tonspurkopie, Martins Werbebildervorschau und Untertitelanzeige auf der [Merkliste](SETUP-MERKLISTE.md).

**Abgleich 28.10.2026: CAN 1.0.2 Build 41 · otr-can 0.1.6 · libmpv 0.41.0 · .NET 10/WPF · Windows 11 x64.** [Versionen](VERSIONEN.md), [Nutzeranleitung](NUTZERANLEITUNG.md).

## Versionen und Auslieferung

| Bereich | Aktueller Stand |
|---|---|
| Öffentlich dokumentierter stabiler Stand | CAN 1.0.2 Build 41, veröffentlicht als v1.0.2 / Latest; Rücknahme von 1.0.1 bleibt dokumentiert |
| Veröffentlichter Paketsatz | CAN 1.0.2 Build 41; App-Dateiversion 1.0.2.41 |
| Lokaler Freeze/Paketauftrag | Build 46: bestätigtes pragmatisches QUICK und Meckerecke eingefroren; Setup/ZIP/Quellen technisch geprüft und schreibgeschützt, Quellstand committet/gepusht. Veröffentlichung gesperrt bis erneuter Ansage; Build 45 erhalten |
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


</details>
