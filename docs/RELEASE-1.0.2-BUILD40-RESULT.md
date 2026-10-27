# CAN 1.0.2 · Build 40 – korrigierter Voraussetzungstext

**Stand 27.10.2026: erstellt und technisch geprüft, nicht veröffentlicht.** Korrektur des Setup-/Portable-Hinweises; Anwendung und Motor funktional unverändert. Keine automatische Installation.

## Anlass und Änderung

Der Nutzer entdeckt im Setup-Abschlusshinweis von Build 39 die veraltete Zeile **„Optionaler Schnittmotor otr-can 0.1.5“**. Die beigelegte EXE meldet tatsächlich bereits **otr_can 0.1.6**. Der Versionsfehler steckt ausschließlich im Text **installer/HINWEIS-DOTNET-UND-WERKZEUGE.txt**, den die Paketierung als **VORAUSSETZUNGEN.txt** übernimmt und Inno Setup über **InfoAfterFile** anzeigt.

Der Text nennt nun **0.1.6**. Zusätzlich prüft **tools/package-release.ps1** diese dokumentierte Version gegen die erwartete Motorversion, bevor es den Motor separat anhand Herkunft, Hash und tatsächlicher **--version**-Ausgabe prüft. Die historisch korrekte Mindestversionsangabe „MP3-Unterstützung ab 0.1.5“ im Werkzeugprüfer bleibt bestehen.

## Technische Prüfung

- Neuer fester Quellsnapshot, gesperrter Restore, Release-Build und Publish, Inno-Setup-Kompilierung erfolgreich: **0 Warnungen / 0 Fehler**.
- **1485/1485 Pakettests**, keine übersprungenen Tests.
- Zusätzlich **925 App- und 288 Medientests**, insgesamt **1213**, gegen bytegleich geprüfte finale Produkt-DLLs bestanden.
- **361 Anwendungs- und Testquelldateien** vollständig bytegleich zu Build 39; keine Laufzeit-/Klebecenter-/Schnittmotoränderung. Die dortigen 21 nativen Klebecenter- und zehn bisherigen Notfall-/Kopienachweise gelten für diesen unveränderten Anwendungscode. Diese nativen Fälle wurden für die reine Text-/Paketprüfung nicht erneut ausgeführt.
- Tatsächlich ausgelieferter Hinweistext in Publish und Portable-ZIP bytegleich zum korrigierten Quelltext; enthält 0.1.6 und keine veraltete Motorangabe 0.1.5. Tatsächliche Motor-EXE **otr_can 0.1.6**, unveränderter SHA-256 **73DEB7D81C0D24241649B28D7ED924D3D64131E2EA84C3013198BBBDCB1CAE3E**.
- Apphost-Dateiversion **1.0.2.40**.
- **660 Quelldateien** im Manifest, **661 Quellen-ZIP-Dateien** und **250 Portable-Dateien** vollständig gegen Manifeste geprüft.
- Frühere Build-35-/36-/37-/38-/39-Paketdateien, vier lokale Lockänderungen, Version.props und Motor erhalten. 47 geschützte Dateien vor Zählerübernahme unverändert; danach allein beabsichtigte Zähleränderung **39 → 40**.
- Sechs Nutzdateien mit SHA-256, acht oberste Paketdateien schreibgeschützt.

## Dateien

Ordner **artifacts/release-1.0.2-build40**.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build40-source.zip | 3486237 | AD0D7C80CDD93C79F7FA3597F7C9AA06654450322C12D957EA41E81BA2F8DDDD |
| CutAssistantNext-1.0.2-Build40-win-x64.zip | 25169664 | CDE20386339420CDE3FFB81A949B645ADB2BADE7572AB04EC7782826A8BF29F7 |
| CutAssistantNext-1.0.2-Build40-win-x64-Setup.exe | 20448429 | 3C37194046CD88848DA401418E98CD48FA0EF78DA1342F6D365BAFB57D1AB654 |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

## Herkunft, Abnahme und Grenzen

Basiscommit **1631aa7367a328bf86fd291a54a6c62f8731626f** plus exakt dokumentierte Arbeitsänderungen, **IncludesWorkingTreeChanges=true**. Vier vorbestehende lokale Produktions-Lockänderungen im Arbeitsverzeichnis erhalten, aus dem Snapshot ausgeschlossen; verwendet wurden die exakten Git-Lockdateien. Funktional unverändert enthalten: Klebecenter aus Build 39, AAC-Notfallweg, Schnittmotor-Cutlist-Metadaten, otr-can 0.1.6 und libmpv 0.41.0. Keine neue Produktionsabhängigkeit.

