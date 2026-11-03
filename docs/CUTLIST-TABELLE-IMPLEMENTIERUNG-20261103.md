# CAN 1.0.3 – gemeinsame Cutlist-Tabelle: Implementierung und Prüfung

**Abschließender Stand 03.11.2026:** CAN 1.0.3 / Zähler 48 einschließlich Buttonkorrektur und persönlicher Sichtbestätigung ist auf Nutzerauftrag eingefroren; Commit und Push sind beauftragt, Veröffentlichung erst nach gesonderter Freigabe. Die folgenden Prüf-/Git-Aussagen dokumentieren ihre jeweiligen früheren Arbeitsschritte. [Aktueller Freeze und Nachweise](FREEZE-1.0.3-20261103.md).

**Aktualisierte persönliche Abnahme 03.11.2026:** Santa Tell Me im grünen Modus mit vier Bereichen und **01:21:24.120** ist für Ton, Bild, Schnitt und Drei-Sekunden-Vorschau positiv gemeldet. Beide Motorprotokolle sind abgeschlossen; gespeicherte Cutlist und Servereintrag liegen vor. Die unten zum damaligen Abschluss noch offene grüne Filmprüfung ist damit ergänzt. Persönliche Wiederaufnahme begonnener Arbeit bleibt offen. [Detaillierter Praxisnachweis](SANTA-TELL-ME-GRUENER-PRAXISTEST-20261103.md).

Stand: **03.11.2026**, lokaler unveröffentlichter Entwicklungsstand **1.0.3**, unveränderter Buildzähler **48**. Umsetzung gemäß ausdrücklicher Entwicklungsfreigabe. Die veröffentlichte Version **1.0.2 Build 48** und ihre Pakete bleiben erhalten. Kein Commit, Push, Tag, Paket oder Release erzeugt.

Grundlagen: [lokale Suche](LOKALE-CUTLISTS-BESTANDSAUFNAHME-20261103.md), [UI und Wiederaufnahme](CUTLIST-TABELLE-UI-BESTANDSAUFNAHME-20261103.md). Die Einschränkungen dieser Analysen werden nicht durch eine neue Arbeitsstandsverwaltung oder Formatänderung umgangen.

## Implementiertes Verhalten

Beim erfolgreichen Öffnen eines Films startet CAN die konfigurierte Serversuche und die gezielte lokale Prüfung unabhängig. Ergebnisse erscheinen im **bestehenden einzelnen DataGrid**, gruppiert in **Server** und **Lokal**. Es gibt kein zweites Auswahlfenster, keine automatische Vorauswahl, Übernahme oder Zusammenführung.

Die Übersicht öffnet sich beim ersten Treffer oder erklärungsbedürftigen Quellenfehler; die andere Quelle darf danach weiterliefern. Auswahl und Details sind dabei bedienbar. „Cutlist laden“ wird erst freigegeben, wenn beide Quellen abgeschlossen sind und der Anwender eine ladbare Zeile ausgewählt hat. „Ohne Cutlist fortfahren“ und Fensterschließen bleiben jederzeit möglich und beenden diese Suchsitzung. Ein lokaler Fund unterdrückt keine Serveranfrage. Fehler einer Quelle verwerfen keine Ergebnisse der anderen.

Ohne Treffer und ohne Fehler öffnet die automatische Suche kein zusätzliches leeres Fenster. Eine bewusst über den bestehenden Server-Button gestartete Suche meldet in diesem Fall die beiden Quellenzustände. Derselbe Button öffnet jetzt die gemeinsame Übersicht; sein vorhandener Hauptfenstertext bleibt erhalten. Der separate manuelle Button „Lokale Cutlist laden“ bleibt verfügbar.

### Tabelle und Auswahl

