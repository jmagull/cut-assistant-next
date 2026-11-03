# Gemeinsame Cutlist-Tabelle und Wiederaufnahme eigener Schnittarbeit

**Einordnung nach Umsetzung am 03.11.2026:** Diese Bestandsaufnahme bewahrt die Untersuchung vor der Implementierungsfreigabe. Inzwischen sind die [gemeinsame Cutlist-Tabelle](CUTLIST-TABELLE-IMPLEMENTIERUNG-20261103.md) und die [Rot-/Grün-Bedienung mit Timeline-Vorschau](SCHNITTMODI-TIMELINE-20261103.md) lokal umgesetzt. Aktueller Entwicklungsabschluss: 1.709 Tests, Version 1.0.3 und Buildzähler 48; kein neues Paket oder Release. [Aktuelle Zusammenfassung](RELEASE-NOTES-1.0.3.md).

Stand: **03.11.2026**, CAN **1.0.2 Build 48**; Untersuchung für 1.0.3. Ergänzt die [Bestandsaufnahme der lokalen Suche](LOKALE-CUTLISTS-BESTANDSAUFNAHME-20261103.md), den Auftrag zur gemeinsamen Tabelle und beide Ergänzungen zur begonnenen Arbeit und freien Quellenwahl. **Keine Implementierung, UI- oder Formatänderung.**

## Ergebnis und verbindliche Entscheidungen

- Die vorhandene einzelne Tabelle kann mit einem gemeinsamen Anzeigemodell in **Server** und **Lokal** gruppiert werden. Eine zweite Tabelle oder Neugestaltung ist nach dem untersuchten Aufbau nicht erforderlich.
- Server- und lokale Suche liefern unabhängig. Ein lokaler Fund unterdrückt niemals die konfigurierte Serversuche. Beide Quellen bleiben gemeinsam sichtbar; keine Quelle wird automatisch ausgewählt, bevorzugt oder übernommen.
- Eine technisch gültige begonnene Cutlist kann wieder geladen und weiterbearbeitet werden. Verbliebene Werbung macht sie weder beschädigt noch unpassend.
- Das heutige Format enthält **keinen verlässlichen Fertig-/Unfertig-Status**. Die Anzeige muss diesen Stand als unbekannt behandeln, statt ihn aus Schnitten, Bewertung oder Kommentar zu erraten.
- Der typische Durchlauf mit getrennten fertigen Entfernungsbereichen funktioniert. Eine **uneingeschränkt verlustfreie Wiederaufnahme des gesamten Arbeitszustands ist heute aber nicht nachgewiesen und in mehreren Fällen widerlegt**: offene Startmarke fehlt, benachbarte Bereiche verlieren ihre innere Trennmarke, und es gibt reproduzierbare Tick-Abweichungen beim Einlesen von Sekundenwerten. Diese Befunde werden nicht durch Ausschluss gültiger begonnener Cutlists gelöst.

Die frühere Idee, nach lokaler Übernahme die automatische Serversuche auszulassen, ist ausdrücklich verworfen. Die folgenden Vorschläge sind zur späteren Freigabe bestimmt; eine eigenständige Arbeitsstandsverwaltung ist nicht beauftragt.

## A. Tatsächlicher Aufbau der vorhandenen Übersicht

| Bereich | Befund im aktuellen Code |
|---|---|
| Serverdaten | [CutlistSearchResult.cs](../src/CutAssistantNext.App/Services/CutlistSearchResult.cs), Zeilen 3–76: unveränderliche Initialwerte, überwiegend Strings; `Id`, Dateiname, Bewertung/Stimmen/Autorwertung, Autor, Name, Kommentar, tatsächlicher Inhalt, Anzahl, Dauer, Downloads, Fehler. `DisplayFormat` wird aus dem Dateinamen abgeleitet, nicht aus einer Medienprüfung. |
| Suche | [CutlistServerClient.cs](../src/CutAssistantNext.App/Services/CutlistServerClient.cs), Zeilen 23–45 und 258–399: Anfrage mit Originaldateinamen an `getxml.php?name=...`, XML lesen, `.cutlist` gegebenenfalls anhängen, Ergebnisliste bilden. Abbruchparameter existiert bereits. |
| Anfangssortierung | `SearchAsync`, Zeilen 390–398: numerische Server-ID absteigend, nicht Erstellungsdatum. Nichtnumerische IDs erhalten für diese Sortierung `long.MinValue`. Das erklärt den UI-Text „Neuere Serverfassungen zuerst“; ein explizites Datum wird nicht verglichen. |
| XAML | [CutlistSearchResultsDialog.xaml](../src/CutAssistantNext.App/Dialogs/CutlistSearchResultsDialog.xaml), Zeilen 51–101: ein schreibgeschütztes `DataGrid`, `AutoGenerateColumns=False`, `ItemsSource={Binding Results}`, `SelectionMode=Single`, ganze Zeile. Spalten sind umordnungs-/größenveränderbar. Keine vorhandene Gruppierung. |
| Spalten | Cutlist, Format, Autor, Bewertung, Stimmen, Autorwertung, Schnitte, Dauer, Downloads. Werte binden direkt an die Server-Ergebnisobjekte. Es gibt keine eigene Sortierbehandlung im Dialog; Spaltenbedienung ist die vorhandene DataGrid-Bedienung. |
| Details | Zeilen 119–148: vorgeschlagener Name und Kommentar binden per `ElementName=ResultsGrid` an `SelectedItem`. `ActualContent` und `Errors` sind im Datenmodell vorhanden, im jetzigen Detailbereich aber nicht dargestellt. |
| Datenbindung / Auswahl | [CutlistSearchResultsDialog.xaml.cs](../src/CutAssistantNext.App/Dialogs/CutlistSearchResultsDialog.xaml.cs), Zeilen 9–43: `DataContext=this`, `IReadOnlyList<CutlistSearchResult>`; **kein eigenes Ergebnis-ViewModel und keine beobachtbare Trefferliste**. Bei Treffern wird bisher `SelectedIndex=0` gesetzt. `SelectedResult` ist ein direkter Cast der ausgewählten Zeile. |
| Aktionen | Zeilen 45–71: „Cutlist laden“ verlangt eine Auswahl und schließt mit `DialogResult=true`; „Ohne Cutlist fortfahren“ schließt mit `false`. Der Dialog selbst lädt keine Cutlist und bearbeitet keine Dateien. |
| Fenster | Titel und Überschrift sind serverbezogen; 1180 × 620, Mindestgröße 1000 × 540. Tabelle und bestehender Detailbereich sind bereits getrennt. Keine zusätzlichen lokalen Dialoge nötig. |

