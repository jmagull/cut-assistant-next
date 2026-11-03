# Santa Tell Me – grüner Praxistest, 03.11.2026

**Abschließender Stand 03.11.2026:** CAN 1.0.3 / Zähler 48 einschließlich Buttonkorrektur und persönlicher Sichtbestätigung ist auf Nutzerauftrag eingefroren; Commit und Push sind beauftragt, Veröffentlichung erst nach gesonderter Freigabe. Die folgenden Prüf-/Git-Aussagen dokumentieren ihre jeweiligen früheren Arbeitsschritte. [Aktueller Freeze und Nachweise](FREEZE-1.0.3-20261103.md).

## Zuordnung und persönliche Rückmeldung

**CAN 1.0.3 · Buildzähler 48**, laufende lokale Entwicklung. Der vorherige Screenshot zeigt **„Das soll bleiben“**, vier grüne Bereiche und die Bestätigung **„Behaltebereiche verwenden“** mit geplanter Dauer **01:21:24.120** und voreingestelltem Nein. Film: **Santa Tell Me – Wer wird meine Weihnachtsliebe**, super RTL, Aufnahme vom 02.11.2026, 20:15 Uhr, HQ/MP4.

Danach übermittelt der Nutzer die gespeicherte Cutlist, ein vollständiges MP4Box-Protokoll und das otr-can-Protokoll. Er bestätigt ausdrücklich **„Ton Bild Schnitt 3-Sekunden Vorschau check :-)“**. Der grüne Filmschnitt ist damit persönlich positiv gemeldet; beide technischen Motorläufe sind unten getrennt nachgewiesen. Die persönliche Rückmeldung wurde gemeinsam mit beiden Protokollen übermittelt und nicht für jeden Motor in einem eigenen Satz wiederholt.

Ein anschließender Screenshot zeigt dieselbe Cutlist in der eigenen Uploadübersicht des Servers: Titel, Autor Joerg, vier Bereiche, Dauer **01:21:24**, Kommentar mit otr-can/MP4Box und Originaldateiname stimmen überein. Der Servereintrag ist sichtbar; der konkrete Upload-Bedienweg wird aus dem Bild allein nicht abgeleitet.

## Gespeicherte Cutlist und Zeitabgleich

Die vom Nutzer benannte Original-Cutlist wurde ausschließlich gelesen. Metadaten: CAN-Version **1.0.3**, **25 fps**, Seitenverhältnis **16:9**, Originaldateigröße **1151972554 Bytes**, vier Keep-Bereiche; vorgesehener Motor **otr-can 0.1.6**. Der Bezug bleibt die Originaldatei **Santa_Tell_Me_-_Wer_wird_meine_Weihnachtsliebe__26.11.02_20-15_srtl_105_TVOON_DE.HQ.mp4**.

| Keep | Anfang (s) | Ende exklusiv (s) | Dauer (s) | Bilder bei 25 fps |
| --- | ---: | ---: | ---: | ---: |
| 1 | 579,760 | 1772,360 | 1192,600 | 29815 |
| 2 | 2234,360 | 3212,920 | 978,560 | 24464 |
| 3 | 3625,680 | 5192,280 | 1566,600 | 39165 |
| 4 | 5698,200 | 6844,560 | 1146,360 | 28659 |
| Summe | | | **4884,120** | **122103** |

**4884,120 s = 01:21:24.120** stimmen mit grüner Tabelle und Bestätigungsabfrage überein. Bei **48000 Hz** ergeben sich **234437760 Samples**, passend zur verifizierten otr-can-Tonspur. Die Serveranzeige ist auf ganze Sekunden beschränkt; eine neue genaue Messung der MP4Box-Ausgabedauer wurde nicht übermittelt.

Der Kommentar nennt vollständig entfernte Werbung, otr-can und alternativ MP4Box/GPAC **26.07**. Kommentar und Autorbewertung sind keine automatische Fertigstellungserkennung durch CAN. Aus dem gespeicherten Format wird kein zusätzlicher Bearbeitungsstatus abgeleitet.

## MP4Box – technischer Lauf

Das beigefügte Protokoll belegt vier erfolgreiche Segmentaufrufe und anschließendes Zusammenfügen aller vier Teile. Die **-splitx**-Aufrufgrenzen lauten:

