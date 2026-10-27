# Furiosa – beide otr-can-Schnitte und Klebecenter persönlich bestätigt

**27.10.2026.** Rückmeldung im laufenden Arbeits-PC-Test nach dem angekündigten Wechsel zu **Setup CAN 1.0.2 Build 40**. Aktueller Motor **otr-can 0.1.6**. Die Zuordnung zur Build-40-Prüfrunde stammt aus der Gesprächsfolge; das Klebeprotokoll enthält keine eigene CAN-Buildzeile.

## Persönliche Rückmeldung

Der Nutzer bestätigt **„schnitt teil 1 mit otr-can check, schnitt teil 2 mit otr-can check, klebecenter: check“** und nach dem vollständigen Erfolgsprotokoll **„Allte stellen geprüft check“**.

Damit sind beide eigenen Schnitte und das anschließende Verbinden samt Kontrolle aller Prüfstellen positiv gemeldet. Der Nutzer bestätigt anschließend zusätzlich ausdrücklich **„und kein absturz“**: Beide otr-can-Schnitte und das anschließende Kleben wurden in dieser Prüfrunde ohne Absturz abgeschlossen. Diese positive Stabilitätsrückmeldung gilt für den gemeldeten Lauf; die Ursache früherer Rechnerstillstände bleibt ungeklärt. Das beigefügte vollständige Protokoll betrifft das Klebecenter; für die beiden Schnitte liegen in dieser Rückmeldung die positiven Nutzerbestätigungen vor.

## Gewählte Filmteile und Tonspuren

| Teil | Bereits geschnittene Datei | Gewählte Spur |
|---|---|---|
| 1 | Furiosa A Mad Max Saga Teil 1.mp4 | Spur 1, AAC, Deutsch, Stereo, 48000 Hz |
| 2 | Furiosa A Mad Max Saga Teil 2.mp4 | Spur 1, AAC, Deutsch, Stereo, 48000 Hz |

Die ausgewählte AAC-Spur wird je Teil verwendet; andere vorhandene Spuren entfallen nur im gemeinsamen Ergebnis. Der Ton wird einmal durchgehend nach AAC-LC mit 192 kbit/s kodiert. Das Video bleibt ohne Neukodierung.

## Erfolgreiche technische Klebeprüfung

Das Protokoll bestätigt:

- Passende Bildformate und Prüfung des Bildrasters/sämtlicher Bildpakete je Teil.
- Prüfung der Quelltonzeitstempel auf Lücken und Überlappungen.
- Vollständige strenge Dekodierung beider gewählten AAC-Spuren.
- Samplegenaue PCM-Verbindung und Vergleich gegen beide Ausgangsspuren.
- Einmaliges durchgehendes AAC-Encoding und abschließenden MP4-Zusammenbau.
- Nachprüfung sämtlicher Bildpakete/Zeiten, AAC-Pakete und Wiedergabezeiten.
- Vollständige Dekodierung des fertigen Tons mit exakter Samplezahl.
- Dekodierung von sechs Sekunden um die Klebestelle, drei Sekunden davor beginnend.
- Abschluss **„Zusammenfügen und technische Prüfung abgeschlossen.“**

| Ergebnis | Protokollierter Wert |
|---|---|
| Dekodierte Ausgabetonsamples | **387448320** |
| Abtastrate | **48000 Hz** |
| Gesamtdauer | **02:14:31.840** |
| Tatsächliche Klebestelle | **01:54:18.320** |
| Drei-Sekunden-Vorlauf | **01:54:15.320** |

Die Konsistenz ist nachvollziehbar: 387448320 / 48000 = 8071,84 Sekunden = 02:14:31.840. Es wird keine im vorliegenden Log nicht genannte Videopaketanzahl ergänzt.

## Einordnung

Zusätzlich zu John Wick bestätigt damit eine weitere große Aufnahme den vollständigen Praxisablauf **otr-can Teil 1 → otr-can Teil 2 → Klebecenter → persönliche Kontrolle** in der Build-40-Setuprunde auf dem Arbeits-PC. Neue Portable-/VM-/Deinstallationsvarianten bleiben gesondert zu prüfen. Das ist keine allgemeine Formatfreigabe und kein Ursachennachweis für die früheren Rechnerstillstände; deren Ursache bleibt ungeklärt.

Nur Praxisdokumentation ergänzt. Kein neuer Medienlauf durch den Agenten, kein Codebuild/Testlauf, keine Paketänderung, kein Zählerwechsel oder Release. Kleine lokale Belege im ignorierten **.build/furiosa-klebecenter-20261027/**: USER-EVIDENCE.json und bytegleiche Kopie des erhaltenen Klebeprotokolls. Originalfilme und das Nutzerprotokoll bleiben unverändert.

[Projektstand](AKTUELLER-STAND.md), [Klebecenter](KLEBECENTER-20261027.md), [Build-40-Paketbericht](RELEASE-1.0.2-BUILD40-RESULT.md), [früherer Furiosa-Befund](OTR-CAN-MAD-MAX-SYSTEMFREEZE-20261020.md).
