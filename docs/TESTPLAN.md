# Testplan – aktueller Funktionsstand

## Testumgebung

- Windows 11 x64
- Release-Build
- repräsentative MP4- und OTR-Dateien, einschließlich MP4-Inhalt mit Dateiendung `.avi`
- mpv/libmpv und ffprobe aus der vorgesehenen Laufzeitstruktur

## Manuelle Kernprüfungen

| Nr. | Prüfung | Erwartetes Ergebnis |
|---|---|---|
| 1 | Anwendung starten | Hauptfenster erscheint ohne Fehler |
| 2 | MP4 öffnen | Video wird im Hauptfenster geladen |
| 3 | Play/Pause | Wiedergabe reagiert zuverlässig |
| 4 | Zeitleiste anklicken und Regler ziehen | Klicks links/mittig/rechts auf Slider-Spur und graue/rote Schnitt-Timeline setzen Position und Slider auf die gewählte Stelle; rote Auswahl bleibt umschaltbar; Reglerziehen funktioniert weiter |
| 5 | Frame vorwärts | einzelner Bildschritt funktioniert |
| 6 | Frame rückwärts | stabiler Rückwärtsschritt |
| 7 | Lautstärke ändern | Ton wird angepasst |
| 8 | Medieninformationen | Dauer, Auflösung und Codecs erscheinen |
| 9 | Datei wechseln | Ressourcen werden sauber freigegeben |
| 10 | Fehlerfall | verständliche Meldung und Logeintrag |
| 11 | Tastatursteuerung | Play/Pause und Frame-Schritte reagieren zuverlässig |
| 12 | Schnittbereich setzen | Anfang und Ende werden korrekt übernommen |
| 13 | Mehrere Bereiche | Bereiche werden chronologisch verwaltet |
| 14 | Timeline-Auswahl | Tabelle und Timeline bleiben synchron |
| 15 | Bereich korrigieren | Anfang und Ende lassen sich korrigieren |
| 16 | Bereich löschen | ausgewählter Bereich wird entfernt |
| 17 | Fenster neu öffnen | Größe bzw. Maximierung wird wiederhergestellt |

## Getrennte Standardordner für Videos und Cutlists

- Unter **Einstellungen → Standardordner …** verschiedene vorhandene Ordner für Originalvideos, geschnittene Videos und eigene Cutlists wählen, speichern und CAN neu starten. Alle drei Werte bleiben erhalten.
- **Videodatei laden** beginnt im Originalordner; **Geschnittene Datei speichern** beginnt im Ausgabeordner und behält die Namensvorschau. Eine manuelle abweichende Auswahl ersetzt keine Standardordner-Einstellung.
- **Lokale Cutlist laden** und **Cutlist speichern** starten im Cutlist-Standardordner. Ohne Vorgabe oder bei nicht verfügbarem Ordner: Öffnen mit Windows-Ordnerwahl, Speichern im Ordner des Originalvideos. Der vorgeschlagene Cutlist-Dateiname bleibt erhalten.
- Nur einen Ordner festlegen, alle Felder leeren oder denselben Ordner für mehrere Zwecke verwenden: unabhängig möglich. Bestehende Konfiguration mit nur zwei Videoordnern bleibt gültig.
- Ungültiger, relativer oder nicht vorhandener Pfad: verständliche Meldung, Dialog bleibt offen. Ein nach dem Speichern entfernter Ordner oder nicht angeschlossenes Laufwerk verhindert die Dateiauswahl nicht; gespeicherte Vorgabe bleibt erhalten.
- **×** leert nur das zugehörige Feld. **Abbrechen** und Fensterkreuz speichern keine Änderungen. Speicherfehler sichtbar; erneuter Versuch möglich.
- Kleine Fenster, lange Pfade und Ordnerauswahl prüfen: Inhalt scrollbar und Speichern/Abbrechen erreichbar. Werkzeugdialoge behalten ihre bisherigen Startordner.

## Abschlussfenster nach dem Schneiden

- Erfolgreicher Schnitt: Nach Erstellung der Ausgabe und Aufräumen erscheint **Schneiden abgeschlossen**, ohne Countdown und ohne automatischen Playerstart.
- Entfernte Vor-/Nachläufe und mehrere Werbepausen: Kontrollzeiten beginnen bei 0 und summieren nur die behaltenen Abschnitte. Beispiel: 582,240–1902,960 s und 2551,320–3645,400 s ergeben einen Übergang bei **00:22:00.720** und ein Ende bei **00:40:14.800**.
- **Hinweise kopieren**: Dateipfad, sämtliche Kontrollstellen, Zeitbezug und Kontrollhinweis sind vollständig in der Zwischenablage; sichtbare Rückmeldung. Bei belegter Zwischenablage kann erneut kopiert werden.
- Original und Schnittliste bleiben geöffnet und bearbeitbar. Spätere Korrekturen verändern die Hinweise zum abgeschlossenen Schnitt nicht.
- Viele Übergänge, langer Dateiname und kleines Fenster: Inhalt scrollbar, beide Buttons erreichbar; Protokoll bei Bedarf aufklappbar und mit Strg+C kopierbar.
- Schließen per Button, Escape oder Fensterkreuz; kein automatisches Schließen.
- Abbruch oder Fehler: Fortschrittsfenster mit bisheriger Fehlermeldung, kein Erfolgsfenster.

