# Halbierungssuche im Hauptfenster – 15.10.2026

Lokale Prüffassung auf Basis **0.3.0 · Build 30 · RC3**, ohne neue Versions-/Buildnummer. Die veröffentlichte Build-30-Ausgabe und ihre Pakete bleiben eingefroren. Persönliche Funktionsabnahme am 15.10.2026 positiv bestätigt.

## Aktuelle AVI-Einschränkung und Prüffassung

Die Hauptplayer-Halbierungssuche wird inzwischen bei **erkanntem AVI-Container vollständig ausgeblendet**; auch der Aufruf ist gesperrt und es wird keine Suchnavigation für AVI eingerichtet. Es entsteht keine leere Zusatzzeile. Dies ersetzt die frühere Freigabe mit nachträglichem Original-Zeitstempelhinweis. Die AVI-Lupe bleibt ebenfalls gesperrt. Der gelbe Hinweis erklärt beide Einschränkungen. Entscheidend ist der analysierte Container; AVI unter `.mp4` bleibt gesperrt, echte MP4 unter `.avi` wird freigegeben. Nach Medienwechsel zu MP4 funktioniert die Suche wieder.

Separater Release-Rebuild **0 Warnungen/0 Fehler**, **1288/1288 Tests**, 0 übersprungen. Bestehende Container-/Warnungs-/Medienwechseltests erweitert. Vier native WPF/mpv-Fälle mit echten synthetischen Medien und irreführenden Endungen bestanden: AVI-Zeile mit Höhe null ausgeblendet, direkte Suchaufrufe und künstlich ausgelöste versteckte Buttons bleiben wirkungslos und unterbrechen laufende Wiedergabe nicht. Nach MP4-Wechsel Suchzeile wieder sichtbar, tatsächlicher Suchsprung und Reset erfolgreich. Hinweisumbruch in drei Breiten und keine Bindungsfehler; Bannerbild visuell kontrolliert. Nachweise: `.build/avi-search-hidden-20261015/{build.log,tests.log,can-tests,proof/PROOF.json,proof/BINDINGS.log,preserved-before.json,preserved-after.json}`.

Geänderte Dateien: `MainWindow.xaml`, `MainWindow.xaml.cs`, `MainWindowViewModel.cs`, `AviSourceWarningDetector.cs` sowie deren bestehende Warnungs-/Container-Tests und Dokumentation. Alle acht Lockdateien sowie Version/Buildzähler bytegleich erhalten. Direkter separater Release-Rebuild ohne Restore mit festem Buildwert 30 anstelle der zählererhöhenden Build-Hilfe. Keine Änderungen an Schnittmotoren oder Zeitstempelzuordnung; die bereits eingeführte otr-can-Vorprüfung bleibt enthalten. Kein neuer Filmschnitt und keine neue persönliche MP4-Abnahme daraus abgeleitet. Öffentliches Paket unverändert.

**Aktueller Startpfad:**

`C:\Users\joerg\Documents\GitHub\cut-assistant-next\.build\avi-search-hidden-20261015\compiled\bin\CutAssistantNext.App\release\CutAssistantNext.App.exe`

Die folgenden ursprünglichen Prüfungen dokumentieren die zuerst ausgelieferte lokale Hauptplayersuche; ihre damalige AVI-Freigabe ist durch diese spätere Einschränkung ersetzt.
## Bedienung

Direkt unter der bisherigen Player-Bedienzeile steht **Halbierungssuche** mit **← 2.000 Frames**, **2.000 Frames →** und **Zurücksetzen**. Die obere Reihenfolge, Lautstärke, Positions-/Frameanzeige, Keyframe-Tasten und der Lupenbutton im Schnittlistenbereich bleiben erhalten. Die neue Gruppe bricht bei knapper Breite um.

Der Startwert kommt aus derselben Einstellung wie bei der Frame-Lupe (Standard 2000); das Hauptfenster liest ihn beim Laden des Videos. Gleiche Richtung behält die Suchweite, Richtungswechsel halbiert sie sofort mit ganzzahliger Abrundung bis mindestens 1 Frame. Jeder Richtungsbutton zeigt seine eigene nächste Weite. Zurücksetzen vergisst Richtung und Weite, ohne die Filmposition zu ändern. Slider, Einzelbild-/Mehrbildschritte, Keyframe-Sprünge und Wiedergabestart beginnen eine neue Hauptplayersuche. Die Frame-Lupe behält ihren unabhängigen Zustand.

