# VM-Setup Build 40 – laufende persönliche Prüfrunde

**Stand 27.10.2026.** Der Nutzer beginnt nach dem Arbeits-PC-Test die VM-Setup-Prüfung und bestätigt den ersten Fall: **„setup Test:ton schnitt bild 3-sekunden vorschau check, teste weiter“**. Die Prüfrunde läuft weiter; keine pauschale VM-, Portable- oder Deinstallationsabnahme.

## Fall 1: Die Teufelsbrigade, klassische AVI mit MP4Box

Originaldatei: **Die_Teufelsbrigade_21.01.21_20-15_tele5_160_TVOON_DE.mpg.HQ.avi**.

Das Protokoll zeigt H.264 High, 720×576, progressiv, 25 fps und Stereo-MP3 mit 48000 Hz. Video und MP3 werden ohne Neukodierung in eine temporäre MP4-Arbeitsdatei kopiert. FFmpeg meldet 263694 übernommene Bilder; anschließend bestätigt CAN die vollständige exakte Videopaketanzahl sowie Stream-, Bildraten- und Laufzeitprüfung.

MP4Box schneidet sechs Abschnitte und fügt sie zusammen. Die abschließende Schnittstellenprüfung durchläuft alle sechs Abschnitte; das Protokoll endet mit **„Fertig.“**. Dies ist ein MP4Box-Fall; ein neuer otr-can-VM-Test ist darin nicht enthalten.

Persönlich bestätigt: **Ton, Schnitt, Bild und Drei-Sekunden-Vorschau**.

## Fall 2: Die Teufelsbrigade, klassische AVI mit otr-can 0.1.6

Gleiche Originaldatei wie Fall 1. Video und MP3 zunächst ohne Neukodierung in eine neue temporäre MP4 übernommen; Paket-/Stream-/Bildraten-/Laufzeitprüfung bestanden. FFMS2 indexiert **263694 Frames**, deren Zeiten als eindeutig, aufsteigend und zum 25-/50-fps-Raster passend bestätigt werden.

**otr-can 0.1.6** verarbeitet sechs Behalteabschnitte mit Grenzkodierung und IDR-geprüfter Videokopie. Das Protokoll bestätigt **182574 Videopakete samt Roh-PTS/DTS**. Der ursprüngliche Stereo-MP3-Ton wird nach PCM-Schnitt einmal nach **AAC-LC, 192000 bit/s, 48000 Hz** kodiert. **350542080 dekodierte Tonsamples** und durchgehende Roh-PTS sind verifiziert; abschließende MP4-Prüfung von Video-/Audiopaketen, Zeiten, Padding und Decoderkonfigurationen erfolgreich. CAN meldet **„OTR-CAN-Schnitt abgeschlossen.“** Keine zusätzliche vollständige Videodekodierung aus der allgemeinen optionalen Logzeile abgeleitet.

Der Nutzer bestätigt **„ton bild schnitt 3-sekunden vorschau check“** und anschließend **„test weiter“**. Damit sind beide Motorwege dieser AVI in der VM persönlich positiv; weitere VM-Prüfung läuft. Die fehlenden ursprünglichen AVI-PTS werden beim Vorbereiten erneut protokolliert und bleiben im Nachweis erhalten. Kein AAC-Notfallweg erforderlich oder behauptet.

## Fall 3: Grey’s Anatomy – Durchs Feuer, MP4 mit MP4Box

Originaldatei: **Greys_Anatomy_-_Die_jungen_Aerzte__Durchs_Feuer_26.09.30_20-15_sixx_55_TVOON_DE.HQ.mp4**.

MP4Box schneidet die MP4 direkt in drei Abschnitte und fügt alle drei zusammen. Die anschließende Schnittstellenprüfung durchläuft Abschnitt 1 bis 3; Abschluss **„Fertig.“**. Keine vorgeschaltete AVI-Vorbereitung in diesem Fall.

Der Nutzer bestätigt **„ton bild schnitt 3-sekunden vorschau check test weiter“**. Ton, Bild, Schnitt und Drei-Sekunden-Vorschau persönlich positiv; weitere VM-Prüfung läuft. Kein otr-can-Fall oder vollständige VM-/Portable-/Deinstallationsabnahme daraus abgeleitet.

## Fall 4: Grey’s Anatomy – Durchs Feuer, MP4 mit otr-can 0.1.6

