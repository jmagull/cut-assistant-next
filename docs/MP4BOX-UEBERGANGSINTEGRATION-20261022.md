# Begrenzte MP4Box-Übergangszuordnung am 22.10.2026

Nach positiver persönlicher Prüfung von Schnitt, Bild, Ton und Drei-Sekunden-Vorlauf im separaten Kontrollplayer wurde die begrenzte Integration ausdrücklich beauftragt. Die reguläre MP4Box-Ergebnisprüfung kann jetzt den bei Furiosa beobachteten kleinen Rücksprung der Bildzeiten zuordnen. Die Kontrollfenster verwenden anschließend die Positionen aus der fertigen Datei statt summierter Planzeiten.

## Umfang und Grenzen

Die vollständige geordnete SHA-256-Zuordnung aller Videopakete zwischen den erzeugten Segmenten und der fertigen Datei bleibt Voraussetzung. Die Ausnahme betrifft ausschließlich H.264 mit folgenden zusätzlichen Merkmalen:

- Der Rücksprung ist positiv und kleiner als ein tatsächlich beobachteter Abstand von 20 oder 40 ms zwischen den ersten drei neuen Bildern. Die nominale oder durchschnittliche Bildrate wird dafür nicht verwendet.
- Nur das letzte alte Bild liegt hinter dem ersten neuen Bild. Der vorletzte alte Zeitstempel liegt davor; der letzte alte Anker ist zugleich das letzte Paket seines Abschnitts. Die beteiligten Grenzzeiten und Paketpositionen sind eindeutig.
- Die Paketdauern des letzten alten und ersten neuen Pakets teilen genau einen solchen Bildabstand. Vollständige Video-DTS steigen im gesamten Film streng auf.
- Der lokale Decoder bestätigt unmittelbar nacheinander vorletztes altes, letztes altes sowie erstes, zweites und drittes neues Bild. Außer dem einen Grenzrücksprung darf das Decoderfenster keine gleichen oder rückläufigen Bildzeiten enthalten.

Andere Codecs, größere/mehrdeutige Überlappungen, fehlende Zeitwerte, Paketabweichungen und unbestätigte Decoderanker bleiben beim vorhandenen Rückfall auf berechnete Prüfstellen. Bestehende Regeln für beschädigte/verworfene Pakete, Dateiänderungen und Abbruch bleiben bestehen. ffprobe läuft mit einem Decoderthread. Keine neue Produktionsabhängigkeit und keine Änderung an MP4Box-Schnittbereichen oder Wiedergabesteuerung.

Der Ergebnisvertrag ergänzt `CutReviewTimingKind.Located` und `CutOutputTimeline.HasOverlappingPresentationTimes`. Solche Ergebnisse benötigen eine Ausgabeidentität und tatsächliche Übergänge. Sie dürfen nicht als `Measured`, `Estimated` oder `Planned` ausgegeben werden. Nicht überlappende bestätigte Ergebnisse bleiben `Measured`.

Abschlussfenster, Zwischenablage und Kontrollplayer erklären die Zuordnung und kleine Überlappung. Der Drei-Sekunden-Vorlauf und die Prüfung der Ausgabeidentität vor dem Laden bleiben erhalten. Die Zuordnung stellt keine fehlenden Bilder wieder her und bescheinigt keine vollständige Bildzählung oder Tonsynchronität.

## Geänderte Bereiche

Produktiver Code: `Core/Cutting/CutResult.cs`, die drei `Media/Analysis/CutOutput*`-/`FfprobeCutOutput*`-Dateien sowie `App/Services/Cutting/Mp4BoxCutService.cs`, `CutCompletionViewModel.cs` und `CutReviewViewModel.cs`. Zugehörige Core-, Media- und App-Tests ergänzt; keine vorhandenen Negativfälle entfernt.

Das separate Werkzeug `tools/prototypes/DiagnosisReview` ergänzt den QA-Aufruf `--cut-and-native-test`: neuer Schnitt über den tatsächlich gebauten internen App-Service, anschließend Übergabe genau dieses produktiven Ergebnisses an die aktuellen Kontrollplayerquellen. Der bisherige reine Diagnosemodus bleibt `Estimated`. Keine Einbindung des Prüfwerkzeugs in die ausgelieferte Anwendung.

## Technische Prüfung

Release-Build mit festem `CanBuildNumber=34`, ohne Warnungen oder Fehler. **1411/1411 Solution-Tests**: Core 200, Media 246, App 893, Cutlists 72. Zusätzlich **40/40 isolierte Diagnose-/Übergabetests**. Nur betroffene C#-Dateien formatiert; Formatprüfung und `git diff --check` erfolgreich.

