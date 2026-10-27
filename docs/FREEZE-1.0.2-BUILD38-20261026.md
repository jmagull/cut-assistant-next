# CAN 1.0.2 Build 38 – manuelle Tonspurauswahl

**Historischer Paketauftrag; spätere Paket- und Praxisnachweise sind im Nachtrag unten verlinkt.**

Stand 26.10.2026. Fortführung des lokalen Setup-/ZIP-/Quellenauftrags nach Einbau des ausdrücklich beauftragten manuellen Schalters für weitere Filmteile. Vorheriger Build 37 bleibt erhalten; keine Veröffentlichung.

Neuer Menüpunkt **Tonspuren → Tonspur für Filmteile auswählen …**, zusätzliche MP4-Kopie ohne Neukodierung, explizite AAC-/AC-3-Auswahl und Anzeige entfallender Spuren. Vollständige strenge Dekodierprüfung und exakter Paket-/Wiedergabezeitvergleich. Die Schnittliste wird nicht angewendet und bestehende Dateien werden nicht ersetzt.

1446 Entwicklungstests und zehn native Fälle bestanden; vollständiger John-Wick-Teil-1-AAC-Test erfolgreich. Quellsnapshot: Basiscommit 1631aa7367a328bf86fd291a54a6c62f8731626f plus dokumentierte aktuelle Arbeitsänderungen. Vier vorbestehende lokale Produktions-Lockänderungen bleiben erhalten und werden ausgeschlossen. Keine neue Produktionsabhängigkeit; Motor 0.1.6 und libmpv 0.41.0 unverändert.

Paketprüfung und persönliche Setup-/Portable-/Dialogabnahme werden separat nachgewiesen. Bekannte Systemhänger sind nicht als behoben eingestuft. Kein Tag, Upload oder Release ohne ausdrückliche Freigabe.

[Umsetzung und Grenzen](TONSPURAUSWAHL-FILMTEILE-20261026.md).

## Nachtrag zum Auftrag – 26.10.2026

Die Pakete sind technisch fertig und geprüft. Der manuelle Tonspurkopierablauf wurde nach Bereitstellung persönlich erfolgreich durchlaufen. Separate neue Portable-/VM-/Deinstallationsfälle und persönliche Abbruch-/Fehlerfallprüfungen bleiben offen. Der ursprüngliche Auftrag und die eingefrorenen Dateien bleiben erhalten. [Paketbericht](RELEASE-1.0.2-BUILD38-RESULT.md), [aktueller Projektstand](AKTUELLER-STAND.md).
