# The Christmas Cookbook – persönlicher Praxistest, 03.11.2026

**Abschließender Stand 03.11.2026:** CAN 1.0.3 / Zähler 48 einschließlich Buttonkorrektur und persönlicher Sichtbestätigung ist auf Nutzerauftrag eingefroren; Commit und Push sind beauftragt, Veröffentlichung erst nach gesonderter Freigabe. Die folgenden Prüf-/Git-Aussagen dokumentieren ihre jeweiligen früheren Arbeitsschritte. [Aktueller Freeze und Nachweise](FREEZE-1.0.3-20261103.md).

## Zuordnung und persönliche Bestätigung

Film: **The Christmas Cookbook – Ein Weihnachtsmenü fürs Herz**, Aufnahme vom **02.11.2026, 22:00 Uhr**, super RTL, HQ/MP4. Laufende lokale Prüfrunde **CAN 1.0.3 · Buildzähler 48 · Power-User 2**; die Zuordnung folgt dem Gespräch und den unmittelbar vorherigen Bildschirmaufnahmen. Das Schnittprotokoll selbst enthält keine CAN-Versionsangabe.

Die vorherigen Aufnahmen zeigen den roten Bearbeitungsmodus, die neue Anordnung und den hervorgehobenen Anfang **00:27:09.240**. Bei späterer Playerposition **00:35:19** bleibt diese Vormerkzeit fest und die Vorschau wächst mit. Die persönlichen Rückmeldungen zur Darstellung sind positiv.

Der Nutzer übermittelt anschließend den erfolgreichen **otr-can-0.1.6**-Lauf und bestätigt ausdrücklich:

> Ton bild Schnitt 3sekunden vorschau top schließe jetzt can und wechsele die Version

Damit sind **Ton, Bild, Schnitt und Drei-Sekunden-Vorschau für diesen otr-can-Lauf persönlich positiv geprüft**. Der Nutzer kündigt danach das Schließen von CAN und den Wechsel in die bereits getrennt bereitgestellte Button-Ausrichtungskorrektur an. Deren Start und persönliche Abnahme werden durch diese Ankündigung noch nicht vorweggenommen.

## Technischer Nachweis aus dem übermittelten Protokoll

- FFMS2-Indexierung vollständig abgeschlossen. CAN bestätigt **194893** eindeutige, aufsteigende Quellbildzeiten und passende 25/50-fps-Zeitstruktur.
- Vier Behalteintervalle verarbeitet; geeignete Videoteile kopiert, Endränder auf der CPU neu kodiert.
- **121724 Videopakete** und rohe PTS/DTS laut Motor verifiziert. Die allgemeine Zeile zur optionalen Vollvideodekodierung belegt keine zusätzlich angeforderte Voll-Dekodierung.
- Eine Tonspur: **AAC, 2 Kanäle, 48000 Hz**, Zielbitrate **164495 bit/s**. **233710080 decodierte Samples**, kontinuierliche rohe PTS; vollständige Spur einmal kodiert.
- Finale MP4 einschließlich Video-/Tonpaketen, PTS/DTS, Padding und Decoderkonfigurationen laut Motor geprüft. Workspace-Sperre freigegeben; CAN endet nach seiner MP4-/Tonspurprüfung mit **„OTR-CAN-Schnitt abgeschlossen.“**

| Keep | Anfang laut Protokoll (s) | Ende (s) | Kopierte Bilder | CPU-Bilder | Summe Bilder |
| --- | ---: | ---: | ---: | ---: | ---: |
| 1 | 549,280000 | 1629,240000 | 26906 | 93 | 26999 |
| 2 | 2119,639999 | 3424,240000 | 32473 | 142 | 32615 |
| 3 | 3931,480000 | 5587,440000 | 41310 | 89 | 41399 |
| 4 | 5995,880000 | 6824,320000 | 20706 | 5 | 20711 |
| Summe | | | **121395** | **329** | **121724** |

Die zweite Startangabe ist hier unverändert aus dem Protokoll übernommen. Die zugehörige Framefolge beginnt bei **52991**; bei 25 fps entspricht das **2119,640 s**. Es wird aus der Dezimaldarstellung keine neue Schnittpunktänderung abgeleitet.

**121724 / 25 = 4868,960 s = 01:21:08.960**. Unabhängig ergibt **233710080 / 48000** dieselbe Dauer. Bild- und Tonzahlen des Motors passen damit rechnerisch zusammen. Dies ist kein zusätzliches eigenes Ausmessen der fertigen Datei.

## Abnahmestand und Grenzen

Der Praxisfall bestätigt die ursprüngliche Power-User-2-Prüffassung einschließlich eines echten otr-can-Schnitts. Für diese Aufnahme liegt hier kein neues MP4Box-Protokoll, keine gesondert übermittelte Cutlist und kein zusätzlicher Server-/Uploadnachweis vor. Die persönliche Wiederaufnahme einer begonnenen Arbeit bleibt ein eigener offener Fall.

Die getrennte Ausrichtungskorrektur unter `.build/power-user2-alignment-20261103/` wurde zuvor gebaut und mit **1728 Tests** sowie einer gezielten nativen Ausrichtungsprüfung erfolgreich kontrolliert. Im vorliegenden Nachtrag werden nur Nutzerprotokoll und Rückmeldung dokumentiert; kein neuer Medienlauf, keine Produktcodeänderung und keine erneut ausgeführte Testsuite. Version 1.0.3 und Zähler 48 bleiben unverändert. Keine echte Filmdatei in Git aufgenommen; kein Commit, Push, Paketbau oder Release.

[Power-User-2-Umsetzung und Buttonkorrektur](POWER-USER-2-IMPLEMENTIERUNG-20261103.md) · [Aktueller Stand](AKTUELLER-STAND.md) · [Testplan](TESTPLAN.md)
