# Doc S02E16 – fertige Cutlist mit beiden Motoren erfolgreich

Stand: **17.10.2026**, CAN **1.0.0 · Build 32**, unveränderter Motor **otr-can 0.1.5**.

**Abschließendes persönliches Ergebnis: „Beide ohne probleme“.** Der vorausgegangene PCM-Abbruch entstand bei einer **noch unfertigen Cutlist**. Nach dem gesetzten Schluss-Schnitt laufen MP4Box und otr-can erfolgreich. Dieser Bericht bewahrt die technische Erklärung des Zwischenstands und seine Auflösung; er führt keinen offenen otr-can-Fehler für den fertigen Doc-Schnitt.

## Fertiger Schnitt und persönliche Rückmeldung

Quelle: `Doc_S02E16_26.10.16_20-15_5plus_50_TVOON_DE.HQ.mp4`, 603506831 Bytes. Bei der Diagnose erfasste SHA-256: **500E88EF41C0873EE3601A1EA5E4EC98A115C0330217D25CAEFC19A47D77AA42**.

Der Nutzer stoppt die erste Untersuchung mit **„die cutlist ist ja garnicht fertig“** und liefert anschließend neue erfolgreiche Protokolle sowie **„Beide ohne probleme“**.

| Abschnitt | MP4Box-Anforderung in Sekunden | otr-can-Protokoll in Sekunden |
|---|---|---|
| 1 | 582.96–2179.96 | 582.96–2180.00 |
| 2 | 2842.36–3737.12 | 2842.36–3737.16 |

MP4Box schneidet beide Abschnitte, fügt sie zusammen und endet mit **„Fertig.“**. Die OTR-Endzeiten liegen jeweils 0,040 s später als die MP4Box-Anforderungen; kein vollständig identischer A/B-Schnittplan behauptet.

otr-can 0.1.5:

- Eine FFMS2-Indexierung, **157446** eindeutige aufsteigende Quellframes und bestätigtes 25-/50-fps-Raster.
- Behalteframes **[14574,54500)** und **[71059,93429)**, insgesamt **62296** Bilder: **62093** kopiert und **203** an den Schnittkanten CPU-kodiert.
- **62296 Videopakete** samt Roh-PTS/DTS verifiziert.
- Eine Stereo-AAC-Spur, **48000 Hz**, Zielbitrate **160281 bit/s**, **119608320** verifizierte dekodierte Samples und kontinuierliche Roh-PTS.
- Abschließende MP4-Verifikation: Video-/Audiopakete, PTS/DTS, Padding und Decoderkonfiguration erhalten.
- Konsistente Sollausgabe aus Behaltezeiten, 25-fps-Frames und Audiosamples: **2491,840 s = 00:41:31.840**. Keine neue unabhängige Messung der fertigen Datei und kein Nachweis einer tatsächlich angeforderten optionalen vollständigen Videodekodierung daraus abgeleitet.

Der Nutzer bestätigt beide Ausgaben persönlich ohne Probleme. Einzelprüfungen jeder Tonspur oder jedes Übergangs wurden in dieser Nachricht nicht gesondert beschrieben; keine zusätzliche VM-/Portable-/Deinstallationsabnahme daraus abgeleitet.

## Merksatz für die Bedienung

**Cutlist einschließlich Schlussbereich fertigstellen. Alle behaltenen Bereiche müssen von den Originaltonspuren vollständig abgedeckt sein.** Bei dieser Aufnahme beseitigt der Schluss-Schnitt den fehlerhaften Zwischenplan, der bis über das Tonende der Gesamtaufnahme reichte.

Daraus folgt **keine allgemeine Pflicht zu einem zusätzlichen Schluss-Schnitt bei jeder Datei**. Sind Video und alle Tonspuren bis zum gewünschten Ende vorhanden, kann auch dieser Bereich behalten werden. otr-can 0.1.5 ergänzt fehlenden Originalton nicht automatisch durch Stille und behält seine strenge Sampleprüfung.

## Historischer Zwischenstand mit unfertiger Cutlist

Der erste MP4Box-Auftrag verwendete `582.96:2179.96` und `2842.36:6297.8`; der otr-can-Plan reichte entsprechend bis **6297.840 s**, ans Ende der kompletten Videoaufnahme. Das Nutzerprotokoll erreichte die Videoverifikation mit **126313** Paketen, brach danach aber ab:

