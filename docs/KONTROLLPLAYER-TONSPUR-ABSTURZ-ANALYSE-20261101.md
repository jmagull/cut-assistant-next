# Kontrollplayer: Absturz nach Tonspurwechsel – Bestandsaufnahme

Stand: 01.11.2026. Community-Bericht von **sfonweb auf cutlist.at**, laut übermitteltem Entwicklungsauftrag. Untersuchte Quellbasis: `42d72fc`, Produktstand CAN 1.0.2 Build 47, libmpv 0.41.0. Dies ist die angeforderte Analyse vor gesonderter Implementierungsfreigabe.

## Ergebnis

Die Fehlerfolge ist mit einer künstlichen MP4-Datei mit zwei Tonspuren gegen die unveränderten Build-47-Produktbibliotheken reproduziert: Nach natürlichem Filmende scheitert der Tonspurwechsel mit der gemeldeten `ArgumentException (Parameter 'id')`; anschließend beendet „Vorherige Prüfstelle“ den separaten Diagnoseprozess durch eine unbehandelte `MpvException: property unavailable` auf dem Ereignisthread des Hanuman-libmpv-Wrappers.

Keine Verwechslung von ffprobe-Streamindex, Listenindex und libmpv-Track-ID im untersuchten Auswahlpfad. Entscheidend sind eine veraltete Spurliste nach EOF, ein irreführender Playerzustand nach dem fehlgeschlagenen Wechsel und ein Fehler im asynchronen Fehlerpfad des Wrappers. Die konkrete Ghosts-Datei wurde nicht getestet; deren vollständige Medien- und Bedienumstände sind damit nicht unabhängig bestätigt.

## Nachgewiesene Fehlerkette und betroffene Methoden

1. `MpvMediaPlayerService.GetAudioTracksAsync` in `src/CutAssistantNext.Media/Playback/MpvMediaPlayerService.Audio.cs` liest `track-list/count`, filtert nach `type=audio` und übernimmt ausdrücklich `track-list/{i}/id`. `i` dient ausschließlich zum Zugriff auf die Liste. ffprobe ist an der Tonspurauswahl des Kontrollplayers nicht beteiligt.
2. `CutReviewViewModel.LoadReadyAsync` speichert diese Spuren und die ausgewählte Spur beim Laden. Die Liste wird bei natürlichem Filmende nicht aktualisiert. Im Nachweis bleiben zwei Einträge im ViewModel, während libmpv nach EOF **null Tonspuren** meldet. Die IDs werden nicht durch die letzte Prüfstelle falsch berechnet; ihr bisheriger Medienkontext ist nicht mehr geladen.
3. `CutReviewViewModel.SelectAudioAsync` prüft nur die gespeicherte Liste und ruft vor dem eigentlichen Wechsel `PauseAsync` auf. Anders als `ReviewAsync` und `SeekAsync` lädt dieser Pfad bei `Ended` nicht neu.
4. `MpvMediaPlayerService.PauseAsync` setzt nach erfolgreichem Setzen von `pause=true` den Dienstzustand uneingeschränkt auf `Paused`, auch wenn zuvor `Ended` galt. Die Datei wird dadurch nicht neu geladen. Im Nachweis: **Ended → Paused**, weiterhin null aktuelle Tonspuren.
5. `MpvMediaPlayerService.SelectAudioTrackAsync` liest die tatsächliche Spurliste erneut. Die zuvor gültige ID fehlt jetzt. Genau dort entsteht die gemeldete Ausnahme mit Parameter `id`, noch vor dem Befehl `set aid`. Es handelt sich um eine eigene Prüfung von CAN, nicht um die Fehlermeldung einer ungültigen nativen ID-Übergabe.
6. Der Fehler tritt vor der Zuweisung von `_selectedAudioTrack` auf. `_completion`, `_index`, `_loadedPath` und die alte Auswahl bleiben erhalten; der Player wird nicht freigegeben. `ExecuteAsync` hebt die Bediensperre im `finally` auf. Der wesentliche beschädigte Zustand ist die Kombination **scheinbar pausiert / Datei tatsächlich beendet / gespeicherte Spuren noch vorhanden**. Der WPF-Pfad `CutReviewWindow.RunAsync` kann diese erste Ausnahme abfangen und im bestehenden Fehlertext anzeigen.
7. `MoveAsync(-1)` ruft `ReviewAsync` auf. Dessen Neuladeentscheidung berücksichtigt `Ended`, `Error`, `Empty`, einen geänderten Abschluss oder Dateipfad. Weil der Zustand nun `Paused` und der gespeicherte Pfad unverändert ist, entfällt das notwendige Neuladen. Es folgt ein Seek auf die nicht mehr geladene Datei.
8. `MpvMediaPlayerService.SeekAsync` verwendet `HanumanLibMpvClient.SetDoublePropertyAsync`, intern `SetPropertyAsync` mit `WaitForResponse=true` und `ThrowOnError=true`. Der Wrapper wirft beim asynchronen Setzfehler auf seinem eigenen Ereignisthread. Die Ausnahme erreicht den wartenden Task nicht regulär und liegt deshalb außerhalb der vorhandenen `try/catch`-Blöcke im Client, Dienst und Kontrollfenster. Der ganze Prozess endet.