### Suche und Import werden heute außerhalb der Tabelle gesteuert

[MainWindow.xaml.cs](../src/CutAssistantNext.App/MainWindow.xaml.cs), Zeilen 1622–1831: `_cutlistSearchInProgress` sperrt Doppelstarts und den Server-Button. Vor der Suche werden Analyse, positive Dauer und persönliche Serverkonfiguration geprüft. Ein eigener `HttpClient` hat 15 Sekunden Timeout. Der aktuelle Aufrufer reicht keinen Filmkontext-Abbruch an `SearchAsync` weiter.

Erst **nach Abschluss** der Serversuche öffnet `CutlistSearchResultsDialog` modal über `ShowDialog`. Bei null Treffern oder Serverfehler folgen heute eigene Meldungen. Nach Auswahl lädt `DownloadBytesAsync` die Serverdatei anhand ihrer echten ID, schreibt sie in eine GUID-Tempdatei, ruft `LoadCutlistFromFile(..., rememberAsUploadCandidate: false)` auf und löscht die Tempdatei im `finally`.

Der manuelle lokale Button nutzt `LoadCutlistFromFile(..., true)`. Der Import ab Zeile 1892 liest synchron, prüft Größenabweichung, fragt gegebenenfalls wegen Endfragmenten, erzeugt den Schnittplan und übernimmt ihn. Danach werden gegebenenfalls Namensvorschlag, Autor und vorgesehener Motor aktualisiert. Das ist der gemeinsame wiederzuverwendende fachliche Weg, nicht der Server-Download.

**Wichtig für die neue Übersicht:** Das bisherige „bei Serverfehler/keinen Servertreffern abbrechen und Dialog nicht öffnen“ kann nicht unverändert um die Gesamtsuche gelegt werden. Nur der serverseitige Abruf einschließlich Anfrage und Anfangssortierung bleibt fachlich erhalten; seine bisherige alleinige Dialogsteuerung muss zur gemeinsamen Quellensteuerung werden.

## B. Drei fachliche Zustände, drei getrennte Prüfungen

| Fachlicher Zustand | Bedeutung | Was CAN heute zuverlässig weiß |
|---|---|---|
| Fertig und gültig | Schnittarbeit ist abgeschlossen, Daten sind verarbeitbar | Technische Verarbeitbarkeit kann geprüft werden. Die Fertigstellung ist im gewöhnlichen Format nicht bewiesen. |
| Angefangen und gültig | Ein Teil der gewünschten Schnittarbeit ist gespeichert, weitere Arbeit steht aus | Dieselbe technische Prüfung; die vorhandenen gültigen Bereiche dürfen geladen werden. Noch vorhandene Werbung ist kein Fehlerkriterium. |
| Beschädigt / nicht verarbeitbar | Inhalt ist strukturell ungültig oder nicht sinnvoll in den Schnittplan überführbar | Reader, Parser und Bereichsprüfungen liefern konkrete Fehler. Bei reinem Zugriffsfehler besser „Nicht lesbar: …“ anzeigen, statt ohne Kenntnis des Inhalts eine Beschädigung zu behaupten. |

Für das gemeinsame Modell sind **Dokumentgültigkeit**, **Filmzuordnung** und **Bearbeitungsstand** unabhängig:

- Ein gültiges Dokument kann für einen anderen Film bestimmt sein: Zuordnung widersprüchlich, nicht dadurch beschädigt.
- Ein lesbares gültiges Dokument mit fehlender Größen-/FPS-Angabe kann unklar zugeordnet sein; das beweist keine unfertige Arbeit.
- Ein begonnenes Dokument mit eindeutigem Originalbezug kann plausibel passen, auch wenn sein letzter Behaltebereich bis zum Filmende reicht oder Werbung enthält.
- Eine vorhandene Datei kann aktuell nicht lesbar sein, ohne dass ihr Inhalt als beschädigt nachgewiesen ist.

**Keine Fertigstellungsheuristik:** Weder Anzahl/Position der Schnitte, Keep-Gesamtdauer, Autorwertung, `MissingBeginning`, `MissingEnding`, `MissingVideo`, `MissingAudio`, `OtherError`, Autor noch Namensvorschlag sind ein verlässlicher Abschlussindikator. Diese Metadaten beschreiben Inhalte, Fehler oder Urheberschaft. Freitext „fertig“ oder „unfertig“ ist kein fest definierter Status. Im aktuellen Zielumfang daher keine Statusplakette „Unfertig“ oder „Fertig“; bei Bedarf „Bearbeitungsstand nicht hinterlegt“ im Detailbereich.

## C. Was eine begonnene eigene Cutlist bereits bewahrt

### Vorhandener Speicher-/Ladeweg

`GenerateCutlistButton_Click`, [MainWindow.xaml.cs](../src/CutAssistantNext.App/MainWindow.xaml.cs), Zeilen 2044–2188, verlangt eine Medienanalyse und erstellt bei Zeile 2103 einen `CreateCutPlanSnapshot`. Der Snapshot kopiert ausschließlich abgeschlossene `RemoveSegment`-Paare und die Mediendauer. Der Dialog speichert diesen Snapshot. Die vorhandene Bewertungsauswahl ist eine Speicherbedingung; ein Fertigstellungskennzeichen oder eine Prüfung „alle Werbung entfernt“ gibt es nicht. Eine Bewertung ist auch bei begonnener Arbeit kein Abschlussnachweis.

