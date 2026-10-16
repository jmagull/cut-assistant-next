# Zwei Ansichten, Startparameter und Drag & Drop – Entwicklung zu CAN 1.0

**Fortschreibung 16.10.2026:** Persönlich geprüfter Funktionsstand für **CAN 1.0.0** eingefroren; Setup, Portable und zugehörige Artefakte beauftragt. [Verbindlicher Freeze-Umfang und offene Paketabnahme](FREEZE-1.0.0-20261016.md). Die folgenden Angaben dokumentieren den vorausgegangenen Entwicklungs- und Prüfzeitpunkt.

Fortsetzung vom **16.10.2026**: Drag & Drop im realen Nutzerlauf positiv bestätigt; rechte Power-User-Werkzeuge nach abgestimmter Skizze verfeinert. Aktueller Stand und neuer Startpfad im [Feinschliff-Bericht](POWER-USER-FEINSCHLIFF-20261016.md). Die folgenden ursprünglichen Größen-/Prüfangaben beschreiben den Stand vom 15.10.; Player und Tabelle behalten weiterhin ihren Elternbaum, die rechten Werkzeuge werden im Folgestand zwischen vorhandenen Layoutcontainern umgeordnet.

Stand: 15.10.2026. Entwicklungsauftrag, keine Release-Freigabe. Ziel **CAN 1.0.0**, Buildnummer erst anhand des später abgenommenen Stands. Version.props, RC3-Kennung, lokaler Buildzähler sowie eingefrorene Releases und Archive bleiben während dieser Umsetzung erhalten.

## Architekturprüfung vor dem Umbau

`App.OnStartup` erzeugt genau ein `MainWindow`. Dieses besitzt je ein `MpvMediaPlayerService`, `PlaybackViewModel`, `MainWindowViewModel` und `CutPlanViewModel`. `MpvVideoHost` erzeugt mit `BuildWindowCore` ein untergeordnetes STATIC-HWND; libmpv bindet sich einmal mit `wid` daran. Die native Ereignisschleife und Freigabe liegen im Mediendienst. Die Initialisierungsaufgabe wird einmal gespeichert; libmpv wird vor dem HWND freigegeben.

Sichere Layoutstrategie: dauerhaft dieselben XAML-Controls und denselben visuellen Elternbaum behalten. Nur Grid-Zeilen/-Spalten, Spans und Größen ändern. Kein Reparenting, kein Unload/Reload, kein zweites ViewModel und keine zweite Playerinstanz. Standard: bisherige Anordnung und 350-DIP-Videofläche. Power User: Playerbereich links über die freie Höhe, Schnittbereich rechts, GridSplitter. Bei schmalen Fenstern vorübergehend Standardanordnung mit Hinweis; gespeicherte Präferenz bleibt erhalten. Die rechte Seite erhält einen eigenen ScrollViewer und die Tabelle weiterhin ihre eigene Bildlaufleiste.

Startparameter und Drop führen in denselben Medienladeweg wie der vorhandene Dateidialog. Eine gemeinsame Bestätigung erfolgt vor Zurücksetzen vorhandener Schnittarbeit. Pfadauflösung und Existenzprüfung laufen außerhalb des UI-Threads. Start erst nach Loaded und Playerinitialisierung. Drag & Drop benötigt zusätzlich zum WPF-Ziel einen nativen OLE-DropTarget am Video-HWND, weil dieses WPF-Ereignisse verdeckt. Kein Eingabeoverlay. Native Registrierung beim Erzeugen, Abmeldung vor dem Zerstören; Dateiinformationen aus CF_HDROP werden freigegeben.

## Umsetzung

