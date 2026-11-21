# Cut Assistant Next 1.0.3 Build 62 – persönliche Windows-11-Prüfung

Diese Prüffassung setzt auf Build 61 auf. Die stabile Veröffentlichung bleibt Build 61. Build 62 wird weder automatisch veröffentlicht noch installiert. Die [persönliche Teilabnahme vom 19.11.2026](PRAXISTESTS-BUILD62-20261119.md) dokumentiert die bereits bestandenen Prüfungen der ersten Prüffassung. Die Gesamtabnahme und die Nachprüfung der Ergänzungen bleiben offen.

## Schnittschaltflächen und Tastatur

In Power-User 1 stehen oben **Videoanfang | Videoende**, darunter **Schnittanfang setzen | Schnittende setzen**. Standard und Power-User 2 behalten ihre bisherige Anordnung. Im grünen Modus bleiben die Behalten-Beschriftungen erhalten.

Unter **Einstellungen → Tastenkürzel** gibt es zusätzlich Schnittanfang, Schnittende, Videoanfang und Videoende. Die Werkseinstellungen sind **A / E / Umschalt+A / Umschalt+E**. Die Spalte Standard zeigt diese Kombinationen. Bestehende persönliche Belegungen einschließlich ausdrücklich deaktivierter Aktionen bleiben beim Laden erhalten. Fehlende neue Aktionen werden nur mit konfliktfreien Werkseinstellungen ergänzt; belegte Tasten werden nicht verdrängt. Einzel-Standard stellt die ausgewählte Belegung wieder her, sofern kein Konflikt besteht. Alle zurücksetzen stellt sämtliche Werkseinstellungen wieder her. Konfliktmeldung, Speicherung und Neustart nachprüfen. In allen drei Ansichten und beiden Schnittmodi prüfen. Gedrückthalten darf eine Schnittmarkierung nur einmal auslösen. Die Navigation behält ihre vorhandene Wiederholung. In Texteingabefeldern hat die Eingabe Vorrang.

## Temporärer Arbeitsordner

Unter **Einstellungen → Standardordner → Temporärer Arbeitsordner** wählen:

1. **Ausgabeordner (Standard)** – bisherige Vorgabe bei neuen und bestehenden Einstellungen.
2. **Windows-TEMP-Verzeichnis**.
3. **Benutzerdefinierter Arbeitsordner** – vorhandenen, beschreibbaren Ordner angeben.

Die Auswahl gilt für MP4Box und otr-can einschließlich ihrer Arbeitskopien. Jeder Schnitt verwendet einen privaten Unterordner. Vor dem Schneiden werden Erreichbarkeit, Schreibzugriff und freier Speicherplatz im Arbeits- und Zielordner geprüft. MP4Box erhält auch für seine eigenen Arbeitsdateien den gewählten Ordner über `-tmp`.

Im Protokoll stehen die tatsächlich benutzten Ordner, gemessene freie Bytes/GiB, die geschätzten zusätzlichen Reserven und das Prüfergebnis. Der Bedarf berücksichtigt Schnittsegmente, Zusammenfügen, Zielkopie sowie bei otr-can PCM-/Indexarbeiten. Die Schätzung ist bewusst vorsichtig und keine Platzgarantie. Bei geschätztem Platzmangel startet der Schnitt nicht; Platz freigeben oder Ordner wechseln. Eine nicht mögliche Speicherplatzmessung wird ausdrücklich als unvollständige Prüfung protokolliert.

Bei verschiedenen Laufwerken wird die fertige Ausgabe vollständig in eine private Datei im Zielordner kopiert, geschlossen und erst danach auf den endgültigen Namen umbenannt. Ein schon vorhandener Zielname wird ausschließlich mit der bisherigen ausdrücklichen Überschreibbestätigung ersetzt. Eine inzwischen veränderte oder neu hinzugekommene Zieldatei wird geschützt. Für die Übernahme wird vorübergehend zusätzlicher Platz im Zielordner benötigt. Bekannte Arbeitsdateien werden nach Erfolg und kontrolliertem Abbruch entfernt; Bereinigungsfehler stehen im Protokoll.

## Speicherinformationen im Einstellungsdialog

