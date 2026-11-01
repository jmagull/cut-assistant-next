# Kontrollplayer – EOF-Absturz und Positionssprung korrigiert

Stand: 01.11.2026. Unveröffentlichter lokaler Entwicklungsstand auf Basis `42d72fc4074acf20e60dc10aea50865519e6ff08`. Implementierungsfreigabe einschließlich Ergänzung zum Positionserhalt vom Nutzer erteilt. Die veröffentlichte **CAN 1.0.2 Build 47** samt Tag und eingefrorenem Paket bleibt unverändert. Kein Versions-/Buildnummerwechsel, Commit, Push oder Release.

## Ergebnis

Der nach natürlichem Filmende reproduzierte Absturz ist im geprüften Entwicklungsstand behoben: Der Kontrollplayer lädt den beendeten Medienkontext vor einem Tonspurwechsel kontrolliert neu. Ein fehlgeschlagener Wechsel erzwingt vor weiterer Navigation eine erneute Prüfung beziehungsweise ein Neuladen. Native Property-Fehler kommen regulär am wartenden Task an, statt den Ereignisthread des gepinnten Wrappers und damit den Prozess zu beenden.

Der zusätzlich gemeldete Positionssprung hatte eine eigene Ursache: `SelectAudioAsync` führte nach **jedem** Spurwechsel ausdrücklich einen Seek zum Start der aktuellen Prüfstelle aus. An der Anfangsprüfstelle war das 0:00, auch wenn der Film inzwischen weitergelaufen war. Für diesen Sprung war kein Neuladen nötig. Reguläre Wechsel führen jetzt keinen Seek aus; die tatsächliche native Position bleibt erhalten. Laufende Wiedergabe wird nach dem kurzen Pausieren fortgesetzt, pausierte Wiedergabe bleibt pausiert.

Bewusste Navigation zu Anfang, Übergängen und Ende behält ihre bisherigen Ziele: Anfang bei null, Übergang minus drei Sekunden, Ende aus tatsächlicher Filmdauer minus drei Sekunden, jeweils an Dateigrenzen begrenzt. Muss nach EOF tatsächlich neu geladen werden, startet der Tonspurwechsel die Vorschau der aktuellen Prüfstelle kontrolliert erneut. Der Positions-/Pause-Erhalt gilt für den regulären Wechsel im gültigen geladenen Kontext.

## Ursachen und persönliche Ghosts-Rückmeldung

Die [technische Bestandsaufnahme](KONTROLLPLAYER-TONSPUR-ABSTURZ-ANALYSE-20261101.md) bleibt der historische Reproduktionsnachweis: veraltete Spurliste nach EOF, irreführender Zustand `Paused` nach `PauseAsync`, ausgelassenes Neuladen beim Zurückspringen und unbehandelte `MpvException: property unavailable` in `AsyncSetPropertyHandler` des Hanuman-Wrappers. Eine Verwechslung von ffprobe-Streamindex, Listenposition und libmpv-Track-ID war nicht die Ursache; diese Zuordnung wurde nicht verändert.

Der Nutzer bestätigt am 01.11.2026 zusätzlich die persönliche Reproduktion mit der tatsächlichen Ghosts-Datei und dem **veröffentlichten Build 47**: normale Wiedergabe und letzte Prüfstelle allein unauffällig, erst **natürliches EOF → Tonspurwechsel mit `id`-Meldung → Vorherige Prüfstelle** führt zum Absturz. Dies ist ein unabhängiger persönlicher Nachweis für den alten Stand. Die nachfolgend beschriebenen eigenen technischen Tests verwenden künstliche Medien. Eine persönliche Abnahme der korrigierten Fassung mit Ghosts wird daraus ausdrücklich nicht abgeleitet.

## Tatsächlich geänderte Dateien und Methoden

Alle folgenden Pfade beziehen sich auf die Repositorywurzel.

