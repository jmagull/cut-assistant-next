# Cut Assistant Next – Wiederholtempo und Fokus des Kontrollplayers

Getrennter Entwicklungsstand auf `codex/navigation-review-focus` auf Basis von `88ffbe4`, der korrigierten laufenden Build-61-Prüffassung.

## Parametrierbare Wiederholungsgeschwindigkeit

Unter **Einstellungen → Darstellung und Schnittbearbeitung → Wiederholung beim Halten** stehen zwei Werte in Millisekunden.

| Einstellung | Standard | Gültige Werte |
| --- | ---: | ---: |
| Bis zur ersten Wiederholung | 350 ms | 150–1.500 ms |
| Abstand weiterer Wiederholungen | 200 ms | 100–1.000 ms |

Sie gelten für Maus und Tastatur bei Sekunden-, Keyframe- und Einzelbildsprüngen. Die Einstellungen werden mit den Darstellungseinstellungen gespeichert, beim Programmstart geladen und nach dem Übernehmen sofort angewendet. Ältere Einstellungsdateien verwenden die bisherigen Standardwerte. Außerhalb der Grenzen liegende Zahlen in bestehenden Dateien werden beim Lesen begrenzt. Die gemeinsamen Sprungaufträge laufen weiterhin einzeln. Loslassen, Fokusverlust, deaktivierter Player und der Beginn des Schließvorgangs stoppen die Wiederholung. Ein bereits an den Player übergebener Sprung darf auslaufen.

Automatisierte Regressionen prüfen die eigenen Zeiten für alle sechs Sprungaktionen, Standardzeiten, Weiterbetrieb der Taktung, Abbruch, ausbleibende Warteschlangen bei langsamem Seek, alte Einstellungsdateien, Persistenz und Wertebereiche.

**Manuelle Abnahme**

1. Auf der Entwicklungsfassung einen Film öffnen. Beim kurzen Klicken oder Drücken jeweils einen einzelnen Sprung auslösen. Alle drei Navigationstypen vorwärts und rückwärts per Maus und Tastatur halten und loslassen.
2. Im Dialog z. B. 550/120 ms übernehmen. Längeren Start und schnellere Folge prüfen. Die Anwendung neu öffnen und gespeicherte Werte sowie die persönliche Tastenbelegung prüfen. Einen ungültigen Wert eingeben und die Meldung prüfen.
3. Während einer Wiederholung den Fokus in ein anderes Fenster wechseln oder CAN schließen. Prüfen, dass keine weitere Folge von Sprüngen startet. Bei einem trägen Seek während der Freigabe erneut drücken und auf ausbleibende nachträgliche Sprünge achten.

Die Übungsfassungen verwenden dasselbe Windows-Benutzerprofil. Wer im ursprünglichen Build 61 danach die Darstellungseinstellungen erneut speichert, kann die zusätzlichen Timing-Felder verlieren, da der ältere Dialog sie nicht kennt.

## Fensterfokus nach Schließen des Kontrollplayers

Der Kontrollplayer gehört weiterhin dem CAN-Hauptfenster. Das Schließen wartet auf die Freigabe des Players. WPF prüft die Aktivierung bei Beginn und unmittelbar vor dem tatsächlichen Schließen. Beim aktiven regulären Schließen aktiviert der Controller das sichtbare, bedienbare Hauptfenster bei Bedarf mit WPFs Window.Activate. War zwischenzeitlich ein anderes Fenster aktiv und ist der Kontrollplayer beim tatsächlichen Schließen im Hintergrund, unterbleibt die explizite Aktivierung. Dasselbe gilt beim Anwendungsende und bei minimiertem Hauptfenster. Ein modales Abschlussfenster kann die Bedienung weiterhin übernehmen. Die Logik greift weder auf den Schnittplan noch auf das Hauptfenster-Medium zu.

Die Mehrfenster-Regression verwendet tatsächliche WPF-Fenster und eine kontrolliert verzögerte Player-Freigabe. Sie prüft das reguläre aktive Schließen mit Tastaturfokus im Hauptfenster, den Wechsel zu einem anderen Fenster während der Freigabe, das Schließen im Hintergrund, den programmgesteuerten Stopp und das erneute Öffnen. Der ursprüngliche sporadische Fehler wurde in der Testumgebung nicht zuverlässig als eigener Ausgangsfall reproduziert. Die Testfenster prüfen den Aktivierungsablauf, ein Wechsel zu einer tatsächlich anderen Windows-Anwendung gehört zur persönlichen Abnahme.

**Manuelle Mehrfenster-Abnahme**

1. CAN, ein anderes Programm und bei Bedarf ein zweites CAN-Fenster öffnen. Einen fertigen Schnitt im Kontrollplayer öffnen.
2. Den aktiven Kontrollplayer jeweils per Schließen-Button, Esc und Fensterschaltfläche schließen. Bei sichtbarem Hauptfenster sofort eine normale CAN-Taste bedienen. Schnittauswahl, Wiedergabeposition und Media-Datei vergleichen.
3. Den Kontrollplayer erneut öffnen, die Freigabe beim Schließen mit einem schnellen Wechsel zu einer anderen Anwendung überlappen lassen. Das andere Programm soll aktiv bleiben. Zusätzlich ein geöffnetes modales CAN-Abschlussfenster und ein minimiertes Hauptfenster ausprobieren.
4. Dieselbe Schnittprüfung anschließend erneut öffnen. Prüfposition und Tonspur prüfen. Die Schritte unter Windows 11 und bei vorhandener Gelegenheit auch unter Windows 10 wiederholen.

## Persönliche Abnahme am 16.11.2026

- **Wiederholtempo unter Windows 11 bestanden.** 550 ms Startverzögerung und 120 ms Wiederholabstand wurden gespeichert und blieben nach Neustart erhalten. Gedrückte Maus- und Tastaturtasten wiederholen wie gewünscht. Loslassen stoppt die Wiederholung.
- **Fensteraktivierung unter Windows 11 bestanden.** Mit mehreren geöffneten Fenstern erhielt CAN nach dem Schließen des Kontrollplayers zuverlässig den Fokus zurück.

Die persönliche Funktionsabnahme umfasst beide Arbeitspakete.

## Gemeinsames Prüfergebnis

Release-Build mit 0 Warnungen und 0 Fehlern. Vollständige Suite mit **2.150 bestandenen Tests**, 0 fehlgeschlagen und 0 übersprungen. Core 200, Media 402, App 1.468, Cutlists 80. Beide Arbeitspakete sind durch getrennte lokale Commits unabhängig prüf- und übernehmbar. Die Schnittshuffle-Erweiterung steht als späterer Punkt in der [Setup-Merkliste](SETUP-MERKLISTE.md#offen--schnittshuffle-über-alle-schnittgrenzen).