Reproduzierter Stack:

```text
HanumanInstitute.LibMpv.MpvException: property unavailable
  at HanumanInstitute.LibMpv.MpvContextBase.ToPropertyChangedEventArgs(MpvEvent e)
  at HanumanInstitute.LibMpv.MpvContextBase.AsyncSetPropertyHandler(MpvEvent e)
  at HanumanInstitute.LibMpv.MpvContextBase.HandleEvent(MpvEvent e)
  at HanumanInstitute.LibMpv.MpvThreadEventLoop.ProcessEvents()
  at System.Threading.Thread.StartCallback()
```

Nur einen weiteren `catch` im Buttonhandler einzubauen würde diese Fehlergrenze nicht beheben.

## Reproduktion und Gegenproben

Isolierter Diagnoseaufbau unter `.build/review-audio-diagnosis-20261101/`, ohne Änderung der Produktions-Solution. Verwendet werden Kopien der veröffentlichten Build-47-Bibliotheken aus dem lokalen eingefrorenen `publish`-Verzeichnis. SHA-256-Abgleich der vier CAN-Bibliotheken, des Wrappers und der vier nativen Laufzeit-DLLs gegen diese Referenz erfolgreich. Der Diagnoseprozess erzeugt ein verborgenes HWND und ruft das originale `CutReviewViewModel` per Reflection auf; es handelt sich nicht um einen Nachbau seiner Zustandslogik.

Testdatei: acht Sekunden H.264, 160 × 90, 25 fps; zwei AAC-Sinustonspuren mit 48 kHz, Sprachkennungen `deu`/`eng`, erzeugt mit lokalem FFmpeg. Prüfstellen: Anfang, 3 s, 5 s, Ende 8 s. Start der Endkontrolle bei 5 s. Lautstärke während der automatisierten Kontrolle auf null gesetzt. Kein Schnittmotorlauf und keine reale OTR-Datei erforderlich.

| Diagnosefall | Beobachtung | Prozessende |
| --- | --- | --- |
| Tonspurwechsel an der Endprüfstelle vor EOF, danach zurück | IDs 1 und 2 gültig; Wechsel und Navigation erfolgreich, Seek 5 s bzw. 2 s | 0 |
| Natürliche EOF, dann Tonspurwechsel | Aktuell 0, gespeichert 2 Spuren; identische `id`-Fehlermeldung; Zustand danach `Paused`, aktuelle Prüfstelle weiter „Ende“, Zurück verfügbar | 0, Fehler erwartungsgemäß aufgefangen |
| Natürliche EOF, Tonspurfehler, danach zurück | Kein Neuladen; unbehandelte Wrapper-Ausnahme auf Ereignisthread | -532462766 / 0xE0434352 |
| Natürliche EOF, unmittelbar zurück ohne Tonspurwechsel | Zustand bleibt bis zur Navigation `Ended`; Datei wird neu geladen, Seek 2 s und Play erfolgreich | 0 |

