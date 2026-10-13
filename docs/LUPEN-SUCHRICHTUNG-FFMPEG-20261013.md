# Lupen-Suchrichtung und FFmpeg-Versionsprüfung – 13.10.2026

## Änderung

Die Lupensuche halbiert nur bei einem Wechsel der Suchrichtung. Der erste Sprung und weitere Sprünge in derselben Richtung verwenden die aktuelle Weite; ein Sprung in die Gegenrichtung verwendet bereits den halbierten Wert. Bei Startwert 2000: **+2000 → +2000 → −1000 → −1000 → +500**. Ungerade Werte werden abgerundet, Minimum 1 Frame. Links-/Rechtsbuttons zeigen ihre tatsächliche nächste Weite. Normale ±1/±10/±20-Schritte verändern weder Weite noch gemerkte Suchrichtung. **Suche neu starten** setzt beides zurück.

Neue Richtung und Weite werden erst nach tatsächlicher Bewegung und erfolgreicher Bildvorschau übernommen. Fehler, Abbruch und ein bewegungsloser Sprung an der Dateigrenze verbrauchen keine Halbierung. Teilweise auf die Dateigrenze begrenzte erfolgreiche Sprünge merken ihre Richtung.

Unter **Einstellungen → FFmpeg-Werkzeuge** ergänzt **Prüfen** eine gemeinsame Kontrolle für FFmpeg und ffprobe. Vom Nutzer bestätigte Regel: **Version größer als 5.1 erforderlich, ab 9.0.2 empfohlen**. 5.1/5.1.0 werden abgewiesen, 5.1.1 ist zulässig. Versionskomponenten werden numerisch verglichen; unbekannte Versionen, falsche Programme, Startfehler und Zeitüberschreitungen melden keinen Erfolg. Unterstützte Releases unter 9.0.2 erhalten einen Empfehlungshinweis. Vorabversionen werden nicht als empfohlenes finales Release eingeordnet.

Git-Entwicklungsbuilds tragen häufig eine Commit-/Datumskennung statt einer Releaseversion. Für ausdrücklich erkannte Git-Kennungen kann ein libavcodec-Stand ab ABI 60 sowohl in der Build- als auch Laufzeitspalte den Mindeststand neuer als 5.1 belegen; damit wird keine exakte Release-/Patchversion ab 9.0.2 behauptet. Der vorhandene **2026-10-04-git-a35c879992**-Build mit libavcodec **63/63** bleibt unterstützt. Die [offizielle FFmpeg-Übersicht](https://ffmpeg.org/download.html) führt 9.0.2 und die Bibliotheksstände der Releasezweige auf.

Die vorhandene OTR-CAN-Werkzeugkontrolle verwendet dieselbe Prüfung. Nur `-version`-Abfragen, konfigurierbare Pfade und vorhandene Laufzeit-Fallbacks. Prüfung einschließlich Pfadauflösung/Prozessstart im Hintergrund, zeitlich begrenzter Prozessrunner, Abbruch beim Schließen. **Prüfen** speichert keine Einstellungen und installiert keine Werkzeuge. Pfadänderung entfernt das alte Ergebnis. Der FFmpeg-Dialog scrollt bei Mindestgröße; Footer und Abbrechen bleiben erreichbar. Pfadkonfiguration und Speichern bleiben wie bisher unabhängig von einer verpflichtenden Prüfrunde; die Prüfung kennzeichnet unterstützte/nicht unterstützte Werkzeuge, die OTR-CAN-Vorprüfung kann ungeeignete Werkzeuge ablehnen.

## Geänderte Bereiche

- Core: `HalvingFrameSearch.cs`.
- App: Lupen-ViewModel und Hilfetexte in Lupen-/Lupeneinstellungsdialog; FFmpeg-ViewModel/-Dialog, neuer `FfmpegToolChecker`, Einbindung in `OtrCanToolChecker`.
- Media: neue, rein testbare `FfmpegVersionPolicy`.
- Passende Core-/App-/Media-Tests und Benutzer-/Status-/Tagebuch-/Merklisten-Dokumentation. Keine neue Produktionsabhängigkeit oder Änderung an Schnittmotoren, Cutlist-Zeitberechnung und Player.

## Prüfstand

Abschließender Stand **0.3.0 · Build 27 · RC3**: Restore ohne Lockdateischreibung erfolgreich, nummerierter Release-Rebuild über `tools/build.ps1 -Configuration Release -NoRestore` mit **0 Warnungen/0 Fehlern**. **1069/1069 CAN-Tests bestanden** (Core 127, Cutlists 72, App 719, Media 151), keine übersprungen; 51 zusätzliche Fälle gegenüber Build 26. Geänderte C#-Dateien formatiert und Diffprüfung bestanden. Alle acht Lockdateien und alle zuvor erfassten persönlichen Einstellungsdateien per SHA-256 unverändert. Vier vorbestehende Produktions-Lockänderungen ausdrücklich erhalten.

Echte ffprobe-/FFmpeg-Bildprüfungen mit den vorhandenen nummerierten AVI-/MP4-Quellen: sechs Zielbilder pro Variante einschließlich Anfang, Mitte und Nähe zum Ende, ±1 und Rückkehr; Suchfolge **−64 → −64 → +32 → +32 → −16** mit Prüfung jeder tatsächlichen Bildnummer und Buttonweite. AVI/25 fps mit und ohne B-Frames, AVI/50 fps mit B-Frames sowie MP4 mit Original-PTS. Original-Teufelsbrigade mit ±1/±10 und tatsächlichem WPF-Übernahmeklick weiterhin erfolgreich. Keine neuen Motor-Schnittaufträge oder Veränderung der realen Aufnahme.

Versionsgrenzen, numerischer Vergleich, unbekannte/zu große Angaben, falscher Programmname, moderne/alte Git-Bibliotheksstände, Pfad-/Start-/Timeoutfehler und Abbruch automatisiert geprüft. Echter aktueller FFmpeg/ffprobe und vollständige OTR-CAN-Werkzeugvorprüfung erfolgreich. Kompilierte FFmpeg-Dialoge für echten Git-Build, simuliertes empfohlenes 9.0.2 und abgewiesenes 5.1 bei **720×560** und **600×420** geprüft; Status scrollbar, Buttons erreichbar, echte Prüfen-Schaltfläche, keine Bindungsfehler. Schließen während laufender Prüfung bricht diese ab. Keine persönlichen Einstellungen gespeichert. Hilfetexte, kleine AVI-Lupenansicht und Lupeneinstellungstext bei 470×310 visuell gesichtet; Text vollständig sichtbar. **57 Integrationsfälle gegen den abschließenden Build 27 bestanden**, einschließlich echter OTR-CAN-Werkzeugkontrolle und Git-Versionen. Der Vorlauf mit 56 Fällen bleibt separat erhalten.

Positive persönliche AVI-/Schnittbewertungen für Build 26 bleiben erhalten. Richtungswechsel-/Versionsbedienung am 13.10.2026 persönlich bestätigt: „FFmpeg Version check“ und „Halb frame sprung check“. Der Nutzer bestätigt den Essentials-Git-Build vom 12.10.2026 statt des zuvor technisch geprüften Full-Git-Builds vom 04.10.2026; die vorherigen technischen Prüfnachweise bleiben ihrem jeweiligen Werkzeugstand zugeordnet. Öffentliche Build-16-RC3-Ausgabe mit otr-can 0.1.5 unverändert. Kein Setup, Portable-Paket, Release, Commit oder Push. Lokale Belege im ignorierten `.build/loupe-direction-ffmpeg-20261013`.
