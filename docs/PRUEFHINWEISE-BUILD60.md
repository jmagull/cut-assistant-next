# Prüfhinweise für Cut Assistant Next 1.0.3 Build 60

Setup und Portable-ZIP enthalten CAN mit Dateiversion **1.0.3.60**. Die Bedienänderungen stehen in [RELEASE-NOTES.md](RELEASE-NOTES.md). Laufzeiten und externe Werkzeuge sind in `VORAUSSETZUNGEN.txt` aufgeführt.

1. Das Setup in einer Windows-11-Testumgebung frisch installieren. Auf einem Testsystem mit Build 59 außerdem ein Update durchführen. Fenstergröße, Lautstärke sowie konfigurierte Pfade für FFmpeg, ffprobe, MP4Box und ffmsindex vor und nach dem Update vergleichen. Anschließend deinstallieren und prüfen, ob Programmeinträge verschwinden und Benutzereinstellungen erhalten bleiben.
2. Das Portable-ZIP in ein frisches Verzeichnis entpacken und CAN dort starten. In beiden Ausgaben den Fenstertitel und `tools/otr-can/otr_can.exe --version` prüfen. Die Versionsausgabe muss `otr_can 0.1.6-audio-diag.1` lauten.
3. In CAN unter **Einstellungen → OTR-CAN-Werkzeuge → Werkzeuge prüfen** die Ausgabe kontrollieren. Einen Testfilm mit MP4Box und mit otr-can schneiden. Für Bild, Ton, Übergänge und die Drei-Sekunden-Vorschau jeweils die fertige Datei prüfen.
4. Einen Sekundensprung kurz und dann gehalten per Tastatur und Maus auslösen. In einem Eingabefeld die Pfeiltasten prüfen. Einen aktiven Bereich an Anfang und Ende korrigieren und die Auswahl prüfen. Den Kontrollplayer öffnen und die Lautstärkeübernahme sowie die Tooltips prüfen.

5. Eine zeitbasierte Cutlist mit kurzem letzten Behaltebereich und unbekannter FPS laden. Beide Ladeoptionen separat prüfen. Bei einem fehlgeschlagenen MP4Box-Probeschnitt den letzten Bereich nach Bestätigung entfernen und die Ausgabe kontrollieren.

Jörg hat die Usability-Änderungen und den korrigierten Endfragmentablauf persönlich unter Windows 11 mit den Entwicklungsfassungen geprüft. Die persönliche Abnahme von Build-60-Setup und Portable-ZIP sowie in einer Windows-11-VM steht aus. Windows 10 wurde nicht persönlich geprüft.
