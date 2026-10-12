# Schritt 3 – Cutlist automatisch speichern

Stand: 12.10.2026. Auftrag „ok dann 3)“. Lokaler Entwicklungsstand **0.2.1 · Build 19 · RC3**. Keine neuen Setup-/Portable-Pakete oder Veröffentlichung.

## Verhalten

Der Cutlist-Erstellungsdialog mit Metadaten, Bewertung, Kommentar und Filmnamensvorschlag bleibt erhalten. Bei **Cutlist speichern** wird die Cutlist unmittelbar im eingestellten Ordner **Eigene Cutlists** gespeichert; der zusätzliche Dateispeicherdialog entfällt.

Der Dateiname bleibt der vollständige Originaldateiname plus .cutlist. Beispiel:

    Ghostbusters__Hexenprobleme_26.09.19_10-50_pro7maxx_25_TVOON_DE.HQ.mp4.cutlist

Keine manuelle Umbenennung, automatische Nummerierung oder Übernahme des editierten Film-Ausgabenamens. Die Originalzuordnung im Cutlist-Inhalt bleibt ebenfalls erhalten.

Eine Cutlist kann weiterhin ohne vorherigen Filmschnitt gespeichert werden. Kein bestimmter Schnittmotor oder neu eingerichtetes Schnittwerkzeug erforderlich. Cutlist-Format, Inhaltserzeugung, Schnittpunkte und Behalteberechnung unverändert.

## Verzeichnis und Konflikte

Ausschließlich OwnCutlistsDirectory verwenden. Fehlende/leere/relative, nicht vorhandene oder nicht beschreibbare Zielordner zurückweisen. Kein Rückfall ins Originalverzeichnis oder einen anderen Ordner, keine automatische Ordneranlage.

Fehlertitel:

**Cutlist konnte nicht gespeichert werden**

Hinweis bei nicht nutzbarem Standardordner:

**Das Standardverzeichnis für eigene Cutlists ist nicht eingerichtet oder nicht erreichbar. Bitte überprüfe den Pfad unter Einstellungen.**

Vorhandene Cutlist nur nach ausdrücklichem Ja in der Überschreibfrage ersetzen. **Nein ist Vorgabe**, erhält die vorhandene Datei und lässt den Erstellungsdialog offen. Schreibgeschützte Ziele und Zielordner mit dem geplanten Dateinamen werden nicht überschrieben.

Eine nach der Vorprüfung auftauchende Cutlist wird ohne Zustimmung nicht ersetzt. Überlange abgeleitete Namen werden abgelehnt, nicht gekürzt. Der ursprüngliche Dateiname bleibt verbindlich.

## Geschützter Schreibweg

CutlistFileWriter.WriteSafely ergänzt den bisherigen Writer; dessen alte Write-Funktion bleibt unverändert für bestehende Aufrufer.

1. Vorhandenes, absolutes Zielverzeichnis verlangen; kein Directory.CreateDirectory.
2. Bestehenden Serializer unverändert für UTF-8 ohne BOM und CRLF verwenden.
3. Eigene temporäre Datei im Zielordner mit CreateNew vollständig schreiben und flushen.
4. Geschlossene temporäre Datei an den endgültigen Zielnamen verschieben. Überschreiben nur bei vorhandener Datei und ausdrücklicher Zustimmung.
5. Nach erfolgreicher Übergabe gehört der temporäre Pfad nicht mehr diesem Schreibvorgang; keine weitere Löschung dort. Bei Fehlschlag nur die selbst erzeugte temporäre Datei entfernen.

Eine vorhandene Cutlist wird vor erfolgreicher Vorbereitung weder geöffnet/trunkiert noch gelöscht. Ein Veröffentlichungsfehler erhält die alte Datei. Schreib- und Aufräumfehler werden nicht stillschweigend ignoriert.

Dateisystemprüfungen und Schreiben laufen über Task.Run außerhalb des UI-Threads. Während des laufenden Speichervorgangs sind doppelte Starts und Schließen des Dialogs blockiert, damit Original-/Uploadzuordnung stabil bleiben. Nach Ablehnung oder Fehler ist der Dialog wieder bedienbar.

_lastSavedCutlistFilePath und Namenszustand werden erst nach erfolgreichem Schreiben aktualisiert. Die bisherige Erfolgsbestätigung folgt danach. Upload bleibt eine gesonderte, unveränderte Aktion.

## Wiederverwendung und Dateien

Die strenge Verzeichnisprüfung und die private Schreibprobe aus Schritt 2 sind in ConfiguredOutputDirectory zusammengeführt. Film- und Cutlist-Ziele nutzen sie; bisherige File-Picker-Fallbacks beim Laden bleiben unverändert. MovieOutputTargetResolver enthält nur diese Wiederverwendung; seine bisherigen Prüfungen und sein Verhalten bleiben erhalten.

Funktionsdateien:

- src/CutAssistantNext.App/MainWindow.xaml.cs: direkte Cutlist-Speicherung, Konfliktentscheidung, asynchroner Ablauf und Erfolgspfad.
- src/CutAssistantNext.App/Services/CutlistOutputTargetResolver.cs: Originalname und ausschließlich konfiguriertes Ziel.
- src/CutAssistantNext.App/Settings/ConfiguredOutputDirectory.cs: gemeinsame strenge Ordnerprüfung.
- src/CutAssistantNext.App/Services/Cutting/MovieOutputTargetResolver.cs: gemeinsame Ordnerprüfung wiederverwenden.
- src/CutAssistantNext.Cutlists/IO/CutlistFileWriter.cs: zusätzlicher geschützter Schreibweg.
- src/CutAssistantNext.App/Dialogs/VideoFolderSettingsDialog.xaml: drei Hilfetexte an direktes Speichern von Film/Cutlist anpassen; keine neue Einstellung oder Änderung der Dateiauswahl.

## Prüfstand

**Build 19 RC3 ohne Warnungen/Fehler. 881/881 CAN-Tests bestanden**, keine übersprungen: Core 74, Cutlists 72, Media 125, App 610.

21 zusätzliche Fälle prüfen insbesondere:

- Exakter Original-Dateiname und ursprüngliche Zuordnung trotz manuell editiertem Film-Ausgabenamen.
- Speicherung ohne Schnittmotor und ohne vorausgehenden Filmschnitt.
- Bytegleichheit zum unveränderten Serializer, Umlaute, BOM-/Zeilenformat und unveränderte Schnittzeiten.
- Fehlende, relative und nicht beschreibbare Ordner ohne Fallback/Neuanlage.
- Bestehende Cutlist, abgelehntes/bestätigtes Ersetzen und Konflikt nach Zielprüfung.
- Schreibschutz und echte Windows-Dateisperre: alte Datei erhalten, temporäre Datei entfernt.
- Fehler bei Veröffentlichung an einem belegten Ordnerpfad; fremde temporäre Dateien erhalten.
- Überlange Namen nicht umbenennen; bisherige Lade-Fallbacks erhalten.
- Bestehende Filmziel-/Motorprüfungen aus Schritt 2 weiterhin erfolgreich.

Der erste Test für Windows-Dateisperren erwartete ausschließlich IOException. Das beobachtete UnauthorizedAccessException wurde als zulässige Dateisystem-Ablehnung berücksichtigt; die Inhalts-/Aufräumprüfungen bleiben bestehen und bestehen nun ebenfalls. Keine Tests entfernt, deaktiviert oder übersprungen.

Echter kompilierter CutlistGenerationDialog geprüft: bewerteter Speichern-Button löst SaveRequested aus. Ein isolierter Prüfcallback verwendet tatsächlichen Resolver/Writer im Hintergrund und erzeugt ohne Motor eine bytegleiche Cutlist mit dem Originalnamen. Fenster bei 720×720 und Mindestgröße 600×400 gerendert; Speichern-Button erreichbar, keine Bindungsfehler. Der Prüfcallback ersetzt den Hauptfenster-Handler im Prüfharness; keine automatische Abnahme seiner nativen Ja/Nein-/Erfolgsmeldungen behauptet.

Zusätzlich kompilierter Standardordner-Dialog bei Normal- und Mindestgröße geprüft: korrigierte Texte und erreichbare Aktionsbuttons, keine persönlichen Einstellungen gespeichert. Hauptfenster-Code geprüft: kein SaveFileDialog mehr; Cutlist-Speicherung ohne ResolveCutlists-Fallback, Nein als Konfliktvorgabe und Uploadpfadzuweisung nach dem erfolgreich abgewarteten Schreibvorgang.

Relevante C#-Dateien formatiert. Ohne Restore gebaut; lokale Buildnummer einmal auf 19 erhöht. Acht lokale Lockdateihashes und veröffentlichte Build-16-Artefakte erhalten. Keine realen Medien oder Einstellungen eingecheckt.

## Nachweise und Grenzen

Belege unter .build/cutlist-auto-save-20261012: BUILD.log, BUILD-final.log, FORMAT.log, TEST.log, TEST-first-failed.log, test-results-final-help, WPF.log, LOCKS-BEFORE.json und ui-proof-final mit WPF-/Ordnertextnachweisen und Renderbildern.

Dateisystemrechte oder Netzwerkzustand können sich nach der Vorprüfung ändern. Fehlermeldung und Schutz des bestehenden Ziels bleiben erforderlich; vollständige Atomarität auf beliebigen Netzwerkdateisystemen wird nicht zugesagt. Falls Dateirechte auch das Aufräumen verhindern, kann eine eigene temporäre Datei verbleiben; Fehler wird gemeldet.

Persönliche Abnahme des neuen Speicherablaufs durch den Nutzer steht noch aus. Kein Commit, Push, Setup oder Release. Nächster Schritt gemäß Plan: **4 – Namensmasken-Editor erweitern**.
