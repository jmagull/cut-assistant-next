# Frame-Lupe ohne Schnittkante – 14.10.2026

Priorität 3 ausdrücklich beauftragt. Lokale Prüffassung **CAN 0.3.0 · Build 29 · RC3**, persönliche Funktionsabnahme am 14.10.2026 bestätigt. Die bereits geprüften Änderungen aus Priorität 1 und 2 sind enthalten. Öffentliche Veröffentlichung und otr-can 0.1.5 unverändert.

## Verhalten

**Frame-Lupe** und **Shift+F** verwenden dieselbe Öffnungsroutine:

- Ohne ausgewählte Schnittkante öffnet **Frame-Lupe · Untersuchungsmodus** um die vom Hauptplayer gemeldete Position. Das gilt bei leerer Schnittliste, vorhandenen abgewählten Bereichen und einer ausgewählten Bereichszeile ohne Start-/Endkante. Ein vorgemerkter Anfang bleibt erhalten.
- Laufende Wiedergabe wird wie bisher zuerst pausiert. Danach wird `PlaybackViewModel.Position` gelesen: die aus mpvs `time-pos`-Ereignissen bekannte erreichte Wiedergabeposition, unabhängig vom Sliderwert oder einem vorgemerkten Seekziel. Die bestehende Frameanalyse wählt den nächsten eindeutig zeitlich zugeordneten Frame.
- Vorschau, Frame-/Suchnavigation und technische Details verwenden dieselben bisherigen Analyse- und Vorschauwege. Fenstertitel, Ausgangsposition, Status und Bedeutungstext kennzeichnen den Untersuchungsmodus.
- **Schnittkante übernehmen** bleibt im Untersuchungsmodus deaktiviert. Das Modell verweigert auch direkte Aufrufe von `CreateSelection`; der Dialog erhält keinen Übernahme-Callback und ignoriert sogar einen künstlich ausgelösten Übernahmeklick. Es wird keine Schnittkante angelegt und keine Schnittliste verändert.
- Mit ausgewählter Start-/Endkante öffnet weiterhin die Kantenbearbeitung. Bildprüfung, AVI-Herkunft, gültige Schnittgrenzen und Übernahme bleiben erhalten.

Ohne analysiertes, im Player verfügbares Video erscheint ein Ladehinweis. Ladesperre, Schutz vor parallelem Öffnen, Medien-/Auswahlwechsel beim Pausieren und Schutz bei mehreren Videostreams bleiben aktiv.

## Architektur und Dateien

- `src/CutAssistantNext.App/ViewModels/FrameLoupeTarget.cs`: kleiner unveränderlicher Öffnungsdatensatz; unterscheidet echte Schnittkante und Untersuchungszeitpunkt, prüft Dateigrenzen und liest die Playerposition.
- `src/CutAssistantNext.App/MainWindow.xaml.cs`: bestehender Öffnungsweg verwendet nach dem Pausieren das Ziel; ausschließlich im Kantenmodus wird die bisherige Übernahme angeschlossen.
- `src/CutAssistantNext.App/ViewModels/FrameLoupeViewModel.cs`: nullable Kantenseite kennzeichnet den Untersuchungsmodus; Modustexte und dauerhaft gesperrte Übernahme. Analyse, Framezuordnung, Vorschau und Suchalgorithmus unverändert wiederverwendet.
- `src/CutAssistantNext.App/Dialogs/FrameLoupeDialog.xaml.cs`: optionaler Callback nur für Kantenmodus; zusätzlicher Übernahmeschutz. Bestehende XAML-Bindung an `CanApply` genügt, kein Layoutumbau.
- `tests/CutAssistantNext.App.Tests/FrameLoupeInspectionTests.cs`: 34 neue automatisierte Fälle. Keine bestehenden Tests entfernt oder abgeschwächt.
- Anleitung, Status und Tagebuch um die lokale Erweiterung ergänzt.

Keine zweite Lupe, künstliche Schnittkante, neue Produktionsabhängigkeit oder doppelte Analyse. Keine Änderung an Namensmasken, Schnittmotoren oder Hauptfenster-Keyframe-Navigation.

## Build und CAN-Tests

Vorhandene Abhängigkeitsdateien aus der letzten erfolgreichen lokalen Prüffassung in einen separaten ignorierten Ausgabeordner übernommen. **Kein Restore ausgeführt.** Reguläres Buildzählerskript wegen der ausdrücklich unveränderten Version nicht aufgerufen; vollständiger Release-Rebuild mit festem `CanBuildNumber=29` und `--no-restore`.

**Release-Rebuild: 0 Warnungen, 0 Fehler. 1168/1168 Tests bestanden**, keine übersprungen: Core 169, Cutlists 72, App 776, Media 151. Gegenüber dem vorigen Stand 34 zusätzliche Fälle.

Neue Tests prüfen leere/nicht ausgewählte Schnittlisten, ausgewählte Zeile ohne Kante, vorgemerkten Anfang, wiederholtes Untersuchen ohne Planänderung, beide ausgewählten Kanten mit Übernahme, erreichte Position trotz abweichendem Sliderziel, Anfang/Ende, kurzen Film, Zwischenframeposition, ungültige Positionen/Playerzustände/fehlende Dauer, Analyse-/Vorschaufehler, fehlende/mehrdeutige Zeitstempel, falschen Stream/PTS/Zeitbasis, leere Bilder, Abbruch und Laden eines weiteren Bildes. AVI-Rekonstruktion bei Containerstart 0/+2/−2 Sekunden bleibt gekennzeichnet; MP4 ohne Original-PTS bleibt gesperrt. Die bisherigen AVI- und Kantenregressionen bestehen weiterhin.

