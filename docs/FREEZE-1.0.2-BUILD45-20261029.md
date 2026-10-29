# CAN 1.0.2 Build 45 – eingefrorener Quell- und Paketstand

**29.10.2026.** Nutzer bestätigt den neuen Tonspurhinweis mit Screenshot und „ich glaube das passt so“, danach ausdrücklich **„setup, zip, src bauen, stand jetzt so einfrieren“**. Keine neue Commit-, Push- oder Veröffentlichungsfreigabe. Öffentlich bleibt Build 41.

Eingefrorene Produktfassung **CAN 1.0.2.45**: Klebecenter-Menü aus Build 44 und Tonspurhinweis mit Textumbruch aus Build 45. Beide Verfahren, automatische Paarvorschläge, Bestätigung, Doppelbelegungsschutz und eigenständiger Kopierdienst unverändert. Motor **otr-can 0.1.6**, Wiedergabe **libmpv 0.41.0**, .NET 10/WPF / Windows 11 x64. Keine neue Produktionsabhängigkeit oder weitere Produktänderung bei der Paketierung.

Quellbasis **d200ebf7567bceeab012ee9a2bb6b564635382a4** plus manifestierte Arbeitsänderungen. SOURCE-SNAPSHOT.json und SOURCE-FILES.sha256 legen den exakten Stand fest; alle Produkt-/Testquellen werden zusätzlich gegenüber Auftragseingang verglichen. Vier vorbestehende lokale Produktions-Lockänderungen bleiben unverändert erhalten und aus dem Quellsnapshot ausgeschlossen; dieser verwendet committed Locks. Keine Filmdateien, lokalen Einstellungen oder Buildausgaben im Quellenarchiv.

Paketierung ausschließlich aus dem manifestierten Snapshot mit **CanBuildNumber=45**, ohne Zählererhöhung. Neuer eigener Ordner **artifacts/release-1.0.2-build45**: Setup, Portable-ZIP, CAN-Quellen, passende unveränderte Motorquellen, libmpv-Prüfquellen/Relink und Prüfsummen. Frühere Pakete erhalten. Nach erfolgreicher Prüfung acht oberste Auslieferungsdateien und manifestierte Snapshot-Dateien schreibschützen; Quell-/Binärarchive unverändert lassen.

Entwicklungsprüfung 1552 Tests und vier native WPF-Fälle positiv. Persönlich bestätigt sind Hinweis und zuvor zehn Build-44-Setup-Funktionsfälle; neue Build-45-Setup-/Portable-/VM-Abnahme bleibt getrennt offen. [Paketbericht](RELEASE-1.0.2-BUILD45-RESULT.md), [Hinweis und Praxisstand](KLEBECENTER-TONSPURHINWEIS-20261029.md).
