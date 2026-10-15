# OTR-CAN-Vorprüfung der Bildzeiten – 15.10.2026

## Ergänzung: Nullstart und vollständiges CFR-Raster

Der neue Nutzerfall **Navy_CIS_26.09.20_20-15_5plus_50_TVOON_DE.mpg.HD.avi.mp4** passiert die reine Monotonieprüfung mit **112423** Bildzeiten, wird von otr-can 0.1.5 aber mit `Experiment supports 25/50 fps CFR only` abgelehnt. Der Inhalt ist AVI trotz `.mp4`; H.264 1920×1080, als interlaced gekennzeichnet, Ton MP2 und AC-3. Die nominelle 50-fps-Angabe im AVI und mittlere MP4-Bildrate **24,990997** entscheiden nicht über die Freigabe.

An einer separaten unveränderten Stream-Copy-MP4 nachgewiesen: FFMS2 beginnt mit **0, 840, 880, 920, 960, 1000 ms**. Die Folge steigt eindeutig an, passt aber nicht zum vom Motor verlangten Zeitraster. Das belegt die konkrete Zeitstrukturgrenze; weder eine allgemeine AVI-/Interlace-Sperre noch eine Ursache fehlender Bildinhalte wird daraus abgeleitet. Original-SHA-256 vor/nach Diagnose identisch: `EED65F11EFD7829ACF7B370B3049DD8F6861374A082A28CF66A12FA5364E6579`.

Die Vorprüfung verwendet jetzt zusätzlich `ValidateForOtrCanAsync`: dieselbe Umrechnung in Mikrosekunden und dieselben Bedingungen wie `Metadata.experimental_cfr_step` im unveränderten Motor 0.1.5 (`can-engine/src/cutting/info.rs`): mindestens zwei Bildzeiten, erster Wert exakt null, zweiter Wert mit Abstand **unter 1 µs** zu **20 oder 40 ms**, sämtliche Zeiten mit höchstens **2 µs** Abweichung vom Raster `Bildindex × zweiter Zeitwert`. Die ganze Folge wird geprüft; bei mehrdeutigen Zeiten bleibt die bisherige speziellere Diagnose vorrangig. Der allgemeine Validator unterstützt weiterhin monotone VFR-/Offset-Zeiten, diese allein geben den Motor jedoch nicht mehr frei.

Bei gültigen, aber vom Motor nicht unterstützten Zeiten erscheint vor dem Schnittaufruf:

> OTR-CAN wurde nicht gestartet: Diese Aufnahme hat keine vom Motor unterstützte Zeitstruktur. Er benötigt konstante 25 oder 50 Bilder pro Sekunde ab Zeit null. Bitte verwende gegebenenfalls MP4Box und prüfe anschließend Bild, Ton und Schnittübergänge.

Keine Änderung an FPS, Bildern, Ton, Original, Vorbereitung, MP4Box oder Motor. Kein automatischer Wechsel und keine Reparatur. Weitere Motorgrenzen, insbesondere zulässige Audio-/Videocodecs, bleiben eigenständige Prüfungen; die neue Rasterprüfung garantiert keine allgemeine Verarbeitbarkeit. Für diese Navy-Aufnahme ist aus dem neuen Protokoll kein erfolgreicher MP4Box-Schnitt oder Bild-/Ton-/Übergangsnachweis abgeleitet.

Geändert: Validator, Aufruf/Meldung im OTR-CAN-Schnittdienst und deren Tests; Dokumentation. Separater Release-Rebuild **0 Warnungen/0 Fehler**, **1308/1308 Tests**, 0 übersprungen (Core 196, Cutlists 72, Media 194, App 846). **20 zusätzliche Fälle** prüfen 25/50 fps, zulässigen Jitter, VFR, Offset, 24/30 fps, Einzelframe, Rasterfehler am Filmende, nominelle 50-fps-Metadaten und Erhalt vorhandener Dateien. Der bisherige VFR-Fall wurde vom Motor-Erfolgstest in die ausdrückliche Ablehnungsprüfung übernommen; die allgemeine VFR-Validatorprüfung bleibt erhalten.

