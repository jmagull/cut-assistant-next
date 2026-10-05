# Entwicklungstagebuch

## 05.10.2026 – Timeline per Mausklick positionieren

- Ein Klick auf die Slider-Spur oder die farbige Schnitt-Timeline setzt die Wiedergabeposition direkt auf die angeklickte Stelle. Reglerziehen und die bisherige Auswahl roter Schnittbereiche bleiben erhalten.
- Die horizontale Klickposition wird auf die Laufzeit abgebildet und auf Anfang/Ende begrenzt; ungültige Koordinaten oder fehlende Laufzeit lösen keinen Seek aus. Der bestehende asynchrone Wiedergabeablauf übernimmt Position, Slider-Aktualisierung und Fehlerbehandlung.
- Neue Tests prüfen Zeitumrechnung, Dateigrenzen, Seek-Verfügbarkeit, laufende Positionierung, Abbruch und Fehler.
- Prüfung am 05.10.2026: Release-Rebuild und anschließender Build des endgültigen Stands erfolgreich, **0 Warnungen, 0 Fehler**; vollständiger Testlauf: **610/610 bestanden**, darunter 33 neue Testfälle, 0 fehlgeschlagen, 0 übersprungen. Die geänderten C#-Dateien wurden formatiert.
- WPF-Prüfung im geladenen Hauptfenster mit Testplayer: Klicks auf graue/rote Schnitt-Timeline und Slider-Spur bei 25/50/75 % setzen Wiedergabe und Slider gemeinsam. Rote Auswahl bleibt umschaltbar und mit dem Schnittplan synchron; Reglerziehen, Schutz vor doppeltem Seek und fehlende Laufzeit geprüft. Keine Bindungsfehler und keine Nutzereinstellungen gespeichert. Die Prüfdateien liegen im ignorierten `.build`-Ordner.
- Der gesperrte Restore scheiterte an den bekannten leeren `win-x64`-Abschnitten der Test-Lockdateien; der reguläre Restore war erfolgreich. Vier zunächst falsch typisierte Testdaten wurden korrigiert. Die ausschließlich vom Restore erzeugten Test-Lockdateiänderungen wurden nach Prüfung aus der Sicherung zurückgenommen.
- Nutzerabnahme am 05.10.2026 bestätigt: Beide Timelines bleiben anklickbar und setzen die Wiedergabeposition; die Auswahl roter Schnittbereiche bleibt erhalten. Die kurz erwogene Beschränkung der unteren Timeline auf die Auswahl wurde vom Nutzer verworfen. Damit sind die drei Punkte Namensmaske, elf Framezeilen und Timeline-Klick abgeschlossen. Version 0.2.1, Build 11 und RC2 bleiben unverändert. Keine Veröffentlichung.

## 05.10.2026 – Elf sichtbare Zeilen in der Frametabelle

- Die Detailtabelle der Frame-Lupe wurde auf elf Framezeilen mit fester Zeilen- und Kopfzeilenhöhe erweitert. Der Detailbereich und die normale Fensterhöhe wurden entsprechend vergrößert; die Vorschau berücksichtigt die tatsächliche Höhe der Details. Kleine Fenster bleiben scrollbar.
- Die bestehende Auswahl-Nachführung bei ±10-Schritten bleibt erhalten. Navigation, Halbierungssuche und Schnittkantenübernahme wurden nicht verändert.
- Prüfung: Release-Rebuild am 05.10.2026 erfolgreich, **0 Warnungen, 0 Fehler**; vollständiger Testlauf: **577/577 bestanden**, 0 fehlgeschlagen, 0 übersprungen. Die Formatprüfung der geänderten C#-Datei war erfolgreich.
- WPF-Darstellungsprüfung mit künstlichen Framedaten: elf vollständige Tabellenzeilen und gemeinsam sichtbarer Ausgangs-/Zielframe nach +10 und −10 bestätigt. Normales Fenster (1050 × 1000), kleines Fenster (780 × 620) und eingeklappte Details geprüft; kleine Fenster bieten Scrollen und erreichbare Abschlussbuttons. Keine WPF-Bindungsfehler. Die Prüfdateien liegen ausschließlich im ignorierten `.build`-Ordner.
- Nutzerabnahme am 05.10.2026 mit realer Videodatei bestätigt: Die Screenshots zeigen elf vollständige Tabellenzeilen (157–167, ausgewählter Frame 162) sowie die angepasste Bildvorschau bei ein- und ausgeklappten Details. Der Nutzer bestätigte die Darstellung mit „super genauso :-)“.
- Version 0.2.1, Build 11 und RC2 bleiben unverändert. Keine Veröffentlichung.

