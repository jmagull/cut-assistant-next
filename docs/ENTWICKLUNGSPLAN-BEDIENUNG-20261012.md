# Entwicklungsrunde nach Build 16 RC3 – Reihenfolge und technische Bestandsaufnahme

Stand: 12.10.2026. Ausgangsbasis: veröffentlichte CAN 0.2.1 Build 16 RC3 mit otr-can 0.1.5; CAN-main a3b586af871ce867fd999f0cf5804596dbb51267.

Auftrag: die gesammelten Änderungswünsche ordnen und anhand der bestehenden Implementierung einen konkreten Ansatz festlegen. Die anfängliche Bestandsaufnahme enthält Planung und Codeanalyse. Anschließend wurden Schritte 1 bis 7 auf ausdrücklichen Auftrag umgesetzt; siehe Umsetzungsstand. Schritt 8 wurde technisch geprüft; offener Werkzeugbefund und persönliche Abnahme sind unten ausgewiesen. Kein Paketbau oder Release. Die zuvor verlangten Vorschläge für Namensmasken und Kontrollplayer sind unten enthalten.

## Ergänzung nach persönlicher Prüfung: Anfang und Ende

Die Bedienrunde aus Build 24 wurde vom Nutzer positiv geprüft; der zunächst gemeldete Maskenverdacht wurde korrigiert. Original-AVI mit beiden Motoren und fertige Übergangsprüfung funktionieren, AVI-Lupe ohne Vorschau separat offen. Auf zusätzlichen Wunsch Anfang-/Ende-Aktionen umgesetzt: Start 0:00, Ende tatsächliche Dauer minus 3 Sekunden, Navigation über alle Stellen, auch bei nur einem Behalteabschnitt. **Build 25 RC3**, **999/999 Tests**, **36 WPF-/Native-Kontrollen** bestanden. [Prüfbericht](KONTROLLPLAYER-ANFANG-ENDE-20261012.md). Anschließend Anfang-/Ende-Prüfung vom Nutzer positiv bestätigt („super“), Schnittvergleich im Player anhand von Kimi ebenfalls bestätigt. AVI-Lupe separat offen; kein Paket, Commit oder Push.

## Prüfstand – Schritt 8 durchgeführt, Freigabe offen

Lokaler **Build 24 RC3**, **0 Warnungen/Fehler**, **987/987 CAN-Tests** bestanden. Neue Verbundtests ergänzen alte/neue Masken, manuelle Filmnamen und Cutlist-Roundtrip; aktuelle WPF-Prüfungen sowie acht reale synthetische Schnittaufträge und deren Kontrollplayer bestanden. Kurztexte und persönliche Vorlagen erhalten. **Ein MP4Box-Zusammenfügungsfehler bei der Datei mit zwei gleichartigen AAC-Spuren bleibt offen**, auch direkt mit dem Werkzeug reproduziert. Persönliche Abnahme und uneingeschränkte Gesamt-/Releasefreigabe nicht behauptet. [Gesamtabnahmebericht](GESAMTABNAHME-BEDIENUNG-20261012.md). Keine Motoränderung, kein Commit, Push oder Paketbau.

## Umsetzungsstand – Schritt 7 abgeschlossen

Prüfen-Aktionen und ein unabhängiger, wiederverwendbarer Kontrollplayer für fertige Übergänge umgesetzt. Start −3 Sekunden, Navigation, Play/Pause, Position und eigene Lautstärke; Datei/Zeiten pro Abschluss erhalten. Laden/Abbruch/Dispose und EOF abgesichert. Lokaler **Build 23 RC3**, **0 Warnungen/Fehler**, **985/985 CAN-Tests**, **25 WPF-/Native-Kontrollen** und unabhängiger Zweiplayer-Nachweis bestanden. [Prüfbericht](SCHNITTUEBERGAENGE-KONTROLLPLAYER-20261012.md). Persönliche Abnahme offen; Schritt 8 offen. Kein Commit, Paket oder Release; Lockänderungen, Einstellungen und Build-16-Artefakte erhalten.

## Umsetzungsstand – Schritt 6 abgeschlossen

Rechtsklick auf Anfang/Ende zur Kontrolle −3/+3 Sekunden im Original umgesetzt, einschließlich Datei-/Filmstückbegrenzung und unverändertem Auswahl-/Lupenzustand. Dauerhafte SVG mit vorgegebenem Text in README und Anleitung ergänzt. Lokaler **Build 22 RC3**, **0 Warnungen/Fehler**, **963/963 CAN-Tests**, echte WPF-Rechtsklick-/Regressionsprüfung mit simuliertem Player sowie SVG-Sichtprüfung bestanden. [Prüfbericht](SCHNITTKONTROLLE-3SEK-20261012.md). Persönliche Abnahme noch offen; Schritt 7 nicht begonnen. Kein Commit, Paket oder Release; Lockänderungen und persönliche Einstellungen erhalten.

