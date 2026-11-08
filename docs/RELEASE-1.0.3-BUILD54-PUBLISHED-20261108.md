# Cut Assistant Next 1.0.3 Build 54 – Release und persönliche Freigabe

## Erfolgreich veröffentlicht und unabhängig nachgeprüft

**2026-11-08T17:20:45Z**: **[Cut Assistant Next 1.0.3 Build 54](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.3-build54)** als stabile **Latest-Version** veröffentlicht, Release-ID **438393808**, kein Entwurf und keine Vorabversion. Acht Assets vor der Freischaltung und anschließend erneut anhand von Namen, Status, Byteanzahl und GitHub-SHA-256 geprüft. Zusätzlich **alle acht veröffentlichten Assets neu heruntergeladen und lokal per SHA-256 identisch bestätigt**. Latest-Endpunkt, Remote-main, annotierter Tag, README und Downloadverweise nach Freischaltung überprüft.

- Persönlich geprüfter funktionaler Quellcommit: **249511d001c327526e447b6a94127f3ac0c085f2**.
- Endgültiger Dokumentations-/Release-Quellcommit, vollständiger Quellarchivstand und Tagziel: **2cc0b98108640e2c2ba6cff422ef2749931e6f6a**.
- Annotierter Tag **v1.0.3-build54**, Tagobjekt **3007d70ac0f6c7f1b3d9837c2f7d723514e5ec3b**, auf exakt diesem Commit. Bestehende Tags nicht verändert.
- main vor Veröffentlichung kontrolliert von **22f4042f825a2db7c392a1f465d05eda3cacd54a** auf diesen Nachfahren vorgerückt, vollständige Historie erhalten. Main und neuer Tag gemeinsam atomar ohne Force gepusht.
- Nach Veröffentlichung folgt ausschließlich dieser Dokumentationsnachtrag auf main. Release-Tag, Sourcesnapshot und alle hochgeladenen Pakete bleiben unverändert beim angegebenen Release-Commit. Der abschließende main-/Dokumentationscommit steht im lokalen `FINAL.json` und im Abschlussbericht; keine Produktänderung.

## Finale Build-, Quellen- und Paketprüfung

Vollständiger Rebuild aus dem endgültigen Release-Quellsnapshot: **0 Warnungen/0 Fehler**. **1857 CAN-Tests und 78 Spotlight-Tests**, zusätzlich erneut dieselben **1935 Tests gegen die endgültig ausgelieferten Produkt-DLLs**, keine Fehler oder übersprungenen Tests. **20 native Audiofälle, 32 native Spotlight-Prüfungen, 32 Kontrollplayer-Prüfungen, vier Fenster-Prozesstests und 13 Motor-Paketprüfungen**, alle erfolgreich. Wiederholungen sind keine zusätzlichen unterschiedlichen Fälle. Automatisierte Entscheidungen und echte persönliche Abnahmen bleiben getrennt bezeichnet.

**848 manifestierte Quelldateien / 849 Quellen-ZIP-Dateien**, vollständiger bytegenauer Vergleich mit dem Release-Commit und dem angehefteten Spotlight-Commit. **265 Publish-Dateien**, ZIP-Inhalt identisch und exakt dieselben 265 Eingaben im erfolgreichen Inno-Compilerlauf. **Fünf äußere ZIP-Archive plus eingebettetes Motorquellarchiv** vollständig mit CRC geprüft. Native DLLs, Lizenz-/Quellenhinweise, App-/Setup-Metadaten sowie Motorversion und -fähigkeiten bestätigt. Keine persönlichen Einstellungen, Logs, lokalen Filmdateien oder Testmedien in den CAN-Paketen.

App- und Setup-Dateiversion **1.0.3.54**, regulärer Produktname **Cut Assistant Next**. Finaler Publish und vollständig entpackte ZIP tatsächlich gestartet, letztere aus einem Ordner mit Leerzeichen. Eigene Startprüfprozesse vor dem Schreiben persönlicher Schließeinstellungen beendet; reguläres Schließen separat in isolierten Fensterprozessen geprüft. Benutzerkonfiguration unverändert. Keine Installation oder Änderung der lokalen Windows-Installation.

**256 bisherige Publish-Dateien bytegleich**, darunter sämtliche Programmdateien, Produktbibliotheken, Symbole, nativen DLLs und der Motor. Ausschließlich acht Dokumentations-/Herkunftsdateien aktualisiert und `DANKSAGUNG.md` ergänzt. App-DLL unverändert **6AB32EA6A55818A9B01AC1C6630CB468A81C0E4DB28F54F51154E7085F18D779**, Motor unverändert **4146EE12F81BB7A4FAEF2D4F25D742A17649959BE8C36491169055F5CF9FA4D1**. Alle produktiven Build-Eingaben im Git-Vergleich mit dem persönlich geprüften Quellstand identisch. Der separate Validierungsneubau erzeugte App-DLL-Hash **4CF9C760ADF505C3E6F8FC5F3E71E7B65856C43CED7D2861AC77EF46EC106434**; diese neu erzeugte DLL wird nicht ausgeliefert. Keine allgemeine binäre Reproduzierbarkeit behauptet.

