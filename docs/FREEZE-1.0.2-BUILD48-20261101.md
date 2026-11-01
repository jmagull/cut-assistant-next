# CAN 1.0.2 Build 48 – freigegebener Releaseumfang

01.11.2026. Der Nutzer hat Commit, Push auf main, Paketierung, Tag **v1.0.2-build48** und stabile Veröffentlichung als GitHub-Latest ausdrücklich freigegeben. Erst nach vollständigem Upload und Verifikation freischalten. Build 47, dessen Tag und eingefrorene Artefakte bleiben unverändert.

## Umfang und Abnahme

- Kontrollplayer-Absturz nach natürlichem EOF, Tonspurfehler und anschließender Navigation behoben.
- Regulärer Tonspurwechsel ohne Positionssprung und mit Erhalt von Play/Pause; bewusste Navigation mit drei Sekunden Vorlauf unverändert.
- Native Property-Fehler am wartenden Task auffangbar; Property-Zugriffe und Dispose koordiniert.
- Keine funktionale Änderung an MP4Box, otr-can, Klebecenter, Cutlists oder Namensmasken; keine neue Produktionsabhängigkeit.
- 1634 Entwicklungstests und 32 native/WPF-Kontrollen in sechs Prozessen erfolgreich. Persönliche korrigierte Ghosts-Prüfung für Teil 1 und Teil 2 durch den Nutzer bestätigt, einschließlich EOF-Absturzfall, Tonspurwechsel und Navigation.

## Versions- und Quellzuordnung

CAN **1.0.2**, Build **48**, Dateiversion **1.0.2.48**, unverändert otr-can **0.1.6** und libmpv **0.41.0**, .NET 10/WPF, Windows 11 x64. Der vorhandene Buildzähler wird genau einmal mit `tools/build.ps1` von 47 auf 48 erhöht; nachfolgende Prüf-/Paketbauten verwenden explizit `CanBuildNumber=48`.

Der Release-Commit enthält exakt den freigegebenen Produkt-/Teststand, das native Testwerkzeug und die aktualisierte Dokumentation. Aus diesem Commit entsteht der manifestierte Quellsnapshot ohne Arbeitsbaum-Overlay. `SOURCE-SNAPSHOT.json` nennt den vollständigen Commit-SHA, `SOURCE-FILES.sha256` alle Quellbytes. Der Release-Tag muss auf diesen tatsächlich gebauten Commit zeigen. Reine Ergebnis-/Veröffentlichungsnachträge auf main verschieben den Tag nicht.

Vier vorbestehende lokale Produktions-Lockänderungen entfernen lediglich win-x64-Einträge; keine neue Paketversion oder Produktabhängigkeit. Sie werden erhalten und aus Commit sowie Quellarchiv ausgeschlossen. Im Paket gelten die committed Locks und ein gesperrter win-x64-Restore.

## Paketumfang und Freigabebedingung

Bewährter Build-47-Ablauf, separater Ausgabeordner **artifacts/release-1.0.2-build48**: Windows-x64-Setup, Portable-ZIP, CAN-Quellarchiv, otr-can-0.1.6-Quellen, bisherige zwei libmpv-Quellen-/Relink-Pakete sowie `SHA256SUMS.txt` und `PUBLISH-FILES.sha256` – insgesamt acht Release-Assets. Werkzeuge und Laufzeitvoraussetzungen unverändert.

Vor Freischaltung: vollständige Pakettests, zusätzliche Tests gegen finale Produktbibliotheken, 32 native/WPF-Kontrollen gegen finale Paketinhalte, vollständiger Quell-/ZIP-/Publish-/Setup-/Versions-/Prüfsummenvergleich. Keine Veröffentlichung bei fehlerhafter Prüfung oder abweichender Buildnummer. Die persönliche Ghosts-Abnahme der Entwicklungsfassung ersetzt keine behauptete neue Setup-/VM-Abnahme.

[Analyse](KONTROLLPLAYER-TONSPUR-ABSTURZ-ANALYSE-20261101.md), [Korrektur und Entwicklungstests](KONTROLLPLAYER-TONSPUR-KORREKTUR-20261101.md), [Release Notes](RELEASE-NOTES-1.0.2.md), [unveränderter Vorgänger Build 47](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md).
