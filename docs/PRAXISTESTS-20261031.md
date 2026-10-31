# Aktuelle persönliche Praxistests – 31.10.2026

Nach den Setup-/Portable-Prüfungen kündigt der Nutzer weitere aktuelle Praxistests an. Diese Fälle werden getrennt von den [Build-47-Setup-Fällen](VM-SETUP-BUILD47-20261031.md) und der [Portable-Runde](VM-PORTABLE-BUILD47-20261031.md) geführt. Die jeweilige Paket-/Rechnerzuordnung folgt den ausdrücklichen Nutzerangaben.

## Fall 1: The Rocky Horror Picture Show, arte HD, 50 fps

**Vom Nutzer nachträglich präzisiert: CAN 1.0.2 Build 46, Setup auf dem Arbeitsrechner.** Dieser Fall gehört damit weder zur Build-47-VM-Runde noch zur Portable-Prüfung.

Referenz: **The_Rocky_Horror_Picture_Show_26.10.30_20-15_arte_95_TVOON_DE.HD.mp4.cutlist**, vom Nutzer aus seinem Filmordner benannt und lesend geprüft. Die reale Videodatei wurde nicht geöffnet oder in Git aufgenommen; die Cutlist wird nicht verändert.

Die Cutlist nennt **Cut Assistant Next 1.0.2**, **50 Bilder/s**, 16:9 und **otr-can 0.1.6** (`otr_can.exe`) als vorgesehenen Schnittmotor. Originaldateigröße laut Cutlist **2944425614 Bytes**. Ein Behaltebereich (`NoOfCuts=1`):

| Planangabe | Wert |
|---|---|
| Anfang | 374,300 s = **00:06:14.300** |
| Dauer | 5720,660 s = **01:35:20.660** |
| Ende, aus Anfang + Dauer berechnet | 6094,960 s = **01:41:34.960** |
| Geplante Bildanzahl bei 50 fps | **286033 Bilder** |

Die Bildanzahl ist aus den Cutlist-Werten berechnet, keine Messung der tatsächlichen Ausgabedatei. Mit nur einem Behaltebereich ist kein innerer Übergang zwischen mehreren Behaltebereichen belegt.

Cutlist-Kommentar: **„Mit Cut Assistant Next geschnitten. Keine Werbung gefunden. Mit otr-can framegenau geschnitten. Blitzer vom arte-Vorspann nicht mehr sichtbar.“** Dieser Vermerk wird als Inhalt der vom Nutzer benannten Cutlist festgehalten, nicht als eigene Bildanalyse.

Der Nutzer bestätigt ausdrücklich **„Ton Bild schnitt 3-sekundenvorschau check“**. Damit sind **Ton, Bild, Schnitt und Drei-Sekunden-Vorschau persönlich bestanden**.

Die Datei selbst enthält keine Buildnummer oder Installationsvariante; die Zuordnung **Build 46 / Setup / Arbeitsrechner** stammt aus der nachfolgenden Nutzerbestätigung. Ein separates Motor-Erfolgsprotokoll, eine gemessene Ausgabebildanzahl und Details der Tonspuren liegen für diesen Fall nicht vor. Der vorgesehene Motor ist anhand der Cutlist zugeordnet; daraus wird keine vollständige Paket-Endabnahme abgeleitet.

## Fall 2: Doc S02E18 – Orientierung, 5plus HQ, 25 fps

**CAN 1.0.2 Build 46, Setup auf dem Arbeitsrechner**, gemäß der unmittelbar zuvor ausdrücklich genannten Umgebung fortgeführt. Referenz: **Doc_S02E18_26.10.30_20-15_5plus_50_TVOON_DE.HQ.mp4.cutlist**. Die Cutlist wurde nur gelesen; Videodatei und Cutlist bleiben unverändert.

Die Datei nennt CAN 1.0.2, **25 fps**, 16:9 und **MP4Box 26.07** als vorgesehenen Schnittmotor. Originaldateigröße laut Cutlist **664161519 Bytes**, zwei Behaltebereiche (`NoOfCuts=2`):

| Bereich | Anfang | Dauer | Berechnetes Ende |
|---|---:|---:|---:|
| 1 | 584,200 s | 1341,040 s | 1925,240 s |
| 2 | 2618,560 s | 1150,160 s | 3768,720 s |

