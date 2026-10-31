# Cut Assistant Next

Cut Assistant Next ist ein Windows-Programm zum Abspielen von Videos, Bearbeiten klassischer Cutlists und Schneiden mit MP4Box oder otr-can. Es verwendet WPF und mpv/libmpv, ohne DirectShow oder installierte Windows-Codec-Pakete vorauszusetzen.

**Stand 31.10.2026: CAN 1.0.2 Build 47 ist eingefroren und zur Veröffentlichung freigegeben.** App-Dateiversion **1.0.2.47**, Motor **otr-can 0.1.6**, Wiedergabe **libmpv 0.41.0**, .NET 10/WPF für Windows 11 x64. Die Veröffentlichung läuft unter **v1.0.2-build47**; der frühere Release v1.0.2 mit Build 41 bleibt erhalten.

[Setup Build 47](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.2-build47/CutAssistantNext-1.0.2-Build47-win-x64-Setup.exe) · [Portable-ZIP](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.2-build47/CutAssistantNext-1.0.2-Build47-win-x64.zip) · [CAN-Quellen](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.2-build47/CutAssistantNext-1.0.2-Build47-source.zip) · [Release, passende Drittanbieterquellen und Prüfsummen](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2-build47). Die Links werden mit Abschluss der freigegebenen Veröffentlichung verfügbar.

## Klebecenter

Unter **Klebecenter → Filmteile zusammenfügen …** zwei fertig geschnittene MP4-Dateien wählen, Reihenfolge prüfen und das gewünschte Verfahren bewusst starten:

| Filmteile | Verfahren |
|---|---|
| Erkannte GPAC/MP4Box-Ausgaben | **QUICK AND COPY**; AUDIO-ENTHUSIAST samt Tonspurauswahl gesperrt, Erklärung direkt bei den Startknöpfen |
| Geeignete otr-can-Ausgaben | **QUICK AND COPY** oder **AUDIO-ENTHUSIAST** |

QUICK übernimmt Video und bestätigte kompatible AAC-/AC-3-Spurpaare ohne Neukodierung. Hinweise und Werkzeugwarnungen stehen in der **Meckerecke**; es gibt keine samplegenaue Zusage und keine zusätzliche Vollprüfung. AUDIO-ENTHUSIAST verbindet genau eine gewählte Tonspur je Teil samplegenau und kodiert sie einmal durchgehend als AAC-LC. Das Bild bleibt bei beiden unverändert. Die GPAC-Erkennung nutzt eine Container-Kennung; bei fehlender Kennung gelten die bisherigen technischen Vorprüfungen. Keine automatische Verfahrensumschaltung.

Nach Abschluss Anfang, Übergang und Ende mit drei Sekunden Vorlauf kontrollieren, bei mehreren Tonspuren jede erhaltene Spur auswählen. QUICK-Prüfzeiten sind angenähert. [Bedienung](docs/NUTZERANLEITUNG.md), [MP4Box-Sperre und Grenzen](docs/MP4BOX-AUDIO-ENTHUSIAST-SPERRE-20261031.md), [QUICK-Regeln](docs/KLEBECENTER-QUICK-PRAGMATISCH-20261029.md).

## Prüfung und Herkunft

**1593 Pakettests** und zusätzlich **1321 Tests gegen die finalen Produktbibliotheken** bestanden; Release-Build ohne Warnungen/Fehler. **250 Portable-Dateien**, **705 manifestierte Quelldateien / 706 Quellen-ZIP-Dateien** und alle acht Auslieferungsdateien vollständig geprüft. Der Paketsatz unter **artifacts/release-1.0.2-build47** bleibt unverändert und schreibgeschützt; vorbestehende lokale Paketlistenänderungen sind aus Quellenarchiv und Commit ausgeschlossen. [Paketbericht](docs/RELEASE-1.0.2-BUILD47-RESULT.md), [Veröffentlichungsnachweis](docs/RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md).

Persönlich bestätigt sind im **Build-47-VM-Setup** die drei Wege MP4Box → QUICK, otr-can → QUICK und otr-can → AUDIO-ENTHUSIAST sowie Sperr-/Dateiwechselchecks. In der **Portable-ZIP** sind beide QUICK-Wege mit Bild/Ton/Schnitt-/Vorschaukontrolle positiv; der otr-can-AUDIO-ENTHUSIAST-Lauf ist technisch erfolgreich, seine persönliche Kontrolle nicht separat zugeordnet. Deinstallation ist für Build 47 nicht gesondert bestätigt. Die anschließende ausdrückliche Nutzerfreigabe zum Einfrieren, Commit/Push und Veröffentlichen ist am 31.10.2026 erteilt. [Setup-Nachweise](docs/VM-SETUP-BUILD47-20261031.md), [Portable-Nachweise](docs/VM-PORTABLE-BUILD47-20261031.md).

Die aktuellen Arbeitsrechner-Praxistests **Rocky Horror (50 fps/otr-can), Doc und Brilliant Minds (25 fps/MP4Box)** sind persönlich positiv und gehören ausdrücklich zum **Build-46-Setup**, nicht zur Build-47-VM. [Praxisbericht](docs/PRAXISTESTS-20261031.md). Frühere Pakete und ihre datierten Abnahmen bleiben erhalten.

## Einrichtung und bekannte Grenzen

Setup und ZIP enthalten den geprüften libmpv-Stack und **tools/otr-can/otr_can.exe** in Version 0.1.6. **.NET 10 Desktop Runtime x64** und für otr-can die Visual-C++-x64-Laufzeit separat installieren. FFmpeg/ffprobe, MP4Box und FFMS2 werden separat eingerichtet. Nach Überinstallation den gespeicherten Motorpfad auf die mitgelieferte EXE prüfen. [Nutzeranleitung](docs/NUTZERANLEITUNG.md), [Versionen](docs/VERSIONEN.md), [Voraussetzungen](installer/HINWEIS-DOTNET-UND-WERKZEUGE.txt).

AVI bleibt experimentell. MP4Box kann Schnittgrenzen verschieben und Werbeblitzer belassen; Prüfpositionen stellen fehlende Bilder nicht wieder her. Die Ursache früherer vollständiger Rechnerstillstände bei großen otr-can-Schnitten ist ungeklärt; spätere positive Läufe belegen keine gezielte Behebung. [Diagnosestand](docs/OTR-CAN-MAD-MAX-SYSTEMFREEZE-20261020.md).

[Aktueller Projektstand](docs/AKTUELLER-STAND.md) · [Dokumentationsübersicht](docs/README.md) · [Release Notes](docs/RELEASE-NOTES-1.0.2.md) · [Testplan](docs/TESTPLAN.md)

<details>
<summary>Historische Entwicklungs-, Paket- und Abnahmestände bis Build 38</summary>

Die folgenden Einträge bewahren den jeweiligen damaligen Stand. Spätere persönliche Bestätigungen und der aktuelle Veröffentlichungsstatus stehen im Überblick oben und in den verlinkten Berichten.

