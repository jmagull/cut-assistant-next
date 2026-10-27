# CAN 1.0.2 · Build 39 – lokaler Paketbericht

**Stand 27.10.2026: erstellt, technisch geprüft und eingefroren; nicht veröffentlicht.** Setup nicht automatisch installiert. Der eigene Klebecenter-Funktionstest wurde vom Nutzer vor der Paketierung positiv bestätigt. Neue persönliche Setup-/Portable-/VM-/Deinstallationsfälle bleiben gesondert offen.

## Inhalt und Herkunft

**Klebecenter → Filmteile zusammenfügen …** ersetzt im neuen Stand das Menü „Tonspuren“. Zwei bereits geschnittene MP4-Dateien direkt im Klebecenter auswählen, zuerst Reihenfolge festlegen, danach eine Tonspur je Teil wählen. Keine zusätzliche Einzelkopie als Vorbereitung und keine erneute Cutlist-Anwendung.

Bild bleibt unverändert; gewählter Ton wird streng und vollständig geprüft, samplegenau verbunden und einmal durchgehend als AAC-LC kodiert. Quellton-Zeitstempel, sämtliches Videopaketmaterial und die Bildzeitachse, AAC-Konfiguration, Auffüllung, Paketidentität beim Umpacken, Tonsamplezahl und Klebestellendekodierung werden geprüft. Der bestehende Kontrollplayer verwendet die tatsächliche Klebestelle mit drei Sekunden Vorlauf.

Enthalten bleiben der ausdrückliche AAC-Notfallweg und korrekte Schnittmotor-Cutlist-Metadaten. Unverändert: **otr-can 0.1.6**, **libmpv 0.41.0**, .NET 10/WPF. Keine neue Produktionsabhängigkeit, keine Installation oder persönliche Werkzeugpfade durch den Paketauftrag verändert.

Gebaut aus festem Quellsnapshot: Basiscommit **1631aa7367a328bf86fd291a54a6c62f8731626f** plus in **SOURCE-SNAPSHOT.json** genannte Arbeitsänderungen. **IncludesWorkingTreeChanges=true**; Produktänderungen weiter uncommittet und ungepusht. Vier vorbestehende lokale Produktions-Lockänderungen wurden erhalten und ausgeschlossen; Snapshot verwendet die exakten Git-Lockdateien.

## Technische Prüfung

- Gesperrter Restore, Release-Build, Publish und Inno-Setup-Kompilierung aus dem festen Snapshot erfolgreich. **0 Warnungen / 0 Fehler**.
- **1485/1485 Solution-Tests**, keine übersprungen: 200 Core, 72 Cutlists, 288 Media, 925 App.
- Zusätzlich **925 App- und 288 Medientests**, insgesamt **1213**, gegen bytegleich geprüfte finale Produkt-DLLs.
- **21 native Klebecenter-Fälle** durch finale Paketbibliotheken bestanden: Reihenfolge, unterschiedliche Videouhren, bewusste zweite Spur, Format-/Spurfehler, beschädigter ausgewählter oder entfallender Ton, Abbruch, vorhandenes Ziel/Zieldatei-Rennen, Sampleabweichung, Warnung, Paket-/Videodatenveränderung, Mono/Stereo/5.1, 44,1 kHz und 50 fps.
- Zusätzlich **zehn bisherige native AAC-Notfall-/Tonspurkopiefälle** gegen finale Paketbibliotheken und beigefügten Motor bestanden.
- Finaler Klebecenter-Dialog sowie beide bisherigen Dialoge gerendert. Klebecenter: keine Vorauswahl bei mehreren Spuren, Aktivierung erst nach Wahl beider Spuren, Tausch hält Auswahl bei zugehöriger Datei; visuell geprüft.
- **658 Quelldateien** im Manifest, **659 Quellen-ZIP-Dateien** vollständig bytegleich geprüft.
- **250 Portable-Dateien** vollständig gegen Publish-Manifest und ZIP geprüft; keine unerwartete Laufzeit oder Schnittwerkzeuge beigefügt.
- Apphost-Dateiversion **1.0.2.39**; Runtime-/Lizenz-/Motorherkunft durch Paketprüfung bestätigt.
- **39 vorbestehende Dateien** vor Übernahme des neuen Zählers unverändert: frühere Build-35-/36-/37-/38-Paketdateien, vier lokale Lockdateien, Version.props, Motor und bisheriger Zähler. Nach allen erfolgreichen Prüfungen einzige beabsichtigte Änderung daran: Zähler **38 → 39**.
- Sechs Nutzdateien in **SHA256SUMS.txt**, acht oberste Paketdateien schreibgeschützt.

## Dateien

