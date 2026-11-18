# Cut Assistant Next 1.0.3 Build 61 – neue Prüfpakete vom 18.11.2026

**Abgeschlossen: gezielte EAC3-Dialogkorrektur, vollständige Dokumentation und tatsächlich erzeugte neue Setup-/Portable-Pakete.** Ausschließlich lokale persönliche Abnahme; keine Veröffentlichung und keine automatische Installation.

## 1. Implementierte Änderung

Bei der ausdrücklichen Motorablehnung **Unsupported audio codec eac3** zeigt CAN jetzt:

**otr-can – EAC3-Tonspur erkannt**

> Eine EAC3-Tonspur wurde erkannt. Der verwendete otr-can-Schnittmotor unterstützt dieses Audioformat derzeit noch nicht.
>
> Möchtest du den Film stattdessen mit MP4Box schneiden?

Sichtbare Entscheidungen: **Mit MP4Box schneiden** und **Abbrechen**. Abbrechen bleibt die Voreinstellung. MP4Box startet ausschließlich nach Zustimmung und mit demselben Schnittauftrag. Ist MP4Box nicht verfügbar, bleibt dessen Schaltfläche gesperrt und eine Erklärung sichtbar.

Die Erkennung prüft vollständige Fehlerzeilen einschließlich verschachtelter Werkzeugfehler; ähnliche Codec-Namen und andere Audiofehler werden nicht als EAC3 eingestuft. Bei dieser eindeutig nicht unterstützten Konstellation werden die unpassenden AAC-Reparatur- und Spur-Abwahl-Assessments ausgelassen. Es wurde keine zusätzliche Medienvorprüfung eingeführt. Die normalen Audiofehler behalten ihren bisherigen Ablauf.

Der otr-can-Motor ist unverändert. Keine EAC3-Dekodierung/-Reparatur, keine Toleranzänderung, keine automatische Tonspurentfernung und keine Änderung an Schnittgrenzen oder Ausgabeeinstellungen. Die allgemeine EAC3-Empfehlungsgrenze und der gesonderte 40-ms-Rasterbefund bleiben ausdrücklich außerhalb dieser Korrektur.

Betroffene Produktdateien:

- src/CutAssistantNext.App/Services/Cutting/OtrCanAacEmergencyCutService.cs: genaue Erkennung des Ablehnungsgrunds.
- src/CutAssistantNext.App/Services/Cutting/OtrCanMp4BoxFallbackCutService.cs: passende Ausweichentscheidung ohne unpassende Recovery-Prüfungen.
- src/CutAssistantNext.App/Dialogs/AudioMp4BoxFallbackDialog.xaml und .xaml.cs: exakter EAC3-Dialog, zwei Entscheidungen, sichere Voreinstellung.
- tests/CutAssistantNext.App.Tests/Eac3FallbackTests.cs: 22 neue gezielte Fälle.
- tools/package-release.ps1: PDB-Dateien vom Vertrieb ausschließen, README und aktuelle Dokumentation einpacken.

## 2. Dokumentation

Aktualisiert: README.md, docs/README.md, docs/NUTZERANLEITUNG.md, docs/RELEASE-NOTES-1.0.3.md, docs/PRUEFHINWEISE-BUILD61.md, docs/TESTPLAN.md, docs/SETUP-MERKLISTE.md, installer/HINWEIS-DOTNET-UND-WERKZEUGE.txt sowie der neue Bericht docs/EAC3-DIALOG-BUILD61.md. Der historische EAC3-Diagnoseharness ist ausdrücklich als Prüfung des vorherigen Kandidaten gekennzeichnet.

Die vollständigen Build-61-Funktionen einschließlich Tastenkürzeln, Wiederholtempo, fortlaufendem Shuffle, K-Markierungen, Empfehlung, Qualitätsalternativen, erneutem Prüfen und freien Namen bleiben dokumentiert. Die acht bestandenen Realfilmschnitte vom 17.11.2026 bleiben erhalten. Auch die Qualitätsalternativsuche samt Größenwarnung und bewusster HQ-/HD-Übernahme sowie der persönlich bestandene Wetterfee-MP4Box-Schnitt vom 18.11.2026 sind berücksichtigt. Ein fehlendes detailliertes MP4Box-Protokoll wird nicht ersetzt oder erfunden. Der Realfilmschnitt wurde nicht wiederholt.

