# MP4Box-Filmteile: AUDIO-ENTHUSIAST sperren – Entwicklungsbuild 47

## Freigabe zum Einfrieren und Veröffentlichen am 31.10.2026

Der Nutzer beauftragt ausdrücklich, den bestätigten Stand einzufrieren, README/Dokumentation abzugleichen, zu committen, zu pushen und zu veröffentlichen. Veröffentlicht wird der unveränderte geprüfte Build-47-Paketsatz unter **v1.0.2-build47**; ältere Releases bleiben erhalten. Die bisherigen Aussagen über noch fehlende Veröffentlichungsfreigaben beschreiben den damaligen Paketierungsstand und sind durch diesen Auftrag abgelöst. Kein bisher nicht gemeldeter persönlicher Test wird dadurch als bestanden ergänzt. [Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md).

**31.10.2026.** Nutzerentscheidung nach den MP4Box-Vorprüfungsablehnungen: Für MP4Box-Schnitte soll AUDIO-ENTHUSIAST nicht verfügbar sein. In der lokalen Entwicklung **CAN 1.0.2 Build 47** umgesetzt. Motor otr-can 0.1.6 unverändert; eingefrorene Build-46-Pakete bleiben erhalten. Keine neue Setup-/Portable-Paketierung, kein Commit/Push oder Release beauftragt.

## Verhalten

Sobald mindestens einer der gewählten Filmteile als GPAC/MP4Box-Ausgabe erkannt ist, sind die Tonspurauswahl für AUDIO-ENTHUSIAST und dessen Startknopf deaktiviert. Ein dauerhaft sichtbarer Hinweis unmittelbar bei den Startknöpfen erklärt:

> MP4Box-Filmteil erkannt. AUDIO-ENTHUSIAST steht dafür nicht zur Verfügung. Bitte QUICK AND COPY verwenden.

Die Regel gilt auch für gemischte Paare. Tauschen nimmt die Herkunft mit; Ersetzen der erkannten Datei hebt die Sperre auf, sofern kein weiterer erkannter MP4Box-Teil vorhanden ist. QUICK AND COPY behält seine eigene Spurauswahl und Bestätigungspflicht. Keine automatische Auswahl oder Ausführung eines Verfahrens.

Der AUDIO-ENTHUSIAST-Dienst prüft dieselbe Herkunft nochmals unter den bestehenden Lesesperren vor ffprobe/Encoding. Ein direkter Aufruf kann die Dialogsperre daher nicht umgehen. Seine übrigen strengen Bild-/Ton-/Sampleprüfungen bleiben bestehen; keine Auffüllung oder Zeitachsenkorrektur eingeführt.

## Erkennung und Grenzen

Die beiden ausgewählten Furiosa-MP4Box-Dateien tragen in einem obersten MP4-`free`-Block die Kennung **IsoMedia File Produced with GPAC 26.07-rev0-ga07cbfff-master**. Der neue asynchrone Leser prüft diesen Herstellerpräfix unabhängig vom Dateinamen. Große Medienblöcke werden per Seek übersprungen; es werden maximal 4096 Blockköpfe und nur die kurzen Kennungspräfixe gelesen. Normale 32-Bit-, erweiterte 64-Bit- und bis zum Dateiende reichende Blocklängen werden behandelt. Handles werden freigegeben, Abbruch wird unterstützt.

Eine GPAC-Kennung belegt eine GPAC-Ausgabe, nicht die komplette Bearbeitungshistorie. Entfernte oder fehlende Kennungen erlauben keine sichere Herkunftsaussage; ein Dateiname allein wird nicht als Nachweis verwendet. Solche Dateien unterliegen weiterhin den vorhandenen strengen AUDIO-ENTHUSIAST-Prüfungen. Die Erkennung ersetzt keine Medienintegritätsprüfung.

## Geänderte Produktdateien

- `src/CutAssistantNext.Media/Analysis/Mp4BoxOriginDetector.cs`: begrenzte Container-Erkennung und gemeinsame Sperrmeldung.
- `src/CutAssistantNext.Media/Joining/Mp4JoinService.cs`: Dienstsperre vor Medienverarbeitung.
- `src/CutAssistantNext.App/Dialogs/JoinDialog.xaml` und `.xaml.cs`: Herkunft beim Laden lesen, Auswahlzustand mitführen, Tonspurauswahl/Start sperren und Hinweis neben den Knöpfen zeigen.
- Zwei neue Testsuiten für Erkennung/Dienst und WPF-Verhalten. Keine Produktionsabhängigkeit ergänzt.

## Prüfungen

- Geänderte C#-Dateien gezielt formatiert, XAML-Einrückung gepflegt; `git diff --check` fehlerfrei.
- Restore erfolgreich, vollständiger Release-Rebuild mit `tools/build.ps1`: **Build 47**, **0 Warnungen / 0 Fehler**.
- **1593 Tests bestanden**, keine übersprungen: **200 Core, 72 Cutlists, 387 Media, 934 App**. Darin **17 neue Fälle**: Herkunft nach Umbenennen, kein Dateinamenraten, große übersprungene Medien-/Paddingblöcke, ungültige/erweiterte Blocklängen, Abbruch, Dienstsperre für beide Positionen, gemischte Paare, Tauschen/Ersetzen und QUICK-Bestätigung.
- Native Prüfung mit den realen lokalen Furiosa-Dateien: beide MP4Box-Teile erkannt, otr-can-Teil 1 ohne GPAC-Kennung. Kompilierter Dialog: AUDIO-ENTHUSIAST gesperrt, QUICK nach Bestätigung aktiv. Kein Schnitt, Encoding oder Zusammenfügen dieser Filme gestartet.
- Dialog bei zwei Inhaltsgrößen gerendert und visuell kontrolliert (852×852 und 632×602, entsprechend normalen und kleinen Fenstergrößen). Der Hinweis sitzt außerhalb des scrollenden Eingabebereichs und bleibt neben den Startknöpfen sichtbar. Eine zunächst innerhalb der Scrollfläche platzierte Meldung wurde dafür in den festen Fußbereich verlegt.

Lokale Nachweise unter `.build/enthusiast-mp4box-build47-20261031/`: `NATIVE-PROOF.json`, zwei Dialog-PNGs und Paketlisten-Sicherung. Die vorbestehenden lokalen Lockänderungen bleiben erhalten. Frühere VM-Fälle beziehen sich weiter auf Build 46; die neue Sperre ist noch nicht persönlich als Setup-/Portable-Paket abgenommen.

## Persönlicher Dialogcheck und Paketauftrag

Am 31.10.2026 bestätigt der Nutzer „check :-)“ und beauftragt Setup, ZIP und Quellen. Die neuen Screenshots zeigen **John Wick Kapitel 4 Teil 1 MP4Box.mp4** und **Teil 2 MP4Box.mp4**, gesperrte AUDIO-ENTHUSIAST-Auswahl und den sichtbaren Hinweis auf QUICK AND COPY. Die QUICK-Paare sind ausgewählt, ihre Bestätigung fehlt noch; dessen Startknopf ist entsprechend gesperrt. Persönlicher neuer Dialogcheck positiv, keine vollständige Setup-/ZIP-/VM-Endabnahme daraus abgeleitet. [Paketauftrag und Nachweise](RELEASE-1.0.2-BUILD47-RESULT.md).

[Vorherige Furiosa-Diagnose](FURIOSA-MP4BOX-AUDIO-ENTHUSIAST-20261031.md), [laufende Build-46-VM-Prüfung](VM-SETUP-BUILD46-20261031.md).
