# Klebecenter – erste integrierte Fassung

**Stand 27.10.2026: seit Build 39 integriert, Bereinigung ergänzt in CAN 1.0.2 Build 41 mit Motor 0.1.6.** Persönlich bestätigt: AC-3/AAC und AAC/AAC samt Bild/Ton/Übergang. Build-40-Setuphinweis im Screenshot korrekt; Build-40-Setup-/ZIP-Prüfung persönlich abgeschlossen; persönliche Setup-Prüfung der neuen Build-41-Bereinigung auf dem Arbeitsrechner mit John Wick positiv: temporäre Datei weg, Bild/Ton/Schnitt/Drei-Sekunden-Vorschau und Klebecenter bestätigt. Neue ZIP-/VM-Prüfung separat offen. [Versionen](VERSIONEN.md).

## Bedienung

**Klebecenter → Filmteile zusammenfügen …** öffnet die gesamte Vorbereitung. Der bisherige Menüpunkt „Tonspuren“ samt zusätzlicher Einzelkopie wird dadurch ersetzt. Die Einzelkopie war eine Vorbereitung für das damals externe Kleben und wird für diesen Ablauf nicht mehr benötigt.

1. Zwei bereits geschnittene MP4-Dateien direkt im Klebecenter auswählen. Eine Datei muss nicht vorher im Hauptplayer geladen sein. Unterschiedliche Ordner sind möglich.
2. Reihenfolge festlegen; „Reihenfolge tauschen“ vertauscht Dateien samt zugehöriger Tonspurauswahl.
3. Je Filmteil eine Tonspur wählen. Bei mehreren Spuren gibt es keine Vorauswahl. Eine allein vorhandene Spur wird angezeigt und ausgewählt. Sprache, Codec, Kanalzahl und Abtastrate sowie entfallende Spuren sind sichtbar.
4. „Prüfen und zusammenfügen …“ prüft zunächst die Formate und fragt nach einer neuen MP4-Ausgabedatei. Bestehende Dateien werden auch bei einem Zieldatei-Rennen nicht ersetzt.
5. CAN prüft die Bildzeitstruktur und dekodiert ausschließlich die gewählten Tonspuren vollständig. Decoderwarnungen, Sampleabweichungen oder unpassende Zeitverhältnisse führen zum Abbruch.
6. Nach technischer Prüfung zeigt der vorhandene Abschlussdialog Anfang, Klebestelle und Ende. „Prüfen“ öffnet den bestehenden Kontrollplayer mit drei Sekunden Vorlauf vor der Klebestelle.

Die Dateien werden vollständig übernommen; Cutlists werden nicht erneut angewendet. Der Zustand im Hauptplayer bleibt erhalten. Die Auswahl ist keine dauerhafte globale Tonspurvoreinstellung.

## Erste Grenzen

Genau zwei MP4-Dateien mit je einer progressiven H.264-Videospur, identischer Decoderkonfiguration, Geometrie, Farbangaben und konstanten 25 oder 50 Bildern/s. Rotations-/sonstige Video-Stream-Zusatzdaten sind zunächst ausgeschlossen. Bildzeit null, lückenloses Bildraster und streng fortlaufende DTS sind erforderlich. Paket-PTS dürfen sich wegen B-Bildern in der Decodierreihenfolge umordnen; die Präsentationszeiten müssen das vollständige eindeutige Bildraster abdecken.

Wählbar sind AAC und AC-3 bei 44100 oder 48000 Hz, Mono, Stereo oder bekannter 5.1-Belegung. Beide gewählten Spuren müssen dieselbe Abtastrate und Kanalbelegung sowie exakt den vollständigen Bildzeitraum ab null haben. Es gibt keine automatische Reparatur, Zeitkorrektur, Abtastratenwandlung oder Video-Neukodierung. Nicht unterstützte Spuren können angezeigt werden; ihre Auswahl wird bei der Vorprüfung verständlich abgelehnt.

Der Ton wird **immer einmal durchgehend als AAC-LC neu kodiert**, für Mono/Stereo 192 kbit/s, für Mehrkanalton 384 kbit/s. Diese erste Fassung bietet kein unbewiesenes Zusammenkopieren einzelner AAC-Paketfolgen. Das Bild bleibt unverändert.

