# Cut Assistant Next 1.0.3 Build 62 – technischer Wiederaufnahmebericht

Stand: 19.11.2026. Dateiversion bleibt 1.0.3.62. **Paketabschluss blockiert; keine aktualisierte installierbare Prüffassung vorhanden.**

Die beiden Ergänzungen bleiben implementiert: Werkseinstellungen A/E/Umschalt+A/Umschalt+E mit Upgrade-Konfliktschutz sowie die asynchrone TEMP-Speicheranzeige. In dieser Fortsetzung wurde keine Funktion erneut implementiert und kein Schnittmotor oder Schnittalgorithmus geändert. Ergänzt wurden Sicherung, Diagnose und dieser Bericht. Kein Test wurde entfernt, deaktiviert, übersprungen oder abgeschwächt.

## Gesicherter Stand und unveränderte Ausgaben

Der vollständige Quellstand liegt unter `.build/worktrees/build62-settings`. Das Verzeichnis ist ein isolierter Export aus Build 61 mit den Build-62-Änderungen, kein registrierter Git-Worktree. Basis: `v1.0.3-build61`, Commit `4a599f3fcc9a5434123ce11059a6d789dd37deaf`. Spotlight: `75ee1475adf20a8c8231ab2989eab7eff37e6b09`. Die neue Sicherung unter `checkpoints/` enthält Quellarchiv, Einzeldateimanifest und SHA-256; ZIP-Integrität und jeder Eintrag werden nach Erstellung geprüft. Sie ist ausschließlich ein Quellcheckpoint, kein Portable-Paket.

Die Setup-, Portable- und Quellpakete von Build 61 und der ersten Build-62-Prüffassung wurden erneut gegen ihre bestehenden SHA-256-Listen geprüft: unverändert. Nachweis: `diagnostics-20261119/PROTECTED-PACKAGES.json`. Die vorbestehenden Änderungen im ursprünglichen Repository bleiben erhalten. Keine Installation, kein Commit/Merge und keine Veröffentlichung ausgeführt.

## Windows-Ausführungsumgebung

Prozessstarts und Schreib-/Lesezugriffe auf eigene Arbeitsdateien sind in dieser Sitzung wieder möglich. Der frühere Fehler `invalid sandbox permission path: 'file:///workspace' is invalid on 'windows'` entsteht bei der Prüfung der Ausführungsberechtigungen, vor dem Start des Projektprozesses. Die aktuelle Umgebung weist Windows-Pfade aus. In den untersuchten Projektkonfigurationen wurde keine entsprechende Sandbox-Konfiguration gefunden. Ein Defekt in CAN als Ursache dieses Startfehlers ist nicht belegt. Die übergeordnete Laufzeitkonfiguration wurde nicht verändert.

Eine vollständige Wiederherstellung ist weiterhin **nicht** erreicht:

- **ACL-Schreiboperation ohne Wirkung:** Der isolierte Diagnoseprozess setzt zunächst eine Sperre für den aktuellen Benutzer und danach in einem separaten eigenen Verzeichnis für Jeder. Beide Aufrufe melden keinen Fehler. Die unmittelbar erneut eingelesene DACL enthält jedoch jeweils keine Deny-Regel und entspricht weiter dem Ausgangszustand. Sowohl .NET-Dateizugriffe als auch natives `CreateFileW` bleiben erlaubt. Die Berechtigungstests können damit ihre erforderliche Ausgangsbedingung nicht herstellen. Original-DACLs wurden jeweils im `finally` zurückgesetzt und byteweise verglichen; eigene Diagnoseverzeichnisse anschließend bereinigt. Keine produktiven Verzeichnisrechte verändert. Vollständige Ausgabe und Probe-Quellcode liegen im Diagnoseordner.
- **VSTest/testhost:** Auch der erneute reguläre `dotnet test`-Versuch bricht nach 90 Sekunden beim Verbindungsaufbau ab. Der Prozessstart selbst funktioniert; die Kommunikation mit dem Testhost bleibt gestört. TRX und vollständiges Log sind gesichert. Ein längerer Timeout ist kein Nachweis einer reparierten Umgebung.
- **Windows PowerShell:** Ein unabhängiger Kindprozess mit nur einer stdout-/stderr-Ausgabe und `exit 7` meldet einen Fehler bei `InitializeDefaultDrives` des FileSystem-Anbieters. Nach mehr als 343 Sekunden war er noch nicht beim Nutzbefehl angekommen; später erfolgten beide Ausgaben und Exitcode 7. Die vorhandenen Werkzeugtests haben dagegen kurze bewusst begrenzte Antwortfristen. Dies liefert eine konkrete Umgebungsursache für die beobachteten Zeitüberschreitungen; die internen Ursachen des Providerfehlers sind hier nicht abschließend geklärt.
- **MSBuild:** Der erste erneute Mehrprozess-Rebuild beendet sich mit Exitcode 1 trotz Ausgabe „0 Warnungen / 0 Fehler“. Ein anschließender vollständiger Einprozess-Rebuild mit `-m:1 -nr:false -p:UseSharedCompilation=false` ist erfolgreich (25,46 Sekunden, Exitcode 0, 0 Fehler, 0 Warnungen). Beide Logs bleiben erhalten.