Ordner **artifacts/release-1.0.2-build39**.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-1.0.2-Build39-source.zip | 3479909 | 8E9E5A74144705A4C3C66A1336CBCBA954E3DCBFF1A5258E805B78BACBB5804E |
| CutAssistantNext-1.0.2-Build39-win-x64.zip | 25169521 | B450688B6A811C4417E7EBBBB03B8A32021EC79BAB43007CA52C7D3B11D382F0 |
| CutAssistantNext-1.0.2-Build39-win-x64-Setup.exe | 20452776 | 3D02DEB88033D44932A035039E1340654C9AE534369FF5F9655649951D07AE00 |
| otr-can-0.1.6-source.zip | 71900 | B10A984080CD6FA4C6C21274CB1E740797B84DF6B66859B21CE2A6E157B3040F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

## Persönlicher Funktionstest

Der Nutzer wählte **John Wick Kapitel 4 Teil 1 otr-can.mp4**, **Spur 2 AC-3/Deutsch/Stereo/48000 Hz**, und **John Wick Kapitel 4 Teil 2 mit Audio Korrektur.mp4**, **Spur 1 AAC/Deutsch/Stereo/48000 Hz**. Eigene Ausgabe **John Wick Kapitel 4 new.mp4**. Vollständiges Erfolgsprotokoll, Screenshot des Prüfdialogs, kopierte Kontrollstellen und anschließend **„Alles super check“** bestätigen den Bedienweg und Bild/Ton/Übergang. **457399680 Tonsamples**, Gesamtdauer **02:38:49.160**, Klebestelle **01:57:37.120**. Die technische AAC/AAC-Referenz umfasst außerdem 238229 unveränderte Videopakete.

Diese persönliche Rückmeldung gilt für den vor der Paketierung bereitgestellten Entwicklungsstand. Eine neue Paketinstallation, VM-/Portable-/Deinstallationsprüfung oder sämtliche persönlichen Abbruch-/Fehlerfälle wird daraus nicht abgeleitet. Setup wird nicht automatisch gestartet.

## Grenzen, Quellen und Nachweise

V1: genau zwei passende progressive H.264-MP4-Dateien mit konstanten 25/50 Bildern/s, vollständigen gleich langen Audio-/Videozeiträumen ab null und passenden AAC-/AC-3-Spuren. Strenge Ablehnung nicht bestätigbarer Zeiten oder Decoderwarnungen. Keine Reparatur, Abtastratenwandlung oder Video-Neukodierung. Ausreichender Platz für PCM und Prüfdateien erforderlich. Ursache früherer vollständiger Furiosa-Rechnerstillstände weiterhin ungeklärt; keine behauptete Behebung.

Lokale Paketnachweise: **.build/release-1.0.2-build39-20261027/** mit PACKAGE-INPUTS.json, PACKAGE-PROOF.json, FINAL.json, PERSONAL-ACCEPTANCE.json, final-package-results/, JOIN-NATIVE-RESULT.json, join-native/, FINAL-NATIVE.json, native-cases/ und Dialogdarstellungen. Paketbuild-/Test-/Installerlogs liegen im neuen Paketsatz. Der Bericht im Quellsnapshot beschreibt den Beginn der Paketierung; dieser Arbeitsbericht ergänzt nachträglich die abschließenden Prüfergebnisse. Eingefrorene Quellenarchive werden nicht verändert.

Keine Veröffentlichung, kein Tag/Upload und kein Commit/Push aus diesem Auftrag. Nächster Schritt: persönliche neue Setup-/Portable-Abnahme.

[Release Notes](RELEASE-NOTES-1.0.2.md), [Umfang](FREEZE-1.0.2-BUILD39-20261027.md), [Klebecenter](KLEBECENTER-20261027.md).

## Nachtrag: veralteter Voraussetzungstext erkannt

Der Nutzer entdeckt im Setuphinweis die Angabe otr-can 0.1.5. Die tatsächliche Motor-EXE dieses Pakets ist korrekt 0.1.6. Text und zusätzlicher Paketabgleich wurden im getrennten Build 40 korrigiert; Build-39-Archive bleiben unverändert. [Korrekturpaket](RELEASE-1.0.2-BUILD40-RESULT.md).

## Nachtrag: zweite Praxisrückmeldung und Build-40-Hinweis

Der Nutzer meldet einen zweiten vollständigen eigenen Klebecenter-Lauf mit **AAC-Spur 1 aus beiden Teilen**, Deutsch/Stereo/48000 Hz. Erfolgsprotokoll: 457399680 Samples, 02:38:49.160 Gesamtdauer und 01:57:37.120 Klebestelle. Rückmeldung **„Auch hier alles prima“**. Der Lauf liegt in der angekündigten Arbeits-PC-Setuprunde vor dem Wechsel zu Setup Build 40; das Protokoll enthält keine eigene Buildanzeige.

Ein weiterer Setup-Screenshot zeigt korrekt **„Optionaler Schnittmotor otr-can 0.1.6“**. Das bestätigt die sichtbare Textkorrektur, nicht den vollständigen Abschluss des neuen Setup-/Portable-/VM-Tests. Beide persönlichen Klebecenter-Kombinationen AC-3/AAC und AAC/AAC sind dokumentiert. [Aktueller Stand](AKTUELLER-STAND.md), [Versionszuordnung](VERSIONEN.md).
