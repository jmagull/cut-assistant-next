# Vera – Messversuch zur MP4Box-Schnittprüfung

Stand: **18.10.2026**, CAN **1.0.0 · Build 32**. Reiner Messversuch nach Freigabe des Nutzers; keine Produktkorrektur, kein Build und keine Veröffentlichung.

## Ergebnis

**Alle drei MP4Box-Übergänge liegen 4,080 Sekunden vor den von CAN berechneten Stellen.** Das erste Segment beginnt tatsächlich erst mit Originalbild **574,040 s** statt **569,960 s**. Die **102 gewünschten Bilder von 569,960 bis einschließlich 574,000 s fehlen**. Das ist durch vollständige Zuordnung der komprimierten Videopakete zum Original abgesichert, nicht nur aus einer Dauer abgeleitet.

Die unabhängig neu erzeugte MP4Box-Ausgabe enthält **132320 Videopakete / 5292,800 s**. Alle Videopaketinhalte sowie PTS und DTS stimmen mit der vorhandenen Nutzerdatei `Vera - Ein ganz spezieller Fall S12E05 Die steigende Flut [17.10.2026] mp4box.mp4` überein. Keine Bytegleichheit der gesamten MP4-Dateien behauptet: Container-Metadaten, insbesondere Erstellungszeiten, unterscheiden sich.

Die vorhandene otr-can-Ausgabe hat **132422 Bilder / 5296,880 s**. Die ersten sechs Bilder jedes nachfolgenden Keep-Abschnitts wurden als dekodierte YUV-Bilder anhand ihrer SHA-256-Werte auf der jeweiligen Planposition bestätigt. Die drei otr-can-Übergänge stimmen damit im untersuchten Film mit dem Plan überein. Kein erneuter otr-can-Schnitt ausgeführt.

**Eine Addition der gemeldeten Segmentdateidauern würde die tatsächlichen MP4Box-Übergänge ebenfalls nicht exakt treffen.** Sie liegt hier um 40, 80 und 120 ms zu früh. Maßgeblich sind die Präsentationszeiten im fertigen Film. An den ersten beiden Übergängen existieren zudem doppelte Video-PTS.

## Eingaben, Werkzeuge und Durchführung

Original: `Vera_-_Ein_ganz_spezieller_Fall__Die_steigende_Flut_S12E5_26.10.17_20-15_sat1gold_115_TVOON_DE.HQ.mp4`, **1014323513 Bytes**, H.264, 720×576, 25 fps, Stereo-AAC 48 kHz.

SHA-256 des Originals: `C6FF1A39219AEBB4DE7C130845BB7D0766A536CDAA1C8B38E8DC797BF4D16E92`.

Lokale MP4-Cutlist mit vier Keeps gelesen und vor dem Schnitt gegen die beauftragten Werte geprüft. Die Halbintervalle des Plans sind `[Start, Start + Dauer)`.

Werkzeuge:

- MP4Box **26.07-rev0-ga07cbfff-master** aus der konfigurierten Installation.
- ffprobe/FFmpeg **2026-10-12-git-615b07170e-essentials_build-www.gyan.dev**.
- Dieselben MP4Box-Medienargumente wie CAN: Einzelaufrufe mit `-splitx`, danach ein Aufruf mit vier `-cat`-Argumenten. Zusätzlich `-p 0` für ein nicht gespeichertes GPAC-Profil. Die Originaldatei und vorhandene Ausgaben wurden ausschließlich gelesen.

Alle vier neuen Segmente und der neue Gesamtfilm liegen getrennt unter `.build/vera-transition-measurement-20261018`; dieses Verzeichnis ist durch die bestehende Git-Regel ausgeschlossen. Vorhandene Dateien wurden nicht überschrieben. Vollständige Paketdaten des Originals, der Segmente und der Neuausgabe wurden mit SHA-256 erfasst. Jedes Segment entspricht einer zusammenhängenden Original-Paketfolge in Dekodierreihenfolge. Die neue Ausgabe enthält exakt die Verkettung dieser vier Paketfolgen.

## Einzelne Segmente

