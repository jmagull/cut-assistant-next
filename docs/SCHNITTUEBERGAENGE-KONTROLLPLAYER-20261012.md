# Schritt 7 – Schnittübergänge im fertigen Film prüfen

Stand: 12.10.2026. Auftrag: „ok dann 7) los :-)“. Lokaler Entwicklungsstand **0.2.1 · Build 23 · RC3**. Die öffentliche Build-16-Ausgabe ist unverändert.

## Ergebnis und Bedienung

Im Abschlussfenster besitzt jeder innere Übergang einen **Prüfen**-Button. Er öffnet einen unabhängigen, nicht modalen CAN-Kontrollplayer mit der tatsächlichen Ausgabedatei. Derselbe Player wird für weitere Übergänge und andere Abschlussfenster wiederverwendet. Nach dem Schließen kann ein neuer Kontrollplayer geöffnet werden.

Der Start liegt drei Sekunden vor dem errechneten Übergang, begrenzt auf Dateianfang und tatsächliche Filmdauer. CAN wartet auf Laden/FileLoaded und eine positive Dauer; nach erfolgreichem Seek startet die Wiedergabe automatisch. Sie läuft über den Übergang hinaus weiter.

Das Fenster enthält Video, Play/Pause, Positionsregler mit Zeitanzeige, Lautstärke, **Vorheriger Schnitt**, **Nächster Schnitt** und **Schließen**. Am ersten/letzten Übergang wird der unmögliche Wechsel deaktiviert; keine umlaufende Navigation. Die Navigation und der Positionsregler funktionieren auch nach dem natürlichen Filmende. Anfang und Ende bleiben im Abschlussfenster sichtbar; für sie wurden keine zusätzlichen Prüfen-Aktionen ergänzt. Ohne innere Übergänge erscheint ein verständlicher Hinweis.

Originalplayer, Schnittliste und Schnittplan werden nicht geladen, pausiert oder verändert. Bereits laufende Originalwiedergabe läuft normal weiter; zum getrennten Anhören kann der Benutzer sie selbst pausieren. Jeder Abschluss hält seine eigenen unveränderlichen Ausgabezeiten. Neue Abschlüsse werden auch bei gleichem Ausgabepfad neu geladen. Nachträgliche externe Änderungen oder Überschreiben einer Ausgabedatei können deren damaligen Inhalt nicht wiederherstellen.

## Technischer Umfang

- **CutCompletionViewModel**: typisierte Übergangsindizes und unveränderliche Übergangsliste zusätzlich zu Anfang/Ende, angepasster Kontrollhinweis.
- **CutCompletionDialog**: Übergangsaktionen über explizite Übergabe des eigenen Modells; vorhandene Kopier-/Protokoll-/Fensterspeicherfunktionen erhalten.
- **CutReviewController**: ein lebendes Kontrollfenster, Wiederverwendung, Wiederöffnung nach vollständig abgeschlossenem Schließen, Hauptfenster-Lebensdauer.
- **CutReviewWindow**: eigener MpvVideoHost, eigener Player und PlaybackViewModel; HWND-Verfügbarkeit, Bedienung und lokale Fehlermeldungen.
- **CutReviewViewModel**: vom Hauptplayer unabhängige Zustände, seriell ausgeführte Initialisierung/Lade-/Seek-/Play-/Lautstärkevorgänge, ereignisgesteuertes Warten mit 20 Sekunden Grenze und Abbruch.
- **MainWindow**: Anschluss ausschließlich nach erfolgreichem Schnitt am gemeinsamen Abschlussweg beider Motoren; Kontrollplayer vor dem eigenen Player schließen.
- **CutReviewViewModelTests**: 22 neue Testfälle.

Fehlende und defekte Ausgaben führen zu verständlichen Meldungen im Kontrollfenster. Bei einem Ladefehler wird nicht automatisch abgespielt; ein erneuter gültiger Prüfauftrag kann dieselbe Instanz verwenden. Keine Verbindung zur Temporärdateiverwaltung und kein weiterer Schnittlauf.

Beim Schließen werden wartende Vorgänge abgebrochen, laufende Befehle vor dem Dispose beendet, eigene Playback- und Lade-Events abgemeldet und die native Instanz vor der Zerstörung des Video-HWND freigegeben. Auch Schließen vor dem ersten Laden sowie während Initialisierung, Laden und Seek wurde geprüft. Das endgültige WPF-Close erfolgt erst nach Rückkehr aus Closing. CAN beendet ein noch offenes Kontrollfenster mit.

Keine Produktionsabhängigkeit, Einstellungsoption oder Änderung an MP4Box, otr-can, FFmpeg, Codecs und Namensmasken hinzugefügt.

