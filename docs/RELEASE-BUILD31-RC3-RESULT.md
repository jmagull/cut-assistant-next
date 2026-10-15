# Paketsatz 0.3.0 · Build 31 · RC3

Stand: 15.10.2026. Auf Nutzerauftrag **eingefroren, gebaut, geprüft, committet und gepusht**. Setup, Portable, CAN-/Motor-/libmpv-Quellen, Relink und Prüfsummen liegen lokal unter `artifacts/release-build31-RC3`. Eine GitHub-Release-Veröffentlichung wurde nicht ausgeführt; Build 30 bleibt die zuletzt veröffentlichte Vorabversion. [Fester Umfang und Grenzen](FREEZE-BUILD31-RC3-20261015.md).

## Fester Quellstand

Gebauter Quellcommit **de8ec0d598b095d85e4559e7fd83012534959637**, annotierter Tag **v0.3.0-build31-rc3**; beide auf `origin/main` beziehungsweise dem Remote-Tag verifiziert. Spätere Ergebnis-/Übergabe-Dokumentation ändert diese Pakete nicht. Dateiversion **0.3.0.31**, tatsächliches WPF-Hauptfenster **Build 31 · RC3**.

Isolierter Git-Export mit committed Lockdateien: **538 Manifesteinträge**, **539 Dateien im Quellen-ZIP** einschließlich des Manifests. Jede ZIP-Datei gegen SHA-256-Quellenmanifest geprüft, feste Herkunft ohne Arbeitsbaumüberlagerung. Vier vorbestehende lokale Produktions-Lockänderungen nicht committet und nicht im Archiv enthalten.

## Inhalt und persönliche Rückmeldungen

Hauptplayer-Halbierungssuche mit unabhängiger Suchweite, Halbierung bei Richtungswechsel und Rücksetzung bei bewusster Navigation. Bei erkanntem AVI einschließlich `.avi.mp4` Suchzeile ausgeblendet und Aufruf gesperrt; Frame-Lupe für Button/Shift+F deaktiviert. Gelber Hinweis erklärt die Einschränkungen, MP4 nach Medienwechsel wieder verfügbar. Gezielte Vorprüfung nach FFMS2: eindeutige aufsteigende Zeiten sowie Nullstart und komplettes 25-/50-fps-Raster entsprechend Motor 0.1.5; ungeeignete Quellen werden vor `cut-can` verständlich abgelehnt. MP4Box, Vorbereitung und Motor unverändert.

Persönlich bestätigt: Hauptplayersuche in allen Richtungen; AVI-Ausblendung und MP4-Wiedereinblendung per Screenshot; Teufelsbrigade-AVI mit otr-can einschließlich Ton/Bild/Übergängen einwandfrei. No Turning Back und Navy CIS zeigen die neue gezielte Sperre im realen Nutzerlauf. Separater No-Turning-Back-MP4Box-Schnitt technisch erfolgreich, Prüfen/Vorschau bestätigt. Kein pauschaler Qualitätsnachweis jeder Ausgabe und keine frische Setup-/VM-Abnahme daraus abgeleitet.

## Tatsächlich geprüft

