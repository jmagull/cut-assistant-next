# Schnittprüfung – Integration der gemessenen Ausgabezeiten

Stand: **18.10.2026**. Lokale Weiterentwicklung nach CAN **1.0.0 · Build 32**, auf ausdrücklichen Nutzerauftrag nach positiver Vera-Prototyp-Prüfung. **Die veröffentlichte Version, der Release-Tag und alle Release-Dateien bleiben unverändert. Keine neue Buildnummer und kein Commit.**

## Ergebnis

Die gemessenen Prüfpositionen sind jetzt im regulären MP4Box-Schnittablauf angeschlossen. Die Anwendung erstellt das Abschlussmodell **nach** dem erfolgreichen Schnitt aus einem strukturierten Ergebnis. Abschlussfenster, kopierte Hinweise und Kontrollplayer verwenden dieselben Ausgabezeiten.

MP4Box bleibt Standard. Schnittparameter, Keep-Bereiche, Cutlist und MP4Box-Modus sind unverändert. otr-can bleibt als Alternative erhalten und verwendet über den gemeinsamen Ergebnisvertrag weiterhin seine unveränderten Planzeiten.

Ein neuer vollständiger Vera-Schnitt durch die integrierten Produktklassen ergab:

| Prüfstelle | Bestätigter Ausgabeübergang | Angeforderter Prüfstart |
|---|---|---|
| 1 | 00:17:00,720 | **00:16:57,720** |
| 2 | 00:35:20,840 | **00:35:17,840** |
| 3 | 01:05:13,880 | **01:05:10,880** |

Die zusätzliche Analyse lief vor der Veröffentlichung und Bereinigung der temporären Segmente. Anschließend waren keine Arbeitssegmente mehr vorhanden. Die Ausgabedatei wurde anhand ihrer gemessenen Identität erneut geprüft; alle drei Prüfstarts und die passenden neuen Originalbilder sind im **regulären CAN-Kontrollplayer** mit echten HWNDs/libmpv-Kontexten bestätigt. Auch die beiden doppelten Video-PTS werden in diesem konkreten Fall richtig getroffen. Keine pauschale Zeitkorrektur und keine Sonderbehandlung nur des ersten Übergangs.

## Bedienung und Fehlerfälle

Im Abschlussfenster steht bei erfolgreich bestätigten MP4Box-Zeiten **„Zeitangaben im fertigen Film geprüft.“**. Direkt darunter steht der getrennte Hinweis:

> MP4Box kann Schnittgrenzen verschieben. Geprüfte Prüfpositionen stellen fehlende Bilder nicht wieder her.

Bei einer gescheiterten Analyse werden die bisherigen Keep-Summen verwendet, aber ausdrücklich als **„Nur berechnete Prüfstellen: Die tatsächlichen Übergänge sind nicht bestätigt.“** bezeichnet. Der konkrete Grund steht im Protokoll. Der Kontrollplayer verspricht bei berechneten Stellen keinen nachgewiesenen tatsächlichen Übergang.

Die folgende Unterscheidung gilt:

- Fehler/Abbruch beim Schneiden: bisheriger Fehler-/Abbruchweg, keine vorzeitige Veröffentlichung.
- Fehler, ungeeignete Zeitstruktur oder fehlendes ffprobe bei der **anschließenden Zusatzprüfung**: fertigen Film veröffentlichen, Prüfstellen ausdrücklich berechnet.
- Abbruch/Timeout während dieser Zusatzprüfung: ebenfalls den bereits fertig geschnittenen Film erhalten und auf den Prüfungsabbruch hinweisen.
- Geänderte Ausgabedatei vor dem Öffnen einer bestätigten Prüfstelle: verständliche Ablehnung statt Verwendung alter gemessener Zeiten für eine andere Fassung.

Die Drei-Sekunden-Berechnung des Players bleibt erhalten. Anfang startet bei null, das Ende verwendet weiterhin die tatsächlich geladene Filmdauer; kurze Vorläufe werden auf den Dateianfang begrenzt.

