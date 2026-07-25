# Anforderungen

## Muss-Anforderungen POC 0.1

- Windows-11-x64-Anwendung
- moderne WPF-Oberfläche
- MP4-Datei über Dateidialog öffnen
- Video im Hauptfenster wiedergeben
- Play/Pause
- Zeitleiste und Zeitangaben
- Einzelbild vorwärts und rückwärts
- Lautstärkeregelung
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
