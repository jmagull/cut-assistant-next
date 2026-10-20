# Isolierter Prototyp: Ausgabeübergänge im Kontrollplayer

Dieser .NET-10-/WPF-Prototyp ist **nicht in CAN eingebaut**. Er verändert weder Schnittplan noch Cutlist und schneidet beim Analysieren keine neue Datei. Er benötigt die erzeugten MP4Box-Segmente und deren Gesamtfilm. Die Produktions-Solution bindet dieses Projekt nicht ein.

## Verfahren und Grenzen

Der Prototyp liest mit ffprobe sämtliche Video-Paketprüfsummen der Segmente und des Gesamtfilms. Die Ausgabe muss exakt der Verkettung der Segment-Paketfolgen entsprechen. Die Übergänge stammen aus den tatsächlichen Ausgabe-PTS, mit rationaler Zeitbasis; Dateidauern und nominelle Bildraten bestimmen diese Stellen nicht. Jeder neue Segmentanfang muss als erster Keyframe mit minimalem Abschnitts-PTS eindeutig sein und wird anschließend lokal dekodiert bestätigt. Bei nicht eindeutigen Daten gibt es einen Fehler und kein neues Ergebnis.

Unterstützt werden zunächst genau eine Videospur, positiver Dateiinhalt, Zeitstart null und geordnete, nicht überlappende Abschnittspräsentation. Doppelte PTS genau an einer Grenze werden ausdrücklich protokolliert. Negative PTS, beschädigte Pakete, verworfene Bilder innerhalb des Films, vorgelagerte Präsentationsbilder und nicht verifizierbare Anfänge werden abgelehnt. Ein ausdrücklich verworfenes Nachlaufpaket des letzten Abschnitts am/nach dem Dateiende wird als Paketinhalt geprüft, bestimmt aber keine sichtbare Grenze. Die Bildprüfung ist keine vollständige Dekodier-/Qualitäts-/Audioprüfung. Das Verfahren liest den komprimierten Videoinhalt vollständig; weitere Optimierung ist noch offen.

Das Ergebnis bindet seine Zeiten mit Dateigröße, Änderungszeit und SHA-256 an den Gesamtfilm. Vor dem Öffnen im Kontrollplayer wird diese Bindung erneut geprüft. Externe Änderungen während der Wiedergabe sind damit nicht ausgeschlossen. Fehlende Filmbilder werden durch richtige Prüfpositionen nicht wiederhergestellt; eine automatische Bildverlustdiagnose aus dem Original ist noch nicht enthalten.

## Bauen und Tests

Aus dem Repository-Hauptordner, mit einem vorhandenen CAN-Laufzeitordner einschließlich der vier nativen Bibliotheken:

```powershell
dotnet restore tools/prototypes/TransitionReview/TransitionReview.csproj --artifacts-path .build/transition-prototype/compiled
dotnet build tools/prototypes/TransitionReview/TransitionReview.csproj -c Release --no-restore --artifacts-path .build/transition-prototype/compiled
.build/transition-prototype/compiled/bin/TransitionReview/release/TransitionReview.exe --self-test
```

Standardmäßig wird der vorhandene lokale Ordner `artifacts/portable-1.0.0-build32-proof` nur gelesen. Alternativ bei Restore und Build `-p:CanAssemblyDirectory=<absoluter Laufzeitordner>` übergeben. Es gibt keine Projektverweise, keine neuen NuGet-Abhängigkeiten und keinen Aufruf des Buildzähler-Skripts. Produktions-Lockdateien werden nicht benötigt oder verändert.

Das Projekt verlinkt die vorhandenen Quellen für Kontrollfenster, Playback-ViewModel und Videohost. Das Kontroll-ViewModel unter Player/ ist der eingefrorene Stand vor der Produktintegration, damit der ursprüngliche Nachweis mit seiner vorhandenen CAN-1.0-Laufzeit reproduzierbar bleibt. Ein eigener Prototyp-Adapter liefert die Ausgabe-Prüfstellen direkt; er erzeugt keinen künstlichen Schnittplan. Das aktuelle Media-/Core-Assembly wird aus dem vorhandenen Laufzeitordner verwendet. Der zusätzliche native Bildlesehelfer dient nur den Prototyp-Prüfungen.

`Run-FixtureChecks.ps1` erzeugt bei ausdrücklichem Aufruf einen kleinen künstlichen Film und getrennte MP4Box-Ausgaben. Er verlangt einen neuen Ausgabeordner und die expliziten Pfade zu Prototyp, FFmpeg, ffprobe und MP4Box. Er prüft gemischte Segmentverschiebungen, einen einzelnen Abschnitt und die Ablehnung einer falschen Reihenfolge. Keine echten Aufnahmen werden eingebaut.

## Analyse und manuelles Prüfen

Eine lokale JSON-Anfrage enthält:

```json
{
  "FfprobePath": "ffprobe.exe",
  "OutputFilePath": "film.mp4",
  "SegmentFilePaths": ["segment-1.mp4", "segment-2.mp4"]
}
```

Relative Pfade gelten relativ zur Anfragedatei. Vollständige Werkzeugpfade sind zweckmäßig. Aufruf:

```powershell
TransitionReview.exe --analyze request.json result.json
TransitionReview.exe --review result.json 1
```

Die Ergebnisdatei muss neu sein; vorhandene Ergebnisse werden nicht überschrieben. Prüfstelle 0 ist der Anfang, 1 bis N sind die Übergänge, N+1 das Ende. Die vorhandene Playerlogik zieht bei Übergängen drei Sekunden ab, begrenzt kurze Vorläufe auf null und verwendet beim Ende die geladene tatsächliche Dauer. Das sichtbare Fenster ist ausdrücklich als Prototyp gekennzeichnet.

`--native-test result.json proof.json reference.json` verwendet eigene Fenster und den echten libmpv-Kontext. Eine Referenzdatei enthält `SourceFilePath` und `TransitionSourceStarts` mit je einem Originalbeginn pro Übergang. Der Prüflauf zeichnet die tatsächlichen Seek-Anforderungen auf, lässt die Wiedergabe über die Übergänge laufen und vergleicht das angezeigte neue Anfangsbild mit dem im Original. Diese Fenster sind stumm und verborgen; eine persönliche Bild-/Tonabnahme folgt daraus nicht. Alle erzeugten Nachweise gehören in einen lokalen ignorierten Arbeitsordner.

Der Bildlesehelfer verwendet die [libmpv-Abfrage `screenshot-raw`](https://mpv.io/manual/stable/#screenshot-commands) und gibt deren Speicher zuverlässig frei. Die PNG-Nachweise entstehen anschließend im Prototyp, unabhängig von einem Screenshot-Dateischreiber in libmpv.

`--native-plan-control control.json proof.json reference.json` ist ein ausdrücklich getrennter Vergleich für eine vorhandene otr-can-Datei. `control.json` enthält die absoluten Felder `FfprobePath`, `OutputFilePath` und `TransitionSeconds`. Dieser Modus prüft den bestehenden Playerablauf mit vorgegebenen Planzeiten und Originalbildern; er erzeugt **keine** verifizierte MP4Box-Ergebnisdatei. Die Nachweise sind als `plan-control-only` gekennzeichnet.
