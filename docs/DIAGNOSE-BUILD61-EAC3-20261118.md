# Cut Assistant Next 1.0.3 Build 61 – Diagnose EAC3

Stand: 18.11.2026. Reiner Diagnose- und Prüfauftrag. **Keine Produktkorrektur, keine Änderung am Motor oder an der Empfehlung, keine Paketänderung und keine Veröffentlichung.**

## 1. Ergebnis

**Der verwendete otr-can-Motor unterstützt EAC3 im strengen CAN-Audiopfad ausdrücklich nicht.** Der Abbruch ist eine Codec-Zulassungsprüfung und kein nachgewiesener Defekt der Tonspur. EAC3 wird vor dem PCM-Dekodieren, dem Zusammensetzen der Audiosamples und dem Audio-Encoding zurückgewiesen.

**Die fehlende eindeutige Empfehlung hat denselben Codec als Auslöser, aber eine getrennte Ursache im CAN-Code:** Sobald irgendeine Tonspur weder AAC noch AC-3 ist, liefert die Empfehlungsfunktion den allgemeinen Text „Keine eindeutige Motor-Empfehlung möglich.“ EAC3 ist damit auch bei vollständig passenden Keyframe-Grenzen ausgeschlossen. Eine HQ-Cutlist als solche wird von dieser Funktion nicht bewertet.

Der erfolgreiche persönliche MP4Box-Schnitt bleibt als vorhandener Referenzbefund bestehen. Er wurde nicht wiederholt. Ohne separates Motorprotokoll werden keine zusätzlichen MP4Box-Paket-, Sample- oder Timingnachweise behauptet.

## 2. Untersuchte Fassung und Schutz des Paketstands

- Diagnosezweig: **codex/diagnose-eac3-build61**, angelegt auf 15579d8 (Kandidatenzweig codex/release-build61-candidate).
- Paketquellstand: 3d78a2b8490544536ef2713a58d5228f78e3e05e; letzte Produktänderung 0d4875a.
- Motor: **0.1.6-audio-diag.1**, Quellcommit **1979a7f897b95d3fab4020129b8b5e634ea569ac**.
- Motor-EXE, installiert und im eingefrorenen Paket: SHA-256 **4146EE12F81BB7A4FAEF2D4F25D742A17649959BE8C36491169055F5CF9FA4D1**.
- Zugehöriges Motorquellarchiv: SHA-256 **983C96E364E02A0F6B87BA1C6034995295E5A5E555C28CFB50BD48FB2CFAC504**.
- Installierte App-DLL und Paket-App-DLL: **FDE61D7E6BCE30E99C630DCC0DC0BE63833A0C5410F1A1CD0DE3CDC4DB941A4D**.

Die neuen C#-Diagnoseprüfungen referenzieren die unveränderten finalen Paketbibliotheken; ihre Kopien wurden bytegenau verglichen. Die Anwendung wurde weder ersetzt noch neu installiert. Setup und ZIP wurden vor und nach der Untersuchung gehasht und sind unverändert:

| Paket | SHA-256 |
| --- | --- |
| CutAssistantNext-1.0.3-Build61-win-x64-Setup.exe | 4577E66372EED3B9BA9AB7186B93D6395F5E5019392D1FCF6182F12B449BBC79 |
| CutAssistantNext-1.0.3-Build61-win-x64.zip | 90AB48891C2D99DE3C1959F2CA118FC8745662C4C42CFFB4D9A0483A806CFA11 |

Keine neue Buildnummer, keine neue RC-Bezeichnung, kein Merge nach main, kein Push, Tag, Upload oder Release. Ergänzt wurden ausschließlich dieser Bericht und separat ausführbare Diagnoseprogramme.

## 3. Konkreter Realfilmbefund

Quelle: Wetterfee_sucht_Weihnachtswunder_26.11.17_20-15_swiss1_105_TVOON_DE.HD.mp4.

Die lediglich lesende ffprobe-Metadatenabfrage bestätigt:

| Streamindex | Format | Eigenschaften |
| --- | --- | --- |
| 0 | H.264 High | 25 fps, Zeitbasis 1/12800, Start 0 |
| 1 | AAC-LC | Stereo, 48 kHz, Zeitbasis 1/48000, Start 0 |
| 2 | EAC3 | Stereo, 48 kHz, Zeitbasis 1/48000, Start 0 |

Containerdauer: 7798,44 Sekunden. Die Metadaten belegen keine vollständige Fehlerfreiheit der Audiozeitachse. Der Originalfilm wurde weder erneut geschnitten noch vollständig dekodiert; Größe und Änderungszeit blieben unverändert. Es wurde keine Original-Cutlist geschrieben oder ersetzt.

Aus dem vom Anwender übergebenen Protokoll:

~~~text
INFO: Verified 126685 video packets and raw PTS/DTS; optional full decoding complete when requested
INFO: Stage 2/3: continuous CPU audio encoding
INFO: Audio: exact kept samples, one complete encode per original track
TRACE: CAN workspace lock released
ERROR: [audio-processing-failure-v1] Audio processing rejected: Unsupported audio codec eac3
ERROR: [audio-processing-failure-v1] Cut rejected; audio details precede this marker.
~~~

Die fünf Bereiche durchliefen die Videoverarbeitung. Daraus wird keine zusätzlich nachgewiesene Vollvideodekodierung abgeleitet. Die Audioverarbeitung scheitert anschließend bei ihrer Codec-Vorprüfung. Stage 3 (Zusammenführen und abschließende MP4-Prüfung) wird nicht erreicht.

**Dies ist die ursprüngliche otr-can-Fehlermeldung. Ein FFmpeg-EAC3-Decoderfehler steht nicht im übergebenen Protokoll und wurde hier nicht erfunden.** Der Motor hat für die Tonverarbeitung noch keinen EAC3-PCM-Dekodierlauf gestartet. Auch die AAC-Spur wird wegen der vorgezogenen Validierung aller Spuren nicht zuvor vollständig verarbeitet.

## 4. Auslösende Implementierung und Unterstützungsumfang

Im zum EXE-Hash passenden Motorquellarchiv:

| Fundstelle | Bedeutung |
| --- | --- |
| src/cutting/ffmpeg.rs:198–216 | Erst Video schneiden, danach audio::cut; Audiofehler erhält den Marker audio-processing-failure-v1 |
| src/cutting/audio.rs:53–58 | Track::validate erlaubt ausschließlich aac, ac3 und mp3; andere Codecs liefern Unsupported audio codec … |
| src/cutting/audio.rs:231–247 | Audiospur-Metadaten lesen und sämtliche Spuren validieren |
| src/cutting/audio.rs:248–292 | Erst danach Arbeitsbereich, Filtergraph und FFmpeg-PCM-Dekodierung erzeugen |
| src/cutting/audio.rs:294–408 | Samplezahl, PCM-Eigenschaften und gegebenenfalls AAC-spezifische Überlappungsdiagnose |
| src/cutting/audio.rs:432–508 | Ausgabe-Codec auswählen, kodieren und Formate/Dauer prüfen |
| src/cutting/ffmpeg.rs:262–265 | AC-3-spezifische Behandlung der 1536-Sample-Framegröße im abschließenden MP4-Pfad |

Die maßgebliche Zulassung lautet:

~~~rust
matches!(self.codec_name.as_str(), "aac" | "ac3" | "mp3")
~~~

| Eingang | Strenger CAN-Pfad | Tatsächliches vorgesehenes Ausgabeformat |
| --- | --- | --- |
| AAC-LC | Zugelassen unter weiteren Format-/Sampleprüfungen | AAC, neu kodiert |
| AC-3 | Zugelassen unter weiteren Format-/Sampleprüfungen | AC-3, neu kodiert |
| MP3 | Zugelassen innerhalb seiner geprüften Mono-/Stereoklasse | AAC-LC, neu kodiert |
| EAC3 | **Vor dem PCM-Pfad abgewiesen** | **Keine freigegebene Ausgabe; keine automatische Umwandlung nach AAC** |

