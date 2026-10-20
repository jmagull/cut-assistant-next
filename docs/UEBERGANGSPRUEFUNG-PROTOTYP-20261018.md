# Ausgabeübergänge – isolierter Kontrollplayer-Prototyp

Stand: **18.10.2026**. Auftrag nach dem [Vera-Messversuch](VERA-MP4BOX-SCHNITTPRUEFUNG-20261018.md): getrennten Prototyp bauen und die gemessenen Übergänge im tatsächlichen libmpv-Kontrollplayer prüfen. **Keine Integration in die Anwendung und keine neue CAN-Ausgabe.**

## Ergebnis

Der Prototyp unter `tools/prototypes/TransitionReview` ermittelt Veras drei Ausgabeübergänge selbständig aus den vier vorhandenen Messsegmenten und dem Gesamtfilm. Es gibt keine Vera-Sonderbehandlung, keinen Vier-Sekunden-Abzug und keine Veränderung der Schnittparameter.

Er verwendet die **unveränderten Quellen** des bestehenden Kontrollfensters, Kontroll-ViewModels, Playback-ViewModels und Videohosts sowie die vorhandenen Media-/Core-Assemblies. Ein eigener Adapter übergibt gemessene Ausgabe-Prüfstellen direkt, ohne einen künstlichen Schnittplan anzulegen. Das Projekt wird separat gebaut und gehört nicht zur Produktions-Solution.

### Vera mit MP4Box

| Übergang | Gemessener Ausgabe-PTS | Tatsächlich angeforderter Prüfstart | Im libmpv-Bildspeicher bestätigtes neues Anfangsbild |
|---|---|---|---|
| 1 | 00:17:00,720 | **00:16:57,720** | 00:17:00,720 |
| 2 | 00:35:20,840 | **00:35:17,840** | 00:35:20,840 |
| 3 | 01:05:13,880 | **01:05:10,880** | 01:05:13,880 |

Alle drei Starts wurden in der echten Player-Aufrufkette aufgezeichnet. Die Wiedergabe lief jeweils über den Übergang hinaus ohne gemeldeten Playerfehler. Zusätzlich wurden die im libmpv-Kontext gehaltenen Anfangsbilder pixelgenau mit dem entsprechenden Originalbild verglichen. Alle drei stimmen überein.

An den ersten beiden Übergängen treten die bereits gemessenen doppelten Video-PTS auf. Auch dort wurde das richtige neue Abschnittsbild bei der gemessenen Zeit im Player bestätigt. Im Frame-Step-Protokoll erscheinen gleiche Positionen/Bildinhalte mehrfach. Daraus folgt keine allgemeine Freigabe beliebiger überlappender Zeitachsen oder eine Reparatur der Zeitstempel. Die automatisierten Prüfungen betreffen Schrittverarbeitung und fortlaufende Wiedergabe; die gezielte persönliche Rückmeldung ergänzt sie im folgenden Abschnitt.

### Persönliche Bestätigung der Vera-Prüfstellen

Am **18.10.2026** bestätigt der Nutzer nach dem Öffnen des separaten Prototyp-Kontrollplayers:

> Übergang 1 beide ja :-)
> Übergang 2 beide ja :-)
> Übergang 3 beide ja :-)

Damit sind für **alle drei MP4Box-Übergänge** der Vorlauf vor dem tatsächlichen Schnitt und der anschließende Szenenwechsel persönlich positiv bestätigt. Die zuvor erläuterten Prüfstarts waren 00:16:57,720, 00:35:17,840 und 01:05:10,880. Eine gesonderte Ton-/Synchronitätsprüfung oder allgemeine Werbeblitzerfreiheit wird aus „beide ja“ nicht zusätzlich abgeleitet. Der nachgewiesene Verlust der 102 Anfangsbilder bleibt bestehen.

Die Rückmeldung ist die persönliche Funktionsbestätigung des isolierten Prototyps. Sie bedeutet keine bereits erfolgte Produktintegration, neue Buildnummer oder Release-Freigabe. Nur dieser Prüfbericht wurde für die Rückmeldung ergänzt; kein erneuter Build oder Testlauf.

### Vergleich mit otr-can

Die **vorhandene** Vera-Ausgabe von otr-can wurde mit dem unveränderten Kontrollplayer-Ablauf und ihren Planzeiten geprüft:

| Übergang | Prüfstart | Bestätigtes neues Originalbild im Player |
|---|---|---|
| 1 | 00:17:01,800 | 00:17:04,800 |
| 2 | 00:35:21,920 | 00:35:24,920 |
| 3 | 01:05:14,960 | 01:05:17,960 |

Drei erfolgreiche Sprünge, Wiedergabe über jeden Übergang sowie identische Pixelprüfsummen der jeweiligen neuen Originalbilder. Dies ist ausdrücklich **Plan-Kontrolle an einer vorhandenen Datei**, keine neue MP4Box-Segmentanalyse und kein neuer otr-can-Schnitt. Die Nachweise tragen `plan-control-only`. Beide Motoren und ihre Produktionsintegration bleiben unverändert.

