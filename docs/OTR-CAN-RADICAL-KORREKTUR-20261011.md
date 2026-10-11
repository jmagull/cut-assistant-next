# Radical – Audiofilter und stehengebliebene Fortschrittsanzeige

Stand: 11.10.2026. Korrektur nach dem persönlichen Torchwood-Test. CAN bleibt **0.2.1 · Build 11 · RC3**, regulärer Motor **0.1.4**.

## Befund

Der Nutzer meldet bei Radical eine unveränderte Anzeige nach 42 Sekunden. Der ursprüngliche Motor-/FFmpeg-Prozess ist bei der Untersuchung bereits beendet; CAN verarbeitet sein Protokoll weiter. Die Thread-Diagnose zeigt den UI-Thread in WPF-TextBox-Layouterstellung über ScrollToEnd innerhalb von TextChanged. Timer und Abschlussanzeige kommen dadurch nicht mehr zum Zug.

Der unabhängige Wiederholungslauf mit 0.1.3 schlägt in der PCM-Vorbereitung fehl. Das Warnprotokoll wächst während der Untersuchung von rund 44 auf rund 100 MB in PowerShells Textdarstellung; mehrere hunderttausend Non-monotonic-DTS-Meldungen. Die Warnungen wurden vom Motor korrekt als Fehler gewertet; die Oberfläche konnte den Fehler wegen der Protokollflut nicht mehr darstellen.

Im Original wechseln AC-3-Downmix-Begleitdaten an Programm-/Werbegrenzen, unter anderem bei 4397,632 und 4814,336 s. Erst später bei 8842,816 s wechselt das eigentliche Kanallayout von 5.1(side) zu Stereo, außerhalb der letzten Behaltegrenze 8376,780 s. AAC bleibt fltp/Stereo. FFmpegs Filtersteuerung wertet auch geänderte Downmix-Begleitdaten als Reinitialisierungsgrund; dabei verlieren die Behalte-/Concat-Filter ihren Zustand und Samplezähler.