**Aktuelle stabile Version: CAN 1.0.0 · Build 32.** Seit **20.10.2026** wieder als **Latest** angeboten: [Setup](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.0/CutAssistantNext-1.0.0-Build32-win-x64-Setup.exe), [Portable-ZIP](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.0/CutAssistantNext-1.0.0-Build32-win-x64.zip), [alle Downloads einschließlich Quellen und Prüfsummen](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.0). **CAN 1.0.1 · Build 34 wurde auf Nutzerwunsch vorübergehend aus der öffentlichen Veröffentlichung genommen und als Entwurf erhalten.** [Rücknahmenachweis](docs/RELEASE-WITHDRAWAL-1.0.1-20261020.md). **Offener kritischer Befund:** kompletter Rechnerstillstand beim erneuten otr-can-Schnitt einer großen Mad-Max-/Furiosa-Datei; Erneut auch unter 2-GiB-/25-Prozent-Ressourcengrenzen aufgetreten; weitere Realfilm-Schnitttests gestoppt. Ursache noch nicht nachgewiesen. [Diagnosestand](docs/OTR-CAN-MAD-MAX-SYSTEMFREEZE-20261020.md). Die Rückkehr zu Build 32 ist kein nachgewiesener Schutz für diesen konkreten Fall. Die folgenden Einträge dokumentieren auch frühere Entwicklungs-, Abnahme- und Veröffentlichungsstände.

**Neuer lokaler Paketsatz: CAN 1.0.2 · Build 38** (26.10.2026). Neue bewusste Tonspurauswahl für weitere bereits geschnittene Filmteile: zusätzliche MP4-Kopie ohne Neukodierung, strenge Prüfung einschließlich exakter Wiedergabezeiten. 1446 Entwicklungstests, zehn native Fälle und echter John-Wick-Teil-1-AAC-Test erfolgreich. Setup, Portable und Quellen getrennt von Build 37 technisch geprüft: 1446 Pakettests, zusätzlich 1174 Tests gegen finale Produktbibliotheken und zehn native Fälle. Persönliche Paket-/Dialogabnahme offen; keine Veröffentlichung ohne Freigabe. [Paketbericht](docs/RELEASE-1.0.2-BUILD38-RESULT.md). [Bedienung und Grenzen](docs/TONSPURAUSWAHL-FILMTEILE-20261026.md), [Paketumfang](docs/FREEZE-1.0.2-BUILD38-20261026.md).

**Neuer lokaler Paketsatz: CAN 1.0.2 · Build 37** (26.10.2026). Bei erkannter otr-can-Tonstörung kann nach ausdrücklicher Rückfrage mit einer streng geprüften AAC-Spur erneut geschnitten werden. Der Spurverlust wird sichtbar genannt. 1429 Entwicklungstests und native Fehler-/Normalfälle erfolgreich; John Wick Teil 2 mit AAC persönlich als passend bestätigt. Setup, Portable und Quellen getrennt von Build 36 technisch geprüft; 1429 Pakettests, zusätzlich 1157 Tests gegen finale Paketbibliotheken. Persönliche Paketabnahme offen; keine Veröffentlichung ohne Freigabe. [Paketbericht](docs/RELEASE-1.0.2-BUILD37-RESULT.md), [Paketumfang](docs/FREEZE-1.0.2-BUILD37-20261026.md), [Umsetzung und Grenzen](docs/OTR-CAN-AAC-NOTFALLWEG-20261026.md).

**Lokaler Nachfolgestand: CAN 1.0.2 · Build 36** (25.10.2026). Die Cutlist-Metadaten können den tatsächlich vorgesehenen Motor otr-can ausweisen. Setup, Portable, Quellen und Prüfsummen liegen getrennt von Build 35 lokal bereit; 1416 Pakettests und 898 weitere App-Tests gegen die finalen Portable-DLLs bestanden. Persönliche Paketabnahme offen. **Keine Veröffentlichung ohne ausdrückliche Freigabe.** [Paketbericht](docs/RELEASE-1.0.2-BUILD36-RESULT.md), [Release Notes](docs/RELEASE-NOTES-1.0.2.md).

**Eingefrorener nächster Prüfstand: CAN 1.0.2 · Build 35** (22.10.2026). Setup, Portable, Quellen und Prüfsummen liegen lokal technisch geprüft bereit: 1411 Pakettests, 40 Diagnosetests, 893 App-Tests gegen finale Paketbibliotheken, elf Motor-Paketprüfungen und drei native künstliche Übergangsprüfungen bestanden. Begrenzte MP4Box-Überlappungszuordnung, unverändert otr-can 0.1.6. Neue persönliche Setup-/VM-/Portable-Abnahme steht aus. **Veröffentlichung frühestens Sonntag, 25.10.2026, nach ausdrücklicher Freigabe; kein automatischer Termin.** [Paketbericht](docs/RELEASE-1.0.2-BUILD35-RESULT.md), [Freeze](docs/FREEZE-1.0.2-20261022.md), [Release Notes](docs/RELEASE-NOTES-1.0.2.md).

Persönliche Setupprüfung **CAN 1.0.1 · Build 34** auf dem Arbeitsrechner am **20.10.2026** erfolgreich: **Vera mit MP4Box** und **Musikladen mit otr-can 0.1.6**, jeweils Bild, Ton, Schnitt und 3-Sekunden-Vorschau bestätigt; Musikladen ohne NDR-Blitzer. Auch **VM-Setup mit MP4Box und otr-can** (Grey’s Anatomy und Teufelsbrigade-AVI, jeweils Bild, Ton, Schnitte und Vorschau) sowie **Deinstallation in der VM** persönlich erfolgreich bestätigt. Auch **Portable in der VM** mit umgestelltem otr-can-Pfad und beiden Motoren erfolgreich bestätigt. **Die vorgesehenen persönlichen Pakettests sind abgeschlossen.** Commit und Push erledigt; am **20.10.2026** veröffentlicht. [Abnahmestand und Paketbericht](docs/RELEASE-1.0.1-BUILD34-RESULT.md).

Lokale Paketfassung **CAN 1.0.1 · Build 34 mit otr-can 0.1.6** (19.10.2026), zur persönlichen Setup-/Portable-/Windows-11-VM-Abnahme. Enthält die geprüfte Musikladen-AAC-Korrektur und die bestehende MP4Box-Übergangsprüfung. Musikladen und Navy CIS mit 0.1.6 persönlich positiv geprüft; MP4Box-Anfangsblitzer bei Musikladen und Radical bleiben offen. Neue Pakete getrennt von Build 33; keine Veröffentlichung. [Release Notes](docs/RELEASE-NOTES-1.0.1.md), [Motorprüfung und persönliche Filmabnahmen](docs/OTR-CAN-0.1.6-INTEGRATION-20261019.md).

Lokaler Musikladen-Test **CAN 1.0.1 · Build 33 mit separat eingebundenem otr-can 0.1.6** (19.10.2026): Testfilm und eigener CAN-Schnitt persönlich bestätigt – kein NDR-Blitzer, Bild und Ton einwandfrei. MP4Box zeigt bei derselben Aufnahme weiterhin einen NDR-Blitzer am Anfang. Weiterer Realfilm **Navy CIS vom 18.10.2026** mit otr-can 0.1.6 persönlich positiv geprüft: Bild, Ton und Übergänge; AAC-/AC3-Spuren im Protokoll erfolgreich verifiziert. Bestehende Setup-/Portable-Pakete enthalten unverändert 0.1.5. [Integration, Nachweise und persönlicher Vergleich](docs/OTR-CAN-0.1.6-INTEGRATION-20261019.md).