## Umsetzungsstand – Schritt 5 abgeschlossen

Einheitliche Zeilenabwahl einschließlich Anfang/Ende umgesetzt; Navigation unabhängig davon erhalten. Gemeinsame Auswahlquelle und Timeline-Bindung beibehalten, Text-/Editierquellen nicht abfangen. Lokaler **Build 21 RC3**, **0 Warnungen/Fehler**, **938/938 CAN-Tests**; kompilierte Tabelle/Timeline mit simuliertem Player, Mouse-Ereignissen, Bindung, Textfokus und Tastaturnavigation geprüft. [Prüfbericht](SCHNITTBEREICH-AUSWAHL-20261012.md). Persönliche Abnahme noch offen; Schritt 6 nicht begonnen. Kein Commit, Paket oder Release; Lockänderungen und persönliche Einstellungen erhalten.

## Umsetzungsstand – Schritt 4 abgeschlossen

Unabhängige S/Staffel/E/Folge/x-Bausteine und neue Standardmaske umgesetzt. Ghostbusters-Referenz mit korrekt interpretiertem OTR-Datum 19.09.2026 und ergänzten 01/01-Serienwerten; leere Seriendaten ohne Kennungen, führende Nullen erhalten. Legacy-Masken bleiben gespeichert unverändert, bekannte Paare werden nur in der Editoransicht geteilt und unverändert wieder in ihrer originalen Syntax serialisiert. Lokaler **Build 20 RC3**, **0 Warnungen/Fehler**, **924/924 CAN-Tests**, echter Editor mit Drop-/Move-/Entf- sowie Reset-/Speichern-/Abbrechen-Ereignissen geprüft. [Prüfbericht](NAMENSMASKEN-EINZELBAUSTEINE-20261012.md). Persönliche Abnahme noch offen; Schritt 5 nicht begonnen. Kein Commit, Paket oder Release; Lockänderungen und Benutzerkonfiguration erhalten.

## Umsetzungsstand – Schritt 3 abgeschlossen

Cutlist-Speicherung direkt im eingerichteten Cutlist-Verzeichnis mit ursprünglichem Aufnahmenamen umgesetzt. Kein Speicherdialog, Fallback oder neuer Ordner; sichere Konfliktentscheidung und geschützter vollständiger Schreibweg im Hintergrund. Lade-Dialoge, Cutlist-Format und Motoren erhalten; gleiche Ordnerprüfung wie Filmziele. Lokaler **Build 19 RC3**, **0 Warnungen/Fehler**, **881/881 CAN-Tests**; echte Dialog-/Speicherprüfung mit isoliertem Prüfcallback und Ordnerhilfeprüfung bestanden. [Prüfbericht und Grenzen](CUTLIST-AUTOSPEICHERN-20261012.md). Persönliche Speicherabnahme noch offen. Schritt 4 (Namensmasken-Editor) wurde nicht begonnen. Lockänderungen erhalten; kein Commit, Paket oder Release.

## Umsetzungsstand – Schritt 2 abgeschlossen

Vollständig editierbarer Filmname, Trennung vom Cutlist-Namenszustand und direktes Ziel im konfigurierten Filmordner umgesetzt. Gemeinsame Validierung und sichere Konfliktentscheidung für beide Motoren; kein Film-Speicherdialog oder Ordner-Fallback. Lokaler **Build 18 RC3**, **0 Warnungen/Fehler**, **860/860 CAN-Tests**, zusätzlich echter WPF-Dialog mit sofortiger Bindung, Neuerzeugen-/Schneiden-Buttons und Normal-/Mindestgröße geprüft. [Prüfbericht](AUSGABENAME-DIREKTSPEICHERN-20261012.md). Neue persönliche Filmabnahme noch offen. Schritt 3 (automatisches Cutlist-Speichern) wurde nicht begonnen. Vier vorbestehende Lockänderungen erhalten; kein Commit, Paket oder Release.

## Umsetzungsstand – Schritt 1 abgeschlossen

Am 12.10.2026 auf Auftrag „1) go“ ausschließlich die Reihenfolge im bestehenden CutActionsPanel geändert. Beschriftungen, Namen, Click-Handler, Bindings und Aktivierungsregeln erhalten; zwei horizontale Abstände passend zur neuen Reihenfolge angepasst. Keine Schnitt-/Speicher-/Masken-/Playerlogik verändert.

Lokaler Entwicklungsbuild **0.2.1 · Build 17 · RC3**, **0 Warnungen/0 Fehler**, **793/793 CAN-Tests bestanden**, keine übersprungen. Bestehende Abhängigkeiten mit NoRestore verwendet; kein Restore oder neuer Paketsatz. Alle acht lokalen Lockdateien unverändert, vier vorbestehende Produktionsänderungen erhalten.

