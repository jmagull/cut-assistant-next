# Projektstatus

## Aktueller Stand

Stand: 05.09.2026

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
- Der tatsächliche Download einer ausgewählten Server-Cutlist ist noch nicht umgesetzt.
- Aktueller vollständiger Testlauf: **218 von 218 Tests erfolgreich**.
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

CCutlist-Server und Cutlist-Komfort:

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
- aktueller Teststand: **222/222 grün**

## Schnittsemantik

Die Benutzeroberfläche beschreibt Bereiche, die entfernt werden sollen.

`RemoveSegment` und `CutPlan` bilden diese Semantik in `CutAssistantNext.Core` ab. Für klassische Cutlists erzeugt `CutlistKeepSegmentBuilder` daraus die komplementären Behaltebereiche.

Das Bedienmodell bleibt damit auf das Entfernen von Werbung, Vorlauf, Nachlauf oder anderen unerwünschten Abschnitten ausgerichtet, während das klassische Cutlist-Format weiterhin seine Keep-Segmente erhält.

## Nächster geplanter Bauabschnitt

Als nächster kleiner V1-Baustein soll das Überschreiben bereits vorhandener Ausgabedateien über einen ausdrücklichen Bestätigungsdialog ermöglicht werden. Eine bestehende Datei darf weiterhin niemals still überschrieben werden.

Danach soll der direkte Upload neu erzeugter Cutlists auf den persönlichen Cutlist-Server folgen. Die dafür benötigte Servergrundlage ist inzwischen vorhanden: persistente persönliche Server-URL, Verbindungstest, automatische Suche, Auswahldialog, Download über Cutlist-ID und gemeinsamer lokaler Lade- und Prüfweg.

Der provisorische manuelle Button für die Serversuche wurde nach erfolgreicher gemeinsamer Ladeintegration entfernt; die Serversuche läuft ausschließlich automatisch nach erfolgreicher Medienanalyse.



## Noch offen

- direkter Upload neu erzeugter Cutlists auf den Cutlist-Server
- Bestätigungsdialog zum Überschreiben vorhandener Ausgabedateien
- echte klassische AVI-Dateien über einen geeigneten V1-Workflow schneiden
- integrierte Unterstützung mehrteiliger Aufnahmen
- spätere Smart-Rendering-Verfahren
- Installer und portable ZIP-Ausgabe
- Programmsymbol und finale V1-Produktgestaltung
- Benutzerhandbuch
