# Build 55 – Sekundensteuerung im CAN-Hauptplayer

**Nachtrag zur Videoende-Logik:** Die frühere Sondermarkierung „letztes Behaltebild“ ist ersetzt. **Schnitt bis Videoende abschließen** beendet jetzt ausschließlich einen offenen Entfernen-Bereich an der tatsächlichen Mediendauer. Aktueller Stand und Prüfergebnisse im [EOF-Korrekturbericht](SPOTLIGHT-VIDEOENDE-BUILD55.md). Die nachstehenden Angaben zur früheren Videoende-Sondermarkierung sind historisch.

## Ergebnis

Gezielte Korrektur innerhalb derselben Entwicklungsfassung **Cut Assistant Next 1.0.3 Build 55**, derselben Arbeitskopie und desselben bestehenden Entwicklungszweigs. Sekundensteuerung aus Spotlight entfernt und in die Hauptplayerleiste eingebunden. Keine Änderung der Versionsdatei, Solution, Projekt-/EXE-Namen, Paketierung oder Motoren. Kein neues Produkt, Repository, Setup, Release, Merge nach main oder Push; nichts installiert. Die ursprüngliche Arbeitskopie mit Nutzeränderungen bleibt unberührt.

## Bedienung

Von links: **Play/Pause → Sekunden zurück → editierbare Sprungweite → Sekunden vor → Bild zurück → Bild vor → Keyframe zurück → Keyframe vor → Lautstärke und übrige Anzeigen**. Die bisherigen −10/+10-Bilder-Buttons sind entfernt; deren Strg-Klick- und Beschriftungsumschaltung entfällt. Standard 10 Sekunden, Auswahl 1/5/10/30/60; eigene positive Sekundenwerte einschließlich Dezimalpunkt oder -komma. Derselbe vorhandene libmpv-Player und PlaybackViewModel.SeekToAsync werden verwendet. Dateigrenzen werden eingehalten; laufende/pausierte Wiedergabe bleibt erhalten. Ungültige Eingaben lösen keinen Sprung aus und verwerfen nicht die letzte gültige Einstellung.

Speicherung im vorhandenen CAN-Einstellungsobjekt WindowSettings, Feld SeekStepSeconds, in window-settings.json neben Größe und Lautstärke. Wiederherstellung beim CAN-Start; fehlende/ungültige Werte ergeben 10. Normales Schließen speichert auch eine Auswahl ohne vorherigen Sprung. Keine neue Einstellungsdatei. Die alte experimentelle spotlight-navigation-settings.json wird nicht mehr gelesen oder geschrieben; eine bereits lokal vorhandene Datei wird nicht ungefragt gelöscht.

Links/Rechts behält die Einzelbildnavigation bei Pause. Mit dem ergänzten Auftrag entfällt auch die alte 20-Bilder-Tastatursteuerung über Strg+Links/Rechts; die nicht mehr benötigten 10-/20-Bilder-Methoden des PlaybackViewModel sind entfernt. Die Sekundensteuerung verwendet ausschließlich die sichtbaren Buttons und das Dropdown, ohne neue Tastenkombination und ohne Strg-Verdoppelung. Eingabefelder und editierbare Auswahlfelder behalten ihre Pfeiltasten. Keyframes, Halbierungssuche und Frame-Lupe verwenden ihre bisherigen Implementierungen. Bloße Navigation verändert keine Schnittpositionen.

## Direkte Suchmechanik und Symmetrie

Die neue Sekundensteuerung verwendet unverändert den bereits in der ersten Build-55-Fassung eingesetzten Weg PlaybackViewModel.SeekToAsync → MpvMediaPlayerService.SeekAsync → SetDoublePropertyAsync("time-pos", Zielsekunden). Vorwärts und rückwärts unterscheiden sich ausschließlich in der Zielposition. Es wird für Sekundensprünge kein frame-step-Befehl und kein mute-/seek-Mehrbildmodus verwendet. Auch pause wird durch diese Suchmechanik nicht gesetzt. Kein Play/Pause/Play- oder Pause/Play/Pause-Zyklus wird für den Sekundensprung eingeführt.