- `App.xaml.cs`: Startargumente und ursprünglichen Arbeitsordner an das bestehende Hauptfenster übergeben.
- `MainWindow.xaml.cs` / `MainWindow.MediaInput.cs`: vorhandenen Ladeablauf einmal als `LoadMediaFileCoreAsync` herausziehen; Startparameter, Dateidialog und Drop verwenden ihn. Gemeinsame Schutzabfrage vor dem Zurücksetzen von Schnittarbeit. Keine parallelen Ladevorgänge oder neue Instanzkommunikation. Abbruch der neuen Pfad-/Analyseoperationen beim Beenden.
- `Services/MediaOpenRequest.cs`: Argumentanzahl, Pfadauflösung und Dateiprüfung; UNC-Normalisierung unabhängig von erreichbaren Netzwerkfreigaben testbar. Drop-Endungen entsprechen dem bisherigen Videofilter, Dateidialog und Startpfad bleiben für andere Dateiendungen offen.
- `Controls/VideoFileDropTarget.cs` und kleine Ergänzung in `MpvVideoHost.cs`: natives OLE-`IDropTarget` am vorhandenen HWND, dazu WPF-Drop am umgebenden Videorand. Asynchrone Existenzprüfung, Kopiercursor/blauer Rand bei gültigem Ziel, kein Overlay. `CF_HDROP`-Speicher freigeben und Registrierung vor HWND-Zerstörung beziehungsweise beim kontrollierten Beenden abmelden. Veraltete Drag-Prüfergebnisse nach Leave/Dispose ignorieren.
- `MainWindow.xaml` / `MainWindow.Layout.cs`: dieselben Controls im selben Elternbaum neu anordnen. Standard weiterhin 350-DIP-Videozeile; Power User nutzt eine Sternzeile ohne feste Videohöhe. Rechte Spalte startet bei 430 DIP, Mindestbreiten 610/340 DIP einschließlich Innenabständen, Splitter 8 DIP. Beim Verkleinern auch zuvor verbreiterte Schnittspalten einpassen.
- `Dialogs/AppearanceSettingsDialog.*` / `Settings/AppearanceSettingsStore.cs`: sofortige Auswahl und persistente Präferenz in separater `appearance-settings.json` im bestehenden Settings-Ordner; Standard als Vorgabe, ungültige Einstellungen fallen darauf zurück. Bestehende andere Einstellungen werden nicht neu geschrieben.
- Zwei neue Testdateien für Pfade/Argumente, Drop-Vorauswahl, Bestätigungsbedarf und Einstellungs-Rundlauf/Fehlerfälle. Keine neue Produktionsabhängigkeit, keine Änderung an Schnittmotoren, Medienanalyse, AVI-Schutz oder Cutlist-/Uploadformat.

## Endgültige Prüfungen

Vollständiger isolierter Release-Rebuild: **0 Warnungen, 0 Fehler**. Vollständige Suite: **1343/1343 Tests**, 0 übersprungen; Core 196, Cutlists 72, Media 194, App 881. **35 neue Tests** gegenüber Build 31. Nur geänderte C#-Dateien formatiert; `git diff --check` erfolgreich.

Zehn native Fallgruppen mit echtem WPF/libmpv: drei Größenvergleiche, Splitterbedienung per Tastatur und Verkleinern einer verbreiterten Spalte, native OLE-Registrierung/echte CF_HDROP-Daten/ungültige Ablagen, abgebrochener Dateiwechsel, vollständiges Drop-Laden einer AVI, Wechsel zurück zu MP4 sowie Keyframes/Halbierung/Lupenvorschau in beiden Ansichten. Startparameter als relativer Pfad durch den produktiven Lade-/Analyseweg erfolgreich. Beim Wechsel kein einziges HWND neu erzeugt; Schnittobjektidentität und pausierte Position erhalten, laufende Wiedergabe nicht unterbrochen. AVI-Suchzeile/Lupe weiterhin gesperrt, bei MP4 wieder verfügbar. Keine WPF-Bindungsfehler.

