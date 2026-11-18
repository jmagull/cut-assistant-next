# Cut Assistant Next 1.0.3 Build 61 – persönliche Abnahme

Auf Windows 11 am Arbeitsrechner beziehungsweise in der VM testen. Die SHA-256-Werte der Prüfpakete vor der Prüfung mit `SHA256SUMS.txt` vergleichen. Bei der portablen Variante das ZIP in einen neuen Ordner entpacken. CAN verwendet .NET Desktop Runtime 10 x64. Die vier libmpv-Dateien und otr-can 0.1.6-audio-diag.1 sind im Paket. FFmpeg, ffprobe, MP4Box und FFMS2 werden über die vorhandene Werkzeugkonfiguration bereitgestellt.

| Prüfung | Ablauf und erwartetes Ergebnis |
| --- | --- |
| Tastenkürzel | Frame-Lupe auf F8 legen, speichern, CAN neu starten und F8 in allen drei Ansichten prüfen. Dieselbe Taste einer anderen Funktion zuweisen. Die Konfliktmeldung nennt die vorhandene Funktion. In einem Eingabefeld Pfeiltasten und Leertaste prüfen. |
| Navigation | Im pausierten Film Einzelbild- und Keyframesprung mit kurzem Klick, gedrückt gehaltener Maus und persönlicher Taste prüfen. Loslassen, Fokuswechsel und Ende der Datei beenden die Wiederholung. S + Pfeiltasten auf Sekundensprünge prüfen. |
| Schnittliste und Hinweis | Eine Grenze am Keyframe und eine daneben setzen. Das K muss an der zugehörigen Zeit stehen. In Rot und Grün, bei der exklusiven Endgrenze, nach Änderung des Plans und nach Laden einer gespeicherten Cutlist wiederholen. Der Keyframe-Tooltip benennt die blauen Markierungen. |
| Motor-Empfehlung | Bei einer H.264-MP4 mit AAC-Tonspuren zuerst geeignete Keyframe-Grenzen und anschließend eine Grenze dazwischen setzen. Die Schaltflächen für die manuelle Wahl beider Motoren bleiben erreichbar. |
| Alternative Cutlists | Option unter Einstellungen → Cutlists aktivieren. HQ und HD derselben Aufnahme suchen. Alternative und Format sind beschriftet. Einen alternativen Treffer auswählen und die Warnung, die bestehenden Importprüfungen und die tatsächlichen Schnittgrenzen prüfen. |
| Werkzeuge | In FFmpeg-Werkzeuge Versions- und Funktionsprüfung getrennt ausführen. In Schnittanwendung die MP4Box-Funktion prüfen. Statusmeldung nach einem Pfadwechsel erneut prüfen. |
| Spotlight | Grobsuche starten, markierte Bereiche in CAN übernehmen und die Feinbearbeitung prüfen. |

## Realfilm für den Wiederaufruf der Schnittprüfung

1. Eine lokal vorhandene H.264-MP4 mit zwei Tonspuren öffnen. Zwei oder mehr Behaltebereiche mit mindestens einem klar erkennbaren Übergang vorbereiten.
2. Mit MP4Box oder otr-can in einen eigens gewählten Testordner schneiden. Den Kontrollplayer über das Abschlussfenster öffnen. Anfang, Übergänge und Ende mit beiden Tonspuren ansehen und anhören.
3. Kontrollplayer und Abschlussfenster schließen. Im Hauptfenster **Schnittprüfung erneut öffnen** wählen. Dieselbe MP4, Prüfstellen und Tonspuren müssen wieder verfügbar sein.
4. Die Prüfung mehrfach öffnen und schließen. Änderungszeit oder SHA-256 der fertigen MP4 vor und nach der Kontrolle vergleichen. Für die Fehlerprüfung die fertige Testdatei umbenennen und den Button erneut wählen.
5. Eine andere Quelldatei öffnen oder einen weiteren Schnitt ausführen. CAN darf die alten Prüfstellen keinem anderen Film zuordnen.

