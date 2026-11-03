# Lokale Cutlists beim Filmöffnen – Bestandsaufnahme

**Einordnung nach Umsetzung am 03.11.2026:** Diese Bestandsaufnahme bewahrt die Untersuchung vor der Implementierungsfreigabe. Inzwischen sind die [gemeinsame Cutlist-Tabelle](CUTLIST-TABELLE-IMPLEMENTIERUNG-20261103.md) und die [Rot-/Grün-Bedienung mit Timeline-Vorschau](SCHNITTMODI-TIMELINE-20261103.md) lokal umgesetzt. Aktueller Entwicklungsabschluss: 1.709 Tests, Version 1.0.3 und Buildzähler 48; kein neues Paket oder Release. [Aktuelle Zusammenfassung](RELEASE-NOTES-1.0.3.md).

Stand: **03.11.2026**. Ausgangsbasis **CAN 1.0.2 Build 48**, Arbeitsbaum-HEAD `de62038c9012a99e10e65b92ba47be570c8ff6c5`. Untersuchung für eine mögliche 1.0.3, **keine Implementierungsfreigabe**.

Berücksichtigt sind der Entwicklungsauftrag und die Ergänzung zur verbindlichen Namenskonvention. Ausschließlich dieses Dokument wird neu angelegt. Produktcode, Oberfläche, Einstellungen, Version, Buildzähler, Pakete und vorhandene Nutzeränderungen bleiben erhalten. Kein Commit oder Push. Die folgenden Umsetzungsvorschläge sind noch nicht implementiert.

**Ergänzung vom 03.11.2026:** Die nachfolgende [UI-Bestandsaufnahme zur gemeinsamen Cutlist-Tabelle und Wiederaufnahme eigener Arbeit](CUTLIST-TABELLE-UI-BESTANDSAUFNAHME-20261103.md) ergänzt diesen Bericht. Verbindlich sind eine gemeinsame Übersicht mit den Gruppen **Server** und **Lokal**, unabhängige Suchen und freie Quellenwahl. Frühere Vorschläge für getrennte lokale Angebote oder das Auslassen der Serversuche sind damit abgelöst. Gültige begonnene Arbeit ist kein beschädigtes Dokument; der Bearbeitungsstand darf nicht aus Schnittanzahl oder -position abgeleitet werden. Der Ergänzungsbericht dokumentiert außerdem die nachgeprüften Grenzen eines verlustfreien Speichern-/Laden-Durchlaufs. Weiterhin keine Implementierung.

## A. Ist-Zustand

### Tatsächlich verwendeter Cutlist-Dateiname

Die Regel lautet exakt:

```csharp
Path.GetFileName(originalFileName) + ".cutlist"
```

Die letzte Videoendung bleibt erhalten. Aus `Film.HQ.mp4` wird **`Film.HQ.mp4.cutlist`**, aus `Film.mpg.HQ.avi` wird **`Film.mpg.HQ.avi.cutlist`**. Es wird weder die Endung ersetzt noch der Film-Ausgabename aus der Namensmaske verwendet. Auch ein vom Nutzer geänderter Ausgabename oder `SuggestedMovieName` bestimmt den Cutlist-Dateinamen nicht.

Nachweis im aktuellen Code:

- [MainWindow.xaml.cs](../src/CutAssistantNext.App/MainWindow.xaml.cs), Zeilen 2063 und 2134–2162: Ausgangspunkt ist `_viewModel.FileName`; daraus werden Originalbasisname, `ApplyToFile` und Speicherziel abgeleitet.
- [CutlistOutputTargetResolver.cs](../src/CutAssistantNext.App/Services/CutlistOutputTargetResolver.cs), Zeilen 14–34: `Path.GetFileName`, Anhängen von `.cutlist`, Prüfung auf ungültige Zeichen und höchstens 255 Zeichen. Kein Kürzen oder Nummerieren.
- [CutlistAutomaticSaveTests.cs](../tests/CutAssistantNext.App.Tests/Services/CutlistAutomaticSaveTests.cs), insbesondere `SavesWithoutCutEngineUsingOriginalNameAndUnchangedDocument` und `RejectsOverlongDerivedNameInsteadOfTruncatingOrRenamingIt`.
- [NamingCutlistWorkflowTests.cs](../tests/CutAssistantNext.App.Tests/Services/NamingCutlistWorkflowTests.cs), Zeilen 166–167: Cutlist-Dateiname und interne Originalzuordnung bleiben an den Originalnamen gebunden.
- Bereits dokumentiert in [CUTLIST-AUTOSPEICHERN-20261012.md](CUTLIST-AUTOSPEICHERN-20261012.md).