`CutlistGenerationViewModel.CreateDocument` bildet mit [CutlistKeepSegmentBuilder.cs](../src/CutAssistantNext.Cutlists/Editing/CutlistKeepSegmentBuilder.cs) das Komplement über `[0, Mediendauer]`. Nicht als Entfernen markierte Teile bleiben erhalten, unabhängig von ihrem Inhalt. `CutlistFileWriter.WriteSafely` speichert das gewöhnliche Cutlist-Dokument. Beim Laden bilden `CutlistCutPlanBuilder` und [CutlistRemoveSegmentBuilder.cs](../src/CutAssistantNext.Cutlists/Editing/CutlistRemoveSegmentBuilder.cs) das Komplement zurück.

Beispiel mit 100 Sekunden Film:

```text
Bisher Entfernen: [10,20] und [40,50]
Gespeichert Behalten: [0,10], [20,40], [50,100]
Wieder geladen Entfernen: [10,20] und [40,50]
```

Die restlichen Werbeblöcke werden nicht gesucht oder ergänzt. Nach dem Laden kann der Anwender etwa `[60,70]` hinzufügen. Dieser konkrete synthetische Fall wurde mit den vorhandenen Build-48-Bibliotheken einschließlich Writer/Reader und `CutPlanViewModel` erfolgreich geprüft.

### Erhaltene und nicht erhaltene Informationen

| Information | Heutiger Befund |
|---|---|
| Bereits abgeschlossene, getrennte Entfernungsbereiche | Als Keep-Komplement gespeichert und grundsätzlich wieder bearbeitbar. Voraussetzungen: derselbe Medienzeitraum, gültige Daten, keine bestätigte fachliche Korrektur. Präzisionsgrenzen siehe unten. |
| Noch nicht bearbeitete Filmteile / Werbung | Bleiben einfach im Keep-Bereich. Der Code muss ihre Bedeutung nicht kennen. |
| Offene Startmarke ohne Ende (`PendingStart`) | Nur im ViewModel, **nicht im `CutPlan`-Snapshot und nicht in der Cutlist**. Speichern lässt die Marke in der laufenden Sitzung zunächst bestehen; nach Neustart/Import ist sie nicht wiederherstellbar. Keine automatische Ergänzung bis Filmende. |
| Auswahl eines Bereichs / einer Kante | Nicht gespeichert. `LoadCutPlan` löscht Auswahl und offene Marke ausdrücklich. |
| Direkte Nachbarschaft zweier Remove-Bereiche | Die inneren Trennmarken gehen im Komplement verloren. `[10,20]` plus `[20,30]` wird nach Speichern/Laden zu `[10,30]`. Derselbe Filmteil wird entfernt, aber zwei separat bearbeitbare Tabellenzeilen und ihre gemeinsame Kante bleiben nicht erhalten. |
| Fertigstellung, Prüf-/Suchfortschritt, aktuelle Playerposition | Kein definierter Cutlist-Zustand dafür vorhanden. Keine Ableitung aus der Schnittgeometrie. |
| Originalgesamtdauer | Im aktuellen `CutlistGeneralMetadata` nicht separat gespeichert; Wiederaufbau verwendet die aktuell analysierte Dauer. |
| Autor, Namensvorschlag, vorgesehener Motor | Im Dokument vorhanden; der Lader merkt Autor/Motor und übernimmt einen nichtleeren Namensvorschlag. Der tatsächliche Autor neuer eigener Dateien folgt weiter der Erstellungslogik und den Einstellungen. |
| Kommentar, Bewertung, Fehlerflags und weitere Metadaten | Reader/Parser lesen sie für das Dokument. Der aktuelle Hauptfenster-Ladeweg bewahrt sie jedoch nicht vollständig als editierbaren Dialogzustand für späteres Neuspeichern; der Erstellungsdialog entsteht erneut aus Einstellungen, Namenszustand und Analyse. Kein vollständiger Projekt-Roundtrip aller Metadaten zugesagt. |

**Randfälle:** Ein Plan ohne Entfernungsbereiche speichert den ganzen Film als einen Keep-Bereich (`NoOfCuts=1`). Ein Plan, der alles entfernt, hat dagegen null Keep-Bereiche (`NoOfCuts=0`) und ist nach heutigem Modell technisch zulässig. Beides beweist weder Fertigstellung noch Beschädigung. Eine leere geplante Ausgabe transparent kennzeichnen; nicht mit einer fehlenden/beschädigten Datei oder „unfertig“ gleichsetzen.

### Reproduzierte Präzisions- und Wiederherstellungsgrenzen

1. **Sekunden → Ticks:** [CutlistSerializer.cs](../src/CutAssistantNext.Cutlists/Serialization/CutlistSerializer.cs), Zeilen 196–209, serialisiert `TimeSpan.TotalSeconds` mit bis zu zehn Nachkommastellen. [CutlistParser.cs](../src/CutAssistantNext.Cutlists/Serialization/CutlistParser.cs), Zeilen 51–80, liest über `double` und `TimeSpan.FromSeconds`. In einem synthetischen Zwei-Stunden-Film mit einem exakten 25-fps-Remove-Bereich `[398,84; 399,28]` ergeben sich nach Serialisierung/Parsing `[398,8399999; 399,2799999]` sowie ein zusätzlicher End-Remove-Bereich `[7199,9999999; 7200]`. Abweichung jeweils **ein Tick = 100 ns** in diesem Beispiel. Das ist kein Nachweis eines verlorenen Videoframes; die geforderte unveränderte Wiederherstellung der Werte und Zeilen ist damit dennoch nicht allgemein erfüllt.
2. **Geänderte Mediendauer:** Aus demselben gespeicherten 100-s-Plan wird beim Wiederaufbau mit 101 s ein zusätzlicher Remove-Bereich `[100,101]`. Bei kürzerer aktueller Dauer können gespeicherte Keep-Bereiche über das Ende reichen und die vorhandene Ablehnung auslösen. Daher Originalkontext und Dauer sorgfältig absichern; keine Dauer aus dem letzten Keep-Ende als allgemeines Originalmetadatum erfinden.
3. **Endfragmentbehandlung:** Eine eigene gültige angefangene Cutlist kann denselben winzigen letzten Keep-Bereich haben wie eine fertige. Der vorhandene Detector erkennt Geometrie, keinen Bearbeitungsstatus. Im synthetischen 100-s-Fall mit Remove `[50;99,96]` wird der letzte Keep-Bereich von 0,04 s erkannt. Der heutige Lader bietet nur Korrektur oder Abbruch: „Nein“ beendet das Laden, „Ja“ verlängert den letzten Remove-Bereich bis 100 s. Ein solcher Arbeitsstand lässt sich über diesen Zweig derzeit **nicht unverändert** wiederaufnehmen. Eine neue Kennzeichnung als beschädigt wäre sachlich falsch.

