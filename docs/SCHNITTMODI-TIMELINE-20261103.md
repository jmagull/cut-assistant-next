# CAN 1.0.3 – Schnittmodi und dynamische Timeline-Markierung

**Abschließender Stand 03.11.2026:** CAN 1.0.3 / Zähler 48 einschließlich Buttonkorrektur und persönlicher Sichtbestätigung ist auf Nutzerauftrag eingefroren; Commit und Push sind beauftragt, Veröffentlichung erst nach gesonderter Freigabe. Die folgenden Prüf-/Git-Aussagen dokumentieren ihre jeweiligen früheren Arbeitsschritte. [Aktueller Freeze und Nachweise](FREEZE-1.0.3-20261103.md).

**Aktualisierte persönliche Abnahme 03.11.2026:** Santa Tell Me im grünen Modus mit vier Bereichen und **01:21:24.120** ist für Ton, Bild, Schnitt und Drei-Sekunden-Vorschau positiv gemeldet. Beide Motorprotokolle sind abgeschlossen; gespeicherte Cutlist und Servereintrag liegen vor. Die unten zum damaligen Abschluss noch offene grüne Filmprüfung ist damit ergänzt. Persönliche Wiederaufnahme begonnener Arbeit bleibt offen. [Detaillierter Praxisnachweis](SANTA-TELL-ME-GRUENER-PRAXISTEST-20261103.md).

Stand: 03.11.2026. Technische Prüfung und gezielte Implementierung gemäß Entwicklungsauftrag „Das kann weg“ / „Das soll bleiben“.

## Ergebnis und Bedienung

Unter **Einstellungen → Darstellung und Schnittbearbeitung …** stehen zwei dauerhaft gespeicherte Arbeitsweisen zur Verfügung:

- **Das kann weg (Rot):** Markierte Bereiche entfernen; bisheriger Standard.
- **Das soll bleiben (Grün):** Nur markierte Bereiche behalten.

Die Arbeitsweise wird beim Öffnen eines Films eingelesen und für dessen Editor festgelegt. Änderungen im Einstellungsdialog ändern den laufenden Schnittplan nicht. Erst der nächste neu geöffnete Film verwendet die neue Einstellung. Bestehende Einstellungen ohne diese Angabe verwenden weiterhin Rot.

Ein vorgemerkter Anfang erzeugt eine feste Linie. Zwischen diesem Anfang und der aktuellen Sliderposition liegt eine halbtransparente rote beziehungsweise grüne Vorschau. Sie folgt dem Slider in beide Richtungen. Erst ein gültiges Ende übernimmt einen abgeschlossenen Bereich in die vorhandene Tabelle. **Esc** verwirft den offenen Anfang und seine Vorschau, ohne fertige Bereiche zu löschen. Die Taste wird im Hauptfenster außerhalb von Texteingaben behandelt.

Schnittanfang und Schnittende müssen weiterhin gültige, aufsteigende Grenzen bilden. Eine rückwärts gezogene Vorschau ändert diese Regel nicht. Ungültige Endpunkte lassen den offenen Anfang und den vorhandenen Plan bestehen.

## Technischer Befund und begrenzte Umsetzung

Der untersuchte Pfad lautet: `CutPlanViewModel` → `CreateCutPlanSnapshot()` → bestehender Cutlist-Export beziehungsweise `CutRequestFactory` → MP4Box/otr-can. Lokale und Server-Cutlists gelangen über den bestehenden Importpfad als kanonischer Entfernungsplan in `LoadCutPlan()`.

Die vorhandenen komplementären Helfer `CutlistKeepSegmentBuilder` und `CutlistRemoveSegmentBuilder` reichen aus. Ein Umbau des Core-Schnittmodells oder eine neue Arbeitsstandsverwaltung war nicht erforderlich.

Der Editor hält die vom Anwender markierten Intervalle. Im roten Modus sind das Entfernungsbereiche, im grünen Modus Behaltebereiche. Der vorhandene private `CutPlan` dient weiterhin zur Prüfung und Bearbeitung dieser Intervalle. `MarkedSegments` benennt die Bedeutung für die Oberfläche; die bisherige Eigenschaft `RemoveSegments` bleibt aus Kompatibilitätsgründen erhalten. Sie darf im grünen Editor nicht als Ausgabeplan verwendet werden. Der tatsächliche Entfernungsplan entsteht ausschließlich als unabhängige Kopie über `CreateCutPlanSnapshot()`.

