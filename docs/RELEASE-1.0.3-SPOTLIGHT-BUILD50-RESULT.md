# CAN 1.0.3 Build 50 – lokaler Spotlight-Paketsatz

Stand **08.11.2026**. **1.0.3 · Build 50 · Spotlight-Entwicklung**, Dateiversion **1.0.3.50**. Setup, Portable und vollständige Quellen lokal gebaut und technisch geprüft. **Noch kein öffentliches Release.** Quellcommit **185e8b350ea0fb5d76f19fefb10bcf220b559203** auf `codex/spotlight-integration` erfolgreich gepusht und per `git ls-remote` bestätigt. Ein späterer reiner Dokumentationscommit ergänzt diesen Ergebnisbericht; paketierter Quellcommit und Produktdateien bleiben unverändert.

## Herkunft und Erhaltung

Der endgültige Build stammt ausschließlich aus einem frischen Git-Export dieses Quellcommits, ohne Arbeitsbaum-Overlay. Vollständige Spotlight-Quellen aus **6973b37591e9b581b20b5a6f2117369b6df0f159** zusätzlich unter `external/spotlight-can` exportiert. CAN-Quell-ZIP und separater Spotlight-Quell-ZIP enthalten bytegleich denselben Spotlight-Stand. **814 manifestierte Dateien**, einschließlich Snapshot-/Bauhinweisen; **815 Dateien** im CAN-Quell-ZIP einschließlich Manifest. Keine Git-Metadaten, Filme, lokalen Einstellungen oder Buildausgaben enthalten.

`SOURCE-SNAPSHOT.json`, `BUILD-SOURCE.txt`, `SOURCE-FILES.sha256` und `publish/RELEASE-PROVENANCE.json` ordnen Quellcommit, Spotlight-Commit, Quell-ZIP-Hash, Dateimanifest und **SDK 10.0.303** zu. Die Quelle im Archiv bildet bewusst den Quellcommit ab und enthält die Paketvorbereitung; dieser nach dem Bau entstandene Ergebnisnachtrag wird nicht nachträglich in den Quell-ZIP eingefügt.

Die Paketroutine wurde zuerst aus einem geprüften Snapshot vor dem Commit vollständig ausgeführt und geprüft. Dessen Git-Baum stimmt exakt mit dem anschließend erzeugten Quellcommit überein. Danach endgültiger neuer Export, Paketbau und erneute Prüfung. Überall ausdrücklich `CanBuildNumber=50` und `CanCandidate=Spotlight-Entwicklung`; das hochzählende `tools/build.ps1` wurde nicht ausgeführt. Lokaler Zähler bleibt **50**. Versionsdatei unverändert.

**2345 geschützte Dateien** in CAN-main, den separaten Audio-Checkouts, bisherigen stabilen/lokalen Paketsätzen und den lokalen Buildzählern unverändert geprüft. Keine ungeprüfte Audio-Build-51-Übernahme, keine Änderung an otr-can-Normalmodus, MP4Box, CAN-Player oder Cutlist-Logik. Die Spotlight-Quellunterkomponente selbst ist unverändert. Öffentlich stabil bleibt **CAN 1.0.2 Build 48**; `main` bleibt unverändert.

## Technische Ergebnisse

| Prüfung | Ergebnis |
| --- | --- |
| Vollständiger Snapshot-Release-Build | **0 Warnungen / 0 Fehler** |
| Frameworkabhängiger win-x64-Publish | **0 Warnungen / 0 Fehler** |
| Ganze CAN-Suite im finalen Snapshot | **1773/1773**, nichts übersprungen |
| CAN-Suiten nochmals mit finalen Paket-DLLs | **1773/1773** |
| Spotlight-Suiten im finalen Snapshot | **78/78**, nichts übersprungen |
| Spotlight nochmals mit finalen Paket-DLLs | **78/78** |
| Native CAN-/Spotlight-Integration mit finalen DLLs | **32/32** |
| Kontrollplayer: sechs native/WPF-Prozesse | **32/32** |
| Fensterzustand: vier getrennte CAN-/Spotlight-Prozesse | **4/4** |
| Neue Fensterprüfungen innerhalb der CAN-Suite | **29/29** |
| Portable gegen Publish-Dateimanifest | **258/258 Dateien bytegleich**, keine zusätzlichen/fehlenden |
| Inno Setup 7 | Erfolgreich kompiliert, exakt dieselben **258 Eingabedateien** |
| Alle sechs ZIPs | Vollständige CRC-Prüfung erfolgreich |
| Quellen-/Motor-/Lizenzdateien und SHA-256 | Manifest-, Herkunfts- und Inhaltsprüfung erfolgreich |

