# Cut Assistant Next 1.0.3 Build 53 – Prüfbericht

Stand **08.11.2026**. Gemeinsame lokale Prüffassung im kanonischen Repository, Branch **codex/integration-build53**. Basis **622454f37a2688d3d09abb3f2431c55fd304c94f** (Build 52); Fensterpersistenz aus **185e8b350ea0fb5d76f19fefb10bcf220b559203**, Referenzzweig zuletzt **70530046ac4127744251c55d0c30106ce4c1aa86** (Build 50). Der genaue paketierte Integrationscommit wird im Quell-/Paketnachweis und im Abschluss genannt.

## Bestandsschutz und Nummer

Die Bestandsprüfung vor Änderungen fand bekannte Zähler 48, 50, 51 und 52 sowie keine belegte 53 in den erfassten Paketen/Entwicklungsrefs. Der neue isolierte Worktree wurde im ursprünglichen Repository auf Build 52 erstellt und dort einmal nummerierend auf **53** gebaut. Nachfolgende Kontrollbuilds setzen 53 ausdrücklich. main und dessen Zähler 48 bleiben unverändert.

Vorheriger Bestand unter `C:/Users/joerg/Documents/GitHub/cut-assistant-next/.build/build53-audit/BEFORE.json` und `BESTAND.txt` erfasst; **4473 geschützte Dateien** umfassen Nutzeränderungen, Freeze-Quellen, lokale Zähler und relevante alte Paketsätze. Kein Stash, Zurücksetzen, Überschreiben der alten Builds oder Ändern ihrer Branchspitzen. Die alten Funktions-Checkouts werden ausschließlich als Quellen genutzt. Einziger neuer maßgeblicher Entwicklungsstand ist der Integrationsbranch im kanonischen Repository.

## Übernommene Änderungen

- 29 geprüfte Fensterfälle und dieselbe Spotlight-Geometrie-/Persistenzimplementierung aus Build 50. Direkte Bereichsübernahme und bestehende Spotlight-Logik aus Build 52 erhalten.
- Audio-Kerndienste aus Build 52 unverändert. Resolver und sein Factory-Aufruf verwenden den passenden mitgelieferten Motor unabhängig von einer früheren Namenskennung. Der Reparaturdialog ist auch im regulär benannten Produkt aktiviert.
- Benutzerdefinierte gespeicherte Motorpfade werden nicht überschrieben; der Indexerpfad bleibt erhalten. Fehlt der Paketmotor, kein stiller Rückfall auf eine alte EXE.
- Regulärer Produktname **Cut Assistant Next** in Titel, Dateibeschreibung und Produktmetadaten; **1.0.3 / Build 53 / Dateiversion 1.0.3.53**. Installer-Datei-/Produktversion ebenfalls 1.0.3.53. Keine Funktionssuffixe an den CAN-Paketnamen. Version.props enthält 53 als Fallback für den Quellstand.
- Keine neue Produktionsabhängigkeit. Core, Media, Cutlists, AAC-Reparatur, Spur-Abwahl und deren Sicherheitsprüfungen nicht durch alte Gesamtdateien ersetzt oder verändert. [Bestands- und Methodenanalyse](INTEGRATION-BUILD53-20261108.md).

## Tatsächlich erneut ausgeführte Prüfungen

| Prüfung | Bestanden | Fehler |
| --- | ---: | ---: |
| Vollständige CAN-Suite | **1855** | **0** |
| CAN nochmals mit finalen Paket-DLLs | **1855** | **0** |
| Spotlight-Suiten | **78** | **0** |
| Spotlight nochmals mit finalen Paket-DLLs | **78** | **0** |
| Neue/übernommene Fensterfälle in CAN-Suite | **29** | **0** |
| Native Spotlight-Integration | **32** | **0** |
| Kontrollplayer, sechs native Prozesse | **32** | **0** |
| Getrennte Fensterprozesse | **4** | **0** |
| Native synthetische Audiofälle | **20** | **0** |
| Vollständiger echter NCIS-Fall | **1** | **0** |
| Motor-Paketvalidierung | **13** | **0** |
| Unveränderte Rust-Quellen: Normalmodus | **63** | **0** |
| Unveränderte Rust-Quellen: AAC-Feature | **78** | **0** |