Der persönliche John-Wick-Klebecenter-Lauf mit AC-3 aus Teil 1 und AAC aus Teil 2 samt Bild/Ton/Übergang bleibt bestätigt. Der Nutzer meldet den begonnenen Setup-Test auf dem Arbeits-PC; dessen Abschluss und neue Build-40-/Portable-/VM-Abnahmen werden nicht vorweggenommen. Keine Aussage, die früheren Furiosa-Rechnerstillstände seien behoben.

Lokale Nachweise: **.build/release-1.0.2-build40-20261027/** mit PACKAGE-INPUTS.json, PACKAGE-PROOF.json, CODE-UNCHANGED.json, FINAL.json und final-package-results/. Der Bericht im eingefrorenen Quellsnapshot dokumentiert den Paketierungsbeginn; dieser Arbeitsbericht ergänzt die tatsächlichen Endergebnisse.

Keine Veröffentlichung, kein Tag/Upload und kein Commit/Push. [Umfang](FREEZE-1.0.2-BUILD40-20261027.md), [Release Notes](RELEASE-NOTES-1.0.2.md), [bisheriger Funktionsnachweis](RELEASE-1.0.2-BUILD39-RESULT.md).

## Nachtrag: zweite Praxisrückmeldung und Build-40-Hinweis

Der Nutzer meldet einen zweiten vollständigen eigenen Klebecenter-Lauf mit **AAC-Spur 1 aus beiden Teilen**, Deutsch/Stereo/48000 Hz. Erfolgsprotokoll: 457399680 Samples, 02:38:49.160 Gesamtdauer und 01:57:37.120 Klebestelle. Rückmeldung **„Auch hier alles prima“**. Der Lauf liegt in der angekündigten Arbeits-PC-Setuprunde vor dem Wechsel zu Setup Build 40; das Protokoll enthält keine eigene Buildanzeige.

Ein weiterer Setup-Screenshot zeigt korrekt **„Optionaler Schnittmotor otr-can 0.1.6“**. Das bestätigt die sichtbare Textkorrektur, nicht den vollständigen Abschluss des neuen Setup-/Portable-/VM-Tests. Beide persönlichen Klebecenter-Kombinationen AC-3/AAC und AAC/AAC sind dokumentiert. [Aktueller Stand](AKTUELLER-STAND.md), [Versionszuordnung](VERSIONEN.md).

## Furiosa: großer Praxislauf in der Build-40-Setuprunde

Nach dem angekündigten Wechsel zu Setup Build 40 bestätigt der Nutzer beide otr-can-Schnitte von Furiosa Teil 1/2 sowie das Klebecenter und alle Prüfstellen. Beide gewählten Spuren AAC/Deutsch/Stereo/48000 Hz. Vollständiges Klebeprotokoll erfolgreich: **387448320 Tonsamples**, Dauer **02:14:31.840**, Klebestelle **01:54:18.320**, Vorlauf **01:54:15.320**. Damit ist der Arbeits-PC-Funktionslauf der Setup-Prüfrunde positiv gemeldet; zusätzliche VM-/Portable-/Deinstallationsvarianten werden daraus nicht abgeleitet. [Praxisbericht](FURIOSA-KLEBECENTER-PRAXIS-20261027.md).

## Nachtrag: VM-Setup-Prüfung läuft

Erster persönlicher Fall Die Teufelsbrigade AVI mit MP4Box erfolgreich: verlustfreie Arbeitsvorbereitung, sechs Schnittabschnitte, Ton/Schnitt/Bild/Drei-Sekunden-Vorschau positiv bestätigt. Nutzer testet weiter; keine vollständige VM-/Portable-/Deinstallationsabnahme. [Laufender Nachweis](VM-SETUP-BUILD40-20261027.md).

## Nachtrag: zweiter VM-Setup-Fall positiv

Die Teufelsbrigade AVI nun auch mit otr-can 0.1.6 erfolgreich; sechs Abschnitte, MP3→AAC-LC, 182574 Videopakete und 350542080 Tonsamples geprüft. Persönlich Ton/Bild/Schnitt/Drei-Sekunden-Vorschau bestätigt; Nutzer testet weiter. Beide Motorwege dieser AVI positiv, keine pauschale VM-Endabnahme. [VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## Nachtrag: dritter VM-Setup-Fall positiv

Grey’s Anatomy – Durchs Feuer als MP4 mit MP4Box in drei Abschnitten erfolgreich geschnitten und geprüft. Nutzer bestätigt Ton/Bild/Schnitt/Drei-Sekunden-Vorschau und testet weiter. [VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## Nachtrag: vier VM-Fälle positiv

Grey’s Anatomy MP4 auch mit otr-can 0.1.6 positiv: drei Abschnitte, 60651 Videopakete und 116449920 Tonsamples verifiziert. Ton/Schnitt/Bild/Drei-Sekunden-Vorschau persönlich bestätigt. AVI und MP4 damit je mit beiden Motoren positiv; Nutzer testet weiter, keine pauschale VM-Endabnahme. [VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## Nachtrag: fünfter VM-Setup-Fall positiv

Furiosa HD mit MP4Box: fünf Abschnitte und vollständige Abschnittsprüfung, Meldung zur Zuordnung kleiner Bildzeitüberlappung am ersten Übergang. Bild/Ton/Schnitt/Drei-Sekunden-Vorschau persönlich positiv; weitere VM-Prüfung läuft. [VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## Nachtrag: sechs VM-Setup-Fälle positiv

Furiosa HD auch mit otr-can 0.1.6 erfolgreich: fünf Abschnitte, 171458 Videopakete, AAC und AC-3 je mit 329199360 Tonsamples technisch geprüft. Bild/Schnitt/Ton/Drei-Sekunden-Vorschau persönlich positiv. AVI, HQ-MP4 und HD damit jeweils mit beiden Motoren bestätigt; Nutzer testet weiter. [VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## Nachtrag: siebter VM-Setup-Fall positiv

Furiosa Teil 2 HD mit MP4Box: zwei Abschnitte, beide geprüft, Abschluss „Fertig.“. Bild/Schnitt/Ton/Drei-Sekunden-Vorschau persönlich positiv, weitere VM-Prüfung läuft. [VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## Nachtrag: acht VM-Schnittfälle positiv

Furiosa Teil 2 auch mit otr-can 0.1.6 erfolgreich: zwei Abschnitte, 30338 Videopakete, AAC/AC-3 je mit 58248960 Tonsamples verifiziert. Bild/Schnitt/Ton/Drei-Sekunden-Vorschau persönlich positiv. Acht Schnittfälle mit beiden Motoren bestätigt; weitere VM-Prüfung läuft. [VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## Nachtrag: VM-Klebecenter positiv

Beide in der VM erzeugten Furiosa-otr-can-Teile mit AAC/AAC verbunden; vollständige technische Prüfung, 387448320 Samples, Gesamtdauer 02:14:31.840, Klebestelle 01:54:18.320. Bild/Ton/Klebestelle/Drei-Sekunden-Vorschau persönlich bestätigt. Neun Funktionstests im VM-Setup positiv, weitere Gesamt-/Portable-/Deinstallationsabnahme nicht daraus abgeleitet. [VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## Nachtrag: VM-Deinstallation bestätigt, Diagnoseordner bleibt

Nutzer bestätigt Deinstallation. Nach erfolgreichem Kleben bleibt ein .can-join-…-Ordner nur mit protocol.txt im Filmordner; dies entspricht der bisherigen bewussten Diagnoseaufbewahrung. Große Zwischenstände werden bereinigt. Erfolgs-Protokollbereinigung als Verbesserung vorgemerkt, noch nicht implementiert; bestehende Nutzerdateien nicht gelöscht. Neue Portable-Abnahme offen. [VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## Nachtrag: erster VM-Portable-Fall positiv

Nach Setup-Deinstallation zur ZIP-Prüfung gewechselt: Grey’s Anatomy MP4 mit MP4Box, drei Abschnitte, Bild/Ton/Schnitt/Drei-Sekunden-Vorschau positiv. Getrennter Portable-Nachweis; kein Gesamtabschluss oder frisches Profil daraus abgeleitet. [Portable-Bericht](VM-PORTABLE-BUILD40-20261027.md).

## Nachtrag: zweiter VM-Portable-Fall positiv

Teufelsbrigade AVI mit otr-can 0.1.6, sechs Abschnitte: Bild/Ton/Schnitt/Drei-Sekunden-Vorschau persönlich bestätigt. 182574 Videopakete und 350542080 Tonsamples, abschließende Paketprüfung erfolgreich. Hinweise der AVI-Vorbereitung im [Portable-Bericht](VM-PORTABLE-BUILD40-20261027.md) erhalten. Zwei Portable-Fälle bestätigt; Gesamtabschluss offen.

## Nachtrag: VM-Portable-Prüfung persönlich abgeschlossen

Dritter Fall Furiosa-Klebecenter AAC/AAC: technische Prüfungen erfolgreich, 387448320 Tonsamples, Klebestelle 01:54:18.320, Dauer 02:14:31.840. Bild/Ton/Klebestelle/Drei-Sekunden-Vorschau persönlich positiv. Nutzer schließt die drei Fälle umfassende ZIP-Runde ausdrücklich mit „damit zip check“ ab. [Portable-Bericht](VM-PORTABLE-BUILD40-20261027.md). Erfolgs-Protokollbereinigung weiterhin nur vorgemerkt; keine Veröffentlichung freigegeben.
