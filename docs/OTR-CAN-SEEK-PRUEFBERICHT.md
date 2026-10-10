# OTR-CAN – getrennte Seek-Versuchsvariante

## Übernahme als regulärer Motor 0.1.2

Nach persönlicher positiver CAN-Abnahme am 10.10.2026 vom Nutzer beauftragt. Die korrigierte Optimierung ist Standard des regulären **0.1.2**-Motors ohne Feature-Schalter. Experimental-Warnhinweis entfernt, neuer separater Build unter target/can-engine-0.1.2-build/release/otr_can.exe. CANs gespeicherter Motorpfad ist nach erfolgreicher Werkzeugprüfung auf diesen Build gestellt; Indexerpfad und übrige Einstellungen erhalten. MP4Box bleibt Vorgabe.

Rust formatiert/gebaut ohne Warnungen, **25/25 Tests**; CAN im bisherigen Ausgabeordner gebaut, **0 Warnungen/0 Fehler**, **743/743 Tests**. Synthetische 60-Sekunden-Datei mit frühem/spätem Keep-Bereich durch CANs vorhandene Motorfabrik/Schnittdienst: optimierter Seek aktiv, kein Experimental-Hinweis, eine Indexierung, zwei Tonspuren, bytegleiche CAN-04-Ausgabe und gleiche decodierte Bild-/Tondatensätze. AVI-Einzelteil weiterhin echtes MP4, Arbeitsbereiche bereinigt.

Vier erneut mit dem regulären Build erzeugte Ausgaben vollständig bytegleich zur geprüften Referenz: Pines **57,2 s**, Navy CIS **23,1 s**, Kimi **31,0 s**, HQ **24,8 s**, jeweils Indexierung plus Motorlauf. Die unten beschriebenen Messgrenzen gelten weiter. Cargo.lock ändert nur die eigene Paketversion. Alle bisherigen Referenz-/Test-EXEs, CAN-Code, acht Lockdateien, Buildzähler und Filme erhalten. Voriger Quellstand und Belege unter .build/otr-can-stable/FINAL-RESULT.json.

Reguläre Motor-EXE SHA-256: 75de8b5b8369ee7ab3b20409fdea94772e4d8ff8a7ea889c89fe1627aa3feda6. CAN bleibt **0.2.1 · Build 11 · RC3**; kein Commit, Push, Setup, Release oder neues Repository. Geerbte Zeitstempel-/Framevorbehalte bleiben erhalten.

## Historische Versuchsphase 0.1.1
Stand: 10.10.2026. Basis: nativer Motor 0.1.1. CAN bleibt **0.2.1 · Build 11 · RC3**. Der Versuch wird ausdrücklich über den Werkzeugpfad ausgewählt; der Nutzer hat ihn anschließend erfolgreich direkt in CAN getestet.

Der Nutzer meldete für The Place Beyond the Pines knapp 15 Minuten otr-can-Laufzeit bei gutem Schnittergebnis und gab nach einem weiteren HQ-Test eine getrennte Optimierungsvariante frei. Die korrigierte Versuchsvariante benötigt hier rund **61 Sekunden für Indexierung und Motorlauf** und erzeugt eine **bytegleiche MP4** zur persönlich geprüften Ausgabe. Das ist ein lokaler Messwert, kein kontrollierter Vergleichsbenchmark zur zuvor berichteten Nutzerlaufzeit.

## Änderung und Build

Im bestehenden Rust-Projekt wurde der opt-in Cargo-Schalter `experimental-local-seek` ergänzt. Ohne diesen Schalter bleibt die bisherige Argumentfolge einschließlich `-map 0` aktiv. Der Standard- und Experimentalmodus wurden jeweils mit 25 Rust-Tests geprüft; drei zusätzliche Fälle sichern frühe/Nullgrenzen sowie späte Mikrosekundengrenzen und die Eingangs-/Zeitoffsetreihenfolge. Formatierung und separater Release-Build erfolgreich, keine Rust-Warnungen. Keine neuen Abhängigkeiten oder Lockdateiänderungen.

