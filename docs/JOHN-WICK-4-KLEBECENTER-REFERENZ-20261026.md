# John Wick 4 – geprüfter Referenzfall für das Klebecenter

**Historischer externer Referenztest vom 26.10.2026.** „Noch nicht integriert“ unten beschreibt den damaligen Stand. Seit Build 39 ist das [Klebecenter integriert und persönlich geprüft](KLEBECENTER-20261027.md), aktuell unverändert in **Build 40 mit otr-can 0.1.6**. [Versionen](VERSIONEN.md).

**Stand: 26.10.2026.** Separat mit FFmpeg ausgeführt und technisch geprüft. Der Nutzer bestätigt ausdrücklich **„sauberer Übergang“**. Das Zusammenfügen ist noch nicht in CAN integriert.

## Ausgangslage und Problemlösung

otr-can 0.1.6 brach bei Teil 2 wegen eines AC-3-Decoderfehlers ab. Der bestätigte AAC-Notfallweg aus Build 37 erzeugte die passende Teil-2-Ausgabe mit einer intakten AAC-Spur. Teil 1 enthielt noch AAC und AC-3; über die manuelle Tonspurauswahl aus Build 38 wurde deshalb eine zusätzliche AAC-Kopie ohne erneute Kodierung erstellt. Beide Originalausgaben blieben erhalten.

| Datei | Dauer | Videopakete | Dekodierte 48-kHz-Tonsamples |
|---|---:|---:|---:|
| John Wick Kapitel 4 Teil 1 otr-can - AAC.mp4 | 01:57:37.120 | 176428 | 338741760 |
| John Wick Kapitel 4 Teil 2 mit Audio Korrektur.mp4 | 00:41:12.040 | 61801 | 118657920 |
| John Wick Kapitel 4 mit Audio Korrektur.mp4 | 02:38:49.160 | 238229 | 457399680 |

Ablage beim Nutzer: **Desktop/down**. Beide Eingaben haben H.264 High, 1920×1080, 25 fps und AAC-LC/Stereo/48000 Hz. Die Video-/Audio-Decoderkonfigurationen stimmen zwischen den Eingaben überein.

## AAC-Auffüllung und exakte Tonzeit

Die Eingaben haben individuelles Decoder-Priming und Endpadding:

| Teil | Anfänglich zu überspringende Samples | Am Ende zu verwerfende Samples |
|---|---:|---:|
| 1 | 1024 | 512 |
| 2 | 1024 | 128 |

Gleiche Codecs und passende Kanäle allein beweisen deshalb noch kein samplegenaues Aneinanderhängen beider kompletten AAC-Paketfolgen. Die Auffüllwerte müssen pro Eingabe berücksichtigt werden; sie dürfen nicht als feste John-Wick-Konstanten in Produktcode übernommen werden.

Für diesen Referenzfall wurden beide Originaltonspuren dekodiert, mit ihrer ausgewiesenen Beschneidung samplegenau als PCM zusammengesetzt und anschließend **einmal durchgehend nach AAC-LC/Stereo/48000 Hz/192 kbit/s** kodiert. Der Ton wird dabei verlustbehaftet neu kodiert. Das Bild wird nicht neu kodiert.

Die gesamte PCM-Zusammensetzung wurde byteweise gegen die nacheinander dekodierten Originale verglichen: **457399680 Samples**, SHA-256 **44eb4d7f104fd64405528d8ac7cd52538fb0866af7434d4ea9a5faebf7132a10**.

## Erkannter Bildzeitversatz und Korrektur

Der erste FFmpeg-Zusammenbau ohne explizites **-copyts** verschob die Bildzeitstempel um **273/12800 Sekunden**, also rund **21.328 ms**. Die Containerlaufzeit betrug dadurch zunächst 9529.181328 statt 9529.16 Sekunden. Die Prüfung erkannte diese Abweichung vor der Übernahme der Zieldatei.

Der abschließende Streamcopy-Zusammenbau verwendet **-copyts** und übernimmt das Video erneut direkt aus der Original-Teileliste; **-auto_convert 0** verhindert hier eine unnötige Bitstromumwandlung der bereits passenden H.264-Streams. Der bereits einmal kodierte durchgehende AAC-Ton wird aus der Zwischenfassung kopiert. Es erfolgt keine weitere Kodierung.

Dies ist der tatsächlich geprüfte Ablauf. Ein künftig vereinfachter direkter Erstaufbau mit korrekten Zeitstempeloptionen muss ebenfalls vollständig geprüft werden, bevor er in CAN übernommen wird.

## Technischer und persönlicher Nachweis

- **238229 Videopakete**: Hash, Größe, Flags, PTS/DTS und Dauer exakt gegen beide Originale verglichen. Teil 2 liegt genau um 7057.12 Sekunden hinter Teil 1.
- **457399680 dekodierte Ausgabetonsamples**, exakt die Summe beider Eingaben.
- **446681 AAC-Pakete** mit kontinuierlichen Zeitstempeln.
- Alle AAC-Pakete, Konfigurationen und Paddingangaben beim abschließenden Umpacken unverändert.
- Sechs Sekunden um die Klebestelle vollständig ohne Warnungen oder Fehler dekodiert.
- Die beiden Ausgangsdateien sind erhalten; private große WAV-/Zwischen-MP4-Dateien nach erfolgreicher Prüfung entfernt.
- Fertige Datei **2820655094 Bytes**. Der Nutzer bestätigt den sauberen Übergang; daraus wird keine vollständige Sichtprüfung des gesamten Films abgeleitet.

Der einzige Hinweis im ersten Encoding war die Stereo-Kanallayout-Erkennung des zweikanaligen PCM-WAV. Kanalzahl und Samplerate waren unabhängig geprüft; dies war kein Decoderfehler. Der abschließende Streamcopy-Zusammenbau und der Übergangsdecode liefen ohne Warnungen.

## Prüfstelle

**Klebestelle 01:57:37.120**, Drei-Sekunden-Vorlauf ab **01:57:34.120**. Gesamtlaufzeit **02:38:49.160**.

## Grundlage für die CAN-Integration

Die Fortsetzung am **27.10.2026** soll daraus einen verständlichen Ablauf entwickeln: Teile auswählen, Reihenfolge festlegen, Tonspuren auswählen, Kompatibilität prüfen, zusammenfügen und das Ergebnis kontrollieren. Vorhandene Dateien bleiben erhalten; Fehler oder nicht bestätigbare Zeitverhältnisse dürfen nicht als Erfolg erscheinen.

Dieser Fall liefert Referenzdateien und einen belegten Prüfweg. Er ist keine allgemeine Freigabe für alle Container, Videocodecs, AAC-Varianten oder Tonspurkonfigurationen. Die AAC-Notfallbehandlung repariert weiterhin keine defekte AC-3-Spur.

Lokale technische Belege im ignorierten Ordner **.build/john-wick4-join-20261026/**: **RESULT.json**, **PCM-PROOF.json**, **OUTPUT-METADATA.json**, **AAC-REMUX-PROOF.log**, **TRANSITION-DECODE.log**, **VERIFICATION-NOTES.md** und die Monitorverzeichnisse mit den tatsächlich verwendeten FFmpeg-Argumenten. Die Filme selbst werden nicht in Git aufgenommen.

[Projektstand](AKTUELLER-STAND.md), [AAC-Notfallweg](OTR-CAN-AAC-NOTFALLWEG-20261026.md), [manuelle Tonspurauswahl](TONSPURAUSWAHL-FILMTEILE-20261026.md), [Merkliste](SETUP-MERKLISTE.md).