**Wiederverwendung:** Die Namensregel ist geeignet, `CutlistOutputTargetResolver.Resolve` als Ganzes dagegen nicht. Der Resolver verlangt ein vorhandenes Ausgabeziel, weist schreibgeschützte Cutlists zurück und ruft `ConfiguredOutputDirectory.ProbeWriteAccess` auf. Diese Probe erzeugt kurzzeitig eine Datei. Eine reine Suche darf das nicht tun; lesbare schreibgeschützte Cutlists müssen auffindbar bleiben. Bei späterer Freigabe nur die reine Namensbildung einschließlich ihrer Namensprüfung in einen gemeinsamen Helfer auslagern und Speicherweg sowie Suche darauf aufbauen. Heute existiert dafür keine eigenständige reine Namensfunktion.

### Speicherort, Format und Einstellung

Eigene Cutlists werden ausschließlich in `VideoFolderSettings.OwnCutlistsDirectory` gespeichert, nicht automatisch neben dem Film. Der zweite Suchort ist deshalb eine zusätzliche Lesemöglichkeit für dort abgelegte oder kopierte Cutlists, keine Änderung des Speicherorts.

[VideoFolderSettingsStore.cs](../src/CutAssistantNext.App/Settings/VideoFolderSettingsStore.cs), Zeilen 8–49, verwaltet den Wert in `%LOCALAPPDATA%\Cut Assistant Next\Settings\video-folder-settings.json`. Standardwert ist leer; beim Laden werden Rand-Leerzeichen entfernt. Fehlende, unlesbare oder beschädigte Einstellungsdateien ergeben leere Standardwerte. Die Einstellung wird unter Standardordner / Eigene Cutlists gepflegt. [VideoFolderPathResolver.cs](../src/CutAssistantNext.App/Settings/VideoFolderPathResolver.cs) prüft vollständige Pfade und Erreichbarkeit. Seine optionale Fallback-Funktion für Dateidialoge ersetzt keine Suche an beiden festgelegten Orten.

`CutlistGenerationViewModel.CreateDocument` erzeugt klassische INI-artige Abschnitte `[General]`, `[CutN]` und `[Info]`. Enthalten sind **Behaltebereiche**, während die Oberfläche mit Entfernungsbereichen arbeitet. `CutlistFileWriter.WriteSafely` speichert UTF-8 ohne BOM, mit CRLF und geschützter Veröffentlichung nach ausdrücklicher Überschreibentscheidung. Die Suche braucht weder diesen Writer noch eine Schreibprobe.

### Heutiges manuelles Laden und Validierungen

`LoadCutlistButton_Click` in [MainWindow.xaml.cs](../src/CutAssistantNext.App/MainWindow.xaml.cs), Zeilen 1834–1890, verlangt eine positive Mediendauer und öffnet einen Dateidialog. Das konfigurierte Cutlist-Verzeichnis dient als Startordner. Danach folgt `LoadCutlistFromFile` ab Zeile 1892. Auch heruntergeladene Server-Cutlists nutzen diese Methode, über eine temporäre Datei und mit `rememberAsUploadCandidate: false`.

| Prüfung / Verarbeitung | Tatsächliches heutiges Verhalten | Für lokale Erkennung |
|---|---|---|
| Datei und Encoding | `CutlistFileReader.Read`: synchrones Lesen; UTF-8, bei ungültigen UTF-8-Bytes Windows-1252 | Wiederverwenden, Dateizugriff außerhalb des UI-Threads |
| Struktur und Werte | `CutlistParser`: `[General]`, `NoOfCuts`, angegebene `[CutN]` mit Sekundenwerten sowie `[Info]` und `RatingByAuthor` erforderlich | Vor Angebot validieren; Fehler nicht als Treffer übernehmen |
| Behaltebereiche | `CutlistKeepSegment` verlangt Start ≥ 0 und Dauer > 0; `CutlistDocument` verhindert Überlappung und falsche Reihenfolge | Unverändert wiederverwenden |
| Historische leere Endmarken | Parser lässt Dauer 0 bei gültigem Start aus; widersprechende `DurationFrames` werden abgewiesen; ausschließlich leere deklarierte Bereiche werden abgewiesen | Kompatibilitätsregel erhalten |
| Dateigröße | `CutlistFileSizeCompatibilityDetector.FindMismatch`: Warnung bei **mehr als 5 %**, bezogen auf gespeicherte Größe; fehlende/nichtpositive Werte liefern ebenfalls `null` | Warnlogik erhalten; `null` ist kein positiver Identitätsnachweis |
| Bereiche hinter Filmende | `CutlistRemoveSegmentBuilder.Build` weist sie zurück | Notwendige Prüfung vor Übernahme |
| Winziger letzter Behaltebereich | `CutlistEndFragmentDetector`: höchstens 3 Frames, Nähe zum Ende mit eigenen 2-/1-Frame-Grenzen; Korrektur nur nach Zustimmung | Unverändert über bestehenden Ladeweg behandeln; keine automatische Korrektur |
| Übernahme | `CutlistCutPlanBuilder` bildet Keep → Remove; `CutPlanViewModel.LoadCutPlan` ersetzt den bisherigen Plan, löscht offene Startmarke und Auswahl | Erst nach ausdrücklicher Übernahmeentscheidung |

