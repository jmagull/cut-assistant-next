# Portable-ZIP Build 40 in der VM – persönliche Prüfung abgeschlossen

**Stand 27.10.2026.** Nach neun positiven Setup-Funktionstests und bestätigter Deinstallation kündigt der Nutzer **„jetzt zip“** an. Die folgenden Fälle werden getrennt als Portable-Prüfung erfasst. Build-40-Zuordnung aus der Gesprächsfolge; keine pauschale Abnahme eines frischen Einstellungsprofils oder aller Portable-Funktionen behauptet.

## Fall 1: Grey’s Anatomy – Durchs Feuer mit MP4Box

Originaldatei: **Greys_Anatomy_-_Die_jungen_Aerzte__Durchs_Feuer_26.09.30_20-15_sixx_55_TVOON_DE.HQ.mp4**.

MP4Box schneidet drei Abschnitte und fügt sie zusammen. Die anschließende Schnittstellenprüfung durchläuft alle drei Abschnitte und endet mit **„Fertig.“**. Keine Videopaketanzahlen, Tonsamplezahlen oder tatsächlichen Ausgabeübergangszeiten im vorliegenden Log ergänzt.

Der Nutzer bestätigt **„bild ton schnitt, 3 sekunden vorschau check“**. Damit ist der erste persönliche VM-Portable-Fall positiv für Bild, Ton, Schnitt und Drei-Sekunden-Vorschau. Dieser Fall enthält MP4Box, keinen neuen otr-can- oder Klebecenter-Portable-Lauf.

## Fall 2: Die Teufelsbrigade AVI mit otr-can 0.1.6

Originaldatei: **Die_Teufelsbrigade_21.01.21_20-15_tele5_160_TVOON_DE.mpg.HQ.avi**.

FFmpeg übernimmt H.264-Bild und MP3-Ton ohne Neukodierung in eine MP4-Arbeitsdatei. Die Vorbereitung meldet fehlende ursprüngliche Videozeitstempel (Timestamps are unset / pts has no value); diese Hinweise bleiben Teil des Nachweises. Die anschließende Stream-, Videopaket-, Bildraten- und Laufzeitprüfung besteht; FFMS2 bestätigt 263694 eindeutige, aufsteigende Bildzeiten.

otr-can 0.1.6 schneidet sechs behaltene Abschnitte. Geprüft sind **182574 Videopakete** mit PTS/DTS und **350542080 dekodierte Tonsamples** bei Stereo/48000 Hz. MP3 wird nach dem PCM-Schnitt einmal verlustbehaftet als AAC-LC mit 192 kbit/s kodiert. Die abschließende Prüfung der MP4-Pakete, Zeitstempel, Padding- und Decoderkonfigurationen besteht; der Schnitt ist abgeschlossen.

Der Nutzer bestätigt **„bild ton schnitt, 3 sekunden vorschau check und geprüft“**. Zweiter persönlicher Portable-Fall positiv; kein pauschaler Gesamtabschluss. Bytegleiche Anhangkopie und CASE2.json liegen unter **.build/vm-portable-build40-20261027/**.

## Fall 3: Furiosa-Klebecenter AAC/AAC – ZIP-Prüfung abgeschlossen

Filmteile: **Furiosa A Mad Max Saga Teil 1 otr-can 2026-10-27.mp4** und **Furiosa A Mad Max Saga Teil 2 otr-can 2026-10-27.mp4**. Jeweils Spur 1: AAC, Deutsch, Stereo, 48000 Hz.

Bild unverändert übernommen; ausgewählter Ton samplegenau verbunden und einmal durchgehend als AAC-LC mit 192 kbit/s kodiert. Das Protokoll bestätigt die Prüfung der Bild-/Tonpakete und Wiedergabezeiten, die vollständige Dekodierung des fertigen Tons sowie die Dekodierung von drei Sekunden vor bis drei Sekunden nach der Klebestelle.

- Geprüfte Tonsamples: **387448320**.
- Klebestelle: **01:54:18.320**; Vorschau beginnt bei **01:54:15.320**.
- Gesamtdauer: **02:14:31.840**.

Der Nutzer bestätigt **„bild ton klebestelle 3-sekunden vorschau check , damit zip check“**. Damit sind dieser dritte Portable-Fall und die persönliche ZIP-Prüfrunde abgeschlossen. Bytegleiche Anhangkopie und CASE3.json unter **.build/vm-portable-build40-20261027/**.

## Prüfstand

Drei Portable-Fälle bestätigt: Grey’s Anatomy/MP4Box, Teufelsbrigade AVI/otr-can und Furiosa-Klebecenter AAC/AAC. Der Nutzer hat die persönliche ZIP-Prüfung ausdrücklich mit „damit zip check“ abgeschlossen. Die neun Setup-Funktionstests und die Deinstallation bleiben eigenständige Nachweise. Der nach erfolgreichem Kleben verbliebene Diagnoseordner ist dokumentiert; dessen Erfolgsbereinigung ist nur vorgemerkt und nicht implementiert.

Keine Code-/Paket-/Zähleränderung, kein neuer Medienlauf/Build/Testlauf durch den Agenten, keine Veröffentlichung. Bytegleiche Anhangkopie und CASE1.json unter **.build/vm-portable-build40-20261027/**. [Setup-Prüfung](VM-SETUP-BUILD40-20261027.md), [Projektstand](AKTUELLER-STAND.md), [Testplan](TESTPLAN.md), [Paketbericht](RELEASE-1.0.2-BUILD40-RESULT.md).