Damit sind vier Diagnosefälle ausgewertet, darunter ein absichtlich reproduzierter Prozessabsturz. Dies sind keine vier bestandenen Regressionstests einer korrigierten Fassung. Logs: `healthy.log`, `eof-error.log`, `eof-nav.log`, `eof-control.log`; Aufbau: `Program.cs`, `Repro.csproj`; zusammengefasster Nachweis: `SUMMARY.json` im genannten lokalen Verzeichnis.

## Vorgesehene begrenzte Korrektur – noch nicht implementiert

1. **Medienkontext vor Tonspurwechsel sicherstellen:** Im Kontroll-ViewModel einen beendeten oder ungültigen Medienkontext vor `PauseAsync` neu laden, Bereitschaft abwarten und die aktuelle Spurliste einlesen. Die gewünschte ID gegen die neu geladene Datei prüfen; bei fehlender eindeutiger Zuordnung verständlich ablehnen. Kein Ersatz durch Listenpositionen und kein blindes Auswählen der ersten Spur.
2. **Fehlerzustand navigierbar erhalten:** Bei fehlgeschlagenem Wechsel den gespeicherten Ladezustand bei Bedarf ungültig machen, damit die nächste Navigation tatsächlich neu lädt. Prüfstellenindex und Abschluss erhalten. Tatsächliche Auswahl zurücklesen und die Anzeige damit synchronisieren; bei einem Fehler nach bereits erfolgtem `set aid` darf die Anzeige keine unbestätigte alte oder neue Auswahl behaupten. Die bestehende Bediensperre, Abbruch- und Freigabelogik beibehalten.
3. **Nachgewiesenen asynchronen Setzfehler absichern:** Den fehlerhaften Wrapperpfad für Property-Setzoperationen gezielt so ersetzen oder kapseln, dass native Fehlerrückgaben als beobachtbare Task-Ausnahme an den Aufrufer gelangen. Ein synchroner Setter außerhalb des UI-Threads ist ein zu prüfender Ansatz; beim optionalen Property-Getter existiert bereits ein entsprechendes Muster. Konkrete API, Abbruch und Lebensdauer vor Umsetzung lokal prüfen. Keine globale Ausnahmeunterdrückung, keine unbeauftragte Wrapper-/libmpv-Aktualisierung und kein allgemeiner Umbau der Wiedergabe.
4. **Vorhandene Diagnose verwenden:** Der erste `id`-Prüffehler wird aktuell im Fenster angezeigt, aber an dieser Stelle nicht über `IAppLogger` protokolliert. Technische Angaben zu angeforderter/bestätigter ID, aktuellem Inventar, Prüfstelle, Playerzustand und Ausnahme über den vorhandenen Logger ergänzen; weiterhin den bestehenden Fehlerbereich nutzen.

Voraussichtlich betroffene Produktdateien: `ViewModels/CutReviewViewModel.cs`, `Dialogs/CutReviewWindow.xaml.cs` und gegebenenfalls `Services/CutReviewController.cs` zur Logger-Weitergabe; `Playback/Interop/HanumanLibMpvClient.cs` für den nachgewiesenen Setzfehler sowie gegebenenfalls `Playback/MpvMediaPlayerService.Audio.cs` für gezielte Diagnostik. Der genaue kleinste Umfang ist bei der Umsetzung festzulegen. Schnittmotoren, Klebecenter-Verarbeitung und Cutlist-Logik haben im reproduzierten Fall keine ursächliche Beteiligung.

## Vorgesehene Regressionstests

Die vorhandenen Tests `ReviewAudioSelectionReplaysSameSpliceWithThreeSecondPreroll`, `SingleAudioTrackDoesNotShowUnnecessaryReviewSelection` und `AudioTrackSelectionUsesRealMpvIdsAndVerifiesReadback` prüfen Auswahl und ID-Zuordnung grundsätzlich. `TransitionAfterNaturalEndReloadsFileBeforeSeekAndPlay` prüft Navigation nach EOF allein. Ihr Zusammenspiel mit verschwindendem Spurinventar, fehlgeschlagenem Wechsel und nachfolgender Navigation ist nicht abgedeckt. Der bisherige ViewModel-Testplayer lässt jede ID-Auswahl erfolgreich sein und modelliert das Entladen am EOF nicht.

