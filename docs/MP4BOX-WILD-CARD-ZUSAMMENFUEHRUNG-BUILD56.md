# MP4Box-Zusammenführung in Build 56

11.11.2026 · Cut Assistant Next 1.0.3 Build 56 · Entwicklungszweig codex/community-feedback-20261109

## Reproduktion mit Wild Card

Die fünf vorhandenen 2AAC-Segmente wurden mit der in CAN konfigurierten MP4Box-Installation verglichen. MP4Box meldet GPAC 26.07-rev0-ga07cbfff-master. Beide Läufe verwendeten dieselben Eingaben in derselben Reihenfolge und je eine neue Ausgabedatei im Windows-Tempordner. Die ursprünglichen Film-, Segment- und Vergleichsdateien wurden nur gelesen.

| Befehl | Beobachtung |
|---|---|
| -cat Segment1 -cat Segment2 … -cat Segment5 Ausgabe.mp4 | Exit-Code 1 beim abschließenden Interleaving. Zweimal „Attempt to overread bitstream“, anschließend „I/O Error“. Der MP4-Index der fehlerhaften Ausgabe meldet eine AAC-Spur mit 462.888 Paketen. |
| -add Segment1 -cat Segment2 … -cat Segment5 -new Ausgabe.mp4 | Exit-Code 0. H.264-Track 1 mit 123.437 Frames, AAC-LC-Tracks 2 und 3 mit je 231.444 Paketen. Video 4.937,480 s, Audio je 4.937,450 s. Beide Audiospuren sind deutsch, stereo, 48 kHz. |

Der Lauf mit -add/-cat/-new enthält pro Spur genau die in Segmentreihenfolge erwarteten Pakete. Die SHA-256-Folgen der Paketnutzdaten stimmen je Spur mit allen fünf Eingabesegmenten und mit dem persönlich geprüften Vergleichsergebnis überein. Die Roh-DTS steigen in allen drei Spuren strikt an. Im zuvor fehlerhaften Vergleichsergebnis wurden 231.443 nicht steigende DTS in der einzigen lesbaren AAC-Spur gezählt. Die Paketprüfung berücksichtigt die Rohzeitstempel unabhängig von MP4-Edit-Listen.

Die Spurvermischung ist auch an zwei vorhandenen Kurzsegmenten sichtbar. Mit reinem -cat endet der Lauf dort sogar mit Exit-Code 0, erzeugt aber nur eine AAC-Spur mit 1.612 Paketen aus zweimal 806 erwarteten Paketen. Der Exit-Code allein erkennt diesen Fehler daher nicht.

## Ursache und Korrektur

