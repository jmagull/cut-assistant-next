# Schritt 4 – Namensmasken-Editor mit unabhängigen Serienbausteinen

Stand: 12.10.2026. Auf Auftrag „4) gogogo“ umgesetzt. Lokaler Entwicklungsstand **0.2.1 · Build 20 · RC3**. Öffentliche Setup-/Portable-Pakete bleiben Build 16 RC3.

## Ergebnis

Die Palette bietet jetzt fünf getrennte, frei kombinierbare Bausteine:

- **S**: S, wenn ein Staffelwert vorhanden ist.
- **Staffel**: eingegebener Staffelwert ohne Präfix.
- **E**: E, wenn ein Folgenwert vorhanden ist.
- **Folge**: eingegebener Folgenwert ohne Präfix.
- **x**: kleines x, wenn Staffel UND Folge vorhanden sind.

Die bisherigen kombinierten „Staffel (S)“-/„Folge (E)“-Elemente werden nicht mehr in der Palette angeboten. Alle übrigen Bausteine und Trennzeichen bleiben erhalten. Einfügen, Verschieben und Löschen betreffen die einzelnen Bausteine.

Eingaben bleiben unverändert: 1 → 1, 01 → 01, 04 → 04. Keine automatische Nullformatierung. Neue S-/E-/x-Marker behandeln fehlende und nur aus Whitespace bestehende Werte als leer. Teilangaben sind möglich: nur Staffel → S01, nur Folge → E02; x entfällt, wenn einer der Werte fehlt.

Bewusst gesetzter Literaltext und Trennzeichen werden nicht pauschal entfernt. Das gilt auch für wörtlich gespeicherte S/E/x-Zeichen. Entfernen eines Bausteins ist von fehlenden Metadaten zu unterscheiden: ein ausdrücklich verbliebener S-Baustein kann bei vorhandener Staffel weiterhin S ausgeben, auch wenn der Zahlenbaustein entfernt wurde.

## Neue Standardmaske und Referenz

Technische Standardmaske:

    %Name% %S%%Staffel%%E%%Folge%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]

Alternative mit x:

    %Name% %Staffel%%x%%Folge%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]

Beispieldatei:

    Ghostbusters__Hexenprobleme_26.09.19_10-50_pro7maxx_25_TVOON_DE.HQ.mp4

NamingEditorSample verwendet den vorhandenen OTR-Parser für Datum und ursprüngliche Dateiinformationen. Name Ghostbusters, Staffel 01, Folge 01 und Folgentitel Hexenprobleme werden ausdrücklich als Beispieldaten ergänzt. Staffel/Folge stammen nicht aus der Originaldatei.

OTR-Datumsblock **26.09.19 = JJ.MM.TT = 19.09.2026**.

Editorvorschau:

    Ghostbusters S01E01 Hexenprobleme [19.09.2026]

Ausgabedateiname einschließlich automatisch ergänzter Endung:

    Ghostbusters S01E01 Hexenprobleme [19.09.2026].mp4

Filmfall mit leeren Serienangaben:

    Die Teufelsbrigade [21.01.2021]

Keine zusätzlichen Bindestriche in der Standardmaske und kein SE bei fehlenden Staffel-/Folgenwerten.

## Rückwärtskompatibilität

Alte Variablen und Präfixsyntax, insbesondere %Staffel:S% und %Folge:E%, werden weiterhin verarbeitet. Masken ohne neue Marker behalten ihre bisherigen Renderingregeln. Der neue Serienblock-Ersatz erhält die umgebenden Leerzeichen bis zur bestehenden abschließenden Normalisierung, damit bei fehlender Staffel der Titel nicht mit E01 oder einer einzelnen Nummer verschmilzt.

Der allgemeine Blockparser bleibt verlustfrei. Eine zusätzliche Editoransicht teilt die beiden bekannten alten Standardpräfixtokens in sichtbare S/Staffel- bzw. E/Folge-Bausteine. Die Herkunft bleibt intern an den Teilbausteinen erhalten. Stehen diese unverändert zusammen, serialisiert der Editor wieder das ursprüngliche Präfixtoken. So bleibt auch bei Bearbeitung anderer Bausteine die alte Syntax und ihr Verhalten erhalten.

Andere individuelle Präfixe oder Freitexte werden nicht umgeschrieben. Gespeicherte Masken bleiben beim Öffnen und Laden unverändert. Eine neue Standardmaske wird nur bei fehlender Vorgabe oder nach ausdrücklichem Klick auf **CAN-Standardmaske** eingesetzt. Bestehende Einstellungen werden nicht ungefragt ersetzt.

