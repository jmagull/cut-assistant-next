# Cut Assistant Next – Kurze Endbereiche in Cutlists

Entwicklungsstand auf Basis von 1.0.3 Build 59 (Commit `8ed81ea`). Die Arbeit liegt im lokalen Zweig `codex/cutlist-end-validation`.

## Ursache

Cutlist 2081774 enthielt sechs Behaltebereiche. Der letzte reichte von 02:29:59,030 bis 02:29:59,080. Das Video endet bei 02:29:59,080 und hat 25 fps. Der einzige Bildanfang im Intervall liegt bei 02:29:59,040. Die AAC-Spur endet bei 02:29:58,632. Der Bereich liegt innerhalb der Videodauer. Build 59 übersprang bei `FramesPerSecond=0` die vorhandene Endfragmentprüfung. MP4Box lieferte danach Exitcode 0 ohne Segmentdatei. CAN brach den Auftrag beim Prüfen der Segmentdatei ab.

## Import und Vorprüfung

- Direkter Import und gemeinsame lokale/Server-Tabelle verwenden dieselbe fachliche Prüfung. Beim tatsächlichen Laden untersucht CAN auffällige Positionen in den letzten drei Videosekunden mit ffprobe. Der vorherige Tabellenbefund kann auf Metadaten beruhen.
- Fehlende, leere, nichtpositive und nicht endliche FPS-Werte behandeln wir bei Sekundenangaben als unbekannt. Eine unlesbare Angabe bleibt ein Formatfehler. Weichen zwei gültige Bildraten voneinander ab, zeigt CAN den Konflikt und lässt den Import erst mit einer passenden Cutlist zu. Reine Bildnummern werden anhand der FPS der Cutlist interpretiert. Bei Cutlists mit Sekunden und Bildnummern bleiben die Sekundenfelder maßgeblich.
- Aufsteigende Bildzeitstempel aus einer fehlerfrei bis zum erwarteten Videoende gelesenen Probe haben Vorrang. CAN zählt Bildanfänge ab dem Start bis ausschließlich zum Ende des Bereichs. Dadurch wirkt sich eine variable Bildrate bei vorhandenen zuverlässigen Zeitstempeln korrekt aus. Ein Bereich mit höchstens drei Bildanfängen gilt nahe dem Videoende als auffällig, wenn danach höchstens zwei weitere Bildanfänge folgen. Bei fehlenden verlässlichen Bildzeiten dient die Bildrate der geladenen Aufnahme zur Abschätzung. Wenn auch diese fehlt, kann eine gültige Cutlist-FPS eine Abschätzung liefern. In beiden Fällen zeigt CAN keine gemessene Bildanzahl an. Aussagen über früher endenden Ton erscheinen nur, wenn die Endzeiten aller erkannten Tonspuren bekannt sind.
- Liegt eine Grenze hinter dem ermittelten Videoende, weist CAN den Import mit beiden Zeitangaben ab. Ein Ende exakt am Videoende ist zulässig. Der Dialog für einen auffälligen, gültigen Bereich bietet **Unverändert laden**, **Letzten Behaltebereich entfernen** und **Abbrechen**. Beim Entfernen bleibt das Ende des vorherigen Bereichs erhalten. Die Quelldatei der Cutlist wird dabei nicht geändert. Ein einziger Behaltebereich kann behalten oder von Hand korrigiert werden.
- Für einen direkten MP4Box-Auftrag prüft CAN einen auffälligen Endbereich anhand des konfigurierten MP4Box-Programms vor dem restlichen Schnitt. Die Probe benutzt einen temporären Zielnamen. Bei bestätigtem fehlendem Segment kann der Bereich ausdrücklich entfernt oder der Auftrag abgebrochen werden. Ein erfolgreicher Probeschnitt verwendet im Auftrag dieselben Werkzeuggrenzen. Einbildbereiche dürfen mit ihrem exklusiven Ende ausgeführt werden, wenn der bisherige Abzug einer Bilddauer den Befehl zu einem leeren Intervall machen würde.
- Die MP4Box-Schnittstrecke prüft alle formatierten Intervalle vor dem ersten Segment. Exitcode 0 bei fehlender Datei führt zum Abbruch mit Segmentnummer, Quell- und Werkzeuggrenzen, Werkzeugmeldung und Korrekturhinweis. Eine vorhandene Zieldatei wird erst nach Fertigstellung ersetzt. Der Frame- und Werkzeugprobe sind Zeitgrenzen gesetzt (5 bzw. 15 Sekunden). Keyframe- und Streamcopy-Grenzen bleiben bestehen. Eine Probe mit vorhandener Ausgabedatei garantiert keine bildgenaue Grenze. Die Probe ist auf direkte MP4Box-Aufträge beschränkt. Bei vorbereiteten Quellen und dem MP4Box-Ersatzweg nach einem otr-can-Fehler gelten die Intervall- und Ausgabedateiprüfungen. Eine Kürzung überstehender Grenzen erfolgt derzeit manuell.

## Prüfergebnisse unter Windows 11

