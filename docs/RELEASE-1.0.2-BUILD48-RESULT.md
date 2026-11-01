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
