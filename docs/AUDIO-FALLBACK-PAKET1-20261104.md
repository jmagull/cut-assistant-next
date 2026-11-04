# Paket 1: präzise Audiodiagnose und bestätigter MP4Box-Neustart

Stand: 04.11.2026. **Lokale persönliche Prüffassung, kein Release.** CAN 1.0.3 · Build 50 · Audio-Diagnose-Entwicklung; Motor 0.1.6-audio-diag.1. Implementiert ist ausschließlich Paket 1 des Entwicklungsauftrags. [Paket 2 ist ein getrenntes Konzept](AUDIO-FEHLERBEHANDLUNG-PAKET2-KONZEPT-20261104.md), ohne produktive Stilleergänzung oder neue allgemeine Tonspur-Ausschlussregel.

## Ausgangspunkt und geschützte Stände

Die Entwicklung liegt in einer neuen CAN-Arbeitskopie cut-assistant-next-audio, Branch codex/audio-diagnostics-fallback, auf der abgeschlossenen Spotlight-Integration 2017673. Der private Submodulstand bleibt 6973b37591e9b581b20b5a6f2117369b6df0f159. Das Original-CAN-Arbeitsverzeichnis mit seinen vorhandenen Nutzeränderungen und Buildzähler 48 blieb unangetastet. Der frühere Spotlight-Prüfbranch und sein Build 49 wurden ebenfalls nicht verändert.

Motor-Arbeitskopie: otr-can-audio, Branch codex/audio-diagnostics, Basis b2ff640. Implementierungscommit: **6fed47094aed9bbca0ed986fc8e5621897ecf600**. Das ursprüngliche Motorverzeichnis einschließlich des vorhandenen AC-3-Experiments und der bisherigen Diagnose blieb erhalten. Keine Änderungen an Martins Originaldateien, keine Force-Pushes, kein Tag und keine Veröffentlichung.

Die maßgebliche ursprüngliche [NCIS-Diagnose](../../otr-can-audio/docs/NCIS-SYDNEY-AAC-DIAGNOSE-20261104.md) wird beim Motor aufbewahrt. Vier innere 512-Sample-Lücken und das 488-ms-Audioende erklären die beiden ursprünglichen Abbrüche exakt. Die historische Entstehung der Daten ist weiterhin ungeklärt.

## Motor: mehr Befund, gleich strenge Ablehnung

Das neue Modul audio_diagnostic.rs untersucht bei einer PCM-Fehlmenge die vollständig dekodierte Originalspur. Es vergleicht ganzzahlige Original-Samplepositionen mit den tatsächlichen Samplezahlen und den benötigten Keep-Intervallen. Eine zusätzliche Diagnose dekodiert außerhalb des WPF-Threads; CANs Prozessbaumabbruch bleibt wirksam.

Eine nachgewiesene Zeitlücke erfordert unter anderem eindeutige Spur-/Zeitbasis-/Formatdaten, aufsteigende PTS ohne Überlappungen, Übereinstimmung von Framedauer und Folge-PTS sowie einen belegten Audioendpunkt. Die unabhängige Keep-Auswahl muss genau die schon erzeugte PCM-Menge ergeben. Sämtliche fehlenden Samples müssen durch die bezeichneten Intervalle erklärt werden. Die Diagnose behauptet keine sichere Reparierbarkeit und keine bewiesene historische Ursache.

Getrennte Befunde:

- internal_gap: fehlende Zeitabdeckung innerhalb des jeweiligen Keeps, auf dessen tatsächlichen Umfang begrenzt.
- audio_end: fehlende Abdeckung nach dem belegten Ende der Tonspur.
- unclassified: beispielsweise widersprüchliche Enddaten, falsche Timebase, Formatwechsel, zusätzliche unerklärte Abweichung oder eine gemischte Lücken-/Überlappungsstruktur.

Es werden maximal 64 Fehlstellen ausgegeben; darüber bleibt die Abweichung unklassifiziert. Diese Grenze erlaubt keine Reparatur. Bei einem Überschuss auf Nicht-AAC-Pfaden wird ohne zusätzlichen vollständigen Dekodierlauf weiter strikt abgebrochen. Der bewährte, streng belegte Ein-Sample-AAC-Überlappungspfad bleibt erhalten.

Das versionierte audio-diagnostic.json liegt direkt im exklusiven CAN-Arbeitsverzeichnis und überlebt die interne Motorbereinigung lange genug zum Einlesen. CAN begrenzt Dateigröße, Schema, Codec-/Spurzuordnung, Werte, Fehlstellenanzahl und Summenkonsistenz. Fehlende oder unbrauchbare Angaben bleiben eine ungeklärte Audiostörung. Technische Rohdaten gehen ins Protokoll. Alle Fehler aus der Audioverarbeitung besitzen außerdem eine eindeutige Kategorie am Ende des Fehlertextes, damit auch ein begrenzter Fehlerpuffer sie behält. Andere Video-, Index- oder Werkzeugfehler lösen kein pauschales Audioangebot aus.