Gebautes MainWindow ohne Start des Hauptplayers geladen und bei 960×780 sowie Mindestgröße 760×600 vermessen: alle acht Schaltflächen innerhalb des WrapPanels, visuelle Reihenfolge korrekt, keine Bindungsfehler. Die tatsächlich kompilierten Schaltflächen zusätzlich in einem unsichtbaren Vorschaufenster gerendert und geprüft. Bei Mindestgröße wird „Namensmaske“ als letztes Element in eine dritte Zeile umgebrochen; alle Buttons bleiben über den bestehenden Scrollbereich erreichbar. Keine Klickfunktion ausgeführt und keine Benutzereinstellung gespeichert; keine neue persönliche Bedienabnahme behauptet.

Nachweise unter .build/button-order-20261012: BUILD.log, TEST.log, test-results, LOCKS-BEFORE.json, MainWindow.before.xaml, LAYOUT.log und ui-proof/{LAYOUT-PROOF.json,BINDINGS.log,buttons-960.png,buttons-760.png}. Keine neuen dauerhaften Tests für diese reine Reihenfolgeänderung angelegt; vorhandene Gesamtsuite und gezielte WPF-Layoutprüfung verwendet.

Nächster Schritt gemäß Plan: **2 – Ausgabedateiname frei editieren und Film-Speicherdialog gemeinsam entfernen**. Schritt 2 wurde noch nicht begonnen. Kein Commit, Push, Setup oder Release für Schritt 1.

## 1. Festgelegte Reihenfolge

| Schritt | Umfang | Grund und Abschluss |
|---|---|---|
| 1 | Schaltflächen neu anordnen | Kleine, separat prüfbare Änderung ohne neue Ablauflogik. Reihenfolge und Erreichbarkeit bei schmalem Fenster prüfen. |
| 2 | Ausgabedateiname frei editieren und Film-Speicherdialog gemeinsam entfernen | Beide Wünsche betreffen denselben Dialog und dieselbe Übergabe an beide Motoren. Name, Zielordner und Konfliktschutz als zusammenhängenden Ablauf testen. |
| 3 | Cutlist automatisch im eingestellten Verzeichnis speichern | Erkenntnisse zur Pfad-/Konfliktbehandlung wiederverwenden; Originaldateiname und Cutlist-Inhalt getrennt von manuellen Filmnamen erhalten. |
| 4 | Namensmasken-Editor erweitern | Erst nach dem endgültigen Namenfeld den Vorschlagsgenerator erweitern. Alte Masken erhalten; Ghostbusters-Referenz und bedingte Einzelbausteine prüfen. |
| 5 | Auswahl zwischen Timeline und Tabelle vereinheitlichen | Alle Tabellenzellen, Kantenfokus und Lupenbedienung gemeinsam prüfen, bevor weitere Mausaktionen hinzukommen. |
| 6 | Rechtsklick zur Schnittkantenkontrolle; SVG und Anleitung | Baut auf der geprüften Zell-/Auswahlbehandlung auf. Navigation ohne Änderung der Schnittwerte. |
| 7 | Kontrollfenster für fertige Übergänge | Größter neuer UI-/Ressourcenumfang; erst nach stabiler Ausgabeübergabe und Navigation. Zweite libmpv-Instanz unabhängig prüfen. |
| 8 | Gesamtabnahme und gesonderter Abgleich der Kurztexte | Durchgängige Bedienung mit beiden Motoren prüfen. Konkrete neue Kurztexte sind noch nicht spezifiziert; keine Texte oder persönlichen Vorlagen eigenmächtig ändern. Kein Release daraus ableiten. |

Jeder Funktionsschritt erhält passende Tests, einen Release-Konfigurationsbuild und eine kurze Dokumentation. Persönliche Bedienprüfung ergänzt automatisierte Tests. Der neue Buildstand ist jeweils ein Entwicklungsstand; veröffentlichte Build-16-Pakete bleiben erhalten. Mehrere Änderungen erst nach ihrer Einzelprüfung zusammenführen.

## 2. Nachgewiesene Ausgangslage im Code

### Gemeinsamer Schneidedialog und Name

