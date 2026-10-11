# CAN 0.2.1 Build 12 – lokaler RC3-Paketsatz

Stand: 11.10.2026. Setup, Portable-ZIP und passende Quellen sind lokal erstellt und automatisch geprüft; die persönliche Paketabnahme ist abgeschlossen. Nicht veröffentlicht. Die bisherige öffentliche Build-7-RC2-Ausgabe bleibt erhalten.

## Quellstand und Paketform

CAN wurde aus einem eingefrorenen Quellarchiv mit 458 manifestierten Dateien gebaut: Basiscommit 885538fba3f81eb6147807a2463af1b950ebcb66 plus dokumentierte Arbeitsbaumänderungen, insbesondere AVI-Warnhinweis und neue Praxisberichte. SOURCE-SNAPSHOT.json nennt die enthaltenen Änderungen; SOURCE-FILES.sha256 hält die Dateien fest. Die vier vorbestehenden src-Lockdateiänderungen wurden ausdrücklich nicht übernommen: Das Archiv enthält deren eingecheckte Fassungen. Im ursprünglichen Checkout blieben alle acht Lockdateien per SHA-256 unverändert.

Frameworkabhängige Windows-x64-Ausgabe: .NET 10 Desktop Runtime separat, vier verifizierte libmpv-0.41.0-DLLs enthalten. FFmpeg, ffprobe, MP4Box und FFMS2/ffmsindex bleiben Benutzerinstallationen; kein DirectShow, Codec-Paket oder WSL. Buildnummer im Checkout weiterhin 12, Version 0.2.1 und RC3.

Der optionale Motor liegt in beiden Binärpaketen unter tools/otr-can/otr_can.exe und wird über Einstellungen > OTR-CAN-Werkzeuge ausgewählt. MP4Box bleibt Standard. Es wird die unveränderte, bereits geprüfte reguläre 0.1.4-EXE mit SHA-256 CDC9E9E3317F8CC4E9512249F28399DE4CCDFCE73491C4DF109A8E5333CD8C2D beigelegt.

## Quellen, Herkunft und Lizenzbestand

