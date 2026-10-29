# CAN 1.0.2 Build 43 – Quell- und Paketumfang

Stand 28.10.2026. Nutzer beauftragt Setup, Portable und weitere Quellenpakete. Öffentliche Version bleibt Build 41; keine neue Veröffentlichungs-, Commit- oder Pushfreigabe.

Basiscommit **d200ebf7567bceeab012ee9a2bb6b564635382a4** plus aktuelle dokumentierte Arbeitsänderungen einschließlich Build-42-Phasenzeiten/RF64 und Build-43-QUICK-Mehrspurverarbeitung, bestätigter Zuordnung, Tonspurwahl im Kontrollfenster und unverändertem AUDIO-ENTHUSIAST. Aktuelle Dokumentation und neue Quellen werden vollständig überlagert. Vier vorbestehende Produktions-Lockänderungen bleiben im Arbeitsrepository unverändert und werden aus dem Quellsnapshot ausgeschlossen; dieser verwendet die committed Lockdateien. Keine neue Produktionsabhängigkeit.

Exakte Eingaben stehen im erzeugten SOURCE-SNAPSHOT.json und SOURCE-FILES.sha256. Paketierung ausschließlich aus diesem Snapshot mit festem CanBuildNumber=43, ohne Erhöhung des lokalen Zählers. Gemeinsame EXE-Dateiversion 1.0.2.43, Motor otr-can 0.1.6 und seine unveränderten passenden Quellen; libmpv 0.41.0 mit den vier per SHA-256 geprüften nativen DLLs sowie Quellen-/Relinkpaketen. Keine Videodateien, Buildausgaben oder privaten Einstellungen im Quellen-ZIP.

Geplante Ausgaben unter artifacts/release-1.0.2-build43: Setup, Portable, CAN-Quellen, Motorquellen, libmpv-Prüfquellen/Relink und Prüfsummen. Bisherige Pakete erhalten. Paketbuild und -tests werden separat im [Paketbericht](RELEASE-1.0.2-BUILD43-RESULT.md) nachgewiesen. Entwicklungsstand: 1552 Tests, 28 QUICK- und 21 bisherige native Fälle positiv; persönlicher Furiosa-QUICK-Check bestätigt. Persönliche Abnahme der neuen Installationspakete steht aus.