- Beide Motorbuttons führen zu MainWindow.ShowCutOutputAsync(engineKind). Namen und Zielpfade müssen daher nur einmal vor der Motorverzweigung behandelt werden.
- CutOutputDialog bindet SuggestedMovieName bisher als OneWay und IsReadOnly.
- CutOutputViewModel hält den Vorschlag separat. Änderungen der oberen Felder aktualisieren ihn bereits nicht automatisch; GenerateSuggestedMovieName berechnet ausdrücklich neu aus der Standardmaske.
- Der gemeinsame Ablauf verwendet den vorgeschlagenen Namen über OutputFileNameBuilder und öffnet anschließend einen SaveFileDialog.
- OutputFileNameBuilder ersetzt ungültige Zeichen durch Unterstriche, kürzt Randzeichen und ergänzt bei reservierten Gerätenamen einen Unterstrich. Das entspricht NICHT der gewünschten Zurückweisung manueller ungültiger Namen.
- CutNamingState wird auch bei Cutlist-Erzeugung verwendet. Ein manueller Filmname darf deshalb nicht ungeprüft als gemeinsamer Cutlist-Namenszustand zurückgeschrieben werden.
- CutRequest besitzt bereits OverwriteExistingOutput und schützt Original-/Arbeitsdateipfade. Beide Motorabläufe prüfen bestehende Ausgaben und verschieben erst ihre fertige Ausgabe. Der UI-Aufruf setzt derzeit overwriteExistingOutput: true, nachdem der native Speicherdialog den Konflikt behandeln konnte.

### Verzeichnisse und Cutlists

- VideoFolderSettings enthält bereits CutVideosDirectory und OwnCutlistsDirectory; neue Einstellungen sind nicht nötig.
- VideoFolderPathResolver.Resolve liefert bei fehlendem Ordner einen leeren Pfad. ResolveCutlists fällt auf das Originalverzeichnis zurück.
- Der Cutlist-Speicherdialog schlägt den vollständigen Originaldateinamen plus .cutlist vor: etwa Aufnahme.HQ.mp4.cutlist.
- CutlistFileWriter.Write erzeugt fehlende Verzeichnisse und überschreibt über File.WriteAllBytes. Nur den Dialog wegzulassen wäre deshalb unsicher.
- Das Uploadverfahren verwendet _lastSavedCutlistFilePath. Dieser darf erst nach tatsächlich erfolgreichem Speichern auf das neue Ziel gesetzt werden.

### Tabelle, Timeline und Kanten

- Die Tabelle ist aktuell IsReadOnly=True, FullRow und Single. Direkte Zellbearbeitung findet dort nicht statt. Zeitkorrektur erfolgt über vorhandene Schnitt-/Kantenfunktionen.
- Timeline und Tabelle binden beide an CutPlanViewModel.SelectedRemoveSegment.
- Die Timeline schaltet dieselbe Auswahl beim erneuten Klick bereits aus.
- Der Tabellenhandler schaltet dieselbe Zeile außerhalb der Anfang-/Endezellen ebenfalls schon aus.
- Klicks auf Anfang/Ende kehren vorher zurück: sie wählen den Bereich, setzen SelectedEdgeSide und springen zur Grenze. Daher fehlt das Umschalten gerade für diese Zellen.
- SelectEdge wählt die Zeile erneut aus; das muss bei einer Abwahl berücksichtigt werden. Abwahl löscht im ViewModel auch SelectedEdgeSide.
- PlaybackViewModel begrenzt Seek-Ziele auf 0 bis Laufzeit. Im untersuchten direkten Seek-Pfad ist keine generelle Sperre für rote Bereiche vorhanden. Die Anforderung zu nicht seekbaren roten Bereichen ist deshalb als zusätzlich abzugleichendes Verhalten zu behandeln, nicht als bereits nachgewiesene zentrale Regel.

### Abschlussfenster und Player

- CutCompletionViewModel erhält Ausgabepfad und Schnittplan. Es berechnet Anfang, Übergänge und nominales Ende aus aufsummierten Behaltedauern.
- CutCompletionDialog zeigt diese Punkte an und kann sie kopieren. Es wird ausschließlich bei cutSucceeded geöffnet.
- Der Hauptplayer besitzt eigene MpvMediaPlayerService-, PlaybackViewModel- und MpvVideoHost-Instanzen und wartet beim Schließen auf die Ressourcenfreigabe.
- Der libmpv-Adapter hält den eigentlichen Kontext und Ereigniskanal pro Instanz. Zur Kontextkonstruktion verwendet WindowedMpvContext zusätzlich einen ThreadStatic-Zwischenspeicher für das Fensterhandle.
- Ein zweites Fenster erscheint damit technisch realistisch, ist aber noch nicht praktisch nachgewiesen. Instanztrennung, Initialisierung, Fehlerfälle und wiederholtes Schließen müssen ausdrücklich geprüft werden.

## 3. Schritt 1: Schaltflächen

Exakte Reihenfolge in CutActionsPanel:

1. Bereich löschen
2. Lokale Cutlist laden
3. Cutlist vom Server laden
4. Schneiden MP4Box (schnell)
5. Schneiden otr-can (framegenau, langsam)
6. Cutlist erzeugen
7. Cutlist auf Server hochladen
8. Namensmaske

WrapPanel und vorhandene Click-Handler, Beschriftungen und Aktivierungsregeln erhalten. Abstände am neuen Zeilenende prüfen. Die optische Reihenfolge erzwingt keine zeitliche Abfolge: Cutlist speichern ohne Film schneiden bleibt möglich.