Eindeutig erläutert: otr-can unterstützt EAC3 weiterhin nicht; MP4Box wird nur nach Bestätigung als alternatives Schnittverfahren gestartet und ist keine allgemeine Reparatur für beschädigten Ton.

## 3. Tatsächlich neu erzielte Testergebnisse

| Suite | Gesamt | Bestanden | Fehler | Übersprungen |
| --- | ---: | ---: | ---: | ---: |
| CAN Core | 200 | 200 | 0 | 0 |
| CAN Media | 402 | 402 | 0 | 0 |
| CAN App | 1507 | 1507 | 0 | 0 |
| CAN Cutlists | 80 | 80 | 0 | 0 |
| Vollständige CAN-Suite | **2189** | **2189** | **0** | **0** |
| Spotlight Core/Media | 59 | 59 | 0 | 0 |
| Spotlight App | 19 | 19 | 0 | 0 |
| CAN und Spotlight gesamt | **2267** | **2267** | **0** | **0** |

Release-Build mit .NET SDK 10.0.303: **0 Warnungen, 0 Fehler**. Vor der Paketierung außerdem **84 gezielte Tests** bestanden. Die 22 neuen Fälle decken exakten EAC3-Text, AAC+EAC3, verschachtelte Fehler, andere Codecs, gesperrtes MP4Box, Abbrechen, explizite Zustimmung und unveränderte allgemeine Audioentscheidungen ab. Bestehende AAC-/Audio-/WPF-/Schnitt-/Fallback-Regressionen sind in der vollständigen Suite enthalten.

Der Windows-x64-Publish erzeugt die RID-spezifische App-DLL. Sie stimmt bytegenau mit dem zugehörigen Release-Build überein. Zusätzlich wurden die Produkt-DLLs in der App-Testausgabe durch die finalen Paket-DLLs ersetzt und die **vollständigen 1507 App-Tests erneut erfolgreich** ausgeführt. Die anderen CAN-Fachbibliotheken stimmen bereits bytegenau mit den Ausgaben des vollständigen Testlaufs überein. Wiederholungen werden nicht zur Gesamtzahl 2267 addiert.

Native Paketprüfungen ebenfalls erfolgreich:

- Sechs Kontrollplayer-Gruppen, Spotlight-Integration und Filmende-/Shuffle-Prüfung gegen die finalen Produkt-DLLs.
- FFmpeg-/ffprobe-Streamcopy: 200 sichtbare Videopakete in richtiger Reihenfolge mit passenden Zeitstempeln und Hashwerten. Der bekannte problematische Vergleichsbuild vom 11.11.2026 wird weiterhin als inkompatibel erkannt.
- MP4Box-Funktion mit zwei Bereichen und zwei AAC-Spuren; synthetische Schnitte mit beiden Motoren liefern jeweils vier Sekunden und zwei AAC-Spuren.
- 20 native Audioregressionen einschließlich AAC-Reparatur, Mehrspurton, Abbruch und Rückfallentscheidungen.
- Motorversion, Fähigkeiten und Motorpaketprüfung.
- Zwölf synthetische EAC3-/AAC-Medienprüfungen mit dem unveränderten Paketmotor.
- Vier neue echte Gesamtabläufe mit synthetischer EAC3-Einspur- beziehungsweise AAC+EAC3-Mehrspurdatei: je Abbrechen und ausdrückliche MP4Box-Zustimmung. Die reale Motorablehnung wurde vom neuen Code erkannt, der tatsächliche WPF-Dialog geprüft und bei Zustimmung MP4Box wirklich ausgeführt. Die Ausgaben enthalten EAC3 beziehungsweise AAC+EAC3 unverändert als Tonspurformate. Abbrechen startet keinen Alternativmotor und veröffentlicht keine Ausgabe. Die Quelldateien blieben unverändert.