**Der bei Vera nachgewiesene Verlust der 102 Anfangsbilder wird nicht behoben.** Eine automatische quantitative Bildverlust-/Zusatzbildanalyse des Originals ist nicht Bestandteil dieser Integration. Der Hinweis unterscheidet die zuverlässigere Positionierung von einer möglichen inhaltlichen Abweichung des Schnittmotors.

## Architektur und Dateien

| Bereich | Änderung |
|---|---|
| Core: `CutResult.cs`, `ICutEngine.cs` | Unveränderliches Schnittergebnis mit Ausgabezeitachse, Herkunft (geprüft, geplant, unbestätigt berechnet) und optionaler Dateiidentität. `RunWithResultAsync` ergänzt den vorhandenen Vertrag kompatibel; der Standardweg erhält die bisherigen Motoraufrufe und Planzeiten. |
| Media: `FfprobeCutOutputAnalyzer.cs`, `FfprobeCutOutputProbe.cs`, `CutOutputProbeModels.cs` | Erprobte Segment-/Paketzuordnung mit rationalen Ausgabe-PTS und lokaler Dekodierbestätigung. Prozessaufrufe sind asynchron und abbrechbar. |
| Media: `CutOutputIdentityVerifier.cs` | SHA-256 sowie Dateigröße und Änderungszeit binden die gemessenen Zeiten an die betreffende Ausgabedatei. |
| Media: `Mp4BoxCutWorkflow.cs`, `ICutOutputAnalyzer.cs` | Zusatzanalyse vor dem Verschieben/Löschen temporärer Dateien; Fehler oder Abbruch der Zusatzprüfung verlieren den geschnittenen Film nicht. Aufräumfehler werden protokolliert. |
| App: `ConfiguredCutOutputAnalyzer.cs`, `ConfiguredMp4BoxCutServiceFactory.cs`, `Mp4BoxCutService.cs` | ffprobe über die vorhandenen konfigurierten Werkzeugpfade einbinden; Ergebnis und verständliche Hinweise liefern. |
| App: `MainWindow.xaml.cs`, `CutCompletionViewModel.cs`, `CutCompletionDialog.xaml` | Abschlussmodell nach dem Schnitt erzeugen, geprüfte Positionen anzeigen/kopieren, Ersatzzeiten und Bildverlust-Hinweis erkennbar darstellen. |
| App: `CutReviewViewModel.cs` | Identität vor dem Laden prüfen und Herkunft der Prüfstellen anzeigen. Seek, Laden, Abspielen, Navigation und Freigabe bleiben im vorhandenen Playerablauf. |
| Tests | Ergebnisvertrag, vollständige Paketfolge, Dekodieranker, Fehler-/Abbruchpfade, Dateiaustausch, Ergebnisübergabe beider Motoren sowie Anzeige/Player absichern. |
| Dokumentation/Werkzeugprototyp | README und dieser Bericht; der frühere isolierte Prototyp erhält eine eingefrorene Kopie seines bisherigen Kontroll-ViewModels, damit er mit der alten Referenzlaufzeit weiter baubar bleibt. Die Produktintegration verwendet diese Kopie nicht. |

Keine neue Produktionsabhängigkeit, keine DirectShow-/Codec-Paket-Abhängigkeit, keine absolute Benutzerpfade im Produktcode.

### Messverfahren

1. ffprobe liefert die vollständigen komprimierten Videopaketfolgen einschließlich SHA-256.
2. Die Pakete der Ausgabe müssen exakt der geordneten Verkettung der erzeugten Segmente entsprechen.
3. Die tatsächlichen Übergangszeiten stammen aus der **fertigen** Videozeitachse, nicht aus Dateidauersummen oder nomineller Framerate.
4. Ein neuer Abschnitt muss einen eindeutigen ersten Zugriffspunkt besitzen. Dieser wird zusätzlich mit passender Paketposition und PTS dekodiert bestätigt.
5. Die Ausgabedatei erhält eine Identität aus Größe, Änderungszeit und SHA-256. Verschieben an den endgültigen Ausgabepfad erhält diese Zuordnung.
6. Vor einem neuen Laden im Kontrollplayer wird die vollständige Identität geprüft; bei Navigation in derselben geladenen Sitzung werden Größe und Änderungszeit geprüft.