CAN: Core 200, Cutlists 80, Media 390, App 1103. Spotlight: Core/Media 59, Bedienung/Host 19. Wiederholungen mit Paket-DLLs sind keine zusätzlichen unterschiedlichen Tests. Die mit Paket-DLLs überschriebenen Prüfordner wurden vor Ausführung bytegenau gegen Publish geprüft.

Native Spotlight-Prüfung: künstlicher zwölfsekündiger Film, echte Übersicht-/Detailbilder, dynamische Laufzeiten, direkte Übernahme in Rot/Grün, Ablehnung/Bestätigung des Ersetzens, Cutlist-Rundlauf, Abbruch und Ressourcenfreigabe. Kontrollplayer: Tonspurwechsel bei Wiedergabe/Pause, EOF, Property-Fehler, Navigation, zwei getrennte Playerkontexte und Ressourcenlebensdauer. Kein neuer Realfilmschnitt oder Server-Upload.

Fensterprozesse: **1200×850 bei X=1960/Y=40 auf Monitor 2** in einem weiteren Prozess unverändert wiederhergestellt; zusätzlich maximiertes Schließen/Öffnen mit denselben normalen Rückkehrmaßen. Normales/minimiertes Schließen, abgebrochenes Schließen, negative Koordinaten, fehlender/dritter Monitor, kleine Auflösung, DPI-Wechsel und beschädigte Daten werden durch die 29 Fensterfälle abgedeckt. Tatsächlich zwei verfügbare 1920×1080-Arbeitsflächen verwendet; negative Monitorursprünge, dritter Monitor, Monitorverlust und DPI-Wechsel sind simuliert. Persönliche Fensterprüfung vom 07.11.2026 bleibt positiv bestätigt.

## Installer, Laufzeit und Quellenbeigaben

Setup-Produktversion **1.0.3**, Anwendungs-Dateiversion **1.0.3.50**; Build/Kandidat stehen im unveränderten Inno-AppVerName-Schema und den Compilerargumenten. Die äußeren Dateinamen folgen nach der unten dokumentierten Umbenennung dem üblichen CAN-Schema. Die leere Setup-Dateiversionsressource entspricht dem bisherigen Installerschema. Setup-Kompression und Compilerprüfung erfolgreich; Eingabedateien vollständig gegen Publish abgeglichen. Das Setup selbst wurde hier nicht installiert oder zur Laufzeit entpackt.

Vier libmpv-DLLs SHA-256-identisch zum geprüften **0.41.0**-Stack. Unverändertes **otr-can 0.1.6** einschließlich Quell-ZIP, Lizenz-/Herkunftsdateien und Motor-Dateimanifest. Motor-EXE-SHA-256: **73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E**. Fähigkeiten **mp3-aac-v1 / aac-overlap-v1** erneut geprüft. Der gesonderte Motor-Quell-ZIP entspricht bytegenau der beigefügten Kopie im Publish. Keine neue Behauptung eines bitidentischen Motor-Neubaus.

Beide bisherigen libmpv-Prüf-/Relinkarchive unverändert übernommen, vollständig CRC-geprüft und erneut gehasht. Vorhandene CAN-Lizenz, NuGet-/Microsoft-/libmpv-/otr-can-Hinweise enthalten; Spotlight-Herkunft und README zusätzlich beigelegt. Im Spotlight-Quellcommit fehlt eine eigene LICENSE-Datei. Dafür wurde keine Lizenz erfunden; die öffentliche Lizenzzuordnung bleibt **vor einer späteren Veröffentlichung offen**. [Herkunft](SPOTLIGHT-QUELLEN.md).

