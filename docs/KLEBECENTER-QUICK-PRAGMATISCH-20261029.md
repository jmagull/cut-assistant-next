# Klebecenter – pragmatisches QUICK AND COPY, Build 46

**Stand 29.10.2026.** Lokale Entwicklung CAN **1.0.2.46**, Motor unverändert **otr-can 0.1.6**, libmpv **0.41.0**, .NET 10/WPF. Der nach den unten dokumentierten persönlichen Tests bestätigte Stand wird jetzt als Build 46 eingefroren und paketiert; Commit/Push sind danach beauftragt. Veröffentlichung erst nach neuer Ansage. [Paketprüfung](RELEASE-1.0.2-BUILD46-RESULT.md). Build 45 bleibt als ursprünglicher Paketsatz erhalten; öffentlich bleibt Build 41.

## Verbindlich korrigierter Auftrag

Die nachträgliche Nutzeranweisung ersetzt die strengen QUICK-Abbruchregeln des vorherigen MUCH-MORE-Auftrags ausdrücklich. QUICK ist pragmatisches FFmpeg-Concat mit `-c copy`. Abweichende H.264-Decoderparameter, bereits vorhandene oder kleine Zeitstempelunregelmäßigkeiten, zusätzliche Audiosamples und FFmpeg-Warnungen sind allein keine Abbruchgründe. Die frühere Rücksprache-Sperre aus der Referenzuntersuchung ist damit aufgehoben; ihre Messungen bleiben historische Diagnose.

AUDIO-ENTHUSIAST bleibt unverändert: genau eine gewählte Spur je Teil, strenge Prüfungen, direkte RF64-Zusammensetzung, einmalige AAC-LC-Neukodierung, Phasenzeiten. Beide Verfahren kopieren das Bild; inkompatible Videostreams werden nicht durch Neukodierung repariert.

## Ablauf und Schutz

- Zwei MP4-Dateien und ihre Reihenfolge wählen. Alle gewünschten kompatiblen AAC-/AC-3-Paare sichtbar zuordnen und bestätigen. Keine doppelte Zuordnung, keine erfundene Ersatzspur, kein stiller Spurverlust.
- Kurze ffprobe-Abfrage der Medienköpfe: MP4, eine H.264-Videospur, lesbare positive Laufzeit, passende Bildabmessungen/Pixelformate sowie tatsächlich vorhandene und kompatible gewählte Tonspuren. Unterschiedliche H.264-Konfigurationshashes sind Hinweise.
- Bei bereits gleicher Streamanordnung direkt die beiden Originale mit FFmpeg concat und `-c copy` verbinden. Nur bei unterschiedlicher Streamanordnung/Zuordnung je Teil eine Streamcopy-Arbeitsdatei mit den gewählten Spuren vorbereiten. Kein Encoder, keine PCM-Datei, keine automatische Umstellung des Verfahrens.
- FFmpeg muss erfolgreich enden. Die neue Ausgabe muss vorhanden, nicht leer und per Medienkopf lesbar sein und genau die gewählten Bild-/Tonspuren enthalten. Prozessabbruch, echte Lese-/Schreibfehler oder falsche Zuordnung bleiben Fehler.
- Originale über reine Lesehandles gegen Änderungen schützen, neue Ausgabe niemals überschreiben. Veröffentlichung der Arbeitsdatei erst nach Mindestprüfung und weiter bestehender Dateiidentität für den Kontrollplayer. Die SHA-256-Dateiidentität schützt die spätere Kontrolle, beweist keine Medienqualität.
- Keine Vollton-/Vollvideodekodierung, keine PCM-Identitätsprüfung, keine vollständigen Paket-/PTS-Scans und keine 3072-Sample-/80-ms-Abbruchgrenze im QUICK-Produktweg. Die früheren strengen Prüfroutinen samt Tests bleiben erhalten, werden hier aber nicht mehr aufgerufen.
- 1 GiB/25 % CPU und ein FFmpeg-Prozess nacheinander, Abbruch, Phasenzeiten und bisherige Bereinigung bleiben: Erfolg räumt bekannte Arbeitsdateien auf; Fehler/Abbruch behält `protocol.txt`, fremde Dateien werden nicht gelöscht.