Damit entstehen die in der Messung gefundenen kumulierten Fehler von 40/80/120 ms durch bloße Dateidauersummen nicht. Unterschiedliche Segmentverschiebungen werden unabhängig voneinander behandelt.

## Prüfungen

**Release-Build der gesamten Lösung: 0 Warnungen, 0 Fehler. Alle 1385/1385 Tests bestanden, 0 übersprungen.**

| Testsuite | Bestanden |
|---|---:|
| Core | 199 |
| Cutlists | 72 |
| Media | 224 |
| App | 890 |
| Gesamt | **1385** |

Die 38 zusätzlichen Fälle gegenüber dem Ausgangsstand von 1347 Tests betreffen unter anderem:

- verifizierte und ausdrücklich geschätzte Ergebnisse, unveränderliche und geordnete Ausgabezeiten;
- verschiedene Zeitabstände und Zeitbasen, kumulierte Abweichungen und doppelte PTS;
- falsche Reihenfolge, fehlende/zusätzliche/geänderte Pakete sowie fehlende oder abweichende Dekodieranker;
- negative oder ungeeignete Präsentationszeiten, vorgelagerte Bilder und echte Überlappungen;
- Einzelabschnitt einschließlich ausdrücklich verworfenem Nachlaufpaket am Dateiende;
- Abbruch, Analysefehler und ungültiges Analyseergebnis bei weiterhin erhaltenem Schnittfilm;
- Messung vor Bereinigung und Veröffentlichung, Identität nach dem Verschieben;
- nachträglichen Dateiaustausch trotz gleicher Größe und wiederhergestellter Änderungszeit;
- neue Ergebnisübergabe für MP4Box und den bisherigen otr-can-Erfolgsweg;
- geprüfte Zeiten in Abschlussfenster, Kopiertext und Kontrollplayer.

Keine Tests entfernt oder abgeschwächt.

### Reale zusätzliche Nachweise

- **Vera:** Neuer MP4Box-Schnitt mit unveränderten Anforderungen. 5292,800 s, drei bestätigte Übergänge, bereinigte Segmente und gültige SHA-256-Zuordnung. Drei tatsächliche Seek-Anforderungen und pixelgleiche neue Abschnittsanfänge im regulären Kontrollplayer geprüft.
- **Künstlicher Film, vier Abschnitte:** Gewünschte Starts teils vor und teils nach den tatsächlich übernommenen Zugriffspunkten. Planübergänge 3,000 / 5,400 / 8,400 s; bestätigte Ausgabezeiten 2,200 / 5,400 / 8,400 s. Drei native Übergangsprüfungen einschließlich auf null begrenztem erstem Vorlauf.
- **Künstlicher Einzelabschnitt:** Tatsächlicher integrierter Schnitt und bestätigtes Ergebnis ohne innere Übergänge. Anfang und Ende im realen Kontrollplayer geprüft.
- **Abschlussanzeige:** Reguläre WPF-Komponente mit dem neuen Vera-Ergebnis gerendert und visuell geprüft. Zeiten und Hinweis lesbar, lange Inhalte über den vorhandenen Scrollbereich erreichbar.
- **Ursprünglicher Prototyp:** Weiter baubar, 0 Warnungen/Fehler und seine 32 Prüfbedingungen weiterhin bestanden.

Die zusätzlichen realen Prüfungen betreffen Videozuordnung und Bedienablauf. Keine neue persönliche Gesamtfilm-/Ton-/Synchronitätsabnahme und kein neuer otr-can-Motorlauf. Die persönliche positive Vera-Prototyp-Prüfung bleibt im früheren Bericht dokumentiert.

### Persönliche Rückmeldung zur integrierten Fassung