Der erste Eingang behält den bisherigen Ton-Dekodierweg vom Originalanfang. Ein zweiter Eingang springt ausschließlich für Videostreams nahe an die Schnittkante: auf die abgerundete Schnittzeit minus 30 Sekunden, mindestens null. `-itsoffset` stellt dessen absolute Zeitleiste wieder her; der bisherige absolute Ausgangsseek, Kantendauer, Codec- und Audiobitratenparameter bleiben erhalten. Alle Streams werden in der bisherigen Reihenfolge zugeordnet. Frühe Kanten behalten den bisherigen Aufruf. Kopierte Mittelstücke, Frame-/Keyframeberechnung, Zusammenfügen, Index-/Dateiverwaltung und CPU-Encoding bleiben erhalten.

Die experimentelle EXE wurde unter `target/can-engine-seek-build/release/otr_can.exe` gebaut und separat als `target/can-engine-seek-build/testmotor/otr_can_seek_test_01.exe` gesichert. Beim Schnitt kennzeichnet eine Warnmeldung den Experimentalbuild. Seine CLI bleibt zur CAN-0.1.1-Schnittstelle kompatibel; die Versionsausgabe allein kennzeichnet den Feature-Build nicht. Aktive Werkzeugeinstellungen wurden nicht geändert.

CANs bestehende Werkzeugprüfung akzeptiert die separat benannte Test-EXE sowie Indexer, FFmpeg und ffprobe. Abschlussvergleich bestätigt 34 unveränderte Rust-/Referenzdateien und 439 unveränderte bestehende CAN-Dateien; die drei bisherigen CAN-Dokumente wurden gezielt ergänzt. Alle acht CAN-Lockdateien und der Buildzähler stimmen mit dem vorherigen Stand überein. SHA-256 der Test-EXE steht im lokalen `FINAL-RESULT.json` und in der Rust-Anleitung.

## Verworfenes erstes Experiment

Zunächst wurde der einzige FFmpeg-Eingang für Bild und Ton gemeinsam nahe an die Schnittkante verschoben. Beim Pines-Film erreichte diese Fassung rund 23 Sekunden einschließlich Indexierung. Alle 137.626 Videopakete waren mit dem bisherigen Ergebnis gleich, einschließlich Paketinhalt, PTS, DTS und Dauer. Beide Tonspuren hatten gleiche Paketanzahl und Zeitstempel, aber **329 AAC- und 708 AC-3-Paketinhalte** unterschieden sich. Deshalb wurde diese Fassung nicht übernommen. Die genaue Ursache innerhalb des Audio-Dekodierwegs wurde nicht bewiesen; die korrigierte Variante erhält dessen vollständige Vorgeschichte. Erste Fassung, Protokolle und Paketvergleiche liegen separat in den lokalen Belegen.

## Prüfung der korrigierten Variante

Synthetische 60-Sekunden-Datei mit H.264/25 fps, AAC und AC-3, drei Behaltebereichen und früher/später Kantenbearbeitung: Ausgabe bytegleich mit dem bestehenden Motor; decodierte Bild-/Tondaten aller drei Streams ebenfalls gleich. Nur der Video-Framehash-Prüfexport normalisiert seine PTS; die Filmdateien werden dabei nicht verändert.

| Aufnahme | Indexierung | Motorlauf | Summe | Vergleich zur bisherigen Ausgabe |
|---|---:|---:|---:|---|
| The Place Beyond the Pines, HD | 3,0 s | 57,8 s | **60,9 s** | ganze MP4 bytegleich |
| Navy CIS, HD | 1,8 s | 14,1 s | **15,8 s** | ganze MP4 bytegleich |
| Kimi, HD | 2,7 s | 35,5 s | **38,3 s** | ganze MP4 bytegleich |
| Wo dein Herz schlägt, HQ | 1,6 s | 29,1 s | **30,7 s** | ganze MP4 bytegleich |

Pro Auftrag genau eine FFMS2-Indexierung. Die neue EXE erhält dieselben vorhandenen Cutlists mit siebenstelligen CAN-Zeitangaben. Navy/Kimi werden mit den geprüften Schritt-6-Ausgaben verglichen, Pines/HQ mit den persönlich geprüften direkten CAN-Ausgaben des Nutzers. Keine alten Ausgaben überschrieben. Original- und Cutlisthashes vor/nach dem Auftrag gleich; Indexdateien unverändert, Schnittunterordner und Locks nach dem Motorlauf entfernt. Indexdateien bleiben absichtlich als lokale Prüfbelege erhalten.

