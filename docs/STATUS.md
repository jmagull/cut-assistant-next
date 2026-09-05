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

Cutlist-Server und Cutlist-Komfort:

- persistente persönliche Server-URL
- Validierung der persönlichen Server-URL
- Verbindungstest
- automatische Suche nach dem Laden eines Videos
- robuste Behandlung von 0, 1 und mehreren Treffern
- breiter Cutlist-Auswahldialog
- Anzeige technischer und qualitativer Serverinformationen
- neutrale Darstellung unterschiedlicher Formate und Autoren
- Sortierung neuerer Serverfassungen über die numerische Cutlist-ID
- nächster Schritt: Download der ausgewählten Cutlist und Übergabe an den bestehenden lokalen Cutlist-Lade- und Prüfweg

## Schnittsemantik

Die Benutzeroberfläche beschreibt Bereiche, die entfernt werden sollen.

`RemoveSegment` und `CutPlan` bilden diese Semantik in `CutAssistantNext.Core` ab. Für klassische Cutlists erzeugt `CutlistKeepSegmentBuilder` daraus die komplementären Behaltebereiche.

Das Bedienmodell bleibt damit auf das Entfernen von Werbung, Vorlauf, Nachlauf oder anderen unerwünschten Abschnitten ausgerichtet, während das klassische Cutlist-Format weiterhin seine Keep-Segmente erhält.

## Nächster geplanter Bauabschnitt

Die im Server-Auswahldialog gewählte Cutlist soll über ihre Cutlist-ID vom persönlichen Cutlist-Server heruntergeladen werden.

Die heruntergeladene Cutlist soll anschließend nicht über einen zweiten Ladealgorithmus verarbeitet werden. Stattdessen soll der bereits vorhandene lokale Cutlist-Ladeweg gemeinsam genutzt werden, einschließlich:

- Medien-Dauerprüfung
- Dateigrößen-Plausibilitätsprüfung
- Erkennung kleiner Endfragmente
- Aufbau des `CutPlan`
- Übernahme von `SuggestedMovieName` in den vorhandenen Naming-State

Erst nach dieser gemeinsamen Ladeintegration wird der provisorische manuelle Button für die Serversuche entfernt.

## Noch offen

- Download einer ausgewählten Cutlist vom Cutlist-Server
- direkter Upload neu erzeugter Cutlists auf den Cutlist-Server
- Bestätigungsdialog zum Überschreiben vorhandener Ausgabedateien
- persistente globale Wiedergabelautstärke
- Doppelklick auf eine Schnittposition zum Anspringen im Player
- echte klassische AVI-Dateien über einen geeigneten V1-Workflow schneiden
- integrierte Unterstützung mehrteiliger Aufnahmen
- spätere Smart-Rendering-Verfahren
- Installer und portable ZIP-Ausgabe
- Programmsymbol und finale V1-Produktgestaltung
- Benutzerhandbuch
