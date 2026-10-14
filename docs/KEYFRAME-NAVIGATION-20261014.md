# Keyframe-Navigation im Hauptfenster – 14.10.2026

**Priorität 4 zur persönlichen Prüfung.** Lokale Fassung **CAN 0.3.0 · Build 29 · RC3**. Nutzer gibt kompakte K-Buttons, blaue Striche im bisherigen Schnittstreifen und `(K)` frei. Anschließend ausdrücklich vereinfacht: **keine Keyframe-Funktion bei AVI**, mit Erklärung im vorhandenen AVI-Infoblock. Diese Festlegung ersetzt die ursprüngliche AVI-Keyframe-Anforderung.

## Sichtbare Änderung

- Neben den Bildschritt-Buttons stehen **◀ K** und **K ▶**, jeweils mit Tooltip für vorherigen/nächsten Keyframe und Analysezustand. Es wird keine zusätzliche feste Bedienzeile angelegt; der vorhandene Umbruch und Scrollbereich bleiben erhalten.
- Die blauen Bereiche am oberen Rand des grauen Schnittstreifens zeigen die Keyframe-Zeiten. Der Hinweistext lautet **„Blaue Bereiche: Keyframes“**. Rote Entfernbereiche und deren Auswahl-/Seekverhalten bleiben erhalten. Mehrere Markierungen derselben Bildspalte werden zusammengefasst; einzelne Frames lassen sich über die K-Buttons exakt ansteuern.
- **(K)** steht neben der Framenummer, wenn der Player pausiert ist und seine gemeldete Position einer analysierten Keyframe-PTS entspricht. Während Sliderziehen, Wiedergabe und laufendem Keyframe-Sprung bleibt es ausgeblendet. Keine Kennzeichnung anhand des Bildtyps I allein.
- **AVI:** K-Buttons vollständig ausgeblendet, keine blauen Markierungen und kein `(K)`. Auch keine Hintergrund-Keyframe-Analyse. Der bestehende gelbe Hinweis enthält: **„Die Keyframe-Anzeige und Keyframe-Navigation im Hauptfenster sind für AVI nicht verfügbar. Die Frame-Lupe bleibt nutzbar.“** Maßgeblich ist der erkannte Container, auch bei irreführender Dateiendung.
- Die vorher abgenommene AVI-Frame-Lupe samt Untersuchungs-/Kantenmodus und rekonstruierter Vorschau bleibt erhalten. Schnittmotoren, Schnittlistenbearbeitung, Namensmasken und Halbierungssuche unverändert.

## Technische Lösung

Einmal pro Dateiladen wird im Hintergrund ein Index aufgebaut. Der vorhandene `FfprobeFrameRunner` und `FfprobeFrameJsonParser` werden wiederverwendet; der neue Indexweg nutzt `-skip_frame nokey -skip_idct all` und prüft zusätzlich ausdrücklich `key_frame=1` und vorhandene Original-PTS. Best-Effort-/DTS-Werte ersetzen keine Originalzeit. Unvollständige, mehrdeutige oder nicht aufsteigende Keyframe-Zeiten und Decoderfehler geben keine Navigation frei. Der Containerstart wird wie bisher abgezogen.

Der Core-Index sucht binär nach streng vorherigem/nächstem Keyframe; die Rundungstoleranz beträgt nur 0,5 ms. Das App-Modell pausiert laufende Wiedergabe, verwendet die erreichte Playerposition und wartet auf deren Bestätigung nach dem normalen Seek. Es sperrt parallele Sprünge, bricht Analysen bei Dateiwechsel/Schließen ab und verwirft verspätete Resultate. Fehler blockieren weder die normale Wiedergabe noch das Schneiden. Der Index bleibt im Speicher; keine zusätzliche Datei neben dem Video.

Neue Produktionsdateien: `VideoKeyframeIndex.cs`, `IKeyframeAnalysisRunner.cs`, `KeyframeNavigationViewModel.cs`. Geändert: `FfprobeFrameRunner.cs`, `CutTimelineTrack.cs`, `MainWindow.xaml/.cs` und `AviSourceWarningDetector.cs`. Tests ergänzen Core-Zeitzuordnung, Modell-Lebenszyklus, AVI-Ausschluss und Prozessargumente; vorhandener AVI-Hinweistest verstärkt. Der vorhandene Playback-Testhelfer ist zur Wiederverwendung intern sichtbar. **Keine Änderung am produktiven mpv-Service oder Playback-Modell**, keine neue Abhängigkeit.

## Prüfung der Endfassung

