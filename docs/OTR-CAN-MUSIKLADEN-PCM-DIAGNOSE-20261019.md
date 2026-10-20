# Musikladen – Diagnose der 34 zusätzlichen PCM-Samples

Stand: **19.10.2026**. Untersucht: unveränderter Rust-Motor **otr-can 0.1.5**. Auftrag: ausschließlich technische Diagnose und Lösungsvorschlag, **keine Implementierung**.

## Ergebnis

**Die Ursache ist am Original nachgewiesen.** Innerhalb des Behaltebereichs gibt es **34 Überschneidungen von jeweils einem Audio-Sample**: Die MP4-Zeittabelle weist an diesen Stellen AAC-Paketdauern von **1023** Ticks bei einer Zeitbasis von **1/48000 s** aus; der Decoder liefert für die zugehörigen AAC-Frames jeweils **1024** Samples. Der nächste Frame beginnt dadurch einen Sample-Tick vor dem rechnerischen Ende seines Vorgängers.

Der Motor wählt mit `atrim` anhand der Originalzeitstempel aus und setzt danach mit `asetpts=N/SR/TB` eine lückenlose Samplefolge zusammen. Dabei bleiben alle 34 überlappenden Samplepositionen doppelt besetzt. Das erklärt **exakt 168760354 statt 168760320 Samples**. Keine pauschale Rundungsvermutung und kein WAV-Metadatenfehler: Eine unabhängige Integer-Auswertung und ein Prüfsummenlauf des unveränderten Motorfilters reproduzieren dieselbe Zahl.

Die 34 zusätzlichen Samplepositionen entstehen **verteilt innerhalb** des Behaltebereichs. Sie sind kein Block am Dateianfang oder Dateiende. Eine zusätzliche Prüfung der PCM-Werte zeigt: **Keine der 34 überlappenden Stereo-Samplepaarungen ist bitgleich.** Daher wäre die Aussage „nur identische Duplikate entfernen“ sachlich falsch.

Die bestehende Sampleprüfung soll erhalten bleiben. Der Befund erfordert eine ausdrücklich definierte Behandlung widersprüchlicher Quellzeiten, wenn diese Aufnahme künftig erfolgreich verarbeitet werden soll.

## Quelle und Umfang

Original: `Musikladen__Uschi_Nerke_und_Manfred_Sexauer_praesentieren____26.09.20_00-45_ndr_60_TVOON_DE.HQ.mp4`.

- Größe: **1212605553 Bytes**.
- SHA-256 vor den Messungen: `5CD5EBE2B90B68A5BA809545EA65139B0006B516D60DD3F275B78C972BC21AB9`.
- Video: H.264, 50 fps, 269925 Frames, Zeitbasis 1/12800, Start 0.
- Eine Tonspur, globaler Streamindex **1**: AAC-LC, Stereo, 48000 Hz, Zeitbasis **1/48000**, Start 0.
- Untersucht wurde der im Nutzerprotokoll tatsächlich verarbeitete Keep **[1073.640000, 4589.480000)** bzw. Frames **[53682,229474)**. Keine Cutlist geändert oder für einen neuen Filmschnitt benutzt.
- Werkzeug: `ffmpeg`/`ffprobe` aus `C:/tools/ffmpeg/bin`, gemeldeter Stand `2026-10-12-git-615b07170e-essentials_build-www.gyan.dev`.

Kein vollständiger Video-/Audio-Neuschnitt und keine neue Filmausgabe. Die Videostufe ist durch das Nutzerprotokoll belegt; sie wurde hier nicht erneut ausgeführt. Die reine Audioreproduktion läuft ab Quellbeginn mit vollständiger Dekodiervorgeschichte.

## Konkrete Codepfade

Datei im Nachbarrepository: [can-engine/src/cutting/audio.rs](../../otr-native-poc/can-engine/src/cutting/audio.rs).

