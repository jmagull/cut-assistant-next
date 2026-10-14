# Paketsatz 0.3.0 · Build 30 · RC3

Stand: 14.10.2026. Vom Nutzer ausdrücklich zum Einfrieren, Bauen und Veröffentlichen freigegeben. Neue Buildnummer 30, bestehende Version 0.3.0 und Kennung RC3. Build 29 bleibt unverändert.

## Enthaltener Stand

- Sichtbare Frame-Lupe rechts vom festen Bereich „Vorgemerkter Anfang“, Shift+F erhalten; ±10-Bilder-Buttons erklären Strg+Klick für ±20.
- Bei leeren Staffel-, Folge- und Folgentitelfeldern endet die automatische Filmmaske nach dem Namen. Bewusstes Neuerzeugen verwendet die aktuelle Maske; geschützte/manuelle Vorschläge bleiben erhalten.
- Frame-Lupe ohne ausgewählte Schnittkante an der erreichten Playerposition; Übernahme im Untersuchungsmodus gesperrt. Bestehender Kantenmodus und AVI-Lupe erhalten.
- MP4-Keyframe-Navigation und blaue Bereiche im vorhandenen Schnittstreifen, K direkt hinter aktueller Framenummer. AVI von Analyse/Buttons/Markern/K im Hauptfenster ausgenommen, Hinweis im AVI-Infoblock.

## Verfahren und Prüfgrenzen

Paketierung aus einem festen Git-Quellcommit mit committed Lockdateien, Quelle mit Dateimanifest und Herkunftsangaben. Vier bereits vor Arbeitsbeginn veränderte Produktions-Lockdateien werden nicht committet und nicht für den Quell-Snapshot verwendet. Setup, Portable und Quellen werden gemeinsam geprüft und mit SHA-256 versehen.

Vor Paketierung: **1215/1215 CAN-Tests**, 0 übersprungen, Release-Rebuild ohne Warnungen/Fehler. 18 Keyframe-Integrationsprüfungen, 46 Untersuchungsmodus-Prüfungen und ergänzende kompilierte K-Anzeige-Prüfung sind in den jeweiligen Entwicklungsberichten dokumentiert. Eine lange HD-MP4 lieferte 3448 Keyframes in 26,6 Sekunden. Der Nutzer bestätigte Bedienung/Filmnamensdialoge/Untersuchungsmodus und gab abschließend den Gesamtstand frei. Diese Rückmeldungen ersetzen keine frische Build-30-Setup-/VM-Abnahme.

Unverändert: otr-can 0.1.5, libmpv 0.41.0 und externe Werkzeuge. Keine neue Produktionsabhängigkeit. Bekannte AVI-Zeitstempelgrenzen und der dokumentierte MP4Box-Befund mit zwei gleichartigen AAC-Spuren bleiben bestehen. .NET 10 Desktop Runtime x64 und externe FFmpeg-/MP4Box-/FFMS2-Werkzeuge bleiben separat einzurichten.

Die Paketprüfung ist erfolgreich abgeschlossen; die [öffentliche Vorabversion](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build30-rc3) wurde am 14.10.2026 um **22:57:30 Europe/Berlin** freigegeben. [Veröffentlichungsnachweis mit sämtlichen SHA-256-Werten](RELEASE-PUBLICATION-BUILD30-RC3.md).

## Tatsächlich gebaut und geprüft

