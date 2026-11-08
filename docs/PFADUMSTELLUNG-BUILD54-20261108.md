# Cut Assistant Next 1.0.3 Build 54 – einheitlicher Motorordner

Auftrag vom 08.11.2026: Den mitgelieferten otr-can-Motor wieder einheitlich unter **`tools\otr-can\otr_can.exe`** bereitstellen. Basis ist der vollständig integrierte Build 53, Quellcommit `b75992921d7b8cee93c6016f22187a2f897ec348`, Dokumentationsstand `6d6a288615c8497cf1db710956daac2219b7d6da`. Die [persönliche Abnahme von Build 53](ABNAHME-BUILD53-20261108.md) bleibt gültiger Nachweis dieses unveränderten Paketsatzes.

Build 54 war bei Prüfung der Entwicklungsrefs, bekannten Zähler und Paketverzeichnisse frei. Der einzige neue maßgebliche Entwicklungsstand liegt im kanonischen Repository `C:\Users\joerg\Documents\GitHub\cut-assistant-next`, Branch **`codex/integration-build54`**, sicherer Worktree `.build/worktrees/build54`. Build 53 bleibt eingefrorene Referenz; keine Weiterentwicklung in den alten Funktions-Checkouts. main und dessen Nutzeränderungen bleiben unangetastet.

## Änderung

Die Motorauflösung verwendet den absoluten Anwendungsordner und dessen relativen Unterpfad `tools/otr-can`. Der aktuelle Arbeitsordner und alte gespeicherte Motorpfade bestimmen die Auswahl nicht. Gespeicherte Einstellungen und der ffmsindex-Pfad werden nicht überschrieben. Eine eventuell vorhandene alte `runtime/otr-can`-Kopie wird nicht verwendet; fehlt der neue Paketmotor, bleibt der verständliche Fehler statt eines stillen Rückfalls erhalten.

Die vorhandene Paketierung und ihre reale Motorprüfung verwenden denselben neuen Pfad für ZIP und Setup. Versionsangaben, Voraussetzungen und aktuelle Bedienhinweise stehen auf 1.0.3 Build 54. Die schon in Build 53 enthaltenen Spotlight-Hinweise und Quellenhinweise werden auch vom regulären Paketwerkzeug mitgeliefert. Historische Build-53-/Audio-Dokumente beschreiben weiterhin ihre damaligen Pfade.

Nur die Ordnerwahl im Resolver ist produktiv geändert. Schnitt-, Audio-, Reparatur-, Klebecenter-, Player- und Spotlight-Logik bleiben bytegleich im Quellvergleich mit Build 53. Der unveränderte geprüfte Motor `0.1.6-audio-diag.1` aus Quellcommit `1979a7f897b95d3fab4020129b8b5e634ea569ac` wird einschließlich Quellen/Lizenzen/Provenienz mitgeliefert; EXE-SHA-256 `4146EE12F81BB7A4FAEF2D4F25D742A17649959BE8C36491169055F5CF9FA4D1`. CAN wird vollständig neu gebaut, keine ausgewechselte Einzel-EXE und keine gemischten CAN-DLL-Sätze.

## Prüfstand vor der Paketierung

- Release-Rebuild 54: **0 Warnungen, 0 Fehler**.
- Gesamte CAN-Suite: **1857 bestanden, 0 Fehler, 0 übersprungen** (Core 200, Cutlists 80, Media 390, App 1187).
- Spotlight: **78 bestanden, 0 Fehler, 0 übersprungen** (59 + 19).
- Paketierung: **13 bestandene Prüfungen**, einschließlich realem Motorstart, Quellen-/EXE-Hashes, Fähigkeiten und Ablehnung beschädigter Pakete. Der positive Fall enthält ausschließlich `tools/otr-can`.
- Resolver prüft auch Anwendungspfade mit Leerzeichen, gespeicherte alte Pfade, fehlenden neuen Motor trotz alter Kopie und den Vorrang des neuen Motors bei vorhandener alter Kopie. Zwei zusätzliche CAN-Fälle, kein bisheriger Verhaltenstest entfernt.