**Änderungen verwerfen** stellt die beim Öffnen geladene Maske wieder her. **CAN-Standardmaske** lädt die neue Standardmaske. Erst **Speichern** übernimmt sie; **Abbrechen** verwirft Änderungen. Der vollständig editierbare Filmname aus Schritt 2 bleibt eine eigenständige Funktion.

## Dateien

- Core/Naming/NameTemplateRenderer.cs: neue bedingte Einzelmarker, alte Syntax erhalten.
- App/Naming/NameTemplateBlockParser.cs: neue Anzeigenamen und verlustfreie geteilte Legacy-Ansicht.
- App/Naming/NamingEditorSample.cs: verbindliche Beispieldatei und ergänzte Beispieldaten.
- App/ViewModels/NamingSettingsViewModel.cs: geteilte Editoransicht beim Anzeigen und Bearbeiten.
- App/Settings/NamingSettingsStore.cs: neue CAN-Standardmaske.
- App/Dialogs/NamingSettingsDialog.xaml: neue Palette, verständlicher Hinweis, aktualisierter Standard-Tooltip.
- App/MainWindow.xaml.cs: neuer Beispielkontext beim Öffnen des Editors.

Dateipfade unter src/CutAssistantNext.Core bzw. src/CutAssistantNext.App. Keine Änderung an OTR-Erkennung, Cutlist-Format, Schnittpunkten, Keep-Berechnung, Motoren, Dateipfaden oder Originalen. Keine neue Produktionsabhängigkeit.

## Prüfungen

**Build 20 RC3: 0 Warnungen/0 Fehler. 924/924 CAN-Tests bestanden**, keine übersprungen: Core 101, Cutlists 72, Media 125, App 626.

43 zusätzliche Fälle prüfen S1E1/S01E01, 1x02/4x01/04x01, 02-13, führende Nullen, fehlende/einzeln fehlende/Whitespace-Werte, bewusst gesetzte Literalzeichen, umgeordnete und einzeln verwendete Bausteine, die Ghostbusters-Referenz samt Datum/Endung und den Filmfall.

Legacy-Masken mit bekannten und individuellen Präfixen werden exakt wieder serialisiert und mit unterschiedlichen Kontextwerten einschließlich Leerwerten geprüft. Isolierte Speicher-/Ladetests erhalten alte und neue Masken ohne automatische Ersetzung. Einfügen, Verschieben, unabhängiges Entfernen, Standardrücksetzung und Verwerfen geprüft. Bestehende Editor-Tests auf die neue sichtbare Teilung angepasst; keine Tests entfernt.

Echter kompilierter WPF-Editor unter isolierten Einstellungen geprüft:

- fünf einzelne Palettenelemente, keine alten kombinierten Palettenbuttons;
- geteilte Anzeige alter Standardpräfixe ohne Änderung des Maskentexts;
- exakte Ghostbusters-Vorschau;
- tatsächliche Drop-Handler mit synthetischen Drop-Ereignissen: neue Elemente einfügen und vorhandenen Baustein verschieben;
- tatsächliche Entf-Tastenbehandlung sowie Standard-/Verwerfen-Buttons;
- modal ausgeführte Speichern-/Abbrechen-Buttons, nur isolierte Einstellungsdatei gespeichert;
- Normalgröße 1100×760 und Mindestgröße 980×700 gerendert und geprüft; keine Bindungsfehler.

Die bestehenden Scrollmöglichkeiten des Editors bleiben erhalten; bei schmalerem Fenster kann die Maskenzeile horizontal gescrollt werden. Kein physischer Maus-Drag oder persönliche Nutzerabnahme daraus abgeleitet.

## Nachweise und Erhaltung

Lokale Belege unter .build/naming-blocks-20261012: BUILD.log, TEST.log, FORMAT.log, LOCKS-BEFORE.json, test-results, WPF.log und ui-proof/{WPF-PROOF.json,BINDINGS.log,editor-1100.png,editor-980.png,isolated-naming.json}.

Relevante C#-Dateien formatiert, ohne Restore gebaut; lokale Buildnummer einmal auf 20 erhöht. Acht lokale Lockdateihashes und alle veröffentlichten Build-16-Artefakte erhalten. Keine persönlichen Namensmasken oder Einstellungen geändert.

Keine neue persönliche Abnahme durch den Nutzer, kein Commit, Push, Setup oder Release. Nächster Schritt gemäß Plan: **5 – Auswahl zwischen Timeline und Tabelle vereinheitlichen**.