Die öffentlich angebotenen Setup-/Portable-/Quell-ZIPs haben gegenüber dem persönlichen Prüfpaketsatz ausdrücklich neue Hashes wegen aktualisierter Dokumentation, Quellenzuordnung und Provenienz. Der ursprüngliche Build-54-Paketsatz im ursprünglichen Arbeitsordner bleibt vollständig unverändert. Die öffentlichen Pakete liegen lokal unter `.build/worktrees/build54/artifacts/release-1.0.3-build54`. Die normalen relativen Paket-/Dateinamenkonventionen bleiben erhalten.

## Verifizierte öffentliche Assets

| Download | Bytes | SHA-256 |
| --- | ---: | --- |
| [CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build54/CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip) | 405926830 | `01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B` |
| [CAN-libmpv-v0.41.0-relink-2026-09-26.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build54/CAN-libmpv-v0.41.0-relink-2026-09-26.zip) | 47273515 | `00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342` |
| [CutAssistantNext-1.0.3-Build54-source.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build54/CutAssistantNext-1.0.3-Build54-source.zip) | 6536660 | `351CB44330885FA6AC6F6EA3E6ED7D350F87B29BA011C850C516468FC3B1559F` |
| [CutAssistantNext-1.0.3-Build54-win-x64-Setup.exe](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build54/CutAssistantNext-1.0.3-Build54-win-x64-Setup.exe) | 20932402 | `2FEC8FAFE48DECF955219A4F897CDAD137C5EB23C44524372BC910B9571B640B` |
| [CutAssistantNext-1.0.3-Build54-win-x64.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build54/CutAssistantNext-1.0.3-Build54-win-x64.zip) | 25739586 | `43EB8E376F12F2DEC5644EC8A51F9038C86621716151FD1084BF7BAFAFBE3B5D` |
| [otr-can-0.1.6-audio-diag.1-source.zip](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build54/otr-can-0.1.6-audio-diag.1-source.zip) | 127908 | `983C96E364E02A0F6B87BA1C6034995295E5A5E555C28CFB50BD48FB2CFAC504` |
| [PUBLISH-FILES.sha256](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build54/PUBLISH-FILES.sha256) | 34272 | `AD111378251852B05CB3A14C9B47115C7ABC3EEE1135DF535716CE7651837BF6` |
| [SHA256SUMS.txt](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build54/SHA256SUMS.txt) | 748 | `E66EFF84E577DE2054137C0B79FE45AC332D8F36098E9E31AB4101C2896E65E9` |

`SHA256SUMS.txt` deckt die sieben anderen Release-Assets einschließlich `PUBLISH-FILES.sha256` ab. Seine eigene Prüfsumme steht in dieser Tabelle und im Remote-Nachweis. Beide libmpv-Archive und das passende Motorquellarchiv bleiben bytegleich. GitHubs automatisch erzeugtes Quellenarchiv enthält das Submodul nicht vollständig und ersetzt den ausdrücklich beigefügten vollständigen Quellsnapshot nicht.

## Abschluss und Bestandsschutz

Alle **13 vorherigen GitHub-Releases** einschließlich des alten Latest **1.0.2 Build 48** und des bestehenden alten Entwurfs unverändert erhalten: IDs, Tagzuordnung, Titel, Texte, Veröffentlichungsstatus und sämtliche Asset-IDs, Namen, Größen und Digests. Alle bisherigen Remote-Tags und übrigen Remote-Branches unverändert; nur main vorgerückt und der neue Tag hinzugefügt.

**14 lokale Nutzerdateien** einschließlich der vier Produktions-Lockänderungen und des unversionierten alten Paketberichts bytegenau gesichert und im ursprünglichen Arbeitsordner unverändert erhalten. Dieser bleibt auf der ursprünglichen Basis **22f4042** mit identischem Änderungsstatus auf **codex/local-before-release54**. Sauberer aktueller main im gemeinsamen kanonischen Repository unter `.build/worktrees/build54`. Der Sicherungsbranch ist nur lokal, keine neue parallele Funktionsentwicklung. Frühere Funktions-Checkouts/Freeze-Commits und Zähler unverändert. **1122** Release-/Einstellungs-/Installationsdateien sowie **4473** frühere Schutzmanifest-Dateien identisch geprüft. Lokaler Release-Zähler weiter **54**, ursprünglicher Arbeitsordner weiter **48**.

