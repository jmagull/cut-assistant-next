# Cut Assistant Next Build 61 – Persönliche Praxistests vom 17.11.2026

## Ergebnis und Nachweisumfang

**4 Realfilme × 2 Schnittmotoren = 8 persönlich erfolgreich geprüfte Filmschnitte unter Windows 11.** Bild, Ton und Synchronität, Schnittstellen und Übergänge sowie Drei-Sekunden-Vorschau wurden vom Anwender als einwandfrei bestätigt. Alle vier Referenzfilme wurden im grünen Behalten-Modus geprüft.

Diese aktualisierte persönliche Rückmeldung ersetzt die vorläufige Zusammenstellung desselben Tages mit noch offenen Bild-/Tonbestätigungen. Sie ergänzt die bisherigen automatisierten Prüfungen und persönlichen Abnahmen. Für sieben Motorläufe wurden technische Protokolle übergeben. MP4Box bei „Die zauberhafte Weihnachtsfee“ ist **persönlich bestanden; separates Motorprotokoll nicht übergeben**. Daraus werden keine exakten MP4Box-Ausgabezeiten oder bildgenauen technischen Garantien abgeleitet.

| Referenzfilm | Behaltebereiche / Dauer | MP4Box | otr-can |
| --- | --- | --- | --- |
| Yaiba – Samurai Legend S01E07 | 1 / 00:23:50.400 | Persönlich bestanden | Persönlich bestanden |
| Wild Kat – Partymonster S01E01 | 1 / 00:21:34.000 | Persönlich bestanden | Persönlich bestanden |
| Die zauberhafte Weihnachtsfee | 4 / 01:21:29.120 | Persönlich bestanden; separates Protokoll fehlt | Persönlich bestanden |
| Meine zauberhaften Weihnachtsschuhe | 4 / 01:23:28.000 | Persönlich bestanden | Persönlich bestanden |

## Referenzdaten aus der übergebenen Zusammenstellung

- **Yaiba:** Quelle Yaiba__Samurai_Legend__Kojiros_Wiederauferstehung_S01E07_26.11.16_18-25_pro7maxx_30_TVOON_DE.HQ.mp4. Behalten 00:07:21.880–00:31:12.280. MP4Box: -splitx 441.88:1872.24 und Schnittstellenprüfung erfolgreich. otr-can: 35.760 Bilder kopiert, 35.760 Videopakete und 68.659.200 Audiosamples verifiziert.
- **Wild Kat:** Quelle Wild_Kat__Partymonster_S01E01_26.11.16_18-10_srtl_30_TVOON_DE.HQ.mp4. Behalten 00:09:52.480–00:31:26.480. MP4Box empfohlen; -splitx 592.48:1886.44 erfolgreich. otr-can: 32.350 Bilder kopiert, 32.350 Videopakete und 62.112.000 Audiosamples verifiziert.
- **Weihnachtsfee:** Quelle Die_zauberhafte_Weihnachtsfee_26.11.16_20-15_srtl_105_TVOON_DE.HQ.mp4. Alle vier Behalteanfänge mit K, alle vier Enden ohne K. otr-can empfohlen. Kopierte/neu kodierte Bilder je Bereich: 30.432/92, 35.979/126, 28.439/98 und 26.865/197. Zusammen 121.715 kopierte und 513 neu kodierte Bilder, 122.228 Videopakete und 234.677.760 Audiosamples verifiziert. Auch MP4Box ausdrücklich persönlich ohne Auffälligkeiten bestätigt.
- **Weihnachtsschuhe:** Quelle Meine_zauberhaften_Weihnachtsschuhe_26.11.16_22-00_srtl_105_TVOON_DE.HQ.mp4. Alle acht Grenzen mit K. MP4Box empfohlen; vier Segmente mit -add, dreimal -cat und -new erfolgreich zusammengefügt und geprüft. otr-can: 125.200 Bilder kopiert, 125.200 Videopakete und 240.384.000 Audiosamples verifiziert.

Die unterschiedlichen exklusiven Endzeiten in Cutlist und MP4Box-Aufruf bleiben als übergebene Referenzwerte erhalten. Die aktuelle Dokumentationsänderung bewertet ihre technische Genauigkeit nicht neu.

## Weitere bestätigte Prüfungen