Der Sonderweg für Endfragmente muss vor einer endgültigen Ablehnung wegen Filmende berücksichtigt werden: Er erlaubt unter seinen Bedingungen bis zu einen Frame Überstand. Ein isolierter Aufruf des normalen Builders würde diesen korrigierbaren Fall vorzeitig abweisen.

Weitere Nebenwirkungen des erfolgreichen Ladens: Namensvorschlag wird gegebenenfalls in `CutNamingState` übernommen, Autor und vorgesehener Motor werden gemerkt, bei lokaler Datei auch der Uploadkandidatenpfad. Diesen Ablauf nach bestätigter Übernahme beibehalten. Beim bloßen Suchen, Prüfen oder Ablehnen darf keiner dieser Zustände verändert werden.

### Verfügbare Zuordnungsmerkmale und tatsächliche Grenzen

| Merkmal | In CAN verfügbar | Gegen Film beim manuellen Laden geprüft? |
|---|---|---|
| Originalname | `[General].ApplyToFile`; CAN schreibt den vollständigen Originalbasisnamen einschließlich Videoendung | **Nein** |
| Größe | `OriginalFileSizeBytes`; aktuelle Größe aus `MediaAnalysisResult.FileSizeBytes` | Ja, aber nur Warnschwelle > 5 % |
| Framerate | `FramesPerSecond`; aktueller erster Videostream hat ebenfalls einen optionalen FPS-Wert | **Nein**; Cutlist-FPS dienen hier der Endfragmentprüfung |
| Gesamtlänge des Originals | Aktuelle Medienanalyse enthält `Duration`; das von CAN erzeugte Cutlist-Modell speichert keine Originalgesamtlänge | Nur Bereichsgrenzen gegen tatsächliches Filmende |
| Schnittzeiten | Sekundenwerte je Keep-Bereich | Struktur, Reihenfolge und Filmende; kein Nachweis derselben Aufnahme |
| Container / Codec | Aktuelle Analyse enthält Format und Streams; Cutlist hat keine entsprechende vollständige Medienidentität | Kein Container-/Codecabgleich im Import |
| Weitere Metadaten | Autor, Bewertung, Kommentar, Seitenverhältnis, Namensvorschlag, vorgesehener Motor, Erstellprogramm/-version | Kein verlässlicher Aufnahme-Identitätsnachweis |
| Hash / Aufnahme-ID | Im verwendeten typisierten Cutlist-Modell kein Filmhash und keine eindeutige Aufnahme-ID | Nicht vorhanden |

`StartFrame` und reguläre `DurationFrames` werden nicht als alternative Schnittgrundlage eingelesen. Ausnahme ist die Widerspruchsprüfung einer leeren Endmarke. Eine FPS-Abweichung führt heute also weder zu einer automatischen Umrechnung noch zu einer Ablehnung des Imports. Andere Container werden im späteren Schnittworkflow behandelt, nicht durch diesen Cutlist-Lader.

Der Parser ist kompatibel und kein vollständiger strenger Validator: unbekannte Felder werden nicht im Modell bewahrt, zusätzliche nicht deklarierte Cut-Abschnitte werden nicht zur Schnittanzahl gerechnet, doppelte Schlüssel/Abschnitte überschreiben vorherige Werte. Bei eingelesenen optionalen FPS-/Größenwerten fehlen eigene Prüfungen auf positive, endliche Werte. `NoOfCuts=0` ist zulässig und ergibt über den Builder einen Plan, der den ganzen Film entfernt. Dies ist weder ein Beweis für Beschädigung noch für unfertige Bearbeitung; die leere geplante Ausgabe sollte ausdrücklich erkennbar sein. Ein Plan ohne gesetzte Entfernungsbereiche ergibt dagegen einen Keep-Bereich für den ganzen Film und `NoOfCuts=1`. Keine dieser Regeln wird in dieser Analyse geändert.

**Fazit zum Bestand:** Es gibt keinen vorhandenen lokalen Kandidatensucher und keinen allgemeinen Klassifikator „passend / unklar / unpassend“. Ein erfolgreich geparstes Dokument oder das Ausbleiben einer Größenwarnung beweist keine eindeutige Filmzuordnung. Auch das Ende des letzten Keep-Bereichs ist keine Originalgesamtlänge; Nachspann oder Werbung dürfen dahinter liegen.

## B. Wiederverwendbare Komponenten

