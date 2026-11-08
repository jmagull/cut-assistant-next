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
