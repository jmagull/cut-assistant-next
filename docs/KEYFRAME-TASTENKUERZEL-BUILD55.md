# Keyframe-Kürzel und Tooltips – 10.11.2026

Lokale Ergänzung derselben Entwicklungsfassung **Cut Assistant Next 1.0.3 Build 55** in .build/worktrees/community-feedback. Keine neue Anwendung, kein neuer Zweig, keine Änderung an Solution, Motoren, Werkzeugablage oder Paketierung.

## Verhalten und Konfliktprüfung

- **Strg + ←:** vorheriger Keyframe.
- **Strg + →:** nächster Keyframe.
- **◀ K** zeigt „Vorheriger Keyframe (Strg + ←)“, **K ▶** „Nächster Keyframe (Strg + →)“. Darunter bleibt der aktuelle Status einschließlich Analyse- und Sprungfehlern sichtbar, auch bei deaktivierten Buttons.

Die Belegung im Hauptfenster war nach dem ausdrücklich beauftragten Wegfall der alten 20-Bilder-Steuerung frei. Links/Rechts ohne Strg bleibt die pausierte Einzelbildnavigation. Strg mit Alt/Umschalt/Windows wird nicht als neues Kürzel übernommen. Leertaste und Umschalt+F bleiben unverändert. Frame-Lupe und Kontrollplayer haben eigene Fenster mit eigenen Tastaturbehandlungen; diese bleiben unverändert. Besonders die Strg+Pfeil-Belegung der Frame-Lupe gilt weiter dort und wird nicht durch die Hauptfensterfunktion verdrängt. Der entsprechende Anleitungseintrag wurde wieder eindeutig dem Dialog zugeordnet.

Die Kürzel verwenden dieselbe KeyframeNavigationViewModel.JumpAsync-Methode wie die Buttons und deren CanPrevious-/CanNext-Prüfung. Verhalten bei laufendem Video bleibt das bestehende Keyframe-Verhalten: der Sprung pausiert. Die unabhängigen Sekunden-Sprünge behalten ihr bisheriges Play/Pause-Verhalten.

Kein Aufruf bei fehlendem Nachbar-Keyframe, unvollständiger/fehlgeschlagener Analyse, AVI, laufendem Seek, laufender Halbierungssuche, geschlossenem/deaktiviertem Hauptfenster, Filmwechsel oder Öffnen der Frame-Lupe. Bereits behandelte Tastenevents werden nicht erneut übernommen. TextBox, RichTextBox, Passwortfelder und Auswahlfelder einschließlich editierbarem Sekunden-Dropdown behalten ihre Eingabebelegung. Geprüft wird auch der tatsächliche Tastaturfokus. Ein akzeptiertes Ereignis wird vor dem ersten asynchronen Warten als behandelt markiert, damit die WPF-Weiterleitung keinen zweiten Vorgang auslöst.

## Änderungen

- src/CutAssistantNext.App/MainWindow.xaml.cs: geschützte Tastaturweiterleitung an die bestehende Keyframe-Navigation, vor der Einschränkung auf pausierte Einzelbildschritte.
- src/CutAssistantNext.App/MainWindow.xaml: direkte Bindung der vorhandenen beiden Tooltips; keine Änderung an Position, Höhe, Breite, Margin, Padding, Stil oder Buttoninstanzen.
- src/CutAssistantNext.App/ViewModels/KeyframeNavigationViewModel.cs: zwei Tooltip-Texte mit Kürzel und Status; Änderungsbenachrichtigungen auch bei Fehler-/Statuswechseln. Keine Änderung an der Sprungberechnung.
- tests/CutAssistantNext.App.Tests/KeyframeShortcutTests.cs: 33 neue automatisierte Fälle für beide Richtungen in allen drei Layouts, andere Modifier, Texteingaben, unzulässige Zustände, Dateigrenzen, Parallelität, laufende Wiedergabe und dynamische Tooltips einschließlich Fehler und Geometrie.
- tools/prototypes/SpotlightIntegrationChecks/Program.cs: bestehender Prüfhelfer verwendet den neuen Tastaturpfad mit echtem Player und kontrolliert Tooltip-Texte in allen Ansichten.
- README.md und docs/NUTZERANLEITUNG.md: aktuelle Kürzel und eindeutige Trennung zur Frame-Lupe.
- docs/KEYFRAME-TASTENKUERZEL-BUILD55.md: dieser Bericht.

Die gezielte erste Prüfung fand einen bisherigen WPF-Bindungsfehler: StringFormat auf der als object gebundenen ToolTip-Eigenschaft erzeugte keinen sichtbaren Vorspann. Daher werden die vollständigen Texte jetzt direkt gebunden. Nach Korrektur bestehen alle 33 neuen Fälle. Bestehende Status-/Fehlertexte wurden nicht entfernt, sondern sind ausdrücklich mitgetestet.

## Prüfung

Release-Build **0 Warnungen / 0 Fehler**, CanBuildNumber=55 unverändert. Neue Tests formatiert; Diff geprüft.

- **1942 CAN-Tests bestanden:** 200 Core, 80 Cutlists, 392 Media, 1270 App; keine Fehler/übersprungen.
- **78 bestehende Spotlight-Tests bestanden:** 59 Core/Media, 19 WPF.
- **56 native Prüfungen bestanden:** tatsächliche CAN-/Spotlight-Fenster und vorhandener libmpv-Player mit synthetischem Film. Keyframe-Kürzel erreichen dieselben exakten Keyframes; Tooltip-Texte in Standard, Power-User 1 und Power-User 2 geprüft. Bisherige Sekunden-, Einzelbild-, Keyframe-, Eingabe-, EOF-, Schnittmodus-, Abbruch- und Ressourcenprüfungen bestehen weiterhin.

Die automatisierten Tastaturfälle verwenden WPF-KeyEventArgs und explizite Modifier am gemeinsamen Handler, ohne globale Windows-Tasten zu injizieren. Die drei Layoutfälle vergleichen außerdem Buttonabmessungen, Abstände, Stil und Rendergröße vor/nach Status- und Fehlerwechsel. Der XAML-Unterschied dieses Nachtrags enthält ausschließlich die beiden Tooltip-Bindungen. Nachweise unter .build/keyframe-shortcut-tests-fixed, .build/keyframe-full-tests, .build/keyframe-spotlight-tests und .build/keyframe-native.

Startbare Entwicklungsdatei:

C:/Users/joerg/Documents/GitHub/cut-assistant-next/.build/worktrees/community-feedback/src/CutAssistantNext.App/bin/Release/net10.0-windows/CutAssistantNext.App.exe

**Persönlicher Nachtrag 10.11.2026:** Keyframe-Shortcuts mit Yaiba S01E03 in der Entwicklungsfassung positiv gemeldet; außerdem Spotlight und beide Schnittmotoren sowie Bild/Ton/Schnitt/Drei-Sekunden-Prüfung bestätigt. [Praxisbericht und Cutlist-Abgleich](YAIBA-BUILD55-PRAXISTEST-20261110.md). Tooltip-Mausprüfung und alle drei Ansichten nicht gesondert bestätigt; neue vollständige Plattform-/VM-Abnahme bleibt offen. Keine Installation, kein Setup oder Release-Paket, kein Commit/Push, Merge oder GitHub-Release. Bestehende ursprüngliche Nutzeränderungen bleiben erhalten. Alle Hilfsprogramme liegen weiter unter tools; tools/otr-can/otr_can.exe bleibt unverändert.