Die Hilfsfunktion output_codec würde für andere Namen zwar den Eingangsnamen weiterreichen. Dieser allgemeine Zweig wird für EAC3 wegen validate nicht erreicht und ist kein Unterstützungsnachweis.

Ein gesonderter FFmpeg-Lauf dekodierte die synthetische EAC3-Stereospur vollständig zu **576000 Samples je Kanal**, ohne Decoderfehler. Damit ist die Behauptung „der installierte Decoder kann EAC3 grundsätzlich nicht lesen“ für diese Testdatei widerlegt. Sie begründet weder eine Freigabe des CAN-Motorpfads noch eine Garantie für alle EAC3-Varianten oder die vollständige Originalaufnahme.

Die bestehende strenge Verifikation ist für EAC3 nicht vollständig implementiert beziehungsweise nachgewiesen. AAC-spezifische Überlappungsregeln, Codec-Framegrößen, Padding, MP4-Dauer und Endsample-Verifikation dürfen nicht ungeprüft auf EAC3 übertragen werden. Eine Erweiterung nur der Codec-Liste wäre deshalb keine hinreichend abgesicherte Korrektur.

Die Suche in den unveränderten C#- und Rust-Quellen einschließlich ihrer Tests ergab vor dieser Diagnose keine EAC3-spezifischen Testfälle. Die allgemeine Rust-Codec-Ablehnung wird bisher exemplarisch mit Opus getestet.

## 5. Bewertung der Sicherheitsmechanismen

| Entscheidung | Bewertung |
| --- | --- |
| Strengen Schnitt abbrechen | Richtig: EAC3 liegt außerhalb des freigegebenen Verfahrens; synthetisch keine fertige Ausgabe veröffentlicht |
| Keine Zeitlücke behaupten | Richtig: Der Fehler entsteht vor der Sample-/Zeitachsenanalyse; keine konkrete Lückendiagnose vorhanden |
| Keine AAC-Reparatur anbieten | Richtig: EAC3 und die originale AAC+EAC3-Mehrspurkonstellation passen nicht zur freigegebenen Einspur-AAC-LC-Stereoklasse |
| Keine ungeprüfte EAC3-Abwahl | Richtig: Nicht unterstützter Codec ist kein Nachweis einer beschädigten Spur; auch die übrigen Spuren und Zeitabdeckung müssten vollständig bewiesen sein |
| MP4Box nach ausdrücklicher Wahl anbieten | Richtig als alternatives Verfahren, ohne Reparaturgarantie oder automatischen Start; persönlicher Erfolg liegt bereits vor |
| Abbrechen als Voreinstellung | Richtig und im tatsächlichen WPF-Dialog erneut geprüft |

CAN klassifiziert den Marker in OtrCanAacEmergencyCutService.cs als allgemeinen Audiofehler. Ohne diagnostische JSON-/Lückenbefunde verwendet OtrCanAudioFailureException den allgemeinen Text. Der Satz „nicht eindeutig als reparierbare Zeitlücke nachgewiesen“ ist vorsichtig, erklärt aber die hier bereits bekannte Codec-Grenze unzureichend. Die generische Darstellung macht aus dem fehlenden EAC3-Support scheinbar ein unbestimmtes Audioproblem.

### Getrennter offener Nebenbefund: 40-ms-Raster

OtrCanAudioTrackRecovery.cs prüft Start.Ticks und End.Ticks exakt modulo 400000 (40 ms). Schon ein einzelner zusätzlicher TimeSpan-Tick (100 ns) sperrt den Pfad. Bei 50-fps-Grenzen kann außerdem ein gültiger 20-ms-Schritt außerhalb dieses engeren Recovery-Rasters liegen.

**Alle zehn im Realfilm-Motorprotokoll gedruckten Grenzen passen als exakt gelesene Dezimalwerte auf 40 ms.** Dennoch zeigt der übergebene Dialog eine Rasterabweichung. Die gedruckten Motorzeiten beziehungsweise Framegrenzen ersetzen nicht die ungerundeten C#-CutRequest-Werte. Eine Rundungsabweichung ist möglich, aber nicht nachgewiesen. Die Originalaufnahme hat 25 fps; eine Erklärung allein durch 50 fps wäre für diesen Film falsch.