| Prüfung | Ergebnis |
| --- | --- |
| Release-Build der Solution | 0 Warnungen, 0 Fehler |
| CAN Core, Cutlists, Media und App | 200 + 80 + 402 + 1.432 = 2.114 bestanden, 0 übersprungen |
| Spotlight | 78 Tests und 61 native Integrationsprüfungen bestanden |
| Kontrollplayer und Hauptplayer | 35 native Kontrollplayer-Prüfungen und 12 Prüfungen zu Filmende/Schnittshuffle bestanden |
| otr-can und AAC | 20 native Audio-Prüfungen bestanden |
| Synthetische MP4Box-Fixtures | GPAC 26.07. Schnitt eines Einbildbereichs erfolgreich. Ein Bereich mit weit zurückliegendem Keyframe und vorher endendem AAC lieferte Exitcode 0 ohne Segment. Vorhandene Ausgabe blieb erhalten. Leere und rückläufige Werkzeugintervalle wurden vor dem ersten Aufruf abgewiesen. |
| Original-Cutlist mit sechs Bereichen | Endfragment erkannt, genau ein Bildanfang gemessen. Native Probe mit `-splitx 8999.03:8999.08` bestätigte Exitcode 0 ohne Datei. Nach simulierter Bestätigung der Entfernung lief der Fünf-Segment-Schnitt durch. |
| Separat manuell korrigierte Cutlist mit fünf Bereichen | Ohne Endfragmentwarnung übernommen und mit MP4Box geschnitten. Ergebnis 741.405.305 Byte, Laufzeit 01:31:11,975, eine Video- und eine Audiospur. |
| Lokale Build-59-Auslieferung | Alle 38 Einträge aus `SHA256SUMS.txt` verifiziert. |

Die Realfilmprüfungen liefen mit einem lokalen Testprogramm gegen die Anwendungslogik und den nativen Schnittmotor. Die WPF-Schaltflächen für Behalten, Entfernen und Abbrechen wurden automatisiert geprüft. Jörg hat den korrigierten Ablauf mit der Entwicklungsfassung unter Windows 11 persönlich bestätigt. Die Ergebnisse der gepackten Build-60-Fassung stehen im Paketbericht. Die Originalaufnahme wurde ausschließlich lokal geprüft.

## Geänderte Dateien

- [NUTZERANLEITUNG.md](NUTZERANLEITUNG.md), [CUTLIST-ENDGRENZEN.md](CUTLIST-ENDGRENZEN.md).
- [MainWindow.xaml.cs](../src/CutAssistantNext.App/MainWindow.xaml.cs), [CutlistResultRow.cs](../src/CutAssistantNext.App/Services/CutlistResultRow.cs), [Mp4BoxCutService.cs](../src/CutAssistantNext.App/Services/Cutting/Mp4BoxCutService.cs), [CutlistEndFragmentDialog.xaml](../src/CutAssistantNext.App/Dialogs/CutlistEndFragmentDialog.xaml), [CutlistEndFragmentDialog.xaml.cs](../src/CutAssistantNext.App/Dialogs/CutlistEndFragmentDialog.xaml.cs), [MainWindow.CutlistEnd.cs](../src/CutAssistantNext.App/MainWindow.CutlistEnd.cs), [CutlistImportReview.cs](../src/CutAssistantNext.App/Services/CutlistImportReview.cs).
- [MediaAnalysisResult.cs](../src/CutAssistantNext.Core/Media/MediaAnalysisResult.cs).
- [CutlistEndFragmentCorrector.cs](../src/CutAssistantNext.Cutlists/Editing/CutlistEndFragmentCorrector.cs), [CutlistEndFragmentDetector.cs](../src/CutAssistantNext.Cutlists/Editing/CutlistEndFragmentDetector.cs), [CutlistParser.cs](../src/CutAssistantNext.Cutlists/Serialization/CutlistParser.cs).
- [FfprobeFrameRunner.cs](../src/CutAssistantNext.Media/Analysis/FfprobeFrameRunner.cs), [FfprobeJsonParser.cs](../src/CutAssistantNext.Media/Analysis/FfprobeJsonParser.cs), [Mp4BoxCutWorkflow.cs](../src/CutAssistantNext.Media/Cutting/Mp4BoxCutWorkflow.cs), [Mp4BoxRunner.cs](../src/CutAssistantNext.Media/Cutting/Mp4BoxRunner.cs), [Mp4BoxSplitRangeBuilder.cs](../src/CutAssistantNext.Media/Cutting/Mp4BoxSplitRangeBuilder.cs), [Mp4BoxSplitRangeFormatter.cs](../src/CutAssistantNext.Media/Cutting/Mp4BoxSplitRangeFormatter.cs), [Mp4BoxSegmentException.cs](../src/CutAssistantNext.Media/Cutting/Mp4BoxSegmentException.cs).
- [CutlistEndFragmentDetectorTests.cs](../tests/CutAssistantNext.Cutlists.Tests/Editing/CutlistEndFragmentDetectorTests.cs), [Mp4BoxCutWorkflowSplitFailureTests.cs](../tests/CutAssistantNext.Media.Tests/Cutting/Mp4BoxCutWorkflowSplitFailureTests.cs), [Mp4BoxRunnerConcatIntegrationTests.cs](../tests/CutAssistantNext.Media.Tests/Cutting/Mp4BoxRunnerConcatIntegrationTests.cs), [CutlistEndFragmentDialogTests.cs](../tests/CutAssistantNext.App.Tests/CutlistEndFragmentDialogTests.cs), [CutlistEndReviewTests.cs](../tests/CutAssistantNext.App.Tests/Services/CutlistEndReviewTests.cs).

Die Korrektur wurde in Cut Assistant Next 1.0.3 Build 60 übernommen.
