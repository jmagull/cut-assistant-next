# Musikladen – isolierter AAC-Korrekturversuch

Stand: **19.10.2026**. Auf Nutzerauftrag getrennt von CAN 1.0.1 erprobt. **Technische Prüfung erfolgreich; persönliche Bild-/Tonabnahme noch offen.** Kein Motorwechsel in CAN, keine neue veröffentlichte Version.

## Ergebnis

Der vollständige Musikladen-Schnitt mit Keep **[1073.640000, 4589.480000)** ist im isolierten Versuch erfolgreich:

- **175792 Videopakete und vollständig dekodierte Videoframes**, rohe PTS/DTS und Präsentationsfolge geprüft; der Lauf erhielt ausdrücklich `--verify-video`.
- **168760320 dekodierte Audiosamples bei 48000 Hz**, Stereo, kontinuierliche rohe PTS und bestandene finale Paket-/Mux-/Decoderkonfigurationsprüfung.
- **34** anhand der Quellzeitstempel nachgewiesene Ein-Sample-Konflikte gezielt behandelt, verteilt innerhalb des Keeps.
- Nominale und samplegenaue Dauer: **3515.840 s = 00:58:35.840**.
- PCM vor Kodierung **bytegenau** zur unabhängig aus der vollständigen Originaldekodierung gewonnenen Referenzauswahl.
- Original, Referenzmotor 0.1.5 und CAN-1.0.1-Pakete erhalten.

[Testfilm](../.build/musikladen-overlap-20261019/real-musikladen/result.mp4): `852624236` Bytes; SHA-256 **`5d454657649a7ac1f925171f381999b278f49ff2f8db2dd9aefb629e9429e71d`**.

Dies ist ein neuer Testfilm im separaten Arbeitsordner. Keine vorhandene Filmausgabe ersetzt. Die verlustbehaftet neu kodierte AAC-Ausgabe wird nicht als PCM-bytegleich zum Original bezeichnet; der Bytevergleich betrifft ausdrücklich die ausgewählten PCM-Werte **vor** der Kodierung.

## Exakte Konfliktregel

Die [ursprüngliche Diagnose](OTR-CAN-MUSIKLADEN-PCM-DIAGNOSE-20261019.md) bleibt gültig. Der Decoder liefert an den betroffenen Stellen je 1024 Samples; der Container weist nur 1023 Sample-Ticks aus. Der folgende Frame startet deshalb einen Tick vor dem rechnerischen Ende seines Vorgängers. Die kollidierenden Tonwerte sind unterschiedlich.

Im Versuch gilt: **Der nächste Frame behält seinen Original-PTS und besitzt die dortige Zeitposition. Der letzte, über diesen PTS hinausreichende Samplevektor des vorherigen Frames entfällt.** Alle Kanalwerte dieser Position werden gemeinsam behandelt. Kein Resampling, keine Glättung und kein pauschales Kürzen am Ende.

Der Korrekturpfad wird ausschließlich nach einer zu großen PCM-Menge einer AAC-LC-Spur aktiv. Er verlangt:

1. Quellzeitbasis exakt **1/Samplerate**, bekannte Quellformate und vollständig abgedeckte Keeps.
2. Vorheriger Frame mit 1024 dekodierten Samples und ausgewiesener Dauer genau 1023; folgender Frame mit 1024 Samples, genau ein Tick Überlapp, passende Kanäle, Layout und Sampleformat.
3. Übereinstimmung der gesamten anhand der Quell-PTS ausgewählten Samplemenge mit der bereits erzeugten PCM-Datei.
4. Exakte Keep-Abdeckung nach den einzeln bewiesenen Konfliktbehandlungen; jede Entfernung muss die Gesamtdifferenz erklären. Keine positive Lücke und kein weiterer unbewiesener Konflikt innerhalb eines Keeps.
5. Korrigierte WAV/RF64-Datei mit exakter Größe, übernommenem Float-Format und unabhängig erneut geprüfter Samplezahl.
6. Anschließend alle bisherigen strengen Audio-/PTS-/Format-/Mux-Prüfungen unverändert.

Pro Konflikt werden Spur, Keep, vorheriger Quellframe, Quell-PTS und Position in der ursprünglichen PCM-Montage protokolliert. Bei Musikladen bleibt die geforderte Sollzahl **168760320** erhalten. Die ursprüngliche Auswahl enthält **168760354** Samples; ausschließlich die 34 belegten Vektoren entfallen.

Die bereits vor dem Keep liegenden Lücken von zusammen rund 224 ms werden nicht durch einen globalen Samplezähler oder eine Nullstart-Korrektur in den Keep verschoben. AAC-Priming und Endpadding bleiben außerhalb dieser Regel.

## Unabhängiger Musikladen-Nachweis