| Szenario | Geplanter Nachweis |
| --- | --- |
| Eine Tonspur, normale Navigation | Anfang → Übergänge → Ende und zurück, ohne unnötige Tonspurauswahl |
| Zwei gültige Spuren, wiederholter Wechsel | Tatsächliche, auch nicht fortlaufende IDs verwenden; bestätigte Auswahl und gleicher Prüfstellenindex |
| Wechsel an erster, mittlerer, letzter Prüfstelle vor EOF | Parametrisierte Fälle; Anfang bei 0, Übergang minus 3 s, Ende aus tatsächlicher Dauer minus 3 s |
| Wechsel nach natürlicher EOF | Live-Inventar entfällt bis zum Neuladen; danach gültige Auswahl, Seek und Play in richtiger Reihenfolge |
| EOF während eines laufenden Wechsels | Zeitlich gesteuerter Testplayer; keine Abhängigkeit allein von einem einmaligen Zustandstest |
| Wechsel scheitert, anschließend zurück | Originalfall sfonweb; Ladezustand wird wiederhergestellt, Prüfstelle bleibt bis zur Navigation erhalten |
| Wechsel scheitert, anschließend weiter | Mittlere Prüfstelle mit vorhandenem Nachfolger; kein Absturz, korrekter nächster Prüfstart |
| Ungültige ID, fehlendes Inventar, native Ablehnung oder fehlende Auswahlbestätigung | Verständlicher Fehler, konsistente Anzeige, Navigation weiter möglich |
| Fehler erst beim Seek nach erfolgreichem Spurwechsel | Tatsächliche Tonspur und Anzeige bleiben konsistent; anschließende Bedienung möglich |
| Drei-Sekunden-Vorschau | Vorlauf, Anfangsbegrenzung, sehr kurze Datei und tatsächliche statt nominaler Enddauer unverändert |
| Schließen/Abbruch während Neuladen und Tonspurwechsel | Keine Befehle gegen freigegebene Ressourcen und keine hängende Bediensperre |
| Native Property-Fehlerrückgabe | Separater Prozess mit echtem libmpv; Fehler wird auffangbar, Prozess überlebt; reine Stubs können den Wrapper-Absturz nicht absichern |

Zusätzlich einen automatisierten WPF-Nachweis für Auswahlbindung, Zurück/Weiter nach Fehler und Schließen vorsehen. Der bisherige native Diagnoseprozess prüft die originalen ViewModel-/Playerpfade, klickt aber keine WPF-Buttons und beobachtet keine Bindungsereignisse.

## Build, Tests und Erhaltung

- Unveränderter Quellstand als Release gebaut: **0 Warnungen, 0 Fehler**. Direkter Build mit `dotnet build CutAssistantNext.sln --configuration Release --no-restore --disable-build-servers -m:1`; bewusst kein zählererhöhender Paket-/Skriptbau während der Analyse.
- Vollständiger anschließender Lösungstestlauf: **1593/1593 bestanden**, 0 fehlgeschlagen, 0 übersprungen: Core **200**, Media **387**, App **934**, Cutlists **72**. TRX unter `.build/review-audio-diagnosis-20261101/verified-tests/`.
- Anfangsversuche mit eingeschränkter Ausführung scheiterten vor den Tests an lokaler Testprozesskommunikation (`SocketException 10013`, Verbindungszeitüberschreitung); sie zählen nicht als bestandene Tests. Wiederholung mit erlaubter lokaler Kommunikation erfolgreich. Der erste parallele Buildversuch beendete sich ohne aussagekräftige Compilerfehler; der anschließende serielle Build war erfolgreich. Der separate Diagnoseaufbau wurde nach Korrektur eigener Kompilierungsfehler ebenfalls mit 0 Warnungen/Fehlern gebaut.
- Keine neuen Produktionsabhängigkeiten, keine Produkt- oder Regressionsteständerung. Die Diagnosemedien und Hilfsdateien bleiben ignoriert unter `.build/`. Einzige neue versionierbare Datei ist dieser Analysebericht.
- Vier vorbestehende Produktions-Lockdateiänderungen erhalten; deren SHA-256-Werte sowie der Buildzähler sind gegenüber dem Analysebeginn unverändert. Release-Artefakte und Tag wurden nicht geändert; kein Commit, Push, Paketbau oder Release.

