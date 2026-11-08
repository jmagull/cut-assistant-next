# Dokumentation – Cut Assistant Next

## Cut Assistant Next 1.0.3 Build 54 – stabile Version

Aktueller gemeinsamer Stand im ursprünglichen Repository **cut-assistant-next**; nach ausdrücklicher persönlicher Freigabe vom **08.11.2026** für `main` und [GitHub Latest](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.3-build54) bestimmt. Dateiversion **1.0.3.54**, libmpv **0.41.0**, mitgelieferter otr-can **0.1.6-audio-diag.1**. Alte Funktionszweige sind historische Quellen; es gibt eine gemeinsame CAN-Version.

Spotlight-CAN ist unter **Werkzeuge → Spotlight-CAN – Visuelle Werbesuche** integriert: Werbeblöcke anhand von Vorschaubildern grob markieren, bestätigte Schnittbereiche direkt an CAN übergeben und dort mit Player/Frame-Lupe präzisieren. Größe, Position und Maximierungszustand werden über CAN-Neustarts erhalten. Mehrmonitor-/DPI-Korrektur hält das Fenster bei geänderter Bildschirmkonfiguration sichtbar. Die Detailsuche hat standardmäßig zwei Sekunden Bildabstand; den letzten Bereich bei Bedarf in CAN mit **Videoende** bis zum tatsächlichen Dateiende abschließen.

Der normale otr-can-Audiomodus prüft streng und verändert bei einem Fehler keine Samples. Bei geeigneten Befunden stehen Abbrechen, ein ausdrücklich bestätigter vollständiger MP4Box-Neustart, Tonspur-Abwahl bei Mehrspurdateien oder **AAC-Zeitlücken reparieren (experimentell)** zur Wahl. Die Reparatur benötigt ausdrückliche Zustimmung, ergänzt nur nachgewiesene unterstützte Lücken positionsgenau mit Stille und prüft den gesamten Schnitt unabhängig. Keine allgemeine Sampletoleranz, keine automatische Reparatur und keine Wiederherstellung verlorener Originaltöne.

Der passende Motor liegt ausschließlich unter **`tools/otr-can/otr_can.exe`** relativ zur Anwendung; gespeicherte Werkzeugpfade werden nicht überschrieben. Für eine ZIP den vollständigen Inhalt in einen frischen Ordner entpacken. Die Portable-Fassung verwendet wie das Setup CANs Benutzereinstellungen unter dem lokalen Windows-Benutzerprofil. Beim Setup-Upgrade können alte `runtime`-Dateien verbleiben; sie werden nicht verwendet.

Voraussetzungen: **Windows 11 x64**, **.NET 10 Desktop Runtime x64**, für otr-can **Microsoft Visual C++ v14 Redistributable x64**. FFmpeg/ffprobe, GPAC/MP4Box und FFMS2/ffmsindex separat einrichten; keine Windows-Codec-Pakete erforderlich. Windows 10 ist nicht persönlich validiert. Bild, jede Tonspur und Schnittübergänge weiterhin selbst kontrollieren. QUICK AND COPY führt keine samplegenaue Tonprüfung aus; AVI bleibt experimentell.

**Cut Assistant Next 1.0.3 Build 54 wurde persönlich erfolgreich geprüft.** Insbesondere Setup in der Windows-VM: Grey’s Anatomy regulär sowie NCIS Sydney – Breathless, S02E07 nach ausdrücklich bestätigter AAC-Reparatur; Bild, Ton und Drei-Sekunden-Vorschau positiv. Die weitergehende persönliche Setup-/ZIP-/Spotlight-/Mehrspur-/Klebecenter-Abnahme von Build 53 bleibt separat zugeordnet.

[Downloads](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.3-build54) · [Release Notes](RELEASE-NOTES-1.0.3.md) · [Spotlight-Anleitung](SPOTLIGHT-README.md) · [Abnahme und Veröffentlichung](RELEASE-1.0.3-BUILD54-PUBLISHED-20261108.md). Die folgenden datierten Entwicklungsstände und alten Prüffassungen sind historische Nachweise, keine aktuelle Installationsanleitung.


## Historischer eingefrorener Paketsatz

**Cut Assistant Next 1.0.3 Build 52** enthält die wiederhergestellte Tonspur-Abwahl, den AAC-Reparaturweg und integriertes Spotlight-CAN. [Setup, Portable-ZIP und Quellpakete](FREEZE-1.0.3-BUILD52.md) · [Tonspur-Abwahl](AUDIO-SPURABWAHL-BUILD52.md) · [Spotlight-Integration](SPOTLIGHT-INTEGRATION-20261104.md)

