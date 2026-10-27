# Diagnosekandidaten mit dem CAN-libmpv-Kontrollplayer prüfen

**Einordnung 27.10.2026:** Dieses README beschreibt den isolierten Diagnose-/Kontrollprototyp, keine aktuelle CAN-Paketversion. Produktive Schnitt-/Klebekontrollen sind separat dokumentiert. Aktuell **CAN 1.0.2 Build 40 / otr-can 0.1.6**. [Projektstand](../../../docs/AKTUELLER-STAND.md), [Versionen](../../../docs/VERSIONEN.md).

Separater Windows-/WPF-Prototyp für den JSON-Bericht von `TransitionDiagnosis`. Keine Einbindung in CAN oder Änderung seiner Schnittlogik. Er verwendet die **aktuellen unveränderten Quellen** von Kontrollfenster, Kontroll-ViewModel, Abschlussmodell, Playback-ViewModel und Videohost. Core, Media, Cutlists und die vier libmpv-Dateien stammen aus einer vorhandenen CAN-Release-Laufzeit.

## Build und Tests

Voraussetzung ist der vorhandene Release-Ausgabeordner `src/CutAssistantNext.App/bin/Release/net10.0-windows`. Alternativ beim Build `CanAssemblyDirectory` auf eine passende vorhandene Laufzeit setzen.

```powershell
$diagnosisTargets = Join-Path $PWD 'tools/prototypes/TransitionDiagnosis/NuGetIsolation.targets'
dotnet restore tools/prototypes/DiagnosisReview/DiagnosisReview.csproj "-p:CustomAfterMicrosoftCommonTargets=$diagnosisTargets"
dotnet build tools/prototypes/DiagnosisReview/DiagnosisReview.csproj -c Release --no-restore "-p:CustomAfterMicrosoftCommonTargets=$diagnosisTargets"
dotnet build tools/prototypes/TransitionDiagnosis.Tests/TransitionDiagnosis.Tests.csproj -c Release --no-restore "-p:CustomAfterMicrosoftCommonTargets=$diagnosisTargets"
dotnet test tools/prototypes/TransitionDiagnosis.Tests/TransitionDiagnosis.Tests.csproj -c Release --no-build --no-restore
```

Die Testsuite enthält zusätzlich die reinen Tests der Diagnoseübergabe und Dateizuordnung. Keine neue Produktionsabhängigkeit, kein Buildzähler-Inkrement.

## Sichtbare Kontrolle

Die **eigene Windows-Programmdatei** aus ihrem Laufzeitverzeichnis starten. Der Start über `dotnet DiagnosisReview.dll` ist nicht der geprüfte Weg: Bei der ersten Erprobung konnten dabei die nativen Begleitdateien nicht geladen werden.

```powershell
$reviewDirectory = Join-Path $PWD 'tools/prototypes/DiagnosisReview/bin/Release/net10.0-windows'
$diagnosisReport = (Resolve-Path '.build/meine-uebergangsdiagnose.json').Path
Start-Process -FilePath (Join-Path $reviewDirectory 'DiagnosisReview.exe') -WorkingDirectory $reviewDirectory -ArgumentList @('--review', ('"' + $diagnosisReport + '"'))
```

Der erste Übergang startet automatisch. **Nächste Prüfstelle** führt zu den weiteren Übergängen, anschließend zum Ende; **Vorherige Prüfstelle** führt zurück, davor zum Anfang. Die Lautstärke startet bei null und lässt sich im Fenster einstellen. Es gibt keine neue Hauptanwendung, Installation oder Änderung der persönlichen CAN-Einstellungen.

Ein optionaler dritter Parameter nach dem Bericht wählt die Prüfstelle: 0 Anfang, 1 erster Übergang, aufsteigend bis zum Filmende. Zum Wiederholen derselben Prüfstelle gegebenenfalls einmal zur vorherigen und wieder zur gewünschten Stelle wechseln.

## Automatisierter nativer Nachweis

`DiagnosisReview.exe --native-test <diagnosis.json> <neues-nachweisverzeichnis>` startet zwei stumme verborgene WPF-Videofenster. Dafür ebenfalls die EXE und ihr Laufzeitverzeichnis als Arbeitsordner verwenden. Ein vorhandenes Nachweisverzeichnis wird abgelehnt. Der gesamte automatische Lauf ist auf vier Minuten begrenzt; einzelne Bereitschaftsprüfungen auf 25 Sekunden.