- `VideoFolderSettingsStore` und die vorhandene Pfadnormalisierung für den konfigurierten Suchort; keine Wiederverwendung von Ausgabe-Schreibproben.
- Die aus `CutlistOutputTargetResolver` auszulagernde Namensbildung, unverändert in ihrer Bedeutung.
- `CutlistFileReader`, `CutlistParser`, `CutlistDocument` und `CutlistKeepSegment` für Format und Struktur.
- `CutlistFileSizeCompatibilityDetector` für den bestehenden Warnbefund, nicht als alleiniger Match-Entscheider.
- `CutlistEndFragmentDetector` / `Corrector`, `CutlistCutPlanBuilder` und `CutlistRemoveSegmentBuilder` für den vorhandenen fachlichen Import.
- Der Übernahmeteil von `LoadCutlistFromFile` mit `CutPlanViewModel.LoadCutPlan` für Plan, Namenszustand, Autor, Motor und lokalen Uploadkandidaten.

Der aktuelle Lader bündelt synchrones Datei-I/O, Rückfragen und Zustandsänderung. Für eine asynchrone Vorprüfung wäre eine kleine Trennung in Einlesen/Prüfergebnis und gemeinsamen bestätigten Import sinnvoll. Parser und Schnittumrechnung dafür nicht nachbauen. Ein ungeprüfter zusätzlicher Aufruf des kompletten Laders während der Kandidatensuche würde schon Rückfragen oder Planersatz auslösen.

## C. Erkennungskonzept – genau zwei gezielte Dateipfade

1. Originalpfad und Analysewerte des gerade geladenen Films als konsistenten Kontext festhalten. Erwarteten Cutlist-Namen einmal nach der bestätigten Regel bestimmen.
2. Suchorte: zuerst `OwnCutlistsDirectory`, zusätzlich `Path.GetDirectoryName(originalMediaPath)`. Leere/relative/ungültige Konfiguration überspringen; niemals relativ zum Arbeitsordner suchen. Keine Ordner anlegen.
3. Vollständige Verzeichnispfade normalisieren, abschließende Trenner ohne Beschädigung von Wurzeln vereinheitlichen und unter üblichen Windows-Bedingungen ohne Beachtung der Groß-/Kleinschreibung deduplizieren. Identische Ordner nur einmal prüfen. Laufwerksalias und UNC-Pfad sind durch Textnormalisierung nicht zwingend als derselbe Ort erkennbar; keine Laufwerks- oder Netzwerksuche zur Auflösung hinzufügen.
4. Je verbleibendem Ort ausschließlich `Path.Combine(directory, expectedFileName)` prüfen und gegebenenfalls lesen. **Keine Verzeichnisauflistung, Wildcards, Ähnlichkeitssuche, Unterordner, alternative Endungsregeln oder zusätzliche Suchorte.** Maximal zwei Kandidatenpfade; ein anders benanntes Dokument bleibt manuell ladbar.
5. Dateizugriff und Parser außerhalb des UI-Threads; für die beiden kleinen Kandidaten konsistente gelesene Inhalte festhalten. Nicht vorhandene Datei = kein Treffer. Nicht erreichbar, gesperrt, unlesbar oder beschädigt = gesonderter Diagnosebefund; der jeweils andere erlaubte Suchort bleibt prüfbar. Dateifehler nicht mit sicherem Nichtvorhandensein gleichsetzen.
6. Inhaltsvergleich und fachliche Bewertung ohne Änderung von Schnitt-/Namens-/Uploadzustand. Erst danach ein Angebot anzeigen. Vor bestätigter Anwendung aktuellen Filmkontext und Schnittzustand erneut prüfen.

Die Filmdatei darf auf einem Netzlaufwerk liegen. Dann ist genau der erlaubte Dateipfad im Filmordner ein gezielter UNC-/Netzlaufwerkszugriff, keine Suche nach Netzwerkfreigaben. Auch ein ausdrücklich konfigurierter Netzwerkordner benötigt nur diesen gezielten Zugriff. Kein HTTP-Aufruf oder Server-Fallback als Teil des lokalen Suchdiensts.

### Gleichnamige Treffer in beiden Verzeichnissen

Der konfigurierte Ordner ist primärer Suchort, **keine Erlaubnis zum Verwerfen eines abweichenden zweiten Treffers**. Beide erlaubten Pfade prüfen, auch wenn der erste Treffer bereits plausibel erscheint.

- Identischer normalisierter Pfad: nur einmal lesen.
- Verschiedene Pfade, **vollständig bytegleicher Inhalt**: ein Kandidat mit beiden Fundorten. Zeitstempel oder Länge allein genügen nicht. Bei höchstens zwei kleinen Dateien ist ein unmittelbarer Bytevergleich ausreichend; ein Hash ist nicht erforderlich.
- Abweichende Bytes: zwei unterschiedliche Dateistände, beide sichtbar erhalten. Unterschiedliche Schnittzeiten, Autoren, Kommentare oder Namensvorschläge dürfen nicht stillschweigend verschwinden. Auch bloß andere Zeilenenden/Encodings sind zunächst eine sichtbare Inhaltsabweichung; eine semantische Zusammenfassung wäre eine zusätzliche, separat zu definierende Funktion.
- Identität nicht prüfbar, etwa wegen Lesefehler: keine behauptete Dublette; unprüfbaren zweiten Fundort kennzeichnen.
- **Nicht** parsen und wieder serialisieren, um Identität zu bestimmen: unbekannte Felder und historische Frameangaben können dabei verloren gehen. Gleiche Schnittpunkte bedeuten ebenfalls nicht dieselbe Cutlist.