**Folgerung für die Umsetzung:** Normale begonnene Cutlists brauchen keine Formatänderung, um angeboten zu werden. Vor einer Zusage „alle gesetzten Schnittpunkte unverändert wiederherstellbar“ sind Zeitpräzision, Nachbarschaft und der Import von Endfragmentfällen gesondert zu entscheiden. Denkbare minimale Korrekturen wären exakte bzw. nachvollziehbar gerundete Sekundenkonvertierung und eine ausdrücklich wählbare unveränderte Übernahme bei Endfragmenten. Das wären Änderungen am bestehenden Import, die **eigene Prüfung/Freigabe** benötigen; in diesem Auftrag werden sie nicht vorgenommen. Keine stillen Snap-/Merge-/Endergänzungsregeln hinzufügen.

Für eine vollständige Sitzung einschließlich offener Startmarke, ursprünglicher Remove-Aufteilung, genauer Dauer/Identität und explizitem Abschlussstatus wären zusätzliche definierte Persistenzdaten erforderlich. Das gewöhnliche Keep-Format kann diese Informationen nicht allein zurückgewinnen. Ein Nebenformat oder eine Arbeitsstandsverwaltung wäre ein separater Auftrag; weder ein Statusfeld im Kommentar noch eine neue Projektdatei wird hier eingeführt. Ob Originalmetadaten bei erneutem Speichern vollständig übernommen werden sollen, wäre ebenfalls separat festzulegen.

## D. Gemeinsames Ergebnismodell ohne vorgetäuschte Serverdaten

Vorschlag: `CutlistSearchResult` als Server-Transportobjekt erhalten und nur für die Anzeige in ein neutrales `CutlistResultRow` abbilden. Das Modell ist ein Konzept, noch kein neuer Produktcode.

| Baustein | Inhalt / Zweck |
|---|---|
| Quellenart | `Server` oder `Lokal`, explizit typisiert; feste Gruppenreihenfolge ist nur Darstellung, keine Vorauswahl |
| Serverherkunft | Echtes `CutlistSearchResult` und echte ID, dazu festgehaltener Suchkontext für den passenden Server; nur dieser Fall erlaubt Download |
| Lokale Herkunft | Einer oder beide normalisierten Fundpfade und ein konsistenter geprüfter Leseinhalt; keine Server-ID, kein Download und keine unnötige Tempkopie |
| Gemeinsame Anzeige | Dateiname, Format-Hinweis, Autor, Namensvorschlag, Kommentar, Keep-Anzahl, Dauer und vorhandene Bewertungen |
| Prüfung | Dokumentgültigkeit, Zuordnungsbefund samt Gründen, Prüfstand „noch nicht geprüft“, Auswahl-/Importierbarkeit getrennt |
| Bearbeitungsstand | Ohne verlässliche persistierte Angabe unbekannt; keine berechnete „Unfertig“-Eigenschaft |
| Herkunftsdetails | „Eigene Cutlists“, „Filmverzeichnis“ oder beide; vollständige Pfade im Tooltip und kopierbaren Detailfeld |

Server-Stimmen, Community-Bewertung und Downloads haben bei lokalen Dateien keinen entsprechenden Wert: **„–“ bzw. nicht verfügbar**, keine erfundenen Nullen. `RatingByAuthor` kann lokal tatsächlich vorhanden sein und wird als Autorwertung angezeigt. Der lokale Autor ist nicht automatisch der aktuelle Benutzer; auch eine fremde Cutlist kann dort liegen. „Lokal“ bezeichnet den Fundort.

Die lokale Anzahl ist `document.Cuts.Count`, also die Zahl der Keep-Bereiche, nicht die Zahl noch zu bearbeitender Werbeblöcke. Eine daraus berechnete lokale Dauer ist die Summe der Keep-Dauern, nicht der Originalfilm oder ein Fertigstellungsgrad. Einheiten transparent halten; Server-Dauer zunächst entsprechend ihrer vorhandenen Darstellung erhalten. Ohne Download fehlen bei Serverzeilen teilweise Dokumentmerkmale; „noch nicht am Dokument geprüft“ statt „sicher passend“. Kein vorsorglicher Download aller Servertreffer nur zur Vereinheitlichung.

Bytegleiche lokale Dateien werden als ein lokaler Eintrag mit beiden Fundorten dargestellt. Abweichende Bytes bleiben zwei Einträge, auch bei gleichen Schnitten, Autoren oder gleich langer Datei. Keine Zusammenfassung einer lokalen und einer Server-Cutlist anhand von Name/Autor/ID. Dass Server-Metadaten ähnlich aussehen, beweist keine Inhaltsidentität.

## E. Konkrete Gruppierung und Bedienung

**Bevorzugt: dasselbe einzelne DataGrid**, neue beobachtbare Ergebnissammlung in einem kleinen `CutlistSearchResultsViewModel`, gruppierte Collection-View nach Quellenart, klare Gruppenköpfe **„Server (n)“** und **„Lokal (n)“**. Der vorhandene Aufbau steht dem nicht entgegen. Zwei separate Tabellen würden eine gemeinsame Auswahl und Tastaturführung erschweren und sind hier nicht nötig.

