# Radical HD – OTR-CAN persönlich geprüft, MP4Box-Werbeblitzer nachgewiesen

Stand: **16.10.2026**. Rückmeldung zur laufenden lokalen 1.0-Prüffassung, weiterhin **0.3.0 · Build 31 · RC3**, Motor **otr-can 0.1.5**. Reine Untersuchung und Dokumentation; keine Motoränderung und keine Veröffentlichung.

## Persönliche Rückmeldung

Der Nutzer meldet für **Radical – Eine Klasse für sich**, HD-Aufnahme vom 08.10.2026, nach erfolgreichem otr-can-Schnitt **keine Werbeblitzer**. Der anschließend gezeigte Cutlist-Server-Screenshot ergänzt ausdrücklich: **„Mit otr-can 0.1.5 blitzfrei. Bild, Ton und alle Schnittübergänge geprüft.“** Damit ist genau diese persönliche Film-/Ton-/Übergangsprüfung positiv bestätigt, keine allgemeine Formatfreigabe. Eine getrennte persönliche Einzelprüfung jeder Tonspur wird aus der zusammenfassenden Tonrückmeldung nicht abgeleitet.

Für MP4Box meldet der Nutzer: Schnitt läuft durch, aber **Werbeblitzer bei 57:27.314**. Die Cutlist ist im Screenshot auf dem Server sichtbar und enthält den entsprechenden MP4Box-Hinweis. Der Screenshot allein belegt nicht den genauen technischen Uploadweg; kein Upload durch den Diagnosehelfer oder diese Chat-Ausführung.

## OTR-CAN-Protokoll und Metadaten

Die vom Nutzer gepostete Ausführung bestätigt genau eine FFMS2-Indexierung und erfolgreiche Vorprüfung von **464866** eindeutigen, aufsteigenden Bildzeiten auf dem unterstützten 25/50-fps-Raster. Originalvideo: H.264, 50 fps, Videodauer 9297,320 s.

Behaltebereiche im Protokoll:

| Bereich | Anfang | Ende exklusiv | Dauer | Bilder |
|---|---:|---:|---:|---:|
| 1 | 950,800 s | 4397,640 s | 3446,840 s | 172342 |
| 2 | 4814,560 s | 8376,780 s | 3562,220 s | 178111 |

Zusammen **350453 Bilder / 7009,060 s**. Der Motor kopiert geeignete Abschnitte und kodiert die Schnittgrenzen auf der CPU neu. Beide vollständigen Tonspuren einmal neu kodiert und technisch geprüft: **AAC Stereo / 48 kHz** und **AC-3 mit sechs Kanälen / 48 kHz**, je **336434880 dekodierte Referenzsamples**, kontinuierliche Audio-PTS. Abschließende Muxprüfung für Video/Audio, rohe PTS/DTS, Padding und Decoderkonfigurationen erfolgreich. Keine zusätzliche vollständige Neudekodierung aller Bilder durch diese Untersuchung behauptet.

Die vorhandene, ausdrücklich mit `otr-can 2026-10-16` bezeichnete Ausgabe hat laut unabhängigem ffprobe-Aufruf für Video und beide Audiostreams übereinstimmend **7009,060 s** Dauer. Die persönliche Qualitätseinschätzung stammt vom Nutzer.

## MP4Box-Vergleich

Untersucht wurden das vorhandene Original und die Datei **„Radical - Eine Klasse fuer sich.mp4“**; keine Datei überschrieben und kein erneuter Schnitt ausgeführt. Die vorhandene MP4Box-Ausgabe hat **7009,697333 s** Videodauer, also **0,637333 s mehr** als die aktuelle otr-can-Ausgabe. Das ist zunächst ein Metadatenunterschied, kein alleiniger Qualitätsnachweis.

Die lokal gespeicherte ältere Cutlist beginnt den ersten Behaltebereich bei **950,780 s**, das neue otr-can-Protokoll bei **950,800 s**: Unterschied ein Bild bei 50 fps. Der zweite Behaltebeginn ist in beiden Fällen **4814,560 s**; die erste Endgrenze ist jeweils **4397,640 s**. Daher keine Behauptung eines vollständig identischen A/B-Schnittplans, wohl aber eine gemeinsame eindeutige Sollgrenze an der konkret untersuchten zweiten Behaltekante.

### Paketnachweis an der gemeldeten Stelle

