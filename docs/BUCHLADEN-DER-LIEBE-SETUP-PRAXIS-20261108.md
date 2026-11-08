# Buchladen der Liebe – persönliche Setup-Prüfung

Stand: **08.11.2026**, CAN **1.0.3 Build 50**, Setup-Fassung auf dem Arbeitsrechner. Die Zuordnung zur Setup-Prüffassung folgt der laufenden Paketabnahme und der ausdrücklichen Nutzerangabe, nicht einer Buildkennung aus den Motorprotokollen.

Film: **Buchladen der Liebe – Winterzauber für zwei Herzen**, Aufnahme vom **07.11.2026, 20:15 Uhr, Swiss1**, HQ/MP4. Der Nutzer hat Spotlight zur groben Schnittauswahl verwendet und anschließend in CAN gearbeitet.

## Persönlich bestätigt

- **MP4Box und otr-can:** Bild, Ton, Schnitt und Drei-Sekunden-Vorschau bestanden.
- **otr-can 0.1.6:** kein Blitzer der Vorgängersendung.
- **Spotlight:** auf Bildschirmgröße maximiert; der Maximierungszustand bleibt erhalten.
- Der letzte betroffene Bereich bis zum tatsächlichen Ende des Originals muss nach der groben Bildauswahl in CAN mit **Videoende** abgeschlossen werden. Dieser Bedienhinweis ist in der neuen [Spotlight-README](SPOTLIGHT-README.md) und der Nutzeranleitung aufgenommen.

Die nachgereichte eigene Cutlist meldet für **MP4Box einen Blitzer der vorherigen Sendung am Anfang**. Das bleibt als bekannter Unterschied zur persönlich blitzerfrei bestätigten otr-can-Ausgabe erhalten. Die positive Bild-/Ton-/Schnitt-/Vorschauprüfung mit MP4Box ist keine Bestätigung der Blitzerfreiheit. Die Rückmeldung bestätigt keine neue saubere VM, keinen Installationstyp (frisch/Überinstallation), keine Deinstallation und keine Portable-Abnahme.

## Eigene Cutlist

Die zusätzlich bereitgestellte Cutlist wurde ausschließlich gelesen und unverändert erhalten. Sie nennt CAN **1.0.3**, **25 fps**, **972471902 Originalbytes**, fünf Behaltebereiche, Autor **Joerg** und vorgesehenen Motor **otr-can 0.1.6**. Die fünf Start-/Dauerpaare stimmen exakt mit der Tabelle unten und den otr-can-Intervallen überein. Gesamt **4922,120 Sekunden**. Buildnummer und Setup-Umgebung ergeben sich aus der persönlichen Rückmeldung, nicht aus der Cutlist.

Im Kommentar stehen die vollständige Werbeentfernung, der framegenaue otr-can-Schnitt und ausdrücklich der Anfangsblitzer mit MP4Box. Die Angaben zu fehlendem Anfang/Ende/Bild/Ton stehen auf 0; das sind gespeicherte Autorangaben, keine zusätzliche eigene Medienprüfung. Kein Upload oder Verändern der persönlichen Datei und keine Aufnahme ihrer Originaldatei in Git.

SHA-256 der unveränderten geprüften Cutlist: `D198D56AF8D5E4EF81A7E3A741C294B922B1A25FEDA46FC75B3229E4390FB50F`.

## Protokollvergleich der fünf Behaltebereiche

| Bereich | otr-can Anfang (s) | otr-can Ende, exklusiv (s) | Dauer (s) | MP4Box-Aufruf (s) |
| --- | ---: | ---: | ---: | --- |
| 1 | 602,24 | 1742,48 | 1140,24 | 602,24–1742,44 |
| 2 | 2083,40 | 3360,64 | 1277,24 | 2083,40–3360,60 |
| 3 | 3751,12 | 4862,16 | 1111,04 | 3751,12–4862,12 |
| 4 | 5244,20 | 6303,12 | 1058,92 | 5244,20–6303,08 |
| 5 | 6627,00 | 6961,68 | 334,68 | 6627,00–6961,64 |

MP4Box verwendet jeweils den um **40 ms** früheren Aufrufendwert. Das wird getrennt vom exklusiven otr-can-Ende dokumentiert; daraus wird keine ausgemessene gleiche Ausgabedauer oder Änderung der etablierten Schnittregeln abgeleitet.

## MP4Box-Nachweis

Das angehängte Nutzerprotokoll zeigt fünf erfolgreiche `-splitx`-Aufrufe, anschließendes Zusammenfügen und Prüfung aller fünf Abschnitte im fertigen Film. Bei den vier Übergängen bleibt der Hinweis **„gleiche Bildzeit am Abschnittswechsel; dekodierter neuer Anfang wird geprüft“** erhalten. Abschluss **„Fertig.“**. Die persönliche Rückmeldung bestätigt danach Bild, Ton, Schnitt und Vorschau. Eine genaue GPAC-Version ist im bereitgestellten Protokoll nicht ausgewiesen.

## otr-can-Nachweis

FFMS2 indexiert einmal. CAN bestätigt **194957 eindeutige, aufsteigende Quellbildzeiten** und die 25/50-fps-Zeitstruktur. Motorversion **0.1.6**.

- Video: **122509 kopierte + 544 CPU-kodierte = 123053 Pakete**; rohe PTS/DTS laut Motor geprüft.
- Eine Tonspur: Stereo, 48000 Hz, AAC-Zielbitrate 163541 bit/s; **236261760 Referenzsamples** und dieselbe Zahl decodierter Ausgabesamples, kontinuierliche rohe PTS.
- Finale MP4 laut Motor einschließlich Bild-/Tonpaketen, Zeitstempeln, Padding und Decoderkonfigurationen geprüft; Workspace-Sperre freigegeben. CAN meldet **„OTR-CAN-Schnitt abgeschlossen.“**

**123053 / 25 = 236261760 / 48000 = 4922,120 s = 01:22:02.120.** Dies entspricht auch der Summe der fünf Behaltedauern. Die allgemeine Motorzeile zur optionalen Vollvideodekodierung belegt keinen gesondert angeforderten vollständigen Decoderlauf.

## Umfang des Nachtrags

Nur bereitgestellte Nutzerprotokolle, eigene Cutlist und persönliche Rückmeldung ausgewertet; Originalfilm und fertige Filme wurden nicht selbst geöffnet oder neu vermessen. Keine Produktcodeänderung, kein neuer eigener Filmschnitt, kein erneuter Build und kein neuer Suite-Lauf für diesen Dokumentationsnachtrag. Der technische Paketnachweis mit 1773 CAN-/78 Spotlight-Tests, nativen Prüfungen und festen Paket-Hashes bleibt erhalten.

Setup, Portable und Quellarchive werden nicht nachträglich verändert. Die neue Spotlight-README wird zusätzlich als lesbare Begleitdatei zum lokalen Paketsatz bereitgestellt; die Prüfsummenliste wird dafür ergänzt. Keine neue Veröffentlichung und keine Freigabe für einen Release-Tag oder Merge nach main.
