# Persönlicher Entwicklungsfassungstest – Yaiba, 10.11.2026

**Cut Assistant Next 1.0.3 Build 55, lokale Entwicklungsfassung:** Der Nutzer meldet Spotlight, MP4Box, otr-can und die Keyframe-Shortcuts positiv. Abschließende persönliche Rückmeldung: „Ton bild schnitt 3 sekunden alles prima, spotlight check, keyframes short cuu check“, anschließend „cut“ als Wortkorrektur. Gemeint sind die Keyframe-Shortcuts.

Film: **Yaiba: Samurai Legend – Das andere magische Schwert, S01E03**, Pro7 Maxx, Aufnahme vom 09.11.2026. Kein eigener neuer Filmschnitt oder eigene Bild-/Tonabnahme in diesem Dokumentationsschritt.

## Gemeldete persönliche Prüfung

| Bereich | Rückmeldung |
| --- | --- |
| Spotlight-CAN | positiv |
| MP4Box-Schnitt | positiv gemeldet |
| otr-can-Schnitt | positiv gemeldet; vollständiges Detailprotokoll mitgeliefert |
| Bild, Ton, Schnitt, Drei-Sekunden-Prüfung | persönlich insgesamt als einwandfrei bestätigt |
| Keyframe-Shortcuts | positiv |

Die Rückmeldung nennt beide Schnittmotoren. Ein separates MP4Box-Protokoll wurde nicht übermittelt; technische Detailaussagen im Folgenden stammen ausschließlich aus dem otr-can-Protokoll und dem Cutlist-Abgleich. Die konkrete neue EOF-Bedienfolge, Sekunden-Sprungweiten, Tooltip-Mausprüfung und alle drei Ansichten wurden in dieser Rückmeldung nicht gesondert beschrieben und werden daraus nicht zusätzlich als persönlich abgenommen gewertet. Auch keine neue Aussage über mehrere Tonspuren; im otr-can-Protokoll ist genau eine Stereospur dokumentiert.

## Cutlist und Protokoll stimmen überein

Die angegebene lokale Cutlist wurde nur gelesen; weder Film noch Cutlist wurden verändert oder ins Repository kopiert. Quelle:

Yaiba__Samurai_Legend__Das_andere_magische_Schwert_S01E03_26.11.09_18-25_pro7maxx_25_TVOON_DE.HQ.mp4.cutlist

SHA-256: 06edaacadfbfc6280a18b414267aa1c0c48c1a6bd3f1eaccd1f181e2f4ec533b

Cutlist-Metadaten: Application=Cut Assistant Next, Version=1.0.3, 25 fps, OriginalFileSizeBytes=350804197 und NoOfCuts=1. Die Buildnummer steht nicht in diesen Cutlist-Metadaten. Die Zuordnung zu Build 55 ergibt sich aus der berichteten Entwicklungsfassung; der Motorpfad im Protokoll liegt im bisherigen lokalen Entwicklungs-Ausgabeordner unter tools/otr-can/otr_can.exe.

| Wert | Cutlist / otr-can |
| --- | --- |
| Behaltebeginn | 378,320 s = 00:06:18.320 |
| Behaltedauer | 1429,480 s = **00:23:49.480** |
| Exklusive Behaltegrenze | 1807,800 s = 00:30:07.800 |
| Behaltene Bilder bei 25 fps | **35.737** |
| Referenzsamples je Kanal bei 48.000 Hz | **68.615.040** |

Die Abschlussanzeige nennt Anfang 00:00:00.000 und Ende 00:23:49.480 im geschnittenen Film. Behaltedauer, Videopaketanzahl und Audiosampleanzahl passen rechnerisch exakt zusammen; mit ganzzahligen Ticks nachgerechnet. Die Anzeige kennzeichnet diese Prüfzeiten selbst als aus dem Schnittplan berechnet. Daraus wird keine zusätzliche unabhängig gemessene Ausgabedauer behauptet.

## otr-can-Nachweis

Gemeldeter Motor: **0.1.6-audio-diag.1** aus dem vorhandenen Build-Ausgabeordner der Arbeitskopie community-feedback. FFMS2 einmal indexiert; vorherige Bildzeitprüfung bestätigt **61.512** eindeutige, aufsteigende Bildzeiten und die unterstützte 25-/50-fps-Zeitstruktur.

Der Motor kopiert für den einen Behaltebereich zunächst 35.689 Bilder im Bereich [378.320000, 1805.880000) und kodiert die anschließenden 48 Bilder bis 1807.800000 auf der CPU. Summe: **35.737** verifizierte Videopakete. Eine AAC-Stereospur mit 48.000 Hz: **68.615.040** Referenzsamples, nach vollständiger Neukodierung genau dieselbe decodierte Anzahl und kontinuierliche rohe PTS bestätigt.

Das Protokoll meldet die abschließende MP4-Prüfung von Video-/Audiopaketen, PTS/DTS, Padding und Decoderkonfigurationen sowie erfolgreich abgeschlossenen Schnitt. Kein Audioreparaturangebot oder Reparaturdurchlauf wird in diesem Lauf gemeldet. Daraus folgt keine neue persönliche AAC-Reparaturabnahme.

## Umfang dieses Nachtrags und Freigabe

Reine Dokumentation der Nutzerprüfung und rechnerischer Cutlist-/Protokollabgleich. Produktcode, Version, Buildzähler, Einstellungen, Werkzeuge und Pakete bleiben unverändert. Keine neuen Build-/Testläufe für diese Dokumentationsänderung; letzter technischer Stand weiterhin 1942 CAN-Tests, 78 Spotlight-Tests und 56 native Prüfungen aus der Keyframe-Ergänzung.

Persönliche lokale Funktionsprüfung für diesen Film positiv. Vollständige neue Plattform-/VM-/Setup-/Portable-Abnahmen und die nicht einzeln genannten Bedienfälle bleiben getrennt offen. Keine Veröffentlichung oder allgemeine Release-Freigabe aus dieser Rückmeldung abgeleitet; keine Installation, kein Commit/Push, Merge oder GitHub-Release.
