# Spotlight-CAN – einheitlicher EOF-Abschluss in Build 55

## Ergebnis und Bedienung

Gezielte Korrektur derselben lokalen Entwicklungsfassung **Cut Assistant Next 1.0.3 Build 55** in der bestehenden Arbeitskopie .build/worktrees/community-feedback. Keine neue Anwendung oder Umstrukturierung.

1. Erstes zu entfernendes Bild wählen und **Schnittbereich Beginn setzen** drücken.
2. Ohne ein weiteres Bild zu wählen **Schnitt bis Videoende abschließen** drücken.
3. Der vorgemerkte Beginn wird mit der bereits bekannten tatsächlichen Mediendauer als Ende zu einem vollständigen Entfernen-Bereich. Dieser steht sofort in der normalen Schnittbereichsliste und zählt für Original-/Entfernen-/Restdauer sowie Transfer und Export.

Ohne offenen Beginn ist der Button deaktiviert; ein direkter Methodenaufruf liefert einen verständlichen Hinweis und ändert nichts. Nach erfolgreichem Abschluss wird der vorgemerkte Beginn gelöscht und der Button wieder deaktiviert. Ein wiederholter Klick kann keine doppelte Markierung erzeugen. Bereits abgeschlossene Bereiche bleiben erhalten. Bei Überschneidung bleiben Liste und offener Beginn unverändert, mit Hinweis. Ein Anfang an EOF erzeugt keinen Nullbereich. Ein unbestätigter Korrekturentwurf wird nicht durch den EOF-Button übernommen. Normale Bereichskorrektur und **Bereich löschen** gelten auch für diesen Bereich.

## Entfernte Sonderlogik und Wiederverwendung

Vollständig entfernt: ausgewähltes Bild als letztes Behaltebild, gesonderter VideoEnd-Zustand/Label, zusätzlicher automatisch vereinigter Nachlauf, WithVideoEnd-Helfer, Videoende-aufheben-Button, Frame-Endresolver im Hostvertrag und die nur dafür eingeführte SpotlightFrameBoundary-Klasse. Diese Klasse war noch eine unversionierte lokale Ergänzung und wurde gelöscht. Die allgemeine CAN-Frameanalyse und Frame-Lupe bleiben unverändert.

Der vorhandene MarkEnd-Ablauf und der neue EOF-Button verwenden denselben internen Abschlusshelfer CompletePendingEnd. Er prüft zunächst die gesamte bestehende Liste einschließlich neuem Bereich mit CutPlan.Validate, bevor etwas geändert wird. Danach wird der Bereich ganz regulär gespeichert. Keine zusätzliche Frameanalyse, kein weiterer FFmpeg-/ffprobe-Aufruf und keine Verschiebung um eine Bilddauer: EOF ist exakt die bereits bekannte Mediendauer. Die aktuelle Bildauswahl wird beim Abschluss nicht erneut gelesen; der vorgemerkte Anfang bleibt maßgeblich.

Kein zusätzlicher Schnittmodus. CANs vorhandene atomare SpotlightMediaSession.TryApply-/CutPlanViewModel.LoadCutPlan-Konvertierung bleibt unverändert: Rot zeigt die Entfernen-Bereiche, Grün ihre komplementären Behalte-Bereiche. Der letzte grüne Behaltebereich endet am Beginn des bis EOF entfernten Bereichs; es gibt keinen zusätzlichen Behalte-Nachlauf. Offene Bereiche können nicht übertragen werden. Bestehende CAN-Sekundensprünge, Einzelbilder, Keyframes, Frame-Lupe, Halbierungssuche, Schnittmotoren und Audioreparatur werden in diesem Nachtrag nicht verändert.

## Konkreter Praxisfall

| Wert | Erwartet und automatisiert bestätigt |
| --- | --- |
| Offener Beginn | 02:00:03.560 |
| Tatsächliche Dateidauer | 02:09:58.000 |
| Entfernen-Bereich | 02:00:03.560–02:09:58.000 |
| Entfernte Dauer | 00:09:54.440 |
| Restdauer ohne weitere Werbeblöcke | 02:00:03.560 |

Zusätzlich derselbe Fall mit zwei vorherigen Werbeblöcken sowie Übernahme nach Rot und Grün geprüft. Zeiten werden im Test exakt aus Dezimaltext/Ticks aufgebaut; auch submillisekundengenaue Anfänge und eine Entfernung von nur einem Tick vor EOF bleiben erhalten.

## Geänderte Dateien dieses Nachtrags

Unter external/spotlight-can:

- src/SpotlightCan.Wpf/MainViewModel.cs: regulärer Abschlusshelfer, EOF-Aktion und Buttonverfügbarkeit; Sonderzustand entfernt; Liste ist wieder die einzige Grundlage für Laufzeit/Transfer/Export.
- src/SpotlightCan.Wpf/MainWindow.xaml und MainWindow.xaml.cs: neuer eindeutiger Buttontext und normaler Clickhandler; alte Sondermarkierung/-aufhebung entfernt.
- src/SpotlightCan.Wpf/SpotlightHostOptions.cs: entfallenen Frame-Endresolver entfernt.
- src/SpotlightCan.Core/CutPlan.cs: gesonderte Nachlaufvereinigung entfernt; vorhandene Validierung und Komplementbildung bleiben.

Im CAN-Repository:

- src/CutAssistantNext.App/MainWindow.Spotlight.cs: nicht mehr benötigten Resolver entfernt; bestehende Sitzungs-/Übergabe-/Fensterlogik erhalten.
- src/CutAssistantNext.App/Services/SpotlightFrameBoundary.cs: entfallene lokale Spezialklasse entfernt.
- tests/CutAssistantNext.App.Tests/Services/SpotlightCommunityFeedbackTests.cs: bisherige Sondermarker-Tests auf den beauftragten EOF-Ablauf umgestellt; positive/negative Fälle, echte Buttonbindung, Praxiszeiten, neun Werbeblöcke und Cutlist-Rundlauf.
- tests/CutAssistantNext.App.Tests/Services/SpotlightMediaSessionTests.cs: reguläre EOF-Bereiche ohne Spezialhelfer sowie vier Praxisfall-Varianten mit/ohne Werbung in Rot/Grün.
- tools/prototypes/SpotlightIntegrationChecks/Program.cs: vorhandener nativer Prüfhelfer drückt den tatsächlichen EOF-Button nach Beginn; prüft deaktivierten/aktivierten Zustand und vollständigen Bereich in beiden Modi.
- README.md und docs/SPOTLIGHT-README.md: korrigierte Bedienung.
- docs/COMMUNITY-FEEDBACK-20261109.md und docs/PLAYER-SEKUNDEN-NAVIGATION-BUILD55.md: historische Sondermarker-Angaben eindeutig mit diesem Nachtrag verknüpft.
- docs/SPOTLIGHT-VIDEOENDE-BUILD55.md: dieser Bericht.

Alle übrigen lokalen Änderungen gehören zu den vorherigen beauftragten Build-55-Arbeiten. Namen, Solution, Projekte, Build-/Paketierungskonzept und Motorablage tools/otr-can/otr_can.exe bleiben erhalten. Keine neuen externen Werkzeuge oder Produktionsabhängigkeiten. Der vorhandene Prüfhelfer liegt weiterhin unter tools/prototypes.

## Build und Prüfungen

Release-Build mit CanBuildNumber=55 erfolgreich: **0 Warnungen / 0 Fehler**. Keine neue Versions- oder Buildnummer. Formatierung der umgestellten Regressionstests und Diff-Prüfung durchgeführt.

- **1909 CAN-Tests bestanden:** 200 Core, 80 Cutlists, 392 Media, 1237 App; 0 Fehler/übersprungen.
- **78 bestehende Spotlight-Tests bestanden:** 59 Core/Media, 19 WPF; 0 Fehler/übersprungen.
- **51 native Integrationsprüfungen bestanden:** tatsächliches CAN-/Spotlight-Fenster, bestehender libmpv-Player, ffprobe und künstlicher Zwölf-Sekunden-Film. Beide Schnittmodi mit echten Vorschaubildern, EOF-Button ohne/mit offenem Beginn, Abschluss ohne zweite Bildauswahl, Tabellen/Timeline, Laufzeiten, Cutlist-Rundlauf, Ersetzen/Ablehnen, Abbruch und Ressourcenfreigabe. CAN-Sekundensprünge ohne unerwünschte Zustandswechsel, Einzelbild-/Keyframe-Navigation und Eingabeschutz erneut bestanden.

Weitere gezielte Fälle: kein Bereich ohne offenen Beginn, beliebiger exakter Anfang bis EOF, neun vorherige Werbeblöcke, normale Löschung/Korrektur, doppelte Ausführung, überlappende Bereiche, unveränderter offener Beginn bei Fehler, angrenzende Bereiche, kein Nullbereich, vollständiger Listen-/Export-/Transferplan. Für den EOF-Abschluss verbietet ein Test-Werkzeug jede zusätzliche Analyse/Bilderzeugung; die Prüfung besteht trotzdem. Die früheren Behaltebild-/Frame-Endresolver-Tests wurden durch die nun maßgeblichen EOF-Fälle ersetzt, keine weiterhin benötigte Frame-Lupen-Testabdeckung entfernt.

Nachweise lokal: .build/eof-tests (TRX), .build/eof-spotlight-tests (TRX), .build/eof-native (results.json und tatsächliche WPF-Aufnahmen für beide Modi). Die vorhandene 100-ns-Rundungsgrenze des klassischen Cutlist-Parsers wird nur im Dateirundlauf toleriert; Modell, Praxisfall und Schnittübergabe werden exakt geprüft. Keine Produktionstoleranz verändert.

## Start und Freigabegrenzen

Startbare lokale Entwicklungsdatei:

C:/Users/joerg/Documents/GitHub/cut-assistant-next/.build/worktrees/community-feedback/src/CutAssistantNext.App/bin/Release/net10.0-windows/CutAssistantNext.App.exe

Version **1.0.3 Build 55**; vorhandene libmpv-DLLs und unveränderter mitgelieferter Motor einschließlich Audioreparatur im bisherigen tools/otr-can-Ordner. Bestehende externe Werkzeugeinstellungen gelten weiter.

Keine automatische Installation, kein Setup/Release-Paket, kein Merge nach main, kein Push und kein GitHub-Release. Änderungen einschließlich der bestehenden Spotlight-Unterkomponente bleiben lokal zur persönlichen Prüfung. **Persönlicher Nachtrag 10.11.2026:** Spotlight und beide Schnittmotoren mit Yaiba S01E03 positiv gemeldet, einschließlich Bild/Ton/Schnitt/Drei-Sekunden-Prüfung. [Praxisbericht](YAIBA-BUILD55-PRAXISTEST-20261110.md). Der genaue EOF-Bedienablauf wurde dabei nicht einzeln beschrieben; eine gesonderte persönliche Prüfung dieses Ablaufs sowie vollständige neue Windows-10-/Windows-11-/VM-/Paketabnahmen bleiben offen. Die angegebene zweistündige Praxiszeit ist mit synthetischen Modelldaten automatisiert geprüft, nicht durch einen neuen zweistündigen Realfilmschnitt. Cutlist-Bewertung unverändert außerhalb dieses Nachtrags.
