# Mad Max / Furiosa – kompletter Rechnerstillstand bei erneutem otr-can-Schnitt

Stand: **20.10.2026**. **Kritischer offener Fehler, Ursache noch nicht nachgewiesen, keine Korrektur behauptet. Erneuter kompletter Stillstand auch unter 2-GiB-/25-Prozent-Grenzen; der Diagnosewaechter verhindert diesen Fehler nicht. Weitere Realfilm-Schnitttests sind vorerst gestoppt und der lokale CAN-Teststarter gesperrt.** CAN 1.0.1 · Build 34 bleibt als Entwurf zurückgenommen; CAN 1.0.0 · Build 32 bleibt Latest. Die früheren positiven Einzeltests ersetzen diesen Großdatei-/Wiederholungsfall nicht.

## Nutzerbeobachtung

Der Nutzer meldet: CAN friert beim zweiten otr-can-Schnitt einer großen Mad-Max-Datei vollständig ein; ein Hardboot war erforderlich. Auf Rückfrage bestätigt er ausdrücklich, dass **der ganze Rechner nicht mehr reagierte**, nicht nur das CAN-Fenster. Eine genaue letzte Bildschirmanzeige wurde nicht mitgeteilt.

## Gesicherte Befunde

- Vorhandenes Original `Furiosa__A_Mad_Max_Saga__Teil_1_2__26.10.18_20-15_4plus_150_TVOON_DE.HD.mp4`: **7474386079 Bytes**, H.264/1080p/25 fps, **12298.800 s** Videodauer, AAC-LC-Stereo und AC3-Stereo mit je 48 kHz.
- Zwei nach erzwungenem Neustart verbliebene Arbeitsordner: `.can-otr-9a36520dbd5047e0b225cb0a99c395ca` und `.can-otr-ac0cebf073934af595c8b6a0ab04b846`.
- In beiden: vollständige FFMS2-Begleitdateien, große Video-Zwischendatei und begonnene PCM-Audioausgaben für beide Spuren. Keine finale Audioausgabe. Damit ist **PCM-Aufbereitung als zuletzt erreichter Verarbeitungsschritt** belegt. Das beweist für sich allein nicht den Auslöser des systemweiten Stillstands.
- Beide Filterdateien verwenden fünf Keeps je Tonspur mit `asplit`, `atrim`, `asetpts` und `concat`. Die Keep-Grenzen sind zwischen beiden Versuchen leicht verändert. Der konkrete Ablauf „zweiter Schnitt“ ist bisher eine Nutzerbeobachtung; die beiden Restordner allein belegen keine Ursache durch wiederholte UI-Nutzung.
- Die spezielle 0.1.6-Musikladen-Korrektur liegt erst nach erfolgreicher PCM-Erzeugung und Samplezählung. In den verbliebenen Aufträgen ist dieser Schritt noch nicht erreicht. Der grundlegende PCM-Filterpfad existiert schon vor 0.1.6; die Rückkehr zu Build 32 ist deshalb **kein nachgewiesener Schutz gegen diesen konkreten Großdateifall**.
- Vorhandene fertige otr-can-Ausgabe: **4439044647 Bytes**, **6881.640 s**, 172041 Videoframes laut Container; AAC und AC3 sind vorhanden. Das ist ein vorhandenes erfolgreich abgeschlossenes Ergebnis, kein neu ausgeführter Schnitt und keine neue persönliche Qualitätsabnahme.
- Windows protokolliert zwei unerwartete Neustarts mit Kernel-Power 41 und EventLog 6008. Gemeldete vorherige Abschaltzeiten: **20:16:09** und **20:36:14**, Neustarts ungefähr **20:17/20:37**. Keine protokollierte Bugcheck-Ursache daraus abgeleitet.
- Im untersuchten Zeitraum wurde **kein Resource-Exhaustion-Ereignis 2004** gefunden; keine einschlägige WHEA-, Grafikreset- oder Datenträgerfehlermeldung nachgewiesen. Ein ausgebliebener Logeintrag schließt Speicher-, Treiber- oder Hardwareprobleme bei einem harten Stillstand nicht aus. Die nach dem Neustart beobachteten rund 16 GiB freien RAM und rund 270 GiB freien Plattenplatz sind ausdrücklich keine Messung während des Fehlers.

Die vorhandenen Originale, fertigen Filme, unvollständigen WAVs und Arbeitsordner wurden nicht verändert oder bereinigt. Kleine Job-Metadaten und Graphen, App-Log und ausgewählte Windows-Ereignisse sind separat gesichert. Dateizeitstempel offener, gewaltsam beendeter Schreibdateien sind keine genaue Messung des letzten verarbeiteten Samples.

## Erste begrenzte Diagnose ohne erneuten Mad-Max-Schnitt

Nur kleine Container-Metadatenabfragen am Original und den bestehenden Ausgaben; keine Vollvideodekodierung, keine neue Indexierung oder erneuter großer Schnitt. Auch die Metadatenprozesse liefen unter erzwungenen Ressourcenlimits.

