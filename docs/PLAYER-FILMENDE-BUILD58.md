# Play am Filmende in Cut Assistant Next 1.0.3 Build 58

## Vergleich der unveränderten Builds

Die drei Tests verwendeten dieselbe synthetische MP4-Datei mit 12,000 Sekunden Laufzeit, 25 fps und einer Tonspur. Die Timeline wurde über den WPF-Seek-Pfad bedient. Die CAN- und libmpv-Zustände, Positionen, Reglerwerte und Zahl der geladenen Tonspuren wurden protokolliert.

| Test | Position und Zustand | Ergebnis |
| --- | --- | --- |
| A – Build 56, Timeline | Das Slider-Ziel 12,000 s landete pausiert bei 11,960 s mit einer geladenen Tonspur. Play erreichte den Zustand `Ended` mit null Tonspuren. | Der Slider-Rücksprung auf 3,000 s löste `property unavailable` aus. CAN wechselte in `Error` und deaktivierte Slider und Play. |
| B – Build 57, Timeline | Gleiche Folge wie in Build 56. | Derselbe Fehler beim Rücksprung. |
| C – Build 57, Schnittshuffle | Der rechte Pfeil landete pausiert bei 11,960 s mit einer Tonspur. Der Slider ging vor Play auf 8,000 s zurück. Play lief bei 8,400 s und der Slider ging anschließend erfolgreich auf 3,000 s zurück. | Ein späteres Abspielen bis zum tatsächlichen EOF versetzte den Player in `Ended` mit null Tonspuren. Der anschließende Slider-Rücksprung auf 4,000 s löste denselben Fehler aus. |

Die Position auf dem letzten darstellbaren Bild allein löste den Fehler im Test nicht aus. Die Position blieb nach dem Entladen auf dem letzten gemeldeten Zeitwert stehen. Build 56 war damit bereits betroffen. Der persönliche Realfilm wurde nicht geöffnet oder verändert.

## Ursache und Korrektur

Vorher entlud libmpv die Datei am natürlichen EOF. CAN hatte Dauer und letzte Position weiter im Speicher und ließ in `Ended` einen Slider-Seek zu. `MpvMediaPlayerService.SeekAsync` setzte dabei über `SetDoublePropertyAsync` die native Eigenschaft `time-pos`. Ohne geladene Datei wies libmpv den Schreibzugriff mit `property unavailable` zurück. CAN wechselte in `Error`. Die Play/Pause-Schaltfläche war im reproduzierten `Ended`-Zustand bereits deaktiviert. Der Fehler kam aus dem Seek. Im WPF-Fehlerdialog lief der Dispatcher-Timer in A und B weiter und der Dialog konnte geschlossen werden. Das belegt einen gesperrten Bedienpfad und einen modalen Dialog. Ein UI-Thread-Deadlock wurde im kontrollierten Test nicht festgestellt.

Build 58 setzt im Hauptplayer libmpvs `keep-open=yes`. Am Ende bleibt die Datei geladen und pausiert auf dem letzten Bild. Play am tatsächlichen EOF lässt die Pause bestehen. Nach einem Seek zu einer früheren Position kann die Wiedergabe wieder starten. Der Hauptplayer behandelt einen vorübergehend nicht verfügbaren Property-Schreibzugriff als Fehler der einzelnen Aktion. CAN protokolliert den technischen Fehler und zeigt im Player einen Hinweis an. Gleichzeitige Play/Pause-Aktionen werden abgefangen, bis die laufende Aktion abgeschlossen ist.

Der Kontrollplayer behält seine bisherige EOF-Behandlung. Schnittshuffle verwendet weiterhin dieselben Sprungziele, Pfeilbuttons und dieselbe Auswahl- und Kantenlogik.

Die beiden Optionen sind im mpv-Handbuch beschrieben. [mpv-Handbuch](https://mpv.io/manual/stable/#options-keep-open).

## Geänderte Dateien

| Datei | Änderung |
| --- | --- |
| `src/CutAssistantNext.Media/Playback/MpvMediaPlayerService.cs` | Option für offenen EOF im Hauptplayer und sicheres Play bei `eof-reached`. |
| `src/CutAssistantNext.Media/Playback/Interop/HanumanLibMpvClient.cs` | Übergibt einen nicht verfügbaren Property-Schreibzugriff an den Aufrufer und vermeidet ein zusätzliches globales Fehlerereignis. |
| `src/CutAssistantNext.App/MainWindow.xaml`, `MainWindow.xaml.cs` | Aktivieren die EOF-Option im Hauptplayer und zeigen Wiedergabefehler direkt am Player. |
| `src/CutAssistantNext.App/ViewModels/PlaybackViewModel.cs` | Verhindert überlappende Play/Pause-Aktionen. |
| `tests/CutAssistantNext.Media.Tests/MpvMediaPlayerServiceTests.cs`, `tests/CutAssistantNext.App.Tests/PlaybackViewModelTests.cs` | Prüfen Play bei offenem EOF, Rücksprung und schnelles erneutes Play/Pause. |
| `tools/prototypes/SpotlightIntegrationChecks/Program.cs`, `Program.MainEof.cs` | Reproduzieren A, B und C an gebauten CAN-Versionen und prüfen den Hauptplayer mit echtem libmpv. |

## Prüfergebnisse und lokale Fassung

- Release-Build mit 0 Warnungen und 0 Fehlern.
- 2.075 CAN-Tests einschließlich MP4Box-Regression und der zwei neuen Unit-Tests bestanden.
- 21 native Hauptplayer-Prüfungen bestanden. Am tatsächlichen EOF meldete libmpv `eof-reached=yes`. CAN blieb bei 11,960 s pausiert, mit einer geladenen Tonspur und bedienbarem Slider. Der Rücksprung, Play und anschließende Bild-, Sekunden- und Keyframe-Navigation funktionierten. Die Schnittshuffle-Abfolge wurde dreimal wiederholt und die Tabellenzeile sowie die Bearbeitungskante blieben erhalten.
- Ein echter `time-pos`-Fehler ohne geladene Datei wurde im Test gezielt ausgelöst. Der Hinweis erschien im Player. Danach ließ sich eine Datei laden und normal bedienen.
- 78 Spotlight-Tests, 61 native CAN-/Spotlight-Integrationsprüfungen und alle sechs nativen Kontrollplayer-Testfälle bestanden.
- Berichte liegen unter `.build/main-eof-build58/`.

Die lokale EXE liegt unter `src/CutAssistantNext.App/bin/Build58/Release/net10.0-windows/CutAssistantNext.App.exe` im Arbeitsverzeichnis. Alle acht Prüfsummen des vorbereiteten Build-56-Pakets stimmen. Der vorhandene otr-can-Motor wurde lokal verknüpft. Bei der damaligen Entwicklungsprüfung wurde Build 58 weder installiert noch paketiert oder veröffentlicht. Der Anwender bestätigte anschließend den Realfilm-Praxistest mit der Entwicklungsfassung von Build 58. Die Abnahme der verpackten Ausgabe wird separat dokumentiert.
