# Power-User-Ansicht – rechte Werkzeuge neu geordnet

**Fortschreibung 16.10.2026:** Persönlich geprüfter Funktionsstand für **CAN 1.0.0** eingefroren; Setup, Portable und zugehörige Artefakte beauftragt. [Verbindlicher Freeze-Umfang und offene Paketabnahme](FREEZE-1.0.0-20261016.md). Die folgenden Angaben dokumentieren den vorausgegangenen Entwicklungs- und Prüfzeitpunkt.

Stand: **16.10.2026**. Ziel weiterhin CAN 1.0.0; lokale Prüffassung ohne Versions-, Buildnummer-, Commit-, Push-, Paket- oder Release-Freigabe.

## Nutzerabnahme und Umfang

Der Nutzer bestätigt **„Video in Player ziehen check“**. Damit ist das Ablegen einer Videodatei im realen Nutzerlauf persönlich positiv gemeldet; keine zusätzliche pauschale Abnahme aller Dateiformate, Mehrfach-/Fehlerfälle oder des Kommandozeilenstarts daraus abgeleitet. Die grundsätzliche Zweispaltenaufteilung wird positiv bewertet. Der neue Auftrag betrifft ausschließlich den Feinschliff der rechten Bedienleiste gemäß abgestimmter Konzeptskizze und Übergabetext.

## Umgesetzte Anordnung

1. **Schnittliste**, darunter kurzer Hinweis „Rot markierte Bereiche werden entfernt.“ Die bisherige Bedienerklärung bleibt als Tooltip verfügbar.
2. Vier gleich breite Schnittpunktbuttons: **Videoanfang | Schnittanfang setzen**, darunter **Schnittende setzen | Videoende**.
3. Gemeinsame Zeile mit vorgemerktem Anfang, fest reservierter vollständiger Zeitangabe und **Frame-Lupe**. Lange Stundenwerte verändern die Lupenposition nicht. Bei schmaler Schnittspalte kann der Beschriftungstext umbrechen, Zeitangabe und Lupe behalten ihre Spalten.
4. Tabelle **Anfang | Ende | Dauer** mit dem verbleibenden Höhenplatz und eigener vertikaler Bildlaufleiste. **Bereich löschen** unmittelbar darunter rechtsbündig, ohne Auswahl weiterhin deaktiviert.
5. Trennlinie und Gruppe **Film schneiden** mit **MP4Box (schnell) | otr-can (framegenau)**. Vollständige bisherige Beschriftungen einschließlich „langsam“ sind als Tooltips zugänglich.
6. Gruppe **Cutlists**: **Lokale Cutlist laden | Cutlist vom Server laden**, darunter **Cutlist erzeugen | Cutlist auf Server hochladen**.
7. **Namensmaske** über die gesamte Breite.

Buttonhöhe 36 DIP, Rasterabstand 8 DIP, ruhige vorhandene CAN-Farben und ein einziger Abschnittsrahmen. Schnittspalte anfänglich 460 DIP, mindestens 430 DIP, weiterhin mit Splitter veränderbar. Bestehender Rückfall auf die Standardanordnung bei schmalem Fenster und Bildlauf bei geringer Höhe bleiben erhalten. Die Tabelle wird auf den verbleibenden Platz begrenzt; eine lange Liste verdrängt die darunterliegenden Werkzeuge nicht.

## Technische Umsetzung und geänderte Dateien

- `src/CutAssistantNext.App/MainWindow.xaml`: leere Power-User-Raster und Gruppenüberschriften, stabile Namen für vorhandene Werkzeuge sowie umbruchfähige Buttoninhalte. Keine duplizierten Funktionen.
- `src/CutAssistantNext.App/MainWindow.CutToolsLayout.cs`: ausschließlich vorhandene rechte Werkzeuge zwischen Standard-WrapPanels und Power-User-Grids umordnen. Alle Buttons, Ereignisse und Bindungen bleiben dieselben. Standardreihenfolge, Beschriftungen, Mindestbreiten, Ränder, Innenabstände, Ausrichtung und Inhaltsdarstellung werden beim Rückwechsel wiederhergestellt. Die vollständigen ursprünglichen Standard-Hinweise bleiben erhalten.
- `src/CutAssistantNext.App/MainWindow.Layout.cs`: Umschaltung der Werkzeuganordnung an bestehende Ansichtsumschaltung anbinden; rechte Mindest-/Anfangsbreite anpassen und Tabellenhöhe berücksichtigen.
- `tests/CutAssistantNext.App.Tests/CutToolLayoutTests.cs`: vier tatsächliche WPF-Tests auf STA-Threads, ohne Videodatei oder native Playerinitialisierung.
- README, Nutzeranleitung, Status und Tagebuch aktualisiert; vorherigen Ansichtsbericht als historischen Stand mit Verweis auf diese Fortsetzung erhalten.