## Umsetzung und Grenzen

- Unveränderte Core-Komponente `HalvingFrameSearch` für beide Suchzustände. Das neue `PlayerFrameSearchViewModel` nutzt pro Sprung ein vorübergehendes `FrameLoupeViewModel` im Untersuchungsmodus: bestehende Frameanalyse, tatsächliche Framezählung, Bereichserweiterung, Grenzen und bildgeprüfte Zeitstempel. Kein Dialog und keine künstliche Schnittkante. Keine pauschale FPS-Umrechnung und kein Vertrauen auf den geschätzten absoluten Player-Frameindex.
- Ausgangspunkt ist nach dem Pausieren die gemeldete Playerposition. Anschließend normaler Seek und Positionsbestätigung: innerhalb von fünf Sekunden muss der Player den Zielframe zeitlich erreichen und nach 100 ms weiterhin innerhalb weniger als des halben nächsten gemessenen Frameabstands liegen. Die sichtbaren Positions-/Framewerte stammen weiterhin ausschließlich vom Player. Der bestehende Keyframe-Weg ist unverändert.
- Ein internes `NavigationRequested`-Ereignis im Playback-Modell trennt bewusste Bedienaktionen von verspäteten Positionshinweisen. Die native Prüfung fand zunächst eine ungewollte Rücksetzung durch einen solchen Hinweis; eigener Regressionstest ergänzt. Lautstärkeänderungen setzen die Suche nicht zurück.
- Während eines Suchsprungs sind konkurrierende Playeraktionen und das Öffnen der Lupe gesperrt. Medienwechsel und Fensterschließen brechen laufende Analysen/Vorschauen ab; alte Ergebnisse werden verworfen und Ereignisabonnements freigegeben. Fehler erscheinen verständlich im Hauptfenster. Suchvorgänge verändern weder Schnittbereiche noch Auswahl oder vorgemerkten Anfang.
- **Original-Zeitstempel sind für die Hauptplayersuche erforderlich.** In der ursprünglichen Prüffassung wurden rekonstruierte AVI-Zeitstempel erst vor dem Seek abgewiesen; inzwischen ist diese Suchgruppe bei AVI bereits ausgeblendet und gesperrt, die Lupe ebenso. Der strenge native Vorlauf zeigte bei `avi-50-b.avi` tatsächlich Bild **116 statt 114** trotz plausibler Playerzeit. Deshalb werden rekonstruierte AVI-Zeiten hier ausdrücklich nicht als sichere Zuordnung behandelt. Auch fehlende, doppelte, nicht aufsteigende oder nicht bestätigte Zielinformationen verbrauchen keine Halbierung. Eine erfolglose Playerbestätigung nach bereits ausgeführtem Seek kann die erreichte Position verändern; diese wird weiter tatsächlich angezeigt.
- FFprobe und FFmpeg müssen wie bei der Lupe konfiguriert sein. Die bestehende Analyse/Vorschau kann bei großen Suchweiten und hochauflösenden Aufnahmen Zeit benötigen; kein Hintergrund-Vollindex und keine neue Abhängigkeit. Eine persönliche HD-/Bedienungsabnahme ist daraus nicht abgeleitet.

## Prüfung