Die Summen messen die beiden externen Prozesse. Vorheriges Hashen, spätere Hash-/Metadatenprüfung, GUI-Bedienung und CANs Ausgabeprüfung sind nicht enthalten. Keine exakten Beschleunigungsfaktoren gegenüber ungemessenen bisherigen Laufzeiten behauptet. Bei der ersten Versuchsfassung liefen teilweise Paketprüfungen anderer Dateien gleichzeitig; deren Zeiten sind keine kontrollierte Vergleichsbasis.

SHA-256 der korrigierten Ausgabe und jeweils ihrer bisherigen Referenz:

- Pines: `1a320cdca80a6932c4b192544a66a2ed2dbb51abb6b505fd7d6856ee51e50855`
- Navy CIS: `7aa2c16cdf35ddc25d76be920a6ef83db6d6bf4cd5e8a58699a1a7c4ca44509d`
- Kimi: `b127b0d9dff0058d91547b3b9165ff6f6fca51308ab7bb12e23e3665b3858726`
- HQ: `7fd85396d652505ad5e87838f0303a0c4aeb11b1de45e33fa1a156f7ac4e7d5b`

## Persönlicher Test direkt über CAN

Nach Auswahl der Test-EXE über CANs Werkzeugpfad hat der Nutzer den Pines-Schnitt direkt in der normalen Oberfläche durchgeführt. Das übermittelte Protokoll enthält den experimentellen Video-Seek mit unverändertem Tonweg, eine Indexierung, fünf abgeschlossene Behaltebereiche, Ausgabe-/Tonspurprüfung und erfolgreichen Abschluss. Anschließend bestätigt der Nutzer für `The Place Beyond the Pines otr-can_2 [08.10.2026].mp4` **„alles prima“** und gleiche Dateigröße zur vorherigen Ausgabe.

Nur lesender Vergleich bestätigt darüber hinaus vollständige Bytegleichheit der beiden direkt in CAN erstellten Dateien: jeweils **1.566.358.736 Bytes**, SHA-256 `1a320cdca80a6932c4b192544a66a2ed2dbb51abb6b505fd7d6856ee51e50855`, identisch auch mit dem isolierten korrigierten Versuch. Eine konkrete Laufzeit dieses persönlichen CAN-Laufs wurde nicht mitgeteilt. Nachweis in `.build/otr-can-seek/PINES-CAN-USER-COMPARISON.json`.

## Erhaltung und Grenzen

Die bisherigen CAN-01/02/03/04-/OTR-Referenz-EXEs, Standalone-0.1.0-/0.1.1-Builds und ursprünglichen Rust-PoC-Quellen bleiben erhalten. Nur Cargo-Feature, experimentelle Seek-/Streamzuordnung, CLI-Warnhinweis und Rust-Dokumentation wurden geändert. CAN-Produktionscode, acht Lockdateien einschließlich der vier bestehenden Nutzeränderungen, Buildzähler, Filme und Einstellungen bleiben erhalten. Kein Commit, Push, Repositorywechsel, Setup oder Release.

Die Bytegleichheit umfasst Bild, beide Tonspuren und Zeitstempel und erhält auch bekannte Referenzauffälligkeiten. Insbesondere werden die im [Schritt-6-Bericht](OTR-CAN-SCHRITT6-PRUEFBERICHT.md) dokumentierten PTS-Abweichungen und der bei Pines gemeldete zusätzliche Videoframe nicht behoben. Die vier isolierten Vergleiche erfolgten über die CLI; zusätzlich ist der direkte CAN-Pines-Test persönlich positiv bestätigt. Kein erneuter vollständiger Decoderlauf, keine getrennte persönliche Hörprüfung beider Tonspuren oder Releasefreigabe aus dieser Rückmeldung abgeleitet.

Quellsicherung, Tests, Buildprotokolle, beide synthetischen Versuche, alle realen Versuchsausgaben, Paketvergleiche und Prüfsummen liegen ausschließlich im ignorierten `.build/otr-can-seek/`. Die Rust-Anleitung steht in `can-engine/SEEK-EXPERIMENT.md`. Der persönliche Pines-Test der separat benannten EXE über CANs Werkzeugpfad ist erfolgreich abgeschlossen; ein allgemeiner Wechsel der Standard-Buildkonfiguration oder eine Veröffentlichung folgt daraus nicht automatisch.
