# Klebecenter – QUICK AND COPY / AUDIO-ENTHUSIAST

**Historischer Nachweis:** Die anschließend verbindlich korrigierte Nutzeranweisung ist ab [Build 46](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md) umgesetzt. QUICK arbeitet nun pragmatisch; die hier beschriebenen strengen Abbruchregeln und eine damalige Rücksprache-Sperre gelten nicht mehr für den aktuellen QUICK-Produktweg. Messungen und der eingefrorene Build-45-Stand bleiben erhalten.

**Lokale Prüffassung CAN 1.0.2 Build 43 vom 28.10.2026.** Entwicklungsbasis Build 42; veröffentlicht bleibt CAN 1.0.2 Build 41, Motor 0.1.6 unverändert. Dieser Auftrag ersetzt den zuvor vorgeschlagenen ausschließlich sampleidentischen Kopiermodus. Mehrspurunterstützung für AAC und AC-3 ist verbindlich.

## Vor der Produktänderung festgelegte Akzeptanzregeln

QUICK AND COPY übernimmt alle vom Anwender bestätigten Spurpaare; eindeutige kompatible AAC-/AC-3-Paare werden gemeinsam vorgeschlagen. Zuordnung und entfallende Spuren stehen vor dem Start sichtbar da und müssen bestätigt werden. Keine Ergänzung fehlender Spuren, keine automatische Neukodierung oder Verfahrensumschaltung. AUDIO-ENTHUSIAST bleibt bei einer gewählten Spur pro Teil und seinem unveränderten strengen Build-42-Ablauf. Die eigenständige Tonspurkopie bleibt erhalten.

1. Alle bestehenden Videoanforderungen bleiben: progressive H.264-MP4, identische Konfiguration, 25/50 fps, Raster ab null, vollständige unveränderte Pakete und exakte PTS/DTS/Dauern mit Teilversatz. Keine Korrektur von Videopaketen oder Videozeiten.
2. Je Tonpaar gleicher Codec (AAC-LC oder AC-3), Konfiguration, Rate und Kanalbelegung. Beide Quelltonachsen müssen wie bisher exakt den jeweiligen Film ab null abdecken. Vollständige strenge Quelldekodierung und genaue Quellsamplezahlen bleiben erforderlich.
3. Komprimierte Audionutzdaten, Paketgröße, Reihenfolge und Anzahl müssen vollständig identisch bleiben. Keine verlorenen, ergänzten oder neu kodierten Audiopakete.
4. Zeitkorrekturen sind nur an der einen inneren Naht erlaubt: Beim ersten AAC-Paket von Teil 2 darf FFmpeg genau max(erwartete DTS, vorherige DTS + 1) verwenden, maximal 1025 Sampleticks. Nur dadurch begründete Dauern der beiden direkt angrenzenden AAC-Pakete dürfen sich ändern. Alle übrigen PTS/DTS bleiben exakt. Bei AC-3 sind keine DTS-Korrekturen erlaubt; eine einzige Nahtüberlappung muss exakt aus nachgewiesenem Endpadding von Teil 1 plus Vorlauf von Teil 2 folgen und kleiner als ein AC-3-Block bleiben.
5. Bekanntes inneres Skip/Discard kann beim Paketanschluss entfallen. Äußeres AAC-Priming und Endpadding bleiben überprüfbar erhalten; bei AC-3 kann das finale Discard beim MP4-Umpacken entfallen. Jede solche Änderung wird im Protokoll und Ergebnis ausgewiesen. Keine pauschale Freigabe beliebiger Padding-/Flagänderungen.
6. Ausgabeston vollständig streng dekodieren, Samples messen und exakt gegen die aus kopierten Codecblöcken minus tatsächlich wirksamen äußeren Skip-/Discard-Werten berechnete Anzahl prüfen. Abweichung zur getrennt dekodierten Quellreferenz darf nur aus diesen nachgewiesenen Randwerten stammen: nicht negativ, höchstens 3 AAC-Blöcke bzw. 2 AC-3-Blöcke und zusätzlich höchstens 80 ms je Spur. Spurende gegenüber dem Video höchstens ein Codecblock; keine schleichende oder anderweitig unbegründete Verschiebung. Keine Zusage einer PCM-Identität oder subjektiv unhörbaren Naht.
7. Warnungen bleiben vollständig sichtbar. Nur genau die zum geprüften ersten Teil-2-AAC-Paket passende Non-monotonic-DTS-Korrektur und die bekannte MP4-Meldung zur fehlenden AC-3-Codec-Framegröße dürfen nach unabhängiger Paket-/Decoderprüfung akzeptiert werden. Jede weitere Warnung, Decoderstörung oder ungeklärte Zeitänderung führt zum Abbruch. Neukodierung und alle Decoderprüfungen behalten den bisherigen strengen Warnungsabbruch.
8. Ergebnisdialog nennt Verfahren, erhaltene Spuren, Sampledifferenz pro Spur und Korrekturen. Vollständiger Videopaketvergleich und streng dekodierte Sechs-Sekunden-Prüfung aller gewählten Tonspuren an der echten Naht. Persönliche Prüfung von Anfang, Naht und Ende bleibt erforderlich.