Nachweise: `.build/release-1.0.3-build54-20261108/` mit `PACKAGE-PROOF.json`, `BUILD-PROOF.json`, `UNCHANGED-PRODUCT.json`, `REMOTE-DRAFT-PROOF.json`, `REMOTE-PROOF.json`, `DOWNLOADED.json`, `PRESERVED.json`, `PUBLISHED.json`, `LATEST.json`, Tag-/Refdaten, Quell-/Test-/Compilerlogs und abschließend `FINAL.json`. Beim Entwurf lieferte der Tag-Lese-Endpunkt zeitweise HTTP 404; der vorhandene eindeutige Entwurf wurde über die Release-ID geprüft, kein zweiter angelegt. Der Release-Textvergleich verwendet ausdrücklich UTF-8 und normalisierte Zeilenenden; ein anfänglicher Vergleich über die Windows-Standardcodepage wurde berichtigt, der unverändert hochgeladene Text stimmt überein. Die Startprüfhilfe wurde vor der maßgeblichen ZIP-Startprüfung auf den tatsächlich entpackten Pfad korrigiert; keine Produktdatei dadurch verändert.

README, Nutzer-/Spotlight-Anleitung, Release Notes, Architektur-/Buildhinweise, aktuelle Status-/Versions-/Testdokumentation, Credits und Voraussetzungstext aktualisiert; historische Berichte und frühere Quellenbelege erhalten. **Veröffentlichung abgeschlossen; hier angehalten.**

## Historischer Vorbereitungsstand vor Veröffentlichung


**08.11.2026:** Der Nutzer hat nach erfolgreicher persönlicher Prüfung ausdrücklich Commit, kontrollierte Übernahme nach main, Push, Release-Tag und stabile GitHub-Veröffentlichung als Latest beauftragt. Der nachstehende Vorbereitungsstand wird nach Veröffentlichung um die tatsächlich verifizierten Remote-Daten ergänzt.

## Verbindlicher Quellstand

Persönlich geprüfte Produktbasis: **249511d001c327526e447b6a94127f3ac0c085f2**, Branch `codex/integration-build54`, Dokumentationsstand davor **6d3b3fc1a46fb1d1c3203dfd2745dfbbc914be28**. Release-Tag entsprechend der bestehenden Konvention **v1.0.3-build54**. CAN **1.0.3 Build 54**, Dateiversion **1.0.3.54**, libmpv **0.41.0**, Motor **0.1.6-audio-diag.1** unter **tools/otr-can/otr_can.exe**.

Die Release-Vorbereitung ändert ausschließlich Dokumentation und Paket-/Quellnachweise. Keine neue Produktfunktion und kein neuer Buildzähler. Alle produktiven Quellen, Ressourcen, Projekte, Tests, Abhängigkeiten und Werkzeuge bleiben gegenüber der persönlich geprüften Basis unverändert. Der endgültige Tag zeigt auf den vollständigen Release-Quellstand mit diesen Dokumentationen; das zusätzliche vollständige Quellarchiv enthält auch das angeheftete Spotlight-CAN-Submodul und den passenden Motorquellstand.

## Persönliche Abnahme

**Cut Assistant Next 1.0.3 Build 54 wurde persönlich erfolgreich geprüft.** Die abschließende Release-Freigabe bestätigt die persönliche Prüfung insgesamt. Konkret im Chat zugeordnet:

- **Setup Build 54 in der Windows-VM, Grey’s Anatomy:** Motor aus `C:\Program Files\Cut Assistant Next\tools\otr-can\otr_can.exe`, regulärer Schnitt mit 60651 Videopaketen und 116449920 Audiosamples, abgeschlossene technische Prüfung. Bild, Ton und Schnitt vom Nutzer positiv bestätigt.
- **NCIS Sydney – Breathless, S02E07, Setup Build 54 in der Windows-VM:** bekannte 512 fehlende AAC-Samples erkannt, experimentelle Reparatur ausdrücklich bestätigt, Schnitt erfolgreich abgeschlossen. Der Nutzer bestätigt ausdrücklich **Ton, Bild und Drei-Sekunden-Vorschau**. Vollständiges übermitteltes Reparaturprotokoll getrennt vom Screenshot geprüft; keine automatische Reparatur.
- [Build-53-Abnahme](ABNAHME-BUILD53-20261108.md): Setup auf Arbeitsrechner sowie Setup/ZIP in der VM, Deinstallation in der VM, Spotlight-Fenster und direkte Schnittübernahme, reguläre MP4Box-/otr-can-Läufe, Mehrspur-/Tonspur-Abwahl, NCIS-Reparatur und beide Klebecenter-Wege. Diese früheren Einzelprüfungen bleiben Build 53 zugeordnet; sie werden nicht zu neu ausgeführten Build-54-Einzeltests umetikettiert.

