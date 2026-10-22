# Furiosa mit otr-can 0.1.6 – erfolgreicher Nutzerlauf am 22.10.2026

Der Nutzer übermittelt ein erfolgreiches reguläres CAN-Protokoll für **Furiosa – A Mad Max Saga, Teil 1/2** (HD, 4plus, Aufnahme 18.10.2026) mit **otr-can 0.1.6** und bestätigt **„Bild, Ton, schnitt, 3 sekunden check“**. Bild, Ton, Schnitt und Drei-Sekunden-Vorlauf sind damit persönlich positiv geprüft.

## Technischer Befund aus dem Protokoll

FFMS2 indexiert die Quelle einmal. CAN bestätigt **307470** eindeutige aufsteigende Quellbildzeiten und die unterstützte 25/50-fps-Zeitstruktur. Der Motor verarbeitet fünf Behalteabschnitte mit CPU-Encoding an den Kanten und Kopieren der dazwischenliegenden Intervalle:

| Abschnitt | Framebereich, Ende exklusiv | Bilder |
|---|---|---:|
| 1 | [14580,71125) | 56545 |
| 2 | [90099,110018) | 19919 |
| 3 | [129196,166434) | 37238 |
| 4 | [173789,201972) | 28183 |
| 5 | [221223,250796) | 29573 |
| Gesamt | | **171458** |

Die Bereiche ergeben bei 25 fps rechnerisch **6858,320 s = 01:54:18.320**. Davon werden laut Teilbereichen **707** Bilder neu kodiert und **170751** kopiert. Der Motor bestätigt **171458 geprüfte Videopakete und rohe PTS/DTS**. Eine zusätzliche vollständige Videodekodierung ist aus der allgemeinen Meldung zur optionalen Prüfung nicht belegt.

Beide protokollierten Tonspuren haben **Stereo/48000 Hz**, eine **AAC**, eine **AC3**. Je Spur weist der Motor **329199360 Referenzsamples** und dieselbe Zahl dekodierter Samples mit kontinuierlichen rohen PTS aus. Das stimmt rechnerisch mit der Video-Behaltedauer überein. Die finale native Prüfung bestätigt Video-/Audiopakete, PTS/DTS, Padding und Decoderkonfigurationen. Anschließend Arbeitsbereich freigegeben, CAN-Ausgabe-/Tonspurprüfung und **„OTR-CAN-Schnitt abgeschlossen.“**

Das Ende der allgemeinen Aufforderung zur persönlichen Kontrolle ist im übermittelten Text abgeschnitten. Der erfolgreiche Motor-/CAN-Abschluss und die anschließende ausdrückliche Nutzerbestätigung sind enthalten. Keine getrennte instrumentelle Synchronitätsmessung jeder Tonspur aus der allgemeinen Tonbestätigung abgeleitet.

## Zwei weitere positive Nutzerläufe am 22.10.2026

Im Anschluss meldet der Nutzer **„2 weiter otr-can läufe mit mad mxx check bei allen parametern“**. Im Kontext der vorherigen Abnahme sind damit zwei zusätzliche erfolgreiche Furiosa-Läufe einschließlich Bild, Ton, Schnitt und Drei-Sekunden-Vorlauf persönlich bestätigt. Für diese beiden Wiederholungen wurden keine zusätzlichen Einzelprotokolle übermittelt.

Die aktuelle Rückmeldeserie umfasst damit **drei vom Nutzer positiv gemeldete otr-can-Läufe**: den oben ausgewerteten Lauf mit vollständigem Abschlussprotokoll und zwei weitere persönlich bestätigte Wiederholungen. Die früheren VM-Erfolge vom 20.10. werden dabei nicht erneut gezählt. Die Paket-/Samplezahlen des ersten Protokolls werden nicht ohne Einzelbeleg auf die Wiederholungen übertragen. Aus der Meldung allein werden keine identischen Schnittgrenzen, Ressourcenbedingungen oder dieselbe ununterbrochene CAN-Sitzung abgeleitet.

## Verhältnis zu den früheren Systemhängern

Der erfolgreiche Lauf beginnt bei **583,200 s**. Das entspricht den früher dokumentierten korrigierten Teil-1-Aufträgen; der letzte geschützte Freeze-Auftrag begann dagegen bei **579,960 s**. Der neue Erfolg ist deshalb kein identischer Wiederholungstest dieses letzten Fehlerauftrags. Ein früherer ungeschützter Fehlversuch hatte ebenfalls den Beginn 583,200 s; der Erfolg ist ein weiterer positiver Befund für diese Fassung.

Die Quelle liegt laut Protokoll unter dem Benutzerpfad `Desktop/down`. Aus diesem Pfad allein werden weder der tatsächliche Ausführungshost noch Ressourcenlimits, Treiberzustand oder die Anzahl vorheriger Aufträge in derselben CAN-Sitzung abgeleitet. Keine neue Ressourcen-/Windows-Ereignismessung durchgeführt. Ursache und Behebung der früheren vollständigen Rechnerstillstände bleiben unbewiesen; die damaligen Belege und die dokumentierte Teststartersperre bleiben erhalten.

Der vorhandene otr-can-Motor ist unabhängig von der neu integrierten MP4Box-Übergangszuordnung. Der Nutzerlauf ist ein Erfolgs- und Qualitätsnachweis im gemeldeten Umfang, keine allgemeine Stabilitäts- oder Releasefreigabe. [Bisheriger Systemfreeze-Befund](OTR-CAN-MAD-MAX-SYSTEMFREEZE-20261020.md), [MP4Box-Prüfung und persönliche Abnahme](MP4BOX-UEBERGANGSINTEGRATION-20261022.md).

## Dokumentationsumfang

Nur dieser Prüfbericht, ein datierter Nachtrag zur Freeze-Diagnose, Status und Tagebuch ergänzt. Protokollzahlen unabhängig nachgerechnet und Diffprüfung durchgeführt. Kein eigener Medienlauf, keine Code-, Werkzeug- oder Einstellungsänderung, kein neuer Build/Testlauf. Letzter CAN-Testnachweis weiterhin **1411/1411 Solution-Tests, 40/40 isolierte Tests und vier von vier native Furiosa-MP4Box-Prüfungen**. Buildnummer 34 und Release-Status unverändert; keine neue Paketierung oder Veröffentlichung.