## Echte Bildanalyse und kompilierte Oberfläche

**46 Integrationsfälle bestanden**, keine WPF-Bindungsfehler:

- 24 Bildfälle mit echten ffprobe-/FFmpeg-Aufrufen: nummeriertes MP4 und drei AVI-Varianten (25 fps mit/ohne B-Frames, 50 fps mit B-Frames), jeweils sechs Positionen einschließlich Anfang, Keyframeumgebung und Nähe zum Ende; tatsächliche Bildnummer vor/nach +1 und zurück −1 geprüft.
- Vier Suchfälle: zweimal gleiche Richtung mit unveränderter Sprungweite, Richtungswechsel halbiert die Weite; tatsächliche Bildnummern geprüft.
- Drei echte MP4-Fälle am Anfang, zwischen zwei Frames und am exakten Dauerende; letzter eindeutig zugeordneter Frame am Ende verfügbar.
- Sechs kompilierte Dialogansichten: MP4-/AVI-Untersuchungsmodus und Kantenmodus jeweils 1050×1000 und 780×620. Echtes PNG geladen, Übernahmebutton erreichbar und nur im Kantenmodus aktiv. Untersuchungs-Prüfbilder visuell gesichtet.
- Drei tatsächliche Übernahmeklick-Prüfungen: künstlicher Klick auf deaktivierten Button bleibt für MP4 und AVI wirkungslos; im Kantenmodus ändert der reale Klick ausschließlich die Startkante, Ende unverändert.
- Fünf Hauptfenster-Öffnungen: leer per tatsächlichem Button, vorhandene abgewählte Liste über die gemeinsame Shift+F-Zielroutine, ausgewählte Zeile ohne Kante per Button, echte Startkante per Button, wiederholt leer. Playerzustand/-position dort simuliert (6 s), abweichendes Sliderziel 10 s; echte Analyse-/Vorschauwerkzeuge und kompilierter Dialog. Ausgangszeit stets 6 s beziehungsweise die ausgewählte 2-s-Kante, Plan unverändert, Öffnungssperre freigegeben.
- Eine Hauptfenster-Ladesperre verhindert das Öffnen.

Diese Hauptfensterprüfung simuliert den gemeldeten mpv-Zustand; kein neuer unabhängiger Vergleich zwischen tatsächlich sichtbarem Hauptplayerbild und Lupenbild behauptet. Shift+F-Verzweigung unverändert und gemeinsame Zielroutine geprüft; keine separat erzeugte native Tastatureingabe. Vorhandene kleine nummerierte Quellen wiederverwendet; keine realen Aufnahmeinhalte kopiert oder neu kodiert. Keine persönlichen Einstellungen geschrieben.

Belege unter `.build/frame-loupe-inspection-20261014`: `test-results/*.trx`, `proof/PROOF.json`, leeres `proof/BINDINGS.log`, Prüfbilder `proof/mp4-780.png` und `proof/avi-780.png` (Normalansichten ebenfalls vorhanden), isolierter Prüfhelfer unter `qa`.

## Erhaltung und Grenzen

Vier vorbestehende Produktions-Lockänderungen per SHA-256 bytegleich. Versionsdatei/Zähler und öffentliche Pakete/Prüfsummen/Tags unverändert; keine Commits, Pushes, Paketierung oder Veröffentlichung. Frühere lokale Prüffassungen erhalten.

Die bekannte Playerposition stammt aus mpv-Ereignissen und ist keine Framenummer oder Slidergeometrie. Ein absoluter Datei-Frameindex und ein unabhängiger mpv-/FFmpeg-Bildvergleich werden weiterhin nicht zugesichert. Rekonstruierte AVI-Zeitstempel bleiben Schätzwerte; fehlende oder mehrdeutige Zuordnung wird nicht als bestätigt angezeigt. Defekte Aufnahmen und die noch fehlende aktive Videospurzuordnung bei mehreren Videostreams werden nicht umgangen.

## Persönliche Prüfung

Neue separate Fassung:

`C:\Users\joerg\Documents\GitHub\cut-assistant-next\.build\frame-loupe-inspection-20261014\compiled\bin\CutAssistantNext.App\release\CutAssistantNext.App.exe`

1. HD-Aufnahme laden, keine Cutlist laden und keine Kante anlegen. Ungefähr zu **00:32:30** navigieren und **Frame-Lupe** anklicken.
2. Titel **Untersuchungsmodus**, Player-Ausgangszeit, Bildvorschau, Frame-/Suchnavigation und deaktiviertes **Schnittkante übernehmen** prüfen. Schließen: Schnittliste weiterhin leer.
3. Mit **Shift+F**, einer vorhandenen abgewählten Schnittliste und bei AVI wiederholen.
4. Anschließend eine vorhandene Start-/Endkante auswählen: bisherige Kantenbearbeitung und gültige Übernahme prüfen.

Persönliche Funktionsabnahme dieser Erweiterung mit **„check funktioniert einwandfrei :-)“** am 14.10.2026 bestätigt. Die Rückmeldung bestätigt die Bedienung insgesamt; keine gesonderte Bestätigung aller obigen Einzeltests, Formate oder Randfälle daraus abgeleitet. Nur Abnahmedokumentation, kein neuer Build/Testlauf; technischer Prüfstand unverändert. Priorität 4 bleibt zurückgestellt.