## Automatisierte Tests

- Video-/Cutlist-Standardordner: unabhängiges Speichern/Laden, Neustart, leere oder beschädigte Einstellungen, fehlende Ordner, relative/ungültige Pfade, Dateien statt Ordnern, Speicherfehler und unveränderte Einstellungen beim Bearbeiten
- ffprobe-Parsing, Medienanalyse und Fehlerabbildung
- Frame-Lupe: Kantenauswahl, Halbierungsfolge, Intervallerweiterung anhand echter Frameanzahl, Dateigrenzen, fehlende/mehrdeutige PTS, Vorschaufehler und Abbruch
- Frame-Lupe-Übernahme: Original-PTS ohne impliziten Frame-/Keyframeversatz, Containerstart bei 0/+2/−2 s, variable Zeitabstände, gesperrter Button während Laden/nach Vorschaufehler; nur die gewählte Kante ändern; unveränderte/ungültige/veraltete Kanten, Überschneidungen und Export der korrigierten Keep-Bereiche
- PTS-Vorschau: absolute Quellzeitstempel, Zeitbasis-Prüfung und Ablehnung mehrdeutiger Bildausgabe
- Startwert der Frame-Lupe: Default 2000, Speichern/Laden, ungültige Werte und Speicherfehler
- `MainWindowViewModel` einschließlich Übernahme der Mediendauer
- `PlaybackViewModel` einschließlich Play/Pause, Seeking, Frames und Lautstärke
- `MpvMediaPlayerService` einschließlich Pausenzustand während des Ladens
- `RemoveSegment` einschließlich Bereichsvalidierung
- `CutPlan` einschließlich Sortierung, Überschneidungsschutz, Ersetzen und Löschen
- `CutPlanViewModel` einschließlich Erfassung, Auswahl und Korrekturmodus
- `CutCompletionViewModel`: Zeiten im geschnittenen Film, kumulierte Behaltezeiten, angrenzende Entfernbereiche, vollständiger Kopiertext, unabhängige Ergebnismomentaufnahme, Stunden über 24 und Millisekunden
- UTF-8-Protokollierung, Größenbegrenzung und Rotation
- `NameTemplateRenderer` einschließlich Namensvariablen, optionaler Präfixe, fehlender Werte und unbekannter Variablen
- `CutlistSettingsStore` einschließlich Standardwerten, Standardautor, Schnelltexten, Laden und Speichern sowie UTF-8 ohne BOM
- `TechnicalNoticeDetector` einschließlich des Sonderfalls `.avi` mit tatsächlich erkanntem MP4/ISO-BMFF-Container
- `CutlistKeepSegmentBuilder` einschließlich der Umrechnung von Remove- in Keep-Bereiche und der Behandlung von Randfällen
- Cutlist-Metadaten einschließlich `NoOfCuts`, Autor, Benutzerkommentar und technischer Hinweise
- `CutlistDocument` einschließlich Konsistenzprüfung zwischen Metadaten und Keep-Bereichen
- `CutlistSerializer` einschließlich klassischer Kompatibilitätszeilen, kulturunabhängiger Zahlenwerte und Golden-Master-Test
- `CutlistFileWriter` einschließlich UTF-8 ohne BOM, CRLF-Zeilenenden und Umlauten
- `CutlistParser` einschließlich des Einlesens klassischer Cutlist-Strukturen
- `CutlistFileReader` einschließlich UTF-8 und historischem Windows-1252-/ANSI-Fallback
- `CutlistGenerationViewModel` einschließlich Herkunftsübernahme, Duplikatschutz, fehlendem beziehungsweise eigenem Autor und Erhalt vorhandener Benutzerkommentare

Aktueller vollständiger Testlauf vom 19.09.2026: **450 von 450 Tests erfolgreich** (Debug-Build). Ein erneuter Release-Testlauf mit diesem Stand steht noch aus.

Zusätzlich wurde die lokale Cutlist-Dateiausgabe in einem Smoke-Test praktisch geprüft. Dabei wurde eine vollständige `.cutlist`-Datei erzeugt und anschließend explizit als UTF-8 eingelesen; auch Umlaute wurden korrekt erhalten.

Cutlist-Reader und Parser sind umgesetzt und automatisiert getestet. Beim Einlesen wird zunächst UTF-8 verwendet; historische Windows-1252-/ANSI-Cutlists werden als Kompatibilitätsfall unterstützt.

## P1 – Cutlist-Bedienung, praktisch abgenommen am 19.09.2026