- Im exportierten Baum: Locked Restore für win-x64, Release-Build **0 Warnungen/0 Fehler**, **1308/1308 Tests**, 0 übersprungen. Core 196, Cutlists 72, Media 194, App 846. Tests erst nach beendetem Build.
- Framework-dependent win-x64-Publish; externe Laufzeit und FFmpeg-/MP4Box-/FFMS2-Werkzeuge nicht versehentlich gebündelt. Lizenz-/Drittanbieterhinweise vorhanden, vier native DLLs exakt geprüfter libmpv-Satz. App-Dateiversion 0.3.0.31 und Quellpin im Paket geprüft.
- **249 Portable-Dateien** aus dem fertigen ZIP extrahiert, jede SHA-256 mit dem Publish-Manifest verglichen, keine zusätzlichen Dateien. Setup mit **Inno Setup 7** aus genau dem Publish-Verzeichnis erfolgreich kompiliert. Keine Installation ins laufende Nutzersystem durchgeführt.
- Gegen die entpackten Programmbits: **vier echte WPF/mpv-Container-/Wechselfälle** mit AVI, MP4 und irreführenden Endungen. AVI-Suchzeile verborgen ohne Platzverbrauch, Aufrufwege wirkungslos und laufende Wiedergabe erhalten; MP4-Suche sichtbar, tatsächlicher Seek und Reset erfolgreich; Lupensperre und erklärender Tooltip erhalten. Drei Fensterbreiten, keine WPF-Bindungsfehler. Versionsanzeige Build 31 RC3 geprüft. Window-State des Helfers isoliert gespeichert.
- **Sechs reale Indexvergleiche**: Teufelsbrigade 263694 Zeiten motorfähig, No Turning Back/Arbeitskopie/MP4Box-Ausgabe mehrdeutig, Navy monoton aber nicht CFR-motorfähig, alte 12-s-SEI-Diagnosekopie ebenfalls kein geeignetes Motor-Zeitraster.
- Produktionsschnittdienst mit tatsächlichem FFMS2-Indexer an **drei Quellen**: No Turning Back und Navy vor Motorstart gestoppt, Teufelsbrigade weiter freigegeben; positiver Motoraufruf im Helfer gezielt abgefangen. Vorhandene Testziele erhalten, je genau ein Indexlauf, Arbeitsordner aufgeräumt. Die ursprünglichen Downloadpfade von No Turning Back/Navy waren inzwischen nicht mehr vorhanden: für No Turning Back die zuvor SHA-256-identische AVI-Sicherung, für Navy die vorhandene reale Diagnose-MP4 verwendet. Der erste Helferlauf meldete fehlendes Original; Wiederholung mit diesen vorhandenen Quellen ohne Produktänderung erfolgreich. Kein neuer kompletter Filmschnitt.
- Helfer-App-/Core-/Media-/Cutlists-Assemblies per SHA-256 identisch mit den entpackten Paketcode-Dateien; keine Prüfung gegen veraltete Entwicklungs-DLLs.
- Motor **otr-can 0.1.5** unverändert, EXE-SHA-256 **FC6ED6568880A995F3D141A9378AA2CE28ADE8DC0E257E93DC972799BB1324A6**, Version und Fähigkeit `mp3-aac-v1` geprüft. Quellencommit **2a474629836d52023bf37cbbdb309ccd1c23acf3**, vorhandene passende Quellen/Provenienz wiederverwendet. Kein neuer Rust-Build oder Nachweis bitidentischer Reproduzierbarkeit behauptet.
- Sechs Binär-/Quellenarchive und `SHA256SUMS.txt` schreibgeschützt eingefroren. Lokale Build-30-Archive bytegleich, alle acht lokalen Lockdateien bytegleich mit Beginn dieser Aufgabe. Nach erfolgreichem vollständigem Paketbuild/-test lokalen Buildzähler auf **31** fortgeschrieben; Produktions-Lockänderungen erhalten.

## Dateien und SHA-256

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-0.3.0-Build31-source-RC3.zip | 3104947 | 74F2E40A1E807220774CE0C0047D7AA20FCEB509605B898828648C163DDF0242 |
| CutAssistantNext-0.3.0-Build31-win-x64-RC3.zip | 24973680 | 9CCD38087D3C19A4A5111A2261C41A028E4F7DD880AA01DEA59FA72A2858A248 |
| CutAssistantNext-0.3.0-Build31-win-x64-Setup-RC3.exe | 20290615 | 3625AE24BEDAD8A35A130783A2A6C94467970E2E6C0196A4AB161FC88747FCCC |
| otr-can-0.1.5-source.zip | 63988 | 3A973278ABDF719B745405D9DFBB930C14A28CFEF40C7FB52A35CA17F7214E57 |
| SHA256SUMS.txt | 659 | 37BBEAD5C669A5407738706273A30BB51394B7B19960312181C26D42D31EE333 |

## Lokale Nachweise und Grenzen

`artifacts/release-build31-RC3` enthält Publish-Manifest, Paketprovenienz, Build-/Installerlogs, TRX und eingefrorene Archive. `artifacts/source-build31-RC3` enthält den tatsächlich gebauten Quellbaum, `artifacts/portable-build31-proof` das entpackte ZIP. Nachweise unter `.build/freeze-build31-20261015`, insbesondere `PACKAGE-PROOF.json`, `SOURCE-ZIP-PROOF.json`, `proof/PROOF.json`, `proof/CFR-PROOF.json`, Helferlogs, Herkunft/Artefaktlisten und Erhaltungsnachweise.

Keine neue Build-31-Installation/Deinstallation in VM, keine frische persönliche Paketabnahme und keine zusätzliche vollständige Film-/Tonspurabnahme behauptet. Externe .NET-10-Desktop-/VC++-Laufzeiten und Medienwerkzeuge erforderlich. AVI bleibt experimentell; weitere Codec-/Audio-/Decodergrenzen sowie der bekannte MP4Box-Zwei-AAC-Befund bleiben bestehen. Die Vorprüfung repariert keine Aufnahme und wechselt den Motor nicht automatisch. Nächster sinnvoller Schritt: persönliche Setup-/Portable-Prüfung dieses Paketsatzes.