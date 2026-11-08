# Cut Assistant Next 1.0.3 Build 53 – kontrollierte Integration

Stand 08.11.2026. Ein maßgeblicher gemeinsamer Entwicklungsstand im kanonischen Repository `C:\Users\joerg\Documents\GitHub\cut-assistant-next`, Branch `codex/integration-build53`. Isolierter Git-Worktree unter `.build/worktrees/build53`; `main` und dessen lokale Nutzeränderungen bleiben erhalten. Keine weitere CAN-Entwicklung in Funktions-Checkouts.

## Bestandsaufnahme vor der Integration

- Kanonisches main: **22f4042f825a2db7c392a1f465d05eda3cacd54a**, lokaler Zähler 48; vorhandene Dokumentations- und Produktions-Lockänderungen sowie ein unversionierter Paketbericht werden nicht angefasst.
- Maßgebliche funktionale Basis: **622454f37a2688d3d09abb3f2431c55fd304c94f**, eingefrorener Build 52; Audio-Checkout sauber. AAC-Reparatur, Tonspur-Abwahl, MP4Box-Alternative und bestehende Spotlight-Integration vorhanden.
- Fensterreferenz: **70530046ac4127744251c55d0c30106ce4c1aa86**, Spotlight-Zweig sauber, Build 50. Die Produktänderung zur Fensterpersistenz stammt aus **185e8b350ea0fb5d76f19fefb10bcf220b559203**; spätere Commits ergänzen nur Nachweise.
- Gemeinsamer Vorfahr: **20176737ed0b11eebd45e1b70bf496f8de7d5830**. Gemeinsame Dateien vor Übernahme verglichen. Keine Notwendigkeit eines ungesicherten Gesamtmerges.
- Bekannte Zähler 48/50/51/52, keine Build-53-Pakete oder entsprechenden Entwicklungsrefs vorhanden. **53** war frei und wurde im isolierten Worktree genau einmal durch den nummerierenden Build erzeugt. Der main-Zähler bleibt 48.
- Schutzmanifest vor der Änderung: **4473 Dateien** einschließlich alter Quellen, lokaler Nutzerdateien, Zähler und relevanter Paketsätze. Nachweis im kanonischen `.build/build53-audit/`.

## Gezielter Änderungsumfang

Aus Build 50 ausschließlich `SpotlightWindowPersistence`, `SpotlightWindowSettings`, optionale Positions-/DPI-Felder im bestehenden `WindowSettingsStore`, die Anbindung in `MainWindow.Spotlight` und die zugehörigen 29 Fensterprüfungen übernommen. Der native Spotlight-Prüfer erhält wie zuvor einen isolierten Store. Die vorhandene Spotlight-Fachlogik aus Build 52 bleibt bestehen; gemeinsame Methoden wurden gezielt ergänzt.

Build 52 schaltete den Audio-Reparaturdialog und die mitgelieferte Motorwahl über eine besondere Entwicklungskennung ein. Diese Namenskopplung entfällt: Die reguläre Anwendung bietet dieselben bestätigten Entscheidungen und verwendet ihren passenden Motor aus `runtime/otr-can`. Eine fehlende Motor-Datei bricht verständlich ab; es gibt keinen stillen Rückfall auf einen alten konfigurierten Motor. Gespeicherte Werkzeugpfade und der Indexer werden nicht verändert. Testfälle prüfen auch alte oder leere gespeicherte Motorpfade.

Die fachlichen Audio-Dienste, AAC-Reparatur, Spur-Abwahl, strengen Prüfungen und Media-/Core-/Cutlist-Bibliotheken bleiben aus Build 52 unverändert. Keine allgemeine Sampletoleranz, keine automatische Reparatur, keine Dateinamens-Sonderregel. Nur der vorhandene Resolver und sein Factory-Aufruf sowie die UI-Aktivierung werden von der früheren Namenskennung entkoppelt.

Anwendung, Installer und Pakete verwenden ausschließlich den regulären Produktnamen und **1.0.3 / Build 53 / Dateiversion 1.0.3.53**, ohne Funktionssuffix. Der Source-Build hat 53 als Fallbacknummer; ein expliziter Buildparameter bleibt möglich. Die externe Komponente Spotlight-CAN und der Motor behalten ihre eigenen Bibliotheks-/Werkzeugnamen und Versionen.

Der passende Motor **0.1.6-audio-diag.1**, Feature `experimental-aac-gap`, wird aus dem unveränderten archivierten Quellcommit **1979a7f897b95d3fab4020129b8b5e634ea569ac** mit gesperrten Offline-Abhängigkeiten neu gebaut. Kein alter CAN-DLL-Satz und kein unkontrollierter EXE-Austausch. Die neue Motor-EXE hat einen eigenen dokumentierten Hash; Bitidentität zur alten EXE wird nicht behauptet.

## Verifikation und persönliche Prüfung

Ergebnisse der erneuten Tests, der native NCIS-Nachweis und der genaue Startpfad werden nach Abschluss im [Prüfbericht](PRUEFBERICHT-BUILD53-20261108.md) festgehalten. Frühere Zahlen sind nur Referenzen, kein neuer Testnachweis. Entscheidungen im automatisierten Audio-Prüfer werden ausdrücklich simuliert; eine neue persönliche Dialog-/Sicht-/Hörprüfung der gemeinsamen Anwendung bleibt erforderlich.

Keine automatische Installation, kein Austausch der installierten Anwendung, kein Merge nach main, kein Release-Tag und kein GitHub-Release. Alte Builds 50/51/52 und die stabile 1.0.2 Build 48 bleiben erhalten. Nach Bereitstellung der lokalen Prüffassung anhalten.
