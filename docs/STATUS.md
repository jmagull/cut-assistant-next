# Projektstatus

## Fehleranzeige im Fortschrittsfenster

- Der konkrete Fehlergrund erscheint direkt im Statusbereich, auch bei eingeklapptem Protokoll. Derselbe Text wird im kopierbaren Protokoll festgehalten.
- Nachgereichte Fortschrittsmeldungen überschreiben die Fehleranzeige nicht. Bei Fehler bleibt das Fenster offen; fehlt ein Fehlertext, wird auf das Protokoll verwiesen.
- Release-Build ohne Fehler/Warnungen; 430/430 Tests bestanden. Live-Test bestätigt: Bei ungültigem ffprobe-Pfad erscheint der konkrete Hinweis auf die FFmpeg-Werkzeuge direkt im Fortschrittsfenster. Auch ein erneuter erfolgreicher Diplomatin-Schnitt mit HQ-Cutlist ist bestätigt.

## Bestätigung des integrierten AVI-Ablaufs

- Live-Test erfolgreich: Die Diplomatin und Rubikon mit jeweils zwei Cutlists. FFmpeg-Vorbereitung, Prüfung und MP4Box-Schnitt abgeschlossen; Anfang, Ende und Ton-Synchronität vom Nutzer als einwandfrei bestätigt.
- Das Protokoll im gemeinsamen Fortschrittsdialog ist standardmäßig aufgeklappt. Es kann eingeklappt und weiterhin in die Zwischenablage kopiert werden.

## Leere Cutlist-Bereiche

- Import ignoriert Null-Längen-Einträge historischer Cutlists (z. B. den leeren Cut1-Endmarker der Diplomatin-ColdCut-Datei). NoOfCuts wird im eingelesenen Dokument angepasst; die Quelldatei bleibt unverändert.
- Negative Dauern, widersprüchliche DurationFrames und ausschließlich leere Bereiche werden weiterhin abgelehnt. Regressionstests prüfen den unveränderten gültigen Filmabschnitt, die Entfernbereiche und erneutes Serialisieren/Einlesen.
- Release-Build ohne Fehler/Warnungen; 427/427 Tests bestanden. Live-Prüfung der zuvor abgelehnten Cutlist steht aus.

## Experimentelle Video-Vorbereitung am 13.09.2026