Diese Regeln sind aus getrennten Diagnosekopien mit unverändertem lokalem FFmpeg abgeleitet: John Wick zeigt genau eine AAC-DTS-Korrektur um 513 Ticks und zwei angrenzende Paketdaueränderungen; Furiosa AAC um 129 Ticks. Furiosa AC-3 behält alle Paketzeiten, hat an der Naht eine durch 896 + 256 Samples erklärte Überlappung und verliert äußeres Discard von 512 Samples. Die Nutzdaten aller ausgewählten Pakete stimmen vollständig. Nachweise: **.build/quick-copy-20261028/PACKET-DIFFERENCES.json** und Diagnoseprotokolle. Die vier realen Referenzdateien sind vorhanden. Diese Voruntersuchung war noch keine Produktabnahme; der abgeschlossene technische Prüfstand steht unten. Persönliche Build-43-Abnahme bleibt offen.

## Präzisierung aus dem nativen Rückwärts-Test

FFmpeg kann bei exakt gleichen AAC-DTS den ersten Teil-2-Wert um genau einen Tick erhöhen, ohne auf Warnungsstufe eine Meldung auszugeben. Der unabhängige vollständige Paketvergleich erkennt diese Änderung und zeigt sie weiterhin an. Nur Original-DTS = vorherige DTS und korrigierte DTS = vorherige DTS + 1 darf ohne Warnungszeile vorkommen. Für einen echten Rücksprung muss die konkrete Warnung weiter exakt zum Paketnachweis passen. Der erste Entwicklungsversuch hat den Gleichheitsfall kontrolliert abgelehnt; Protokoll bleibt erhalten. Kein Lockern von Video-, Decoder-, Nutzdaten- oder Samplegrenzen.

## Umsetzung in Build 43

