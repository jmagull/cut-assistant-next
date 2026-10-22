# MP4Box – isolierte Übergangsdiagnose vom 22.10.2026

Nach Bestandsaufnahme ausdrücklich freigegebener nächster Schritt: ein separater Diagnoseprototyp für vorhandene Dateien. Keine Produktkorrektur, neue CAN-Version, Paketierung, Commit-/Push-/Tag- oder Releaseänderung. **CAN 1.0.0 Build 32 bleibt Stable/Latest, 1.0.1 Build 34 bleibt Draft.**

## Umfang

Unter `tools/prototypes/TransitionDiagnosis` liegt ein .NET-10-Konsolenwerkzeug mit eigener Testsuite. Der vorhandene Cutlist-Parser wird wiederverwendet; keine neue Produktionsabhängigkeit. Die Produktions-Solution, CAN-Quellen, Schnittmotoren und Schnittparameter bleiben unverändert.

Das Werkzeug ordnet Videopakete einer fertigen MP4Box-Datei anhand ihrer SHA-256-Nutzdatenhashes dem Original und anschließend den geplanten Behaltebereichen zu. Ein konfigurierbarer Zuordnungsspielraum nimmt verschobene MP4Box-Grenzen auf; überlappende Fenster bleiben mehrdeutig. Wiederholte Nutzdaten sind nur mit einer identischen, gleich langen Originalpaketfolge zwischen zwei eindeutigen Nachbarankern auflösbar. Es gibt keine Auswahl des ersten passenden Hashes und kein Überbrücken eines Schnittsprungs.

Für jeden Übergang werden Paket-PTS, DTS, Dauer, Byteposition, Originalzuordnung, Präsentationsüberlappung und kleine lokale Decoderfenster protokolliert. Decoderreihenfolge und Best-Effort-Zeit werden getrennt von Paketpräsentation und Decoder-DTS behandelt. Ein fertiger JSON-Bericht ist **keine** automatisch bestätigte CAN-Prüfstelle: `ReviewPositionConfirmed` bleibt false.

## Vorhandene Furiosa-Datei

Untersucht wird die lokale Ausgabe **Furiosa A Mad Max Saga Teil 1 2026-10-21 [18.10.2026] MP4Box.mp4**, zusammen mit dem Original Teil 1/2 und dessen vorhandener fünfteiliger MP4Box-Cutlist. Identitäten und vollständige Pfade sind ausschließlich im ignorierten JSON-Nachweis gesichert. Kein neuer Filmschnitt.

Der erste Lauf findet 307470 Quellpakete und 171218 Ausgabepakete. 109 Pakete haben im Original mehrfach vorkommende Nutzdaten und bleiben in dieser ersten Fassung ausdrücklich unaufgelöst. Daraus folgt die gezielte, separat getestete Erweiterung um beidseitig verankerte Paketfolgen. Der erste Bericht bleibt erhalten und wird nicht nachträglich überschrieben.

Der abschließende Lauf ordnet **alle 171218 Ausgabepakete** zu, davon **109 über beidseitig verankerte Originalfolgen**. Es bleiben keine unaufgelösten Pakete; die fünf Behaltebereiche erscheinen in der geplanten Reihenfolge. Genau vier Quellfolgensprünge liegen an den vier Abschnittswechseln. Keine verworfenen oder beschädigten Paketflags in der Ausgabe. Diese vollständige Zuordnung bestätigt die Paketmitgliedschaft unter den dokumentierten Zuordnungsbedingungen, nicht die inhaltliche Richtigkeit des Films.

| Übergang | CAN-Planposition in s | Ausgabe-PTS des neuen Anfangs in s | Abweichung zum Plan | Lokale Decoderbeobachtung |
|---|---:|---:|---:|---|
| 1 → 2 | 2261,789063 | 2260,671729 | −1,117333 s | Rückläufige PTS |
| 2 → 3 | 3058,563437 | 3053,653104 | −4,910333 s | Ein beobachteter Wechsel |
| 3 → 4 | 4548,085708 | 4543,390604 | −4,695104 s | Ein beobachteter Wechsel |
| 4 → 5 | 5675,379958 | 5666,080271 | −9,299687 s | Ein beobachteter Wechsel |

Zeiten sind zur Anzeige gerundet; der JSON-Nachweis enthält die rational umgerechneten Dezimalwerte. Die Planzeiten stammen aus der untersuchten lokalen Cutlist. Keine Zeiten einer anderen Ausgabevariante übernommen.

Am ersten Übergang folgt in der **tatsächlich von ffprobe ausgegebenen Decoderfolge** auf das letzte Bild des alten Bereichs bei **2260,680000 s** das erste Bild des neuen Bereichs bei **2260,671729 s**. Die Differenz beträgt **8,270833 ms rückwärts**. Damit ist die Abbruchbedingung von CAN in dieser konkreten Ausgabe nachvollzogen; das Problem ist nicht lediglich eine andere Reihenfolge der komprimierten B-Frame-Pakete. Über das genaue Präsentationsverhalten von libmpv ist damit noch nicht entschieden.

Die Paketzuordnung findet als erste Originalbilder der fünf Bereiche **584,320 / 3607,720 / 5167,600 / 6956,160 / 8849,040 s**. Die angeforderten Anfänge liegen bei **583,200 / 3603,9270001 / 5167,8285834 / 6951,5527501 / 8848,920 s**. MP4Box übernimmt also unterschiedliche tatsächliche Anfänge; der dritte liegt sogar vor der angeforderten Zeit. Ein einheitlicher Korrekturwert ist damit ungeeignet. Diese Beobachtung allein ist keine vollständige Mengenbilanz aller fehlenden/zusätzlichen Originalbilder.