Eine zusätzliche vollständige Originaldekodierung erzeugt eine reine PCM-Referenz. Ein getrennter Prüfhelfer wählt daraus direkt anhand der dekodierten Original-PTS die Keep-Werte und begrenzt an den belegten 1023-Tick-Stellen die zeitliche Zuständigkeit des Vorgängerframes. Er verwendet **nicht** den Rust-Korrekturplan als Vorgabe.

- Unveränderte Motor-PCM-Montage stimmt bytegenau mit der ursprünglichen zeitbasierten Auswahl überein: **168760354** Samples.
- Neue PCM-Montage stimmt bytegenau mit der unabhängig normalisierten Auswahl überein: **168760320** Samples.
- Alle 34 vom Motor berechneten Einzelpositionen stimmen zusätzlich exakt mit den unabhängig ermittelten Positionen überein.
- SHA-256 der korrigierten PCM-Nutzdaten: **`d42eea486fc34d9eb676da03801165d4c5d51b8f82100e23b597f5d22aca62fc`**.
- Warnungsprotokoll der unabhängigen Originaldekodierung leer. Erfolgreicher Motorlauf ohne WARN/ERROR; finale native Verifikationen bestanden.
- Keine belastbare Laufzeitangabe: Der monotone Hilfszähler lieferte einen offensichtlich unplausiblen Rohwert. Dieser bleibt im JSON als Rohmessung erhalten und wird nicht als Sekundenmessung ausgegeben. Samplezahlen, Bytevergleiche und Prüfsummen sind davon unabhängig.

Original-SHA-256 vor/nach: **`5cd5ebe2b90b68a5ba809545ea65139b0006b516d60dd3f275b78c972bc21ab9`**. Indexdaten unverändert, privater Motorarbeitsbereich vollständig bereinigt. Die ausdrücklich gesicherten Auditdateien liegen außerhalb des privaten Arbeitsbereichs und sind beabsichtigte lokale Nachweise.

## Synthetische Prüfungen und Regressionen

**53/53 Rust-Tests** bestanden: 47 Bibliotheks- und 6 CLI-Tests. Die ursprünglichen 44 Tests bleiben erhalten; neun neue Testfunktionen enthalten die relevanten parametrisierten Grenzfälle. Formatcheck sowie separater Release-Build mit **Compilerwarnungen als Fehler** erfolgreich. Keine neuen Abhängigkeiten.

Die neuen Tests behandeln bekannte, voneinander verschiedene PCM-Werte, 32/44.1/48 kHz, 1/2/6/8 Kanäle, Keep-Längen bei 25/50 fps, mehrere Keeps, Einzelframe-Längen, Konflikte exakt an Start/Ende, unbenötigte Vorgeschichtslücken, falsche/fehlende PTS, größere und unbelegte Konflikte, Formatwechsel sowie RIFF/RF64 und abgeschnittene Daten. Die Kopierprüfung vergleicht sämtliche erhaltenen Kanalbytes gegen unabhängig erzeugte erwartete Werte.

**14/14 vollständige synthetische Medienfälle** bestanden:

| Fallgruppe | Nachweis |
|---|---|
| Sechs gesunde Referenzaufträge | Vollständige MP4-Ausgaben bytegleich zwischen unverändertem 0.1.5 und Versuch; MP3/AAC/AC3, 25/50 fps, 32/44.1/48 kHz, Stereo/Mono/Mehrkanal und gemischte Tonspuren enthalten. |
| Drei Überlapp-Aufträge | Stereo-AAC 48 kHz, Mehrkanal-AAC 32 kHz und AAC 44.1 kHz neben unverändert behandeltem MP3 und AC3. Jeder PCM-Kanal bytegenau unabhängig geprüft; jeweils zwei gezielt erzeugte Konflikte. Der bisherige Motor weist die getesteten reinen AAC-Konfliktdateien zurück. |
| Fünf Negativaufträge | Zwei-Sample-Überlapp, echte Lücke zwischen weiteren Überlappungen, AC3-Überlapp, verspäteter Tonbeginn und zu früher Tonabschluss weiterhin abgelehnt. Kein fertiges Ergebnis und keine privaten Motorreste. |

Alle positiven PCM-Montagen wurden mit einer separaten vollständigen Quellendekodierung abgeglichen. Video-/Audio-/Mux-Verifikation und Indexerhaltung werden vom vollständigen Motorlauf mitgeprüft. Die synthetischen Konflikte entstehen durch gezielte Änderungen der Audio-`stts`-Tabelle einer neu erzeugten Testdatei; die ursprünglichen Testquellen und reale Originalaufnahme bleiben unverändert.

## Code und Abgrenzung

Eigener Versuch: [experiments/audio-sample-overlap](../../otr-native-poc/experiments/audio-sample-overlap/README.md), eine Kopie des erhaltenen `can-engine`-Standes.

