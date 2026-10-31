# CAN 1.0.2 Build 47 – erstellter und geprüfter Paketsatz

**31.10.2026: Setup, Portable-ZIP und passende Quellen nach persönlichem Dialogcheck erstellt und technisch geprüft.** Dateiversion **1.0.2.47**, Motor **otr-can 0.1.6**, Wiedergabe **libmpv 0.41.0**, .NET 10/WPF für Windows 11 x64. Auslieferungsdateien und manifestierte Quellen schreibgeschützt gesichert. [Paketumfang](FREEZE-1.0.2-BUILD47-20261031.md), [Änderung und Prüfgrenzen](MP4BOX-AUDIO-ENTHUSIAST-SPERRE-20261031.md).

## Bestätigter Umfang

Erkannte GPAC/MP4Box-Filmteile sperren AUDIO-ENTHUSIAST im Klebecenter einschließlich seiner Tonspurauswahl. Hinweis dauerhaft bei den Startknöpfen, unabhängig vom Scrollstand. QUICK bleibt nach bestätigter Tonspurzuordnung nutzbar; keine automatische Verfahrensauswahl. Herkunft wird bei Tauschen/Ersetzen mitgeführt; Dienst prüft nochmals vor der Medienverarbeitung. Erkennung über vorhandene Container-Kennung, keine Zusage bei entfernter oder fehlender Kennung. Bestehende Medienprüfungen und Encodingverfahren unverändert.

Persönlicher John-Wick-Dialogcheck positiv; Screenshot zeigt zwei MP4Box-Dateien und korrekte Sperre. **VM-Setup-Nachtrag 31.10.2026:** MP4Box-Sperre für AUDIO-ENTHUSIAST persönlich bestätigt; QUICK AND COPY mit AAC und AC-3 technisch erfolgreich in **27,83 s**, Bild/Ton/Drei-Sekunden-Vorschau persönlich bestanden. Ausgabeübernahme und Bereinigung laut Protokoll erfolgreich; Meckerecke bewahrt Werkzeugwarnung und Laufzeitunterschiede. Weitere VM-Fälle, Endabnahme, Deinstallation und neue Portable-ZIP-Prüfung bleiben offen. [Laufende Build-47-Runde](VM-SETUP-BUILD47-20261031.md). Die Build-46-VM-Runde bleibt getrennt dokumentiert.

## Technische Paketprüfung

