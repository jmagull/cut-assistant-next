# Cut Assistant Next Build 61 – Fortlaufender Schnittshuffle

Lokale Windows-Prüffassung aus `codex/continuous-schnittshuffle`, aufbauend auf der persönlichen Windows-11-Abnahme von Wiederholtempo und Kontrollplayer-Fokus in Commit `d81fab9`.

## Bedienung

- Rechts geht vom Anfang zum Ende eines markierten Bereichs und danach zum Anfang des nächsten. Links läuft dieselbe Folge rückwärts. Am ersten und letzten Eintrag endet die Navigation ohne Umlauf.
- Tabelle und Timeline folgen über die bestehende Schnittauswahl. Die ausgewählte Kante bleibt der Ausgangspunkt für den nächsten Shuffle-Sprung, auch nach Einzelbildschritten an der Kante.
- Ohne ausgewählte Kante startet die Navigation beim ausgewählten Bereich. Innerhalb dieses Bereichs geht es zur Kante in der gewählten Richtung. Ohne Bereichsauswahl gilt die aktuelle Playerposition für die Suche in der gewählten Richtung.
- Treffen das Ende eines Bereichs und der Anfang des Folgebereichs auf denselben Zeitpunkt, bleiben beide Stationen erhalten. Vorwärts kommt zuerst das Ende, rückwärts zuerst der Anfang.
- Zur Einzelbildkontrolle pausiert der Shuffle-Sprung eine laufende Wiedergabe. Am exklusiven Dateiende wird das letzte sichtbare Bild angefahren. Die gespeicherte Schnittgrenze bleibt dabei am Dateiende.
- Bei leerer Liste oder einem noch offenen Schnittanfang bleibt der Shuffle ausgeblendet. Der Tab-Zugriff auf die beiden Pfeile bleibt möglich. Beim Mausklick hält CAN einen geeigneten vorhandenen Hauptfensterfokus fest. Aus einem Texteingabefeld kann der Fokus auf den Pfeil wechseln.

## Persönlicher Test unter Windows 11

1. Die lokale Prüffassung starten und eine vorhandene Cutlist mit mindestens drei markierten Bereichen laden. Rechts von Anfang zu Ende und zum nächsten Bereich klicken. Links die Strecke zurückgehen. An beiden äußeren Grenzen noch einmal klicken. Schnittliste, Timeline und Zeiten vergleichen.
2. Maus auf dem rechten Pfeil lassen. Klicken, mit den Einzelbildtasten mehrere Bilder über die Kante hinweg prüfen, erneut klicken. Der nächste Shuffle-Sprung folgt der Schnittreihenfolge. Diese Folge für links wiederholen. Aus laufender Wiedergabe und mit Fokus auf einem anderen Bedienelement beginnen.
3. Dieselben Schritte in Standard, Power-User 1 und Power-User 2 sowie im Rot- und Grün-Modus prüfen. Für den Moduswechsel den Film neu öffnen. Wenn möglich angrenzende Bereiche und einen bis Dateiende reichenden Bereich einbeziehen.
4. Die bereits persönlich abgenommenen Funktionen stichprobenartig prüfen. Wiederholtempo 550/120 ms und Rückkehr zum Hauptfenster beim Schließen des Kontrollplayers. Cutlist vor und nach der Kontrolle vergleichen.

## Technischer Nachweis

Release-Build ohne Warnungen oder Fehler. **2.153 Tests bestanden**, 0 fehlgeschlagen, 0 übersprungen. Core 200, Media 402, App 1.471, Cutlists 80. Die 26 gezielten Schnittshuffle-Tests umfassen Vor-/Rücklauf, angrenzende Grenzen, Endpunkte, beide Schnittmodi, alle drei Layouts und geroutete WPF-Tastenereignisse zwischen Klick-Ereignissen. Reale Maus- und Tastatureingaben im Film bleiben Teil der persönlichen Prüfung. Timing und Kontrollplayer-Fokus stammen aus dem persönlich abgenommenen Entwicklungszweig. Ihre automatisierten Regressionen liefen in der Gesamtsuite erneut.

Die `CutAssistantNext.App.exe` im Ordner der lokalen Prüffassung direkt starten. Die Fassung verwendet .NET Desktop Runtime 10 x64 und die vorhandenen CAN-Benutzereinstellungen.