| Datei | Änderung |
| --- | --- |
| `src/CutAssistantNext.App/ViewModels/CutReviewViewModel.cs` | `SelectAudioAsync` erkennt ungültigen Medienkontext, lädt nötigenfalls neu, prüft aktuelle IDs und liest die bestätigte Auswahl zurück. Regulärer Wechsel ohne Seek und mit Erhalt von Play/Pause. Fehler invalidieren den Ladevermerk, aktualisieren soweit möglich die tatsächliche Auswahl und protokollieren Details. `NeedsReload`, `ReloadAsync`, `RefreshAudioAsync` und `ReviewPosition` bündeln die unmittelbar benötigten Wiederanlauf-/Zeitregeln; Navigation und manuelles Seeking verwenden sie. `ReviewAsync` berücksichtigt auch EOF während des Pausierens. |
| `src/CutAssistantNext.App/Dialogs/CutReviewWindow.xaml.cs` | Vorhandenen `IAppLogger` an das ViewModel weitergeben; vorhandener Fehlerbereich und bestehende Fehlerbehandlung bleiben bestehen. |
| `src/CutAssistantNext.App/Services/CutReviewController.cs` | Logger an das Kontrollfenster weitergeben. |
| `src/CutAssistantNext.App/Dialogs/CutReviewWindow.xaml` | Zugänglichkeitsbeschreibung der Tonspurauswahl beschreibt jetzt den Positionserhalt statt erneuten Abspielens. Keine neuen UI-Elemente. |
| `src/CutAssistantNext.Media/Playback/MpvMediaPlayerService.cs` | `PauseAsync` ändert den Wiedergabezustand atomar nur von `Playing` nach `Paused`; `Ended`, `Empty`, `Loading` und `Error` werden durch das Setzen der Pause-Eigenschaft nicht zu vermeintlich geladenen Kontexten. |
| `src/CutAssistantNext.Media/Playback/Interop/HanumanLibMpvClient.cs` | `SetPropertyAsync` verwendet die tatsächlich vorhandene synchrone `MpvContextBase.SetProperty<T>`-API auf einem Worker. `AccessPropertyAsync` serialisiert native Property-Zugriffe mit `DisposeAsync`; auch der bereits synchrone optionale Getter verwendet diesen Lebensdauerschutz. Keine Wrapper-/libmpv-Aktualisierung und keine globale Ausnahmeunterdrückung. |
| `tests/CutAssistantNext.App.Tests/ViewModels/CutReviewViewModelTests.cs` | Bestehenden Testplayer um Medienverlust am EOF, tatsächliche Auswahländerung und gezielte Fehler-/Abbruchsteuerung ergänzt. Bisherigen Tonspurtest auf Positionserhalt ohne erneuten Seek angepasst und entsprechend umbenannt. Keine Tests entfernt. |
| `tests/CutAssistantNext.App.Tests/ViewModels/CutReviewAudioRegressionTests.cs` | 38 zusätzliche parametrisierte beziehungsweise einzelne Fälle für Auswahl, EOF, Fehler, Navigation, Zeitgrenzen, Positionserhalt und Schließen. |
| `tests/CutAssistantNext.Media.Tests/MpvMediaPlayerServiceTests.cs` | Drei zusätzliche Zustandsfälle für Pause nach EOF/Fehler beziehungsweise im leeren Kontext. Der bestehende Weiterleitungstest signalisiert jetzt vor Pause einen geladenen Medienkontext, statt aus `Empty` eine aktive Wiedergabe abzuleiten. |
| `tools/prototypes/ControlPlayerRegression/ControlPlayerRegression.csproj`, `Program.cs`, `README.md` | Dauerhaft reproduzierbarer separater Windows-x64-Testprozess gegen die gebauten Produktbibliotheken; sechs Prozessfälle mit realem libmpv, zwei HWND-Kontexten und originalem WPF-Kontrollfenster. Keine Aufnahme in die Produktions-Solution und keine neue Paketabhängigkeit. |
| `docs/KONTROLLPLAYER-TONSPUR-KORREKTUR-20261101.md` | Dieser Abschlussbericht. Der Analysebericht erhält einen datierten Verweis auf Freigabe, Nutzerbestätigung und Umsetzung. |

MP4Box, otr-can, Klebecenter-Verarbeitung, Cutlists und die grundsätzlich korrekte Track-ID-Zuordnung sind unverändert.

## Fehlerbehandlung und Ressourcenlebensdauer

Bei einem fehlgeschlagenen Tonspurwechsel bleiben Abschluss und Prüfstellenindex erhalten. `_loadedPath` wird ungültig gemacht, damit nachfolgende Navigation nicht blind einen scheinbar noch geladenen Kontext verwendet. Die tatsächliche Auswahl wird nach erfolgreichem Wechsel sowie nach Fehler soweit lesbar aus libmpv zurückgelesen. Bei nicht lesbarem Inventar wird keine alte Auswahl als bestätigt ausgegeben. Beim Wiederladen wird eine zuvor bestätigte Spur nur dann wiederhergestellt, wenn ihre tatsächliche ID vorhanden ist; eine fehlende Spur erzeugt eine Meldung statt einer blind ausgewählten Ersatzspur.