Restore aus vorhandenem lokalem Cache mit isolierten NuGet-Lockdateien. Der normale Buildwrapper wurde wegen des ausdrücklich eingefrorenen Buildzählers nicht verwendet; stattdessen direkter vollständiger Release-Build mit festem Buildwert. Keine neue Version oder Paketierung. Nachweise liegen ausschließlich unter `.build/transition-integration-20261022/`.

## Neuer realer Furiosa-Schnitt

Quellfilm und Cutlist anhand des bestehenden Diagnoseberichts vor dem Lauf über Größe, Zeit und SHA-256 geprüft. Neuer regulärer MP4Box-Schnitt in `.build/transition-integration-20261022/furiosa/INTEGRATED-MP4BOX.mp4`, ohne Überschreiben der vorhandenen Filmdateien. Der Workflow erzeugt fünf Abschnitte, analysiert die endgültige Ausgabe und räumt seine Segmentdateien auf.

`CUT-RESULT.json` bestätigt den echten `CutAssistantNext.App.Services.Cutting.Mp4BoxCutService`, `Located`, Überlappungskennzeichnung und vier exakt auf .NET-Ticks gerundete Referenzpositionen. Laufzeit 01:54:08.841; Ausgabe 4424042342 Bytes, SHA-256 `19E878BDB570550E75D9D7D8EE1F4BA1B31E81792E97C1271D9E38B64F350B76`. Der Hash des neuen Containers unterscheidet sich vom früheren Schnitt; die Übergangsreferenzen wurden gesondert geprüft.

| Übergang | Zugeordnete Ausgabezeit | Kontrollstart drei Sekunden davor |
|---|---|---|
| 1 | 00:37:40.6717292 | 00:37:37.6717292 |
| 2 | 00:50:53.6531042 | 00:50:50.6531042 |
| 3 | 01:15:43.3906042 | 01:15:40.3906042 |
| 4 | 01:34:26.0802708 | 01:34:23.0802708 |

**Vier von vier nativen Prüfungen bestanden**, mit `ActualProductionCutResult=true` und `TimingKind=Located` im Nachweis `furiosa/native/NATIVE-PROOF.json`: tatsächlicher Seek-Auftrag drei Sekunden vor der produktiven Prüfstelle, fortgesetzte Wiedergabe über jeden Übergang und pixelgleiche erste neue Originalbilder im Schrittfenster. Die letzte alte Referenz wurde am ersten Übergang weiterhin nicht beobachtet, an den übrigen drei Übergängen schon. Das ist keine vollständige Bildverlustmessung während normaler Wiedergabe. Die Positionsvergleichstoleranz bleibt 81 ms; Playerbenachrichtigungen sind keine unabhängig gemessenen Bildzeitstempel. Beide nativen Player wurden freigegeben.

Die vier im QA-Lauf verwendeten Produktbibliotheken App, Core, Media und Cutlists sind per SHA-256 identisch zum abschließend gebauten Release-Ausgabeordner. Alle acht vorhandenen Lockdateien sowie Buildzählerdateien vor/nach dem Lauf per Hash identisch. `PRESERVED-BEFORE.json` und `PRESERVED-AFTER.json` halten den Abgleich fest; frühere lokale Lockänderungen wurden nicht zurückgesetzt.

Der automatische Lauf ersetzt keine persönliche Abnahme. Der frühere Befund zur letzten alten Bildreferenz bleibt offen. Die Ursache der drei früheren Systemhänger mit otr-can ist weiterhin ungeklärt; diese Änderung betrifft dessen Schnittmotor nicht.

## Persönliche Prüfung im regulären CAN am 22.10.2026

Nach der Integration meldet der Nutzer ausdrücklich **„Bild, Ton, Schnitt, 3 Sekunden check“** und liefert das Protokoll eines eigenen Furiosa-MP4Box-Schnitts mit fünf Segmenten. Das Protokoll zeigt die abschließende Prüfung aller fünf Abschnitte, am ersten Übergang den neuen Hinweis **„geringfügig überlappende Bildzeiten; Paketfolge und dekodierter Abschnittswechsel werden zugeordnet“** und anschließend **„Fertig.“** Im übermittelten Text erscheinen keine Fehler-/Warnmeldungen oder Hinweise auf den berechneten Rückfall.