- Vorhandene Server-Spalten, Bewertungen, Stimmen, Downloads, Namensvorschlag und Kommentar bleiben erhalten. Server-Treffer werden anfangs weiter nach numerischer echter ID absteigend sortiert; die ID bleibt verborgen.
- Benutzersortierung wirkt innerhalb der beiden Gruppen. Später eintreffende Ergebnisse setzen eine bewusste Auswahl nicht zurück. Gruppenüberschriften sind keine importierbaren Zeilen.
- Lokale Daten erhalten keine Server-ID. Community-Bewertungen, Stimmen und Downloads sind dort „–“; eine tatsächlich vorhandene Autorwertung bleibt sichtbar.
- Eine zusätzliche kompakte Spalte zeigt den Prüfstand. Der vorhandene Detailbereich enthält Name, Kommentar, vollständige lokale Fundpfade und Befundgründe. Die Pfade/Befunde sind außerdem als Zeilen-Tooltip erreichbar. Bei schmalem Fenster bleiben die rechten Spalten horizontal erreichbar.
- Beide Quellenzustände stehen kompakt unter der Tabelle; bei gekürzter Darstellung zeigt der Tooltip den vollständigen Text.
- Keine Anzeige „Fertig“ oder „Unfertig“. Für lokale Dokumente wird ausdrücklich kein hinterlegter Bearbeitungsstand behauptet. Die lokale Dauer ist die Summe der Behaltebereiche, kein Originaldauer- oder Fortschrittsnachweis.

## Gezielte lokale Suche

Die aus dem Speicherziel-Resolver ausgelagerte gemeinsame Namensfunktion lautet weiterhin:

```csharp
Path.GetFileName(originalFileName) + ".cutlist"
```

Alle Originalendungen bleiben erhalten. Die bisherige Prüfung ungültiger Zeichen und der Grenze von 255 Zeichen bleibt beim Speichern bestehen. Die Suche benutzt dieselbe reine Namensfunktion, ohne Ausgabeordner-/Schreibproben zu übernehmen.

Es werden höchstens zwei konkrete Pfade geöffnet: dieser Name im konfigurierten eigenen Cutlist-Verzeichnis und im Originalfilmordner. Verzeichnisnormalisierung einschließlich abschließender Trenner und Windows-Groß-/Kleinschreibung vermeidet doppelte Prüfung identischer Pfade. Leere/relative Konfigurationen fügen keinen Suchort hinzu. Kein Auflisten von Verzeichnissen, keine Wildcards, Unterordner, ähnlichen Namen, zusätzlichen Laufwerke oder zusätzliche Netzwerksuche.

Vollständig bytegleiche Dateien werden zu einer lokalen Zeile mit beiden Fundorten zusammengefasst. Andere Bytes ergeben getrennte Zeilen, auch bei nur unterschiedlichen Kommentaren oder unbekannten Feldern. Nicht auf Zeitstempel, Dateilänge, identische Schnitte oder Parse-/Serialize-Normalisierung vertrauen. Lesbare ReadOnly-Dateien funktionieren.

Die automatische Prüfung begrenzt jede Datei auf **4 MiB** und die deklarierte Bereichsanzahl auf **10000**; je Pfad gilt ein logisches **5-Sekunden-Zeitlimit**. Der zweite Pfad wird unabhängig geprüft. Reader/Parser werden wiederverwendet; das zusätzliche Bereichslimit ist optional und gilt nicht für den bestehenden manuellen Parseraufruf. UTF-8 einschließlich BOM, UTF-16-BOM und der vorhandene Windows-1252-Fallback sind gegen den bisherigen Dateireader geprüft.

**Prüfung ist getrennt von Bearbeitungsstand:** Technisch nicht verarbeitbare Inhalte werden gekennzeichnet und sind nicht normal ladbar. Bei anderem `ApplyToFile` oder klar abweichender FPS gibt es einen Zuordnungswiderspruch. Fehlende Metadaten und Größenabweichungen bleiben nachvollziehbare unklare Befunde; die vorhandene Größenrückfrage wird beim Import erhalten. Für den automatischen FPS-Vergleich gilt eine absolute Toleranz von **0,001 fps**, keine Umrechnung von Schnittwerten. Ein passender Name samt Metadaten ist kein Filmhash.

Noch enthaltene Werbung, ein großer letzter Keep-Bereich oder wenige Schnitte sind kein Ablehnungsgrund. Auch `NoOfCuts=0` wird nicht als Bearbeitungsstatus gewertet; eine leere geplante Ausgabe wird ausdrücklich erklärt.

## Gemeinsamer Import und Schutz vorhandener Daten

