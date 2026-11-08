# Versionen und Nachweise

## Cut Assistant Next 1.0.3 Build 54 – stabile Version

Aktueller gemeinsamer Stand im ursprünglichen Repository **cut-assistant-next**; nach ausdrücklicher persönlicher Freigabe vom **08.11.2026** für `main` und [GitHub Latest](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.3-build54) bestimmt. Dateiversion **1.0.3.54**, libmpv **0.41.0**, mitgelieferter otr-can **0.1.6-audio-diag.1**. Alte Funktionszweige sind historische Quellen; es gibt eine gemeinsame CAN-Version.

Spotlight-CAN ist unter **Werkzeuge → Spotlight-CAN – Visuelle Werbesuche** integriert: Werbeblöcke anhand von Vorschaubildern grob markieren, bestätigte Schnittbereiche direkt an CAN übergeben und dort mit Player/Frame-Lupe präzisieren. Größe, Position und Maximierungszustand werden über CAN-Neustarts erhalten. Mehrmonitor-/DPI-Korrektur hält das Fenster bei geänderter Bildschirmkonfiguration sichtbar. Die Detailsuche hat standardmäßig zwei Sekunden Bildabstand; den letzten Bereich bei Bedarf in CAN mit **Videoende** bis zum tatsächlichen Dateiende abschließen.

Der normale otr-can-Audiomodus prüft streng und verändert bei einem Fehler keine Samples. Bei geeigneten Befunden stehen Abbrechen, ein ausdrücklich bestätigter vollständiger MP4Box-Neustart, Tonspur-Abwahl bei Mehrspurdateien oder **AAC-Zeitlücken reparieren (experimentell)** zur Wahl. Die Reparatur benötigt ausdrückliche Zustimmung, ergänzt nur nachgewiesene unterstützte Lücken positionsgenau mit Stille und prüft den gesamten Schnitt unabhängig. Keine allgemeine Sampletoleranz, keine automatische Reparatur und keine Wiederherstellung verlorener Originaltöne.

Der passende Motor liegt ausschließlich unter **`tools/otr-can/otr_can.exe`** relativ zur Anwendung; gespeicherte Werkzeugpfade werden nicht überschrieben. Für eine ZIP den vollständigen Inhalt in einen frischen Ordner entpacken. Die Portable-Fassung verwendet wie das Setup CANs Benutzereinstellungen unter dem lokalen Windows-Benutzerprofil. Beim Setup-Upgrade können alte `runtime`-Dateien verbleiben; sie werden nicht verwendet.

Voraussetzungen: **Windows 11 x64**, **.NET 10 Desktop Runtime x64**, für otr-can **Microsoft Visual C++ v14 Redistributable x64**. FFmpeg/ffprobe, GPAC/MP4Box und FFMS2/ffmsindex separat einrichten; keine Windows-Codec-Pakete erforderlich. Windows 10 ist nicht persönlich validiert. Bild, jede Tonspur und Schnittübergänge weiterhin selbst kontrollieren. QUICK AND COPY führt keine samplegenaue Tonprüfung aus; AVI bleibt experimentell.

**Cut Assistant Next 1.0.3 Build 54 wurde persönlich erfolgreich geprüft.** Insbesondere Setup in der Windows-VM: Grey’s Anatomy regulär sowie NCIS Sydney – Breathless, S02E07 nach ausdrücklich bestätigter AAC-Reparatur; Bild, Ton und Drei-Sekunden-Vorschau positiv. Die weitergehende persönliche Setup-/ZIP-/Spotlight-/Mehrspur-/Klebecenter-Abnahme von Build 53 bleibt separat zugeordnet.

[Downloads](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.3-build54) · [Release Notes](RELEASE-NOTES-1.0.3.md) · [Spotlight-Anleitung](SPOTLIGHT-README.md) · [Abnahme und Veröffentlichung](RELEASE-1.0.3-BUILD54-PUBLISHED-20261108.md). Die folgenden datierten Entwicklungsstände und alten Prüffassungen sind historische Nachweise, keine aktuelle Installationsanleitung.



