# OTR-CAN – Frame- und Zeitstempeldiagnose vor Version 1.0

Stand: 11.10.2026. Unveränderter regulärer Motor **0.1.2**, CAN **0.2.1 · Build 11 · RC3**. Untersuchung, keine Korrektur des Schnittkerns oder Motorpfadumstellung.

## Ergebnis

Gezielte, vollständig nummerierte Bilder zeigen reproduzierbare Fehler bei der Aufteilung in kodierten Anfang, kopierte Mitte und kodiertes Ende. Bei **12 von 16 Diagnosefällen** ist die ausgegebene Bildfolge falsch. Eine richtige Gesamtbildzahl kann ein fehlendes und ein doppeltes Bild verdecken.

Die vorhandene Frame-Lupe ordnet in **30 Prüfungen** echtes PNG, Original-PTS und übernommene Schnittzeit korrekt zu. Der Nutzer meldet mit der aktuellen Version keine neue Auffälligkeit der Bildnavigation. Daraus wird keine pauschale Abnahme sämtlicher Player-/AVI-Sonderfälle abgeleitet.

## Prüfaufbau und Grenzen

Zwei künstliche H.264-Aufnahmen mit 25 fps, 45 Sekunden, Keyframeabstand 25 und deaktivierten Szenenwechsel-Keyframes: einmal ohne B-Frames, einmal mit maximal drei B-Frames. Jedes Bild trägt eine robuste binäre Kennung in zwölf großen Graustufenstreifen. Alle 1.125 Kennungen jeder Quelldatei wurden nach echter Dekodierung als Folge 0 bis 1.124 verifiziert. Bildnummern in diesem Bericht sind **nullbasiert**, keine lokalen Zeilennummern der CAN-Lupe.

Beide Dateien besitzen je eine AAC- und AC-3-Mono-Tonspur mit 48 kHz. Bitraten der Generierung: AAC 96 kbit/s und AC-3 384 kbit/s. Diese Diagnose ersetzt keine neue Prüfung aller realen Stereo-/Sechskanaldateien.

Acht Keep-Fälle pro Datei, alle mit derselben erhaltenen Motor-EXE geschnitten. Je Quelle ein vorbereiteter FFMS2-Index, für die seriellen Diagnoseaufrufe wiederverwendet. Keine Indizierung innerhalb des Motors. Das ist eine gezielte CLI-Diagnose, kein neuer CAN-Bedientest und keine erneute Verarbeitung der persönlichen Filme.

Nur beim Rohbildexport werden Ausgabe-PTS fortlaufend gesetzt, damit der Prüfmuxer alle decodierten Bilder ausgeben kann. **Die dekodierte Bildfolge und ihre Kennungen bleiben erhalten.** Original-PTS werden separat mit ffprobe geprüft; weder die MP4-Datei noch ihr Ton werden normalisiert.

## Nachgewiesene Bildfehler

| B-Frames | Keep-Bilder, Ende ausgeschlossen | Soll | Ist | Befund |
|---|---|---:|---:|---|
| 0 | 25–65 | 40 | 39 | Bild 50 fehlt |
| 0 | 24–65 | 41 | 39 | Bilder 24 und 50 fehlen |
| 0 | 20–65 | 45 | 43 | Bilder 24 und 50 fehlen |
| 0 | 20–75 | 55 | 54 | Bild 24 fehlt |
| 0 | 5–20 | 15 | 15 | exakt |
| 0 | 24–25 | 1 | 0 | keine Videospur, nur Audio; Motor meldet trotzdem Erfolg |
| 0 | 5–6 | 1 | 1 | exakt |
| 0 | 1001–1050 | 49 | 48 | Bild 1024 fehlt |
| 3 | 25–65 | 40 | 41 | Bild 53 doppelt |
| 3 | 24–65 | 41 | 41 | Bild 24 fehlt, Bild 53 doppelt |
| 3 | 20–65 | 45 | 45 | Bild 24 fehlt, Bild 53 doppelt |
| 3 | 20–75 | 55 | 56 | Bild 24 fehlt, unerwünschte Bilder 75 und 77 |
| 3 | 5–20 | 15 | 15 | exakt |
| 3 | 24–25 | 1 | 2 | Bild 24 fehlt; unerwünschte Bilder 25 und 26 |
| 3 | 5–6 | 1 | 1 | exakt |
| 3 | 1001–1050 | 49 | 50 | Bild 1024 fehlt; unerwünschte Bilder 1050 und 1053 |