`CutlistSelectionReader` löst nur die Herkunft auf: Serverdaten über echte Serveradresse/ID herunterladen, lokale Dateien ausschließlich lesen. Für Serverdaten wird keine lokale Cutlist überschrieben und keine künstliche lokale Tempdatei benötigt. Bei lokalen Dateien werden die kompletten Bytes vor der Übernahme erneut mit dem Suchsnapshot verglichen. Eine inzwischen geänderte oder nicht mehr lesbare Fassung wird mit Hinweis auf erneute Suche zurückgewiesen; bei bytegleichen Kopien kann ein weiterhin identischer Fundort verwendet werden.

Alle drei Wege – Serverauswahl, lokale Tabellenzeile und manueller Dateidialog – führen anschließend in **`LoadCutlistDocument`**. Bestehende Dateigrößenwarnung, Endfragmententscheidung und Keep-/Remove-Umrechnung bleiben erhalten. Es gibt keinen zweiten Schnittimport und keine Verschärfung der manuellen Zuordnungsregeln.

Vor Planersatz wird vorhandene Schnittarbeit einschließlich offener Startmarke und geladenem Cutlist-Kontext mit einer ausdrücklich zu bestätigenden Ersetzungsfrage geschützt; **Nein ist Vorgabe**. Namensvorbereitung erfolgt vor dem endgültigen Planersatz. Autor, Namensvorschlag und vorgesehener Motor verwenden weiterhin den vorhandenen Übernahmeweg.

Filmkontext besteht aus Originalpfad, Analyseinstanz und Ladegeneration. Ergebnisse eines früheren Films bzw. einer beendeten Suchsitzung dürfen weder veröffentlicht noch angewandt werden. Abbruch wird an Server-/Dateizugriffe weitergegeben, verspätete Ergebnisse werden verworfen. Die bisherige Sperre gegen gleichzeitigen Dateiwechsel während der Suche bleibt zusätzlich bestehen.

Der Uploadkandidat entspricht der tatsächlich bestätigten Herkunft: lokaler Import setzt den verifizierten lokalen Pfad, Serverimport setzt **keinen** lokalen Pfad und entfernt eine frühere lokale Uploadzuordnung. Ein späteres Speichern setzt wie bisher erst nach erfolgreichem Schreiben einen aktuellen lokalen Kandidaten. Die bestehende Überschreibfrage beim Speichern bleibt unverändert; eine Serverauswahl erteilt dafür keine Zustimmung.

## Begonnene Schnittarbeit und offene Startmarke

Vollständige, bereits gesetzte Remove-Bereiche können weiterhin als gewöhnliche Cutlist gespeichert werden, auch wenn weitere Werbung vorhanden ist. Ein Regressionstest speichert drei solche Bereiche, findet die eigene Datei erneut, lädt sie über die neue lokale Auswahl und verändert anschließend das Ende eines vorhandenen Bereichs. Gespeicherte Dateibytes bleiben durch Laden/Bearbeiten unverändert; eine Speicherung ist weiterhin eine eigene Handlung.

**Neue Schutzprüfung:** Vor dem Erzeugen des zu speichernden Snapshots prüft `CutlistSaveGuard.CreateSnapshot` auf `PendingStart`. Bei offener Startmarke erscheint:

> **Schnitt noch nicht abgeschlossen**
>
> Es werden nur vollständige Schnitte gespeichert. Bitte den begonnenen Schnitt zuerst beenden oder die offene Schnittmarkierung entfernen.

Der Erstellungs-/Schreibweg wird dann nicht geöffnet bzw. ausgeführt. Geschlossene Bereiche und offene Marke bleiben erhalten. Nach Setzen eines vorläufigen Endes ist Speichern möglich; der Schnitt kann nach dem späteren Laden weiterbearbeitet werden. Es wird weder ein Ende erfunden noch verlangt, dass der gesamte Film fertig bearbeitet ist.

Keine neue Projektdatei, offene-Marken-Persistenz, Playerpositionsspeicherung, Werbeerkennung, Formatversion oder Fertigstellungskennzeichnung eingeführt.

## Geänderte Dateien und Methoden