Im grünen Modus ergänzt diese Methode das Komplement der Behaltebereiche mit dem vorhandenen `CutlistRemoveSegmentBuilder`. Die Schnittmotoren erhalten dadurch dasselbe `CutRequest`-Modell und dieselben Keep-Zeiten wie bei einem fachlich gleichwertigen roten Plan. Es gibt keine Umrechnung pro Sliderbewegung. Core, Media, Schnittmotoren, Klebecenter, Cutlist-Parser und Dateiformat wurden in diesem Entwicklungsschritt nicht geändert.

## Geänderte Dateien und Aufgaben

| Datei unter `src/CutAssistantNext.App/` | Änderung |
| --- | --- |
| `ViewModels/CutPlanViewModel.cs` | `CutEditingMode`, modeabhängige Texte und Ausgabedauer; `Initialize`, `LoadCutPlan`, `CreateCutPlanSnapshot`, `GetEdgeReviewPosition`, `CancelPendingStart`; gemeinsame Intervallbearbeitung bleibt erhalten. |
| `Settings/AppearanceSettingsStore.cs` | Zusätzliche Einstellung `CutEditing`; Standard Remove; ungültige oder fehlende Angabe fällt auf Remove zurück, gültige Layoutwahl bleibt erhalten. |
| `Dialogs/AppearanceSettingsDialog.xaml` und `.xaml.cs` | Bestehende Layoutauswahl um zwei Schnittbearbeitungsoptionen und deren Erklärung ergänzt; beide Einstellungen gemeinsam speichern. |
| `Controls/CutTimelineTrack.cs` | Rote/grüne fertige Markierungen; eigener `DrawingVisual` für Vorschau und Startlinie; eingefrorene Farbressourcen. |
| `MainWindow.xaml` | Bestehende Timeline und Tabelle an `MarkedSegments` binden; Modus, offener Anfang und Sliderposition binden; zutreffende Texte und Einstellungsmenü. |
| `MainWindow.xaml.cs` | Einstellung einmal beim Filmöffnen einlesen; Esc-Abbruch; Modus an Frame-Lupe geben; grüne Auswahl vor Export und Schnitt bestätigen. |
| `MainWindow.CutEditing.cs` (neu) | Gemeinsame Bestätigung der grünen Ausgabeauswahl; offene Markierung sperrt diese Ausgabe. |
| `MainWindow.CutToolsLayout.cs` | Beim Zurückwechseln zur Standardansicht die Inhaltsbindungen der beiden Schnittbuttons erhalten. |
| `ViewModels/FrameLoupeViewModel.cs` | Zutreffende Bedeutung der halb offenen Start-/Endkante im grünen Modus; Zeit-/Frameübernahme unverändert. |

Vier Testdateien wurden ergänzt beziehungsweise erweitert: `CutEditingModeTests.cs`, `CutEditingTimelineTests.cs`, `CutToolLayoutTests.cs` und `FrameLoupeViewModelTests.cs` unter `tests/CutAssistantNext.App.Tests/`.

## Dauerhafte Einstellung und laufender Film

Verwendet wird weiterhin `appearance-settings.json` im vorhandenen lokalen CAN-Einstellungsverzeichnis. Der neue Datensatz enthält neben `Layout` die numerische Enum-Angabe `CutEditing` (Remove = 0, Keep = 1). Es gibt keine zusätzliche Einstellungsdatei und keine neue Produktionsabhängigkeit.

`LoadMediaFileCoreAsync()` liest die Auswahl vor dem Analysieren des neuen Films im Hintergrund ein. Die anschließende Initialisierung des Editors verwendet diese erfasste Auswahl. Der Einstellungsdialog ändert den Modus eines bereits initialisierten Editors nicht. Der allgemeine vorhandene Layoutwechsel bleibt sofort möglich.

## Vorschau, Leistung und Synchronisierung