| Segment | Keep-Start | Keep-Dauer | MP4Box-Anforderung | Gemeldete Datei-/Videodauer | Videopakete |
|---|---:|---:|---|---:|---:|
| 1 | 569,960 | 1024,800 | `569.96:1594.72` | 1020,680 | 25518 |
| 2 | 2138,800 | 1100,120 | `2138.8:3238.88` | 1100,080 | 27503 |
| 3 | 3799,960 | 1793,040 | `3799.96:5592.96` | 1793,000 | 44826 |
| 4 | 6136,040 | 1378,920 | `6136.04:7514.92` | 1378,880 | 34473 |

Alle Zeiten in Sekunden. Die MP4Box-Endanforderung ist entsprechend dem bestehenden CAN-Code um eine Framedauer gegenüber dem exklusiven Keep-Ende vermindert. Der Versuch verändert diese Behandlung nicht.

Die gemeldeten Einzeldauern summieren sich auf **5292,640 s**, während der fertige Film **5292,800 s** meldet. Auch die PTS-Spanne der enthaltenen Videopakete ist nicht automatisch gleich der gemeldeten Segmentdauer. Das erste Segment hat beispielsweise Paket-PTS von 0 bis 1020,720 s, obwohl seine gemeldete Dauer 1020,680 s ist.

## Verifizierte Übergänge und Prüfstarts

Die Spalte „MP4Box“ bezeichnet den PTS des ersten dekodierten Bildes des folgenden Keep-Abschnitts. Seine Herkunft wurde über die Paketposition zum Original und zusätzlich durch SHA-256 des dekodierten Bildinhalts bestätigt.

| Übergang | CAN-Plan / otr-can bestätigt | MP4Box bestätigt | Aktueller CAN-Prüfstart | Prüfstart 3 s vor MP4Box-Übergang |
|---|---|---|---|---|
| 1 → 2 | 00:17:04,800 | **00:17:00,720** | 00:17:01,800 | **00:16:57,720** |
| 2 → 3 | 00:35:24,920 | **00:35:20,840** | 00:35:21,920 | **00:35:17,840** |
| 3 → 4 | 01:05:17,960 | **01:05:13,880** | 01:05:14,960 | **01:05:10,880** |

Die heutigen CAN-Prüfstarts liegen somit jeweils **1,080 s nach** dem ersten Bild des nächsten Abschnitts. Der Drei-Sekunden-Abzug funktioniert; seine Bezugsposition ist falsch.

| Übergang | Summe vorheriger Segmentdateidauern | Bestätigter PTS | Fehler der Dauersumme |
|---|---:|---:|---:|
| 1 → 2 | 1020,680 | 1020,720 | −0,040 |
| 2 → 3 | 2120,760 | 2120,840 | −0,080 |
| 3 → 4 | 3913,760 | 3913,880 | −0,120 |

Die im vorherigen Diagnosegespräch genannten **4,120 s** waren die Differenz zwischen erster Keep-Dauer und gemeldeter erster Segmentdateidauer. Sie waren ausdrücklich an die Annahme gebunden, dass diese Dateidauer auch die Übergangsposition bestimmt. Der Messversuch widerlegt diese Annahme: Für den Übergang in der fertigen Datei beträgt die Abweichung **4,080 s**.

## Zusätzlicher Befund: Schnittenden und doppelte PTS

Neben den 102 fehlenden Anfangsbildern zeigt die vollständige Originalzuordnung folgende Abweichungen gegenüber den Keep-Halbintervallen:

| Segment | Fehlende gewünschte Originalbilder | Zusätzlich enthaltene Originalbilder |
|---|---|---|
| 1 | 102 Anfangsbilder sowie PTS 1594,720 | PTS 1594,760 |
| 2 | PTS 3238,880 | PTS 3238,920 |
| 3 | keine | keine |
| 4 | PTS 7514,920 | PTS 7514,960 |

Das sind insgesamt **105 fehlende und 3 zusätzlich enthaltene Originalpakete**, netto 102 weniger. Eine gleiche Bildanzahl beweist somit keine identischen Schnittgrenzen. Die 102 Anfangsbilder sind als eigener, zusammenhängender Verlust bestätigt; der Befund an den Enden muss davon getrennt bleiben.

Am ersten Übergang haben das letzte dekodierte Bild aus Segment 1 (Original-PTS 1594,760) und das erste aus Segment 2 (Original-PTS 2138,800) beide Ausgabe-PTS **1020,720**. Am zweiten Übergang haben das letzte Bild aus Segment 2 (Original-PTS 3238,920) und das erste aus Segment 3 (Original-PTS 3799,960) beide Ausgabe-PTS **2120,840**. Am dritten Übergang ist die Abfolge regulär: 3913,840 → 3913,880.

