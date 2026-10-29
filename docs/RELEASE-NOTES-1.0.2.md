# Cut Assistant Next 1.0.2 · Build 46 – eingefrorener Stand

**29.10.2026:** QUICK AND COPY verwendet FFmpeg concat mit `-c copy`. Alle bestätigten kompatiblen AAC-/AC-3-Spurpaare bleiben ohne Neukodierung erhalten. Technische Warnungen, vorhandene PTS-Unregelmäßigkeiten, unterschiedliche H.264-Decoderparameter oder zusätzliche Samples allein brechen QUICK nicht ab. Keine obligatorischen vollständigen PCM-/Video-/Paketprüfungen im QUICK-Weg.

**Meckerecke – technische Hinweise:** eigener Abschnitt im vorhandenen Protokoll mit vorliegenden Diagnoseangaben und sämtlichen FFmpeg-Meldungen. Dafür keine zusätzlichen Vollprüfungen. Ergebnis nennt die Streamcopy und bittet um Kontrolle. Echte Prozess-/Ausgabe-/Dateischutzfehler bleiben Abbruchgründe; Originale, Auswahlbestätigung, Protokoll, Phasenzeiten und Drei-Sekunden-Vorschau erhalten. QUICK-Nahtzeit ausdrücklich angenähert.

**AUDIO-ENTHUSIAST unverändert**, einschließlich strenger Prüfungen, RF64, AAC-Neukodierung und Zeitmessung. Beide Verfahren kopieren Video. 1576 Tests, 27 native QUICK- und 21 bisherige Fälle bestanden; neun Enthusiast-Ausgaben bytegleich. Drei reale Referenzen technisch erfolgreich; zwei eigene MP4Box-QUICK-Läufe mit AAC+AC-3 persönlich auf Bild/Ton/Klebestelle/Drei-Sekunden-Vorschau bestätigt: Furiosa 23,74 s, danach John Wick 16,44 s. [Details](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md).

Dateiversion **1.0.2.46**, otr-can **0.1.6**, libmpv **0.41.0**. Bestätigter Stand eingefroren; Setup/Portable/Quellen erstellt, technisch geprüft und schreibgeschützt. 1576 Pakettests plus 1304 Tests gegen finale DLLs und native Prüfungen positiv; Quellstand danach committet und nach origin/main gepusht. [Paketprüfung](RELEASE-1.0.2-BUILD46-RESULT.md). Veröffentlichung erst nach erneuter Ansage. Build-45-Pakete bleiben erhalten, öffentlich bleibt Build 41. Die folgenden Abschnitte beschreiben den damaligen Build-45-Paketstand und dessen frühere strenge QUICK-Regeln.

## Historisch: Build 45 – eingefrorene lokale Paketfassung

**29.10.2026: Stand auf Nutzerwunsch eingefroren; Setup, Portable und passende Quellen für Build 45 beauftragt. Keine Veröffentlichungsfreigabe.** Öffentlich bleibt CAN 1.0.2 Build 41. CAN-Dateiversion **1.0.2.45**, unveränderter Motor **otr-can 0.1.6**, Wiedergabe **libmpv 0.41.0**, .NET 10/WPF für Windows 11 x64.

## Klarer Hinweis an jedem Tonspurpaar

**„Dieses Tonspurpaar übernehmen (nur bei übereinstimmendem Tonformat)“**, mit Textumbruch. Zusätzlich: Jede Tonspur darf nur einmal zugeordnet werden; nicht benötigte Paare abwählen und Auswahl erneut bestätigen. Hinweis vom Nutzer anhand des Build-45-Screenshots persönlich bestätigt. Ausschließlich Dialogtext/Textumbruch gegenüber Build 44; keine Änderung der Paarvorschläge, Schutzregeln oder Medienverarbeitung.

## Vereinfachtes Klebecenter-Menü

Nur **Klebecenter → Filmteile zusammenfügen …** öffnen. Filmteile, Reihenfolge und Tonspuren direkt in diesem Dialog wählen. Der zusätzliche Eintrag **Tonspur für Filmteile auswählen …** entfällt; der Nutzer hat das vereinfachte Menü persönlich geprüft. Die eigenständige Tonspurkopie bleibt im Code für eine mögliche spätere Werkzeugrubrik erhalten. Beide Klebeverfahren bleiben gegenüber Build 43 unverändert; bestehende Build-43-Pakete bleiben erhalten.

## Zwei ausdrücklich gewählte Klebeverfahren

**QUICK AND COPY** übernimmt passende H.264-Videopakete und die bestätigten AAC-/AC-3-Spurpaare ohne Neukodierung. Eindeutige gemeinsame Spuren werden vorgeschlagen; Zuordnung und entfallende Spuren vor dem Start bestätigen. Furiosa kann AAC und AC-3 gemeinsam behalten; bei John Wick ist nur die gemeinsame AAC-Spur nötig. Fehlende Spuren werden nicht ergänzt. Vollständiger komprimierter Paketvergleich und vollständige Tondekodierung bleiben erforderlich. Eng begrenzte nachgewiesene Audioabweichungen und Zeitkorrekturen werden pro Spur angegeben; keine Zusage einer sampleidentischen Tonwellenform.

