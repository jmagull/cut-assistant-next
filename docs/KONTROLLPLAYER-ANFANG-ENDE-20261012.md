# Kontrollplayer – Anfang und Ende prüfen

Stand: 12.10.2026. Lokaler **Build 25 RC3**. Auftrag: Anfang und Ende auch bei nur einem Behalteabschnitt und zusätzlich bei allen anderen Filmen prüfen.

## Verhalten

Im Abschlussfenster besitzen jetzt **alle Kontrollstellen** einen Prüfen-Button:

- **Anfang:** tatsächliche Ausgabedatei laden, bei 00:00:00 starten und automatisch abspielen.
- **Übergang:** wie bisher drei Sekunden vor der berechneten Stelle starten, auf die Datei begrenzt.
- **Ende:** drei Sekunden vor dem **tatsächlichen geladenen Filmende** starten und automatisch abspielen. Die berechnete Plandauer bestimmt diesen Start nicht.
- Ist die Datei kürzer als drei Sekunden, liegt der Start der Endprüfung bei 00:00:00.

Dies funktioniert ohne innere Übergänge ebenso wie bei mehreren Behalteabschnitten, für beide Schnittmotoren. **Vorherige Prüfstelle / Nächste Prüfstelle** navigieren in der Reihenfolge Anfang → Übergänge → Ende, ohne Umlauf. Am Anfang ist Zurück deaktiviert, am Ende Weiter. Der vorhandene Kontrollplayer wird weiter wiederverwendet; Play/Pause, Positionsregler und eigene Lautstärke bleiben erhalten.

Die Zeiten im Abschlussfenster bleiben die unveränderlichen berechneten Planwerte. Der Kontrollplayer zeigt bei Ende die tatsächlich geladene Dauer und weist auf den Start drei Sekunden davor hin. Originalplayer, Schnittplan, Cutlist und Ausgabe werden durch die Kontrolle nicht verändert.

## Geänderte Funktionsdateien

- ViewModels/CutCompletionViewModel.cs: typisierte Kontrollstellen für Anfang/Übergang/Ende und eindeutiger Index über die gesamte Liste; bisherige Übergangsindizes für deren Nummerierung erhalten.
- Dialogs/CutCompletionDialog.xaml/.cs: Prüfen bei jeder Stelle; Übergabe des Kontrollstellenindex.
- ViewModels/CutReviewViewModel.cs: Anfang bei null, Ende aus tatsächlicher Dauer, Navigation über alle Kontrollstellen; bekannte Lade-/Abbruch-/Dispose-/EOF-Behandlung erhalten.
- Dialogs/CutReviewWindow.xaml: Titel Geschnittenen Film prüfen und Beschriftung der erweiterten Navigation.
- Tests/ViewModels/CutReviewViewModelTests.cs: bisherige Navigationstests an den erweiterten Umfang angepasst, früheren Ein-Bereich-Ablehnungstest durch Anfang-/Ende-Prüfung ersetzt, zwölf zusätzliche Fälle.

Keine Änderung an Motoren, Medienanalyse, Codecs, Einstellungen, Namensmasken oder Frame-Lupe. Keine Produktionsabhängigkeit hinzugefügt.

## Prüfung

**Build 25**, 0 Warnungen und 0 Fehler. **999/999 CAN-Tests bestanden**, 0 übersprungen: Core 122, Cutlists 72, Media 125, App 680. Geänderte C#-Dateien begrenzt formatiert. Kein Test entfernt oder deaktiviert; bestehende Erwartungen für die jetzt ausdrücklich beauftragte Anfang-/Ende-Navigation angepasst.

Neue Tests prüfen Anfang und Ende mit/ohne Übergänge, Plandauer versus tatsächliche Dauer, sehr kurze Filme, Start bei null, automatisches Play nach Seek, Indizes und ungültige Kontrollstellen. Bestehende Fehler-, Abbruch-, Wiederverwendungs- und EOF-Tests bleiben erfolgreich.

