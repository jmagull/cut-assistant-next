# Cut Assistant Next Build 61 – Frei editierbare Filmnamen

## Umsetzung am 17.11.2026

Die Namensmaske liefert weiterhin den Ausgangsvorschlag. In **Cutlist erzeugen** ist der fertige vorgeschlagene Filmname nun direkt editierbar. Die TwoWay-Bindung übernimmt die Eingabe während des Tippens. Der vorhandene Schutzstatus für geladene Cutlist-Namen wird auch für manuell bearbeitete Vorschläge verwendet. Metadaten- und Maskenänderungen ersetzen einen geschützten Namen nicht. **Aus obigen Eingaben neu erzeugen** hebt den Schutz bewusst auf und rendert die aktuell konfigurierte Maske; anschließend ist erneute manuelle Bearbeitung möglich.

Beim Speichern gelangt der manuelle Titel in SuggestedMovieName. ApplyToFile, OriginalName und Schnittbereiche werden weiterhin aus der ursprünglichen Aufnahme beziehungsweise dem bestehenden Schnittplan übernommen. NameTemplateRenderer und die Standardmaske sind unverändert; insbesondere bleibt %OriginalName% eine Referenz auf die ursprüngliche Aufnahme.

Die beiden Schnittmotoren verwenden weiterhin denselben editierbaren Ausgabedialog und denselben Zielpfad-Resolver. Der tatsächliche Ausgabename wird zusätzlich als getrennte, nur für die aktuelle Sitzung geltende Eigenschaft im CutNamingState erhalten. Auch beim Schließen des Ausgabedialogs vor dem Schneiden bleibt dieser Name für das nächste Öffnen mit dem anderen Motor erhalten. Dabei werden abgebrochene Metadatenänderungen nicht als neue Cutlist-Daten übernommen. Ein Medienwechsel verwirft den Zustand über den bestehenden Ablauf. Der Ausgabe-Dateiname und der vorgeschlagene Cutlist-Filmname bleiben getrennt bearbeitbar.

Leere und unter Windows ungültige vorgeschlagene Namen werden beim Cutlist-Speichern mit der bereits vorhandenen Dateinamensvalidierung abgewiesen. Während des Tippens darf das Feld vorübergehend leer sein. Die Cutlist speichert den Titel wie eingegeben, ohne automatisch eine MP4-Endung anzuhängen. Bei der tatsächlichen Schnittausgabe wird .mp4 wie bisher ergänzt, sofern noch nicht vorhanden. Originaldateischutz, explizite Überschreibbestätigung und sicheres Cutlist-Schreiben bleiben aktiv. Die bloße Titelbearbeitung schreibt keine fremde Cutlist zurück.

## Geänderte Dateien

- src/CutAssistantNext.App/Dialogs/CutlistGenerationDialog.xaml
- src/CutAssistantNext.App/ViewModels/CutlistGenerationViewModel.cs
- src/CutAssistantNext.App/ViewModels/CutOutputViewModel.cs
- src/CutAssistantNext.App/State/CutNamingState.cs
- src/CutAssistantNext.App/MainWindow.xaml.cs
- tests/CutAssistantNext.App.Tests/EditableMovieNameTests.cs
- Dieser Bericht, Build-61-Entwicklungsbericht und SETUP-MERKLISTE.md.

## Technischer Prüfbericht

- Release-Kompilierung und Windows-x64-Publish mit .NET 10: **0 Warnungen, 0 Fehler**; Dateiversion 1.0.3.61.
- Gezielte Naming-/Cutlist-/Output-Suite: **126 Tests bestanden**.
- Vollständige CAN-Suite: **2.167 Tests bestanden**, 0 fehlgeschlagen, 0 übersprungen. Core 200, Media 402, App 1.485, Cutlists 80.
- Formatprüfung der geänderten Namensklassen und neuen Tests bestanden. Der zusätzliche Check der gesamten MainWindow-Datei meldet eine schon vor dieser Änderung vorhandene einzeilige if/return-Formatierung (vorher Zeile 1705); diese fachfremde Stelle bleibt unverändert. git diff --check ist fehlerfrei.
- 14 zusätzliche Fälle prüfen Standard- und persönliche Masken einschließlich %OriginalName%, Metadatenänderungen nach manueller Eingabe, explizite Neugenerierung, erneute Bearbeitung, geladene Vorschläge, Cutlist-Datei-Roundtrip, unveränderte Originalzuordnung und Schnitte, leere/ungültige Namen sowie getrennte Ausgabe-/Cutlist-Titel und die Wiederverwendung des gemeinsamen Motor-Namenszustands.
- Ein tatsächlich geöffnetes WPF-Cutlist-Fenster prüft das sichtbare editierbare Textfeld, seine TwoWay-Bindung, die Übernahme der Eingabe und das Verhalten nach Metadatenänderung und Neugenerierung.
- Native Werkzeugtests mit den vorhandenen FFmpeg-/ffprobe-Referenzen vom 12.10., 11.11. und 14.11.2026 sowie MP4Box sind aktiviert; bestehende Codec-/Mehrspur- und Timing-/Fokus-/Shuffle-Regressionen laufen in der Gesamtsuite mit.
- Vollständiger lokaler Programmordner: vier libmpv-Dateien gegen die eingebundene Quelle verglichen; **104 otr-can-Manifestdateien** per SHA-256 geprüft. Gebündelter Motor meldet otr_can 0.1.6-audio-diag.1. Drittanbieterhinweise sind enthalten. Externe FFmpeg-, ffprobe-, GPAC- und FFMS2-Konfiguration bleibt wie bisher erforderlich.