- „Videodatei laden“ öffnet MP4, AVI, MKV und weitere Videoformate. Originaldatei und Originalanalyse bleiben Grundlage für Serversuche, Schnittmarken und Cutlist-Erstellung.
- Erst „Schneiden“ bietet bei einem anderen erkannten Container die experimentelle FFmpeg-Vorbereitung mit Ja/Nein an. MP4-Inhalt wird anhand der Analyse erkannt, auch bei AVI-Dateiendung.
- FFmpeg kopiert alle Streams ohne Neukodierung in eine eindeutig benannte temporäre MP4. Nicht MP4-kompatible Streams führen zum Fehler statt zu stiller Konvertierung oder Entfernung.
- Vor MP4Box prüft CAN Container, Video-/Audiostreams, Codecs, Auflösung, Bildrate, Laufzeit und relative Stream-Startzeiten. Bis zu zwei Frames Unterschied und mittlere Bildratenabweichungen von rechnerisch höchstens zwei Frames über die gesamte Laufzeit sind erlaubt und werden im Detailprotokoll vermerkt. Laufzeittoleranz: zwei Frames, mindestens 100 ms; Startzeittoleranz: zwei Videoframes beziehungsweise 100 ms für Audio. Fehlende Frame-/Startzeit-Metadaten werden nur protokolliert. Deutliche Abweichungen oder veränderte Streams stoppen weiterhin den Schnitt. Das ist eine Metadaten-Plausibilitätsprüfung, kein vollständiger Nachweis jedes Bildzeitstempels oder der Ton-Synchronität.
- Der gemeinsame Fortschrittsdialog bietet das standardmäßig aufgeklappte „Protokoll“, Live-Ausgabe, Kopieren und Abbruch. Temporäre MP4-Dateien werden nach Ende, Fehler oder Abbruch entfernt; fehlgeschlagenes Aufräumen wird angezeigt. Die Originaldatei wird nicht verändert.
- Frühere manuelle Testbasis laut Gespräch: „Rubikon“ und „Die Diplomatin“, jeweils OTR-HD-AVI, per FFmpeg remuxt und mit MP4Box geschnitten. Auch der neue integrierte Dialog-/Schnittablauf wurde mit beiden Dateien erfolgreich live geprüft.
- Aktuelle technische Integrationstests: künstliche MPEG-4-AVI ohne Ton und H.264/AAC-MKV wurden über den neuen Runner remuxt und bestanden die Ergebnisprüfung. Existierende Zieldateien und vorab abgebrochene Aufträge werden abgewiesen. Auch die künstliche AVI mit MP3-Ton besteht jetzt: 51 gegenüber 50 gemeldeten Frames bei gleicher decodierter Bildanzahl; die kleine Metadaten-/Bildratenabweichung wird protokolliert.
- Diplomatin-Grenzfall korrigiert: Die Bildratentoleranz wird über die Original-Laufzeit berechnet. Die um 20 ms längere MP4-Containerlaufzeit hatte zwei Frames fälschlich als 2,000006 Frames erscheinen lassen. Mit Original-AVI und vorhandener remuxter MP4 direkt geprüft: bestanden. Regressionstest mit 317399/317397 Frames und 6347,98/6348 Sekunden ergänzt.
- Abschließender Release-Build: keine Fehler/Warnungen. 427/427 Tests bestanden.

## UX-Ergänzung am 13.09.2026

- Rückmeldung: Fenstergröße wird ohne geladenes Video nicht zuverlässig wiederhergestellt. Beim Speichern werden für normale Fenster jetzt ActualWidth/ActualHeight verwendet; für maximierte/minimierte Fenster weiterhin RestoreBounds. Speichererfolg oder -fehler wird protokolliert. Live bestätigt: Größenänderung, Schließen ohne Videoladen und Wiederherstellung beim Neustart funktionieren. Die ursprüngliche Ursache wurde nicht abschließend reproduziert. Ergänzte Tests prüfen den Einstellungs-Rundlauf ohne Wiedergabedaten und einen Schreibfehler. Build ohne Fehler/Warnungen, 402/402 Tests bestanden.

- Die untere Aktionsleiste bietet nach „Lokale Cutlist laden“ den Button „Cutlist vom Server laden“.
- Er startet für die aktuell geladene Mediendatei erneut die vorhandene Serversuche mit demselben Treffer-Dialog und Download-/Ladeweg wie beim Öffnen einer Mediendatei. Auch nach Abbruch oder Ablehnen einer Cutlist kann erneut gesucht werden.
- Gleichzeitige Serversuchen und ein Dateiwechsel während der Suche werden verhindert.
- „Namensmaske“ und „Schneiden“ stehen rechts. Die Buttons sind kompakter und ohne Auslassungspunkte; bei schmalen Fenstern bricht die linke Gruppe um.
- Live bestätigt: Wechsel zwischen zwei Server-Cutlists ohne erneutes Öffnen der MP4, aktualisierte Schnittzeiten und Übernahme des Namensvorschlags. Der Button-Umbruch bei schmalen Fenstern funktioniert.
- Bei kurzen Fenstern wurde die Schnittliste durch die feste Player-Höhe aus dem sichtbaren Bereich gedrückt. Der Hauptbereich hat jetzt eine automatische vertikale Scrollleiste, damit die unteren Aktionen erreichbar bleiben. Im Live-Test bestätigt.
- Release-Build: 0 Warnungen, 0 Fehler. Tests einschließlich der ergänzten Einstellungsprüfungen: 402/402 bestanden.
- Live bestätigt: flüssiges Scrollen bis zu allen unteren Buttons sowie Wiederherstellung der letzten Fenstergröße.
- Zwei dabei sichtbare Folgefehler sind korrigiert: Das native Videofenster wird über eine Windows-Fensterregion auf den Scroll-Anzeigebereich begrenzt (einschließlich DPI-Skalierung); die Player-Bedienelemente einschließlich Lautstärke und Zeitangaben können bei geringer Breite umbrechen. Beide Korrekturen sind im Live-Test bestätigt: Die Menüleiste bleibt frei und die Zeitangaben sind vollständig sichtbar.

