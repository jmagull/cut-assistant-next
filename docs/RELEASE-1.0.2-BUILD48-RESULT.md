# CAN 1.0.2 Build 48 – Paketprüfung

**01.11.2026: vollständiger Paketsatz technisch geprüft und als stabile [Latest-Version v1.0.2-build48](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2-build48) veröffentlicht.** App-Dateiversion **1.0.2.48**, otr-can **0.1.6**, libmpv **0.41.0**, .NET 10/WPF, Windows 11 x64.

## Herkunft und Umfang

Release-Commit und Tagziel **5afa5ff099d3b5e2f0586ce5899aafde6feabf58**. Exakter Git-Export ohne Arbeitsbaum-Overlay; **716 manifestierte Quelldateien / 717 Quellen-ZIP-Dateien** vollständig gegen SOURCE-FILES.sha256 und ZIP geprüft. Die vier vorbestehenden lokalen Produktions-Lockänderungen sind ausgeschlossen; die committed win-x64-Locks wurden im Snapshot mit Locked-Mode-Restore verwendet. SOURCE-SNAPSHOT.json und RELEASE-PROVENANCE.json ordnen Quellenarchiv, Manifest, SDK, native Laufzeit und Motor zu.

Kontrollplayer-Korrekturen: EOF-Kontext wiederherstellen, Navigation nach Tonspurfehler sicher fortsetzen, reguläre Position und Play/Pause erhalten, native Property-Fehler auffangbar machen und Dispose koordinieren. Keine weitere Funktionsänderung an Schnittmotoren, Klebecenter, Cutlists oder Namensmasken. [Analyse](KONTROLLPLAYER-TONSPUR-ABSTURZ-ANALYSE-20261101.md), [Implementierung](KONTROLLPLAYER-TONSPUR-KORREKTUR-20261101.md), [Freeze](FREEZE-1.0.2-BUILD48-20261101.md).

## Technische Prüfung

- Erfolgreicher nummerierter vollständiger Build über tools/build.ps1: Zähler genau einmal 47 → 48, **0 Warnungen / 0 Fehler**, anschließend **1634/1634 Tests**. Ausgabe wegen einer geöffneten beziehungsweise noch sperrenden Entwicklungsinstanz separat unter .build/release-1.0.2-build48-20261101/numbered-output. Keine weitere Buildnummernerhöhung.
- Unabhängiger vollständiger Snapshot-Build, Tests und frameworkabhängiger win-x64-Publish mit explizitem CanBuildNumber=48: **0 Warnungen / 0 Fehler**, **1634/1634 Pakettests**: Core 200, Media 390, App 972, Cutlists 72; 0 übersprungen.
- Zusätzlich **1362 Tests gegen exakt die finalen ausgelieferten Produkt-DLLs**: App 972, Media 390. Vier Produkt-DLLs jeweils SHA-256-identisch zum Publish. Keine Addition dieser Wiederholungen zur Anzahl unterschiedlicher Lösungstests.
- **32/32 native/WPF-Kontrollen in sechs separaten Prozessen**, jeweils Exitcode 0: regulärer Wechsel mit tatsächlichen Positionsmessungen (7), echter nativer Property-Fehler und Wiederanlauf (3), EOF und Fehler/Zurück (10), zwei echte Playerkontexte (3), Cancellation/Dispose (4), originales WPF-Kontrollfenster samt Fehlerbindung/Navigation/Schließen (5). Testprozess referenziert exakt die finalen Paketbibliotheken.
- **250/250 Portable-Dateien** bytegenau gegen Publish-Manifest und ZIP; keine Diagnosemedien, TRX oder Laufzeitlogs im Publish. Alle fünf ZIP-Archive einschließlich Drittanbieterquellen mit vollständiger CRC-Prüfung erfolgreich.
- Inno Setup 7 erfolgreich; Compilerprotokoll enthält exakt dieselben **250 Dateien** wie das Publish-Manifest, ohne fehlende oder zusätzliche Eingaben. Setup-Produktversion getrimmt **1.0.2**, App-Dateiversion **1.0.2.48**; Build 48 über unverändertes Inno-AppVerName und Compilerparameter sowie Dateinamen ausgewiesen. Setup-Dateiversionsressource bleibt wie bei Build 47 leer beziehungsweise mit Leerzeichen aufgefüllt; kein neuer Versionsmechanismus eingeführt.
- Vier native DLLs unverändert gegen den geprüften libmpv-Stack; Lizenzhinweise vorhanden. .NET 10 Desktop Runtime x64 und externe Werkzeuge weiterhin separat; kein .NET-/FFmpeg-/ffprobe-/MP4Box-/FFMS-Bündel. Motor-EXE **73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E**, Version 0.1.6, Fähigkeiten mp3-aac-v1 / aac-overlap-v1.
- Alle acht Auslieferungsdateien vor und nach Freischaltung anhand GitHub-Größen und SHA-256-Digests verifiziert. Build-47-Release und seine acht Assets unverändert erhalten.

