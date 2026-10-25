# Cut Assistant Next 1.0.2 · Build 36

**Lokaler Nachfolgestand vom 25.10.2026; nicht veröffentlicht.** Setup, Portable-ZIP, passende Quellen und Prüfsummen werden getrennt von Build 35 erstellt. Eine Veröffentlichung erfolgt nur nach ausdrücklicher Nutzerfreigabe; es gibt keinen automatischen Termin.

## Neu gegenüber Build 35

Beim Erzeugen einer Cutlist lässt sich der vorgesehene Schnittmotor sichtbar auswählen. CAN schlägt den zuletzt erfolgreich für dieselbe Aufnahme verwendeten Motor vor, sonst den Motor der geladenen Cutlist. Bei otr-can schreibt CAN den EXE-Dateinamen und die durch --version ermittelte Motorversion statt der MP4Box-Angaben. Ist die Version nicht ermittelbar, bleibt dieses Feld leer. Der Kommentar-Schnellbaustein bleibt unabhängig von der Motorwahl.

## Änderungen

Die MP4Box-Schnittkontrolle kann jetzt auch bestimmte geringfügig überlappende H.264-Bildzeiten im fertigen Film eindeutig zuordnen. Der Kontrollplayer startet drei Sekunden vor der zugeordneten Prüfstelle. Abschlussfenster, Zwischenablage und Player erklären die kleine Überlappung.

Die Zuordnung setzt die vollständige geordnete Videopaketfolge, passende Zeitwerte und bestätigte dekodierte Nachbarbilder voraus. Sie erkennt das unterstützte Muster unabhängig vom Filmnamen. Größere oder mehrdeutige Überschneidungen bleiben bei ausdrücklich berechneten Prüfstellen. Der bestehende Prüfweg für gleiche oder nicht überlappende Bildzeiten bleibt erhalten.

MP4Box-Schnittparameter und Cutlists werden dadurch nicht verändert. Die Prüfung stellt keine fehlenden Bilder wieder her und beseitigt keine Werbeblitzer. Der Vera-Befund zu ausgelassenen Anfangsbildern bleibt bestehen.

**otr-can 0.1.6** und die geprüfte libmpv-0.41.0-Laufzeit bleiben unverändert enthalten. Keine neue Produktionsabhängigkeit.

## Prüfbasis und Grenzen

Der Entwicklungsstand besteht **1416 Solution-Tests und 40 isolierte Diagnosetests**. Ein neuer Furiosa-MP4Box-Schnitt über den regulären App-Service bestand vier native Playerprüfungen. Persönlich bestätigt sind Furiosa mit MP4Box sowie S.W.A.T. und Fire Country mit dem jeweils dokumentierten Prüfumfang. Furiosa mit otr-can wurde in der aktuellen Serie dreimal erfolgreich gemeldet; S.W.A.T. ebenfalls persönlich positiv geprüft. Paketprüfungen und persönliche Setup-/Portable-Abnahme werden separat dokumentiert; frühere Build-34-Paketabnahmen gelten nicht automatisch für Build 35.

**Offen bleibt die Ursache früherer vollständiger Rechnerstillstände bei großen wiederholten Furiosa-Schnitten mit otr-can.** Positive spätere Läufe belegen keine gezielte Fehlerbehebung. Diese Version enthält keine behauptete Korrektur dieses Systemfehlers. Auch der technische Befund zur letzten alten Bildreferenz am ersten Furiosa-MP4Box-Übergang bleibt erhalten. AVI bleibt experimentell. Bild, Ton, Übergänge und Tonsynchronität persönlich kontrollieren.

## Voraussetzungen

Windows 11 x64 mit separat installierter **.NET 10 Desktop Runtime x64**. FFmpeg/ffprobe, GPAC/MP4Box und FFMS2 müssen eingerichtet sein; für otr-can ist die passende Microsoft-VC++-Laufzeit erforderlich. Vorhandene persönliche Werkzeugpfade bleiben konfigurierbar.

[Freeze und Veröffentlichungsgrenze](FREEZE-1.0.2-20261022.md), [MP4Box-Nachweis](MP4BOX-UEBERGANGSINTEGRATION-20261022.md), [Furiosa-otr-can-Prüfungen](OTR-CAN-FURIOSA-PRAXIS-20261022.md), [Nutzeranleitung](NUTZERANLEITUNG.md).
