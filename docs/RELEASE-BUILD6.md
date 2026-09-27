# CAN 0.2.0 Build 6: lokaler Release-Kandidat RC1

RC1 bezeichnet die zusammengehörigen lokalen Quell-, ZIP- und Setup-Pakete. Die Programmversion bleibt 0.2.0, die Buildnummer 6. Eine öffentliche Veröffentlichung ist damit noch nicht erfolgt.

## Quellstand

Der Quellstand wird aus den versionierten und nicht ignorierten neuen Dateien des Arbeitsbaums exportiert. Er umfasst auch die noch nicht eingecheckten Änderungen. `SOURCE-SNAPSHOT.json` nennt den zugrunde liegenden Git-Commit, Version und Buildnummer; `SOURCE-FILES.sha256` erfasst jede enthaltene Projektdatei. Die beiden erzeugten Metadateien sind nicht gegenseitig selbst gehasht; der SHA-256-Wert des gesamten Quell-ZIPs umfasst beide. Ignorierte Buildausgaben, lokale Einstellungen, Git-Metadaten und native Binärdateien sind nicht Teil des CAN-Quellpakets.

Die NuGet-Abhängigkeiten werden in `packages.lock.json` je Projekt festgehalten. Der Paketierungsaufruf prüft den Quellstand vor und nach dem Build und verwendet den gesperrten Restore. Das ist ein nachvollziehbarer Build-Nachweis, keine Behauptung eines bitidentischen Neuaufbaus mit beliebigen Werkzeugversionen.

## Voraussetzungen zum erneuten Bauen

- Windows x64, Windows PowerShell 5.1, .NET 10 SDK (für RC1 verwendet: 10.0.303).
- Inno Setup 7 mit `ISCC.exe` für das Setup.
- Die vier unveränderten DLLs des dokumentierten CAN-libmpv-0.41.0-Builds. Sie können aus dem CAN-ZIP oder dem Quellen-Prüfpaket entnommen werden. `setup-libmpv.ps1` prüft ihre festgelegten SHA-256-Werte.
- Die durch die Sperrdateien festgelegten NuGet-Pakete im Cache oder erreichbar über die Paketquelle.

Das Quellen-Prüfpaket und das Relink-Paket für libmpv sind eigenständige Release-Bestandteile; siehe [libmpv-Dokumentation](libmpv/v0.41.0/README.md). Sie werden nicht durch das CAN-Quellpaket ersetzt.

## Paketierung aus dem entpackten Quell-ZIP

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\package-release.ps1 `
  -BuildNumber 6 `
  -NativeSourceDirectory 'C:\Pfad\zum\Vier-DLL-Ordner' `
  -SourceArchivePath 'C:\Downloads\CutAssistantNext-0.2.0-Build6-source-RC1.zip' `
  -OutputDirectory 'C:\Pfad\zu\neuem\RC1-Ausgabeordner' `
  -InnoCompilerPath 'C:\Program Files\Inno Setup 7\ISCC.exe' `
  -Candidate RC1
```

Der Ausgabeordner muss neu sein. Der Aufruf baut die Lösung ohne Erhöhung des Buildzählers, führt die Tests aus, erzeugt ein frameworkabhängiges Publish, prüft die vier nativen DLLs und die Ausschlüsse für Laufzeitdateien und externe Werkzeuge und baut ZIP und Setup. Ein Fehler stoppt die weiteren Schritte; Protokolle und Teilergebnisse bleiben zur Diagnose erhalten. Ein Wiederholungsversuch verwendet einen neuen Ausgabeordner.

Die Ausgabe enthält `SHA256SUMS.txt`, die Dateiliste `PUBLISH-FILES.sha256`, Build-/Testprotokolle und im Programmordner `RELEASE-PROVENANCE.json` mit dem SHA-256-Wert des zugehörigen Quell-ZIPs. Der erfolgreiche Quellstand lässt sich damit auch ohne einen neuen Git-Commit eindeutig zuordnen. Das normale `tools/build.ps1` ist für diesen festen Build-6-Aufruf ungeeignet, weil es den Zähler erhöht.

Vor dem öffentlichen Bereitstellen gehören das CAN-Quellpaket, ZIP, Setup, die beiden libmpv-Quellen-/Relink-Archive und die aktuellen Lizenzhinweise zusammen. Die Prüfsummen werden nach dem Hochladen erneut kontrolliert. Der RC1-Pakettest wird getrennt von den älteren VM-Vorschautests dokumentiert.
