# Portable-ZIP Build 47 in der VM – persönliche Prüfnachweise

**Veröffentlichungsnachtrag 31.10.2026:** Der bestätigte Stand ist nach Nutzerfreigabe eingefroren, committet/gepusht und als [v1.0.2-build47 / Latest](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2-build47) veröffentlicht. Die folgenden datierten Prüf-/Auftragsstände bleiben als Historie erhalten; nicht separat gemeldete persönliche Testschritte werden dadurch nicht ergänzt. [Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md).

## Freigabe zum Einfrieren und Veröffentlichen am 31.10.2026

Der Nutzer beauftragt ausdrücklich, den bestätigten Stand einzufrieren, README/Dokumentation abzugleichen, zu committen, zu pushen und zu veröffentlichen. Veröffentlicht wird der unveränderte geprüfte Build-47-Paketsatz unter **v1.0.2-build47**; ältere Releases bleiben erhalten. Die bisherigen Aussagen über noch fehlende Veröffentlichungsfreigaben beschreiben den damaligen Paketierungsstand und sind durch diesen Auftrag abgelöst. Kein bisher nicht gemeldeter persönlicher Test wird dadurch als bestanden ergänzt. [Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md).

**Stand 31.10.2026: CAN 1.0.2 Build 47.** Nach der [Setup-Prüfung](VM-SETUP-BUILD47-20261031.md) kündigt der Nutzer ausdrücklich **„jetzt zip“** an. Der folgende Lauf wird als eigener Portable-Nachweis geführt. Eine vorausgegangene Setup-Deinstallation wird daraus nicht abgeleitet.

## Fall 1: John Wick Kapitel 4, MP4Box-Teile mit QUICK AND COPY

Der Screenshot zeigt **John Wick Kapitel 4 Teil 1 MP4Box.mp4** und **Teil 2 MP4Box.mp4** in der VM. Beide Tonspurpaare AAC und AC-3 sind ausgewählt. Der sichtbare Hinweis bestätigt die Erkennung: **„MP4Box-Filmteil erkannt. AUDIO-ENTHUSIAST steht dafür nicht zur Verfügung. Bitte QUICK AND COPY verwenden.“**

Das vollständige Erfolgsprotokoll zeigt jeweils Spur 1 AAC und Spur 2 AC-3, Deutsch, Stereo, 48000 Hz. Video und beide Tonspuren werden ohne Neukodierung kopiert: `-map 0:0 -map 0:1 -map 0:2 -c copy`. Ressourcenmeldung: 1 GiB Arbeitsspeicher, 25 % CPU und ein Verarbeitungsthread.

| Phase | Zeit laut Protokoll |
|---|---:|
| Erste Eingabe prüfen | 0,0826587 s |
| Zweite Eingabe prüfen | 0,0548258 s |
| FFmpeg-Concat | 23,2481585 s |
| Ausgabe prüfen | 0,2264563 s |
| Ausgabe-SHA-256 | 4,3739011 s |
| Ausgabe übernehmen | 0,0003947 s |
| Technische Hinweise | 0,0068581 s |
| Bereinigung | 0,0020160 s |
| Gesamt | **28,6188598 s**, Anzeige **00:00:28.61** |

Alle protokollierten Phasen sind erfolgreich, einschließlich Ausgabeübernahme und Bereinigung. Die Meckerecke bewahrt die Werkzeugmeldung `track 2: codec frame size is not set` und die Laufzeitunterschiede:

- Teil 1: Bild 7054,480000 s; beide Tonspuren 7054,440000 s.
- Teil 2: Bild 2471,880000 s; beide Tonspuren 2471,840000 s.
- Ausgabe: Bild 9526,360000 s, AAC 9526,357333 s, AC-3 9526,336000 s.
- Containerdauer: Teilesumme 9526,360000 s, Ausgabe 9526,397333 s. Keine samplegenaue Tonprüfung im QUICK-Modus.

Der Nutzer bestätigt **„Ton Bild 3 Sekunden check“**. Ton, Bild und Drei-Sekunden-Vorschau persönlich bestanden. Keine gesonderte Hörbewertung jeder einzelnen Spur oder zusätzliche persönliche Dateisystemkontrolle behauptet. Die vergleichbaren Laufzeitangaben der früheren Setup-Ausgabe ersetzen keinen Byteidentitätsnachweis; es ist ein eigener Portable-Lauf.

## Zusätzlicher Sperrcheck: Furiosa-MP4Box-Teile

Der neue Screenshot zeigt **Furiosa A Mad Max Saga Teil 1 MP4Box.mp4** und **Teil 2 MP4Box.mp4**. Die MP4Box-Sperrmeldung ist sichtbar und AUDIO-ENTHUSIAST deaktiviert. Nutzer bestätigt **„wird auch abgewiesen check“**. Die Sperre ist somit in der Portable-Prüfung für John Wick und Furiosa persönlich positiv gemeldet. Dies ist ein Dialogcheck, kein weiterer Verarbeitungslauf.

## Fall 2: Furiosa-otr-can-Teile mit QUICK AND COPY