Der erste gesperrte Restore ohne Runtimeangabe wurde wegen der vorhandenen win-x64-Sperrdateien abgelehnt. Anschließend mit dem passenden `-r win-x64` erfolgreich wiederhergestellt; keine Sperrdatei oder Abhängigkeit dafür verändert. Die Schlussprüfung der Pakete und nativen Abläufe wird in einem anschließenden Dokumentationsnachweis ergänzt.

## Persönlicher Test und Bestandsschutz

Neuer Startpfad nach Paketierung:
`C:\Users\joerg\Documents\GitHub\cut-assistant-next\artifacts\release-1.0.3-build54\publish\CutAssistantNext.App.exe`

Für die ZIP den vollständigen neuen Paketsatz in einen **frischen Ordner** entpacken. Nicht nur die EXE ersetzen oder Build 53 von Hand umsortieren. Im Schnittprotokoll muss der Motor aus diesem Ordner unter `tools\otr-can` erscheinen. Ein kurzer regulärer Schnitt und der bestätigte AAC-Reparaturfall sind die gezielte persönliche Wiederholungsprüfung; die bisherige vollständige Abnahme betrifft Build 53.

Das Setup schreibt denselben neuen Pfad. Bei Installation über einen alten Stand können dessen `runtime`-Dateien noch vorhanden sein; sie werden von CAN nicht verwendet. Keine pauschale automatische Verzeichnislöschung. Eine bereits vom Nutzer deinstallierte VM kann die neue Setup-Fassung frisch prüfen. Es erfolgt hier keine automatische Installation oder Deinstallation.

Der vollständige echte NCIS-Langlauf und die Rust-Prüfungen aus Build 53 werden nicht als neu ausgeführt ausgegeben. Für diese reine Pfadumstellung bleiben sie Referenz; passende native Kurzfälle werden erneut mit dem verpackten Motor ausgeführt. Keine geänderte Audiotoleranz, keine automatische Reparatur.

Keine Veröffentlichung, kein Push, kein Merge nach main, kein Release-Tag. Alte Builds einschließlich des persönlich geprüften Build 53 und die stabile 1.0.2 Build 48 bleiben unverändert. Nach Bereitstellung wieder zur persönlichen Prüfung anhalten.


## Abschlussnachweis der fertigen Pakete

Paketierter vollständiger Quellstand: **249511d001c327526e447b6a94127f3ac0c085f2**, Branch `codex/integration-build54`. Das vorhandene reguläre Paketwerkzeug wurde direkt aus dem unveränderten exportierten Quell-Snapshot ausgeführt: gesperrter Restore, vollständiger Build, CAN-Suite, Publish und Inno-Setup-Kompilierung. Ein abschließender reiner Dokumentationscommit ergänzt diesen Nachweis; die paketierten Quellen und Produktdateien werden dadurch nicht verändert.

Erneute Schlussprüfung mit den **tatsächlich verpackten Produkt-DLLs**: **1857 CAN- und 78 Spotlight-Tests bestanden**, keine Fehler oder übersprungenen Tests. Wiederholungen zählen nicht als zusätzliche unterschiedliche Fälle. Dazu **32 native Spotlight-Prüfungen, 32 Kontrollplayer-Prüfungen, vier Fenster-Prozesstests und 20 native synthetische Audiofälle**, alle erfolgreich. Die Audiofälle decken reguläres Schneiden, bestätigte Reparatur, Ablehnung, nicht unterstützte Schäden, laufenden Abbruch, MP4Box-Alternative und Mehrspur-/Tonspur-Abwahl ab. Der Prüfer simuliert die ausdrücklichen Entscheidungen; keine neue persönliche Dialogabnahme behauptet.

Die 20 Audiofälle starten den **fertig verpackten Motor unter `tools/otr-can`** und binden die finalen Dateihashes. App-DLL: `6AB32EA6A55818A9B01AC1C6630CB468A81C0E4DB28F54F51154E7085F18D779`. Motor-EXE unverändert: `4146EE12F81BB7A4FAEF2D4F25D742A17649959BE8C36491169055F5CF9FA4D1`. Laufzeit der Audio-Kurzmatrix: 47,813 Sekunden, eigener Windows-Job mit 2 GiB und 25 % CPU einschließlich Kindprozessen. Der lange echte NCIS-Test und Rust-Suiten aus Build 53 bleiben ausdrücklich vorherige Nachweise; für diese reine Ordneränderung nicht wiederholt.

