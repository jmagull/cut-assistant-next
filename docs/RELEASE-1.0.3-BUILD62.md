# Cut Assistant Next 1.0.3 Build 62

Dateiversion: **1.0.3.62**. Änderungen gegenüber der stabilen Ausgabe Build 61:

## Bedienung

Die vier Schnittschaltflächen sind ausschließlich in **Power-User 1** wie folgt angeordnet:

| Linke Spalte | Rechte Spalte |
| --- | --- |
| Videoanfang | Videoende |
| Schnittanfang setzen | Schnittende setzen |

Standardansicht und Power-User 2 behalten ihre Anordnung. Die vorhandenen Schaltflächen, dynamischen Beschriftungen und der Rot-/Grün-Modus bleiben erhalten.

Vier weitere Schnittaktionen sind frei mit Tastenkürzeln belegbar:

| Aktion | Werkseinstellung |
| --- | --- |
| Schnittanfang setzen | A |
| Schnittende setzen | E |
| Videoanfang setzen | Umschalt+A |
| Videoende setzen | Umschalt+E |

Die Werkseinstellungen stehen in der Spalte „Standard“. Individuelle Belegungen bleiben beim Upgrade erhalten; bereits belegte Kombinationen werden nicht verdrängt. Bei einem solchen Konflikt kann die neue Aktion zunächst unbelegt bleiben. „Standard“ und „Alle zurücksetzen“ stellen die Werkseinstellungen über die vorhandene Shortcut-Verwaltung wieder her.

Die Aktionen funktionieren in allen drei Ansichten und beiden Schnittmodi. Texteingaben lösen keine Schnittaktion aus; Gedrückthalten verursacht keine unbeabsichtigten mehrfachen Schnittaktionen. Bestehende Navigation und ihre Wiederholparameter bleiben erhalten.

## Temporärer Arbeitsordner und Speicheranzeige

Unter **Einstellungen → Standardordner → Temporärer Arbeitsordner** stehen für MP4Box und otr-can gemeinsam drei Varianten zur Wahl:

- Ausgabeordner – weiterhin die anfängliche Standardeinstellung.
- Windows-TEMP-Verzeichnis.
- Benutzerdefinierter Arbeitsordner.

Die dauerhaft gespeicherte Auswahl zeigt den tatsächlich verwendeten Pfad, die Datenträgerkapazität, freien Platz in GiB und die belegte Ordnergröße, soweit zuverlässig ermittelbar. „Aktualisieren“ liest die Werte erneut ein. Beim Wechsel der Ordnerart wird die Anzeige aktualisiert; die Ermittlung der Ordnergröße erfolgt ohne blockierende Verzeichnisberechnung auf der Oberfläche. Nicht verfügbare Werte werden verständlich gekennzeichnet.

Vor Schnittbeginn werden Erreichbarkeit, Schreibmöglichkeit und freier Platz von Arbeits- und Zielverzeichnis geprüft. Die Bedarfsschätzung berücksichtigt Segmentdateien und Zusammenfügen. Messwerte und Schätzungen sind getrennt; die Prüfung gibt keine Garantie für den tatsächlich benötigten Platz.

Die Übernahme ins Ziel schützt vorhandene Zieldateien und Originale. Eigene temporäre Auftragsdateien werden nach Erfolg und kontrolliertem Abbruch bereinigt; Bereinigungsfehler werden protokolliert. Fremde TEMP-Dateien werden nicht automatisch gelöscht.

## Schnittprotokoll und Diagnose

Das Protokoll bietet scrollbare vollständige Ausgabe, deutlich sichtbare Abbruchmeldungen und weiterhin „In Zwischenablage“. Vollständige technische Meldungen bleiben erhalten.

Tatsächliche Arbeits- und Zielverzeichnisse, Speicherplatzmessungen, Bedarfsschätzungen, Warnungen sowie technische Schreib-, Kopier- und Bereinigungsfehler sind nachvollziehbar dokumentiert. Festgestellte Ursachen werden von Schätzungen und Vermutungen getrennt. Ein Schreibfehler wird nicht ohne Nachweis als Audiofehler oder beschädigte Quelle ausgegeben.

Die bestehenden Schnittmotoren und Schnittalgorithmen wurden durch diese Änderungen fachlich nicht erweitert. Eine neue EAC3-Verarbeitung oder Audioreparatur durch MP4Box wird nicht zugesagt.

## Qualitätssicherung und Pakete

- **2.365/2.365 automatisierte Tests bestanden**, keine Fehler und keine übersprungenen Tests.
- **9/9 native Original-QA-Prüfungen bestanden**.
- Release-Build: **0 Fehler, 0 Warnungen**.
- Persönliche Gesamtabnahme auf Windows-11-Arbeitsrechner und separater Windows-11-VM am **21.11.2026** abgeschlossen: Setup, Deinstallation, Portable, individuelle Tastenkürzel, TEMP-Anzeige, MP4Box/otr-can und realer AAC-Abbruch mit ausdrücklich bestätigtem MP4Box-Neustart.

Die bereits geprüften Setup-, Portable- und Quellpakete vom 20.11.2026 werden bytegleich beibehalten. Ein echter Transfer zwischen zwei unterschiedlichen physischen Laufwerken wurde nicht gesondert persönlich getestet.

Die dynamische Anzeige der jeweils individuell zugewiesenen Tastenkürzel in den vier Schnitt-Tooltips ist für **Build 63 ff.** vorgemerkt und nicht Bestandteil von Build 62.

## Downloads und Prüfsummen

- [Windows-Setup](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build62/CutAssistantNext-1.0.3-Build62-win-x64-Setup.exe)
- [Portable-ZIP](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build62/CutAssistantNext-1.0.3-Build62-win-x64.zip)
- [Quellarchiv](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build62/CutAssistantNext-1.0.3-Build62-source.zip)
- [SHA-256-Prüfsummen](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build62/SHA256SUMS.txt)
- [Paketbericht und Links zu den begleitenden libmpv- und otr-can-Quellen](https://github.com/jmagull/cut-assistant-next/releases/download/v1.0.3-build62/PAKETBERICHT.md)