## Aktueller Stand

Stand: 11.09.2026

- GitHub-Repository `cut-assistant-next` ist angelegt und derzeit privat.
- Der aktuelle Arbeitsbranch ist `feature/cut-application-configuration`.
- Das Projekt verwendet C#, .NET 10 und WPF.
- ffprobe ist über eine eigene Schnittstelle für die Medienanalyse eingebunden.
- MP4-Dateien sowie OTR-Dateien mit tatsächlichem MP4-Inhalt und Dateiendung `.avi` können ausgewählt und analysiert werden.
- Container-, Video- und Audiodaten werden in der WPF-Oberfläche angezeigt.
- mpv/libmpv ist als eingebetteter MediaPlayer integriert.
- Play/Pause, Seeking, Einzelbildnavigation und Lautstärkeregelung sind umgesetzt.
- Manuelle Schnittplanung mit Remove-Bereichen ist umgesetzt.
- Schnittbereiche können gesetzt, ausgewählt, korrigiert und gelöscht werden.
- Die Schnitt-Timeline visualisiert die markierten Bereiche.
- Bestehende Cutlists können eingelesen und auf die geladene Mediendatei angewendet werden.
- Fremd-Cutlists, deren Schnittbereiche über die Mediendauer hinausreichen, werden kontrolliert abgelehnt.
- Kleine Endfragmente historischer Cutlists werden erkannt und können kontrolliert korrigiert werden.
- Abweichungen zwischen der in der Cutlist gespeicherten Originaldateigröße und der geladenen Datei werden erkannt und dem Benutzer angezeigt.
- Cutlists können aus dem aktuellen Schnittplan erzeugt und lokal gespeichert werden.
- `ApplyToFile` bleibt dabei immer der exakte Originaldateiname der geladenen Mediendatei.
- Die Namensbildung ist über eine persistente Standard-Namensmaske konfigurierbar.
- Für die Namensmaske steht ein visueller Editor zur Verfügung; die technische Rohmaske bleibt bewusst schreibgeschützt.
- Standardautor und bis zu fünf Schnellbausteine werden als Cutlist-Einstellungen gespeichert.
- Neue Cutlists erhalten standardmäßig den editierbaren Kommentar `Mit Cut Assistant Next geschnitten.`.
- MP4Box ist als konfigurierbarer Schnittmotor integriert.
- Der MP4Box-Pfad wird nicht fest im Programmcode verdrahtet.
- Der MP4Box-Schnitt wurde mit realen OTR-Aufnahmen praktisch bestätigt.
- Ein Fortschrittsdialog zeigt Status und Ausgabe des Schnittvorgangs.
- Die geschätzte Laufzeit des geschnittenen Videos wird bereits vor dem Schnitt angezeigt.
- Die persönliche URL des Cutlist-Servers kann in den Cutlist-Einstellungen gespeichert werden.
- Die Verbindung zum Cutlist-Server kann aus den Einstellungen heraus getestet werden.
- Nach erfolgreichem Laden eines Videos wird bei vorhandener Server-Konfiguration automatisch nach passenden Cutlists gesucht.
- Die Serversuche verwendet den vollständigen Originaldateinamen.
- Ein HTTP-200-Ergebnis mit leerem Antwortkörper wird als regulärer Fall `0 Treffer` behandelt und nicht als XML-Fehler.
- Die Praxistests für `0 Treffer`, `1 Treffer` und `mehrere Treffer` wurden erfolgreich durchgeführt.
- Mehrere Cutlists derselben Aufnahme werden gemeinsam angeboten; unterschiedliche Formate wie MP4 und AVI werden nicht ausgefiltert.
- Suchergebnisse werden nach numerischer Cutlist-ID absteigend sortiert. Die ID dient dabei nur als derzeitiger Näherungswert für die Reihenfolge neuerer Serverfassungen und wird nicht als Datum interpretiert.
- Ein eigener breiter Auswahldialog zeigt die Server-Treffer.
- Beim Wechsel der ausgewählten Cutlist werden vorgeschlagener Filmname und Kommentar unmittelbar aktualisiert.
- Der Dialog zeigt unter anderem Format, Autor, Community-Bewertung, Stimmen, Autorenbewertung, Anzahl der Schnitte, Dauer und Downloadzahl.
- Das Format wird aus dem Cutlist-Dateinamen abgeleitet, beispielsweise `MP4 HQ`, `MP4 HD`, `AVI HQ` oder `AVI`.