- C#-Formatierung und Diffprüfung erfolgreich. Vorhandene Abhängigkeitsdateien in separater ignorierter Ausgabe verwendet; **kein Restore**. Vollständiger Release-Rebuild mit fester Buildnummer 29: **0 Warnungen, 0 Fehler**.
- **1215/1215 Tests**, keine übersprungen: Core 196, Cutlists 72, App 794, Media 153. **47 zusätzliche Fälle** gegenüber der Priorität-3-Fassung. Vor Aufgabenbeginn vorhandene Tests nicht entfernt oder abgeschwächt. Neue AVI-Experimente entsprechend der ausdrücklich geänderten Aufgabenabgrenzung durch Ausschlussprüfungen ersetzt.
- **18 isolierte Integrationsprüfungen:** echte mpv-Instanz und tatsächlich ausgelöste K-Buttons, acht Vor-/Rücksprünge einer nummerierten MP4 vor/nach AVI-Wechsel; jedes tatsächlich von mpv dargestellte Bild unabhängig aus dem Rohbildspeicher ausgelesen und mit der analysierten Bildnummer verglichen. Abgehen vom Keyframe entfernt `(K)`, Schnittplan bleibt gleich.
- Drei AVI-Varianten (25 fps mit/ohne B-Frames, 50 fps mit B-Frames): K-Buttons `Collapsed`, kein Index/keine Marker/kein `(K)`, kein Analyseaufruf; neuer Infotext sichtbar. Jeweils echter bisheriger ffprobe-/FFmpeg-Lupenweg im Untersuchungs- und Kantenmodus erfolgreich; nur im Kantenmodus Übernahme verfügbar.
- Hauptfenster bei **1533×900, 960×780, 760×600**: K-Buttons erreichbar, bei kleinen Höhen über den vorhandenen Scrollbereich. Rote Flächen und blaue obere Striche zusätzlich anhand gerenderter Pixel geprüft. MP4-/AVI-Prüfbilder visuell gesichtet. **Keine WPF-Bindungsfehler**, keine persönlichen Einstellungen geschrieben.
- Lange HD-MP4 **Die Hart 2**, Dauer **03:05:00.640**: vollständige reine Hintergrundanalyse erfolgreich, **3448 Keyframes in rund 26,6 s**. Ein davor bewusst nach 25 s abgebrochener Zeitmesslauf bestätigt den Abbruchweg; nicht als abgeschlossene Analyse gezählt. Für lange Aufnahmen braucht die Analyse also merklich Zeit. Der Player bleibt währenddessen bedienbar, K-Buttons bis zum Abschluss deaktiviert; Tooltip zeigt den Zustand. Keine Ergebnisdatei geschnitten oder persönliche Videoinhalte kopiert.

Belege unter `.build/keyframe-navigation-20261014`: finale CAN-Tests in `accepted-scope-tests`, native Ergebnisse/Prüfbilder/Bindungslog in `final-proof`, vollständige Zeitmessung `final-proof/LONG-MP4.json`, isolierter Prüfhelfer unter `qa`.

## Vorbefunde und Abgrenzung

Der schnelle reine Keyframe-Decoderweg verschob bei AVI rekonstruierte Zeiten. Eine vollständige AVI-Probe beseitigte das nicht allgemein: echte mpv-Prüfung fand zunächst einen kleinen Uhrenversatz und bei einer 50-fps-Variante ein abweichendes Bild. Eine zusätzliche Erfassung des initialen Playerzeitpunkts wurde untersucht. **Auf ausdrücklichen Nutzerwunsch sind alle zusätzlichen AVI-Ursprungs-/Toleranz-/Playeränderungen aus der Endfassung entfernt.** Vorbefunde und experimenteller Prüfhelfer bleiben ausschließlich lokal unter `proof` bzw. `qa/AVI-exploration.cs.txt` erhalten.

Die automatische Freigabeprüfung lehnte zwischenzeitlich das vorgeschlagene Überspringen zweier AVI-Prüfbedingungen ab. Diese Bedingungen wurden nicht übersprungen; stattdessen wurde die Bilddiagnose vorgezogen und der Fehler nachgewiesen. Der spätere AVI-Ausschluss ist die ausdrückliche Nutzerentscheidung und wird eigenständig getestet. MP4-Bildvergleiche bleiben streng.