Geänderte Datei für Schritt 1: src/CutAssistantNext.App/MainWindow.xaml. Keine Motor- oder Schnittplanänderung. Umgesetzt und geprüft, siehe Umsetzungsstand.

## 4. Schritt 2: Freier Filmname und direktes Ausgabeziel

CutOutputViewModel erhält eine beschreibbare Eigenschaft für den aktuellen Basisdateinamen. TwoWay-Bindung mit sofortiger Übernahme; manuelle Eingaben werden bei Änderungen der oberen Felder nicht überschrieben. Der Button „Aus obigen Eingaben neu erzeugen“ ersetzt sie bewusst durch den automatisch gerenderten Vorschlag.

Hilfetext unter dem Feld exakt:

**Du kannst den gesamten Dateinamen abändern.**

Der manuelle Filmname bleibt lokal zum aktuellen Ausgabeauftrag. Die gemeinsame Cutlist-/Maskeninformation darf dadurch nicht verändert werden. Bereits übernommene Cutlist-Namensvorschläge bleiben beim Öffnen als Ausgangsvorschlag erhalten. Die gespeicherte Standardmaske wird nicht neu gespeichert.

Eine strenge, separat testbare Zielnamensprüfung verwenden. Keine stillschweigende Ersetzung, Kürzung oder Umbenennung des manuellen Namens. Leerwert, Windows-Gerätenamen, verbotene Zeichen, Pfadbestandteile und unzulässige Endpunkte/-leerzeichen verständlich zurückweisen. Zulässige innere Leerzeichen und bewusst gesetzte Trennzeichen erhalten. .mp4 einmal ergänzen; eine bereits angegebene .mp4-Endung nicht verdoppeln.

Ziel ausschließlich aus dem aktuellen CutVideosDirectory und dem validierten Dateinamen zusammensetzen. Vollständigen, existierenden und nutzbaren Ordner verlangen; keine Ersatzordner und kein automatisches Anlegen als Ausweichlösung. Hinweise nennen Einstellungen → Standardordner. Die reale Dateioperation bleibt maßgeblich: ein vorheriger Schreibtest kann spätere Rechte-/Netzwerkfehler nicht ausschließen. Prüfung außerhalb des UI-Threads, doppelte Starts verhindern.

Bestehendes Ziel: verständliche Wahl „Überschreiben“ oder „Abbrechen“, standardmäßig sichere Ablehnung. Nur nach ausdrücklicher Entscheidung OverwriteExistingOutput setzen. Vorher Original-/Arbeitsdateischutz prüfen; ein Original darf auch mit Bestätigung nicht ersetzt werden. Entsteht nach der Prüfung ein neuer Konflikt, ohne vorherige Zustimmung abbrechen. Keine zusätzlichen SaveFileDialog-Aufrufe.

Geplante Dateien: CutOutputDialog.xaml, CutOutputViewModel.cs, gemeinsamer Ablauf in MainWindow.xaml.cs sowie kleiner Zielpfad-/Namensprüfer unter App/Services/Cutting. Bestehenden sanierenden Helfer nicht global für andere Aufrufer umdeuten.

Tests: Titel vollständig ersetzen; Datum ändern/entfernen; S04E01 → 4x01; Änderungen oben überschreiben manuelle Eingabe nicht; Neuerzeugung ersetzt sie bewusst; gültige Namen unverändert; ungültige Namen abgelehnt; fehlender/nicht beschreibbarer Ordner; bestehendes Ziel, Abbruch und bestätigtes Ersetzen; Originalschutz; identischer Auftragspfad für beide Motoren; Cutlist-/Maskenzustand unverändert.

## 5. Schritt 3: Cutlist direkt speichern

CutlistGenerationDialog zur Bearbeitung des Inhalts bleibt erhalten; ausschließlich der zusätzliche Dateispeicherdialog entfällt.

Ziel ausschließlich OwnCutlistsDirectory plus vollständiger Originaldateiname plus .cutlist. Nicht den editierten Filmnamen übernehmen. ResolveCutlists mit Original-Fallback für diesen Ablauf nicht verwenden; Verhalten anderer Dateiauswahldialoge erhalten.

Bei fehlendem, nicht existierendem oder nicht nutzbarem Ordner:

**Cutlist konnte nicht gespeichert werden**

**Das Standardverzeichnis für eigene Cutlists ist nicht eingerichtet oder nicht erreichbar. Bitte überprüfe den Pfad unter Einstellungen.**

Technische Detailmeldung bei Bedarf ergänzen. Kein Ersatzordner und keine automatische Neuerstellung eines verschwundenen Ordners.

