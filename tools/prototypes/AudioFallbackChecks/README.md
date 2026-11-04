# Native Audio-/MP4Box-Prüfung

Dieses Prüfprogramm referenziert fertige Produkt-DLLs und baut CAN nicht neu. AssemblyName entspricht dem vorhandenen InternalsVisibleTo-Testnamen. Es ist kein Produktbestandteil. Keine zusätzliche Produktionsabhängigkeit.

Vorher CAN vollständig bauen/publizieren und den Diagnosemotor separat bauen. Dann:

~~~powershell
dotnet build .\tools\prototypes\AudioFallbackChecks\AudioFallbackChecks.csproj --configuration Release -p:CanAssemblyDirectory='<vollständiger Paketordner>'
py -3 .\tools\prototypes\AudioFallbackChecks\run_checks.py --work '<neuer eigener Ausgabeordner>' --harness '<gebaute CutAssistantNext.App.Tests.exe>' --engine '<Diagnosemotor.exe>' --indexer '<ffmsindex.exe>' --ffmpeg '<ffmpeg.exe>' --ffprobe '<ffprobe.exe>' --mp4box '<MP4Box.exe>'
~~~

Der Ausgabeordner muss neu sein. Nur künstliche H.264/AAC-Dateien werden erzeugt, mit Movie-Timebase 48000. Gezielte stts-Änderungen bei unveränderten Paketbytes schaffen innere Lücken. Für die eindeutigen Fälle werden die Audio-Endmetadaten mitgeführt; die unklassifizierte Negativklasse mischt eine positive Lücke mit einer Überlappung. Keinen echten Film als Patch-Eingabe verwenden.

Erwartet: zwölf neue Fälle mit tatsächlichen Werkzeugaufrufen; bei zusätzlich übergebenem --legacy-fixtures-Verzeichnis die vier bisherigen synthetischen AAC-/AC-3-Notfallfälle, insgesamt 16. Diese optionalen Fixtures stammen aus der früheren CAN-Prüfung und sind nicht in Git enthalten. Jede Entscheidung ist ein ausdrücklich simulierter Callback; das ist keine persönliche Dialogabnahme.

Die Fälle prüfen Befund, exakte innere/endständige Fehlmengen, unbekannte Strukturen, Auswahl nach/außerhalb von Lücken, Dateierhalt und vollständige Bereinigung sowie einen tatsächlich ausgeführten MP4Box-Neustart. results.json enthält Quell-, App- und Motorprüfsummen. Das Original wird streamend gehasht, nicht vollständig in den Speicher geladen.

Zum Rendern des echten WPF-Rückfragedialogs:

~~~powershell
& '<Prüfprogramm.exe>' render '<eigener Bildausgabeordner>'
~~~

Ein privater NCIS-Kurzreferenzlauf wurde zusätzlich getrennt durchgeführt. Dafür wird eine eigene plan.json mit den halboffenen Keep-Zeitpaaren übergeben und der native decline-Modus direkt verwendet. Die reguläre Matrix bleibt rein synthetisch. Reale Filme und Protokollausgaben gehören ausschließlich in ignorierte Arbeitsordner.