Zusätzliche Prüfungen auf der bestehenden mpv-Clientgrenze bestätigen für beide Richtungen und für Pause/laufende Wiedergabe ausschließlich time-pos-Schreibvorgänge, keine zusätzlichen Commands, keine pause-Schreibvorgänge und keine Zustandswechsel. Der native Prüflauf zeichnet Zustandsereignisse während der Sekundensprünge auf: kein kurzzeitiger Playing-Zustand bei Pause und kein kurzzeitiger Paused-Zustand bei laufender Wiedergabe. Damit bleibt die in der persönlichen ersten Build-55-Prüfung bestätigte direkte Sprungmechanik erhalten. Eine erneute persönliche Sichtprüfung der Zwischenbilder mit echtem Film ist offen.

Die unveränderte Einzelbildsteuerung verwendet weiterhin ihre vorhandenen mpv-Framebefehle für genau ein Bild. Der generische native Frame-Service bleibt dafür bestehen; die alten 10-/20-Bilder-Buttons, Strg-Klick-Umschaltung, Hauptfenster-Tastenwege und ViewModel-Wrapper sind entfernt. Keine Motor- oder libmpv-Produktionsänderung.

## Spotlight-Bestandsschutz

Entfernt: Sekunden-Pfeilbuttons, Sprungweiten-Dropdown, spezielle Pfeiltastenbehandlung, Navigations-/Speicherdelegaten im Hostvertrag, Modellzustand und Navigationsmethode sowie SpotlightNavigationSettingsStore. Die dazugehörigen Tests wurden auf den CAN-Player umgestellt. Ein Test stellt ausdrücklich sicher, dass Spotlight keine Sekundensteuerung mehr anbietet.

Erhalten: Keyframe-/Vorschaubildübersicht, Detailsuche und Bildabstand, Bildauswahl und Korrekturen, Werbebeginn/-ende, **Gewähltes Bild als Videoende**, **Videoende aufheben**, vollständige Laufzeitberechnung und Übergabe einschließlich Nachlauf. Sämtliche zuvor ergänzten Videoende-Tests bleiben bestehen, einschließlich mehrerer Werbeblöcke und neun Blöcken in beiden CAN-Schnittmodi. Die Cutlist-Bewertungsintegration bleibt unverändert offen und wurde nicht erneut bearbeitet.

## Maße und Darstellung

Keine zusätzliche Bedienzeile und keine Änderung von Playerhöhe oder Video-Layoutregeln. Vorhandener Buttonstil, Padding der übrigen Buttons und vorhandene Layoutvarianten bleiben erhalten. Neue Sekundenbuttons und Dropdown haben **36 DIP** in Standard/Power-User 1 und **30 DIP** in der bereits kompakten Power-User-2-Ansicht. Die kompakte Gruppe wird beim Layoutwechsel genauso zurückgesetzt wie die übrigen Playeraktionen. Alle vorhandenen Bedienelemente bleiben im selben Playerbereich.

Tatsächliche Vorher-/Nachher-Messung bei Fenster 1900 × 1000 (WPF-DIP):

| Ansicht | Buttonhöhe vorher/nachher | Bedienzeile vorher/nachher | Videofläche vorher/nachher |
| --- | --- | --- | --- |
| Standard | 36 / 36 | 58,4633 / 58,4633 | 1779 × 348 / 1779 × 348 |
| Power-User 1 | 36 / 36 | 58,4633 / 58,4633 | 1328 × 534,5233 / 1328 × 534,5233 |
| Power-User 2 | 30 / 30 | 58,4633 / 58,4633 | 1328 × 490,5233 / 1328 × 490,5233 |