- Titel/Überschrift neutral zu „Cutlists für diesen Film“ ändern; bisherige Spalten und Detailfelder erhalten.
- Eine kompakte Spalte „Befund“ für Zuordnung/Prüfstand ergänzen. Fundort und Gründe bevorzugt im bestehenden Detailbereich bzw. Tooltip, nicht als breite permanente Pfadspalte. Keine zusätzliche „Unfertig“-Spalte.
- Community-Werte für lokale Zeilen als nicht verfügbar darstellen. Gruppenkopfzeilen sind nicht ladbare Datensätze; es wird immer höchstens eine echte Zeile ausgewählt.
- **Kein `SelectedIndex=0` und kein automatisches Umschalten der Auswahl**, wenn eine zweite Quelle später Ergebnisse liefert. „Cutlist laden“ nur bei bewusst gewählter verarbeitbarer Zeile; Fehlerzeilen bleiben zur Diagnose einsehbar, aber nicht normal ladbar.
- Initiale Server-Reihenfolge numerisch nach echter ID absteigend erhalten. Bestehende Benutzersortierung muss innerhalb beider Quellgruppen wirken, ohne die Gruppen zu vermischen oder bei neuen Ergebnissen die Auswahl zurückzusetzen. Dazu View-/Sortierlogik ausdrücklich testen; kein globaler künstlicher ID-Sortierschlüssel für lokale Dateien.
- Für leere bzw. noch suchende Quellen deren Zustand kompakt oberhalb oder unterhalb der Tabelle anzeigen: „Server: sucht / n Treffer / nicht eingerichtet / Fehler“ und „Lokal: prüft / n Treffer / kein Treffer / eingeschränkt“. Eine leere Collection-View-Gruppe existiert nicht automatisch; daher keine erfundenen leeren Trefferzeilen nur als Gruppenersatz.
- Details hängen weiter an der ausgewählten Zeile. Ergänzt werden Fundort, Befundgründe und gegebenenfalls „Bearbeitungsstand nicht hinterlegt“. Nicht verfügbare Werte nicht mit gültiger Leer-/Nullinformation verwechseln.
- Die vorhandenen beiden Aktionen reichen: **„Cutlist laden“** und **„Ohne Cutlist fortfahren“**. Fenster schließen entspricht Nichtübernahme. Keine vorgeschaltete lokale Ja/Nein-Box und danach separate Serverauswahl.

Vorschlag zur Entscheidung während laufender Quellen: Tabelle, Details und Abbruch sofort bedienbar halten, Ergebnisse unabhängig nachreichen. „Cutlist laden“ erst nach terminalem Zustand beider Quellen freigeben (fertig, nicht eingerichtet, Fehler oder begründetes Zeitlimit), damit der erste lokale Fund nicht faktisch die späteren Serverangebote verdrängt. „Ohne Cutlist fortfahren“ bleibt jederzeit möglich und beendet diese Suchsitzung. Das ist eine vorgeschlagene Bedienentscheidung; dafür brauchen beide Quellen begrenzte Wartezeiten und klare Statusangaben. Ein UI-Zeitlimit beendet einen blockierten synchronen Dateizugriff nicht automatisch physisch; verspätete Ergebnisse müssen verworfen werden.

Wenn beide Quellen vollständig ohne Treffer bleiben, kein zusätzliches leeres Auswahlfenster erzwingen. Bei mindestens einem Treffer oder einem erklärungsbedürftigen Befund genügt die gemeinsame Übersicht; keine zusätzlichen Fehlermeldungsserien je Quelle. Ein bereits sichtbares Suchfenster kann nach Abschluss leer bleiben und den bestehenden Weiter-Button anbieten. Der bisherige Server-Button kann dieselbe Übersicht erneut öffnen bzw. aktualisieren; das manuelle **„Cutlist laden“** mit Dateidialog bleibt separat erhalten.

## F. Suchsteuerung und einheitlicher bestätigter Import

Integrationspunkt bleibt der gemeinsame Film-Ladekern nach erfolgreicher Analyse/Player-Ladung und Einrichtung der Navigation, derzeit [MainWindow.xaml.cs](../src/CutAssistantNext.App/MainWindow.xaml.cs), Zeilen 2502–2524. Startparameter, Dateiauswahl und Drag & Drop kommen dort zusammen. Den heutigen alleinigen Serveraufruf durch eine gemeinsame Suchsitzung ersetzen, nicht um zwei hintereinander geschaltete Auswahlfenster ergänzen.

