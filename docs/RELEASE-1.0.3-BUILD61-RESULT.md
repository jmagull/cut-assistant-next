# Cut Assistant Next 1.0.3 Build 61 – Paket- und Prüfberichte

## Abschlussstand vom 18.11.2026

Die persönliche Funktions- und Installationsabnahme ist abgeschlossen, einschließlich der EAC3- und Tastaturkorrekturen. Der geprüfte Programmstand liegt auf codex/build61-shortcuts im Commit 04a0dd609e30c545c88a40bcd9ba5ca30a25022b. Die abschließende Dokumentationsaktualisierung wird lokal committet und in die Releasepakete übernommen. Der finale Paketquellcommit, die neu ausgeführten Tests, Prüfsummen und Paketgrößen stehen in PAKETBERICHT.md und PACKAGE-MANIFEST.json unter artifacts/release-1.0.3-build61-final-20261118. [Persönliche Abnahme](ABNAHME-BUILD61-20261118.md). Veröffentlichung bedarf einer gesonderten Freigabe.

## Historischer Kandidatenbericht vom 17.11.2026

Stand: 17.11.2026. **Releasekandidat technisch erstellt. Keine Freigabe zur Veröffentlichung.** Setup und ZIP bleiben zur persönlichen Abnahme unverändert erhalten. Es erfolgten kein Merge nach main, kein Push, kein Tag, kein Upload, kein GitHub-Release und keine Installation beim Anwender. Die öffentliche stabile Ausgabe bleibt Build 60.

## Herkunft und Umfang

- Kandidatenzweig: codex/release-build61-candidate.
- Paketquellcommit: **3d78a2b8490544536ef2713a58d5228f78e3e05e**, ohne ungesicherte Änderungen. Produktcode zuletzt aus 0d4875a.
- codex/build61, codex/navigation-review-focus und codex/continuous-schnittshuffle sind vollständig als Vorfahren enthalten. Es war kein konfliktbehaftetes Zusammenführen nötig.
- Enthalten: konfigurierte Tastenkürzel, gespeicherte Wiederholzeiten, fortlaufender Shuffle, Fokuskorrektur, blaue K-Markierungen, Empfehlung, Qualitätsalternativen, Wiederaufruf der Schnittprüfung und freie Namen in allen drei Dialogen. Bestehende Rot-/Grün-, Cutlist- und Bewertungsfunktionen bleiben enthalten.
- Spotlight-Komponente: 75ee1475adf20a8c8231ab2989eab7eff37e6b09. SOURCE-SNAPSHOT.json und das vollständige Quellarchiv dokumentieren CAN, Spotlight und Motorquellen.
- Version und Dateiversion: **1.0.3 / 1.0.3.61**. Explizite Buildnummer 61; das nummernerhöhende build.ps1 wurde deshalb nicht verwendet. Keine neue Produktfunktion oder Architekturänderung für die Paketierung.

## Pakete und Integrität

Ablage: C:/Users/joerg/Documents/GitHub/cut-assistant-next/artifacts/release-1.0.3-build61-candidate-20261117-rc1

| Datei | Größe in Bytes | SHA-256 |
| --- | ---: | --- |
| CutAssistantNext-1.0.3-Build61-win-x64-Setup.exe | 20872795 | 4577E66372EED3B9BA9AB7186B93D6395F5E5019392D1FCF6182F12B449BBC79 |
| CutAssistantNext-1.0.3-Build61-win-x64.zip | 25053812 | 90AB48891C2D99DE3C1959F2CA118FC8745662C4C42CFFB4D9A0483A806CFA11 |

PUBLISH-FILES.sha256 beschreibt alle **259** Dateien des Programmordners. Die ZIP-Einträge und sämtliche erneut entpackten Dateien stimmen exakt damit überein. Das Setup-Kompilierprotokoll bestätigt dieselben 259 Eingabedateien. Dies prüft die Kompilierbasis, ersetzt aber keine tatsächliche Installation oder Deinstallation.

Vollständigkeit: vier gegen ihre Referenz geprüfte libmpv-DLLs, otr-can einschließlich Lizenzhinweisen und Quellen unter **tools/otr-can/otr_can.exe**. Das Motorpaket enthält 105 Dateien einschließlich seines Manifests; dessen 104 Einträge wurden per SHA-256 geprüft. Keine PDB-Dateien, persönlichen Konfigurationen, Videotestdaten oder Entwicklungsverzeichnisse in der Portable-ZIP. Das vollständige Quellarchiv umfasst 921 Dateien einschließlich Manifest. Quell- und Relinkarchive für libmpv sowie passende Motorquellen liegen separat bei.

Setup und ZIP wurden nach den Funktionstests erneut gehasht: unverändert. Die geprüften früheren Build-60-Pakete und die Build-61-Namensprüffassung blieben ebenfalls unverändert. PACKAGE-MANIFEST.json und SHA256SUMS.txt enthalten die maschinenlesbaren Nachweise.

