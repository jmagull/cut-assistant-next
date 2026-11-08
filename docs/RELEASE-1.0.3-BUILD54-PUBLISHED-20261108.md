# Cut Assistant Next 1.0.3 Build 54 – Release und persönliche Freigabe

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