1. Unveränderlichen Filmkontext festhalten: normalisierter Originalpfad, Originalname, Analysewerte, Sitzung/Generation. Positive Dauer und gültige Analyse explizit prüfen, weil `AnalyzeAsync` Fehler intern fängt.
2. Genau die bisherigen Serveranfrage und die lokale Zwei-Pfad-Prüfung unabhängig starten. Lokaler Dienst bleibt strikt bei `Path.GetFileName(originalFileName) + ".cutlist"` im konfigurierten Cutlist-Ordner und Originalfilmordner, dedupliziert, ohne Auflistung, Rekursion, Schreibprobe oder weitere Suchorte.
3. Ergebnisse und Fehler pro Quelle auffangen und auf dem Dispatcher in dieselbe beobachtbare Sammlung übertragen. Kein gemeinsamer Catch, der bei einem Fehler die andere Quelle oder das Fenster unterdrückt; kein ausschließliches Warten auf ein erfolgreiches `Task.WhenAll` vor Anzeige.
4. Eigene quellenspezifische Zustände und Abbruchsignale an die bereits abbrechbaren Servermethoden durchreichen. Gesamtflag für Doppelstarts von den laufenden Quellzuständen trennen. Die heutige `CanOpenMedia`-Sperre verhindert während `_cutlistSearchInProgress` einen Filmwechsel; bei späterer Lockerung oder Abbruch/Schließen bleibt die Generationsprüfung trotzdem erforderlich.
5. Fenster einmal mit der Suchsitzung verbinden und während laufender Abrufe aktualisieren. Keine Sammlung aus einer älteren Sitzung in ein neues Filmfenster übernehmen. Dialog-/Fensterende beendet Bindung und Anwendung späterer Ergebnisse. Keine UI-Aktualisierung nach Dispose.
6. Erst bewusste Auswahl und „Cutlist laden“ erzeugen einen Importauftrag: Server über echten Download; lokal aus geprüftem Inhalt bzw. erneut identitätsgeprüftem Originalpfad. Keine Umwandlung lokaler Pfade in Server-IDs und keine lokale Datei über den Downloadweg behandeln.
7. Einen gemeinsamen Importkern für bereits eingelesenes Dokument, bestehende Größen-/Endfragment-Rückfragen, Planbildung und Zustandsübernahme nutzen. Den manuellen Reader-Aufrufer darauf delegieren lassen; bisherige manuelle Regeln nicht verschärfen. Endfragment-Wiederaufnahme und Präzisionsbefund aus Abschnitt C dürfen dabei nicht stillschweigend „behoben“ werden.
8. Direkt vor Anwendung Filmkontext und aktuellen Schnittzustand erneut prüfen. Ohne Zustimmung zu notwendigem Ersatz und gegebenenfalls bestehender Korrektur keine Übernahme. Nach Fehler/Ablehnung bleiben Bereiche, offene Marke, Auswahl, Namenszustand, Autor, Motor und Uploadzustand erhalten. Namensvorbereitung und andere potenziell fehlschlagende Arbeit vor dem endgültigen Planersatz erledigen; der heutige Lader setzt den Plan bereits vor dem Laden von Namenseinstellungen.

### Freie Wahl bei Wiederaufnahme eigener Arbeit

Expliziter Referenzfall: Der Anwender hat eigene Bereiche gespeichert, später gibt es zusätzlich Server-Cutlists. Die Übersicht zeigt **beides vollständig**, ohne Aussage „lokal unfertig“ aus der Geometrie. Er kann eigene Arbeit weiterführen, eine beliebige Serverfassung übernehmen oder ohne angebotene Cutlist selbst schneiden.

| Entscheidung | Wirkung auf aktuellen Plan | Wirkung auf gespeicherte eigene Cutlist |
|---|---|---|
| Lokal laden | Nach bewusster Bestätigung gespeicherte Bereiche übernehmen; erforderliche Ersatz-/Validierungsentscheidungen beachten | Nur lesen; nicht verändern |
| Server laden | Genau den ausgewählten Serverplan nach denselben Schutzprüfungen übernehmen | Nicht schreiben, überschreiben, löschen oder zusammenführen |
| Ohne Cutlist / Fenster schließen | Vorhandene Schnittarbeit unverändert erhalten; bei frisch geladenem Film bleibt der initiale Plan | Unverändert |
| Aktuelle ungespeicherte Arbeit vorhanden | Vor Ersetzung ausdrücklich bestätigen lassen, einschließlich offener Startmarke; bei Nein erhalten | Unverändert |
| Später eigene Cutlist speichern | Aktuellen Plan nach bestehender Speicherlogik speichern | Exakter vorhandener Name erfordert weiterhin ausdrückliche Überschreibentscheidung, Nein als Vorgabe |

Kein Auto-Merge von Server- und lokalen Bereichen. Die Auswahl selbst ersetzt keinen Plan. Laden schreibt keine Cutlist; der spätere Speicherbutton ist eine getrennte Handlung. Der Bestand hat keinen vollständigen Dirty-Zähler. Konservativer Ersetzungsschutz für vorhandene Bereiche, offene Marke oder geladenen Cutlist-Kontext reicht für diesen Umfang; keine neue Arbeitsstandsverwaltung daraus ableiten.

**Uploadkontext beachten:** Lokal bestätigtes Laden setzt den lokalen Uploadkandidaten. Serverimport darf keine Beziehung zu einer zuvor geladenen lokalen Datei als angeblich aktueller Cutlist fortführen. Im Bestand setzt `rememberAsUploadCandidate:false` keinen neuen Pfad, löscht aber einen alten Pfad auch nicht. Beim Wechsel Lokal → Server muss diese Zuordnung im gemeinsamen Importkonzept ausdrücklich konsistent werden; eine alte Datei darf weder als ausgewählte Serverfassung hochgeladen noch durch den Import verändert werden. Falls später neu gespeichert wird, erst dessen erfolgreicher Schreibvorgang setzt wieder einen aktuellen lokalen Uploadkandidaten. Diese Zustandskorrektur ist zu testen und nicht als bereits vorhandener Schutz auszugeben.

## G. Minimaler späterer Änderungsumfang und Risiken

| Datei / Bereich | Nach Freigabe erforderliche Arbeit |
|---|---|
| `App/Services/CutlistSearchResult.cs`, `CutlistServerClient.cs` | Server-Transportdaten und numerische Anfangssortierung erhalten; primär Adapter zum neuen Modell. Bestehende Cancellation nutzen. Keine künstlichen lokalen Felder/IDs in Serverrequests. |
| Neues neutrales Ergebnis-/Herkunftsmodell | Gemeinsame Anzeige, echte Herkunft, lokale Fundorte, Inhaltsidentität, technische Prüfung und unabhängige Quellenzustände |
| Neues `CutlistSearchResultsViewModel` | Beobachtbare Treffer, gruppierte View, stabile Auswahl/Sortierung, Status und Verfügbarkeit des Ladebuttons |
| `CutlistSearchResultsDialog.xaml` und `.xaml.cs` | Zwei Gruppen, neutrale Texte, knapper Befund/Details, Auswahl des neutralen Modells; automatische Erstselektion entfernen |
| `MainWindow.xaml.cs`, vorzugsweise ergänzende Partial-Datei | Eine Suchsitzung statt alleiniger Server-Dialogsteuerung, unabhängige Fehlerbehandlung und Dispatcherupdates, Filmkontext/Abbruch, gemeinsame Importanwendung und Ersetzungsschutz |
| Bereits vorgeschlagener Namenshelfer / lokaler Suchdienst | Reine gemeinsame Namensregel, exakt zwei Pfade, begrenztes Lesen, Bytevergleich, keine Schreibprüfungen |
| Cutlists-Reader / Kompatibilitätsbewertung | Bestehende Parser nutzen, technische Gültigkeit und Filmzuordnung trennen; noch vorhandene Werbung/Bearbeitungsstand nicht bewerten |
| Tests und Dokumentation | Gruppen, Fehler-/Zeitverhalten, Wiederaufnahme, Zustands-/Dateischutz und Importregressionen |