## Erneut ausgeführte automatisierte Tests

| Suite | Gesamt | Bestanden | Fehlgeschlagen | Übersprungen |
| --- | ---: | ---: | ---: | ---: |
| CAN Core | 200 | 200 | 0 | 0 |
| CAN Media | 402 | 402 | 0 | 0 |
| CAN App | 1485 | 1485 | 0 | 0 |
| CAN Cutlists | 80 | 80 | 0 | 0 |
| CAN gesamt | **2167** | **2167** | **0** | **0** |
| Spotlight Core/Media | 59 | 59 | 0 | 0 |
| Spotlight App | 19 | 19 | 0 | 0 |
| Beide Testsuiten gesamt | **2245** | **2245** | **0** | **0** |

Release-Build: **0 Warnungen, 0 Fehler**, .NET SDK 10.0.303. Der feste Quellsnapshot wurde mit den vorhandenen, angehefteten Abhängigkeiten wiederhergestellt, gebaut, getestet und für Windows x64 veröffentlicht. Native Werkzeugtests waren mit den angegebenen EXE-Pfaden aktiviert. Namens-, Empfehlungs-, Timing-, Fokus- und Shuffle-Regressionen liefen in der Gesamtsuite mit. TEST-RESULTS.json und die sechs TRX-Dateien enthalten die tatsächlichen Ergebnisse dieses Auftrags.

Zusätzlich liefen gegen die endgültigen Paketbibliotheken sechs Kontrollplayer-Gruppen, die Spotlight-/Filmende-/Shuffle-Integration, beide synthetischen Schnittmotoren, **20 Audioregressionen** und die Motorpaketprüfungen erfolgreich. Die Audiomatrix bestätigt auch den SHA-256 der verwendeten App-DLL. Diese zusätzlichen Kontrollen werden nicht zur Zahl der Unit-Tests addiert.

## Werkzeug- und Startprüfung

| Werkzeug | Geprüfter Stand | Ergebnis |
| --- | --- | --- |
| libmpv | 0.41.0, enthalten | Vier DLL-Hashes stimmen; native Playerkontrollen bestanden |
| otr-can | 0.1.6-audio-diag.1, enthalten | Version, Fähigkeiten, FFMS2-Aufruf und synthetischer Schnitt bestanden |
| FFmpeg / ffprobe | 12.10.2026, 615b07170e, extern | Streamcopy: 200 sichtbare H.264-Pakete mit richtiger Reihenfolge, Zeitstempeln und Hashwerten |
| Fehlerhafte Vergleichsversion | 11.11.2026, 23c4540053 | Erwartungsgemäß abgelehnt: nur 50 von 200 Paketen |
| GPAC / MP4Box | 26.07-rev0-ga07cbfff-master, extern | Zwei Bereiche mit -add/-cat/-new und zwei getrennten AAC-Spuren bestanden |
| FFMS2 / ffmsindex | Extern, per SHA-256 identifiziert | Indexer erreichbar und beim otr-can-Schnitt ausgeführt; CLI und DLL-Metadaten liefern keine numerische Version |

Beide Schnittmotoren erzeugten aus denselben Testbereichen jeweils eine vier Sekunden lange MP4 mit zwei AAC-Spuren. Gespeicherte externe Werkzeugkonfigurationen blieben im Harness unverändert. TOOL-VERSIONS.json und Pruefnachweise/native/engine-check-and-both-cuts.log enthalten die Einzelheiten.

Die frisch entpackte Portable-Fassung wurde außerhalb ihres Programmordners gestartet. Fenstertitel und EXE-/Setup-Dateiversion bestätigen Build 61. Der Paketmotor ist vorhanden und ausführbar. **19 Einstellungsdateien** vor/nach dem Start verglichen: unverändert. Dies ist eine technische Startprüfung, keine persönliche Sicht-/Hörabnahme.

Windows 11 x64, .NET 10 Desktop Runtime x64 und für otr-can Microsoft Visual C++ v14 Redistributable x64 bleiben erforderlich. FFmpeg, ffprobe, MP4Box und FFMS2 werden entsprechend dem bisherigen Paketkonzept separat konfiguriert. Setup und ZIP enthalten keine vollständige .NET-Runtime.

## Persönliche Realfilmabnahme vom 17.11.2026

| Referenzfilm | MP4Box | otr-can |
| --- | --- | --- |
| Yaiba – Samurai Legend S01E07 | Bestanden | Bestanden |
| Wild Kat – Partymonster S01E01 | Bestanden | Bestanden |
| Die zauberhafte Weihnachtsfee | Bestanden | Bestanden |
| Meine zauberhaften Weihnachtsschuhe | Bestanden | Bestanden |

