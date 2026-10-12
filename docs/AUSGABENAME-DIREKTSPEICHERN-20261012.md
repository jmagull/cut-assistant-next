# Schritt 2 – Freier Filmname und direktes Ausgabeziel

Stand: 12.10.2026. Auf ausdrücklichen Auftrag „2) go“ umgesetzt. Lokaler Entwicklungsstand **0.2.1 · Build 18 · RC3**; öffentliche Setup-/Portable-Pakete bleiben Build 16 RC3.

## Ergebnis

Im gemeinsamen Schneidedialog für MP4Box und otr-can ist der komplette Ausgabedateiname editierbar. Film-/Serientitel, S04E01/4x01, Datum und zulässige Trennzeichen lassen sich direkt ändern. Hilfetext exakt: **Du kannst den gesamten Dateinamen abändern.**

Der zusätzliche Film-Dateispeicherdialog entfällt. CAN verwendet ausschließlich **Einstellungen → Standardordner → Geschnittene Videos** und ergänzt .mp4 einmal. Der Name wird nicht stillschweigend bereinigt, gekürzt oder umbenannt. Zulässige innere Leerzeichen bleiben erhalten.

Manuelle Eingaben haben im CutOutputViewModel einen eigenen Wert OutputMovieName. Änderungen der oberen Felder überschreiben ihn nicht. „Aus obigen Eingaben neu erzeugen“ ersetzt ihn bewusst durch den erneut berechneten Vorschlag – auch wenn dieser automatische Vorschlag unverändert geblieben ist. Bereits aus einer Cutlist übernommene Vorschläge dienen weiterhin als Ausgangswert.

Der manuelle Name wird nicht in CutNamingState zurückgeschrieben. Die gespeicherte Standardmaske, Originalidentität und Cutlist-Namensdaten werden durch den manuellen Filmnamen nicht verändert. Bisherige Bearbeitung der oberen Namensdaten und ausdrückliche Vorschlagsneuerzeugung bleiben erhalten.

## Ziel- und Konfliktprüfung

- Ungültige Zeichen, Steuerzeichen, Ordnerpfade, leere Namen, unzulässige Schlusszeichen, Windows-Gerätenamen und zu lange Dateinamenskomponenten vor Schnittbeginn zurückweisen.
- Eine bereits eingegebene .mp4-Endung nicht verdoppeln; gültige Schreibweise erhalten.
- Ausgabeordner muss vollständig konfiguriert, vorhanden und nutzbar sein. Kein Fallback und kein automatisches Anlegen eines fehlenden Ordners.
- Ein eigener, kurzlebiger Testdateihandle prüft den Schreibzugriff. Er verwendet CreateNew und DeleteOnClose und öffnet weder Original noch Ausgabe zum Schreiben. Die Prüfung läuft auf einem Hintergrundthread.
- Originalpfadvergleich vor Schreibprobe und vor Überschreibfrage; Originalschutz im bestehenden CutRequest zusätzlich erhalten.
- Bestehendes Ausgabeziel nur nach eigener Ja/Nein-Frage überschreiben; **Nein ist Vorgabe**. Abbruch lässt den Dialog zur Korrektur offen.
- Exists/OverwriteExistingOutput werden getrennt übergeben. Erscheint ein nicht bestätigtes Ziel zwischen Prüfung und Veröffentlichung, erhalten beide Motorabläufe diese Datei und melden einen Konflikt.
- Ein Zielordner mit dem gewünschten Dateinamen oder eine schreibgeschützte Zieldatei wird zurückgewiesen.
- Während Prüfung/Start bleibt der Dialog gesperrt; nach Ablehnung oder Fehler wieder bedienbar. Der vorhandene Fortschritts-/Abbruchablauf bleibt erhalten.

Schreibproben sind eine Vorprüfung, keine Garantie gegen spätere Rechte-, Platz- oder Netzwerkänderungen. Solche Fehler werden weiterhin vom eigentlichen Schnitt-/Veröffentlichungsablauf gemeldet; keine automatische Ordnerauswahl.

## Geänderte Funktionsdateien