## 05.10.2026 – Standard-Namensmaske ohne Bindestrich vor dem Folgentitel

- Die CAN-Standardmaske setzt vor den optionalen Folgentitel nur noch ein Leerzeichen. Der zugehörige Editorbaustein heißt jetzt **Folgentitel (Leerzeichen)**. Bestehende gespeicherte Masken einschließlich des bisherigen Bindestrich-Präfixes bleiben gültig.
- **CAN-Standardmaske** lädt die neue Vorgabe; die Nutzeranleitung wurde ergänzt und die bestehende Standardmaskenprüfung angepasst.
- Prüfung: Release-Rebuild am 05.10.2026 vom Nutzer erfolgreich ausgeführt, ohne Warnungen oder Fehler; vollständiger Testlauf: **577/577 bestanden**, 0 fehlgeschlagen, 0 übersprungen. Diese Ergebnisse wurden aus der Übergabe übernommen.
- Nutzerabnahme am 05.10.2026: Nach **CAN-Standardmaske** zeigt der Editor **Folgentitel (Leerzeichen)** und die Vorschau `Hunting Party - Die Moerderjagd S02E13 Xander Wax [25.08.2026]`. Der Nutzer bestätigte die Darstellung mit „Passt :-)“.
- Version 0.2.1, RC2 und lokale Buildnummer 11 bleiben unverändert. Keine Veröffentlichung.

## 03.10.2026 – Navy-CIS-Praxistest und Tagesabschluss

- Der Nutzer bestätigte die Lupe, Einzelframeschritte und die Auswahl von Keyframes in der Detailtabelle. Die Übernahme wurde mit dem ersten Filmstart bei 582,240 s praktisch verwendet; anschließend wurde eine korrigierte Cutlist gespeichert und mit CAN geschnitten.
- Die korrigierte Liste enthält die Behaltebereiche 582,240–1902,960 s und 2551,320–3645,400 s. Die geprüften ersten zehn Bilder der CAN-Ausgabe stimmen mit der Quelle ab 582,240 s überein. Im geprüften Übergang wurden keine Werbebilder gefunden; dort traten jedoch doppelte Video-PTS auf. Tonstichproben lagen vor dem Übergang bei 0 ms und danach bei etwa +8 ms gegenüber den angeforderten Quellzeiten. Das ersetzt keine vollständige Bild-/Tonabnahme.
- Externer Test mit CutlistDude 1.1.2 Build 96 unter WSL: zwei Behaltebereiche, Smart Rendering und finaler Mux in 17 Sekunden abgeschlossen. Die Ausgabe beginnt jedoch mit dem Quellbild bei 582,200 s; am zweiten Filmstart wird das Werbebild bei 2551,280 s übernommen. Renderplan und Bildvergleich bestätigen damit einen Ein-Frame-Versatz gegenüber den CAN-Schnittzeiten. Tonstichproben zeigten etwa −51 bis −56 ms gegenüber den angeforderten Quellzeiten; daraus allein folgt wegen der ebenfalls verschobenen Videobilder kein abschließendes Urteil zur Bild-/Ton-Synchronität.
- CutlistDude/CL_OFFSET, MP4Box-Modus und allgemeine Cutlist-Grenzen wurden nicht angepasst. Die externe Frame-Zuordnung bleibt der nächste Untersuchungspunkt. Frame-Lupe, ausdrückliche Kantenübernahme und zusätzliche Halbierungssuche bilden den heutigen abgeschlossenen Funktionsumfang.

## 03.10.2026 – Frame-Lupe, Schritt 3: Gewählten Frame als Schnittkante übernehmen

