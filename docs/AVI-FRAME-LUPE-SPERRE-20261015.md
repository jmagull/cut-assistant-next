# AVI-Frame-Lupe: vorläufige Sperre vom 15.10.2026

Auf Nutzerauftrag ist die Frame-Lupe in der lokalen Prüffassung für erkannte AVI-Container vorläufig gesperrt. Die Beschränkung gilt für den Hauptfenster-Button und den gemeinsamen Öffnungsweg von Button und Shift+F, sowohl mit Schnittkante als auch im Untersuchungsmodus. Sie greift vor Pause, Werkzeugauflösung oder Dialogaufbau. Der gelbe Hinweis und der auch am deaktivierten Button verfügbare Tooltip lauten:

> Die Frame-Lupe steht für AVI-Container derzeit nicht zur Verfügung.

Die vorhandene Container-Erkennung wird gemeinsam für Warnung, Keyframe-Hinweis und Lupenfreigabe verwendet; Dateiendungen entscheiden nicht. Während neuer Analyse und ohne analysierte Videospur ist der Button ebenfalls deaktiviert. Nach AVI → MP4 erfolgt die Freigabe erneut. Ein bisheriger Hinweis der Hauptplayersuche auf die nun gesperrte AVI-Lupe wurde entfernt; deren Zeitstempelprüfung bleibt erhalten. MP4-Lupen behalten ihre bisherigen Sicherheitsprüfungen: Die AVI-Sperre löst nicht jede mögliche Zeitstempelstörung in anderen Containern.

## Geänderte Dateien

- Core `Metadata/AviSourceWarningDetector.cs`: gemeinsame AVI-Erkennung und aktualisierter Hinweis.
- App `MainWindow.xaml`, `MainWindow.xaml.cs`, `ViewModels/MainWindowViewModel.cs`: Button, Tooltip, Öffnungsschutz und Benachrichtigungen bei Dateianalyse/-wechsel.
- App `ViewModels/KeyframeNavigationViewModel.cs`, `ViewModels/PlayerFrameSearchViewModel.cs`: widersprüchliche Bedienhinweise bereinigt.
- Drei bestehende Testdateien für Warnung, Hauptfenster-Freigabe und Keyframe-Hinweis; README, Nutzeranleitung, Status und Tagebuch aktualisiert.

## Prüfung

Separater Release-Rebuild mit festem Buildwert 30, ohne Restore: **0 Warnungen, 0 Fehler**. **1253/1253 Tests bestanden**, 0 übersprungen: Core 196, Cutlists 72, Media 153, App 832. Neun zusätzliche Fälle prüfen Container statt Endung, analysierte Videospur sowie Freigabewechsel; bestehende Tests sichern Analysefehler, Abbruch und AVI → MP4 ab.

Vier native Fälle mit echten synthetischen AVI-/MP4-Dateien in WPF und libmpv bestanden: AVI gesperrt, anschließende MP4 freigegeben, AVI unter `.mp4` weiterhin gesperrt und MP4 unter `.avi` freigegeben. Auch der direkte gemeinsame Öffnungsweg von Button/Shift+F und ein künstlich ausgelöstes Button-Ereignis öffnen bei AVI keinen Dialog und pausieren laufende Wiedergabe nicht. Der deaktivierte Button bietet den erklärenden Tooltip. Hinweisumbruch bei 760/960/1533 Fensterbreite geprüft, keine WPF-Bindungsfehler. Nachweise: `proof/PROOF.json`, `proof/BINDINGS.log`, zwei Bannerbilder; Bannerbild visuell kontrolliert. Die erste Helferausführung scheiterte an fehlenden nativen Bibliotheken im separaten Helferordner; nach Ergänzung des bestehenden unveränderten Vier-DLL-Satzes ist dieselbe Prüfung vollständig bestanden. Dies erforderte keine Produktkorrektur.

Alle acht Lockdateien sowie `Version.props` und `.build/build-number.txt` gegen ihre vorher gesicherten SHA-256-Werte identisch; die vier bereits zuvor geänderten Produktions-Lockdateien bleiben erhalten. `git diff --check` ohne Fehler.
Die projektspezifische Build-Hilfe erhöht den Buildzähler und wurde deshalb für diese Prüffassung durch `dotnet build ... --no-restore -t:Rebuild --artifacts-path ... -p:CanBuildNumber=30` ersetzt. Wiederverwendet werden vorhandene lokale NuGet-Metadaten. Kein Restore, keine neue Produktionsabhängigkeit, kein Paketbau und keine Veröffentlichung. Version/Build bleiben **0.3.0/30/RC3**; öffentliche Pakete bleiben unverändert.

## Persönlicher Schnittvergleich

Das vom Nutzer gelieferte Protokoll stammt jetzt ausdrücklich von **Die_Teufelsbrigade_21.01.21_20-15_tele5_160_TVOON_DE.mpg.HQ.avi**. FFmpeg kopiert Video und Ton in eine temporäre MP4, verifiziert alle **263694 Videopakete** und meldet dabei zahlreiche fehlende Eingangs-PTS. Anschließend gelingt otr-can 0.1.5 der vollständige Schnitt: **182574 Videopakete**, **350542080 dekodierte Audiosamples**, geprüfte Enddatei. Die nachfolgende persönliche Rückmeldung bestätigt **Ton, Bild und Übergänge als einwandfrei**. Der vorherige MP4Box-Lauf war ebenfalls technisch erfolgreich.

Dies bestätigt einen erfolgreichen AVI-Verarbeitungsfall, repariert oder widerlegt aber nicht die gesondert nachgewiesenen widersprüchlichen Bildzeiten von **No Turning Back**. Die AVI-Sperre ist eine bewusst konservative Bedienentscheidung, kein Nachweis, dass alle AVI-Dateien beschädigt sind. Schnittmotoren, Vorbereitung und Zuordnung wurden nicht verändert.

## Separater Startpfad

`C:\Users\joerg\Documents\GitHub\cut-assistant-next\.build\avi-loupe-block-20261015\compiled\bin\CutAssistantNext.App\release\CutAssistantNext.App.exe`

Build-, Test- und Oberflächennachweise liegen ausschließlich unter dem ignorierten Arbeitsordner `.build/avi-loupe-block-20261015`.