Vorbereitende lokale Fehler sind protokolliert: Der erste Restore ohne Runtimebezeichner passte nicht zu den vorhandenen unterschiedlich behandelten Locks. Ein gesperrter win-x64-Restore gegen separate Kopien der committed Locks war erfolgreich, ohne die vier Nutzerdateien zu ändern. Lokale Builds beziehungsweise ein späterer Zusatzabgleich des bisherigen Entwicklungs-Ausgabeordners scheiterten an Dateisperren einer noch vorhandenen CAN-Instanz; dieser Ordner ist keine Paketquelle. Ein zu früher erster Paketstart fand das noch nicht fertig geschriebene Quellen-ZIP nicht; der anschließende reguläre Paketlauf erfolgte erfolgreich aus dem vollständigen Snapshot. Ein Setup-Prüfvergleich wurde um das Trimmen der auch in Build 47 vorhandenen Ressourcen-Leerzeichen berichtigt. Keine dieser Prüfkorrekturen änderte Produktquellen oder Auslieferungsartefakte. Die maßgeblichen vollständigen nummerierten und Snapshot-/Paketbuilds waren erfolgreich.

## Persönliche Praxisabnahme

Der Nutzer bestätigt die korrigierte Entwicklungsfassung mit diesen Originaldateien:

- Ghosts__Weihnachtsgeister_-_Teil_1_26.10.23_19-15_comedycentral_21_TVOON_DE.HQ.mp4
- Ghosts__Weihnachtsgeister_-_Teil_2_26.10.26_19-15_comedycentral_25_TVOON_DE.HQ.mp4

Ursprünglicher EOF-Absturzfall, regulärer Tonspurwechsel und Prüfstellen-Navigation einwandfrei. Diese persönliche Bestätigung gilt der korrigierten Entwicklungsfassung. Keine neue persönliche Setup-/Portable-/Windows-11-VM-/Deinstallationsabnahme behauptet; die Paketinhalte sind separat technisch geprüft.

### Nachtrag 01.11.2026 – Setup persönlich positiv geprüft

Nach der Veröffentlichung bestätigt der Nutzer: **„setup installiert und mit der commedy-Testdatei geprüft, alles bestens :-)“**. Damit sind die Installation des veröffentlichten **CAN 1.0.2 Build 48** und die anschließende Nutzung mit der comedy-Testdatei persönlich positiv bestätigt. Diese Rückmeldung ergänzt die frühere Abnahme der Entwicklungsfassung um einen tatsächlichen Setup-Nachweis. Eine genaue Dateizuordnung, VM-, Portable- oder Deinstallationsprüfung wurde dabei nicht gesondert genannt.

### Nachtrag 02.11.2026 – AVI-Altfall auf dem Arbeitsrechner positiv geprüft

**CAN 1.0.2 Build 48, Setup auf dem Arbeitsrechner**, vom Nutzer ausdrücklich zugeordnet. Original: **A_90s_Christmas_Eine_weihnachtliche_Zeitreise_25.12.06_20-15_srtl_105_TVOON_DE.mpg.HQ.avi** (A ’90s Christmas – Eine weihnachtliche Zeitreise). Laut übermitteltem Protokoll AVI mit H.264 High, 720×576, 16:9, **25 fps**, MP3-Stereo mit **48000 Hz**, Quelldauer **02:00:48.800**.