Gleiche Original-MP4 wie Fall 3: **Greys_Anatomy_-_Die_jungen_Aerzte__Durchs_Feuer_26.09.30_20-15_sixx_55_TVOON_DE.HQ.mp4**.

FFMS2 indexiert **119927 Quellframes**; CAN bestätigt eindeutige, aufsteigende Bildzeiten und das 25-/50-fps-Raster. otr-can 0.1.6 schneidet drei Behalteabschnitte und verifiziert **60651 Videopakete samt Roh-PTS/DTS**. Audio: Stereo, 48000 Hz, Ziel-AAC 165457 bit/s. **116449920 dekodierte Tonsamples** und durchgehende Roh-PTS sind verifiziert. Der abschließende MP4-Zusammenbau besteht die Paket-/Zeit-/Padding-/Decoderkonfigurationsprüfung; CAN meldet **„OTR-CAN-Schnitt abgeschlossen.“** Keine zusätzliche vollständige Videodekodierung aus der optionalen Logzeile abgeleitet.

Der Nutzer bestätigt **„Ton schnitt, bild 3-sekunden vorschau check test weiter“**. Damit sind klassische AVI und MP4 in dieser VM-Setuprunde jeweils mit MP4Box und otr-can persönlich positiv geprüft. Die gesamte VM-Prüfung bleibt laufend; keine neue Portable-/Deinstallationsabnahme daraus abgeleitet.

## Fall 5: Furiosa Teil 1, HD-MP4 mit MP4Box

Originaldatei: **Furiosa__A_Mad_Max_Saga__Teil_1_2__26.10.18_20-15_4plus_150_TVOON_DE.HD.mp4**.

MP4Box schneidet fünf Abschnitte und fügt sie zusammen. Die Schnittstellenprüfung durchläuft alle fünf Abschnitte. Am ersten inneren Übergang meldet CAN ausdrücklich **„geringfügig überlappende Bildzeiten; Paketfolge und dekodierter Abschnittswechsel werden zugeordnet“**. Der Lauf endet mit **„Fertig.“**.

Der Nutzer bestätigt **„bild ton schnitt 3-sekunden vorschau check, teste weiter“**. Damit ist auch der frühere Furiosa-MP4Box-Problemfall in dieser VM-Runde für Bild/Ton/Schnitt/Drei-Sekunden-Vorschau positiv gemeldet. Keine eigenen gemessenen Übergangszeiten oder Videopaketanzahlen im vorliegenden Log ergänzt. Die begrenzte Übergangszuordnung stellt keine von MP4Box ausgelassenen Bilder wieder her und ist kein neuer otr-can-Stabilitätsnachweis.

Die VM-Prüfrunde läuft weiter; keine vollständige VM-/Portable-/Deinstallationsabnahme daraus abgeleitet.

## Fall 6: Furiosa Teil 1, HD-MP4 mit otr-can 0.1.6

Gleiche Originaldatei wie Fall 5: **Furiosa__A_Mad_Max_Saga__Teil_1_2__26.10.18_20-15_4plus_150_TVOON_DE.HD.mp4**.

FFMS2 indexiert **307470 Quellframes**; die Bildzeitprüfung bestätigt eindeutige, aufsteigende Zeiten und die 25-/50-fps-Struktur. otr-can 0.1.6 schneidet fünf Behalteabschnitte mit Grenzkodierung und IDR-geprüften Kopierbereichen. **171458 Videopakete samt Roh-PTS/DTS** verifiziert.

Beide ursprünglichen Tonspuren bleiben im regulären Schnitt erhalten: **AAC/Stereo/48000 Hz, Ziel 160555 bit/s** und **AC-3/Stereo/48000 Hz, Ziel 384000 bit/s**. Je Spur **329199360 dekodierte Samples** und durchgehende Roh-PTS erfolgreich geprüft. Abschließende MP4-Prüfung bestätigt Video-/Audiopakete, PTS/DTS, Padding und Decoderkonfigurationen. CAN meldet **„OTR-CAN-Schnitt abgeschlossen.“** Kein Notfall-Spurverlust und keine aus der allgemeinen optionalen Logzeile abgeleitete vollständige Videodekodierung.

Der Nutzer bestätigt **„bild schnitt ton 3-sekunden check, teste weiter“**. Die persönliche Tonbestätigung wird nicht ohne weitere Angabe einzeln auf jede Spur ausgedehnt; beide Spuren sind im Protokoll technisch geprüft. AVI, HQ-MP4 und Furiosa HD sind damit in dieser VM-Runde jeweils mit MP4Box und otr-can positiv. Weitere Prüfung läuft; keine pauschale VM-/Portable-/Deinstallationsabnahme oder behauptete Klärung früherer Freeze-Ursachen.

