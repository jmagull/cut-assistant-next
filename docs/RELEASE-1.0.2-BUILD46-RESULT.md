# CAN 1.0.2 Build 46 – eingefrorener und geprüfter Paketsatz

**29.10.2026: Setup, Portable und passende Quellen vollständig erstellt, technisch geprüft und auf Nutzerauftrag eingefroren. Quellstand anschließend committet und nach origin/main gepusht. Keine Veröffentlichung ohne erneute Ansage.** Dateiversion **1.0.2.46**, Motor unverändert **otr-can 0.1.6**, Wiedergabe **libmpv 0.41.0**, .NET 10/WPF für Windows 11 x64. Öffentlich bleibt Build 41. [Freeze](FREEZE-1.0.2-BUILD46-20261029.md), [Bedienung](NUTZERANLEITUNG.md), [Verfahren und persönliche Praxis](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md).

## Eingefrorener Umfang

QUICK AND COPY verwendet pragmatischen FFmpeg-Concat mit `-c copy`, bestätigter AAC-/AC-3-Mehrspurzuordnung und kurzen Mindestprüfungen. Keine Ablehnung allein wegen kleiner/vorhandener PTS-Unregelmäßigkeiten, unterschiedlicher H.264-Konfigurationshashes oder zusätzlicher Samples. Keine vollständige PCM-/Paketidentitätsprüfung oder obligatorische Vollvideodekodierung im QUICK-Produktweg. **Meckerecke – technische Hinweise** sammelt vorhandene Angaben und sämtliche Werkzeugwarnungen, ohne zusätzliche Vollprüfungen zu starten. Echte Prozess-/Ausgabe-/Dateischutzfehler bleiben Abbruchgründe.

AUDIO-ENTHUSIAST bleibt streng mit genau einer Spur pro Teil, direkter RF64-Zusammensetzung und einmaliger AAC-LC-Neukodierung. Beide Verfahren kopieren Video. Dateischutz, Abbruch, Phasenzeiten, erfolgreiche Bereinigung, Tonspurwahl und Drei-Sekunden-Kontrolle bleiben. QUICK-Prüfstellen ausdrücklich berechnet/angenähert. Kein Standardverfahren und keine automatische Umstellung.

Quellbasis **d200ebf7567bceeab012ee9a2bb6b564635382a4** plus **52 manifestierte Arbeitsänderungen**, **697 manifestierte Quelldateien / 698 Quellen-ZIP-Dateien**, **250 Portable-Dateien**. Vier vorbestehende lokale Produktions-Lockänderungen unverändert erhalten und aus Snapshot/Commit ausgeschlossen; Snapshot verwendet committed Locks. Keine neue Produktionsabhängigkeit, Videos oder privaten Einstellungen. Buildzähler bleibt **46**. Alle **373 Produkt-/Testdateien gegenüber Freeze-Auftragseingang unverändert**.

**artifacts/release-1.0.2-build46** enthält sechs Assets und zwei Prüfsummenmanifeste. Alle **acht obersten Auslieferungsdateien** sowie **698 Snapshot-Dateien** schreibgeschützt, Hashes geprüft. Früheren Build-40/41/43/44/45-Paketsatz erhalten.

## Technische Paketprüfung