Ein isolierter Prüfhelfer startet seine Kindprozesse **suspendiert**, ordnet sie vor dem Fortsetzen einem Windows-Job zu und erzwingt Prozess-/Job-Speichergrenze, 25 Prozent CPU-Hardcap, Prozessanzahl und Zeitlimit. Wenn die Zuordnung scheitert, bleibt kein ungeschützter laufender Prozess zurück. Dies ist **nur ein Diagnosehelfer**, noch keine eingebaute CAN-Schutzfunktion.

Vergleich mit künstlichen Tondaten:

- 20 Minuten AAC-/AC3-Stereo bei 48 kHz, fünf verteilte Keeps, gleicher grundsätzlicher Filteraufbau und Float-WAV-Ausgabe.
- **384 MiB Prozess-/Job-Speichergrenze**, 25 Prozent CPU-Hardcap, 35 Sekunden Zeitlimit für die PCM-Erzeugung.
- Lauf beendet sich erfolgreich, kein Timeout. Gemessener maximaler Job-Commit **26771456 Bytes, etwa 25.5 MiB**.
- Damit bisher **keine Reproduktion eines allgemeinen Filter-Speicherwachstums oder Rechnerstillstands**. Der synthetische Fall bildet weder die ganze Mad-Max-Dauer noch den zweiten Schnitt innerhalb derselben WPF-Sitzung vollständig nach. Der Verdacht auf ungebremste PCM-Pufferung bleibt unbewiesen; keine Filteränderung auf Verdacht vorgenommen.

Lokale Nachweise unter `.build/mad-max-freeze-20261020`: begrenzter Prozesshelfer, Guard-Smoke-Test, synthetischer Filter und Messwerte, Container-Metadaten, Ereignisübersicht und gesicherte Restjob-Metadaten. Keine Programmänderung, kein neuer Produktbuild, keine Paketierung oder Wiederveröffentlichung.

## Nächster Untersuchungsschritt

Vor einem weiteren Großdateiversuch müssen Prozessbaum-Speicher, CPU, verbleibender Commit-Speicher und Datenträgerlast mitgeschrieben und ein vor Prozessstart wirksamer Abbruchschutz verwendet werden. Zusätzlich Wiederholungsfall innerhalb derselben CAN-Sitzung mit kleinen Dateien prüfen. Erst mit belastbarem Befund eine gezielte Korrektur entwickeln und gegen Bild-/Ton-/Sample- sowie Wiederholungsprüfungen verifizieren. Eine erneute öffentliche Freigabe ist nicht erteilt.


## Nachtrag: direkter otr-can-0.1.6-Lauf ohne CAN

Auf Nutzerwunsch am 20.10.2026 direkt gestartet, CAN war dabei beendet. Verwendet wurde die unveränderte EXE mit SHA-256 `73deb7d81c0d24241649b28d7ed924d3d64131e2ea84c3013198bbbdcb1cae3e`, expliziten FFmpeg-/ffprobe-Pfaden und einer Kopie des gesicherten FFMS2-Index samt Begleitdateien. Kein neuer Index und kein Zugriff auf alte Arbeitsordner zum Schreiben.

Schnittbereiche stammen aus dem letzten abgebrochenen Auftrag: `[583.200,2845.000)`, `[3603.960,4400.720)`, `[5167.840,6657.360)`, `[6951.560,8078.880)`, `[8848.920,10031.840)` Sekunden. Die später beigefügte ursprüngliche Cutlist weicht an Grenzen ab; der Nutzer bestätigt zwischenzeitlich korrigierte Schnittwerte. Ein zusätzlicher Lauf mit der ursprünglichen Cutlist wurde deshalb nicht gestartet. Der dafür vorbereitete lokale Helfer wurde nicht ausgeführt.

Der separate Diagnosewächter schließt den Motor und alle Kindprozesse in einen Windows-Job ein, vor dem Fortsetzen des suspendiert gestarteten Motors: 2 GiB Prozess-/Job-Commitgrenze, 25 Prozent CPU-Hardcap, höchstens acht gleichzeitig aktive Prozesse, 15 Minuten Zeitlimit, Abbruch bei knappem Systemspeicher oder weniger als 30 GiB freiem Datenträgerplatz. Kindprozessvererbung, verweigerte Speicherzuweisung und Prozessbaumabbruch wurden vorher mit kleinen künstlichen Prozessen geprüft. Dies ist weiterhin nur ein lokaler Diagnosehelfer, keine Produktfunktion und keine Garantie gegen systemweite Treiber-/Hardwarefehler.

Ergebnis:

- **Erfolgreich, Exitcode 0**, 285.17 Sekunden, kein Wächterabbruch, kein Rechnerstillstand beobachtet.
- **171458 Videopakete**, **329199360 Samples je Tonspur**, **6858.320 Sekunden**; AAC und AC3 mit je 48 kHz/Stereo.
- PCM-Erzeugung, beide Audio-Sample-/PTS-Prüfungen und finale MP4-Paket-/PTS-/DTS-/Padding-/Konfigurationsprüfungen bestanden. Keine zusätzliche vollständige Videodekodierung angefordert.
- Höchster Job-Commit **1468645376 Bytes, rund 1.37 GiB**. Ressourcenmessungen alle etwa 0.5 Sekunden in `resources.jsonl`.
- Unabhängige begrenzte Containerabfrage bestätigt H.264, AAC und AC3, Videoframezahl und die erwarteten Dauern. Persönliche Bild-/Ton-/Schnittprüfung dieser neuen Ausgabe ist offen.