Eine Datei kann nach dem Angebot ersetzt werden. Bei späterer Implementierung entweder den geprüften Inhalt unverändert durch denselben Importkern übernehmen oder beim erneuten Lesen Inhaltsidentität prüfen und Änderungen erneut anbieten. Kein blindes erneutes Laden vom zuvor geprüften Pfad. Das gleiche Filmkontext-Prinzip gilt bei Abbruch, Fensterende und Dateiwechsel.

### Vorgeschlagene Bewertung zusätzlich zum vorhandenen Import

Dies ist eine neue Kandidatenbewertung für das automatische Angebot; die bestehenden manuellen Importregeln bleiben erhalten:

| Ergebnis | Vorschlag |
|---|---|
| Belastbar zugeordnet | Exakter erwarteter Dateiname, exakter `ApplyToFile`-Basisname einschließlich Endung, positive exakt gleiche Originalgröße, plausible endliche positive und konsistente FPS, gültige nutzbare Bereiche innerhalb der verfügbaren Mediendauer; keine widersprüchlichen Befunde. Als „Metadaten stimmen überein“ anbieten, nicht als kryptographisch bewiesene Aufnahmeidentität. |
| Unklar / möglicherweise passend | Identitätsmetadaten fehlen, Größen unterscheiden sich auch innerhalb der bisherigen 5-%-Toleranz, Framerate ist nicht sicher vergleichbar oder Endfragment erfordert bewusste Korrektur. Gründe anzeigen, keine ungefragte Übernahme. |
| Widersprüchlich / nicht regulär anbieten | Abweichendes `ApplyToFile`, klar andere FPS (z. B. 25 statt 50), Größenwarnung > 5 %, ungültige Datei/Bereiche oder nicht korrigierbare Bereiche hinter Filmende. Als abgewiesenen Befund erklären; bewusstes manuelles Laden bleibt verfügbar. |

Für FPS sind Rundungsunterschiede von echten Ratenunterschieden zu trennen; Vergleichstoleranz muss vor Implementierung anhand vorhandener ffprobe-Werte/Serializerpräzision festgelegt und getestet werden. Keine neue beliebige Toleranz als vorhandene Regel ausgeben. Fehlende Größen/FPS bleiben unbekannt, niemals durch 0, 25 fps oder geschätzte Originaldauer ersetzen. Eine Grenze innerhalb des Films beweist nur Anwendbarkeit, nicht dieselbe Aufnahme. Sogar Name + Größe + FPS garantieren ohne Filmhash keine eindeutige Identität.

## D. Bedienkonzept und Integrationspunkt

**Nachtrag:** Das nachstehende ursprüngliche Angebotskonzept ist durch die gemeinsame gruppierte Tabelle im [Ergänzungsbericht](CUTLIST-TABELLE-UI-BESTANDSAUFNAHME-20261103.md) ersetzt. Die Beschreibung des vorhandenen Öffnungspfads und der notwendigen Zustandsprüfungen bleibt relevant.

### Gemeinsamer Öffnungspfad

[MainWindow.MediaInput.cs](../src/CutAssistantNext.App/MainWindow.MediaInput.cs), Zeilen 18–31 und 67–87: Startparameter und Drag & Drop führen über `OpenMediaFileAsync` nach `LoadMediaFileCoreAsync`. Die Dateiauswahl ruft denselben Kern nach `ConfirmMediaReplacement` auf.

[MainWindow.xaml.cs](../src/CutAssistantNext.App/MainWindow.xaml.cs), Zeilen 2486–2524: vorhandenen Schnitt-/Namenszustand zurücksetzen → `AnalyzeAsync` → Schnittplan mit Mediendauer initialisieren → Player laden → Frame-Navigation/Keyframes starten → automatische Serversuche.

**Geeigneter Integrationspunkt:** nach erfolgreichem `LoadAsync`, `ConfigurePlayerFrameSearch` und Start der Keyframe-Ladung, **unmittelbar vor dem derzeitigen Aufruf von `SearchCutlistsForCurrentMediaAsync(skipIfNotConfigured: true)` bei Zeile 2523**. Dort sind Originalpfad, Analyse und initialisierter Schnittplan vorhanden; alle drei Öffnungswege werden erfasst. Zusätzlich explizit `AnalysisResult`, positive Mediendauer und gültigen Filmkontext prüfen: `AnalyzeAsync` fängt Analysefehler intern ab, statt sie grundsätzlich weiterzuwerfen. Keine Erkennung allein aufgrund eines erfolgreich geladenen Players.