Für die Erklärung dieses Nebenbefunds werden die zehn Start-/Endwerte als rohe TimeSpan.Ticks aus genau diesem Auftrag benötigt, gegebenenfalls die nach der manuellen Korrektur gespeicherte Cutlist sowie die verwendeten PTS-/Keyframe-Zeitbasen. Im Quellordner lag zum Untersuchungszeitpunkt keine gleichnamige gespeicherte Cutlist neben dem Film. Ohne diese Daten bleibt offen, ob ein Präzisionsrest oder eine andere Übergabeabweichung vorlag. Die Rasterprüfung wurde nicht verändert. Auch bei exakt passendem Raster verhindert die weitere AAC-/AC-3-Zulassung eine unbewiesene EAC3-Spur-Abwahl.

Dieser offene Nebenbefund ändert nichts an der eindeutig bewiesenen EAC3-Codec-Ablehnung des normalen Motors.

## 6. Schnittmotor-Empfehlung getrennt erklärt

CutEngineRecommendation.cs berücksichtigt analysiertes Container-/Video-/Audioformat, Behaltebereiche, Keyframe-Index und Werkzeugverfügbarkeit. Voraussetzungen der aktuellen Funktion:

- Als MP4 erkannt, genau ein H.264-Videostream mit Index 0 und positiver Bildrate.
- Jede Tonspur ist AAC oder AC-3. **Eine einzige EAC3-Spur führt sofort zur allgemeinen Antwort.**
- Keyframe-Index und mindestens ein Behaltebereich vorhanden.
- MP4Box: alle Behalteanfänge und exklusiven Enden auf Keyframes, wobei das tatsächliche Dateiende gesondert zulässig ist, und MP4Box verfügbar.
- otr-can: mindestens eine Grenze zwischen Keyframes und otr-can verfügbar.
- Bei sonstigen Konstellationen oder fehlendem passenden Werkzeug: keine eindeutige Empfehlung. Die Funktion erzwingt keinen alternativen Motor.

Die Benutzeroberfläche setzt den Text nach Aktualisierung sichtbar. Sie blendet ihn zum Beispiel während eines offenen Schnittanfangs oder ohne vollständige Schnittliste aus. Bei EAC3 allein ist der erwartete Text **„Keine eindeutige Motor-Empfehlung möglich.“**, keine EAC3-spezifische Erklärung. Ob dieser allgemeine Text im damaligen Schnittfenster tatsächlich sichtbar war, lässt sich aus dem übergebenen Fortschritts-/Fehlerdialog nicht rekonstruieren. Der Codec-bedingte fachliche Rückgabewert wurde jedoch direkt gegen die Paket-DLL reproduziert.

Die HQ-Herkunft der Cutlist ist kein Eingabeparameter der Empfehlung. Bewertet wird der nach Übernahme und manueller Bearbeitung vorhandene Plan zur geladenen HD-Aufnahme. Die K-Markierungen belegen daher nicht die ursprüngliche Passung der HQ-Cutlist. Manuell passende K-Grenzen beseitigen den unabhängigen EAC3-Ausschluss nicht.

| Frage | Befund |
| --- | --- |
| Video an Keyframes kopierbar? | Unabhängig vom Audiocodec zu bewerten; Videopfad kam im vorliegenden Log durch |
| EAC3 mit otr-can vollständig abgesichert verarbeitbar? | In diesem Motorstand ausdrücklich nicht unterstützt |
| EAC3 mit MP4Box verarbeitbar? | Für diesen Film persönlich bestanden; keine pauschale Garantie für andere Dateien abgeleitet |
| Warum keine MP4Box-Empfehlung trotz persönlichem Erfolg? | Die gemeinsame konservative Audiofreigabe blockiert beide Empfehlungszweige, bevor die Keyframes bewertet werden |

