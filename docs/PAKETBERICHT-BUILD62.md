# Cut Assistant Next 1.0.3 Build 62 – Paketbericht

Technische Qualitätssicherung und persönliche Gesamtabnahme waren am 21.11.2026 abgeschlossen. Die Dateiversion der freigegebenen Pakete lautet **1.0.3.62**.

| Prüfung | Ergebnis |
| --- | --- |
| Automatisierte Tests auf einem regulären Windows-11-Rechner | 2.365/2.365 bestanden, 0 fehlgeschlagen, 0 übersprungen |
| Originaler nativer QA-Harness | 9/9 bestanden |
| Gesonderte native Audioregressionen | 20/20 bestanden |
| Dokumentierter Release-Build | 0 Fehler, 0 Warnungen |
| Produktbibliotheken | Sieben SHA-256-Vergleiche mit den getesteten Ständen bestanden |
| Portable-Dateien | 268 gegen Paketmanifest geprüft |
| Quellarchiv | 948 Einträge gegen das Quellmanifest geprüft |
| Spotlight-CAN im Quellarchiv | 42 Dateien stimmen mit dem verknüpften Git-Commit `75ee1475adf20a8c8231ab2989eab7eff37e6b09` überein |

Die früheren AppContainer-Fehlversuche betrafen vier ACL-Tests, drei Prozess-Tests und eine native ACL-Prüfung. Diese acht Fälle bestanden in den regulären externen Wiederholungen. Die historischen Ergebnisse bleiben in der internen Abschlussdokumentation erhalten.

Der Anwender hat auf dem Windows-11-Arbeitsrechner und in einer Windows-11-VM die Installation, das frühere Upgrade, die Deinstallation und die Portable-Fassung bestätigt. Die persönlichen Prüfungen umfassten die neuen Tastenkürzel, die TEMP-Einstellungen und Anzeige, reale Schnitte mit MP4Box und otr-can sowie Abbruchdiagnose, MP4Box-Fallback und anschließend Bild, Ton, Synchronität, Schnittübergänge und TEMP-Bereinigung. Ein Transfer zwischen zwei verschiedenen physischen Laufwerken war nicht Teil der persönlichen Praxisprüfung.

Setup, Portable-ZIP und Quellarchiv entsprechen bytegenau den zur persönlichen Abnahme bereitgestellten Dateien. `SHA256SUMS.txt` dokumentiert die SHA-256-Werte der Release-Dateien. `PACKAGE-MANIFEST.json`, `PUBLISH-FILES.sha256` und `SOURCE-FILES.sha256` beschreiben den geprüften Paketinhalt.

Der CAN-Git-Stand enthält die 901 CAN-Quelldateien aus dem geprüften Archiv unter den bestehenden Git-Zeilenendenregeln. Das Spotlight-Submodul bleibt auf dem oben genannten Commit. Das Quellarchiv enthält zusätzlich die eigene Exportdokumentation und das unveränderte otr-can-Quellarchiv. Für die untersuchte Dateiversion 1.0.3.62 wurde der dokumentierte Build-Parameter `-p:CanBuildNumber=62` verwendet.

## Begleitende Quellen

Die unveränderten Begleitarchive sind über die bisherige Build-61-Veröffentlichung bereits öffentlich verfügbar:

- [libmpv 0.41.0 Quellen und Prüfpaket](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build61/CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip)
- [libmpv 0.41.0 Relink-Unterlagen](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build61/CAN-libmpv-v0.41.0-relink-2026-09-26.zip)
- [otr-can 0.1.6-audio-diag.1 Quellarchiv](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build61/otr-can-0.1.6-audio-diag.1-source.zip)
