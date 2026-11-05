# CAN / otr-can: drei Entscheidungen nach Audiofehler – 05.11.2026

**Lokale Entwicklungsversion: CAN 1.0.3 · Build 51 · Audio-Fehlerbehandlung-Entwicklung.** Der strenge otr-can-Normalmodus ist unverändert. Die drei Entscheidungen werden erst nach einem sicher beendeten Tonfehler angeboten. Keine Änderung des stabilen CAN-Standes, keine Veröffentlichung, kein Tag und keine automatische Audioreparatur.

~~~text
C:/Users/joerg/Documents/GitHub/cut-assistant-next-aac/artifacts/CAN-Audio-Fehlerbehandlung-Entwicklung/CutAssistantNext.App.exe
~~~

Den Ordner zusammenhalten. .NET 10 Desktop Runtime x64 und die bereits eingerichteten externen Werkzeuge werden benötigt. Nur die entsprechend benannte Prüffassung verwendet ihren beiliegenden Motor unter runtime/otr-can. Einstellungen und insbesondere die bestehende MP4Box-Konfiguration werden weiterverwendet und durch die Paketwahl nicht geändert. Der Motor bleibt als Entwicklungsausgabe von 0.1.6-audio-diag.1 mit zusätzlichem aac-gap-repair-v1 gekennzeichnet.

## Persönliche Abnahme des vollständigen CAN-Schnitts

Am 05.11.2026 bestätigte Joerg den vollständigen NCIS-Schnitt aus CAN Build 51 nach Auswahl der experimentellen AAC-Reparatur. Bild, Ton, Schnitt, Übergänge und Drei-Sekunden-Vorschau waren störungsfrei. Er meldete eine lange Wartezeit.

Die nachgereichte Cutlist enthält drei Behaltebereiche ab 430,76 s, 1916,04 s und 3449,52 s mit 992,28 s, 1006,48 s und 409,16 s Dauer. Die Summe ergibt **00:40:07.920**. Ihr MP4Box-Eintrag stammt aus den Cutlist-Metadaten. Das persönliche Schnittprotokoll belegt den tatsächlich ausgeführten otr-can-Reparaturlauf. Die fertige persönliche Ausgabe ist mit dem technisch geprüften Referenzfilm bytegleich. Beide haben den SHA-256 `90948259c6823edc23c1e4af5b6778e5a7ba654c16109cab9f226d020a3fea0a`.

Das persönliche Protokoll enthält die Arbeitsschritte ohne Zeitstempel. Es zeigt den strengen Erstversuch, die erneute Indexierung und Vorprüfung sowie den vollständigen bestätigten Neustart. Im separaten Referenzlauf dauerten die vollständige Dekodierung des Quellbilds rund 4:18 min, die Dekodierung des Ausgabebilds 2:22 min und der erneute Pixelvergleich 2:23 min. Damit entfielen rund neun Minuten auf diese drei Bildprüfungen. Die Zeitstempel belegen den Referenzlauf. Die Dauer des persönlichen Laufs lässt sich aus den vorliegenden Daten nicht beziffern.

Für eine Beschleunigung bietet sich eine gemeinsame Dekodierung für Bildfolge und Pixelvergleich an. Vor einer Übernahme sind die gleichen Prüfwerte, dieselbe Ausgabedatei und das Ablehnungsverhalten an fehlerhaften Eingaben nachzuweisen. Dieser Nachtrag dokumentiert die persönliche NCIS-Abnahme von Build 51. Die Produktivfreigabe ist separat.

## Bedienung im bestehenden Schnittfenster

Beim Schneiden mit otr-can öffnet der vorhandene, dem Schnittfortschritt zugeordnete Fehlerdialog **„otr-can – Audioproblem erkannt“**. Bei nachgewiesener AAC-Reparierbarkeit nennt er Spur, betroffene Quellstellen, fehlende Samples und Dauer. Das Beispiel mit einer Lücke zeigt AAC-LC/Stereo/48 kHz, 512 Samples und 10,67 ms.

| Entscheidung | Verhalten |
| --- | --- |
| Mit MP4Box schneiden | Expliziter Neustart mit denselben Behaltebereichen, bestehenden Einstellungen und der bisherigen MP4Box-Vorbereitung. Die Verfügbarkeit wird geprüft. Ein erfolgreicher Fallback ist kein Reparatur- oder Synchronitätsnachweis. |
| AAC-Zeitlücken reparieren (experimentell) | Nur nach vollständigem, eindeutig unterstütztem Quellen- und Planbeweis. Startet den gesamten Auftrag neu, mit dem zuvor gezeigten und gebundenen Einfügeplan. Fehlende ursprüngliche Audiodaten werden durch Stille belegt, nicht rekonstruiert. |
| Abbrechen | Standard und Esc. Kein weiterer Motorstart, keine Änderung an Quelle, Cutlist, Schnittplan oder vorhandener Ausgabe. |

