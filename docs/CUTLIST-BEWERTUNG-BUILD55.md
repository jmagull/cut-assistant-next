# Cutlist-Bewertung – implementiert in Build 55

Die Bewertung wurde in Build 55 eingeführt und in Build 56 persönlich bestätigt. Der Anwender vergab fünf Sterne. Die serverseitig gespeicherte Bewertung blieb als „Mit 5 bewertet“ sichtbar und der Eintrag verschwand aus der Liste unbewerteter Downloads. Build 58 enthält denselben Bewertungsablauf.

Stand: 10.11.2026. Lokale Entwicklungsfassung **Cut Assistant Next 1.0.3 Build 55**, vorhandener Zweig codex/community-feedback-20261109 und Arbeitskopie .build/worktrees/community-feedback. Kein neues Projekt, keine Abhängigkeit, keine neue Buildnummer.

Der Folgeauftrag hebt die frühere Einschränkung zur HTTP-200-Auswertung ausdrücklich auf. Der [vorherige Untersuchungsbericht](CUTLIST-BEWERTUNG-UNTERSUCHUNG-BUILD55.md) bleibt als historische Analyse erhalten; die damalige Zurückstellung gilt nicht mehr. Die Funktion ist jetzt implementiert.

## Verhalten zur persönlichen Prüfung

1. Film öffnen, eine Server-Cutlist suchen und tatsächlich laden. Beim Anzeigen, Auswählen oder Herunterladen erscheint keine Frage zur neuen Cutlist.
2. Schnittpunkte prüfen oder korrigieren. Ein fertig geschnittener Film ist nicht erforderlich.
3. Beim Laden einer anderen Cutlist, beim Öffnen eines anderen Films oder beim Beenden bietet CAN **Bewerten**, **Überspringen** und **Abbrechen** an. Bereits bestehende Ersetzen-Rückfragen bleiben vorgeschaltet.
4. **Bewerten** öffnet den Dialog mit Originalbezeichnung, Autor, Serverhost, Server-ID und Film. Keine Note ist vorausgewählt. Die sechs Stufen 0–5 entsprechen dem Folgeauftrag. Erst **Absenden** startet die Übermittlung.
5. Bei Erfolg steht **„Bewertung an den Server übermittelt.“** im Dialog. **Weiter** oder Schließen setzt den begonnenen Vorgang fort.
6. **Überspringen** setzt ohne Übermittlung fort. **Abbrechen**, Schließen des Bewertungsdialogs ohne erfolgreichen Versand oder **Zur Schnittarbeit** nach einem Fehler brechen den Wechsel/das Beenden ab.
7. Bei einer laufenden Übermittlung sind Note und Senden gesperrt. Ein Schließversuch bricht die Anfrage ab; der Dialog wartet auf das Ergebnis, bevor Ressourcen freigegeben werden.

Abbruch erhält Schnittplan, offenen Schnittbeginn, bisherigen Herkunftskontext und Player. Die Anwendung setzt shutdown/isClosed und gibt Playerressourcen erst nach einer zustimmenden Entscheidung frei. Gleichzeitige Wechsel und wiederholtes Schließen während der Entscheidung sind gesperrt.

Die vorhandene Aufforderung, Schnittarbeit vor dem Verwerfen bei Bedarf zu speichern, bleibt erhalten. Sie wird nicht als automatische Speicherung ausgegeben. Es wurde keine zusätzliche allgemeine Speicherabfrage beim Beenden eingeführt.

## Aktueller Serverweg