Der lokale Suchdienst erhält nur Suchorte, Originalkontext und Abbruchsignal; `MainWindow` steuert Angebot und bestätigten Import. Keine Suche in mpv, ffprobe, Schnittmotoren oder im Anzeige-ViewModel. Fehlgeschlagene optionale Cutlist-Suche getrennt behandeln, damit sie nicht im vorhandenen Catch als „Wiedergabe fehlgeschlagen“ erscheint.

### Angebot und Schutz laufender Arbeit

- Ein eindeutig bewertbarer Kandidat: kurzer Hinweis mit Quelle und wesentlichen Metadaten, „Schnittpunkte übernehmen“ oder „Nicht übernehmen“. Keine automatische Übernahme; Ablehnung erhält aktuellen Zustand.
- Zwei abweichende Dateistände: Auswahl mit beiden vollständigen Fundorten, Autor, Anzahl der Bereiche, Metadaten-/Warnbefunden und Hinweis auf unterschiedliche Inhalte. Keine automatische Auswahl nur wegen Verzeichnisreihenfolge oder Änderungsdatum.
- Bytegleiche Kopien: ein Angebot, beide Fundorte nennen; nach Bestätigung einen klar bestimmten lokalen Pfad als Uploadkandidat merken, vorzugsweise den konfigurierten Fundort.
- Unklare Kandidaten: Gründe sichtbar nennen und bewusste Entscheidung verlangen. Vorhandene Größen-/Endfragment-Rückfragen im gemeinsamen Import erhalten; keine automatische Reparatur.
- Keine Treffer: kein zusätzlicher Fehlerdialog aus der lokalen Suche. Diagnoseprobleme zusammengefasst und zurückhaltend kennzeichnen, keinen Fehlerdialog pro Pfad.
- Vor Planersatz stets den aktuellen Zustand prüfen, einschließlich gesetzter Bereiche, offener Startmarke und bereits geladener Cutlist. Die heutige Methode `LoadCutlistFromFile` bietet dafür selbst keinen allgemeinen Ersetzungsschutz. Rückfrage beim Filmwechsel schützt bisherige Arbeit, ersetzt aber keine Prüfung direkt vor Cutlist-Übernahme.

**Bestehende Serversuche erhalten:** CAN sucht heute nach dem Player-Laden automatisch im konfigurierten Cutlist-Server. Der neue lokale Dienst führt keine zusätzliche Netzsuche aus. Gemäß ergänzendem Auftrag müssen Server- und lokale Suche unabhängig laufen und sämtliche verfügbaren Ergebnisse in einer gemeinsamen Übersicht zeigen. Ein lokaler Treffer darf die Serverabfrage nicht unterdrücken; ein Serverfehler darf lokale Treffer nicht verhindern. Keine automatische Übernahme, Quellenbevorzugung oder Zusammenführung. Der Server-Button und das manuelle Laden bleiben erhalten.

## E. Risiken und Sonderfälle

- **Langsame Netzpfade:** `Task.Run` hält die Oberfläche frei, beendet aber einen bereits blockierten synchronen Windows-Dateizugriff nicht zuverlässig. Ein `CancellationToken` allein ist kein harter I/O-Timeout. Asynchrone Reads, begrenzte Eingaben, Abbruchprüfung und Verwerfen verspäteter Ergebnisse vorsehen; für harte Zeitgrenzen wäre ein eigener gekapselter Diagnoseprozess eine zusätzliche Abwägung, kein vorhandener Mechanismus.
- **Beschädigte oder riesige Kandidaten:** Der heutige Reader liest vollständig, der Parser allokiert anhand `NoOfCuts`. Bei automatisch angestoßenem Lesen sind begründete Größen-/Anzahlgrenzen oder eine entsprechend begrenzte Reader-Variante zu prüfen. Konkrete Grenzen nicht ohne Testdaten festlegen; keine zweite Parserimplementierung.
- **Film umbenannt:** Nur Cutlist mit dem daraus exakt erwarteten Namen wird gefunden. Eine alte anders benannte Cutlist bleibt bewusst ausschließlich manuell erreichbar. Interner abweichender Originalname macht die Zuordnung unklar/widersprüchlich.
- **Abweichende Endung oder andere Aufnahme:** Keine Suche nach gleichnamigem Stamm mit anderer Endung. Gleiche Filmtitel sind kein Identitätsnachweis. Originalpfad verwenden, niemals eine temporär vorbereitete MP4-Arbeitsdatei.
- **ReadOnly:** Für die Erkennung zulässig, solange lesbar. Keine Schreibberechtigungen oder Schreibprobe erforderlich.
- **Verzeichnis fehlt / nicht zugänglich:** Zweiten erlaubten Ort trotzdem prüfen; kein automatischer Fallback auf Installationsordner, Arbeitsverzeichnis oder andere Laufwerke.
- **Änderungen während der Suche:** Sowohl Filmkontext als auch Cutlist-Inhalt und aktuelle Schnittarbeit vor Übernahme absichern. Fehler oder Ablehnung dürfen keine Teilübernahme hinterlassen.

