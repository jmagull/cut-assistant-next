# Cut Assistant Next 1.0.3 Build 61 – Entwicklungs- und Testbericht

Der Branch `codex/build61` basiert auf `91a84267910f7154978b75bafc771b01fe67d9d9`. Dieser Commit ergänzt den veröffentlichten Build-60-Commit `6d7c7e7ceb0f6d0e6d54a0722550c53560c94b7b` um Dokumentation. Der genaue Paketquellstand steht in `SOURCE-SNAPSHOT.json` und in der Laufzeit-Provenienz des Prüfpakets.

| Punkt | Umsetzung |
| --- | --- |
| 1 Werkzeugprüfung | Versionsstatus und Funktionsstatus für FFmpeg/ffprobe und MP4Box. Prüfungen verwenden die konfigurierten EXE-Dateien, synthetische Medien und einen SHA-256-gebundenen Cache. Ein nachgewiesener Fehler blockiert nur den betroffenen Schnittweg. |
| 2 Tastenkürzel | Zentrale Belegungen der Hauptfensterbefehle, Tabelle mit Standard und persönlicher Belegung, Aufnahme per Tastendruck, Konfliktprüfung, Speicherung, Rücksetzen und aktuelle Tooltips. Dialogeigene Tastaturbefehle bleiben innerhalb der jeweiligen Dialoge. |
| 3 Wiederholung | Sekunden, Keyframes und Einzelbilder nutzen dieselbe serialisierte Wiederholsteuerung. 350 ms bis zur Wiederholung, anschließend mindestens 200 ms zwischen den Aufträgen. Tastatur und Maus, Freigabe bei Loslassen, Fokusverlust oder Deaktivierung. |
| 4 Blaues K | Unabhängige Markierung von Anfang und exklusivem Ende mit dem vorhandenen Keyframe-Index. Kein K vor Analyse oder bei fehlendem Index. |
| 5 Keyframe-Tooltip | Die feste Farberklärung zu roten Bereichen wurde entfernt. |
| 6 Motor-Empfehlung | Berücksichtigt Behaltebereiche, Keyframe-Lage, Format, Audio-Streams, vorhandene Werkzeuge und bekannte Selbsttestfehler. Beschränkt auf nachweisbar unterstützte H.264-MP4-Konstellationen. Die Motorwahl erfolgt weiterhin manuell. |
| 7 Qualitätsalternativen | Optionale Suche nach OTR-Standard-, HQ- und HD-Dateinamen derselben Aufnahme, Markierung in der gemeinsamen Tabelle und Bestätigung vor dem Import. Dateigrößen-, Bildraten- und Endgrenzenprüfungen bleiben im Importpfad. |
| 8 Schnittprüfung | Der Wiederaufruf übergibt die fertiggestellte MP4 und ihre ursprünglichen Prüfpositionen an den bestehenden Kontrollplayer. Medienwechsel und Beginn eines weiteren Schnittlaufs löschen den Eintrag. |

## Korrektur der Schnittlistenanzeige nach dem ersten persönlichen Test

In der ersten Prüffassung blieben die Zellen **Anfang** und **Ende** leer. Die **Dauer** war sichtbar. Das unabhängig gebundene blaue K erschien weiterhin an passenden Grenzen.

Ursache war die Inline-Syntax der beiden WPF-Bindungen in den neuen Zelltemplates. Die XAML-Markuperweiterung verarbeitete die Escape-Zeichen der TimeSpan-Formatierung beim Parsen, sodass dem Formatierer die für die Zeit-Trennzeichen erforderlichen Backslashes fehlten. Die Spalte **Dauer** nutzte bereits ein explizites Binding-Element und war deshalb nicht betroffen.

Die Zeittexte in beiden Zelltemplates verwenden nun das explizite Binding-Element mit derselben Formatierung wie **Dauer**. Das K bleibt ein eigener Textblock neben der Zeit, dessen Sichtbarkeit vom vorhandenen Keyframe-Index abhängt. Die Schnittdaten, Schnittlogik und Cutlist-Serialisierung sind unverändert.