Referenz ist der am selben Tag untersuchte öffentliche [Sniplist-Webclient](https://sniplist.mepaso.net/vendor.js), restService.setCutlistRating und dessen gemeinsamer HTTP-Helfer:

- POST an **/api/set-rating am Ursprung der ursprünglich verwendeten persönlichen Serveradresse**, beispielsweise https://sniplist.mepaso.net/api/set-rating beziehungsweise http://cutlist.at/api/set-rating.
- JSON enthält die numerische ursprüngliche **id** und die bewusst gewählte **rating** von 0 bis 5.
- **X-Authorization: Bearer <FRED>**, FRED aus dem einzigen 64-stelligen Pfadsegment der bereits konfigurierten Adresse. Kein zusätzlicher Benutzer oder Registrierungsschritt.
- CAN identifiziert sich als CutAssistantNext; kein Vortäuschen des alten Clients und kein rate.php-Fallback.
- HTTP **200** gilt wie ausdrücklich freigegeben als Annahme. Erkennbare JSON-Fehlerfelder, success/ok=false, Fehlerstatus und eindeutige einfache Fehlertexte werden trotzdem abgelehnt.
- Andere HTTP-Statuscodes bestätigen keinen Erfolg. Keine erfundene Zuordnung historischer error=1/error=2 zum heutigen Server. Fehlertexte nennen mögliche Ablehnungsgründe als Möglichkeiten, nicht als ausgedachte Servercodes.
- Transportfehler, Abbruch, Timeout und HTTP-5xx führen zu einem unbekannten Ausgang; andere Ablehnungen werden separat gespeichert. Timeout: 15 Sekunden.
- Keine Weiterleitung oder automatische Wiederholung: Der produktive HttpClient deaktiviert Redirects. FRED wird nicht an einen Umleitungsserver weitergegeben.

Serverantworten und HTTP-Ausnahmetexte werden nicht ungefiltert angezeigt oder protokolliert, weil sie die Kennung wiederholen könnten. Die Bewertungshistorie enthält ebenfalls keinen FRED und keine persönliche URL. Der bestehende Speicher der persönlichen Serveradresse bleibt die einzige gespeicherte Kennungsquelle.

**Keine echte Bewertung oder sonstige verändernde Serveranfrage für die Entwicklung ausgeführt.** Alle neuen HTTP-Tests verwenden einen simulierten HttpMessageHandler. HTTP 200 ist eine serverseitige Annahme gemäß Webclient, keine darüber hinaus unabhängig geprüfte Datenbankbestätigung.

## Herkunft, Eigentümer und Zustand

CutlistSelectionReader reicht eine Serverherkunft ausschließlich aus dem explizit gewählten und erfolgreich heruntergeladenen Servertreffer weiter. Lokale Dokumente erhalten keine Herkunft, auch wenn Dateiname oder INI-Metadaten zu einem Servertreffer passen.

Erst nach erfolgreichem LoadCutPlan wird die Herkunft im aktiven Bewertungsablauf als verwendet registriert. Sie enthält ursprüngliche persönliche Serveradresse und Account im Arbeitsspeicher, Original-ID, Serverautor, Originalbezeichnung und tatsächlich geladenen Film. Fehlgeschlagener oder abgelehnter Import aktiviert nichts. Eine reine Auswahl oder ein Download allein aktiviert nichts. Manuelle Schnittkorrekturen behalten den Ursprung. Die Aggregatbewertung beziehungsweise Autor-Selbsteinschätzung wird nicht als persönliche Fremdbewertung interpretiert.

Ausgeschlossen werden lokale Quellen, fehlende/ungültige IDs, nicht unterstützte oder mehrdeutige Serveradressen, nicht verwendete Downloads, leerer Serverautor und bereits gespeicherte Versandversuche. Eine erneute Übernahme derselben Server-/Account-/ID-Kombination löst keine Frage zum identischen Kandidaten aus.

Eigene Cutlists werden über den vorhandenen Standardautor erkannt; Vergleich ohne Beachtung der Groß-/Kleinschreibung und äußerer Leerzeichen gegen Serverautor sowie Dokumentautor. Ein abweichender oder fehlender eigener Autorenname beweist keine fremde Eigentümerschaft. Deshalb spricht die Oberfläche neutral von einer „verwendeten Server-Cutlist“. Eine serverseitige Ablehnung eigener oder bereits bewerteter Listen wird als Ablehnung angezeigt. Es wurde keine unbelegte Eigentümer-ID aus der alten UserID oder dem Dateinamen abgeleitet.

## Dauerhafter Duplikatschutz

Der bestehende CAN-Mechanismus „JSON im lokalen Settings-Ordner“ wird auch für **cutlist-ratings.json** verwendet. Kein weiteres Konfigurationssystem. Der Dateiname liegt unter LocalApplicationData / Cut Assistant Next / Settings.

Der Schlüssel ist SHA-256 aus ursprünglichem Serverursprung, FRED/Account und kanonisierter positiver ID. Verschiedene Server, Accounts und IDs haben getrennte Zustände. Die persönliche Kennung wird nicht im Klartext oder reversibel im neuen Speicher abgelegt; Anzeigenamen und Filmpfade werden dort ebenfalls nicht benötigt. Die bereits konfigurierten Serveradressen werden nicht nachträglich aufgrund vermeintlicher Aliasgleichheit zusammengelegt.

Vor jeder Netzwerkanfrage wird **Attempted** atomar und mit Flush gespeichert. Misslingt dies, wird nichts gesendet. Danach wird der Status **Accepted**, **Rejected** oder **Unknown** gespeichert. Ein Absturz oder ein Fehler beim abschließenden Speichern lässt den vorher gespeicherten Versuch bestehen. Sperre, erneutes Einlesen und atomarer Dateiaustausch verhindern gleichzeitige Reservierungen derselben Identität; temporäre Dateien und Handles werden freigegeben.

Jeder vorhandene Versandversuch verhindert neue automatische Angebote und Anfragen, auch nach Neustart. Es gibt in dieser Fassung **keinen manuellen Wiederholungsbutton**, damit ein unklarer Ausgang keine versehentliche Doppelbewertung erzeugt. Auch bekannte Ablehnungen werden nicht automatisch erneut angeboten. Bei einem beschädigten oder unlesbaren Zustandsspeicher kann der Anwender ausdrücklich ohne Bewertung fortfahren oder den Wechsel abbrechen; ein unsicherer Versand wird nicht gestartet.

## Geänderte Komponenten

Neue Dateien:

- Services/CutlistRatingSource.cs: geprüfte Serverherkunft und verwendeter Medienkontext.
- Services/CutlistServerClient.Rating.cs: aktueller POST, Annahme-/Fehlerbehandlung.
- Services/CutlistRatingService.cs: Versand, Duplikatsperre und Entscheidung vor Verlassen.
- Settings/CutlistRatingSettingsStore.cs: dauerhafte Bewertungshistorie.
- MainWindow.CutlistRating.cs: Dialoganbindung, Übergangssperre und verständlicher Speicherfehler.
- Dialogs/CutlistRatingQuestionDialog.xaml und .xaml.cs: drei Entscheidungen.
- Dialogs/CutlistRatingDialog.xaml und .xaml.cs: sechs Noten, expliziter Versand, Status und Abbruch.
- Tests: Services/CutlistRatingTests.cs, Services/CutlistRatingWorkflowTests.cs und CutlistRatingUiTests.cs.
- Dieser Bericht.

Vorhandene Dateien erweitert:

- Services/CutlistServerClient.cs: partial-Deklaration, vorhandenen HttpClient wiederverwenden; Suche/Download/Upload unverändert.
- Services/CutlistSelectionReader.cs: Herkunft zusammen mit dem importierten Dokument zurückgeben.
- MainWindow.CutlistSearch.cs: Übernahme abwarten und Herkunft weiterreichen.
- MainWindow.xaml.cs: Nachfrage vor Cutlist-Übernahme, Medienreset und Ressourcenfreigabe; Herkunft erst nach erfolgreichem Planladen setzen.
- MainWindow.MediaInput.cs: Übergangssperre; bestehende Ersetzen-Frage erhalten.
- README.md, NUTZERANLEITUNG.md und historischer Untersuchungsbericht: aktueller Status und Bedienung.

Alle Pfade unter Services/Settings/Dialogs beziehen sich auf src/CutAssistantNext.App; Testpfade auf tests/CutAssistantNext.App.Tests. Die bisherigen ungesicherten Änderungen wurden vor Beginn zusätzlich unter .build/20261110-before-rating-implementation gesichert.

## Automatisierte Prüfung

**Release-Build: 0 Warnungen, 0 Fehler; Version 1.0.3.55 / 1.0.3+build.55.**

- **2045 CAN-Tests bestanden**, keine Fehler oder übersprungenen Tests: 200 Core, 80 Cutlists, 392 Media, 1373 App. Darin **103 neue Bewertungsfälle** in drei Testklassen. Nachweise: .build/rating-implementation-full-tests/*.trx.
- **78 Spotlight-Tests bestanden**: 59 Core/Media und 19 WPF, keine Fehler oder übersprungenen Tests. Nachweise: .build/rating-implementation-spotlight-tests/*.trx.
- **56 native Prüfungen bestanden** mit echtem CAN-/Spotlight-Fenster, libmpv und vorhandenem synthetischem Film: Sekunden-Sprünge, Play/Pause, Einzelbilder, Keyframe-Kürzel/Tooltips in allen drei Ansichten, EOF, beide CAN-Schnittmodi, Laufzeiten, Übernahme/Abbruch und Ressourcenfreigabe. Nachweis: .build/rating-implementation-native/results.json.
- Neue C#-Dateien gezielt formatiert; Diff auf Whitespacefehler geprüft. Prüfsummenvergleich gegen die gesicherte Baseline bestätigt: bestehende Player-/Keyframe-/Sekunden-/Spotlight-/Schnittmotorquellen und otr-can.exe unverändert.

Befehle: dotnet restore CutAssistantNext.sln --runtime win-x64 --locked-mode; dotnet build CutAssistantNext.sln --configuration Release --no-restore -p:CanBuildNumber=55; dotnet test CutAssistantNext.sln --configuration Release --no-build. Beide bestehenden Spotlight-Testprojekte zusätzlich mit --configuration Release --no-restore. Vorhandenen nativen Prüfhelfer unter tools/prototypes/SpotlightIntegrationChecks gebaut und mit .build/community-test.mp4 ausgeführt. Der nummernsteigernde Buildhelfer wurde nicht aufgerufen.

Neue Tests decken unter anderem alle Noten, exakten Request mit simuliertem Server, HTTP-200-Annahme, erkennbare Fehler, Nicht-200-Status, Timeout/Abbruch, ungültige IDs/Adressen, Doppelklick, konkurrierende Dienste, Neustart, unklaren Ausgang und Speicherfehler ab. Weiterhin Herkunft nur nach Verwendung, eigene/lokale/unbenutzte Quellen, identische/neue ID, ursprünglicher Account nach Einstellungswechsel und alle drei Entscheidungen.

WPF-Prüfungen verwenden die tatsächlichen Dialoge und die drei tatsächlichen Einstiegspunkte für Cutlistwechsel, Filmwechsel und Beenden. Abbruch und Bewertungsfehler erhalten vollständige Bereiche und offenen Beginn; verweigerte bestehende Ersetzen-Fragen verhindern bereits die Bewertungsfrage. Gleichzeitiges Schließen/Öffnen ist blockiert. Erfolgreiches beziehungsweise übersprungenes Bewerten kann den tatsächlichen Shutdown abschließen. Testfenster-/Bewertungszustände sind in temporäre Ordner umgeleitet. Sichtprüfung des gerenderten Dialogs: sechs sichtbare Stufen, Identität ohne FRED, deaktiviertes Absenden ohne Auswahl.

Der erste Buildversuch scheiterte an der noch geöffneten bisherigen CAN-EXE. Nach dem vom Anwender bestätigten Schließen konnte neu gebaut werden. Eine Nullable-Warnung für optionalen Dokumentautor wurde korrigiert, nicht unterdrückt.

## Startbare Fassung und persönliche Prüfung

[Entwicklungs-EXE](../src/CutAssistantNext.App/bin/Release/net10.0-windows/CutAssistantNext.App.exe)

Vollständiger lokaler Pfad:

C:/Users/joerg/Documents/GitHub/cut-assistant-next/.build/worktrees/community-feedback/src/CutAssistantNext.App/bin/Release/net10.0-windows/CutAssistantNext.App.exe

**Persönlicher Praxistest am 10.11.2026 erfolgreich bestätigt.** Nach dem Screenshot des Bewertungsdialogs meldete der Anwender „hat alles geklappt“. Der gezeigte Fall betrifft Ghosts – Weihnachtsgeister – Teil 1, Server cutlist.at, Cutlist-ID 2081052; im Screenshot war Stufe 5 ausgewählt. Die anschließende Rückmeldung bestätigt den erfolgreichen Bewertungsablauf im persönlichen Einsatz. Einzelne Wechselwege und ein erneuter Programmstart wurden dabei nicht gesondert beschrieben. Diese Bestätigung ist keine Freigabe für Installation oder Veröffentlichung.

Weiterhin zu beachten: Eigener Standardautor sollte dem Servernamen entsprechen; andernfalls kann CAN eigene Listen nur eingeschränkt vorab erkennen. Ein Server mit Weiterleitung wird bewusst nicht automatisch verfolgt. Bei unklarem Ergebnis ist keine Wiederholung im Dialog vorgesehen.

Spotlight, Sekunden-Sprünge, Keyframe-Kürzel/Tooltips, Frame-/Keyframeberechnung, Timeline/Rot-Grün-Konvertierung, MP4Box, otr-can und AAC-Reparatur wurden in diesem Nachtrag nicht geändert. Keine neue Installation, kein Setup, Commit, Push, Merge oder Release.

**Bestätigung zur Build-56-Paketvorbereitung.** Der Anwender bestätigte die serverseitige Bewertung, ihre dauerhafte Anzeige und das Entfernen der Cutlist aus der Liste unbewerteter Downloads.