Lokale Ausgabe und Nachweise: `.build/mad-max-freeze-20261020/standalone-20261020-210037/`; `monitor/COMMAND.json`, `monitor/process.log`, `monitor/resources.jsonl`, `monitor/RESULT.json` und `SUMMARY.json`. Original und frühere Ergebnisse bleiben erhalten.

**Einordnung:** Der direkte Motorlauf mit der großen Datei und den korrigierten Keep-Bereichen ist unter den genannten Grenzen möglich. Gegenüber den eingefrorenen CAN-Läufen änderten sich gleichzeitig der Anwendungskontext und die Ressourcenbegrenzung. Deshalb weder CAN als Ursache nachgewiesen noch ein Problem ohne Ressourcenbegrenzung ausgeschlossen. Auch die erste/zweite Ausführung innerhalb derselben WPF-Sitzung ist damit nicht reproduziert. Der kritische Befund bleibt offen; keine Produktkorrektur, keine erneute Veröffentlichung. Der nächste Vergleich muss diese Unterschiede einzeln prüfen und Ressourcen weiter protokollieren.


## Geschuetzter CAN-Teststart vorbereitet

Auf anschliessenden Nutzerwunsch lokaler Starthelfer unter `.build/mad-max-freeze-20261020/CAN-Teststart.cmd`. Startet die installierte Build-34-App aus ihrem Installationsordner, nach Hashabgleich mit dem geprueften Paket und Kontrolle der eingestellten Motor-/Werkzeugpfade. Bestehende CAN-/Schnittprozesse und doppelte Teststarts werden abgelehnt. Keine Aenderung an Produktcode, installierten Dateien, Werkzeugkonfiguration oder Originalcutlist durch den Helfer.

CAN wird vor dem Fortsetzen einem eigenen Windows-Job zugeordnet; derselbe Job umfasst seine Kindprozesse. 2 GiB Prozess-/Job-Commit und 25 Prozent CPU wie beim Direktlauf, maximal acht gleichzeitig aktive Prozesse. Fuer die interaktive Vor- und Nachbereitung gilt diesmal ein Sitzungslimit von 45 Minuten statt 15 Minuten. Die bisherigen Speicher-/Plattenreserven gelten weiter. Ressourcenmessung ergaenzt um PID, Prozessname, privaten Speicher, Working Set, CPU und Prozess-I/O. Separater Stopphelfer `CAN-Test-stoppen.cmd`; normaler Abschluss ueber CAN-Schliessen. Ein Schutzabbruch beendet auch CAN; offene Schnittarbeit ist vorher zu speichern. Keine Garantie gegen Treiber-/Hardwarestillstaende.

Geprueft: Python-Syntax und Startvorbedingungen; realer achtsekundiger CAN-Start ohne Film im begrenzten Job, erwarteter Zeitlimitabbruch und Prozessspeichermessung; separate kuenstliche Tests fuer Stoppdatei/Kindprozessbeendigung, verweigerte Kindprozess-Speicherzuweisung und Aufraeumen bei absichtlich fehlgeschlagener Messfunktion. Eine fehlerhafte Rechtewahl in der ersten Testpruefung wurde korrigiert (Warten auf einen Prozess erfordert SYNCHRONIZE); anschliessend alle drei Schutztests bestanden. Keine laufenden CAN-/Schnittprozesse nach den Pruefungen verblieben.

Zusaetzlich separate `Furiosa-korrigierter-Test.cutlist` mit den fuenf exakt dokumentierten Keep-Grenzen des erfolgreichen Direktlaufs vorbereitet, um den CAN-Vergleich reproduzierbar zu halten. Die vom Nutzer beigefuegte Originalcutlist wird nicht ersetzt. Anwendungsschritte und Grenzen in `CAN-Teststart-ANLEITUNG.md`. Der persoenliche erste/zweite Schnitt innerhalb dieser geschuetzten CAN-Sitzung ist noch offen. Kein neuer Produktbuild und keine Veroeffentlichung.

### Korrektur der vorbereiteten Testcutlist

Beim persoenlichen Laden meldete CAN das fehlende Pflichtfeld `Info.RatingByAuthor`. Fehler in der lokal erzeugten Diagnosecutlist, kein Befund zum Rechnerstillstand. `RatingByAuthor=0` (unbewertet) in Datei und Generator ergaenzt. Anschliessend die korrigierte Datei direkt mit dem Parser der installierten Build-34-DLL unter .NET 10 geladen: fuenf Keeps erfolgreich, alle Start-/End-Ticks exakt gegen die dokumentierten korrigierten Grenzen geprueft. Keine Programm- oder Schnittgrenzenaenderung; die laufende geschuetzte CAN-Sitzung kann zum erneuten Laden offen bleiben.

### Erster persoenlicher CAN-Schnitt unter Schutz erfolgreich