Nach Bereitstellung der lokalen CAN-Prüffassung bestätigt der Nutzer am **18.10.2026**: „sieht gut aus startet immer 3 sekunden vor dem Schnitt“. Damit ist der gewünschte Drei-Sekunden-Vorlauf auch persönlich bestätigt. Diese Rückmeldung bezieht sich auf den Start der Schnittprüfung; eine gesonderte Ton-/Synchronitäts- oder Gesamtfilmabnahme ist darin nicht enthalten.


### Weiterer persönlicher Praxisfall: Die Teufelsbrigade

Am **18.10.2026** bestätigt der Nutzer mit beigefügtem Protokoll und Cutlist: **„passt alles :-)“**. Dies wird als positive persönliche Rückmeldung zu diesem zusätzlichen Schnitt-/Prüfablauf festgehalten.

- Quelle: **Die Teufelsbrigade**, OTR-HQ-AVI, H.264, 25 fps, MP3-Ton.
- Cutlist: sechs zu behaltende Abschnitte, somit fünf innere Übergänge; Cutlist-ID 1825616.
- Protokoll: verlustfreies Umverpacken nach MP4 mit Kopieren von Video und Ton, vollständige und exakte Videopaketanzahl sowie bestandene Stream-/Bildraten-/Laufzeitprüfung. Anschließend sechs MP4Box-Segmente, Zusammenfügen und Zusatzanalyse bis Abschnitt 6 von 6.
- FFmpeg meldet beim Umverpacken fehlende Paketzeitstempel. Diese Warnungen bleiben als Beobachtung dokumentiert; die nachfolgende Arbeitsdateiprüfung ist laut Protokoll bestanden.
- Der beigefügte Text endet bei der Abschnittsprüfung und enthält keine abschließenden gemessenen Übergangszeiten. Der erfolgreiche persönliche Gesamteindruck stammt aus der ausdrücklichen Nutzerrückmeldung. Daraus werden keine zusätzlichen exakten Messwerte oder eine gesonderte vollständige Ton-/Synchronitätsabnahme abgeleitet.

Die genannte Original-Cutlist und das übergebene Protokoll bleiben unverändert. Kopien samt Prüfsummen liegen ausschließlich im ignorierten Nachweisordner unter `.build/transition-integration-20261018/personal-teufelsbrigade/`.


### Weiterer persönlicher Praxisfall: Die Hart 2

Am **18.10.2026** bestätigt der Nutzer zur HD-MP4-Aufnahme **Die Hart 2**: **„auch hier stimmen die Übergänge mit 3 Sekunden Vorlauf :-)“**. Damit sind die Übergänge und ihr Drei-Sekunden-Vorlauf in diesem weiteren realen Praxisfall persönlich positiv geprüft.

Das beigefügte Protokoll zeigt fünf MP4Box-Segmente und vier innere Übergänge. Bei **allen vier** Abschnittswechseln meldet die Zusatzanalyse gleiche Bildzeiten und die Prüfung des dekodierten neuen Anfangs. Der Lauf endet mit **„Fertig.“**. Dieser Fall ergänzt die persönliche Praxisprüfung ausdrücklich um wiederholte Bildzeitstempel an sämtlichen inneren Übergängen.

Exakte gemessene Ausgabezeiten sind im Text nicht enthalten und werden nicht aus den Schnittanforderungen errechnet. Eine gesonderte vollständige Ton-/Synchronitätsabnahme wird aus der Rückmeldung nicht abgeleitet. Das beigefügte Protokoll bleibt unverändert; eine Kopie mit Prüfsumme liegt im ignorierten Nachweisordner `.build/transition-integration-20261018/personal-die-hart-2/`.


### Weiterer persönlicher Praxisfall: Winter Spring Summer or Fall

Am **18.10.2026** bestätigt der Nutzer zur HQ-MP4-Aufnahme **Winter Spring Summer or Fall**: **„auch hier stimmt alles :-)“**. Dies ist eine weitere positive persönliche Rückmeldung zum Schnitt-/Kontrollablauf.

