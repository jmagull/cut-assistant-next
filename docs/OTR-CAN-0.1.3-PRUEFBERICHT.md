# OTR-CAN 0.1.3 – Integration und Prüfbericht

Stand: 11.10.2026. Regulärer lokaler CPU-Motor **0.1.3**, CAN **0.2.1 · Build 11 · RC3**. Kein Installationspaket oder Release veröffentlicht.

## Entscheidung und Umsetzung

Der Nutzer hat nach positiver persönlicher Bild-/Tonprüfung ausdrücklich die vollständige Audio-Neukodierung gewählt: „ok dann machen wir die neukodierung beim audio, wir hatten ja zeit gewonnen“.

Die getrennt geprüfte Framekorrektur und Audio-Timeline sind zunächst in experiments/combined-engine zusammengeführt und anschließend als regulärer can-engine-Stand 0.1.3 übernommen worden. Video, Audio und abschließender MP4-Mux sind getrennte Module. Die bisherige CAN-CLI mit expliziten Werkzeugpfaden bleibt erhalten; normale Aufträge benötigen keinen Versuchsschalter.

- Video: halboffene Framebereiche, nachgewiesene IDR-Grenzen vor GOP-Kopie, exakte Paketanzahl, kurzer sicherer Seek für CPU-Kanten, erhaltene Präsentationszeiten und einheitliche CFR-Decodierzeiten. Offene GOPs ohne geeignetes IDR-Grenzenpaar werden innerhalb des Keeps neu kodiert.
- Audio: Original mit vollständiger Dekodiervorgeschichte einmal lesen, Keep-Samples je Spur als PCM zusammenfügen und **jede vollständige Tonspur einmal neu kodieren**. Codec, Kanäle/-layout, Samplerate und Zielbitrate bleiben erhalten. Das ist verlustbehaftet; insbesondere AACs tatsächliche Durchschnittsbitrate muss nicht exakt der Zielbitrate entsprechen.
- Mux: geprüfte Video-/Audiostufen kopieren, Präsentationsende einschließlich AC-3-Rahmenlänge und Padding erhalten; sämtliche finalen Pakete und Decoder-Konfigurationen gegen die Einzelstufen vergleichen. Warnungen werden nicht als sauberer Erfolg übergangen.
- CAN: Original, Keep-Segmente, Ziel, einmalige FFMS2-Indexierung, Abbruch und Bereinigung weiterhin selbst verwalten. MP4Box bleibt Standard. Cutlists erhalten keine neue Pflichtabhängigkeit.

CAN übersetzt sechs neue Motorphasen in verständliche Meldungen für Video, Originalton, Audio-Neukodierung, Zusammenführen und Prüfung. Die Originalmeldungen bleiben im Protokoll. Prozentwerte gibt es weiterhin nur aus tatsächlich gemessener Indexierung; für lange Decoder-/Encoderphasen wird keine Restzeit oder Gesamtprozentzahl erfunden.

## Automatisierte und echte Prüfungen

**40/40 Rust-Tests** und **750/750 CAN-Tests bestanden**. Rust formatiert und als separater Release-Build gebaut; CAN Release-Build ohne Warnungen oder Fehler. Keine neuen Produktionsabhängigkeiten, acht CAN-Lockdateien gegen den Aufgabenbeginn unverändert.

Der gemeinsame Versuch besteht 16 ursprüngliche nummerierte Bildfälle mit/ohne B-Frames sowie acht weitere Fälle mit 25/50 fps, einzelnen Bildern, späten und mehrfachen Keeps: **24 exakte Bildfolgen**. Jeweils beide Tonspuren mit korrekter decodierter Samplezahl und kontinuierlichen rohen PTS; zusätzlich Stereo-AAC mit 44,1 kHz und AC-3 mit sechs Kanälen/48 kHz. Die vorherigen getrennten Video-/Audio-Nachweise bleiben in ihren Berichten erhalten.

Der reguläre Motor wurde über CANs tatsächliche Motorfabrik und Schnittdienst geprüft: Werkzeug-Vorprüfung, genau ein Indexer- und ein Motoraufruf, unveränderte Originale und Indizes, endgültige Ausgabeprüfung und vollständiges Aufräumen des privaten Arbeitsbereichs. Der synthetische Auftrag verwendet Leerzeichen/Umlaute im Dateipfad und einen Prozess-PATH ohne FFmpeg; alle Werkzeuge werden ausdrücklich übergeben. Die neuen Audio-/Mux-Meldungen erscheinen als übersetzter Fortschritt.

Ein zusätzlicher regulärer Vergleich mit und ohne **--verify-video** liefert bytegleiche MP4-Dateien. Im Normalmodus werden alle Video-Pakete/PTS/DTS, vollständig decodierte Audio-Samples/PTS und sämtliche finalen Mux-Pakete geprüft. Der optionale Schalter dekodiert zusätzlich das gesamte Video zur Frameprüfung; seine Laufzeit ist nicht der normale Schnittbenchmark.

## The Place Beyond the Pines

Regulärer Auftrag über CAN mit fünf Behaltebereichen, einschließlich Indexierung und CAN-Ausgabeprüfung: **213,24 Sekunden / rund 3:33 Minuten**. Der vorangehende gemeinsame Prototyp benötigte 206,26 Sekunden. Einzelmessungen auf diesem Rechner, keine allgemeine Laufzeitgarantie.