Der späte Fall prüft auch den tatsächlich aktiven optimierten Video-Seek. Dieselbe Fehlerart tritt bereits bei frühen Kanten auf; sie wurde nicht erst durch die Geschwindigkeitsoptimierung eingeführt.

Beispiel Keep 20–65 mit B-Frames, Sollfolge 20 bis 64:

~~~text
Ist: 20,21,22,23,25,...,49,50,53,51,52,53,54,...,64
                  ^ 24 fehlt     ^ vorgezogen und später doppelt
~~~

In diesem Beispiel läuft der originale decodierte Video-PTS bei Bildposition 31 von **1,301328 s auf 1,261328 s** zurück: **40 ms**, dieselbe Größenordnung und Fehlerart wie im bisherigen Navy-CIS-Befund. Das ist ein Nachweis am nummerierten Prüfvideo, kein neu durchgeführter vollständiger Originalbildabgleich von Navy oder Pines.

## Ursachen im vorhandenen Kern

1. **Gemischte Grenzsemantik:** Intervalle haben Länge Ende minus Anfang, das zeitliche Ende ist ausgeschlossen. Der Anfangsteil wird dennoch bis erster Keyframe minus eins konstruiert. Damit fehlt das letzte Bild vor dem Keyframe. Bei nur einem Anfangsbild entsteht ein Nullintervall und dieser Teil wird ausgelassen.
2. **Unklare Eigentümerschaft des letzten Keyframes:** Der Endteil beginnt bei letztem Keyframe plus eins. Ohne B-Frames enthält die kopierte Mitte diesen End-Keyframe nicht, deshalb fehlt er. Mit B-Frames liefert derselbe Zeit-Copy dagegen zusätzliche spätere Präsentationsbilder; im Beispiel enthält die Mitte bereits Bild 53, bevor das kodierte Ende wieder bei Bild 51 beginnt.
3. **Ein Keyframe genau am ausgeschlossenen Keep-Ende wird noch als innere Grenze zugelassen.** Der Ein-Bild-Fall direkt davor kann deshalb eine leere kopierte Mitte oder ausschließlich unerwünschte Folgebilder liefern.

Betroffene Stellen: can-engine/src/cutting/ffmpeg.rs, Funktion extract_interval; can-engine/src/cutting/interval.rs, len/is_empty/to_key_frames. Eine bloße Änderung um einen Frame oder eine nachträgliche PTS-Neunummerierung reicht für alle Fälle nicht aus.

## Audio und bislang unsichtbare Warnungen

Die drei Teile des Beispiels 20–65 wurden mit den identischen Argumenten des bestehenden Kerns separat nachgestellt. Das anschließend zusammengesetzte MP4 ist **bytegleich** zur unveränderten Motor-Ausgabe:

SHA-256: c58118ffc6fbd18771b010d42da75aab373fece5bd5bc87cf19d1c4cc144abc1.