Das beigefügte Protokoll zeigt fünf MP4Box-Segmente, vier innere Übergänge, die Zusatzanalyse aller fünf Abschnitte und den Abschluss **„Fertig.“**. An den Übergängen **1, 2 und 3** werden gleiche Bildzeitstempel und die Prüfung des dekodierten neuen Anfangs gemeldet. Am vierten Übergang enthält das Protokoll keine solche Meldung. Damit ergänzt dieser Fall die Praxisprüfung um unterschiedlich behandelte Abschnittswechsel innerhalb desselben Films.

Exakte gemessene Ausgabezeiten sind im Text nicht enthalten. Die positive persönliche Rückmeldung wird festgehalten, ohne daraus zusätzliche Messwerte oder eine gesonderte vollständige Ton-/Synchronitätsabnahme abzuleiten. Das Protokoll bleibt unverändert; eine Kopie mit Prüfsumme liegt ausschließlich im ignorierten Nachweisordner `.build/transition-integration-20261018/personal-winter-spring-summer-or-fall/`.


### HD-Praxisfall Radical: persönliche Prüfung positiv, berechnete Prüfstelle, bekannter Blitzer

Am **18.10.2026** bestätigt der Nutzer zu **Radical – Eine Klasse für sich**, HD-MP4-Aufnahme vom 08.10.2026: **„Übergänge stimmen, incl. Blitzer bei 00:57:26.840 , wie gehabt :-)“**.

Das Protokoll zeigt zwei MP4Box-Segmente und somit einen inneren Übergang. Die Zusatzanalyse meldet ausdrücklich: **„Die tatsächlichen Übergänge konnten nicht bestätigt werden. Die Prüfstellen sind nur aus dem Schnittplan berechnet.“** Grund: **„Überlappende oder ungeordnete Segmentpräsentation ist nicht eindeutig zuzuordnen.“** Danach endet der Lauf mit **„Fertig.“**.

Damit ist die persönliche Übergangskontrolle positiv, während das Programm korrekt auf **berechnete** Zeiten zurückgegriffen hat. Dieser Fall ist **kein** Nachweis automatisch bestätigter Ausgabezeiten. Der fertige Schnitt bleibt bei nicht eindeutiger Zusatzanalyse erhalten.

Der Nutzer sieht den bekannten MP4Box-Blitzer weiterhin, jetzt bei **00:57:26,840**. Die frühere Untersuchung [Radical-MP4Box-Werbeblitzer vom 16.10.2026](RADICAL-MP4BOX-WERBEBLITZER-20261016.md) dokumentiert einen älteren Ausgabezustand und eine andere gemeldete Position. Die neue Nutzerposition wird eigenständig festgehalten; kein erneuter Paket-/Bildnachweis und keine Ursache der Positionsabweichung behauptet. Der bekannte inhaltliche Schnittfehler ist weiterhin offen.

Das übergebene Protokoll bleibt unverändert; Kopie und Prüfsumme liegen ausschließlich unter `.build/transition-integration-20261018/personal-radical/`. Eine allgemeine Ton-/Synchronitätsabnahme oder eine blitzfreie Ausgabe wird aus dieser Rückmeldung nicht abgeleitet.


### Zweiter HD-Praxisfall: The Place Beyond the Pines

Am **18.10.2026** bestätigt der Nutzer zum zweiten angekündigten HD-Test, **The Place Beyond the Pines**: **„Auch stimmt alles :-)“**. Die persönliche Rückmeldung zum Schnitt-/Kontrollablauf ist damit positiv.

Das Protokoll zeigt fünf MP4Box-Segmente, vier innere Übergänge und die Zusatzanalyse aller fünf Abschnitte. An den Übergängen **2 und 3** werden gleiche Bildzeitstempel mit Prüfung des dekodierten neuen Anfangs gemeldet; an **1 und 4** enthält das Protokoll keine solche Meldung. Der Lauf endet mit **„Fertig.“**, ohne Meldung eines Rückfalls auf berechnete Prüfstellen.