- Restore aus dem Snapshot im Locked Mode, Release-Build und Publish erfolgreich; **0 Warnungen / 0 Fehler**, Dateiversion **1.0.2.46**. Inno Setup 7 erfolgreich aus dem geprüften Publish-Verzeichnis kompiliert.
- **1576 Pakettests:** 200 Core, 72 Cutlists, 375 Media, 929 App; keine Fehler oder übersprungenen Tests.
- Zusätzlich **1304 Tests gegen genau die final ausgelieferten Produktbibliotheken:** 929 App und 375 Media; alle vier Produkt-DLLs bytegleich zum Publish.
- **27 native QUICK-Fälle** gegen finale Paket-DLLs positiv: automatische AAC+AC-3-Erhaltung in beiden Reihenfolgen, einzelne/bewusst anders zugeordnete Spur, vorhandene Quellenauffälligkeiten und unbekannte/DTS-Warnungen, Zeitbasiswechsel, Mono/44,1 kHz/5.1/50 fps. Negative Fälle: fehlende/falsche/doppelte Zuordnung, fehlende Bestätigung, inkompatibles Bildformat, Prozessfehler, leere/unlesbare Ausgabe, Abbruch, vorhandene/rivalisierende Ausgabe. Originale und Bereinigung geprüft. QUICK enthält dabei weder PCM-Encoding noch `-xerror`; Warnungen allein werden nicht mehr negativ gewertet.
- **21 native AUDIO-ENTHUSIAST-Fälle erneut bestanden; alle neun erfolgreichen Ausgaben bytegleich zu Build 45.** Strenge Fehler-/Warnungsablehnung bleibt in diesem Verfahren erhalten.
- Kompiliertes MainWindow mit genau einem Klebecenter-Eintrag, beide Methoden ohne Standard, Mehrspurvorschläge, Bestätigungspflicht und Widerruf bei Änderung/Tausch sowie sichtbarer John-Wick-AC-3-Auslassung geprüft. **Vier native Hinweis-/Auswahlfälle bei 680/900 DIP**, Textumbruch und Doppelbelegungsschutz positiv.
- Kontrollplayer aus dem Paket mit echter Furiosa-Mehrspurdatei und libmpv geprüft: AAC-ID 1 und AC-3-ID 2 gelesen, beide umgeschaltet und zurückgelesen, Seek auf 6845,841 s. Testfenster unsichtbar/stumm; keine persönliche Hörprüfung daraus abgeleitet.
- Quellenmanifest/ZIP vollständig byteweise verglichen. Alle **250 Portable-Nutzdateien** gegen Manifest/ZIP geprüft. Fünf native Prüfhilfen verwenden exakt finale Produkt-DLLs. Native Bibliotheken, Lizenz-/Herkunftshinweise, Voraussetzungen, Quellen und laufzeitabhängige Konfiguration kontrolliert; kein unbeauftragtes .NET-/FFmpeg-/MP4Box-/FFMS-Bündel.
- Mitgelieferter Motor direkt **otr_can 0.1.6**, Fähigkeiten **mp3-aac-v1 / aac-overlap-v1**. EXE-SHA **73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E**; passende unveränderte Quellen enthalten.
- **50 geschützte Dateien unverändert:** acht Locks, Version.props, Buildzähler und 40 frühere oberste Auslieferungsdateien. Kein Release-Tag, Upload oder GitHub-Release erstellt.

## Persönliche Abnahme und Grenzen

