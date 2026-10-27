# AAC-Notfallweg nach otr-can-Tonfehler

Stand: **26.10.2026**. Seit **Build 37** in den lokalen Setup-/Portable-/Quellpaketen enthalten und im realen CAN-Ablauf persönlich bestätigt; im aktuellen CAN 1.0.2 Build 40 weiterhin enthalten. Motor **otr-can 0.1.6** bleibt unverändert. Keine Veröffentlichung.

## Einordnung im aktuellen Klebecenter

Der automatische Notfallweg bleibt beim Schneiden erhalten. Fertige Teile werden seit Build 39 im Klebecenter verbunden, unverändert in Build 40: zuerst Dateien/Reihenfolge, danach Tonspur je Teil. Keine separate Vorbereitungskopie erforderlich. Persönlich bestätigt: AC-3/AAC und AAC/AAC. [Bedienung](KLEBECENTER-20261027.md), [Versionen](VERSIONEN.md).

## Anlass und Bedienung

John Wick Kapitel 4 Teil 2 wird beim regulären otr-can-Schnitt wegen eines AC-3-Decoderfehlers abgelehnt. Der Fehler liegt vor dem zweiten Behaltebereich; bloßes Ignorieren der Warnung beweist keine korrekte Tonzeitachse. Der Nutzer wählt deshalb einen ausdrücklich bestätigten Notfallweg mit der intakten AAC-Spur.

CAN versucht weiterhin zuerst den normalen Schnitt mit allen Tonspuren. Nur bei einer erkannten PCM-Aufbereitungsstörung und mindestens zwei Originaltonspuren mit einer zuordenbaren AAC-Spur erscheint die neue Rückfrage. Sie nennt Sprache, Titel, Kanalzahl und Samplerate der verfügbaren AAC-Spuren und zeigt alle entfallenden Spuren an. Eine einzige Kandidatenspur wird vorbelegt; bei mehreren muss der Nutzer wählen. Erst „Mit dieser AAC-Spur schneiden“ erlaubt den erneuten Versuch. Abbrechen behält den ursprünglichen Fehler und erzeugt keine Notfallausgabe.

Die ausgewählte AAC-Spur muss über die gesamte Aufnahme ohne Decoderfehler oder Warnung dekodierbar sein. Auch ein Fehler in einem später entfernten Bereich dieser gewählten Spur führt weiterhin zum Abbruch. Die Prüfung repariert keinen defekten Ton.

## Umsetzung und Dateischutz

- OtrCanCutService meldet die betreffende native Audiostörung mit einem eigenen Fehlertyp. Im Normalfall verlangt die Ausgabeprüfung weiterhin sämtliche Originaltonspuren.
- OtrCanAacEmergencyCutService koordiniert Rückfrage, AAC-Prüfung, private Arbeitskopie und genau einen erneuten Schnitt. Nur die ausdrücklich gewählte AAC-Spur bildet die erwartete Audiostruktur des zweiten Versuchs; weitere Notfallversuche sind ausgeschlossen.
- FfprobeStreamCopyVerifier vergleicht sämtliche Video- und AAC-Pakete der vorbereiteten Arbeitskopie: Anzahl, Hash, Größe, Flags, PTS/DTS, Dauer, Skip-Samples/Discard-Padding, weitere ausgewiesene Side-Data sowie Codec-Konfiguration. Verschiedene Zeitbasen werden ohne Rundung mit ganzzahliger Bruchrechnung verglichen. Die Vorbereitung verwendet Streamcopy.
- Der unveränderte Motor indexiert und schneidet die geprüfte AAC-Arbeitskopie mit den ursprünglichen Behaltezeiten. Sein reguläres vollständiges AAC-Encoding und sämtliche native Ausgabeprüfungen bleiben aktiv.
- Sprache und Titel werden aus ffprobe eingelesen. Der Spurverlust erscheint im Fortschrittsprotokoll, im Abschlussfenster und als editierbarer Kommentar beim anschließenden Erzeugen der Cutlist.
- Originale, vorhandene Filme und eine vorhandene Zieldatei bei erfolglosem oder abgebrochenem Versuch bleiben erhalten. Private Arbeitsdateien werden bereinigt. Laufende Werkzeuge und der Auswahlvorgang unterstützen Abbruch.

Keine neue Produktionsabhängigkeit; keine Änderung an installierten Werkzeugpfaden, Motor-EXE, Buildzähler oder eingefrorenen Build-36-Paketen.

## Prüfungen

Release-Build ohne Warnungen/Fehler; **1429/1429 Solution-Tests** bestanden: 200 Core, 72 Cutlists, 249 Media und 908 App. Formatprüfung und Git-Whitespaceprüfung bestanden.

Die neuen Tests decken ausdrückliche Auswahl, Ablehnung, Abbruch, gesunde Aufträge, andere Fehler, Dateierhaltung, Aufräumen, mehrere AAC-Sprachen/Titel, weiter strenge Normalprüfung, Warnungsablehnung und exakte Paketzeitvergleiche ab.

Vier native Fälle mit künstlich erzeugtem H.264/AAC/AC-3-Material und dem unveränderten Motor 0.1.6 bestanden:

1. Gesundes Material: reguläre Ausgabe mit beiden Tonspuren, keine Notfallfrage.
2. Defektes AC-3-Paket im entfernten Bereich: normaler Schnitt scheitert, bestätigte Testauswahl liefert genau eine AAC-Spur.
3. Derselbe Fehler, Auswahl abgelehnt: kein Ergebnis und keine verbliebenen Arbeitsdateien.
4. Zusätzlich beschädigtes AAC: Notfallprüfung bricht ab, kein Ergebnis.