## Meckerecke – technische Hinweise

Ein eigener Abschnitt im vorhandenen Protokoll sammelt alle vorliegenden nicht kritischen Hinweise: abweichende nominale/mittlere Bildrate, unterschiedliche Stream-/Containerlaufzeiten, H.264-Decoderparameter und sämtliche FFmpeg-/ffprobe-Meldungen, einschließlich Zeitstempelkorrekturen. Eventuell bekannte Sampleabweichungen können dort stehen; zusätzliche Samplemessungen werden dafür nicht gestartet. Warnungen bleiben Hinweise, unabhängig von einer früheren Warnungs-Allowlist.

Während des Laufs erscheinen Werkzeugmeldungen wie bisher; am Ende werden sie zusätzlich gesammelt in der Meckerecke ausgegeben. Ein temporärer Spool verhindert eine zusätzliche unbegrenzt wachsende Warnungsliste im Arbeitsspeicher. Das vorhandene vollständige UI-Protokoll bleibt erhalten; die kurze Liveanzeige darf weiterhin gekürzt sein. Nach Erfolg ist das Protokoll im Abschlussdialog einsehbar/kopierbar; nach Fehler liegt es zusätzlich im Arbeitsordner.

Ergebnis sinngemäß: **QUICK AND COPY abgeschlossen; Video und ausgewählte Tonspuren ohne Neukodierung übernommen. Technische Hinweise stehen gegebenenfalls in der Meckerecke. Bitte den fertigen Film, insbesondere die Klebestelle, kontrollieren.** Keine Zusage einer sampleidentischen Tonwellenform oder wissenschaftlichen Qualitätsprüfung.

## Kontrolle

Anfang, Übergang und Ende bleiben über „Prüfen“ erreichbar; die Vorschau startet drei Sekunden vorher, bei Mehrspur auf jeder erhaltenen Spur. Die QUICK-Übergangsposition stammt aus der von concat verwendeten Containerlaufzeit des ersten Teils und ist ausdrücklich **näherungsweise**, keine durch vollständige Paketprüfung nachgewiesene Naht. Die Ausgabe-Laufzeit wird gelesen. Liegen unbrauchbare Grenzangaben vor, wird die Näherung im Protokoll erläutert. AUDIO-ENTHUSIAST behält seine bisherigen nachgewiesenen Zeiten.

## Prüfung

- Release-Rebuild über `tools/build.ps1`: **Build 46**, **0 Warnungen/0 Fehler**.
- **1576 Tests bestanden**: 200 Core, 72 Cutlists, 375 Media, 929 App; nichts übersprungen. 24 neue Fälle für pragmatische Akzeptanz, lesbare Ausgabe, Mehrspurzuordnung, Meckerecke, Abbruch/Dateischutz. Ein erster Test fand einen Windows-Dateifreigabefehler beim Lesen des Warnungsspools; Writer wird jetzt vor der Übernahme geschlossen, finale Fälle positiv.
- **27 native QUICK-Fälle** mit kleinen synthetischen MP4s bestanden: unter anderem automatische AAC+AC-3-Erhaltung in beiden Reihenfolgen, explizite einzelne Spur/andere Streamposition, beschädigte Quelle ohne obligatorische Vollprüfung, Zeitbasiswechsel, unbekannte/DTS-Warnung, Mono/44,1 kHz/5.1/50 fps. Negative Fälle prüfen fehlende/falsche Spuren, inkompatible Bildabmessungen, doppelte Zuordnung, fehlende Bestätigung, Prozessfehler, leere/unlesbare Ausgabe, Abbruch und Überschreibrennen. Originale und Bereinigung geprüft.
- **21 bisherige native AUDIO-ENTHUSIAST-Fälle erneut bestanden; alle neun erfolgreichen Ausgaben bytegleich zu Build 45.** Dessen Dienst, RF64 und strenge gemeinsame Verifikation bleiben zur vor Auftrag gesicherten Quelle unverändert.
- Kompilierten WPF-Dialog geprüft: beide Spuren vorgeschlagen, Bestätigung erforderlich, Änderungen/Tausch widerrufen sie, keine Verfahrensvoreinstellung, entfallende John-Wick-AC-3-Spur sichtbar. Menü weiterhin ein Eintrag. Die native Testumgebung wurde mit den Produkt-Abhängigkeitsdateien gestartet, nachdem ihr isolierter erster Start eine fehlende Wrapper-Auflösung gemeldet hatte; keine Produktabhängigkeit geändert.