Das Protokoll belegt die verlustfreie Vorbereitung einer MP4-Arbeitsdatei durch FFmpeg mit Video- und Audiostreamcopy. FFmpeg meldet dabei **„Timestamps are unset in a packet for stream 0“** und wiederholt **„pts has no value“**. Die anschließende CAN-Prüfung bestätigt die vollständige, exakte Videopaketanzahl sowie Stream-, Bildraten- und Laufzeitprüfung. Das belegt keine allgemeine Reparatur der AVI-Zeitstempel.

Anschließend vier MP4Box-Behaltebereiche in Sekunden: **323,92–2221,48**, **2640,12–3513,72**, **3921,88–5116,16** und **5504,32–6327,24**. Alle vier Segmente werden zusammengefügt; die Schnittstellenprüfung durchläuft Abschnitt 1 bis 4 und endet mit **„Fertig.“**. Die MP4Box-Version ist im übermittelten Protokoll nicht ausgewiesen.

Der Nutzer bestätigt **Ton, Bild, Schnitt und 3-Sekunden-Check** persönlich als bestanden. Damit liegt ein weiterer positiver Praxisfall des veröffentlichten Build-48-Setups vor. Der Nachweis gilt für diesen MP4Box-Lauf; daraus folgt keine otr-can-, VM- oder Portable-Abnahme. Reine Dokumentation der Nutzerprüfung, kein eigener neuer Medienlauf; bekannte AVI-Grenzen bleiben bestehen.

### Nachtrag 02.11.2026 – derselbe AVI-Altfall auch mit otr-can bestanden

Derselbe Originalfilm **A_90s_Christmas_Eine_weihnachtliche_Zeitreise_25.12.06_20-15_srtl_105_TVOON_DE.mpg.HQ.avi** jetzt in einem eigenen Lauf mit **otr-can 0.1.6**. Nutzer bestätigt ausdrücklich **„Ton Bild Schnitt 3 Sekunden vorschau check setup arbeitsrechner alt-fall check :-)“**. Damit sind Ton, Bild, Schnitt und Drei-Sekunden-Vorschau persönlich bestanden; **Setup auf dem Arbeitsrechner**, Build 48 gemäß der unmittelbar zuvor ausdrücklich genannten und hier fortgeführten Prüfumgebung.

Das neue Protokoll zeigt erneut FFmpeg-Streamcopy in eine MP4-Arbeitsdatei mit denselben Warnungen zu fehlenden AVI-Zeitstempeln. CAN bestätigt anschließend Stream-, Videopaket-, Bildraten- und Laufzeitprüfung. FFMS2 indexiert **181220 Frames**; die CAN-Vorprüfung bestätigt eindeutige, aufsteigende Bildzeiten und eine passende 25/50-fps-Zeitstruktur.

otr-can schneidet vier Behaltebereiche mit CPU-Neukodierung der erforderlichen Randbereiche und Kopieren der übrigen Videobereiche. Der Motor verifiziert **119713 Videopakete samt PTS/DTS**. Die MP3-Stereospur (48000 Hz) wird nach PCM-Schnitt einmal vollständig nach **AAC-LC, 192000 bit/s** kodiert; **229848960 decodierte Samples** und kontinuierliche Audio-PTS sind laut Protokoll bestätigt. Abschließende MP4-/Paketprüfung erfolgreich, Motorarbeitsordnersperre freigegeben, **„OTR-CAN-Schnitt abgeschlossen.“**

Damit ist dieser AVI-Altfall in der laufenden Build-48-Setup-Prüfrunde **mit MP4Box und otr-can jeweils technisch und persönlich positiv** belegt. Die konkreten Bereichsenden des otr-can-Protokolls liegen jeweils 40 ms nach den zuvor protokollierten MP4Box-Aufrufwerten; daraus wird keine identische Ausgabedauer beider Motoren behauptet. Bekannte allgemeine AVI-Grenzen bleiben bestehen. Nur Nutzerprotokoll und persönliche Rückmeldung ausgewertet, kein eigener Medienlauf und keine neue VM-/Portable-Abnahme.

