# Namensmaske ohne Neustart – 13.10.2026

## Fehler und Ursache

Nutzer meldet mit vier Bildern: Standardmaske von S01E01 auf 01x01 ändern und speichern; bei bereits geladener Chicago-Fire-Datei im Cutlist-Dialog weiter S13E07, auch nach **Aus obigen Eingaben neu erzeugen**. Erst ein App-Neustart verwendet 13x07.

`MainWindow` las die aktuellen Namenseinstellungen, übergab sie aber im Zweig mit vorhandenem `CutNamingState` nicht an die Cutlist-Factory. Deren ViewModel verwendete ausschließlich die alte Maske aus dem geladenen Zustand. Zusätzlich verwendete der Schnittdialog beim Neuerzeugen zwar bereits die aktuelle Standardmaske, gab aber im anschließenden Namenszustand weiter die vorherige Maske zurück.

## Korrektur

- `MainWindow.xaml.cs`: aktuelle Namenseinstellungen auch bei vorhandenem Namenszustand an die Cutlist-Factory übergeben.
- `CutlistGenerationViewModelFactory.cs`: aktuelle Standardmaske für diesen Dialog weiterreichen.
- `CutlistGenerationViewModel.cs`: gespeicherte Standardmaske getrennt vom bisherigen Namenszustand halten. Vorhandene Vorschau, Namen und Kontext bleiben beim Öffnen erhalten. Erst das ausdrückliche Neuerzeugen rendert mit aktueller Standardmaske und übernimmt diese in den danach weitergegebenen Zustand.
- `CutOutputViewModel.cs`: nach ausdrücklichem Neuerzeugen die tatsächlich verwendete Standardmaske ebenfalls im Namenszustand weitergeben.
- Sieben neue Fälle in `NamingCutlistWorkflowTests.cs` und relevante Dokumentation. Keine neue Abhängigkeit, keine Änderung an Parser, gespeicherten Benutzermasken, Cutlist-Zeiten, Player oder Schnittmotoren.

Geschützte Fremd-/eigene Vorschläge bleiben bei Eingabeänderung und Speichern ohne ausdrückliches Neuerzeugen erhalten. Manuelle Filmnamen werden weiterhin nur durch bewusstes Neuerzeugen ersetzt. Datum, führende Null in Folge 07 und bearbeiteter Folgentitel bleiben erhalten. Eine fehlerhafte neue Maske verwirft beim Renderfehler weder bisherigen Namen noch Schutzstatus.

## Prüfungen

Abschlussstand **0.3.0 · Build 29 · RC3**, normaler Release-Rebuild über `tools/build.ps1 -Configuration Release -NoRestore`, **0 Warnungen/0 Fehler**. Restore ohne Lockdateischreibung erfolgreich. **1076/1076 CAN-Tests bestanden**, keine übersprungen (Core 127, Cutlists 72, App 726, Media 151). Betroffene C#-Dateien formatiert; Diffprüfung bestanden. Alle acht Lockdateien und die zuvor erfassten persönlichen Einstellungsdateien per SHA-256 unverändert.

Neue Tests: Save/Load derselben isolierten Namenseinstellungen während einer Sitzung, alter geladener Zustand, geschützter und ungeschützter Vorschlag, Änderung des Folgentitels und bewusstes Neuerzeugen; Schutz fremder/manueller Namen auch mit Legacy-Maske; mehrfacher S/E↔x-Wechsel; Weitergabe vom Schnitt- zum Cutlist-Dialog; ungültige Maske ohne Zustandsverlust.

**Sechs WPF-/Speicherprüfungen** gegen Build 29: zwei gespeicherte Maskenwechsel über die tatsächliche **Speichern**-Schaltfläche, vier Cutlist-Ansichten bei **720×720** und **600×400** für beide Richtungen. Echter **Aus obigen Eingaben neu erzeugen**-Klick aktualisiert die gebundene schreibgeschützte Vorschau von **Chicago Fire S13E07 Unantastbar otr-can II [12.10.2026]** auf **Chicago Fire 13x07 Unantastbar otr-can II [12.10.2026]** und zurück. Kein App-Neustart oder erneutes Laden der Datei, Eingaben erhalten. Isolierte Cutlists gespeichert und wieder gelesen; Namensvorschlag, Originalzuordnung und Schnittbereiche erhalten. Keine Bindungsfehler; normale und kleine Ansicht visuell gesichtet, ausschließlich isolierte Einstellungen geschrieben.

Erster Vorprüfbuild scheiterte an fehlendem Core-Naming-Import im neuen Test; Import ergänzt, alle Tests erhalten, Wiederholung erfolgreich. Erster visueller Prüfhelfer suchte die Vorschau fälschlich als TextBlock; korrekt ist das bereits vorhandene schreibgeschützte TextBox-Feld. Nur Helfer angepasst, Mindestgröße nach Wiederherstellung des isolierten Fensterzustands ausdrücklich gesetzt und Prüfung erfolgreich wiederholt. Erstprotokolle erhalten.

## Persönliche Abnahme – 13.10.2026

Der Nutzer wiederholt die Fehlerfolge in fünf Bildern und bestätigt **„tatatatat :-) check“**:

1. Ausgangsvorschau **Chicago Fire S13E07 Unantastbar [12.10.2026]**.
2. Bewusstes Neuerzeugen verwendet die gespeicherte x-Maske: **Chicago Fire 13x07 Unantastbar [12.10.2026]**.
3. Maske absichtlich ungewöhnlich auf **S01xxE01** ändern und speichern; Editorbeispiel **Ghostbusters S01xxE01 Hexenprobleme [19.09.2026]**.
4. Cutlist-Dialog erneut öffnen: bisheriger Vorschlag **13x07** bleibt zunächst erhalten.
5. Bewusstes Neuerzeugen verwendet sofort die neue Maske: **Chicago Fire S13xxE07 Unantastbar [12.10.2026]**.

Maskenwechsel ohne App-Neustart persönlich positiv bestätigt, einschließlich mehrfacher Wechsel und ungewöhnlicher Bausteinkombination. Name, Staffel 13, Folge 07, Folgentitel und Datum bleiben in den Bildern erhalten. Der Vorschlag wird erst durch die ausdrückliche Aktion ersetzt. Daraus wird keine zusätzliche persönliche Cutlist-Dateispeicher-/Wiederladeprüfung oder neue Motorabnahme abgeleitet. Nutzerbilder lokal unter `.build/naming-mask-live-20261013/user-proof/acceptance-1.png` bis `acceptance-5.png` gesichert.

Persönliche Wiederholungsprüfung der Nutzerfolge erfolgreich bestätigt. Die neue Standardmaske wird beim Öffnen des Dialogs geladen; Änderungen außerhalb von CAN während eines bereits geöffneten modalen Cutlist-Dialogs sind nicht Bestandteil dieser Korrektur. Bisherige persönliche AVI-/Motor-/Bedienabnahmen und separate MP4Box-Blitzerbefunde bleiben erhalten. Öffentliche Build-16-RC3-Pakete unverändert. Kein Commit, Push, Setup/Portable oder Release.

Lokale Belege im ignorierten `.build/naming-mask-live-20261013`: Build-/Testprotokolle, isolierte Einstellungen/Cutlists, `UI-PROOF.json`, Prüfbilder und Erhaltungsprüfsummen.