### Cutlist-Server: Download und gemeinsamer Ladeweg

Die automatische Cutlist-Serversuche wurde bis zum vollständigen Download erweitert.

Technischer Ablauf:

- Suche weiterhin über `getxml.php?name=<vollständiger Originaldateiname>`.
- Die vom Benutzer ausgewählte Cutlist wird über `getfile.php?id=<Cutlist-ID>` heruntergeladen.
- Der Download erfolgt als rohe Bytes (`byte[]`), damit historische Cutlists nicht durch eine vorschnelle Zeichenkodierungsumwandlung verändert werden.
- Die Bytes werden ausschließlich als temporäre `.cutlist`-Datei abgelegt.
- Anschließend wird derselbe `LoadCutlistFromFile(...)`-Pfad verwendet wie beim manuellen Laden einer lokalen Cutlist.
- Dadurch gelten für lokale und vom Server geladene Cutlists identisch:
  - Dateigrößen-Plausibilitätsprüfung,
  - Endfragment-Prüfung und gegebenenfalls Korrektur,
  - CutPlan-Erzeugung,
  - Übernahme von `SuggestedMovieName`,
  - bestehende Fehlerbehandlung.
- Die temporäre Datei wird nach dem Ladeversuch wieder entfernt.
- Der auf dem Server angegebene Cutlist-Dateiname wird nicht als lokaler Temp-Pfad verwendet.

Der bisherige lokale Ladealgorithmus wurde hierfür ohne fachliche Verhaltensänderung in `LoadCutlistFromFile(...)` herausgelöst.

Tests nach der Erweiterung: **220/220 grün**.

#### Produktiver Praxistest mit Grey's Anatomy

Die automatische Suche und der anschließende Download einer MP4-Cutlist vom persönlichen cutlist.at-Zugang wurden erfolgreich getestet.

Die vom Server heruntergeladene Cutlist und dieselbe lokal vorhandene Cutlist ergaben identisch:

- vier Behaltebereiche,
- geschnittene Laufzeit `00:41:05.320`,
- denselben übernommenen `SuggestedMovieName`,
- dieselbe Namensvorschau im Schneiden-Dialog.

Damit ist praktisch bestätigt, dass vom Server geladene Cutlists denselben fachlichen Ladeweg durchlaufen wie lokale Cutlists.

Zusätzlich wurde für dieselbe MP4-Mediendatei eine auf dem Server vorhandene AVI-Cutlist getestet.

CAN zeigte erwartungsgemäß die vorhandene Dateigrößenwarnung:

- Größe laut AVI-Cutlist: `555.678.064 Bytes`
- geladene MP4-Datei: `667.888.009 Bytes`
- Abweichung: `20,2 %`

Nach bewusster Bestätigung ließ sich die AVI-Cutlist dennoch laden. Die Timeline erwies sich als praktisch deckungsgleich mit der MP4-Cutlist:

- Filmende bei beiden Cutlists: `01:29:58.080`
- Unterschiede ausschließlich an einzelnen Werbegrenzen,
- größte beobachtete Abweichung etwa `1,35 s`,
- Gesamtdifferenz der geschnittenen Laufzeit etwa `4,215 s`.

Die kleinen Unterschiede sind mit unterschiedlich gesetzten Schnittmarken bzw. menschlichem Augenmaß bei der Werbung vereinbar und sprechen nicht für eine grundsätzlich andere Timeline.

Praxisfolgerung:

**Eine abweichende Dateigröße oder ein anderes Container-/Ausgabeformat bedeutet nicht automatisch eine inkompatible Schnitt-Timeline.**

Die Dateigrößenprüfung bleibt deshalb bewusst eine Plausibilitätswarnung mit Benutzerentscheidung und kein hartes Inkompatibilitätskriterium. AVI-Cutlists werden in der formatneutralen Serversuche weiterhin nicht ausgefiltert.

Dieser Befund unterstützt zugleich die geplante V1-Strategie für echte AVI-Dateien: Wenn eine AVI-Datei verlustfrei für MP4Box vorbereitet werden kann und dabei ihre Timeline erhalten bleibt, können vorhandene historische AVI-Cutlists grundsätzlich weiterverwendbar sein. Dies muss später mit echten alten OTR-AVI-Dateien praktisch verifiziert werden.


### Cutlist-Server: direkter Upload

Der direkte Upload lokal gespeicherter Cutlists auf den persönlichen Cutlist-Server ist umgesetzt und praktisch bestätigt.

Technischer und fachlicher Ablauf:

- Für Suche, Download und Upload wird dieselbe konfigurierte persönliche Server-URL verwendet.
- Eine Cutlist muss vor dem Upload lokal gespeichert worden sein oder bewusst über `Cutlist laden …` von einem lokalen Pfad geladen werden.
- Beim Laden einer neuen Mediendatei wird ein zuvor gemerkter Upload-Kandidat verworfen.
- Vor dem Upload wird `ApplyToFile` nochmals gegen den vollständigen Originaldateinamen der aktuell geladenen Mediendatei geprüft.
- Eine automatisch vom Server heruntergeladene temporäre Cutlist wird nicht unmittelbar als eigener Upload-Kandidat behandelt.
- Eine Server-Cutlist kann jedoch als Vorlage geladen, bearbeitet und anschließend als eigene Cutlist lokal gespeichert und hochgeladen werden.
- Der Upload erzeugt im Speicher eine serverkompatible Kopie; die lokal gespeicherte Original-Cutlist wird nicht verändert.
- Für die Serverkopie werden `Application=Cut Assistant`, `Version=0.26.5.6` und `Author=joerg` verwendet.
- Benutzerkommentar, Bewertung, `SuggestedMovieName`, Fehlerangaben, Schnittbereiche und weitere fachliche Metadaten bleiben aus der gespeicherten Cutlist erhalten.
- Die Übertragung erfolgt als klassischer Multipart-POST einschließlich der vom bisherigen Cut Assistant verwendeten Formularfelder.
- Ein Doppelupload während eines laufenden Uploads wird verhindert.
- Der Server muss eine gültige numerische Cutlist-ID zurückgeben; andernfalls gilt der Upload als fehlgeschlagen.

#### Produktiver Praxistest am 11.09.2026

Der erste direkte Upload aus Cut Assistant Next auf den persönlichen cutlist.at-Zugang war erfolgreich.

- Server-ID: `2078572`
- Autor auf dem Server: `joerg`
- Autorenbewertung: `5`
- Benutzerkommentar und vorgeschlagener Filmname wurden korrekt übernommen.
- Die hochgeladene Cutlist erschien anschließend regulär in der persönlichen Upload-Übersicht.