Die Testauswahl wurde im Prüfprogramm ausdrücklich simuliert; dies ersetzt keine persönliche Dialogabnahme. Die Originaldateien der vier Fälle blieben anhand ihrer SHA-256 unverändert. Gesunde und Notfallausgabe haben jeweils **351360 dekodierte AAC-Samples**; das vollständige dekodierte PCM ist bytegleich, SHA-256 080743b494f4f06ab1a01f8e0748fc9002a46c3479a8ee4d85c366974374a3d7. Der neue WPF-Dialog wurde separat gerendert und auf lesbaren Text und vollständige Schaltflächen geprüft.

## John Wick: zwei passende Prüfdateien

**Teil 2:** Der bereits nachgewiesene native AC-3-Fehler wurde im Prüfprogramm als Einstieg wiederverwendet, um einen weiteren vollständigen, zwangsläufig erfolglosen ersten Videoschnitt zu vermeiden. Anschließend liefen die neue strenge AAC-Prüfung und vollständige Paketprüfung sowie echter neuer FFMS2-/otr-can-0.1.6-Schnitt durch die regulären CAN-Dienste erfolgreich.

- Behaltebereiche [2583.12,3509.36) und [4299.52,5845.32).
- **61801 Videopakete**, **118657920 AAC-Samples**, **2472.04 Sekunden**.
- Native Prüfung bestätigt kontinuierliche Audio-PTS und unveränderte Video-/Audiopakete, PTS/DTS, Padding und Decoderkonfiguration beim abschließenden MP4-Zusammenbau.
- Gesamter Prüfprozess einschließlich Kindprozessen unter Windows-Job-Grenzen: 2 GiB, 25 Prozent CPU, 900 Sekunden; erfolgreich nach rund 93 Sekunden, maximal rund 1.28 GiB Commit.

**Teil 1:** Aus dem bereits erfolgreich geschnittenen Film wurde eine zusätzliche AAC-Kopie erstellt. Kein neuer Videoschnitt und keine neue Tonkodierung. Sämtliche Bild- und AAC-Pakete einschließlich Zeitstempeln und Konfiguration wurden vollständig gegen die vorhandene Ausgabe verglichen. Laufzeit **7057.12 Sekunden**.

Beide neuen Dateien haben H.264 High, 1920×1080, 25 fps, Videozeitbasis 1/12800 sowie genau eine AAC-LC-Stereospur, 48000 Hz, Audiozeitbasis 1/48000. Die jeweiligen Video- und Audio-Extradata-Hashes stimmen zwischen den Teilen überein. Damit ist die unterschiedliche Tonspuranzahl als Hindernis beseitigt. **Die neue Teil-2-Ausgabe und anschließend auch der vollständige automatische Rückfrage-/Schnittablauf nach Bereitstellung von Build 37 wurden vom Nutzer persönlich bestätigt.** Die passende Teil-1-Kopie wurde danach mit Build 38 persönlich erzeugt. Beide Teile sind inzwischen separat mit FFmpeg zusammengefügt; der Nutzer bestätigt den sauberen Übergang. [Referenzfall](JOHN-WICK-4-KLEBECENTER-REFERENZ-20261026.md).

Lokale Nachweise und Prüfdateien liegen im ignorierten Ordner **.build/aac-emergency-20261026/**, darunter native Testprotokolle, Paketvergleich, Dialogdarstellung, Ressourcennachweis und **john-wick-part1/** beziehungsweise **john-wick-part2/**. Reale Medien werden nicht in Git aufgenommen.

## Abgrenzung zum AC-3-Versuch

Die getrennte Untersuchung in **../otr-native-poc/experiments/audio-discard-errors/** wurde nicht in den regulären Motor übernommen. Künstliche Fälle waren erfolgreich; bei der echten Aufnahme wichen trotz passender Samplezahl und Zeitstempel zwei PCM-Blöcke gegenüber der kontinuierlichen Referenz ab. Diese Alternative gilt deshalb als nicht bestanden. Der AAC-Notfallweg verwendet sie nicht.

Build 37 und der Nachfolger Build 38 sind lokal paketiert. Der automatische Rückfrage-/Prüf-/Schnittablauf ist persönlich bestätigt; separate neue Portable-/VM-/Deinstallationsfälle und persönliche Abbruch-/Fehlerfallprüfungen bleiben offen. Keine Veröffentlichung ohne ausdrückliche Freigabe.

## Persönlicher vollständiger CAN-Ablauf – Nachtrag vom 26.10.2026

Nach Bereitstellung von Build 37 liefert der Nutzer das vollständige Protokoll und den Screenshot des Abschlussfensters: Der normale Versuch scheitert erneut am ursprünglichen AC-3-Decoderfehler, anschließend wird die AAC-Auswahl ausdrücklich bestätigt. Strenge AAC-Prüfung, geprüfte Arbeitskopie und erneuter nativer Schnitt laufen erfolgreich durch. Das Protokoll bestätigt **61801 Videopakete**, **118657920 Tonsamples** und die finale native Ausgabeprüfung. Im Abschlussfenster stehen die ausgewählte AAC-Spur und die entfallene AC-3-Spur korrekt. Nutzerbewertung: **„alles top“**.

Die simulierte Auswahl im vorangegangenen technischen Test und dieser nachfolgende persönliche Bedienlauf sind getrennte Nachweise. Es wird daraus keine zusätzliche frische VM-/Portable-Installation oder persönliche Abbruchprüfung abgeleitet. Die gemeinsame Kopiervorbereitung wurde in Build 38 um den exakten Wiedergabezeitvergleich erweitert und mit dem automatischen Ablauf erneut nativ geprüft. [Aktueller Stand](AKTUELLER-STAND.md).
