# Paket 2: kontrollierte Audioalternativen – technischer Vorschlag

Stand 04.11.2026. **Konzept, keine Implementierungs- oder Produktivfreigabe.** Paket 1 bietet eine belegte Diagnose und einen bewusst bestätigten MP4Box-Neustart. Dieser Bericht beschreibt getrennte spätere Entwicklungsschritte. Der Normalmodus bleibt streng; eine Zeitlücke belegt nicht den ursprünglichen Toninhalt und nicht die historische Ursache.

## 1. Bausteine und Freigabeschritte

1. Gemeinsames, rein fachliches Modell der dekodierten Quellzeitachse: ganzzahlige PTS, tatsächliche Samplezahl, deklarierte Dauer, Format, Endbeleg und Zuordnung zu Keeps. Quellenidentität und Werkzeugversion gehören zum Nachweis. Paket 1 liefert einen konservativen Teil davon, jedoch keinen Reparaturauftrag.
2. Separater Vorschlag für Tonspurausschluss. Benutzer entscheidet über konkret genannte Originalspuren. Neuer vollständiger Auftrag mit unabhängiger Vorbereitung und strenger Ausgabeprüfung.
3. Isoliertes CLI-/Laborprogramm für genau begrenzte innere AAC-Lücken. Zunächst keine Freischaltung in CAN. Es erzeugt einen vorab prüfbaren Einfügeplan und einen unabhängig verifizierten PCM-Nachweis.
4. Gesonderte Endregel mit eigener Bestätigung. Erst nach kompletter Medienregression und persönlicher Abnahme eine CAN-Einbindung vorschlagen.

Für alle Schritte gelten eigene neue Arbeitsverzeichnisse, Abbruch des gesamten Prozessbaums, unveränderte Originale und geschützte bestehende Ausgaben. Keine Fortsetzung einer teilweise fehlgeschlagenen Verarbeitung. Normale gesunde Aufträge sollen keinen Reparaturzweig verwenden.

## 2. Optionale Spuren kontrolliert ausschließen

Wiederverwendbar sind AacEmergencyOffer/Choice, SelectedAudioSource, AacEmergencySource, OtrCanAacEmergencyCutService und die Tonspurzuordnung des Klebecenters. Bereits vorhanden: ausdrückliche Zustimmung, Anzeige entfallener Spuren, strenge vollständige Dekodierung der gewählten Spur, Streamcopy und exakter Paket-/Zeit-/Konfigurationsvergleich sowie vollständiger neuer otr-can-Lauf. Paket 1 erhält diesen bestehenden, persönlich geprüften Weg. Es erweitert ihn nicht zu einer allgemeinen Ausschlussregel.

Nicht ausreichend ist allein ein erfolgreicher Decoderlauf nach null: NCIS dekodiert sauber und hat trotzdem Zeitlücken. Die verbleibenden Spuren müssen deshalb zusätzlich gegen die tatsächlich benötigten Original-Keeps auf ihre **Zeitabdeckung** geprüft werden. Eine nachgewiesene Störung kann vor dem Keep liegen und dessen Ton beeinflussen; Decoder-Vorgeschichte bleibt Teil des strengen Pfades.

Vorgeschlagener Ablauf:

- Nur einen originalen Stream mit nachgewiesener Fehlklassifikation benennen. „Optional“ wird aus der bewussten Benutzerentscheidung abgeleitet, nicht aus Sprache oder Kanalzahl geraten.
- Mindestens eine vollständig geeignete Resttonspur nachweisen. Keine Entfernung der letzten Spur. Die Auswahl zeigt Codec, Sprache/Titel, Layout, Rate und alle entfallenden Spuren.
- Zur Vorbereitung unveränderte Originaldatei und bestätigte Streamidentitäten verwenden; Dateilänge/Hash sowie Streamindices erneut prüfen. Original- und vorbereitete Indizes können verschieden sein und werden explizit zugeordnet.
- Den gesamten Auftrag neu beginnen. Keep-Snapshot, Ziel- und Überschreibentscheidung bleiben erhalten. Erwartete Ausgabespuren werden als exakte Liste geführt und niemals nach einem Fehler nachträglich auf die tatsächlich vorhandene Anzahl reduziert.
- Entfallene Spuren im Protokoll, Abschlussfenster und bei bewusst neu erzeugten Cutlist-Metadaten sichtbar nennen. Bei Fehlschlag weiterhin MP4Box anbieten.

Erweiterungsbedarf: ExpectedOriginal und heutige Motorauswahl sind auf den bestehenden Ein-AAC-Notfallweg zugeschnitten. Allgemeine Restspur-Mengen und AC-3-Restspuren brauchen einen eigenen erwarteten Streamplan. Der vorhandene AAC-Dialog darf nicht einfach zum vermeintlichen Beweis intakter Restspuren umetikettiert werden.

## 3. Innere Lücken: exakte Rekonstruktion einer Zeitachse