- `src/cutting/audio_overlap.rs`, Auswahlplan ab Zeile **36**, WAV/RF64-Kopie ab **242**: reine Integer-Auswahlregel, kontrollierte Samplekopie und neue Tests.
- `src/cutting/audio.rs`: bisheriger Normalpfad unverändert, eng begrenzter Fehlerpfad mit zusätzlicher Quellprüfung und strenger Gegenprüfung der korrigierten PCM-Datei.
- `src/cutting/mod.rs`: neues internes Modul anschließen.
- README des Experiments: Regel, Build, Herkunft und Grenzen. Keine Änderung an Videoverarbeitung, CLI-Vertrag, Produktionsabhängigkeiten oder Originalmotor.

Experiment-EXE: `otr-native-poc/target/audio-sample-overlap-20261019/release/otr_can.exe`.

SHA-256: **`9020ab8b0f4a112001379fccf952998776938091e051d06d00e454f84da69183`**.

Die Cargo-/CLI-Paketkennung bleibt im getrennten Versuch **0.1.5**; dies ist ausdrücklich **keine neue freigegebene 0.1.5-Ausgabe**. Der Korrekturpfad protokolliert `EXPERIMENTAL`. Kein Austausch oder Aktivieren des bisherigen Motors. Eine produktive Übernahme benötigt einen eigenen freigegebenen Motorstand und CAN-Integrationstest.

## Grenzen, Fehlversuche und nächster Schritt

Die Behandlung verwirft an jeder bewiesenen Stelle einen unterschiedlichen Samplevektor. Eine lokale Signalunstetigkeit ist damit nicht grundsätzlich ausgeschlossen. Die zeitliche Zuordnung und alle übrigen PCM-Werte sind technisch nachgewiesen; **persönliche Bild-/Tonkontrolle und akustische Abnahme sind noch offen**. Keine allgemeine Freigabe beliebiger Quellen, Audiocodecs oder FFmpeg-Versionen. Der Versuch benötigt die von den hier verwendeten Werkzeugen gelieferten vollständigen Quellframe-Felder einschließlich `duration`; fehlende Belege führen zum Fehler.

Die Quellzeitprüfung wird nur bei einer zu großen PCM-Menge aktiv. Der Versuch führt für sonst bereits akzeptierte Quellen keine allgemeine neue Eingangsanalyse ein und verspricht deshalb keine Erkennung aller denkbaren kompensierten Quellfehler im bisherigen Normalpfad. Bei einem Korrekturauftrag wird zusätzlich die Tonspur mit ffprobe dekodiert; zusätzliche PCM-Dateien benötigen Zeit und Speicher. Kein stummer allgemeiner Resampler und keine automatische Tonergänzung.

Der erste echte Konflikttest wurde nach korrekt erkannten Positionen wegen FFmpegs Warnung zur geschätzten Roh-PCM-Dauer abgelehnt. Die Roh-PCM-Eingabe wurde durch exaktes WAV/RF64 ersetzt; keine Warnung unterdrückt. Ein dabei aufgetretener Rust-Borrow-Fehler im neuen RF64-Testhelfer wurde korrigiert, ohne Tests zu entfernen. Der erste noch nicht angeschlossene Teststand meldete erwartungsgemäß ungenutzte Modulbestandteile; im angeschlossenen endgültigen Stand bestehen Test und Release-Build mit `-D warnings`.

Nächster Schritt ist die persönliche Kontrolle des Testfilms auf Bild, Ton und Anfang/Ende. Es gibt bei diesem einen Keep keinen inneren Bereichsübergang. Besonders können die Bereiche um die verteilten Konfliktstellen angehört werden; sie sind im vollständigen `PCM-PROOF.json` mit Originalzeitstempeln dokumentiert. Anschließend wird über eine produktive Übernahme entschieden.

## Erhaltung und Nachweise

**Alle 634 vor Beginn geschützten Dateien bytegleich**, einschließlich Originalaufnahme, ursprünglichem Motor und dessen Quellen, vier vorbestehenden lokalen Produktions-Lockänderungen, sonstigen lokalen CAN-Änderungen, beider Release-Paketsätze und Buildzähler **33**. Keine CAN-Produktdatei oder vorhandene Dokumentation geändert; neu ist ausschließlich dieser Bericht. Keine persönlichen Einstellungen geändert, kein Commit, Push, Tag oder Release.

Nachweise ausschließlich ignoriert unter `.build/musikladen-overlap-20261019/`: `BEFORE.json`, Test-/Build-/Formatprotokolle, `MATRIX.json`, `EXTRA.json`, individuelle Motorprotokolle und unabhängige `PCM-PROOF.json`, verworfene Erstversuche unter `matrix/`, erfolgreiche Fälle unter `matrix-v2/` und `extra/`, `real-musikladen/engine.log`, vollständiger Testfilm, PCM-Auditdateien und `FINAL.json`. Größere private PCM-/Mediennachweise werden nicht in Git oder CAN-Pakete aufgenommen.