Geplante Gesamtdauer **2491,200 s = 00:41:31.200**, entsprechend **62280 Bildern** bei 25 fps. Der innere Übergang liegt rechnerisch bei **00:22:21.040** im geschnittenen Film. Dies sind Cutlist-Planwerte, keine gemessenen MP4Box-Ausgabezeiten oder tatsächliche Ausgabebildzählung.

Der Cutlist-Kommentar vermerkt mit CAN und MP4Box 26.07 ausgeführten Schnitt sowie vollständig entfernte Werbung. Vorgeschlagener Filmname: **Doc S02E18 Orientierung [30.10.2026]**.

Der Nutzer bestätigt **„bild ton schnitt 3-sekunden vorschau check“**. **Bild, Ton, Schnitt und Drei-Sekunden-Vorschau persönlich bestanden.** Ein separates Motor-/Schnittstellenprotokoll oder Tonspurdetails wurden für diesen Fall nicht übermittelt. Die Build-/Setup-/Rechnerzuordnung folgt der laufenden Praxistestrunde, nicht den Cutlist-Metadaten; keine neue VM-/Portable-Endabnahme.

## Fall 3: Brilliant Minds S01E11 – Die andere Frau, 5plus HQ, 25 fps

**CAN 1.0.2 Build 46, Setup auf dem Arbeitsrechner**, gemäß der laufenden, vom Nutzer zuletzt genannten Praxisumgebung fortgeführt. Referenz: **Brilliant_Minds_26.10.30_21-05_5plus_60_TVOON_DE.HQ.mp4.cutlist**, dazu ein übermitteltes MP4Box-Schnittprotokoll. Beide Quellen nur gelesen; Cutlist und Videodatei unverändert.

Cutlist: CAN 1.0.2, **25 fps**, 16:9, vorgesehener Motor **MP4Box 26.07**, Originaldateigröße **728351743 Bytes**. Vorgeschlagener Filmname **Brilliant Minds S01E11 Die andere Frau [30.10.2026]**. Drei Behaltebereiche:

| Bereich | Anfang | Dauer | Berechnetes Ende |
|---|---:|---:|---:|
| 1 | 770,080 s | 867,320 s | 1637,400 s |
| 2 | 2366,600 s | 1189,240 s | 3555,840 s |
| 3 | 4296,160 s | 345,120 s | 4641,280 s |

Geplante Gesamtdauer **2401,680 s = 00:40:01.680**, entsprechend **60042 Bildern** bei 25 fps. Rechnerische innere Übergänge bei **00:14:27.320** und **00:34:16.560** im geschnittenen Film. Dies sind Cutlist-Planwerte, keine neu gemessenen tatsächlichen MP4Box-Ausgabezeiten oder Ausgabebildzahlen.

Das übermittelte Protokoll belegt drei MP4Box-`-splitx`-Aufrufe für dieselbe Quelldatei, anschließendes `-cat` aller drei Segmente und das Schreiben der MP4. Die Schnittstellenprüfung durchläuft Abschnitt 1 bis 3 und endet mit **„Fertig.“**. An **Übergang 1** meldet CAN gleiche Bildzeit am Abschnittswechsel; der dekodierte neue Anfang wird zusätzlich geprüft. Dieser Prüfhinweis wird erhalten und ist im vorliegenden Lauf kein Abbruch.

Der Nutzer bestätigt **„bold ton schnitt 3-sekunden top“**, im Kontext als **Bild, Ton, Schnitt und Drei-Sekunden-Vorschau persönlich bestanden** verstanden. Kein neuer Agenten-Schnitt, kein eigener Decoderlauf und keine vollständige Paket-Endabnahme daraus abgeleitet. Zusammen mit Rocky Horror und Doc sind jetzt drei aktuelle Praxisfälle unter der genannten Arbeits-PC-Setup-Umgebung dokumentiert.

## Erhaltung

Nur Arbeitsdokumentation ergänzt. Cutlist, Videodateien, Produktcode, Buildnummer 47 sowie bestehende schreibgeschützte Pakete und Prüfsummen unverändert.

[Testplan](TESTPLAN.md), [Projektstand](AKTUELLER-STAND.md), [Tagebuch](TAGEBUCH.md).