Nutzerprotokoll bestaetigt Abschluss mit 171458 Videopaketen und je 329199360 Samples fuer AAC/AC3; finale MP4- und anschliessende CAN-Ausgabepruefung erfolgreich. Messstand der weiterhin aktiven Sitzung `can-session-20261020-211443-680837`: CAN-PID 34520 bleibt offen, zum Stand nach dem ersten Schnitt nur noch CAN im Job aktiv (keine verbleibenden Schnitt-Kindprozesse). Bisheriger maximaler Job-Commit 1956749312 Bytes (rund 1.82 GiB); CAN zuletzt 515633152 Bytes privater Speicher (rund 492 MiB), gemessener CAN-Hoechstwert 742240256 Bytes. Diese Werte beweisen kein Speicherleck; die erste Messung direkt beim Prozessstart ist kein vergleichbarer Zustand mit geladenem Film. Snapshot `FIRST-CUT-REPORTED-SUCCESS.json` lokal gesichert. Der zweite Schnitt innerhalb derselben Sitzung steht noch aus; persoenliche Bild-/Tonpruefung dieser neuen Ausgabe nicht aus dem technischen Abschluss abgeleitet. Ursache weiterhin offen.

### Zweiter persoenlicher CAN-Schnitt in derselben Sitzung unter Schutz erfolgreich

Auch das zweite vom Nutzer gesendete Protokoll endet erfolgreich: identische 171458 Videopakete, je 329199360 Audio-Samples, kontinuierliche Audio-PTS und finale MP4-/CAN-Ausgabepruefungen bestanden. Beide Schnitte liefen ohne CAN-Neustart in PID 34520. Snapshot `SECOND-CUT-REPORTED-SUCCESS.json` gesichert.

Messstand nach dem zweiten Lauf: nur CAN selbst im Job aktiv, keine verbliebenen Schnitt-Kindprozesse. Privater CAN-Speicher nach dem ersten Lauf 515633152 Bytes (rund 492 MiB), nach dem zweiten 495157248 Bytes (rund 472 MiB), Hoechstwert im zweiten Beobachtungsfenster 519090176 Bytes (rund 495 MiB). In diesem begrenzten Wiederholungsfall kein zunehmender CAN-Speicherbedarf belegt. Hoechster Job-Commit beider Laeufe 1985138688 Bytes (rund 1.85 GiB), innerhalb 2 GiB. Mindestens rund 13.6 GiB physischer Systemspeicher und 42.2 GiB Commit-Reserve verfuegbar. Keine Allgemeinaussage ueber ungeschuetzte Laeufe daraus ableiten.

Zwischenstand: direkter Motorlauf sowie zwei CAN-Schnitte derselben grossen Datei mit denselben korrigierten Grenzen unter Speicher-/CPU-Begrenzung erfolgreich. Der urspruengliche Komplettstillstand ist unter diesen Bedingungen nicht reproduziert; weder dessen Ursache noch eine gezielte Produktkorrektur nachgewiesen. Die Ressourcenbegrenzung bleibt ein wesentlicher Unterschied zum urspruenglichen Vorfall. Kein Anlass, jetzt einen unbegrenzten Wiederholungsversuch zu starten oder Build 34 wieder freizugeben. Normaler CAN-Abschluss beendet die Messsitzung und erzeugt deren Abschlussdateien; persoenliche Bild-/Tonabnahme beider neuer Ausgaben ist nicht aus den technischen Logs abgeleitet.

### Furiosa Teil 2: MP4Box und anschliessend otr-can erfolgreich

Weiterer Nutzerfall mit `Furiosa__A_Mad_Max_Saga__Teil_2_2__26.10.18_22-45_4plus_55_TVOON_DE.HD.mp4`: beigefuegtes MP4Box-Protokoll verarbeitet beide Segmente, fuegt zusammen und beendet die Uebergangspruefung ohne die Teil-1-Fallbackwarnung. Das anschliessend gesendete otr-can-0.1.6-Protokoll bestaetigt 165004 Quellframes, 30338 Ausgabepakete und je 58248960 Samples fuer AAC/AC3 bei 48 kHz sowie finale MP4- und CAN-Ausgabepruefungen.

Neue geschuetzte Sitzung in CAN-PID 31136. Prozessmessung bestaetigt zuerst MP4Box (ab 107.75 s), danach otr-can (ab 187.88 s) innerhalb derselben CAN-Sitzung. Bisheriger Job-Spitzencommit 1941798912 Bytes (rund 1.81 GiB), letzter Messstand nur CAN im Job aktiv. Snapshot `FURIOSA-PART2-REPORTED-SUCCESS.json` im zugehoerigen Sitzungsordner gesichert. Auch die vorherige Teil-1-Testsitzung ist inzwischen normal mit Exitcode 0 ohne Schutzabbruch beendet.

Damit positiver geschuetzter Motorwechsel bei Teil 2; keine Reproduktion des urspruenglichen Teil-1-Systemstillstands. Unterschiedliche Datei/Groesse und Ressourcenbegrenzung bleiben als Bedingungen zu beachten. Keine persoenliche Bild-/Tonabnahme aus den technischen Protokollen abgeleitet. Keine Produktkorrektur, kein Build und keine Freigabe daraus abgeleitet.