| Datei | Änderung |
|---|---|
| [Version.props](../Version.props) | Lokale Entwicklungsversion 1.0.3; Buildzählermechanismus unverändert |
| [MainWindow.xaml.cs](../src/CutAssistantNext.App/MainWindow.xaml.cs) | Alten alleinigen Serverablauf ausgelagert; manuellen Reader mit gemeinsamem `LoadCutlistDocument` verbunden; Ersetzungsschutz, staged Namensvorbereitung, Uploadzuordnung; Save-Guard und Filmgeneration |
| [MainWindow.CutlistSearch.cs](../src/CutAssistantNext.App/MainWindow.CutlistSearch.cs) | Neue gemeinsame Suchsitzung, unabhängige Quellen, Auswahl/Download, Kontext- und Abbruchprüfung, `ConfirmCutlistReplacement` |
| [CutlistSearchResultsDialog.xaml](../src/CutAssistantNext.App/Dialogs/CutlistSearchResultsDialog.xaml) und [Code-behind](../src/CutAssistantNext.App/Dialogs/CutlistSearchResultsDialog.xaml.cs) | Dasselbe DataGrid gruppieren; neutrale Texte, Befund/Details, kompaktere Anordnung, keine Vorauswahl, gruppenstabile Sortierung |
| [CutlistSearchResultsViewModel.cs](../src/CutAssistantNext.App/ViewModels/CutlistSearchResultsViewModel.cs) | Beobachtbare gemeinsame Liste, Quellenstatus, unabhängiges Eintreffen, Ladeverfügbarkeit und Sitzungsinvalidierung |
| [LocalCutlistDiscoveryService.cs](../src/CutAssistantNext.App/Services/LocalCutlistDiscoveryService.cs) | Exakte zwei Pfade, begrenztes Lesen, Timeout-/Fehlerbehandlung, vollständiger Bytevergleich |
| [CutlistResultRow.cs](../src/CutAssistantNext.App/Services/CutlistResultRow.cs) | Neutrale Anzeige und getrennte Server-/lokale Herkunft, Metadaten und Zuordnungsbefund |
| [CutlistMediaContext.cs](../src/CutAssistantNext.App/Services/CutlistMediaContext.cs) | Originalpfad, Analyseinstanz und Ladegeneration validieren |
| [CutlistSelectionReader.cs](../src/CutAssistantNext.App/Services/CutlistSelectionReader.cs) | Bestätigte Herkunft auflösen und lokale Inhaltsidentität erneut prüfen |
| [CutlistSaveGuard.cs](../src/CutAssistantNext.App/Services/CutlistSaveGuard.cs) | Snapshoterzeugung bei offener Startmarke mit verständlichem Hinweis sperren |
| [CutlistFileName.cs](../src/CutAssistantNext.Cutlists/IO/CutlistFileName.cs) und [CutlistOutputTargetResolver.cs](../src/CutAssistantNext.App/Services/CutlistOutputTargetResolver.cs) | Vorhandene Namensbildung rein auslagern und gemeinsam nutzen; Speicherschutz erhalten |
| [CutlistFileReader.cs](../src/CutAssistantNext.Cutlists/IO/CutlistFileReader.cs) und [CutlistParser.cs](../src/CutAssistantNext.Cutlists/Serialization/CutlistParser.cs) | Geprüfte Bytes ohne Tempdatei einlesen; optionales Bereichslimit für automatische Kandidaten. Bestehender `Parse(string)`-/manueller Readerweg erhalten |

Neue Testdateien: `LocalCutlistDiscoveryTests.cs`, `CutlistResumeAndSelectionTests.cs`, `CutlistSearchSessionTests.cs`, `CutlistSearchDialogTests.cs` unter `tests/CutAssistantNext.App.Tests` und `IO/CutlistSnapshotReaderTests.cs` unter `tests/CutAssistantNext.Cutlists.Tests`. **Keine bestehenden Tests entfernt, abgeschwächt oder deaktiviert.**

## Build und Regressionstests

**Abschluss: Lösungsbuild erfolgreich, 0 Warnungen / 0 Fehler. 1675/1675 Tests bestanden, 0 fehlgeschlagen, 0 übersprungen.** Gegenüber 1634 bisherigen Tests sind 41 neue Fälle hinzugekommen.

| Testsuite | Bestanden |
|---|---:|
| Core | 200 |
| Cutlists | 80 |
| Media | 390 |
| App | 1005 |
| Gesamt | **1675** |

