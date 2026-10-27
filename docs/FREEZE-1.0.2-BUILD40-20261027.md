# CAN 1.0.2 Build 40 – korrigierter Voraussetzungstext

**Späterer Abgleich 27.10.2026:** Die folgende Beschreibung erhält den damaligen Paketauftrag. Build 40 inzwischen technisch erstellt; Hinweis und Motor korrekt 0.1.6. Beide Klebecenter-Kombinationen positiv bestätigt, vollständige neue Setup-Endabnahme noch offen. [Projektstand](AKTUELLER-STAND.md), [Versionen](VERSIONEN.md).

Stand 27.10.2026. Build 39 enthält tatsächlich otr-can 0.1.6, zeigt aber im Voraussetzungstext noch 0.1.5. Der Nutzer hat die veraltete Angabe beim Setup entdeckt.

Geändert werden nur der Motorversionshinweis in installer/HINWEIS-DOTNET-UND-WERKZEUGE.txt und der Versionsabgleich in tools/package-release.ps1 sowie die Dokumentation. Klebecenter, Schnittmotor und sonstiger Anwendungscode bleiben gleich. Korrekte Motor-EXE unverändert 0.1.6.

Neue Pakete als Build 40 getrennt von Build 39; alte Pakete und vier lokale Produktions-Lockänderungen erhalten. Gesperrter Restore, Build, Tests, Quellen-/Portable-Prüfung und Vergleich des tatsächlich ausgelieferten Hinweistexts mit Motorversion erforderlich. Buildzähler erst nach Erfolg von 39 auf 40 übernehmen. Kein automatisches Installieren, kein Commit/Push und keine Veröffentlichung.

Die persönlichen Klebecenter-Erfolge und nativen Laufzeitprüfungen aus Build 39 gelten mit ihrem dokumentierten Umfang; neue Paket-/Installationsabnahme bleibt offen. [Paketbericht](RELEASE-1.0.2-BUILD40-RESULT.md).
