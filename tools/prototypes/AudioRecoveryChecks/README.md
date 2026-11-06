# Native Prüfung der drei Audioentscheidungen

Dieses separate Werkzeug referenziert die fertigen CAN-DLLs aus CanAssemblyDirectory. Es verwendet echte otr-can-, FFMS2-, FFmpeg-, ffprobe- und MP4Box-Aufrufe. Entscheidungen kommen aus einem ausdrücklich simulierten Callback; das WPF-Dialogbild wird ohne Bedienung eines Nutzerfensters gerendert. Es werden keine CAN-Einstellungen gespeichert. Der Fehlerfall übergibt absichtlich einen falschen Bestätigungswert an den tatsächlichen Motor; der neue Motor muss den Lauf ablehnen.

Die synthetischen Quellen erzeugt can-engine/tests/aac_gap_media.py im passenden Rust-Repository. Der optionale NCIS-Pfad bleibt privat. Jeder Prüflauf benötigt einen neuen Ergebnisordner. Pro Fall bleiben CAN-Protokoll, ggf. WPF-Bild und die tatsächlich gelesenen Rust-Prüfbelege erhalten. Testausgaben und Filme bleiben außerhalb von Git.

~~~powershell
dotnet build tools/prototypes/AudioRecoveryChecks/AudioRecoveryChecks.csproj -c Release -p:CanAssemblyDirectory=PAKETORDNER
# Die EXE aus ihrem Build-Ordner starten:
CutAssistantNext.App.Tests.exe ENGINE FFMSINDEX FFMPEG FFPROBE MP4BOX FIXTURE-DIR NEW-RESULT-DIR [NCIS-INPUT [JOHN-WICK-INPUT]]
~~~

Der optionale Realtest prüft bewusst nur zwei Frames rund um die belegte NCIS-Lücke, führt aber alle strengen Quellen- und Ausgabeprüfungen aus. Mit `CAN_AUDIO_RECOVERY_CASE=ncis-real-full` wird der vollständige Referenzschnitt aus CAN ausgeführt und sein Ausgabehash geprüft. Der zusätzliche John-Wick-Pfad verwendet die historischen zwei Behaltebereiche und prüft den vollständigen bestätigten Neustart mit AAC. Die optionalen synthetischen Mehrspurquellen heißen `healthy-two-tracks.mp4`, `ac3-damaged.mp4`, `aac-damaged.mp4`, `aac-damaged-aligned-ac3.mp4` und `two-damaged.mp4`.