Alle TRX-, JSON- und nativen Einzelprotokolle liegen unter Pruefnachweise beziehungsweise den Testverzeichnissen. Keine realen Filmdateien wurden für diese Tests geändert oder erneut geschnitten.

## 4. Neue Pakete und SHA-256

Gemeinsamer Ablageordner:

~~~text
C:\Users\joerg\Documents\GitHub\cut-assistant-next\artifacts\release-1.0.3-build61-candidate-20261118
~~~

| Datei | Exakte Größe in Bytes | SHA-256 |
| --- | ---: | --- |
| CutAssistantNext-1.0.3-Build61-win-x64-Setup.exe | 20883861 | E969D1C5BD2C41B6A312FFFDF35F73181E2C9D52628DE10FB75B2293B0A54A3A |
| CutAssistantNext-1.0.3-Build61-win-x64.zip | 25138936 | 9D506C82E5279AB3D9039ED7FA95791F8E55DD60821B2ADD8ED00D3FF4E043E9 |

Die Dateinamen, Produktversion **1.0.3**, Buildnummer **61** und Dateiversion **1.0.3.61** bleiben unverändert. Das Verzeichnis verwendet ausschließlich das Datum, keine neue RC-Bezeichnung.

Enthalten sind vollständiger Programmordner, libmpv 0.41.0 mit den vier verifizierten nativen Bibliotheken sowie otr-can 0.1.6-audio-diag.1 unter **tools/otr-can/otr_can.exe** mit Quellen-/Lizenzhinweisen. FFmpeg, ffprobe, MP4Box und FFMS2 sowie .NET 10 Desktop Runtime x64 und gegebenenfalls Visual C++ v14 Redistributable x64 bleiben nach dem vorhandenen Paketkonzept externe Voraussetzungen.

## 5. Paket- und Startprüfung

**268 Programmdateien** im Publish-Manifest, in der Portable-ZIP, im frisch entpackten Programmordner und in den protokollierten Setup-Kompiliereingaben stimmen überein. Vollständiges Datei-/SHA-256-Manifest: PUBLISH-FILES.sha256. Quellen: 929 Dateien einschließlich Manifest; Quellarchiv, Motorquellen und libmpv-Quell-/Relinkarchive sind vorhanden und auf Archivintegrität geprüft.

Alle 104 Einträge des Motor-Manifests stimmen; einschließlich des Manifests umfasst sein Ordner 105 Dateien. Der Motorhash bleibt **4146EE12F81BB7A4FAEF2D4F25D742A17649959BE8C36491169055F5CF9FA4D1**. Die vier libmpv-Hashes stimmen mit der bewährten Referenz überein. Keine PDB-Dateien, persönlichen Konfigurationen oder zusätzlichen Entwicklungs-/Videotestdateien in der Portable-ZIP.

Die neue App-DLL hat SHA-256 **82A39E7A8BFBDB53CC320EDE082CD5983802974D4CDBD94662CE7B0A98E9EEF4** und unterscheidet sich von der vorherigen Kandidaten-App. Die Paket-DLLs stammen aus dem frischen Release-Publish und wurden in den nativen Tests sowie dem vollständigen App-Testlauf verwendet. README, Nutzeranleitung, Release Notes, EAC3-Hinweise, Voraussetzungen und vorhandene Lizenzdateien sind enthalten.

Portable-Start aus frisch entpacktem Ordner erfolgreich. Titel und Datei-/Produktmetadaten bestätigen Build 61. **19 persönliche Einstellungsdateien** vor und nach dieser Startprüfung verglichen: unverändert.

Setup wurde zusätzlich ausschließlich im **/HELP-Modus** mit direktem Prozessstart geprüft; Start möglich, keine Installation ausgeführt. Die Hilfeprüfung wurde nach kurzer Beobachtung gezielt beendet (Exitcode -1); daraus wird kein erfolgreicher Installations-/Upgrade-Lauf abgeleitet. Kein verbleibender zugehöriger /HELP-Prozess gefunden. Installation, Upgrade, Verknüpfungen und Deinstallation bleiben persönliche Prüfpunkte.