Konflikt: bestehende Datei erhalten; bewusstes Ersetzen oder Abbrechen anbieten, keine automatische Nummerierung/Umbenennung. Für diesen UI-Ablauf einen geschützten Schreibweg ergänzen, der einen nicht bestätigten Konflikt auch bei einer zwischenzeitlich neu angelegten Datei abweist. Nach Möglichkeit zuerst in eine eigene temporäre Datei im Zielordner schreiben und erst vollständig geschrieben veröffentlichen; eigene temporäre Datei im Fehlerfall entfernen. Bestehende Cutlist nicht vor erfolgreicher Vorbereitung löschen. Alte Writer-Aufrufer nicht ungeprüft verändern.

Erfolgsbestätigung und _lastSavedCutlistFilePath erst nach Erfolg aktualisieren. Upload bleibt die vorhandene gesonderte Aktion.

Geplante Dateien: MainWindow.xaml.cs, eng begrenzter geschützter Schreibweg in CutlistFileWriter.cs oder eigener Speicher-Service und entsprechende IO-/Ablauftests.

Tests: exakter Originalname, Inhalte/Cutzeiten unverändert, kein Dialog/Fallback, leere/fehlende/unbeschreibbare Ziele, sichere Konfliktentscheidung, Fehler während des Schreibens, keine beschädigte vorhandene Datei, gültiger Uploadpfad erst nach Erfolg; Cutlist ohne vorherigen Filmschnitt und unabhängig vom Motor speichern.

## 6. Schritt 4: Namensmasken – konkreter Vorschlag

Vorhandener Renderer unterstützt einfache Variablen und bedingte Präfixe, etwa %Staffel:S%. Letztere geben S nur bei vorhandenem Staffelwert aus. Literal-S und Literal-E sind dagegen immer sichtbar.

Empfohlener Ansatz: neue semantische Einzelbausteine S, Staffel, E, Folge und x anbieten. Als kleine explizite Renderer-Erweiterung bedingte Einzelvariablen verwenden:

- %S%: S nur bei vorhandenem Staffelwert.
- %Staffel%: Staffel unverändert, einschließlich führender Nullen.
- %E%: E nur bei vorhandenem Folgenwert.
- %Folge%: Folge unverändert.
- %x%: x nur wenn Staffel UND Folge vorhanden sind.

Diese Syntax ist seit Schritt 4 implementiert. Leer-/Whitespace-Werte, einzeln fehlende Werte und Umordnen sind in Renderer-/Parser-Tests geprüft. Bewusst eingegebener Literaltext bleibt Literaltext; keine globale Entfernung von S, E, x oder Bindestrichen.

Vorgeschlagene neue Standardmaske:

    %Name% %S%%Staffel%%E%%Folge%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]

Variante mit x:

    %Name% %Staffel%%x%%Folge%%Folgentitel: % [%Tag%.%Monat%.%YYYY%]

Bisherige %Staffel:S% und %Folge:E% weiterhin rendern. Im grafischen Editor werden bekannte alte Präfixtokens als Einzelbausteine angezeigt. Unveränderte benachbarte Teilpaare behalten intern ihre Herkunft und werden wieder in ursprünglicher Syntax serialisiert. Keine automatische Änderung gespeicherter Benutzermasken. Andere individuell angepasste Präfixe verlustfrei als bestehende Bausteine erhalten. Die bisherigen kombinierten S-/E-Bausteine werden nicht mehr in der Palette angeboten.

„CAN-Standardmaske“ verwendet die neue Standardmaske, „Änderungen verwerfen“ weiterhin die beim Öffnen geladene Maske. Drag-and-drop, Reihenfolge und Entfernen erhalten.

Verbindliches Editor-/Testbeispiel:

Original: Ghostbusters__Hexenprobleme_26.09.19_10-50_pro7maxx_25_TVOON_DE.HQ.mp4

Name Ghostbusters, Staffel 01, Folge 01, Folgentitel Hexenprobleme, Datum 19.09.2026. Staffel/Folge sind ergänzte Beispieldaten. OTR-Parser interpretiert den Datumsblock bereits als Jahr.Monat.Tag; kein Umbau der OTR-Erkennung nötig. Der Parser ergänzt bisher auch keinen separaten Folgentitel aus dieser Aufnahme; den Beispielkontext ausdrücklich setzen.

Vorschau: Ghostbusters S01E01 Hexenprobleme [19.09.2026]
Dateiname: Ghostbusters S01E01 Hexenprobleme [19.09.2026].mp4

Filmfall ohne Serienwerte: Die Teufelsbrigade [21.01.2021], kein SE. Zusätzlich nur Staffel, nur Folge, leere Werte, 1/01/04 sowie frei gesetzte Bindestriche und ältere Masken prüfen.

Geplante Dateien: NameTemplateRenderer.cs, NameTemplateBlockParser.cs, NamingSettingsDialog.xaml, NamingSettingsViewModel.cs soweit nötig, NamingSettingsStore.cs (Default), Beispielkontext in MainWindow.xaml.cs und vorhandene Core-/App-Namingtests.