## Alle Tests erneut ausgeführt

Wegen des blockierten Testhosts wurden dieselben unveränderten Testassemblies mit dem originalen xUnit-Executor im Prozess ausgeführt. Der Hilfsrunner erfasst jeden Test einschließlich vollständiger Fehlermeldungen und Stacktraces. Er erlaubt vier parallele xUnit-Threads und respektiert die vorhandenen Collection-Einstellungen für WPF. Dies ersetzt keine noch offene reguläre VSTest-Abnahme in einer funktionsfähigen Umgebung.

| Suite | Gesamt | Bestanden | Fehlgeschlagen | Übersprungen |
| --- | ---: | ---: | ---: | ---: |
| CAN Core | 200 | 200 | 0 | 0 |
| CAN Media | 421 | 418 | 3 | 0 |
| CAN App | 1.586 | 1.582 | 4 | 0 |
| CAN Cutlists | 80 | 80 | 0 | 0 |
| Spotlight | 59 | 59 | 0 | 0 |
| Spotlight App | 19 | 19 | 0 | 0 |
| **Gesamt** | **2.365** | **2.358** | **7** | **0** |

Keine xUnit-Infrastrukturfehler im alternativen Lauf. Dieser Lauf ist vollständig beendet, aber nicht erfolgreich. Der früher stockende Media-Lauf ist damit bis zum Ende geprüft: 421 Fälle, drei Fehler.

Die **33 neuen Fälle** sind Teil des App-Laufs: 9 Werkseinstellungs-/Migrationsfälle, 16 Speicherinformationsfälle, 2 WPF-Einstellungsdialogfälle und 6 Kombinationen aus Ansicht und Rot-/Grün-Modus. Ergebnis: **32 bestanden, 1 fehlgeschlagen**. Der eine neue Fehler ist konkret `WorkingDirectoryInfoTests.MissingReadPermissionIsReportedAndAclIsRestored`: erwartet unbekannte Ordnergröße mit Zugriffshinweis, tatsächlich 0 Bytes für das leere weiterhin lesbare Verzeichnis. Vollständige Liste: `NEW-33-TESTS.json`.

### Vier ACL-Fehler einzeln

| Testklasse und Methode | Erwartung | Tatsächlich | Diagnose |
| --- | --- | --- | --- |
| `CutlistAutomaticSaveTests.RejectsUnwritableFolderAndRestoresOnlyItsChangedAccessPermissions` | InvalidOperationException beim Schreibtest | Keine Ausnahme | gesetztes Schreibverbot nicht wirksam |
| `MovieOutputTargetResolverTests.RejectsUnwritableFolderWithSettingsHintAndNoProbeLeftBehind` | InvalidOperationException mit Einstellungshinweis | Keine Ausnahme | gesetztes Schreibverbot nicht wirksam |
| `Build62SettingsTests.WorkspaceRejectsActualMissingWritePermissionAndLeavesNoProbe` | UnauthorizedAccessException | Keine Ausnahme | gesetztes Schreibverbot nicht wirksam |
| `WorkingDirectoryInfoTests.MissingReadPermissionIsReportedAndAclIsRestored` | unbekannte Größe und Zugriffshinweis | 0 Bytes, kein Zugriffsfehler | gesetztes Leseverbot nicht wirksam; der neue fehlgeschlagene Test |