### Nachtrag 02.11.2026 – Venom mit zwei Tonspuren und Tonwechsel am Videoende bestanden

**Venom: The Last Dance**, Original **Venom__The_Last_Dance_26.11.01_20-15_4plus_150_TVOON_DE.HD.mp4**. Zuordnung **CAN 1.0.2 Build 48 / Setup / Arbeitsrechner** aus der laufenden, zuletzt ausdrücklich genannten Prüfumgebung fortgeführt; im übermittelten Protokoll selbst keine CAN-Buildanzeige.

Das Protokoll belegt sechs MP4Box-Schnitte mit den Bereichen (Sekunden) **587,12–2251,96**, **3124,08–3943,08**, **4798,72–5409,04**, **5690,64–6409,76**, **7208,44–8176,12** und **9018,52–9524,96**, anschließend Zusammenfügen aller sechs Segmente. Für das erste Segment werden eine Video- und zwei Tonspuren im Ziel angelegt. Die Schnittstellenprüfung durchläuft alle sechs Abschnitte und endet mit **„Fertig.“**. An den Übergängen **1, 3, 4 und 5** meldet CAN gleiche Bildzeiten und prüft jeweils den dekodierten neuen Anfang; die Hinweise bleiben dokumentiert. Audio-Codecs, Sprachen und MP4Box-Version sind darin nicht ausgewiesen.

Der Nutzer bestätigt **Schnitt, Bild, Tonspur 1 und Tonspur 2, Tonwechsel auch am Videoende sowie Drei-Sekunden-Vorschau** persönlich als bestanden. Damit liegt ein weiterer positiver Mehrspur-Praxisnachweis einschließlich des für Build 48 relevanten Tonwechsels am Videoende vor. Ein bereits eingetretenes natürliches EOF wird in dieser Rückmeldung nicht gesondert beschrieben. Nur Nutzerprotokoll und persönliche Prüfung dokumentiert, kein eigener neuer Medienlauf.

### Nachtrag 02.11.2026 – Venom auch mit otr-can und beiden Tonspuren bestanden

Derselbe Originalfilm **Venom__The_Last_Dance_26.11.01_20-15_4plus_150_TVOON_DE.HD.mp4** jetzt mit **otr-can 0.1.6**, sechs Behaltebereiche. **Setup Build 48 auf dem Arbeitsrechner** aus der laufenden Prüfumgebung fortgeführt; das Protokoll selbst enthält keine CAN-Buildanzeige. FFMS2 und CAN bestätigen **307482 eindeutige, aufsteigende Quellbildzeiten** und eine passende 25/50-fps-Zeitstruktur.

Der Motor kodiert die erforderlichen Videoränder auf der CPU neu und kopiert die übrigen Bereiche. **132191 Videopakete samt rohen PTS/DTS** verifiziert. Beide Tonspuren werden nach dem PCM-Schnitt jeweils einmal vollständig kodiert: **Ton 1 AAC, Zielbitrate 160526 bit/s; Ton 2 AC-3, Zielbitrate 384000 bit/s**, jeweils Stereo/48000 Hz. Für **jede Spur 253806720 decodierte Samples** und kontinuierliche rohe PTS bestätigt. Finale MP4 einschließlich aller Video-/Audiopakete, Zeitstempel, Padding und Decoderkonfigurationen laut Motor erfolgreich verifiziert; Sperre freigegeben, **„OTR-CAN-Schnitt abgeschlossen.“** Die optionale vollständige Videodekodierung ist durch die allgemeine Protokollzeile nicht gesondert belegt.

