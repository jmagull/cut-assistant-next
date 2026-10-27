# CAN 1.0.2 Build 37 – Paketauftrag vom 26.10.2026

**Historischer Paketauftrag; spätere Paket- und Praxisnachweise sind im Nachtrag unten verlinkt.**

Der Nutzer beauftragt Setup, Portable-ZIP und passende Quellen für den geprüften AAC-Notfallweg. Neuer lokaler Paketsatz unter artifacts/release-1.0.2-build37; Build 36 bleibt erhalten. Keine Veröffentlichung, kein Tag oder Upload ohne ausdrückliche Freigabe.

Der Quellsnapshot enthält die aktuelle CAN-Weiterentwicklung einschließlich Rückfrage, strenger AAC-Prüfung, Paket-/Zeitstempelvergleich, kontrolliertem Wiederholungsversuch und sichtbarem Spurverlust. Keine neue Produktionsabhängigkeit. otr-can 0.1.6 und die libmpv-0.41.0-Laufzeit sowie deren passende Quellenarchive bleiben unverändert.

Quellbasis: Git-HEAD 1631aa7367a328bf86fd291a54a6c62f8731626f plus die im Snapshot dokumentierten aktuellen Arbeitsänderungen. Vier vorbestehende lokale Produktions-Lockänderungen werden erhalten und aus dem Snapshot ausgeschlossen; die gesperrten Abhängigkeiten werden aus dem Git-Stand übernommen. SOURCE-SNAPSHOT.json und SOURCE-FILES.sha256 dokumentieren den vollständigen tatsächlich gebauten Inhalt.

Entwicklungsprüfung: Release ohne Warnungen/Fehler, 1429/1429 Tests, vier native Normal-/Fehlerfälle, echter John-Wick-Teil-2-AAC-Schnitt technisch erfolgreich und vom Nutzer als passend bestätigt. Paket-Restore/Build/Tests/Publish, Installer-Kompilierung, endgültige Manifest-/ZIP-Prüfungen und Tests gegen die finalen Paketbibliotheken werden separat nachgewiesen.

Persönliche Installation, neuer Dialog, Portable-Pfade und gegebenenfalls VM-Deinstallation sind an Build 37 noch zu prüfen. Die positive Setup-Abnahme von Build 36 ersetzt diese nicht. Die Ursache früherer vollständiger Systemhänger bleibt ungeklärt; dafür wird keine Korrektur behauptet.

[Notfallweg und native Prüfung](OTR-CAN-AAC-NOTFALLWEG-20261026.md), [Release Notes](RELEASE-NOTES-1.0.2.md).

## Nachtrag zum Auftrag – 26.10.2026

Die Pakete sind technisch fertig und geprüft. Der automatische AAC-Notfallablauf wurde nach Bereitstellung persönlich erfolgreich durchlaufen. Separate neue Portable-/VM-/Deinstallationsfälle und persönliche Abbruch-/Fehlerfallprüfungen bleiben offen. Der ursprüngliche Auftrag und die eingefrorenen Dateien bleiben erhalten. [Paketbericht](RELEASE-1.0.2-BUILD37-RESULT.md), [aktueller Projektstand](AKTUELLER-STAND.md).