`CutTimelineTrack` besitzt eine zusätzliche Zeichenebene. `PendingStart` und `CurrentPositionSeconds` aktualisieren ausschließlich diese Ebene. Die Position ist an `TimelineSlider.Value` gebunden; damit laufen Dragging, Seeking und die bestehenden Player-/Framepositionsupdates durch denselben Anzeigeweg.

Die Vorschau zeichnet höchstens eine Fläche und eine Startlinie, ohne die Intervallkollektion zu verändern. Rückwärtsbewegung zeichnet zwischen Minimum und Maximum der beiden Positionen; die Startlinie bleibt am ursprünglichen Anfang. Vorschau und fertige Bereiche verwenden unterschiedliche Deckkraft. Größenänderungen und Filmwechsel aktualisieren beziehungsweise leeren die Darstellung.

Die statische Timeline mit fertigen Bereichen und Keyframes muss bei reinen Positionsupdates nicht neu gezeichnet werden. Der WPF-Test verwendet 10.000 Keyframepositionen und prüft bei 200 zusätzlichen Positionsänderungen je Fall, dass deren Enumeration nicht erneut stattfindet. Dies ist ein struktureller Leistungstest, kein allgemeiner Hardware-Benchmark.

Die vorhandene Tabelle, Auswahl, Grenzbearbeitung und Löschfunktion arbeiten in beiden Modi auf derselben markierten Kollektion. Die Frame-Lupe übernimmt weiterhin die Grenze vor dem ausgewählten Frame, ohne einen zusätzlichen Frameversatz. Die Rechtsklick-Kantenprüfung sucht im grünen Modus innerhalb des Behaltebereichs, statt in einem zu entfernenden Nachbarbereich. Kontrollplayer und dessen Drei-Sekunden-Übergangsvorschau bleiben unverändert; sie erhalten das aus dem tatsächlichen Ausgabeplan berechnete Ergebnis.

## Laden, Speichern und noch unmarkierte Bereiche

Importierte Cutlists werden weiterhin unverändert gelesen und geprüft. Im roten Editor werden deren Entfernungsbereiche angezeigt; im grünen Editor werden die vorhandenen Behaltebereiche aus dem kanonischen Importplan abgeleitet. Die geplante Ausgabe bleibt gleich. Die gemeinsame Server-/Lokal-Tabelle, manuelle Ladefunktionen, Quellenschutz und Überschreibentscheidungen bleiben erhalten.

Im grünen Modus sind unmarkierte Teile weiterhin unmarkiert im Editor. Es werden daraus weder zusätzliche sichtbare Schnittpunkte noch ein Bearbeitungsstatus erzeugt. Beschreibung und geschätzte Ausgabedauer zeigen, dass nur grüne Bereiche ausgegeben werden. Vor Speichern oder Schneiden erscheint eine Bestätigung mit Anzahl und Gesamtdauer der Behaltebereiche und dem ausdrücklichen Hinweis, dass auch noch nicht bearbeitete unmarkierte Filmteile fehlen werden. Standardantwort ist Nein. Diese Entscheidung gilt für den unmittelbar folgenden modalen Vorgang.

Ein offener Anfang verhindert das Speichern in beiden Modi über den bestehenden `CutlistSaveGuard`. Er wird weder ergänzt noch stillschweigend verworfen. Der grüne Ausgabepfad sperrt auch den Schnitt mit offener Markierung. Die bisherige rote Schnittausgabe wurde dabei nicht verändert.

Eine leere grüne Auswahl hat Ausgabedauer null. Sie wird nicht automatisch in „ganzen Film behalten“ umgedeutet. Der vorhandene Schnittauftrag akzeptiert keine leere Ausgabe; ein ausdrücklich bestätigter Cutlist-Export kann weiterhin null Behaltebereiche beschreiben. Ohne verlässliche Metadaten wird kein Zustand „fertig“ oder „unfertig“ behauptet. Das bestehende Format speichert vollständige Intervalle, keinen zusätzlichen Arbeitsfortschritt oder offenen Anfang.

## Build und automatisierte Regression

Abschließender Release-Build: **0 Warnungen, 0 Fehler**. Alle **1.709 Tests bestanden**, keiner übersprungen:

| Projekt | Erfolgreich |
| --- | ---: |
| Core.Tests | 200 |
| Cutlists.Tests | 80 |
| Media.Tests | 390 |
| App.Tests | 1.039 |
| Gesamt | 1.709 |

Gegenüber dem Ausgangsstand dieses Auftrags (1.685) sind 24 Testfälle hinzugekommen. Die bestehenden Tests wurden erhalten. Neue Prüfungen decken insbesondere ab:

- Gleiche kanonische Entfernungsintervalle, gleiche Motor-Keep-Zeiten und gleiche gespeicherte Keep-Intervalle aus gleichwertigen roten/grünen Markierungen.
- Speichern, erneutes Laden und Weiterbearbeiten ohne Änderung der Originaldatei; explizite lokale und Server-Auswahl im grünen Modus.
- Standardwert, alte/ungültige Einstellung, dauerhafte Speicherung über eine neue Store-Instanz, unveränderter laufender Plan und Anwendung beim nächsten Editor.
- Offener Anfang, fehlgeschlagenes Ende und Abbruch ohne Verlust abgeschlossener Bereiche in beiden Modi.
- Mehrere Intervalle, Filmgrenzen, halb offene Grenzen und ein Tick kurze Bereiche; Bearbeiten und Löschen.
- Modusgerechte Texte der Frame-Lupe bei identischen übernommenen Framezeiten und Navigation.
- Echte WPF-Bindungen des Hauptfensters und deren Erhalt beim Wechsel Standard/Power-User; echte Slider-/Timeline-/Tabellensteuerung mit unverändertem Schnittplan während der Vorschau.
- Transparenz, feste Startlinie, Vorwärts-/Rückwärtsbewegung, Übernahme, Abbruch und statische Zeichenleistung für Rot/Grün auf hellem/dunklem Hintergrund.

Gebaut und getestet wurde getrennt unter `.build/cut-editing-modes-20261103/`, ausdrücklich mit `CanBuildNumber=48`. Das reguläre Buildskript wurde nicht verwendet, weil es den Zähler erhöhen würde. Restore verwendete die vorhandenen Lockdateien über den bereits bestehenden isolierten Restore-Pfad; die Lockdateien im Arbeitsbaum wurden nicht verändert.

```text
dotnet restore CutAssistantNext.sln --artifacts-path .build/cut-editing-modes-20261103/out -r win-x64 --locked-mode --disable-parallel -p:CustomBeforeMicrosoftCommonTargets=C:/Users/joerg/Documents/GitHub/cut-assistant-next/.build/cutlist-table-20261103/restore-locks.targets
dotnet build CutAssistantNext.sln --configuration Release --no-restore --artifacts-path .build/cut-editing-modes-20261103/out -p:CanBuildNumber=48 --disable-build-servers -m:1
dotnet test CutAssistantNext.sln --configuration Release --no-build --no-restore --artifacts-path .build/cut-editing-modes-20261103/out --logger trx --results-directory .build/cut-editing-modes-20261103/test-results
```

Nur die Dateien dieses Entwicklungsschritts wurden mit `dotnet format whitespace --no-restore --include …` formatiert. `git diff --check` liefert keine Fehler.

## Praktische WPF-Prüfung und Grenzen

Tatsächliche WPF-Fenster und Controls wurden auf STA-Threads geöffnet und bedient: vorhandener Einstellungsdialog mit grüner Auswahl und Speichern, sowie die echte Timeline zusammen mit Slider und Tabelle in beiden Modi auf hellem und dunklem Hintergrund. Die Vorschau wurde vorwärts und rückwärts bewegt, abgebrochen und als abgeschlossener Bereich übernommen. Hauptfenster-Bindungen und Standard-/Power-User-Wechsel wurden zusätzlich im vorhandenen Hauptfenster geprüft. Die gemeinsamen Cutlist-Dialogprüfungen bleiben erfolgreich.

Elf lokale Bildnachweise liegen im nicht versionierten Verzeichnis `.build/cut-editing-modes-20261103/ui-proof/`: je vier Ansichten mit offener und fertiger Markierung, Einstellungsdialog sowie zwei gemeinsame Cutlist-Dialogansichten. Visuell kontrolliert wurden unter anderem `Keep-light-pending.png`, `Remove-dark-complete.png` und `settings-green.png`.