Alle Produkt-/Testausgaben liegen getrennt unter `.build/cutlist-table-20261103/out`. Restore erfolgte im Locked Mode mit separaten Kopien der committed Lockdateien; die vier vorbestehend geänderten Produktions-Locks wurden nicht überschrieben. Keine neue Produktionsabhängigkeit.

Der nummerierende `tools/build.ps1`-Aufruf wurde wegen der ausdrücklichen Vorgabe **keine Buildnummernerhöhung** durch einen vollständigen direkten Lösungsbuild mit festem `CanBuildNumber=48` ersetzt:

```text
dotnet restore CutAssistantNext.sln --artifacts-path .build/cutlist-table-20261103/out -r win-x64 --locked-mode --disable-parallel -p:CustomBeforeMicrosoftCommonTargets=<separate restore-locks.targets>
dotnet build CutAssistantNext.sln --configuration Release --no-restore --artifacts-path .build/cutlist-table-20261103/out -p:CanBuildNumber=48 --disable-build-servers -m:1
dotnet test CutAssistantNext.sln --configuration Release --no-build --no-restore --artifacts-path .build/cutlist-table-20261103/out --logger trx --results-directory .build/cutlist-table-20261103/test-results
```

Nur geänderte C#-Dateien mit `dotnet format whitespace --no-restore --include ...` formatiert. Abschließendes Build-Binärprotokoll `BUILD-final.binlog`; vier finale TRX-Dateien mit Zeitkennungen `17_39_13` / `17_39_14` im Ergebnisordner. Frühere fehlgeschlagene Läufe bleiben ebenfalls nachvollziehbar erhalten.

Vorbereitende Fehler wurden korrigiert: lösungsweiter Build mit RuntimeIdentifier vom SDK abgewiesen (RID verbleibt nur im Restore), eine lokale Variablenkollision und zwei Test-Analyzerhinweise; anschließend die im echten WPF-Test gefundene TwoWay-Gruppenkopfbindung und die fehlende Abfangbehandlung von `InvalidDataException`. Die Bildprüfung zeigte zusätzlich zu wenig Tabellenplatz in der Mindestgröße; Detailbereich und Hinweise wurden kompakter gemacht, die Sichtprüfung wurde verstärkt. Kein grüner Stand durch Testentfernung oder gelockerte Assertions erzeugt.

### Praktische WPF-Prüfung

Die **drei neuen WPF-Fälle** sind in den 1005 App-Tests enthalten. Es wurden echte kompilierte Fenster auf einem STA-Thread angezeigt und die modal ausgeführten Laden-/Schließen-Aktionen geprüft. Sichtbare Server-/Lokalgruppen, fehlende Vorauswahl, bewusste Auswahl, initiale Server-ID-Reihenfolge und Benutzer-Sortierung sind überprüft.

Die Darstellung wurde bei **1180 × 620** und unveränderter Mindestgröße **1000 × 540** gerendert und visuell kontrolliert. Drei Trefferzeilen beider Gruppen sind tatsächlich erzeugt und sichtbar, Tabellenhöhe mindestens 150; Name, Kommentar, Fundpfad und beide Aktionen erreichbar. Rechte Spalten sind bei Mindestbreite horizontal scrollbar. Bilder: `.build/cutlist-table-20261103/ui-proof/cutlists-1180.png` und `cutlists-1000.png`.

Weitere automatisierte Fälle prüfen zwei exakte Suchpfade, ReadOnly, gleichnamige bytegleiche/abweichende Dateien, fehlende Metadaten versus Widerspruch, beschädigte/zu große Kandidaten, gesperrte/langsame Quellen, unabhängige Antwortzeiten/Fehler, Abbruch und Filmkontext sowie geänderte lokale Dateiinhalte. Serverauswahl und verweigertes Überschreiben erhalten die vorhandene eigene Datei. Der gemeinsame Reader ist gegen UTF-8, BOM- und Legacy-Dateien geprüft. Die 390 unveränderten Media-Tests sowie vorhandenen Motor-/Kontrollplayer-Tests sind im Gesamtstand enthalten.

Dies ist eine technische WPF- und synthetische Regression, keine neue persönliche Film-/Setup-/Portable-Abnahme und kein neuer Schnitt mit echten Aufnahmen. Eine persönliche Prüfung mit eigener lokaler Cutlist und konfiguriertem Server ist der sinnvolle nächste Schritt.