Automatisierte Nachweise sind davon getrennt: [Build-54-Pfad-/Paketprüfung](PFADUMSTELLUNG-BUILD54-20261108.md), [vollständige Integrationsprüfung Build 53](PRUEFBERICHT-BUILD53-20261108.md). Die endgültigen Release-Prüfungen werden im Abschlussnachweis ergänzt.

## Bewährte Paketstruktur und unveränderte Produktdateien

Die acht bisherigen Auslieferungstypen bleiben erhalten: CAN-Setup, Portable-ZIP, vollständiger CAN-Quellsnapshot, passendes Motorquellarchiv, unveränderte libmpv-Quellen und Relink sowie `PUBLISH-FILES.sha256` und `SHA256SUMS.txt`. Reguläre Namen **CutAssistantNext-1.0.3-Build54-…**, keine CAN-Funktionssuffixe.

Die persönlich geprüften Build-54-Pakete im ursprünglichen `artifacts/release-1.0.3-build54` bleiben unverändert. Der öffentliche Satz wird unter derselben relativen Release-Verzeichnisstruktur im sauberen Release-Worktree erzeugt. Nach dokumentarischer Aktualisierung von Anleitungen, Voraussetzungen und Quellenzuordnung werden Setup/ZIP neu verpackt. Die tatsächlich geprüften Produktbinärdateien werden bytegleich übernommen; ein vollständiger Build aus dem endgültigen Quellsnapshot sowie CAN-/Spotlight-, native Audio- und Paketprüfungen kontrollieren den Stand erneut. Abweichende Archiv-/Setup-Hashes werden ausdrücklich dokumentiert.

Unveränderter App-DLL-Hash: **6AB32EA6A55818A9B01AC1C6630CB468A81C0E4DB28F54F51154E7085F18D779**. Unveränderter Motor-EXE-Hash: **4146EE12F81BB7A4FAEF2D4F25D742A17649959BE8C36491169055F5CF9FA4D1**. Angeheftetes Spotlight-CAN: **6973b37591e9b581b20b5a6f2117369b6df0f159**. Motorquelle: **1979a7f897b95d3fab4020129b8b5e634ea569ac**, Feature `experimental-aac-gap`. Keine Behauptung allgemeiner binärer Reproduzierbarkeit.

## Schutz lokaler Änderungen und früherer Releases

Im ursprünglichen Arbeitsordner liegen 14 bekannte lokale Dateien aus der früheren Build-48-Paketierung, einschließlich vier Produktions-Lockdateien und eines unversionierten Berichts. Sie sind bytegenau unter `.build/release-1.0.3-build54-20261108/user-files/` sowie als Patch gesichert und werden nicht übernommen, gestasht oder verworfen. Der Arbeitsordner bleibt mit denselben Dateien und derselben Basis **22f4042** auf dem lokalen Sicherungsbranch **codex/local-before-release54**. So kann main ohne Überschreiben dieser Änderungen im sauberen Integrations-Worktree übernommen werden. Dieser Sicherungsbranch ist keine zusätzliche CAN-Funktionsentwicklung und wird nicht veröffentlicht.

Die gemeinsame kanonische Git-Verwaltung bleibt unverändert; die vollständige Historie wird durch kontrolliertes Fast-forward nach main erhalten. Bei abweichender Remote-Historie oder inhaltlichem Konflikt wird angehalten. Frühere Tags und GitHub-Releases einschließlich 1.0.2 Build 48 bleiben erhalten. Kein Force-Push und keine automatische Installation oder Änderung der lokalen Windows-Installation.

## Veröffentlichungskontrollen

Der Tag wird erst nach fertiger lokaler Paketprüfung angelegt und gemeinsam mit main ohne Force gepusht. Alle Assets werden zunächst in einen eindeutigen GitHub-Entwurf hochgeladen; Name, Größe, Uploadstatus und SHA-256 werden gegen den lokalen Satz geprüft. Erst dann Freischaltung als stabile Latest-Version. Danach werden veröffentlichte Assets, Tagziel, main, Latest, README-/Downloadlinks und frühere Releases erneut kontrolliert. Der Ausführungsnachweis liegt unter `.build/release-1.0.3-build54-20261108/`.

[Release und Downloads](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.3-build54) · [Release Notes](RELEASE-NOTES-1.0.3.md) · [Anleitung](NUTZERANLEITUNG.md) · [Spotlight](SPOTLIGHT-README.md) · [Danksagung](DANKSAGUNG.md).
