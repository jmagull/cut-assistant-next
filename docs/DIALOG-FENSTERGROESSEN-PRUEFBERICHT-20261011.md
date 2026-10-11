# Dialogfenstergrößen – Prüfbericht

Stand: 11.10.2026. CAN **0.2.1 · Build 11 · RC3**.

## Verhalten

Die Dialoge **Schneiden**, **Video vorbereiten und schneiden**, **Schneiden abgeschlossen** und **Cutlist erzeugen** speichern ihre Breite, Höhe und Maximierung getrennt. Beim nächsten Aufruf und nach einem Programmneustart werden die Werte wiederhergestellt. Das Fortschrittsfenster behält damit auch die vergrößerte Fläche für sein Protokoll.

Vorhandene Cutlist-Fensterdaten werden weiter aus cutlist-window-settings.json gelesen. Die drei anderen Dialoge verwenden eigene cut-output-, cut-progress- und cut-completion-window-settings.json-Dateien im bestehenden lokalen Einstellungsordner. Hauptfenstergröße, Lautstärke und Werkzeugpfade werden dadurch nicht überschrieben.

Ungültige oder unlesbare Größen fallen auf Standardwerte zurück; übergroße Maße werden an WPFs WorkArea begrenzt. Bei kleiner Arbeitsfläche wird auch die effektive Mindestgröße reduziert. Maximierte und minimierte Fenster speichern ihre normalen Rückkehrmaße; ein minimiertes Fenster wird beim nächsten Aufruf nicht minimiert geöffnet. Fensterpositionen bleiben beim bisherigen Zentrieren.

Ein abgebrochener Schließversuch schreibt keine Einstellungen. Die Maße werden vor dem Zerstören des Fensters erfasst und erst nach tatsächlichem Schließen gespeichert. Ereignishandler werden danach freigegeben. Ein Speicherfehler wird diagnostisch über Trace gemeldet und verhindert das Schließen nicht.

## Geänderter Umfang

- Gemeinsame Größenprüfung und getrennte Dateipfade in src/CutAssistantNext.App/Settings/DialogWindowSettings.cs.
- WPF-Lebenszyklus in src/CutAssistantNext.App/Dialogs/DialogWindowPersistence.cs.
- Vier Dialogkonstruktoren verwenden die gemeinsame Speicherung; die bisherige doppelte Cutlist-Größenlogik entfällt.
- 14 zusätzliche Tests unter tests/CutAssistantNext.App.Tests/Settings/DialogWindowSettingsTests.cs.
- README, Anleitung, Status, Merkliste und Tagebuch aktualisiert.

Keine neue Produktionsabhängigkeit, keine Änderung an Schnittmotoren oder Medienoperationen.

## Nachweise

Release-Build ohne Warnungen oder Fehler; **767/767 CAN-Tests bestanden**. Die 14 neuen Fälle prüfen getrennte Dateien, Standardwerte, Wiederherstellung, Maximierung, ungültige Maße, kleine Arbeitsflächen, ungültige Bildschirmangaben, separate Speicherinstanzen und beschädigte JSON-Dateien.

Echter WPF-Test mit allen vier Dialogen und isolierten Einstellungen in zwei getrennten Prozessen:

| Dialog | Gespeichert und nach Neustart wiederhergestellt |
|---|---|
| Schneiden | 1250 × 820 |
| Fortschritt/Protokoll | 1280 × 850 |
| Schneiden abgeschlossen | 1310 × 880 |
| Cutlist erzeugen | 1340 × 910 |

Zusätzlich maximiert, danach minimiert geschlossen: normale Rückkehrmaße 900 × 700 und Maximierungswunsch erhalten. Beim Neustart maximiert wiederhergestellt. Abgebrochenes Fortschrittsfenster-Schließen erzeugt keine Datei; endgültiges Schließen speichert. Bestehendes Cutlist-JSON nur mit Width/Height erfolgreich geladen. Vergrößertes Protokollfenster gerendert und geprüft. Persönliche Einstellungen durch die Prüfdialoge nicht verändert.

Im ersten Build war eine eigene Textersetzung zu breit und änderte Bedingungen neben den Konstruktoren; gezielt korrigiert. Ein bestehender Prozessabbruchtest scheiterte im ersten Gesamtlauf an einer kurz gesperrten PID-Datei; gezielte Wiederholung und anschließender vollständiger Testlauf bestanden. Keine Änderung oder Entfernung dieses Tests. Ein erster WPF-Prüfhelfer setzte ShowActivated=false auch für ein bereits maximiertes Fenster; nur den Helfer an WPFs Regel angepasst, danach Neustartprüfung bestanden. Erstprotokolle erhalten.

## Persönliche Rückmeldung

Am 11.10.2026 bestätigt der Nutzer nach einem weiteren Chicago-Fire-Schnitt direkt in CAN: **„Fenstergröße Übernahme : check“**. Die Größenwiederherstellung ist damit auch im persönlichen Schnittablauf positiv bewertet. Die Rückmeldung benennt keine separate persönliche Prüfung jedes der vier Dialoge; die vollständige Vier-Dialog-/Neustartabdeckung stammt aus dem oben beschriebenen WPF-Test.

## Erhaltung

Acht Lockdateien einschließlich der vier vorbestehenden Änderungen erhalten. Rust-Motor 0.1.4 unverändert. Buildnummer 11 und RC3 erhalten; kein Commit oder Push im Rahmen dieser Umsetzung.

Lokale Belege im ignorierten .build/dialog-window-sizes-20261011: BUILD-FINAL.log, TESTS-FINAL.log, PROCESS-TEST-RETRY.log, UI-SAVE.log, UI-LOAD.log, isolierte Save-/Load-Ergebnisse und Renderbild sowie Abschlussbeleg.