Das vorhandene Protokoll nennt angeforderte ID, tatsächlich bestätigte ID beziehungsweise „unbekannt“, aktuelles Spurinventar mit Auswahlstatus, Playerzustand, Prüfstellenindex/-text und die Ausnahme. Fehlgeschlagene Inventarabfrage wird ebenfalls protokolliert. Der Nutzer sieht Fehler weiterhin im vorhandenen Kontrollfensterbereich, ohne zusätzliche Dialoge.

Die Property-Korrektur umgeht ausschließlich den nachgewiesen fehlerhaften asynchronen Setterpfad des gepinnten Wrappers. Native Fehlerrückgaben werden weiterhin als Fehler behandelt. Ein abgebrochener Aufruf darf den Kontext nicht freigeben, solange die synchrone native Operation noch läuft: Die Property-Sperre bleibt bis zum tatsächlichen Task-Ende gehalten, und Dispose wartet darauf. Abbruch vor Erwerb der Sperre verhindert die Operation. Bei bereits laufenden nativen Aufrufen wird Cancellation nach deren Ende beobachtet; es wird kein lediglich abgebrochener Wartevorgang mit weiterlaufendem Zugriff zurückgelassen.

## Abschließende automatisierte Lösungstests

Release-Build der vollständigen Solution: **0 Warnungen, 0 Fehler**. Versionseigenschaften und lokaler Buildzähler unverändert; deshalb bewusst direkter Build ohne das zählererhöhende Paket-Buildskript.

```powershell
dotnet build .\CutAssistantNext.sln --configuration Release --no-restore --disable-build-servers -m:1 --verbosity minimal
dotnet test .\CutAssistantNext.sln --configuration Release --no-build --logger trx --results-directory .build/review-audio-fix-20261101/final-tests -m:1
```

| Testsuite | Bestanden | Fehler / übersprungen |
| --- | ---: | ---: |
| Core | 200 | 0 / 0 |
| Media | 390 | 0 / 0 |
| App | 972 | 0 / 0 |
| Cutlists | 72 | 0 / 0 |
| **Gesamt** | **1634** | **0 / 0** |

Ausgangsstand 1593; **41 zusätzliche Lösungstestfälle**: 38 App und drei Media. Der vorherige Bestand bleibt erhalten. Der gezielte Kontrollplayer-Lauf vor dem letzten Formatierungsschritt bestand **76/76 Fälle**; alle sind im finalen vollständigen Lauf enthalten und werden nicht nochmals zur Gesamtzahl addiert.

Abgedeckt sind eine Tonspur mit normaler Navigation, wiederholte Auswahl nicht fortlaufender realer IDs, erste/mittlere/letzte Prüfstelle, sehr kurze Dateien und tatsächliche Dauer, Wechsel nach EOF, EOF während Pause/Wechsel, Fehler mit nachfolgendem Zurück und Weiter, ungültige/verschwundene IDs, fehlendes Inventar, fehlende Auswahlbestätigung, Fehler nach bereits angewendeter Auswahl, Fehler beim anschließenden EOF-Seek sowie Schließen/Abbruch bei Inventarabfrage, Neuladen, Auswahl und Seek einschließlich wartender Navigation. Acht neue Positionserhalt-Fälle prüfen vier Prüfstellen jeweils während Play und Pause mit mehreren Wechseln bei fortgeschrittener Position; dabei darf kein neuer Seek oder Reload entstehen.

Während der Entwicklung gab es korrigierte Kompilierungsfehler und einen fehlgeschlagenen Zwischenlauf wegen der beschriebenen alten Pause-Testvorbedingung. Diese werden nicht als erfolgreiche Nachweise gezählt. Die Formatierung benötigte erlaubte lokale Hilfsprozesskommunikation; der zunächst eingeschränkte Aufruf scheiterte am Named-Pipe-Zugriff. Formatierung danach erfolgreich, auf die betroffenen C#-Dateien begrenzt. Finale Build-/Testergebnisse siehe oben; `git diff --check` erfolgreich.

## Native und WPF-Nachweise des finalen Stands

Der neue Testprozess referenziert die tatsächlich gebauten App-/Core-/Media-Bibliotheken; deren Kopien im Testausgabeordner sind per SHA-256 identisch zu den finalen Entwicklungsbibliotheken. Wrapper und native Laufzeit wurden nicht aktualisiert. Interne Methoden und das Kontrollfenster werden per Reflection aus der Produktassembly aufgerufen, nicht als nachgebaute Logik. Testdatei wie im ursprünglichen Nachweis: acht Sekunden H.264, 160 × 90, 25 fps, zwei AAC-Spuren mit 48 kHz. Lautstärke im automatisierten Lauf auf null gesetzt.

