# VM-Setup Build 46 – laufende persönliche Prüfrunde

**Stand 31.10.2026: CAN 1.0.2 Build 46.** Der Nutzer bestätigt die Setup-Installation in der VM mit „Installation 1.0.2 Build 46 check“. Der Screenshot zeigt dieselbe Version im Hauptfenster. Anschließend bestätigt er QUICK AND COPY mit „Ton Bild Schnitt 3 Sekunden Vorschau check, ich teste weiter“. Die Prüfrunde läuft weiter.

## Fall 1: Klebecenter QUICK AND COPY mit AAC und AC-3

Der Screenshot zeigt als zweiten Filmteil **John Wick Kapitel 4 Teil 2 MP4Box.mp4**. Beide Tonspurpaare sind ausgewählt und die Zuordnung ist ausdrücklich bestätigt: jeweils Spur 1 (AAC) und Spur 2 (AC-3), Deutsch, Stereo, 48000 Hz. Laut Anzeige entfallen keine Tonspuren. Daraus wird kein neuer Schnittlauf der Filmteile in dieser VM abgeleitet.

Das übermittelte Protokoll bestätigt für beide Eingaben dieselben zwei gewählten Tonformate. FFmpeg übernimmt Video und beide zugeordneten Tonspuren ohne Neukodierung per Concat und Streamcopy. Gemeldete Ressourcengrenzen: 1 GiB Arbeitsspeicher, 25 % CPU und ein Verarbeitungsthread.

| Phase | Zeit laut Protokoll |
|---|---:|
| Erste Eingabe prüfen | 0,078 s |
| Zweite Eingabe prüfen | 0,057 s |
| FFmpeg-Concat | 25,587 s |
| Ausgabe prüfen | 0,118 s |
| Ausgabe-SHA-256 | 4,164 s |
| Ausgabe übernehmen | 0,000491 s |
| Technische Hinweise | 0,008 s |
| Arbeitsordner bereinigen | 0,004 s |
| Gesamt | **30,0369156 s**, Anzeige **00:00:30.03** |

Alle protokollierten Phasen sind erfolgreich, einschließlich Ausgabeübernahme und Bereinigung. Das ist ein Protokollnachweis der Erfolgsbereinigung; eine zusätzliche persönliche Dateisystemkontrolle wird nicht behauptet.

## Meckerecke im erfolgreichen Lauf

- FFmpeg meldet `track 2: codec frame size is not set`; die Meldung wird in der Meckerecke erhalten und verhindert den Erfolg nicht.
- Teil 1: Bild 7054,480 s; beide Tonspuren jeweils 7054,440 s. Teil 2: Bild 2471,880 s; beide Tonspuren jeweils 2471,840 s.
- Ausgabe: Bild 9526,360 s, AAC 9526,357333 s, AC-3 9526,336 s.
- Containerlaufzeiten: Summe der Filmteile 9526,360 s; Ausgabe 9526,397333 s. Im QUICK-Modus erfolgt keine samplegenaue Tonprüfung.

Diese Hinweise sind Bestandteil des erfolgreichen pragmatischen QUICK-Laufs. Die persönliche Kontrolle von **Ton, Bild, Schnitt/Klebestelle und Drei-Sekunden-Vorschau** ist positiv bestätigt. Die allgemeine Tonbestätigung wird nicht als gesonderte Hörbewertung jeder Spur ausgelegt.

## Fall 2: AUDIO-ENTHUSIAST mit John Wick Kapitel 4

Verwendete Filmteile: **John Wick Kapitel 4 Teil 1.mp4** und **John Wick Kapitel 4 Teil 2.mp4**. Jeweils Spur 1 gewählt: AAC, Deutsch, Stereo, 48000 Hz. Video bleibt unverändert; die beiden gewählten Tonspuren werden vollständig dekodiert, samplegenau per RF64 verbunden und vollständig mit den Ausgangsspuren verglichen. Anschließend wird eine durchgehende AAC-LC-Spur mit 192 kbit/s einmal kodiert.

Das Erfolgsprotokoll bestätigt die Bild-/Quelltonzeitprüfungen, Kopiernachweise, den finalen MP4-Zusammenbau und die Nachprüfung aller Bildpakete/-zeiten sowie AAC-Pakete und Wiedergabezeiten. Der fertige Ton wird vollständig auf exakte Samplezahl geprüft; die Klebestelle wird mit drei Sekunden Vor- und Nachlauf dekodiert. **457399680 Tonsamples**, Klebestelle **01:57:37.120**, Gesamtdauer **02:38:49.160**. Alle protokollierten Phasen einschließlich Ausgabeübernahme und Bereinigung erfolgreich.