**Weiterer Praxisfall 31.10.2026 – Brilliant Minds S01E11 Die andere Frau:** Laufende Umgebung **Build 46 / Setup / Arbeitsrechner** fortgeführt. Cutlist mit 25 fps, MP4Box 26.07 und drei Behaltebereichen; Protokoll bestätigt Schneiden, Zusammenfügen und alle drei Schnittstellenprüfungen bis „Fertig.“. Übergang 1 mit zusätzlicher Prüfung des dekodierten neuen Anfangs bei gleicher Bildzeit. Bild/Ton/Schnitt/Drei-Sekunden-Vorschau persönlich bestanden. Zusammen mit Rocky Horror und Doc drei aktuelle Praxisfälle dokumentiert. [Nachweis](PRAXISTESTS-20261031.md#fall-3-brilliant-minds-s01e11--die-andere-frau-5plus-hq-25-fps).

**Weiterer Praxisfall 31.10.2026 – Doc S02E18 Orientierung:** Unter der zuletzt genannten Umgebung **Build 46, Setup auf dem Arbeitsrechner** fortgeführt. Cutlist nennt 25 fps, MP4Box 26.07 und zwei Behaltebereiche; Kommentar vermerkt vollständig entfernte Werbung. Bild/Ton/Schnitt/Drei-Sekunden-Vorschau persönlich bestanden. [Praxisnachweis und Planwerte](PRAXISTESTS-20261031.md#fall-2-doc-s02e18--orientierung-5plus-hq-25-fps). Kein neuer Build-47-VM-/Portable-Fall.

**Praxisnachtrag 31.10.2026 – Build 46, Setup auf dem Arbeitsrechner:** The Rocky Horror Picture Show (arte HD vom 30.10.2026), Cutlist mit 50 fps, einem Behaltebereich und vorgesehenem Motor otr-can 0.1.6. Ton/Bild/Schnitt/Drei-Sekunden-Vorschau persönlich bestanden. Cutlist-Kommentar nennt keine Werbung und keinen verbliebenen arte-Vorspann-Blitzer. Build-/Setup-/Rechnerzuordnung vom Nutzer ausdrücklich präzisiert. Getrennt von Build-47-VM/Portable und ohne vollständige neue Paket-Endabnahme dokumentiert. [Praxisnachweis](PRAXISTESTS-20261031.md#fall-1-the-rocky-horror-picture-show-arte-hd-50-fps).

Der eingefrorene Entwicklungsstand ist persönlich positiv bestätigt: **Furiosa aus MP4Box-Teilen mit AAC+AC-3 in 23,74 s** und danach **John Wick aus neuen MP4Box-Teilen mit AAC+AC-3 in 16,44 s**, jeweils Bild, Ton, Übergang/Klebestelle und Drei-Sekunden-Vorschau. Das doppelte Furiosa-Protokoll zählt als ein Lauf. Die allgemeine Tonbestätigung ist keine gesonderte Hörbewertung jeder einzelnen Spur. Weitere automatische otr-can-/AAC-Referenzen bleiben getrennte Nachweise.

**Nachtrag 31.10.2026 – persönliche VM-Setup-Prüfung begonnen:** Installation von CAN 1.0.2 Build 46 bestätigt, Version im Screenshot sichtbar. QUICK AND COPY mit AAC und AC-3 erfolgreich in **30,03 s**, Ton/Bild/Schnitt/Klebestelle/Drei-Sekunden-Vorschau persönlich positiv. Nutzer testet weiter; vollständige VM-Endabnahme, Deinstallation und neue Portable-ZIP-Prüfung bleiben offen. [Laufender VM-Bericht](VM-SETUP-BUILD46-20261031.md).

Laufzeitabhängig: .NET 10 Desktop Runtime x64 separat installieren; FFmpeg/ffprobe konfigurieren. Unter Einstellungen → OTR-CAN-Werkzeuge die mitgelieferte **tools/otr-can/otr_can.exe** wählen und Version 0.1.6 kontrollieren; ältere gespeicherte Pfade können auf andere Kopien zeigen. Beide Verfahren verlangen passende Bildformate; Video wird nicht automatisch repariert. QUICK liefert keine sampleidentische Qualitätsgarantie.

Archive bewahren den Dokumentationsstand bei Snapshot-Erstellung vor Abschluss der Paketprüfung. Dieser aktuelle Arbeitsbericht ergänzt spätere vollständige Ergebnisse und Prüfsummen; schreibgeschützte Archive und Quellsnapshot werden dafür nicht nachträglich verändert.

**Zweiter VM-Funktionsfall am 31.10.2026:** AUDIO-ENTHUSIAST mit John Wick Kapitel 4 und AAC/AAC technisch erfolgreich in **00:05:54.41**; Bild, Ton und Drei-Sekunden-Vorschau persönlich bestanden. **457399680 Tonsamples**, Klebestelle **01:57:37.120**, Dauer **02:38:49.160** verifiziert, Ausgabeübernahme und Bereinigung erfolgreich. Beide Klebeverfahren sind damit in der laufenden VM-Setuprunde positiv; keine vollständige Endabnahme. [Nachweis](VM-SETUP-BUILD46-20261031.md#fall-2-audio-enthusiast-mit-john-wick-kapitel-4).

## Auslieferungsdateien

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build46-source.zip | 3641085 | F5048755AA4F8733B2AFEF49FDAEB9678A47355AA104E0D16985C8CD1DE318D8 |
| CutAssistantNext-1.0.2-Build46-win-x64.zip | 25235162 | 64E06105595A3D97D432B51059E6B3D33AA11F4FA5DF0A8D3FD19CC2B0E7A023 |
| CutAssistantNext-1.0.2-Build46-win-x64-Setup.exe | 20502256 | 72C913638786D42E4E77BFAB073779C62BE5AB8919E949B5C5A72FE4D96BCB51 |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

**SHA256SUMS.txt** beschreibt diese sechs Assets; **PUBLISH-FILES.sha256** alle 250 Portable-Dateien. Nachweise unter **.build/release-1.0.2-build46-20261029/**: PACKAGE-INPUTS.json, PACKAGE-PROOF.json, FINAL.json, SOURCE-CODE-FROZEN.json, geschützte Dateihashes, Test-TRX sowie native Fall-/Menü-/Dialog-/Hinweis-/Playernachweise.

## Git und Veröffentlichung

Implementierung, aktuelle Dokumentation, Freeze und Paketbericht mit **f60e480d12bc87263d1c3f28ef05ddba502c57ec** committet und nach **origin/main** gepusht; anschließend Remote-Hash identisch nachgewiesen. Dieser Abschlussnachtrag ist eine reine Dokumentationsänderung, keine neue Produkt-/Paketfassung. Der Snapshot manifestiert weiterhin exakt seine frühere Basis plus Arbeitsänderungen; die späteren Ergebnis-/Git-Nachträge im Repository ändern keine eingefrorenen Archive.

Die vier vorbestehenden lokalen Produktions-Locks bleiben unverändert außerhalb des Commits. Buildausgaben und Archive bleiben lokal unter artifacts und sind nicht in Git aufgenommen. **Keine Veröffentlichung, kein Release-Tag und kein Asset-Upload ohne erneute ausdrückliche Ansage.** Öffentlich bleibt Build 41.