**8 von 8 bestanden**, jeweils Bild, Ton, Schnittübergänge und Vorschau ohne Auffälligkeiten. Sieben technische Motorprotokolle sind in den früheren Nachweisen dokumentiert. Die Weihnachtsfee mit MP4Box ist persönlich nachbestätigt; ein separates Motorprotokoll wurde nicht übergeben. Es wird keines ergänzt oder erfunden. Diese Prüfungen gelten für die dokumentierten Entwicklungsfassungen unter Windows 11 und werden nicht nachträglich dem neuen Setup zugerechnet. Kein persönlicher Windows-10-Test.

## Offene persönliche Abnahme

1. Eigene Tastenkombinationen: Konflikte, Speicherung, Neustart und alle drei Layouts.
2. Tatsächliche Serversuche nach Qualitätsalternativen samt Kennzeichnung, Warnung und bewusster Übernahme.
3. Fachliche Schlussbewertung der Empfehlung, insbesondere Keyframe-Anfänge bei exklusiven Nicht-Keyframe-Enden. Erfolgreiche Filmschnitte begründen keine allgemeine Genauigkeitsgarantie.
4. Neue freie Namen: tatsächliche Ausgabe beider Motoren, Speicherung/Wiederladen einer Cutlist, Erhalt manueller Änderungen und bewusste Neugenerierung.
5. Frisches Setup, Upgrade, Verknüpfungen, Deinstallation und Portable-Nutzung in einer sauberen Windows-11-VM. Eine solche VM-Abnahme war für diesen Lauf nicht verfügbar. Setupstruktur und gemeinsamer AppId wurden statisch geprüft; keine automatische Installation.

Die bereits bestätigten acht Filmschnitte müssen ohne konkreten Anlass nicht wiederholt werden. Wiederholtempo mit Neustartpersistenz, Kontrollplayer-Fokus und Shuffle sind bereits persönlich bestätigt; die neuen Pakete benötigen ihre eigene Prüfung.

## Befunde und Grenzen der technischen Prüfung

- Vorläufe der isolierten Ausführungsumgebung scheiterten zunächst an fehlenden Windows-Umgebungswerten. Ein Vorlauf hatte dadurch 150 WPF-Fehler bei der Initialisierung der Schriftpfade. Nach Ergänzung von PATH/PATHEXT, Profil- und Windows-Pfaden bestand dieselbe unveränderte Produktsuite vollständig. Die Vorlaufprotokolle bleiben erhalten; die Tabelle oben beschreibt ausschließlich den abschließenden Lauf.
- Der historische native Spotlight-Prototyp erwartete noch den früheren Shuffle (Sprung auf dieselbe ausgewählte Anfangsgrenze; Ausblenden ohne Auswahl). Für die Zusatzprüfung wurde eine separate Harness-Kopie an den fortlaufenden Shuffle angepasst und auf den vollständigen Abschluss des asynchronen UI-Befehls synchronisiert. Fehlende Teilklassen wurden beim ersten Harness-Kopierversuch nachgetragen. Keine Produktkorrektur und kein Testfallentfall; alte Fehlversuche bleiben in den Nachweisen erhalten. Die aktuelle Kopie sowie Skripte liegen unter Pruefnachweise.
- Ein AAC-Reparaturfall scheiterte im tief verschachtelten Arbeitsverzeichnis beim Veröffentlichen der Ausgabe mit Windows-Pfadfehler **os error 3**. Die vollständige 20-Fälle-Matrix bestand im kurzen temporären Verzeichnis. Lange Arbeits-/Ausgabepfade bleiben damit ein dokumentiertes Risiko; keine Änderung am Motor vorgenommen.
- Die geprüften externen Werkzeugstände sind die oben genannten. Der früher geprüfte FFmpeg-Build vom 14.11.2026 wurde in diesem Paketlauf nicht erneut verwendet. Bekannte MP4Box-Keyframe-/Werbeblitzer-Grenzen und experimentelle AVI-/AAC-Reparaturgrenzen bleiben bestehen.

## Aktualisierte Dokumentation und Übergabe

Aktualisiert wurden README.md, docs/README.md, docs/NUTZERANLEITUNG.md, docs/RELEASE-NOTES-1.0.3.md und docs/PRUEFHINWEISE-BUILD61.md. Dazu kommen dieser Bericht, Paketmanifest, Testübersicht, Werkzeugidentitäten und Integritätsnachweise. Dokumentationsänderungen wurden mit git diff --check geprüft. Die eigentlichen Produktänderungen stammen aus den bereits dokumentierten Entwicklungszweigen.

**Nächster Schritt:** die unveränderten Pakete anhand PRUEFHINWEISE.md persönlich abnehmen. Erst eine spätere ausdrückliche Freigabe darf die Veröffentlichung auslösen.