## Verarbeitung und Nachweis

- Getrennte Medien-/Verarbeitungslogik in **Mp4JoinService**, strenge Analyse/Streamingprüfung in **FfprobeStreamCopyVerifier.Join**, Oberfläche in **JoinDialog** und **MainWindow.Join**. Vorhandene Werkzeugpfade, Fortschrittsanzeige, Abschlussdialog und Kontrollplayer werden wiederverwendet. Keine neue Produktionsabhängigkeit.
- Quelldateien bleiben für die gesamte Verarbeitung gegen Schreiben gesperrt. Gewählte Audioindizes werden explizit verwendet; beschädigte entfallende Spuren werden nicht dekodiert. Die vollständige Quellton-Paketzeitachse muss lückenlos sein, einschließlich korrektem Anfang und Ende unter Berücksichtigung der AAC-Auffüllung; eine passende Samplezahl allein genügt nicht.
- Beide gewählten Tonspuren werden mit Fehlerabbruch als Float-PCM dekodiert. Ihre vollständige Samplezahl muss dem exakten Videouhr-Zeitraum entsprechen. PCM wird in gewählter Reihenfolge verbunden und vollständig per SHA-256 gegen die dabei gelesenen Ursprungsdaten verglichen. RF64 mit expliziter Kanalmaske vermeidet Größenbegrenzung und geschätzte Formatangaben.
- Video wird zunächst je Teil unverändert separat vorbereitet und vollständig geprüft. Unterschiedliche exakt vereinbare Zeitbasen werden dabei auf die erste Videouhr umgerechnet und rational geprüft. Das ist keine Änderung der Bildzeitpunkte.
- Zusammenbau mit expliziten Zeitstempeloptionen; anschließend vollständiger Vergleich aller Videopaket-Hashes, Größen, Flags, DTS/PTS und Dauern gegen beide Eingaben mit dem nachgewiesenen Versatz für Teil 2.
- AAC-LC-Konfiguration, Paketidentität beim letzten Umpacken, Priming/Endpadding, durchgehende Tonzeitachse und vollständige dekodierte Ausgabesamplezahl werden geprüft. Ältere und neuere ffprobe-Feldnamen für Skip-/Discard-Samples werden berücksichtigt; das schwächt die bisherige Tonspurkopieprüfung nicht ab.
- Sechs Sekunden um die Klebestelle werden streng dekodiert. Vor Übernahme der neuen Datei wird ihre SHA-256-Identität für den Kontrollplayer bestimmt. Erst danach erfolgt die Übernahme ohne Überschreiben.
- FFmpeg läuft im separaten Windows-Job mit 1 GiB Prozessspeicherlimit, 25 % CPU, niedrigerer Priorität und jeweils einem Verarbeitungsthread; Zweistunden-Zeitlimit pro Prozess. Das belegt keine Behebung der früheren Furiosa-Systemhänger.
- Freier Speicher wird konservativ einschließlich unkomprimiertem Ton und Prüfdateien geprüft. Große Zwischendateien werden nach Erfolg, Fehler oder Abbruch gezielt gelöscht. Ab Build 41 bleiben **protocol.txt** und der private **.can-join-…**-Ordner nur bei Fehler/Abbruch oder nicht vollständig möglicher Bereinigung erhalten. Nach Erfolg werden Protokoll und leerer Ordner entfernt. Das Fehlerprotokoll enthält Schritte und exakte Werkzeugargumente.

## Technische Prüfungen

Release-Build **0 Warnungen / 0 Fehler**, **1485/1485 Solution-Tests**: 200 Core, 72 Cutlists, 288 Media, 925 App. Neue Fälle prüfen exakte Zeitbasen, fehlende/abweichende Spuren, Formatgrenzen, Rotation, PCM-Reihenfolge, Abbruch, vorhandene Ziele und AAC-Padding-Feldnamen.