**Windows 11 x64** ist die persönlich geprüfte Zielplattform. **Windows 10 ist nicht persönlich validiert.** .NET 10 Desktop Runtime x64, FFmpeg/ffprobe, GPAC/MP4Box und gegebenenfalls FFMS2/ffmsindex sowie Visual-C++-Laufzeit bleiben separat einzurichten. Kein .NET-, FFmpeg-, ffprobe-, MP4Box- oder FFMS-Laufzeitbündel. Die Portable-Struktur ist vollständig für den dokumentierten frameworkabhängigen Betrieb.

## Dateien, Größen und SHA-256

Paketordner:
**`C:\Users\joerg\Documents\GitHub\cut-assistant-next-spotlight\artifacts\release-1.0.3-build50`**

| Datei | Bytes | SHA-256 |
| --- | ---: | --- |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-1.0.3-Build50-source.zip | 6341654 | 31200C22667786751BB05FED7D53F461090A36F73239B92C7C3D888ADAE4B261 |
| CutAssistantNext-1.0.3-Build50-win-x64-Setup.exe | 20648565 | 5AC4BEE806B782345CDD7A1282D1D6B6D70E02E042CD0DF8F49C4E1CA29E3603 |
| CutAssistantNext-1.0.3-Build50-win-x64.zip | 25424103 | A6AF6C8C12E4BE35B08A471CF20E2322AB5F2BE9A54D33F06AEEA218279E5882 |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| PUBLISH-FILES.sha256 | 33650 | 0400E37362EAD9D7347708F26201E02230AD1B98E15DA0489A78488843AF1C58 |
| README-Spotlight.md | 3143 | 8EE8B457AD901A48CA03AC06CF908E3F81D5E9690000EAE6A7C16A151F5AE764 |
| SHA256SUMS.txt | 921 | 68E24E14B58D29AAC880DC062C72A0B46749390FCB19252FA7CD6C8DF1BC953D |
| SpotlightCan-6973b37-source.zip | 740232 | D52E0E8B516FAF55A2F38C8075D6A5BD36494279F631C15AE9D5FA22415C04E0 |

`SHA256SUMS.txt` erfasst alle übrigen neun auszuliefernden Dateien einschließlich `PUBLISH-FILES.sha256`; der eigene Hash steht in dieser Tabelle. `PUBLISH-FILES.sha256` beschreibt alle 258 Laufzeit-/Hinweisdateien. Keine Logdateien, Diagnosefilme oder Testergebnisse in Portable oder Setup. Paketlogs und TRX liegen separat in `logs/` beziehungsweise `test-results/`, nicht in der Auslieferungsstruktur.

## Offene persönliche Prüfungen und Git-Status

- **Persönliche Setup-Prüfung auf dem Arbeitsrechner inzwischen positiv:** Buchladen der Liebe mit MP4Box und otr-can, jeweils Bild/Ton/Schnitt/Drei-Sekunden-Vorschau. otr-can ohne Vorgängersendungs-Blitzer, MP4Box mit Anfangsblitzer laut eigener Cutlist. Spotlight-Grobauswahl und Maximierungserhalt bestätigt. [Praxisnachweis](BUCHLADEN-DER-LIEBE-SETUP-PRAXIS-20261108.md). VM-Setup inzwischen mit MP4Box und otr-can am Film Grey's Anatomy – Durchs Feuer für Schnitt/Bild/Ton/Drei-Sekunden-Vorschau/Spotlight positiv bestätigt. [VM-Nachweis](VM-SETUP-BUILD50-20261108.md). Installationstyp (frisch/Überinstallation), sauberer VM-Ausgangszustand, konkrete Gast-Windows-Version, Deinstallation und Portable nicht gesondert bestätigt. Die automatisierte Prüfung selbst hat keine Installation auf dem Arbeitsrechner ausgeführt.
- Das Setup verwendet die bisherige CAN-Installationsidentität und kann eine bestehende Installation aktualisieren. Zur persönlichen Setup-Abnahme ein getrenntes Testsystem verwenden. Portable in einen neuen Ordner entpacken und Werkzeugpfade, insbesondere den beigelegten otr-can, kontrollieren.
- Physische Monitorabkopplung und globale Windows-Skalierungswechsel nicht durchgeführt; simulierte Tests sind entsprechend gekennzeichnet. Persönliche Fensterkomfort-Bestätigung ist keine nachträgliche Setup-/Portable-Abnahme.
- Öffentliche Spotlight-Lizenzzuordnung vor einer späteren Release-Entscheidung klären. Dieser lokale Kandidat ist technisch geprüft, keine öffentliche Auslieferungsfreigabe.