Damit ist die integrierte Schnitt-/Bild-/Ton-/Vorlaufkontrolle zusätzlich persönlich positiv bestätigt. Die frühere Abnahme des separaten Diagnoseplayers bleibt ein eigener Befund. Keine vollständige Bildzählung oder separate Messung jeder Tonspur/Tonsynchronität aus dieser Rückmeldung abgeleitet; der technische Bildankerbefund bleibt erhalten.

Nur dieser Bericht, Projektstatus und Tagebuch ergänzt. Kein neuer Code-, Build- oder Testlauf; der technische Nachweis bleibt **1411/1411 Solution-Tests, 40/40 isolierte Tests und vier von vier native Prüfungen**. Buildnummer und Release-Status unverändert.

## Weiterer Praxisfall: S.W.A.T. – Der Schatz der Santa Clara

Am 22.10.2026 bestätigt der Nutzer **„Schnitt, Ton, 3 Sekunden check“** für die HD-Aufnahme vom 21.10.2026 auf 3plus. Das beigefügte MP4Box-Protokoll zeigt drei Segmente, die Prüfung aller drei Abschnitte und den Abschluss **„Fertig.“** Am zweiten Übergang meldet es **„gleiche Bildzeit am Abschnittswechsel; dekodierter neuer Anfang wird geprüft“**. Für den ersten Übergang erscheint keine solche Meldung; eine geringfügige Überlappung wird in diesem Lauf nicht gemeldet. Im übermittelten Text erscheinen keine Fehler-/Warnmeldungen oder Hinweise auf berechnete Ersatzzeiten.

Dieser weitere persönlich erfolgreiche Praxisfall verwendet den bisherigen Prüfweg für gleiche Bildzeitstempel und ergänzt damit die Erprobung nach der Erweiterung. Er ist kein zusätzlicher Nachweis des neuen Überlappungsmusters. Schnitt, Ton und Drei-Sekunden-Vorlauf sind im ausdrücklich gemeldeten Umfang bestätigt; eine gesonderte Bildabnahme wurde in dieser Rückmeldung nicht genannt.

Nur Dokumentation ergänzt; kein neuer Code-/Build-/Testlauf. Technische Tests, Buildnummer 34, Release-Status und bestehende offene Befunde unverändert.

## Weiterer Praxisfall: Fire Country – Todesfalle, S3E14

Am 22.10.2026 bestätigt der Nutzer **„Schnitt, Bild, Ton, 3 Sekunden Sprung check“** für **Fire Country – Todesfalle, S3E14**, HQ-Aufnahme vom 21.10.2026 auf 3plus. Das beigefügte MP4Box-Protokoll zeigt drei Segmente und die abschließende Prüfung aller drei Abschnitte. An **beiden inneren Übergängen** werden gleiche Bildzeiten und die Prüfung des dekodierten neuen Anfangs gemeldet. Der Lauf endet mit **„Fertig.“**; im übermittelten Text erscheinen keine Fehler-/Warnmeldungen oder Hinweise auf berechnete Ersatzzeiten.

Schnitt, Bild, Ton und Drei-Sekunden-Sprung sind persönlich positiv bestätigt. Nach dem HD-Fall S.W.A.T. ergänzt dieser HQ-Fall die Praxisprüfung des bestehenden Wegs für gleiche Zeitstempel. Eine geringfügige Überlappung wird hier nicht gemeldet; der Lauf ist daher kein weiterer Nachweis des neu ergänzten Überlappungsmusters.

Nur Integrationsbericht, Status und Tagebuch ergänzt, kein neuer Code-/Build-/Testlauf. Vorhandene Testnachweise, Buildnummer 34, Release-Status und offene technische Befunde bleiben unverändert.

## Versionsstand

Lokale Version 1.0.1, Buildzähler 34 erhalten. Vorhandene Nutzeränderungen und acht vorhandene NuGet-Lockdateien bewahrt. Keine Commits, Pushes, Tags, Installer, Portable-Pakete oder Releaseänderungen. 1.0.0 Build 32 bleibt die zuletzt bestätigte Stable-/Latest-Version, 1.0.1 Build 34 Draft. Eine Veröffentlichung benötigt einen eigenen Auftrag.

Die persönliche Kontrolle im regulären CAN nach einem eigenen Schnitt ist im oben gemeldeten Umfang abgeschlossen; zusätzliche reale Formate bleiben gesondert zu erproben. Vor einer neuen Auslieferung sind ein eigener Versions-/Paketierungsauftrag und die passende Paketabnahme erforderlich.