### Künstliche Schnittfälle

Ein selbst erzeugter 20-Sekunden-Film, H.264 320×180, 25 fps, GOP 50, B-Frames, wurde separat mit MP4Box geschnitten. Der erste gewünschte Beginn liegt vor dem übernommenen Zugriffspunkt, weitere gewünschte Anfänge liegen danach beziehungsweise exakt darauf. Die Planübergänge sind **3,000 / 5,400 / 8,400 s**; die gemessenen Ausgabeübergänge **2,200 / 5,400 / 8,400 s**. Damit ist ein einheitlicher Korrekturwert ausdrücklich ungeeignet.

Alle drei tatsächlichen Anfangsbilder wurden auch im libmpv-Kontext gegen den künstlichen Originalfilm bestätigt. Die angeforderten Prüfstarts waren **0 / 2,400 / 5,400 s**. Die Begrenzung eines Vorlaufs von weniger als drei Sekunden auf Dateianfang ist damit auch real geprüft.

Weitere reale ffprobe-Fälle:

- Ein einzelner MP4Box-Abschnitt erzeugt keine inneren Übergänge.
- Seine MP4-Edit-Liste verwirft ein nachgeordnetes Paket am Dateiende. Dessen Inhalt wird geprüft, es wird aber nicht als sichtbarer Zeitanker behandelt.
- Eine absichtlich vertauschte Segmentreihenfolge wird abgelehnt; keine Ergebnisdatei wird veröffentlicht.

## Implementiertes Verfahren

1. Vorhandene Segmentdateien und Ausgabe prüfen; genau eine Videospur, positive Laufzeit und Zeitstart null verlangen.
2. Video-Pakete mit ffprobe **streamend** lesen. Die vollständigen SHA-256-Paketfolgen aller Segmente müssen in derselben Reihenfolge exakt im Gesamtfilm stehen. Keine Annahme über gleiche Zeitbasen oder nominelle Bildraten.
3. Aus den endgültigen Ausgabe-PTS und ihrer rationalen Zeitbasis den ersten präsentierten Zugriffspunkt je Abschnitt bestimmen. Ein früherer Präsentationsbeginn als der erste Zugriffspunkt, nicht geordnete Grenzen und echte Überschneidungen werden zurückgewiesen. Gleichheit des letzten/vorderen PTS an einer Grenze bleibt sichtbar vermerkt.
4. Den jeweiligen Zugriffspunkt durch lokale ffprobe-Dekodierung mit übereinstimmender Paketposition und PTS bestätigen. Nur danach gilt die Stelle als `FullPacketSequenceAndDecodedAnchor`.
5. Ergebnisdatei mit Ausgabeidentität, SHA-256, Segmentdaten, Zeiten, Verifikationsstatus und Hinweisen erstellen. Vor dem Öffnen im Player Dateigröße, Änderungszeit und vollständigen SHA-256 erneut prüfen.
6. Im unveränderten Kontrollplayer drei Sekunden abziehen; Anfang und Ende weiter nach dessen bestehenden Regeln behandeln.

Unvollständige oder widersprüchliche Daten führen im Prototyp zu einem Fehler, ohne neue Ergebnisdatei. Es gibt keinen stillen Rückfall auf theoretische Zeiten. Der Film selbst wird nicht verändert. Eine spätere Produktintegration muss Analysefehler vom Erfolg des Schnittauftrags unterscheiden und berechnete Ersatzpositionen ausdrücklich kennzeichnen.

## Build und Prüfstand

**Separater Release-Build: 0 Warnungen / 0 Fehler.** Formatierung der neuen C#-Dateien geprüft. **32/32 selbständige Prüfbedingungen bestanden**, darunter:

- unregelmäßige Präsentationsabstände und unterschiedliche kumulierte Segmentlängen;
- doppelte PTS an einer Grenze;
- falsche Reihenfolge, veränderte Pakete, zusätzliche oder fehlende Pakete, leere Segmente;
- vorhandener Paketanker ohne passenden Dekodiernachweis oder mit abweichendem PTS;
- negative PTS, verworfene Bilder innerhalb des Films, beschädigte Pakete und ungeeignete Abschnittsanfänge;
- überlappende Präsentation, fehlender Nullstart und Grenzen außerhalb der Dauer;
- fehlende oder doppelte Eingabepfade, Abbruch und Änderungen während der Analyse;
- Einzelabschnitt samt verworfenem Nachlaufpaket am Dateiende;
- ungültige und nicht ganzzahlige rationale Zeitbasen;
- nachträglich veränderte Ausgabe trotz gleicher Größe und wiederhergestellter Änderungszeit.

Zusätzlich **neun native Übergangsprüfungen** mit echten HWNDs/libmpv-Kontexten: drei Vera/MP4Box, drei Vera/otr-can, drei künstlicher Film. Für jeden: tatsächliche Seek-Anforderung, Wiedergabe über die Kante und pixelgleicher neuer Abschnittsanfang gegen das Original. Die automatisierten Fenster waren stumm und verborgen; Player wurden vor Zerstörung ihrer HWNDs freigegeben.