| Spur | Ergebnis |
|---|---|
| H.264 / 25 fps | 137625 Bilder / 5505,000 s |
| AAC-LC / Stereo / 48 kHz | 264240000 Samples / 5505,000 s |
| AC-3 / Stereo / 48 kHz | 264240000 Samples / 5505,000 s |

Die reguläre CAN-Ausgabe ist **vollständig bytegleich** zur persönlich geprüften gemeinsamen Pines-Testdatei v3. Die positive Rückmeldung „bild und ton passen einwandfrei zusammen“ gilt damit für denselben Dateiinhalt. Keine ausdrücklich getrennte Hörabnahme jeder Tonspur oder allgemeine Releasefreigabe daraus abgeleitet.

Übergänge: **18:16.880 · 30:49.080 · 49:25.520 · 1:21:47.200**; Ende **1:31:45.000**.

SHA-256 der regulären Ausgabe und der persönlich geprüften Testdatei:
A1467DDBB5D9B90C5FF3B6D9B3DB745ABF73D8928756E57AF6C2E29DBCBE3111

Die früheren knapp 15 Minuten sind eine Nutzerrückmeldung zum alten Motor, kein kontrollierter Direktvergleich. Gegenüber 0.1.2 kommt die vollständige Audio-Neukodierung und Prüfung hinzu; dessen früherer 57-Sekunden-Lauf enthielt die inzwischen nachgewiesenen Frame-/Zeitstempelfehler.

## Torchwood / Vergiftet – persönlicher HQ-Praxistest

Am 11.10.2026 vom Nutzer direkt über CAN mit regulärem Motor 0.1.3 geschnitten: Torchwood__Vergiftet_26.09.28_23-05_tele5_75_TVOON_DE.HQ.mp4, drei Behaltebereiche. Das übermittelte Abschlussprotokoll zeigt genau eine FFMS2-Indexierung, erfolgreichen nativen Schnitt, Video-/Audio-/Mux-Prüfungen und abschließende CAN-Ausgabeprüfung.

Laut Protokoll **77956 Video-Pakete bei 25 fps** und eine **AAC-Stereospur, 48 kHz, 149675520 decodierte Samples**, Zielbitrate 164300 bit/s. Video- und Audiolänge entsprechen beide **3118,240 s / 51:58.240**. Berechnete Übergänge: **15:03.640** und **36:12.280**.

Nach persönlicher Prüfung bestätigt der Nutzer: **„kein Blitzer, ton ist top :-)“**. Damit ist ein weiterer realer HQ-Schnitt mit einer Tonspur positiv bewertet. Diese Angaben beruhen auf dem vom Nutzer übermittelten Protokoll und seiner Sicht-/Hörprüfung; keine neue unabhängige Datei-/Hashprüfung und keine Laufzeitmessung vorgenommen. Dieser Film ersetzt keine getrennte Mehrtonspurprüfung.

## Aktivierung, Erhaltung und Grenzen

CANs lokaler konfigurierbarer Motorpfad ist nach den Prüfungen über seine eigene Einstellungsverwaltung auf target/can-engine-0.1.3-build/release/otr_can.exe im Rust-Projekt gestellt. Der bisherige Pfad ist lokal gesichert, der FFMS2-Pfad unverändert. Keine absoluten Benutzerpfade in den Produktionsquellen. Originale, Cutlists, frühere Referenz-/Versuchs-EXEs und ursprünglicher OTR-Quellbaum erhalten. Git-Historie, Lizenzen und Danksagung bleiben erhalten; kein Commit, Push oder Branchwechsel.

SHA-256:

- reguläre 0.1.3-EXE: 45899131798BCADBD24623AD68E2C468E3D8CBF177540415A53780D899697E8B
- erhaltene 0.1.2-EXE: 75DE8B5B8369EE7AB3B20409FDEA94772E4D8FF8A7EA889C89FE1627AA3FEDA6

Der neue Motor ist bewusst begrenzt auf eine Videospur an Position 0, AVC/H.264 mit vier Byte NAL-Längen, 25/50-fps-CFR und Quellstart null sowie AAC-LC/AC-3. Zugelassen sind 32/44,1/48 kHz; tatsächlich synthetisch geprüft wurden 44,1/48 kHz. Nicht unterstützte Streams, Raten oder Zeitstrukturen werden abgelehnt. Keine pauschale Reparatur beschädigter Quellen und keine verlustfreie ursprüngliche Tonübernahme.

Vor Version 1.0 folgen weitere reale Dateiformate und getrennte Tonspurkontrollen sowie Paket-/Lizenz- und Installationsabnahme. Die bisherigen Navy-/Kimi-Referenzen belegen frühere Motorstände und ersetzen keinen neuen 0.1.3-Praxistest.

## Lokale Belege

Ignoriert unter .build/combined-engine-20261011: identity/RESULT.json, mixed/RESULT.json, pilot/MODE-COMPARISON.json, stable-can-synthetic/RESULT.json und stable-can-pines/RESULT.json, Originalprotokolle, übersetzte Fortschrittsmeldungen, Einstellungsbackup und Abschlussbeleg. Prüfhelfer/Versuchsdateien werden nicht Teil eines CAN-Pakets.

Ergänzende Herkunft der Korrekturen: [Frame-Diagnose](OTR-CAN-FRAME-DIAGNOSE-20261011.md), [Frame-Korrektur](OTR-CAN-FRAME-KORREKTUR-20261011.md), [Audio-Prüfung](OTR-CAN-AUDIO-PRUEFBERICHT-20261011.md).
