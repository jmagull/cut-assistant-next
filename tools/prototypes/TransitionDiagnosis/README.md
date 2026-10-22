# MP4Box-Übergangsdiagnose

Separater .NET-10-Konsolenprototyp. Keine Einbindung in CAN oder die Produktions-Solution. Er liest ein Original, eine **bereits fertige MP4Box-Ausgabe** und die dazugehörige Cutlist. Es werden weder neue Schnitte noch Audioverarbeitung, Neukodierung oder Playeraufrufe ausgeführt. Die Eingaben bleiben während der Diagnose gegen Schreiben und Löschen gesperrt.

## Bauen und testen

Im Repository-Verzeichnis mit installiertem .NET-10-SDK:

```powershell
$diagnosisTargets = Join-Path $PWD 'tools/prototypes/TransitionDiagnosis/NuGetIsolation.targets'
dotnet restore tools/prototypes/TransitionDiagnosis.Tests/TransitionDiagnosis.Tests.csproj "-p:CustomAfterMicrosoftCommonTargets=$diagnosisTargets"
dotnet build tools/prototypes/TransitionDiagnosis.Tests/TransitionDiagnosis.Tests.csproj -c Release --no-restore "-p:CustomAfterMicrosoftCommonTargets=$diagnosisTargets"
dotnet test tools/prototypes/TransitionDiagnosis.Tests/TransitionDiagnosis.Tests.csproj -c Release --no-build --no-restore
```

Der Prototyp nutzt den vorhandenen Cutlist-Parser per Projektreferenz und dieselben bereits im Repository verwendeten Testpakete. Keine neue Produktionsbibliothek. Die optionale Restore-Isolierung schreibt Lockdateien ausschließlich unter `.build/transition-diagnosis/locks`. Der direkte Build erhöht den CAN-Buildzähler nicht. Das nummerierende `tools/build.ps1` gehört nicht zum Prototypablauf.

## Vorhandene Dateien untersuchen

Beispielpfade ersetzen; das Verzeichnis für den Bericht muss bereits existieren:

```powershell
dotnet tools/prototypes/TransitionDiagnosis/bin/Release/net10.0/TransitionDiagnosis.dll `
  --ffprobe 'C:\Werkzeuge\ffmpeg\bin\ffprobe.exe' `
  --source 'D:\Filme\Original.mp4' `
  --output 'D:\Filme\Fertiger MP4Box-Film.mp4' `
  --cutlist 'D:\Filme\Original.mp4.cutlist' `
  --report '.build\meine-uebergangsdiagnose.json'
```

Standard: 15 Sekunden Zuordnungsspielraum um jeden geplanten Behaltebereich, zehn Minuten Gesamtzeitlimit. Optionen `--margin-seconds` (0–60) und `--timeout-seconds` (1–3600). Jeder Videostrom ist auf zwei Millionen Pakete begrenzt. Genau eine Videospur erforderlich. Abbruch mit Strg+C; Exitcode 2 bedeutet Abbruch/Zeitlimit, 1 Fehler, 0 einen fertig geschriebenen **Diagnosebericht**, keine erfolgreiche Schnittabnahme.

ffprobe läuft mit einem Decoderthread und nach Möglichkeit niedriger Prozesspriorität. Paketlisten werden zeilenweise gelesen. Es werden kleine Übergangsfenster angefordert; eine Überlappung, die ein Fenster von mehr als 20 Sekunden erfordern würde, bleibt unaufgelöst. Zum Dekodieren kann ffprobe vor dem angeforderten Fenster am vorherigen Keyframe beginnen; im Bericht bleiben nur Bilder innerhalb des Fensters. Metadaten-/Dekodierantworten sind auf 16 MiB begrenzt. Bei Fehler und Abbruch werden gestartete Prozesse beendet und abgewartet. Berichtspfad und Eingabepfade müssen verschieden sein; vorhandene Berichte werden auch bei einem zwischenzeitlichen Dateikonflikt nicht überschrieben. Der Bericht wird erst nach vollständigem Schreiben unter seinem endgültigen Namen angelegt.

## Befunde richtig lesen

- Videopakete werden anhand ihres SHA-256-Nutzdatenhashes dem Original zugeordnet. Mehrfach vorkommende Hashes werden nur aufgelöst, wenn zwei eindeutige Nachbaranker exakt eine gleich lange, lückenlos identische Originalpaketfolge einschließen. Kein Überbrücken von Quellsprüngen oder veränderten Paketen.
- Die Zuordnung zu Behaltebereichen verwendet Original-PTS und die ausdrücklich angegebenen Zeitfenster. Überlappende Zuordnungsfenster, Pakete außerhalb aller Fenster oder fehlende Original-PTS bleiben unbestimmt. Das Verfahren benötigt keine erhaltenen MP4Box-Segmentdateien.
- `AllPacketsUniquelyMapped` bedeutet vollständige Paket-/Bereichszuordnung unter diesen Bedingungen; `KeepOrderMatchesPlan` verlangt zusätzlich genau die geplante Reihenfolge. Dies ist keine Aussage über korrekte Schnittinhalte.
- PTS, DTS, Dauer, Byteposition, Flags und Originalzuordnung bleiben im Paketumfeld getrennt sichtbar. Paketzeiten verwenden die jeweilige rationale Videozeitbasis; Framerate und Containerdauer ersetzen diese Zuordnung nicht.
- `PacketPresentation` unterscheidet Überlappung, gleiche und getrennte PTS. `SeparatedPts` prüft keine lückenlose Bilddauer. `OverlapSeconds` ist die Differenz zweier Zeitstempel, nicht die Länge einer nachgewiesenen sichtbaren Doppelbelichtung.
- Dekodierte Bilder werden über `pkt_pos` zurück zu den Ausgabepaketen zugeordnet. PTS, Decoder-DTS und Best-Effort-Zeit bleiben getrennt. Die ausgegebene Decoderfolge wird **nicht nach PTS sortiert**. `NonMonotonicDecodedPts`, `InterleavedKeeps`, gleiche Grenzzeiten und fehlende Zuordnungen bleiben erkennbar.
- `PacketCandidateSeconds`, dessen Planabweichung und `CandidateReviewStartSeconds` sind Diagnosekandidaten. Auch `SingleObservedSwitch` bestätigt ausschließlich das lokale ffprobe-Ergebnis. **`ReviewPositionConfirmed` bleibt immer false**: keine Übertragung in CAN, keine automatische Aussage über das von libmpv präsentierte Bild.

Jeder Bericht enthält SHA-256, Dateigröße und Änderungszeit von Original, Ausgabe und Cutlist. Vollständige lokale Pfade stehen nur in den lokalen Berichten; Berichte und reale Filme gehören nicht in Git. Audio, Synchronität, Werbeblitzer, Szenenwiederholungen und die frühere Systemfreeze-Ursache werden nicht bewertet. Für neu kodierte otr-can-Grenzen eignet sich der reine Nutzdatenvergleich zum Original nicht allgemein.

Realer Befund und Prüfstand: [Diagnosebericht vom 22.10.2026](../../../docs/MP4BOX-UEBERGANGSDIAGNOSE-20261022.md).
