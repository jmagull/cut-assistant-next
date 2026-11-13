# Schnittshuffle in Cut Assistant Next 1.0.3 Build 57

## Bedienung

Einen vollständigen Bereich in der Schnittliste auswählen. **←** springt zu seinem Anfang, **→** zu seinem Ende. Der CAN-Player aktualisiert Vorschau und Zeitleiste. Tabellenzeile, Bearbeitungskante und gespeicherte Schnittgrenzen bleiben erhalten. Das funktioniert für rote Entfernen-Bereiche und grüne Behalte-Bereiche.

Die Pfeile messen jeweils 34 × 30 DIP und haben 4 DIP Abstand. Ihre Tooltips und zugänglichen Namen benennen das jeweilige Sprungziel. Bei fehlendem Film, fehlender Auswahl oder offenem Schnittanfang bleibt die Gruppe ausgeblendet. Der Platz neben der Halbierungssuche bleibt beim Auswahlwechsel reserviert.

In Power-User 2 stehen „Videoende“, Schnittshuffle und Halbierungssuche bei ausreichender Breite in dieser Reihenfolge. Bei schmalerem Fenster rücken Schnittshuffle und Halbierungssuche unter die Schnittbuttons. In Standard und Power-User 1 steht Schnittshuffle unmittelbar vor der Halbierungssuche. „Videoende“ befindet sich in diesen Ansichten bei den Schnittbuttons.

Am Dateiende zielt die Navigation auf eine Position einen nominalen Frame vor dem Ende. Bei unbekannter Bildrate rechnet CAN mit 25 Bildern pro Sekunde. Bei variabler Bildrate kann die Vorschau vom letzten Bild abweichen. Eine gerade laufende Wiedergabe pausiert für den Sprung. Die gespeicherte Endegrenze bleibt auf der vollen Mediendauer.

## Geänderte Dateien

| Datei | Änderung |
| --- | --- |
| `src/CutAssistantNext.App/MainWindow.xaml` | Gemeinsame Pfeilgruppe mit Sichtbarkeitsregeln, Tooltips, zugänglichen Namen und Umbruch. |
| `src/CutAssistantNext.App/MainWindow.xaml.cs` | Verbindet die Gruppe mit dem Schnittplan. |
| `src/CutAssistantNext.App/MainWindow.PowerUser2.cs` | Platziert die gemeinsame Gruppe in der kompakten Ansicht und erhält die Pfeilmaße. |
| `src/CutAssistantNext.App/MainWindow.Schnittshuffle.cs` | Verwendet den bestehenden Player-Sprung und behandelt das Dateiende. |
| `src/CutAssistantNext.App/ViewModels/CutPlanViewModel.cs` | Ermittelt, ob der aktuell ausgewählte Bereich vollständig und bearbeitbar ist. |
| `tests/CutAssistantNext.App.Tests/PowerUser2LayoutTests.cs` | Prüft Sprünge und Auswahl in beiden Schnittmodi sowie Maße, Umbruch, Skalierung und EOF. |
| `tools/prototypes/SpotlightIntegrationChecks/Program.cs` | Prüft den Sprung mit dem vorhandenen libmpv-Player an einem kurzen Testvideo. |

## Prüfergebnisse

- Release-Build mit 0 Warnungen und 0 Fehlern.
- 2.073 CAN-Tests bestanden, einschließlich MP4Box-Testfällen und 23 neuen Fällen für Schnittshuffle.
- 78 Spotlight-Tests und 61 native Integrationsprüfungen bestanden. Am 12-Sekunden-Testvideo mit 25 fps sprang libmpv aus laufender Wiedergabe auf 11,960 Sekunden und anschließend zurück zum Schnittanfang. Die Zeitleiste folgte und die gespeicherte Endegrenze blieb bei 12,000 Sekunden.
- WPF-Layout in allen drei Ansichten bei zwei Fensterbreiten und 100, 125, 150 sowie 200 Prozent Skalierung geprüft. Die 24 Renderings liegen unter `.build/schnittshuffle-build57/layouts/`. Die automatischen Testergebnisse liegen in `can-final/`, `spotlight/` und `native-final/` desselben Ordners.
- Die persönliche Abnahme mit eigenen Filmen steht noch aus.

## Entwicklungsfassung

Die ausführbare Datei liegt im Arbeitsverzeichnis unter `src/CutAssistantNext.App/bin/Build57/Release/net10.0-windows/CutAssistantNext.App.exe`. Die Dateiversion lautet `1.0.3.57`. Der Ordner verwendet den vorhandenen otr-can-Motor über eine lokale Verknüpfung.

Alle acht SHA-256-Prüfsummen des vorbereiteten Build-56-Pakets stimmen mit der vorhandenen Prüfsummenliste überein. Für Build 57 wurden keine Installations- oder Release-Pakete erstellt.