- Geänderte C#-Dateien formatiert, Diffprüfung erfolgreich. Vollständiger separater Release-Rebuild ohne Restore: **0 Warnungen, 0 Fehler**. Direkter Build mit festem `CanBuildNumber=30`, da `tools/build.ps1` entgegen dem Auftrag den Zähler erhöhen würde.
- **1244/1244 CAN-Tests bestanden**, 0 übersprungen: Core 196, Cutlists 72, App 823, Media 153. **29 zusätzliche Fälle** gegenüber dem Ausgangsstand. Suchfolgen, Abrundung/Minimum, wiederholtes Reset ohne Seek, unveränderte Schnittliste/Markierung, Grenzen/einzelner Frame, variable Zeitabstände und Containerstart, manuelle Navigation, Medienwechsel/Abbruch, Fehler, parallele Aufrufe, unabhängige Lupe, verspätete Ereignisse sowie kleine Uhrenabweichung gegenüber falschem Zielframe geprüft. Zwei anfängliche neue Testaufbaufehler (ungültige Kantenkombination, 100-ns-Rundung des bestehenden Sliderwegs) korrigiert; keine vorbestehenden Tests entfernt oder abgeschwächt.
- **30 native Integrationsfälle bestanden**, keine WPF-Bindungsfehler: zehn exakte Zielbildvergleiche über tatsächlich aus libmpv ausgelesene Bildnummern für MP4 und VFR, zwei Default-2000-Sprünge zur letzten echten Bildposition, Keyframe-Navigation/K-Kennzeichnung, Reset/Bildnavigation/unveränderte Schnittliste/unabhängiger Lupenweg, drei sichere AVI-Abweisungen mit anschließend weiterhin funktionierender AVI-Lupe und zwölf Layoutfälle. Layoutgrößen **1533×900, 960×780, 760×600**, jeweils Suchgruppe mit **100/125/150/200 % Layoutvergrößerung**; dies ist keine umgeschaltete Windows-DPI-Sitzung. Normalansicht und kleine 200-%-Ansicht visuell gesichtet, Buttons vollständig und über bestehenden Scrollbereich erreichbar.
- Native Vorbefunde bleiben unter `proof/AVI-50-UNSAFE-MAPPING.json` erhalten. Der lokale Prüfhelfer benötigte zunächst die vier nativen DLLs im eigenen Laufzeitordner; anschließend fehlende AVI-Endzeitstempel und beim Layoutvergleich die WPF-Außenabstände korrekt berücksichtigt. Der endgültige Prüflauf steht in `proof/PROOF.json`; frühere fehlgeschlagene Läufe zählen nicht als Erfolg.
- Alle **acht Lockdateien SHA-256-bytegleich** zum Beginn, einschließlich der vier vorbestehenden Produktionsänderungen. Buildzähler **30 → 30**, `Version.props` unverändert. Kein Restore, Commit, Push, Setup, Portable-Paket oder Release. Schnittmotoren und öffentliche Ausgaben unverändert.

Belege: `.build/main-halving-search-20261015/build.log`, `final-tests`, `tests-accepted.log`, `proof/PROOF.json`, `proof/BINDINGS.log` und Layoutbilder im selben `proof`-Ordner. Prüfhilfen und Medien bleiben ausschließlich im ignorierten `.build`-Verzeichnis.

## Persönliche Prüfung

Startdatei:

`C:\Users\joerg\Documents\GitHub\cut-assistant-next\.build\main-halving-search-20261015\compiled\bin\CutAssistantNext.App\release\CutAssistantNext.App.exe`

HD-MP4 laden, Slider verwenden, zweimal rechts und einmal links suchen, mehrere Richtungswechsel testen. Danach mit den K-Tasten präzisieren und einen Schnittpunkt setzen. Zurücksetzen muss die aktuelle Position erhalten. Nach manueller Navigation beginnt die Suche wieder mit dem Startwert. Anschließend die bisherige Frame-Lupe prüfen. Bei AVI ohne Original-Zeitstempel muss die neue Hauptplayersuche verständlich auf die Lupe verweisen und die Position erhalten.

Persönliche Funktionsabnahme am 15.10.2026 positiv bestätigt. Die Prüffassung trägt weiterhin die vorhandene Versionsanzeige; zur Unterscheidung vom veröffentlichten Programm ausschließlich den obigen separaten Pfad verwenden.

## Persönliche Abnahme – 15.10.2026

Der Nutzer bestätigt die Halbierungssuche als prächtig funktionierend und die Navigation in alle Richtungen mit check. Größere Bilder werden als langsam beschrieben; keine eigene Laufzeitmessung oder neue allgemeine AVI-Freigabe daraus abgeleitet. Persönliche Funktionsabnahme positiv abgeschlossen. Der neue AVI-Zeitstempelfall wird gesondert [untersucht](AVI-ZEITSTEMPEL-NO-TURNING-BACK-20261015.md).