Lokale Paketvorbereitung **1.0.1 · Build 33** (19.10.2026): MP4Box verwendet im Abschlussfenster und Kontrollplayer die geprüften tatsächlichen Ausgabeübergänge mit drei Sekunden Vorlauf. Bei nicht bestätigbarer Analyse bleibt der fertige Film erhalten; berechnete Prüfstellen werden ausdrücklich gekennzeichnet. **otr-can 0.1.5 bleibt unverändert**, einschließlich des bekannten Musikladen-Audiobefunds. Setup und Portable sind zur persönlichen Paketabnahme vorgesehen; noch keine Veröffentlichung. [Änderungen und Grenzen](docs/RELEASE-NOTES-1.0.1.md), [Integrationsprüfung](docs/UEBERGANGSPRUEFUNG-INTEGRATION-20261018.md).

**Cut Assistant Next 1.0.0 · Build 32 – die erste stabile Version** ist seit **18.10.2026** als **Latest** veröffentlicht. [Setup](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.0/CutAssistantNext-1.0.0-Build32-win-x64-Setup.exe), [Portable-ZIP](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.0/CutAssistantNext-1.0.0-Build32-win-x64.zip) und [alle Downloads einschließlich Quellen, Lizenzen und SHA-256](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.0). **1347/1347 Pakettests**, Release **0 Warnungen/0 Fehler**; Produktiv-Setup sowie Setup, Deinstallation und Portable in der Windows-11-VM persönlich geprüft. Alle sieben Uploads vor/nach Freigabe verifiziert. [Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-1.0.0.md), [Benutzeranleitung](docs/NUTZERANLEITUNG.md), [Paket- und persönliche Prüfungen](docs/RELEASE-1.0.0-BUILD32-RESULT.md), [Grenzen und Release Notes](docs/RELEASE-NOTES-1.0.0.md). Die folgenden Einträge dokumentieren frühere Entwicklungs-/Prüffassungen und erhaltene RC-Releases.

Power-User-Feinschliff vom **16.10.2026**: rechte Werkzeuge gemäß abgestimmter Skizze neu gruppiert – Schnittpunkte, Tabelle mit rechtsbündigem Löschen, **Film schneiden**, **Cutlists**, breite **Namensmaske**. Gleiche Buttons und Funktionen, Standardanordnung beim Rückwechsel erhalten. Kommandozeilenübergabe mit realer HD-Aufnahme, Drag & Drop und Power-User-Layout persönlich bestätigt; Frame-Lupe und Frame-Liste geprüft, Schnitte mit MP4Box und otr-can erfolgreich gemeldet. Der bekannte [MP4Box-Werbeblitzer bei Radical](docs/RADICAL-MP4BOX-WERBEBLITZER-20261016.md) bleibt dokumentiert; weitere Release-Prüfungen stehen aus. Release **0 Warnungen/0 Fehler**, **1347/1347 Tests** und native Dialog-/Layoutprüfung erfolgreich. [Aktuelle Prüffassung, Vorschau und Grenzen](docs/POWER-USER-FEINSCHLIFF-20261016.md). Kein Versions-/Buildnummerwechsel und keine Veröffentlichung.

Lokale Prüffassung zur Vorbereitung von **CAN 1.0.0** (15.10.2026): optionaler Videopfad beim Start, Drag & Drop direkt in den Player und **Einstellungen → Darstellung → Standard-/Power-User-Ansicht**. Ein Player und ein Schnittzustand bleiben beim sofortigen Wechsel erhalten; vorhandene Schnittarbeit wird vor dem Dateiwechsel durch eine Rückfrage geschützt. **1343/1343 Tests**, Release-Rebuild **0 Warnungen/0 Fehler**, native Layout-/Navigations-/AVI-Prüfungen und beide Schnittmotoren synthetisch erfolgreich. [Umsetzung, separater Startpfad und offene persönliche Abnahme](docs/ZWEI-ANSICHTEN-START-DROP-20261015.md). Die Versionsanzeige bleibt bewusst **0.3.0 · Build 31 · RC3**; diese lokale Weiterentwicklung ist kein neues veröffentlichtes Build-31-Paket. Versionserhöhung, endgültiger Freeze, Commit, Push, Tag, Paketierung und stabiles Release bleiben gesondert freizugeben.

Vorherige öffentliche Vorabversion vom 15.10.2026: **0.3.0 · Build 31 · RC3** mit unverändertem Motor **otr-can 0.1.5**. [Setup, Portable-ZIP, Quellen und Prüfsummen](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build31-rc3) sind verfügbar. Alle sieben Uploads vor/nach Veröffentlichung anhand Größe und SHA-256-Digest geprüft; [Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-BUILD31-RC3.md). Persönlich bestätigt: Setup in der VM, Deinstallation, Portable-ZIP in der VM und Videoschnitt mit MP4Box/otr-can.
Eingefrorener Paketsatz vom 15.10.2026: **0.3.0 · Build 31 · RC3** mit Hauptplayer-Halbierungssuche, gezielter AVI-Bedienungssperre und otr-can-Zeitstrukturvorprüfung. **1308/1308 Entwicklungstests**, persönlicher AVI-/MP4-Umschalttest und gezielte Motorablehnungen bestätigt. Setup, Portable, Quellen und Prüfsummen sind aus Quellcommit **de8ec0d598b095d85e4559e7fd83012534959637** erstellt und geprüft; **1308/1308 Pakettests**, 249 Portable-Dateien und 539 Quellen-ZIP-Dateien erfolgreich verifiziert. Quellstand und Tag **v0.3.0-build31-rc3** auf GitHub gesichert; Pakete lokal unter `artifacts/release-build31-RC3` und unverändert beim öffentlichen Build-31-Release; [Umfang und Grenzen](docs/FREEZE-BUILD31-RC3-20261015.md), [Paketprüfbericht](docs/RELEASE-BUILD31-RC3-RESULT.md). Unveränderter Motor **otr-can 0.1.5**. Nachträgliche persönliche Paketabnahme vom 15.10.2026: **Setup in der VM, Deinstallation und Portable-ZIP in der VM erfolgreich; Videos mit MP4Box und otr-can geschnitten.**
Vorherige öffentliche Vorabversion vom 14.10.2026: **0.3.0 · Build 30 · RC3** mit unverändertem optionalem Motor **otr-can 0.1.5**. [Setup, Portable-ZIP, Quellen und Prüfsummen](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build30-rc3) sind verfügbar. Release-Build ohne Warnungen/Fehler, **1215/1215 CAN-Tests** bestanden; alle sieben Uploads vor und nach Freigabe anhand ihrer Größen und GitHub-SHA-256-Digests geprüft. [Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-BUILD30-RC3.md), [Paketprüfbericht](docs/RELEASE-BUILD30-RC3-RESULT.md).

