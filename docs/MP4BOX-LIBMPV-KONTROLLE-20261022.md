# Furiosa – libmpv-Prüfung der Diagnosekandidaten

Stand 22.10.2026. Auf ausdrückliche Nutzerfreigabe nach reproduzierter [Paket-/Decoderdiagnose](MP4BOX-UEBERGANGSDIAGNOSE-20261022.md). Separater Prüflauf, keine Änderung am produktiven Schnitt-/Playercode, keine neue CAN-Version, kein neuer Filmschnitt und keine Paketierung oder Veröffentlichung.

## Technisches Ergebnis

Vier reale Übergangsprüfungen mit zwei echten HWND-/libmpv-Kontexten bestanden. Das aktuelle CAN-Kontrollfenster und sein ViewModel sind unverändert als Quellen eingebunden. Die tatsächlich angeforderten Seek-Ziele entsprechen jeweils dem Diagnosekandidaten minus drei Sekunden. Die normale Wiedergabe läuft jeweils über den Übergang hinaus. Das erste neue Originalbild wurde anschließend im libmpv-Schrittfenster pixelgenau wiedergefunden.

| Übergang | Kandidat / neues Bild | Angeforderter Prüfstart | Im Player gemeldete Position des passenden neuen Bildes |
|---|---|---|---|
| 1 | 00:37:40,671729 | **00:37:37,671729** | 2260,671729 s |
| 2 | 00:50:53,653104 | **00:50:50,653104** | 3053,653104 s |
| 3 | 01:15:43,390604 | **01:15:40,390604** | 4543,390604 s |
| 4 | 01:34:26,080271 | **01:34:23,080271** | 5666,080271 s |

Die tatsächlichen Playerpositionsmeldungen am Wiedergabestart passen in diesem Lauf auf die angeforderten Zeiten bis auf die gezeigte Mikrosekundenrundung. Beide Player wurden vor ihren Videohosts freigegeben. Vier separate Grenznachweise, Originalreferenzen, Bildschritte und Gesamtnachweis liegen lokal unter `.build/diagnosis-review-20261022/native-apphost/`. Der Gesamtnachweis enthält Medienidentitäten und den SHA-256 des verwendeten Nutzerberichts `.build/meine-uebergangsdiagnose.json`.

## Erster Übergang: wichtige Einschränkung

Trotz der aus ffprobe bekannten rückläufigen PTS wird das **erste neue Originalbild bei 3607,720 s** im libmpv-Schrittfenster an der Kandidatenposition wiedergefunden. Das letzte alte Referenzbild aus dem Original bei **2845,000 s** wurde in diesem Schrittfenster **nicht** beobachtet. An den drei anderen Grenzen wurden jeweils sowohl die letzte alte Referenz als auch der neue Anfang beobachtet.

Dieser Unterschied bleibt ausdrücklich offen. Daraus folgt weder, dass der fertige Film alle gewünschten Bilder enthält, noch dass die letzte alte Referenz bei jeder Art von Wiedergabe verworfen wird. Der Lauf beobachtet normale Wiedergabe über die Grenze und vergleicht danach Bilder im pausierten Schrittmodus. Er ist kein vollständiger Bildmitschnitt der laufenden Wiedergabe und keine allgemeine Reparatur überlappender Präsentationszeiten.

Die Diagnosezeiten werden im separaten Kontrollplayer weiterhin als **Kandidaten**, nicht als produktiv integrierte Ausgabezeiten geführt. Die persönliche Bewertung des separaten Prüflaufs ist inzwischen positiv bestätigt; siehe folgenden Abschnitt. Der technische Befund zum letzten alten Referenzbild bleibt davon getrennt erhalten.

## Persönliche Prüfung am 22.10.2026

Nach Öffnen des separaten Kontrollfensters für die vier Furiosa-Übergänge bestätigt der Nutzer ausdrücklich: **„Schnitt, Bild, Ton und 3 Sekunden check“**.

Damit sind Schnitt, Bild, Ton und Drei-Sekunden-Vorlauf im angebotenen persönlichen Prüflauf positiv bewertet. Die Rückmeldung enthält keine getrennten Einzelmesswerte pro Übergang oder Einzeltonspur und wird nicht zu einer vollständigen Bildzählung oder gesonderten Synchronitätsmessung erweitert. Die technische Nichtbeobachtung der letzten alten Bildreferenz am ersten Übergang bleibt dokumentiert.