Videopakete aus kurzen Originalfenstern um beide Übergangskanten und aus der MP4Box-Ausgabe um 57:27 wurden per SHA-256 des komprimierten Paketinhalt verglichen. **348** Ausgabepakete ließen sich eindeutig Originalpaketen zuordnen; **32** davon liegen außerhalb der vorgesehenen Behaltebereiche. Davon liegen **31** unmittelbar vor dem zweiten Behaltebeginn, von **4813,940 bis 4814,540 s**. Ein weiteres Paket bei **4397,660 s** liegt hinter der ersten Endgrenze.

Der zweite kopierte Abschnitt beginnt mit einem als Keyframe markierten Originalpaket bei **4813,940 s** statt bei **4814,560 s**: **0,620 s früher**. CANs aktueller MP4Box-Pfad verwendet laut Quellcode `-splitx`. Die Hilfe der tatsächlich installierten MP4Box-Version beschreibt dafür das Verschieben des Startzeitpunkts zum nächstgelegenen Random Access Point. Der beobachtete Abschnittsbeginn passt dazu. Keine Aussage, dass jeder Keyframe unter allen H.264-Varianten ein geeigneter unabhängiger IDR-Start ist.

Nahe der Nutzerposition:

- Gemeldeter Blitzer: Ausgabe **3447,314 s / 57:27.314**.
- Nächstes entsprechendes Videopaket: Ausgabe **3447,317333 s**.
- Identischer komprimierter Paketinhalt: Original **4814,400 s**.
- Dieses Originalbild liegt **0,160 s vor** dem Sollbeginn 4814,560 s und gehört zum ausgeschlossenen Bereich.

### Dekodierter Bildnachweis

Zusätzlich genau ein Video-Bild ab der gemeldeten Ausgabeposition und genau ein Originalbild bei 4814,400 s dekodiert, ohne Audiobearbeitung oder neue Ausgabedatei. Identischer SHA-256 der rohen YUV420P-Bilddaten:

`4CB9CF73BE8D797227F1FFCDCC22D0ECB56784188358E40537BE1FA21F5E4BB2`

Damit ist das tatsächlich angezeigte fremde Originalbild an der gemeldeten Position technisch bestätigt, nicht nur das Vorhandensein möglicherweise unsichtbarer Preroll-Pakete. Ob das Bild Werbung zeigt, stammt aus der persönlichen Sichtprüfung des Nutzers; keine zusätzliche eigene Filmszenenbewertung behauptet.

## Einordnung und Konsequenz

Für diesen Film ist **otr-can 0.1.5 persönlich blitzfrei abgenommen**. Der erfolgreiche MP4Box-Prozessabschluss garantiert an nicht passenden Zugriffspunkten keine framegenauen Grenzen. Der konkrete Blitzer ist bestätigt und bleibt als bekannte Medien-/Motorgrenze offen. Kein Nachweis einer durch das neue Layout verursachten Regression und keine Korrektur behauptet.

Für die hier bereits geprüfte framegenaue Ausgabe ist die vorhandene otr-can-Datei geeignet. Eine Änderung des MP4Box-Schnittmodus oder des Umgangs mit GOP-Grenzen wäre ein gesonderter Schnittmotor-Auftrag mit eigener Bild-/Ton-/Playerprüfung; Bestandteil dieser Untersuchung ist ausschließlich die Eingrenzung.

## Nachweise und Erhaltung

Diagnose unter `.build/radical-mp4box-20261016`:

- `PACKET-COMPARISON.json`: Zuordnung der Pakete, ausgeschlossene Bilder und Nähe zur gemeldeten Zeit.
- `DECODED-FRAME-COMPARISON.json`: gleicher dekodierter Bildhash.
- Vorher-/Nachher-Erhaltungsprüfung der übrigen Quellen und des Buildzählers.

Original und vorhandene Ausgaben ausschließlich gelesen; keine reale Videodatei in Git aufgenommen. Vergleich identifiziert die aktuell vorhandenen Dateien anhand von Dateinamen/Metadaten und lokalen Paket-/Bildhashes, ohne eine vollständige SHA-256-Provenienz aller großen Videodateien zu behaupten. Keine Änderung an Anwendung, Schnittmotoren, Werkzeugkonfiguration, Lockdateien, Version, Buildzähler, eingefrorenen Paketen oder Releases.

Technischer Ausgangsnachweis der unveränderten Anwendung: Release **0 Warnungen/0 Fehler**, **1347/1347 Tests**. Die reale Diagnose ist eine zusätzliche gezielte Einzelprüfung und ersetzt keine allgemeine Medienabnahme.
