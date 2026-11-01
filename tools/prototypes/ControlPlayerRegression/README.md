# Kontrollplayer – native Regression des Tonspur-/EOF-Absturzes

Eigenständiger Windows-x64-/WPF-Testprozess gegen die **gebauten Produktbibliotheken**. Keine neue Produktionsabhängigkeit, keine Änderung der Solution. Interne Kontrollplayer-Methoden werden per Reflection im originalen Produktcode aufgerufen; WPF-Tests instanziieren das originale Kontrollfenster. Eine unbehandelte Wrapper-Ausnahme beendet den Prozess und lässt den Lauf scheitern.

Voraussetzungen: .NET-10-SDK, fertig gebauter CAN-Entwicklungsstand mit den vorhandenen vier libmpv-DLLs und ein lokales FFmpeg zum Erzeugen der kleinen Testdatei. Keine reale OTR-Datei und kein Schnittmotor nötig. Von der Repositorywurzel aus:

```powershell
$proof = Join-Path (Get-Location) '.build/control-player-regression'
New-Item -ItemType Directory -Force $proof | Out-Null
# Vorhandenen lokalen FFmpeg-Pfad einsetzen; die Datei wird nur im Nachweisordner angelegt.
$ffmpeg = 'C:\Pfad\zu\ffmpeg.exe'
$media = Join-Path $proof 'two-audio.mp4'
& $ffmpeg -hide_banner -loglevel error -nostdin `
    -f lavfi -i testsrc2=size=160x90:rate=25 `
    -f lavfi -i sine=frequency=440:sample_rate=48000 `
    -f lavfi -i sine=frequency=880:sample_rate=48000 `
    -t 8 -map 0:v -map 1:a -map 2:a -c:v libx264 -threads:v 1 `
    -preset ultrafast -c:a aac -metadata:s:a:0 language=deu `
    -metadata:s:a:1 language=eng $media
if ($LASTEXITCODE -ne 0) { throw 'Testdatei konnte nicht erzeugt werden.' }

dotnet build .\tools\prototypes\ControlPlayerRegression\ControlPlayerRegression.csproj -c Release
if ($LASTEXITCODE -ne 0) { throw 'Native Tests konnten nicht gebaut werden.' }
Push-Location .\tools\prototypes\ControlPlayerRegression\bin\Release\net10.0-windows
try {
    foreach ($case in 'regular', 'property', 'eof', 'two-player', 'lifetime', 'wpf') {
        & .\ControlPlayerRegression.exe $case $media (Join-Path $proof "$case.json")
        if ($LASTEXITCODE -ne 0) { throw "Native Regression fehlgeschlagen: $case" }
    }
} finally { Pop-Location }
```

Die eigene EXE aus ihrem Ausgabeordner starten, damit die nativen Begleit-DLLs gefunden werden. Mit `-p:CanAssemblyDirectory=...` lässt sich beim Bauen ein anderer vorbereiteter CAN-Ordner wählen. Der Prüflauf schreibt ausschließlich in den übergebenen Berichtsordner und in seine üblichen Build-Verzeichnisse; veröffentlichte Pakete werden nicht verändert.

| Prozessfall | Prüfungen |
| --- | --- |
| `regular` | Vier echte Tonspurwechsel während laufender Wiedergabe mit protokollierten Vorher-/Nachherpositionen; Wechsel während Pause; mittlere Prüfstelle und ihre Vorschauposition erhalten; bewusste Navigation weiterhin drei Sekunden vor EOF. |
| `property` | Tatsächlicher Seek ohne geladene Datei: `MpvException: property unavailable` muss am wartenden Task ankommen. Anschließend Laden, Pause, Seek, Play und Lautstärke funktionsfähig. |
| `eof` | Tatsächliches natürliches EOF mit verschwundenem Spurinventar; Pause erhält `Ended`; Wechsel mit gültiger ID lädt neu; Drei-Sekunden-Vorlauf; gewöhnliche EOF-Navigation behält gewählte Spur; nicht verfügbare ID an letzter Prüfstelle und anschließendes Zurück überleben. |
| `two-player` | Zwei echte HWND-/libmpv-Kontexte; EOF und Tonspurwechsel im Kontrollplayer lassen Hauptplayerposition/-spur/-zustand unverändert; Hauptplayer nach Kontrollplayer-Dispose weiter bedienbar. |
| `lifetime` | Abbruch vor Erwerb des Property-Zugriffs, ausstehender Zugriff vor Dispose, Zugriff nach Dispose und wiederholtes Dispose. Der interne Property-Semaphor wird für die deterministische Ablaufsteuerung per Reflection verwendet. |
| `wpf` | Echte native Wiedergabe und echte WPF-Auswahlbindung; gezielt injizierte Tonspurablehnung, danach Zurück/Weiter und konsistente Auswahl; Schließen während gezielt blockiertem, abbrechbarem Wechsel. |

Ausgaben sind JSON mit `Passed`, den einzelnen `Checks`, Fehlern und Logger-Nachweisen. Die beiden Fehler-/Blockierstellen des WPF-Falls sind Testinjektionen; der `property`-Fehler und das EOF-Verhalten verwenden unveränderte reale libmpv-Rückgaben. Verborgene HWNDs/Fenster, keine persönliche Bild-/Ton-/Synchronitätsabnahme. Die WPF- und Unit-Tests ergänzen einander; die Prozessfälle gehören bewusst nicht zum plattformunabhängigen normalen Unit-Testlauf.
