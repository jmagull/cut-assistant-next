# VM-Setup Build 47 – persönliche Prüfnachweise

**Veröffentlichungsnachtrag 31.10.2026:** Der bestätigte Stand ist nach Nutzerfreigabe eingefroren, committet/gepusht und als [v1.0.2-build47 / Latest](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2-build47) veröffentlicht. Die folgenden datierten Prüf-/Auftragsstände bleiben als Historie erhalten; nicht separat gemeldete persönliche Testschritte werden dadurch nicht ergänzt. [Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md).

## Freigabe zum Einfrieren und Veröffentlichen am 31.10.2026

Der Nutzer beauftragt ausdrücklich, den bestätigten Stand einzufrieren, README/Dokumentation abzugleichen, zu committen, zu pushen und zu veröffentlichen. Veröffentlicht wird der unveränderte geprüfte Build-47-Paketsatz unter **v1.0.2-build47**; ältere Releases bleiben erhalten. Die bisherigen Aussagen über noch fehlende Veröffentlichungsfreigaben beschreiben den damaligen Paketierungsstand und sind durch diesen Auftrag abgelöst. Kein bisher nicht gemeldeter persönlicher Test wird dadurch als bestanden ergänzt. [Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md).

**Stand 31.10.2026: CAN 1.0.2 Build 47**, zugeordnet zum unmittelbar zuvor bereitgestellten Setup-Paket. Der Nutzer bestätigt in der VM ausdrücklich: **„setup bei MP4Box wird Audio-Enthusiast blockiert“**. Die neue Sperre ist damit auch im VM-Setup persönlich positiv geprüft.

## Fall 1: MP4Box-Teile mit QUICK AND COPY, AAC und AC-3

Das übermittelte Protokoll zeigt je Filmteil die bestätigte Zuordnung von **Spur 1 AAC** und **Spur 2 AC-3**, jeweils Deutsch, Stereo, 48000 Hz. FFmpeg übernimmt das Video und beide Tonspuren ohne Neukodierung mit `-map 0:0 -map 0:1 -map 0:2 -c copy`. Dateinamen der Eingaben enthält dieser Protokollauszug nicht; es wird kein neuer eigener MP4Box-Schnittlauf daraus abgeleitet.

Die spätere ausdrückliche Nutzerzusammenfassung ordnet diesen erfolgreichen QUICK-Weg MP4Box-Filmteilen zu. Die Zuordnung beruht auf der Rückmeldung, nicht auf Dateinamen im Protokoll.

| Phase | Zeit laut Protokoll |
|---|---:|
| Erste Eingabe prüfen | 0,0734900 s |
| Zweite Eingabe prüfen | 0,0540831 s |
| FFmpeg-Concat | 22,9306532 s |
| Ausgabe prüfen | 0,1121843 s |
| Ausgabe-SHA-256 | 4,3915923 s |
| Ausgabe übernehmen | 0,0005634 s |
| Technische Hinweise | 0,0067990 s |
| Bereinigung | 0,0023404 s |
| Gesamt | **27,8317790 s**, Anzeige **00:00:27.83** |

Alle protokollierten Phasen einschließlich Ausgabeübernahme und Bereinigung sind erfolgreich. Gemeldete FFmpeg-Grenzen: 1 GiB Arbeitsspeicher, 25 % CPU und ein Verarbeitungsthread.

## Hinweise im erfolgreichen Lauf

- FFmpeg meldet `track 2: codec frame size is not set`; die Meldung bleibt in der Meckerecke erhalten.
- Teil 1: Bild 7054,480000 s; beide Tonspuren 7054,440000 s. Teil 2: Bild 2471,880000 s; beide Tonspuren 2471,840000 s.
- Ausgabe: Bild 9526,360000 s, AAC 9526,357333 s, AC-3 9526,336000 s.
- Containerdauer: Summe der Teile 9526,360000 s, Ausgabe 9526,397333 s. QUICK enthält keine samplegenaue Tonprüfung.