- Die Lupe zeigt die neue Schnittzeit und bietet **Schnittkante übernehmen**. Nur eine bestätigte, anzeigbare Vorschau mit Original-PTS innerhalb der Datei kann übernommen werden. Laden, fehlende/falsche/leere Vorschau und Bildanzeigefehler sperren die Übernahme.
- Die Zeit entsteht aus Original-PTS × Zeitbasis minus bekanntem Containerstart, mit der bestehenden 100-ns-Zeitauflösung. Eine Startkante des Entfernbereichs liegt vor dem ersten zu entfernenden Bild; eine Endkante vor dem ersten anschließend behaltenen Bild. Es gibt keinen versteckten ±1-Frameversatz und keinen automatischen Keyframe-Sprung.
- `ApplyFrameEdge` prüft das ursprüngliche Segment und die Kantenseite, ändert nur die angefragte Grenze und erhält deren Auswahl. Ungültige Positionen, umgekehrte/leere Bereiche, Überschneidungen und veraltete Ergebnisse lassen den Schnittplan unverändert. Die Lupe meldet Übernahmefehler und bleibt geöffnet. Erfolgreiche Übernahme schließt sie; Abbrechen/Escape/Fensterschließen ändern nichts.
- Original-PTS, Zeitbasis, Videostream und alte/neue Kantenzeit werden im vorhandenen App-Protokoll festgehalten. Die Cutlist-Datei wird weiterhin bewusst über **Cutlist erzeugen** gespeichert; sie erhält die korrigierten Keep-Abschnitte aus dem aktuellen Schnittplan.
- Release-Build **11**: **0 Warnungen, 0 Fehler**; **577/577 Tests bestanden**. Neue Tests prüfen bestätigte Vorschauen, Ladezustand, Containerstart bei 0/+2/−2 s, variable Zeitabstände, Kantengeometrie, atomare Fehlerbehandlung, veraltete Auswahl und den Export korrigierter Keep-Grenzen.
- WPF-Button mit echter ffprobe-/FFmpeg-Vorschau ausgeführt: Ausgangskante 2,200 s → ein Frame vor → Übernahme bei 2,240 s, korrekte Endkante und Abbrechen unverändert. Reale Auswahl mit Containerstart +2 s und variablen Bildabständen bestätigt. Fenster bei normaler/kleiner Größe gerendert und geprüft; keine Bindungswarnungen.
- Die vorangegangenen Navy-CIS-Praxistests des Nutzers bestätigen ±1, Tabellenwahl von Keyframes und Bildwechsel an mehreren Schnittkanten. Der separate MP4Box-Versuch mit der Originaldatei bestätigt `-splitx`-Startverschiebungen und einen genauen Einzelstart mit `-splitf`; beim Zusammenfügen entstehen jedoch 107 statt 104 angeforderter Bilder, auch mit libmpv bestätigt. Der MP4Box-Schnittmotor wird deshalb in diesem Schritt nicht geändert.
- Der anschließende Praxistest mit korrigierter Cutlist und externem CutlistDude-/Tonvergleich ist im Tagesabschluss oben dokumentiert. Eine vollständige Schnittabnahme, dauerhafte rohe Frame-Metadaten im Schnittplan, absoluter Dateiindex und der unabhängige Abgleich zum sichtbaren Hauptplayerbild bleiben spätere Arbeit. CutlistDude und CL_OFFSET wurden nicht verändert.

## 03.10.2026 – Frame-Lupe, Schritt 2: Shift+F, Bildvorschau und Suchnavigation