Die Timeline-Prüfung verwendet synthetische Zeitbereiche; die dunkle Variante prüft die Erkennbarkeit der Farben auf dunklem Hintergrund und führt kein neues App-Theme ein. Ein erneuter vollständiger Filmschnitt mit anschließender menschlicher Bild-/Tonkontrolle im neuen grünen Modus wurde in diesem Schritt nicht durchgeführt. Die Motor-Eingangsdaten sind automatisiert geprüft, die bestehenden Motor-/Kontrollplayer-Regressionen bestehen. Der zuvor vom Anwender bestätigte otr-can-Filmschnitt bleibt ein Test des vorherigen Entwicklungsstands und wird hier nicht als grüner Bedienungstest gezählt.

## Git- und Release-Stand

Der Arbeitsbaum enthält weiterhin die bereits vorhandenen Änderungen an CAN 1.0.3 und der gemeinsamen Cutlist-Tabelle sowie die hier beschriebenen Änderungen. Nichts wurde gestaged, committet, gepusht, gepackt oder veröffentlicht.

Vor Beginn dieses Auftrags wurde ein SHA-256-Ausgangsvergleich unter `.build/cut-editing-modes-20261103/BEFORE.json` gesichert. Gegenüber diesem Ausgangsstand unverändert bestätigt: `Version.props` (1.0.3), `.build/build-number.txt` (48), die vorhandenen Produktions-Lockdateien, sämtliche Core-/Media-/Cutlists-Quelldateien sowie die vorhandenen Nutzeränderungen in `AKTUELLER-STAND.md`, `RELEASE-1.0.2-BUILD48-RESULT.md` und `TAGEBUCH.md`. Die veröffentlichten Release-Artefakte wurden nicht bearbeitet.

Für diesen Auftrag geändert: zehn App-Quelldateien einschließlich der neuen Partial-Datei, vier Testdateien einschließlich zweier neuer Dateien und dieser neue Bericht. Der vollständige lokale Git-Status liegt zusätzlich unter `.build/cut-editing-modes-20261103/GIT-STATUS.txt`.

Der Entwicklungsbuild liegt unter `.build/cut-editing-modes-20261103/out/bin/CutAssistantNext.App/release/CutAssistantNext.App.exe`. Sinnvoller nächster Praxistest ist ein Film im grünen Modus einschließlich Speichern/Wiederaufnehmen, Schnitt und menschlicher Prüfung sämtlicher Übergänge und Tonspuren.

## Nachtrag 03.11.2026 – persönliche Anzeigeprüfung und Dokumentationsabgleich

Nach dem technischen Abschluss zeigt der Nutzer den tatsächlichen Einstellungsdialog mit beiden Arbeitsweisen. Anschließend bestätigen zwei Screenshots für **Chicago Fire S13E09 – Eine Gefälligkeit** die grüne Behaltemarkierung beziehungsweise die roten komplementären Entfernungsbereiche. In beiden Ansichten steht dieselbe geplante Ausgabedauer **00:39:28.320**. Die Rückmeldung „:-)“ bestätigt die gezeigte Darstellung. Das sind keine zusätzlichen Nachweise eines grünen Filmschnitts, der dynamischen Bedienung oder einer vollständigen Wiederaufnahme. Der zuvor bestätigte otr-can-Filmschnitt bleibt getrennt zugeordnet.

Der Nutzer meldet CAN danach geschlossen und beauftragt, README und Dokumentation auf Stand zu bringen. Die zentralen Übersichten, Nutzeranleitung, Testplan, Merkliste, Architektur und Versionszuordnung werden dafür aktualisiert; [Entwicklungsnotizen 1.0.3](RELEASE-NOTES-1.0.3.md) bündeln die Änderungen. Die im Implementierungsabschluss oben noch unverändert gesicherten Status-/Tagebuchdateien werden nun auf diesen ausdrücklichen Dokumentationsauftrag hin ergänzt. Produktcode, Version, Buildzähler und Release-Artefakte bleiben erhalten.