Die Hinweise verhindern den Erfolg gemäß den geltenden QUICK-Regeln nicht. Der Nutzer bestätigt **„quick and cicopy check“** sowie **„Bild, ton , 3 Sekunden check“**. QUICK, Bild, Ton und Drei-Sekunden-Vorschau persönlich bestanden. Keine gesonderte Hörbewertung je Spur oder zusätzliche persönliche Dateisystemkontrolle aus dieser Rückmeldung abgeleitet.

## Zusätzlicher Dialogcheck: von MP4Box zu otr-can wechseln

Der Nutzer bestätigt mit zwei Screenshots **„bild 2 ist die Meldung weg :-)“**. Bild 1 zeigt Furiosa Teil 1/2 mit MP4Box im Dateinamen und den sichtbaren MP4Box-Sperrhinweis. Bild 2 zeigt stattdessen Furiosa Teil 1/2 mit otr-can im Dateinamen; der Hinweis ist verschwunden. Der Dialog aktualisiert den Herkunftshinweis beim Wechsel des Dateipaars damit auch in der VM wie vorgesehen.

Der AUDIO-ENTHUSIAST-Startknopf ist in Bild 2 weiterhin ausgegraut; die beiden eigenen Tonspurauswahlen dieses Verfahrens sind im gezeigten Scrollausschnitt nicht sichtbar. Der Start setzt zusätzlich deren Auswahl voraus. Bestätigt ist hier das Entfernen des MP4Box-Hinweises nach dem Dateitausch, kein neuer AUDIO-ENTHUSIAST-Verarbeitungslauf. Der bisherige QUICK-Erfolg wird nicht erneut gezählt.

## Fall 2: otr-can-Teile mit QUICK AND COPY, AAC und AC-3

Nach dem Screenshot-Wechsel zu Furiosa-otr-can-Teilen meldet der Nutzer einen weiteren erfolgreichen QUICK-Lauf und bestätigt ausdrücklich die Kombination **otr-can → QUICK AND COPY**. Das neue Protokoll enthält keine Eingabedateinamen; Herkunft und Fallzuordnung folgen der Nutzerzusammenfassung und der laufenden Prüfrunde.

Je Teil Spur 1 AAC und Spur 2 AC-3, jeweils Deutsch/Stereo/48000 Hz. Video und beide Spuren ohne Neukodierung übernommen. **Concat 35,5501070 s**, Ausgabeprüfung 0,2132881 s, Ausgabe-SHA-256 6,7337980 s, Bereinigung 0,0019884 s; **Gesamt 42,7975635 s**, Anzeige **00:00:42.79**. Alle protokollierten Phasen einschließlich Ausgabeübernahme und Bereinigung erfolgreich.

Meckerecke erhält die Meldung `track 2: codec frame size is not set` und eine Audio-DTS-Korrektur: vorher 329199616, aktuell 329199360, von FFmpeg auf 329199617 geändert. Ausgabedauern: Bild **8071,840000 s**, AAC **8071,861333 s**, AC-3 **8071,856000 s**. Container: Teilesumme **8071,840000 s**, Ausgabe **8071,872000 s**. Keine samplegenaue Tonprüfung im QUICK-Modus.

Der Nutzer bestätigt **„BIld, ton 3 sekunden vorschau check“**. Bild, Ton und Drei-Sekunden-Vorschau persönlich bestanden. Hinweise bleiben dokumentiert; daraus wird keine exakte Zeitstempel-/Sampleidentität abgeleitet.

## Fall 3: Furiosa-otr-can-Teile mit AUDIO-ENTHUSIAST

Angehängtes vollständiges Protokoll zu **Furiosa A Mad Max Saga Teil 1 otr-can.mp4** und **Teil 2 otr-can.mp4**, jeweils Spur 1 AAC/Deutsch/Stereo/48000 Hz. Video bleibt unverändert. Gewählte Tonspuren vollständig dekodiert, samplegenau per RF64 verbunden und mit beiden Ausgangsspuren verglichen; einmalige durchgehende AAC-LC-Kodierung bei 192 kbit/s.

Bild-/Quelltonzeitprüfungen, Kopiernachweise, finaler MP4-Zusammenbau und Prüfungen aller Bild-/AAC-Pakete sowie Wiedergabezeiten erfolgreich. Fertiger Ton vollständig auf exakte Samplezahl geprüft; Klebestelle mit drei Sekunden Vor- und Nachlauf dekodiert. **387448320 Tonsamples**, Klebestelle **01:54:18.320**, Gesamtdauer **02:14:31.840**.

