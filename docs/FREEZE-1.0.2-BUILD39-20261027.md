# CAN 1.0.2 Build 39 – Paketumfang

**Späterer Abgleich 27.10.2026:** Die folgende Beschreibung erhält den damaligen Paketauftrag. Build 40 inzwischen technisch erstellt; Hinweis und Motor korrekt 0.1.6. Beide Klebecenter-Kombinationen positiv bestätigt, vollständige neue Setup-Endabnahme noch offen. [Projektstand](AKTUELLER-STAND.md), [Versionen](VERSIONEN.md).

Stand 27.10.2026. Der Nutzer bestätigt den eigenen John-Wick-Klebecenter-Lauf als „Alles super check“ und beauftragt Setup, ZIP und weitere Quellenpakete.

## Umfang

- Klebecenter im bisherigen Menüplatz „Tonspuren“; zwei fertige MP4-Filmteile, Reihenfolge zuerst, danach eine Tonspur je Teil im selben Dialog.
- Keine zusätzliche Tonspurkopie als Vorbereitung, keine erneute Cutlist-Anwendung.
- Unveränderte Bildpakete, samplegenauer Ton und einmaliges durchgehendes AAC-LC-Encoding; strenge Quell-/Ausgabeprüfung und vorhandener Kontrollplayer mit drei Sekunden Vorlauf.
- Enthält den bisherigen ausdrücklichen AAC-Notfallweg und die korrekten Schnittmotor-Cutlist-Metadaten.
- Unverändert otr-can 0.1.6, libmpv 0.41.0, .NET 10, WPF, keine neue Produktionsabhängigkeit.

## Herstellung und Grenzen

Neuer getrennter Paketsatz **artifacts/release-1.0.2-build39** mit Setup, Portable, exakten CAN-Quellen, passenden unveränderten Motor-/libmpv-Quellen, Relink-Paket und Prüfsummen. Herstellung ausschließlich aus festem Snapshot mit Basiscommit plus dokumentierten Arbeitsänderungen. Vier lokale Produktions-Lockänderungen erhalten und ausgeschlossen. Vorhandene Pakete bleiben erhalten; Buildzähler erst nach vollständiger erfolgreicher Paketprüfung von 38 auf 39 übernehmen.

Entwicklungsbasis: 1485 Tests, 21 native Klebecenter-Fälle, zehn bisherige native Notfall-/Tonspurkopiefälle und realer John-Wick-Nachweis. Die Paketprüfung wird getrennt dokumentiert. Persönlicher neuer Setup-/Portable-/VM-Test bleibt offen.

**Keine Veröffentlichung, kein Tag, kein Upload und kein Commit/Push aus diesem Auftrag.** Die Ursache früherer Furiosa-Rechnerstillstände bleibt ungeklärt. AVI-/MP4Box-Grenzen bestehen fort.

[Funktionsbeschreibung und persönliche Rückmeldung](KLEBECENTER-20261027.md), [Paketbericht](RELEASE-1.0.2-BUILD39-RESULT.md).