Keine übersprungenen Suite-Tests. Wiederholungen sind keine zusätzlichen unterschiedlichen Fälle. CAN-Verteilung: Core 200, Cutlists 80, Media 390, App 1185. 1825 Fälle aus Build 52 plus 29 Fensterfälle und ein zusätzlicher Produktnamens-/Metadatenfall. Die bisherigen Resolverfälle wurden auf die beabsichtigte reguläre Motorauflösung angepasst, keine Audio-Verhaltenstests entfernt.

Release-Build und Publish ohne Warnungen oder Fehler; geänderte C#-Dateien formatiert, Git-Diff geprüft. Die Schlusswiederholung enthält die endgültigen Produktmetadaten. Die vier Fenster-/Testdateien stimmen exakt mit der geprüften Build-50-Implementierung überein. Native Helfer laden dieselben Produkt-DLLs wie die bereitgestellte Anwendung.

Die 20 Audiofälle decken gesundes AAC, mehrere gesunde Tonspuren, Einzel-/Mehrfachlücken, außerhalb der Keeps liegende Lücken, Schnittgrenzen, nicht unterstützte/größere Lücken, fehlendes Audioende, ausdrückliche Ablehnung, MP4Box-Auswahl, gescheiterte Reparatur mit erneuter Entscheidung, laufenden Abbruch und Tonspur-Abwahl mit erhaltener Auswahl ab. Vorhandene Ausgaben und Originale bleiben erhalten; keine automatische Reparatur oder automatische MP4Box-Fortsetzung.

Acht erfolgreiche synthetische normale/Reparatur-/Spur-Abwahl-Ausgaben zusätzlich bytegleich zu Build-52-Referenzen. Ein ergänzender Ganzdateivergleich von MP4Box-Ausgaben war wegen verschiedener Container-Erstellungszeiten nicht bytegleich; anschließend **574 Medienpakete einschließlich Payload-Hashes, PTS/DTS, Dauern und Streamkonfigurationen identisch** geprüft. Keine Prüftoleranz oder Produktlogik geändert.

## Echter NCIS-Nachweis mit den finalen Dateien

Die lokal vorhandene Aufnahme **NCIS Sydney – Breathless, S02E07** wurde über dieselben drei Behaltebereiche vollständig verarbeitet. Strenger normaler Erstversuch bricht wie vorgesehen ab; Vorprüfung weist **512 fehlende Samples** nach. Der native Prüfer simuliert genau eine ausdrückliche Reparaturentscheidung. Keine MP4Box-Ausführung im Reparaturfall. Danach vollständiger Neustart und unabhängige Bild-, Ton- und Containerkontrolle.

Bestätigt: **60198 Videobilder / 25 = 115580160 Audiosamples / 48000 = 2407,920 s = 00:40:07.920**. **115579648 Original-Samplevektoren + 512 positionsgenau ergänzte Stillevektoren**. Quelle vor/nach Verarbeitung per SHA-256 unverändert. Keine pauschale Sampletoleranz, keine Dateinamens-Sonderbehandlung.

Fertige Ausgabe bytegleich zum bereits persönlich geprüften Build-51/52-Referenzfilm:
`90948259C6823EDC23C1E4AF5B6778E5A7BA654C16109CAB9F226D020A3FEA0A`

Der finale Prüflauf bindet die tatsächlichen App-/Motor-Hashes:

- App-DLL: `71BFE47B143C501D0BC235B483A0B8B809FF3C653C0E31F826592A7F957307D6`
- Motor-EXE: `4146EE12F81BB7A4FAEF2D4F25D742A17649959BE8C36491169055F5CF9FA4D1`
- Quellmotor unverändert aus **1979a7f897b95d3fab4020129b8b5e634ea569ac**, Feature `experimental-aac-gap`, gesperrter Offline-Release-Build. Keine Bitidentität zur früheren Motor-EXE behauptet.
- Laufzeit des vollständigen finalen Prüflaufs: **784.422 Sekunden**. Eigener Windows-Job mit **2 GiB Job-Speicherlimit und 25 % CPU-Obergrenze** einschließlich Kindprozessen.