| Phase | Zeit laut Protokoll |
|---|---:|
| RF64-Verbindung und PCM-Vergleich | 14,350 s |
| Einmalige AAC-Kodierung | 228,412 s |
| Finaler MP4-Zusammenbau | 13,384 s |
| Abschließender Bildnachweis | 18,445 s |
| Vollständige Dekodierung des fertigen Tons | 13,696 s |
| Dekodierung der Klebestelle | 0,886 s |
| Bereinigung | 0,506 s |
| Gesamt | **354,4070075 s**, Anzeige **00:05:54.41** |

Der Nutzer bestätigt **„bild, ton 3 sekunden vorschau check“**. Bild, Ton und Drei-Sekunden-Vorschau persönlich bestanden; die technische Klebestellenprüfung ist im Protokoll belegt. Dies ist ein eigener AUDIO-ENTHUSIAST-Lauf mit anderen Eingabedateinamen als beim QUICK-Fall; kein unmittelbarer Geschwindigkeitsvergleich identischer Dateien. Die laufende VM-Setup-Zuordnung wird aus der unmittelbar vorangehenden Rückmeldung fortgeführt.

## Fall 3: QUICK AND COPY mit einer AAC-Spur

Eigener weiterer Lauf mit **John Wick Kapitel 4 Teil 1.mp4** und **John Wick Kapitel 4 Teil 2.mp4**, jeweils gewählte Spur 1: AAC, Deutsch, Stereo, 48000 Hz. Für die eindeutige Zuordnung werden zunächst Video und die gewählte Tonspur jedes Teils per Streamcopy vorbereitet. Anschließend werden die vorbereiteten Teile ohne Neukodierung verbunden; das Protokoll zeigt eine Video- und eine Audiozuordnung für die Ausgabe.

| Phase | Zeit laut Protokoll |
|---|---:|
| Vorbereitung Teil 1 | 4,560 s |
| Vorbereitung Teil 2 | 4,426 s |
| Prüfung der vorbereiteten Grenze | 0,333 s |
| FFmpeg-Concat | 14,285 s |
| Ausgabe prüfen | 0,088 s |
| Ausgabe-SHA-256 | 3,518 s |
| Bereinigung | 0,246 s |
| Gesamt | **28,8817387 s**, Anzeige **00:00:28.88** |

Alle protokollierten Phasen einschließlich Ausgabeübernahme und Bereinigung sind erfolgreich. FFmpeg meldet **Non-monotonic DTS** für die kopierte Audiospur: vorher 338742272, aktuell 338741760, auf 338742273 geändert. Die Warnung bleibt in der Meckerecke erhalten und verhindert den Erfolg nicht. Bilddauer und Summe der Filmteile jeweils **9529,160000 s**, Ton-/Containerdauer der Ausgabe **9529,181333 s**. QUICK enthält keine samplegenaue Tonprüfung; aus dem erfolgreichen Lauf wird keine exakte Zeitstempel- oder Sampleidentität abgeleitet.

Der Nutzer bestätigt **„ton, bild, 3 sekunden vorschau check“**. Ton, Bild und Drei-Sekunden-Vorschau persönlich bestanden. Dieser AAC-Einspurfall wird getrennt vom QUICK-Mehrspurfall und vom AUDIO-ENTHUSIAST-Lauf gezählt. Die VM-Setup-Zuordnung wird aus der laufenden Prüfrunde fortgeführt.

## Offener Befund: zwei MP4Box-Teile mit AUDIO-ENTHUSIAST

Nach den drei erfolgreichen Funktionsfällen meldet der Nutzer ein Problem mit zwei MP4Box-Teilen und AUDIO-ENTHUSIAST. Der Screenshot zeigt die blaue Meldung: **„Bild und gewählte Tonspur müssen denselben vollständigen Zeitraum ab null haben; eine automatische Zeitkorrektur erfolgt nicht.“** Auf Nachfrage bestätigt der Nutzer: „es passiert nichts sondern die blaue Meldung kommt“. Dies ist eine Ablehnung vor dem eigentlichen Zusammenfügen, kein abgeschlossener oder bestandener weiterer Lauf.