## Aktuelle Zuordnung

| CAN | Build | otr-can | Stand |
| --- | ---: | --- | --- |
| 1.0.3 | 54 | 0.1.6-audio-diag.1 | Stabile Veröffentlichung, Tag v1.0.3-build54, Dateiversion 1.0.3.54; Motor unter tools/otr-can |
| 1.0.3 | 53 | 0.1.6-audio-diag.1 | Persönlich bestätigte integrierte Referenz vor der reinen Pfadumstellung |
| 1.0.2 | 48 | 0.1.6 | Frühere stabile Veröffentlichung, unverändert erhalten |

Aktueller Paketsatz: `artifacts/release-1.0.3-build54` im sauberen Release-Worktree des ursprünglichen Repositorys. Der vor Veröffentlichung persönlich geprüfte Satz im ursprünglichen Arbeitsordner bleibt unveränderte Referenz. [Quellen, Pakete und Veröffentlichung](RELEASE-1.0.3-BUILD54-PUBLISHED-20261108.md).

<details>
<summary>Historische Versionstabelle und Nachweise bis 05.11.2026</summary>

Die folgenden „aktuell“-Angaben beziehen sich ausschließlich auf den damaligen Stand.

**Stand 05.11.2026.** CAN-Anwendung, Buildnummer, Schnittmotor und Wiedergabebibliothek haben getrennte Versionen. [Aktueller Stand](AKTUELLER-STAND.md).

| CAN | Build | otr-can | Einordnung |
|---|---:|---|---|
| 1.0.3 | 52 | 0.1.6-audio-diag.1 | [Eingefrorener lokaler Paketsatz](FREEZE-1.0.3-BUILD52.md) mit bestätigter Tonspur-Abwahl, AAC-Reparatur und Spotlight-CAN |
| 1.0.3 | 51 | 0.1.6-audio-diag.1 | Lokale Audio-Fehlerbehandlung-Entwicklung mit Spotlight-CAN und [Setup, Portable-ZIP und Quellen](PAKETE-BUILD51-20261105.md) |
| 1.0.3 | 48 (Zähler fortgeführt) | 0.1.6 | Eingefroren am 03.11.2026: gemeinsame Cutlist-Tabelle, Rot/Grün und dynamische Vorschau, Power-User 2 mit Buttonkorrektur; 1728 Tests, Commit/Push beauftragt; Veröffentlichung erst nach Freigabe |
| 1.0.0 | 32 | 0.1.5 | Früherer stabiler Stand |
| 1.0.1 | 34 | 0.1.6 | Veröffentlichung zurückgenommen; datierter Nachweis |
| 1.0.2 | 35 | 0.1.6 | MP4Box-Übergangszuordnung |
| 1.0.2 | 36 | 0.1.6 | Schnittmotor-Cutlist-Metadaten |
| 1.0.2 | 37 | 0.1.6 | Bestätigter AAC-Notfallweg |
| 1.0.2 | 38 | 0.1.6 | Damalige Einzelkopie für externes Kleben |
| 1.0.2 | 39 | 0.1.6 | Integriertes Klebecenter; Voraussetzungstext noch fälschlich 0.1.5 |
| 1.0.2 | 40 | 0.1.6 | Hinweis berichtigt; Setup-/ZIP-Prüfung persönlich abgeschlossen |
| 1.0.2 | 41 | 0.1.6 | Früherer stabiler Release v1.0.2 vom 27.10.2026; bleibt erhalten |
| 1.0.2 | 42 | 0.1.6 | Lokaler Entwicklungsbuild: Phasenzeiten und direkte RF64-Zusammensetzung; nicht paketiert/veröffentlicht |
| 1.0.2 | 43 | 0.1.6 | Lokale Prüffassung: QUICK AND COPY mit AAC-/AC-3-Mehrspurzuordnung, unveränderter AUDIO-ENTHUSIAST, Tonspurwahl im Kontrollplayer; Setup/Portable/Quellen lokal geprüft, nicht veröffentlicht |
| 1.0.2 | 44 | 0.1.6 | Lokale Menüvereinfachung: separate Tonspurkopie im Klebecenter ausgeblendet; Auswahl beim Zusammenfügen bleibt. Menü persönlich bestätigt; Setup/Portable/Quellen lokal technisch geprüft; persönliche neue Paketabnahme offen, nicht veröffentlicht |
| 1.0.2 | 45 | 0.1.6 | Lokaler Hinweis am Tonspurpaar-Haken und Erklärung doppelter Zuordnung; nur XAML-Text/Textumbruch, persönlich bestätigt und eingefroren; Setup/ZIP/Quellen technisch geprüft und schreibgeschützt eingefroren; neue persönliche Paketabnahme offen, nicht veröffentlicht |
| 1.0.2 | 46 | 0.1.6 | Bestätigtes pragmatisches QUICK mit Meckerecke eingefroren; Setup/ZIP/Quellen technisch geprüft und schreibgeschützt; Quellstand committet/gepusht, Veröffentlichung erst nach Ansage |
| 1.0.2 | 48 | 0.1.6 | Stabile Latest-Version v1.0.2-build48 vom 01.11.2026; Kontrollplayer-EOF und Positionserhalt korrigiert, 1634 Pakettests + 1362 finale DLL-Tests + 32 native Kontrollen |
| 1.0.2 | 47 | 0.1.6 | Stabile Vorgängerversion v1.0.2-build47, am 31.10.2026 veröffentlicht; MP4Box-Sperre, Setup-/Portable-Nachweise und acht identische Uploads dokumentiert |