Die neuen Segmentanfänge selbst wurden jeweils durch identische dekodierte Bildhashes bestätigt. Die zusätzlichen Endbilder der ersten beiden Segmente erscheinen auch in der lokalen Dekoderfolge. **Keine persönliche Werbe-/Filmszenenbewertung und keine Aussage über die konkrete Behandlung gleicher PTS in jedem Player.** Paketpräsenz allein beweist insbesondere am Dateiende keine sichtbare Wiedergabe außerhalb der Containerdauer. Eine allgemeine Reparatur dieser Zeitstempel oder Schnittenden gehört nicht zur Korrektur der Prüfpositionen.

## otr-can und vorhandene Varianten

Für die vorhandene `...S12E05...otr-can.mp4` wurden an allen drei Planübergängen jeweils sechs aufeinanderfolgende dekodierte Bilder mit den Originalbildern am zugehörigen Keep-Anfang verglichen. Alle **18 Bildhashes** stimmen exakt und an der erwarteten Ausgabe-Framenummer überein:

- Übergang 1: ab Ausgabeframe 25620 / 1024,800 s, Original ab 2138,800 s.
- Übergang 2: ab Ausgabeframe 53123 / 2124,920 s, Original ab 3799,960 s.
- Übergang 3: ab Ausgabeframe 97949 / 3917,960 s, Original ab 6136,040 s.

Diese drei Keep-Anfänge sind im konkreten Fall bereits geeignete kopierte Kanten. Daraus wird keine allgemeine Bytegleichheit an von otr-can neu kodierten Kanten abgeleitet. Keine vollständige Qualitäts-/Tonprüfung und kein neuer otr-can-Motorlauf.

Zwei weitere vorhandene Dateien mit dem älteren Namensschema `...12-05...` haben **5292,960 s / 132324 Videoframes**. Sie sind eine andere Ausgabevariante als die hier reproduzierte Datei mit 5292,800 s. Für sie wurden nur Metadaten erfasst; die Herkunft ihrer abweichenden Enden wurde nicht untersucht. Diese Varianten werden nicht miteinander vermischt.

## Konsequenz für die Umsetzung

Der vorgeschlagene gemeinsame Ergebnisvertrag bleibt sinnvoll: geplante Grenzen, tatsächliche Ausgabeübergänge, Herkunft/Verifikationsstatus und unabhängige Hinweise auf Inhaltsabweichungen. Das Abschlussmodell sollte erst nach dem Schnitt daraus entstehen. Die Cutlist und die Schnittanforderungen bleiben unangetastet.

Für MP4Box sollten vor dem Löschen der temporären Dateien Segmentzuordnung und Paketanzahlen erfasst werden. Nach `-cat` muss CAN die zugehörigen Präsentationszeiten in der endgültigen Videozeitachse ermitteln. Eine unabhängige Zuordnung über Paketfolgen an den Grenzen kann dies absichern. **Weder `format.duration` aufsummieren noch pauschal ein Bild je Segment oder 4,080 Sekunden abziehen.** Auch Paketanzahl geteilt durch nominelle Bildrate ersetzt bei allgemeinen Zeitstempelproblemen keine Verifikation. Im vorliegenden 25-fps-Fall trifft diese Rechnung die gemessenen Übergänge, ist aber kein allgemeiner Beweis.

Für otr-can kann der vorhandene Planpfad erhalten bleiben; der konkrete Vera-Vergleich bestätigt ihn. Ein gemeinsamer Ergebnisvertrag muss berechnete und unabhängig verifizierte Werte unterscheiden. Fehlende Analyse darf nicht als erfolgreiche Übergangsverifikation erscheinen. Ein bereits gelungener Schnitt muss bei einer gescheiterten Zusatzanalyse erhalten bleiben.

Die Prüfanzeige kann mit den ermittelten Ausgabezeiten richtig positioniert werden. **Fehlende Filmbilder werden dadurch nicht wiederhergestellt.** Eine separate, belastbare Warnung sollte tatsächlichen Verlust beziehungsweise zusätzlich übernommene Bilder beschreiben. otr-can bleibt für geeignete Aufnahmen eine bewusst wählbare Alternative; MP4Box bleibt der schnelle Standard.