- CAN-Quellcommit **57878bd90b3e044218f8f157b6922552a170f1d4**, annotierter Tag **v0.3.0-build30-rc3**. Remote-Tag auf diesen Quellcommit geprüft. Spätere reine Dokumentationscommits ändern die Pakete nicht.
- Isolierter Git-Export, **526 Quellen-Manifesteinträge**, **527 Dateien im Quellen-ZIP** einschließlich des Manifests. Vier Produktions-Lockdateien per Git-Blob-ID identisch mit dem Quellcommit; lokale Änderungen nicht übernommen.
- Locked Restore, Release-Build **0 Warnungen/0 Fehler**, anschließend **1215/1215 Tests**, 0 Fehler/übersprungen: Core 196, Cutlists 72, App 794, Media 153. Tests erst nach erfolgreich beendetem Build.
- Framework-dependent win-x64-Publish, App-Dateiversion **0.3.0.30**, RC3 im tatsächlichen Hauptfenster. Separat benötigte .NET-Laufzeit sowie externe FFmpeg-/ffprobe-/MP4Box-/FFMS2-Werkzeuge nicht versehentlich eingebettet. Lizenz-/Drittanbieterhinweise vorhanden; vier libmpv-DLLs identisch mit dem geprüften Satz.
- **249 Portable-Dateien** aus dem fertigen ZIP extrahiert und jede SHA-256 mit dem Publish-Manifest verglichen; keine zusätzlichen Dateien. Setup mit **Inno Setup 7** aus genau diesem Publish-Ordner erfolgreich kompiliert.
- Gegen die entpackten Programmdateien erneut **18 native MP4-/AVI-Ausschluss-/Layout-Prüfungen**: acht Keyframe-Sprünge mit unabhängig ausgelesener sichtbarer Bildnummer, Wegschritt entfernt K, unveränderte Schnittpläne; drei AVI-Varianten ohne Hauptfenster-Keyframes bei weiterhin nutzbarer Lupe; drei Fenstergrößen, rote/blaue Pixel, kontrollierter Analyseabbruch. Keine WPF-Bindungsfehler. Prüfbilder gesichtet.
- Ergänzende kompilierte Prüfung der drei tatsächlichen Textzustände **Frame 36.206 (K) / ca. 210.000**, ohne K und unbekannte Zahlen. K-Geometrie vor dem Schrägstrich korrekt, AVI ausgenommen und Status enthält exakt **„Blaue Bereiche: Keyframes“**. Keine Bindungsfehler.
- Unveränderte Motor-EXE **otr-can 0.1.5**, SHA-256 **FC6ED6568880A995F3D141A9378AA2CE28ADE8DC0E257E93DC972799BB1324A6**, Version und Fähigkeit `mp3-aac-v1` erfolgreich abgefragt. Motorquelle **2a474629836d52023bf37cbbdb309ccd1c23acf3**; kein neuer Rust-Build/Testlauf nötig oder behauptet.
- Sechs Archive und `SHA256SUMS.txt` eingefroren und schreibgeschützt. Alle sieben GitHub-Dateien im Entwurf und nach öffentlicher Freigabe auf eindeutigen Namen, Uploadstatus, Größe und SHA-256-Digest geprüft; Release-Text identisch mit Freigabehinweisen.
- Öffentliche Build-29- und Build-16-Releases einschließlich Metadaten/Asset-IDs/Hashes unverändert. Lokale Build-29-Archive unverändert. Alle acht lokalen Lockdateien bytegleich; nur die vier vorbestehenden Produktionsänderungen bleiben im Arbeitsverzeichnis. Lokaler Buildzähler nach erfolgreichem Build/Pakettest ausdrücklich auf **30** fortgeschrieben.

## Lokale Belege

- `artifacts/release-build30-RC3`: Setup, Portable, CAN-/Motor-/libmpv-Quellen, sechs Archivprüfsummen, Publish-Manifest, Herkunft und Paketlogs/TRX.
- `artifacts/source-build30-RC3/CutAssistantNext-0.3.0-Build30-source-RC3`: tatsächlich gebauter Quell-Snapshot.
- `artifacts/portable-build30-proof`: unverändert entpacktes Portable-ZIP für die kompilierten Prüfungen.
- `.build/freeze-build30-20261014`: Quell-/Paketnachweise, Upload-Erwartungen, REST-Nachweise vor/nach Freigabe, UI-Prüfhelfer/-Bilder und Erhaltungsnachweise.

Keine frische Installation/Deinstallation in der VM, keine neue persönliche Build-30-Schnittabnahme und keine zusätzlichen realen Ausgabe-/Tonspurprüfungen durchgeführt. Die dokumentierten bisherigen Abnahmen bleiben gültig für ihren jeweiligen Stand. Die nach Veröffentlichung aktualisierte Repository-Dokumentation steht separat vom unveränderten Quell-Snapshot.