Aktuell veröffentlichte App-Dateiversion **1.0.2.48**, stabile Latest-Version **v1.0.2-build48**, libmpv **0.41.0**, otr-can **0.1.6**, .NET 10, Windows 11 x64/WPF. [Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD48-PUBLISHED-20261101.md). Build 47 bleibt unverändert erhalten.

Lokale Entwicklungs-Dateiversion **1.0.3.48**; die gleiche Buildzählerzahl bedeutet keine Identität mit dem veröffentlichten **1.0.2.48**-Paketsatz. Keine neue Buildnummer vergeben. [Entwicklungsnotizen](RELEASE-NOTES-1.0.3.md), [Freeze](FREEZE-1.0.3-20261103.md).




## Technische Nachweise

- Entwicklung 1.0.3: **1728/1728 Tests** (200 Core, 80 Cutlists, 390 Media, 1058 App), Release-Build ohne Warnungen/Fehler. Reale WPF-Controls, persönliche Anzeigeprüfung und anschließend vollständiger grüner Santa-Tell-Me-Filmschnitt positiv gemeldet. [Praxisnachweis](SANTA-TELL-ME-GRUENER-PRAXISTEST-20261103.md). [Schnittmodi](SCHNITTMODI-TIMELINE-20261103.md), [Cutlist-Tabelle und Chicago-Fire-Praxis](CUTLIST-TABELLE-IMPLEMENTIERUNG-20261103.md).

- Build 48: 1634 Pakettests, zusätzlich 1362 Tests gegen finale Produktbibliotheken, 32 native/WPF-Kontrollen in sechs Prozessen. Persönliche korrigierte Ghosts-Teil-1-/Teil-2-Abnahme bestätigt. [Pakete und Prüfsummen](RELEASE-1.0.2-BUILD48-RESULT.md).

- Build 47: 1593 Pakettests und zusätzlich 1321 Tests gegen finale DLLs, native MP4Box-/otr-can-Erkennung und zwei Dialoggrößen positiv. Setup-/Portable-Funktionsnachweise getrennt dokumentiert, Veröffentlichung nach ausdrücklicher Nutzerfreigabe erfolgt. [Pakete und Prüfsummen](RELEASE-1.0.2-BUILD47-RESULT.md), [Veröffentlichung](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md).