## Stand vom 03.11.2026

**03.11.2026 – eingefrorener Entwicklungsstand CAN 1.0.3, Buildzähler 48.** Gemeinsame Server-/Lokal-Cutlist-Tabelle, Rot/Entfernen und Grün/Behalten mit dynamischer Vorschau sowie Standard / Power-User 1 / Power-User 2 einschließlich Buttonausrichtung sind umgesetzt. Letzter technischer Stand: **1.728/1.728 Tests**, Release-Build ohne Warnungen/Fehler. Quellstand und Dokumentation werden auf Nutzerauftrag committet und gepusht; keine neuen Pakete und keine Veröffentlichung ohne gesonderte Freigabe. Öffentlich bleibt der dokumentierte stabile Stand **CAN 1.0.2 Build 48**. [Freeze und Prüfgrenzen](FREEZE-1.0.3-20261103.md).

[Entwicklungsnotizen 1.0.3](RELEASE-NOTES-1.0.3.md) · [Cutlist-Tabelle](CUTLIST-TABELLE-IMPLEMENTIERUNG-20261103.md) · [Schnittmodi und Vorschau](SCHNITTMODI-TIMELINE-20261103.md).

## Eingefrorenen Stand 1.0.3 zuerst lesen

- [Freeze CAN 1.0.3](FREEZE-1.0.3-20261103.md): Funktionsumfang, Prüfstand, persönliche Sichtbestätigung und Freigabegrenze für die Veröffentlichung.

- [Christmas Cookbook: persönlicher Power-User-2-Filmschnitt](CHRISTMAS-COOKBOOK-PRAXISTEST-20261103.md): otr-can, Bild/Ton/Schnitt/Drei-Sekunden-Vorschau positiv; anschließende Sichtprüfung der Buttonkorrektur positiv.

- [Power-User 2: Implementierung und WPF-Aufnahmen](POWER-USER-2-IMPLEMENTIERUNG-20261103.md): dritte dauerhafte Layoutoption, kompakte Schnittbuttons, rote/grüne Vormerkzeile; 1728 Tests und zwölf native Prüffallgruppen. Das [vorausgegangene Konzept](PROFI-ANSICHT-2-KONZEPT-20261103.md) bleibt als historische Vorstufe erhalten.

- [Santa Tell Me: grüner Praxistest](SANTA-TELL-ME-GRUENER-PRAXISTEST-20261103.md): Cutlist, vier Behaltebereiche, MP4Box/otr-can-Protokolle, persönliche Ton-/Bild-/Schnitt-/Vorschauprüfung und Servereintrag.

- [Entwicklungsnotizen 1.0.3](RELEASE-NOTES-1.0.3.md): Funktionen, Bedienung, Nachweise und offene Praxisfälle.
- [Gemeinsame Cutlist-Tabelle](CUTLIST-TABELLE-IMPLEMENTIERUNG-20261103.md): exakte lokale Suche, Quellenschutz, Dauer und registrierte Downloads.
- [Schnittmodi und dynamische Timeline](SCHNITTMODI-TIMELINE-20261103.md): Rot/Grün, Einstellung, Vorschau, Speicherschutz und persönliche Screenshots.
- [Nutzeranleitung](NUTZERANLEITUNG.md), [Projektstand](AKTUELLER-STAND.md), [Testplan](TESTPLAN.md) und [Setup-Merkliste](SETUP-MERKLISTE.md).
- Vorausgegangene Bestandsaufnahmen: [lokale Cutlists](LOKALE-CUTLISTS-BESTANDSAUFNAHME-20261103.md), [gemeinsame Tabelle](CUTLIST-TABELLE-UI-BESTANDSAUFNAHME-20261103.md). Sie beschreiben die Untersuchung vor der Umsetzung.

## Stabil veröffentlicht: 1.0.2 Build 48

**01.11.2026: CAN 1.0.2 Build 48 ist als stabile Latest-Version veröffentlicht.** [Release v1.0.2-build48](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2-build48), App-Dateiversion **1.0.2.48**. Build 47 bleibt unverändert als stabile Vorgängerversion erhalten.

## Veröffentlichungs- und bisherige Funktionsnachweise

- [Build-48-Veröffentlichung](RELEASE-1.0.2-BUILD48-PUBLISHED-20261101.md) und [Paketbericht](RELEASE-1.0.2-BUILD48-RESULT.md): Commit, Tag, Tests, acht Assets und Prüfsummen.