- Zwei ausdrücklich gewählte Aktionen mit den beauftragten Beschriftungen, ohne Enter-Standard oder Verfahrensvoreinstellung. Die Erläuterungen bleiben unmittelbar unter den Aktionen sichtbar.
- QUICK-Zuordnung pro Tonpaar mit Auswahl beider Quellen. Eindeutig kompatible AAC-/AC-3-Paare gemeinsam vorgeschlagen; doppelte oder unvollständige Zuordnungen verhindern den Start. Entfallende Spuren je Quelle sichtbar, verpflichtende Bestätigung. Dateitausch nimmt Spurwahl mit und verlangt erneute Bestätigung. Fehlende/mehrdeutige Spuren nie selbständig ergänzen.
- AUDIO-ENTHUSIAST hat seine getrennte bisherige Einzelspurauswahl. Mp4JoinService.cs und seine Build-42-Verarbeitung bleiben bytegleich. Die Oberfläche ergänzt lediglich Verfahrensname und Gesamtzeit im Ergebnis. Die bisherigen Phasennamen bleiben erhalten.
- QuickCopyJoinService ist eigenständig. Ausgewählte Quellen streng prüfen und vollständig dekodieren; je Teil ausschließlich Video und bestätigte Tonspuren in definierter Reihenfolge ohne Neukodierung vorbereiten. Vorbereitete Videopakete unverändert vergleichen; Ton einschließlich eng erklärter AC-3-Endpaddingdarstellung prüfen. Anschließend Concat ohne automatischen Bitstreamfilter, unverändertem Video und exakt gewählten Tonspuren. Keine Codecfilter, kein AAC-/AC-3-Encoder, kein Rückfall auf den anderen Dienst.
- Gesamte Videoausgabe gegen beide Quellen vergleichen. Jede Tonspur vollständig auf Paketidentität, Konfiguration, genaue Zuordnung, begrenzte Zeitänderungen, Randauffüllung und daraus berechnete Samples prüfen. Alle gewählten Ausgabetonspuren streng vollständig dekodieren; tatsächliche Samplezahl muss exakt der Erklärung entsprechen. Keine PCM-Identität behauptet.
- Abschluss mit Videolaufzeit und tatsächlich nachgewiesener Naht, sechs Sekunden streng dekodierter Nahtprüfung einschließlich aller gewählten Spuren, vollständiger Dateiidentität und Übernahme ohne Überschreiben. Zeitangaben sind Video-Präsentationszeiten; die transparent genannte zusätzliche Ton-Endauffüllung kann die Containerdauer geringfügig verlängern.
- Eigener .can-quick-Arbeitsordner. Nach Erfolg nur bekannte Arbeitsdateien, geschlossenes Protokoll und leeren Ordner löschen. Fehler/Abbruch behalten Diagnose; fremde/gesperrte Dateien bleiben sichtbar gemeldet erhalten. Konservative Platzprüfung berücksichtigt die PCM-Prüfung; Spuren und Filmteile werden nacheinander verarbeitet.
- Im Kontrollplayer ist bei mehreren Spuren die Wahl **Tonspur prüfen** sichtbar. Tatsächliche libmpv-IDs werden gelesen, das Umschalten wird zurückgelesen und bestätigt. Die aktuelle Prüfstelle startet mit der gewählten Spur erneut mit drei Sekunden Vorlauf. Einspurige Ausgaben benötigen keine zusätzliche Wahl. Die lange technische QUICK-Ergebnisnotiz bleibt im Abschlussdialog; im Kontrollfenster steht nur ihr kurzer erster Absatz.
- Die vorhandene Funktion **Tonspur für Filmteile auswählen …** ist zusätzlich wieder im Klebecenter-Menü erreichbar. Ihr Dialog, MainWindow.AudioTrackCopy.cs und AudioTrackCopyService.cs sowie die eigentliche Kopier-/Prüflogik bleiben unverändert.

## Automatisierte und native Prüfungen

- Vollständiger vorgeschriebener Release-Build reserviert **43**, 0 Warnungen/Fehler. Weitere lokale Korrekturbuilds behalten denselben Zähler; Dateiversion **1.0.2.43** bestätigt. Version.props bleibt 1.0.2.
- **1552 Tests bestanden:** 200 Core, 72 Cutlists, 351 Media, 929 App; nichts übersprungen. Darin die 1505 bisherigen Tests und 47 zusätzliche Auswahl-/Zuordnungs-/Warnungs-/Zeitachsen-/Sample-/Cleanup-/Playerfälle. Keine Entfernung bestehender Tests.
- **28 native QUICK-Fälle bestanden:** Vorwärts/Rückwärts, andere Videouhr, automatisch vorgeschlagene und bestätigte AAC+AC-3-Paare in beiden Reihenfolgen, nur AC-3, bewusst zweite AAC-Spur, beschädigte entfallende/gewählte Spur, fehlende Spur, inkompatibles Video, Codecfehlzuordnung, gleiche Dateien, fehlende Bestätigung, doppelte Zuordnung, vorhandene Ausgabe, Abbruch, Ausgabedateirennen, unbekannte Warnung, Decoderwarnung, falsche Samplezahl, manipulierte Audio-/Videonutzdaten, falsche DTS-Warnung, Mono, 44,1 kHz, 5.1 und 50 fps. Positive Mehrspurfälle kontrollieren ausdrücklich H.264 + AAC + AC-3 in der Ausgabe. Negative Fälle nur bei korrekter Ablehnung positiv gewertet. Originale und Erfolg-/Fehlerbereinigung kontrolliert.
- **21 bestehende native AUDIO-ENTHUSIAST-Fälle erneut bestanden. Alle neun erfolgreichen MP4s vollständig bytegleich zu Build 42.** Vorbestehender Neukodierungsdienst und eigenständiger Tonspurkopiedienst bytegleich zur vor Auftrag gesicherten Basis.
- WPF-Dialog nativ geprüft: beide kompatiblen Spuren vorgeschlagen, QUICK erst nach Bestätigung aktiv, Änderungen/Tausch widerrufen Bestätigung, keine Vorauswahl einer von mehreren Enthusiast-Spuren, kein Standardverfahren und John-Wick-AC-3-Auslassung sichtbar. Gerenderte Vorschau kontrolliert.
- Echte libmpv-Prüfung an der zweispurigen Furiosa-Ausgabe: AAC-ID 1 und AC-3-ID 2 erkannt, beide gezielt geschaltet und Rücklesebestätigung geprüft; Seek auf 6878,64 s. Testfenster unsichtbar, automatischer Test stummgeschaltet. Dies ist keine persönliche Hörabnahme.