Die lokale EXE liegt unter artifacts/review-1.0.3-build61-filmnamen/CutAssistantNext.App.exe im Repository-Hauptverzeichnis. Der Ordner enthält Testnachweise mit Build-/Test-/Publish-Protokollen, vier TRX-Dateien, Quellenstand und SHA256SUMS.txt. Er ist eine lokale Prüffassung, kein neu erstelltes Setup-Release.

## Persönlicher Test unter Windows 11 – noch offen

Die neue Prüffassung verwendet die vorhandenen CAN-Benutzereinstellungen. Vollständigen Programmordner beibehalten und die EXE direkt daraus starten; .NET Desktop Runtime 10 x64 ist erforderlich.

1. **MP4Box:** Mit einer persönlichen %OriginalName%-Maske einen Film öffnen. Im Schnittdialog den gesamten Ausgabenamen ändern, den Dialog schließen, otr-can öffnen und anschließend wieder MP4Box. Der Name bleibt erhalten. Einen kurzen Testschnitt ausführen und den tatsächlichen Dateinamen prüfen.
2. **otr-can:** Den Namen erneut ändern, Metadaten bearbeiten und einen kurzen Testschnitt ausführen. Der eingegebene Name muss verwendet werden. Vorhandene Zieldateien dürfen nur nach ausdrücklicher Bestätigung überschrieben werden.
3. **Cutlist erzeugen:** Das Feld „Vorgeschlagener Filmname (frei editierbar)“ bei unveränderter %OriginalName%-Maske zum Beispiel auf „Chicago Fire S13E07 – Unantastbar“ setzen. Eine Bewertung auswählen, als eigene Cutlist speichern und SuggestedMovieName sowie die unveränderte ApplyToFile-Zuordnung kontrollieren. Auch mit einer geladenen fremden Cutlist ausprobieren.
4. **Neugenerierung:** Nach manuellem Titel Metadaten ändern: Titel bleibt. „Aus obigen Eingaben neu erzeugen“ wählen: aktueller Maskenvorschlag erscheint. Erneut manuell ändern und speichern. Leere und ungültige Namen testen; anschließend einen zulässigen Titel eingeben.

Die neue persönliche Namensabnahme ist getrennt von den bereits bestandenen acht Realfilmschnitten. Wiederholtempo, Kontrollplayer-Fokus und fortlaufender Shuffle sind auf derselben Entwicklungsbasis enthalten.

## Späteres Batchcenter – ausschließlich Vorgabe

Das Batchcenter wird hier nicht implementiert. Für dessen spätere Entwicklung gilt verbindlich: Jeder Auftrag erhält einen maskenbasierten, individuell frei editierbaren Titel. Beim Einreihen werden der gewählte Ausgabename und die Originalzuordnung eingefroren; spätere globale Maskenänderungen verändern bestehende Aufträge nicht. Bis zur erfolgreichen Kontrolle und Freigabe ergänzt die Batchverwaltung eine eindeutige Batch-ID außerhalb des frei editierbaren Filmtitels. Erst der spätere Freigabeablauf darf diese ID aus dem Dateinamen entfernen, ohne vorhandene Dateien zu überschreiben. Intern bleibt die ID für Protokollierung und Nachvollziehbarkeit erhalten. Die konkrete Batch-Implementierung und ihr Freigabeablauf bleiben einem gesonderten Auftrag vorbehalten.

## Freigabestatus

Entwicklung auf codex/continuous-schnittshuffle auf Basis 13d7d04; Versionsnummer weiterhin 1.0.3 Build 61. Lokale Prüffassung zur persönlichen Abnahme. Kein Merge nach main, kein Release-Tag und keine Veröffentlichung.