Der Nutzer bestätigt auch für diesen Lauf **Schnitt, Bild, Ton 1 und Ton 2, Tonwechsel auch am Videoende und Drei-Sekunden-Vorschau** persönlich als bestanden. Damit ist „Venom: The Last Dance“ **mit MP4Box und otr-can jeweils technisch und persönlich positiv** geprüft, einschließlich beider Tonspuren und Tonwechsel am Videoende. Ein bereits eingetretenes natürliches EOF wird nicht separat beschrieben. Die otr-can-Bereichsenden liegen im Protokoll jeweils 40 ms nach den vorherigen MP4Box-Aufrufwerten; keine identischen Ausgabedauern behauptet. Reine Dokumentation des übermittelten Protokolls und der persönlichen Rückmeldung, kein eigener Medienlauf.

### Nachtrag 02.11.2026 – Navy CIS mit MP4Box bestanden

Original **Navy_CIS_26.11.01_20-15_5plus_50_TVOON_DE.HD.mp4**, 5plus-HD-Aufnahme vom 01.11.2026. **CAN 1.0.2 Build 48 / Setup / Arbeitsrechner** aus der laufenden Prüfumgebung fortgeführt. Das übermittelte Protokoll belegt zwei MP4Box-Behaltebereiche in Sekunden: **577,84–1861,32** und **2605,68–3781,44**, anschließendes Zusammenfügen sowie die Schnittstellenprüfung beider Abschnitte bis **„Fertig.“**. Beim Anlegen der Zielspuren erscheinen eine Video- und zwei Tonspuren; Codecs, Sprachen und MP4Box-Version werden darin nicht ausgewiesen.

Der Nutzer bestätigt **Schnitt, Bild, Ton und Drei-Sekunden-Vorschau** persönlich als bestanden. Eine getrennte Prüfung beider Tonspuren oder ein Tonwechsel am Videoende wird für diesen Fall nicht gesondert genannt. Nur Nutzerprotokoll und persönliche Rückmeldung dokumentiert, kein eigener Medienlauf.

### Nachtrag 02.11.2026 – Navy CIS auch mit otr-can und Tonspurwechsel bestanden

Derselbe Originalfilm **Navy_CIS_26.11.01_20-15_5plus_50_TVOON_DE.HD.mp4**, jetzt mit **otr-can 0.1.6** und zwei Behaltebereichen. **Setup Build 48 auf dem Arbeitsrechner** aus der laufenden Prüfumgebung fortgeführt. FFMS2 und CAN bestätigen **157431 eindeutige, aufsteigende Quellbildzeiten** und eine passende 25/50-fps-Zeitstruktur. Erforderliche Videoränder werden auf der CPU neu kodiert, die übrigen Bereiche kopiert; **61483 Videopakete samt rohen PTS/DTS** verifiziert.

Beide Tonspuren werden nach PCM-Schnitt jeweils einmal vollständig kodiert: **Ton 1 AAC, Zielbitrate 160276 bit/s; Ton 2 AC-3, Zielbitrate 384000 bit/s**, jeweils Stereo/48000 Hz. Für **jede Spur 118047360 decodierte Samples** und kontinuierliche rohe PTS bestätigt. Finale MP4 mit allen Video-/Audiopaketen, Zeitstempeln, Padding und Decoderkonfigurationen laut Motor verifiziert; Sperre freigegeben, **„OTR-CAN-Schnitt abgeschlossen.“**

Der Nutzer bestätigt **Ton einschließlich Tonspurwechsel, Schnitt und Drei-Sekunden-Vorschau** persönlich als bestanden. Damit liegen für Navy CIS positive Praxisnachweise für beide Motorwege vor; der Umfang der früheren MP4Box-Bestätigung und dieser otr-can-Bestätigung bleibt jeweils separat beschrieben. Ein Tonwechsel am Videoende oder eine neue ausdrückliche Bildbestätigung wird in dieser Rückmeldung nicht gesondert genannt. Nur Protokoll und persönliche Rückmeldung dokumentiert, kein eigener Medienlauf.

### Nachtrag 02.11.2026 – Navy-CIS-Cutlist abgeglichen