Quellen: [FFmpeg zu Filter-Reinitialisierung und verlorenem Zustand](https://ffmpeg.org/ffmpeg.html#Main-options), [FFmpeg-Filtersteuerung: DOWNMIX_CHANGED / send_frame](https://github.com/FFmpeg/FFmpeg/blob/master/fftools/ffmpeg_filter.c). Keine FFmpeg-Quellen übernommen.

## Korrektur

**CAN:** Prozessmeldungen in einem threadgeschützten Puffer sammeln, bei UI-Ticks und vor Abschluss zusammengefasst übernehmen. Das sichtbare Protokoll zeigt einen begrenzten Ausschnitt von etwa 32 KiB, mit Hinweis auf die vollständige Kopie über In Zwischenablage. Der gesamte Inhalt bleibt für Kopieren und Abschlussprotokoll erhalten. Lange Fehlergründe im Status begrenzen, vollständigen Grund im Protokoll erhalten. Scrollen erst nach der Textänderung mit niedriger Dispatcher-Priorität ausführen und beim Schließen abbrechen.

**Motor:** Vor der PCM-Vorbereitung -reinit_filter:a 0 setzen, damit Samplezähler und noch wartende Keep-/Concat-Zweige über geänderte Begleitdaten erhalten bleiben. Originalton weiterhin ab Anfang mit vollständiger Dekodiervorgeschichte lesen, höchstens bis zur letzten benötigten Behaltegrenze. Diese Begrenzung allein war im ersten Versuch nicht ausreichend; der verworfene Lauf bleibt als Beleg erhalten.

Keine Pakete oder Samples über -drop_changed verwerfen, keine Zeitstempelwarnungen ausblenden und keine Ausgabe trotz fehlerhafter Verifikation akzeptieren. Audio-Codecs, Kanäle, Raten, Zielbitraten und die vollständige Neukodierung bleiben erhalten. Warnungen und Sample-/Format-/Timelineabweichungen stoppen weiter den Schnitt. Wechsel tatsächlicher Audioformate innerhalb benötigter Bereiche sind nicht allgemein freigegeben.

Neuer separater Build 0.1.4; 0.1.3 und alle älteren EXEs erhalten.

## Prüfungen

- Release-Build von CAN ohne Warnungen/Fehler; **753/753 CAN-Tests**. Drei zusätzliche Tests für große Warnungsblöcke, Reihenfolge, neue Meldungen während eines Flushes und begrenzte Anzeige bei vollständiger Protokollkopie.
- Echte isolierte WPF-Wiedergabe des gesicherten Fehlerprotokolls: **807910 Zeilen**, **50447542 Zeichen** vollständig erhalten, nur 32841 Zeichen sichtbar. Timer läuft, Fehler sichtbar, Schließen erreichbar; 15 Timer-Ticks in rund 2,28 s. Renderbild geprüft. Keine Behauptung, die bereits laufende alte Anwendung nachträglich repariert zu haben.
- **40/40 Rust-Tests**, Formatprüfung, Release-Build mit Compilerwarnungen als Fehler. Gleicher Abhängigkeitsbestand.
- Separater Korrekturmotor besteht erneut **24/24 nummerierte Bildfolgen** und beide Tonspuren: 25/50 fps, einzelne/späte/mehrere Keeps, 44,1/48 kHz, Stereo und sechs Kanäle.
- Radical im getrennten Versuch erfolgreich: **350454 Video-Pakete / 50 fps**, AAC-Stereo und AC-3 mit sechs Kanälen, je **336435840 decodierte Samples / 48 kHz**, durchgehende rohe Audio-PTS. Enddauer **7009,080 s / 1:56:49.080**, Übergang **57:26.860**. Mux-Pakete und Decoder-Konfigurationen stimmen mit den geprüften Einzelstufen überein.
- Unabhängige Auswahl der Original-Samplepositionen stimmt vor Kodierung bytegenau mit der PCM-Zusammenstellung überein: AAC **2691486720 Bytes**, AC-3 **8074460160 Bytes**. Alle sechs AC-3-Kanäle im Vergleich enthalten. Die unabhängige Original-AC-3-Dekodierung meldet weiterhin einen unvollständigen Frame; dieser Befund bleibt ausdrücklich dokumentiert. Beide behaltenen Samplefolgen sind dennoch exakt. Der eigentliche korrigierte PCM-Aufruf arbeitet ohne Warnungen.

Die ersten Diagnosehelfer-/Restoreversuche bleiben erhalten: acht bestehende Lockdateien verwenden unterschiedliche Runtime-Angaben für App- und Testprojekte. Der isolierte Build erhielt dafür nur eine lokale Test-Targets-Datei; keine Lockdateien zur Erzeugung eines grünen Ergebnisses geändert. Der erste unabhängige Sample-Prüfhelfer stoppte wegen der Original-AC-3-Warnung; diese Warnung im zweiten Beleg ausdrücklich erhalten und die Samplegleichheit weiterhin strikt geprüft.

## Regulärer CAN-Abschluss und Aktivierung

0.1.4 über CANs tatsächliche Motorfabrik/Schnittdienst geprüft: synthetischer Unicode-/Leerzeichenpfad mit zwei Tonspuren in 2,65 s und Radical in **247,08 s / rund 4:07 Minuten**, einschließlich genau einer Indexierung, Motorlauf und CAN-Ausgabeprüfung. Beide Aufträge erhalten Originale und Indexdaten und bereinigen den privaten Arbeitsbereich vollständig. Werkzeuge ausdrücklich übergeben, kein FFmpeg im Prozess-PATH.

Die gesamte reguläre Radical-MP4 ist bytegleich zum separat geprüften Korrekturversuch (dessen nativer Lauf ohne Indexierung 216,54 s dauerte). Einzelmessungen, keine allgemeine Laufzeitgarantie.

- reguläre Ausgabe: .build/radical-stall-20261011/regular-can-radical/Radical-common-engine.mp4
- Ausgabe-SHA-256: 0D20096E2C796B2BE8A8A20B1C5EDFC99CC5139675DC82BC33D8FF77FB79BB97
- reguläre Motor-EXE: target/can-engine-0.1.4-build/release/otr_can.exe im Rust-Repository
- Motor-SHA-256: CDC9E9E3317F8CC4E9512249F28399DE4CCDFCE73491C4DF109A8E5333CD8C2D

Nach diesen Prüfungen den konfigurierbaren CAN-Pfad über CANs eigene Einstellungsverwaltung auf 0.1.4 gestellt. Vorherigen Pfad lokal gesichert, FFMS2-Pfad erhalten. Alte 0.1.3-EXE per Prüfsumme unverändert. CAN im bisherigen Ausgabeordner gebaut und erneut 753 Tests bestanden. Finale WPF-Wiedergabe des realen Warnprotokolls erneut bestanden: 15 Timer-Ticks in rund 2,22 s, Fehler und Schließen erreichbar, gesamter Text erhalten. Keine neue Pflichtabhängigkeit, keine Änderungen an Originalen, Cutlists, MP4Box oder CAN-Version/Buildnummer.

## Direkter Nutzerlauf mit 0.1.4

Am 11.10.2026 übermittelt der Nutzer einen erfolgreichen neuen Schnitt aus CAN im eigenen Zielordner: Radical - Eine Klasse fuer sich otr-can [08.10.2026].mp4. Das Protokoll nennt Motor 0.1.4, genau eine FFMS2-Indexierung, 350454 Video-Pakete, beide Tonspuren (AAC-Stereo und AC-3/6 Kanäle) mit jeweils 336435840 decodierten Samples / 48 kHz und durchgehenden rohen PTS sowie bestandene Mux- und CAN-Ausgabeprüfung. Übergang 57:26.860, Ende 1:56:49.080.

Nach der persönlichen Videoprüfung bestätigt der Nutzer: **„übergang ist sauber, ton ist synchron“**. Zusätzlich nimmt er die Stereospur als lauter als die 5.1-Spur wahr. Keine Laufzeitmessung für diesen Nutzerlauf mitgeteilt. Zusätzlich wünscht er gespeicherte Fenstergrößen der Schnittdialoge, besonders des Fortschritts-/Protokollfensters; zunächst auf der [Merkliste](SETUP-MERKLISTE.md) vermerkt.

## Belege und nächste Kontrolle

Ignoriert unter .build/radical-stall-20261011: can-stacks.txt, native.stderr.txt, verworfener begrenzter Lauf, state-result.json, identity/RESULT.json, mixed/RESULT.json, SOURCE-PCM-COMPARISON.json, UI-RESULT.json und UI-replay.png, Build-/Testprotokolle sowie reguläre CAN-Aufträge und Abschlussbeleg. Große PCM-Referenzen sind ausdrücklich benannte Diagnosebelege, keine privaten Motor-Zwischenreste und kein Paketinhalt.

Die persönliche Prüfung des neuen direkten 0.1.4-Schnitts ist positiv zurückgemeldet: sauberer Übergang und synchroner Ton. Stereo klingt für den Nutzer lauter als 5.1. Der Nutzer bestätigt anschließend: **„ist im Orginal auch so :-)“**. Der wahrgenommene Unterschied bestand bereits vor dem Schnitt. Keine quantitative Pegelmessung vorgenommen.

Mögliche Einflüsse sind die unterschiedlichen Abmischungen sowie der 5.1-Downmix und Lautstärkemetadaten bei der Wiedergabe. AC-3 enthält unter anderem Dialnorm- und Dynamikangaben; diese können die Wiedergabelautstärke beeinflussen. Siehe [FFmpeg: AC-3-Metadaten](https://ffmpeg.org/ffmpeg-codecs.html#AC_002d3-Metadata) und [Decoderoptionen](https://ffmpeg.org/ffmpeg-codecs.html#AC_002d3-Decoder-Options). Keine konkrete Ursache allein aus dem gehörten Unterschied abgeleitet. Die vollständige ursprüngliche AC-3-Bitstream-Metadatenübernahme ist nicht zugesagt; die Audio-Neukodierung erhält Codec, Layout, Rate und Zielbitrate.

Der vorgeschlagene Originalvergleich ist durch die persönliche Rückmeldung ergänzt: Stereo klingt auch dort lauter als 5.1. Daraus ergibt sich kein Anlass für eine automatische Pegelanpassung. Keine automatische Lautstärkenormalisierung beauftragt oder umgesetzt. Kein Commit, Push oder Release.