**Player, Videohost und Tabelle bleiben in ihrem bisherigen visuellen Elternbaum.** Nur die rechten Werkzeuge werden neu einsortiert. Keine Änderung an Playerzustand, Medienanalyse, Motoren, Cutlist-/Uploadlogik, Namensmasken oder AVI-Schutz. Vorhandener Startparameter-/Drop-Code bleibt erhalten. Keine neue Abhängigkeit.

## Automatisierte Prüfung

Vollständiger Release-Rebuild **0 Warnungen, 0 Fehler**. Gesamtsuite **1347/1347 Tests**, 0 übersprungen: Core 196, Cutlists 72, Media 194, App 885. Gegenüber dem ersten Ansichtsstand vier zusätzliche Tests:

- Verbindliche Rasterpositionen, Reihenfolge und Identität der vorhandenen Buttons; Tabelle bleibt am selben Elternteil.
- Mehrfacher Wechsel stellt die ursprüngliche Standardreihenfolge und sämtliche gespeicherten Darstellungswerte wieder her.
- Aktivierungs-/Tooltipbindungen und echte Klickereignisse bleiben funktionsfähig: Schnittanfang setzen, Bereich auswählen und löschen; Löschbutton ohne Auswahl deaktiviert.
- Vollständiger mehrstelliger Stundenwert, feste Zeitspalte und erhaltene ausführliche Motorinformationen.

Geänderte C#-Dateien formatiert; `git diff --check` erfolgreich. Isolierter Locked Restore mit vorhandener lokaler Test-Runtime-Vorgabe. Rebuild ohne zählererhöhendes Buildskript, weil Versions-/Buildwechsel ausdrücklich nicht beauftragt ist.

## Native Prüfung und Sichtkontrolle

**Zwölf Fallgruppen** gegen die endgültigen WPF/libmpv-Assemblies bestanden, keine WPF-Bindungsfehler. Vorherige Ansichtsregression erhalten: Startpfad, gleicher HWND bei pausierter/laufender Wiedergabe, gleiche Schnittdaten, Splitter und schmale Fenster, natives Drop-Laden, abgebrochener Dateiwechel, AVI-/MP4-Wechsel und Navigations-/Lupenvorschau in beiden Ansichten.

Neue native Prüfungen:

- Leere Tabelle und **40 Schnittbereiche**; Bildlauf innerhalb der Tabelle, weiterhin erreichbare darunterliegende Aktionsgruppen.
- Schnittspalte 430 DIP und verbreiterte Spalte; gleiche Rasterbreiten und tatsächliche Werkzeuggrenzen innerhalb des sichtbaren Bereichs.
- Vollständige Anzeige **256204778:48:04.477**, gemessene Textbreite innerhalb der Zeitspalte; Lupenposition beim Wechsel von kurzem zu langem Zeitwert identisch.
- Alle vier Schnittpunktbuttons und **Bereich löschen** tatsächlich betätigt.
- **Frame-Lupe** über ihren Button geöffnet und reale Vorschau abgewartet; beide Motor-Ausgabedialoge, Cutlist-Erzeugung und Namensmaskendialog geöffnet und abgebrochen.
- Lokale Cutlist-Dateiauswahl geöffnet und abgebrochen. Upload- und Serverbutton über ihre bestehenden Schutzmeldungen geprüft; bewusst kein echter Upload und kein erneuter Onlineabruf.
- Betroffene Dialog-Fensterpräferenzen vor/nach Prüfaufrufen bytegleich erhalten; keine Cutlist gespeichert, kein Schnittauftrag gestartet, keine Einstellungen aus einem Dialog übernommen.

