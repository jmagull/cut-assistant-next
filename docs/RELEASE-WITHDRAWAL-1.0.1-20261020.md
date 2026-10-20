# Vorübergehende Rücknahme – CAN 1.0.1 · Build 34

Am **20.10.2026** ausdrücklich vom Nutzer beauftragt und durchgeführt:

1. **CAN 1.0.1 · Build 34 aus der öffentlichen Veröffentlichung genommen.** Der vorhandene GitHub-Release ist jetzt ein Entwurf. Seine sieben Dateien bleiben unter denselben Asset-IDs mit identischen Größen und SHA-256-Digests erhalten.
2. **CAN 1.0.0 · Build 32 wieder als aktuelle stabile Latest-Version eingestellt.** Kein Prerelease, kein Entwurf; Setup, Portable, Quellen und Prüfsummen unverändert verfügbar.

[Aktuelle stabile Downloads: CAN 1.0.0 · Build 32](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.0).

## Prüfung

- Zurückgenommener Release **v1.0.1**, ID **418710813**: `draft=true`; Zugriff auf die Release-Tag-API ohne Anmeldung liefert **HTTP 404**.
- Öffentlicher Latest-Endpunkt ohne Anmeldung bestätigt **v1.0.0**, Release-ID **416326970**.
- Sämtliche bestehenden Release-Titel, Texte, Quellziele, Prerelease-Flags und Asset-IDs/-Namen/-Größen/-Digests erhalten. Lediglich der Veröffentlichungsstatus von 1.0.1 sowie die Latest-Zuordnung wurden umgestellt.
- Keine Release-Datei gelöscht, ersetzt oder neu gebaut. Keine Tags oder Quellcommits gelöscht oder verschoben. Lokale Build-34-Pakete und positive Testnachweise bleiben erhalten.
- README und aktuelle Statushinweise zeigen wieder auf 1.0.0. Der Quellstand und lokale Buildzähler werden dadurch nicht auf eine ältere Entwicklungsversion zurückgesetzt.

Ein technischer Rücknahmegrund wurde vom Nutzer in diesem Auftrag nicht genannt; es wird keiner ergänzt. Eine erneute Veröffentlichung von 1.0.1 erfolgt erst auf neuen ausdrücklichen Auftrag.

Lokale Nachweise: `.build/withdraw-1.0.1-20261020` mit ursprünglicher Release-/Latest-Liste, Änderungsanfragen, neuer Release-/Latest-Liste und `VERIFIED-WITHDRAWAL.json`. Die Dokumentationskorrektur wird separat committet und gepusht; Programme und Paketbytes bleiben unverändert.