Sechs reale Indexvergleiche bestanden: Navy streng abgelehnt trotz monotoner Zeiten; Teufelsbrigade mit 263694 Zeiten weiterhin freigegeben; No Turning Back und seine vorbereitete/MP4Box-Ausgabe weiterhin wegen Doppelzeiten gesperrt. Auch die alte 12-s-SEI-Diagnosekopie ist mit **0, 60, 80 … ms** nicht motorfähig: Sie war nur für monotone Zeiten positiv geprüft und bleibt keine freigegebene Reparatur. Der erste Vergleichshelfer enthielt dafür noch die alte reine Monotonie-Erwartung; diese wurde anhand der unveränderten Zeitcodedatei für den zusätzlichen CFR-Modus korrigiert, ohne Produktprüfung zu lockern.

Zusätzlich Produktionsschnittdienst und echter FFMS2-Indexer an **drei Quellen**: Navy und No Turning Back starten keinen Motor; Teufelsbrigade passiert die Freigabe (folgender Motoraufruf im Helfer bewusst abgefangen). Je genau ein Indexlauf, vorhandene Testziele unverändert und Arbeitsordner aufgeräumt. Kein neuer vollständiger Filmschnitt. Nachweise unter `.build/otr-can-cfr-preflight-20261015`: `build.log`, `tests.log`, `proof/PROOF.json`, `proof/navy-time-analysis.json`, Quellhashvergleich und Lock-/Versionsvergleich. Alle acht Lockdateien und Version/Buildzähler erhalten. **0.3.0/30/RC3**, separater direkter Rebuild mit festem Buildwert statt zählererhöhender Build-Hilfe, kein Restore, Commit, Push, Paket oder Release.

**Aktuelle Prüffassung**, einschließlich AVI-Suchzeile ausgeblendet:

`C:\Users\joerg\Documents\GitHub\cut-assistant-next\.build\otr-can-cfr-preflight-20261015\compiled\bin\CutAssistantNext.App\release\CutAssistantNext.App.exe`

Die folgenden Abschnitte dokumentieren die erste reine Monotonie-Vorprüfung; für die aktuelle Motorfreigabe gelten zusätzlich die obigen CFR-Anforderungen.
## Verhalten der ersten Vorprüfung

CAN prüft nach der ohnehin nötigen FFMS2-Indexierung und vor dem Aufruf `cut-can` die Bildzeiten der **tatsächlich an den Motor übergebenen Schnittquelle**. Bei AVI mit MP4-Vorbereitung wird der Index der vorbereiteten MP4 geprüft. Es gibt keine zweite Indexierung und keine zusätzliche Neukodierung.

Die Prüfung verlangt FFMS2-Zeitcodeformat v2, mindestens eine gültige, endliche und nicht negative Bildzeit sowie eine streng aufsteigende Folge. Dabei wird dieselbe Umrechnung von Millisekunden über Sekunden in abgeschnittene Mikrosekunden wie im unveränderten otr-can 0.1.5 verwendet. Auch Werte, die erst in dieser Genauigkeit zusammenfallen, sperren den Schnitt. Variable Bildabstände und ein positiver Zeitversatz sind erlaubt; keine pauschale FPS-Annahme. Die Quelldatei und Zeitcodedatei werden nicht geändert.

Bei ungeeigneten Zeiten startet CAN den Schnittmotor nicht und zeigt:

> OTR-CAN wurde nicht gestartet: Für diese Datei liegen keine verlässlich aufsteigenden Bildzeiten vor. Bitte verwende gegebenenfalls MP4Box und prüfe anschließend Bild, Ton und Schnittübergänge.

Das Protokoll ergänzt den ersten beanstandeten Frame beziehungsweise die ungültige Indexzeile. Bestehende Zieldateien werden nicht ersetzt; der private Arbeitsordner wird wie bisher aufgeräumt. Der Vorgang bleibt abbrechbar und läuft außerhalb des WPF-UI-Threads. Es gibt keinen automatischen Wechsel zu MP4Box.

## Umfang und Grenzen

Dies ist eine gezielte Sperre für nicht zuverlässig zuordenbare Bildzeiten, keine allgemeine AVI-Sperre. Sie gilt auch für MP4 mit demselben Fehler. Die AVI-Lupensperre bleibt eine separate Bedienentscheidung. Eine fehlende Keyframe-Leiste ist kein Diagnoseergebnis für den Schnittmotor.