Alle **sechs getrennt gestarteten Prozesse** endeten mit **Exitcode 0**, insgesamt **32/32 Kontrollen**:

| Prozessfall | Kontrollen | Ergebnis |
| --- | ---: | --- |
| `regular` | 7 | Vier echte Wechsel während Play: 1,280 → 1,320 s; danach dreimal 1,320 → 1,320 s. Pausierter Wechsel 1,320 → 1,320 s, weiterhin pausiert. Mittlere Prüfstelle und Position erhalten; bewusste Navigation weiterhin drei Sekunden vor Ende. |
| `property` | 3 | Echter Seek ohne geladenen Film erzeugt weiterhin `MpvException: property unavailable`, jetzt regulär am wartenden Task auffangbar. Prozess überlebt; nach Laden funktionieren Pause, Seek, Play und Lautstärke. |
| `eof` | 10 | Tatsächlich verschwundenes Inventar nach natürlichem EOF; Pause erhält `Ended`; gültiger Wechsel lädt neu und spielt korrekte Endvorschau. Gewöhnliche EOF-Navigation lädt neu und behält die gewählte Spur. Nicht verfügbare ID nach EOF bleibt kontrollierter Fehler; anschließend Zurück erfolgreich, kein Prozessabsturz. |
| `two-player` | 3 | Zwei echte HWND-/libmpv-Kontexte: Kontrollplayer-Wechsel nach EOF und Navigation verändern Hauptplayerposition, -zustand und -tonspur nicht. Hauptplayer nach Kontrollplayer-Dispose weiterhin mit Play/Pause/Seek bedienbar. |
| `lifetime` | 4 | Deterministisch wartender Property-Zugriff abbrechbar, Dispose wartet auf dessen Zugriffsschutz, Zugriff nach Dispose wird abgelehnt, wiederholtes Dispose sicher. |
| `wpf` | 5 | Originales WPF-Kontrollfenster mit echtem libmpv: bestätigte Tonspur korrekt gebunden; gezielt injizierte Ablehnung stellt tatsächliche Auswahl wieder her; Zurück und Weiter nach Fehler funktionieren; Schließen bricht einen gezielt blockierten Auswahlaufruf ab und gibt den nativen Player frei. |

Der im alten Build unbehandelte **native** Property-Fehler wird im `property`-Fall weiterhin wirklich ausgelöst. Es handelt sich nicht um einen bloß simulierten Fehler. Beim gültigen Spurwechsel nach EOF im korrigierten Stand entsteht die alte `id`-Meldung dagegen nicht mehr, weil kontrolliert neu geladen wird. Zusätzlich wird eine tatsächlich nicht vorhandene ID 999 geprüft, um Fehler plus anschließendes Zurück unabhängig vom erfolgreichen EOF-Wechsel abzusichern. Die Ablehnung und Blockierung im WPF-Fall sind ausdrücklich Testinjektionen; Wiedergabe, Auswahlliste und Fenster sind real.

Laufanleitung: [ControlPlayerRegression/README.md](../tools/prototypes/ControlPlayerRegression/README.md). Native Buildprüfung ebenfalls 0 Warnungen/Fehler. Finale Nachweise unter `.build/review-audio-fix-20261101/`: `final-build.log`, `final-native-build.log`, `final-tests/*.trx`, sechs `final-native-*.json` und `FINAL-SUMMARY.json`. Die nativen Einzelkontrollen werden nicht mit den Lösungstests zu einer gemeinsamen Zahl vermischt.

## Git- und Release-Erhaltung

HEAD bleibt `42d72fc4074acf20e60dc10aea50865519e6ff08`; Release-Tag `v1.0.2-build47` bleibt erhalten (Tagobjekt `f62b0673e8c3615f258a796664b9944b622e63f2`). Es wurde kein Commit, Push oder Tag-/Releasebefehl ausgeführt.

Die vier schon vorliegenden Änderungen an `src/CutAssistantNext.{App,Core,Cutlists,Media}/packages.lock.json` bleiben vorhanden und wurden nicht bearbeitet; ihre SHA-256-Werte stimmen mit dem Ausgangszustand überein. Dasselbe gilt für `Version.props` und `.build/build-number.txt`. Der bereits vor Implementierungsbeginn unversionierte Analysebericht bleibt erhalten und erhält nur den unten beschriebenen Nachtrag.

