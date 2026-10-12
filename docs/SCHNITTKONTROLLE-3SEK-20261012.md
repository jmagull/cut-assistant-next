# Schritt 6 – Schnittkanten per Rechtsklick kontrollieren

Stand: 12.10.2026. Auftrag „6) gogogo“. Lokaler Entwicklungsstand **0.2.1 · Build 22 · RC3**. Keine neuen Pakete oder Veröffentlichung.

## Ergebnis

Rechtsklick auf eine **Anfang**-Zelle springt im vorhandenen Originalplayer drei Sekunden vor die eingetragene Grenze. Rechtsklick auf **Ende** springt drei Sekunden danach.

Beispiel: Werbebereich 10:00–12:00 → Prüfpositionen **09:57** und **12:03**.

Playerposition und Slider werden über den vorhandenen SeekToAsync-Ablauf aktualisiert. Es wird keine automatische Wiedergabe gestartet; ein bereits laufender Player behält seinen bisherigen Zustand. Auswahl, Kantenfokus für die Frame-Lupe, Schnittzeiten und Cutlist-Daten bleiben erhalten.

Nur Anfang-/Ende-Datenzellen erhalten die neue Navigation. Dauer, Überschriften und andere Flächen behalten ihr bisheriges Verhalten. Text-/Editierquellen werden nicht abgefangen. Der Linksklick-Ablauf aus Schritt 5 bleibt erhalten.

## Begrenzungen

Zunächst exakt ±3 Sekunden anhand der Originalzeitachse berechnen. Danach auf Originaldauer und angrenzenden Filmabschnitt begrenzen:

- Nahe Dateianfang wird die Position auf 0 begrenzt; bei Anfang 0 ebenfalls 0.
- Nahe Dateiende wird auf die Originaldauer begrenzt; bei Ende gleich Dateiende ebenfalls diese Grenze.
- Vor einer Startkante mit kurzem Filmstück liegt die früheste Position am Ende des vorangehenden roten Bereichs. Beispiel: vorheriger roter Bereich endet bei 09,000 s, neue Kante bei 10,000 s → Ziel 09,000 s.
- Nach einer Endkante mit kurzem Filmstück wird die Prüfposition strikt vor dem nächsten roten Anfang begrenzt. Die Zeitdomäne verwendet dessen Anfang minus einen TimeSpan-Tick, damit das errechnete Ziel nicht selbst im nächsten roten Bereich liegt.
- Berühren sich rote Bereiche innerhalb der Datei unmittelbar, gibt es auf dieser Seite keinen angrenzenden Filmabschnitt. Ein verständlicher Hinweis erscheint; kein Sprung in einen anderen roten Bereich.
- Ein Filmstück von nur einem Tick wird in der Zeitberechnung erhalten. Tatsächlich dargestellte Frames hängen weiterhin von Player/Originalzeitstempeln ab; die Navigation ersetzt keine Frame-Lupe.

Im untersuchten allgemeinen Seek-Pfad existiert keine zentrale Rotbereichssperre. Die neue Prüfposition wird daher gezielt gegen die unmittelbar angrenzenden Bereiche berechnet. Normaler Timeline-/Slider-/Linksklick-Seek wird nicht umgebaut.

Keine zusätzliche Drei-Sekunden-Einstellung. Kein Eingriff in Schnittmotoren, Keep-Berechnung, Frame-Korrektur oder Originaldateien.

## Implementierung

- **Core/Editing/CutEdgeReviewPositionCalculator.cs**: reine, testbare Berechnung mit Originaldauer, angrenzenden Bereichen, Tickgenauigkeit und Schutz vor Überlauf.
- **App/ViewModels/CutPlanViewModel.cs**: Berechnung für den aktuellen Schnittplan, ohne Änderung seines Zustands.
- **App/MainWindow.xaml.cs**: eigener Rechtsklick-Handler, begrenzte Zell-/Editierauswertung, unveränderte Auswahl und bestehender Player-Seek.
- **App/MainWindow.xaml**: Rechtsklick-Ereignis an der bestehenden Tabelle anschließen.