## Bekannte, bewusst erhaltene Grenzen

- Sekunden-/Tick-Abweichungen und das Zusammenfallen direkt benachbarter Remove-Bereiche bleiben unverändert.
- Vorhandene Endfragment-Rückfrage bleibt Korrigieren oder Abbrechen; kein neuer unveränderter Endfragmentimport. Das ist kein Beschädigungs- oder Fertigstellungsnachweis.
- Kein vollständiger Sitzungs-/Metadaten-Roundtrip; keine persistierte offene Marke oder Playerposition.
- Automatische Dateigrenzen können außergewöhnlich große Cutlists ausschließen. Der vorhandene manuelle Ladeweg behält seine bisherigen Regeln.
- Ein logisches Zeitlimit bzw. Abbruch kann einen bereits in Windows blockierten synchronen Datei-Öffnungsaufruf nicht garantiert physisch beenden. Die UI wartet darauf nicht unbegrenzt, und verspätete Ergebnisse werden nicht angewandt. Netzwerkpfad-Aliase werden nicht durch Laufwerks-/Freigabesuche aufgelöst.
- Es gibt weiterhin keinen kryptographischen Film-Identitätsnachweis. Metadatenbefunde werden nicht als Fertigstellung oder absolute Aufnahmeidentität ausgegeben.

## Erhaltung und Git-Stand

Die vorbestehenden Änderungen an `AKTUELLER-STAND.md`, `RELEASE-1.0.2-BUILD48-RESULT.md`, `TAGEBUCH.md` und den vier Produktions-Lockdateien sind gegen ihre Ausgangs-SHA-256 unverändert. Buildzähler weiterhin **48**. Die beiden vorherigen Analyseberichte wurden in diesem Implementierungsschritt nicht geändert und bleiben als frühere Untersuchung erhalten.

Alle **sechs eingefrorenen Archive/Programme** unter `artifacts/release-1.0.2-build48` wurden erneut gegen ihre vorhandenen SHA256SUMS geprüft: Quellen-ZIP, Portable, Setup, otr-can-Quellen und beide libmpv-Quellenpakete stimmen überein. Keine Release-Datei neu erstellt. Nachweise `PRESERVED-BEFORE.json`, `PRESERVED-AFTER.json` und `RELEASE48-UNCHANGED.json` im Entwicklungsordner.

Git enthält die beauftragten lokalen Code-/Teständerungen, Version 1.0.3 und diesen Bericht zusätzlich zu den sieben vorbestehenden Nutzeränderungen und den beiden unversionierten Analyseberichten. Keine Dateien gestaged, kein Commit/Push/Tag/Release durchgeführt. `git diff --check` ohne Fehler; lediglich LF/CRLF-Hinweise zu Dateien mit vorhandener bzw. gezielt bearbeiteter Zeilenformatierung.

**Implementierung und technische Qualitätssicherung beendet. Hier anhalten; persönliche Abnahme oder Veröffentlichung nur auf weiteren Auftrag.**

## Nachtrag 03.11.2026 – Daueranzeige vereinheitlicht

Beim persönlichen Test mit NCIS Sydney zeigte die Serverzeile Sekunden (`2459.08`), die lokale Zeile dagegen einen TimeSpan mit langen Nachkommastellen (`00:40:59.0799999`). Auf Nutzerhinweis verwendet `CutlistResultRow.Duration` jetzt für beide Quellen **HH:MM:SS.mmm**, in diesem Fall **00:40:59.080**. Nur die Anzeige wird auf die nächste Millisekunde gerundet; gespeicherte Schnittwerte, importierte Dokumente und Server-Rohdaten bleiben unverändert. Stunden laufen über 24 hinaus weiter; unbekannte oder ungültige Angaben erscheinen als „–“.

Geändert: `CutlistResultRow.cs`, neuer Regressionstest `CutlistDurationDisplayTests.cs` und die tatsächliche Zelltextprüfung im vorhandenen `CutlistSearchDialogTests.cs`. Die bestehenden Tests bleiben erhalten. Der konkrete Zahlenfall einschließlich lokalem Tick-Rest, Sekundenübertrag, mehr als 24 Stunden, deutsche Oberflächenkultur und ungültige/fehlende Serverwerte sind geprüft. Die echten WPF-Zellen zeigen nun in allen drei Beispielzeilen übereinstimmend `00:01:20.000`; beide Fenstergrößen wurden erneut gerendert und kontrolliert.