Die vom Nutzer benannte **Navy_CIS_26.11.01_20-15_5plus_50_TVOON_DE.HD.mp4.cutlist** nur lesend geprüft. Vorgeschlagener Filmname: **Navy CIS S23E18 Die Edna-Dateien [01.11.2026]**. Metadaten: CAN 1.0.2, **25 fps**, 16:9, Originaldateigröße **2093275829 Bytes**, vorgesehener Motor **otr-can 0.1.6**, zwei Behaltebereiche.

Cutlist-Planwerte: **Start 577,84 s / Dauer 1283,52 s / Ende 1861,36 s** und **Start 2605,68 s / Dauer 1175,80 s / Ende 3781,48 s**. Geplante Gesamtdauer **2459,32 s = 00:40:59.320**; bei 25 fps **61483 Bilder** und bei 48000 Hz **118047360 Samples je Tonspur**. Diese berechneten Planwerte stimmen mit der Videopaketanzahl und den verifizierten Audio-Samplezahlen des otr-can-Protokolls überein. Die früheren MP4Box-Aufrufenden liegen jeweils **40 ms** vor diesen Cutlist-Endwerten; keine Gleichheit der tatsächlichen Ausgabedauern beider Motoren daraus abgeleitet.

Der Cutlist-Kommentar vermerkt vollständig entfernte Werbung, framegenauen otr-can-Schnitt und alternativ MP4Box aus GPAC **Version 26.07**. Dies sind Angaben der Cutlist; das frühere MP4Box-Protokoll selbst weist keine Versionsnummer aus. Die persönlichen Prüfbestätigungen bleiben wie oben dokumentiert. Cutlist unverändert; nur ergänzender Metadaten- und Zahlenabgleich, kein neuer Schnitt oder eigener Bild-/Tontest.

## Auslieferungsdateien

Lokaler eingefrorener Paketsatz: **artifacts/release-1.0.2-build48**. Genau die folgenden acht Dateien sind veröffentlicht. SHA256SUMS.txt prüft die sechs Archive/Programme; PUBLISH-FILES.sha256 alle 250 Portable-Nutzdateien.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-1.0.2-Build48-source.zip | 3726259 | 902A4C9A0287A05A824DB4119157B1B6A883CA7E86678FF10F11C1AB980C4A17 |
| CutAssistantNext-1.0.2-Build48-win-x64-Setup.exe | 20518042 | 0FB98F8F2555413669ABCA9233A240CDA2BA4D340286D695B3E8B799486D3B9C |
| CutAssistantNext-1.0.2-Build48-win-x64.zip | 24555623 | B6C6C2D23DF7B48CDAD2FC1CD3197D3C1B6F82E54B9EFBD0311ED44C07B254E2 |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| PUBLISH-FILES.sha256 | 32911 | 6CA21506C4C285A61A24FD52D3C1E602473FC9ACA812F4D4DADC606C6E466252 |
| SHA256SUMS.txt | 647 | B2F82062AF061E737927F180C8E52D92119493A108F494B362AD9DFF9E3D3D5E |

## Erhaltung und Nachweise

**58 geschützte Ursprungsdateien** während der Paketierung unverändert, **377 Produkt-/Testdateien** seit Snapshot-Vorbereitung unverändert. Die vier lokalen Produktions-Lockänderungen und Version.props stimmen weiterhin mit ihren Ausgangsprüfsummen überein. Der Buildzähler ist 48. Alte Releases/Tags/Archive nicht geändert.

Nachweise unter **.build/release-1.0.2-build48-20261101/**: PACKAGE-INPUTS.json, PACKAGE-PROOF.json, QA-SUMMARY.json, SETUP-PROOF.json, ARCHIVE-CRC.json, ALL-ASSETS.json, nummerierter Build/Testlauf, final-package-results/*.trx, native-*.json, PUBLISHED.json, REMOTE-DRAFT-PROOF.json und REMOTE-PROOF.json. Paketlogs und vier Snapshot-Test-TRX zusätzlich unter artifacts/release-1.0.2-build48. Quellenarchiv und Dokumentation im Paket bewahren den Stand vor dem reinen Ergebnisnachtrag auf main.

[Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD48-PUBLISHED-20261101.md), [Anleitung](NUTZERANLEITUNG.md), [Versionen](VERSIONEN.md).