Ein vorangegangener vollständiger Lauf vor der abschließenden Korrektur der Produktmetadaten bestand ebenfalls und bleibt getrennt erhalten; maßgeblich sind die oben gebundenen finalen Dateien. Original und persönliche Cutlist wurden nicht verändert. Kein neuer echter John-Wick-Langlauf; Spur-Abwahl in der synthetischen Mehrspurmatrix erneut geprüft.

## Start und Fenster

Startpfad:
**`C:\Users\joerg\Documents\GitHub\cut-assistant-next\artifacts\release-1.0.3-build53\publish\CutAssistantNext.App.exe`**

Die lokal gebaute EXE startet mit **Cut Assistant Next · 1.0.3 · Build 53**, regulärem Produktnamen und korrekter Dateiversion. Der eigene Startprüfprozess wird vor dem Schreiben persönlicher Schließ-Einstellungen beendet; bestehende Benutzereinstellungen bleiben per Hash erhalten. Reguläres Schließen wird zusätzlich in isolierten nativen CAN-/Spotlight-Fensterprozessen geprüft. Keine Installation und kein Austausch der weiterhin installierten Anwendung.

Zwei tatsächlich verfügbare Monitor-Arbeitsflächen verwendet, Wiederherstellung auf Monitor 2 über getrennte Prozesse bei **1200×850 / X=1960 / Y=40**, einschließlich Maximierung und normalen Rückkehrmaßen. Negative Koordinaten, fehlender/dritter Monitor, kleine Auflösung und DPI-Wechsel simuliert; keine physische Monitorabkopplung oder globale Windows-Skalierungsänderung. Neue persönliche Fenster-/Dialog-/Sicht-/Hörprüfung der gemeinsamen Fassung bleibt offen.

## Persönliche Prüfung und Grenzen

Bitte die neue Anwendung aus dem vollständigen Ordner starten, Spotlight verschieben/maximieren und nach Schließen/CAN-Neustart prüfen. Beim bekannten NCIS-Fall die angebotene AAC-Reparatur ausdrücklich wählen, den vollständigen Prüfablauf abwarten und Bild/Ton/Übergänge/Drei-Sekunden-Vorschau persönlich kontrollieren. Für einen weiteren Mehrspurfall Tonspur-Abwahl prüfen. Automatisierte Entscheidungen ersetzen keine neue persönliche Dialogabnahme.

Windows 11 x64 bleibt die geprüfte Zielplattform; Windows 10 nicht persönlich validiert. .NET 10 Desktop Runtime x64 sowie eingerichtete FFmpeg-/ffprobe-, MP4Box- und FFMS2-Werkzeuge bleiben extern. Mitgelieferter Motor und vier geprüfte libmpv-DLLs enthalten. Eine saubere VM-/Setup-/Portable-Installation und Deinstallation dieser neuen Fassung werden nicht als durchgeführt ausgegeben.

Keine Veröffentlichung, kein Release-Tag, kein Merge nach main und keine automatische Installation. Die gemeinsame Prüffassung wird nach Abschluss zur persönlichen Prüfung angehalten. Alte Funktions-Checkouts/Freeze-Pakete und stabile 1.0.2 Build 48 bleiben unverändert.

Vorbereitende Prüfhilfen: kopierte Motor-Hinweise hatten zunächst geerbten Schreibschutz; nur die neue Kopie wurde beschreibbar gemacht. Ein zusätzlicher Quellvergleich behandelte noch unversionierte neue Dateien zunächst als fehlend; nach gezieltem Erfassen stimmen sie exakt mit Build 50 überein. Alle maßgeblichen Suite-/nativen Tests bestanden; keine Fehler durch Testentfernung oder gelockerte Produktprüfungen umgangen.

Nachweise im Worktree unter `.build/proof/`: finale TRX in `final-tests-metadata/results/`, `recheck-native.log`, `final-native/`, `final-audio-synthetic/`, `final-ncis/` und `.run.json`, Rust-/Build-/Publishlogs, Produktmetadaten und `PRESERVATION.json`. [Integration](INTEGRATION-BUILD53-20261108.md) · [Bedienung](NUTZERANLEITUNG.md) · [Spotlight-README](SPOTLIGHT-README.md).