Damit ist der vollständige Server-Workflow von automatischer Suche über Download und Bearbeitung bis zum erneuten Upload praktisch nachgewiesen.

## Vorhandene Projekte

- `CutAssistantNext.App`
- `CutAssistantNext.Core`
- `CutAssistantNext.Media`
- `CutAssistantNext.Cutlists`
- `CutAssistantNext.App.Tests`
- `CutAssistantNext.Core.Tests`
- `CutAssistantNext.Media.Tests`
- `CutAssistantNext.Cutlists.Tests`

## Aktueller Bauabschnitt

Cutlist-Server, Upload und Cutlist-Komfort:

- persistente persönliche Server-URL
- Validierung der persönlichen Server-URL
- Verbindungstest
- automatische Suche nach erfolgreicher Medienanalyse
- robuste Behandlung von 0, 1 und mehreren Treffern
- breiter Cutlist-Auswahldialog
- Anzeige technischer und qualitativer Serverinformationen
- neutrale Darstellung unterschiedlicher Formate und Autoren
- Sortierung neuerer Serverfassungen über die numerische Cutlist-ID
- Download der ausgewählten Cutlist über ihre Cutlist-ID
- bytegenauer Download ohne vorschnelle Zeichenkodierungsumwandlung
- gemeinsame Verarbeitung lokaler und heruntergeladener Cutlists über denselben Lade- und Prüfweg
- erfolgreicher Praxistest mit MP4- und AVI-Cutlists derselben Aufnahme
- Klick auf `Anfang` oder `Ende` eines Schnittbereichs springt den Player exakt auf die jeweilige Schnittposition
- die Bereichsauswahl bleibt beim Anspringen erhalten, sodass unmittelbar per Frame-Navigation feinjustiert werden kann
- die zuletzt eingestellte Wiedergabelautstärke wird global in `window-settings.json` gespeichert und nach dem nächsten Start wiederhergestellt
- ältere `window-settings.json` ohne Lautstärkewert bleiben kompatibel
- Lautstärke-Wiederherstellung im Praxistest mehrfach erfolgreich geprüft
- Shutdown nach der Änderung praktisch geprüft; kein reproduzierbarer Shutdownfehler festgestellt
- Überschreiben vorhandener Ausgabedateien über den nativen Windows-Speicherdialog erfolgreich umgesetzt und praktisch getestet; ohne ausdrückliche Bestätigung wird niemals überschrieben
- bei bestätigtem Überschreiben bleibt die vorhandene Zieldatei bis zum erfolgreichen finalen MP4Box-Join erhalten; auch ein fehlgeschlagener Join lässt die vorhandene Datei unangetastet
- Schneiden-Dialog nachgeschärft: „Aus obigen Eingaben neu erzeugen“; Vorschau ist read-only, markierbar und kopierbar
- aktueller Teststand: **399/399 grün**

### UI-Feinschliff und Timeline-Auswahl am 12.09.2026

- Ein Klick auf einen Schnittbereich in der Timeline wählt diesen weiterhin dunkelrot aus.
- Ein erneuter Klick auf denselben bereits ausgewählten Schnittbereich hebt die Auswahl nun wieder auf.
- Der Klick auf einen Schnittbereich in der Schnitttabelle springt weiterhin korrekt zur Schnittposition; die Auswahl kann anschließend auch über die Timeline wieder aufgehoben werden.
- Die Hauptfenster-Beschriftungen wurden für die V1-Oberfläche vereinfacht:
  - „MP4-Medienanalyse mit ffprobe“ → „Analyse, Schnitt und mehr“
  - „Videowiedergabe – technischer Host“ → „Player“
  - „Schnittbereiche – entfernen“ → „Schnittliste“
- Die Änderungen wurden im laufenden Cut Assistant Next praktisch geprüft; die Oberfläche wirkt damit klarer und produktnäher.
- Teststand nach den Änderungen: **399/399 grün**.
- Zugehörige Commits: `2ec111b` und `f8576cd`.