Die Fehler wurden unverändert reproduziert; frühere Wiederholung mit einem anderen isolierten TEMP-Stamm zeigte dieselben vier Fehler. Der separate DACL-Rücklesetest belegt die fehlende Testvoraussetzung. Das ist keine erfolgreiche Prüfung der Fehlerbehandlung bei tatsächlich fehlenden Rechten. Alle vier bleiben offen, bis sie unter wirksamen Windows-ACLs bestehen.

### Drei Prozessfehler einzeln

| Test | Ergebnis | Zusammenhang |
| --- | --- | --- |
| `ToolProbeRunnerTests.ConcurrentReadersAvoidDeadlockOnLargeHelpOutput` | Werkzeug-Antwortzeit überschritten | PowerShell-Start gestört; unabhängiger Providerfehler nachgewiesen |
| `ToolProbeRunnerTests.ReadsBothOutputStreamsAndPreservesFailureExitCode` | Werkzeug-Antwortzeit überschritten | gleicher PowerShell-Startpfad |
| `CutToolRunnerTests.CancellationTerminatesChildProcessBeforeReturning` | TaskCanceledException beim Warten auf Kindprozess-Marker | PowerShell erreicht den Markerschreibbefehl nicht innerhalb der bestehenden Frist |

`StreamsBothPipesWithoutDeadlockAndReportsExitFailure` sowie der Test für Argumente mit Leerzeichen/Anführungszeichen/Unicode sind im abgeschlossenen Lauf bestanden. Vollständige technische Ursachen und Stacktraces aller sieben Fehler stehen unverändert in `FAILED-TESTS.json` und den Suite-JSONs.

## Native Schnitt-, Audio- und Dateisicherheitsprüfungen

Der unveränderte native Build-62-Prüfer wurde mit dem bestehenden otr-can-Motor `0.1.6-audio-diag.1`, FFMS2, FFmpeg/ffprobe und MP4Box erneut ausgeführt. Sechs Schnitte (zwei Motoren × drei TEMP-Varianten) durchliefen ihre vorhandenen Prüfungen; die Ausgaben wurden zusätzlich unabhängig mit ffprobe geprüft: jeweils zwei AAC-Spuren, ungefähr vier Sekunden, Speicherinformationen protokolliert, eigene Arbeitsverzeichnisse entfernt. Beide kontrollierten Abbrüche durchliefen ihre Prüfungen; vorhandene Zielinhalte blieben erhalten und Auftragsdateien wurden bereinigt.

Der neunte Fall endet ausdrücklich erfolglos mit `Write permission was ignored`, entsprechend der separat nachgewiesenen ACL-Störung. Deshalb ist der native Gesamtlauf **nicht bestanden**. Die abschließende Quellidentitätsprüfung hinter diesem Fall wurde nicht erreicht; sie wird nicht als bestanden ausgegeben. Teilnachweise und alle acht Schnitt-/Abbruchprotokolle sind gesichert. Es wurde kein echter Transfer zwischen zwei physischen Laufwerken vorgetäuscht; diese persönliche Prüfung bleibt offen.

Die vorhandenen **20 nativen Audioregressionen sind vollständig bestanden** (Exitcode 0, `Passed: true`, `Count: 20`). Dazu gehören gesunde und beschädigte AAC-/AC3-Spuren, Mehrspurdateien, Grenzen, Abbrüche und bestehende MP4Box-Ausweichabläufe. Vollständige Ergebnisse inklusive Quell- und Bibliotheksprüfsummen: `AUDIO-20-RESULTS.json`. Diese Ergebnisse behaupten keine neue EAC3-Unterstützung.

Die sieben Produktbibliotheken sind zwischen Release-Ausgabe, App-Testverzeichnis, xUnit-Hilfsrunner und beiden nativen Prüfern bytegleich. `TESTED-BINARY-COMPARISON.json` enthält die SHA-256-Werte. Die Quellen von Core, Media, Cutlists, App-Schnittdiensten und Spotlight sind gegenüber der ersten Build-62-Prüffassung unverändert (`CUTTING-SOURCE-COMPARISON.json`). **Ein Vergleich mit endgültigen aktualisierten Paketbibliotheken ist noch nicht möglich, da keine neuen Pakete erstellt wurden.**

## Persönliche Teilabnahme bleibt gültig dokumentiert

