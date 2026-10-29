# CAN 1.0.2 Build 44 – Quell- und Paketumfang

Stand 29.10.2026. Menüvereinfachung vom Nutzer mit „tip top gecheckt“ persönlich bestätigt; anschließend Setup, ZIP, Quellen und weitere passende Pakete ausdrücklich beauftragt. Keine neue Veröffentlichungs-, Commit- oder Pushfreigabe. Öffentlich bleibt Build 41.

Basiscommit d200ebf7567bceeab012ee9a2bb6b564635382a4 plus dokumentierte Arbeitsänderungen. Gegenüber Build 43 ausschließlich den zusätzlichen Tonspurkopie-Eintrag aus MainWindow.xaml entfernt; Tonspurwahl direkt beim Zusammenfügen, beide Medienverfahren und eigenständiger Kopierdienst unverändert. Spätere Werkzeugrubrik mit Tonspurkopie, Martins Werbebildervorschau und Untertitelanzeige nur vorgemerkt. Keine neue Produktionsabhängigkeit.

Quellsnapshot mit SOURCE-SNAPSHOT.json und SOURCE-FILES.sha256. Vier vorbestehende Produktions-Lockänderungen bleiben im Arbeitsrepository unverändert und werden aus dem Snapshot ausgeschlossen; dieser verwendet committed Locks. Paketierung aus dem manifestierten Snapshot mit CanBuildNumber=44 ohne weitere Erhöhung. Dateiversion 1.0.2.44, Motor otr-can 0.1.6 und passende Quellen, libmpv 0.41.0 mit vier geprüften nativen DLLs und Quellen-/Relinkpaketen. Keine Filmdateien, privaten Einstellungen oder Buildausgaben im Quellenarchiv.

Neuer Ausgabeordner artifacts/release-1.0.2-build44: Setup, Portable, CAN-Quellen, Motorquellen, libmpv-Prüfquellen/Relink sowie Prüfsummen. Frühere Pakete unverändert. Neue Setup-/Portable-/VM-Abnahme bleibt getrennt offen. [Paketbericht](RELEASE-1.0.2-BUILD44-RESULT.md), [Bedienung](NUTZERANLEITUNG.md).