### Weiterer HD-Fall mit 50 fps: Das Vergessen / Spurlos im Moor

Nutzerprotokolle bestaetigen MP4Box-Schnitt eines Behaltebereichs samt Ausgabepruefung ohne Fallbackwarnung sowie anschliessenden otr-can-0.1.6-Erfolg. 198007 Quellframes; Ausgabebereich [426.580,3035.140) mit 130428 Videopaketen bei 50 fps; AAC-Stereo/48 kHz mit exakt 125210880 Samples und kontinuierlichen PTS; finale MP4- und CAN-Ausgabepruefungen bestanden. Die MP4Box-Grenze 3035.120 statt 3035.140 entspricht der anderen Motoruebergabe; keine Ausgabeidentitaet beider Motoren behauptet.

Weiterhin geschuetzte CAN-PID 31136; nach diesem Fall nur CAN selbst aktiv, 395317248 Bytes privater CAN-Speicher (rund 377 MiB). Der Job-Spitzencommit bleibt unveraendert bei 1941798912 Bytes (rund 1.81 GiB). Snapshot `DAS-VERGESSEN-REPORTED-SUCCESS.json` lokal gesichert. Weiterer positiver begrenzter HD-Fall, nun auch mit 50 fps; Ursache des urspruenglichen Stillstands weiterhin offen. Persoenliche Bild-/Tonpruefung aus den technischen Logs nicht abgeleitet.

### Weiterer HQ-Fall mit 50 fps: Das Vergessen / Versunken

Beide Nutzerprotokolle erfolgreich: MP4Box verarbeitet einen Behaltebereich und beendet die Ausgabepruefung ohne Fallbackwarnung; otr-can 0.1.6 bestaetigt 197998 Quellframes, 132143 Ausgabepakete bei 50 fps und 126857280 Samples fuer AAC-Stereo/48 kHz, kontinuierliche rohe Audio-PTS sowie die finale MP4- und CAN-Ausgabepruefung. Als HQ-Fall entsprechend Dateiname dokumentiert, nicht als weiterer HD-Fall.

Weiterhin geschuetzte CAN-PID 31136; danach nur CAN selbst im Job, 396271616 Bytes privater Speicher (rund 378 MiB). Gegenueber dem vorherigen Fall rund 1 MiB Unterschied, kein wachsender Speicherbedarf aus dieser Einzelabweichung abgeleitet. Job-Spitzencommit bleibt 1941798912 Bytes (rund 1.81 GiB). Lokaler Snapshot `VERSUNKEN-HQ-REPORTED-SUCCESS.json`. Technischer Erfolg unter Schutzgrenzen; persoenliche Bild-/Tonpruefung und Ursache des frueheren Systemstillstands bleiben getrennt und offen.

### Weiterer HQ-Fall mit 50 fps: Das Vergessen / Verborgen

Beide Nutzerprotokolle technisch erfolgreich: MP4Box verarbeitet einen Behaltebereich und beendet die Ausgabepruefung ohne Fallbackwarnung. otr-can 0.1.6 bestaetigt 198019 Quellframes, 134637 Ausgabepakete bei 50 fps sowie AAC-Stereo/48 kHz mit 129251520 Samples und kontinuierlichen rohen PTS; finale MP4- und CAN-Ausgabepruefungen bestanden. Am Ende wird genau ein Videobild neu kodiert. Keine persoenliche Bild-/Tonabnahme daraus abgeleitet.

Aktuelle geschuetzte CAN-PID 31136; privater CAN-Speicher 395489280 Bytes, 1 aktive Prozesse im Job, bisheriger Job-Spitzencommit 1941798912 Bytes. Snapshot VERBORGEN-HQ-REPORTED-SUCCESS.json lokal im Sitzungsordner gesichert. Der technische Erfolg gilt fuer die begrenzte Testsitzung; Ursache des urspruenglichen Komplettstillstands weiter offen. Keine Produktaenderung oder Freigabe.


## Erneuter kompletter Rechnerstillstand trotz Ressourcenbegrenzung

Der Nutzer meldet beim erneuten Furiosa-/Mad-Max-Teil-1-Schnitt einen vollstaendigen Rechnerstillstand bei angezeigten 00:01:38 und Stage 2/3. Die zuvor erfolgreichen Schnitte unter Begrenzung sind damit **keine Entwarnung**. Ein allgemeiner Ressourcenwaechter allein ist fuer diesen Fehlerfall nachweislich keine ausreichende Schutzmassnahme.

Gesicherte Messdaten der bestehenden CAN-PID 31136 und Job-Policy bestaetigen die aktive Begrenzung: 2 GiB Prozess-/Job-Commit, 25 Prozent CPU, acht aktive Prozesse maximal, 45 Minuten Sitzungszeit. Letzter gespeicherter Messpunkt nach 1783.80 Sitzungssekunden, aus Startzeit/Monotonzeit ungefaehr **22:11:16.82**. Vier aktive Prozesse: CAN, otr-can, conhost und FFmpeg. Kein Abschluss-RESULT und kein protokollierter Waechterabbruch.