Neu in Build 30: sichtbarer **Frame-Lupe**-Button mit **Shift+F**, Hinweise auf **Strg+Klick für ±20 Bilder**, reine Filmnamen bei leeren Staffel-/Folge-/Folgentitelfeldern und ein **Untersuchungsmodus der Frame-Lupe ohne Schnittkante**. Im Hauptfenster zeigen **blaue Bereiche die Keyframes**, **◀ K / K ▶** springen zum Nachbarn und **(K)** steht direkt hinter der aktuellen Framenummer. AVI ist von dieser Hauptfenster-Keyframe-Anzeige und -Navigation ausgenommen; der gelbe AVI-Infoblock erklärt dies, die AVI-Frame-Lupe bleibt nutzbar. Die bisherigen technischen Prüfungen umfassen **1215/1215 CAN-Tests**. Der Nutzer hat den aktuellen Stand zum Einfrieren und Veröffentlichen freigegeben. Eine neue Setup-/VM-Praxisabnahme wird daraus nicht abgeleitet.

Lokale Prüffassung vom 15.10.2026: **Halbierungssuche direkt im Hauptplayer**, kompakt unter der bisherigen Navigation. **1244/1244 Tests**, Release-Rebuild ohne Warnungen/Fehler und 30 native Integrationsfälle. Original-Zeitstempel erforderlich. In der aktuellen lokalen Prüffassung ist die gesamte Halbierungssuche für erkannte AVI-Container ausgeblendet und gesperrt; der gelbe Hinweis erklärt dies. Neu geprüft: **1288/1288 Tests**, vier native AVI/MP4-Wechselfälle, Build ohne Warnungen/Fehler. [Prüfbericht und separater Startpfad](docs/HALBIERUNGSSUCHE-HAUPTFENSTER-20261015.md). Persönliche Funktionsabnahme am 15.10.2026 positiv bestätigt; größere Bilder werden als langsam beschrieben. Keine neue Buildnummer oder Veröffentlichung; öffentliche Build-30-Pakete unverändert.

Lokale OTR-CAN-Vorprüfung vom 15.10.2026: Nach FFMS2-Indexierung verhindern doppelte, rückläufige oder ungültige Bildzeiten sowie fehlender Nullstart oder ein nicht passendes 25/50-fps-Zeitraster den Schnittaufruf. **No Turning Back und der neue Navy-CIS-Fall werden gesperrt, Teufelsbrigade bleibt freigegeben.** **1308/1308 Tests**, Build ohne Warnungen/Fehler und reale Indexiervergleiche bestanden. [Nachweis, Grenzen und neue Prüffassung](docs/OTR-CAN-ZEITSTEMPEL-VORPRUEFUNG-20261015.md).
Lokale Ergänzung vom 15.10.2026: **Frame-Lupe für erkannte AVI-Container gesperrt**, einschließlich Shift+F. Der gelbe Hinweis lautet: „Die Frame-Lupe steht für AVI-Container derzeit nicht zur Verfügung.“ MP4Box und otr-can bleiben unverändert. Der Nutzer bestätigt den otr-can-Schnitt der ursprünglichen Teufelsbrigade-AVI nach Bild-, Ton- und Übergangsprüfung als einwandfrei. [Umsetzung, Prüfungen und separater Startpfad](docs/AVI-FRAME-LUPE-SPERRE-20261015.md).
AVI-Diagnose vom 15.10.2026: **No Turning Back** hat widersprüchliche Bildzeiten, auch nach dem technisch erfolgreichen MP4Box-Schnitt. H.264-Timingangaben als Auslöser eingegrenzt; reine Ursachenanalyse ohne Produktänderung. [Diagnosebericht](docs/AVI-ZEITSTEMPEL-NO-TURNING-BACK-20261015.md).

Veröffentlichter Entwicklungsstand vom 13.10.2026: **0.3.0 · Build 29 · RC3** ergänzt die neue Schaltflächenreihenfolge, frei editierbare Filmnamen, direktes Speichern von Film/Cutlist, getrennte Serienbausteine, gemeinsame Bereichsauswahl, die Rechtsklick-Schnittkontrolle **−3/+3 Sekunden** im Original und einen unabhängigen Kontrollplayer für fertige Übergänge. **1076/1076 CAN-Tests** und WPF-Prüfungen mit echten libmpv-Instanzen bestanden. Die [Gesamtabnahme](docs/GESAMTABNAHME-BEDIENUNG-20261012.md) enthält acht erfolgreiche reale synthetische Schnittaufträge und einen offenen MP4Box-Befund bei einer Datei mit zwei gleichartigen AAC-Spuren; die Bedienrunde aus Build 24 ist inzwischen vom Nutzer positiv geprüft, einschließlich beider Motoren und Original-AVI. Build 25 erweitert den Kontrollplayer um Anfang und Ende; diese Funktion ist ebenfalls persönlich positiv geprüft. Build 26 ermöglicht in der Frame-Lupe bei erkanntem AVI ohne Original-PTS die Vorschau und Kantenübernahme über einen ausdrücklich gekennzeichneten, bildgeprüften rekonstruierten Zeitstempel. [AVI-Lupenprüfung und Grenzen](docs/AVI-FRAME-LUPE-20261013.md). Der Nutzer bestätigt die AVI-Lupe und den Teufelsbrigade-Schnitt mit beiden Verfahren als erfolgreich, jeweils ohne Blitzer. Build 27 setzt „Suchsprung nur bei Richtungswechsel halbieren“ um und ergänzt **Prüfen** unter FFmpeg-Werkzeuge: Versionen **größer als 5.1** erforderlich, **ab 9.0.2** empfohlen. [Prüfbericht](docs/LUPEN-SUCHRICHTUNG-FFMPEG-20261013.md). FFmpeg-Versioncheck, Richtungswechsel-Halbsprung und alternative Folgennomenklatur sind persönlich bestätigt. Build 28 erklärt die von Hand ergänzten Staffel-/Folgenwerte direkt an der Maskenvorschau. Build 29 verwendet nach einer gespeicherten Maskenänderung beim bewussten Neuerzeugen im Cutlist-Dialog sofort die aktuelle Maske, auch bei bereits geladener Datei und ohne Neustart. [Maskenwechsel-Prüfung](docs/NAMENSMASKE-OHNE-NEUSTART-20261013.md). [Aktueller Prüfstand](docs/KONTROLLPLAYER-ANFANG-ENDE-20261012.md). Die öffentliche Build-29-Ausgabe enthält diese Bedienungsänderungen. [Film-Ausgabe](docs/AUSGABENAME-DIREKTSPEICHERN-20261012.md), [Cutlist-Speichern](docs/CUTLIST-AUTOSPEICHERN-20261012.md), [Namensmasken](docs/NAMENSMASKEN-EINZELBAUSTEINE-20261012.md), [Bereichsauswahl](docs/SCHNITTBEREICH-AUSWAHL-20261012.md), [Schnittkontrolle](docs/SCHNITTKONTROLLE-3SEK-20261012.md), [Kontrollplayer](docs/SCHNITTUEBERGAENGE-KONTROLLPLAYER-20261012.md).