Der Ausschluss ist ausdrücklich im Code vorgesehen. Ob genau dieser pauschale Ausschluss als dauerhaft gewünschtes Produktverhalten beschlossen wurde, ist dadurch nicht belegt. Er ist für die Kommunikation zu grob, aber keine zufällige Folge der fremden Cutlist.

## 7. Reproduzierbare Prüfungen

Werkzeuge: die aktuell konfigurierten FFmpeg-/ffprobe-Dateien unter tools/ffmpeg, Git-Build 12.10.2026 (615b07170e), FFMS2-Indexer sowie der mit dem installierten Motor bytegleiche Paketmotor. Exakte Pfade, Hashes, Befehlsargumente, Standardausgabe und Fehlerausgabe sind unter Nachweise gespeichert.

Es wurden ausschließlich neue, zwölf Sekunden lange synthetische H.264-Dateien mit AAC-LC, EAC3 oder beiden Spuren erzeugt, jeweils bei **25 und 50 fps**. FFMS2 bestätigt Keyframes im Sekundenabstand. Fünf Behaltebereiche beginnen jeweils auf einem solchen Keyframe. Enden liegen entweder ebenfalls dort oder ein gültiges Bild später (40 ms bei 25 fps, 20 ms bei 50 fps).

| Je Bildrate ausgeführte Prüfung | Ergebnis |
| --- | --- |
| EAC3, fünf Bereiche mit Keyframe-Enden | Erwartete Unsupported-audio-codec-Ablehnung |
| EAC3, fünf Bereiche mit Enden zwischen Keyframes | Gleiche erwartete Codec-Ablehnung |
| EAC3, gesamter Film ohne innere Schnittgrenze | Gleiche erwartete Codec-Ablehnung |
| AAC+EAC3, fünf Bereiche | Gleiche erwartete Codec-Ablehnung vor der PCM-Verarbeitung aller Spuren |
| EAC3 mit gezielt eingefügter Zeitstempellücke | Gleiche Codec-Ablehnung; kein Nachweis einer vom Motor untersuchten Lücke |
| AAC-LC, fünf Keyframe-Bereiche | Strenger Schnitt erfolgreich |
| AAC-LC, Enden zwischen Keyframes | Strenger Schnitt erfolgreich |

Die EAC3-Zeitstempellücke wurde durch eine gezielte Verschiebung um 3072 Samples (64 ms bei 48 kHz) eingefügt und unabhängig mit ffprobe nachgewiesen. Die normale EAC3-Quelle besitzt durchgehend monotone, lückenlos anschließende Audioframe-Zeitstempel. Damit ist eine Zeitlücke keine Voraussetzung für den beobachteten Fehler.

Je Bildrate: vier Audiozeitstempelprüfungen, eine unabhängige EAC3-Dekodierprüfung und sieben strenge Motorfälle, zusammen **12 bestandene Diagnoseprüfungen**. Über beide Bildraten: **24**. Erwartete Motorablehnungen werden dabei als bestandene Negativprüfungen ausgewiesen, nicht als erfolgreiche Filmschnitte.

Weitere **24 Diagnoseprüfungen gegen die finalen Produkt-DLLs** bestanden: Empfehlung mit AAC/AC-3/EAC3/Mischton, Werkzeug-/Indexgrenzen, Fehlerklassifizierung, verweigerte Reparatur/Spur-Abwahl, exakte Rasterprüfung, geschützte Fallback-Entscheidung und tatsächlicher WPF-Dialog. Die vier Fallback-Orchestrierungsfälle verwenden Engine-Stubs; sie führen keinen zusätzlichen MP4Box-Realfilmschnitt aus.

Zusätzlich erneut ausgeführt: **66 vorhandene Regressionstests**, **66 bestanden, 0 fehlgeschlagen, 0 übersprungen**, für Empfehlung, Fallback-Dialog/-Steuerung, Audio-Diagnose und Spur-Abwahl. Die vollständige 2167er-CAN-Suite wurde für diesen produktcodefreien Diagnoseauftrag nicht erneut ausgeführt und wird nicht als neues Ergebnis ausgegeben.