Der vorhandene Kontrollplayer zieht weiterhin drei Sekunden von der **Planposition** ab. Gegenüber den jetzt beobachteten Abschnittsanfängen beginnt er damit ungefähr **1,883 s davor / 1,910 s danach / 1,695 s danach / 6,300 s danach**. Das erklärt die persönlich gemeldeten unpassenden Kontrollsprünge konkret.

Identität der untersuchten MP4Box-Ausgabe: **4424042342 Bytes**, SHA-256 **C61D7E765DC5688EB00BD2BF1B015322B23821B54AD8E4ECA4CCBA16362C88C4**. Videodauer rund **6848,840271 s**, Containerdauer **6848,841 s**. Die Zeitbasis der Ausgabe ist **1/48000**; sie wird unabhängig von der Originalzeitbasis verwendet. **Keine** dieser Angaben bescheinigt Tonqualität, Synchronität oder Sichtbarkeit jedes Paketbildes.

Nächster kleiner Entwicklungsschritt: diese vier Kandidaten in einem separaten libmpv-Kontrolllauf gegen die zugeordneten Originalbilder prüfen, besonders den rückläufigen ersten Übergang. Erst mit diesem Nachweis eine begrenzte Erweiterung der produktiven Übergangsprüfung entwerfen. Die bestehende Ablehnung von Überlappungen bleibt erhalten.

## Tests und Erhaltung

- Release-Build der Produktions-Solution: 0 Warnungen, 0 Fehler; unveränderte Buildnummer 34. Direkter Build statt nummerierendem Skript, um die verbindliche Versionssperre einzuhalten.
- Bestehende Tests: **1385/1385**, davon Core 199, Media 224, App 890, Cutlists 72; keine übersprungen.
- Eigener Prototyp: **29/29 Tests**, keine übersprungen, Release ohne Warnungen/Fehler. Unter anderem unabhängige Zeitbasen, negative DTS, gleiche/überlappende PTS, rückläufige Decoderfolge, wiederholte Payloads, verankerte Folgen, veränderte/lückenhafte Folgen, falsche Keep-Reihenfolge, fehlende Werkzeuge, Abbruch und Schutz bestehender Dateien.
- Restore mit eigenem Lockdateipfad und vorhandenen Paket-Caches; kein neuer Produktionspaketstand. Die anfänglichen Sandboxprobleme bei Buildhost-/Testkommunikation sind keine bestandenen Testläufe. Formatierung und abschließende Builds/Tests wurden mit funktionierender lokaler .NET-Kommunikation ausgeführt.
- Beim ersten Gesamttestlauf wurde ein identischer TRX-Dateiname für alle vier Suiten verwendet und deshalb jeweils überschrieben. Der abschließende Lauf wurde mit automatischen, verschiedenen TRX-Namen gesichert: erneut **1385/1385**, vier getrennte Nachweise unter `complete-regression-results/`; zusätzlich **29/29** unter `final-test-results/`. Formatprüfung der neuen C#-Dateien ohne ausstehende Änderungen bestanden. Auch der dokumentierte wiederverwendbare isolierte Restore-/Buildweg wurde erfolgreich geprüft.
- Reeller Ein-Sekunden-Abbruchtest: Exitcode 2, kein Teilbericht und kein verbliebener zugehöriger ffprobe-Prozess; `TIMEOUT-PROOF.json`.

Lokale Nachweise: `.build/transition-diagnosis-20261022/`, insbesondere `FURIOSA-MP4BOX-FINAL.json`. **Alle acht vorhandenen Produktions-/Test-Lockdateien und der Buildzähler sind per SHA-256 unverändert** gegenüber dem Ausgangszustand; neun erfolgreiche Erhaltungsprüfungen. Vorbestehende README-/Rücknahme-/Freeze-Berichtsänderungen wurden nicht bearbeitet. Die Anwendung wurde nicht installiert oder gestartet.

## Stabilitätsstand und Grenzen

Die Übergabe vom 22.10. berichtet persönlich erfolgreiche Tests vom 21.10. für Strange World, Navy CIS: Origins, zwei NCIS: Sydney-Folgen und wiederholte Furiosa-Läufe. Die HWiNFO-Zusammenfassung nennt moderate Temperaturen, keine thermische CPU-Drosselung und keine WHEA-Gesamtfehler. Diese positiven Anwenderbefunde werden übernommen; sie sind keine in diesem Arbeitsschritt erneut durchgeführte Stabilitätsprüfung oder unabhängige CSV-Auswertung.

Die drei früheren vollständigen Windows-Hänger bleiben ungeklärt. Keine BIOS-/RAM-/Treiberänderung, kein neuer otr-can-Lauf und kein Belastungstest. Der MP4Box-Zeitbefund beweist keinen Zusammenhang mit diesen Hängern. Eine spätere Korrektur der Prüfstellen würde weder fehlende Bilder wiederherstellen noch Bild-/Tonqualität, Werbefreiheit oder dauerhafte Systemstabilität bestätigen.

[Bedienung, Aufbau und Prüfgrenzen des Werkzeugs](../tools/prototypes/TransitionDiagnosis/README.md).