Eingefrorener Paketsatz vom 13.10.2026: **0.3.0 · Build 29 · RC3** aus Quellcommit **ce4bc15b118d8080a78d7db5396db3ccf830ca89**, mit unverändertem **otr-can 0.1.5**. Setup, Portable-ZIP, passende CAN-/Motor-/libmpv-Quellen und SHA-256-Prüfsummen liegen lokal unter `artifacts/release-build29-RC3` und stehen beim [Build-29-Release](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build29-rc3) bereit. **1076/1076 Pakettests**, **249 Portable-Dateien** und **509 Quellen-ZIP-Dateien** geprüft. [Paketprüfbericht](docs/RELEASE-BUILD29-RC3-RESULT.md). Produktivinstallation sowie VM-Setup mit beiden Motoren und Deinstallation und Portable-Start/Pfade/AVI-Schnitt mit beiden Motoren sind persönlich bestätigt; die öffentliche Build-16-Ausgabe bleibt unverändert.

Cut Assistant Next (CAN) ist ein Windows-Programm zum Abspielen von Videos, Bearbeiten klassischer Cutlists und Schneiden mit MP4Box oder dem optionalen otr-can. Die Oberfläche verwendet WPF und mpv/libmpv, ohne DirectShow oder installierte Windows-Codec-Pakete vorauszusetzen.

Für CAN 1.0.0 ist der persönlich geprüfte Funktionsumfang eingefroren; die stabile Veröffentlichung folgt nach abschließender Paketabnahme. Geeignete H.264-AVI-Dateien können für beide Motoren experimentell ohne Neukodierung der Streams nach MP4 umgepackt werden. otr-can 0.1.5 dekodiert MP3, schneidet den Ton als PCM und kodiert ihn danach **einmal nach AAC-LC**; eine zusätzliche AAC-Vorkodierung entfällt. Die Tonkodierung bleibt verlustbehaftet. Bestehende AAC-/AC3-Abläufe bleiben erhalten. MP4Box bleibt Standard. Direkt unter der Schnittliste wählst du den Motor für diesen Auftrag.

Für Build 16 sind frische Setup-Installation in der VM, beide Motoren, Deinstallation und Portable-ZIP in der VM mit beiden Motoren vom Nutzer positiv bestätigt. Ein Setup-AVI-Ergebnis wurde außerdem an Anfang, Ende, allen Schnittübergängen sowie auf Bild und Ton persönlich abgenommen. Der bekannte technische Video-PTS-Befund bleibt offen; AVI bleibt experimentell. [Paketprüfbericht](docs/RELEASE-BUILD16-RC3-RESULT.md), [AVI-/MP3-Prüfbericht](docs/AVI-OTR-CAN-MP3-20261012.md) und [gesicherte Quellen](docs/QUELLSTAND-BUILD16-RC3-20261012.md) beschreiben Prüfstand und Grenzen.

Die vorherige [Build-16-RC3-Ausgabe](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.1-build16-rc3), die [Build-13-RC3-Ausgabe](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.1-build13-rc3) und frühere Releases bleiben verfügbar. Die ältere AAC-/AC3-Erprobung steht im [Radical-Korrekturbericht](docs/OTR-CAN-RADICAL-KORREKTUR-20261011.md). Weitere reale Formate und getrennte Tonspurkontrollen folgen vor Version 1.0.

</details>

## Lizenz

Der vom CAN-Projekt selbst entwickelte Quellcode steht unter **GPL-3.0-or-later**. Der vollständige Lizenztext steht in [LICENSE](LICENSE). Eingebundene Bibliotheken, NuGet-Pakete und die .NET-Laufzeit behalten ihre eigenen Lizenzen; die [libmpv-Hinweise](docs/libmpv/v0.41.0/THIRD-PARTY-NOTICES.md) und die [.NET-/NuGet-Bestandsaufnahme](docs/DOTNET-LICENSING.md) ordnen sie zu. Die öffentliche RC3-Ausgabe ist als Vorabversion gekennzeichnet; die zugehörigen Quellen und Prüfsummen stehen beim Release.

## Danksagung

CAN baut auf Ideen und Arbeit vieler Menschen und Projekte auf. Die [Danksagung](docs/DANKSAGUNG.md) nennt die Beteiligten und würdigt auch die KI-unterstützte Entwicklung.

## Einstieg

- [Aktueller Stand](docs/AKTUELLER-STAND.md): Build 38, bestätigte Abläufe, offene Paketprüfungen und nächste Aufgabe.
- [Klebecenter-Referenzfall](docs/JOHN-WICK-4-KLEBECENTER-REFERENZ-20261026.md): John Wick 4, AAC-Padding und geprüfte Bild-/Tonzeitachse.
- [Nutzeranleitung](docs/NUTZERANLEITUNG.md): Einrichtung, Bedienung, Cutlists, Schneiden und Hilfe bei Fehlern.
- [Projektstatus](docs/STATUS.md): umgesetzter Umfang, bestätigte Tests und offene Aufgaben.
- [Entwicklungstagebuch](docs/TAGEBUCH.md): aktuelle Änderungen, Prüfergebnisse und offene Schritte.
- [Setup-Merkliste](docs/SETUP-MERKLISTE.md): geplantes Gesamtpaket und vorgemerkte V2-Funktionen.
- [Testplan](docs/TESTPLAN.md): reproduzierbare Prüfungen und Testbasis.
- [Build-16-RC3-Prüfbericht](docs/RELEASE-BUILD16-RC3-RESULT.md): gesicherter Quellcommit, technische Prüfungen und persönliche Setup-/VM-/Portable-Abnahme.
- [RC2-Prüfbericht](docs/RELEASE-BUILD7-RC2-RESULT.md): Quellstand, Paketprüfsummen, 496 automatisierte Tests, Portable-Starttest und verbleibende Praxistests.
- [Aktueller RC3-Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-BUILD30-RC3.md): Build 30, fester Quelltag und sieben geprüfte öffentliche Downloads.
- [Build-30-Paketprüfung](docs/RELEASE-BUILD30-RC3-RESULT.md): eingefrorener Stand, Build, Tests und Portable-Prüfung.
- [Bisheriger RC3-Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-BUILD29-RC3.md): Build 29, fester Quelltag und sieben geprüfte öffentliche Downloads.
- [Früherer RC3-Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-BUILD16-RC3.md): Quelltag, sieben öffentliche Downloads und geprüfte GitHub-Digests.
- [Früherer RC2-Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-BUILD7-RC2.md): Build 7 und dessen persönliche Paketprüfung.
- [Architektur](docs/ARCHITECTURE.md) und [Entscheidungen](docs/DECISIONS.md): technische Hintergründe und Entwicklungsgeschichte.

## Was CAN derzeit kann

