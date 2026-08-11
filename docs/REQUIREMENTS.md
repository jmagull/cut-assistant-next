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
- aus den zu entfernenden Schnittbereichen komplementäre Behaltebereiche für klassische Cutlists erzeugen
- klassische Cutlists mit `[General]`-, `[CutN]`- und `[Info]`-Bereichen erzeugen
- Cutlist-Metadaten wie vorgeschlagenen Filmnamen, Autor, Benutzerkommentar und technische Hinweise abbilden
- Namensbildung für Cutlists über eine konfigurierbare Namensmaske ermöglichen
- erzeugte Cutlists lokal als `.cutlist`-Datei speichern
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
- keine fest codierten Installationspfade für spätere Schnittmotoren
- Cutlist-Zeit- und Zahlenwerte kulturunabhängig serialisieren
- erzeugte Cutlists als UTF-8 ohne BOM mit CRLF-Zeilenenden schreiben
- Unit-Tests für testbare Fachlogik
- manueller Test mit mindestens drei OTR-MP4-Dateien