**AUDIO-ENTHUSIAST – NEUKODIERUNG** behält das bewährte Build-42-Verfahren: genau eine gewählte Tonspur je Teil, strenge Quelldekodierung, samplegenaue direkte RF64-Zusammensetzung mit vollständigem Nutzdatenvergleich und einmalige durchgehende AAC-LC-Kodierung. Das Bild bleibt unverändert. Unterschiedliche Quellcodecs wie AC-3/AAC sind möglich, wenn Abtastrate und Kanalbelegung passen.

Keine Verfahrensvoreinstellung, kein Enter-Standard und keine automatische Umschaltung bei Fehlern. Beide Originaldateien bleiben erhalten, vorhandene Ausgaben werden nicht überschrieben. Genau zwei kompatible progressive H.264-MP4-Dateien mit 25/50 fps, Ton bei 44100/48000 Hz und unterstützter Kanalbelegung. Nicht nachweisbare Fälle werden abgelehnt.

## Kontrolle und Bedienung

Im Abschlussdialog Anfang, tatsächliche Klebestelle und Ende mit „Prüfen“ kontrollieren; die Vorschau beginnt drei Sekunden vor der Naht. Im Kontrollfenster bei mehreren erhaltenen Spuren unter **Tonspur prüfen** zwischen AAC und AC-3 wechseln; dieselbe Prüfstelle wird erneut abgespielt. Die Auswahl der zu übernehmenden Spuren erfolgt direkt im Zusammenfügen-Dialog.

Phasenzeiten und Gesamtzeit sind sichtbar bzw. im Protokoll enthalten. Nach Erfolg bekannte temporäre Dateien samt Erfolgsprotokoll aufräumen; bei Fehler/Abbruch Diagnose erhalten. Keine neue GPU-Integration oder erhöhte Ressourcengrenzen. Der frühere Furiosa-Systemstillstand bleibt als ungeklärter historischer Befund dokumentiert.

## Prüfstand

Build-45-Entwicklung: **1552 Tests**, Release ohne Warnungen/Fehler, vier native WPF-Hinweis-/Auswahlfälle; Menü und neuer Hinweis persönlich bestätigt. Build-44-Setup auf dem Arbeitsrechner: acht HD-Schnitte und John Wick in beiden Klebeverfahren positiv; keine pauschale neue Setup-/Portable-Abnahme daraus abgeleitet. Build 43: 28 native QUICK-Fälle und 21 bisherige native Klebecenter-Fälle bestanden; neun erfolgreiche Neukodierungsausgaben bytegleich zu Build 42. John Wick und Furiosa technisch vollständig geprüft, Furiosa mit beiden Tonspuren. Eigener Furiosa-QUICK-Lauf vom Nutzer auf Bild, Ton und Klebeübergang im Drei-Sekunden-Fenster positiv bestätigt. Eine neue Setup-/Portable-/VM-Abnahme wird daraus nicht abgeleitet.

[Funktionsnachweis und genaue Qualitätsgrenzen](KLEBECENTER-QUICK-COPY-20261028.md), [Anleitung](NUTZERANLEITUNG.md), [Paketprüfung](RELEASE-1.0.2-BUILD45-RESULT.md), [Quellumfang](FREEZE-1.0.2-BUILD45-20261029.md). Der archivierte Dokumentationsstand ist der Erstellungsstand; der abschließende Paketbericht wird im Arbeitsrepository um Prüfsummen und Ergebnisse ergänzt.

## Richtigen Motor nach Überinstallation wählen

Mitgeliefert **tools/otr-can/otr_can.exe**, Version **0.1.6**. Gespeicherten Pfad unter Einstellungen → OTR-CAN-Werkzeuge auf diese neue mitgelieferte EXE prüfen. FFmpeg/ffprobe, GPAC/MP4Box und FFMS2 bleiben externe Werkzeuge. Voraussetzungstext nennt korrekt Motor 0.1.6; .NET 10 Desktop Runtime x64 separat erforderlich. Für den Motor zusätzlich Microsoft Visual C++ v14 x64. Das Klebecenter verwendet die konfigurierten FFmpeg-Werkzeuge.

<details>
<summary>Historische Releasehinweise der veröffentlichten Build-41-Fassung</summary>

# Cut Assistant Next 1.0.2 · Build 41

Am 27.10.2026 nach ausdrücklicher Freigabe als **v1.0.2 / Latest veröffentlicht**. CAN 1.0.2 Build 41 enthält **otr-can 0.1.6** und **libmpv 0.41.0**. Setup und Portable-ZIP gehören zum gleichen geprüften Quellenstand.

## Klebecenter