**Separat zu prüfen, nicht beiläufig Teil der Tabellenänderung:** genaue Sekunden-/Tick-Konvertierung; unveränderter Endfragmentimport; Bewahrung angrenzender Einzelbereiche; persistente offene Markierung und vollständige Metadaten-/Sitzungswiederherstellung. Für die beiden letzten Punkte reicht die aktuelle Keep-Repräsentation allein nicht. Kein zusätzliches Produktionspaket ist für die gemeinsame Tabelle erkennbar nötig.

Wesentliche Regressionen: serverbezogene String-Bindings/Sortierung verändern, Auswahl beim Nachladen verlieren, doppelte Dialoge, Serverfehler als Wiedergabefehler melden, Rückfragen doppelt ausführen, alte Uploaddatei behalten, vermeintlich fehlende Werbung als Defekt werten, Endfragmentkorrektur ohne echte Wahl anwenden. MP4Box, otr-can, Klebecenter, Kontrollplayer, Halbierungssuche und Frame-Lupe werden funktional nicht erweitert.

## H. Ergänztes Test- und Abnahmekonzept

### Quellen und Oberfläche

1. Nur Server, nur lokal, beide Quellen, kein Treffer; fehlende Serverkonfiguration verhindert lokale Prüfung nicht.
2. Server später / lokal später; jeweils bestehende bewusste Auswahl und Sortierung stabil. Initial keine gewählte Zeile und keine automatische Übernahme. Beide Quellzustände korrekt anzeigen, kein Abbruch der Serversuche durch lokalen Fund.
3. Serverfehler mit lokalem Treffer; lokale beschädigte/gesperrte Datei mit Servertreffern; erster Suchpfad unerreichbar bei Treffer im zweiten. Pro Quelle terminalen Zustand erreichen, keine global unterdrückte Übersicht.
4. Bytegleiche lokale Dateien einmal mit beiden Fundorten; unterschiedliche Inhalte getrennt, auch nur abweichende Kommentare oder unbekannte Felder. ReadOnly lesbar. Maximal zwei exakte Pfade, keine Verzeichnisauflistung oder Schreibprobe.
5. Gruppen Server/Lokal bei Benutzer-Sortierung erhalten; bisherige numerische Server-ID-Anfangssortierung, Bewertungen, Kommentare, Name und Downloads erhalten. Lokale Community-Werte nicht erfinden. Gruppenköpfe nicht laden, Detail-/Pfadtexte kopierbar.
6. Kein Produktumbau in dieser Phase; spätere WPF-Abnahme bei bisheriger Normal-/Mindestgröße mit langen Namen, zwei lokalen Konfliktdateien und vielen Serverzeilen. Tastaturauswahl, Laden, Weiter und Fensterschließen kontrollieren.

### Begonnene Arbeit und Freiwilligkeit

7. Mehrere fertige Remove-Bereiche, weitere Werbung bleibt: speichern, Film erneut öffnen, lokalen Treffer auswählen, laden, vorhandene Bereiche prüfen und zusätzlichen Bereich setzen. Nicht aus großen Keep-Resten als unpassend/beschädigt/unfertig klassifizieren.
8. Derselbe Wiederaufnahmefall mit inzwischen mehreren Serverangeboten: beide Gruppen vollständig, eigene Arbeit / Server / keine Cutlist frei wählbar. Für jede Wahl lokale Dateibytes vor/nach vergleichen; keine Löschung, Speicherung oder Zusammenführung durch Auswahl, Import oder Ablehnung.
9. Aktuelle ungespeicherte Bereiche und offene Startmarke: Ersetzen bestätigen oder ablehnen; bei Ablehnung den gesamten Zustand erhalten. Nicht nur Bereichsanzahl, sondern auch geänderte Werte, offene Marke und geladenen Kontext absichern.
10. Späteres Speichern unter dem vorhandenen Originalnamen: bisherige Überschreibfrage erhalten, Nein schützt alte Datei, bestätigtes Ja erst nach erfolgreichem Schreiben. Auswahl einer Serverfassung erteilt keine vorweggenommene Überschreiberlaubnis.
11. Offene Startmarke beim Speichern: heutige Nichterhaltung reproduzieren, geschlossene Bereiche erhalten. Nicht als vollständig wiederhergestellte Sitzung ausgeben und keine neue Endmarke erfinden.
12. Adjazente Remove-Bereiche, keine Remove-Bereiche und alles entfernt getrennt prüfen. Einzelmarken/Zeilen versus gleiches entferntes Zeitintervall unterscheiden. Anzahl der Keep-Bereiche ist kein Abschlussstatus.
13. Zeitpräzision beim echten Serializer-/Parserdurchlauf: 25/50 fps, Bruchraten, Dezimalgrenzen, unveränderte und abweichende aktuelle Dauer, Wiederholung mehrerer Speicher-/Ladezyklen. Auch winzige zusätzlich entstandene Bereiche prüfen; nicht nur sichtbare Millisekunden oder Bereichssummen vergleichen.
14. Gültiger eigener Endfragmentfall: bestehende Ja/Nein-Wirkung dokumentieren; vor Freigabe einer geänderten unveränderten Ladeoption eigene Regressionen für beide bisherigen Wege verlangen. Ein Endfragment allein macht die Cutlist weder beschädigt noch nachweislich unfertig.
15. Beschädigte Struktur/überlappende Bereiche, fremder Filmname, fehlende Größe/FPS und klare Zuordnungswidersprüche getrennt prüfen. Status „Bearbeitungsstand unbekannt“ bleibt von diesen Befunden unabhängig.