- Job-Spitzencommit **2103017472 Bytes**, unter der Grenze **2147483648 Bytes**; dieser Hoechstwert ist nicht der aktuelle gesamte Speicherverbrauch im letzten Audiomesspunkt.
- Letzter FFmpeg-Privatspeicher **90537984 Bytes, rund 86.3 MiB**; CAN **632991744 Bytes, rund 603.7 MiB**.
- Letzte physische RAM-Reserve **16918773760 Bytes, rund 15.8 GiB** und Commit-Reserve **47310307328 Bytes, rund 44.1 GiB**. Freier Plattenplatz rund 224 GiB. Diese Messung liegt vor dem Ausfall und nicht erst nach dem Neustart.
- Gegen Ende eine **9.19-Sekunden-Messluecke**, danach stark verlangsamte Fortschritte in Prozess-I/O und CPU-Zaehlern. Dies ist ein Hinweis auf eine Stoerung waehrend des Datei-/Tonpfads, keine Messung der physischen Datentraegerwarteschlange und kein Beweis fuer einen bestimmten Treiber oder defekte Hardware.

Neue unvollstaendige Arbeit `.can-otr-f20acc79d5164bd0a356762d40475668`: `video.mp4` vorhanden; PCM-WAVs **1485570048** und **1486880768** Bytes; keine finale Tonkodierung. Damit erneut **Dekodieren/PCM-Zusammenstellung** als zuletzt erreichter Audioschritt, vor Samplepruefung, AAC-Ueberlappungskorrektur und eigentlicher kontinuierlicher Neukodierung. Dateizeitstempel und zuletzt gespeicherte I/O-Zaehler koennen bei erzwungenem Neustart hinter den verbliebenen Dateidaten zurueckliegen.

Wichtige Eingangsabweichung: Der nun gesicherte Filter beginnt bei **579.960 s**, die erfolgreichen kontrollierten Teil-1-Laeufe begannen bei **583.200 s**. Die weiteren Keeps entsprechen den korrigierten Grenzen. Deshalb **kein identischer Schnittauftrag** gegenueber den positiven kontrollierten Teil-1-Tests; weder allein der Motorwechsel noch allein die geaenderte erste Schnittgrenze sind als Ursache bewiesen. Die frueheren ungeschuetzten Ausfaelle betrafen ebenfalls unterschiedliche erste/letzte Grenzen.

Windows: Kernel-Power 41 um **22:14:27**, EventLog 6008 nennt unerwartetes Herunterfahren um **22:12:38**. Die auf Warnungen/Fehler begrenzten System-/Anwendungsabfragen ab 22:00 liefern bislang keine Fehlermeldung vor dem Neustart. Keine Ursache aus Kernel-Power ableiten. C: liegt laut Speicherverwaltung auf **WD_BLACK SN850X HS 2000GB / NVMe**, gemeldeter Zustand Healthy/Online; dies schliesst einen zeitweiligen Treiber-/I/O-Fehler nicht aus. Erweiterte Zuverlaessigkeitszaehler waren ueber die Abfrage nicht verfuegbar.

Zusaetzlich nur lesend identifiziert: laufende Dateisystemtreiber **Acronis File Protector**, **Acronis NG Antimalware Driver** und **Micron SSD Cache (mtihint 1.7.3.0)**. Die zugehoerigen FilterManager-Meldungen stammen vom Neustart, nicht vom Zeitpunkt des Stillstands. Diese Filter sind eine **zu pruefende Wechselwirkung im Datei-/Speicherpfad, keine nachgewiesenen Verursacher**. Keine Sicherheitssoftware, kein Cache, Treiber oder Dienst wurde deaktiviert oder veraendert.

Originale und unvollstaendige Arbeitsdateien bleiben unveraendert. Ressourcenprotokoll, Graph, Job-Inventar, App-Log, Windows-Ereignisse und kompakte Systemdaten separat unter `.build/mad-max-freeze-20261020/freeze-under-guard` gesichert. Der lokale CAN-Teststarter verweigert weitere Starts ueber `TESTSTART-GESPERRT.txt`; kein weiterer Schnitt und keine belastende Diagnose gestartet. Keine Produktaenderung, kein neuer Build, kein Commit/Push, keine erneute Freigabe. Build 34 bleibt zurueckgezogen.

Naechster Schritt: gespeicherte I/O-/Zeitverlaeufe und den Windows-Datentraeger-/Filtertreiberpfad pruefen. Vor gezielten, vom Nutzer abgestimmten Aenderungen an Cache-/Sicherheitssoftware keine weiteren Wiederholungen mit der Grossdatei. Eine Produktloesung ist weiterhin nicht nachgewiesen.

## Vorbereiteter Versionsvergleich in der VM

Nutzer schlaegt alten Stand mit otr-can 0.1.5 und neuen Stand in einer VM vor. Empfehlung: zunaechst beide unveraenderten Kombinationen nacheinander in der VM vergleichen, Build 34/0.1.6 und Build 32/0.1.5; der alte Stand ist auf dem Arbeitsrechner kein belegter Schutz, da der PCM-Pfad schon vor 0.1.6 bestand. VM bleibt vom Host-/Datentraegerbetrieb abhaengig, keine umfassende Hostschutzgarantie.