## Fall 7: Furiosa Teil 2, HD-MP4 mit MP4Box

Originaldatei: **Furiosa__A_Mad_Max_Saga__Teil_2_2__26.10.18_22-45_4plus_55_TVOON_DE.HD.mp4**.

MP4Box schneidet zwei Abschnitte und fügt sie zusammen. Die abschließende Schnittstellenprüfung durchläuft Abschnitt 1 und 2; Abschluss **„Fertig.“**. Im vorliegenden Protokoll keine eigene Videopaketanzahl, Tonsamplezahl oder gemessene Ausgabeübergangszeit angegeben; solche Werte werden nicht ergänzt.

Der Nutzer bestätigt **„bild schnitt ton 3-sekunden check, teste weiter“** und danach nochmals **„teste weiter“**. Bild, Schnitt, Ton und Drei-Sekunden-Vorschau positiv; weitere VM-Prüfung bleibt laufend. Dieser Fall bestätigt MP4Box für Teil 2, noch keinen neuen otr-can-Teil-2- oder Klebecenter-VM-Lauf.

## Fall 8: Furiosa Teil 2, HD-MP4 mit otr-can 0.1.6

Gleiche Originaldatei wie Fall 7: **Furiosa__A_Mad_Max_Saga__Teil_2_2__26.10.18_22-45_4plus_55_TVOON_DE.HD.mp4**.

FFMS2 indexiert **165004 Quellframes**; eindeutige aufsteigende Bildzeiten und das 25-/50-fps-Raster bestätigt. otr-can 0.1.6 schneidet zwei Behalteabschnitte und verifiziert **30338 Videopakete samt Roh-PTS/DTS**.

Beide Tonspuren regulär erhalten: AAC/Stereo/48000 Hz mit Ziel 160598 bit/s sowie AC-3/Stereo/48000 Hz mit Ziel 384000 bit/s. Je Spur **58248960 dekodierte Samples** und kontinuierliche Roh-PTS verifiziert. Abschließende MP4-Prüfung bestätigt Video-/Audiopakete, Zeiten, Padding und Decoderkonfigurationen; CAN meldet **„OTR-CAN-Schnitt abgeschlossen.“** Keine aus der optionalen Logzeile abgeleitete vollständige Videodekodierung.

Der Nutzer bestätigt **„bild schnitt ton 3-sekunden check, teste weiter“**. Bild/Schnitt/Ton/Drei-Sekunden-Vorschau persönlich positiv; keine ohne weitere Angabe getrennte persönliche Hörprüfung jeder Spur behauptet. Furiosa Teil 1 und Teil 2 damit in der VM jeweils mit beiden Motoren positiv. Ein VM-Klebecenter-Lauf ist noch nicht Bestandteil dieser acht Schnittfälle. Die gesamte Prüfrunde läuft weiter.

## Fall 9: Klebecenter in der VM – Furiosa Teil 1 und Teil 2

Verwendete fertige Filmteile: **Furiosa A Mad Max Saga Teil 1 otr-can 2026-10-27.mp4** und **Furiosa A Mad Max Saga Teil 2 otr-can 2026-10-27.mp4**. Jeweils ausdrücklich **Spur 1: AAC, Deutsch, Stereo, 48000 Hz** gewählt.

Das vollständige Protokoll bestätigt passende Bildformate, beide Bild-/Quelltonzeitachsen, strenge vollständige Audiodekodierung, samplegenaue PCM-Verbindung mit Vergleich, einmaliges kontinuierliches AAC-LC-Encoding mit 192 kbit/s und finalen MP4-Zusammenbau bei unverändertem Video. Anschließend Prüfung der Bildpakete/-zeiten, AAC-Pakete/Wiedergabezeiten, vollständig dekodierter Tonsamplezahl und sechs Sekunden um die Klebestelle. Ressourcenmeldung für FFmpeg: 1 GiB Speicher und 25 Prozent CPU. Werkzeugpfad aus der VM-Konfiguration, keine Übernahme des Arbeits-PC-Pfads.