## Reale Produktläufe

Alle drei Fälle mit dem neuen Produktdienst, gleicher FFmpeg-Version und denselben Ressourcengrenzen nacheinander. Laufzeiten einschließlich kurzer Ein-/Ausgabeabfragen, gegebenenfalls Streamvorbereitung, Dateiidentität, Meckerecke und Bereinigung. Reine Concat-Zeiten sind separat ausgewiesen; nicht mit früheren Vollprüfungen gleichsetzen.

| Referenz | erhaltene Spuren | Concat | Produkt gesamt | gelesene Containerdauer | angenäherter Übergang |
|---|---|---:|---:|---:|---:|
| Furiosa MP4Box | AAC + AC-3 | 14.01 s | 20.28 s | 02:14:22.881750 | 01:54:08.841000 |
| Furiosa otr-can | AAC + AC-3 | 13.16 s | 19.58 s | 02:14:55.192000 | 01:54:41.640000 |
| John Wick 4 | nur AAC | 10.14 s | 21.93 s | 02:38:49.181333 | 01:57:37.120000 |

Verwendet wurden die vorhandenen `Furiosa  A Mad Max Saga  Teil 1/2 MP4Box.mp4`, `Furiosa  A Mad Max Saga  Teil 1/2 otr-can.mp4` (jeweils doppelte Leerzeichen im Originalnamen) sowie `John Wick Kapitel 4 Teil 1/2.mp4`. Diese lokalen Schnitte haben teils andere Längen als frühere Nutzerreferenzen; alte Nahtzeiten werden nicht übertragen. Bei John Wick ist die gemeinsame AAC-Spur ausdrücklich gewählt; die AC-3-Spur von Teil 1 entfällt bewusst. Medienkopf bestätigt jeweils H.264 und exakt diese Audiospuren. Originalgröße/Änderungszeit erhalten, erfolgreiche Arbeitsordner entfernt.

Furiosa MP4Box läuft trotz unterschiedlicher H.264-Decoderparameter und nicht idealer Bildzeitangaben erfolgreich. Die Meckerecke nennt diese vorhandenen Angaben und Laufzeitunterschiede. Keine neue vollständige Sample-/Videoanalyse nur für Hinweise gestartet. **Persönliche Bild-/Hör-/Übergangsabnahme dieser drei Build-46-Ausgaben steht aus.** Der bereits persönlich bestätigte manuelle MP4Box-Concat ist ein früherer getrennter Vorgang.

Lokale Prüfdaten: `.build/pragmatic-quick-copy-20261029/REAL-REFERENCES.json`, `QUICK-NATIVE-RESULTS.json`, `ENTHUSIAST-NATIVE.json`, Test-TRX und Protokolle. Keine echten Filmdateien in Git, keine neuen Abhängigkeiten. Die Entwicklungstests stammen direkt aus dem Entwicklungsordner. Der anschließende Auftrag zum Freeze/Paketieren sowie Commit/Push ist im Paketbericht getrennt dokumentiert.

## Persönlicher Build-46-Praxislauf bestätigt