Prüfhelfer benötigte zunächst die native Bibliothek im eigenen Laufzeitordner. Der gebündelte mpv besitzt keinen PNG-Encoder; deshalb tatsächliches Rohbild über die native Client-API ausgelesen und nur im Prüfhelfer mit WPF gespeichert. Anfangs falsche Namensräume und eine Layoutprüfung ohne Berücksichtigung des vorhandenen Scrollbereichs wurden im Helfer korrigiert. Keine solche Rohbild-/Diagnoselogik in der Anwendung.

Technische Referenzen: [ffprobe-Frameausgabe](https://ffmpeg.org/ffprobe.html), [Decoderoptionen](https://ffmpeg.org/ffprobe-all.html), [mpv-Zeit-/Seek-/Rohbildschnittstelle](https://mpv.io/manual/stable/), [libmpv-Client-API 0.41.0](https://github.com/mpv-player/mpv/blob/v0.41.0/include/mpv/client.h). Die Prüfungen bestätigen die verwendeten MP4-Beispiele; keine allgemeine Zusage für alle Codecs, defekten Dateien oder Dateiformate. Mehrere Videostreams bleiben ohne aktive Spurzuordnung gesperrt.

## Nachtrag – K direkt an der aktuellen Framenummer

Auf Nutzerwunsch steht das blaue K nun eindeutig bei der aktuellen Zahl: **Frame 36.206 (K) / ca. 210.000**. Nur `MainWindow.xaml`, Anzeigeprojektionen im `PlaybackViewModel` und bestehende Format-/Benachrichtigungsprüfungen angepasst; Erkennung, Navigation und AVI-Ausschluss unverändert.

Neue separate Prüffassung unter `.build/keyframe-indicator-position-20261014/compiled/bin/CutAssistantNext.App/release`. Vollständiger Release-Rebuild **0 Warnungen/0 Fehler**, maßgebliche **1215/1215 Tests** nach korrigiertem Vorprüf-Assert in `final-test-results`. Vorzeitiger Lauf gegen kopierte alte Ausgaben wird nicht als Beleg verwendet. Echter kompilierter WPF-Anzeigetest mit K/ohne K/unbekannten Framezahlen, Reihenfolge auch geometrisch geprüft; AVI ohne K, keine Bindungsfehler. Belege und gesichtetes `proof/current-frame-k.png` im selben Aufgabenordner. Nur isolierte Testfenster, keine persönlichen Einstellungen geschrieben.

Die zuvor angesprochene dichte Keyframe-Spur bleibt unverändert: Blaue Markierungen können bei langen Filmen zusammenwachsen; helle Stellen sind Lücken. Diese Darstellungsfrage ist kein Teil der K-Positionskorrektur. Persönliche Prüfung der neuen Anordnung offen; bisherige Prüffassungen, Version/Zähler und Lockdateien erhalten, kein Restore/Commit/Push/Release.

## Nachtrag – Farblegende „Blaue Bereiche: Keyframes“

Der Nutzer wünscht diese Formulierung statt „Blaue Striche“. Der Status-/Tooltiptext lautet nun **„Blaue Bereiche: Keyframes; rote Flächen: Entfernbereiche.“** Zeichenlogik und K-Anordnung unverändert; Anleitung entsprechend angepasst. Neue separate Fassung unter `.build/keyframe-label-20261014/compiled/bin/CutAssistantNext.App/release`, vollständiger Release-Rebuild ohne Warnungen/Fehler, **1215/1215 Tests**, keine übersprungen. Testbelege in `.build/keyframe-label-20261014/test-results`. Kein Restore, Versions-/Lockdateiwechsel, Commit, Push oder Release.

## Persönliche Prüfung

Separate Prüffassung:

`C:\Users\joerg\Documents\GitHub\cut-assistant-next\.build\keyframe-label-20261014\compiled\bin\CutAssistantNext.App\release\CutAssistantNext.App.exe`

MP4 laden, Hintergrundanalyse abwarten (Tooltip über K-Buttons), blaue Bereiche und beide Sprungrichtungen prüfen. Auf erreichtem Keyframe erscheint `(K)`, nach einem Bildschritt verschwindet es. AVI laden: neuer Hinweis oben, keine Keyframe-Elemente; die Frame-Lupe weiterhin nutzbar. Danach zurück zu MP4: Keyframe-Funktion nach Analyse wieder sichtbar.

Persönliche Abnahme offen. Vier vorbestehende Produktions-Lockänderungen, Version/Buildnummer **0.3.0/29/RC3**, bisherige Prioritäten 1–3 sowie öffentliche Pakete/Tags/Prüfsummen erhalten. Keine Commits, Pushes, Paketierung oder Veröffentlichung.