**Aktueller Gesamtstand: Build 0 Warnungen / 0 Fehler; 1685/1685 Tests bestanden**, 0 fehlgeschlagen, 0 übersprungen: Core 200, Cutlists 80, Media 390, App 1015. Die zehn zusätzlichen Testfälle ergänzen den vorherigen Stand von 1675. Version 1.0.3 und Buildzähler 48 unverändert, keine Veröffentlichung.

Die aktualisierte Prüffassung und ihre Nachweise liegen separat unter `.build/cutlist-duration-20261103/` (Ausgaben unter `out/bin/CutAssistantNext.App/release`, `BUILD.binlog`, `test-results`, `ui-proof`). Die zuvor laufende Prüffassung wurde durch den Build nicht überschrieben. Nach dem vom Nutzer gemeldeten Schließen wurde die aktualisierte Fassung gestartet; Prozesspfad und Hauptfenstertitel „Cut Assistant Next – 1.0.3 · Build 48“ wurden bestätigt.

## Nachtrag 03.11.2026 – Downloadzähler eindeutig beschriftet

Der Nutzer erläutert die beiden Zahlen der Serverseite: **registrierte Downloads / alle Downloads**. Im gezeigten John-Wick-Fall steht dort `0 / 24`. Der bestehende Metadatenabruf `getxml.php` wurde lesend geprüft: Er liefert dazu ausschließlich `<downloadcount>0</downloadcount>`, keinen zweiten Downloadzähler. Die persönliche Serveradresse wurde nicht ausgegeben; es wurde keine Cutlist heruntergeladen.

Die bisherige einzelne Zahl wird deshalb als **„Downloads (reg.)“** beschriftet. Ein Tooltip erklärt, dass die Gesamtzahl aller Downloads von der verwendeten Serversuche nicht mitgeliefert wird. Der vorhandene XML-Parser, die Zählerwerte, Downloads und Sortierung bleiben unverändert; keine erfundene Gesamtzahl, kein Schätzen und kein zusätzliches Auslesen von Webseiten als Teil der Anwendung.

Geändert wurden nur der Spaltenkopf samt Tooltip in `CutlistSearchResultsDialog.xaml`, die präzisierende Feldbeschreibung in `CutlistSearchResult.cs` und dieser Nachtrag. **Build: 0 Warnungen / 0 Fehler; alle 1685 Tests bestanden**, keine übersprungen. Die reale WPF-Ansicht wurde mit der breiteren, vollständig lesbaren Beschriftung erneut bei beiden Fenstergrößen geprüft. Kein Versions-/Buildnummerwechsel und keine Veröffentlichung.

Aktualisierte Prüffassung und Nachweise: `.build/cutlist-download-label-20261103/`, Anwendung unter `out/bin/CutAssistantNext.App/release`. Der laufende persönliche Test wurde nicht durch einen Neustart unterbrochen.

## Persönliche Prüfrunde 03.11.2026 – Anzeige und Chicago Fire mit otr-can

**Anzeige persönlich bestätigt:** Der Nutzer bestätigt mit „Passt :-)“ und Screenshot für **NCIS Sydney S02E05 – Shucked** die gemeinsame Server-/Lokalübersicht, je einen Treffer ohne Vorauswahl, die übereinstimmende Dauer **00:40:59.080** und **„Downloads (reg.)“**. Die Screenshots zuvor zeigen außerdem die bewusste Auswahl beider Quellen samt jeweils passenden Details. Eine tatsächliche Übernahme wird aus diesen Auswahlbildern allein nicht abgeleitet.

**Neuer realer Schnittfall:** **Chicago Fire S13E09 – Eine Gefälligkeit**, Originaldatei `Chicago_Fire__Eine_Gefaelligkeit_S13E09_26.11.02_21-00_sf2_45_TVOON_DE.HD.mp4`. Zuordnung zur laufenden lokalen **CAN-1.0.3-Prüfrunde mit Buildzähler 48** aus der zuletzt geöffneten Prüffassung fortgeführt; das übermittelte Schnittprotokoll selbst enthält keine CAN-Version. Keine Setup-/Portable-Abnahme daraus abgeleitet.

