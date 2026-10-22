# Freeze für CAN 1.0.2 · Build 35

Am **22.10.2026** beauftragt der Nutzer ausdrücklich: **„einfrieren, setup, zip etc überführen, commit und push, veröffentlichen erst am sonntag nach freigabe“**.

Damit sind Funktions-Freeze, lokaler Paketbau einschließlich Quellen/Lizenzen/Prüfsummen sowie Commit und Push autorisiert. **Keine Veröffentlichung, kein Release-Tag und keine automatische Freigabe.** Frühester Veröffentlichungstag ist **Sonntag, 25.10.2026**; zusätzlich ist eine erneute ausdrückliche Freigabe erforderlich. Bestehende öffentliche Releases und der zurückgenommene 1.0.1-Entwurf bleiben erhalten.

## Eingefrorener Umfang

Der persönlich geprüfte Entwicklungsstand wird als **1.0.2 · Build 35** paketiert. Neue Versions-/Buildkennung zur eindeutigen Unterscheidung von 1.0.1 Build 34. Die begrenzte MP4Box-H.264-Überlappungszuordnung ergänzt den vorhandenen Ausgabeprüfweg. Abschluss und Kontrollplayer verwenden die zugeordneten tatsächlichen Zeiten mit Drei-Sekunden-Vorlauf. Fehler-/Abbruch- und Identitätsschutz bleiben erhalten.

Bestehende Funktionen und unveränderte Motor-/Laufzeitdateien: **otr-can 0.1.6**, **libmpv 0.41.0**. Keine neuen Funktionen außerhalb dieses geprüften Umfangs. Diagnoseprototypen gehören zu den Quellen, nicht in die Programmverteilung. [Änderungen und Grenzen](RELEASE-NOTES-1.0.2.md).

## Paketweg

Eigener neuer Ausgabeordner `artifacts/release-1.0.2-build35`, ohne Überschreiben alter Pakete. Setup und Portable entstehen aus demselben geprüften Publish-Verzeichnis. Passende CAN-Quellen mit Quellcommit und SHA-256-Dateimanifest, unveränderte otr-can-Quellen sowie libmpv-Quellen-/Relink-Archive und Prüfsummen gehören zum Paketsatz.

Die vier vorbestehenden lokalen Produktions-Lockänderungen werden bewahrt und aus dem Quellcommit ausgeschlossen. Im eingefrorenen Quellstand werden die bestehenden committierten Lockdateien verwendet; Restore mit `win-x64` im gesperrten Modus. Keine Aktualisierung von Abhängigkeiten. Der lokale Buildzähler wird erst nach erfolgreichem vollständigem Paketbuild und Tests von 34 auf 35 übernommen.

Quellcommit und exakte Buildbytes werden im Paketnachweis festgehalten. Spätere Paket-/Abnahmenachträge sind separate Dokumentationscommits und ändern die eingefrorenen Pakete nicht. Paketbau selbst veröffentlicht nichts.

## Nachweise und offene Abnahmen

Letzter Entwicklungsnachweis: **1411/1411 Solution-Tests, 40/40 isolierte Tests, vier von vier native Furiosa-MP4Box-Prüfungen**, Release ohne Warnungen/Fehler. Persönliche Filmprüfungen sind in den jeweiligen Berichten dokumentiert. Neue Paket-/Installer-/Portable-Prüfungen stehen bei Erstellung dieses Freeze-Dokuments noch aus und werden im Ergebnisbericht ergänzt.

Die früheren Systemhänger sind trotz positiver Wiederholungen nicht ursächlich geklärt. Diese Paketierung behauptet keine Fehlerbehebung. Bekannte MP4Box-Bild-/Schnittgrenzen bleiben dokumentiert. Die bisherigen persönlichen Build-34-Setup-/VM-/Portable-Abnahmen werden nicht auf Build 35 übertragen.