Dies ist die persönliche Abnahme des **separaten Prototyp-Prüflaufs**, keine bereits erfolgte Produktintegration, neue Versionsfreigabe oder Klärung der früheren Systemhänger. Der nächste kleine Entwicklungsschritt kann eine begrenzte Integration der abgesicherten Zuordnung sein; dafür bleibt ein eigener Auftrag erforderlich.

## Umsetzung und Bedienung

Neues Werkzeug `tools/prototypes/DiagnosisReview`, mit der bestehenden CAN-Release-Laufzeit. Kontrollfenster, Kontroll-ViewModel, Abschlussmodell, Playback-ViewModel und Videohost stammen direkt aus den aktuellen Produktquellen. Die bereits erprobten nativen Screenshot- und Seek-Recorder-Helfer des älteren Prototyps werden unverändert wiederverwendet. Keine neue Produktionsbibliothek und keine Änderung an der Produktions-Solution.

Der sichtbare Player startet am ersten Übergang. **Nächste Prüfstelle** wechselt zu Übergang 2–4 und danach zum Ende; zurück geht es über **Vorherige Prüfstelle**. Lautstärke startet bei null. Die persönliche Frage lautet je Übergang: Beginnt die Wiedergabe ungefähr drei Sekunden vor dem tatsächlichen Szenenwechsel und läuft der Übergang plausibel weiter?

Lokaler Wiederöffnungsstarter: `.build/diagnosis-review-20261022/Start-Furiosa-Kontrolle.cmd`. Er startet die Windows-Programmdatei aus ihrem Laufzeitordner und nutzt den vorhandenen Nutzerbericht. Kein Bauen oder Schneiden beim Öffnen. [Technische Anleitung und Prüfgrenzen](../tools/prototypes/DiagnosisReview/README.md).

## Validierung und Erhaltung

Eigenständiger Release-Build ohne Warnungen/Fehler. **40/40 Prototyptests** inklusive elf neuer Fälle für Diagnoseübergabe, Referenzseiten, unveränderte Behandlung rückläufiger Kandidaten, abweichende Bildanker, unvollständige Daten und Dateiidentität. Bestehende 29 Diagnosefälle bleiben erhalten.

Der erste native Start über `dotnet DiagnosisReview.dll` scheiterte beim Laden der libmpv-Begleitdateien und zählt nicht als bestandener Lauf. Der Start über die eigene EXE mit dem Laufzeitordner als Arbeitsverzeichnis lief erfolgreich. Keine native Bibliothek ersetzt und keine Systemkonfiguration verändert.

Abschließender Lösungsbuild: **0 Warnungen, 0 Fehler**. Bestehende Regressionstests **1385/1385** bestanden (Core 199, Media 224, App 890, Cutlists 72), keine übersprungen; getrennte TRX-Nachweise unter `.build/diagnosis-review-20261022/regression-results`. Zusätzlich **40/40** Prototyptests. Formatprüfung der neuen C#-Dateien ohne ausstehende Änderungen und `git diff --check` erfolgreich.

Alle acht vorhandenen Lockdateien und der Buildzähler sind per SHA-256 unverändert gegenüber dem Ausgangszustand; neun erfolgreiche Erhaltungsprüfungen. Deshalb direkter Release-Build mit expliziter Buildnummer 34 statt des zählererhöhenden Skripts. Quellen-/Laufzeitpins und Gesamtnachweise liegen ausschließlich im ignorierten Nachweisverzeichnis. Die vorbestehenden Nutzeränderungen bleiben erhalten.

Persönliche Schnitt-/Bild-/Ton-/Drei-Sekunden-Prüfung im oben genannten Umfang bestätigt; keine gesonderte Synchronitätsmessung und kein Nachweis einer behobenen Systemfreeze-Ursache. **1.0.0 Build 32 bleibt freigegebener Stable-Stand; Build 34 bleibt Draft.** Eine produktive Erweiterung der Übergangsbestimmung benötigt einen eigenen begrenzten Auftrag nach Bewertung dieser Ergebnisse.

Die persönliche Rückmeldung wurde ausschließlich in diesem Bericht, Status und Tagebuch ergänzt. Kein neuer Build oder Testlauf für den reinen Dokumentationsnachtrag; bestehender Nachweis weiterhin 40/40 Prototyptests, 1385/1385 Regressionstests und Release ohne Warnungen/Fehler. Anwendung, Motoren, Medien, Lockdateien und Buildzähler unverändert.