Unter **Klebecenter → Filmteile zusammenfügen …** zwei bereits geschnittene MP4-Dateien auswählen. Zuerst Reihenfolge festlegen, dann pro Teil eine Tonspur wählen. Bei mehreren Spuren gibt es keine Vorauswahl. Die allein gewählte Spur jedes Teils wird verwendet; übrige Spuren entfallen. Beide gewählten Spuren müssen Abtastrate und Kanalbelegung teilen. AAC/AAC und AC-3/AAC sind möglich.

Das Bild bleibt unverändert. Der Ton wird samplegenau verbunden und **einmal durchgehend als AAC-LC neu kodiert** (192 kbit/s bei Mono/Stereo, 384 kbit/s bei Mehrkanalton). Eine Vorbereitungskopie oder erneute Anwendung einer Cutlist ist nicht nötig. Genau zwei passende progressive H.264-MP4-Dateien mit konstanten 25/50 fps werden unterstützt; unpassende Formate oder fehlerhafte Zeitachsen werden abgelehnt.

Nach Abschluss **Anfang, Klebestelle und Ende** über „Prüfen“ kontrollieren. Die Vorschau beginnt drei Sekunden vor der tatsächlichen Klebestelle. Nach Erfolg werden Arbeitsdateien und Erfolgsprotokoll samt leerem Arbeitsordner entfernt. Bei Fehler oder Abbruch bleibt die Diagnose erhalten; gesperrte oder fremde Inhalte werden gemeldet und nicht rekursiv gelöscht.

[Anleitung](NUTZERANLEITUNG.md), [Klebecenter und Grenzen](KLEBECENTER-20261027.md), [Bereinigungsnachweis](KLEBECENTER-BEREINIGUNG-20261027.md).

## Schnitt und Ton

Der otr-can-Schnitt kann bei erkanntem Tonfehler nach ausdrücklicher Auswahl mit einer streng geprüften intakten AAC-Spur fortgesetzt werden. Entfallene Spuren werden angezeigt; Warnungen werden nicht pauschal ignoriert. Gesunde Aufträge behalten ihren normalen Ablauf. Cutlist-Metadaten nennen den vorgesehenen Schnittmotor samt Version.

Der MP4Box-Prüfablauf ordnet begrenzte Bildzeitüberlappungen am tatsächlichen Abschnittswechsel zu. Fehlende Originalbilder werden dadurch nicht wiederhergestellt; Werbeblitzer können bei MP4Box bestehen bleiben. Geeignete ältere H.264-AVI-Dateien werden experimentell ohne Video-Neukodierung für den Schnitt vorbereitet; MP3 wird beim otr-can-Schnitt einmal nach AAC-LC kodiert.

## Richtige Werkzeugversion

**Mitgeliefert: otr-can 0.1.6**, im Programmordner unter **tools/otr-can/otr_can.exe**. Nach einer Überinstallation unter **Einstellungen → OTR-CAN-Werkzeuge** diesen Pfad prüfen und die mitgelieferte EXE wählen. Eine alte gespeicherte Einstellung kann auf eine frühere Motorversion zeigen. Der frühere Hinweis „0.1.5“ wurde korrigiert; die Paketprüfung gleicht Text, tatsächliche EXE und Herkunft ab.

FFmpeg/ffprobe, GPAC/MP4Box und FFMS2/ffmsindex bleiben externe Werkzeuge. Das Klebecenter verwendet die konfigurierten FFmpeg-Werkzeuge. Windows 11 x64, .NET 10 Desktop Runtime x64 und für otr-can die Visual-C++-x64-Laufzeit werden benötigt. Details stehen im mitgelieferten Voraussetzungstext.

## Prüfstand

1489 Pakettests, zusätzlich 1217 Tests gegen finale Produktbibliotheken und 21 native Klebecenter-Fälle bestanden; Release-Build ohne Warnungen/Fehler. John Wick und Furiosa persönlich geprüft. Build-40-Setup-/ZIP-VM-Runde bestätigt; Build-41-Setup auf dem Arbeitsrechner einschließlich verschwundener temporärer Datei und Bild/Ton/Schnitt/Drei-Sekunden-Vorschau/Klebecenter positiv. Eine zusätzliche Build-41-ZIP-/VM-Prüfung wird daraus nicht abgeleitet.

Die Ursache früherer kompletter Furiosa-Rechnerstillstände bleibt ungeklärt; positive spätere Läufe belegen keine gezielte Behebung. Bitte jede Ausgabe auf Bild, Ton, Synchronität und Übergänge kontrollieren.

Die beigelegten Quellenarchive gehören exakt zu den geprüften Binärpaketen und enthalten Dokumentation aus ihrem Erstellungsstand vor der Veröffentlichungsfreigabe. Die aktuellen Repository-Unterlagen und diese Releasebeschreibung ergänzen die spätere Abnahme/Freigabe. [Paketbericht und Prüfsummen](RELEASE-1.0.2-BUILD41-RESULT.md).

</details>