Motorquellen aus can-engine am sauberen Rust-Commit 8831021ae0e412b122b7db85583a79039f642343; keine privaten alten Decoder-/Vendor-/Versuchsbäume im Motorquellpaket. MPL-2.0-Lizenz, ursprüngliche SPDX-/Copyright-Hinweise, Herkunft und Danksagung erhalten. Das passende Quell-ZIP liegt direkt im Binärpaket unter tools/otr-can/sources/ und zusätzlich im Release-Ordner. Der Zugriff hängt damit nicht vom privaten Entwicklungsrepository ab. Die [Mozilla-MPL-FAQ Q9/Q10](https://www.mozilla.org/en-US/MPL/2.0/FAQ/) erläutert Quellbereitstellung und Hinweise für modifizierte MPL-Programme.

DEPENDENCIES.json und Originaltexte für 40 aufgelöste Windows-Fremd-Crates einschließlich Build-/Makroabhängigkeiten beigelegt; Lizenzdeklarationen MIT, Apache-2.0, Unlicense und zusätzlich Unicode-3.0. Rust-Standardbibliotheks-Hinweise ebenfalls enthalten. CANs GPL-3.0-or-later-Text, vorhandene NuGet-/Desktop-Runtime-Bewertung und libmpv-Hinweise bleiben erhalten. Keine pauschale Neubewertung anderer Paketformen.

Die unveränderten libmpv-Quellen-/Relink-Archive aus RC2 sind im Paketsatz enthalten und erneut gegen die bekannten SHA-256-Werte geprüft.

Die Motor-EXE importiert VCRUNTIME140.dll sowie Windows-/UCRT-Bibliotheken. Für den Motor ist deshalb Microsoft Visual C++ v14 Redistributable x64 separat erforderlich. [Microsofts Laufzeit-/Downloadhinweise](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist) und der [offizielle x64-Download](https://aka.ms/vc14/vc_redist.x64.exe) sind in den Einrichtungstexten verlinkt. Die Laufzeit wird nicht mitverteilt; reine Cutlist-Erstellung benötigt den Motor nicht.

## Automatische und lokale Prüfungen

- Gesperrter Windows-x64-Restore aus dem Quellarchiv erfolgreich.
- Vollständiger Release-Build und Publish: 0 Warnungen, 0 Fehler; 785/785 CAN-Tests, kein übersprungener Test. Core 74, Cutlists 65, Media 125, App 521.
- Motorquellen formatiert, offline mit Cargo.lock neu gebaut und 40/40 Rust-Tests bestanden. Der Kontroll-Neubau ist nicht bytegleich zur beigelegten ursprünglichen EXE (Kontroll-SHA-256 9177D391ACAE372A654E58D85A43810AB8169B3D21DC514A3D8106788268E675); keine bytegenaue Reproduzierbarkeit behauptet und der aktive Motor nicht ersetzt.
- 249 Dateien im frisch entpackten Portable-ZIP vollständig gegen PUBLISH-FILES.sha256 geprüft, keine Zusatzdateien. Quellen-/Motorprovenienz und vier native DLLs geprüft. Keine verbotenen gebündelten .NET-Laufzeitdateien oder separat zu installierenden Werkzeuge gefunden.
- Mit tatsächlich entpackten CAN-Assemblies und mitgeliefertem Motor ein künstlicher Zwei-Bereich-Schnitt: 90 tatsächlich decodierte Bilder, AAC-Stereo und AC-3/5.1, genau eine Indexierung und ein Motoraufruf, Original-/Indexdaten unverändert und Arbeitsordner bereinigt. Lauf rund 2,7 s; kein Realfilm-Benchmark. Keine persönlichen Einstellungen geschrieben.
- Isolierter WPF-Prüfhelfer mit entpackten Assemblies und echten libmpv-DLLs: Hauptfenster, Titel Build 12 RC3, native Playerinitialisierung und AVI-Hinweis geprüft; reguläres asynchrones Schließen bestanden, nur isolierte Fensterdaten gespeichert. Prüfbild gesichtet. Dies prüft die entpackten Komponenten, nicht den unveränderten CAN-Apphost-Start auf einem frischen Rechner.
- Inno-Setup erfolgreich erstellt; Installation und CAN-Schnittablauf mit MP4Box und otr-can am 11.10.2026 vom Nutzer in einer Windows-11-VM bestätigt. Deinstallation und anschließender separater Portable-Test dieses Pakets vom Nutzer als erfolgreich bestätigt; persönliche Bild-/Tonprüfung positiv.

Erster Paketsatz wegen alter Buildnummer im Anleitungstext als release-build12-RC3-first erhalten, Anleitung korrigiert und endgültiger Satz erneut aus passendem Quellarchiv gebaut. Eine Wiederholung stoppte korrekt wegen eines schon vorhandenen Quellarchivs; ursprüngliches Archiv erhalten und neues Archiv erstellt. Der erste WPF-Prüfhelfer lief über dotnet und fand dadurch native Abhängigkeiten nicht; Aufruf über seine eigene Windows-EXE korrigiert, erfolgreich wiederholt und ausschließlich den verbliebenen ersten Prüfprozess beendet. Keine Produktkorrektur oder Testentfernung zur Umgehung dieser Helferfehler.

## Persönliche Rückmeldung: Überinstallation

Am 11.10.2026 bestätigt der Nutzer die Installation über die vorhandene Ausgabe: **„Drüber installieren : check 0.2,1, 12, RC3“**. Damit sind die Überinstallation und die vom Nutzer abgelesene Versionsanzeige **0.2.1 · Build 12 · RC3** positiv bestätigt. Der Nutzer kündigt anschließend den VM-Test an; für diesen liegt aus dieser Rückmeldung noch kein Ergebnis vor. Film-/Tonprüfung, Deinstallation und separater Portable-Test werden weiterhin getrennt festgehalten.

## Windows-11-VM: Installation und beide Schnittmotoren bestätigt

Am 11.10.2026 bestätigt der Nutzer auf einer **Windows-11-VM**: **Installation, CAN-Werkzeugeinrichtung, Video laden, Cutlist laden, MP4Box und otr-can jeweils „check“**. Damit sind die persönliche VM-Installation und der CAN-Ablauf mit beiden Motoren positiv zurückgemeldet.

Das beigefügte otr-can-Protokoll betrifft **Grey’s Anatomy – Durchs Feuer (HQ)**, Aufnahme vom 30.09.2026. Es nennt Motor **0.1.4**, genau eine FFMS2-Indexierung sowie drei Behalteintervalle **592,560–1759,160 s**, **2314,200–3005,480 s** und **3565,520–4133,680 s**. Video-Schnittkanten werden auf CPU kodiert, geeignete IDR-Abschnitte kopiert.

Geprüft laut Protokoll: **60651 Video-Pakete** und rohe PTS/DTS; eine **AAC-Stereospur, 48 kHz**, Zielbitrate **165457 bit/s**, mit **116449920 decodierten Samples** und kontinuierlichen rohen Audio-PTS. Finale MP4-Pakete, Zeitstempel, Padding und Decoder-Konfigurationen geprüft; nativer Schnitt und anschließende CAN-Ausgabeprüfung erfolgreich abgeschlossen. Der MP4Box-Erfolg ist durch den persönlichen „check“ bestätigt; für diesen Lauf wurde hier kein gesondertes MP4Box-Protokoll mitgeliefert.

Die Summe der Keep-Dauern, Video-Pakete bei 25 fps und Audio-Samplezahl ergeben übereinstimmend nominal **2426,040 s / 40:26.040**. Rechnerische Kontrollübergänge im Ergebnis: **19:26.600** und **30:57.880**. Werte aus dem übermittelten Protokoll, keine neue unabhängige Prüfung der tatsächlichen VM-Datei oder zusätzlicher vollständiger Video-Decoderlauf. Keine Laufzeit mitgeteilt.

Anschließend bestätigt der Nutzer **„ton und bild check“**. Damit sind die geprüften VM-Schnittergebnisse auch persönlich in Bild und Ton positiv bewertet. Danach bestätigt der Nutzer **„deinstallation check“**: Die Deinstallation der Setup-Ausgabe in der Windows-11-VM ist ebenfalls erfolgreich zurückgemeldet. Der anschließende separate Portable-Test wird im folgenden Abschnitt ebenfalls positiv bestätigt. Die HQ-Datei enthält laut otr-can-Protokoll eine Tonspur; daraus folgt keine neue Mehrtonspur-Abnahme.

## Separater Portable-ZIP-Test persönlich bestätigt

Nach der erfolgreichen Setup-Deinstallation kündigt der Nutzer ausdrücklich **„jetzt zip test :-)“** an und bestätigt anschließend am 11.10.2026 **„bei zip auch alles check“**. Dazu nennt er **otr-can** und **„Durchs Feuer“**. Die Rückmeldung bezieht sich auf den zuvor beschriebenen separaten Portable-Ablauf mit beiden Schnittmotoren und Bild-/Tonprüfung. Damit ist auch die persönliche Portable-Paketprüfung positiv abgeschlossen.

Für diesen ZIP-Lauf wurden in dieser Nachricht kein gesondertes Schnittprotokoll, keine Laufzeit und keine Datei-/Hashdaten mitgeliefert. Die oben dokumentierten 60651 Pakete und 116449920 Audio-Samples stammen aus dem separaten Setup-/VM-Protokoll und werden nicht als neue Messung dieses Portable-Laufs ausgegeben.

Die persönliche Paketabnahme des lokalen **0.2.1 · Build 12 · RC3**-Satzes ist damit abgeschlossen: Überinstallation/Versionsanzeige, Windows-11-VM-Installation und Werkzeugeinrichtung, Laden von Video/Cutlist, beide Schnittmotoren, positive Bild-/Tonprüfung, Deinstallation und separater ZIP-Test bestätigt. Die bestehenden technischen Format-/Tonspurgrenzen bleiben erhalten; eine allgemeine Unterstützung beliebiger Medien wird daraus nicht abgeleitet. Der Paketsatz ist weiterhin unveröffentlicht.

## Persönliche Prüfung dieses Kandidaten

1. Setup in der Windows-11-x64-Test-VM installieren und CAN über die angelegte Verknüpfung starten. Titel 0.2.1 Build 12 RC3 und eingerichtete Werkzeugpfade prüfen.
2. MP4 abspielen, Cutlist laden und mit MP4Box schneiden; Anfang, Übergänge, Ende und Ton prüfen.
3. Den beigelegten tools/otr-can/otr_can.exe auswählen, FFMS2/ffmsindex und FFmpeg/ffprobe einrichten; gegebenenfalls Microsoft Visual C++ x64-Laufzeit installieren. Einen nativen Schnitt prüfen, bei mehreren Spuren jede einzeln abhören.
4. Dialoggröße ändern, erneut öffnen und nach CAN-Neustart prüfen. Mit einer geeigneten AVI-Aufnahme den gelben Hinweis prüfen; die ausgeschlossene untaugliche alte Aufnahme nicht untersuchen.
5. Setup deinstallieren. Danach Portable-ZIP in einen eigenen vollständigen Ordner entpacken, direkt dessen EXE starten und den Schnitt-/Wiedergabeablauf gesondert prüfen. Auf einem frischen Profil keine vorausgesetzten alten CAN-Einstellungen verwenden.

Persönliche Rückmeldungen anschließend mit Paketkennung festhalten. Keine neue 1.0-Version, kein Tag, kein Commit/Push und keine Veröffentlichung im Rahmen dieser Vorbereitung.

## Reproduzierbarer lokaler Paketbau

Das CAN-Quellarchiv vollständig entpacken. Den vier-DLL-libmpv-Ordner gemäß dessen Dokumentation bereitstellen, den Motorunterordner aus dem Portable-ZIP als Motorpaket verwenden und Inno Setup bereitstellen. Aus dem entpackten CAN-Quellordner:

```powershell
.\tools\package-release.ps1 -BuildNumber 12 -Candidate RC3 `
    -NativeSourceDirectory '<vier-DLL-Ordner>' `
    -SourceArchivePath '<CAN-Quell-ZIP>' `
    -OutputDirectory '<neuer Ausgabeordner>' `
    -InnoCompilerPath '<ISCC.exe>' `
    -OtrCanPackageDirectory '<tools\otr-can aus dem Portable-ZIP>'
```

Das Skript prüft Quell-/Motor-Manifeste, führt Restore/Build/Tests/Publish durch, prüft Paketgrenzen und erzeugt ZIP und Setup. Dieser nach dem endgültigen Paketbau erstellte Bericht ist nicht Teil des unveränderten Quellarchivs; er beschreibt die dazugehörigen Ergebnisse. Lokale Belege: artifacts/release-build12-RC3/logs, test-results, PUBLISH-FILES.sha256, .build/release-build12-RC3/portable-startup-proof und portable-cut-test.

## Pakete und Prüfsummen

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-0.2.1-Build12-source-RC3.zip | 2804568 | F1EC46978B7A5EDABB425CD6651AD71DC820A68B71FAB5DFB2F3FBE40EE5301D |
| CutAssistantNext-0.2.1-Build12-win-x64-RC3.zip | 24880671 | 48776B943CEBCDE915B56BA4D5D39E0F2EE14B57E09CEB6384338593264E3E1B |
| CutAssistantNext-0.2.1-Build12-win-x64-Setup-RC3.exe | 20213934 | 80B344BD05EEF0F24F2A32BBE276CD32A7C7FF134325129D5411EADD86638B35 |
| otr-can-0.1.4-source.zip | 61927 | B92DE748E1C5469AF2C28E5C789BDF417A333C0FC9129D73927C79DAF369508B |
