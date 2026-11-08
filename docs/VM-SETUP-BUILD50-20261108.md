# CAN 1.0.3 Build 50 – persönliche VM-Setup-Prüfung

Stand **08.11.2026**. Der Nutzer bestätigt für die **Setup-Fassung in der VM**: **Schnitt, Bild, Ton, Drei-Sekunden-Vorschau und Spotlight bestanden**. Zuordnung zu Build 50 aus der laufenden Paketabnahme; das Protokoll selbst enthält keine CAN-Buildkennung.

Film laut bereitgestelltem Protokoll: **Grey’s Anatomy – Die jungen Ärzte: Durchs Feuer**, Aufnahme vom **30.09.2026, 20:15 Uhr, sixx**, HQ/MP4. Das erste Protokoll belegt **MP4Box**, das nachgereichte Protokoll **otr-can 0.1.6** als Schnittmotor. Die anschließende Rückmeldung „genauso hier check :-)“ bestätigt auch otr-can für Schnitt, Bild, Ton und Drei-Sekunden-Vorschau in derselben VM-Setup-Prüfung.

| Abschnitt | MP4Box-Aufruf Anfang (s) | MP4Box-Aufruf Ende (s) |
| --- | ---: | ---: |
| 1 | 592,56 | 1759,12 |
| 2 | 2314,20 | 3005,44 |
| 3 | 3565,52 | 4133,64 |

Drei Segmente werden erfolgreich geschnitten und zusammengefügt. Anschließend prüft CAN alle drei Abschnitte im fertigen Film; Abschluss **„Fertig.“**. Die persönliche Rückmeldung bestätigt die anschließende Sicht-/Hörkontrolle einschließlich Vorschau sowie Spotlight in dieser VM-Setup-Umgebung. Keine eigene Messung des fertigen Films oder Ausführung in der VM durch den Agenten.

## Nachgereichter otr-can-Lauf – ebenfalls persönlich bestanden

FFMS2 indexiert einmal, CAN bestätigt **119927 eindeutige, aufsteigende Quellbildzeiten** und die 25/50-fps-Zeitstruktur. **otr-can 0.1.6** verarbeitet dieselben drei Behaltebereiche:

| Abschnitt | Anfang (s) | Ende, exklusiv (s) | Dauer (s) |
| --- | ---: | ---: | ---: |
| 1 | 592,56 | 1759,16 | 1166,60 |
| 2 | 2314,20 | 3005,48 | 691,28 |
| 3 | 3565,52 | 4133,68 | 568,16 |

**60365 kopierte + 286 CPU-kodierte = 60651 Videopakete**, rohe PTS/DTS laut Motor verifiziert. Eine Stereo-Tonspur mit **48000 Hz**, AAC-Zielbitrate **165457 bit/s**, **116449920 Referenzsamples** und gleich vielen decodierten Ausgabesamples; kontinuierliche rohe PTS bestätigt.

**60651 / 25 = 116449920 / 48000 = 2426,040 s = 00:40:26.040.** Dies entspricht der Summe der drei Behaltedauern. Die MP4Box-Aufrufenden im ersten Protokoll liegen jeweils 40 ms vor diesen exklusiven Enden; keine eigene Ausmessung oder Zusage einer identischen MP4Box-Ausgabedauer daraus abgeleitet.

Der Motor bestätigt die finale MP4 einschließlich unveränderter Bild-/Tonpakete, PTS/DTS, Padding und Decoderkonfigurationen, gibt die Workspace-Sperre frei und CAN meldet **„OTR-CAN-Schnitt abgeschlossen.“** Die allgemeine Zeile zur optionalen vollständigen Dekodierung belegt keinen ausdrücklich angeforderten Vollvideodekoderlauf.

Die persönliche Rückmeldung bestätigt denselben Prüfungsumfang wie zuvor: Schnitt, Bild, Ton und Drei-Sekunden-Vorschau bestanden. Damit sind beide Schnittmotoren für die VM-Setup-Fassung positiv geprüft. Keine zusätzliche Aussage zur Blitzerfreiheit.

## Umfang und noch offene Punkte

- **Bestätigt:** Nutzung der Setup-Fassung in der VM, MP4Box- und otr-can-Schnitt, Bild, Ton, Drei-Sekunden-Vorschau, Spotlight.
- **Nicht gesondert bestätigt:** Portable, Deinstallation, frische Installation gegenüber Überinstallation, sauberer VM-Ausgangszustand und konkrete Gast-Windows-Version. Keine zusätzliche Aussage zur Blitzerfreiheit oder zu physischen Monitor-/DPI-Wechseln.
- Die positive Prüfung beider Schnittmotoren auf dem Arbeitsrechner mit **Buchladen der Liebe** bleibt ein eigener [Praxisnachweis](BUCHLADEN-DER-LIEBE-SETUP-PRAXIS-20261108.md). Sie wird nicht als zusätzlicher otr-can-VM-Test gezählt.

Nur persönliches Nutzerprotokoll und Rückmeldung dokumentiert. Produktcode, Buildnummer 50, Setup, Portable, Quellen und Prüfsummen unverändert. Kein erneuter Build oder Suite-Lauf für diesen Dokumentationsnachtrag. Keine Veröffentlichung, kein Release-Tag und kein Merge nach main.

[Paketbericht](RELEASE-1.0.3-SPOTLIGHT-BUILD50-RESULT.md) · [Spotlight-README](SPOTLIGHT-README.md).
