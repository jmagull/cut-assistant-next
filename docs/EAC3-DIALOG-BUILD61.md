# Cut Assistant Next Build 61 – EAC3-Dialogkorrektur vom 18.11.2026

Die ausdrückliche otr-can-Fehlermeldung Unsupported audio codec eac3 wird anhand einer vollständigen Fehlerzeile einschließlich verschachtelter Werkzeugfehler erkannt. Andere Codecs oder lediglich ähnliche Namen werden nicht als EAC3 eingestuft.

Der bestehende Ausweichdialog zeigt den Titel **otr-can – EAC3-Tonspur erkannt** und die vom Anwender vorgegebene Meldung:

> Eine EAC3-Tonspur wurde erkannt. Der verwendete otr-can-Schnittmotor unterstützt dieses Audioformat derzeit noch nicht.
>
> Möchtest du den Film stattdessen mit MP4Box schneiden?

Es stehen **Mit MP4Box schneiden** und **Abbrechen** zur Verfügung. Abbrechen bleibt die Voreinstellung. Ohne verfügbares MP4Box ist dessen Schaltfläche gesperrt; die Verfügbarkeitsprüfung und die serverunabhängige Werkzeugkonfiguration bleiben erhalten. Der bisherige vollständige Neustart mit unverändertem Auftrag erfolgt nur nach ausdrücklicher Zustimmung.

Bei dieser eindeutig nicht unterstützten Codec-Konstellation entfallen die nicht passenden AAC-Reparatur-/Spur-Abwahl-Assessments. Es wird keine neue Medienvorprüfung eingeführt. Andere Audiofehler verwenden weiterhin die bestehenden Assessments, Dialoge und Entscheidungen. Der Motor, die Auswahl der Tonspuren, Schnittgrenzen, Ausgabeparameter, allgemeine Motor-Empfehlung und 40-ms-Prüfung werden nicht geändert. Keine EAC3-Dekodierung, -Reparatur, stille Formatwandlung oder automatische Spurentfernung.

MP4Box ist kein allgemeines Reparaturverfahren für beschädigte Tonspuren. Die EAC3-Unterstützung von otr-can wurde nicht erweitert.

## Gezielte Regressionen

Neue Tests prüfen vollständige Fehlerzeilen, CRLF/Whitespace, verschachtelte Fehler, ähnliche Codec-Namen, AAC+EAC3, exakten WPF-Text, ausschließlich zwei sichtbare Entscheidungen, Abbrechen als Standard, fehlendes MP4Box und die Bestätigung mit identischem Schnittauftrag. Andere Audiofehler müssen ihre bisherigen Recovery-Assessments weiter aufrufen. Die vollständigen CAN-/Spotlight-Ergebnisse und die neue Paketprüfung stehen im abschließenden Paketbericht vom 18.11.2026; frühere Zahlen werden nicht als neuer Lauf übernommen.

## Persönliche Referenzen und offene Abnahme

Die acht Realfilmschnitte vom 17.11.2026 bleiben bestätigt. Am 18.11.2026 sind die Suche nach einer HQ-Cutlist bei geladener HD-Aufnahme, die Größenwarnung, die bewusste Übernahme und der MP4Box-Schnitt der Wetterfee persönlich bestanden. Es liegt dafür kein separates MP4Box-Protokoll vor. Die neue Dialog- und Paketfassung ist damit noch nicht persönlich abgenommen.

Die Diagnose DIAGNOSE-BUILD61-EAC3-20261118.md dokumentiert den vorherigen unveränderten Kandidaten. Ihre Feststellung zum damaligen generischen Dialog ist historisch; dieser Auftrag ersetzt ausschließlich jene EAC3-Darstellung. Ihre Codec-/Empfehlungs-/Rasterbefunde bleiben bestehen.

## Paketierung

Neue vollständige Pakete in einer datierten separaten Kandidatenablage, Produktversion 1.0.3, Dateiversion 1.0.3.61. Frühere Pakete bleiben unverändert. Paketbau entfernt PDB-Dateien aus dem Programmordner, enthält README und aktuelle Anwenderhinweise und verwendet weiterhin den unveränderten Motor unter tools/otr-can/otr_can.exe. Die .NET-Desktop-Runtime und externen Werkzeuge bleiben separat erforderlich.

Keine Veröffentlichung, kein Push oder Merge nach main und keine automatische Installation auf dem Arbeitsrechner. Setup-/Upgrade-/Deinstallationsprüfung, Portable-Nutzung und der neue EAC3-Dialog werden für die persönliche Windows-11-Abnahme vorbereitet.
