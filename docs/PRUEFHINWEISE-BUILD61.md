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

Die persönliche Sicht- und Hörkontrolle ist noch offen.