| Segment | Anfang (s) | MP4Box-Aufrufende (s) |
| --- | ---: | ---: |
| 1 | 579,760 | 1772,320 |
| 2 | 2234,360 | 3212,880 |
| 3 | 3625,680 | 5192,240 |
| 4 | 5698,200 | 6844,520 |

Die Aufrufenden liegen jeweils **40 ms / ein Bild bei 25 fps** vor den exklusiven Cutlist-Enden; das ist die vorhandene MP4Box-Aufrufdarstellung und keine neue Änderung des grünen Editors. Daraus allein wird keine tatsächliche Verkürzung der fertigen Ausgabe abgeleitet.

Die anschließende Schnittstellenprüfung verarbeitet Abschnitt 1 bis 4 und endet mit **„Fertig.“** An Übergang **1 und 3** wird wegen gleicher Bildzeit am Abschnittswechsel der dekodierte neue Anfang zusätzlich geprüft. Diese Meldungen bleiben dokumentiert; sie sind im übermittelten Lauf keine Abbruchmeldung. Beim ersten angehängten Teil erzeugt MP4Box die benötigten Video-/Ton-Zielspuren und hängt danach die weiteren Teile an.

## otr-can 0.1.6 – technischer Lauf

- FFMS2-Indexierung abgeschlossen; CAN bestätigt **194907** eindeutige, aufsteigende Quellbildzeiten und die 25/50-fps-Zeitstruktur.
- Vier Keep-Intervalle stimmen mit den exklusiven Cutlist-Grenzen überein. Geeignete Teile werden kopiert und die Endränder neu kodiert: **79 / 1 / 2 / 103 Bilder**, insgesamt **185 CPU-Bilder** neben **121918 kopierten Bildern**.
- **122103 Videopakete** und rohe PTS/DTS laut Motor verifiziert. Die allgemeine Zeile zur optionalen Vollvideodekodierung beweist keinen zusätzlich angeforderten Voll-Dekodierlauf.
- Eine Tonspur: **AAC, 2 Kanäle, 48000 Hz**, Zielbitrate **164353 bit/s**. **234437760 decodierte Samples** und kontinuierliche rohe PTS verifiziert; komplette Tonspur einmal kodiert.
- Finale MP4 einschließlich Video-/Tonpaketen, PTS/DTS, Padding und Decoderkonfigurationen laut Motor geprüft. Arbeitsordnersperre freigegeben; CAN schließt seine Ausgabe-/Tonspurprüfung mit **„OTR-CAN-Schnitt abgeschlossen.“** ab.

Die Prüfung der realen Bild-/Tonwirkung stammt aus der persönlichen Rückmeldung, die technischen Zahlen aus dem übermittelten Protokoll. Cutlist und Videodateien wurden nicht verändert und kein eigener weiterer Filmschnitt gestartet.

## Aktualisierter Abnahmestand

- Grüner Modus mit mehreren getrennten Behaltebereichen, Ausgabebestätigung und vollständigem Filmschnitt: **persönlich positiv gemeldet**.
- Speichern als klassische Cutlist: **vorliegende gültige Datei mit den vier gezeigten Bereichen**.
- Servereintrag derselben Cutlist: **im Screenshot sichtbar**.
- Vollständige persönliche Wiederaufnahme einer gespeicherten begonnenen Arbeit einschließlich Weiterbearbeiten: **weiterhin offen**; Speichern und Serveranzeige allein belegen diesen Ablauf nicht.
- Kein neuer Setup-/Portable-/VM-Nachweis. Keine Änderung an Produktcode, Schnittmotoren, Format, Version oder Buildzähler; kein Commit/Push/Paket/Release.

Der zuvor dokumentierte automatische Entwicklungsstand bleibt **1709 bestandene Tests, Build ohne Warnungen/Fehler**. Dies ist ein ergänzender persönlicher Praxisnachweis und kein zusätzlicher automatisierter Testfall.

[Projektstand](AKTUELLER-STAND.md) · [Testplan](TESTPLAN.md) · [Schnittmodi](SCHNITTMODI-TIMELINE-20261103.md) · [Entwicklungsnotizen](RELEASE-NOTES-1.0.3.md)