Das Protokoll enthält keine exakten gemessenen Ausgabezeiten. Es wird keine zusätzliche vollständige Ton-/Synchronitätsabnahme aus der Rückmeldung abgeleitet. Originalprotokoll unverändert; Kopie und Prüfsumme liegen ausschließlich unter `.build/transition-integration-20261018/personal-the-place-beyond-the-pines/`.

Damit liegen persönliche Rückmeldungen zu **sechs verschiedenen Aufnahmen** vor: Vera, Die Teufelsbrigade, Die Hart 2, Winter Spring Summer or Fall, Radical und The Place Beyond the Pines. Bei Radical bleiben der dokumentierte Rückfall auf berechnete Prüfstellen und der bekannte MP4Box-Blitzer ausdrücklich bestehen. Die übrigen Rückmeldungen sind keine allgemeine Freigabe sämtlicher Medienformate.


### Separater Vera-Praxisfall mit der Cutlist von IONix

Nach ausdrücklicher Ankündigung **„Vera mit Culist von IONix“** bestätigt der Nutzer am **18.10.2026** zu diesem gesonderten Durchlauf: **„hier stimmt jetzt auch alles , 3 Sekunden Vorlauf klappt prima :-)“**. Damit sind der Kontrollablauf und der Drei-Sekunden-Vorlauf auch mit dieser weiteren Cutlist persönlich positiv geprüft. Die Autorenzuordnung IONix stammt aus der Nutzerangabe; eine zusätzliche Cutlist-Datei wurde zu diesem Durchlauf nicht übergeben.

Das Protokoll zeigt vier MP4Box-Segmente und drei innere Übergänge. Die angeforderten Schnittbereiche sind **570,2713334–1594,76**, **2138,8–3238,92**, **3799,96–5593** und **6136,04–7514,96 s** der Quelle. Diese Werte sind die protokollierten MP4Box-Anforderungen, keine gemessenen Ausgabeübergänge. Die Zusatzanalyse läuft über alle vier Abschnitte und endet mit **„Fertig.“**, ohne Meldung eines Rückfalls auf berechnete Prüfstellen oder gleicher Bildzeitstempel an den Übergängen.

Der Durchlauf bleibt vom ursprünglichen Vera-Test getrennt. Exakte gemessene Ausgabezeiten sind im beigefügten Text nicht enthalten; die frühere Vera-Messtabelle wird dafür nicht übernommen. Aus dem passenden Kontrollvorlauf wird keine Wiederherstellung fehlender Bilder und keine zusätzliche vollständige Ton-/Synchronitätsabnahme abgeleitet.

Kopie und Prüfsumme des unveränderten Protokolls liegen ausschließlich unter `.build/transition-integration-20261018/personal-vera-ionix/`. Der dokumentierte persönliche Praxisstand umfasst jetzt **sieben getrennte Schnitt-/Prüffälle mit sechs verschiedenen Aufnahmen**.

## Build, Erhaltung und transparente Hinweise

Der lokale Buildzähler bleibt ausdrücklich **32**. Deshalb wurde direkt mit `dotnet build` und `CanBuildNumber=32` in einen getrennten Ausgabeordner gebaut; das zählererhöhende Buildskript wurde nicht aufgerufen. Die installierte Anwendung und die veröffentlichten Pakete wurden nicht ersetzt.

Der erste Wiederherstellungsversuch im gesperrten Modus erkannte bereits vorbestehende unterschiedliche Runtime-Vorgaben zwischen den vier Produktions- und den Test-Lockdateien. Statt diese zu verändern, verwendet der Integrationsbuild eigene Wiederherstellungsdateien unter dem ignorierten Diagnoseordner. **Alle acht ursprünglichen Lockdateien bleiben bytegleich zum Ausgangszustand; insbesondere bleiben die vier vorbestehenden lokalen Änderungen erhalten.** Compilerwarnungen wurden nicht unterdrückt.