**Portable-Fortschreibung:** Furiosa-MP4Box-Sperre persönlich bestätigt. otr-can-QUICK mit AAC und AC-3 in **41,93 s**, Ton/Bild/Schnitt/Drei-Sekunden-Vorschau persönlich bestanden; Quellherkunft vom Nutzer geklärt. Eigener Furiosa-otr-can-AUDIO-ENTHUSIAST-Anhang technisch erfolgreich in **5:26,37 min**, **387448320 Samples**, Klebestelle **01:54:18.320**, Dauer **02:14:31.840**; persönliche Kontrolle nicht separat zugeordnet. Beide Läufe mit erfolgreicher Ausgabeübernahme und Bereinigung, drei Portable-Läufe insgesamt dokumentiert. ZIP-Endabnahme bleibt offen. [Portable-Abnahmestand](VM-PORTABLE-BUILD47-20261031.md#abnahmestand).

**Portable-ZIP-Prüfung in der VM begonnen:** Nach „jetzt zip“ eigener John-Wick-MP4Box-/QUICK-Lauf mit AAC und AC-3 in **28,61 s** erfolgreich. Screenshot zeigt MP4Box-Sperre; Ton/Bild/Drei-Sekunden-Vorschau persönlich bestanden. Ausgabeübernahme/Bereinigung positiv; weitere Portable-Fälle und ZIP-Endabnahme noch offen. Keine vorausgegangene Setup-Deinstallation behauptet. [Getrennter Portable-Bericht](VM-PORTABLE-BUILD47-20261031.md).

**Persönliche VM-Fortschreibung:** Drei Verarbeitungsläufe positiv: MP4Box → QUICK AND COPY (**27,83 s**), otr-can → QUICK AND COPY mit AAC und AC-3 (**42,79 s**) und otr-can → AUDIO-ENTHUSIAST mit AAC/AAC (**5:37,95 min**). Der Nutzer bestätigt die Zuordnung ausdrücklich. Neue QUICK-Bild-/Ton-/Drei-Sekunden-Kontrolle positiv, AUDIO-ENTHUSIAST-Bild/Ton positiv; letzteres technisch mit **387448320 Samples**, Klebestelle **01:54:18.320**, Dauer **02:14:31.840** vollständig geprüft. Dialogsperre und Hinweisentfernung beim Wechsel bestätigt. Vollständige VM-Endabnahme, Deinstallation und Portable bleiben separat offen. [Verfahrenszuordnung und Fälle](VM-SETUP-BUILD47-20261031.md#vom-nutzer-festgehaltene-verfahrenszuordnung).

**Weiterer persönlicher VM-Dialogcheck:** Zwei Furiosa-MP4Box-Teile zeigen den Sperrhinweis; beim Wechsel auf das otr-can-Paar verschwindet er. Zwei Screenshots bestätigen die Aktualisierung des Herkunftshinweises. Kein zusätzlicher Verarbeitungslauf. [VM-Nachweis](VM-SETUP-BUILD47-20261031.md#zusätzlicher-dialogcheck-von-mp4box-zu-otr-can-wechseln).

- Ausschließlich aus dem manifestierten Snapshot gebaut: Basiscommit **3aa00a138565c043b34fcb959f2c28d937f83b0b** plus **17 aufgenommene Arbeitsänderungen**. **705 manifestierte Quelldateien / 706 Quellen-ZIP-Dateien**, **250 Portable-Dateien** vollständig gegen Manifest und ZIP geprüft.
- Locked-Mode-Restore für win-x64, Release-Build, Tests und Publish erfolgreich; **0 Warnungen / 0 Fehler**, App-Dateiversion **1.0.2.47**. Inno Setup 7 erfolgreich aus dem geprüften Publish-Verzeichnis kompiliert.
- **1593 Pakettests:** 200 Core, 72 Cutlists, 387 Media, 934 App; alle bestanden, keine übersprungen.
- Zusätzlich **1321 Tests gegen die finalen ausgelieferten Produkt-DLLs:** 934 App und 387 Media; vier Produkt-DLLs jeweils bytegleich zum Publish.
- Kompilierter Paketdialog mit exakt finalen vier Produkt-DLLs geprüft: reale Furiosa-MP4Box-Teile erkannt, otr-can-Vergleich ohne GPAC-Kennung, AUDIO-ENTHUSIAST gesperrt und QUICK nach Bestätigung aktiv. Zwei Inhaltsgrößen (852×852 / 632×602) gerendert; Sperrgrund im kleinen Fenster sichtbar. Kein neuer Schnitt-/Encodinglauf großer Filme.
- Native Laufzeit-DLLs und Lizenzhinweise geprüft; .NET 10 Desktop Runtime x64 bleibt separat erforderlich. Kein unbeauftragtes .NET-/FFmpeg-/ffprobe-/MP4Box-/FFMS-Bündel. Motor im Paket **otr_can 0.1.6**, Fähigkeiten **mp3-aac-v1 / aac-overlap-v1**, EXE-SHA **73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E**. Passende unveränderte Motor-/libmpv-Quellen ergänzt.
- **376 Produkt-/Testdateien seit Paketauftrag unverändert**, ebenso **58 geschützte Ursprungsdateien** einschließlich der acht Locks, Version.props, Buildzähler 47 und früherer Auslieferungsdateien.
- Zusätzliche Setup-Versionsprüfung präzisiert: Das bestehende Inno-Skript definiert keine Dateiversionsressource mit Buildnummer; diese Zeichenfolge ist leer. Produktversionsressource ist **1.0.2**, Setup-Anzeigename/Compilerparameter und Dateiname führen Build 47, enthaltene App-Dateiversion **1.0.2.47**. Die ursprüngliche zusätzliche Annahme einer Setup-Dateiversion 1.0.2.47 wurde gegen das unveränderte Skript korrigiert; keine Paketänderung dafür vorgenommen.

## Auslieferungsdateien und SHA-256

Alle Dateien liegen unter **artifacts/release-1.0.2-build47**.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build47-win-x64-Setup.exe | 20508494 | E7AA9C6E61FF7333D9BE94AA631050389D9BAE3BBCB80B77015EBE16453D5712 |
| CutAssistantNext-1.0.2-Build47-win-x64.zip | 25239558 | 394DBE9B09727E3D9E9C3FC9E31927500DFA2C5D27BA6AEDDF9A84A9887DFAA4 |
| CutAssistantNext-1.0.2-Build47-source.zip | 3670880 | E6514B83521C8EAC52AB92DA758F4B3A01435D5E067D0EC307615D2ED5FA6772 |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

**SHA256SUMS.txt** prüft die sechs Assets, **PUBLISH-FILES.sha256** alle 250 Portable-Nutzdateien. Alle acht obersten Auslieferungsdateien und 706 manifestierte Snapshot-Dateien schreibgeschützt. Nachweise unter **.build/release-1.0.2-build47-20261031/**: PACKAGE-INPUTS.json, PACKAGE-PROOF.json, PRESERVED-BEFORE/AFTER.json, PRODUCT-BEFORE.json, SETUP-PROOF.json, TRX und native Dialogprüfung.

## Erhaltung und Veröffentlichung

Buildnummer bleibt 47; Produktcode seit Paketauftrag unverändert. Frühere Pakete und vorbestehende Nutzeränderungen erhalten. Snapshot nimmt die vier lokalen Produktions-Lockänderungen nicht auf, sondern verwendet die committed Locks. Archive bewahren den Dokumentationsstand bei Snapshot-Erstellung; spätere Ergebnisnachträge ändern sie nicht.

## Freigabe zum Einfrieren und Veröffentlichen am 31.10.2026

Der Nutzer beauftragt ausdrücklich, den bestätigten Stand einzufrieren, README/Dokumentation abzugleichen, zu committen, zu pushen und zu veröffentlichen. Veröffentlicht wird der unveränderte geprüfte Build-47-Paketsatz unter **v1.0.2-build47**; ältere Releases bleiben erhalten. Die bisherigen Aussagen über noch fehlende Veröffentlichungsfreigaben beschreiben den damaligen Paketierungsstand und sind durch diesen Auftrag abgelöst. Kein bisher nicht gemeldeter persönlicher Test wird dadurch als bestanden ergänzt. [Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md).

Die Veröffentlichung ist jetzt beauftragt und wird nach Commit/Push und identischer Prüfung aller acht Remote-Assets abgeschlossen. Bereits bekannte zusätzliche persönliche Prüfvarianten bleiben getrennt dokumentiert.
