# Usability-Änderungen auf Basis von Cut Assistant Next 1.0.3 Build 58

Der lokale Entwicklungsstand basiert auf dem Quelltag `v1.0.3-build58` und dem Commit `58281bf`. Der separate Checkout `codex/usability-build58` erhält die vorhandenen lokalen Änderungen im ursprünglichen Arbeitsordner. Die Dateiversion des Entwicklungsbuilds lautet 1.0.3.58.

## Änderungen und Dateien

| Bereich | Dateien |
| --- | --- |
| Sekundensprünge bei Tastendruck und Mausklick, Wiederholung und Abbruch | `MainWindow.SecondsJump.cs`, `MainWindow.xaml`, `MainWindow.xaml.cs`, `MainWindow.MediaInput.cs`, `Services/SecondsJumpRepeat.cs` |
| Schnittshuffle und Bearbeitungskante nach einer Korrektur | `ViewModels/CutPlanViewModel.cs` |
| Lautstärke des Kontrollplayers beim Öffnen und während der Wiedergabe | `Services/CutReviewController.cs`, `Dialogs/CutReviewWindow.xaml`, `Dialogs/CutReviewWindow.xaml.cs`, `ViewModels/CutReviewViewModel.cs` |
| Tooltips für belegte Dialogschaltflächen | `Dialogs/{AacEmergency,AppearanceSettings,AudioMp4BoxFallback,AudioTrackCopy,Credits,CutCompletion,CutlistRating,CutlistRatingQuestion,FrameLoupe,FrameLoupeSettings,Join,VideoFolderSettings,VideoInformation}Dialog.xaml` |
| Automatisierte Tests | `tests/CutAssistantNext.App.Tests/{CutPlanViewModelTests,KeyframeShortcutTests,PowerUser2LayoutTests,SecondsJumpRepeatTests}.cs`, `tests/CutAssistantNext.App.Tests/ViewModels/CutReviewViewModelTests.cs`, `tools/prototypes/ControlPlayerRegression/Program.cs` |
| Bedienhinweise | `README.md`, `docs/NUTZERANLEITUNG.md` |

App-Dateien ohne Pfadpräfix liegen unter `src/CutAssistantNext.App/`. Die Keyframe- und Frame-Lupe-Tooltips im Hauptfenster zeigen weiterhin ihre vorhandenen Kürzel. Die Pfeile für Schnittshuffle springen per Klick zur jeweiligen Schnittgrenze. Pfeiltasten im Hauptplayer dienen weiterhin der Frame-Navigation.

## Prüfung

- Release-Build mit 0 Warnungen und 0 Fehlern. C#-Formatierung im Ordnermodus und `git diff --check` geprüft.
- 2.090 CAN-Tests und 78 Spotlight-Tests bestanden, einschließlich der nativen MP4Box-Matrix. Die VSTest-Verbindung zum Testhost lief in der Arbeitsumgebung in ein Timeout. Die vollständigen Test-Assemblies wurden direkt mit xUnit 2.9.3 ausgeführt.
- Native WPF/libmpv-Prüfungen mit Testvideos bestanden. 35 Kontrollplayer-Prüfungen, 12 Prüfungen zu Filmende und Schnittshuffle sowie 61 Integrationsprüfungen für CAN und Spotlight.
- Zwei bestehende PowerShell-Prozesstests erreichten bei paralleler Ausführung das fristgerechte Ende nicht. Die separate Wiederholung und der anschließende vollständige Durchlauf bestanden.
- Jörg hat Tastatur- und Mauswiederholung, die erhaltene Schnittauswahl, den zentralen Lautstärkeregler, die Lautstärkeübernahme und die Tooltips mit dem Usability-Entwicklungsstand persönlich unter Windows 11 geprüft. Die persönliche Prüfung der neuen Build-59-Pakete steht aus. Windows 10 wurde nicht persönlich geprüft.