Der kopierte Mittelteil ist trotz angeforderter 1,0 Sekunden **1,08 Sekunden** lang und enthält 27 Videobilder. Seine AAC-Spur dauert 1,005333 Sekunden, AC-3 1,010667 Sekunden. FFmpeg concat verwendet die Containerdauer zur Verschiebung aller nachfolgenden Streams. Unterschiedliche Streamlängen können dabei Lücken verursachen; siehe [FFmpeg concat-Dokumentation](https://ffmpeg.org/ffmpeg-formats.html#concat).

Die nachgestellte Zusammenfügung meldet **acht Non-monotonic-DTS-Korrekturen** für AAC/AC-3. FFmpeg erhöht die betroffenen DTS teilweise nur um einen Tick. Der aktuelle Rust-Code sammelt stderr, gibt es bei erfolgreichem Exit aber nicht weiter; diese Warnungen fehlen deshalb im normalen Motorprotokoll.

Bei der echten Audio-Dekodierung dieses kleinen Beispiels wurden unter anderem **53,333 ms AAC-Lücke** und **64 ms AC-3-Lücke** sowie Paket-/Frameüberlappungen gemessen. Die Prüfung nutzt Audio-PTS und tatsächliche nb_samples bei 48 kHz. Kein neuer Lippen-Synchronitätsnachweis oder PCM-Inhaltsvergleich behauptet.

Steigende gespeicherte Paket-DTS allein beweisen damit keine saubere ursprüngliche Timeline. Die frühere Referenz-/Bytegleichheit bleibt korrekt, bestätigt aber auch diese geerbten Merkmale.

## Frame-Lupe

Prüfung über die tatsächlich gebauten CAN-Komponenten, FfprobeFrameRunner, FfmpegFramePreviewRunner und FrameLoupeViewModel:

- echte PNG-Kennung, Original-PTS und ausgewählte Schnittzeit;
- Vorwärts-/Rückwärtsschritte an Keyframes;
- Erweiterung des Analysefensters;
- späte Quellposition mit FFmpeg-Eingangsseek;
- Halbierungssuche und Zurücksetzen der Suchweite;
- zweiter schneller Aufruf während Busy: wird wie vorgesehen ignoriert, bestätigtes Bild bleibt passend.

30 Prüfungen bestanden, jeweils mit und ohne B-Frames. Geprüft wurde die Startkanten-ViewModel-/Vorschaukette, kein neuer vollständiger GUI-/Hauptplayer-End-to-End-Test. Der Nutzer meldet aktuell keine beobachtete Bildnavigation-Auffälligkeit. Der alte AVI-Problemfall wird daraus nicht als behoben ausgegeben.

## Konkreter Korrekturvorschlag

Als getrennte Versuchsvariante entwickeln, den aktiven 0.1.2-Motor und alle Referenzen erhalten:

1. Durchgehend halboffene Framebereiche **[Start, Ende)** verwenden. Planung als [S,K1), [K1,K2), [K2,E), ohne pauschales minus/plus eins. Jeder Frame gehört genau einem Teil; ein einzelnes Bild ist gültig. Keyframe-Suche am Keep-Ende respektiert dessen Ausschluss.
2. Kopierte Video-GOPs decoder-sicher begrenzen. Endgrenzen müssen auch bei PTS/DTS-Reihenfolge stimmen; -t allein darf nicht als Nachweis dienen. Benötigte Paket-/GOP-Informationen ermitteln und Teilstücke auf Bildanzahl, Bildfolge und außerhalb liegende Präsentationsbilder prüfen. Offene GOPs und Nicht-IDR-Keyframes ausdrücklich berücksichtigen.
3. Video- und Audio-Übergänge getrennt planen. Audio-Encoder-Vorlauf, Padding und Paketgrenzen berücksichtigen, bevor globale Concat-Offsets angewendet werden. Alle Tonspuren, Kanäle, Sampleraten und Bitraten erhalten. FFmpeg-Zeitstempelwarnungen sichtbar machen und im Versuch nicht als erfolgreicher Qualitätsnachweis übergehen. Über eine eventuell nötige weitergehende Audio-Neukodierung erst anhand der Ergebnisse entscheiden.
4. Die identifizierten Fälle als sinnvolle Regressionstests aufnehmen; zusätzlich 25/50 fps, nichtnulligen Quellstart und weitere GOP-/Audiokonstellationen prüfen. Danach reale Schnitte gegen Original und Cutlist kontrollieren: richtige Bilder an allen Übergängen, Sollbildzahl, unveränderte rohe PTS-Prüfung und Tonspur-/Synchronitätsprüfung. Referenztreue allein ist kein Ersatz für diesen Korrektheitsnachweis.
5. Erst nach bestandenen Prüfungen persönliche Abnahme und bewusste Übernahme in CAN. Bis dahin keine Version-1.0-Freigabe auf Grundlage der bisherigen Bytegleichheit.

## Belege und Erhaltung

Alle künstlichen Videos, nummerierten Rohbilder, Frameproben, Fehlerprotokolle, Teilstücke, Lupen-Prüfhilfe und Ergebnisse ausschließlich im ignorierten .build/quality-20261011:

- identity/RESULT.json und frame-identity.ps1;
- parts-v2/COMPARISON.json, concat.log und Teilstückmetadaten;
- AUDIO-TIMELINE.json;
- LOUPE-RESULT.json mit 30 Bild-/PTS-/Zeitprüfungen.

Zwei Fehler der eigenen Prüfhelfer wurden korrigiert und ihre ersten Protokolle erhalten: falsch übertragener Werkzeugpfad in der Lupenhilfe sowie falsch zusammengesetzte Argumente im ersten Teilstück-Helfer. Sie werden nicht als App-/Motorfehler gewertet.

Keine Produktionsquellen, Schnittparameter, Einstellungen oder Referenzfilme geändert. Native EXE 0.1.2 bleibt erhalten; keine Änderung an Buildnummer, Version, Lockdateien, Commits oder Remotes. Nur Diagnosebericht, Status und Tagebuch ergänzt. Letzter regulärer CAN-Teststand bleibt 743/743, Rust 25/25; heute wurde kein unveränderter regulärer Build allein wegen der Befunddokumentation wiederholt.