Diagnoseprojekt: Release-Build **0 Warnungen, 0 Fehler**; C#-Whitespace formatiert, Python-Syntaxprüfung und Git-Whitespaceprüfung erfolgreich. Keine neue externe Abhängigkeit.

## 8. Einordnung und sichere Korrekturvorschläge

**Einordnung: allgemeine, im implementierten Motor ausdrücklich nicht unterstützte Eingangskonstellation. Kein nur filmspezifischer Fehler und kein Beweis einer defekten EAC3-Spur.** Zusätzlich bestehen Verständlichkeits-/Vorprüfungsdefizite in CAN sowie der separat noch ungeklärte Präzisions-/Rasterbefund.

Für einen gesondert freizugebenden allgemeinen Korrekturauftrag bietet sich an:

1. Vor der aufwendigen Index-/Videoverarbeitung den tatsächlichen Audiocodec mit dem unterstützten Motorspektrum abgleichen und EAC3 ausdrücklich benennen. Die Medienkompatibilität ist von der bloßen Werkzeug-Erreichbarkeit zu unterscheiden.
2. Den bekannten Codec-Ausschluss verständlich anzeigen; keine Zeitlücke oder beschädigte Spur suggerieren und keine Prüfungen lockern.
3. Video-/Keyframe-Eignung und Audioverträglichkeit pro Motor getrennt erklären. MP4Box nur anhand tatsächlich geprüfter Voraussetzungen bewerten; otr-can bei EAC3 in diesem Stand nicht empfehlen. Die Benutzerwahl bleibt bewusst.
4. Den 40-ms-Nebenbefund erst anhand ungerundeter Auftragswerte prüfen. Keine pauschale Toleranz oder Rasterumgehung einführen.
5. Eine eventuelle echte EAC3-Motorerweiterung als eigenen Auftrag behandeln: definierter Ausgabe-Codec, Sample-/PTS-Kontrolle, Codec-Framegrößen, Padding/Endton, MP4-Mux und Mehrspur-/Abbruchregressionen. Das bloße Hinzufügen zu einer Zulassungsliste genügt nicht.

**Keiner dieser Vorschläge wurde umgesetzt.**

## 9. Auswirkung auf die Build-61-Freigabe

Die bereits erfolgreichen AAC-/AC-3- und MP4Box-Abnahmen bleiben gültige Befunde. Die Sicherheitsmechanismen haben eine nicht freigegebene Weiterverarbeitung verhindert; eine vollständige EAC3-Unterstützung von otr-can darf daraus nicht zugesagt werden.

Empfehlung: **Die spätere Veröffentlichung vorerst zurückstellen, bis diese unterstützte Formatgrenze und ihre Nutzerkommunikation ausdrücklich bewertet und abgenommen sind.** Für eine Veröffentlichung mit klar dokumentierter Einschränkung muss EAC3 nicht zwingend sofort im Motor implementiert werden. Eine frühe verständliche Ablehnung und eine nachvollziehbare motorbezogene Empfehlung wären die engere, sichere Verbesserung. Ob dies noch in Build 61 aufgenommen werden soll, erfordert eine gesonderte Freigabe und gegebenenfalls neu gebaute, erneut geprüfte Pakete.

Die eingefrorenen vorhandenen Pakete bleiben unverändert; der Diagnoseauftrag ist keine Veröffentlichungsfreigabe.

## 10. Dateien und Nachprüfbarkeit

- Bericht im Diagnosezweig: docs/DIAGNOSE-BUILD61-EAC3-20261118.md.
- Reproduzierbare Diagnosequellen: tools/prototypes/Eac3Diagnosis (README, Python-Medienprüfung, C#-Policy-/Dialogprüfung).
- Lokale Übergabe: artifacts/diagnose-build61-eac3-20261118 mit Nachweisen, Original-Metadatenabfrage, User-Protokollauszug, Quell-/Werkzeugidentitäten, TRX und Integritätsvergleich.
- Keine realen Medienkopien in Git oder im Nachweispaket. Synthetische Medien liegen ausschließlich im neu angelegten temporären Prüfverzeichnis.
