# EAC3-Diagnose für Cut Assistant Next Build 61

Dieses separate Diagnoseprogramm verändert keine Produktfunktion. Es verwendet die gebündelten Produkt-DLLs über CanAssemblyDirectory und den vorhandenen Motor. Keine neuen NuGet-Produktabhängigkeiten.

## Voraussetzungen

Windows x64, .NET 10 SDK/Desktop Runtime, Python 3.11+, FFmpeg/ffprobe mit libx264/AAC/EAC3, FFMS2 und der unveränderte Build-61-Motor. Neue, kurze Prüfverzeichnisse verwenden. Keine realen Filmdateien als Ausgabe oder Testdaten einsetzen.

## Synthetische Motorprüfung

Alle Pfade sind eigene Argumente. Die Programme überschreiben kein bestehendes Prüfverzeichnis. Beispiel mit einzusetzenden Pfaden:

~~~powershell
python run_media.py --ffmpeg <ffmpeg.exe> --ffprobe <ffprobe.exe> --ffmsindex <ffmsindex.exe> --engine <otr_can.exe> --work <neues-kurzes-Verzeichnis> --fps 25
~~~

Mit --fps 50 und einem weiteren neuen Verzeichnis wiederholen. results.json unterscheidet erfolgreiche AAC-Schnitte von erwarteten EAC3-Ablehnungen. Die einzelnen Befehlsargumente und stdout/stderr bleiben erhalten. Je Lauf zwölf Diagnoseprüfungen.

## Policy- und Dialogprüfung gegen Paketbibliotheken

~~~powershell
dotnet restore Eac3Diagnosis.csproj
dotnet build Eac3Diagnosis.csproj -c Release -p:CanAssemblyDirectory=<vollständiger-Paketprogrammordner>
~~~

Die erzeugte CutAssistantNext.App.Tests.exe mit folgenden sechs Argumenten ausführen:

1. Verzeichnis des synthetischen 50-fps-Laufs.
2. Neues Ergebnisverzeichnis.
3. Motor-EXE.
4. ffmsindex-EXE.
5. FFmpeg-EXE.
6. ffprobe-EXE.

Der Assemblyname ist ausschließlich für den bereits bestehenden internen Testzugang notwendig. Es ist keine CAN-Anwendungsfassung und darf nicht installiert oder als Produktpaket verteilt werden. Die Prüfung referenziert die vorhandenen DLLs und schreibt results.json mit 24 Diagnoseprüfungen. Fallback-Engine-Stubs prüfen die bewusste Entscheidung ohne zusätzlichen Realfilmschnitt; die tatsächlichen Codec-Ablehnungen werden separat vom Python-Programm gegen den echten Motor geprüft.

Die AC-3-Policykontrolle verwendet analysierte AAC-Videodaten mit einem kontrolliert ersetzten Audiodeskriptor; sie ist kein neuer AC-3-Medienfunktionstest. Der WPF-Dialog wird auf einem STA-Thread erzeugt und ohne interaktive Benutzerentscheidung geprüft.

Die Schutzprüfungen dürfen nicht verändert werden, um EAC3 zu akzeptieren. Sie dokumentieren das Verhalten des eingefrorenen Produkts. Die Einordnung und offenen Punkte stehen in docs/DIAGNOSE-BUILD61-EAC3-20261118.md.