## F. Voraussichtlicher Implementierungsumfang nach Freigabe

| Datei / Bereich | Kleiner vorgesehener Eingriff |
|---|---|
| `App/Services/CutlistOutputTargetResolver.cs` und neuer reiner Namenshelfer | Vorhandene Dateinamensbildung auslagern; Speichersemantik unverändert erhalten |
| Neuer `App/Services/LocalCutlistDiscoveryService.cs` samt Ergebnismodell | Zwei exakte Pfade, Ordner-Deduplizierung, Lese-/Fehlerbefunde, Inhaltsvergleich und Kontext/Abbruch; WPF-unabhängig |
| Neue Kompatibilitätsbewertung unter `Cutlists/Compatibility/` | Vorhandene Struktur-/Größenbefunde bündeln; `ApplyToFile` und FPS separat bewerten, fehlende Daten ausdrücklich ausweisen |
| `Cutlists/IO/CutlistFileReader.cs`, gegebenenfalls | Begrenztes asynchrones Einlesen bzw. wiederverwendbares Decoding geprüfter Bytes, ohne Formatduplikation |
| `App/MainWindow.xaml.cs`, ggf. neue Partial-Datei für lokale Cutlists | Ein Aufruf am gemeinsamen Öffnungspunkt; bestehenden Importkern für manuell/lokal/Server gemeinsam halten, Angebot und Zustandsprüfung steuern |
| Kleine lokale Auswahl-/Angebotsdarstellung | Ein Kandidat oder zwei abweichende Stände samt Befunden, ausdrückliche Übernahme/Ablehnung |
| Tests und Dokumentation | Namensregel, Suchgrenzen, Konflikte, Bewertung und unveränderten Import absichern |

Keine erforderlichen Änderungen an Schnittmotoren, Frame-Lupe, Halbierungssuche, Kontrollplayer, Klebecenter oder Namensmasken; keine zusätzliche Produktionsabhängigkeit vorgesehen. Bestehende Einstellungen reichen aus, kein neues Einstellungsformat nötig. Die Server-Trefferklasse/-Anzeige ist auf Server-ID und Download zugeschnitten und sollte nicht ungeprüft als lokales Modell übernommen werden.

## G. Testkonzept und Analyseprüfung

### Bereits vorhandene wiederverwendbare Tests

- App: `CutlistAutomaticSaveTests`, `NamingCutlistWorkflowTests`, `MovieSeriesNamingWorkflowTests`: exakter Originalname, getrennte Film-Ausgabenamen, gespeicherte Zuordnung und geschützter Speicherweg.
- App: `VideoFolderSettingsTests`, `MediaOpenRequestTests`, `CutPlanViewModelTests`: Ordnerkonfiguration, Öffnungswege/Schutz, Planübernahme und offene Startmarke.
- Cutlists: `CutlistParserTests`, `CutlistFileReaderTests`, `EmptyCutlistRangeTests`, `CutlistRangeValidationTests`, `CutlistDocumentTests`: Format, Encodings, historische Endmarken und ungültige Bereiche.
- Cutlists: `CutlistFileSizeCompatibilityDetectorTests`: fehlende Werte, kleine Abweichung, exakt 5 % und große Abweichung.
- Cutlists: `CutlistImportWorkflowTests`, `CutlistCutPlanBuilderTests`, `CutlistRemoveSegmentBuilderTests`, `CutlistEndFragmentDetectorTests`, `CutlistEndFragmentCorrectorTests`: bestehender Import einschließlich Endfragmententscheidung.

### Neue Prüfungen erst mit Implementierung