| Stelle | Verhalten |
|---|---|
| Zeilen 115–126, `sample` / `samples` | `round(Zeit * Samplerate)` je Grenze; Summieren der Differenzen. |
| Zeilen 209–218 | Schnittintervalle auf Videoframes abbilden und deren Originalzeiten als Audiogrenzen übernehmen. |
| Zeilen 128–152, insbesondere 141 | Je Spur `asplit`, je Keep zeitbasiertes `atrim`, danach `asetpts=N/SR/TB`, `concat` und erneutes `asetpts`. |
| Zeilen 247–279 | `-copyts`, `-reinit_filter:a 0`, Lesen ab Anfang bis zur letzten Keep-Grenze; PCM-Ausgabe als `pcm_f32le` in WAV/RF64. |
| Zeilen 283–300, **Fehlermeldung in Zeile 297** | Prüfen des PCM-Formats, der Zeitbasis und der von ffprobe gelesenen WAV-`duration_ts` auf die Sollsamplezahl. Hier bricht der Nutzerlauf ab. |
| `verify_frames`, Zeilen 154–184 | Nach späterer Audiokodierung zusätzlich exakte dekodierte Samplezahl und lückenlose rohe PTS prüfen. Diese Stufe wird im Fehlerlauf noch nicht erreicht. |

Die tatsächliche PCM-Zählung im Motor erfolgt also über die Dauer in Sample-Ticks der erzeugten WAV-Datei, nicht über eine vorherige Validierung der Quellframe-Zeitfolge. Diese wurde hier unabhängig vollständig gemessen.

## Exakte Rechnung und Schnittgrenzen

Die Sollrechnung ist für diese Grenzen korrekt:

```text
Start = round(1073.640000 * 48000) =  51534720
Ende  = round(4589.480000 * 48000) = 220295040
Soll  = Ende - Start             = 168760320
Dauer = 3515.840000 s
```

Die Grenzen liegen exakt auf ganzzahligen 48-kHz-Samplepositionen. Die Mikrosekundendarstellung des Motors und Floating-Point-Rundungen erklären die Abweichung von 34 Samples hier nicht.

Alle folgenden Frame-/Sampleindizes sind **nullbasiert**, nach Berücksichtigung des Decoder-Primings:

| Größe | Start | Exklusives Ende |
|---|---:|---:|
| Gewünschte Position auf der Quellzeitachse, in Sample-Ticks | 51534720 | 220295040 |
| Betroffener dekodierter AAC-Frame | 50316 | 215121 |
| PTS dieses Frames | 51534327 | 220294613 |
| Dekodierte Samples in diesem Frame | 1024 | 1024 |
| Position innerhalb dieses Frames | 393 | 427 |
| Position in der kumulierten dekodierten Samplefolge | 51523977 | 220284331 |
| Differenz Quell-PTS minus kumulierter Sampleindex | 10743 | 10709 |

Damit behält der Filter **631 Samples aus dem ersten Frame**, **164804 vollständige Frames mit je 1024 Samples** und **427 Samples aus dem letzten Frame**:

```text
631 + 164804 * 1024 + 427 = 168760354
220284331 - 51523977     = 168760354
10743 - 10709           = 34
```

Die unabhängige Zählung benutzt für jeden dekodierten Frame den Schnitt der Intervalle `[PTS, PTS + nb_samples)` und `[Start, Ende)`. Sie ergibt exakt denselben Wert wie der Motorfilter.

## Wo die 34 Überschneidungen liegen

Für benachbarte Frames wurde ganzzahlig geprüft:

```text
Delta = PTS_des_nächsten_Frames - (PTS_des_vorigen_Frames + dessen_nb_samples)
```

Im Keep gibt es **genau 34 Abweichungen**, jede mit **Delta = -1**, und keine positive Lücke.

Erste Stelle:

```text
Frame 61369: PTS 62852599, 1024 dekodierte Samples
Erwarteter nächster PTS: 62853623
Frame 61370: tatsächlicher PTS 62853622
MP4-Paketdauer des Vorgängers: 1023
Quellzeit des nächsten Frames: 1309.450458333 s
```

Letzte Stelle: nächster Frame **214757**, PTS **219921877**, statt **219921878**, Quellzeit **4581.705770833 s**.

Die vollständige Liste steht in `.build/musikladen-audio-diagnose-20261019/timeline-analysis.json`; die zugehörigen Paketdauern stehen in `packet-analysis.json`.

Die MP4-`stts`-Tabelle wurde zusätzlich direkt mit einem kleinen lesenden Box-Parser geprüft. Sie enthält für die ganze Audioaufnahme **43 Paketdauern von 1023**, davon **9 vor dem Keep und 34 im Keep**. Damit sind die verkürzten Zeitabstände schon im Container hinterlegt, unabhängig von ffprobes Darstellung oder der PCM-Konvertierung. Der Audio-Paketnachweis und der Decoder-Nachweis stimmen überein.

Vor dem Keep liegen außerdem vier größere positive Zeitsprünge: **1536 + 6144 + 1536 + 1536 = 10752 Ticks**. Nach den neun frühen Ein-Sample-Überschneidungen verbleiben am Keep-Start **10743 Ticks** Versatz zwischen Quellzeit und kumulierter Samplezahl. Das ist relevant für mögliche Reparaturen: Ein bloßer Wechsel auf `start_sample=Zeit*Samplerate` würde hier den Anfang um **10743 dekodierte Samples, etwa 223.813 ms**, verschieben. Die Quellzeitachse darf nicht durch einen unkalibrierten Samplezähler ersetzt werden.

## Decoder, Priming, Padding und Resampling

- Das erste AAC-Paket hat PTS -1024 und `skip_samples=1024`. Die dekodierte Folge startet bei PTS 0. Das Priming liegt weit vor dem Keep und erklärt die 34 Samples nicht.
- Quell-Endpadding liegt am Ende der Aufnahme, ebenfalls außerhalb dieses Keeps. Die Tonspur reicht bis etwa **5397.567 s**, der Keep endet schon bei **4589.480 s**. Der frühere Doc-Fall mit fehlendem Ton am Aufnahmeende ist daher keine Erklärung für diesen Fehler.
- Im Keep liefern die betroffenen AAC-Frames 1024 Samples, obwohl ihre ausgewiesene Paketdauer nur 1023 Ticks beträgt. Dieses Zusammenspiel von Container-Timing und Decoder-Ausgabe ist direkt belegt; es ist kein Beweis für einen defekten AAC-Decoder.
- Die Originalrate bleibt bei 48000 Hz. Die Reproduktion benötigt keine Änderung der Samplerate und keinen aktivierten asynchronen Zeitausgleich. Ein impliziter Formatwechsel zu Float-PCM allein beseitigt den Zeitkonflikt nicht und ist nicht dessen nachgewiesene Ursache.
- `asetpts=N/SR/TB` ändert Zeitstempel, entfernt aber keine Samples. Es macht die ausgewählte Folge lückenlos und verdeckt dabei deren vorherige lokale Überschneidungen.

