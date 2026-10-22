# S.W.A.T. mit otr-can 0.1.6 – persönliche Praxisprüfung

Am **22.10.2026** übermittelt der Nutzer das reguläre CAN-Schnittprotokoll für **S.W.A.T. – Der Schatz der Santa Clara** (HD-Aufnahme vom 21.10.2026, 3plus) mit **otr-can 0.1.6** und bestätigt ausdrücklich **„Bild, ton, 3-sekunden check, schnitt check“**. Bild, Ton, Schnitt und Drei-Sekunden-Vorlauf sind damit persönlich positiv geprüft.

## Befund aus dem Nutzerprotokoll

FFMS2 indexiert die Quelle einmal. CAN bestätigt **172492** eindeutige, aufsteigende Quellbildzeiten und die unterstützte 25/50-fps-Zeitstruktur. Der Motor meldet CPU-Encoding an den Schnittkanten und Kopieren der dazwischenliegenden Intervalle.

| Behalteabschnitt | Framebereich, Ende exklusiv | Bilder | Dauer bei 25 fps |
|---|---|---:|---:|
| 1 | [14885,47934) | 33049 | 1321,960 s |
| 2 | [66610,82576) | 15966 | 638,640 s |
| 3 | [102894,113511) | 10617 | 424,680 s |
| Gesamt | | **59632** | **2385,280 s = 00:39:45.280** |

Die protokollierten Teilbereiche ergeben rechnerisch **345** neu kodierte und **59287** kopierte Bilder. Diese Summe stimmt mit den vom Motor geprüften **59632 Videopaketen** überein. Das Log bestätigt die Prüfung der Video-PTS/DTS. Eine zusätzliche vollständige Videodekodierung ist durch die allgemeine Meldung über die optionale Dekodierprüfung nicht belegt.

Für die eine protokollierte Tonspur werden **Stereo, 48000 Hz, AAC** und **114493440 Referenzsamples** ausgewiesen. Die Zahl entspricht rechnerisch exakt 2385,280 s bei 48 kHz. Der Motor bestätigt dieselbe dekodierte Samplezahl mit kontinuierlichen PTS, anschließend die finale MP4-Prüfung einschließlich Video-/Audiopaketen, PTS/DTS, Padding und Decoderkonfigurationen. Der Arbeitsbereich wird freigegeben; CAN meldet die abschließende MP4-/Tonspurprüfung und **„OTR-CAN-Schnitt abgeschlossen.“**

Diese technischen Angaben stammen aus dem Nutzerprotokoll; in diesem Dokumentationsschritt wurden weder Filmdateien neu analysiert noch ein weiterer Schnitt ausgeführt. Die persönliche Tonbestätigung ist keine gesonderte instrumentelle Synchronitätsmessung.

## Einordnung neben dem MP4Box-Test

Dieselbe Aufnahme wurde zuvor mit MP4Box in drei Segmenten geschnitten. Dort bestätigte der Nutzer **Schnitt, Ton und Drei-Sekunden-Vorlauf**; das Protokoll zeigte gleiche Bildzeit am zweiten Übergang und einen erfolgreichen Abschluss. Die jetzige Rückmeldung ergänzt ausdrücklich **Bild, Ton, Schnitt und Vorlauf für otr-can**. Der genaue Umfang beider persönlichen Prüfungen bleibt getrennt dokumentiert.

Der otr-can-Lauf ist ein weiterer erfolgreicher Realfall mit dem vorhandenen Motor 0.1.6. Er prüft nicht die neue MP4Box-Überlappungszuordnung und klärt die früheren Systemhänger beim Furiosa-/Mad-Max-Schnitt nicht auf. [MP4Box-Praxisfall](MP4BOX-UEBERGANGSINTEGRATION-20261022.md#weiterer-praxisfall-swat--der-schatz-der-santa-clara), [Motorintegration](OTR-CAN-0.1.6-INTEGRATION-20261019.md), [offener Systemfreeze-Befund](OTR-CAN-MAD-MAX-SYSTEMFREEZE-20261020.md).

## Änderungsumfang

Nur dieser Bericht, Projektstatus und Tagebuch ergänzt; Format-/Diffprüfung durchgeführt. Kein neuer Code-, Build- oder Testlauf. Letzter CAN-Nachweis weiterhin **1411/1411 Solution-Tests, 40/40 isolierte Diagnosetests und vier von vier native Furiosa-Prüfungen**. Buildnummer 34, Werkzeugeinstellungen, Motor, vorhandene Pakete und Release-Status unverändert. Keine neue Paketierung oder Veröffentlichung.