- FFmpeg/ffprobe vom 12.10.2026 erkannt; synthetische Streamcopy-Prüfung mit 200 H.264-Paketen einschließlich Reihenfolge, Zeitstempeln und Hashwerten bestanden.
- GPAC/MP4Box 26.07 erkannt; Funktionstest mit zwei Bereichen und zwei AAC-Tonspuren bestanden.
- otr-can 0.1.6-audio-diag.1 und FFMS2 ausgeführt. Fehlendes Motorpaket im lokalen Testprogrammordner ergänzt; alle 104 Manifestdateien geprüft. Der Anwender bestätigt anschließend den funktionierenden Schnitt.
- Fortlaufender Schnittshuffle und Tooltip persönlich erfolgreich geprüft. Daraus wird keine gesonderte vollständige Abnahme jeder Layout-/Modus-/Grenzfallkombination abgeleitet.
- Wiederholtempo 550/120 ms, Speicherung nach Neustart, Halten und Stopp beim Loslassen persönlich bestanden. Die frühere Abnahme vom 16.11.2026 bleibt erhalten.
- Fokus nach Schließen des Kontrollplayers mit mehreren Fenstern bestanden; Schnittprüfung ohne neue Verarbeitung erneut geöffnet.
- Blaue K-Markierungen und beide Motor-Empfehlungen in mehreren Praxisfällen sichtbar und überprüft. Frühere persönliche Rückmeldungen zum roten Entfernen-Modus, Mehrspurton, Spotlight und Wiederaufruf der Prüfung bleiben erhalten.

Die zuletzt dokumentierte gemeinsame technische Suite auf codex/continuous-schnittshuffle umfasst **2.153 bestandene Tests** (Core 200, Media 402, App 1.471, Cutlists 80), ohne Fehler oder übersprungene Fälle; Release-Build ohne Warnungen oder Fehler. Die vorherigen 2.150 Tests auf codex/navigation-review-focus bleiben als historischer Nachweis erhalten. Für diese reine Dokumentationsänderung wurden Build und Tests nicht erneut ausgeführt; die Dokumentationsdiffs wurden geprüft.

## Noch offene Abnahme

1. **Persönliche Tastenkürzel:** geänderte Kombinationen tatsächlich nutzen, Konflikterkennung, Neustartpersistenz und alle drei Layouts prüfen.
2. **Alternative Cutlist-Qualitäten:** reale Serversuche, Kennzeichnung, Warnungen und bewusste Übernahme anderer Qualitätsstufen prüfen.
3. **Fachliche Schlussbewertung der Motor-Empfehlung:** Behalteanfänge und exklusive Enden unterscheiden, MP4Box an Nicht-Keyframe-Enden sowie Übereinstimmung der K-Anzeige und Empfehlungsgrundlage untersuchen. Der erfolgreiche Weihnachtsfee-Test ersetzt keine allgemeine Genauigkeitsgarantie. Empfehlungslogik unverändert.
4. **Finale Paketprüfung:** gemeinsamen Entwicklungsstand und frisch erstellte Setup-/Portable-Pakete auf Vollständigkeit und Integrität prüfen. Die Motorergänzung im lokalen Testordner ist kein Nachweis für ein neues Auslieferungspaket. Eine persönliche VM-Abnahme dieser Fassung ist nicht bestätigt.

Die acht erfolgreich geprüften Filmschnitte müssen ohne konkreten Anlass nicht wiederholt werden. Persönliche Bestätigungen gelten für die geprüfte Windows-11-Umgebung; keine persönliche Windows-10-Abnahme.

## Entwicklungs- und Freigabestatus

Dokumentiert auf codex/continuous-schnittshuffle, Produktcodebasis 6c21601 mit den zuvor persönlich abgenommenen Timing-/Fokusänderungen aus d81fab9. Die Tests beziehen sich auf die jeweils tatsächlich verwendeten Prüffassungen; sämtliche Einzeltests werden nicht pauschal einer einzigen EXE zugerechnet.

Build 61 bleibt Entwicklungs- und Prüffassung. Dieser Auftrag umfasst die Dokumentation der Prüfergebnisse. Keine neue Buildnummer, kein Merge nach main, kein Tag und keine Veröffentlichung. Bestehende Pakete bleiben unverändert.