FFmpeg dokumentiert ausdrücklich, dass `atrim` mit Zeitgrenzen anhand der Zeitstempel auswählt und mit Samplegrenzen lediglich durchlaufende Samples zählt; unterschiedliche Ergebnisse bei unpassenden Zeitstempeln sind daher erwartbar: [atrim-Dokumentation](https://ffmpeg.org/ffmpeg-filters.html#atrim). Der [Filterquellcode](https://github.com/FFmpeg/FFmpeg/blob/master/libavfilter/trim.c) begrenzt die Auswahl innerhalb eines Frames, normalisiert jedoch keine Überschneidungen zwischen Frames. Die reale Messung ist hier der versionsbezogene Nachweis; der verlinkte Upstream-Code dient der Einordnung.

## Synchronität und Grenzen der Aussage

**Der Abbruch ist berechtigt, die allgemeine Sollrechnung ist nicht zu streng.** Die bisherige Audiomontage setzt stillschweigend voraus, dass Quell-PTS und dekodierte Samplemengen innerhalb eines Keeps zusammenpassen. Das ist bei dieser Aufnahme nicht erfüllt.

Würde man nur die Prüfung abschalten und die komplette ausgewählte Samplefolge mit 48000 Hz ausgeben, wäre sie **34/48000 s = 0.708333 ms länger** als die gewünschte Videozeit. Die Abweichung baut sich über die 34 Stellen auf. Das ist ein messbarer Zeitkonflikt, unabhängig davon, ob er persönlich hör- oder sichtbar wäre. Eine Wahrnehmbarkeits- oder endgültige A/V-Abnahme wurde nicht vorgenommen; weitere spätere Prüfungen können einen solchen Lauf außerdem erneut ablehnen.

Nicht geklärt ist, **warum der Ersteller der Quelldatei diese 1023-Tick-Dauern gesetzt hat**. Eine Uhren-/Zeitstempelkorrektur beim ursprünglichen Aufzeichnen oder Umverpacken ist plausibel, aber nicht nachgewiesen. Die gemeldete Lavf-Version allein beweist weder Entstehungsweg noch Verantwortlichkeit.

## Korrekturvarianten und Empfehlung

| Variante | Bewertung und Risiko |
|---|---|
| PCM-Prüfung lockern oder Sollzahl durch Istzahl ersetzen | Nicht empfehlen. Beseitigt den Zeitkonflikt nicht und könnte andere Fehler verdecken. |
| Am Ende 34 Samples abschneiden | Nicht empfehlen. Verschiebt den Fehler an die Schlusskante; die 34 inneren Konflikte bleiben unbehandelt. |
| Unkalibriertes `start_sample` / `end_sample` oder `asetpts` vor dem Schnitt | Nicht empfehlen. Ignoriert hier bereits vor dem Keep etwa 224 ms Quellzeitversatz. |
| Frühzeitige Quellzeitprüfung mit genauer Ablehnung | Sicherer erster Verbesserungsschritt, spart die vorherige Videobearbeitung. Ermöglicht für diese Quelle allein noch keinen erfolgreichen Schnitt. |
| Kontrollierte Normalisierung der nachgewiesenen Überschneidungen auf das Original-Zeitraster | Geeigneter Ansatz für einen separat beauftragten Korrekturversuch. Erfordert eine feste Signalbehandlung und neue inhaltliche Prüfungen. |
| Allgemeines asynchrones Resampling | Alternative mit größerer Eingriffstiefe. Kann Samples ändern, ergänzen oder entfernen; ohne genaue Konfiguration und Validierung keine geeignete Pauschallösung. |

**Empfohlener Versuch:** Die decodierten Quellframes je Spur gegen deren tatsächliche PTS prüfen, bevor `asetpts` diese Information verwirft. Für den hier exakt erkannten Fall anschließend die Samplefolge auf die ursprüngliche Zeitachse normalisieren; erst danach Keeps zusammenfügen und wie bisher einmal kodieren. Fehlerfreie Quellen sollen den bisherigen unveränderten Pfad durchlaufen.

Eine konkrete, nachvollziehbare Regel wäre: Bei einem nachgewiesenen Ein-Sample-Überlapp den nächsten Frame an seinem originalen PTS beginnen lassen und den zeitlich darüber hinausreichenden letzten Samplevektor des vorherigen Frames ausschließen. Das würde in diesem Keep **genau an den 34 dokumentierten Positionen je einen Stereo-Samplevektor entfernen**, keine 34 Samples am Ende. Die resultierende Auswahl hätte rechnerisch genau 168760320 Positionen auf dem gewünschten Zeitraster. **Dies ist eine vorgeschlagene Regel, keine implementierte oder akustisch freigegebene Korrektur.** Die kollidierenden Werte unterscheiden sich; die Auswahl verändert den Ton und kann lokal eine Signalunstetigkeit erzeugen. Eine Resampling-Variante könnte solche Unstetigkeiten anders behandeln, verändert dafür zusätzliche Nachbarwerte und muss ebenfalls geprüft werden.

Unter den gleichzeitig geforderten Bedingungen „jeden unterschiedlichen dekodierten Samplewert unverändert behalten“, „48000 Hz“ und „exakt dieselbe Präsentationszeit“ gibt es für diese widersprüchlichen Eingangszeiten keine verlustfreie Lösung. Die Signalbehandlung muss bewusst entschieden werden; ein allgemeiner zulässiger Zählfehler wäre dafür kein Ersatz.

Sicherheitsbedingungen für eine Umsetzung:

1. Zeitbasis, tatsächliche Samplerate und Quellframe-PTS ganzzahlig bzw. rational abgleichen. Nicht lediglich einen maximalen Gesamtsamplefehler zulassen.
2. Die freigegebene Konfliktklasse exakt definieren, jede betroffene Stelle und Änderung protokollieren. Positive Lücken, größere Überlappungen, fehlende PTS oder Formatwechsel nicht stillschweigend mitreparieren. Nicht benötigte Vorgeschichte weiterhin dekodieren, ihre Lücken aber nicht mit einem globalen Nullstart auf den Keep übertragen.
3. Behandlung für alle Kanäle einer Spur gemeinsam und für jede Tonspur separat durchführen. Eine Aufnahme mit nur einer AAC-Stereospur ist kein Mehrspur-/AC3-/MP3-Nachweis.
4. Für jeden Keep seine exakte Länge und Abdeckung prüfen, danach PCM-Gesamtzahl, kodierte/dekodierte Samplezahl, kontinuierliche rohe PTS sowie finale Mux-Prüfungen unverändert streng anwenden.
5. Für unauffällige Quellen die unveränderte PCM-Auswahl belegen; die bisherigen Originalbeispiele und Formatmatrizen erneut prüfen.

FFmpeg bietet zeitstempelgesteuerten Ausgleich an, wobei `async`, `min_comp` und `min_hard_comp` festlegen, wann Werte gestreckt, gekürzt oder ergänzt werden. **Ein bloßes `aresample=async=1` ist für diese Ein-Sample-Fälle keine validierte Korrektur**; die dokumentierte Standardgrenze für harte Korrekturen beträgt 0.1 s: [Resampler-Dokumentation](https://ffmpeg.org/ffmpeg-resampler.html). Hier wurde keine Resampler-Korrektur ausprobiert und kein fertiger Filter als sicher freigegeben.

## Tests und zusätzliche Messungen vor einer Korrektur

Bereits ausgeführt:

- Bestehender Rust-Formatcheck erfolgreich.
- Bestehender Motor unverändert kompiliert und getestet, separater Test-Ausgabeordner: **38 Bibliotheks- und 6 CLI-Tests, 44/44 bestanden**, keine fehlgeschlagenen/ignorierten Tests.
- Originalton vollständig dekodiert: **253001 Frames**, Warnungsprotokoll leer.
- Audio-Pakete vollständig gelesen und `stts` direkt geprüft: **253002 Pakete**, darin 43 Dauern von 1023 Ticks.
- Integer-Auswahl unabhängig nachgerechnet und unveränderten Motorfilter als `pcm_f32le`-Frameprüfsummen ausgeführt: **168760354 Samples**, erste Ausgabe-PTS 0, keine Ausgabe-PTS-Lücke, Warnungsprotokoll leer.
- An allen 34 Konflikten die letzten/ersten Stereo-Samplevektoren der beiden betroffenen Frames verglichen: **0/34 bitgleich**, keine Decoderwarnungen.

Die bestehenden Audiotests prüfen Sollsampleberechnung und die Kontinuität **der Ausgabe**, aber keine reale unregelmäßige Quell-PTS-Folge vor `asetpts`. Ihr Erfolg widerlegt den Befund deshalb nicht.

Vor Freigabe einer Umsetzung zusätzlich notwendig:

- Synthetische Fälle für exakt bekannte Überlapp-Positionen und Samplewerte; 25/50 fps, 32/44.1/48 kHz, mehrere Keeps, Schnitt direkt auf bzw. neben einer Konfliktstelle, Anfang/Ende/Einzelframe, unterschiedliche Kanalzahlen und gemischte Spuren.
- Negative Fälle: echte Tonlücken und zu früher Tonabschluss (Doc-Zwischenfall), verzögerter Beginn, größere oder nicht klassifizierte Überschneidungen, falsche/fehlende PTS und Formatwechsel müssen weiter fehlschlagen.
- Unabhängiger PCM-Referenzvergleich für die gewählte Konfliktregel; nicht lediglich Gesamtzahl prüfen. Änderungen dürfen ausschließlich an den erklärten Stellen bzw. bei Resampling im ausdrücklich definierten Umfeld auftreten.
- Anschließend vollständiger Musikladen-Schnitt im separaten Arbeitsordner: exakte 175792 Videoframes und 168760320 Audiosamples, unveränderte finale Prüfungen sowie persönliche Bild-/Tonprüfung. Eine erfolgreiche Audiodiagnose ersetzt diesen Nachweis nicht.
- AAC-/AC3-/MP3- und Mehrspurregressionen einschließlich bisher erfolgreicher Realbeispiele. Danach gezielte CAN-Integration sowie regulärer Build/Test mit geschützt erhaltenen Lockdateien.

## Nachweise und Erhaltung

Diagnosebelege liegen ausschließlich unter `.build/musikladen-audio-diagnose-20261019/` (ignoriert): Quellhash, Metadaten, dekodierte Framezeiten, Paketzeiten, `timeline-analysis.json`, `packet-analysis.json`, `mp4-stts-proof.json`, `integer-crosscheck.json`, Motorfilter/Frameprüfsummen und `original-filter-proof.json`, Kollisionswerte samt kleinem PCM-Frameauszug und Prüfhelfern, Rust-Testprotokoll und abschließender Erhaltungsnachweis.

Ein erster Exportversuch der Paketstatistik scheiterte an numerischen JSON-Dictionary-Schlüsseln im PowerShell-Helfer; danach mit String-Schlüsseln korrekt neu erstellt und unabhängig gegen die rohe MP4-Tabelle geprüft. Keine Quelldaten dafür verändert. Die ersten Sandbox-Lesezugriffe blieben ohne Ausgabe; die erfolgreichen Messungen nutzten die freigegebene lokale Ausführung.

Abschließende SHA-256-Prüfung: **67 geschützte Bestandsdateien unverändert**, darunter alle vorbestehenden lokalen Änderungen und sämtliche erfassten Motorquellen; Originalhash unverändert. Das Rust-Repository bleibt sauber. Im CAN-Git-Status kommt ausschließlich dieser neue Bericht hinzu. Ein erster Aufruf des abschließenden Prüfhelfers hatte eine nicht initialisierte Pfadvariable; nach expliziter Initialisierung wurde die komplette Erhaltungsprüfung erfolgreich wiederholt. Nachweis: `preservation-final.json`.

Einzige neue Projektdokumentation ist dieser Bericht. Keine Änderung an CAN-/Rust-Produktcode, Tests, MP4Box-Übergangsprüfung, Lockdateien, Originalmedien, Cutlist, vorhandener Filmausgabe, Release-Artefakten, Einstellungen oder Versions-/Buildnummer. Kein Commit, Push oder Release. Kein CAN-Restore/Produktbuild für diese reine Diagnose; die Rust-Tests bauen nur in den separaten Diagnoseordner.
