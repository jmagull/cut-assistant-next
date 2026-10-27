# Manuelle Tonspurauswahl für weitere Filmteile

**Historischer Build-38-Baustein. Aktuell CAN 1.0.2 Build 40.** Seit Build 39 erfolgt die Tonspurauswahl nach Dateien/Reihenfolge direkt im [Klebecenter](KLEBECENTER-20261027.md); separater Menüpunkt und Vorbereitungskopie entfallen. Die folgenden Bedien-/Prüfangaben erhalten den damaligen Stand. [Versionen](VERSIONEN.md).

Stand **26.10.2026**. In den lokalen **Build-38-Paketen** enthalten, technisch geprüft und vom Nutzer im realen manuellen Bedienweg erfolgreich durchlaufen. Keine Veröffentlichung.

## Bedienung

Den **bereits geschnittenen Filmteil** laden. Menü **Tonspuren → Tonspur für Filmteile auswählen …** öffnen und dieselbe geeignete Tonspur wie beim anderen Filmteil bewusst auswählen, beispielsweise Deutsch/AAC/Stereo. Es gibt keine Vorauswahl. Der Dialog benennt alle in der neuen Kopie entfallenden Tonspuren.

„Zusätzliche Kopie speichern …“ fragt nach einem neuen MP4-Dateinamen. Die gesamte geladene Datei wird kopiert; eine geladene Schnittliste wird **nicht angewendet**. Video und gewählte Tonspur werden nicht neu kodiert. Vorhandene Dateien werden nicht überschrieben; die bisherige Datei und der aktuelle Schnittzustand bleiben erhalten. Die Funktion speichert keine globale Tonspurvoreinstellung.

Wählbar sind AAC und AC-3 in einer MP4 mit genau einer Videospur und mindestens zwei Tonspuren. CAN vergleicht keinen anderen Filmteil und führt die Teile nicht zusammen. Sprache, Codec, Kanalzahl und Samplerate sind sichtbar, damit der Nutzer die passende Spur wählen kann.

## Umsetzung

Neue Dienste **AudioTrackCopyService** und **SelectedAudioSource**, neuer Dialog **AudioTrackCopyDialog**, Menüanbindung in **MainWindow.AudioTrackCopy.cs**. Der Fortschrittsdialog kann eine passende Operationsüberschrift zeigen. Die Hintergrundverarbeitung unterstützt Abbruch; Medienwechsel/Drag & Drop sind während des Vorgangs gesperrt. Der bestehende automatische AAC-Notfallweg verwendet dieselbe streng geprüfte Kopiervorbereitung, behält aber seine bisherige ausdrückliche Zustimmung und AAC-Beschränkung.

Die ausgewählte Spur wird vollständig mit Fehlerabbruch dekodiert. Jede Decoderwarnung führt zur Ablehnung. FFmpeg kopiert anschließend ausgewähltes Video und Audio mit ursprünglichen Zeitstempeln in einen privaten Arbeitsbereich. Erst nach erfolgreicher Prüfung wird die neue Zieldatei atomar angelegt. Fehler, Abbruch oder eine zwischenzeitlich angelegte Zieldatei erhalten bestehende Dateien und bereinigen den Arbeitsbereich.

Die Prüfung vergleicht Codec, Decoderkonfiguration, Sprache/Titel, sämtliche Paket-Hashes, Größen, Flags, PTS/DTS, Dauer und Skip-/Discard-Padding. Neu werden auch **start_pts und duration_ts jeder Spur exakt zwischen den Zeitbasen verglichen**. Das schützt vor veränderter MP4-Wiedergabebeschneidung trotz identischer Pakete.

AC-3 trägt die Decoderkonfiguration im Bitstrom; bei fehlender separater Extradata wird deren Inhalt durch alle Paket-Hashes abgedeckt. Genau der bekannte MP4-Muxerhinweis „track 1: codec frame size is not set“ darf ausschließlich während AC-3-Streamcopy auftreten, bleibt im Protokoll sichtbar und ersetzt keine Prüfung. Alle Decoderwarnungen und anderen Kopierwarnungen werden weiterhin abgelehnt.

