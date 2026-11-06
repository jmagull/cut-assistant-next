# Cut Assistant Next 1.0.3 Build 52 – eingefrorener Stand

Der am **05.11.2026** persönlich geprüfte Build 52 ist der lokale Paketsatz für Windows 11 x64. Produktname und Verknüpfungen lauten **Cut Assistant Next**. Die Dateiversion ist **1.0.3.52**.

## Funktionen und persönliche Prüfung

- **Tonspur-Abwahl.** Bei John Wick – Kapitel 4, Teil 2 zeigte der persönlich geprüfte Dialog die fehlerhafte AC-3-Spur 2 und den Erhalt von AAC-Spur 1. Der vollständige technische Neustart lieferte 61.801 Videopakete und 118.657.920 dekodierte AAC-Samples. [Bedienung und Prüfgrenzen](AUDIO-SPURABWAHL-BUILD52.md)
- **Experimentelle AAC-Reparatur.** Für NCIS Sydney – Breathless bestätigte der Anwender den Praxistest. Die technische Prüfung wies 512 positionsgenau ergänzte Samples und eine bytegleiche Ausgabe zum persönlich geprüften Build-51-Referenzfilm nach. [Audioablauf](AUDIO-DREI-OPTIONEN-20261105.md)
- **Klebecenter.** Beide Verfahren wurden mit den zwei otr-can-Teilen von John Wick persönlich bestätigt. QUICK AND COPY lief in 19,03 s und meldete 21,3 ms Laufzeitunterschied zwischen Bild und Ton. AUDIO-ENTHUSIAST lief in 5:06,46 min und verifizierte 457.392.000 Tonsamples sowie die Klebestelle bei 01:57:36.960.
- **Spotlight-CAN.** Die visuelle Werbesuche ist über das Werkzeugmenü erreichbar. Die dynamische Anzeige zählt vollständige Entfernen-Bereiche, berücksichtigt Überlappungen einmal und aktualisiert Original-, Entfernen- und Restlaufzeit. [Spotlight-Integration](SPOTLIGHT-INTEGRATION-20261104.md)

## Persönliche Setup-Tests vom 06.11.2026

Joerg hat das Windows-x64-Setup des eingefrorenen Build 52 installiert und drei Ergebnisse persönlich geprüft:

- **John Wick – Kapitel 4, Teil 2.** Der installierte Motor erkannte den Decoderfehler der AC-3-Spur. Nach Bestätigung schnitt CAN mit der AAC-Spur erneut. Die MP4-Prüfung bestätigte 61.801 Videopakete und 118.657.920 dekodierte AAC-Samples.
- **NCIS Sydney – Breathless.** Nach dem kontrollierten Erstabbruch lief die bestätigte Reparatur der inneren Lücke mit 512 Samples vollständig durch. Die protokollierte Ausgabeprüfsumme stimmt mit der des geprüften Referenzfilms überein.
- **A 90s Christmas – Eine weihnachtliche Zeitreise.** Der reguläre otr-can-Schnitt mit vier Behaltebereichen lief erfolgreich durch. Verifiziert wurden 119.713 Videopakete und 229.848.960 AAC-Samples bei 48 kHz. Bild und Ton ergeben rechnerisch 01:19:48.520.

## Pakete und Voraussetzungen

| Datei | Inhalt |
|---|---|
| `CutAssistantNext-1.0.3-Build52-win-x64-Setup.exe` | Windows-x64-Setup mit der bisherigen CAN-Installationskennung |
| `CutAssistantNext-1.0.3-Build52-win-x64.zip` | Portable Anwendung zum vollständigen Entpacken in einen eigenen Ordner |
| `CutAssistantNext-1.0.3-Build52-source.zip` | CAN-Quellen einschließlich der eigenständigen Spotlight-CAN-Quellen, Tests und Dokumentation |
| `otr-can-0.1.6-audio-diag.1-source.zip` | Quellen des beigefügten Rust-Motors mit dem Merkmal `experimental-aac-gap` |
| `CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip` und `CAN-libmpv-v0.41.0-relink-2026-09-26.zip` | libmpv-Quellen, Nachweise und Material zum erneuten Linken |
| `SHA256SUMS.txt`, `PUBLISH-FILES.sha256`, `PAKETPRUEFUNG.md` | Prüfsummen und Nachweis zum Paketsatz |

Setup und Portable-ZIP benötigen die **.NET 10 Desktop Runtime x64**. Je nach Schnittweg kommen die in CAN konfigurierten Werkzeuge FFmpeg/ffprobe, GPAC/MP4Box und FFMS2/ffmsindex zum Einsatz. Der beigefügte otr-can-Motor liegt unter `runtime\otr-can\otr_can.exe`. Das Setup verwendet den gewohnten CAN-Programmordner und kann eine vorhandene Installation aktualisieren. Die Portable-Anwendung startet über `CutAssistantNext.App.exe`. Beide Varianten verwenden die CAN-Benutzereinstellungen. [Nutzeranleitung](NUTZERANLEITUNG.md)

## Quellstand und technische Prüfung

Die Paketierung übernimmt die geprüften Programmdateien aus der Build-52-Entwicklungsausgabe. Das Quellarchiv enthält denselben technischen Snapshot und die aktualisierte Dokumentation. `SOURCE-SNAPSHOT.json` dokumentiert Git-Basis, eingebundene Spotlight-CAN- und Motor-Revision sowie den Bezugsnachweis. Das Quellpaket wurde mit gesperrten Abhängigkeiten gebaut. Die Bauanleitung steht in [BUILD-SOURCE.txt](../BUILD-SOURCE.txt) im Quellarchiv. Die Build-52-Entwicklungsausgabe bestand 1.825 CAN-Tests, 22 native Medienprüfungen und 63 beziehungsweise 78 Rust-Tests. Der Paketprüfbericht enthält die Verifikation des eingefrorenen Paketsatzes.