- Videos laden, mit ffprobe analysieren und mit mpv wiedergeben.
- Play/Pause, Seeking, Einzelbildschritte und Lautstärkeregelung.
- Zu entfernende Bereiche markieren, auswählen, korrigieren und löschen.
- Frame-Lupe über den sichtbaren Button oder Umschalt+F öffnen: ausgewählte Schnittkante bearbeiten oder ohne Kante die aktuelle Playerposition untersuchen. Nur im Kantenmodus ist eine Übernahme möglich.
- Bei MP4 Keyframes im bestehenden Schnittstreifen anzeigen und zum vorherigen/nächsten Keyframe springen; AVI ist davon ausdrücklich ausgenommen.
- Zusätzlich zur Navigation mit ±1/±10 Frames mit halbierten Suchsprüngen navigieren: standardmäßig 2000, 1000, 500 … bis 1 Frame, mit einstellbarem Startwert und Neustart.
- Cutlists lokal laden, erzeugen und speichern.
- Passende Server-Cutlists automatisch suchen und die Suche erneut per Button öffnen.
- Lokal gespeicherte Cutlists nach Bestätigung auf den konfigurierten Server hochladen.
- Fremde Cutlists als Vorlage verwenden, deren Namensvorschlag bewusst beibehalten oder neu erzeugen und die Herkunft bei abweichendem Autor im Kommentar dokumentieren.
- Schnittzeiten einheitlich anzeigen und die Namensvorschau markieren und kopieren.
- Ausgabenamen über Namensmasken und Cutlist-Namensvorschläge vorbereiten.
- MP4-Inhalte direkt mit MP4Box schneiden, auch bei abweichender Dateiendung.
- Andere Container nach Bestätigung experimentell mit FFmpeg verlustfrei nach MP4 umpacken und anschließend mit MP4Box schneiden.
- Optional mit OTR-CAN aus geeigneten MP4-Dateien oder experimentell vorbereiteten H.264-AVI-Dateien schneiden, FFMS2 einmal indexieren und unterstützte Tonspuren übernehmen; CPU-Encoding an Video-Schnittgrenzen und vollständige Audio-Neukodierung, bei MP3 einmal nach AAC-LC.
- **Lokale 1.0.2:** Bei passender otr-can-Tonstörung nach bewusster Bestätigung mit streng geprüfter AAC-Spur erneut schneiden.
- **Lokale 1.0.2:** Zusätzliche MP4-Kopie mit einer bewusst gewählten AAC-/AC-3-Spur für weitere bereits geschnittene Filmteile speichern; ohne Neukodierung und ohne Anwenden der Schnittliste.
- Fortschritt und Protokoll anzeigen, kopieren und den Vorgang abbrechen.
- Hauptfenster und die vier Schnittdialoge (Schneiden, Fortschritt/Protokoll, Abschluss, Cutlist erzeugen) merken Größe und Maximierung; die Player-Lautstärke wird ebenfalls gespeichert. Inhalte bleiben bei kleinen Fenstern scrollbar.

Die reine Cutlist-Erstellung benötigt keine Video-Umwandlung. Erst beim Schneiden wird eine gegebenenfalls notwendige MP4-Arbeitsdatei erzeugt. Serversuche und Cutlist-Zuordnung beziehen sich dabei weiterhin auf die Originaldatei.

## Schnittkanten vor dem Schneiden kontrollieren

Ab dem lokalen Build 22 kannst du im Originalplayer die Umgebung einer Schnittkante per Rechtsklick prüfen:

![Schnittkontrolle: vor dem Schnittanfang und nach dem Schnittende jeweils drei Sekunden im Original prüfen](docs/images/schnittkontrolle-3sek.svg)

Rechtsklick auf Anfang: 3 Sekunden zurück. Rechtsklick auf Ende: 3 Sekunden vorwärts. So lassen sich die Filmszenen unmittelbar vor und nach einem Schnittbereich kontrollieren.

Dateigrenzen und sehr kurze angrenzende Filmabschnitte begrenzen den Sprung. Auswahl und Schnittwerte bleiben erhalten; die Wiedergabe startet nicht automatisch. Anschließend kannst du die vorhandene Frame-Lupe zur Korrektur verwenden.

## Schnittübergänge nach dem Schneiden prüfen

Ab dem lokalen Build 25 öffnet **Prüfen** neben Anfang, jedem Übergang und Ende im Abschlussfenster die fertige Datei in einem eigenen CAN-Kontrollplayer, auch bei nur einem Behalteabschnitt. Am Anfang startet die Wiedergabe bei 0:00, vor einem Übergang drei Sekunden davor und am Ende drei Sekunden vor der tatsächlich geladenen Filmdauer. Sehr kurze Filme beginnen bei 0:00. **Vorherige Prüfstelle** und **Nächste Prüfstelle** wechseln zwischen allen Kontrollstellen. Play/Pause, Positionsregler und eigene Lautstärke stehen bereit. Das Fenster wird wiederverwendet; Originalplayer und Schnittliste bleiben erhalten. Bitte auch Anfang, Ende, Bild, Ton und Tonsynchronität prüfen. [Bedienung und Prüfgrenzen](docs/SCHNITTUEBERGAENGE-KONTROLLPLAYER-20261012.md).


**Lokale Weiterentwicklung nach 1.0.0 (18.10.2026):** MP4Box ermittelt die Prüfstellen jetzt zusätzlich aus seinen Segmenten und der fertigen Videozeitachse. Geprüfte Ausgabezeiten und lediglich berechnete Prüfstellen werden unterschieden. Scheitert die Zusatzprüfung oder wird sie abgebrochen, bleibt der fertig geschnittene Film erhalten. Fehlende Bilder werden durch die Korrektur der Prüfpositionen nicht wiederhergestellt. otr-can behält seine bisherigen Planzeiten. Noch keine neue Veröffentlichung; [Integration und Prüfungen](docs/UEBERGANGSPRUEFUNG-INTEGRATION-20261018.md).

## Grenzen

MP4 ist ein Container, keine Bezeichnung für einen bestimmten Videocodec. Ob sich ein anderes Format verlustfrei vorbereiten lässt, hängt von seinen Streams ab. Die MP4Box-Vorbereitung verwendet Stream-Copy. Der optionale OTR-CAN-Motor kopiert geeignete IDR-Abschnitte, kodiert Video-Schnittgrenzen und jede vollständige Tonspur auf der CPU neu. Ungeeignete Dateien oder deutliche Abweichungen werden mit einer Fehlermeldung gestoppt.

Die allgemeine Container-Vorbereitung prüft Medienstruktur und Zeitverhältnisse. Beim AAC-Notfallweg und bei der manuellen Tonspurkopie werden zusätzlich sämtliche übernommenen Bild-/Tonpakete und Wiedergabezeiten exakt verglichen. Diese technischen Prüfungen ersetzen nicht die persönliche Kontrolle der Schnittstellen und Tonsynchronität. OTR-CANs Video-Smart-Rendering und Audio-Neukodierung sind synthetisch und mit Pines geprüft; Stapelverarbeitung und integrierte mehrteilige Aufnahmen sind noch nicht umgesetzt.

Insbesondere bei historischen AVI-Dateien können bereits im Original Probleme mit Zeitstempeln, Bildaktualisierung oder Ton-Synchronität auftreten. Eine irreführende Dateiendung wie `.avi.mp4` ändert nichts am tatsächlich erkannten Container. CAN zeigt bei tatsächlich erkanntem AVI bereits nach dem Laden einen gelben Hinweis auf mögliche Probleme im Original; eine abweichende Dateiendung wird zusätzlich erklärt. Weitere Praxistests erfolgen mit geeigneten neuen Aufnahmen.