**264 Anwendungsdateien**: einzeln per SHA-256 geprüft, ZIP exakt identisch und alle Dateieinträge im erfolgreichen Installer-Compilerlauf enthalten. Kein `runtime`-Motor im neuen Paket; der einzige mitgelieferte Motor liegt unter `tools/otr-can`. **847 Quelldateien** im Quellmanifest geprüft; alle versionierten CAN-/Spotlight-Dateien bytegleich mit den angegebenen Commits. Beide äußeren ZIPs und das enthaltene Motor-Quellarchiv bestehen die CRC-Prüfung. Der zusätzliche unveränderte Motor-Quell-ZIP stimmt mit dem eingebetteten Archiv überein. `SHA256SUMS.txt` deckt die fünf folgenden Auslieferungs-/Manifestdateien ab.

| Datei | Bytes | SHA-256 |
| --- | ---: | --- |
| CutAssistantNext-1.0.3-Build54-source.zip | 6523698 | `D715CDE32AEB9187F9A71CC350E5E5828B15496BF6DE8B18839C05C475CBA155` |
| CutAssistantNext-1.0.3-Build54-win-x64.zip | 25734653 | `DFAA7BAABDCE78F20AD547BBEA7B37AABBB63CEB6E0CD9824C41407C1A9EDFB5` |
| CutAssistantNext-1.0.3-Build54-win-x64-Setup.exe | 20930966 | `EB98B334ECC8C45E8CFFEC8F7FE3F29F95892A5FAA2C06686EC3292FA01D61B6` |
| otr-can-0.1.6-audio-diag.1-source.zip | 127908 | `983C96E364E02A0F6B87BA1C6034995295E5A5E555C28CFB50BD48FB2CFAC504` |
| PUBLISH-FILES.sha256 | 34191 | `79010B8D50183F6ACF058D4BC5B035208A3F7C6585B7B14DB78378AEB10D12E4` |

Die vollständig entpackte ZIP wurde tatsächlich aus einem Ordner **mit Leerzeichen** gestartet, außerdem die EXE im finalen Publish-Ordner. Titel `Cut Assistant Next · 1.0.3 · Build 54`, regulärer Produktname und Dateiversion `1.0.3.54` bestätigt. Persönliche Einstellungen vor/nach beiden Starts unverändert; nur die selbst erzeugten Startprüfprozesse vor dem Schreiben von Schließeinstellungen beendet. Reguläres Schließen separat in isolierten Fensterprozessen geprüft. Setup-Datei-/Produktversion ebenfalls `1.0.3.54`; Setup nicht installiert.

Bestandsschutz erneut geprüft: **4473 ursprüngliche geschützte Dateien plus 269 Build-53-Paketdateien unverändert**. HEADs, Git-Status und lokale Zähler der Ausgangscheckouts einschließlich Build-53-Worktree unverändert. Neuer Zähler nur im Build-54-Worktree auf 54. Keine Nutzeränderung überschrieben, kein Push/Merge/Tag/Release und keine automatische Installation.

Nachweise unter `.build/proof/`: `PACKAGE-VERIFICATION.json`, `PRESERVATION-FINAL.json`, `APPHOST-PUBLISH.json`, `APPHOST-FINAL.json`, `SETUP-METADATA.json`, finale TRX in `final-tests-metadata/results/`, `final-native/`, `final-audio-synthetic/` und `.run.json`. Die Build-/Test-/Publish-/Installerlogs des regulären Paketlaufs liegen im neuen Paketordner unter `logs/`; Quell- und Dateinachweise stehen in Quellarchiv, `RELEASE-PROVENANCE.json` und `PUBLISH-FILES.sha256`.

**Bereit zur persönlichen Prüfung und hier angehalten.** Build 53 ist persönlich bestätigt; für Build 54 sind ein kurzer regulärer Schnitt und der gewünschte AAC-Reparaturweg mit Motorpfad `tools\otr-can` noch persönlich zu bestätigen. Eine neue VM-Installation/Deinstallation wurde hier nicht vorgenommen.