| Fenster in logischen Einheiten | Standard: Video-Höhe | Power User: Video-Höhe |
|---|---:|---:|
| 1920 × 1080 | 348 DIP | ca. 615 DIP |
| 1536 × 864 | 348 DIP | ca. 423 DIP |
| 1280 × 720 | 348 DIP | ca. 423 DIP |

Die letzten beiden Größen bilden die ungefähren logischen Arbeitsflächen bei Full HD mit 125/150 % Skalierung nach. Dies ist **kein Test tatsächlicher DPI-/Mehrmonitorwechsel**. Bei geringer Höhe verwendet Power User mindestens 840 DIP Arbeitshöhe mit äußerem Bildlauf; dadurch bleiben größere Vorschau und Navigation erreichbar. Unter 1100 DIP Inhaltsbreite Standardanordnung mit verständlichem Hinweis; Präferenz beim Verbreitern wiederhergestellt. Ein erster Test zeigte bei wenig Höhe eine kleinere Vorschau; diese Layoutschwäche wurde vor der endgültigen Prüfung durch die scrollbare Mindestarbeitsfläche behoben.

Zusätzlich **36 bestehende WPF/libmpv-Kontrollplayerprüfungen** bestanden: Anfang, Übergänge, Ende, Navigation, Play/Pause, Positions-/Lautstärkeregler, EOF, fehlende/defekte Ausgaben und Freigabe beim Schließen. Hauptplayer und Schnittzustand bleiben erhalten.

Drei synthetische Schnittläufe mit unveränderten konfigurierten Motoren: MP4Box direkt, otr-can 0.1.5 direkt und MP4Box nach lokalem Cutlist-Speichern/-Laden. H.264 320×180, 25 fps, AAC/AC3; jeweils drei Behaltebereiche, rund 12 s Ausgabe, beide Tonspuren erhalten und alle Streams vollständig dekodierbar. Exakter manueller Ausgabename, Zeitbereiche, Original-SHA-256, Schutz bestehender Zieldateien und aufgeräumte Arbeitsverzeichnisse geprüft. Kein Upload an einen echten Server; Uploadregression durch bestehende automatisierte Tests, Cutlist-/Namensformat unverändert. Keine persönliche Bild-/Tonabnahme daraus abgeleitet.

Die native Online-Cutlistsuche wird im Prüfhelfer bewusst übersprungen, ohne Benutzereinstellungen oder Produktionscode dafür zu verändern; der echte Medienlade-/Analyseweg bleibt aktiv. Erste isolierte Helferläufe benötigten zusätzlich explizites Vorladen der nativen DLLs; die korrigierten Helfer bestehen. Zwei anfängliche Locked-Restore-Versuche zeigten die bereits bestehende unterschiedliche Runtime-Kennzeichnung der lokalen Produktions-/Test-Lockdateien; rein lokale Test-Runtime-Vorgabe korrigiert, keine Lockdatei umgeschrieben.

## Lokale Nachweise und Startpfad

Prüffassung aus dem aktuellen Arbeitsbaum, **keine Paketierung**:

`.build/can-views-20261015/compiled/bin/CutAssistantNext.App/release/CutAssistantNext.App.exe`

Aus dem vollständigen Ordner starten; die vier nativen DLLs liegen daneben. Die unveränderte Anzeige **0.3.0 · Build 31 · RC3** kennzeichnet hier weiterhin den nicht erhöhten Versionsstand; dieser Arbeitsbaum unterscheidet sich funktional von den eingefrorenen öffentlichen Build-31-Paketen. Keine stabile Version 1.0 oder neue veröffentlichte Buildnummer behauptet.

Nachweise unter `.build/can-views-20261015`: `release-build.log`, `tests-final.log`, vier TRX unter `test-results-final`, `proof/PROOF.json`, leeres `proof/BINDINGS.log`, Layout-PNGs, `review-proof/proof.json` und `engine-results/RESULT.json`. Vorherige gescheiterte Helferdiagnosen bleiben zur Nachvollziehbarkeit erhalten; maßgeblich sind die abschließenden erfolgreichen JSON-Nachweise. Die Helferreferenzen zeigen auf die isolierten neuen Assemblies.