Das Nutzerprotokoll belegt:

- **otr-can 0.1.6**, FFMS2-Indexierung erfolgreich; **210002** eindeutige, aufsteigende Quellbildzeiten und passende 25/50-fps-Zeitstruktur bestätigt.
- Ein Behalteintervall **562,640–2930,960 s**: CPU-Ränder mit 157 und 243 Bildern, dazwischen 118016 kopierte Bilder. **118416 Videopakete und rohe PTS/DTS** laut Motor verifiziert.
- **Tonspur 1:** AAC, 2 Kanäle, 48000 Hz; **Tonspur 2:** AC-3, 6 Kanäle, 48000 Hz. Je Spur **113679360 decodierte Samples** und kontinuierliche rohe PTS bestätigt. Die geplante Behaltedauer **2368,320 s = 00:39:28.320** stimmt mit diesen Samplezahlen überein.
- Finale MP4 einschließlich Video-/Audiopaketen, PTS/DTS, Padding und Decoderkonfigurationen geprüft; Arbeitsordnersperre freigegeben, abschließende CAN-Ausgabeprüfung beendet mit **„OTR-CAN-Schnitt abgeschlossen.“** Die allgemeine optionale Dekodierungszeile beweist keine zusätzlich angeforderte Vollvideodekodierung.

Der Nutzer bestätigt ausdrücklich: **„Schnitt Ton Bild, 3 Sekunden vorschau tonspur 1 und 2 check“**. Damit sind für diesen otr-can-Lauf **Schnitt, Bild, beide Tonspuren und Drei-Sekunden-Vorschau persönlich positiv geprüft**. Ein bestimmter lokaler oder serverseitiger Cutlist-Ursprung und die Wiederaufnahme einer unfertigen Cutlist sind für diesen Lauf nicht genannt.

Nur übermitteltes Protokoll und persönliche Rückmeldung dokumentiert; kein eigener weiterer Medienlauf, keine Codeänderung, kein Neubuild und keine erneute automatisierte Testsuite. Der zuletzt vollständig geprüfte Code-/Teststand bleibt **1685/1685 Tests, 0 Warnungen / 0 Fehler**, Version 1.0.3 und Buildzähler 48. Keine realen Mediendateien in Git aufgenommen, kein Commit/Push/Release.

## Nachtrag 03.11.2026 – nachfolgender Schnittmodi- und Dokumentationsstand

Die danach implementierten Rot-/Grün-Schnittmodi und die dynamische Timeline erhöhen den lokalen Gesamtstand auf **1.709/1.709 Tests** (200 Core, 80 Cutlists, 390 Media, 1.039 App), Release-Build ohne Warnungen/Fehler. Die oben genannten 1.685 Tests bleiben der damalige Abschluss der Tabellenrunde. Gemeinsame Tabelle, exakte Zwei-Pfad-Suche und Quellenschutz bleiben erhalten; zusätzliche Tests laden lokale und Server-Cutlists ausdrücklich auch in den grünen Editor.

Persönliche Screenshots zeigen die Auswahl der Arbeitsweise und für Chicago Fire rote beziehungsweise grüne Markierungen mit derselben geplanten Dauer **00:39:28.320**. Sie ergänzen die Anzeigeprüfung, nicht die oben getrennt dokumentierte persönliche Filmschnitt-/Tonabnahme. Vollständiger neuer grüner Filmschnitt und persönliche Wiederaufnahmeprüfung stehen aus. [Schnittmodi und genaue Nachweise](SCHNITTMODI-TIMELINE-20261103.md), [Entwicklungsnotizen 1.0.3](RELEASE-NOTES-1.0.3.md).

Auf anschließenden Nutzerauftrag werden README und die zentralen Dokumente aktualisiert. Die bei diesem früheren Implementierungsabschluss erhaltenen Status-/Tagebuchdateien dürfen damit nun dokumentarisch ergänzt werden; Code, Version 1.0.3, Buildzähler 48 und Veröffentlichungsstand bleiben unverändert.