### Befunde während der Entwicklung

Der erste Rückwärts-Test deckte die oben dokumentierte stille Ein-Tick-Korrektur auf; deren genaue Paketregel blieb unverändert, nur der präzise Warnungsabgleich wurde um diesen unabhängig bewiesenen Gleichheitsfall ergänzt. Erste Fehlerdiagnose bleibt erhalten.

Der echte Player-Test zeigte außerdem einen Fehler des vorhandenen Hanuman-Wrappers: Eine asynchrone Abfrage fehlender optionaler Trackmetadaten kann auf dessen Ereignisthread werfen. Die neue optionale String-Abfrage verwendet deshalb den nativen synchronen Getter auf einem Hintergrundthread und fängt ausschließlich PropertyUnavailable/PropertyNotFound ab. Andere Fehler werden weitergegeben; die WPF-Oberfläche wird nicht blockiert. Der erfolgreiche native Umschalttest verwendet diese finale Abfrage. Keine Bibliothek aktualisiert.

Die neue Getter-Testumgebung musste über ihren eigenen Apphost samt den vier bereits mitgelieferten nativen DLLs gestartet werden, damit die Windows-DLL-Suche der regulären CAN-Ausgabe entspricht. Keine Änderungen an der Produkt-Laufzeitstruktur.

## Reale Referenzen und Qualitätsgrenzen

Erste vollständige Produktläufe bestanden:

| Quelle / gewählte Spuren | Videopakete | unveränderte Tonpakete | Samples Quelle → Ausgabe je Kanal | Differenz |
|---|---:|---:|---|---:|
| John Wick, AAC/AAC | 238229 | 446682 AAC | 457399680 → 457401216 | +1536 / 32,000 ms |
| Furiosa, AAC/AAC | 202379 | 379463 AAC | 388567680 → 388568832 | +1152 / 24,000 ms |
| Furiosa, AC-3/AC-3 | dieselbe Videospur | 252975 AC-3 | 388567680 → 388569344 | +1664 / 34,667 ms |

John Wick: **02:38:49.160**, Naht **01:57:37.120**, Vorschau **01:57:34.120**. Die beim finalen Produktbefehl bewahrte Anfangstrimmung erklärt, warum die Sampledifferenz kleiner ist als die +2560 Samples im zuvor vom Nutzer beschriebenen manuellen Befehl; die beiden Befehlsvarianten sind nicht identisch. Die Videopakete und ihre Zeitpunkte bleiben exakt nachgewiesen.

Diese Furiosa-Dateien ergeben **02:14:55.160**, Naht **01:54:41.640**, Vorschau **01:54:38.640**. Sie haben damit eine andere Schnittlänge als der frühere Build-40-Fall mit 02:14:31.840; alte Referenzwerte werden nicht auf die neuen Dateien übertragen. Eindeutige automatische Zuordnung nimmt AAC und AC-3 gemeinsam auf; beide Paketreihen und vollständigen Dekodierungen bestanden.