Im Bereich Temporärer Arbeitsordner stehen der tatsächlich aufgelöste Pfad, die von Windows gemeldete Datenträgerkapazität, der freie Speicherplatz und die Größe der Dateiinhalte im gewählten Arbeitsordner. Alle Größen sind in GiB angegeben. **Aktualisieren** liest sie erneut. Änderungen an der TEMP-Auswahl, am benutzerdefinierten Pfad oder am Ausgabeordner lösen eine neue Abfrage aus.

Datenträgerwerte erscheinen unabhängig von der langsameren Ordnerberechnung. Diese läuft im Hintergrund, ist abbrechbar und zeitlich sowie nach Anzahl der Einträge begrenzt. Veraltete Ergebnisse überschreiben keine neue Auswahl. Bei Zugriffsfehlern, Verknüpfungen oder nicht vollständig ermittelbarem Umfang erscheint ein Hinweis anstelle einer scheinbar vollständigen Größe. Es werden keine Dateien gelöscht.

Die Ordnergröße ist die Summe der Dateigrößen, **nicht** der freie Datenträgerplatz und auch keine physische Belegungsmessung von Komprimierung oder Dateisystem-Metadaten. Laufende TEMP-Dateien können sich während der Abfrage ändern. Die unabhängige Speicherplatzprüfung unmittelbar vor dem Schnitt bleibt unverändert.

## Protokoll und Diagnose

Das vollständige Protokoll ist vertikal und horizontal scrollbar und bleibt über **In Zwischenablage** verfügbar. Ältere Einträge werden in der Anzeige nicht abgeschnitten. Beim Hochscrollen folgt die Anzeige nicht weiter automatisch dem Ende; am Ende wird die automatische Fortschreibung wieder aufgenommen.

Fehler erscheinen deutlich rot und fett oberhalb des Protokolls, auch wenn es eingeklappt wurde. Dort steht eine begrenzte Zusammenfassung, im Protokoll die vollständige technische Ausnahme einschließlich innerer Ursachen. Nach einem Fehler erfolgt kein automatisches Schließen.

Eindeutige Betriebssystem-/Werkzeugmeldungen über Platzmangel werden gekennzeichnet. Ein allgemeiner Write-Error beweist keinen Platzmangel; hier nennt CAN die noch offene Ursache und verweist auf die Messwerte und technischen Meldungen. Bei Schnitt-/Übernahmefehlern ergänzen spätere Messwerte die Werte vor dem Start. Daraus werden keine unbelegten Aussagen über beschädigte Quellen oder Audiofehler abgeleitet.

## Prüfrunde

- Power-User 1: gewünschte Anordnung; Standard/Power-User 2 vor und nach mehreren Wechseln unverändert.
- Vier Tastenaktionen: Rot/Grün, alle Ansichten, Gedrückthalten, Eingabefelder und Neustart.
- Beide Motoren jeweils mit Ausgabeordner, Windows-TEMP und eigenem Arbeitsordner schneiden. Tonspuren, Bild, Übergänge und Drei-Sekunden-Vorschau prüfen.
- Arbeitsordner auf einem anderen physischen Laufwerk als dem Ziel wählen und die sichere Dateiübernahme prüfen.
- Kontrolliert beim Schneiden bzw. Kopieren abbrechen: Original und bisherige Zieldatei müssen erhalten bleiben, Auftragsordner anschließend entfernt sein.
- Protokoll vergrößern/verkleinern, ältere Einträge und lange Zeilen ansehen und alles kopieren. Bei Fehlern vollständiges Protokoll übergeben.
- Setup-Upgrade der ersten Prüffassung am Arbeitsrechner ist persönlich bestanden. Aktualisierte Prüffassung und beide Ergänzungen nachprüfen; Portable-Start und VM-Installation/-Deinstallation bleiben offen.
- Die bisherige vollständige MP4Box-Abnahme von Wetterfee bleibt gültig dokumentiert. Die gesonderte EAC3-/MP4Box-Fallback-Ausgabe noch auf Bild, Ton und Synchronität prüfen.

Keine Freigabe für GitHub-Veröffentlichung oder Änderung der stabilen Ausgabe. Keine neue EAC3-Verarbeitung, kein Batchcenter, kein LosslessCut-Import/-Export und keine Änderung der Namensmaske II.