Unverändert bleiben Sollsampleberechnung, Keep-Grenzen, strenge Akzeptanzregeln, bestehende AAC-Korrektur, AC-3/MP3-Verarbeitung, kontinuierliche Einmalkodierung und finale native Prüfungen. Keine neue Produktionsabhängigkeit; Cargo.lock ändert ausschließlich die eigene Entwicklungsversion.

Bekannte Grenze des bisherigen Normalpfads: Eine bereits passende Nettosamplezahl löst weiterhin keine neue allgemeine Prüfung der kompletten ursprünglichen Audiozeitachse aus. Daraus wird keine neue Synchronitätsgarantie abgeleitet.

## CAN: bewusster vollständiger Neustart

OtrCanCutService übernimmt die Diagnose nur bei einem erkannten Audiofehler, bevor sein privates Verzeichnis bereinigt wird. Danach erhält der Anwender einen eigenen Dialog. Standardaktion ist **Abbrechen**; der MP4Box-Button erfordert eine eigene bewusste Auswahl. Die Nachfrage wird bei Abbruch des laufenden Auftrags geschlossen.

Erst nach Zustimmung prüft CAN den konfigurierten MP4Box-Pfad, startet eine begrenzte Versionsabfrage und verlangt eine erfolgreiche Erkennung des Programms. Fehlende EXE, Startfehler oder falsches Programm stoppen den neuen Versuch. Es wird keine Einstellung automatisch auf MP4Box umgestellt.

OtrCanMp4BoxFallbackCutService ruft anschließend den vorhandenen Mp4BoxCutService/Mp4BoxCutWorkflow auf. Es ist derselbe unveränderliche CutRequest mit identischen Keeps, Originalidentität, Ziel und Überschreibentscheidung. Falls die bestehende Arbeitsquelle noch kein MP4 ist, wird die vorhandene verlustfreie VideoPreparation verwendet. Ihre eigene private Kopie wird anschließend bereinigt.

Der fehlgeschlagene otr-can-Auftrag muss erst vollständig zurückgekehrt sein, einschließlich seiner Bereinigung. Eine unvollständige Motor-Datei wird niemals als MP4Box-Eingabe weiterverwendet. MP4Box bekommt die bisherige gültige Original-/Vorbereitungsquelle. Neue Zielkonflikte und vorhandene gültige Ausgaben bleiben durch den vorhandenen MP4Box-Workflow geschützt. Bei nicht löschbaren privaten Resten enthält das Protokoll deren Pfad; es erfolgt kein unkontrolliertes Entfernen anderer Verzeichnisse.

Die tatsächliche erfolgreiche Methode wird für spätere bewusst erzeugte Cutlist-Metadaten als MP4Box gemerkt. Eine bereits vorhandene Cutlist wird nicht automatisch neu geschrieben. Abschlussfenster und Protokoll enthalten ausdrücklich:

> MP4Box-Fallback: Nach dem otr-can-Tonfehler ausdrücklich gewählte alternative Schnittmethode. Dies ist keine nachgewiesene Audioreparatur. Bitte Bild, Ton, Tonsynchronität und alle Schnittübergänge persönlich kontrollieren.

Auch die bereits vorhandenen Hinweise auf verschobene MP4Box-Grenzen oder nicht bestätigte Ausgabeübergänge bleiben sichtbar.

## Vorhandener AAC-Notfallweg

Bei den bisher erkannten PCM-Decoder-/Warnungsfehlern bleibt die ausdrückliche Ein-AAC-Auswahl mit vollständiger Quellenprüfung und exaktem Streamcopy-Nachweis erhalten. Neu diagnostizierte Paketzeitlücken aktivieren **keine** allgemeine Tonspurausschlussregel. Bei Ablehnung der bisherigen AAC-Auswahl oder nicht sicher vorbereitbarer AAC-Alternative bleibt eine davon getrennte bewusste MP4Box-Entscheidung möglich. Nach einem bestätigten AAC-Neustart gibt es keine automatische Kette weiterer Spurauswahlen.

## Persönliche Prüffassung

Start: artifacts/CAN-Audio-Diagnose-Entwicklung/CutAssistantNext.App.exe. ZIP: artifacts/CAN-Audio-Diagnose-Build50-Pruefkopie.zip. Den gesamten Ordner zusammenhalten. .NET 10 Desktop Runtime x64 und die bereits eingerichteten externen Werkzeuge bleiben Voraussetzungen.