CAN ließ MP4Box auch das erste Segment per -cat in eine leere Ausgabe einfügen. GPAC sucht beim Anhängen zunächst nach einer Zielspur mit passender Track-ID. Fehlt sie, kann die Suche auf passende Medien- und Samplebeschreibung sowie bei Audio auf die Sprachkennung zurückfallen. Bei den gleichartigen deutschen AAC-Spuren erzeugt der alte Aufruf zunächst nur eine Audio-Zielspur. Die zweite Eingabespur landet in derselben Zielspur. Der Quellcode erklärt die beobachtete Spurzuordnung. Der genaue interne Zugriff hinter der abschließenden Bitstream-Fehlermeldung wurde nicht instrumentiert. [GPAC-Quellcode zur Track-Zuordnung](https://github.com/gpac/gpac/blob/a07cbfff/applications/mp4box/fileimport.c#L2924-L2999)

Mit -add importiert GPAC die drei Tracks des ersten Segments separat. Die folgenden -cat-Aufrufe finden bereits die passenden Track-IDs. -new erzeugt die Ausgabedatei neu. [GPAC-Dokumentation zu Importoptionen](https://wiki.gpac.io/MP4Box/mp4box-import-opts/)

Geändert wurde der Befehlsaufbau in src/CutAssistantNext.Media/Cutting/Mp4BoxRunner.cs, Methode RunConcatAsync.

~~~text
MP4Box -add Segment1.mp4 -cat Segment2.mp4 … -cat SegmentN.mp4 -new Ausgabe.mp4
~~~

Mp4BoxCutWorkflow liefert bei einem einzelnen Behaltebereich weiterhin die geschnittene Segmentdatei direkt aus. Bei mehreren Bereichen verwendet er für MP4Box einen eindeutigen Arbeitsnamen. Erst nach Erfolg veröffentlicht er das Ergebnis unter dem gewünschten Namen. Die bestehende Überschreibfreigabe, Fehlerausgabe und Bereinigung der Arbeitsdateien greifen auf diesem Weg.

## Prüfungen

- Release-Build 1.0.3.56 mit 0 Warnungen und 0 Fehlern.
- 2.050 CAN-Tests bestanden, davon 200 Core, 80 Cutlists, 397 Media und 1.373 App. Die fünf neuen Fälle sind enthalten.
- 78 Spotlight-Tests und 56 native CAN-/Spotlight-Prüfungen mit synthetischem Film bestanden.
- Alle Tests der abschließenden Läufe liefen ohne Fehler oder übersprungene Fälle.

Der neue Argumenttest in Mp4BoxRunnerConcatTests.cs prüft den vollständigen Befehlsaufbau mit Pfaden mit Leerzeichen. Mp4BoxRunnerConcatIntegrationTests.cs führt den echten Runner mit je drei kurzen synthetischen H.264-Segmenten aus. Die Matrix umfasst AAC, AAC+AC-3, AAC+AAC und AC-3+AC-3. Bei gleichartigen Doppelspuren haben beide Tonspuren dieselbe Sprachkennung und unterscheidbare Paketnutzdaten. ffprobe prüft die Anzahl und Reihenfolge der Streams, Codecs, Sprachkennungen, Paket-Hashes, Laufzeiten und Zeitstempel. Für die optionale Matrix werden die vorhandenen Programmdateien über CAN_MP4BOX_TEST_EXE, CAN_FFMPEG_TEST_EXE und CAN_FFPROBE_TEST_EXE angegeben.

Die bestehenden Workflow-Tests decken den einzelnen Behaltebereich, Fehler und Abbruch, Erhalt bereits existierender Ergebnisse und die Bereinigung der temporären Dateien ab. Die erste Gesamtausführung zeigte eine zeitabhängige Anfangsabfrage im bestehenden Spotlight-EOF-Test. Derselbe Fehler trat mit dem vorhandenen Build-55-Test auf. SpotlightCommunityFeedbackTests.cs wartet nun vor der ersten Button-Abfrage auf die WPF-Datenbindung. Die Sollwerte und der geprüfte Klickpfad blieben dabei erhalten.

Protokolle, Argumente, ffprobe-Auswertungen und Testergebnisse liegen lokal unter .build/wild-card-20261111. Die zwei direkten Vergleichsausgaben liegen im separaten Windows-Tempordner can-wild-card-native-bbaf7a627a324e0b8bcd8c9d82a0b4d4. Der erste Versuch unter eingeschränkten Rechten scheiterte an GPAC-Arbeitsdateien. Der direkte Vergleich mit den regulären Schreibrechten lieferte die oben aufgeführten Exit-Codes.

## Entwicklungsfassung und persönlicher Test

Build 56 wurde mit dem normalen Buildzähler in einem eigenen Ausgabeordner erzeugt, da Build 55 geöffnet war. SHA-256-Prüfsummen bestätigen die erhaltenen Build-55-Programmdateien. Der Eintrag tools/otr-can im Build-56-Ordner verweist per Windows-Verzeichnisverknüpfung auf dieselbe vorhandene Motorablage.

[CutAssistantNext.App.exe für Build 56](../src/CutAssistantNext.App/bin/Build56/Release/net10.0-windows/CutAssistantNext.App.exe)

**Persönlicher Wild-Card-Vollschnitt mit Build 56 erfolgreich.** Das CAN-Protokoll zeigt fünf geschnittene Segmente, die neue Befehlsfolge mit einmal -add, viermal -cat und -new sowie den Abschluss „Fertig.“ nach der Übergangsprüfung. Der Anwender bestätigte den erfolgreichen Schnitt und meldete zwei sichtbare Werbeblitzer. Die Keyframe-Blitzer liegen außerhalb der Korrektur der Zusammenführung.

**Persönlicher otr-can-Gegentest mit Build 56 erfolgreich.** Derselbe Wild-Card-Originalfilm wurde mit dem Motor 0.1.6-audio-diag.1 über fünf Behaltebereiche geschnitten. Das Protokoll bestätigt 123.445 Videopakete und zwei durchgehend geprüfte AAC-Spuren mit jeweils 237.014.400 dekodierten Samples bei 48 kHz. Der Anwender bestätigte Bild, Ton und Schnittübergänge und beobachtete keine Werbeblitzer.

**Weiterer persönlicher MP4Box-Test mit Build 56 erfolgreich.** Bei *Long Lost Christmas – Wiedersehen zu Weihnachten* schnitt CAN vier Segmente und fügte sie mit einmal -add, dreimal -cat und -new zusammen. Das Protokoll zeigt die Prüfung der drei Übergänge und endet mit „Fertig.“. Der Anwender bestätigte Bild, Ton, Schnitt und Drei-Sekunden-Vorschau.

**Persönlicher MP4Box-Test mit Build 56 erfolgreich.** Bei *NCIS: Sydney – Mango Madness* (S02E09) schnitt CAN drei Segmente und fügte sie mit einmal -add, zweimal -cat und -new zusammen. Nach der Prüfung der Schnittstellen meldet das Protokoll „Fertig.“. Der Anwender bestätigte den erfolgreichen Praxistest.