Vorschlag für eine erste, noch zu prüfende Laborfehlerklasse: **AAC-LC, 48000 Hz, Stereo, unverändertes fltp-Format, Quellbeginn null, Zeitbasis 1/48000, 1024 tatsächlich dekodierte Samples, 1536 deklarierte Ticks und exakt 512 Ticks bis zum nächsten belegten Frame.** Kein Decoderfehler, keine Überlappung, kein Formatwechsel, kein widersprüchliches Skip-/Padding-Verhalten, lückenlose Belege für alle übrigen benötigten Positionen.

Vorgeschlagene erste Studiengrenzen: maximal 512 Ticks pro innerer Lücke und maximal acht ausgewählte Lücken/4096 ergänzte Samplevektoren pro Spur und Auftrag. Das sind konservative Vorschläge, **keine aus der Diagnose ableitbaren Sicherheitsgarantien**. Größere oder andere Klassen, auch 32/44,1 kHz oder Mehrkanalton, zunächst ablehnen. Die 64-Diagnose-Einträge aus Paket 1 sind ausdrücklich keine Reparaturfreigabegrenze.

Für jedes Keep [a,b) mit vorausgehender Ausgangsdauer P gilt:

~~~text
Vorhandenes Quellsample an Position s → Ausgangsposition P + (s - a)
Belegtes fehlendes Intervall [g0,g1) → genau diese Ausgangsplätze mit Nullwerten belegen
~~~

Es werden Samplevektoren über **sämtliche Kanäle gemeinsam** behandelt. Eine nur teilweise behaltene Lücke wird ausschließlich auf ihren Schnitt mit [a,b) begrenzt. Bestehende PCM-Werte bleiben an der zugehörigen Originalzeit; kein Nachfüllen erst am Keep-Ende. Lücken außerhalb der Keeps erhalten keine Einfügung und verändern keine späteren Auswahlgrenzen.

Einträge im Einfügeplan: Quellhash, Streamidentität, Keep, ursprüngliche PTS und Paketbeleg, erster/letzter fehlender Sampletick, Ausgangsposition, exakte Anzahl, Kanalformat und bewusst gewählte Rekonstruktionsregel. Keine generelle Neuberechnung vorhandener Quell-PTS aus einem durchlaufenden Samplezähler.

