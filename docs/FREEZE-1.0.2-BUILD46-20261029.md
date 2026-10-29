# CAN 1.0.2 Build 46 – eingefrorener Quell- und Paketstand

**29.10.2026.** Nach persönlicher Bestätigung von Furiosa und John Wick aus MP4Box-Teilen ausdrücklich beauftragt: Stand einfrieren, Dokumentation abgleichen, Setup/ZIP/Quellen erstellen, prüfen, anschließend Commit und Push. **Veröffentlichung erst nach erneuter Ansage.** Öffentlich bleibt Build 41; kein Tag, Release oder Asset-Upload bei diesem Auftrag.

Eingefroren wird die bestätigte Produktlogik **CAN 1.0.2.46**, ohne weitere Codeänderung oder Zählererhöhung. QUICK AND COPY: FFmpeg concat mit Streamcopy, bestätigte kompatible AAC-/AC-3-Mehrspurzuordnung, kurze Mindestprüfung und Meckerecke. Keine Ablehnung allein wegen technischer Warnungen, H.264-Konfigurationsunterschieden oder Sampleabweichungen; keine obligatorischen PCM-/Vollvideo-/Paketprüfungen. AUDIO-ENTHUSIAST bleibt streng mit direktem RF64 und einmaliger AAC-LC-Neukodierung. Originalschutz, Abbruch, Phasenzeiten, Erfolgsbereinigung und Drei-Sekunden-Kontrolle bleiben.

Motor **otr-can 0.1.6**, Wiedergabe **libmpv 0.41.0**, .NET 10/WPF für Windows 11 x64. Keine neue Produktionsabhängigkeit. Der separate Tonspurkopiedienst bleibt im Code erhalten, das Menü enthält nur Filmteile zusammenfügen.

373 Produkt-/Testdateien gegenüber Auftragseingang per Hash geschützt. Acht vorhandene Locks, Version.props, Buildzähler 46 und 40 oberste Auslieferungsdateien der Builds 40/41/43/44/45 gesichert. Vier vorbestehende lokale Produktions-Lockänderungen bleiben unangetastet und werden weder ins Quellenarchiv noch in den Commit übernommen; das Archiv verwendet committed Locks. Frühere Pakete bleiben erhalten.

Quellbasis d200ebf7567bceeab012ee9a2bb6b564635382a4 plus manifestierte Arbeitsänderungen. SOURCE-SNAPSHOT.json und SOURCE-FILES.sha256 beschreiben den exakten Snapshot. Paketierung ausschließlich daraus mit CanBuildNumber=46; danach keine nachträgliche Archivänderung. Der Paketbericht ergänzt spätere Prüfergebnisse und Git-Nachweis im Arbeitsrepository. Keine Videos, privaten Einstellungen oder Buildausgaben in Git oder im CAN-Quellenarchiv.

Zielordner **artifacts/release-1.0.2-build46**: Setup, Portable-ZIP, CAN-Quellen, passende unveränderte Motorquellen, libmpv-Prüfquellen/Relink und Prüfsummen. Nach erfolgreicher Prüfung acht oberste Auslieferungsdateien und manifestierte Quelldateien schreibschützen.

Entwicklung: 1576 Tests, 27 native QUICK- und 21 bisherige Fälle positiv; neun Enthusiast-Ausgaben bytegleich zu Build 45. Persönliche QUICK-Abnahme mit beiden Tonspuren: Furiosa 23,74 s und John Wick 16,44 s, jeweils Bild/Ton/Klebestelle/Drei-Sekunden-Vorschau. Neue Setup-/Portable-/VM-Abnahme bleibt gesondert.

[Paketbericht](RELEASE-1.0.2-BUILD46-RESULT.md), [aktuelle QUICK-Regeln und Praxis](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md), [Bedienung](NUTZERANLEITUNG.md).