| Phase | Zeit laut Protokoll |
|---|---:|
| RF64-Verbindung und PCM-Vergleich | 13,4502197 s |
| Einmalige AAC-Kodierung | 182,0762366 s |
| Finaler MP4-Zusammenbau | 28,4088457 s |
| Abschließender Bildnachweis | 23,4062740 s |
| Vollständige Dekodierung des fertigen Tons | 10,6109346 s |
| Dekodierung der Klebestelle | 0,9034477 s |
| Bereinigung | 0,3172077 s |
| Gesamt | **337,9512008 s**, Anzeige **00:05:37.95** |

Alle protokollierten Phasen einschließlich Ausgabeübernahme und Bereinigung erfolgreich. Nutzer bestätigt **„auch hier bild ton check“** und ausdrücklich den Weg **otr-can → AUDIO-ENTHUSIAST**. Bild und Ton persönlich bestanden; die Drei-Sekunden-Vorschau ist in dieser zweiten Bestätigung nicht nochmals ausdrücklich genannt. Ihre technische Klebestellendekodierung ist im Protokoll belegt.

## Vom Nutzer festgehaltene Verfahrenszuordnung

| Herkunft der Filmteile | QUICK AND COPY | AUDIO-ENTHUSIAST |
|---|---|---|
| MP4Box | Persönlich bestanden, Fall 1 | Bei erkannter GPAC-Kennung gesperrt; in der VM bestätigt |
| otr-can | Persönlich bestanden, Fall 2 | Persönlich bestanden, Fall 3 |

Dies sind drei eigene bestätigte Verarbeitungsläufe der Build-47-VM-Setuprunde und die zugehörigen Dialogchecks. Keine pauschale Zusage für beliebige Dateien oder vollständige VM-/Portable-Endabnahme.

## Abnahmestand

| Prüfung | Stand |
|---|---|
| MP4Box-Sperre für AUDIO-ENTHUSIAST im VM-Setup | Persönlich bestanden |
| Dateipaar von MP4Box zu otr-can wechseln | Hinweis bei MP4Box sichtbar, bei otr-can verschwunden; persönlich per Screenshot bestätigt |
| MP4Box → QUICK AND COPY mit AAC und AC-3 | Technisch erfolgreich; Bild/Ton/Drei-Sekunden-Vorschau persönlich bestanden |
| otr-can → QUICK AND COPY mit AAC und AC-3 | Technisch erfolgreich; Bild/Ton/Drei-Sekunden-Vorschau persönlich bestanden |
| otr-can → AUDIO-ENTHUSIAST mit AAC/AAC | Technisch erfolgreich; Bild und Ton persönlich bestanden |
| Weitere VM-Fälle und Endabnahme | Noch offen |
| Installationsvariante, Deinstallation und Portable-ZIP Build 47 | Nicht separat bestätigt |

Die VM-Setuprunde umfasst jetzt drei positive Verarbeitungsläufe sowie die Sperr-/Dateiwechselchecks; eine vollständige Paket-/VM-Endabnahme ist noch nicht gemeldet. Die frühere [Build-46-Runde](VM-SETUP-BUILD46-20261031.md) bleibt getrennt dokumentiert. Produktcode, Buildnummer 47 und schreibgeschützte Pakete unverändert; dieser Nachtrag ändert nur die Arbeitsdokumentation.

## Wechsel zur getrennten Portable-Prüfung

Nach drei positiven Verarbeitungswegen und den Dialogchecks meldet der Nutzer „jetzt zip“. Der erste John-Wick-MP4Box-/QUICK-Fall samt Bild-/Ton-/Drei-Sekunden-Kontrolle ist als eigener [Portable-Nachweis](VM-PORTABLE-BUILD47-20261031.md) dokumentiert. Keine Setup-Deinstallation oder pauschale Setup-Endabnahme aus dem Wechsel abgeleitet.

[Paketbericht](RELEASE-1.0.2-BUILD47-RESULT.md), [Testplan](TESTPLAN.md), [Projektstand](AKTUELLER-STAND.md).