Nur der ausdrücklich so gekennzeichnete Candidate **Audio-Diagnose-Entwicklung** verwendet automatisch seinen mitgelieferten Motor runtime/otr-can/otr_can.exe. AudioDevelopmentEngine schreibt dabei keine globale Einstellung; der gespeicherte Motorpfad und FFMS2-Pfad werden erhalten. Fehlt das Paketbinary, wird verständlich abgebrochen. Alle anderen Candidate-Namen und stabile Builds verwenden unverändert die konfigurierte Motorwahl. Der tatsächlich gestartete Pfad steht im Protokoll.

Der Motor liegt mit MPL-Lizenz, Abhängigkeitshinweisen und dem zu seinem Commit passenden Quellarchiv im Paket. FFmpeg, ffprobe, FFMS2 und MP4Box werden nicht zusätzlich mitgeliefert. Die vorhandene Spotlight-Integration einschließlich gemeinsamer Laufzeitübersicht bleibt enthalten.

## Beispiel aus dem echten NCIS-Referenzlauf

Für die technische Gegenprüfung wurde bewusst nur der Zwei-Frame-Keep [3858.600, 3858.680) verwendet; kein kompletter neuer Episodenschnitt. Die Originaldatei wurde anhand ihres bekannten SHA-256 vor und nach dem Lauf als unverändert bestätigt. Der Test simuliert die Ablehnung des MP4Box-Buttons und schützt eine vorhandene Ziel-Sentineldatei.

~~~text
otr-can konnte Tonspur 1 (AAC, 48000 Hz) nicht sicher verarbeiten.
Innere Zeitlücke ab 01:04:18.645, Behaltebereich 1:
512 fehlende Samples (10,667 ms).
Erwartet: 3840 Samples, vorhanden: 3328.
Die strenge Prüfung hat den Schnitt abgebrochen.
~~~

Der Befund lokalisiert dieselbe Lücke bei Sample [185214976,185215488), die im ursprünglichen Drei-Keep-Fall den dritten Keep betrifft. Die vollständigen ursprünglichen Fall-A-/Fall-B-Audiomessungen stammen aus dem getrennten Diagnoseauftrag; daraus wird kein neuer vollständiger Episodenschnitt mit der Prüffassung behauptet.

## Build- und Testergebnisse

| Prüfung | Ergebnis |
| --- | --- |
| Locked .NET-Restore für win-x64 | erfolgreich |
| Erforderlicher vollständiger tools/build.ps1-Build | Build 50, Release, **0 Warnungen / 0 Fehler** |
| Gesamte CAN-Solution | **1795 bestanden**, keine ausgelassen; Core 200, Cutlists 80, Media 390, App 1125 |
| App-Tests nochmals mit finalen Produkt-DLLs | **1125 bestanden**, keine ausgelassen |
| Motorformatierung, Offline-/Locked-Release-Build | erfolgreich |
| Vollständige Rust-Tests | **63 bestanden**, keine ausgelassen; 57 Bibliothek, 6 CLI |
| Alle bisherigen vollständigen synthetischen Motor-Medienregressionen | **14 bestanden**, inklusive unabhängiger bytegenauer PCM-Vergleiche und unveränderter gesunder Ausgaben |
| Neue native CAN-Matrix gegen das Paket | **16 bestanden**, tatsächliche FFmpeg-/ffprobe-/FFMS2-/otr-can-/MP4Box-Aufrufe |
| Privater kurzer NCIS-Referenzlauf gegen das Paket | bestanden, korrekte 512er-Diagnose, Ziel-/Originalerhalt, vollständige Bereinigung |
| Bestehende native Spotlight-/CAN-Prüfung gegen das Paket | **32 bestanden**, beide Bearbeitungsmodi, Player, Übergabe, Dateisperre, Laufzeitanzeige, Abbruch und Wiederöffnung |
| Neuer WPF-Dialog | gerendert und visuell geprüft; vollständige lesbare Buttons, Abbrechen als Standard |

Bei den wiederverwendeten CLI-Negativregressionen ist ausschließlich die zusätzliche versionierte Diagnosedatei im aufruferverwalteten Temp-Ordner neu zulässig. Unveränderte Indexdaten, Ablehnung, fehlende fertige Ausgabe sowie sämtliche PCM-/Bytevergleiche werden weiterhin geprüft. CAN selbst entfernt auch diese Datei mit seinem gesamten privaten Auftrag.

Die 16 nativen Fälle umfassen reguläres AAC, eine und mehrere innere Lücken, mehrere Keeps, außerhalb liegende Lücken, Schnittbeginn direkt danach, genau am Audioende endende Auswahl, kürzeres Audio, Lücke plus Endfehlmenge, eine gemischte unklassifizierte Lücken-/Überlappungsstruktur, Abbruch während der Entscheidung, tatsächlich bestätigten MP4Box-Neustart sowie alle vier vorhandenen synthetischen AAC-/AC-3-Notfallfälle einschließlich beschädigten AACs.

