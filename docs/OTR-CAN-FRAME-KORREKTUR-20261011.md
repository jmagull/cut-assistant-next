# OTR-CAN – getrennte Video-Korrektur des Framefehlers

Stand: 11.10.2026. Auftrag: begrenzte Testvariante nach der [Frame-Diagnose](OTR-CAN-FRAME-DIAGNOSE-20261011.md). **Keine Übernahme in CAN. Diese Testausgaben enthalten ausschließlich Video, keinen Ton.**

## Ergebnis

Die fehlerhaften Bildfolgen sind im geprüften Video-Versuch korrigiert: **16/16 ursprüngliche Fälle**, zusätzlich **7/7 Bildfälle** mit 50 fps, offenen GOPs und Dateiende, jeweils mit exakter binärer Bildkennung und korrekten rohen Video-PTS. Ein versetzter Quellstart wird ausdrücklich abgelehnt. **31/31 Rust-Tests** bestehen; separater Release-Build und Formatprüfung ohne Compilerwarnungen oder Fehler.

Pines enthält jetzt **137.625 tatsächlich decodierte Bilder / 5505,000 Sekunden Video**, entsprechend der nominalen Cutlist. Die bisherige Referenz meldet 137.626 Bilder. Der neue Versuch prüft sämtliche decodierten PTS gegen die fortlaufende 25-fps-Timeline und gespeicherte Paket-DTS auf strenges Steigen. Keine FFmpeg-Zeitstempelkorrektur oder Decoderwarnung akzeptiert.

Das Ergebnis gilt für den beschriebenen Video-Umfang. Audio, Lippen-Synchronität, allgemeine Container-/Codec-Unterstützung und eine Version-1.0-Freigabe sind damit nicht erledigt.

## Getrennte Implementierung

Neue Quellen ausschließlich im Rust-Repository unter experiments/frame-boundaries, abgeleitet von can-engine 0.1.2 / Git d7b77b3ce8d19e05a40ce0b6249f1019fa29015c. Bestehende Copyright-/MPL-Hinweise, Herkunft und reguläre Quellen erhalten. Eigene Cargo-Dateien, README, Lizenz und CLI-Tests. Keine neuen Abhängigkeiten.

Kernänderungen in der Testkopie: cutting/ffmpeg.rs, cutting/info.rs, main.rs und Cargo-Paket-/Binärkennung. Binärdatei target/frame-boundaries-build/release/otr_can_frame_test.exe, Kennung 0.1.2-frame.1. Der Aufruf verlangt ausdrücklich **--video-only**; ein regulärer CAN-Aufruf wird dadurch abgelehnt.

Der reguläre Motor 0.1.2 bleibt unter seinem bisherigen Pfad aktiv. Keine C#-Änderung, keine Einstellungen verändert, keine bestehende Referenz-EXE oder Filmausgabe ersetzt.

## Was korrigiert wurde

- Durchgehend halboffene Bereiche [Start, Ende). Kein pauschales minus/plus eins; auch ein einzelnes Bild vor einem Keyframe bleibt gültig. Ein Keyframe genau am ausgeschlossenen Ende darf eine Kopie beenden, aber kein neues nichtleeres Teil innerhalb des Keeps beginnen.
- Video-Mittelstücke nur zwischen zwei tatsächlich im AVC-Paket nachgewiesenen IDR-Grenzen kopieren. Ein Keyframe-Flag allein genügt nicht. Ohne solches Paar wird der Keep im Versuch vollständig kodiert; offene GOPs werden nicht blind kopiert.
- Kopie über exakte Video-Paketanzahl begrenzen. Zusätzlich Präsentationszeiten und Paketanzahl jedes Teilstücks prüfen. Die Aussage gilt für die unterstützten H.264-Konstellationen, nicht als allgemeine Gleichsetzung von Paketen und Bildern für alle Codecs.
- Auch den späten Video-Vorlauf auf einen nachgewiesenen IDR legen. Falls erforderlich vom Anfang dekodieren. Der erste Open-GOP-Versuch mit bloßem 30-Sekunden-Seek erzeugte eine Decoderwarnung und wurde verworfen.
- Ausschließlich Video-Teilstücke mit expliziten CFR-Dauern zusammenfügen. PTS ausdrücklich erhalten; DTS in Paket-Dekodierreihenfolge mit ausreichend Vorlauf berechnen. Danach echte Dekodierung prüfen. Bildinhalte und B-Frame-Präsentationsreihenfolge werden nicht durch neu erfundene PTS kaschiert.
- Erfolgreiche FFmpeg-Prozesse mit Warnungen im Video-Versuch ablehnen, Warntext protokollieren. Eine erste eigene setts-Erprobung ohne ausdrückliches pts=PTS verlor korrekte Präsentationszeiten; verworfen, Protokolle erhalten. Die verwendete Fassung erhält PTS explizit.