Separate lokale Furiosa-letzter-Fehlerfall-VM.cutlist aus dem gesicherten letzten Filter erzeugt, erste Grenze 579.960 Sekunden, fuenf Keeps, Gesamtdauer 6861.560 Sekunden. Mit den Parser-DLLs von Build 32 und installiertem Build 34 erfolgreich geladen und Grenzen geprueft. Beide vorhandenen Portable-ZIPs anhand eingefrorener Release-SHA-256 bestaetigt. Protokoll-Version des Motors kontrollieren, weil Portable-Fassungen dieselben benutzerspezifischen Werkzeugpfade teilen koennen. Anleitung VM-VERSIONEN-VERGLEICH.md im lokalen Diagnoseordner. Kein VM-/Hostschnitt gestartet, kein Release veraendert oder veroeffentlicht; bisherige Teststartersperre bleibt bestehen. VM-Ergebnisse noch offen.

## VM auf einem anderen Host: zwei erfolgreiche otr-can-Laeufe gemeldet

Der Nutzer stellt klar: Die Test-VM laeuft auf einem **anderen Host**, nicht auf dem vom Stillstand betroffenen Arbeitsrechner. Der zuvor fuer eine VM auf demselben Host genannte gemeinsame physische Datentraegerpfad gilt deshalb nicht fuer diesen Vergleich. Die Umgebung des betroffenen Arbeitsrechners bleibt von diesen VM-Laeufen getrennt.

Der Nutzer meldet zwei erfolgreiche Laeufe mit otr-can 0.1.6 in dieser VM und sendet ein vollstaendiges Erfolgsprotokoll: 307470 Quellframes, 171458 Video-Ausgabepakete, je 329199360 Samples fuer AAC/AC3 bei 48 kHz, kontinuierliche rohe Audio-PTS sowie erfolgreiche finale MP4- und CAN-Ausgabepruefungen. Quelle liegt laut VM-Protokoll unter C:/Users/jmagu/Downloads. Anschliessend startet der Nutzer MP4Box; dessen Ergebnis und ein danach folgender otr-can-Lauf stehen noch aus. Keine lokale Hostmessung auf diese entfernte VM uebertragen; persoenliche Bild-/Tonpruefung nicht aus Logs abgeleitet.

**Weiterhin unterschiedliche erste Schnittgrenze:** Das gepostete VM-Protokoll beginnt bei **583.200 Sekunden** und entspricht damit den zuvor erfolgreichen begrenzten Teil-1-Auftraegen. Der letzte auf dem Arbeitsrechner eingefrorene Auftrag begann bei **579.960 Sekunden**, bei ansonsten passenden korrigierten Keeps. Zwei positive VM-Laeufe sind damit ein positiver Vergleich mit derselben grossen Quelldatei, aber noch keine exakte Gegenprobe des letzten Fehlerauftrags. Dafuer ist die vorhandene Furiosa-letzter-Fehlerfall-VM.cutlist mit Beginn 579.960 und 6861.560 Sekunden Gesamtdauer vorgesehen. Ursache und verantwortliche Komponente bleiben offen; weder Treiberfehler noch neue Motorregression bewiesen. Keine Aufhebung der lokalen Teststartersperre oder Release-Ruecknahme.

### VM: Altersfreigabe-Korrektur und anschliessender MP4Box-Lauf

Nutzer erklaert den Beginn 583.200 Sekunden mit dem Entfernen der Altersfreigabe. Fuer die laufenden Vergleiche bleibt diese gewuenschte Fassung erhalten. Die erste Grenze und die weiteren Keeps entsprechen auch einem frueheren auf dem Arbeitsrechner fehlgeschlagenen ungeschuetzten Auftrag; der VM-Test ist deshalb ein brauchbarer Vergleich zu diesem frueheren Fall, auch wenn der letzte geschuetzte Ausfall bei 579.960 Sekunden begann. Keine Ruecknahme der Nutzerkorrektur verlangt.

Neues beigefuegtes VM-MP4Box-Protokoll aus 63935c32-3c70-45eb-86ae-65a6f296a94f: alle fuenf Segmente verarbeitet und zusammengefuegt, danach Uebergangspruefung bei Abschnitt 2 mit derselben Meldung zu ueberlappender/ungeordneter Segmentpraesentation abgebrochen; fertige Ausgabe erhalten, Pruefstellen nur berechnet. Damit tritt diese Pruefwarnung ebenfalls in der VM auf dem anderen Host auf. Daraus kein Zusammenhang mit dem Rechnerstillstand bewiesen. Der Nutzer startet nun wieder otr-can in derselben VM-CAN-Sitzung; Ergebnis dieses Motorwechsels noch offen. Keine lokalen Tests gestartet oder Teststartersperre aufgehoben.

### VM: otr-can nach MP4Box ebenfalls erfolgreich

