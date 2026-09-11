# Testplan – POC 0.1

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

Aktueller vollständiger Testlauf: **399 von 399 Tests erfolgreich**.

Zusätzlich wurde die lokale Cutlist-Dateiausgabe in einem Smoke-Test praktisch geprüft. Dabei wurde eine vollständige `.cutlist`-Datei erzeugt und anschließend explizit als UTF-8 eingelesen; auch Umlaute wurden korrekt erhalten.

Cutlist-Reader und Parser sind umgesetzt und automatisiert getestet. Beim Einlesen wird zunächst UTF-8 verwendet; historische Windows-1252-/ANSI-Cutlists werden als Kompatibilitätsfall unterstützt.