1. Exakte Namen für MP4, mehrteilige Endungen, AVI, Umlaute/Leerzeichen, unveränderte Namen trotz Ausgabevorschlag; zu lange Namen ohne Kürzung. Gleicher Stamm, andere Aufnahme oder Endung bleibt unbeachtet.
2. Treffer nur im konfigurierten Ordner, nur neben dem Film, an beiden Orten; identische normalisierte Ordner nur einmal. Nachweis über Dateisystem-Abstraktion: ausschließlich die erlaubten exakten Pfade, keinerlei Enumeration/Rekursion oder weitere Zugriffe.
3. Zwei bytegleiche Dateien mit verschiedenen Zeitstempeln zusammenfassen; gleiche Länge mit unterschiedlichen Bytes, nur andere Metadaten, gleiche Schnitte mit anderem Autor und unbekannten Zusatzfeldern als Konflikt erhalten. Ein beschädigter zweiter Treffer verschwindet nicht still.
4. Leere/relative/fehlende/unlesbare Konfiguration, gesperrte/gelöschte Dateien, ReadOnly, unerreichbares Netzlaufwerk; anderer Suchort bleibt wirksam, UI bleibt bedienbar. Lesen verändert keine Dateien und legt keine Probe-/Zieldateien an.
5. `ApplyToFile` exakt/abweichend/fehlend, Größe exakt/±5 %/>5 %/fehlend/nichtpositiv, FPS 25/50/29,97/Rundungen/fehlend/NaN/Infinity; keine erfundenen Ersatzwerte. Filmende und Endfragment-Sonderweg sowie `NoOfCuts=0` prüfen.
6. Angebot ablehnen oder Auswahl abbrechen: Bereiche, offene Startmarke, Namenszustand, Autor, Motor und Uploadkandidat unverändert. Bestätigte Übernahme ergibt denselben Zustand wie der bisherige manuelle Import. Vorhandene Schnittarbeit wird erst nach eigener Entscheidung ersetzt.
7. Datei-/Filmwechsel und Fensterende während langsamer Suche, Cutlist-Austausch nach Angebot, verspätete Ergebnisse: keine Übernahme in falschen Film und keine Teiländerung. Gemäß ergänzendem Auftrag beide Quellen unabhängig und gemeinsam sichtbar halten; lokale Funde unterdrücken die Serversuche nicht. Wiederaufnahme gültiger begonnener Arbeit und freie Wahl gegenüber neu verfügbaren Server-Cutlists gemäß Ergänzungsbericht testen.
8. Praktische WPF-Abnahme aller drei Öffnungswege mit einem und zwei Treffern, langsamen Netzpfaden und Ablehnung; danach manuelles Laden und unveränderter Schnitt mit beiden Motoren kontrollieren. Keine echten Filme in Git aufnehmen.

### In dieser Analyse ausgeführte Prüfung

Quellcode und oben verlinkte Dokumentation wurden gelesen. Vor Beginn waren bereits drei Dokumente (`AKTUELLER-STAND.md`, `RELEASE-1.0.2-BUILD48-RESULT.md`, `TAGEBUCH.md`) sowie die vier Produktions-`packages.lock.json` geändert. Diese Änderungen werden nicht bearbeitet.

Ausgeführt, jeweils mit vorhandenen Testbibliotheken und ohne Build/Restore:

```text
dotnet test tests/CutAssistantNext.Cutlists.Tests/CutAssistantNext.Cutlists.Tests.csproj --configuration Release --no-build --no-restore
dotnet vstest .build/release-1.0.2-build48-20261101/final-app-tests/CutAssistantNext.App.Tests.dll /TestCaseFilter:FullyQualifiedName~Cutlist
```

**Ergebnis: 72/72 Cutlists-Tests und 135/135 auf Cutlists gefilterte App-Tests bestanden; jeweils 0 fehlgeschlagen, 0 übersprungen.** Die normale App-Testausgabe unter `tests/.../bin/Release/net10.0-windows` enthält keine Test-DLL; deshalb wurde ausdrücklich der vorhandene isolierte Build-48-Teststand gegen die finalen Produktbibliotheken verwendet. Dies sind Prüfungen bestehender Binärstände, kein Nachweis eines neu kompilierten Arbeitsbaums und keine neue praktische WPF-/Filmabnahme.

Beide ersten Testversuche unter eingeschränktem Prozesszugriff wurden abgebrochen: VSTest konnte innerhalb von 90 Sekunden keine Verbindung zum Testhost herstellen. Die Wiederholung derselben gezielten Läufe mit erweitertem lokalem Prozesszugriff war erfolgreich. Kein Test wurde entfernt oder verändert.

Abschlussprüfung: SHA-256-Nachvergleich von **412 vorhandenen Dateien** (Quell-/Testdateien einschließlich Locks, die drei vorbestehend geänderten Dokumente, Version, Buildzähler und ausgewählte vorhandene Produkt-/Testbibliotheken) ohne Abweichung. Git-Status enthält zusätzlich zu den sieben vorbestehenden Änderungen ausschließlich dieses neue Dokument. Lokale Dokumentverweise und nachgestellte Leerzeichen geprüft; `git diff --check` ohne Fehler, lediglich bestehende LF/CRLF-Hinweise zu den drei bereits geänderten Dokumenten.

Ein nummerierter Build über `tools/build.ps1` wurde in dieser reinen Analysephase bewusst nicht ausgeführt, weil er den ausdrücklich zu erhaltenden Buildzähler erhöhen würde. **Version 1.0.2 und Buildzähler 48 unverändert.** Kein Restore, neuer Quellbuild, Produktcode-Formatierung, Commit, Push oder Paketierung. Nächster Schritt wäre die gesonderte Freigabe eines kleinen Implementierungsschritts für gemeinsamen Namenshelfer und gezielte Zwei-Pfad-Erkennung samt Tests; aus dieser Analyse folgt keine Freigabe.

**Nach dieser Bestandsaufnahme anhalten. Implementierung ausschließlich nach gesonderter Freigabe.**
