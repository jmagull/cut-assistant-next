# Schritt 8 – Gesamtabnahme der Bedienungsrunde

Stand: 12.10.2026. Auftrag: „8) go“. Lokaler Entwicklungsstand **0.2.1 · Build 24 · RC3**.

**Die technische Prüfrunde ist durchgeführt. Die neuen Bedienabläufe sind geprüft; eine uneingeschränkte Gesamt- oder Releasefreigabe wird wegen eines reproduzierten MP4Box-Befunds und der noch offenen persönlichen Abnahme nicht behauptet.**

## Umfang und Änderungen

Die Schritte 1–7 wurden zusammen geprüft: Schaltflächenreihenfolge, freier Filmname, direkte Ausgabe, automatisches Cutlist-Speichern, alte/neue Namensmasken, Tabellen-/Timeline-Auswahl, Rechtsklick-Kontrolle und unabhängiger Kontrollplayer.

Zwei neue Verbundtests in **tests/CutAssistantNext.App.Tests/Services/NamingCutlistWorkflowTests.cs** sichern den Zusammenhang zwischen alter/neuer Maske, manuellem Filmnamen, direktem Schnittauftrag ohne gespeicherte Cutlist, Cutlist-Dateiname/Originalzuordnung, Speichern und Wiederladen, identischen Behaltebereichen und berechneten Ausgabeübergängen. Keine Produktionsfunktion für Schritt 8 verändert.

Neue Kurztexte waren nicht spezifiziert. Die vorhandenen fünf persönlichen Vorlagen und der separat konfigurierbare sechste otr-can-Text bleiben erhalten. Ein Motor-Kurztext wird weiterhin ausschließlich nach bewusstem Klick angehängt; Motorwahl oder Schnittauftrag fügen nichts automatisch ein. Vorhandene Tests prüfen Laden alter Einstellungen, eigene/geleerte sechste Texte sowie ausdrückliches Anwählen in beiden Erstellungswegen. CutlistGenerationDialog und ApplyQuickText wurden zusätzlich gelesen; kein Text oder Autor gespeichert oder verändert. Der vorhandene allgemeine CAN-Kommentar bleibt bestehen.

README, Nutzeranleitung, Status, Tagebuch und Entwicklungsplan wurden auf Prüfstand und Grenzen aktualisiert. Kein Motorumbau, keine Änderung der Keep-Berechnung oder des Cutlist-Formats, keine zusätzlichen Produktionsabhängigkeiten.

## Build und Lösungstests

Projekt-Buildskript erfolgreich: **Build 24**, **0 Warnungen, 0 Fehler**. Korrekturbau mit derselben Nummer. Neue Testdatei begrenzt formatiert.

**987/987 CAN-Tests bestanden**, 0 übersprungen: Core 122, Cutlists 72, Media 125, App 668. Keine Tests entfernt oder deaktiviert. Der anfängliche Vergleich zweier KeepSegment-Listen in den neuen Tests verglich Objektreferenzen; auf den unveränderten Start-/Dauer-Wertvergleich korrigiert. Die sichtbaren Schnittwerte waren bereits identisch.

Die gesamte Testsuite umfasst weiterhin Namens-/Ordnerfehler, bestehende und spät entstandene Zielkonflikte, bestätigtes Ersetzen, Abbrüche und Fehler, Cutlist-IO, Schnittzeiten, Zeitkorrektur und Lupenfachlogik. Uploadtests verwenden simulierte Gegenstellen; keine Test-Cutlist wurde an einen Server geschickt.

## Reale Werkzeuge mit erzeugten Medien

FFmpeg erzeugte drei kleine Quellen mit H.264 320×180 und 25 fps: MP4 mit zwei AAC-Stereo-Spuren, MP4 mit AAC/AC3 und AVI mit MP3-Stereo. Keine reale Benutzeraufnahme wurde verarbeitet oder neu kodiert. Die AVI für die endgültige Matrix verwendet übliche H.264-B-Frames.

Alle Aufträge erhielten denselben Plan: entfernt 5–8, 12–15 und 18 Sekunden bis Dateiende; behalten 0–5, 8–12 und 15–18. Nominale Ausgabe 12 Sekunden, Übergänge bei 5 und 9 Sekunden. Einheitlicher manuell gewählter Basisname:

    Ghostbusters - 1x01 - Hexenprobleme [19.09.2026]

Nur die isolierten konfigurierten Ausgabeordner unterscheiden die Aufträge. Reale CAN-Resolver, CutRequestFactory, VideoPreparation und konfigurierte Motorservices wurden verwendet; kein Ersatzordner und kein Dateispeicherdialog. Jede erfolgreich erzeugte Ausgabe wurde anschließend mit FFmpeg einschließlich sämtlicher Tonspuren streng dekodiert.