**387448320 Tonsamples**, Gesamtdauer **02:14:31.840**, Klebestelle **01:54:18.320**, Drei-Sekunden-Vorlauf **01:54:15.320**. Diese Zeit-/Samplewerte stimmen mit dem Arbeits-PC-Referenzfall überein; keine Bitgleichheit der gesamten Dateien behauptet. Das Protokoll endet mit **„Zusammenfügen und technische Prüfung abgeschlossen.“**

Der Nutzer bestätigt **„jetzt setup klebecenter: bild, ton, klebestelle 3-sekunden vorschau check“**. Damit ist das Klebecenter auch im VM-Setup persönlich positiv geprüft. Zusammen mit den acht Schnittfällen sind neun Funktionstests bestätigt. Eine pauschale Beendigung der VM-Runde sowie Portable-/Deinstallationsabnahmen werden nicht vorweggenommen.

## Deinstallation persönlich bestätigt; Diagnoseordner bleibt

Der Nutzer bestätigt **„deinstallation check“**. Beobachteter Rest im Film-/Ausgabeordner: **.can-join-1c228393a3cc469488f215ef90f82288**, darin **protocol.txt**. Der beigefügte Text ist das erhaltene Klebeprotokoll, kein Deinstallationslog.

Ursache am aktuellen Quellcode geprüft: **Mp4JoinService** entfernt im finally-Block seine großen PCM-/WAV-/Video-/AAC-/MP4-Zwischendateien, behält das kleine Diagnoseprotokoll aber bewusst auch bei erfolgreichem Kleben. Der Release-Deinstaller verwaltet installierte Anwendungsdateien; für Film-/Downloadordner ist keine Löschroutine eingerichtet. Der beobachtete einzelne Protokollordner entspricht damit der bisherigen Aufräumregel, nicht einem neu gescheiterten Klebelauf.

Die Deinstallation wird mit diesem Befund als persönlich positiv gemeldet festgehalten. Verbesserungsbedarf: Nach erfolgreichem Kleben Diagnoseordner möglichst ebenfalls aufräumen; bei Fehlern Diagnose weiter erhalten. Das ist eine vorgemerkte Verhaltensänderung und **noch nicht implementiert**. Bestehende Nutzerordner werden nicht durch den Agenten oder pauschal bei der Deinstallation gelöscht. Neue Portable-Abnahme weiterhin offen.

## Einordnung der AVI-Meldungen

Beim Umpacken protokolliert FFmpeg fehlende ursprüngliche Videozeitstempel („Timestamps are unset“, „pts has no value“). Diese Meldungen bleiben im Nachweis erhalten. Der vorhandene experimentelle AVI-Vorbereitungsweg läuft bis zur erfolgreichen Paket-/Stream-/Zeitprüfung und zum positiv geprüften Schnitt durch. Es wird weder ein warnungsfreier Quellbestand noch eine allgemeine AVI-Reparatur behauptet.

## Abnahmestand nach Deinstallation

Neun Funktionstests sind bestätigt: acht Schnittfälle (vier Dateien jeweils mit beiden Motoren) und das Furiosa-Klebecenter; weitere VM-Setup-Fälle gemäß Nutzerankündigung offen; neue Portable-Prüfung nicht daraus ableiten; Deinstallation inzwischen persönlich mit dem oben beschriebenen Diagnoseordner-Befund bestätigt. Build 40 enthält otr-can 0.1.6; die acht Fälle dokumentieren MP4Box und otr-can jeweils getrennt. Keine Änderung an Produktcode, Motor, Paketen, Prüfsummen oder Buildzähler. Kein neuer Medienlauf/Build/Testlauf durch den Agenten und keine Veröffentlichung.

Lokale Belege im ignorierten **.build/vm-setup-build40-20261027/**: bytegleiche Anhangkopien und CASE1.json bis CASE9.json (Fälle 4 und 8 anhand direkt eingefügter Protokolle) sowie UNINSTALL.json. [Projektstand](AKTUELLER-STAND.md), [Testplan](TESTPLAN.md), [Paketbericht](RELEASE-1.0.2-BUILD40-RESULT.md).

## Übergang zur getrennten ZIP-Prüfung

Nach neun bestätigten Funktionstests und „deinstallation check“ kündigt der Nutzer „jetzt zip“ an. Die neue Portable-Prüfrunde wird unabhängig von den Setup-Fällen dokumentiert; erster Grey’s-Anatomy-/MP4Box-Fall positiv. [Portable-Bericht](VM-PORTABLE-BUILD40-20261027.md).