Wegen des ausdrücklich nicht freigegebenen Versions-/Buildwechsels wurde das zählererhöhende `tools/build.ps1` durch den gleichwertigen vollständigen Release-Rebuild ohne Zähleränderung ersetzt:

```powershell
$localTargets = Join-Path $PWD '.build/can-views-20261015/BuildOverrides.targets'
dotnet restore .\CutAssistantNext.sln --locked-mode --artifacts-path .build/can-views-20261015/compiled "-p:DirectoryBuildTargetsPath=$localTargets"
dotnet build .\CutAssistantNext.sln --configuration Release --no-restore --artifacts-path .build/can-views-20261015/compiled "-p:DirectoryBuildTargetsPath=$localTargets" -p:CanBuildNumber=31 -t:Rebuild
dotnet test .\CutAssistantNext.sln --configuration Release --no-build --no-restore --artifacts-path .build/can-views-20261015/compiled "-p:DirectoryBuildTargetsPath=$localTargets"
```

Die rein lokale Targets-Datei setzt `RuntimeIdentifiers=win-x64` ausschließlich für Projekte mit `.Tests` am Namensende, passend zu deren vorbestehenden Lockdateien. Alle acht Lockdateien, Version.props und `.build/build-number.txt` gegen die vor Beginn gespeicherten SHA-256/Werte geprüft: unverändert. Vier vorbestehende Produktions-Lockänderungen weiterhin vorhanden. Eingefrorene Archive, Tags und Release-Dateien nicht angefasst.

## Verbleibende persönliche Abnahme

- Reale HD-Aufnahmen: Start mit/ohne Argument, gültige/fehlende Datei, tatsächliche Explorer-Drag-Gesten in beiden Ansichten, Dateiwechsel bestätigen/abbrechen.
- Während pausierter/laufender Wiedergabe mit bearbeiteter Cutlist umschalten; Splitter mit Maus, Maximieren/Verkleinern/Wiederherstellen, Präferenz nach Neustart prüfen.
- Echte Windows-Skalierung 100/125/150 % und Wechsel zwischen unterschiedlich skalierten Monitoren. Kleine Fenster und rechte Bildlaufleiste einschließlich aller Schnittaktionen prüfen.
- Reale HD-Schnitte mit MP4Box und otr-can einschließlich Anfang, Ende, Schnittstellen, jeder Tonspur und Synchronität; Cutlist-Erstellung, echter Upload, Namensmasken und Kontrollplayer persönlich bestätigen.
- Neue Installation/Deinstallation/Portable-Abnahme in einer Windows-11-VM erst mit später ausdrücklich freigegebenen Paketen. Frühere Build-31-Abnahmen nicht auf diese Weiterentwicklung übertragen.

Bis dahin keine persönliche Gesamtfreigabe, kein endgültiger Freeze und keine Veröffentlichung. AVI bleibt experimentell; vorhandene Zeitstrukturgrenzen und MP4Box-Zwei-AAC-Befund bleiben bestehen.

## Zielversion und Freigaben

Die drei Arbeitspakete bilden den geplanten Funktionsabschluss für 1.0. Nach Umsetzung folgt Stabilisierung, Regression, Dokumentation und persönliche Abnahme mit realen HD-Aufnahmen sowie Installation/Deinstallation/Portable in Windows 11. Keine weiteren größeren Funktionen ohne ausdrückliche Nutzerentscheidung. Endgültiger Freeze, Versionserhöhung, Commit, Push, Tag, Pakete und stabile GitHub-Veröffentlichung benötigen jeweils die gesonderte Nutzerfreigabe. Vorhandene technische Grenzen, insbesondere AVI und MP4Box bei zwei gleichartigen AAC-Spuren, bleiben dokumentiert.