Commit und Push des Quellcommits sind erfolgreich und remote bestätigt. Nach dem Quellcommit war der Spotlight-Arbeitsbaum sauber. Dieser reine Ergebnisnachtrag wird in einem gesonderten Dokumentationscommit auf demselben Zweig gesichert; die abschließende Branchspitze und der saubere Git-Status werden im Chat-Abschluss und lokalen Git-Nachweis festgehalten. **Kein Merge nach main, kein Release-Tag, kein GitHub-Release und kein Upload der Pakete.**

Prüfnachweise im ignorierten `.build/`: `spotlight50-final-package.log`, `spotlight50-final-archives.json`, `spotlight50-final-qa/` mit TRX/nativen JSON/Prozesslogs, `spotlight50-final-qa.log`, `spotlight50-source-push.txt`, `spotlight50-preserved.json` und Export-Metadaten unter `spotlight50-final-source/`. Die vorgelagerte Prüfung vor dem Quellcommit ist getrennt unter `spotlight50-precommit-*` erhalten. Kleine Hilfs-Ausleseprüfungen wurden auf LiteralPath für TRX-Namen mit eckigen Klammern korrigiert; keine Test-/Produktänderung, keine Testentfernung und kein fehlgeschlagener Suite-Lauf in diesem Paketauftrag.

[Bedienung](NUTZERANLEITUNG.md) · [Release Notes](RELEASE-NOTES-1.0.3.md) · [Fensterprüfung](SPOTLIGHT-FENSTERZUSTAND-20261107.md) · [Paketverfahren](PAKETVORBEREITUNG-SPOTLIGHT-BUILD50-20261108.md).

## Nachtrag 08.11.2026 – übliche CAN-Paketnamen

Auf Nutzerwunsch Paketordner und die drei CAN-Paketdateien ohne Spotlight-Zusatz umbenannt. Setup, Portable-ZIP und CAN-Quell-ZIP sind bytegleich zu den zuvor geprüften Dateien; ihre SHA-256-Werte bleiben gleich. Anwendung, Quellstand, Build 50 und die technischen Kandidatenkennungen wurden nicht geändert. Kein neuer Build erforderlich.

SHA256SUMS.txt nennt nun die üblichen Dateinamen; dessen neuer eigener Hash steht oben. Alle sechs ZIPs, 258 Publish-/Portable-Dateien, Installer-Eingaben und sämtliche Prüfsummen nach der Umbenennung erneut erfolgreich geprüft. Interne Herkunftsdaten und das Quell-ZIP-Wurzelverzeichnis bewahren die ursprünglichen Namen zum Bauzeitpunkt; der identische Quell-ZIP-Hash ordnet den heutigen Dateinamen eindeutig zu. Der bisherige Nachweis gilt weiter. Umbenennungsnachweis: .build/spotlight50-renamed.json und spotlight50-renamed-archives.json. Keine neue Veröffentlichung.

## Persönliche Setup-Abnahme – Buchladen der Liebe, 08.11.2026

