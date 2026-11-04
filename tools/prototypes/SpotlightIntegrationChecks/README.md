# Native CAN-/Spotlight-Prüfung

Vorher CAN-Release-Entwicklungsstand mit nativen libmpv-DLLs bauen. Dieses Werkzeug referenziert dessen fertige DLLs und baut die Anwendung nicht neu. Als Argument ausschließlich eine eigene synthetische zwölfsekündige MP4-Datei verwenden; echte Anwenderdateien sind nicht nötig. FFmpeg und ffprobe als vollständige Pfade übergeben, Ausgabe in einem separaten Arbeitsordner.

~~~powershell
dotnet build .\tools\prototypes\SpotlightIntegrationChecks\SpotlightIntegrationChecks.csproj --configuration Release
& .\tools\prototypes\SpotlightIntegrationChecks\bin\Release\net10.0-windows\SpotlightIntegrationChecks.exe 'Pfad zum synthetischen Film.mp4' 'Pfad zu ffmpeg.exe' 'Pfad zu ffprobe.exe' 'artifacts/spotlight-integration/native'
~~~

Erwartet: 32 erfolgreiche Prüfungen, Exitcode 0, results.json und Fenster-PNGs. Beide Editorarbeitsweisen, wirkliche ffprobe-/FFmpeg-/mpv-Arbeit, direktes Ersetzen/Ablehnen, Dateisperre, Cutlist-Schreiben/Lesen, Korrektur, Abbruch und Wiederöffnung. WPF-Fensterpersistenz wird in den Ausgabeordner umgeleitet; Werkzeugwerte sind ein isoliertes CAN-Einstellungsobjekt, Bestätigungen ein automatisierter Callback. Keine Änderungen an Anwenderkonfigurationen. Die Sichtbilder bilden WPF-Inhalt ab; native mpv-Videooberflächen werden über den Playerzustand geprüft.