- [Build-48-Freeze](FREEZE-1.0.2-BUILD48-20261101.md): freigegebener Kontrollplayer-Stand und Releasegrenzen.
- [Kontrollplayer-Korrektur](KONTROLLPLAYER-TONSPUR-KORREKTUR-20261101.md): EOF, Positionserhalt, Tests und Praxisabnahme.

- [Projektstand](AKTUELLER-STAND.md): Version, Verfahren, Nachweise und Grenzen.
- [Build-47-Veröffentlichung](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md): Freigabe und anschließende Commit-/Tag-/Assetprüfung.
- [Build-47-Paketbericht](RELEASE-1.0.2-BUILD47-RESULT.md): Setup, Portable, genaue Quellen und Prüfsummen.
- [Build-47-Freeze](FREEZE-1.0.2-BUILD47-20261031.md): eingefrorener Produktstand und Herkunft.
- [Release Notes](RELEASE-NOTES-1.0.2.md), [Nutzeranleitung](NUTZERANLEITUNG.md), [Versionszuordnung](VERSIONEN.md).
- [MP4Box-Sperre für AUDIO-ENTHUSIAST](MP4BOX-AUDIO-ENTHUSIAST-SPERRE-20261031.md): Container-Erkennung, Dialog-/Dienstsperre und Grenzen.
- [QUICK und Meckerecke](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md): pragmatischer Kopierweg ab Build 46, im Build 47 enthalten.
- [Build-47-Setup in der VM](VM-SETUP-BUILD47-20261031.md) und [Portable-ZIP in der VM](VM-PORTABLE-BUILD47-20261031.md): getrennte persönliche Nachweise.
- [Praxistests vom 31.10.2026](PRAXISTESTS-20261031.md): Rocky Horror, Doc und Brilliant Minds mit Build-46-Setup auf dem Arbeitsrechner.
- [Testplan](TESTPLAN.md), [Setup-Merkliste](SETUP-MERKLISTE.md), [Status](STATUS.md), [Tagebuch](TAGEBUCH.md).
- [Architektur](ARCHITECTURE.md), [Entscheidungen](DECISIONS.md), [Anforderungen](REQUIREMENTS.md), [Projektdefinition](PROJECT_DEFINITION.md).

## Frühere Versionen und Funktionsnachweise

- [Build-46-Pakete](RELEASE-1.0.2-BUILD46-RESULT.md), [Build 45](RELEASE-1.0.2-BUILD45-RESULT.md), [Build 44](RELEASE-1.0.2-BUILD44-RESULT.md), [Build 43](RELEASE-1.0.2-BUILD43-RESULT.md) bleiben erhalten.
- [Build-41-Veröffentlichung](RELEASE-1.0.2-PUBLISHED-20261027.md) und [Paketbericht](RELEASE-1.0.2-BUILD41-RESULT.md) dokumentieren den vorherigen öffentlichen Stand.
- [Klebecenter](KLEBECENTER-20261027.md), [Bereinigung](KLEBECENTER-BEREINIGUNG-20261027.md), [Phasenzeiten/RF64](KLEBECENTER-PHASENZEITEN-RF64-20261028.md), [zwei Klebeverfahren](KLEBECENTER-QUICK-COPY-20261028.md).
- [AAC-Notfallweg](OTR-CAN-AAC-NOTFALLWEG-20261026.md), [historische Tonspurkopie](TONSPURAUSWAHL-FILMTEILE-20261026.md), [Furiosa-Diagnose](FURIOSA-MP4BOX-AUDIO-ENTHUSIAST-20261031.md).

## Historie und unveränderte Archive

Datierte Berichte bewahren ihre damaligen Versions-, Abnahme- und Freigabestände. Ältere Angaben wie „öffentlich Build 41“ oder „keine Veröffentlichung beauftragt“ sind dort historische Aussagen. Eingefrorene Quellen-/Paketarchive behalten ihre damalige Dokumentation und Prüfsummen; Repository und Releasebeschreibung ergänzen spätere Tests und Freigaben.

Die [libmpv-Unterlagen](libmpv/v0.41.0/README.md) beschreiben den unveränderten 0.41.0-Stack. Drittanbieter-Versionen und Lizenzen werden nicht auf CAN-Buildnummern umgeschrieben. READMEs unter tools/prototypes gehören zu isolierten Diagnosen. Aktuelle Installationspakete verwenden installer/HINWEIS-DOTNET-UND-WERKZEUGE.txt.