Die Gesamtabnahme in Build 24 hat bei einer erzeugten Datei mit zwei gleichartigen AAC-Stereo-Spuren einen MP4Box-Zusammenfügungsfehler reproduziert, auch direkt mit GPAC 26.07. CAN veröffentlicht dabei keine fertige Ausgabe. Der AAC/AC3-Fall funktioniert; eine allgemeine Ursache oder Aussage über alle Mehrspurdateien ist noch nicht belegt. [Nachweis und Prüfgrenzen](docs/GESAMTABNAHME-BEDIENUNG-20261012.md).

**Nachtrag zur VM-Prüfung Build 46 (31.10.2026):** Auch AUDIO-ENTHUSIAST mit John Wick Kapitel 4 und AAC/AAC persönlich für Bild, Ton und Drei-Sekunden-Vorschau bestanden; Verarbeitung 00:05:54.41. Installation und beide Klebeverfahren positiv, gesamte VM-Prüfrunde weiterhin laufend. [Laufender VM-Bericht](docs/VM-SETUP-BUILD46-20261031.md).

## Für Anwender

Vorgesehene Umgebung ist Windows 11 x64. Der freigegebene Paketsatz CAN 1.0.2 Build 47 bietet Setup und Portable-ZIP mit otr-can 0.1.6. Die .NET 10 Desktop Runtime muss separat installiert werden; die vier geprüften libmpv-DLLs sind enthalten. ffprobe, FFmpeg und MP4Box werden separat installiert und über die Einstellungen konfiguriert.

Die [Nutzeranleitung](docs/NUTZERANLEITUNG.md) erklärt die Einrichtung ohne Entwicklungswerkzeuge.

## Optional: CAN über die Windows-Kommandozeile starten

CAN unterstützt genau eine Videodatei als Startparameter. Die Anwendung startet, lädt die Datei automatisch und führt die normale Medienanalyse aus.

Ist CAN über den Windows-PATH erreichbar, genügt beispielsweise:

```powershell
CutAssistantNext.App.exe "D:\Filme\MeinFilm.mp4"
```

**Der PATH-Eintrag ist optional** und für die normale Nutzung von CAN nicht erforderlich. Ohne PATH-Eintrag kannst du jederzeit den vollständigen Programmpfad verwenden; in PowerShell steht vor einem in Anführungszeichen gesetzten Programmpfad der Aufrufoperator `&`:

```powershell
& "C:\Pfad\zu\CAN\CutAssistantNext.App.exe" "D:\Filme\MeinFilm.mp4"
```