## Nachgewiesene Prüfungen

**Build 23** mit dem Projekt-Buildskript erfolgreich; anschließend Korrekturbauten mit derselben Nummer. **0 Warnungen, 0 Fehler.** Formatierung auf die betroffenen C#-Dateien begrenzt. Finale Lösungstests: **985/985**, 0 übersprungen.

| Projekt | Bestandene Tests |
| --- | ---: |
| Core | 122 |
| Cutlists | 72 |
| Media | 125 |
| App | 666 |

Die 22 neuen Fälle prüfen Ausgabetimes, Begrenzung auf tatsächliche Dauer, beide Reihenfolgen von FileLoaded/Dauer, Datei→Seek→Play, Wiederverwendung/Navigation, ältere/neue Abschlüsse einschließlich gleichem Dateinamen, fehlende Datei, defekte Datei, Timeout, fehlende Übergänge, Zustandsentkopplung, Schließen während Initialisierung/Laden/Seek/Warten und queued Auftrag, wiederholtes Dispose und Navigation/Seek nach EOF.

Vor der Integration wurden **zwei echte libmpv-Kontexte mit unterschiedlichen HWNDs** geprüft: unabhängiges Laden, Seek und Play; Original pausiert bei 1 Sekunde, Kontrollplayer unabhängig bei 4,8 Sekunden. Der ThreadStatic-Handlepfad funktionierte im konkreten Test; keine Änderung am Interop erforderlich.

Die abschließende kompilierte WPF-Prüfung bestand **25 Kontrollen** mit realem Haupt- und Kontrollplayer: sichtbare/verborgene Übergangsbuttons, automatischer Start, nächster Übergang, Play/Pause, Tastatur- und Thumb-Seeking einschließlich Capture-Freigabe, eigene Lautstärke, Fensterwiederverwendung, ältere Abschlusszuordnung, fehlende/defekte Datei und Wiederholung, unveränderte Originalposition/Schnittliste, Schließen/Wiederöffnen, Schließen während realem Laden, Navigation nach EOF, CAN-Ende und Schließen vor erster Initialisierung. Keine WPF-Bindungsfehler.

Verwendet wurde eine selbst erzeugte 12-Sekunden-Datei mit H.264 320×180/25 fps und AAC 48 kHz. Die WPF-Ereignisse wurden automatisiert ausgelöst, ohne den physischen Mauszeiger zu bewegen; native Videohosts waren verborgen. Die Layoutbilder enthalten daher einen schwarzen nativen Videobereich und sind keine fotografische Bild-/Tonabnahme. Persönliche Einstellungen wurden für den Test nicht gespeichert; Fensterstores waren isoliert.

Lokale, ignorierte Nachweise unter **.build/transition-review-20261012**:
- probe: einfacher realer Zweiplayer-Nachweis.
- verified-tests: finaler Lösungstestlauf.
- qa-current/proof.json: finale 25 WPF-/Native-Kontrollen.
- qa-current/completion.png und review.png: Layoutkontrolle.
- preservation.json: acht unveränderte Lockdateien, zwölf unveränderte persönliche Einstellungsdateien und sieben unveränderte Build-16-Releaseartefakte.

Keine Produktions-Restore-/Bereinigungsaktion, kein Commit, Push, Setup-, Portable- oder Releasebau. Die vier vorbestehenden Produktions-Lockänderungen bleiben erhalten.

## Grenzen und nächster Schritt

Die gemeinsame Abschlussintegration ist für beide Motoren vorhanden. Für Schritt 7 wurden keine neuen realen Motor-Schnittaufträge ausgeführt; vorhandene Motortests blieben erfolgreich. Die persönliche Prüfung realer MP4Box-/otr-can-Ausgaben mit dem neuen Fenster steht aus. Insbesondere Bilddarstellung, hörbarer Ton, Synchronität und Übergänge bleiben persönlich abzunehmen. Es gibt weiterhin keine Tonspurauswahl im CAN-Player; zusätzliche Spuren können im externen Player kontrolliert werden.

Berechnete Übergänge sind Orientierungspunkte; kleine Abweichungen des tatsächlichen Schnittergebnisses bleiben möglich. Sehr kurze Filme begrenzen den Start entsprechend. Der bekannte AVI-/Video-PTS-Befund wird durch diese Bedienänderung nicht behoben.

**Schritt 8** bleibt offen: gemeinsam vorgesehene Gesamtabnahme und gegebenenfalls konkret beauftragte Kurztexte. Keine neue automatische Motorangabe und keine Änderung persönlicher Kommentare vorweggenommen.