Pfade unter src/CutAssistantNext.Core bzw. src/CutAssistantNext.App. Veraltete/nicht zum aktuellen Plan gehörende Bereiche werden zurückgewiesen, bevor navigiert wird.

## SVG und Dokumentation

Dauerhafte Grafik: **docs/images/schnittkontrolle-3sek.svg**, in README eingebunden. Hellgrüne Filmabschnitte, hellroter Werbebereich, Grenzen 10:00/12:00, Pfeile zu 09:57/12:03 und Beschriftungen −3 Sekunden/+3 Sekunden.

Die Grafik besitzt eine feste helle Grundfläche mit kontrastreichem Text und bleibt damit auch vor dunklem GitHub-Hintergrund lesbar. XML geprüft, mit vorhandener gebündelter SVG-Rasterisierung gerendert und vor heller/dunkler Umgebung kontrolliert. Keine externen Bildreferenzen, Scripts oder neuen CAN-Abhängigkeiten. Die Schemazeitachse ist ausdrücklich nicht maßstabsgetreu.

Text unter der Abbildung exakt:

„Rechtsklick auf Anfang: 3 Sekunden zurück. Rechtsklick auf Ende: 3 Sekunden vorwärts. So lassen sich die Filmszenen unmittelbar vor und nach einem Schnittbereich kontrollieren.“

Nutzeranleitung um Ablauf und Grenzfälle ergänzt.

## Prüfstand

**Build 22 RC3 ohne Warnungen/Fehler. 963/963 CAN-Tests bestanden**, keine übersprungen: Core 122, Cutlists 72, Media 125, App 644.

25 zusätzliche Fälle: exakte Beispielsprünge; Nähe zu Anfang/Ende; kurze vorherige/nachfolgende Filmstücke; direkt berührende rote Bereiche; Einzel-Tick-Intervalle; unveränderte Zeitpräzision; TimeSpan-Überlaufgrenze; veraltete Bereiche und ungültige Seiten; vorhandene Auswahl/Lupenziele/PendingStart/Schnittzeiten bleiben unverändert.

Echte kompilierte MainWindow-/Tabellen-/Timeline-Komponenten mit simuliertem Player und synthetischen Rechtsklick-Ereignissen geprüft:

- Anfang/Ende derselben und anderer Zeilen: 7/23/27/43 s beim 10–20-/30–40-s-Beispiel.
- Auswahl und ausgewählte Kante bleiben erhalten; kein automatisches Play.
- Dauerzelle und eingebettetes Textfeld erzeugen keine neue Prüf-Navigation.
- Kurze Filmstücke begrenzen korrekt, ebenso Originalanfang und Originalende.
- Bisherige Linksklick-Auswahl, Abwahl, Kanten-Sprünge, Bindung, Textfokus und native Tastaturnavigation aus Schritt 5 weiterhin erfolgreich.
- Schnittzeiten unverändert, keine Bindungsfehler, keine persönlichen Einstellungen gespeichert.

Der Hinweisdialog für fehlenden angrenzenden Film wurde nicht automatisiert bestätigt; Null-Ergebnis der Berechnung und zugehöriger UI-Zweig sind geprüft. Keine persönliche neue Film-/Lupe-Abnahme oder tatsächliche Video-Framegenauigkeit daraus abgeleitet.

## Nachweise und Erhaltung

Belege unter .build/edge-review-20261012: BUILD.log, TEST.log, FORMAT.log, test-results, LOCKS-BEFORE.json, WPF.log, ui-proof/WPF-PROOF.json sowie Schnittkontroll-PNGs vor heller/dunkler Umgebung. PNGs und Raster-Helfer sind nur lokale Prüfartefakte; die SVG ist die Dokumentationsquelle.

Relevante C#-Dateien formatiert, ohne Restore gebaut; Nummer einmal auf 22 erhöht. Alle acht lokalen Lockdateihashes und veröffentlichten Build-16-Artefakte erhalten. Keine neuen Produktionsabhängigkeiten oder persönlichen Einstellungen.

Kein Commit, Push, Setup oder Release. Persönliche Abnahme noch offen. Nächster Schritt: **7 – unabhängiges Kontrollfenster für Übergänge der fertig geschnittenen Datei**.
