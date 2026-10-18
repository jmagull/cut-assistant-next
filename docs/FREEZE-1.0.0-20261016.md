# Funktions-Freeze für Cut Assistant Next 1.0.0

Stand: **16.10.2026**. Der Nutzer erklärt den persönlich geprüften Stand für eingefroren und „1.0 Release ready“ und beauftragt anschließend **„jetzt setup, zip etc. etc.“**. Damit sind Funktions-Freeze und die Erstellung des stabilen lokalen Paketsatzes beauftragt.

Ziel: **1.0.0 · Build 32**, ohne RC-Kennung. Build 32 folgt auf den bisherigen erfolgreichen Vollbuild 31. Die neue Buildnummer wird erst nach erfolgreichem vollständigem Build-/Testlauf als lokaler Zähler übernommen. Bestehende RC-Releases, Tags, Archive und Prüfsummen bleiben erhalten.

## Eingefrorener Funktionsumfang

- Optionaler Videopfad beim Kommandozeilenstart, Laden über den bestehenden CAN-Ladeweg.
- Drag & Drop von Videodateien direkt in den Player; bestehende Schnittarbeit vor einem Dateiwechsel geschützt.
- Standard- und Power-User-Ansicht in den Einstellungen; ein gemeinsamer Player, Schnittzustand und erhaltene Position beim Ansichtswechsel.
- Persönlich geprüfter Layout-Feinschliff: großer Player links, Schnittliste und klar gruppierte Schnitt-/Cutlist-Werkzeuge rechts.
- Bestehende Frame-/Keyframe-Navigation, Frame-Lupe, Frame-Liste, Halbierungssuche, Cutlist-Erstellung/-Verwaltung, Namensmasken und Kontrollplayer.
- Zwei bestehende Schnittmotoren: MP4Box und unverändertes **otr-can 0.1.5**.
- AVI-Schutzmaßnahmen und Zeitstrukturvorprüfung vor dem otr-can-Aufruf bleiben erhalten.

Ab jetzt stehen Fehlerbehebung, Stabilisierung, Regression, Dokumentation und Release-Vorbereitung im Vordergrund. Neue größere Funktionen erfordern eine ausdrückliche Nutzerentscheidung.

## Persönliche Abnahme

Kommandozeilenübergabe mit realer HD-Aufnahme und Drag & Drop persönlich bestätigt. Anschließend ausdrücklich gemeldet: **Frame-Lupe geprüft**, **Frame-Liste geprüft**, **MP4Box-Schnitt erfolgreich**, **otr-can-Schnitt erfolgreich**, **Power-User-Layout persönlich geprüft**. Der anschließende Screenshot zeigt den geladenen Film und die gefüllte Schnittliste im neuen Layout.

Radical mit otr-can 0.1.5 ist vom Nutzer nach Bild, Ton und allen Übergängen als blitzfrei bestätigt. Der erfolgreiche MP4Box-Ablauf hebt den bekannten Werbeblitzer bei **57:27.314** nicht auf; die technische Grenze bleibt dokumentiert. [Persönliche und native Layoutprüfung](POWER-USER-FEINSCHLIFF-20261016.md), [Radical-Befund](RADICAL-MP4BOX-WERBEBLITZER-20261016.md).

## Pakete und Quellen

Erstellter und technisch geprüfter lokaler Paketsatz: `artifacts/release-1.0.0-build32`.

- Setup und Portable-ZIP, Windows x64, abhängig von der separat installierten .NET-10-Desktop-Laufzeit.
- Vollständige passende CAN-Quellen mit Herkunft und Dateimanifest.
- Unveränderte otr-can-0.1.5-Quellen und zugehörige Lizenz-/Herkunftsnachweise.
- Geprüftes libmpv-Quellen-/Prüfpaket und Relink-Archiv mit bestehenden Lizenzen.
- SHA-256-Prüfsummen aller auszuliefernden Dateien, Publish-Dateimanifest sowie Build-, Test- und Installerprotokolle.
- Benutzeranleitung und [Release Notes](RELEASE-NOTES-1.0.0.md).

Quell- und Paketnachweise werden im [Ergebnisbericht](RELEASE-1.0.0-BUILD32-RESULT.md) festgehalten. Die vier vorbestehenden lokalen Produktions-Lockänderungen bleiben erhalten und werden wie bei den RC-Paketen aus dem Release-Quellstand ausgeschlossen; verwendet werden die bestehenden Git-Lockdateien. Ein freigegebener Quellcommit ist vor der endgültigen Veröffentlichung erforderlich; ein eventuell vorbereiteter Arbeitsbaum-Snapshot wird ausdrücklich als solcher gekennzeichnet.

## Verbleibende Freigaben und Grenzen

Die Funktionsabnahme und der lokale Paketauftrag sind erteilt. **Die GitHub-Veröffentlichung wurde nach der vollständigen persönlichen Paketabnahme am 18.10.2026 ausdrücklich beauftragt.** Der lokale Quellcommit wurde anschließend ausdrücklich freigegeben; Commit und Push wurden am 17.10.2026 beauftragt und ausgeführt. Tag **v1.0.0** und stabile GitHub-Veröffentlichung als Latest sind am 18.10.2026 gesondert freigegeben und mit vollständiger Uploadprüfung ausgeführt; [Nachweis](RELEASE-PUBLICATION-1.0.0.md). Am **18.10.2026** sind **Setup und Deinstallation in der VM** samt dokumentierten Funktionsprüfungen persönlich bestätigt. **Portable/ZIP in der VM ist anschließend ebenfalls persönlich erfolgreich geprüft**, einschließlich anderem otr-can-Pfad, MP4-Schnitten mit beiden Motoren samt Bild/Ton und Halbierungssprung sowie AVI-Schnitten mit beiden Motoren. Damit sind die geplanten persönlichen Setup-/Deinstallations-/Portable-Prüfungen des neuen Pakets abgeschlossen; ältere Build-31-Abnahmen werden dafür nicht herangezogen. Echte DPI-/Mehrmonitorwechsel sind noch nicht persönlich bestätigt.

AVI bleibt experimentell. Ungeeignete Zeitstrukturen, Codec-/Audio-/Decodergrenzen und bekannte MP4Box-Grenzen bleiben transparent dokumentiert. Version 1.0 verspricht keine fehlerfreie Unterstützung sämtlicher Medien. Externe FFmpeg/ffprobe-, GPAC/MP4Box- und FFMS2-Werkzeuge sowie für otr-can die passende VC++-Laufzeit bleiben erforderlich. Keine neue bitidentische Reproduzierbarkeit der unveränderten Motor-EXE behauptet.

## Paketabschluss

Lokaler Quellcommit **3e2ac03dfe4a701ecbc95c80cc06ff2527309b9f**; Setup, Portable, passende Quellen-/Lizenzarchive und Prüfsummen erstellt und schreibgeschützt eingefroren. **1347/1347 Pakettests**, Release **0 Warnungen/0 Fehler**, 13 native Fallgruppen erfolgreich. Buildzähler nach bestandenem Paketlauf auf 32 übernommen. VM-Setup, Deinstallation und Portable sind am 18.10.2026 persönlich bestätigt; Tag v1.0.0 und stabile Latest-Veröffentlichung wurden anschließend am 18.10.2026 ausdrücklich freigegeben und ausgeführt; [Ergebnis und Prüfsummen](RELEASE-1.0.0-BUILD32-RESULT.md).
