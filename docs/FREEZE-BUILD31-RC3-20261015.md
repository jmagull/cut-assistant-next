# Eingefrorener Paketsatz 0.3.0 · Build 31 · RC3

Stand: 15.10.2026. Nutzerauftrag: **geprüften Stand einfrieren, Setup, ZIP und weitere zugehörige Artefakte erstellen, Commit und Push**. Neue Buildnummer **31**, Version **0.3.0**, Kennung **RC3**. Der öffentliche Build 30 und ältere eingefrorene Pakete bleiben unverändert. Eine GitHub-Release-Veröffentlichung ist ein gesonderter Schritt.

## Fester Funktionsumfang

- Halbierungssuche im Hauptplayer mit unabhängiger Suchweite, Halbierung bei Richtungswechsel und Reset bei bewusster Navigation; tatsächlich analysierte und bestätigte Zielframes.
- Für erkannte AVI-Container Hauptplayer-Halbierungssuche ausgeblendet und gesperrt, Frame-Lupe für Button und Shift+F deaktiviert. Gelber Hinweis erläutert die Einschränkungen. Nach Wechsel zu echtem MP4 erscheinen Suchzeile und Keyframes wieder, Lupe wird verfügbar. Container statt Dateiendung, auch bei `.avi.mp4`.
- OTR-CAN-Vorprüfung nach genau einer FFMS2-Indexierung: eindeutige aufsteigende Zeiten sowie Nullstart und vollständiges 25-/50-fps-CFR-Raster entsprechend Motor 0.1.5. Bei ungeeigneten Quellen verständliche Ablehnung vor dem eigentlichen `cut-can`-Aufruf, kein automatischer Motorwechsel.
- Motor **otr-can 0.1.5**, Quellencommit **2a474629836d52023bf37cbbdb309ccd1c23acf3**, EXE-SHA-256 **FC6ED6568880A995F3D141A9378AA2CE28ADE8DC0E257E93DC972799BB1324A6** unverändert. Geprüfter Vier-DLL-libmpv-Satz und zugehörige Quellen/Relink-Archive unverändert.
- Keine Änderungen am MP4Box-Verfahren, keine automatische Timingreparatur, Bildratenumrechnung oder neue Produktionsabhängigkeit.

## Gesicherte Abnahmen

Entwicklungsstand: **1308/1308 Tests**, 0 übersprungen, Release-Rebuild ohne Warnungen/Fehler. Native Hauptplayersuche und AVI/MP4-Wechsel geprüft; reale Indexvergleiche und Produktionsschnittdienst mit FFMS2 bestätigt. Die genauen Prüfumfänge stehen in [Halbierungssuche](HALBIERUNGSSUCHE-HAUPTFENSTER-20261015.md), [AVI-Lupensperre](AVI-FRAME-LUPE-SPERRE-20261015.md), [Zeitstempelursache](AVI-ZEITSTEMPEL-NO-TURNING-BACK-20261015.md) und [OTR-CAN-Vorprüfung](OTR-CAN-ZEITSTEMPEL-VORPRUEFUNG-20261015.md).

Persönliche Rückmeldungen vom 15.10.2026: Hauptplayersuche in allen Richtungen positiv; große Bilder langsamer. Teufelsbrigade-Original-AVI mit otr-can erfolgreich geschnitten und **Ton/Bild/Übergänge einwandfrei geprüft**. No Turning Back vor Motorstart wegen Bildzeit 257 gesperrt; separater MP4Box-Schnitt technisch erfolgreich, Prüfen/Vorschau bestätigt. Navy CIS vor Motorstart wegen nicht unterstütztem Zeitraster gesperrt. AVI-Suchzeile ausgeblendet und nach MP4-Wechsel wieder sichtbar, mit Screenshots bestätigt. Daraus keine erneute vollständige Qualitätsabnahme jeder Ausgabe ableiten.

## Paketverfahren

Quellcommit gezielt ohne die vier bereits vor Arbeitsbeginn modifizierten Produktions-Lockdateien einfrieren. Quellen aus genau diesem Commit exportieren, einschließlich Herkunft und SHA-256-Manifest. Die vier lokalen Lockdateien werden weder zurückgesetzt noch ins Quellenarchiv übernommen; alle acht lokalen Lockdateien müssen unverändert bleiben.

Im exportierten Quellbaum `tools/package-release.ps1` mit **BuildNumber 31 / RC3** ausführen: gesperrter Restore, Release-Build, Tests, framework-dependent win-x64-Publish, Inno-Setup und Portable-ZIP. Bereits geprüfte passende Motor-/libmpv-Quellenarchive ergänzen. ZIP-Inhalte vollständig gegen Manifest und Herkunft prüfen; Versionen, Lizenzen, native DLLs, tatsächliche WPF-Bindungen und AVI/MP4-Umschaltung gegen das entpackte Paket kontrollieren. Erst danach Pakete und Prüfsummen einfrieren und Commit/Push abschließen.

Pakete: `artifacts/release-build31-RC3`; Quellen: `artifacts/source-build31-RC3`; lokale Nachweise: `.build/freeze-build31-20261015`. Gebauter Quellcommit und abschließende Ergebnisse werden im [Paketprüfbericht](RELEASE-BUILD31-RC3-RESULT.md) festgehalten; spätere Dokumentationscommits verändern den Quellpin nicht.

## Grenzen

.NET 10 Desktop Runtime x64, externe FFmpeg/ffprobe-, GPAC/MP4Box- und FFMS2-Werkzeuge sowie bei otr-can VC++ v14 x64 bleiben erforderlich. AVI bleibt experimentell. Die Vorprüfung erfasst bekannte Zeitstrukturgrenzen; Codec-, Audio-, Decoder- und weitere Motorfehler sind nicht allgemein ausgeschlossen. Bekannter MP4Box-Befund mit zwei gleichartigen AAC-Spuren bleibt bestehen. Nach Paketierung persönlich bestätigt: Setup in der VM, Deinstallation, Portable-ZIP in der VM und Videoschnitt mit MP4Box/otr-can. Keine vollständige frische Film-/Tonspurabnahme sämtlicher Ausgaben daraus ableiten. Keine neue Byte-Reproduzierbarkeit der unveränderten Motor-EXE behaupten.
## Abschluss

Quellcommit **de8ec0d598b095d85e4559e7fd83012534959637** und Tag **v0.3.0-build31-rc3** auf origin gesichert. Paketbau und sämtliche technischen Prüfungen bestanden; sechs Archive und Prüfsummen eingefroren. Buildzähler 31, lokale Lockänderungen und Altartefakte erhalten. [Abschließender Nachweis](RELEASE-BUILD31-RC3-RESULT.md). Keine neue öffentliche Release-Veröffentlichung. Persönliche VM-/Setup-/Portable-/Deinstallationsabnahme und Videoschnitt mit beiden Motoren anschließend am 15.10.2026 bestätigt.