51 neue CAN-Testfälle kommen zu den 1744 der Spotlight-Basis; die zehn neuen Motorfälle zu den 53 vorhandenen Rust-Tests. Kein vorhandener Test wurde entfernt. Die Auswahl in den nativen Ablauftests ist ein ausdrücklich simulierter Callback, keine persönliche Dialogabnahme.

Frühe native Prüfläufe wurden wegen relativer statt absoluter Werkzeugpfade und einer ungeeigneten Negativfixture korrigiert. Das bloße Beibehalten alter MP4-Dauerfelder erzeugte keine ungeklärte Zeitachse im Decoder; die finale Negativfixture enthält deshalb eine echte gemischte Lücken-/Überlappungsstruktur. Diese wird weiterhin strikt und ohne behauptete Reparierbarkeit abgelehnt. Kein Sicherheitscheck wurde zum Erreichen eines grünen Tests gelockert.

Lokale Nachweise: .build/audio-regression-verified-20261104/, .build/audio-native-verified-20261104/, .build/audio-ncis-verified-20261104/, .build/audio-package-spotlight-20261104/, .build/audio-dialog-20261104/ sowie artifacts/audio-package-unit-tests/. Die Motor-Arbeitskopie bewahrt target/audio-diagnostics-final-tests.log. Ergebnisse enthalten konkrete Input-/Motor-/App-Prüfsummen; Package-Provenance und ZIP-Prüfsumme liegen bei den Artefakten. Reale Medien bleiben außerhalb von Git.

## Bekannte Einschränkungen und nächster Schritt

Die Aufnahme wird im strengen otr-can-Modus weiterhin abgelehnt. Es wird kein Originalton erfunden, zeitlich verschoben oder still ergänzt. Ein sauberer FFmpeg-Dekoderlauf allein beweist keine vollständige Samplezeitachse.

Beim synthetischen MP4Box-Fallback konnte die vorhandene Ausgabeanalyse verworfene Bilder innerhalb des Films nicht eindeutig zuordnen. Die gültige Ausgabe blieb erhalten und die Prüfstellen wurden ausdrücklich als berechnet/geschätzt gekennzeichnet; der Grund steht im Protokoll. Dieser bekannte MP4Box-Prüfvorbehalt wurde nicht unterdrückt oder als gemessene Synchronität ausgegeben.

Eine neue persönliche Abnahme von vollständiger NCIS-Auswahl, Rückfragedialog und MP4Box-Ergebnis steht aus. Ebenso werden keine neue Setup-/VM-/Portable-Installationsabnahme oder Lösung des früheren Mad-Max-/Furiosa-Systemstillstands behauptet. Die Prüffassung ist ein lokaler Ordner/ZIP, kein neuer öffentlicher Paketsatz.

Nächster Schritt: Diese Fassung mit der vorhandenen NCIS-Cutlist persönlich prüfen, einmal Abbrechen testen und anschließend ausdrücklich MP4Box wählen; danach Anfang, Ende, Übergänge und Bild/Ton kontrollieren. Erst danach über die getrennten Labor-/Implementierungsfreigaben aus Paket 2 entscheiden.

## Persönlicher Rückfalltest und Textkorrektur nach Build 50

Der Anwender zeigt die neue Diagnose für die vollständige Drei-Keep-Auswahl: Behaltebereich 3, Originalzeit 01:04:18.645, 512 fehlende Samples, Soll 115580160/Ist 115579648. Anschließend bestätigt er den MP4Box-Neustart mit „funktioniert“. Damit sind die tatsächliche Diagnoseanzeige und der bewusst gewählte Rückfall persönlich bestätigt. Eine gesonderte vollständige Bild-/Ton-/Übergangskontrolle und ein persönlicher Abbruchtest werden daraus nicht zusätzlich abgeleitet.

Auf seinen Hinweis nennt die Rückfrage nun ausdrücklich die **MP4Box-Konfiguration aus den Einstellungen**. Der bisherige Satz zur Verfügbarkeit entfällt aus diesem Erklärungstext. Die begrenzte Start-/Versionsprüfung verwendet weiterhin genau diese konfigurierte Programmdatei. Keine neue Konfiguration und keine erneute Pfadauswahl. Die ausgelieferte Build-50-Prüfkopie bleibt unverändert; die Textkorrektur ist für den nächsten Entwicklungsbuild vorgesehen.

Textkorrektur als lokaler Entwicklungsbuild 51 gebaut: 0 Warnungen, 0 Fehler; sämtliche 1795 Solution-Tests bestanden, keine ausgelassen. Keine neue Paketierung oder Veröffentlichung.