- Shift+F öffnet ein eigenes Lupenfenster für die zuletzt angeklickte Start- oder Endkante. Laufende Wiedergabe wird pausiert. Eine reine Bereichsauswahl reicht nicht; bei mehreren Videostreams wird die noch fehlende Zuordnung zur aktiven mpv-Spur ausdrücklich gemeldet.
- Die Ausgangsnähe wird aus Schnittzeit plus bekanntem Containerstart gesucht. Die Bildvorschau läuft unabhängig vom Hauptplayer: FFmpeg behält die Quellzeitstempel, wählt den PTS des analysierten Frames und liefert ein PNG. Der vom Filter gemeldete PTS und dessen Zeitbasis müssen zum gewählten Frame passen. Fehlende Original-PTS, doppelte/nicht aufsteigende Zeiten und mehrdeutige Bildausgabe erhalten keine bestätigte Vorschau.
- Normale Navigation bleibt zusätzlich verfügbar: ±1/±10, Strg für ±20. Die Halbierungssuche verwendet eigene Buttons oder Umschalt+Links/Rechts. Sie beginnt standardmäßig mit 2000 Frames und halbiert nach einem erfolgreich angezeigten Suchsprung. Normale Schritte verbrauchen die Folge nicht; Reset stellt den Startwert wieder her. An Dateigrenzen wird begrenzt; ohne Bewegung bleibt die Suchweite erhalten.
- Reicht der Analyseabschnitt nicht, wird er erweitert, bis genügend tatsächlich decodierte Frames verfügbar sind oder die Dateigrenze erreicht ist. Die Schrittzahl wird nicht aus einer Bildrate berechnet. Fehler und Abbruch räumen die Vorschau auf; Schließen beendet laufende Analyse-/Vorschauprozesse.
- Einstellungen → Frame-Lupe speichert einen Startwert zwischen 1 und 100000 in der bestehenden Benutzereinstellungsstruktur. Ungültige/fehlende Einstellungen fallen auf 2000 zurück; Speicherfehler werden gemeldet.
- Optionale Framedetails zeigen rohe PTS, Best-Effort-Timestamp, Paket-DTS, Zeitbasis, Keyframe, Bildtyp und Abstand zur Ausgangskante. Die sichtbare Nummer gilt ausdrücklich nur für den geladenen Abschnitt. Kleine Fenster behalten scrollbare Details und erreichbare Navigationsbuttons.
- Rohe negative Quellzeiten sind für die ffprobe-Analyse jetzt erlaubt; die frühere Einschränkung auf nichtnegative Intervallstarts wurde entsprechend angepasst und durch einen passenden Test ersetzt.

### Nachweise

- Finaler Release-Build 10: **0 Warnungen, 0 Fehler**. Vollständige Testsuite: **556/556 bestanden**.
- Echte ffprobe-/FFmpeg-Vorschauen bei 25 und 50 fps, benachbarte I-/B-Bilder, variable Bildrate und Containerstart bei 2 Sekunden bestätigt.
- Gesamter ViewModel-/Werkzeugablauf mit einem erzeugten 160-Sekunden-Video bei 25 fps: Ausgang 100 Sekunden → 2000 Frames zurück auf 20 Sekunden → 1000 Frames vor auf 60 Sekunden; nächster Schritt 500. Beide Vorschaubilder wurden über PTS und Zeitbasis bestätigt.
- WPF-Fenster ohne Anzeige auf dem Desktop gerendert und visuell geprüft: normale Größe, offene Framedetails, kleine Fenstergröße und Einstellungsdialog. Keine WPF-Bindungsfehler. Initialisierung, normaler Schritt und begrenzter Suchsprung wurden im selben Ablauf ausgeführt.
- Noch kein Nutzer-Praxistest von Shift+F im Hauptfenster oder mit Navy CIS/Dumb Money.

### Verbleibender Umfang

Dieser Schritt verändert keine Schnittkante. Die exakte Rückübernahme, Frame-Metadaten an den Schnittkanten, ein absoluter Dateiindex und der Abgleich zur tatsächlichen mpv-Bild-/Zeitkoordinate bleiben nächste Arbeitsschritte. Die geprüfte Bild-/PTS-Zuordnung gilt für die unabhängige FFmpeg-Vorschau. Große Analyseerweiterungen können je nach Quelldatei dauern. CutlistDude/CL_OFFSET und die bestehenden Schnittmotoren bleiben unverändert.

## 03.10.2026 – Frame-Lupe, Schritt 1: Kantenauswahl und Frameanalyse