Bei unbekanntem Fehler, nicht unterstütztem Format oder Audioende fehlt der Reparaturbutton; der Dialog erklärt den Grund. Ist MP4Box nicht verfügbar, ist auch dessen Aktion gesperrt. Wenn ein bestätigter Reparaturlauf scheitert, erscheint derselbe Dialog erneut mit MP4Box und Abbrechen. Ein weiterer Reparaturversuch wird nicht angeboten. Es gibt keine automatische Wiederholung oder automatische Umschaltung. Die alten optionalen Spur-Ausschlusswege werden in dieser neuen Prüffassung nicht vorgeschaltet.

Gerenderte WPF-Belege im lokalen Paket unter checks/: 01-aac-reparatur-verfuegbar.png, 02-aac-reparatur-nicht-verfuegbar.png und 03-aac-reparatur-abgebrochen.png. Die native Matrix benutzt ausdrücklich simulierte Benutzerentscheidungen und echte Motoren; sie ersetzt keine persönliche Dialogabnahme.

## Technik und Dateischutz

CANs AacGapRepairService nutzt den vorhandenen Prozess- und Arbeitsordnerablauf. Der strenge erste Versuch endet mitsamt Bereinigung, bevor der getrennte Vorprüfauftrag startet. Der neue Ordner erhält einen frischen FFMS2-Index und einen Snapshot der unveränderten Keep-Liste. Quelle, Tools, Snapshot, Index, Sidecars und Reparaturplan werden an SHA-256 und exakte Zeitgrenzen gebunden. Die Vorprüfung startet keinen Encoder. Nach ausdrücklicher Wahl erstellt der Motor einen vollständigen neuen Schnitt in einem getrennten Reparaturordner; keine Teilresultate des fehlgeschlagenen Schnitts werden verwendet.

Der Motor implementiert Analyse, Einfügeplan, PCM-Rekonstruktion und unabhängigen Bytevergleich in Rust über die bestehenden Medienwerkzeuge. Der PCM-Vergleich verwendet eine frische Zeitachse und zweite vollständige Originaldekodierung. Jeder vorhandene Kanalwert bleibt an seiner vorgesehenen Zeitposition, nur nachgewiesene leere Positionen erhalten positive Nullwerte. Ein kontinuierlicher AAC-LC-Encode und der unveränderte etablierte Rust-Videoschnitt erzeugen die Kandidaten. Der Mux kopiert beide Spuren. Die unabhängige Endprüfung umfasst AAC-PTS, Nutzsamples, Vorlauf und Padding, Paketidentitäten, Vergleich zum PCM, MP4-Container, Bildanzahl/-folge, Bildqualität und SPS/PPS/Slice-Referenzen. CRF-0-Ränder mit High 4:4:4 Predictive werden abgelehnt. Kein C#-Labor wird zur Laufzeit aufgerufen.

CAN übernimmt erst nach bestandenem, zur aktuellen Ausgabedatei passendem Prüfbericht und zusätzlicher Ausgabeanalyse. Änderungen von Quelle, Plan oder vorhandenem Ziel führen zum Abbruch. Die vorhandene MP4Box-Alternative und der strenge normale Schnitt bleiben bestehen. Betroffene Dateien: vorhandener WPF-Fehlerdialog, MainWindow und Schnittfactory; AudioRecoveryOffer, AacGapRepairService und die erweiterte Fallback-Steuerung; gezielte Unit- und native Prüfungen. Kein neues Einstellungsmenü.

## NCIS Sydney

Der vollständige private Fall A wurde mit unveränderten drei Keeps im experimentellen Rust-Pfad verarbeitet. Die bisherige korrigierte Labor-MP4 bleibt unverändert. Der neue Film:

~~~text
C:/Users/joerg/Documents/GitHub/otr-can-aac-rust/target/audio-three-options-20261105/NCIS-Rust-AAC-Entwicklung.mp4
~~~

- Belegte Quelllücke [185214976, 185215488), 512 Vektoren ab Ausgabeposition 115578496. **115.579.648 unveränderte Originalvektoren + 512 Stillevektoren**, insgesamt 115.580.160 Nutzvektoren, **00:40:07.920**. PCM und einmalig erzeugtes AAC sind bytegleich zu den Laborbestandteilen. 112.873 AAC-Pakete, 1.024 Vorlauf, 768 Padding, 494 Kanalprüffenster mit bestem Lag null.
- **60.198 Bilder**, davon 60.007 kodiert und dekodiert bytegleiche Kopien; 191 Randbilder mit bewährten x264-Parametern. Mindest-PSNR 43,04 dB, durchgehend High/8-Bit-yuv420p, SPS wie im Original. Reine Videospur bytegleich zur korrigierten Laborfassung. Unveränderte Schnittstellen **00:16:32.280** und **00:33:18.760**.
- Vollständige FFmpeg-Bild-/Tonprüfung sowie zwei fensterlose libmpv-Bildfolgen mit je 50 tatsächlich gerenderten Frames über die Übergänge. Mindest-PSNR 42,22 bzw. 44,46 dB; keine Warnungen. Software-Frame-Schritte ohne Zwischen-Seeks, keine persönliche Echtzeitabnahme in MPC-HC/VLC.
- Reales Ende (Fall B): die letzten zwei Sekunden des Videos schließen 23.424 unbelegte Ticks (488 ms) ein. Vollständige Quellenanalyse verweigert jeden Reparaturplan für diesen Fall. Kein fehlender Endton wird aufgefüllt.