Befindest du dich bereits im CAN-Ordner, verwende in PowerShell `.\` vor dem Programmnamen:

```powershell
.\CutAssistantNext.App.exe "D:\Filme\MeinFilm.mp4"
```

Dateipfade mit Leerzeichen in Anführungszeichen setzen. Relative Videopfade beziehen sich auf den Arbeitsordner beim Start, nicht auf das CAN-Installationsverzeichnis. Ohne Dateiparameter startet CAN wie bisher. Fehlende oder unzugängliche Dateien sowie mehrere Argumente werden verständlich gemeldet. Dateizuordnungen werden nicht automatisch geändert.

### Schritt 1 – CAN-Verzeichnis ermitteln

Öffne im Windows-Explorer den Installations- oder entpackten Portable-Ordner, in dem sich `CutAssistantNext.App.exe` befindet, und kopiere den vollständigen Ordnerpfad aus der Adresszeile. Nur das Verzeichnis eintragen, nicht den vollständigen Pfad zur EXE-Datei.

### Schritt 2 – Benutzer-PATH unter Windows 11 ergänzen

1. Suche im Startmenü nach **Umgebungsvariablen**.
2. Öffne **Umgebungsvariablen für dieses Konto bearbeiten**.
3. Wähle unter **Benutzervariablen** den Eintrag **Path**.
4. Klicke auf **Bearbeiten** und anschließend auf **Neu**.
5. Füge das zuvor ermittelte CAN-Verzeichnis ein.
6. Bestätige alle Dialoge mit **OK**.

Bestehende PATH-Einträge nicht löschen oder überschreiben. Für den eigenen Benutzer-PATH sind normalerweise keine Administratorrechte erforderlich.

### Schritt 3 – Einrichtung überprüfen

Öffne ein neues PowerShell-Fenster. Bereits geöffnete Terminalfenster gegebenenfalls vollständig schließen und neu starten, damit sie den geänderten PATH übernehmen.

```powershell
Get-Command CutAssistantNext.App.exe
```

Die Ausgabe sollte den vollständigen Pfad zur gewünschten CAN-EXE anzeigen. Alternativ:

```powershell
where.exe CutAssistantNext.App.exe
```

Zeigt `where.exe` mehrere Fundstellen, prüfe deren Reihenfolge und ob die gewünschte CAN-Version zuerst gefunden wird. `Get-Command` zeigt, welchen Aufruf PowerShell tatsächlich verwendet.

### Schritt 4 – Videodatei übergeben

Ersetze den Beispielpfad durch eine vorhandene Aufnahme:

```powershell
CutAssistantNext.App.exe "D:\Filme\MeinFilm.mp4"
```

Erwartung: CAN startet, lädt die Aufnahme, führt die Medienanalyse aus und zeigt das Video im Player. Anschließend kannst du die Schnittbearbeitung wie gewohnt verwenden.

Setup und Portable-Version verändern den Windows-PATH nicht automatisch. Es gibt keine zusätzliche Installer-Option zur PATH-Verwaltung; die optionale Einrichtung erfolgt durch dich. Wenn du den Portable-Ordner später verschiebst, passe einen selbst angelegten PATH-Eintrag entsprechend an.

## Aus dem Quellcode bauen

Voraussetzungen: Windows 11 x64, .NET-10-SDK und die vier DLLs des [geprüften CAN-libmpv-0.41.0-Kandidaten](docs/libmpv/v0.41.0/README.md) in einem lokalen Ordner. Das Setup-Skript prüft die SHA-256-Werte und kopiert die DLLs in den ignorierten Laufzeitordner. Ein .NET-Restore benötigt die passenden Pakete im lokalen Cache oder Netzwerkzugriff.

Im Repository-Verzeichnis:

```powershell
.\tools\setup-libmpv.ps1 -SourceDirectory 'C:\Pfad\zum\Vier-DLL-Ordner'
dotnet restore .\CutAssistantNext.sln
.\tools\build.ps1 -Configuration Release -NoRestore
dotnet test .\CutAssistantNext.sln --configuration Release --no-build
```

Die Anwendung liegt danach unter `src\CutAssistantNext.App\bin\Release\net10.0-windows\CutAssistantNext.App.exe`. Der Build kopiert die vier vorbereiteten DLLs und die Drittanbieterhinweise in das Ausgabeverzeichnis. Für Analyse, experimentelles Umpacken und Schnitt sind zusätzlich die konfigurierten externen Werkzeuge erforderlich.

## Version und Buildnummer

Vorherige öffentliche Vorabversion vom 15.10.2026: **0.3.0 · Build 31 · RC3** mit unverändertem Motor **otr-can 0.1.5**. [Setup, Portable-ZIP, Quellen und Prüfsummen](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build31-rc3) sind verfügbar. Alle sieben Uploads vor/nach Veröffentlichung anhand Größe und SHA-256-Digest geprüft; [Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-BUILD31-RC3.md). Persönlich bestätigt: Setup in der VM, Deinstallation, Portable-ZIP in der VM und Videoschnitt mit MP4Box/otr-can.
Eingefrorener Paketsatz vom 15.10.2026: **0.3.0 · Build 31 · RC3** mit Hauptplayer-Halbierungssuche, gezielter AVI-Bedienungssperre und otr-can-Zeitstrukturvorprüfung. **1308/1308 Entwicklungstests**, persönlicher AVI-/MP4-Umschalttest und gezielte Motorablehnungen bestätigt. Setup, Portable, Quellen und Prüfsummen sind aus Quellcommit **de8ec0d598b095d85e4559e7fd83012534959637** erstellt und geprüft; **1308/1308 Pakettests**, 249 Portable-Dateien und 539 Quellen-ZIP-Dateien erfolgreich verifiziert. Quellstand und Tag **v0.3.0-build31-rc3** auf GitHub gesichert; Pakete lokal unter `artifacts/release-build31-RC3` und unverändert beim öffentlichen Build-31-Release; [Umfang und Grenzen](docs/FREEZE-BUILD31-RC3-20261015.md), [Paketprüfbericht](docs/RELEASE-BUILD31-RC3-RESULT.md). Unveränderter Motor **otr-can 0.1.5**. Nachträgliche persönliche Paketabnahme vom 15.10.2026: **Setup in der VM, Deinstallation und Portable-ZIP in der VM erfolgreich; Videos mit MP4Box und otr-can geschnitten.**
Vorherige öffentliche Vorabversion vom 14.10.2026: **0.3.0 · Build 30 · RC3** mit unverändertem optionalem Motor **otr-can 0.1.5**. [Setup, Portable-ZIP, Quellen und Prüfsummen](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build30-rc3) sind verfügbar. Release-Build ohne Warnungen/Fehler, **1215/1215 CAN-Tests** bestanden; alle sieben Uploads vor und nach Freigabe anhand ihrer Größen und GitHub-SHA-256-Digests geprüft. [Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-BUILD30-RC3.md), [Paketprüfbericht](docs/RELEASE-BUILD30-RC3-RESULT.md).

Vorherige öffentliche Vorabversion vom **12.10.2026: 0.2.1 · Build 16 · RC3**, Release-Build ohne Warnungen/Fehler, **793/793 CAN-Tests und 44/44 Rust-Tests bestanden**. Setup, Portable, CAN-/Motorquellen, libmpv-Quellen-/Relink-Pakete und Prüfsummen stehen beim [Release](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.1-build16-rc3) bereit. Alle sieben Uploads stimmen mit den lokalen Größen und SHA-256-Werten überein. Frische VM-Setup-Installation, beide Motoren, Deinstallation und Portable mit beiden Motoren persönlich bestätigt; ausführliche Setup-Filmabnahme dokumentiert. Überinstallation und frisches Portable-Profil nicht gesondert bestätigt. Frühere Releases bleiben unverändert verfügbar.

Öffentliche Vorabversion vom 01.10.2026: **0.2.1 · Build 7 · RC2** mit korrigierter Upload-Identität und überarbeiteten Cutlist-Einstellungen. Der Release-Build ist ohne Warnungen oder Fehler abgeschlossen; **496/496 Tests bestanden**. Setup, Portable-ZIP, Quellen und Prüfsummen stehen beim Release bereit. Die frühere Ausgabe **0.2.0 Build 6 – RC2** bleibt unverändert verfügbar.

Die inhaltliche Version steht zentral in `Version.props`: Patch für Korrekturen, Minor für neue Funktionen, Major für einen größeren Versionssprung. Sie wird bewusst gepflegt.

Eine optionale Vorabkennung wird über die Build-Eigenschaft `CanCandidate` gesetzt und im Fenstertitel sowie in der Hauptüberschrift angezeigt. Der stabile Build 47 trägt keine RC-Kennung. Die numerische Version für Cutlists und HTTP-Uploads enthält diese Kennung nicht.

`tools/build.ps1` baut die gesamte Lösung neu und erhöht den lokalen Zähler in `.build/build-number.txt` genau einmal bei erfolgreichem Abschluss. Fehlgeschlagene Builds erhöhen ihn nicht. Titel, Hauptüberschrift und Dateieigenschaften verwenden dieselbe Version und Buildnummer. Einzelne Projekt-Builds und direkte IDE-/dotnet-Builds verwenden die zuletzt erfolgreiche Nummer (bei einem neuen Checkout zunächst 0); für eine neue nummerierte Ausgabe das Skript verwenden. Der Zähler ist lokal, wird nicht mit Git synchronisiert und identifiziert keine weltweit eindeutige Veröffentlichung. Gleichzeitige Skript-Builds im selben Checkout werden durch eine Dateisperre verhindert.

## Projektstruktur

| Bereich | Aufgabe |
|---|---|
| `src/CutAssistantNext.App` | WPF, Dialoge, ViewModels, Einstellungen und Ablaufsteuerung |
| `src/CutAssistantNext.Core` | Medienmodelle, Schnittplan, Namensbildung und Verträge |
| `src/CutAssistantNext.Media` | ffprobe, mpv und Prozessausführung für FFmpeg/MP4Box |
| `src/CutAssistantNext.Cutlists` | Cutlist-Import, Export, Metadaten und Bereichsumrechnung |
| `tests` | vier automatisierte Testsuiten |
| `samples` | kleine Referenzdaten |
| `tools` | Einrichtung von libmpv und versionierte Builds |
| `docs` | Anleitung, Status, Testplan und technische Dokumentation |

Historischer RC2-Release-Stand (01.10.2026): **0.2.1 Build 7 – RC2**, Release-Build ohne Warnungen oder Fehler und **496/496 automatisierte Tests bestanden**. Alle 148 Dateien des Portable-ZIPs wurden geprüft; Start, Versionsanzeige und reguläres Beenden waren erfolgreich. Die Installation in einer VM und ein dreiteiliger HQ-MP4-Schnitt von „Grey’s Anatomy – Durchs Feuer“ wurden bestätigt; Bild und Ton der fertigen Datei waren einwandfrei. Die Setup-Deinstallation wurde ebenfalls als erfolgreich bestätigt. Der separate Portable-ZIP-Schnitt mit drei Behaltebereichen wurde ebenfalls erfolgreich abgeschlossen; Bild und Ton der fertigen Datei wurden vom Benutzer als einwandfrei bestätigt. Details stehen im [damaligen Prüfbericht](docs/RELEASE-BUILD7-RC2-RESULT.md); die früheren Pakettests bleiben im [Build-6-Prüfbericht](docs/RELEASE-BUILD6-RC2-RESULT.md) dokumentiert.