- Build 46: 1576 Tests, 27 native QUICK- und 21 bisherige Fälle; neun bytegleiche Enthusiast-Ausgaben; drei reale Referenzen erfolgreich. [Ablauf und Grenzen](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md).

- Build 43: 28 native QUICK-Fälle, 21 bisherige Klebecenter-Fälle und neun bytegleiche Neukodierungsausgaben gegen Build 42; echte John-Wick-/Furiosa-Mehrspurprüfung und native Player-Umschaltung. [Nachweis und Grenzen](KLEBECENTER-QUICK-COPY-20261028.md).

- Build 41: 1489 Pakettests, zusätzlich 1217 Tests gegen finale DLLs und 21 native Klebecenter-Fälle einschließlich Erfolgs-/Fehler-/Abbruchbereinigung. Persönliche Setup-Prüfung auf dem Arbeitsrechner mit John Wick positiv, einschließlich verschwundener temporärer Datei; neue ZIP-/VM-Prüfung getrennt offen. [Paketbericht](RELEASE-1.0.2-BUILD41-RESULT.md).

- Build 38: 1446 Pakettests, zusätzlich 1174 Tests gegen finale DLLs, zehn native Fälle.
- Build 39: 1485 Pakettests, zusätzlich 1213 Tests gegen finale DLLs, 21 Klebecenter- und zehn bisherige native Fälle.
- Build 40: 1485 Pakettests und zusätzlich 1213 Tests gegen finale DLLs erneut bestanden. 361 Anwendungs-/Testquellen bytegleich zu Build 39; dessen 31 native Fälle wurden für die Textkorrektur nicht wiederholt.
- Persönlich: John Wick AC-3/AAC und AAC/AAC mit Bild/Ton/Übergang bestätigt. Nach Umstieg auf Setup Build 40 auch beide Furiosa-otr-can-Schnitte und Klebecenter samt allen Prüfstellen positiv. Screenshot bestätigt Hinweis 0.1.6. Arbeits-PC-Funktionslauf der Setup-Prüfrunde positiv; zusätzliche Portable-/VM-/Deinstallationsvarianten bleiben offen. [Furiosa-Praxisbericht](FURIOSA-KLEBECENTER-PRAXIS-20261027.md).

Ein neuer CAN-Build bedeutet keinen neuen Motor. Historische 0.1.3-/0.1.4-/0.1.5-Angaben werden nicht auf 0.1.6 umgeschrieben. „MP3-Unterstützung ab 0.1.5“ bleibt richtig.

Power-User 2 ist zusätzlich lokal implementiert: zwölf native Prüffallgruppen mit geladenem Film und WPF-DPI 100/125/150/200 %. [Bericht und WPF-Aufnahmen](POWER-USER-2-IMPLEMENTIERUNG-20261103.md). Die Buildzählerzahl bleibt 48; dies ist keine neue Build-48-Paketveröffentlichung.

## Quellen und Pakete

Aktuell **artifacts/release-1.0.2-build48**: Setup, Portable-ZIP, **CutAssistantNext-1.0.2-Build48-source.zip**, passende unveränderte Motor-/libmpv-Quellen, Relink und Prüfsummen. SOURCE-SNAPSHOT.json und SOURCE-FILES.sha256 nennen die exakten Eingaben. [Paketbericht 48](RELEASE-1.0.2-BUILD48-RESULT.md).

**artifacts/release-1.0.2-build40** mit **CutAssistantNext-1.0.2-Build40-source.zip**, SOURCE-SNAPSHOT.json/SOURCE-FILES.sha256. Passend dazu unverändert **otr-can-0.1.6-source.zip**, **CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip**, Relink-Paket und SHA256SUMS.txt. Keine echten Filme im Repository.

[Paketbericht 40](RELEASE-1.0.2-BUILD40-RESULT.md), [Paketbericht 39](RELEASE-1.0.2-BUILD39-RESULT.md), [Rücknahme 1.0.1](RELEASE-WITHDRAWAL-1.0.1-20261020.md), [stabiler Build 32](RELEASE-1.0.0-BUILD32-RESULT.md).

</details>
