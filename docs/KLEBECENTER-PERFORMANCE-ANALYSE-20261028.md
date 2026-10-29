# Klebecenter – GPU- und Laufzeituntersuchung

**Stand: 28.10.2026. Untersuchte Produktversion: CAN 1.0.2 Build 41, otr-can 0.1.6.** Reiner Untersuchungsbericht; keine Produktimplementierung, kein Build, keine Paketierung, kein Commit/Push und keine Veröffentlichung. Der veröffentlichte Stand bleibt erhalten.

## 1. Ergebnis und Aussagegrenzen

**Für die vorhandene Pipeline ist keine sinnvolle GPU-Beschleunigung nachgewiesen.** NVENC/Quick Sync betreffen hier Video; CAN kodiert das Video beim Kleben nicht neu. Der tatsächlich verwendete AAC-Encoder läuft auf der CPU und meldet keine Encoder-Threadunterstützung. Die GPU würde weder PCM-Dateikopien noch die Paket-/Zeitstempelprüfungen ersetzen.

**Belegt sind beträchtliche AAC-Rechenarbeit und mehrfacher Datei-/Prüfaufwand.** Ein historischer John-Wick-Monitor misst 224,23 s für den damaligen gemeinsamen AAC-/Muxschritt. Aktuelle Einzelmessungen zeigen unter anderem 8,91 s für einen Videopaket-Hashdurchlauf und 11,34 s für einen Remux mit faststart der erhaltenen 2,82-GB-Ausgabe. Diese Werte sind keine vollständige Laufzeitzerlegung von Build 41.

**Ein kompletter aktueller John-Wick-Klebelauf ist nicht reproduzierbar:** Die dokumentierten beiden Ausgangsteile fehlen unter ihren bekannten Pfaden. Die fertige Ausgabe und die früheren Nachweise sind vorhanden. Deshalb sind historische Zeiten, aktuelle Einzelmessungen und noch ungetestete Optimierungen nachfolgend getrennt.

Empfehlung: Nach Freigabe zuerst ausschließlich eine Phasenzeitmessung am unveränderten Ablauf ergänzen. Erste anschließende Optimierung wäre die direkte, weiterhin vollständig geprüfte RF64-Zusammensetzung statt des Umwegs über joined.pcm. Kein ungeprüfter GPU- oder Encoderwechsel.

## 2. Konkrete Umgebung und GPU-Frage

### Tatsächlich verwendete Werkzeuge

Die lokale CAN-Einstellung zeigt auf **C:/tools/ffmpeg/bin/ffmpeg.exe** und das zugehörige ffprobe. Beide melden:

**2026-10-12-git-615b07170e-essentials_build-www.gyan.dev**.

Es handelt sich damit um den konkreten Git-Build und nicht um eine lediglich angenommene FFmpeg-Releaseversion. Vollständige Versions-/Buildausgabe, Encoderliste und EXE-Hashes liegen in den lokalen Messnachweisen.

Testrechner: Windows 11 Pro 10.0.26300, Intel Core i5-14500, 14 Kerne/20 logische Prozessoren, etwa 32 GiB RAM, NVIDIA GeForce RTX 2060. Eine FFmpeg-Encoderliste allein beweist weder passende Hardware noch einen aktiven Intel-Grafikadapter. Quick Sync wurde nicht als funktionierendes Gerät initialisiert; für die reine Audiopipeline wäre das ohne Nutzen.

### AAC und Hardware