Die Rückmeldung vom 19.11.2026 bleibt vollständig unter `docs/PRAXISTESTS-BUILD62-20261119.md` erhalten: Power-User-1-Sichtprüfung, Shortcut-Konfiguration, Speicherung von Windows-TEMP, erfolgreicher Wetterfee-MP4Box-Schnitt mit fünf Segmenten und persönlich einwandfreiem Bild/Ton/Übergängen, technischer EAC3-Abbruch mit ausdrücklich bestätigtem MP4Box-Ausweichschnitt sowie Setup-Upgrade am Arbeitsrechner. Bereinigung ist jeweils laut Protokoll bestätigt. Die gemeldeten 164,75 GiB bzw. 163,87 GiB sind damalige Messwerte; 7,46 GiB Arbeitsbedarf und 3,67 GiB Zielbedarf sind damalige Schätzungen.

Die persönliche Bild-/Ton-/Synchronitätsprüfung der gesonderten Fallback-Ausgabe bleibt offen. Ebenfalls offen: beide Ergänzungen, alle Ansichten/Modi/Tastenwiederholung/Neustart, eigener Arbeitsordner, echter Laufwerkswechsel, Dateischutz/Abbrüche, Protokollfenstergrößen, Portable-Start sowie VM-Installation/-Deinstallation. Die technische Wiederholung erweitert keine persönliche Freigabe.

## Konkrete Wiederaufnahmebedingungen und Paketabschluss

1. Eine Windows-11-Entwicklungsumgebung mit ausschließlich gültigen Windows-Sandboxpfaden verwenden. Kein `file:///workspace` in den von der Laufzeit geprüften Berechtigungswurzeln. Das ist eine Anforderung an die Entwicklungsumgebung, kein Anlass für Änderungen an produktiven Windows-Verzeichnisrechten.
2. Die beigefügte isolierte ACL-Probe muss die gesetzte Deny-Regel tatsächlich zurücklesen können; Lesen/Erstellen muss dann `ERROR_ACCESS_DENIED` ergeben. Anschließend muss die ursprüngliche DACL wiederhergestellt werden. Den vorhandenen Probe-Code bei Bedarf um eine harte Fehlerprüfung ergänzen; seine derzeitige Exit-0-Ausgabe bedeutet nur „Diagnose beendet und bereinigt“, nicht „ACL-Umgebung geeignet“.
3. Ein einfacher Windows-PowerShell-Kindprozess muss ohne FileSystem-Providerfehler rechtzeitig starten; die bestehenden Werkzeugtestfristen unverändert lassen. Reguläre VSTest/testhost-Kommunikation muss funktionieren.
4. Gesicherten Quellstand mit .NET SDK 10.0.303 und den dokumentierten nativen Abhängigkeiten laden, SHA-256-Manifeste prüfen. `dotnet restore CutAssistantNext.sln --locked-mode -r win-x64` und vollständigen Release-Rebuild mit expliziter Nummer 62 ausführen. Die erfolgreichen Einprozess-Parameter stehen in BUILD-SOURCE.txt. Kein automatisches Erhöhen auf Build 63.
5. Die 33 neuen Fälle sowie vollständige CAN-/Spotlight-Suiten regulär erneut ausführen; native 9-Fall-Speicher-/Dateischutzprüfung und alle 20 Audioregressionen wiederholen. Keine Auslassungen. Erst bei erfolgreichen Pflichtprüfungen den Paketabschluss beginnen.
6. Neue Build-62-Portable- und Setup-Dateien in einem separaten Ausgabeordner erzeugen und prüfen. Die veröffentlichte Build-61-Ausgabe und erste Prüffassung erhalten. Finale veröffentlichbare Bibliotheken erneut gegen die getesteten Dateien vergleichen und erforderlichenfalls direkt an diesen Paketbibliotheken testen. SHA-256, vollständiges Dateimanifest und abschließenden Paketbericht erst für diese fertigen Pakete erstellen.
7. Aktualisierte installierbare Prüffassung zur persönlichen Windows-11-Nachprüfung vorlegen. GitHub-Veröffentlichung, Merge nach main und Release-Freigabe bleiben ausdrücklich gesperrt, bis Jörg gesondert zustimmt.

Dieser Bericht dokumentiert den gesicherten Entwicklungsstand und die Wiederaufnahmebedingungen. Er ist **keine technische Gesamtabnahme und keine Paket- oder Release-Freigabe**.