| Prüfung | Ergebnis |
|---|---|
| Fremde Namensvorschau übernehmen | Ursprünglicher Vorschlag bleibt beim Ändern der Namensfelder erhalten |
| „Aus obigen Eingaben neu erzeugen“ | Bewusste Umschaltung auf die eigene Namensmaske funktioniert; danach aktualisiert sich die Vorschau |
| Herkunftshinweis bei Fremdautor | Wird automatisch ergänzt; eigener Autor bleibt erhalten |
| Wiederholtes Öffnen und fehlender Autor | Keine doppelten Hinweise und keine leeren Herkunftsangaben |
| Zeitdarstellung | Anfang, Ende und Dauer einheitlich mit drei Nachkommastellen |
| Namensvorschau kopieren | Markierung und Strg+C praktisch geprüft; Vorschau aktualisiert sich weiterhin bei Änderungen |

## Integrierter AVI-Ablauf und UI – Stand 19.09.2026

| Prüfung | Ergebnis / Stand |
|---|---|
| Diplomatin AVI, zwei Cutlists, Vorbereitung bis MP4-Ausgabe | Live erfolgreich; Wiedergabe, Anfang/Ende und Ton-Synchronität bestätigt |
| Rubikon AVI, zwei Cutlists, Vorbereitung bis MP4-Ausgabe | Live erfolgreich; Wiedergabe und Ton-Synchronität bestätigt |
| Diplomatin HD-AVI mit HQ-Cutlist | Dateigrößenwarnung bewusst akzeptiert; Schnitt erfolgreich |
| Zwei Frames Unterschied bei Diplomatin | Akzeptiert und protokolliert; 20-ms-Containerverlängerung als Regression abgesichert |
| Fehlender ffprobe-Pfad | Konkreter Fehlergrund direkt im Fortschrittsfenster live bestätigt; ursprünglichen Pfad nach dem Test wiederherstellen |
| Server-Cutlist erneut auswählen | Ohne erneutes Videoladen live bestätigt |
| Schmales/kurzes Fenster | Umbruch, Scrollen, Videobegrenzung und sichtbare Zeitangaben live bestätigt |
| Fenstergröße ohne Videoladen speichern | Live bestätigt |
| Leerer ColdCut-Endmarker | Import-/Schnittplan-/Roundtrip-Tests mit Diplomatin-Werten bestanden; separater Live-Nachweis des konkreten Originaleintrags nicht festgehalten |
| Künstliche AVI ohne Ton, AVI mit MP3 und H.264/AAC-MKV | Remux und Metadatenprüfung technisch ausgeführt und bestanden |
| Vorhandene Remux-Zieldatei / vorab abgebrochener Auftrag | Im technischen Integrationstest abgewiesen |
| Enigma AVI, H.264/MP3, 25 fps | Remux und MP4Box-Schnitt erfolgreich; praktische Filmkontrolle und Decoderlauf mit normalisierten Ausgabezeitstempeln bestanden; PTS-/DTS-Auffälligkeiten dokumentiert |
| Wo die Liebe hinfällt, Full-HD-AVI mit Endung `.avi.mp4` | Bildfehler und fehlerhafte Bildaktualisierung bei Frame-Sprüngen in CAN beobachtet; Tonversatz in MPC-HC berichtet; kein erfolgreicher Schnittnachweis |

Der letzte vollständige automatisierte Testlauf umfasst 450 erfolgreiche Tests. Die frühere Aufschlüsselung auf die vier Testsuiten bezog sich auf den Stand mit 443 Tests und wurde für diesen Dokumentationsstand nicht erneut einzeln erhoben. Vollständige automatisierte GUI- oder Video-Synchronitätstests sind nicht nachgewiesen.

### Weitere manuelle Fehler- und Grenzfallprüfungen

- Vorbereitung mit „Nein“ ablehnen: Original und Schnittmarken bleiben verfügbar.
- Während eines längeren FFmpeg-Laufs abbrechen: Prozess endet, MP4Box startet nicht, temporäre Datei wird aufgeräumt.
- Während MP4Box abbrechen: Abbruchmeldung prüfen; keine neue endgültige Ausgabe, vorhandene Ausgabe unverändert und temporäre Dateien aufgeräumt.
- Ungültigen FFmpeg-/MP4Box-Pfad sowie nicht MP4-kompatible Streams prüfen: verständlicher Fehler, keine automatische Neukodierung.
- Ziel bereits vorhanden: Bestätigung und Erhalt der alten Ausgabe bei Fehlschlag vor dem Ersetzen prüfen.
- Fehleranzeige bei eingeklapptem Protokoll prüfen; Kopieren und erneuten Versuch kontrollieren.
- Andere Eingangscontainer, weitere Audio-/Videostreams und verschiedene Bildraten getrennt erproben.
- Bei Erfolg immer Filmgrenzen, Werbeschnitte und Ton-Synchronität der tatsächlichen Ausgabe kontrollieren.

Diese Liste kennzeichnet noch zu vertiefende Prüfungen; sie behauptet keine bereits erfolgte vollständige Abdeckung. Keine realen Filmdateien ins Repository aufnehmen.

Die Review-Korrekturen sind durch Tests für widersprüchliche Cutlist-Bereiche sowie die Ausgabeübernahme nach erfolgreichem Zusammenfügen abgesichert. Fehler, Abbruch, vorhandene Ausgaben und während des Schnitts neu angelegte Zieldateien werden geprüft.
