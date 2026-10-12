# Paketbau auf otr-can 0.1.5 umgestellt – Schritt 1

Stand: 12.10.2026. Nur Paketbau-Voraussetzungen und Hinweise angepasst. Keine Commits, Quellen-Snapshots, Publish-Ausgaben, Setup-Dateien oder Portable-Releasepakete erstellt. Die bisherigen Release-Artefakte bleiben erhalten.

## Änderungen

- tools/package-release.ps1 verwendet eine zentrale Sollversion 0.1.5 und erwartet sources/otr-can-0.1.5-source.zip. CAN-Snapshot und Motor-Provenienz müssen diese Version erklären. Die bestehenden Manifest-, EXE-, Quellen- und Kopierprüfsummenprüfungen bleiben erhalten.
- Die tatsächlich kopierte EXE wird mit --version und capabilities geprüft; die exakte Fähigkeit mp3-aac-v1 muss als eigener Token vorkommen. Eine bloße Versionsangabe reicht nicht. Die Prüfung erfolgt nur bei beigefügtem optionalem Motor; der Paketbau ohne Motor bleibt erhalten.
- installer/HINWEIS-DOTNET-UND-WERKZEUGE.txt nennt 0.1.5, die beigelegte EXE, Pfadprüfung nach Überinstallation sowie experimentelle AVI-Vorbereitung und einmalige verlustbehaftete MP3→AAC-LC-Kodierung. Externe FFmpeg-/ffprobe-/FFMS2-Voraussetzungen bleiben erhalten.
- tools/tests/Test-OtrCanPackage.ps1 prüft den tatsächlichen Motorblock und dessen Werkzeugrunner aus dem PowerShell-Syntaxbaum. Der vollständige Paketbau wird nicht gestartet.

## Prüfung

10 Prüffälle jeweils unter PowerShell 7 und Windows PowerShell 5.1 bestanden:

1. Kein optionaler Motor: kein Motorordner und keine Motoraufrufe.
2. Gültige Angaben: Kopie der tatsächlichen 0.1.5-EXE, Versions-/Fähigkeitsabfragen und kopierte Dateien geprüft.
3. Alter CAN-Snapshot mit Motorversion 0.1.4 abgelehnt.
4. Alte Motor-Provenienz 0.1.4 abgelehnt.
5. Fehlende Quellen abgelehnt.
6. Veränderte EXE per Manifestprüfung abgelehnt.
7. Falsche Quellenprüfsumme trotz ansonsten passendem Manifest abgelehnt.
8. Manifestpfad außerhalb des Motorpakets abgelehnt.
9. Fehlende MP3-Fähigkeit beziehungsweise irreführender Teilstring abgelehnt.
10. Abweichende CLI-Versionsmeldung abgelehnt.

Nur Testdaten: Quellen-/Lizenzangaben sind synthetische Prüfwerte, keine gültigen Vertriebsquellen oder fertigen Motorpakete. Im positiven Fall wird die reale EXE gestartet; für die beiden negativen Werkzeugmeldungen wird ausschließlich deren Probe-Ausgabe kontrolliert ersetzt. Die spätere tatsächliche Quellenpaketprüfung steht noch aus.

Eine anfängliche Syntaxprüfung beanstandete die Variableninterpolation vor einem Doppelpunkt im Prüfhelfer. Diese wurde korrigiert; alle Fälle anschließend unter beiden Shells bestanden. Keine Prüfungen entfernt oder abgeschwächt.

Normaler CAN-Release-Rebuild über tools/build.ps1 -Configuration Release -NoRestore: **Build 16, 0 Warnungen, 0 Fehler**. **793/793 CAN-Tests**, 0 fehlgeschlagen, 0 übersprungen. Anwendungscode und Motor unverändert; die neue Buildnummer stammt aus dem vorgeschriebenen Validierungsbuild. Keine Paketwiederherstellung. Alle acht CAN-Lockdateien per SHA-256 unverändert zum Aufgabenbeginn; vier vorbestehende Produktionsänderungen erhalten. Rust-Stand unverändert 0.1.5, zuletzt 44 Tests.

Diff-/Syntaxprüfungen bestanden. Nachweise im ignorierten .build/package-motor-015-20261012: BUILD.log, TESTS.log, POWERSHELL51.log, pwsh-checks/RESULT.json, powershell51-checks/RESULT.json und LOCKS-FINAL.json.

## Wiederholen

Aus dem Repository, mit einer vorhandenen Motor-EXE und einem neuen Prüfverzeichnis:

```powershell
.\tools\tests\Test-OtrCanPackage.ps1 `
    -EngineExecutablePath '<Pfad zur otr-can-0.1.5-EXE>' `
    -ScratchDirectory '<neues Verzeichnis für Prüfdaten>'
```

## Nächster Schritt

Schritt 2: geprüfte CAN-/Motoränderungen gezielt sichern, die vier lokalen Lockdateiänderungen erhalten und daraus passende feste Quellen-Snapshots und Motor-Provenienz für 0.1.5 vorbereiten. Erst danach folgen der vollständige Paketbau, Paketabnahme und Veröffentlichung. Der vorhandene Filmprüfbericht und dessen technische Zeitstempelgrenze bleiben unverändert gültig.