Kompilierter Abschlussdialog und unabhängiger realer libmpv-Player: **36 WPF-/Native-Kontrollen bestanden**, ohne Bindungsfehler. Prüfen-Buttons für Anfang/Ende/Übergänge, Ein-Bereich-Dialog, automatische Wiedergabe, vollständige Navigation, tatsächlich 12 Sekunden statt nominal 90 Sekunden beim Endziel sowie eine 2-Sekunden-Datei geprüft. Auch Fehler, Wiederöffnung, Schließen während Laden und unveränderter Hauptplayer/Schnittplan erneut geprüft. Mindestgrößen gerendert und kontrolliert; Abschlussinhalt bleibt scrollbar.

Automatisierte WPF-Ereignisse, verborgene Videohosts und erzeugte Testmedien; kein physischer Mauszeiger benutzt. Dies ersetzt keine persönliche Bild-/Tonabnahme. Anschließend hat der Nutzer die Anfang-/Ende-Funktion von Build 25 positiv bestätigt, siehe unten.

Lokale Nachweise unter **.build/start-end-review-20261012**: tests/*.trx, qa/proof.json, qa/*minimum.png, PRESERVATION.json. Acht Lockdateien einschließlich der vier vorbestehenden Produktionsänderungen, zwölf persönliche Einstellungen und sieben öffentliche Build-16-Artefakte per Hash unverändert. HEAD unverändert. Keine Produktions-Restore-/Bereinigung, kein Commit, Push, Setup, Portable-Paket oder Release.

## Persönliche Abnahme von Build 25

Der Nutzer bestätigt **„prüfen: Anfang und Ende super :-)“**. Die neue Funktion ist damit persönlich positiv geprüft.

Zusätzlich übermittelt er zwei Bilder aus dem Kontrollplayer für Kimi [04.10.2026], ausdrücklich **Bild 1 MP4Box, Bild 2 otr-can**, und bestätigt, dass die Schnittunterschiede gut auffallen. Beide zeigen den berechneten Übergang 00:45:25.640: in der MP4Box-Aufnahme ist eine SRF-zwei-Senderblende sichtbar, in der otr-can-Aufnahme die Filmszene. Die angezeigten aktuellen Playerpositionen unterscheiden sich (45:26 beziehungsweise 45:25); die Bilder sind ein persönlicher Anschauungsvergleich, kein automatischer Abgleich identischer Frames oder Nachweis identischer Cutlists. Keine zusätzliche quantitative Ton-/Synchronitätsmessung daraus abgeleitet.

Nur Dokumentation aktualisiert; kein neuer Build, Commit, Push oder Paketbau.

## Persönliche Rückmeldungen zu Build 24

Der Nutzer bestätigte vor dieser Erweiterung:

- Cutlist direkt ohne zusätzlichen Speicherdialog gespeichert.
- Namensmaske im MP4Box-Schneidedialog korrekt übernommen; manuelles Ändern des gesamten Ausgabenamens funktioniert.
- Film direkt im konfigurierten Standardordner gespeichert.
- Ghostbusters erfolgreich mit MP4Box und otr-can geschnitten; Bild und Ton beider Ausgaben persönlich einwandfrei.
- Teufelsbrigade: Kontrollplayer und Übergangsprüfung funktionieren; auch Original-AVI mit MP4Box sowie otr-can positiv geprüft.
- Zuletzt auch im Cutlist-Erstellungsdialog die x-Maske korrekt übernommen. Den ersten Eindruck einer fehlenden Maskenübernahme ausdrücklich korrigiert; kein bestätigter allgemeiner Maskenfehler.
- Abschließend die umgesetzten Bedienänderungen insgesamt als funktionierend bestätigt.

**Offen bleibt separat die Frame-Lupe bei der Original-AVI:** fehlende Bildvorschau, Dialog meldet fehlende Original-PTS, Übernahme gesperrt. Die neue Anfang-/Ende-Prüfung betrifft die fertige Ausgabe und behebt diese Original-AVI-Grenze nicht.

Der künstliche MP4Box-Zwei-AAC-Befund aus der technischen Prüfrunde bleibt als separater Testbefund dokumentiert. Radical und die neuen persönlichen MP4Box-/AVI-Erfolge gelten weiter; daraus ist kein Rückschritt des bewährten Arbeitsablaufs nachgewiesen.