- src/CutAssistantNext.App/Dialogs/CutOutputDialog.xaml: editierbares TwoWay-Feld und Hilfetext.
- src/CutAssistantNext.App/ViewModels/CutOutputViewModel.cs: separater Filmname, ausdrückliche Neuerzeugung.
- src/CutAssistantNext.App/MainWindow.xaml.cs: gemeinsamer direkter Zielpfad, sichere Überschreibentscheidung und Wiederfreigabe des Dialogs.
- src/CutAssistantNext.App/Services/Cutting/OutputFileNameBuilder.cs: zusätzliche strenge Filmnamensprüfung; bisherige sanierende Funktion unverändert.
- src/CutAssistantNext.App/Services/Cutting/MovieOutputTargetResolver.cs: expliziter Zielordner, Original-/Konflikt-/Schreibprüfung.

Die Motorimplementierungen, Cutlist-Erstellung/-Speicherung, Einstellungen, Keep-Berechnung und Originaldateien wurden nicht verändert. Schritt-1-Anordnung erhalten. Kein neues Produktionspaket oder externe Abhängigkeit.

## Prüfungen

**Build 18 RC3: 0 Warnungen/0 Fehler. 860/860 CAN-Tests bestanden**, 0 fehlgeschlagen/übersprungen: Core 74, Cutlists 65, Media 125, App 596.

67 zusätzliche Testfälle:

- Exakte zulässige Namen und Erweiterung; alle wesentlichen ungültigen Windows-Namen/Zeichen, Rand- und Längenfälle.
- Manuelle Änderung einschließlich Datum/Serienschreibweise, leere Zwischenstände beim Tippen, keine automatische Überschreibung, ausdrückliche Neuerzeugung und unveränderter Cutlist-Vorschlag.
- Echte isolierte Dateisystemprüfungen: fehlende/relative/leere Ordner, vorhandene Ausgabe, Originalschutz, Ordnerkonflikt, ReadOnly und verweigerte Windows-Schreibrechte. Probe hinterlässt keine Datei und verändert keine vorhandene Datei.
- Beide tatsächlichen CAN-Motorservices mit simulierten externen Werkzeugen: exakter gewählter Pfad, neues Ziel, abgelehntes Ersetzen, bestätigtes Ersetzen erst nach erfolgreicher Vorbereitung und erst während Verarbeitung entstandene Konflikte. Schnittplan und Original erhalten; Arbeitsdateien aufgeräumt. Kein neuer realer Filmschnitt daraus abgeleitet.

Echter kompilierter WPF-Schneidedialog unter isolierter Fenstereinstellung geprüft: TwoWay-Bindung aktualisiert sofort; Feld editierbar; obere Namensänderung erhält manuelle Eingabe; tatsächlicher Neuerzeugen-Button ersetzt sie; CutRequested erhält den aktuellen Namen; Hilfetext exakt. Normalgröße 720×650 und Mindestgröße 600×520 gerendert und kontrolliert, Buttons und Feld/Hilfe über den vorhandenen Scrollbereich erreichbar, keine Bindungsfehler. Kein Hauptplayer und keine echten Schnittwerkzeuge gestartet; keine persönlichen Einstellungen gespeichert.

Initiale Fehler in neu geschriebenen Testdaten (Escapes und öffentlicher Testparameter mit internem Enum) korrigiert. Die erste ACL-Prüfung scheiterte beim Wiederherstellen aller Sicherheitsabschnitte; Wiederherstellung auf den tatsächlich veränderten DACL-Abschnitt begrenzt. Keine Tests entfernt, deaktiviert oder übersprungen. Abschließender Build und vollständige Testsuite bestanden.

Neue/angepasste C#-Dateien im notwendigen Umfang formatiert. Abhängigkeiten ohne Restore verwendet; Buildnummer einmal auf 18 erhöht. Alle acht lokalen Lockdateihashes erhalten, einschließlich der vier vorbestehenden Produktionsänderungen.

## Nachweise und nächster Schritt

Lokale Belege: .build/output-name-20261012/{BUILD.log,BUILD-final.log,TEST.log,FORMAT.log,LOCKS-BEFORE.json,test-results-final-formatted,WPF.log} und ui-proof/{WPF-PROOF.json,BINDINGS.log,dialog-720.png,dialog-600.png}. Frühere fehlgeschlagene Prüfläufe separat erhalten.

Eine neue persönliche Film-/Bedienabnahme durch den Nutzer ist noch nicht erfolgt. Kein Commit, Push, Setup oder Release. Nächster freizugebender Schritt: **3 – Cutlist automatisch im eingestellten Verzeichnis speichern**.