### Video-Informationen und Hauptfenster-Layout am 12.09.2026

- Die technischen Diagnoseblöcke „Datei und Container“, „Video“ und „Audio“ wurden aus dem Hauptfenster entfernt.
- Über den neuen Menüpunkt „Video-Informationen …“ werden diese Angaben nun in einem eigenen Dialog angezeigt.
- Der Dialog verwendet direkt das vorhandene `MainWindowViewModel`; Analysewerte werden nicht kopiert und es wurde keine zusätzliche Daten- oder Analyselogik eingeführt.
- Der Dialog zeigt bei normaler Fenstergröße sämtliche technischen Informationen vollständig an.
- Bei kleinerer Fenstergröße übernimmt ein ScrollViewer automatisch die Navigation durch die technischen Informationen.
- Die Schnittliste wurde neu strukturiert:
  - obere Schnittmarken-Bedienzeile bleibt dauerhaft sichtbar
  - untere Aktionsleiste mit „Bereich löschen“, „Cutlist erzeugen …“, „Cutlist hochladen …“, „Cutlist laden …“, „Namensmaske …“ und „Schneiden …“ bleibt dauerhaft sichtbar
  - nur die eigentliche Schnitt-Tabelle scrollt bei längeren Schnittlisten
- Die vertikalen Abstände im Hauptfenster wurden gezielt reduziert, ohne die Videofläche zu verkleinern.
- Dadurch sind bei normaler Fenstergröße mehrere Schnittbereiche gleichzeitig sichtbar, während sämtliche wichtigen Bedienfunktionen dauerhaft erreichbar bleiben.
- Nicht mehr verwendete Styles im Hauptfenster wurden entfernt.
- Build und vollständiger Testlauf erfolgreich: **399/399 Tests grün**.
- Der Umbau wurde mehrfach im laufenden Cut Assistant Next praktisch geprüft.
- Zugehöriger Commit: `1446f2e`.

**Liebes Tagebuch:** Der junge Padawan war sehr fleißig und heute voll auf Cupertino-Style fixiert. Redmond blieb außen vor.

## Schnittsemantik

Die Benutzeroberfläche beschreibt Bereiche, die entfernt werden sollen.

`RemoveSegment` und `CutPlan` bilden diese Semantik in `CutAssistantNext.Core` ab. Für klassische Cutlists erzeugt `CutlistKeepSegmentBuilder` daraus die komplementären Behaltebereiche.

Das Bedienmodell bleibt damit auf das Entfernen von Werbung, Vorlauf, Nachlauf oder anderen unerwünschten Abschnitten ausgerichtet, während das klassische Cutlist-Format weiterhin seine Keep-Segmente erhält.

## Nächster geplanter Bauabschnitt

Vor dem nächsten größeren V1-Baustein wird zunächst der V1-Feinschliff der Hauptoberfläche abgeschlossen. Als nächster UI-Schritt werden die technischen Diagnoseblöcke „Datei und Container“, „Video“ und „Audio“ aus dem Hauptfenster entfernt und über „Video-Informationen …“ in einen eigenen Dialog verlagert.

Danach steht als nächster größerer V1-Baustein die Unterstützung echter klassischer OTR-AVI-Dateien an. Der Cutlist-Server-Workflow von automatischer Suche über Download bis zum direkten Upload ist inzwischen vollständig umgesetzt und praktisch bestätigt. Der provisorische manuelle Button für die Serversuche wurde nach erfolgreicher gemeinsamer Ladeintegration entfernt; die Serversuche läuft ausschließlich automatisch nach erfolgreicher Medienanalyse.



## Noch offen

- echte klassische AVI-Dateien über einen geeigneten V1-Workflow schneiden
- integrierte Unterstützung mehrteiliger Aufnahmen
- spätere Smart-Rendering-Verfahren
- Installer und portable ZIP-Ausgabe
- Programmsymbol und finale V1-Produktgestaltung
- Benutzerhandbuch