Die Sandbox-Befehlsausführung reagierte vor Beginn der Arbeiten nicht, selbst bei einfachen Ausgaben. Die harmlose Gegenprüfung außerhalb der Sandbox war erfolgreich. Projektzugriffe, Build und Prüfungen wurden danach über den funktionierenden lokalen Zugang ausgeführt; keine Sandbox-/Benutzereinstellungen verändert.

Der erste Einzelabschnitts-Nativtest enthielt im getrennten Prüfhelfer eine Lautstärkeanweisung vor der Initialisierung eines nie benötigten Übergangsplayers. Der Schnitt und seine Analyse waren dabei bereits erfolgreich. Der Helfer prüft den Einzelabschnitt jetzt ausdrücklich über Anfang und Ende; dieser Nachlauf ist erfolgreich. Das war kein Fehler des Produktablaufs. Die früheren Protokolle bleiben erhalten.

Vor/nach verglichen werden alle zuvor erfassten 601 Dateien. Erwartete Quell-/Dokumentationsänderungen sind ausdrücklich aufgeführt; Release-Dateien, Originalaufnahme, ursprüngliche Cutlist, vorhandene Vera-Ausgaben, persönliche Einstellungen, Lockdateien, Buildzähler, HEAD und Tag bleiben erhalten. Neue echte Medien und Nachweise liegen ausschließlich unter `.build/`. Kein Commit, Push, Tagwechsel oder Release.

## Grenzen und nächster Schritt

Die erprobten Grenzen des Messverfahrens bleiben bewusst sichtbar:

- Zusätzliche vollständige Leseprüfung komprimierter Videodaten kostet Zeit; sie kodiert das Video nicht neu. Die Zusatzprüfung hat eine Grenze von zehn Minuten.
- Zunächst genau eine Videospur mit Zeitstart null. Nicht eindeutige Zugriffspunkte, komplexes Preroll, negative Zeiten oder echte Überschneidungen werden nicht als bestätigt ausgegeben.
- Andere Codecs, GPAC-Versionen und weitere reale Aufnahmen benötigen weitere Praxisprüfung. Mehrere Tonspuren werden nicht als Bildanker verwendet.
- Änderungen während einer bereits laufenden Wiedergabesitzung lassen sich ohne dauerhafte Dateisperre nicht vollständig ausschließen.
- Korrigierte Prüfzeiten beheben weder Bildverluste noch Werbeblitzer oder andere inhaltliche Schnittprobleme.

Der Drei-Sekunden-Vorlauf der lokalen Integrationsfassung ist persönlich bestätigt. Als nächster Schritt sind weitere eigene MP4Box-/otr-can-Aufträge sinnvoll. Versionserhöhung, Paketierung und Veröffentlichung bleiben gesondert zu entscheiden.

## Lokale Nachweise

`.build/transition-integration-20261018`:

- `BEFORE.json`, `GIT-BEFORE.json`, `FINAL.json`: Ausgangs- und Erhaltungsnachweis;
- `RESTORE.log` und `RESTORE-ISOLATED.log`: transparenter erster Versuch und erfolgreiche isolierte Wiederherstellung;
- `NuGetIsolation.targets`, `locks/`: eigene Wiederherstellungsdateien;
- `BUILD.log`, `FORMAT.json`, `FORMAT-VERIFY.log`, `TEST-VERIFIED.log` und `verified-test-results/`;
- `real-vera/result.json`, `INTEGRATION-PROOF.json`, `NATIVE-PROOF.json`, `PROTOCOL.log`, `completion-content.png` sowie neue Vera-Testdatei;
- `real-synthetic/`, `real-single/` und zugehörige Laufprotokolle;
- `qa-harness/`: isolierter Helfer mit Referenzen auf die tatsächlich integrierten Produkt-Assemblies;
- `PROTOTYPE-BUILD.log` und `PROTOTYPE-SELFTEST.json`: Erhaltung des ursprünglichen Referenzwerkzeugs.

Vollständige lokale Benutzerpfade verbleiben in diesen ignorierten Nachweisen.