Der Quellcode ordnet die Meldung der Metadaten-Vorprüfung `FfprobeStreamCopyVerifier.ParseJoinPart` zu. Sie verlangt unter anderem Startzeit null und exakt gleiche Bild-/Tonlaufzeiten in ihren jeweiligen Zeitbasen. Die Vorprüfung läuft im Auswahldialog vor Ausgabedateiwahl und Start des Dienstes. Der Fehler wird dort als blauer Statustext angezeigt. Die fehlende QUICK-Zuordnungsbestätigung im Screenshot sperrt nur QUICK; AUDIO-ENTHUSIAST verwendet seine eigenen beiden Tonspurauswahlen.

Die früher übermittelten MP4Box-/QUICK-Messwerte aus Fall 1 zeigen bei beiden Teilen jeweils 40 ms kürzeren Ton als Bild. Solche Abweichungen reichen für die strenge AUDIO-ENTHUSIAST-Ablehnung aus. Das ist ein passender Hinweis, aber noch kein gemessener Nachweis für die jetzt ausgewählten Dateien: Die Eingabepfade sind im neuen Screenshot nicht sichtbar, und die konkrete auslösende Metadatenbedingung ist nicht einzeln ausgewiesen. Auch weitere Format-/Zeitbedingungen verwenden dieselbe Sammelmeldung.

**Stand:** Nutzerbestätigte Vorprüfungsablehnung, Ursache im konkreten Dateipaar noch zu präzisieren. Kein Nachweis beschädigter Dateien oder eines VM-spezifischen Fehlers. Prüfregeln und Produktcode unverändert; eine mögliche Anpassung der Zeitachsenbehandlung ist noch nicht umgesetzt. Nächster Untersuchungsschritt: exakte ffprobe-Start-/Endzeiten der ausgewählten Bild-/Tonspuren und die auslösende Bedingung ermitteln.

## Laufender Abnahmestand

**Getrennte Weiterentwicklung nach den Ablehnungen:** In Entwicklungsbuild 47 wird AUDIO-ENTHUSIAST für anhand der GPAC-Kennung erkannte MP4Box-Filmteile bereits im Dialog gesperrt. [Umsetzung und technische Prüfung](MP4BOX-AUDIO-ENTHUSIAST-SPERRE-20261031.md). Die hier dokumentierten persönlichen VM-Fälle stammen weiterhin aus Build 46; die neue Sperre ist damit noch nicht persönlich abgenommen.

| Prüfung | Stand |
|---|---|
| Setup-Installation in der VM | Persönlich bestanden; Build 46 im Screenshot sichtbar |
| QUICK AND COPY mit AAC und AC-3 | Technisch erfolgreich, persönliche Bild-/Ton-/Schnitt-/Vorschaukontrolle bestanden |
| AUDIO-ENTHUSIAST mit AAC/AAC | Technisch erfolgreich, Bild/Ton/Drei-Sekunden-Vorschau persönlich bestanden |
| QUICK AND COPY mit einer AAC-Spur | Erfolgreich mit dokumentierter DTS-Warnung; Ton/Bild/Drei-Sekunden-Vorschau persönlich bestanden |
| Zwei MP4Box-Teile mit AUDIO-ENTHUSIAST | Vorprüfungsablehnung persönlich bestätigt; offener Befund, nicht als Erfolg gezählt |
| Weitere VM-Funktionsfälle | Nutzer testet weiter |
| Deinstallation und Portable-ZIP Build 46 | Noch nicht bestätigt |

Drei positive Funktionsfälle und die Installation sind dokumentiert; keine vollständige VM-Endabnahme oder Veröffentlichungsfreigabe. Frühere Build-40-VM-Nachweise und die Schnittprüfungen vom 30.10.2026 bleiben getrennt zugeordnet. Produktcode, Buildnummer und eingefrorene Pakete unverändert.

## Prüfung des Dokumentationsnachtrags

Release-Build mit unveränderter Buildnummer 46 erfolgreich, 0 Warnungen/Fehler. Alle **1576 Tests** bestanden (200 Core, 72 Cutlists, 375 Media, 929 App; keine übersprungen). `git diff --check` fehlerfrei. Geändert wurden ausschließlich README, Projektstand, Testplan, Tagebuch, Paketbericht und dieser neue VM-Bericht; die vier vorbestehenden lokalen Produktions-Lockänderungen sind erhalten. Diese Entwicklungsprüfungen ergänzen die Dokumentation und zählen nicht als weitere persönliche VM-Fälle.

[Testplan](TESTPLAN.md), [Projektstand](AKTUELLER-STAND.md), [Build-46-Paketbericht](RELEASE-1.0.2-BUILD46-RESULT.md).