Die ersten Gesamtzeiten einschließlich aller Prüfungen betrugen 102,48 s für John Wick und 169,51 s für Furiosa mit zwei Tonspuren. Der eigentliche Concat dauerte darin 12,25 bzw. 16,00 s. Diese vollständige Produktprüfung ist nicht mit den ungeprüften manuellen reinen Kopierzeiten 7,09 / 9,41 s gleichzusetzen. Ein abschließender sequenzieller Vergleich beider John-Wick-Verfahren unter gleichen Grenzen wird zusätzlich separat festgehalten.

**Für die technischen Entwicklungsreferenzen bleibt die persönliche Abnahme getrennt zu betrachten. Der unten dokumentierte eigene Furiosa-Lauf ist inzwischen persönlich auf Bild, Ton und Naht samt Drei-Sekunden-Fenster bestätigt.** Die vom Nutzer berichteten manuellen Hörtests sind getrennte Vorgänge. Vollständige Videonutzdaten-/Zeitachsenidentität und Nahtdekodierung sind nachgewiesen; keine zusätzliche vollständige Videodekodierung des ganzen Films behauptet. Nicht kompatible oder nicht durch die engen Regeln erklärte Fälle werden abgelehnt. Andere FFmpeg-Versionen mit abweichenden Warnungsformaten können konservativ scheitern.

## Nachweise und unveränderte Grenzen