Die neuen Setup-/ZIP-Dateien wurden nach den Funktionstests erneut gehasht: unverändert. Die **19** bisherigen Kandidatendateien sowie dessen **259** Programmdateien wurden mit ihren vorhandenen Manifesten verglichen: unverändert. Die frühere Prüffassung bleibt am bisherigen Ort vollständig erhalten.

## 6. Branch und Commit

- Arbeitszweig: **codex/build61-eac3-dialog**.
- Exakter Paketquellcommit: **009ca79cdd35e7527c1e7d816b1e4b0611be2112**.
- Quellenbasis enthält die gesamte bisherige Build-61-Entwicklung und die abgeschlossene Diagnose b2ad7c9.
- Angehängte Spotlight-Quelle unverändert: 75ee1475adf20a8c8231ab2989eab7eff37e6b09.
- SOURCE-SNAPSHOT.json und RELEASE-PROVENANCE.json im Programmordner verbinden Quellarchiv, Bibliotheken und Werkzeuge. Ein nachfolgender lokaler Berichtskommit ändert die bereits erzeugten Pakete nicht.

## 7. Verbleibende Grenzen und persönliche Abnahme

Die Korrektur erweitert keine EAC3-Unterstützung des Motors. Die allgemeine Schnittmotor-Empfehlung bei EAC3 und der separate 40-ms-Rasterbefund bleiben unverändert. Bekannte MP4Box-Keyframe-/Werbeblitzer-, AVI- und experimentelle AAC-Reparaturgrenzen bleiben bestehen; sehr lange AAC-Arbeits-/Ausgabepfade sind weiterhin ein dokumentierter Grenzfall. Für die native Audiomatrix wurde ein kurzer frischer Arbeitsordner verwendet. Der historische Spotlight-Prüfharness nutzt die bereits dokumentierte synchronisierte Shuffle-Erwartung für den fortlaufenden Ablauf; keine neue Produktänderung dafür.

Konkrete persönliche Windows-11-Prüfung:

1. CAN schließen, neues Setup installieren und eine bestehende Installation aktualisieren. Start, Version, Verknüpfungen und erhaltene Einstellungen kontrollieren. Deinstallation vorzugsweise in der VM testen.
2. Neues ZIP vollständig in einen neuen Ordner entpacken; Version, Motorpfad und Werkzeuge prüfen.
3. Wesentliche Build-61-Funktionen einschließlich Navigation/Wiederholtempo, Shuffle, Schnittliste/K-Markierung, freie Namen, Cutlists und Wiederaufruf der Prüfung stichprobenartig kontrollieren. Bereits bestehende persönliche Abnahmen bleiben erhalten.
4. Bei reproduzierter EAC3-Ablehnung Titel, Wortlaut und beide Schaltflächen kontrollieren. Zunächst Abbrechen prüfen; kein MP4Box-Lauf darf starten. Einen zusätzlichen MP4Box-Schnitt nur nach bewusster Wahl und in ein neues Ziel ausführen, falls gewünscht. Den bereits bestandenen Wetterfee-Referenzschnitt nicht unnötig wiederholen oder überschreiben.
5. Diese neue Paket-/Dialogfassung persönlich bestätigen. Automatisierte Tests und technische Startprüfungen gelten ausdrücklich nicht als persönliche Windows-11-Abnahme. Keine persönliche Windows-10-Prüfung behauptet.

## 8. Veröffentlichungsstatus

**Neue lokale Prüfpakete tatsächlich erstellt und technisch geprüft. Persönliche Abnahme offen.** Kein Push, kein Merge nach main, kein GitHub-Release oder Release-Tag, keine Änderung der öffentlichen Latest-Version und keine öffentliche Bereitstellung. Die öffentliche stabile Version bleibt unverändert. Eine Veröffentlichung erfordert weiterhin eine gesonderte ausdrückliche Freigabe.