### Import und Lebenszyklus

16. Server und lokal über denselben fachlichen Importkern, vorhandene Größen-/Endfragmententscheidungen genau einmal; Namensvorschlag, Autor, vorgesehener Motor und Uploadzuordnung korrekt. Keine automatische Mischung mit aktuellem Plan.
17. Lokal → Server → Uploadversuch darf nicht die alte eigene Datei als aktuell ausgewählte Serverfassung behandeln. Lokal → Ablehnung, Server → Abbruch und Importfehler erhalten vorherigen Zustand und alle Originaldateien.
18. Filmwechsel, gleichnamiger Film in anderem Ordner, neues Suchfenster, Fensterende und verspätete Antworten: keine veralteten Zeilen oder Anwendung im falschen Film. Ausgetauschte lokale Datei nach Vorprüfung erneut absichern.
19. Server-Button und manuelles Laden unverändert nutzbar; bestehende Dateigrößenwarnung > 5 %, historische Endmarken, Keep/Remove-Validierung und geschützte Speicherung bleiben getestet. Bekannte Präzisionsbefunde nicht durch gelockerte Assertions oder entfernte Tests verdecken.

### Tatsächlich ausgeführte Analyseprüfung

Vorhandene Bibliotheken ohne Produkt-Neubau/Restore geprüft:

- **72/72 Cutlists-Tests**, `dotnet test ...Cutlists.Tests.csproj --configuration Release --no-build --no-restore`.
- **135/135 App-Tests** mit Filter `FullyQualifiedName~Cutlist` gegen den vorhandenen isolierten Build-48-Teststand.
- **21/21 App-Schnittplan-Tests** mit Filter `FullyQualifiedName~CutPlanViewModelTests` gegen denselben Stand.
- Jeweils null fehlgeschlagen und null übersprungen. Diese grünen bestehenden Tests widerlegen die neu gefundenen Lücken nicht; die bisherigen Fälle decken diese Vollständigkeitsgarantie nicht ab.

Zusätzliche isolierte Diagnose unter `.build/cutlist-resume-analysis-20261103/`, SDK 10.0.303 / vorhandene Referenzpakete 10.0.11, vorhandene finale Build-48-Produktbibliotheken. Nur ein eigenständiges Diagnoseprogramm wurde kompiliert, ohne Solution-Build, Restore oder Buildnummerwechsel. Writer/Reader arbeiteten ausschließlich mit eigenen synthetischen Cutlists im Diagnoseordner. Keine reale Film-/Nutzerdatei und keine persönlichen Einstellungen geöffnet oder verändert.

Nachweis `RESULT.json`: normaler Teilplan unverändert wieder geladen und um dritten Bereich erweitert; offener Start nicht wiederhergestellt; benachbarte Bereiche zusammengefallen; Alles-/Nichts-Entfernen verarbeitbar; geänderte Dauer erzeugt zusätzlichen Endbereich; unvollständige Struktur wird abgewiesen; eigener Endfragmentfall löst bestehende Korrekturgeometrie aus. Zusätzlich **je 1000 deterministische 25-/50-fps-Pläne** eines 7200-s-Films durch tatsächlichen Serializer/Parser und Builder geschickt: **175 bzw. 182 Fälle mit abweichenden Tick-Bereichen, null Verarbeitungsfehler**. Diese Zahlen beschreiben den synthetischen Prüfsatz, keine Häufigkeit bei realen Aufnahmen.

In einer vorläufigen Variante wurden alle Matrixfälle über denselben Dateipfad wiederholt veröffentlicht; dabei traten vereinzelt Zugriffsverweigerungen auf. Um das von der Zeitumrechnung zu trennen, verwendet die abschließende Matrix denselben echten Serializer/Parser direkt im Speicher; die gezielten Dateifälle laufen weiterhin über Writer/Reader. Die Präzisionsabweichungen sind so ohne Dateizugriffsfehler reproduziert. Keine Test-/Produktänderung zur Grünfärbung vorgenommen.

Abschlussprüfung: SHA-256-Nachvergleich von **411 vorhandenen Dateien** (Quell-/Testdateien einschließlich Locks, README/Regeln, die drei vorbestehend geänderten Dokumente, Version/Buildzähler und vorhandene finale Test-/Produktbibliotheken) ohne Abweichung. Der Git-Status enthält außer den sieben vorbestehenden Nutzeränderungen nur die beiden Analyseberichte: den gezielt ergänzten ersten Bericht und dieses neue Dokument. Die Diagnose liegt im bestätigten ignorierten `.build`-Bereich. Alle lokalen Dokumentverweise sind gültig, beide Berichte ohne nachgestellte Leerzeichen. `git diff --check` ohne Fehler, lediglich bestehende LF/CRLF-Hinweise zu den drei zuvor geänderten Dokumenten.

Kein neuer Gesamt-/Produktbuild und keine praktische WPF-Abnahme durchgeführt, da keine Oberfläche implementiert wird und Build 48 zu erhalten ist. **Produktcode, vorhandene Nutzeränderungen, Version 1.0.2 und Buildzähler 48 unverändert; kein Restore, Commit, Push, Paket oder Release.** Nächster Schritt ist die gesonderte Freigabe des Tabellen-/Suchkonzepts und die ausdrückliche Abgrenzung der in Abschnitt C nachgewiesenen Importgrenzen. Keine automatische Umsetzung aus dieser Bestandsaufnahme.

**Hier endet die Bestandsaufnahme. Umsetzung erst nach gesonderter Freigabe; keine eigenständige Arbeitsstandsverwaltung beauftragt.**
