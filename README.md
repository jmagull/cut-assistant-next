# Cut Assistant Next

Cut Assistant Next (CAN) ist ein Windows-Programm zum Abspielen von Videos, Bearbeiten klassischer Cutlists und Schneiden mit MP4Box. Die Oberfläche verwendet WPF und mpv/libmpv, ohne DirectShow oder installierte Windows-Codec-Pakete vorauszusetzen.

Stand: 13.09.2026. CAN ist weiterhin ein Proof of Concept mit experimenteller Unterstützung weiterer Eingangscontainer.

## Einstieg

- [Nutzeranleitung](docs/NUTZERANLEITUNG.md): Einrichtung, Bedienung, Cutlists, Schneiden und Hilfe bei Fehlern.
- [Projektstatus](docs/STATUS.md): umgesetzter Umfang, bestätigte Tests und offene Aufgaben.
- [Testplan](docs/TESTPLAN.md): reproduzierbare Prüfungen und Testbasis.
- [Architektur](docs/ARCHITECTURE.md) und [Entscheidungen](docs/DECISIONS.md): technische Hintergründe und Entwicklungsgeschichte.

## Was CAN derzeit kann

- Videos laden, mit ffprobe analysieren und mit mpv wiedergeben.
- Play/Pause, Seeking, Einzelbildschritte und Lautstärkeregelung.
- Zu entfernende Bereiche markieren, auswählen, korrigieren und löschen.
- Cutlists lokal laden, erzeugen und speichern.
- Passende Server-Cutlists automatisch suchen und die Suche erneut per Button öffnen.
- Lokal gespeicherte Cutlists nach Bestätigung auf den konfigurierten Server hochladen.
- Ausgabenamen über Namensmasken und Cutlist-Namensvorschläge vorbereiten.
- MP4-Inhalte direkt mit MP4Box schneiden, auch bei abweichender Dateiendung.
- Andere Container nach Bestätigung experimentell mit FFmpeg verlustfrei nach MP4 umpacken und anschließend mit MP4Box schneiden.
- Fortschritt und Protokoll anzeigen, kopieren und den Vorgang abbrechen.
- Fenstergröße, Maximierung und Lautstärke speichern; Inhalte bei kleinen Fenstern umbrechen bzw. scrollbar halten.

Die reine Cutlist-Erstellung benötigt keine Video-Umwandlung. Erst beim Schneiden wird eine gegebenenfalls notwendige MP4-Arbeitsdatei erzeugt. Serversuche und Cutlist-Zuordnung beziehen sich dabei weiterhin auf die Originaldatei.

## Grenzen

MP4 ist ein Container, keine Bezeichnung für einen bestimmten Videocodec. Ob sich ein anderes Format verlustfrei vorbereiten lässt, hängt von seinen Streams ab. CAN verwendet Stream-Copy und führt keine automatische Neukodierung durch. Ungeeignete Dateien oder deutliche Abweichungen werden mit einer Fehlermeldung gestoppt.

Die Prüfung der Arbeitsdatei ist eine Plausibilitätsprüfung anhand der Medieninformationen. Sie ersetzt nicht die Kontrolle der Schnittstellen und der Ton-Synchronität. Smart Rendering, Stapelverarbeitung, integrierte mehrteilige Aufnahmen sowie Installer/portable Veröffentlichung sind noch nicht umgesetzt.

## Für Anwender

Vorgesehene Umgebung ist Windows 11 x64. Die derzeitige Build-Ausgabe benötigt die passende .NET-10-Desktop-Laufzeit und die mitgelieferte `libmpv-2.dll` neben dem Programm. ffprobe, FFmpeg und MP4Box werden über die Einstellungen konfiguriert. Eine fertige Installationsroutine gehört noch nicht zum Projektstand.

Die [Nutzeranleitung](docs/NUTZERANLEITUNG.md) erklärt die Einrichtung ohne Entwicklungswerkzeuge.

## Aus dem Quellcode bauen

Voraussetzungen: Windows 11 x64, .NET-10-SDK und 7-Zip für die Einrichtung der nativen Wiedergabebibliothek. Das Setup-Skript lädt die im Repository festgelegte libmpv-Version und prüft den Archiv-Hash. Restore und erstmalige Einrichtung benötigen Netzwerkzugriff.

Im Repository-Verzeichnis:

```powershell
.\tools\setup-libmpv.ps1
dotnet restore .\CutAssistantNext.sln
dotnet build .\CutAssistantNext.sln --configuration Release --no-restore
dotnet test .\CutAssistantNext.sln --configuration Release --no-build
```

Die Anwendung liegt danach unter `src\CutAssistantNext.App\bin\Release\net10.0-windows\CutAssistantNext.App.exe`. Der Build kopiert die vorbereitete libmpv-Bibliothek in das Ausgabeverzeichnis. Für Analyse, experimentelles Umpacken und Schnitt sind zusätzlich die konfigurierten externen Werkzeuge erforderlich.

## Projektstruktur

| Bereich | Aufgabe |
|---|---|
| `src/CutAssistantNext.App` | WPF, Dialoge, ViewModels, Einstellungen und Ablaufsteuerung |
| `src/CutAssistantNext.Core` | Medienmodelle, Schnittplan, Namensbildung und Verträge |
| `src/CutAssistantNext.Media` | ffprobe, mpv und Prozessausführung für FFmpeg/MP4Box |
| `src/CutAssistantNext.Cutlists` | Cutlist-Import, Export, Metadaten und Bereichsumrechnung |
| `tests` | vier automatisierte Testsuiten |
| `samples` | kleine Referenzdaten |
| `tools` | Einrichtung von libmpv |
| `docs` | Anleitung, Status, Testplan und technische Dokumentation |

Letzter bestätigter Stand: Release-Build ohne Fehler oder Warnungen, **443/443 Tests bestanden**. Die Diplomatin und Rubikon wurden mit jeweils zwei Cutlists im integrierten AVI-Ablauf erfolgreich geschnitten und vom Nutzer im Player geprüft. Einzelheiten und Einschränkungen stehen im [Testplan](docs/TESTPLAN.md).