## Auswirkungen und verbleibende Grenzen

Die Kontrollplayer-Korrektur betrifft Wiederanlauf nach EOF, Tonspurbindung, Fehlerzustand, Navigation und Abbruch beim Schließen. Die Vorschau muss dieselbe Prüfstelle mit unverändertem Drei-Sekunden-Vorlauf wiedergeben. Ein erneutes Laden kann die libmpv-Standardspur zurücksetzen; die beabsichtigte bestätigte Auswahl muss deshalb ausdrücklich berücksichtigt werden.

Eine Absicherung des gemeinsamen Property-Setters kann auch Pause, Lautstärke und Seeking im Hauptplayer beeinflussen. Dafür sind gezielte Fehler-, Abbruch- und Zweiplayer-Regressionen erforderlich. Native Ressourcen müssen bis zum tatsächlichen Ende einer laufenden nativen Operation gültig bleiben; ein abgebrochener Wartevorgang allein beendet diese Operation nicht.

Nicht praktisch geprüft: die konkrete Ghosts-Datei, ihre tatsächlich geschnittene MP4Box-/otr-can-Ausgabe, persönliche Bild-/Ton-/Synchronitätsabnahme, die gesamte vorgeschlagene Testmatrix und eine korrigierte Fassung. Die bestandenen Bestandstests belegen daher keine Behebung. Der nächste Schritt ist die gesonderte Freigabe der hier beschriebenen Korrektur und Regressionstests im neuen Entwicklungsstand; Build 47 bleibt die unveränderte veröffentlichte Referenz.

## Nachtrag: Freigabe, Ghosts-Bestätigung und Umsetzung

Nach dieser Bestandsaufnahme hat der Nutzer die begrenzte Implementierung freigegeben. Er bestätigt am 01.11.2026 zudem die persönliche Reproduktion mit der tatsächlichen Ghosts-Datei im veröffentlichten Build 47: Erst natürliches EOF, danach Tonspurwechsel und Zurück lösen den Absturz aus; die letzte Prüfstelle allein genügt nicht. Diese persönliche Bestätigung ergänzt den obigen eigenen synthetischen Nachweis, ohne ihn nachträglich als eigenen Originaldateitest auszugeben.

Zusätzlich wurde der reguläre Positionssprung untersucht: Der bisherige explizite Seek nach jedem Tonspurwechsel sprang zum Vorschauanfang der ausgewählten Prüfstelle. Die ergänzte Freigabe verlangt hierfür Positionserhalt einschließlich Play/Pause; erneuter Drei-Sekunden-Vorlauf bleibt der bewussten Navigation beziehungsweise dem nötigen Wiederanlauf nach EOF vorbehalten. Die ursprünglichen Vorschläge zum erneuten Abspielen bei jedem Wechsel sind damit für reguläre Wechsel durch die spätere Nutzeranweisung ersetzt.

Implementierung und abschließende technische Prüfung sind im [Korrekturbericht](KONTROLLPLAYER-TONSPUR-KORREKTUR-20261101.md) dokumentiert: **1634/1634 Lösungstests** und **32/32 native/WPF-Kontrollen in sechs Prozessen** erfolgreich. Die persönliche Ghosts-Abnahme des korrigierten Entwicklungsstands ist noch offen. Die oben beschriebenen ursprünglichen Ergebnisse und Freigabegrenzen bleiben als datierte Analyse erhalten.