## Prüfungen, Einschränkungen und Erhaltung

- Vier MP4Box-Splits und ein `-cat` mit Exit-Code 0.
- Alle **132320** Videopakete der Segmente vollständig zum Original und anschließend zur fertigen Datei zugeordnet.
- Identische Videopaketinhalte, PTS und DTS gegenüber der passenden vorhandenen MP4Box-Ausgabe.
- Drei Übergänge dekodiert und über Paketpositionen zugeordnet; vier Abschnittsanfänge zusätzlich durch identische rohe Bildhashes bestätigt.
- 18 dekodierte Bildhashes der vorhandenen otr-can-Ausgabe an den drei Planübergängen bestätigt.
- **587 vorbestehende Dateien** per SHA-256 vor/nach verglichen: alle zuvor versionierten Dateien, Buildzähler, acht lokale Lockdateien einschließlich der vier vorbestehenden Änderungen, lokale Release-Dateien, persönliche CAN-Einstellungsdateien sowie Vera-Original, Cutlist und vorhandene Ausgaben erhalten. HEAD und lokaler Tag `v1.0.0` unverändert. Keine Remote-Änderung ausgeführt.
- Einziger neuer versionierbarer Projektinhalt: dieser Bericht. Keine Änderung an Anwendungscode, Version, Buildzähler 32 oder Release-Dateien; kein Restore, CAN-Build, neuer Lösungstestlauf, Commit, Push oder Release. Der bisherige Paketnachweis von 1347/1347 Tests wird nicht als neuer Testlauf ausgegeben.

GPAC meldete auch mit `-p 0` einen verweigerten Schreibtest für seine persönliche `gpaccfgtest.txt`. Die Meldungen bleiben vollständig in den Protokollen; alle Medienaufrufe waren erfolgreich und ihre Videopakete wurden unabhängig verifiziert. Keine Lockerung von Zugriffsrechten und keine Änderung persönlicher CAN-Einstellungen.

Ein erster lokaler Dekodierhelfer-Aufruf kombinierte erhaltene absolute Zeitstempel mit einer ungeeigneten Ausgabe-Dauerbegrenzung und lieferte eine leere Bildhashliste. Die Begrenzung wurde auf eine explizite Bildanzahl korrigiert; sämtliche oben berichteten Bildprüfungen stammen aus dem erfolgreichen Folgelauf. Dieser Diagnosehelfer ist kein Produktcode.

Die Stichproben ersetzen keine vollständige Filmdekodierung, persönliche Sichtprüfung, Ton-/Synchronitätsprüfung oder neue CAN-/WPF-Abnahme. Die gemessenen Video-PTS belegen die technische Zuordnung; die Behandlung doppelter PTS im konkreten Kontrollplayer wurde hier nicht zusätzlich gemessen.

## Lokale Nachweise

`.build/vera-transition-measurement-20261018` enthält:

- `measure.py`, `verify.py`, `finalize.py`: Diagnosehelfer und reproduzierbare Prüfbedingungen.
- `PRESERVED-BEFORE.json`, `GIT-BEFORE.json`, `FINAL.json`: Hash- und Git-Erhaltungsnachweis.
- `MEASUREMENT.json`, `SOURCE-COVERAGE.json`, `TRANSITION-COMPARISON.json`: Segmentdaten, vollständige Fehl-/Zusatzbildlisten und Dauersummenvergleich.
- `VERIFICATION.json`, `OTR-DECODED-VERIFIED.json`: dekodierte Übergänge und Bildprüfsummen.
- `source-packets.stdout`, `segment-*-packets.stdout`, `final-packets.stdout`, `existing-mp4box-packets.stdout`: vollständige Videopaketdaten samt Hashes.
- `*-streams.json`, `*-decoded.stdout`, `*-framehash.stdout`: Medieninformationen, lokale Dekodierung und rohe Bildhashes.
- `*.command.json`, `*.stderr`, `DIAGNOSTIC-NOTES.json`: Argumente, Laufzeiten, Exit-Codes und transparente Diagnosehinweise.
- `segment-1.mp4` bis `segment-4.mp4`, `vera-mp4box-measurement.mp4`: separat erzeugte Messausgaben; keine Originale und keine Dateien für Git.

Der komplette Originalpfad und persönliche lokale Pfade verbleiben in den ignorierten Diagnosedaten. Keine Zugangsdaten erhoben.