**21 native Klebecenter-Fälle** erfolgreich: Vorwärts-/Rückwärtsreihenfolge, unterschiedliche Videouhren, bewusst gewählte zweite Tonspur, fehlende Spur, inkompatibles Video, beschädigter gewählter Ton, gleiche Eingabedatei, vorhandenes Ziel, Abbruch, Zieldatei-Rennen, falsche Samplezahl, Decoderwarnung, veränderte Paketdaten, Mono, 44,1 kHz, 5.1, 50 fps, gezielt veränderte Videodaten sowie beschädigte zweite Tonspur einmal entfallend und einmal ausgewählt. Negative Fälle gelten nur bei korrekter Ablehnung als bestanden. Große temporäre Dateien wurden bereinigt; Originale/Zieldateien bleiben erhalten.

Zusätzlich **zehn bisherige native AAC-Notfall-/Tonspurkopiefälle** gegen aktuelle Produktbibliotheken erfolgreich. WPF-Dialog separat gerendert und geprüft: Start ohne Auswahl, keine Vorauswahl bei mehreren Spuren, Aktivierung erst nach Wahl beider Spuren, Reihenfolgetausch mit zugehöriger Spurauswahl.

## Technischer Referenzlauf direkt im integrierten Dienst

Verwendet wurde **John Wick Kapitel 4 Teil 1 otr-can.mp4**, also der bereits geschnittene Teil 1 **mit beiden Tonspuren**, nicht die zusätzliche AAC-Kopie. Explizit gewählt: **Spur 1, AAC, Deutsch, Stereo, 48000 Hz**. Teil 2: **John Wick Kapitel 4 Teil 2 mit Audio Korrektur.mp4**, ebenfalls seine AAC-Spur 1.

Technisch vollständig bestanden: **238229 unveränderte Videopakete**, **457399680 dekodierte Tonsamples**, Gesamtdauer **02:38:49.160**, Klebestelle **01:57:37.120**, Vorschau ab **01:57:34.120**. Die neue Ausgabe liegt separat unter dem ignorierten Entwicklungs-Prüfordner; die Desktop-Filme wurden nicht ersetzt. Diese separat vom Agenten erzeugte Prüfausgabe wurde nicht zusätzlich persönlich bestätigt; die unten beschriebenen selbst erzeugten Nutzerdateien sind positiv geprüft.