Die Bedienzeile enthält auch die bereits vorhandenen mehrzeiligen Zeit-/Frameanzeigen; ihre gemessene Höhe bleibt unverändert. Die neue Gruppe braucht in Standard/Power-User 1 denselben Gesamtplatz wie die beiden entfernten Buttons; Power-User 2 verwendet die bestehende kompakte Gestaltung. Kleinere Fenster verwenden weiterhin das vorhandene Umbrechen/Scrollen. Die bestehenden DPI-/Layouttests bis 200 % bleiben erhalten. Tatsächliche Aufnahmen mit sichtbaren Keyframe-Tasten wurden geprüft. Die WPF-Aufnahmen bilden die native Videofläche schwarz ab; tatsächliche Wiedergabe ist separat über den echten Player geprüft.

## Geänderte Dateien dieses Korrekturschritts

- src/CutAssistantNext.App/MainWindow.xaml: kompakte Sekunden-Gruppe und Reihenfolge; ±10-Bilder-Buttons entfernt.
- src/CutAssistantNext.App/MainWindow.xaml.cs: Sekunden-Clickhandler, Eingabefeldschutz, Laden/Speichern der Sprungweite; alte Strg-Klick-Beschriftungslogik entfernt.
- src/CutAssistantNext.App/MainWindow.PowerUser2.cs: vorhandene kompakte Höhen/Abstände und Rücksetzen der neuen Gruppe.
- src/CutAssistantNext.App/ViewModels/PlaybackViewModel.cs: validierte positive Sekundenwerte, begrenzter relativer Sprung über SeekToAsync, Vorgaben und speicherbarer Wert.
- src/CutAssistantNext.App/Settings/WindowSettingsStore.cs: zusätzliches Feld im bestehenden Einstellungsobjekt.
- src/CutAssistantNext.App/MainWindow.Spotlight.cs: Navigationseinstellungen/-delegaten entfernt, Frame-Endprüfung erhalten.
- src/CutAssistantNext.App/Settings/SpotlightNavigationSettingsStore.cs: aus der vorherigen lokalen Erstfassung entfernt; war noch nicht versioniert.
- external/spotlight-can/src/SpotlightCan.Wpf/MainWindow.xaml, MainWindow.xaml.cs, MainViewModel.cs und SpotlightHostOptions.cs: ausschließlich Sekundensteuerung/-zustand/-anbindung entfernt; Videoende erhalten.
- tests/CutAssistantNext.Media.Tests/MpvMediaPlayerServiceTests.cs: zusätzliche Prüfung symmetrischer time-pos-Sprünge ohne Framebefehle/Pause-/Zustandswechsel.
- tests/CutAssistantNext.App.Tests/PlaybackViewModelTests.cs: bisherige vier 10-/20-Frame-Wrapper-Tests auf bewusste Sekundenwerte im Dropdown umgestellt, einschließlich direktem Seek und Ausschluss von Frame-Schritten.
- tests/CutAssistantNext.App.Tests/PlayerSecondsNavigationTests.cs: neue beziehungsweise umgestellte CAN-Navigationstests.
- tests/CutAssistantNext.App.Tests/PowerUser2LayoutTests.cs: entfernte ±10-Referenz auf bestehenden Einzelbildbutton umgestellt; neue Gruppen-/Höhen-/Reihenfolgeprüfungen in allen drei Ansichten.
- tests/CutAssistantNext.App.Tests/Services/SpotlightCommunityFeedbackTests.cs: Navigationsfälle nach CAN verschoben; Videoende-Fälle erhalten und fehlende Spotlight-Sekundensteuerung geprüft.
- tools/prototypes/SpotlightIntegrationChecks/Program.cs: vorhandener Prüfhelfer mit tatsächlichen CAN-Buttons, Keyframe-/Frame-Prüfung, Eingabeschutz, unveränderten Schnittpositionen und Geometriemessung.
- README.md, docs/NUTZERANLEITUNG.md, docs/SPOTLIGHT-README.md: aktuelle Bedienung.
- docs/COMMUNITY-FEEDBACK-20261109.md: Erstfassung eindeutig als historisch gekennzeichnet; Bewertungsbefund unverändert.
- docs/PLAYER-SEKUNDEN-NAVIGATION-BUILD55.md: dieser Bericht.