Layout bei 1920×1080, 1536×864 und 1280×720 logischen Einheiten nativ geprüft und gerendert. Die letzten beiden Größen sind Näherungen für Full HD mit 125/150 % Skalierung; **keine echte DPI-/Mehrmonitorabnahme**. Die größere Videofläche bleibt erhalten (bei 1920×1080 ca. 615 DIP Höhe statt 348 in Standard). Der größere rechte Bereich nimmt nur etwas Videobreite in Anspruch.

Eigene Renderbilder des tatsächlichen rechten WPF-Bereichs mit leerer/gefüllter Liste visuell kontrolliert. Die gesamte Hauptfensteraufnahme des unsichtbaren Prüfhelfers kann nach Spaltenänderungen veraltete Zeichnungen zeigen; maßgeblich für die Sichtprüfung sind die separat gerenderten Werkzeugbilder und die gemessenen Grenzen. Dies ist eine Einschränkung der automatisierten Aufnahme, kein Nachweis eines Fehlers im sichtbaren Programm.

## Prüffassung und Nachweise

Aktuelle Prüffassung:

`.build/power-tools-20261016/compiled/bin/CutAssistantNext.App/release/CutAssistantNext.App.exe`

Aus dem vollständigen Ordner starten. Anzeige bewusst weiterhin **0.3.0 · Build 31 · RC3**, funktional neuer lokaler Arbeitsstand; kein neues veröffentlichtes Build-31-Paket. Vorherige Prüffassung und eingefrorene Release-Pakete bleiben erhalten.

Nachweise unter `.build/power-tools-20261016`: `release-build.log`, `tests.log`, vier TRX unter `test-results`, `native-final.log`, `proof/PROOF.json`, leeres `proof/BINDINGS.log`, `proof/tools-1920.png`, `proof/tools-filled.png`, Größen-/Standardaufnahmen und Erhaltungsnachweise. Die zwölf nativen Fallgruppen verwenden Assemblies, deren SHA-256 mit den vier endgültigen CAN-Assemblies abgeglichen wird.

Alle acht Lockdateien sowie Version.props und `.build/build-number.txt` unverändert gegenüber Beginn; insbesondere vier vorbestehende Produktions-Lockänderungen erhalten. Keine Änderungen an eingefrorenen Archiven, Tags oder Veröffentlichungen.

## Persönliche Rückmeldungen nach dem Feinschliff

Am **16.10.2026** bestätigt der Nutzer ausdrücklich **Frame-Lupe geprüft**, **Frame-Liste geprüft**, **MP4Box-Schnitt erfolgreich**, **otr-can-Schnitt erfolgreich** und **Power-User-Layout persönlich geprüft**. Kommandozeilenübergabe mit einer realen HD-Aufnahme und Drag & Drop waren zuvor ebenfalls persönlich positiv bestätigt. Die persönliche Begutachtung des Layout-Feinschliffs ist damit erfolgt.

Für Radical wurde otr-can 0.1.5 zusätzlich nach Bild, Ton und allen Übergängen als blitzfrei bestätigt. Der erfolgreiche MP4Box-Ablauf ist getrennt vom nachgewiesenen Werbeblitzer bei **57:27.314** zu bewerten; dessen Behebung wurde nicht bestätigt. [Konkreter Befund und Vergleichsgrenzen](RADICAL-MP4BOX-WERBEBLITZER-20261016.md).

## Offen

Echte DPI-/Mehrmonitorwechsel, weitere HD-Workflow- und Release-Regressionen sowie die Paket-/Windows-11-VM-Prüfung dieses Arbeitsstands bleiben offen. Bestehende AVI-/Mediengrenzen und der dokumentierte MP4Box-Werbeblitzer bleiben erhalten. Die obige technische Layoutprüfung startete selbst keinen realen Filmschnitt; die späteren erfolgreichen Schnittläufe sind persönliche Nutzerrückmeldungen. Die frühere persönliche Build-31-/VM-Abnahme wird nicht auf diese Prüffassung übertragen. Keine endgültige 1.0-Freigabe oder Veröffentlichung.