Der große Lauf erfolgte vor den letzten Fortschritts-/Protokollergänzungen, der expliziten AAC-LC-Profilangabe und der zusätzlichen Quellton-Zeitstempelvorprüfung; die finalen Bibliotheken prüfen beide Quellton-Zeitachsen und die fertige John-Wick-Bild-/Tonzeitachse erneut. Die kleinen nativen Fälle verwenden den finalen Stand. Technische Details und lokale Prüfausgabe: **.build/klebecenter-20261027/**, insbesondere **NATIVE-RESULT.json**, **JOHN-WICK.log**, **JOHN-WICK-FINAL-VERIFY.log**, **source-clock-tests/**, **source-clock-native/**, **regression/NATIVE-RESULT.json**, **dialog-final.png**.

## Persönlicher Klebecenter-Lauf bestätigt

Der Nutzer führte danach selbst den vollständigen Ablauf im Klebecenter aus: Teil 1 **John Wick Kapitel 4 Teil 1 otr-can.mp4**, bewusst **Spur 2: AC-3, Deutsch, Stereo, 48000 Hz**; Teil 2 **John Wick Kapitel 4 Teil 2 mit Audio Korrektur.mp4**, **Spur 1: AAC, Deutsch, Stereo, 48000 Hz**. Damit ist zusätzlich zur technischen AAC/AAC-Referenz auch AC-3/AAC im realen Bedienweg bestätigt.

Die neue Datei **John Wick Kapitel 4 new.mp4** wurde erfolgreich erzeugt. Das vollständige Protokoll bestätigt Quellton-Zeitstempel, strenge Dekodierung, unveränderte Bildpakete, einmalige durchgehende AAC-Kodierung und alle Ausgabeprüfungen: **457399680 Tonsamples**, Gesamtdauer **02:38:49.160**, Klebestelle **01:57:37.120**. Screenshot und kopierte Abschlussinformationen zeigen Anfang, Klebestelle und Ende im Prüfdialog. Anschließend bestätigt der Nutzer ausdrücklich **„Alles super check“** und beauftragt Setup, ZIP und weitere Quellenpakete. Eine neue Installer-/VM-/Portable-Abnahme wird daraus nicht abgeleitet.

## Zweiter eigener Lauf: AAC/AAC

In der angekündigten Arbeits-PC-Setuprunde liefert der Nutzer ein weiteres vollständiges Erfolgsprotokoll: **Teil 1 Spur 1 AAC**, **Teil 2 Spur 1 AAC**, jeweils Deutsch/Stereo/48000 Hz. Alle Prüfungen erfolgreich: 457399680 Tonsamples, 02:38:49.160 Dauer, 01:57:37.120 Klebestelle. Rückmeldung **„Auch hier alles prima“**. Danach Wechsel zu Setup Build 40 angekündigt. Das Protokoll enthält keine eigene Buildanzeige; daraus folgt keine vollständige Installations-/VM-Abnahme.

## Aktuelle Pakete und nächster Schritt

**Build 40**: 1485 Pakettests plus 1213 Tests gegen finale DLLs. Die 21 Klebecenter- und zehn bisherigen nativen Paketfälle stammen aus Build 39; 361 Anwendungs-/Testquellen in Build 40 bytegleich, Motor unverändert. Build 40 korrigiert nur Voraussetzungstext und Paketabgleich. Screenshot bestätigt Setuphinweis 0.1.6; vollständige neue Setup-/Portable-/VM-Endabnahme bleibt offen.

Arbeitsdokumente ergänzen spätere Rückmeldungen; eingefrorene Archive, Paketdokumente und Prüfsummen behalten ihren Erstellungsstand. Kein neuer Build, Commit/Push oder Release für diesen Dokumentationsabgleich.

[Paketbericht 40](RELEASE-1.0.2-BUILD40-RESULT.md), [Funktionsnachweis 39](RELEASE-1.0.2-BUILD39-RESULT.md), [ursprüngliche externe Referenz](JOHN-WICK-4-KLEBECENTER-REFERENZ-20261026.md), [Projektstand](AKTUELLER-STAND.md).

## Furiosa: großer Praxislauf in der Build-40-Setuprunde

Nach dem angekündigten Wechsel zu Setup Build 40 bestätigt der Nutzer beide otr-can-Schnitte von Furiosa Teil 1/2 sowie das Klebecenter und alle Prüfstellen. Beide gewählten Spuren AAC/Deutsch/Stereo/48000 Hz. Vollständiges Klebeprotokoll erfolgreich: **387448320 Tonsamples**, Dauer **02:14:31.840**, Klebestelle **01:54:18.320**, Vorlauf **01:54:15.320**. Damit ist der Arbeits-PC-Funktionslauf der Setup-Prüfrunde positiv gemeldet; zusätzliche VM-/Portable-/Deinstallationsvarianten werden daraus nicht abgeleitet. [Praxisbericht](FURIOSA-KLEBECENTER-PRAXIS-20261027.md).

## Klebecenter-Arbeitsordner ab Build 41

Nach Erfolg werden temporäre Dateien, Erfolgsprotokoll und leerer Arbeitsordner entfernt. Bei Fehler oder Abbruch bleibt die Diagnose erhalten; ihr Pfad steht im Protokoll. Gesperrte Dateien oder fremde Inhalte werden gemeldet und erhalten. Bereits verbliebene Ordner älterer Läufe werden nicht automatisch durchsucht. [Änderung und Prüfung](KLEBECENTER-BEREINIGUNG-20261027.md).

## Mitgelieferten Motor nach Überinstallation prüfen

Der aktuelle Paketsatz enthält **otr-can 0.1.6** unter **tools/otr-can/otr_can.exe**. Unter **Einstellungen → OTR-CAN-Werkzeuge** den Pfad auf diese mitgelieferte EXE setzen. Eine übernommene Einstellung kann weiterhin auf eine ältere Kopie zeigen. Die Versionsprüfung muss **0.1.6** melden. Das Klebecenter verwendet die konfigurierten FFmpeg-Werkzeuge; je Teil wird genau eine gewählte Tonspur übernommen und der verbundene Ton einmal als AAC-LC kodiert.