## 7. Schritt 5: Einheitliche Auswahl

Klickauswertung zunächst in eine kleine, testbare Entscheidung trennen: getroffene Zeile, getroffene Spalte, aktuelle Auswahl und optionales Navigationsziel. Gemeinsame Auswahlquelle SelectedRemoveSegment erhalten.

Erster Klick wählt; erneuter Klick auf dieselbe Zeile wählt ab; andere Zeile wechselt. Auch in Anfang-/Endezellen weiter zum jeweiligen Zeitpunkt springen. Der Abwahlpfad darf nicht anschließend über SelectEdge dieselbe Zeile wieder auswählen. Bei Abwahl ist keine Kante mehr für die Frame-Lupe gewählt; eine folgende Auswahl stellt die Kante wieder her. Dies in der Bedienprüfung ausdrücklich prüfen.

Dauerzelle, Anfang, Ende, Wechsel zwischen Anfang/Ende, Timeline-Klick, Tastaturnavigation, pending start, Löschen und Umschalt+F prüfen. Keine neue direkte Zellbearbeitung einführen. Bestehende Zeitkorrektur und Halbierungssuche erhalten. Kleine Änderung im Tabellenhandler und gegebenenfalls ein Entscheidungshilfe-/ViewModel-Test; keine neue zweite Auswahlquelle.

## 8. Schritt 6: Kontrolle vor dem Schnitt und SVG

Rechtsklick ausschließlich auf Anfang oder Ende einer Datenzeile:

- Anfang: Grenze minus genau drei Sekunden.
- Ende: Grenze plus genau drei Sekunden.
- Ziel auf Originaldauer begrenzen; keine automatische Wiedergabe.
- Bei kurzen angrenzenden Filmabschnitten nicht in eine weitere entfernte Passage springen, sondern auf eine gültige Position im angrenzenden Filmabschnitt begrenzen. Fehlender angrenzender Filmabschnitt ist ein eigener Test-/Meldungsfall.
- Schnittwerte und Cutlist unverändert. Linksklick, Auswahl und Lupenbedienung aus Schritt 5 erhalten.

Da keine allgemeine rote Seek-Sperre im untersuchten direkten Pfad nachgewiesen ist, dafür eine kleine eigene Navigation-Zielberechnung verwenden und ihre Grenzfallregeln testen; keinen pauschalen Umbau des normalen Seek-Verhaltens vornehmen.

Dauerhaftes SVG unter docs/images/schnittkontrolle-3sek.svg: helle grüne Filmabschnitte, roter Werbebereich, Grenzen 10:00/12:00, Pfeile zu 09:57/12:03, −3 Sekunden/+3 Sekunden, lesbare Farben und Kontraste auch bei dunklem GitHub-Hintergrund. In README einbinden, Anleitung ergänzen.

Text unter Grafik exakt:

„Rechtsklick auf Anfang: 3 Sekunden zurück. Rechtsklick auf Ende: 3 Sekunden vorwärts. So lassen sich die Filmszenen unmittelbar vor und nach einem Schnittbereich kontrollieren.“

Testfälle: normale Grenzen, Anfang/Ende der Datei, kurze oder fehlende angrenzende Filmabschnitte, keine Veränderung von Auswahl-/Schnittdaten durch Navigation, Slider und Hauptplayerposition, alle bisherigen Linksklick-/Lupenaktionen.

## 9. Schritt 7: Zweiter libmpv-Player – umgesetzter Vorschlag

Umgesetzt in Build 23; Nachweise und verbleibende persönliche Prüfung im [Prüfbericht](SCHNITTUEBERGAENGE-KONTROLLPLAYER-20261012.md). Die folgenden Absätze dokumentieren den ursprünglichen Entwurf.

Eigenes, nicht modales Kontrollfenster mit eigenem MpvVideoHost, MpvMediaPlayerService und PlaybackViewModel bzw. kleinem ReviewViewModel. Keine externe Playerabhängigkeit.

Ein kleiner, vom Hauptfenster gehaltener Controller verwaltet genau EIN lebendes Kontrollfenster. Beim erneuten Prüfen aktivieren und Datei/Prüfpunkt aktualisieren; nach Schließen später sauber neu erzeugen. Abschlussfenster übergibt eine unveränderliche Zuordnung aus tatsächlichem OutputFilePath und zugehörigen Übergangszeiten. Mehrere Abschlussfenster dürfen nie Zeiten einer anderen Ausgabe verwenden.

„Prüfen“ für jeden Übergang. Start max(0, Übergang − 3 s), auf tatsächlich geladene Dateidauer begrenzt. Erst nach Load/FileLoaded und erfolgreichem Seek automatisch Play. Bedienelemente: Play/Pause, Position/Slider, vorheriger/nächster Schnitt und Schließen. Am ersten/letzten Übergang den jeweils unmöglichen Wechsel deaktivieren. Kein automatisches Stoppen nach kurzem Prüfbereich.