- Tabellenklicks auf Anfang/Ende speichern jetzt zusätzlich die Kantenseite im `CutPlanViewModel`. Wechsel der Bereichsauswahl, Ersetzen, Entfernen, Reset und Neuladen verwerfen das alte Kantenziel.
- `FfprobeFrameRunner` liest einen begrenzten Abschnitt eines ausdrücklich ausgewählten Videostreams mit `-show_frames`. Original-PTS, Best-Effort-Timestamp, optionaler Paket-DTS, Dauer, Keyframe-Kennzeichen und Bildtyp bleiben getrennt erhalten; Zeitbasis und verfügbare Stream-/Container-Startzeiten werden mitgeführt.
- `LocalIndex` zählt nur innerhalb des Probe-Ergebnisses. Es gibt noch keinen absoluten Datei-Frameindex und keine bestätigte Zuordnung zum sichtbaren mpv-Bild. Seek-Preroll wird nicht abgeschnitten; fehlende Daten werden nicht erfunden.
- `HalvingFrameSearch` stellt unabhängig von der normalen Navigation die Folge 2000, 1000, 500, 250, 125, 62, 31, 15, 7, 3, 1 bereit. Ein eigener Startwert und Reset sind im Modell möglich; die Einstellungen und Bedienung sind noch nicht angeschlossen. Nur ein erfolgreich abgeschlossener Suchsprung soll die Folge weiterführen.
- Die vorhandene Navigation ±1/±10 sowie die Strg-Schritte, Cutlist-Zeitwerte und Schnittmotoren wurden nicht geändert. Shift+F ist weiterhin für den nächsten Oberflächenschritt vorgesehen.

### Nachweise

- Release-Build 8: **0 Warnungen, 0 Fehler**. Vollständige Testsuite: **523/523 bestanden**, davon 27 neue Fälle für Kantenauswahl, Halbierungsfolge und Frameanalyse.
- Separater Smoke-Test mit erzeugten H.264-MP4s bei 25 und 50 fps: echter Aufruf des neuen Dienstes, absolute Streamauswahl (Video hinter Audio), PTS-Abstände 0,04/0,02 Sekunden sowie Keyframe-/B-Bild-Daten bestätigt.
- Anfrage 2,2–3,2 Sekunden: tatsächlich gelieferte Frames 2,0–3,12 Sekunden (25 fps) bzw. 2,0–3,16 Sekunden (50 fps). Der tatsächliche Analyseanfang wird nicht mit dem angefragten Anfang gleichgesetzt.
- Probe am Videoende: 25 Frames, bei zwei abschließend ausgegebenen Frames kein DTS; die Werte bleiben leer. Audioauswahl wird als ungültiger Videostream abgewiesen. Abbruch eines gestarteten ffprobe-Prozesses beendet und wartet den Prozess ab.
- Noch kein praktischer Lupentest mit Navy CIS oder Dumb Money; die Lupenoberfläche ist noch nicht implementiert.

### Nächster Schritt nach Rückmeldung

Shift+F an die gespeicherte Kante anbinden, Analysefenster in der Oberfläche zeigen und die Zuordnung zwischen Quell-PTS und mpv-Zeit/Bild verifizieren. Danach zusätzliche halbierte Suche mit einstellbarem Startwert, normale Fein-Navigation und ausdrückliche Übernahme der Kantengrenze ergänzen. Klassische Cutlists speichern weiterhin Zeitwerte; Frame-Metadaten im Schnittmodell und ein vollständiger Frameindex folgen gesondert.

## 01.10.2026 – Upload-Korrektur und Cutlist-Einstellungen

Veröffentlichter Stand: **CAN 0.2.1 · Build 7 · RC2**. Die vorherige Ausgabe **0.2.0 Build 6 – RC2** bleibt unverändert verfügbar.

### Änderungen

- Beim Upload bleiben Autor, Programmname und Version der gespeicherten Cutlist erhalten. Die vorher fest eingetragenen Werte überschreiben diese Angaben nicht mehr.
- Die getrennten HTTP-Formularfelder verwenden `app=CutAssistantNext` und die numerische Version der laufenden CAN-Anwendung, unabhängig von einer geladenen Fremd-Cutlist.
- RC2 ist auch für normale lokale Builds hinterlegt. Die Patch-Version wurde auf 0.2.1 erhöht und Build 7 regulär erstellt.
- Ausführliche URL-Hilfe erscheint erst nach einem fehlgeschlagenen Verbindungstest. Ein erfolgreicher erneuter Test lässt sie ausgeblendet.
- Der Einstellungsdialog ist höher und wird am oberen Rand des aktuellen Monitor-Arbeitsbereichs platziert. Kleine Bildschirme behalten die Scrollmöglichkeit.
- Die zusätzliche HTTPS-Adresse von Sniplist wird mit gültigem FRED akzeptiert, ohne sie im Dialog hervorzuheben. Details stehen in ADR-009.
- Nutzeranleitung und Architekturentscheidungen wurden ergänzt. Die Windows-x64-Lockdateien berücksichtigen Version 0.2.1; externe Paketversionen bleiben unverändert.