Die übrigen lokalen Änderungen stammen weiterhin aus dem ursprünglichen Videoende-Auftrag. Keine Testentfernung der erhaltenen Videoende-Funktion. Keine zusätzliche Produktionsabhängigkeit. Hilfsprogramme bleiben ausschließlich unter tools; vorhandene C#-Projekte wurden nicht verschoben. Der Laufzeitpfad tools/otr-can/otr_can.exe sowie der vorhandene Motor einschließlich Audioreparatur bleiben unverändert.

## Build und Tests

Release-Build mit explizitem CanBuildNumber=55 erfolgreich, **0 Warnungen / 0 Fehler**. Für diese gezielte Build-55-Korrektur wurde der bestehende Zähler nicht auf eine neue Ausgabe erhöht. dotnet build CutAssistantNext.sln --configuration Release --no-restore -p:CanBuildNumber=55 verwendet die vorhandene Buildstruktur. Abschließend Formatierung, gesperrter Windows-x64-Restore und Rebuild geprüft.

- **1905 CAN-Tests bestanden:** 200 Core, 80 Cutlists, 392 Media, 1233 App; 0 Fehler/übersprungen.
- **78 Spotlight-Tests bestanden:** 59 Core/Media und 19 WPF; 0 Fehler/übersprungen.
- **47 native Prüfungen bestanden:** vorhandene Anwendung, libmpv und ffprobe mit künstlichem Zwölf-Sekunden-Film. Sekundenbuttons bei Pause/Wiedergabe; weiterhin exakte Einzelbildschritte um 40 ms und Keyframe-Sprünge; Eingabefeldschutz; Schnittpositionen unverändert; echte Spotlight-Vorschaubilder; Videoende aus Original-PTS; Laufzeiten und Übergabe in beiden Schnittmodi; Ablehnen/Bestätigen, Abbruch und Ressourcenfreigabe.

Automatisierte Sekundenfälle: vor/zurück mit 1/5/10/30/60 und eigenen Dezimalwerten, Dateigrenzen, Wiedergabezustand, ungültige Eingaben und gespeicherte Werte, Persistenz mit Erhalt von Fenstergröße/Lautstärke, Abbruch und Seek-Fehler. Native Messungen und WPF-Layouttests bestätigen die Höhen und Reihenfolge. Nachweise unter .build/navigation-final-tests, .build/navigation-spotlight-tests, .build/navigation-before und .build/navigation-final-native; dort results.json, player-geometry.json, geometry-comparison.json und drei tatsächliche Playeraufnahmen.

## Start und offene Prüfung

Startbare Entwicklungsdatei in derselben Arbeitskopie:

src/CutAssistantNext.App/bin/Release/net10.0-windows/CutAssistantNext.App.exe

Vollständiger Pfad: C:/Users/joerg/Documents/GitHub/cut-assistant-next/.build/worktrees/community-feedback/src/CutAssistantNext.App/bin/Release/net10.0-windows/CutAssistantNext.App.exe

Version **1.0.3 Build 55**, vorhandene libmpv-DLLs und bytegleicher Build-54-Motor unter tools/otr-can. Für echte Schnittarbeit bleiben die bisherigen konfigurierten externen Werkzeuge erforderlich. Keine automatische Installation, kein Setup oder Release erzeugt. Nicht automatisch gestartet, außer den isolierten automatisierten Prüfprozessen.

Offen: persönliche Bedienung mit echtem Film; vollständige Windows-10-/Windows-11-Plattform-, Mehrmonitor-/physische DPI-, Setup-/Portable-Abnahme. Die bisherigen kleinen Fenster-/DPI-Regressionstests ersetzen keine neue persönliche Plattformabnahme. Keine neuen Realfilmschnitte oder Audiofreigaben behauptet. Die Cutlist-Bewertung bleibt außerhalb dieses Korrekturauftrags offen.
