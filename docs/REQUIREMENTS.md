# Anforderungen

## Muss-Anforderungen POC 0.1

- Windows-11-x64-Anwendung
- moderne WPF-Oberfläche
- MP4-Dateien sowie OTR-Dateien mit tatsächlichem MP4-Inhalt und Dateiendung `.avi` über Dateidialog öffnen
- Video im Hauptfenster wiedergeben
- Play/Pause
- Zeitleiste und Zeitangaben
- Einzelbild vorwärts und rückwärts
- Lautstärkeregelung
- Tastatursteuerung für Play/Pause und Einzelbildnavigation
- Schnittanfang und Schnittende an der aktuellen Videoposition setzen
- mehrere zu entfernende Schnittbereiche verwalten
- vorhandene Schnittbereiche auswählen, korrigieren und löschen
- Schnittbereiche proportional auf einer eigenen Timeline darstellen
- Erfassungs- und Korrekturmodus klar voneinander trennen
- zuletzt verwendete Fenstergröße und maximierten Zustand wiederherstellen
- Medieninformationen über ffprobe
- Logdatei mit verständlichen Fehlermeldungen
- keine DirectShow-/DSPack-Abhängigkeit
- keine installierten Codec-Pakete erforderlich

## Qualitätsanforderungen

- reproduzierbarer Release-Build
- keine blockierte Oberfläche bei Medienoperationen
- saubere Ressourcenfreigabe
- verständliche Meldung bei fehlendem mpv oder ffprobe
- keine fest codierten Benutzerpfade
- Unit-Tests für testbare Fachlogik
- manueller Test mit mindestens drei OTR-MP4-Dateien