Anfang/Ende bleiben im Abschlussfenster sichtbar; zusätzliche Aktionen für sie sind nicht beauftragt. Bei nur einem Behaltebereich gibt es keine inneren Übergänge; verständlich darstellen und keine ungültige Auswahl erzeugen.

Originalplayer nicht neu laden, nicht seeken und seine Einstellungen nicht überschreiben. Seine laufende Wiedergabe darf normal fortschreiten; das Kontrollfenster setzt keine fremde Position. Kein Zugriff auf Schnitt-/Temporärdateiverwaltung.

Ressourcen: Öffnen erst bei gültigem Video-HWND, seriell initialisieren; eigene Events/Abbruchtoken verwalten. Schließen während Initialisierung, Laden und Seek sicher behandeln, auf DisposeAsync warten und Host/Events freigeben. Beim Beenden von CAN auch das Kontrollfenster kontrolliert schließen. Den ThreadStatic-Handlepfad mit zwei Instanzen ausdrücklich prüfen; daraus bisher weder einen Fehler noch einen erfolgreichen Mehrplayer-Nachweis ableiten.

Tests: getrennte Fake-Playerzustände; Datei→Laden→Seek→Play-Reihenfolge; Wiederverwendung; ältere/neue Abschlussfenster; fehlende/defekte Datei; Begrenzung und Dateidauerabweichungen; kein Übergang; Schließen während asynchroner Arbeit; beide Player gleichzeitig mit echten libmpv-Instanzen, richtige Fenster und Ton, Ressourcenfreigabe. Je ein echter MP4Box-/otr-can-Ausgabefilm zur persönlichen Kontrolle.

Geplante neue Dateien: Dialogs/CutReviewDialog.xaml/.cs, ViewModels/CutReviewViewModel.cs und kleiner Controller; Anpassungen in CutCompletionDialog.xaml/.cs und MainWindow.xaml.cs. Vorhandene Media-Player-Schnittstelle möglichst unverändert wiederverwenden. Erst ein funktionierender kleiner Mehrplayer-Nachweis, dann komfortable Übergangsnavigation.

## 10. Kurztexte und Gesamtabnahme

Bisher ist der otr-can-Kurztext nur eine explizit vom Nutzer anwählbare Vorlage; ApplyQuickText hängt ihn nach Klick an. Er wird nicht automatisch aus der neuen Buttonreihenfolge eingefügt. Dies erhalten. Vor einer gesonderten Textüberarbeitung konkrete Anforderungen sammeln; eine Motorangabe darf nicht aus einer geplanten Motorwahl als erfolgreicher Schnitt abgeleitet werden. Vorhandene persönliche Texte nicht überschreiben.

Abnahme der gesamten Runde:

- Selbst setzen → direkt mit frei gewähltem Motor schneiden, ohne Cutlist speichern zu müssen.
- Cutlist ohne Filmschnitt automatisch speichern; später laden und weiterarbeiten; vorhandener Upload.
- Manueller Filmname, gespeicherte Maske und ursprünglicher Cutlist-Dateiname bleiben unabhängig.
- Neue Masken, alte Masken, Ghostbusters-Datum und Filmfall.
- Beide Motoren verwenden dieselben Namen-/Zielregeln und denselben Schnittplan.
- Timeline/Tabelle, Linksklick/Rechtsklick, Zeitkorrektur und Lupe zusammen.
- Fertige Übergänge im unabhängigen Kontrollfenster; Hauptplayer und Schnittliste erhalten.
- Kleine Fenster und wiederholtes Öffnen/Schließen.
- Fehler und Abbrüche beschädigen keine Originale oder vorhandenen Ausgaben.

Keine Motoränderung, kein Cutlist-Formatwechsel und keine Keep-Berechnungsänderung geplant. Die bekannte AVI-/PTS-Grenze bleibt gesondert dokumentiert; diese UI-Runde ist keine Zeitstempelreparatur.

## 11. Erhaltung und Nachweise

Git-Status zu Beginn: ausschließlich die vier vorbestehenden geänderten Produktions-packages.lock.json. Alle acht lokalen Lockdateien vor/nach diesem Planungsschritt per SHA-256 abgleichen. Keine Restaurierung, Bereinigung, Paketänderung, Nutzer-Einstellungsänderung, Commit oder Push in diesem Schritt.

Analyse basiert auf gelesenem lokalem Code. Neue Verhaltensvorschläge, Mehrplayerfähigkeit und Grenzfallregeln sind damit noch nicht implementiert oder praktisch bewiesen. Bestehender Prüfstand der veröffentlichten Ausgabe bleibt 793 CAN-/44 Rust-Tests; keine neuen Tests oder Builds für die reine Planung ausweisen.
