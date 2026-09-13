# AGENTS.md

## Projektauftrag

Dieses Repository enthält **Cut Assistant Next – Proof of Concept**: einen modernen Nachfolger des Cut Assistant für Windows 11, ohne DirectShow.

Arbeite in kleinen, überprüfbaren Schritten. Eine Aufgabe gilt erst als abgeschlossen, wenn der Code gebaut, getestet und die Änderung kurz dokumentiert wurde.

## Verbindliche technische Leitplanken

- Programmiersprache: C#
- Zielplattform des ersten POC: Windows 11 x64
- Framework: .NET 10
- Benutzeroberfläche: WPF
- Videowiedergabe: mpv/libmpv
- Medienanalyse: ffprobe
- DirectShow, DSPack und Abhängigkeiten von installierten Windows-Codecs sind verboten.
- MP4Box und FFmpeg werden erst in späteren Ausbaustufen als Schnittmotoren integriert.
- Oberfläche, Anwendungslogik, Medienwiedergabe und Medienanalyse sind klar zu trennen.

## Referenz auf den alten Cut Assistant

Das Repository `abc874/ca2018` dient nur als fachliche und technische Referenz.

- Alten Delphi-/Pascal-Code nicht ungeprüft kopieren.
- Keine GPL-Codebestandteile übernehmen, sofern dies nicht ausdrücklich beauftragt und dokumentiert wurde.
- Bewährte Abläufe, Dateiformate und Bedienideen dürfen analysiert und eigenständig neu implementiert werden.

## Arbeitsweise

Vor einer Änderung:

1. `README.md` und relevante Dateien unter `docs/` lesen.
2. Git-Status prüfen.
3. Kurz beschreiben, welche Dateien geändert werden sollen.
4. Nur den für die Aufgabe notwendigen Umfang verändern.

Nach einer Änderung:

1. Code formatieren.
2. Build und Tests ausführen.
3. Knapp berichten: geänderte Dateien, Build-/Testergebnis, bekannte Einschränkungen und sinnvoller nächster Schritt.
4. Keine Fehler verstecken und keine Tests entfernen, nur um einen grünen Build zu erhalten.

## Build- und Testbefehle

Sobald `CutAssistantNext.sln` vorhanden ist:

```powershell
dotnet restore .\CutAssistantNext.sln
.\tools\build.ps1 -Configuration Release -NoRestore
dotnet test .\CutAssistantNext.sln --configuration Release --no-build
```

Falls die Solution noch nicht existiert, darf sie nur im Rahmen einer ausdrücklich beauftragten Initialisierungsaufgabe angelegt werden.

## Codequalität

- Nullable Reference Types aktivieren.
- Warnungen nicht leichtfertig unterdrücken.
- Native Ressourcen, Prozesse und Dateihandles zuverlässig freigeben.
- Medienoperationen dürfen die WPF-Oberfläche nicht blockieren.
- Asynchrone Vorgänge sollen Abbruch und verständliche Fehlerbehandlung unterstützen.
- Keine absoluten Benutzerpfade in den Quellcode einbauen.
- Externe Werkzeuge und Bibliotheken über konfigurierbare Pfade oder eine klar definierte Laufzeitstruktur einbinden.
- Logs dürfen keine Zugangsdaten enthalten.

## Abhängigkeiten

- Neue Produktionsabhängigkeiten nur hinzufügen, wenn sie für die aktuelle Aufgabe erforderlich sind.
- Vorher Zweck, Lizenz und Wartungsstatus prüfen.
- Keine Bibliothek einführen, die DirectShow indirekt voraussetzt.

## Tests

- Fachlogik möglichst ohne Benutzeroberfläche und ohne echte Videodatei testbar halten.
- Für Parser, Zeitberechnung und spätere Cutlist-Logik Unit-Tests anlegen.
- Reale OTR-Videodateien nicht in Git aufnehmen.
- Kleine anonymisierte Cutlists und ffprobe-Testdaten dürfen unter `samples/` liegen.

## Git-Regeln

- Keine Buildausgaben, Videodateien, Zugangsdaten oder lokalen Einstellungen committen.
- Keine umfangreichen Umformatierungen mit funktionalen Änderungen vermischen.
- Bestehende Nutzeränderungen nicht ungefragt überschreiben oder zurücksetzen.

## Code-Review-Regeln

Besonders prüfen:

- versehentliche DirectShow-/DSPack-Abhängigkeiten,
- blockierende Arbeit auf dem WPF-UI-Thread,
- nicht freigegebene libmpv-, Prozess- oder Dateiressourcen,
- fehlende Fehlerbehandlung bei nicht vorhandenen Dateien und Werkzeugen,
- fest codierte Pfade,
- ungetestete Zeit- und Frameberechnungen,
- Vermischung von UI und Fachlogik.