```text
PCM count differs for audio stream 1: expected 242520960, got 242483328
```

Die Video-Vorprüfung mit 157446 Frames hatte bestanden. Die erneute Wiedergabe des Motorlogs innerhalb der Fehlermeldung wird nicht als Nachweis eines zweiten Motorlaufs/einer zweiten Indexierung gewertet.

ffprobe am Original zeigte H.264-Video bis **6297.840 s (01:44:57.840)** und Stereo-AAC bei 48000 Hz bis **6297.056 s (01:44:57.056)**. Die Originaltonspur endet **0,784 s = 37632 Samples** früher. Die letzten Audiopakete bestätigen das Tonende; das letzte AAC-Paket enthält 512 Samples ausgewiesenes Endpadding. Der unfertige zweite Behaltebereich reichte über den vorhandenen Ton hinaus.

Eine unabhängige Audiodekodierung mit dem unveränderten 0.1.5-Motorfilter (`-copyts`, `-reinit_filter:a 0`, `asplit`, zwei `atrim`-Zweige, `asetpts`, `concat`) reproduzierte **exakt 242483328 Samples** statt 242520960. PCM-Frame-Prüfsummen statt einer Tondatei gespeichert; erste Ausgabe-PTS null, Samplefolge kontinuierlich, keine Warnungen. Die fehlenden 37632 Samples erklären den damaligen Abbruch. Diese technische Beobachtung bleibt gültig; ihre frühere Einordnung als ungelöster finaler Doc-Fehler ist durch die vollständige Cutlist und den erfolgreichen Folgelauf überholt.

Eine damalige reine Audiovariante mit Behalte-Ende **6297.040 s** lieferte exakt 242482560 Sollsamples ohne Warnungen. Diese Hilfsprüfung hätte nur die letzten 20 Videoframes / 0,8 s abgeschnitten und wurde **nicht als Filmschnitt ausgeführt**. Sie ist keine Empfehlung anstelle der inzwischen fertiggestellten Cutlist; der tatsächlich gewünschte Schluss liegt bereits bei **3737.160 s**.

## Dokumentationskorrektur und Erhaltung

Die Untersuchung wurde auf ausdrücklichen Nutzerstopp beendet; die zu diesem Zeitpunkt noch ausstehende Abschlussprüfung wurde nicht ausgeführt. Nach der neuen Rückmeldung mit fertiger Cutlist wurde die Abnahmedokumentation wieder aufgenommen. README, Anleitung, Projektstatus, Tagebuch und Paketbericht kennzeichnen den Zwischenplan entsprechend und erfassen den finalen Doc-Test als erfolgreich. Kein Quellcode oder Motor geändert, keine PCM-Prüfung gelockert und kein fehlender Ton automatisch ergänzt.

Die Diagnose veränderte weder Original noch Cutlist; die fertige Cutlist wurde vom Nutzer erstellt. Eine neue Prüfung zur Erfassung der finalen Abnahme bestätigt die sieben eingefrorenen Release-Dateien, übrigen Projektdateien und Buildzähler 32 unverändert. Original-SHA-256 wird im neuen Abschlussnachweis erneut gegen den Diagnose-Ausgangshash geprüft. Technischer CAN-Paketnachweis weiterhin **1347/1347 Tests**; kein neuer CAN-Build/Testlauf für diese reine Dokumentationskorrektur.

Lokale Diagnose-Nachweise: `.build/doc-audio-pcm-20261017` mit `source-streams.json`, `audio-tail-packets.json`, `SOURCE-SHA256.txt`, Filterdateien, `original-keep-audio.framecrc`, `DECODE-PROOF.json`, `bounded-end-audio.framecrc`, `BOUNDED-END-PROOF.json` und Warnungsprotokollen. Für die neue finale Nutzerabnahme maßgeblich: `.build/acceptance-doc-finished-cutlist-20261017/ACCEPTANCE.json` und `MP4BOX-USER.log`. Ein vor dem Stopp nicht erstellter Diagnose-Abschlussnachweis wird nicht als vorhandener Beleg ausgegeben.