Auf Nachfrage ordnet der Nutzer den neuen QUICK-Lauf ausdrücklich den **otr-can-Teilen** zu. Der daneben gezeigte MP4Box-Screenshot belegt den getrennten Sperrcheck, nicht die Eingaben dieses QUICK-Laufs.

Je Teil Spur 1 AAC und Spur 2 AC-3, Deutsch/Stereo/48000 Hz; Video und beide Tonspuren ohne Neukodierung übernommen. **Concat 35,6650356 s**, Ausgabeprüfung 0,0951567 s, Ausgabe-SHA-256 6,0266015 s, Bereinigung 0,0005867 s; **Gesamt 41,9317889 s**, Anzeige **00:00:41.93**. Alle protokollierten Phasen einschließlich Ausgabeübernahme und Bereinigung erfolgreich.

Meckerecke bewahrt `track 2: codec frame size is not set` und die Audio-DTS-Korrektur: vorher 329199616, aktuell 329199360, auf 329199617 geändert. Ausgabe: Bild **8071,840000 s**, AAC **8071,861333 s**, AC-3 **8071,856000 s**; Container-Teilesumme **8071,840000 s**, Ausgabe **8071,872000 s**. Keine samplegenaue Tonprüfung im QUICK-Modus.

Der Nutzer bestätigt **„ton bild schnitt 3 sekunden check“**. Ton, Bild, Schnitt/Klebestelle und Drei-Sekunden-Vorschau persönlich bestanden.

## Fall 3: Furiosa-otr-can-Teile mit AUDIO-ENTHUSIAST

Der neue Anhang enthält einen eigenen vollständigen Erfolgsbericht für **Furiosa A Mad Max Saga Teil 1 otr-can.mp4** und **Teil 2 otr-can.mp4**, jeweils Spur 1 AAC/Deutsch/Stereo/48000 Hz. Bild unverändert, gewählte Tonspuren vollständig dekodiert, samplegenau per RF64 verbunden und mit beiden Ausgangsspuren verglichen; einmalige durchgehende AAC-LC-Kodierung mit 192 kbit/s.

Bild-/Quelltonzeitprüfung, Kopiernachweise, finaler MP4-Zusammenbau und Prüfung aller Bild-/AAC-Pakete sowie Wiedergabezeiten erfolgreich. Fertiger Ton vollständig auf exakte Samplezahl geprüft, Klebestelle mit drei Sekunden Vor-/Nachlauf dekodiert: **387448320 Tonsamples**, Klebestelle **01:54:18.320**, Gesamtdauer **02:14:31.840**.

| Phase | Zeit laut Protokoll |
|---|---:|
| RF64-Verbindung und PCM-Vergleich | 12,2721169 s |
| Einmalige AAC-Kodierung | 183,0537596 s |
| Finaler MP4-Zusammenbau | 18,8726186 s |
| Abschließender Bildnachweis | 23,6498047 s |
| Vollständige Dekodierung des fertigen Tons | 10,6088421 s |
| Dekodierung der Klebestelle | 3,4445328 s |
| Bereinigung | 0,3242423 s |
| Gesamt | **326,3726217 s**, Anzeige **00:05:26.37** |

Alle protokollierten Phasen einschließlich Ausgabeübernahme und Bereinigung erfolgreich. Die ausdrücklich formulierte persönliche Ton-/Bild-/Schnitt-/Vorschau-Bestätigung steht beim QUICK-Protokoll; eine gesonderte persönliche AUDIO-ENTHUSIAST-Kontrolle ist in dieser Rückmeldung nicht ausdrücklich zugeordnet. Dieser Lauf wird daher als technisch erfolgreich dokumentiert. Keine Byteidentität mit den früheren Setup-Ausgaben behauptet.

## Abnahmestand

| Portable-Prüfung | Stand |
|---|---|
| MP4Box-Sperre für AUDIO-ENTHUSIAST | John Wick und Furiosa im Screenshot sichtbar, persönlich positiv |
| MP4Box → QUICK AND COPY mit AAC und AC-3 | Technisch erfolgreich; Ton/Bild/Drei-Sekunden-Vorschau persönlich bestanden |
| otr-can → QUICK AND COPY mit AAC und AC-3 | Technisch erfolgreich; Ton/Bild/Schnitt/Drei-Sekunden-Vorschau persönlich bestanden |
| otr-can → AUDIO-ENTHUSIAST mit AAC/AAC | Technisch erfolgreich; persönliche Kontrolle nicht separat zugeordnet |
| Weitere Portable-Fälle und ZIP-Endabnahme | Noch offen |

Drei eigene Portable-Verarbeitungsläufe sind dokumentiert, die beiden QUICK-Ausgaben ausdrücklich persönlich positiv geprüft. Die ZIP-Endabnahme ist noch nicht gemeldet. Setup- und Portable-Fälle bleiben getrennt. Produktcode, Buildnummer 47, schreibgeschützte Pakete und Prüfsummen unverändert; nur Arbeitsdokumentation ergänzt.

[Paketbericht](RELEASE-1.0.2-BUILD47-RESULT.md), [Testplan](TESTPLAN.md), [Projektstand](AKTUELLER-STAND.md).