## Prüfungen

Release-Build **0 Warnungen/0 Fehler**, **1446/1446 Tests**: 200 Core, 72 Cutlists, 249 Media, 925 App. Format-/Whitespaceprüfung bestanden. Keine Produktionsabhängigkeit hinzugefügt.

**Zehn native Fälle** mit echten Werkzeugen bestanden: vollständige manuelle AAC- und AC-3-Kopie; Ablehnung geänderter AC-3-Wiedergabedauer trotz unveränderter Pakete; Ablehnung beschädigter gewählter AC-3-/AAC-Spuren; erfolgreiche AAC-Auswahl bei beschädigtem entfallendem AC-3; außerdem alle vier bisherigen automatischen Notfall-/Normalfälle. Originale aller Fälle SHA-256-gleich, private Arbeitsdateien bereinigt.

Dekodiertes PCM der vollständigen manuellen AAC-Kopie: **864000 Samples**, bytegleich zur Quelle. AC-3-Fall mit unverändertem Wiedergabezeitraum: **864512 Samples**, ebenfalls bytegleich (konsistente AC-3-Rauscherzeugung im unabhängigen Vergleich). Eine Quelle mit auf 18 Sekunden beschnittenem AC-3 würde durch einfaches Streamcopy 512 zusätzliche Samples präsentieren; genau dieser Fall wird nun abgelehnt.

Der Dialog wurde separat gerendert und visuell geprüft. Zusätzlich geprüft: anfangs keine Auswahl, Speichern deaktiviert, explizite Auswahl aktiviert Speichern und zeigt entfallende Spuren.

**Echter John-Wick-Teil 1:** Bereits fertig geschnittene Ausgabe über den neuen manuellen Dienst in eine zusätzliche AAC-Datei kopiert. Vollständige Dekodier-/Paket-/Wiedergabezeitprüfung erfolgreich; keine neue Kodierung. Unter 1-GiB-/25-Prozent-Windows-Job-Grenzen nach rund 25 Sekunden erfolgreich, maximal rund 149 MiB Commit. Die bisherige Datei wurde nicht geändert.

Lokale Details im ignorierten Ordner **.build/audio-track-copy-20261026/**: NATIVE-RESULT.json, PCM-PROOF.json, cases3/, dialog/, real-monitor/ und john-wick-part1/. Reale Medien bleiben außerhalb von Git.

## Grenzen und Abnahme

MP4-Streamcopy kann eine vorhandene Wiedergabebeschneidung nicht in jedem Fall exakt übernehmen. Dann wird die Kopie verworfen und eine verständliche Fehlermeldung gezeigt; es gibt keine automatische Neukodierung oder Zeitkorrektur. Andere Audioformate und mehrere Videospuren sind nicht freigegeben.

Der automatische AAC-Notfallweg nach Bereitstellung von Build 37 und die neue manuelle Tonspurauswahl nach Bereitstellung von Build 38 sind persönlich durchlaufen. Der Nutzer liefert das vollständige erfolgreiche Kopierprotokoll sowie den Screenshot der Abschlussmeldung für **John Wick Kapitel 4 Teil 1 otr-can - AAC.mp4**. Ein separater neuer Portable-/VM-/Deinstallationsnachweis oder eine persönliche Abbruch-/Fehlerfallabnahme liegt damit nicht vor. Keine Änderung am Motor 0.1.6 und keine behauptete Behebung früherer Systemhänger. Veröffentlichung nur nach ausdrücklicher Freigabe.

## Erster Baustein für das Klebecenter

Die passende Teil-1-AAC-Kopie wurde anschließend mit der bestätigten Teil-2-AAC-Ausgabe separat durch FFmpeg zusammengefügt. Der Nutzer bestätigt den sauberen Übergang. Das Zusammenfügen ist noch nicht in CAN eingebaut; die manuelle Tonspurauswahl ist der erste Baustein. [Geprüfter Referenzfall](JOHN-WICK-4-KLEBECENTER-REFERENZ-20261026.md), [weitere Planung](SETUP-MERKLISTE.md).