Die verfügbaren AAC-Encoder sind **aac** und **aac_mf**. Ein AAC-Encoder für NVENC, CUDA oder Quick Sync ist in dieser Installation nicht vorhanden. Die vorhandenen h264/hevc/av1-NVENC-/QSV-/AMF-Einträge sind Videoencoder. NVIDIA beschreibt NVENC/NVDEC als Video-Encode-/Decode-Einheiten; FFmpeg dokumentiert die entsprechenden Videocodecs. [NVIDIA Video Codec SDK](https://developer.nvidia.com/video-codec-sdk), [FFmpeg-Encoder](https://www.ffmpeg.org/ffmpeg-codecs.html).

Der native Encoder meldet lokal **Threading capabilities: none**, **aac_coder = nmr** und **aac_nmr_speed = 0**. Die Hilfe bezeichnet höhere NMR-Speedwerte ausdrücklich als Tausch von Qualität gegen Geschwindigkeit. CAN setzt aktuell keinen Coder fest, sondern erbt diesen Werkzeugstandard. Mehr Encoderthreads sind hier kein belegter Beschleunigungsweg. Die Online-Dokumentation ist bei der Defaultangabe teilweise widersprüchlich; maßgeblich für diesen Befund sind die tatsächlich abgefragte EXE und deren archivierte Hilfe. Der aktuelle offizielle Quelltext führt ebenfalls AAC_CODER_NMR als Default. [AAC-Quelltext](https://www.ffmpeg.org/doxygen/trunk/aacenc_8c_source.html).

**aac_mf ist keine nachgewiesene GPU-Lösung.** Es ist die Media-Foundation-Anbindung; die lokale Encoderhilfe bietet s16 als Eingabeformat. Microsoft dokumentiert für seinen AAC-MFT 16-Bit-PCM. Gegenüber der heutigen Float-PCM-Verarbeitung wären also ein anderer Encoder, zusätzliche Formatumwandlung und eine Abhängigkeit vom Windows-Codecpfad zu prüfen. Die Projektleitplanke verlangt Unabhängigkeit von installierten Windows-Codecs. Keine Empfehlung für diesen Weg; keine zusätzliche Bibliothek oder Codecinstallation vorgenommen. [Microsoft AAC Encoder](https://learn.microsoft.com/en-us/windows/win32/medfound/aac-encoder).

GPU-Decodierung hätte im bestehenden Ablauf nur beim sechssekündigen Übergangsdecode überhaupt Videobilder zu bearbeiten. Die übrige Videoarbeit ist Streamcopy oder Paketprüfung. Der gemessene komplette Vorschauaufruf dauert 0,78 s; ein praktischer GPU-Vorteil dafür ist nicht belegt. Video-Neukodierung bleibt ausgeschlossen. [FFmpeg Streamcopy](https://ffmpeg.org/ffmpeg.html#Streamcopy).

## 3. Tatsächlich implementierter Ablauf

Untersucht: Mp4JoinService, FfprobeStreamCopyVerifier und dessen Join-Teil, BoundedJoinToolRunner, JoinDialog und MainWindow.Join.

### FFmpeg-Aufrufe des Dienstes

| Anzahl | Aufgabe | Wesentliche Optionen / Datei |
|---:|---|---|
| 2 | Gewählte Quelltonspur vollständig dekodieren | -map 0:AudioIndex, -c:a pcm_f32le, audio-0/1.pcm |
| 2 | Video je Teil unverändert vorbereiten | -copyts, -c:v copy, -an, vereinbarte video_track_timescale, video-0/1.mp4 |
| 1 | Verbundenen Ton einmal kodieren | joined.wav → -c:a aac -profile:a aac_low -b:a 192k/384k -threads:a 1 → continuous.m4a |
| 1 | Video und durchgehenden Ton muxen | ffconcat, -copyts, -auto_convert 0, -c copy, -avoid_negative_ts disabled, +faststart → result.mp4 |
| 1 | Fertige Tonspur vollständig nachdekodieren | result.mp4 → verified.pcm; exakte Dateilänge entspricht erwarteter Samplezahl |
| 1 | Klebestelle dekodieren | drei Sekunden davor, sechs Sekunden Gesamtlänge, Bild und Ton → null |

Das sind **acht FFmpeg-Prozesse pro erfolgreichem Dienstlauf**. Jede gewählte Eingabespur wird einmal vollständig dekodiert; anschließend wird die neu kodierte Gesamtausgabe einmal vollständig zur Samplezahlprüfung dekodiert. Hinzu kommt der kurze Bild-/Ton-Übergangsdecode. Keine vollständige Videodekodierung des Films.

Zwischen diesen Aufrufen führt C# die PCM-Zusammensetzung, SHA-256-Vergleiche, RF64-Verpackung und die abschließende vollständige Dateiidentitätsprüfung aus. Die fertige Datei wird erst nach allen Nachweisen ohne Überschreiben an den Zielpfad verschoben. Die Klebestelle ist die exakt geprüfte Dauer von Teil 1; die Paketprüfung weist den entsprechenden Zeitversatz im Ergebnis nach. Danach erfolgt die bekannte Erfolgsbereinigung.

### ffprobe und Eingabedurchläufe

Im Backend sind **27 ffprobe-Prozesse** statisch nachvollziehbar: 13 Metadatenabfragen und 14 vollständige Paketdurchläufe. Die normale Oberfläche fügt bei einmaliger Dateiauswahl und Bestätigung vier Metadatenabfragen hinzu. Wiederholte Auswahl kann weitere Aufrufe auslösen.

| Backend-Aufgabe | Metadatenabfragen | Paketdurchläufe |
|---|---:|---:|
| Beide Eingaben und fertiges Ergebnis inspizieren | 3 | 0 |
| Quellvideo-Zeitstruktur prüfen | 0 | 2 |
| Gewählte Quellton-Zeitachsen prüfen | 0 | 2 |
| Beide vorbereiteten Videos gegen Original prüfen | 4 | 4 |
| Fertiges Video gegen beide Originale prüfen | 3 | 3 |
| Durchgehendes AAC beim letzten Mux gegenprüfen | 2 | 2 |
| Fertige AAC-Zeitachse prüfen | 1 | 1 |
| **Summe** | **13** | **14** |

Pro Originalteil ergeben sich **sechs logische Medien-/Paketdurchläufe**: Videozeitprüfung, Tonzeitprüfung, Tondekodierung, Videokopie, Vergleich gegen vorbereitete Videokopie und finaler Videovergleich. Hinzu kommen drei Backend-Metadatenabfragen je Teil. Das bedeutet ausdrücklich nicht sechs identische physische Vollplattenlesungen: Streamauswahl, MP4-Seeks und Betriebssystemcache verändern die tatsächlichen Bytes und Datenträgerzugriffe. Genaues physisches Platten-I/O wurde nicht gemessen.

ReadAsync berechnet derzeit auch in reinen Zeitachsenprüfungen SHA-256-Pakethashes, obwohl dort nicht jeder dieser Werte verwendet wird. Weitere Ausgabescans prüfen AAC-Paketidentität und Kontinuität getrennt. Diese Stellen sind konkrete Kandidaten für eine zusammengefasste Auswertung mit unverändertem Prüfumfang.

### PCM-Dateioperationen

John Wick: N = 457399680 Samples je Kanal, zwei Kanäle, Float32. Eine vollständige PCM-Menge **B = N × 2 × 4 = 3659197440 Bytes = 3,408 GiB**.

| Schritt | PCM lesen | PCM schreiben |
|---|---:|---:|
| Zwei Quellspuren dekodieren | komprimierte Quelle | B |
| audio-0/1.pcm → joined.pcm, erwarteten Hash bilden | B | B |
| joined.pcm unabhängig vollständig nachhashen | B | – |
| joined.pcm → RF64 joined.wav kopieren | B | B |
| AAC-Kodierung liest joined.wav | B | komprimiertes AAC |
| Fertiges AAC → verified.pcm | komprimierte Ausgabe | B |
| **Unkomprimierter Nutzdatenverkehr** | **4B** | **4B** |

Das sind rund **14,637 GB PCM-Lesevolumen plus 14,637 GB PCM-Schreibvolumen**, zusammen **29,274 GB**. Video, komprimierte Audiodaten und kleine Header sind darin nicht enthalten. Gleichzeitig liegen normalerweise höchstens ungefähr 2B PCM vor, nicht alle vier Vollmengen; die Dateien werden schrittweise gelöscht.

### Grenzen und Serialisierung

Alle Schritte laufen im Dienst nacheinander. Zwei Quellenvergleiche können intern zwei ffprobe-Prozesse zugleich offen halten. FFmpeg hat pro Prozess einen eigenen Windows-Job mit 1 GiB Speicher und 25 % CPU-Hardcap sowie niedrigerer Priorität. Das ist **kein globales 25-%-Budget für CAN, ffprobe und sämtliche gleichzeitig laufenden Werkzeuge**. Mehr parallele Jobs würden die Last addieren. Codec-Threadoptionen begrenzen außerdem nicht automatisch jeden Hilfsthread des gesamten FFmpeg-Prozesses.

Quellcodebelege: [Dienst und Ablauf](https://github.com/jmagull/cut-assistant-next/blob/v1.0.2/src/CutAssistantNext.Media/Joining/Mp4JoinService.cs#L18), [PCM-Hash und RF64](https://github.com/jmagull/cut-assistant-next/blob/v1.0.2/src/CutAssistantNext.Media/Joining/Mp4JoinService.cs#L195), [Paketvergleiche](https://github.com/jmagull/cut-assistant-next/blob/v1.0.2/src/CutAssistantNext.Media/Analysis/FfprobeStreamCopyVerifier.cs#L17), [Zeitachsenprüfungen](https://github.com/jmagull/cut-assistant-next/blob/v1.0.2/src/CutAssistantNext.Media/Analysis/FfprobeStreamCopyVerifier.Join.cs#L78), [Ressourcengrenzen](https://github.com/jmagull/cut-assistant-next/blob/v1.0.2/src/CutAssistantNext.Media/Tools/BoundedJoinToolRunner.cs#L18).

## 4. Messwerte und ihre Bedeutung

### A. Historische Originalreferenz vom 26.10.2026

RESULT.json, PCM-PROOF.json, AAC-REMUX-PROOF.log und VERIFICATION-NOTES.md sind vorhanden. Ebenso die früheren Monitorverzeichnisse mit RESULT.json und Ressourcenmessungen.

| Historischer Schritt | Gemessene Zeit | Einschränkung |
|---|---:|---|
| Quellton → zusammengesetztes PCM/RF64 | 7,00 s | damaliger externer FFmpeg-Filtergraph, nicht die heutigen C#-Kopien |
| AAC kodieren und Video gemeinsam muxen | 224,23 s | enthält AAC, Videodurchlauf, Muxing und faststart; AAC-Zeit nicht isoliert |
| Korrigierender Streamcopy-Remux | 11,02 s | einmalige Korrektur der damaligen externen Referenz, kein zusätzlicher obligatorischer Build-41-Remux |

Alle drei Jobs hatten 25 % CPU-Hardcap. Der mittlere Job hatte 1536 MiB statt des heutigen 1024-MiB-Limits. Er benötigte etwa 255 CPU-Sekunden; der reine Remux etwa 16 CPU-Sekunden. Damit ist erhebliche zusätzliche Rechenarbeit im kombinierten Kodierschritt belegt. Die 224 Sekunden dürfen weder vollständig AAC zugerechnet noch zu einem Anteil an einer unbekannten Gesamtlaufzeit umgerechnet werden. Unüberwachte Prüf- und C#-Zeiten fehlen; der alte FFmpeg-EXE-Hash ist nicht in diesen Monitoren belegt.

### B. Aktuelle Einzelmessungen vom 28.10.2026

Verwendet wurde die erhaltene fertige John-Wick-Ausgabe mit **2820655094 Bytes**, 9529,16 s, 238229 Videopaketen und der dokumentierten Klebestelle bei 7057,12 s. Die beiden Referenz-Eingabeteile fehlen unter den fünf konkret geprüften bekannten Namen; keine Laufwerke oder privaten Ordner wurden dafür breit durchsucht. Unter anderem John Wick Kapitel 4 mit Audio Korrektur.mp4 und John Wick Kapitel 4 new.mp4 sind noch vorhanden.

Bestehende Messhilfe standalone_guard wurde unverändert verwendet. Prozessgrenzen: 1 GiB, 25 % CPU, Laufzeitlimit, Start unter Windows-Job. Keine Produktinstrumentierung und keine alternative Klebeimplementierung. Separate Ausgaben ausschließlich unter .build/klebecenter-performance-20261028/. Diese Einzelmessungen enthalten nicht den WPF-Dialog und nicht den C#-Paketparser. Dateicaches wurden nicht geleert; es handelt sich um einen lokalen Warm-/Mischcache-Befund.

| Aktueller Einzelaufruf | Zeit |
|---|---:|
| Metadaten mit Decoderkonfigurationshash | 0,11 s |
| Alle Videopakete und Payload-SHA-256 nach compact ausgeben | 8,91 s |
| Alle Audiopakete und Payload-SHA-256 nach compact ausgeben | 5,16 s |
| Gesamte AAC-Tonspur dekodieren, PCM nach null | 7,44 s |
| Sechs Sekunden Bild/Ton um die Klebestelle dekodieren | 0,78 s |
| Vollständige 2,82-GB-Datei per SHA-256 lesen | 2,224 s |
| Reiner Remux derselben Ausgabe ohne faststart | 8,47 s |
| Reiner Remux derselben Ausgabe mit faststart | 11,34 s |

Die ffprobe-Zeiten umfassen Ausgabe von etwa 37,5 MB Video- bzw. 70,8 MB Audiopakettext in eine Messdatei. Im Produkt stehen stattdessen Pipe-Übertragung und C#-Parsing an. Die Werte sind daher keine exakten Produktphasenzeiten. Auch der Audio-Decode nach null misst nicht die Kosten des heutigen verified.pcm-Schreibens.

**faststart:** Bei diesem einen Messpaar steigen die logischen Lese-/Schreibzähler von jeweils ungefähr 2,82 GB auf jeweils ungefähr 5,63 GB. Der Mehraufwand beträgt hier 2,87 s. FFmpeg dokumentiert dafür einen zweiten Durchgang zum Verschieben des moov-Index an den Dateianfang. Das bestätigt zusätzliche I/O-Arbeit, aber kein statistisch allgemeingültiges Zeitverhältnis. Logische Prozess-I/O-Zähler sind keine Messung physischer Plattenbytes. [FFmpeg MP4/faststart](https://www.ffmpeg.org/ffmpeg-formats.html).

### C. Isolierter AAC-Vergleich – Messung, keine Umstellungsempfehlung

Aus Minute 10 bis 15 der bereits fertigen Ausgabe wurden einmal 300 s Float-PCM extrahiert: 14400000 Samples je Kanal. Für den Vergleich wurde unverändertes PCM in einen RF64-Header mit expliziter Stereo-Kanalmaske verpackt, wie ihn CAN bereits erzeugt; Nutzdaten-Hash vor/nach Verpackung identisch.

| AAC-Coder, gleiche nominelle 192 kbit/s | Drei warnungsfreie Läufe | Median |
|---|---|---:|
| nmr, aktueller lokaler Standard | 6,14 / 6,16 / 6,05 s | **6,14 s** |
| twoloop, isolierter Vergleich | 4,66 / 4,91 / 4,53 s | **4,66 s** |

Hier benötigt twoloop **24,1 % weniger Zeit**, entsprechend ungefähr 1,32-fachem Durchsatz. Beide Varianten wurden streng vollständig dekodiert: AAC-LC, Stereo, 48 kHz und exakt 14400000 Samples je Kanal. Gemessene Streambitrate: etwa **192006 bit/s bei nmr**, **196425 bit/s bei twoloop**.

**Nicht bewiesen:** gleiche hörbare Qualität, gleiche Kompressionseffizienz bei identischer tatsächlicher Bitrate, unverändertes Verhalten bei 5.1/44,1 kHz und gleicher Gewinn beim ganzen Originalfilm. Das Test-PCM stammt bereits aus einer AAC-Ausgabe und ist nicht das ursprüngliche Referenz-PCM vor dessen erster Kodierung. Keine Empfehlung, ungeprüft auf twoloop oder höhere NMR-Speedwerte zu wechseln.

Ein erster Messversuch mit dem von FFmpeg erzeugten WAV hatte die Meldung Guessed Channel Layout: stereo. Dieser Versuch ist ausdrücklich nicht als CAN-konformer Erfolg gewertet. Nach expliziter Kanalmaske wurden alle sechs maßgeblichen Messläufe ohne Warnung wiederholt. Keine Decoderwarnung wurde für einen erfolgreichen Produktlauf ignoriert.

## 5. Optimierungsmöglichkeiten – Nutzen, Risiken und Voraussetzungen

| Maßnahme | Gesicherter Ansatz / möglicher Nutzen | Risiko und Voraussetzung | Bewertung |
|---|---|---|---|
| Direkt RF64 statt joined.pcm und anschließender Kopie | Spart genau eine vollständige PCM-Lesung und -Schreibung: bei John Wick **7,318 GB logische PCM-I/O**, 25 % des jetzigen PCM-Nutzdatenverkehrs | Reihenfolge, erwartete Samplezahl, RF64-Header/Kanalmaske und unabhängigen Payload-Readback-Hash vollständig erhalten; Peak-Platzbedarf nicht automatisch halbiert | **Erster Optimierungskandidat nach Messinstrumentierung** |
| Fertiges AAC zu binärem Zähler statt verified.pcm dekodieren | Spart eine weitere PCM-Dateischreibung von **3,659 GB**; vollständige Dekodierung bleibt | Binär-stdout darf nicht mit aktuellem -progress pipe:1 kollidieren; getrennte Fortschritts-/Fehlerkanäle, vollständiges Drainen, Timeout/Abbruch und exakter Bytezähler nötig | Danach isoliert prüfen |
| Audio und Video je Quelle in einem FFmpeg-Aufruf ausgeben | Ein Quelldurchlauf und ein Prozessstart pro Teil weniger | -copyts und Optionen je Ausgabe sauber trennen; Priming/Endpadding, Videozeitbasis und beide Ausgaben unverändert nachweisen | Mittlerer Aufwand, messbarer Nutzen noch offen |
| Paketprüfungen/Metadaten gemeinsam auswerten | Wiederholte AAC-Ausgabescans, Metadatenabfragen und nicht verwendete Hashberechnung reduzieren | Sämtliche bisher tatsächlich geprüften Felder behalten; unveränderliche Metadaten mit Quell-/Dateiidentität binden; keine Prüfung einfach streichen | Nach Phasenmessung priorisieren |
| PCM direkt zwischen Decoder und Encoder pipen | Große Dateiwege potenziell vermeiden | Heutiger unabhängiger Zusammensetzungsnachweis muss ersetzt, nicht weggelassen werden; Backpressure, Samplegrenzen, Kanalmaske, Prozessfehler und Abbruch werden komplexer | Nicht erster Schritt; zunächst geprüfte RF64-Datei behalten |
| AAC-Encoding und finalen Mux zusammenfassen | Ein Zwischencontainer und Prozess weniger | Unabhängiger Vergleich AAC-Zwischenpakete gegen finale Pakete entfällt ohne Ersatzbeweis; nur geringer AAC-I/O-Anteil betroffen | Vorerst nicht empfehlen |
| Unabhängige Schritte parallel ausführen | Metadatenabfragen oder Quellenarbeit könnten sich überlappen | Globales Ressourcenbudget erforderlich; zwei 25-%-Jobs können 50 % Gesamtbudget bedeuten, mehr RAM und konkurrierende Diskzugriffe; Fehler/Abbruch sauber koordinieren | Keine pauschale Parallelisierung |
| faststart weglassen | Im Einzelpaar 2,87 s weniger und etwa 5,63 GB weniger logische Lese-/Schreibarbeit zusammen | Ändert die bewährte Dateiorganisation für frühen Wiedergabestart; kein allgemeiner Kompatibilitätsnachweis | Nicht einfach abschalten |
| moov_size statt nachträglicher Indexverschiebung | Index am Anfang ohne vollständigen faststart-Verschiebedurchlauf möglich | Sichere Reservierung für beide Spuren/Packetzahlen nötig; zu wenig Platz führt zum Muxfehler; Nutzwert noch ungemessen | Späterer separater Versuch |
| Anderer AAC-Coder / aac_mf | twoloop auf einem Ausschnitt schneller gemessen | Andere Bitrate/psychoakustische Entscheidungen; Qualitätsgleichheit unbewiesen. aac_mf zusätzlich Windows-Codec-/16-Bit-Abhängigkeit | Keine Produktumstellung aufgrund dieser Messung |
| Mehr Threads / höheres CPU-Limit | Kein belegter Nutzen beim nativen AAC-Encoder ohne Threadunterstützung | Mehr Last und Abweichung von bisherigen Schutzgrenzen; nicht jede Prozessarbeit ist single-threaded | Nicht als erste Maßnahme |
| GPU / Video-Neukodierung | Kein nutzbarer AAC-GPU-Encoder in der konkreten Installation | Video-Neukodierung verletzt Auftrag, GPU-Videodecode beträfe nur kurzen Test | **Keine GPU-Integration vorschlagen** |

Die I/O-Einsparungen sind Rechenwerte aus identischen Nutzdatenmengen, keine versprochenen Sekunden oder Gesamtbeschleunigungen. Weniger Dateiarbeit und identischer Prüfumfang erscheinen ohne Qualitätsverlust erreichbar; die konkrete Implementierung müsste das anschließend beweisen.

## 6. Kleinster erster Entwicklungsschritt – erst nach Freigabe

**Zunächst nur optionale Phasenzeitmessung im unveränderten Klebecenter.** Eine monotone Uhr erfasst je bestehenden Schritt Start/Ende: Metadaten, Videozeitprüfung, Tonzeitprüfung, Decode je Teil, Videokopie und deren Vergleich, PCM-Join/Readback, RF64-Verpackung, AAC-Encoding, finaler Mux, finale Paket-/Tonprüfungen, Voll-Decode, Vorschau, Datei-SHA und Bereinigung.

Ausgabe in separater Diagnose: tatsächlicher Werkzeugpfad plus Version/Hash, gewählte Streamindizes, Dateigrößen, erwartete/erreichte Samples, Prozess-/Phasenzeiten und soweit möglich logische I/O-/CPU-Zähler. Vorhandene Statusmeldungen liefern heute keine belastbare Zeitzerlegung; Fortschrittsmeldungen out_time_us sind Medienzeit und keine Wallclock-Messung. Keine Änderung der Befehle, Tonqualität, Grenzen oder Prüfentscheidungen. Diese Produktinstrumentierung wurde in diesem Auftrag **nur vorgeschlagen**.

Danach genau eine Optimierung: JoinPcm und WrapWave zu einer direkten RF64-Zusammensetzung mit demselben unabhängigen Payload-Hashnachweis zusammenführen. Zuerst Tests für identische PCM-Nutzdaten, Reihenfolge, kurze/fehlende Daten, RF64 jenseits 4 GiB, Mono/Stereo/5.1, Kanalmaske und Abbruch; noch kein Encoderwechsel und keine Parallelisierung.

## 7. Reproduzierbarer Vorher-Nachher-Vergleich John Wick 4

1. Die beiden tatsächlich bereits geschnittenen Referenzteile wieder bereitstellen. Dokumentiert waren **John Wick Kapitel 4 Teil 1 otr-can - AAC.mp4** und **John Wick Kapitel 4 Teil 2 mit Audio Korrektur.mp4**. Keine Ersatzteile aus dem fertigen Film erzeugen und als Originalreferenz ausgeben. Alternativ den zweispurigen fertigen Teil 1 verwenden, aber Auswahl und neue Baseline ausdrücklich dokumentieren.
2. SHA-256, Größen und Streamkonfigurationen beider Teile erfassen. Pfade dürfen sich ändern, Film-/Toninhalt und Reihenfolge nicht. AAC-Spuren ausdrücklich auswählen; eine AC-3/AAC-Variante getrennt führen.
3. Originale schreibgeschützt behandeln, neue Ziele ausschließlich in getrennten Messordnern. Werkzeug-EXEs anhand Hash/Version fixieren, Ressourcenlimits beibehalten. Hardware, Laufwerke, freien Platz und Cachezustand dokumentieren; keine Cachebereinigung unterstellen.
4. Nach freigegebener Instrumentierung unveränderte Build-41-Logik als Baseline messen. Danach genau die freigegebene einzelne Optimierung. Reihenfolge A/B abwechseln, nach einem Aufwärmlauf mindestens drei Messungen je Variante; Median, Bereich und CPU-/I/O-Werte berichten. Kalte und warme Durchläufe getrennt bewerten.
5. Zuerst **alle bisherigen Prüfungen unverändert** bestehen lassen. Für eine reine I/O-Optimierung zusätzlich exakt gleichen PCM-Payload-Hash und Samplefolge zur Baseline nachweisen. Der alte Referenzhash **44eb4d7f104fd64405528d8ac7cd52538fb0866af7434d4ea9a5faebf7132a10** ist ein Vergleichsziel nur bei wirklich identischen ursprünglichen Teilen, Auswahl und Decoderbedingungen; Abweichungen untersuchen, nicht pauschal akzeptieren.
6. Zielwerte: **238229 Videopakete**, **457399680 Samples je Kanal**, **9529,16 s Gesamtdauer**, **7057,12 s Klebestelle**, Vorschau ab **7054,12 s**, AAC-LC Stereo/48 kHz/nominal 192 kbit/s. Videopaketdaten und rationale PTS/DTS samt Übergangsversatz gegen die Originale prüfen; AAC-Priming/Endpadding, durchgehende Zeitachse und vollständige dekodierte Ausgabesamplezahl erhalten.
7. Negative Fälle weiter nachweisen: defekte gewählte Spur, defekte entfallende Spur, Sampleverlust/-zusatz, falsche Reihenfolge, unpassendes Format, Abbruch, vorhandenes Ziel/Zieldatei-Rennen und gesperrte Bereinigung. Nur kontrollierte Ablehnung gilt als bestanden. Anfang, Klebestelle und Ende persönlich auf Bild/Ton/Synchronität prüfen.
8. Erst eine nachgewiesene Zeit-/I/O-Verbesserung mit unverändertem Qualitätsnachweis rechtfertigt Übernahme. Encoderexperimente benötigen darüber hinaus einen eigenen Hör-/Qualitätsvergleich; gleiche Samplezahl und AAC-LC allein beweisen keine gleiche Tonqualität.

## 8. Nachweise und unveränderter Produktstand

Lokale Analyse: **.build/klebecenter-performance-20261028/**. Enthält ursprünglichen Prüfauftrag/Nachweiskopien, Werkzeuglisten/-Hashes, historische Monitore, aktuelle Einzelmessungen samt Befehlen/Ressourcenlogs, warnungsfreien AAC-Vergleich, PCM-Hash und Sample-/Profilprüfung. Größere ausschließlich selbst erzeugte Testmedien werden nach Berichtserstellung entfernt; Messlogs bleiben erhalten. Die fertige John-Wick-Quelle wurde ausschließlich gelesen, Größe und Änderungszeit blieben unverändert; nachträglicher Datei-SHA-256: **da9df30b9104a3ff19fdeabd3b1663cd1e54b6f8ee89ba04d82007110bb70f89**.

Unverändert: Produktquellen, Tests, Werkzeuge/Installer, die vier bekannten lokalen Produktions-Lockdateien, Version.props, Buildzähler **41**, otr-can 0.1.6 und veröffentlichte Pakete. Keine Produkt-Builds oder -Tests erneut ausgeführt, weil ausschließlich untersucht und dokumentiert wurde. Die Messläufe sind keine neue allgemeine Produktabnahme. Keine Implementierung ohne anschließende ausdrückliche Freigabe.
