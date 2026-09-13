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
| 4 | Zeitleiste bewegen | Position ändert sich nachvollziehbar |
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

## Automatisierte Tests

- ffprobe-Parsing, Medienanalyse und Fehlerabbildung
- `MainWindowViewModel` einschließlich Übernahme der Mediendauer
- `PlaybackViewModel` einschließlich Play/Pause, Seeking, Frames und Lautstärke
- `MpvMediaPlayerService` einschließlich Pausenzustand während des Ladens
- `RemoveSegment` einschließlich Bereichsvalidierung
- `CutPlan` einschließlich Sortierung, Überschneidungsschutz, Ersetzen und Löschen
- `CutPlanViewModel` einschließlich Erfassung, Auswahl und Korrekturmodus
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

Aktueller vollständiger Testlauf: **430 von 430 Tests erfolgreich**.

Zusätzlich wurde die lokale Cutlist-Dateiausgabe in einem Smoke-Test praktisch geprüft. Dabei wurde eine vollständige `.cutlist`-Datei erzeugt und anschließend explizit als UTF-8 eingelesen; auch Umlaute wurden korrekt erhalten.

Cutlist-Reader und Parser sind umgesetzt und automatisiert getestet. Beim Einlesen wird zunächst UTF-8 verwendet; historische Windows-1252-/ANSI-Cutlists werden als Kompatibilitätsfall unterstützt.

## Integrierter AVI-Ablauf und UI – Stand 13.09.2026

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

Die aktuellen automatisierten Suiten umfassen 40 Core-, 59 Cutlists-, 79 Media- und 252 App-Tests: insgesamt 430. Es sind keine vollständigen automatisierten GUI- oder Video-Synchronitätstests.

### Weitere manuelle Fehler- und Grenzfallprüfungen

- Vorbereitung mit „Nein“ ablehnen: Original und Schnittmarken bleiben verfügbar.
- Während eines längeren FFmpeg-Laufs abbrechen: Prozess endet, MP4Box startet nicht, temporäre Datei wird aufgeräumt.
- Während MP4Box abbrechen: Abbruchmeldung prüfen, eventuelle neue Zieldatei nicht als fertigen Schnitt behandeln.
- Ungültigen FFmpeg-/MP4Box-Pfad sowie nicht MP4-kompatible Streams prüfen: verständlicher Fehler, keine automatische Neukodierung.
- Ziel bereits vorhanden: Bestätigung und Erhalt der alten Ausgabe bei Fehlschlag vor dem Ersetzen prüfen.
- Fehleranzeige bei eingeklapptem Protokoll prüfen; Kopieren und erneuten Versuch kontrollieren.
- Andere Eingangscontainer, weitere Audio-/Videostreams und verschiedene Bildraten getrennt erproben.
- Bei Erfolg immer Filmgrenzen, Werbeschnitte und Ton-Synchronität der tatsächlichen Ausgabe kontrollieren.

Diese Liste kennzeichnet noch zu vertiefende Prüfungen; sie behauptet keine bereits erfolgte vollständige Abdeckung. Keine realen Filmdateien ins Repository aufnehmen.