**.build/quick-copy-20261028/** enthält Build-42-Quellsicherung, BEFORE.json, reale Metadaten/Paketvergleiche, erste Diagnose- und Produktläufe, NATIVE-RESULTS.json, ENTHUSIAST-NATIVE.json, ENTHUSIAST-BYTE-PROOF.json, final-tests/, Dialogbild/-Prüfung, NATIVE-PLAYER.json und abschließenden Realvergleich. Echte Filmdateien bleiben im ignorierten Prüfverzeichnis, nicht in Git. Keine Produktionsabhängigkeit hinzugefügt. Alle bestehenden Lockdateien einschließlich der vier Nutzeränderungen bleiben bytegleich zum Beginn; kein Commit, Push, Setup, ZIP oder Release.

Die Verwendung tatsächlicher Tonspur-IDs und der auswählbaren aid-Eigenschaft entspricht der [offiziellen mpv-Dokumentation](https://mpv.io/manual/stable/). Der frühere [Machbarkeitsbericht](KLEBECENTER-ZWEI-AUDIOVERFAHREN-MACHBARKEIT-20261028.md) bleibt als datierter Nachweis des strengeren damaligen Vorschlags erhalten; der spätere ausdrückliche Umsetzungsauftrag erlaubt für QUICK die hier dokumentierte andere Qualitätszusage.

## Abschließender Vergleich mit finalen Bibliotheken

Nacheinander, identische aktuelle John-Wick-Eingaben und dieselben FFmpeg-/ffprobe-EXEs. Beide Dienste im identischen überwachten Windows-Job mit 1 GiB und 25 % CPU, keine parallelen Medienoperationen. Ein gemessener kompletter Lauf je Verfahren, kein geleerter Datenträgercache, keine statistische Serienmessung. Zeiten umfassen die jeweilige gesamte Verarbeitung und Prüfungen samt Bereinigung ab Dienststart; Dateiauswahl nicht enthalten.

| Fall | Gesamtzeit des Dienstes | Ergebnis |
|---|---:|---|
| John Wick, AUDIO-ENTHUSIAST AAC/AAC | **309,11 s / 05:09,11** | 457399680 Samples je Kanal, bisherige strenge Nachweise bestanden |
| John Wick, QUICK AND COPY AAC/AAC | **87,15 s / 01:27,15** | 457401216 Samples je Kanal, erklärte +1536 Samples / 32 ms |
| Furiosa, QUICK AND COPY AAC+AC-3 | **138,36 s / 02:18,36** | beide Spuren automatisch vorgeschlagen, ausdrücklich bestätigt und erhalten; getrennte Nachweise bestanden |

Die finalen QUICK-Ausgaben sind vollständig bytegleich zu ihren ersten geprüften Produktläufen. Das belegt reproduzierte Ergebnisse dieser beiden konkreten Dateien trotz unterschiedlicher Messzeiten. Die Laufzeitunterschiede zu den ersten Produktmessungen werden nicht als weitere Codeoptimierung ausgegeben; Datenträgercache und Systemlast wurden nicht normiert. Die separat gelaufenen drei finalen Aufträge sind in **FINAL-REAL-COMPARISON.json** mit vollständigen Phasenprotokollen gesichert.

**Technische Umsetzung abgeschlossen; lokale Build-43-Fassung zur persönlichen Prüfung bereit.** Keine persönliche Qualitätsabnahme der neuen Produktdateien aus der automatisierten Prüfung ableiten. Setup, Portable und Veröffentlichung bleiben unverändert bei Build 41. Keine Git-Aktion oder Paketierung beauftragt/ausgeführt.

## Eigener Nutzerlauf Furiosa mit beiden Tonspuren – 28.10.2026

Der Nutzer legt ein vollständiges erfolgreiches QUICK-Protokoll und die Abschlussinformationen für **Furiosa A Mad Max Saga - zusammengefügt Build 43 quick copy.mp4** vor. Ausgabe im persönlichen Filmordner. Anschließend bestätigt der Nutzer ausdrücklich: **„bild ton und klebeübergang mit 3-sekunden fenster top und check“**. Damit sind Bild, Ton und Klebeübergang mit Drei-Sekunden-Fenster für diesen eigenen QUICK-Lauf persönlich positiv gemeldet.

Eingaben:

- **Furiosa A Mad Max Saga Teil 1 2026-10-22 [18.10.2026] otr-can II.mp4**
- **Furiosa  A Mad Max Saga  Teil 2 otr-can.mp4**

Je Teil Spur 1 AAC und Spur 2 AC-3, Deutsch/Stereo/48000 Hz; beide Paare übernommen. **378370 AAC-Pakete und 252246 AC-3-Pakete unverändert nachgewiesen.** Beide Ausgabetonspuren vollständig dekodiert; jeweils **387449600 Samples je Kanal** gegenüber **387448320** in der getrennten Quellreferenz, also **+1280 Samples / 26,667 ms**. Gleiche Samplezahl beider Ausgabespuren bedeutet keine bewiesene PCM-Identität.

AAC: innere Trimmung 1280 Samples nicht angewendet, Endpadding erhalten. Eine genau zugeordnete DTS-Korrektur von **329198336 auf 329198593** (vorherige DTS **329198592**). AC-3: innere Trimmung 768 Samples und zusätzliches Endpadding 512 Samples erklären die gesamte Differenz. Die bekannte Meldung zur fehlenden AC-3-Codec-Framegröße erscheint in den drei Kopierphasen; deren Akzeptanz ist durch die vollständigen Paket- und Decoderprüfungen abgesichert. Keine Decoderstörung oder fehlgeschlagene Phase im eingereichten Protokoll.

Gesamt **113,759 s (01:53,75)**, davon Concat **11,025 s**; einschließlich aller technischen Prüfungen und erfolgreicher Cleanup-Phase. Letztere meldet normale Rückkehr; eine persönliche Sichtkontrolle des entfernten Arbeitsordners ist damit nicht separat bestätigt.

Ausgabezeiten: Anfang **00:00:00.000**, Naht **01:54:18.320**, Ende **02:14:31.840**, technische Sechs-Sekunden-Nahtprüfung ab **01:54:15.320**. Diese Schnittlänge entspricht dem früher dokumentierten kürzeren Furiosa-Fall und unterscheidet sich von den beiden technischen Entwicklungsreferenzen mit 02:14:55.160. Laufzeiten daher nicht als direkter A/B-Vergleich werten.

Quelle: Nutzeranhang **9d71247b-e965-4d8a-9552-0744298eae98/Eingefügter Text.txt** und anschließend kopierte Abschlussinformationen. Nur Dokumentation ergänzt; Build, Produktcode, bestehende 1552 Tests, Pakete und Git bleiben unverändert. Nachgereichte persönliche Bestätigung: Bild, Ton und Klebeübergang im Drei-Sekunden-Fenster positiv. Eine gesonderte Einzelbestätigung je Codec oder zu Anfang/Ende wurde dabei nicht aufgezählt; solche zusätzlichen Prüfumfänge werden nicht behauptet. Keine pauschale Paket-/VM-Abnahme oder Veröffentlichungsfreigabe daraus ableiten.