**Zusätzlich durch Build 51:** der gezielte Zwei-Frame-Keep rund um die nachgewiesene Lücke durchlief normalen Abbruch, neue Vorprüfung, bewusst simulierte Bestätigung, vollständigen Motorneustart und Endprüfung: **2 Bilder, 3.840 Nutzvektoren, davon 512 bestätigte Stillevektoren**, SHA-256 3afa9c1b941a1b2c70b8c610cc99ea60233fa58bc4089a7f5607b8227da9c4b8. Der vollständige Episodenschnitt oben stammt aus dem separaten Rust-Referenzlauf, nicht aus diesem kurzen CAN-Test. Der zusätzliche finale Hash-Vergleich vor der atomaren Übernahme wurde mit der finalen synthetischen Matrix und im Paket geprüft; der lange Rust-Lauf verwendete den gepufferten Stand unmittelbar davor. Alle Arbeits- und Prüfartefakte bleiben lokal, kein Film in Git.

## Build- und Prüfnachweise

| Prüfung | Ergebnis |
| --- | --- |
| CAN Release-Build 51 und lokale Win-x64-Ausgabe | 0 Warnungen, 0 Fehler |
| Gesamte CAN-Solution | **1.807/1.807** (200 Core, 80 Cutlists, 390 Media, 1.137 App); 12 neue Tests |
| App-Tests gegen die endgültigen Paket-DLLs | **1.137/1.137** |
| Eigenständige Spotlight-Solution am unveränderten Pin | **78/78** |
| Native Spotlight-/CAN-Regressionsprüfungen gegen Paket | **32/32** |
| Rust ohne Feature / mit Feature | **63/63 / 78/78**, Release-Builds ohne Compilerwarnungen |
| Bestehende Motor-Medienregressionen im Normalaufruf | **14/14** |
| Neue Rust-Medienmatrix | **17/17** |
| Native CAN-Matrix mit finalem Motor und Paket-DLLs | **15/15**, davon 14 synthetisch und ein gezielter Realfall |
| Vollständiger NCIS-Fall A, AAC-/Video-Beweise | bestanden; Fall B wie vorgesehen abgelehnt |

Erfasst: gesundes AAC, Einzel- und Mehrfachlücken, außerhalb der Keeps liegende Lücken, Schnittgrenzen, unpassende und größere Lücken, Audioende, Mehrspur, Abbrechen vor/während Verarbeitung, Dateischutz, bestätigter MP4Box- und Reparatur-Neustart, tatsächliche Motorablehnung bei absichtlich falscher Bestätigung und erneute Entscheidung, Einzelbild vor kopiertem IDR sowie Ablehnung der alten verlustfreien Profilvariante. Gesunde Ausgaben der bestehenden Motor-Matrix bleiben bytegleich. Der erste native Versuch enthielt einen Testdatenfehler (Ende als Dauer); er wurde im Test korrigiert und nicht als bestanden gezählt. Der vollständige NCIS-Lauf wurde nach Korrektur eines ungepufferten JSON-Lesers neu gestartet. Keine Prüfschwelle wurde zur Erlangung eines grünen Ergebnisses gelockert.

## Grenzen und Git

Die experimentelle Option ist bewusst auf genau eine nachgewiesene AAC-LC-Stereospur/48 kHz/fltp/1:48000, regulären Vorlauf und 512er-Quelllücken begrenzt: maximal acht ausgewählte ursprüngliche Lücken, insgesamt 4.096 ergänzte Vektoren. Nur die geprüfte H.264-High-Klasse, progressiv, 25 fps, 8 Bit yuv420p, maximal 1920×1080, auf exakten Framegrenzen. Vorbereitete Arbeitsquellen, Mehrspur und fehlendes Audioende bleiben außen vor. Mehrere vollständige Dekodierungen benötigen zusätzliche Zeit und temporären Speicher. Die unveränderte Tonkodierung ist weiterhin verlustbehaftet; nachgewiesene Lückenstille ersetzt keine fehlenden Originalinhalte.

Rust-Commit **1979a7f897b95d3fab4020129b8b5e634ea569ac**, Branch codex/aac-gap-rust. CAN-Entwicklungszweig codex/audio-three-options auf dem bestätigten Paket-1-Commit ba1c482; der endgültige CAN-Commit und die Prüfsummen stehen in der lokalen Paket-Provenienz. Spotlight-Modul unverändert **6973b37591e9b581b20b5a6f2117369b6df0f159**. Hauptarbeitskopien und deren vorhandene Nutzeränderungen sind erhalten. Keine Tags, kein Push.

Die persönliche Abnahme des vollständigen NCIS-Schnitts mit Build 51 ist im Nachtrag oben dokumentiert. Für die separate Labor-MP4 liegt die positive MPC-HC-Rückmeldung zum Einzelbild bei 00:33:18.760 vor. Eine Produktivfreigabe erfolgt gesondert.