**Aktualisierung 17.11.2026:** Acht persönliche Realfilmschnitte (vier Filme, jeweils beide Motoren) sind für Bild, Ton/Synchronität, Übergänge und Drei-Sekunden-Vorschau bestanden. Der Wiederaufruf der Schnittprüfung wurde ebenfalls persönlich erfolgreich bestätigt. Die obigen Schritte bleiben als Testanleitung erhalten und sind keine pauschale Liste unerledigter Prüfungen. Die noch offenen Abnahmepunkte stehen im [Praxisbericht](PRAXISTESTS-BUILD61-20261117.md).

## Abnahmestand des lokalen Releasekandidaten vom 17.11.2026

Bestanden: acht Realfilmschnitte, Wiederholtempo mit Speicherung nach Neustart, Kontrollplayer-Fokus und fortlaufender Schnittshuffle in der dokumentierten Windows-11-Umgebung. Für sieben Motorläufe liegen Protokolle vor; MP4Box bei der Weihnachtsfee ist persönlich nachbestätigt, ohne separates Protokoll.

Offen bleiben:

1. Eigene Tastenkombinationen mit Konflikten, Neustart und allen drei Layouts.
2. Reale Serversuche nach anderen Qualitäten, Kennzeichnung und Qualitätswarnungen.
3. Fachliche Schlussbewertung der Empfehlung, besonders Keyframe-Anfänge bei exklusiven Nicht-Keyframe-Enden.
4. Freie Namen in MP4Box, otr-can und Cutlist erzeugen: tatsächliche Dateinamen, Speicherung, Wiederladen, Erhalt nach Metadatenänderung und bewusste Neugenerierung. Siehe EDITIERBARE-FILMNAMEN-BUILD61.md.
5. Frisches Setup und ZIP in der Windows-11-VM einschließlich Upgrade, Verknüpfungen und Deinstallation. Keine automatische Installation auf dem Arbeitsrechner. Externe Werkzeugpfade prüfen; persönliche Einstellungen erhalten.

Die acht Filmschnitte müssen ohne konkreten Anlass nicht wiederholt werden. Keine persönliche Windows-10-Abnahme. Veröffentlichung bleibt gesondert freizugeben.

## Neue Pakete vom 18.11.2026 – persönliche Abnahme

1. Vorheriges CAN schließen. Neues Setup installieren beziehungsweise vorhandene Installation aktualisieren. Version 1.0.3 Build 61, Verknüpfungen, Start und erhaltene Einstellungen prüfen. Installation/Deinstallation in der Windows-11-VM prüfen.
2. Neues Portable-ZIP vollständig in einen frischen Ordner entpacken. Start, Motorpfad und wesentliche Build-61-Funktionen kontrollieren.
3. Bei reproduzierbarer ausdrücklicher EAC3-Ablehnung Titel und Wortlaut kontrollieren. Nur „Mit MP4Box schneiden“ und „Abbrechen“; Abbrechen als Standard. Einmal abbrechen und prüfen, dass kein MP4Box-Lauf beginnt. Anschließend nur bei gewünschtem neuen Testschnitt ausdrücklich MP4Box bestätigen. Den bereits persönlich bestandenen MP4Box-Referenzschnitt nicht unnötig wiederholen oder überschreiben.
4. EAC3 wird weiterhin nicht vom otr-can-Motor unterstützt. Die allgemeine Motor-Empfehlung und der 40-ms-Rasterbefund sind unverändert. MP4Box ist kein allgemeines Audioreparaturverfahren.

Automatisierte Tests und technische Startprüfungen ersetzen diese persönliche Abnahme nicht. Die Qualitätsalternativsuche und der MP4Box-Wetterfee-Schnitt vom 18.11.2026 sind als bestehende persönliche Referenzen dokumentiert.

## Persönliche Rückmeldung vom 18.11.2026

**Windows-11-Setup-Test und sichtbarer neuer EAC3-Dialog persönlich bestanden.** Anwenderbestätigung und Screenshot sind im [Abnahmenachtrag](ABNAHME-BUILD61-20261118.md) eingeordnet. Weitere Paket-/Bedienprüfungen bleiben getrennt zu bestätigen. Die Paketdateien werden durch diesen Dokumentationsnachtrag nicht verändert.