Der Regressionstest arbeitet mit dem tatsächlich gerenderten Hauptfenster-DataGrid. Die ursprünglichen Templates zeigten im Test für einen vorhandenen Zeitwert einen leeren Text. Die korrigierte Fassung besteht alle sechs Kombinationen aus Rot-/Entfernen- und Grün-/Behalten-Modus mit Standard, Power-User 1 und Power-User 2. Je Kombination werden mehrere Zeilen, die drei Zeitspalten, beide Zustände der Keyframe-Markierung, eine Grenzkorrektur und das Laden einer bestehenden Cutlist geprüft.

Geänderte Dateien für diese Korrektur: MainWindow.xaml, CutToolLayoutTests.cs und dieser Entwicklungsbericht.

## Persönliche Abnahme – aktualisiert am 17.11.2026

**Acht von acht Realfilmschnitten persönlich bestanden:** Yaiba S01E07, Wild Kat S01E01, Die zauberhafte Weihnachtsfee und Meine zauberhaften Weihnachtsschuhe, jeweils mit MP4Box und otr-can unter Windows 11. Bild, Ton/Synchronität, Übergänge und Drei-Sekunden-Vorschau wurden positiv bestätigt. MP4Box bei Weihnachtsfee ist persönlich bestanden; das separate Motorprotokoll wurde nicht übergeben. [Vollständiger Praxisbericht und offene Abnahmepunkte](PRAXISTESTS-BUILD61-20261117.md).

Auf dem weiterführenden Branch codex/continuous-schnittshuffle sind Wiederholtempo und Fokus persönlich abgenommen; der erweiterte Shuffle wurde ebenfalls persönlich erfolgreich gemeldet. Die zuletzt dokumentierte gemeinsame Suite umfasst 2.153 bestandene Tests. Die folgenden technischen Ergebnisse mit 2.136 Tests gehören zur früheren Build-61-Basis.

## Prüfungen

- Release-Kompilierung unter Windows mit .NET SDK 10.0.303, 0 Warnungen und 0 Fehler. Die Gesamtsuite umfasst 2.136 bestandene Tests (Core 200, Media 402, App 1.454, Cutlists 80), ohne Fehlversuche oder übersprungene Fälle im abschließenden Lauf.
- FFmpeg/ffprobe 12.10.2026 (`615b07170e`) und 14.11.2026 (`604ef3784d`) liefern je 200 sichtbare Videopakete. Die Paketdaten, Reihenfolge und Zeitstempel entsprechen dem unabhängigen Sollwert. Der Build 11.11.2026 (`23c4540053`) liefert trotz Exitcode 0 nur 50 sichtbare Pakete und wird abgelehnt. Das aus einem guten und einem betroffenen Build gemischte Werkzeugpaar wird erkannt.
- GPAC 26.07 (`a07cbfff`) erzeugt für zwei definierte Bereiche die erwarteten H.264- und AAC-Pakete. Die mit `-add/-cat/-new` zusammengefügte MP4 enthält alle drei Spuren in richtiger Reihenfolge. Die bestehende native Matrix deckt zusätzlich AAC/AC-3, AAC/AAC, AC-3/AC-3 und kurze Endsegmente ab.
- Native synthetische Schnitte mit MP4Box und dem gebündelten otr-can 0.1.6-audio-diag.1 einschließlich FFMS2 ergaben jeweils eine vier Sekunden lange MP4 mit zwei AAC-Spuren. Sechs Kontrollplayer-Gruppen sowie Spotlight, Videoende und Schnittshuffle liefen erfolgreich.
- Der Kontrollplayer öffnete die frisch geschnittene otr-can-Ausgabe zweimal mit beiden Tonspuren und ihren Prüfpositionen. Der SHA-256-Wert der MP4 blieb gleich. Zwanzig zusätzliche native Audioregressionen liefen erfolgreich, darunter AAC-Zeitlückenreparatur, AC-3, mehrere Tonspuren und Rückfallpfade.
- Die persönliche Realfilm-Sicht- und Hörkontrolle am Windows-11-Arbeitsrechner ist für die acht oben genannten Schnitte bestanden. Eine persönliche VM-Abnahme dieser Fassung ist nicht bestätigt. Persönliche Tastenkürzel, alternative Cutlist-Qualitäten, fachliche Schlussbewertung der Motor-Empfehlung und finale Paketprüfung bleiben laut [aktualisiertem Praxisbericht](PRAXISTESTS-BUILD61-20261117.md) offen.

## Geänderte Dateien