### Nachweise

- Letzter Release-Build: **0 Warnungen, 0 Fehler**; vollständige Testsuite: **496/496 bestanden**.
- Die Windows-x64-Paketwiederherstellung mit `--locked-mode` ist erfolgreich.
- Echte Testuploads bestätigten den Erhalt der Cutlist-Metadaten sowie die Annahme von `app=CutAssistantNext` (ID 2079437) und HTTP-`version=0.2.0` (ID 2079440). Die HTTP-Felder sind nicht separat in der heruntergeladenen Cutlist sichtbar; die Zuordnung beruht auf der jeweils getesteten Anwendung. Weitere Details in ADR-010.
- Der Benutzer hat die Löschung sämtlicher Test-Cutlists bestätigt.
- Der spätere Upload aus CAN 0.2.1 wurde mit Server-ID 2079448 bestätigt (`GeneratedOn=2026-10-01 20:40:32`). Die Serverfassung erhält Programmname, Version, Autor, Schnittbereiche, Namensvorschlag und Kommentar der lokalen Cutlist. Damit ist auch der Upload-Ablauf mit HTTP-Version 0.2.1 praktisch bestätigt; die HTTP-Felder selbst werden nicht separat in der Cutlist angezeigt.
- Der Benutzer hat die vergrößerte und nach oben verschobene Fensterdarstellung bestätigt.
- Nach Veröffentlichung: Installation in einer VM mit GPAC, .NET 10 und FFmpeg bestätigt. „Grey’s Anatomy – Durchs Feuer“ (HQ-MP4) wurde in drei Behaltebereichen geschnitten und vollständig zusammengefügt; das Protokoll endet mit „Fertig.“. Die Cutlist stimmt mit den Schnittaufrufen überein und enthält CAN 0.2.1 sowie Autor Joerg. Bild und Ton der fertigen Datei wurden vom Benutzer als einwandfrei bestätigt.

- Die anschließende Deinstallation der Setup-Version in der VM wurde vom Benutzer als erfolgreich bestätigt.

- Der separate Portable-ZIP-Schnitt mit drei Behaltebereichen wurde ebenfalls erfolgreich abgeschlossen; Bild und Ton der fertigen Datei wurden vom Benutzer als einwandfrei bestätigt. Das neue Protokoll verwendet andere temporäre Dateien als der Setup-Test und zeigt drei abgeschlossene Segment-Schnitte, vollständiges Zusammenfügen und einen erfolgreichen Abschluss.

### Zusätzlicher Server-Praxistest

- Mit konfigurierter Sniplist-HTTPS-URL wurden Verbindungstest, Suche (zwei Treffer) und Download mit geladenen Schnittbereichen bestätigt.
- Der anschließende Upload war erfolgreich; der Dialog zeigt Server-ID 2079449. Ein separater Inhaltsabgleich dieser Serverfassung ist nicht dokumentiert.
- Die vorgesehenen Funktionsprüfungen für Setup, Portable-ZIP und den zusätzlichen Serverablauf sind damit abgeschlossen. Dies ist keine allgemeine Garantie für alle Dateien oder Serverzustände.

### Veröffentlichung um 20:03 Uhr MESZ

- Änderungen mit Commit `dcf4123` gesichert und nach GitHub gepusht.
- Setup und Portable-ZIP aus dem frisch entpackten Quellarchiv dieses Commits gebaut: 496/496 Tests, keine Buildwarnungen oder Fehler.
- 148 Portable-Dateien geprüft; Start mit korrekter Versionsanzeige und reguläres Beenden erfolgreich.
- Alle sechs Uploads anhand von Größe und GitHub-SHA-256-Digest mit den lokalen Dateien abgeglichen.
- Nach Nutzerauftrag als Vorabversion [v0.2.1-build7-rc2](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.1-build7-rc2) veröffentlicht. Tag-Zuordnung und öffentlicher Status bestätigt. Prüfbericht und Veröffentlichungsnachweis wurden nach dem Paketbau ergänzt.
