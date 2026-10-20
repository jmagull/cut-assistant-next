# Cut Assistant Next 1.0.1 · Build 34

Lokaler Paketsatz vom **19.10.2026**, zur persönlichen Setup-/Portable-/Windows-11-VM-Abnahme. **Noch nicht veröffentlicht.** Die veröffentlichte Version 1.0.0 · Build 32 bleibt unverändert.

## Neu gegenüber 1.0.0

- Der MP4Box-Abschluss und Kontrollplayer verwenden nach erfolgreicher Zusatzanalyse die tatsächlichen Übergangszeiten des fertigen Films. Die Schnittprüfung startet wie vorgesehen drei Sekunden vor dem Übergang, bei sehr frühen Übergängen entsprechend am Dateianfang.
- Das Abschlussfenster unterscheidet bestätigte Ausgabezeiten sichtbar von nur berechneten Prüfstellen. Kopierte Hinweise und Kontrollplayer verwenden dieselbe Zuordnung.
- Scheitert die zusätzliche Analyse oder wird sie abgebrochen, bleibt der bereits erfolgreich geschnittene Film erhalten. Der Hinweis erklärt den Rückgriff auf berechnete Prüfstellen.
- Die bestätigten Zeiten sind an die konkrete Ausgabedatei gebunden. Eine zwischenzeitlich ausgetauschte Datei wird vor dem erneuten Laden erkannt.

MP4Box-Schnittparameter und Cutlists bleiben erhalten. **otr-can 0.1.6** ist in Setup und Portable enthalten. Bei nachgewiesenen einzelnen AAC-Zeitüberschneidungen korrigiert er gezielt die PCM-Auswahl; strenge Sample-, Zeitstempel- und Ausgabeprüfungen bleiben bestehen. Keine neue Produktionsabhängigkeit.

## Voraussetzungen und Grenzen

Windows 11 x64 mit separat installierter **.NET 10 Desktop Runtime x64**. FFmpeg/ffprobe, GPAC/MP4Box und FFMS2 müssen eingerichtet sein. Für otr-can ist die passende Microsoft-VC++-Laufzeit erforderlich. Geprüfte libmpv-Dateien und otr-can 0.1.6 sind enthalten; externe Medienwerkzeuge und .NET-Laufzeiten werden nicht mitgeliefert.

Die Zusatzanalyse liest die Videodaten und kann zusätzliche Zeit benötigen. Nicht eindeutige Zeitstrukturen oder nicht bestätigte Zugriffsstellen führen zu ausdrücklich berechneten Prüfstellen. Eine erfolgreiche Positionsprüfung stellt keine vom Schnittmotor ausgelassenen Bilder wieder her und beseitigt keine Werbeblitzer. Bei Musikladen bleibt der persönlich gemeldete NDR-Blitzer am MP4Box-Anfang bestehen. Bei Radical bleibt der bekannte MP4Box-Blitzer bestehen; die letzte persönliche Prüfung verwendete wegen uneindeutiger Segmentzeiten berechnete Prüfstellen.

**Musikladen mit otr-can 0.1.6:** 34 anhand der Originalzeitstempel einzeln bewiesene Ein-Sample-Konflikte werden kanalweise gemeinsam behandelt. Der folgende AAC-Frame besitzt die strittige Zeitposition; ausschließlich der darüber hinausreichende letzte Samplevektor des Vorgängers entfällt. Keine pauschale Toleranz, globale Verschiebung oder allgemeine Reparatur beschädigter Aufnahmen. Technischer Nachweis: 53 Rust-Tests, 14 vollständige synthetische Medienfälle und echte CAN-Integration. Testfilm und eigener CAN-Schnitt persönlich positiv bestätigt: kein NDR-Blitzer, Bild und Ton einwandfrei. Weiterer Realfilm Navy CIS persönlich positiv geprüft: Bild, Ton und Übergänge; AAC-/AC3-Prüfungen erfolgreich. [Integration, unabhängiger PCM-Nachweis und Grenzen](OTR-CAN-0.1.6-INTEGRATION-20261019.md).

AVI bleibt experimentell. Die bestehenden AVI-Bedienungssperren und Zeitstrukturprüfungen bleiben erhalten. Nicht jedes Medienformat und nicht jede Aufnahme wird unterstützt. Bild, Ton und alle Schnittstellen persönlich kontrollieren.

## Prüfbasis und persönliche Paketabnahme

Die integrierte MP4Box-Verbesserung wurde mit **1385/1385 automatisierten Tests**, Release-Build ohne Warnungen/Fehler und technischen Kontrollplayer-Prüfungen erprobt. Persönliche Rückmeldungen liegen für sieben Schnitt-/Prüffälle mit sechs Aufnahmen vor; [Einzelnachweise und Einschränkungen](UEBERGANGSPRUEFUNG-INTEGRATION-20261018.md).

Für Build 34 werden Restore, Build, Tests und Publish im eingefrorenen Quellstand sowie Paketinhalt, Lizenzen, Motor und Prüfsummen erneut geprüft. Die abschließenden Ergebnisse werden im separat bereitgestellten Paketprüfbericht dokumentiert. Die persönliche Setup-/Portable-/VM-Abnahme von Build 34 ist noch offen; frühere Paketabnahmen und die positiven lokalen Filmprüfungen ersetzen sie nicht.

[Benutzeranleitung](NUTZERANLEITUNG.md). Commit, Push, Tag und Veröffentlichung sind für diesen lokalen Paketschritt nicht beauftragt.