Der Normalpfad mit asetpts=N/SR/TB darf erst **nach** dem belegten Aufbau der vollständigen Zeitplätze verwendet werden. Automatisches aresample mit async ist als pauschaler Reparaturschalter ungeeignet: Es kann abhängig von seinen Optionen Samples strecken, verkürzen, entfernen oder Stille ergänzen. Diese Möglichkeiten sind in der [FFmpeg-Filterdokumentation](https://github.com/FFmpeg/FFmpeg/blob/master/doc/filters.texi) beschrieben. Vorschlag ist ein expliziter Einfügeplan mit eigener PCM-Zusammenstellung, kein unkontrollierter automatischer Zeitausgleich.

Die Ausgabe darf nur als **bewusst rekonstruierte Zeitachse mit Stille** bezeichnet werden. Nullwerte stellen keine verlorene Sprache oder Musik wieder her. Auch zehn Millisekunden Stille können akustisch auffallen; Klicks an den Rändern dürfen nicht ohne weitere Freigabe durch zusätzliche Überblendung und damit veränderte Originalwerte kaschiert werden.

## 4. Früheres Audioende unabhängig behandeln

NCIS: Audioende 204420096/48000 s, Videoende 204443520/48000 s. Die 23424 Samples am Ende sind keine der inneren 512-Sample-Lücken.

| Variante | Zeitzuordnung und Nutzererwartung | Container-/Prüfbedarf | Vorschlag |
| --- | --- | --- | --- |
| Bewusst kürzere Tonspur | Vorhandener Ton behält seine Position, Video bleibt vollständig. Hinter dem Audioende existiert kein kodierter Ton. Alle späteren Keeps müssen als tonloser Suffix ausgewiesen sein. | Eigene erwartete Audiodauer, AAC-Vorlauf/Endpadding und Mux-Prüfung; kein -shortest zum Abschneiden des Videos. Unterschiedliche Spurenden müssen in mpv, VLC und Windows-Wiedergabe praktisch geprüft werden. | Sinnvolle isolierte Studie, keine spontane Lockerung der bisherigen Gleichheitsprüfung. |
| Exakte Endstille | Vorhandene Werte bleiben zugeordnet, Ende erhält genau den ausdrücklich gewählten tonlosen Abschnitt; gemeinsame geplante Länge ist für Anwender übersichtlich. | Separate Zustimmung mit konkreter Dauer; Einfügeplan/PCM-Beweis wie innen, jedoch eigener Fehlerklasse. AAC-Padding und MP4-Präsentationsende exakt überprüfen. | Bei späterem Reparaturversuch gesondert wählbar. Studiengrenze beispielsweise 500 ms; größere Fälle zunächst ablehnen. |
| Abbruch, MP4Box anbieten | otr-can verspricht keine Rekonstruktion. Anwender entscheidet über alternative Schnittmethode und persönliche Kontrolle. | Bestehender Paket-1-Ablauf; MP4Box-Erfolg bleibt kein Synchronitätsbeweis. | **Aktuell vorgesehener Standard.** |

Die Beispiele 500 ms bzw. 24000 Samples bei 48 kHz sind vorgeschlagene Laborgrenzen. Dass NCIS mit 488 ms darunterliegt, begründet keine Produktivfreigabe. Die historische Ursache und der fehlende Toninhalt bleiben unbekannt.

Movie-Timebase und Track-Timebase dürfen nicht vermischt werden. FFmpeg bietet movie_timescale, video_track_timescale und use_editlist als getrennte MP4-Muxeroptionen; siehe [offizielle Muxerdokumentation](https://github.com/FFmpeg/FFmpeg/blob/master/doc/muxers.texi). Daraus folgt noch keine getestete Kompatibilität einer konkreten Ausgabe. Der jetzige Motor verwendet bereits ganzzahlige geeignete Zeitskalen und prüft die beim Mux übernommenen Pakete; die jeweilige Endregel muss in diese Nachweise ausdrücklich eingehen.

## 5. Abschlussnachweis und Tests

Die reine Gesamtsamplezahl genügt nicht. Der Laborpfad muss unabhängig vom erzeugenden Plan verifiziert werden:

1. Vollständige Originaldekodierung mit Quell-PTS als Referenz; Frame- und Pakettabelle sowie Decoder-Vorlauf/Endpadding getrennt erfassen.
2. Für **jeden** Ausgangsblock Quellposition oder bestätigten Stilleeintrag nachweisen. Vor der verlustbehafteten Kodierung alle vorhandenen PCM-Kanalwerte bytegenau gegen die Referenz vergleichen, jede eingefügte Position auf exakt Null prüfen. Keine zusätzlichen Nullen außerhalb des Plans.
3. Einen globalen Abgleich der Sampleplätze und Keep-Prefixe durchführen. Lücken, Überlappungen, falsche Reihenfolge und ein kompensierender Netto-Samplefehler dürfen nicht durchrutschen.
4. Danach jede komplette Tonspur genau einmal kodieren. Dekodierte Ausgabe auf Anzahl, Beginn, Ende und PTS-Kontinuität gemäß ausdrücklich gewählter Regel prüfen. Wegen der verlustbehafteten Kodierung ist hier keine Bytegleichheit mit Original-PCM zu erwarten; der exakte PCM-Beweis muss davor liegen.
5. Finalen Mux vollständig prüfen: Videoframefolge und Zeiten unverändert zur freigegebenen Schnittausgabe, gewählte Streamanzahl/-identität, Konfigurationen, Vorlauf, Padding, Präsentationsende sowie paketgenauer Vergleich der Zwischen- und Enddateien. Keine verdeckte Kürzung durch den Mux.
6. Bild-/Ton-Zuordnung zusätzlich mit synthetischen sichtbaren Framezählern und zeitlich bekannten Kanalimpulsen vor und nach **jeder** Lücke und Schnittkante prüfen. Encoder-Laufzeit/Vorlauf getrennt berücksichtigen. Persönliche Prüfung im CAN-Kontrollplayer bleibt eine eigene Abnahme.

Medienmatrix: reguläres AAC/AC-3/MP3, eine/mehrere 512er-Lücken, teilweise behaltene Lücke, Lücke direkt vor/nach/auf Schnittgrenze, mehrere Keeps, Einzelkeep, exakt belegtes Audioende, längeres Video, innere Lücke plus Endfehlmenge, mehrere Spuren mit einer schlechten Spur, letzte verbleibende Spur, neue/alte Ein-Sample-Überlappung, positive Lücke plus kompensierende Überlappung, wechselndes Format, fehlende/negative/duplizierte PTS, unpassende Timebase, Warnungen, widersprüchliche Enddaten und Überschreitung jeder Studiengrenze. Bei Abbruch, fehlendem Werkzeug und Dateikonflikt bleiben Originale, vorhandene Ziele und Schnittdaten erhalten.

Gezielt auch einen absichtlich falsch platzierten Stilleblock gleicher Länge prüfen: Die Gesamtsumme wäre richtig, der Positionsvergleich muss ihn dennoch ablehnen. Bei Tonspurausschluss sind falsch zugeordnete Indizes/Sprache/Titel und nur scheinbar intakte Restspuren Negativfälle. Alle bisherigen Motor- und CAN-Regressionen bleiben bestehen.

NCIS bleibt privater Referenzfall mit dokumentiertem Hash und getrennten vollständigen Audiomessungen. Keine Filmdatei ins Repository. Weitere große Realfilm-Schnitte erst gezielt und unter Ressourcengrenzen; der bekannte Mad-Max-/Furiosa-Systemstillstand wird durch dieses Konzept nicht als gelöst betrachtet.

## 6. Entscheidungsvorlage

Empfehlung: Zuerst den bestehenden Paket-1-Rückfall persönlich prüfen. Danach gegebenenfalls **ein isoliertes Labor für ausschließlich die beschriebene 512er-AAC-Klasse** freigeben. Tonspurausschluss und die beiden Endvarianten bleiben getrennte Arbeitspakete. Eine Freigabe des Konzepts oder eines Labors schaltet keine produktive Reparatur in CAN frei.