| Quelle / Auftrag | MP4Box | otr-can 0.1.5 |
| --- | --- | --- |
| MP4, zwei gleichartige AAC-Stereo-Spuren, direkt | **Fehler beim Zusammenfügen** | Erfolgreich, 2 AAC-Spuren, 12 s |
| MP4, AAC/AC3, direkt | Erfolgreich, AAC/AC3, 12,002667 s | Erfolgreich, AAC/AC3, 12 s |
| AVI, H.264 mit B-Frames / MP3, direkt | Erfolgreich, MP3, 12 s | Erfolgreich, AAC, 12 s |
| MP4, zwei AAC-Spuren, gespeicherte Cutlist wieder geladen | Nicht erneut ausgeführt | Erfolgreich, 2 AAC-Spuren, 12 s |
| MP4, AAC/AC3, gespeicherte Cutlist wieder geladen | Erfolgreich, AAC/AC3, 12,002667 s | Nicht erneut ausgeführt |
| AVI/MP3, gespeicherte Cutlist wieder geladen | Nicht erneut ausgeführt | Erfolgreich, AAC, 12 s |

**Acht erfolgreiche Aufträge und ein fehlgeschlagener Auftrag.** Die Matrix ist ausdrücklich keine vollständig grüne Werkzeugabnahme.

Bei den erfolgreichen Aufträgen: exakter Filmname, unveränderte Behaltebereiche vor/nach Vorbereitung, erwartete Streamzahl und Audioformate, vollständige strenge Dekodierung, unveränderte Quelle und aufgeräumte Arbeitsdateien. Ein anschließender unbestätigter Zielkonflikt wurde von beiden Services abgewiesen und die bestehende Ausgabe per Hash erhalten.

Cutlists separat unter dem vollständigen Originaldateinamen plus .cutlist gespeichert, Konflikt ohne Zustimmung abgewiesen, Inhalte wieder eingelesen und Schnittwerte unverändert weiterverwendet. Der manuelle Filmname ersetzt weder Originalzuordnung noch Cutlist-Namen.

### Offener MP4Box-Befund

Die neu erzeugte Datei mit zwei AAC-Stereo-Spuren führt bei der installierten **GPAC 26.07-rev0-ga07cbfff-master** zum Abbruch des Zusammensetzens: **[BS] Attempt to overread bitstream**, **I/O Error**, Exit-Code 1.

Der Fehler wurde zusätzlich mit direkten MP4Box-Aufrufen für drei Teilstücke und deren Zusammenfügen reproduziert, ohne CAN-Code oder Oberfläche. Quelle und Teilstücke lassen sich lesen; strenge Quelldekodierung und einfaches Umpacken funktionieren. Der Kontrollversuch mit nur einer AAC-Spur funktioniert; die AAC/AC3-Matrix funktioniert ebenfalls. Dies grenzt den Befund ein, beweist aber noch keine allgemeine Ursache oder dass alle Dateien mit zwei AAC-Spuren betroffen wären.

CAN veröffentlichte nach dem fehlgeschlagenen Auftrag keine fertige Zieldatei und räumte seine Arbeitsdateien auf. Die Quelle blieb unverändert. Mp4BoxRunner und Mp4BoxCutWorkflow wurden in dieser Bedienrunde nicht geändert. Der Befund ist deshalb separat vom neuen Bedienverhalten zu klären; keine stille Motoränderung oder automatische Umwandlung zusätzlicher Tonspuren vorgenommen.

### Vorlauf und Grenzen der Testdateien

Eine zuerst erzeugte AVI ohne H.264-B-Frames wurde nach Vorbereitung von otr-can mit **Experiment supports 25/50 fps CFR only** abgewiesen. MP4Box verarbeitete diese Variante, mit einer gemessenen Ausgabe von 11,905 s. Dieser negative Vorlauf bleibt dokumentiert; aus der nominellen ffprobe-Bildrate allein folgt keine otr-can-Eignung. Die genaue Zeitstempel-/FFMS2-Ursache wurde hier nicht repariert.

Die endgültige H.264-B-Frame-AVI wurde neu erzeugt, ohne den fehlgeschlagenen Vorlauf zu löschen, und mit beiden Motoren erfolgreich verarbeitet. Im Prüfhelfer wurde außerdem die falsche Annahme korrigiert, otr-can wandle auch vorhandenes AC3 nach AAC um: AAC und AC3 behalten ihre bisherigen Zielcodecs; MP3 wird nach AAC kodiert.

Der bekannte Befund historischer AVI-Video-PTS bleibt unabhängig davon offen. Die positiven Ergebnisse sind synthetische technische Tests, keine neue persönliche Filmabnahme.

## WPF und fertige Ausgaben

Die bereits vorhandenen Prüfhilfen wurden gegen die neu gebauten Build-24-Assemblies erneut ausgeführt:

- CutOutputDialog: vollständige Namenseingabe, Erhaltung manueller Änderungen, ausdrückliche Neuerzeugung, CutRequested, Normal-/Mindestgröße.
- CutlistGenerationDialog: bewertetes Speichern, konfiguriertes Ziel, bytegleiche Cutlist, Normal-/Mindestgröße.
- Namensmasken-Editor: Einzelbausteine, alte Masken, Ghostbusters-Beispiel und Datum 19.09.2026, Drag/Umordnen/Entfernen, Zurücksetzen, Speichern/Verwerfen.
- Kompilierte Tabelle/Timeline: Auswahl/Abwahl, linke Grenzsprünge, Rechtsklick −3/+3, kurze Filmabschnitte, Textfokus und Tastaturnavigation; Zeiten und Lupenziel erhalten.
- Kontrollplayer: erneut **25 WPF-/Native-Kontrollen** bestanden, einschließlich Fehlersituationen, Schließen während Laden, Wiederöffnung und Navigation nach Filmende.

Zusätzlich wurden **alle acht tatsächlich mit den Motoren erzeugten Ausgaben** in derselben unabhängigen Kontrollinstanz geladen und ihre beiden Übergänge geprüft: automatischer Start bei 2 beziehungsweise 6 Sekunden, richtige Datei, Navigationsgrenzen und keine Fehlermeldung. Die acht Schaltflächen des Hauptfensters haben die verlangte Reihenfolge. Bei Mindestgröße bleiben die Kontrollplayer-Buttons erreichbar.

Der echte Originalplayer blieb pausiert an seiner vorherigen Position; ausgewählter Bereich, Startkante für die Lupe und Schnittwerte blieben erhalten. Beim Beenden von CAN wurde der Kontrollplayer freigegeben. Anschließend konnten alle Ausgaben exklusiv zum Lesen geöffnet werden, ohne verbliebene Player-Dateihandles.

Automatisierte WPF-Ereignisse und verborgene Videohosts, kein physischer Mauszeiger benutzt. Die Prüfungen ersetzen weder einen sichtbaren Bedienlauf noch persönliche Bild-, Hör- oder Tonsynchronitätskontrolle.

## Erhaltung und lokale Nachweise

Nachweise ausschließlich unter **.build/final-workflow-20261012**, ignoriert:

- tests-final: finaler Lösungstestlauf.
- engine-final/RESULT.json: acht erfolgreiche Fälle, ein ausdrücklich ausgewiesener Fehler, Quellen-/Ausgabehashes und Cleanup.
- engine-final/*-failure.log sowie gpac-diag/*.log: MP4Box-Fehler und direkte Gegenversuche.
- engine-verified/*-failure.log: negative AVI-/CFR-Prüfung und ursprünglicher Prüfhelfer-Codecvergleich.
- ui-output, ui-cutlist, ui-naming, ui-selection-edge, ui-review: erneute Einzelprüfungen.
- review-actual/RESULT.json: Kontrollplayer mit acht realen Motorausgaben.
- PRESERVATION.json: **8 Lockdateien, 12 persönliche Einstellungsdateien, 7 öffentliche Build-16-Artefakte und 3 erzeugte Quellen unverändert**.

Die vier vorbestehenden Produktions-Lockänderungen wurden ausdrücklich erhalten. Rust-/Motorquellen nicht geändert; installierte otr-can-EXE weiterhin SHA-256 FC6ED6568880A995F3D141A9378AA2CE28ADE8DC0E257E93DC972799BB1324A6. HEAD bleibt a3b586af871ce867fd999f0cf5804596dbb51267.

Keine Produktions-Restore-/Bereinigungsaktion, kein Commit, Push, Setup, Portable-Paket oder Release. Wiederherstellung nur der neuen lokalen Prüfprojekte mit bereits vorhandenen Referenzen, keine Produktionsabhängigkeit hinzugefügt.

## Noch erforderliche persönliche Abnahme

1. Originalaufnahme laden und Bereiche setzen; erneuter Tabellen-/Timeline-Klick wählt ab, Anfang/Ende springen weiterhin.
2. Rechtsklick vor/nach einem Schnitt prüfen, anschließend vorhandene Frame-Lupe und Zeitkorrektur benutzen.
3. Vollständigen Filmnamen samt Datum/Serienschreibweise ändern; direkt in den konfigurierten Ordner schneiden.
4. Cutlist auch ohne Filmschnitt speichern; Originalname plus .cutlist und richtigen Ordner kontrollieren; später laden.
5. Alte eigene Maske erhalten; neue Standardmaske bei Bedarf ausdrücklich wählen; Vorschau/Datum prüfen.
6. Je eine geeignete reale MP4Box- und otr-can-Ausgabe im Kontrollfenster ansehen und anhören: Anfang, Ende, sämtliche Übergänge, Bild, Ton und Synchronität.
7. Älteres Abschlussfenster wieder verwenden, Fenster verkleinern und mehrmals öffnen/schließen.
8. Vorhandene Mehrspurdatei zusätzlich im externen Player je Tonspur kontrollieren.

Vor einem neuen Paket/Release den beschriebenen MP4Box-Befund entscheiden und die persönliche Abnahme dokumentieren. Für eine gezielte neue Kurztextüberarbeitung erst konkrete Texte festlegen.