Beigefuegtes Protokoll 1fa6f406-da7e-4771-bc61-bebce46992a4 bestaetigt einen weiteren vollstaendigen otr-can-0.1.6-Erfolg in der VM auf dem anderen Host, im laufenden Testablauf nach MP4Box mit dessen Uebergangspruefungswarnung. Wieder Beginn 583.200 Sekunden, 171458 Video-Ausgabepakete, je 329199360 Samples fuer AAC/AC3, kontinuierliche rohe Audio-PTS und finale MP4-/CAN-Ausgabepruefungen erfolgreich. Damit laut Nutzerablauf mindestens drei positive otr-can-Laeufe (zwei vor MP4Box gemeldet, dieser danach). Der Nutzer meldet mit 'und nochmals' eine weitere Wiederholung an; deren Ausgang nicht vorwegnehmen.

Die MP4Box-Pruefwarnung und der Wechsel MP4Box zu otr-can sind in dieser anderen Umgebung somit zusammen mit einem erfolgreichen Folgeauftrag beobachtet. Das belegt weder deren generelle Unbedenklichkeit auf dem Arbeitsrechner noch einen bestimmten lokalen Treiber als Ursache. Unterschiede bei Host, VM, Werkzeugversionen und Startbedingungen bleiben zu beachten. Der komplette Rechnerstillstand ist in der VM bislang nicht nachgestellt. Persoenliche Bild-/Tonabnahme nicht aus dem technischen Abschluss abgeleitet; keine Produktkorrektur oder erneute Releasefreigabe.

## Tagesabschluss auf Nutzerwunsch

Der Nutzer beendet die heutige Testreihe ('fuer heute der letzte Test'). Die letzte Anlage 849c5e0a-2fd5-4255-b273-9feb12a0235e endet technisch erfolgreich, enthaelt aber dieselbe Ausgabe-Auftragskennung ab1f094860f24ecab4be12db6a23eab4 wie das unmittelbar vorherige VM-Protokoll. Deshalb aus dieser erneuten Anlage keinen zusaetzlichen unabhaengig belegten vierten VM-Schnitt zaehlen. Gesichert bleiben die zwei zuvor vom Nutzer gemeldeten VM-Erfolge und der danach per Protokoll bestaetigte Erfolg im Testablauf nach MP4Box; keine neuen Fehler in den gesendeten VM-Protokollen.

Abschlussstand: auf dem Arbeitsrechner kompletter Systemstillstand bei Mad-Max-Teil-1-PCM-Erzeugung, zuletzt auch mit aktiven Ressourcenlimits; lokale Diagnosedaten erhalten und Teststarter gesperrt. Mehrere weitere HD-/HQ-Faelle dort unter Schutz erfolgreich, VM auf anderem Host mit otr-can 0.1.6 und Motorwechsel bisher erfolgreich. Ursache offen; keine nachgewiesene Produktkorrektur und keine pauschale Zuweisung an Cache, Acronis, Hardware, App oder Motor. Build 34 bleibt zurueckgezogen, keine neuen Pakete/Commits/Pushs/Veröffentlichungen. Fuer heute keine weiteren Medienlaeufe oder Tests gestartet. Naechste Untersuchung kann die vorhandenen Belege sowie Werkzeug-/Treiberunterschiede vergleichen; keine Fortsetzung oder Automation eingeplant.

## Nachtrag am 22.10.2026: weiterer erfolgreicher Furiosa-Nutzerlauf

Neues Nutzerprotokoll mit otr-can 0.1.6: 307470 Quellframes, fünf Behalteabschnitte, **171458** geprüfte Video-Ausgabepakete und je **329199360** dekodierte Samples für AAC/AC3 bei 48 kHz. Beide Audio- und finale MP4-/CAN-Prüfungen erfolgreich abgeschlossen. Der Nutzer bestätigt anschließend ausdrücklich **Bild, Ton, Schnitt und Drei-Sekunden-Vorlauf**.

Beginn **583,200 s**, damit keine identische Gegenprobe des letzten geschützten Fehlerauftrags ab 579,960 s. Aus dem Benutzerpfad allein wird kein Ausführungshost oder Ressourcen-/Treiberzustand abgeleitet. Dieser zusätzliche positive Lauf hebt weder den früheren Fehlerbefund noch die dokumentierte lokale Teststartersperre auf; eine Ursache oder Korrektur ist weiterhin nicht nachgewiesen. Nur Dokumentationsnachtrag, kein eigener Schnitt oder Diagnose-/Buildlauf. [Protokollauswertung und persönliche Abnahme](OTR-CAN-FURIOSA-PRAXIS-20261022.md).

Anschließend bestätigt der Nutzer **zwei weitere otr-can-Läufe mit Mad Max/Furiosa, „check bei allen parametern“**. Damit drei positiv gemeldete Läufe in der Rückmeldeserie vom 22.10., davon einer mit vollständigem vorgelegtem Abschlussprotokoll. Für die zwei zusätzlichen Wiederholungen fehlen Einzelprotokolle; identische Auftragsgrenzen und dieselbe CAN-Sitzung werden daraus nicht abgeleitet. Positive Wiederholungsbefunde ergänzt, Ursache der früheren Systemhänger weiterhin offen. [Umfang der Wiederholungsbestätigung](OTR-CAN-FURIOSA-PRAXIS-20261022.md#zwei-weitere-positive-nutzerläufe-am-22102026).