Hintergrund: [FFmpeg -frames](https://ffmpeg.org/ffmpeg.html#Main-options), [setts](https://ffmpeg.org/ffmpeg-bitstream-filters.html#setts), [Concat-Dauern](https://ffmpeg.org/ffmpeg-formats.html#concat). Der Korrektheitsnachweis beruht auf gemessenen Bildkennungen und Zeitstempeln, nicht allein auf Optionsbeschreibungen.

Der Versuch unterstützt derzeit genau eine Videospur an Streamposition 0, AVC/H.264 mit vier Byte NAL-Längen, 25/50 fps CFR und Quellstart null. VFR, andere Bildraten und versetzte Quellstarts werden abgelehnt. Kein allgemeiner VFR-/AVI-Fix behauptet.

## Pines gegen Original und Cutlist

| Keep | Originalframes [Start, Ende) | Bilder |
|---|---|---:|
| 1 | [37733,65155) | 27422 |
| 2 | [82386,101191) | 18805 |
| 3 | [116788,144699) | 27911 |
| 4 | [151303,199845) | 48542 |
| 5 | [217004,231949) | 14945 |
| Summe | | 137625 |

Zusätzlicher Bildvergleich: je drei Bilder an Anfang und Ende der fünf Keeps sowie je sechs Bilder um die neun internen CPU-/Copy-Übergänge, insgesamt **84 Vergleiche**. Vergleich nach echter Dekodierung als 128 × 72 Graubild, jeweils gegen das beabsichtigte Originalbild und Nachbarbilder ±2. Alle beabsichtigten Bilder erfüllen die Vergleichsgrenze; maximale mittlere absolute Abweichung 0,267 von 255.

**Grenze:** 39 Vergleiche sind wegen gleicher oder sehr ähnlicher benachbarter Bilder innerhalb der 0,05-Vergleichstoleranz nicht eindeutig. Deshalb kein Nachweis von 84 ausschließlich eindeutig identifizierten Realfilmframes behauptet. Die nummerierten synthetischen Tests liefern den eindeutigen Sequenznachweis. Der Realvergleich unterstützt die Zuordnung an den geprüften Stellen; keine persönliche Sichtabnahme ersetzt.

Pines-Prüflauf **352,9 Sekunden**, einschließlich vollständiger Dekodierung für die PTS-/Bildprüfung. Vorbereiteter Index wurde in einen neuen Versuch-Arbeitsordner kopiert. Kein neuer normaler CAN-Auftrag und kein mit den früheren 57,2 Sekunden vergleichbarer Schnittbenchmark.

Der volle Pines-Lauf verwendete den vor den zusätzlichen Stream-Eingangswächtern gebauten Testmotor. Nach deren Ergänzung: finale EXE erneut gegen alle 23 nummerierten Bildfälle und die Quellstart-Ablehnung geprüft; zusätzlich spätes Drei-Bilder-Pines-Stück erfolgreich. Video-Schnittalgorithmus dabei unverändert.

## Belege und Erhaltung

Ignorierter CAN-Ordner .build/frame-fix-20261011:

- identity-delivered/RESULT.json: 16 ursprüngliche Bildfälle mit finaler EXE.
- extra-delivered/RESULT.json: sieben weitere exakte Bildfolgen und abgelehnter Quellstart.
- pines/cut.log und RESULT.json: kompletter Film, Bild-/PTS-/DTS-Prüfung.
- pines/edges/RESULT.json und pines-edges.ps1: 84 Originalbildvergleiche.
- pines/final-binary-check.log: drei späte Bilder mit finaler EXE.
- build.log, tests.log, PRESERVATION.json und FINAL-RESULT.json.
- Erste verworfene Versuche und fehlerhafte eigene Prüfhilfen erhalten: anfänglicher Helfer-Präfix, falsch getrennte CLI-Keeps und korrigierte Build-Übertragung. Keine App-Fehler daraus abgeleitet.

SHA-256:

- finale Test-EXE: EF004BB22A601C92BD9B7CCCFB78F159245613DAF718D9F3095AD68031018A7C
- erhaltener Pines-Testmotor: 47A55A6A1BA13D1A9CD913BE3840F855BD9A94CF961AFC60E395DC501CB47B27
- Pines VIDEO-ONLY: A86D2FB7277F2D71DAD5214E80D89F68FA3A7DDF2B4748FC65AA2D664CBB1A09
- regulärer unveränderter 0.1.2-Motor: 75DE8B5B8369EE7AB3B20409FDEA94772E4D8FF8A7EA889C89FE1627AA3FEDA6

CAN bleibt **0.2.1 · Build 11 · RC3**. Produktionsquellen, acht CAN-Lockdateien und sämtliche vorbestehenden Rust-Dateien per Prüfsumme erhalten. Keine Commits, Pushes oder Branchwechsel. Kein neuer regulärer CAN-Build notwendig: C# unverändert; letzter regulärer Prüfstand 743 CAN-/25 Rust-Tests. Die 31 Tests betreffen ausschließlich das neue Experiment.

## Nächster Schritt

Audio getrennt untersuchen und korrigieren: PCM-/Sample-Zuordnung an den Schnittstellen, Encoder-Vorlauf und Padding, AAC-/AC-3-Paketgrenzen und Zusammenfügung gegen dieselben Keep-Zeiten prüfen. Alle Tonspuren, Kanäle, Sampleraten und Bitraten bewahren; über weitergehende Audio-Neukodierung anhand der Messergebnisse entscheiden. Danach Bild und Ton gemeinsam mit realen Filmen prüfen, persönlich abnehmen und erst dann bewusst in CAN übernehmen.

## Anschließende Integration

Nach getrennten Audio-Prüfungen und ausdrücklicher Nutzerwahl ist die Korrektur im gemeinsamen regulären Motor 0.1.3 übernommen. Aktueller Prüfstand: [0.1.3-Prüfbericht](OTR-CAN-0.1.3-PRUEFBERICHT.md). Dieser Bericht bewahrt die vorausgegangene isolierte Video-Prüfung.