Der Nutzer bestätigt die Setup-Fassung auf dem Arbeitsrechner mit MP4Box und otr-can für Bild, Ton, Schnitt und Drei-Sekunden-Vorschau positiv. Spotlight wurde zur Grobauswahl genutzt; maximiertes Öffnen bleibt erhalten. Die eigene Cutlist bestätigt fünf Behaltebereiche, 25 fps und 01:22:02.120. otr-can zeigt nach persönlicher Rückmeldung keinen Vorgängersendungs-Blitzer; für MP4Box steht im Cutlist-Kommentar ausdrücklich ein Blitzer der vorherigen Sendung am Anfang. [Vollständiger Praxisnachweis](BUCHLADEN-DER-LIEBE-SETUP-PRAXIS-20261108.md).

Neue [Spotlight-README](SPOTLIGHT-README.md): Vorschaubilder standardmäßig im einstellbaren Zwei-Sekunden-Abstand; den letzten betroffenen Bereich, der bis zum tatsächlichen Original-Dateiende reichen soll, anschließend in CAN mit **Videoende** abschließen. Die kurze Anleitung liegt zusätzlich als **README-Spotlight.md** neben Setup/Portable im Paketordner und ist in SHA256SUMS.txt erfasst. Die obige Tabelle und der Hash der Prüfsummenliste sind dafür aktualisiert. Setup, Portable, Quellen und alle übrigen ursprünglichen Auslieferungsdateien bleiben bytegleich; Build 50 unverändert.

Nur Dokumentations- und Ausleseprüfung, kein neuer Build oder Suite-Lauf. Cutlist unverändert gelesen, Zeit-/Bild-/Samplezahlen mit Dezimalrechnung abgeglichen; sechs Archive, 258 Portable-/Setup-Eingabedateien und sämtliche Prüfsummen erneut erfolgreich geprüft. Saubere VM, Portable-Abnahme, Deinstallation und physische Skalierungswechsel bleiben offen. Kein neues öffentliches Release.

## Persönliche VM-Setup-Prüfung – 08.11.2026

Der Nutzer bestätigt **Setup in der VM: Schnitt, Bild, Ton, Drei-Sekunden-Vorschau und Spotlight bestanden**. Das beigefügte MP4Box-Protokoll zu **Grey’s Anatomy – Die jungen Ärzte: Durchs Feuer** zeigt drei erfolgreiche Segmente, Zusammenfügen und abschließende Abschnittsprüfung mit „Fertig.“. [Details und Prüfgrenzen](VM-SETUP-BUILD50-20261108.md).

Damit ist die persönliche MP4Box-/Spotlight-Nutzung der Setup-Fassung in der VM positiv belegt. Portable, Deinstallation, saubere Neuinstallation und genaue Gast-Windows-Version sind nicht separat bestätigt. otr-can ist durch den nachfolgenden persönlichen Nachtrag ebenfalls positiv geprüft. Die oben zuvor offene VM-Abnahme ist durch diesen Nachtrag in diesem konkreten Umfang aktualisiert. Nur Dokumentation, keine Änderung an Produktcode, Build 50, Paketen oder Prüfsummen; kein erneuter Build/Suite-Lauf und keine Veröffentlichung.

## Persönlicher VM-Nachtrag – otr-can ebenfalls bestanden

Der Nutzer bestätigt anschließend auch **otr-can 0.1.6** in der VM mit **Grey’s Anatomy – Durchs Feuer** für Schnitt, Bild, Ton und Drei-Sekunden-Vorschau positiv. Das nachgereichte Protokoll nennt **60651 Videopakete** und **116449920 Audiosamples bei 48000 Hz**, rechnerisch übereinstimmend **00:40:26.040**; finale MP4-Prüfung und CAN-Abschluss erfolgreich. [Vollständiger VM-Nachweis](VM-SETUP-BUILD50-20261108.md).

Damit ist die persönliche VM-Setup-Prüfung für **beide Schnittmotoren und Spotlight** positiv dokumentiert. Portable und Deinstallation bleiben offen; Installationstyp, sauberer Ausgangszustand und genaue Gast-Windows-Version wurden nicht separat benannt. Nur Dokumentation und rechnerischer Protokollabgleich, keine Produkt-/Paketänderung, kein erneuter Build/Suite-Lauf und keine Veröffentlichung.