Die neue Logik wurde als Prototyp gebaut und getestet. **Kein Restore/Build/Testlauf der Produktions-Solution**; deren bisherige 1347 Pakettests werden nicht als neuer Lauf ausgegeben. Das Buildzähler-Skript wurde entsprechend der Vorgabe nicht aufgerufen. **Buildzähler bleibt 32.** Keine neue Produktionsabhängigkeit oder Änderung einer vorhandenen Lockdatei.

## Transparente Diagnosegrenzen

Die ersten nativen Bildnachweisversuche scheiterten am `screenshot-to-file`-Aufruf. Eine Diagnosefassung zur Speicherabfrage hatte zunächst einen falschen Reflection-Zugriff auf die geschützte Handle-Eigenschaft. Beides betraf ausschließlich den separaten Prüfhelfer; die Fehlläufe werden nicht als bestandene Prüfungen gewertet. Der endgültige Helfer verwendet `screenshot-raw`, prüft und kopiert die Pixel und gibt alle nativen Ergebnis-/Argumentressourcen wieder frei. Die endgültigen neun Prüfungen sind erfolgreich.

Die erste künstliche Einzelabschnittsprüfung deckte das am Dateiende ausdrücklich verworfene Nachlaufpaket auf. Die Behandlung ist darauf begrenzt, zusätzlich separat abgesichert und im Ergebnis vermerkt. Verworfene Bilder innerhalb des Films bleiben eine Ablehnung. Frühere Fehlnachweise bleiben lokal erhalten.

## Noch offen und empfohlener nächster Schritt

Der Prototyp liefert einen belastbaren Nachweis für die untersuchten Dateien; **er ist noch keine allgemeine Produktionslösung**:

- Eine vollständige Leseprüfung aller komprimierten Videopakete kostet zusätzliche Zeit. Breitere Laufzeitmessungen und eine mögliche Beschränkung auf abgesicherte Ankerfolgen sind noch offen.
- Zeitachsen mit anderem Start, mehreren Videospuren, komplexem Preroll oder echten Überschneidungen sind bewusst nicht unterstützt. Andere Codecs, GPAC-Versionen, Tonspurkonstellationen und reale Aufnahmen benötigen weitere Abnahme.
- Externe Änderungen während der Wiedergabe bleiben möglich. Die Prüfung vor dem Öffnen ersetzt keine Dateisperre während der gesamten Sitzung.
- Bildverlust-/Zusatzbildwarnungen aus einer Originalzuordnung sind nicht implementiert. Der sichtbare Prototyp weist allgemein darauf hin: **Die Prüfposition wird korrigiert; fehlende Bilder werden nicht wiederhergestellt.**
- Keine automatische Änderung von MP4Box-Modus, Cutlist, gewünschten Grenzen oder otr-can.

Die **persönliche Vera-Prüfung von Vorlauf und Szenenwechsel ist für alle drei Übergänge positiv bestätigt**. Als nächster Entwicklungsschritt kann der Ergebnisvertrag in CAN integriert werden: Messung vor Segmentbereinigung, Auswertung nach `-cat`, Abschlussmodell nach dem Schnitt und klar gekennzeichnete unbestätigte Ersatzzeiten bei Analysefehlern. Diese Integration ist in diesem Prototyp-Schritt noch nicht umgesetzt.

## Dateien und lokale Nachweise

Versionierbare neue Dateien: `tools/prototypes/TransitionReview/` mit eigenem Projekt, Analyse, Tests, Playeradapter und Anleitung sowie dieser Bericht. Die unveränderten Playerquellen unter `src/` werden nur verlinkt. Der frühere Vera-Messbericht bleibt erhalten.

Lokale, ignorierte Nachweise unter `.build/transition-review-prototype-20261018`:

- `BUILD.log`, `FORMAT.json`, `FORMAT-VERIFY.log`, `SELFTEST.json`;
- `vera-request.json`, `vera-final-result.json`, `ANALYZE.log`;
- `native-final/proof.json`, `native-otr/proof.json`, `native-synthetic/proof.json` samt Laufprotokollen und PNG-Pixelnachweisen;
- `fixtures-final/FIXTURES-PROOF.json`, künstliche Medien, Einzelabschnitts- und Ablehnungsprotokolle;
- frühe `native-run*`- und `fixtures-verified`-Versuche zur Diagnose;
- `PRESERVED-BEFORE.json`, `MEASUREMENT-DOC-BEFORE.json`, `FINAL.json`: Erhaltungsnachweis für die zuvor erfassten 587 Dateien, den bestehenden Messbericht, Lockdateien, Originale, vorhandene Ausgaben, persönliche Einstellungen, Release-Dateien, HEAD und Tag.

Keine Videos, lokalen Ergebnisdateien oder Einstellungen für Git vorgesehen. Kein Commit, Push, Tagwechsel oder Release.
