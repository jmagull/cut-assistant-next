# Paketsatz 0.3.0 · Build 30 · RC3

Stand: 14.10.2026. Vom Nutzer ausdrücklich zum Einfrieren, Bauen und Veröffentlichen freigegeben. Neue Buildnummer 30, bestehende Version 0.3.0 und Kennung RC3. Build 29 bleibt unverändert.

## Enthaltener Stand

- Sichtbare Frame-Lupe rechts vom festen Bereich „Vorgemerkter Anfang“, Shift+F erhalten; ±10-Bilder-Buttons erklären Strg+Klick für ±20.
- Bei leeren Staffel-, Folge- und Folgentitelfeldern endet die automatische Filmmaske nach dem Namen. Bewusstes Neuerzeugen verwendet die aktuelle Maske; geschützte/manuelle Vorschläge bleiben erhalten.
- Frame-Lupe ohne ausgewählte Schnittkante an der erreichten Playerposition; Übernahme im Untersuchungsmodus gesperrt. Bestehender Kantenmodus und AVI-Lupe erhalten.
- MP4-Keyframe-Navigation und blaue Bereiche im vorhandenen Schnittstreifen, K direkt hinter aktueller Framenummer. AVI von Analyse/Buttons/Markern/K im Hauptfenster ausgenommen, Hinweis im AVI-Infoblock.

## Verfahren und Prüfgrenzen

Paketierung aus einem festen Git-Quellcommit mit committed Lockdateien, Quelle mit Dateimanifest und Herkunftsangaben. Vier bereits vor Arbeitsbeginn veränderte Produktions-Lockdateien werden nicht committet und nicht für den Quell-Snapshot verwendet. Setup, Portable und Quellen werden gemeinsam geprüft und mit SHA-256 versehen.

Vor Paketierung: **1215/1215 CAN-Tests**, 0 übersprungen, Release-Rebuild ohne Warnungen/Fehler. 18 Keyframe-Integrationsprüfungen, 46 Untersuchungsmodus-Prüfungen und ergänzende kompilierte K-Anzeige-Prüfung sind in den jeweiligen Entwicklungsberichten dokumentiert. Eine lange HD-MP4 lieferte 3448 Keyframes in 26,6 Sekunden. Der Nutzer bestätigte Bedienung/Filmnamensdialoge/Untersuchungsmodus und gab abschließend den Gesamtstand frei. Diese Rückmeldungen ersetzen keine frische Build-30-Setup-/VM-Abnahme.

Unverändert: otr-can 0.1.5, libmpv 0.41.0 und externe Werkzeuge. Keine neue Produktionsabhängigkeit. Bekannte AVI-Zeitstempelgrenzen und der dokumentierte MP4Box-Befund mit zwei gleichartigen AAC-Spuren bleiben bestehen. .NET 10 Desktop Runtime x64 und externe FFmpeg-/MP4Box-/FFMS2-Werkzeuge bleiben separat einzurichten.

Paket- und Veröffentlichungsnachweise werden nach erfolgreicher Prüfung mit konkreten Hashes, Quellcommit und öffentlichen Links ergänzt. Es werden keine ungeprüften Installationen oder Schnittergebnisse behauptet.