Für jeden Übergang:

1. Letztes altes und die ersten bis zu drei neuen Referenzbilder aus dem Original im tatsächlichen libmpv-Bildspeicher erfassen.
2. Das unveränderte CAN-Kontroll-ViewModel aufrufen und den tatsächlichen Seek-Auftrag auf Kandidat minus drei Sekunden prüfen; Vorläufe unter drei Sekunden werden wie im Produkt auf null begrenzt.
3. Normale Wiedergabe bis über den Übergang beobachten, anschließend in einem lokalen Schrittfenster libmpv-Bilder erfassen.
4. Den Pixel-SHA-256 des ersten neuen Originalbildes im Schrittfenster vergleichen und die gemeldete Playerposition protokollieren. Pixelgleiche benachbarte Referenzbilder gelten als mehrdeutig und werden nicht als eindeutiger erster Bildanker akzeptiert.

`Passed` bestätigt diesen technischen Ablauf: angeforderter Seek, fortgesetzte Wiedergabe und eindeutiger Bildanker im Schrittfenster nahe der Kandidatenposition. Die Positionstoleranz des Schrittvergleichs beträgt 81 ms; Playerpositionsmeldungen sind keine unabhängig gemessenen Bildzeitstempel. Der Pixelvergleich im Schrittfenster beweist nicht, dass jedes Bild auch während normaler Wiedergabe erscheint. Ob die letzte alte Referenz im Schrittfenster beobachtet wurde, bleibt separat sichtbar.

Original, Ausgabe und Diagnosebericht bleiben gegen Schreiben/Löschen gesperrt. Original und Ausgabe werden vor dem Start mit Größe, Änderungszeit und vollständigem SHA-256 an den Bericht gebunden; das aktuelle Kontroll-ViewModel prüft die Ausgabeidentität zusätzlich. Nachweise enthalten den SHA-256 des verwendeten Diagnoseberichts. Medien fehlen oder wurden verändert: verständlicher Fehler statt Verwendung alter Zeiten. Nachweise und PNGs gehören ausschließlich unter `.build/`.

Auch nach bestandenem nativem Lauf bleiben die Werte im Abschlussmodell ausdrücklich `Estimated`, einschließlich Diagnosehinweis. Keine automatische Freigabe als produktive Messung, Bild-/Tonabnahme oder Release. Beide nativen Player werden vor dem Schließen ihrer Videohosts freigegeben.

Reale Ergebnisse: [Furiosa-libmpv-Prüfung vom 22.10.2026](../../../docs/MP4BOX-LIBMPV-KONTROLLE-20261022.md).

## Produktive Integration prüfen

`DiagnosisReview.exe --cut-and-native-test <diagnosis.json> <neues-verzeichnis> <mp4box.exe> <ffprobe.exe>` führt zusätzlich einen **neuen echten MP4Box-Schnitt** aus. Dafür ebenfalls die eigene EXE mit ihrem Laufzeitverzeichnis als Arbeitsordner starten. Vorhandene Nachweisverzeichnisse werden abgelehnt. Die Quell- und Cutlist-Identitäten sowie die 25-fps-Referenz werden vorab geprüft. Der Aufruf lädt den tatsächlich gebauten `Mp4BoxCutService` aus der CAN-Anwendung; keine nachgebaute Schnittlogik. Ausgabe und Nachweise entstehen ausschließlich im angegebenen neuen Verzeichnis.

Der Lauf erwartet `Located`, die Überlappungskennzeichnung und exakt die vier auf .NET-Ticks gerundeten Referenzzeiten. Anschließend erhält der aktuelle Kontrollplayer **das tatsächliche produktive CutResult**. Der bisherige Diagnosemodus bleibt dagegen `Estimated`. `CUT-RESULT.json`, `CUT-LOG.txt` und `native/NATIVE-PROOF.json` unterscheiden die beiden Prüfwege ausdrücklich. Der Gesamtaufruf ist auf 20 Minuten begrenzt, der anschließende native Teil zusätzlich auf vier Minuten. Keine persönliche Ton-/Bildabnahme oder Releasefreigabe daraus abgeleitet.