Am 29.10.2026 meldet der Nutzer nach eigenem QUICK-Lauf: **„Bild, ton Klebestelle 3 sekunden vorschau check :-) 23 sekunden :-)“**. Bild, Ton, Klebestelle und Drei-Sekunden-Vorschau damit für diesen Lauf persönlich positiv bestätigt.

Das Protokoll zeigt beide gewählten deutschen Stereospuren bei 48000 Hz: **AAC und AC-3**, jeweils mit explizitem Mapping und `-c copy`. Concat **16,779 s**, Ausgabe-Dateiidentität **6,129 s**, Gesamtzeit **23,740 s**. Ausgabeabfrage, Übernahme und Bereinigung erfolgreich. Die Warnung `track 2: codec frame size is not set` sowie vorhandene Bildraten-, Decoderparameter- und Laufzeitunterschiede erscheinen in der Meckerecke, ohne den erfolgreichen Vorgang zu verhindern. Die gelesenen Laufzeiten entsprechen der oben beschriebenen Furiosa-MP4Box-Referenz.

Das zweimal eingefügte Protokoll ist identisch (gleicher Arbeitsordner und gleiche Phasenzeiten) und zählt als **ein** persönlich bestätigter Lauf. Keine gesonderte Hörbewertung je Tonspur aus der allgemeinen Tonbestätigung abgeleitet. Die drei automatisch erzeugten Entwicklungsreferenzen bleiben getrennte Dateien; keine pauschale Setup-/ZIP-/VM-Abnahme daraus abgeleitet. Nur Dokumentation aktualisiert; Code, Buildzähler 46 und eingefrorene Pakete unverändert.

## John Wick: neue MP4Box-Teile mit beiden Tonspuren persönlich bestätigt

Nach angekündigter Neuerzeugung beider Filmteile mit MP4Box meldet der Nutzer am 29.10.2026 einen weiteren erfolgreichen Build-46-QUICK-Lauf. Gespeicherte Ausgabe: **John Wick Kapitel 4 MP4Box - zusammengefügt.mp4**. Beide Teile haben laut Protokoll passende deutsche **AAC- und AC-3-Stereospuren bei 48000 Hz**; beide Spurpaare sind gewählt und per `-map 0:1 -map 0:2 -c copy` übernommen.

- FFmpeg-Concat: **12,190 s**.
- Dateiidentität: **3,973 s**.
- Gesamtzeit: **16,440 s**, in der Oberfläche als **00:00:16.43** formatiert.
- Berechnete Übergangsposition: **01:57:34.480**; gelesene Ausgabedauer: **02:38:46.397**.
- Ausgabeabfrage, Übernahme und Bereinigung erfolgreich. Vorhandene Bild-/Tonlaufzeitunterschiede und `track 2: codec frame size is not set` in der Meckerecke dokumentiert; kein Abbruch allein wegen dieser Hinweise.

Persönliche Rückmeldung: **„Ton bild, übergang klebestelle und 3 sekunden top“**. Damit sind Bild, Ton, Übergang/Klebestelle und Drei-Sekunden-Vorschau für diesen eigenen MP4Box-Mehrspurlauf positiv bestätigt. Die Rückmeldung schlüsselt die Hörkontrolle nicht je einzelner Spur auf. Dies ist ein weiterer eigener Lauf zusätzlich zum zuvor bestätigten Furiosa-/MP4Box-Fall mit 23,74 s.

Diese neu mit MP4Box geschnittenen Dateien sind getrennt von der älteren John-Wick-AAC-Notfall-/otr-can-Referenz zu betrachten. Deren abweichende Schnittlänge und frühere Einspurwahl werden nicht übertragen. Der Dialog bezeichnet die QUICK-Prüfstellen weiterhin korrekt als berechnet; die persönliche Kontrolle bestätigt den hier gehörten/gesehenen Übergang, ersetzt aber keinen technischen Sampleidentitätsnachweis. Nur Dokumentation aktualisiert, keine Code-/Build-/Paketänderung und kein Commit/Push/Release.
