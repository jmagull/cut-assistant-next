# Entwicklungstagebuch

## 01.10.2026 – Upload-Korrektur und Cutlist-Einstellungen

Veröffentlichter Stand: **CAN 0.2.1 · Build 7 · RC2**. Die vorherige Ausgabe **0.2.0 Build 6 – RC2** bleibt unverändert verfügbar.

### Änderungen

- Beim Upload bleiben Autor, Programmname und Version der gespeicherten Cutlist erhalten. Die vorher fest eingetragenen Werte überschreiben diese Angaben nicht mehr.
- Die getrennten HTTP-Formularfelder verwenden `app=CutAssistantNext` und die numerische Version der laufenden CAN-Anwendung, unabhängig von einer geladenen Fremd-Cutlist.
- RC2 ist auch für normale lokale Builds hinterlegt. Die Patch-Version wurde auf 0.2.1 erhöht und Build 7 regulär erstellt.
- Ausführliche URL-Hilfe erscheint erst nach einem fehlgeschlagenen Verbindungstest. Ein erfolgreicher erneuter Test lässt sie ausgeblendet.
- Der Einstellungsdialog ist höher und wird am oberen Rand des aktuellen Monitor-Arbeitsbereichs platziert. Kleine Bildschirme behalten die Scrollmöglichkeit.
- Die zusätzliche HTTPS-Adresse von Sniplist wird mit gültigem FRED akzeptiert, ohne sie im Dialog hervorzuheben. Details stehen in ADR-009.
- Nutzeranleitung und Architekturentscheidungen wurden ergänzt. Die Windows-x64-Lockdateien berücksichtigen Version 0.2.1; externe Paketversionen bleiben unverändert.

### Nachweise

- Letzter Release-Build: **0 Warnungen, 0 Fehler**; vollständige Testsuite: **496/496 bestanden**.
- Die Windows-x64-Paketwiederherstellung mit `--locked-mode` ist erfolgreich.
- Echte Testuploads bestätigten den Erhalt der Cutlist-Metadaten sowie die Annahme von `app=CutAssistantNext` (ID 2079437) und HTTP-`version=0.2.0` (ID 2079440). Die HTTP-Felder sind nicht separat in der heruntergeladenen Cutlist sichtbar; die Zuordnung beruht auf der jeweils getesteten Anwendung. Weitere Details in ADR-010.
- Der Benutzer hat die Löschung sämtlicher Test-Cutlists bestätigt.
- Der Benutzer hat die vergrößerte und nach oben verschobene Fensterdarstellung bestätigt.
- Nach Veröffentlichung: Installation in einer VM mit GPAC, .NET 10 und FFmpeg bestätigt. „Grey’s Anatomy – Durchs Feuer“ (HQ-MP4) wurde in drei Behaltebereichen geschnitten und vollständig zusammengefügt; das Protokoll endet mit „Fertig.“. Die Cutlist stimmt mit den Schnittaufrufen überein und enthält CAN 0.2.1 sowie Autor Joerg. Bild und Ton der fertigen Datei wurden vom Benutzer als einwandfrei bestätigt.

### Noch offen

- Ein echter Sniplist-Verbindungs-/Upload-Test ist noch nicht bestätigt.
- Der neue HTTP-Versionswert 0.2.1 wurde noch nicht separat am Server praktisch getestet.
- Die Setup-Deinstallation und ein separater Videoschnitt mit dem neuen Portable-ZIP stehen noch aus.

### Veröffentlichung um 20:03 Uhr MESZ

- Änderungen mit Commit `dcf4123` gesichert und nach GitHub gepusht.
- Setup und Portable-ZIP aus dem frisch entpackten Quellarchiv dieses Commits gebaut: 496/496 Tests, keine Buildwarnungen oder Fehler.
- 148 Portable-Dateien geprüft; Start mit korrekter Versionsanzeige und reguläres Beenden erfolgreich.
- Alle sechs Uploads anhand von Größe und GitHub-SHA-256-Digest mit den lokalen Dateien abgeglichen.
- Nach Nutzerauftrag als Vorabversion [v0.2.1-build7-rc2](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.1-build7-rc2) veröffentlicht. Tag-Zuordnung und öffentlicher Status bestätigt. Prüfbericht und Veröffentlichungsnachweis wurden nach dem Paketbau ergänzt.