- `README.md`
- `Version.props`
- `docs/BUILD61-ENTWICKLUNGSSTAND.md`
- `docs/NUTZERANLEITUNG.md`
- `docs/PRUEFHINWEISE-BUILD61.md`
- `docs/RELEASE-NOTES-1.0.3.md`
- `installer/HINWEIS-DOTNET-UND-WERKZEUGE.txt`
- `src/CutAssistantNext.App/Controls/KeyframeBoundaryVisibilityConverter.cs`
- `src/CutAssistantNext.App/CutAssistantNext.App.csproj`
- `src/CutAssistantNext.App/Dialogs/CutApplicationSettingsDialog.xaml`
- `src/CutAssistantNext.App/Dialogs/CutApplicationSettingsDialog.xaml.cs`
- `src/CutAssistantNext.App/Dialogs/CutlistSettingsDialog.xaml`
- `src/CutAssistantNext.App/Dialogs/FfmpegSettingsDialog.xaml`
- `src/CutAssistantNext.App/Dialogs/FfmpegSettingsDialog.xaml.cs`
- `src/CutAssistantNext.App/Dialogs/ShortcutSettingsDialog.xaml`
- `src/CutAssistantNext.App/Dialogs/ShortcutSettingsDialog.xaml.cs`
- `src/CutAssistantNext.App/MainWindow.CutEngineRecommendation.cs`
- `src/CutAssistantNext.App/MainWindow.CutReview.cs`
- `src/CutAssistantNext.App/MainWindow.CutToolsLayout.cs`
- `src/CutAssistantNext.App/MainWindow.CutlistSearch.cs`
- `src/CutAssistantNext.App/MainWindow.SecondsJump.cs`
- `src/CutAssistantNext.App/MainWindow.xaml`
- `src/CutAssistantNext.App/MainWindow.xaml.cs`
- `src/CutAssistantNext.App/Services/CutlistQualityAlternatives.cs`
- `src/CutAssistantNext.App/Services/CutlistResultRow.cs`
- `src/CutAssistantNext.App/Services/Cutting/ConfiguredCutEngineFactory.cs`
- `src/CutAssistantNext.App/Services/Cutting/ConfiguredMp4BoxCutServiceFactory.cs`
- `src/CutAssistantNext.App/Services/Cutting/CutEngineRecommendation.cs`
- `src/CutAssistantNext.App/Services/Cutting/Mp4BoxCutService.cs`
- `src/CutAssistantNext.App/Settings/CutlistServerSettingsStore.cs`
- `src/CutAssistantNext.App/Settings/FfmpegToolChecker.cs`
- `src/CutAssistantNext.App/Settings/OtrCanToolChecker.cs`
- `src/CutAssistantNext.App/Settings/PlaybackShortcuts.cs`
- `src/CutAssistantNext.App/Settings/ToolFixtures/mp4box-v1.mp4`
- `src/CutAssistantNext.App/Settings/ToolFixtures/streamcopy-v1.mp4`
- `src/CutAssistantNext.App/Settings/ToolFunctionChecks.cs`
- `src/CutAssistantNext.App/ViewModels/CutPlanViewModel.cs`
- `src/CutAssistantNext.App/ViewModels/CutlistSettingsViewModel.cs`
- `src/CutAssistantNext.App/ViewModels/FfmpegSettingsViewModel.cs`
- `src/CutAssistantNext.App/ViewModels/KeyframeNavigationViewModel.cs`
- `src/CutAssistantNext.App/ViewModels/MainWindowViewModel.cs`
- `src/CutAssistantNext.Media/Tools/FfmpegVersionPolicy.cs`
- `tests/CutAssistantNext.App.Tests/CutEngineRecommendationTests.cs`
- `tests/CutAssistantNext.App.Tests/CutToolLayoutTests.cs`
- `tests/CutAssistantNext.App.Tests/CutlistQualityAlternativesTests.cs`
- `tests/CutAssistantNext.App.Tests/KeyframeBoundaryVisibilityTests.cs`
- `tests/CutAssistantNext.App.Tests/PlaybackShortcutsTests.cs`
- `tests/CutAssistantNext.App.Tests/Settings/ToolFunctionChecksTests.cs`
- `tests/CutAssistantNext.Media.Tests/Tools/FfmpegVersionPolicyTests.cs`