Die Entscheidung fällt erst nach Vorbereitung/Indexierung. Werkzeugprüfungen und FFMS2 laufen zuvor weiter; verhindert wird der eigentliche `cut-can`-Schnittaufruf. Andere Motorgrenzen, fehlerhafte Keyframe-Indizes, Decoderfehler oder Audioprobleme werden dadurch nicht allgemein vorab ausgeschlossen. Ein positiver Zeitstempeltest garantiert keine fehlerfreien Übergänge. MP4Box, Vorbereitung, Motor 0.1.5 und seine eigenen Schutzprüfungen bleiben unverändert.

## Änderung und Prüfung

- `src/CutAssistantNext.Media/Analysis/FfmsTimecodeValidator.cs`: asynchrone, abbrechbare Reader-Prüfung, ohne Dateien oder Prozesse selbst zu besitzen.
- `src/CutAssistantNext.App/Services/Cutting/OtrCanCutService.cs`: Vorprüfung nach Indexierung, verständliche Meldung und Abbruch vor Motorstart.
- Neue Validator-Tests und erweiterte Schnittdienst-Tests; zwei bestehende Workflow-Testhelfer erzeugen jetzt gültige Zeitcode-Testdaten anstelle des bisherigen Platzhaltertexts. Keine Tests entfernt oder abgeschwächt.

Separater Release-Rebuild: **0 Warnungen, 0 Fehler**. **1288/1288 Tests bestanden**, 0 übersprungen: Core 196, Cutlists 72, Media 180, App 840. **35 zusätzliche Fälle**: doppelte/rückläufige Zeiten, Mikrosekunden-Kollisionen, nicht endliche/negative/fehlerhafte Werte, fehlende Daten/Kopfzeile, CFR/VFR/Offset, deutsche Umgebung, Abbruch, Motoraufrufsperre und Erhalt von Original, Arbeitsquelle sowie bestehendem Ziel.

Fünf vorhandene reale FFMS2-Zeitcodedateien wurden mit Hashvergleich unverändert geprüft:

| Zeitindex | Ergebnis |
| --- | --- |
| No Turning Back, Original-AVI | gesperrt, erste Kollision bei Bild 257, Zeile 258 |
| No Turning Back, reguläre MP4-Arbeitskopie | ebenfalls bei Bild 257 gesperrt |
| Vorhandene technisch erfolgreiche MP4Box-Ausgabe | gesperrt, erste Kollision bei Bild 7 |
| Teufelsbrigade-AVI | alle 263694 Zeiten freigegeben |
| Frühere 12-s-Diagnosekopie ohne SEI | 598 Zeiten freigegeben; weiterhin keine freigegebene Reparatur |

Zusätzlich Produktionsschnittdienst mit **echtem FFMS2-Indexierer** geprüft: No Turning Back mit Original-AVI und regulärer vorbereiteter MP4 stoppt nach genau einem Indexlauf und **ohne Motoraufruf**. Teufelsbrigade-Originalindex passiert nach genau einem Indexlauf die neue Prüfung; der folgende Motoraufruf wird im Prüfhelfer gezielt abgefangen. In beiden Fällen bleiben vorhandene Testziele unverändert und Arbeitsordner werden vollständig entfernt. Kein neuer vollständiger Filmschnitt und keine neue Bild-/Ton-/Übergangsabnahme daraus abgeleitet; die persönliche erfolgreiche Teufelsbrigade-Abnahme bleibt der separate Nutzernachweis.

Nachweise unter `.build/otr-can-preflight-20261015`: `build.log`, `tests.log`, `can-tests/`, `proof/PROOF.json`, `preserved-before.json`, `preserved-after.json`. Alle acht Lockdateien sowie Version und Buildzähler SHA-256-identisch zum Beginn dieser Änderung. Build **0.3.0/30/RC3** unverändert, kein Restore, Commit, Push, Paket oder Release. Direkter separater Build statt der zählererhöhenden Build-Hilfe: `dotnet build ... --configuration Release --no-restore -t:Rebuild --artifacts-path ... -p:CanBuildNumber=30`.

## Separate Prüffassung

`C:\Users\joerg\Documents\GitHub\cut-assistant-next\.build\otr-can-preflight-20261015\compiled\bin\CutAssistantNext.App\release\CutAssistantNext.App.exe`

Enthält außerdem die bereits geprüfte Hauptplayer-Halbierungssuche und die AVI-Lupensperre. Öffentliche Build-30-Pakete werden nicht verändert.