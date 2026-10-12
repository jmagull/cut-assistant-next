# Schritt 5 – Einheitliche Auswahl in Timeline und Schnittliste

Stand: 12.10.2026. Auftrag „also los mit 5)“. Lokaler Entwicklungsstand **0.2.1 · Build 21 · RC3**. Keine neuen Pakete oder Veröffentlichung.

## Verhalten

Ein erster Klick auf eine Schnittbereichszeile wählt den Bereich aus. Ein erneuter Klick auf dieselbe Zeile hebt die Auswahl auf; eine andere Zeile wechselt sie. Das gilt jetzt auch für die Zellen **Anfang** und **Ende**. Timeline und Tabelle verwenden weiterhin gemeinsam SelectedRemoveSegment.

Ein Klick auf Anfang oder Ende springt weiterhin exakt an die entsprechende Grenze im Original – auch wenn der Klick gerade die Auswahl aufhebt. Auswahl und Navigation sind voneinander getrennt. Beim Abwählen ist keine Kante mehr für die Frame-Lupe ausgewählt. Ein nächster Klick auf Anfang/Ende wählt Bereich und zugehörige Kante wieder aus.

Beim Wechsel von Anfang zu Ende innerhalb derselben bereits ausgewählten Zeile wird ebenfalls abgewählt; der Sprung ans Ende bleibt erhalten. Das entspricht dem gewünschten Umschalten der Zeile unabhängig von der angeklickten Spalte.

## Fokus und Bearbeitung

Vorher geprüft: Die Tabelle ist IsReadOnly=True, FullRow und Single. Direkte Bearbeitung von Schnittzeiten in Tabellenzellen ist derzeit nicht aktiviert. Zeitkorrekturen erfolgen über die vorhandenen Schnitt-/Kantenfunktionen und die Frame-Lupe.

Die bisherige native Auswahl und Fokusbehandlung beim ersten Klick auf eine Dauerzelle beziehungsweise andere Nicht-Kantenzellen bleibt erhalten. Erneute Nicht-Kanten-Klicks wählen wie bisher ab. Neue Kantenzellen-Klicks verhindern die anschließende automatische Wiederauswahl durch die Tabelle.

TextBoxBase-Quellen und bereits editierende Zellen werden von der neuen Klicklogik nicht abgefangen. Es wird keine neue Zellbearbeitung eingeführt. Tastaturnavigation, Markierungszustand, Zeitkorrektur und Löschen bleiben über die bestehenden Funktionen verfügbar.

Die Timeline verwendet beim internen Umschalten SetCurrentValue, damit ihre bestehende TwoWay-Auswahlbindung erhalten bleibt. Seek-Position, Farblogik und Schnittzeiten werden dadurch nicht verändert.

## Implementierung

- **CutPlanViewModel.ToggleSegmentSelection**: vorhandenen Bereich/optional zugehörige Kante auswählen oder denselben Bereich abwählen. Ungültige oder veraltete Bereiche werden vor einer Zustandsänderung zurückgewiesen.
- **MainWindow.xaml.cs**: Anfang-/Endezellen verwenden die neue Auswahloperation und unabhängig davon den vorhandenen SeekToAsync-Pfad. Nach fehlgeschlagener Auswahl keine Navigation. Bestehenden Nicht-Kanten-/Fokusablauf erhalten.
- **CutTimelineTrack.cs**: interaktiven Wertwechsel ohne Ablösen der Auswahlbindung durchführen.

Dateien unter src/CutAssistantNext.App. Keine zweite Auswahlquelle, keine Änderung an Schnitt-/Keep-Berechnung, Cutlist-Format, Motoren, Originalen oder Frame-Lupen-Implementierung.

Abwahl löscht weiterhin den Kantenfokus und deaktiviert die Bearbeitung des zuvor gewählten Bereichs. Das ist dieselbe Zustandslogik wie bei der bisherigen Abwahl über Timeline/Dauerzelle. Ein vorher gesetzter offener Anfang wird bei Auswahl eines bestehenden Bereichs wie bisher freigegeben.

## Prüfungen

**Build 21 RC3 ohne Warnungen/Fehler. 938/938 CAN-Tests bestanden**, keine übersprungen: Core 101, Cutlists 72, Media 125, App 640.

14 zusätzliche Fälle prüfen Auswahl/Abwahl ohne Kante und mit Anfang/Ende, Wechsel der Kante innerhalb derselben Zeile, Wechsel auf andere Zeile, PendingStart-Regeln, unveränderte Schnittzeiten, Zurückweisung veralteter Bereiche/ungültiger Seiten sowie anschließende Zeitkorrektur, Löschen und Frame-Kantenkorrektur. Alte Lupenziele nach Abwahl/Planwechsel bleiben ungültig.

Echte kompilierte MainWindow-/DataGrid-/Timeline-Komponenten mit simuliertem Player geprüft:

- synthetische Mouse-Preview- und bei Bedarf nachfolgende MouseDown-Ereignisse auf echten Zellen;
- erster/wiederholter Anfangsklick, Endklick, anderer Bereich und andere Kante derselben Zeile;
- exakte Seek-Ziele 10/20/30/40 Sekunden, auch beim Abwählen;
- Auswählen/Abwählen in Dauerzellen und unveränderte native Auswahl;
- Auswahlbindung zwischen Timeline, ViewModel und Tabelle in beide Richtungen; Bindung bleibt bestehen;
- eingefügtes Textfeld erhält seinen Fokus ohne unerwünschten Seek oder Auswahlverlust;
- native Down-Tastennavigation wechselt die Zeile und aktualisiert Timeline/Kantenfokus;
- Schnittzeiten bleiben unverändert; keine Bindungsfehler.

Timeline-Auswahlwechsel wurde über den gebundenen CurrentValue-Pfad geprüft; kein physischer Maus-Klick auf Timeline-Pixel behauptet. Kein realer Player oder neue Film-/Lupe-Praxistest gestartet. Der native Video-Host war an einen simulierten Player angeschlossen, alle Komponenten am Ende freigegeben. Persönliche Einstellungen wurden nicht gespeichert.

Die ersten Prüfharness-Läufe benötigten einen ergänzten Resolver für bestehende Laufzeit-DLLs und eine vollständige Preview-/MouseDown-Ereignisfolge für die native Dauerzellenauswahl. Das waren Anpassungen des isolierten Harness, keine weiteren Produktänderungen. Abschließender WPF-Lauf erfolgreich.

## Dokumentation und Erhaltung

Anleitung, README, Plan, Status und Tagebuch aktualisiert. Relevante C#-Dateien im notwendigen Umfang formatiert. Build ohne Restore; lokale Nummer einmal auf 21 erhöht.

Acht lokale Lockdateihashes und veröffentlichte Build-16-Artefakte erhalten. Keine persönlichen Einstellungen, Medien, Commit-, Push- oder Release-Aktionen.

Belege unter .build/segment-selection-20261012: BUILD.log, TEST.log, FORMAT.log, test-results, LOCKS-BEFORE.json, WPF.log und ui-proof/{WPF-PROOF.json,BINDINGS.log,selected-grid.png}. Erster fehlgeschlagener Harness-Lauf separat erhalten.

Persönliche Bedienabnahme noch offen. Nächster Schritt gemäß Plan: **6 – Rechtsklick-Kontrolle drei Sekunden vor/nach der Schnittkante, samt SVG und Anleitung**.