Neu beziehungsweise geändert sind ausschließlich die oben aufgeführten Produkt-/Test-/Dokumentationsdateien und die drei Dateien des nativen Testwerkzeugs. Sie sind nicht gestaged oder committet. Hilfsdaten, Medien, JSON-Proofs und Buildausgaben liegen in ignorierten Verzeichnissen und gehören nicht zum Quellumfang.

Alle **acht eingefrorenen Auslieferungsdateien** wurden gegen ihre bestehenden veröffentlicht dokumentierten SHA-256-Werte geprüft: sechs Assets gegen `SHA256SUMS.txt`, beide Prüfsummenmanifeste gegen den [Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md). Zusätzlich stimmen **250/250 Dateien** des eingefrorenen Portable-`publish` mit `PUBLISH-FILES.sha256` überein. Nachweise: `RELEASE-PRESERVATION.json`, `PUBLISH-PRESERVATION.json`. Lokale Entwicklungsbibliotheken wurden ausschließlich unter den normalen `src`-/`tests`-/Werkzeug-Buildverzeichnissen neu gebaut.

## Auswirkungen, Grenzen und nächster Schritt

Der gemeinsame Property-Pfad betrifft auch Hauptplayer-Pause, Seek und Lautstärke. Die vollständigen vorhandenen Tests und der native Zweiplayer-/Wiederanlaufnachweis sind erfolgreich. Die Korrektur ändert keine Schnittzeiten, Cutlists, Medienausgaben oder Schnittmotoren. Die leichte Pause beim Tonspurwechsel bleibt; ein regulärer Wechsel startet weder Film noch Prüfstelle erneut.

Noch offen ist die persönliche Bild-/Ton-/Synchronitätsabnahme der **korrigierten Entwicklungsfassung**, insbesondere mit der tatsächlichen Ghosts-Datei und realen Schnittausgaben. Der Nutzer hat die Originaldatei im alten veröffentlichten Build getestet; im neuen Stand wurden keine eigenen Ghosts-/Realfilm-Schnitte und keine neuen Setup-/Portable-/VM-Abnahmen durchgeführt. Der native Hauptplayer-Nachweis verwendet einen zweiten echten Dienst/HWND-Kontext; er ist keine persönliche Abnahme sämtlicher Hauptfensterbedienelemente. Verborgene Testfenster ersetzen keine Hör-/Sichtprüfung.

Cancellation beendet einen bereits laufenden synchronen nativen Property-Aufruf nicht gewaltsam; Dispose wartet auf dessen Ende. Ein dauerhaft blockierter nativer Aufruf würde dadurch auch das Schließen verzögern. Ein solcher Block wurde nicht beobachtet; der Lebensdauerschutz verhindert in diesem Fall die vorzeitige Freigabe statt einen weiterlaufenden Zugriff abzukoppeln. Allgemeine Decoder-/Wrapperfehler außerhalb des hier geprüften Property-Pfads sind nicht pauschal ausgeschlossen.

Die beauftragte Implementierung und technische Prüfung sind abgeschlossen. Nächster sinnvoller Schritt ist die gesonderte persönliche Abnahme des lokalen Entwicklungsstands mit Ghosts und den Tests A–D. Paketierung, Versionsvergabe, Commit, Push und Veröffentlichung bleiben gesonderten Aufträgen vorbehalten.

## Nachtrag: persönliche Praxisabnahme und Releasefreigabe

Der Nutzer bestätigt am 01.11.2026 die korrigierte Entwicklungsfassung mit **Ghosts – Weihnachtsgeister, Teil 1 (23.10.2026)** und **Teil 2 (26.10.2026)**: ursprünglicher EOF-Absturzfall, regulärer Tonspurwechsel und Prüfstellen-Navigation einwandfrei. Die zuvor offene persönliche Ghosts-Abnahme ist damit erfolgt. Neue Setup-/Portable-/VM-Abnahmen werden daraus nicht abgeleitet. Commit, Push, Paketierung, Tag **v1.0.2-build48** und stabile Latest-Veröffentlichung sind ausdrücklich freigegeben; abschließende Paketprüfung vor Veröffentlichung bleibt verbindlich. [Build-48-Freeze](FREEZE-1.0.2-BUILD48-20261101.md).

**Veröffentlichungsnachtrag:** Die Korrekturen sind nach persönlicher Ghosts-Teil-1-/Teil-2-Abnahme, Releasefreigabe und vollständiger Paketprüfung als **CAN 1.0.2 Build 48 / Latest** veröffentlicht. [Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD48-PUBLISHED-20261101.md). Build 47 bleibt unverändert.
