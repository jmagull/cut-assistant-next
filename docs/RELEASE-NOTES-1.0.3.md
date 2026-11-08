# Entwicklungsnotizen – CAN 1.0.3

## Cut Assistant Next 1.0.3 Build 54 – gemeinsame lokale Prüffassung

Ein maßgeblicher Entwicklungsstand im ursprünglichen Repository **cut-assistant-next**, Branch **codex/integration-build54**. Die geprüfte Audio-Fehlerbehandlung aus Build 52 und die Spotlight-Fensterpersistenz aus Build 50 gehören zu derselben regulär benannten Anwendung. Alte Funktions-Checkouts bleiben unveränderte Quellen; neue Entwicklung erfolgt im kanonischen Repository.

Der normale Audioablauf bleibt streng. Nach einem belegten Tonfehler stehen Abbrechen, bestätigter MP4Box-Neustart und bei passendem Befund ausdrücklich bestätigte AAC-Reparatur oder Tonspur-Abwahl bereit. Der passende Motor wird aus `tools/otr-can` verwendet; gespeicherte Werkzeugpfade bleiben erhalten. Keine automatische Reparatur oder pauschale Sampletoleranz.

Spotlight unter **Werkzeuge → Spotlight-CAN – Visuelle Werbesuche** übergibt grobe Bereiche an CAN, zeigt Original-/Entfernen-/Restdauer und behält Größe, Position und Maximierung. Mehrmonitor-/DPI-Korrektur hält das Fenster sichtbar. Die Detailsuche hat standardmäßig zwei Sekunden Bildabstand. Den letzten betroffenen Bereich bei Bedarf in CAN mit **Videoende** bis zum tatsächlichen Dateiende abschließen.

Voraussetzungen bleiben Windows 11 x64, .NET 10 Desktop Runtime x64 und die in CAN eingerichteten externen FFmpeg-/ffprobe-/MP4Box-/FFMS2-Werkzeuge. Windows 10 wurde nicht persönlich validiert. Build 53 wurde vom Nutzer auf Arbeitsrechner und in der VM einschließlich Setup/ZIP, Spotlight, Audiofällen und Klebecenter persönlich bestätigt. Build 54 ändert ausschließlich den Motorordner und ist lokal zur erneuten Prüfung dieses Pfades bestimmt; keine automatische Installation und kein öffentliches Release. Stabil veröffentlicht bleibt 1.0.2 Build 48.

[Integration und Herkunft](INTEGRATION-BUILD53-20261108.md) · [Prüfbericht Build 53](PRUEFBERICHT-BUILD53-20261108.md) · [Spotlight-README](SPOTLIGHT-README.md).


[Pfadumstellung und neue Prüffassung](PFADUMSTELLUNG-BUILD54-20261108.md).

## Build 52 · eingefrorener lokaler Paketsatz vom 05.11.2026

Der gemeinsame otr-can-Fehlerdialog bietet nach Nachweis einer beschädigten Spur und verwendbarer Restspuren den bestätigten vollständigen Neustart mit den geprüften Spuren. [Bedienung und technische Grenzen](AUDIO-SPURABWAHL-BUILD52.md)

Die persönliche Prüfung bestätigte den John-Wick-Dialog, den NCIS-Praxistest und beide Klebecenter-Verfahren. Der Paketsatz trägt den Namen **Cut Assistant Next** und enthält Setup, Portable-ZIP sowie CAN-, Spotlight-CAN-, Motor- und libmpv-Quellen. [Freeze und Nachweise](FREEZE-1.0.3-BUILD52.md)

## Build 51 · lokale Audio-Prüffassung vom 05.11.2026

- **Spotlight-CAN:** Grobe Entfernen-Bereiche an CAN übergeben und dort präzisieren. Die dynamische Anzeige zeigt Original-, Entfernen- und Restlaufzeit im Format HH:MM:SS.mmm. Abgeschlossene Bereiche zählen, Überlappungen werden einmal berücksichtigt.
- **otr-can:** Nach einem Tonfehler stehen Abbrechen, ein vollständiger Neustart mit dem eingerichteten MP4Box und bei geeigneten AAC-Lücken die ausdrücklich bestätigte experimentelle Reparatur zur Wahl.
- **Praxis:** Die vollständige NCIS-Sydney-AAC-Reparatur und reguläre Schnitte wurden persönlich geprüft. Die vollständige Bildprüfung kann den Reparaturlauf deutlich verlängern.
- **Pakete:** Setup unter dem gewohnten Namen Cut Assistant Next, Portable-ZIP und Quellen einschließlich Spotlight-CAN. Beide Programmpakete enthalten den für diese Fassung vorgesehenen Motor 0.1.6-audio-diag.1.

[Paketübersicht](PAKETE-BUILD51-20261105.md) · [Audioablauf und Prüfergebnisse](AUDIO-DREI-OPTIONEN-20261105.md) · [Spotlight-CAN](SPOTLIGHT-INTEGRATION-20261104.md)

## Build 48 · eingefrorener Stand vom 03.11.2026

Stand: 03.11.2026. **Auf Nutzerauftrag eingefrorener Entwicklungsstand, nicht veröffentlicht.** Der Buildzähler bleibt 48, lokale Dateiversion 1.0.3.48. Der dokumentierte stabile öffentliche Paketsatz bleibt CAN 1.0.2 Build 48 (1.0.2.48). Commit und Push von Quellstand und Dokumentation sind beauftragt. Keine neuen Setup-/Portable-/Quellpakete; Veröffentlichung erst nach gesonderter Freigabe. [Freeze und Nachweise](FREEZE-1.0.3-20261103.md).

## Gemeinsame Cutlist-Auswahl

- Server und lokale Dateien gemeinsam unter **Server**/**Lokal** anzeigen, ohne Vorauswahl oder automatische Übernahme.
- Lokal nur **Originaldateiname einschließlich Endung + .cutlist** im eingerichteten Cutlist-Verzeichnis und Filmverzeichnis prüfen. Keine Rekursion, keine ähnlichen Dateinamen. Manuelle Ladefunktionen bleiben erhalten.
- Bytegleiche lokale Treffer zusammenfassen, unterschiedliche gleichnamige Inhalte getrennt anbieten. Quellenfehler einzeln anzeigen und lokale Inhalte vor Übernahme erneut prüfen.
- Eigene begonnene, technisch gültige Cutlists bleiben ladbar; keine unbelegte Anzeige „Unfertig“. Aktuelle Arbeit vor Ersetzen schützen, gespeicherte eigene Datei bei Serverwahl oder Ablehnung unverändert lassen. Keine Zusammenführung.
- Dauer beider Quellen einheitlich **HH:MM:SS.mmm**. **Downloads (reg.)** kennzeichnet den gelieferten registrierten Zähler; die Gesamtzahl aller Downloads ist im bisherigen Suchabruf nicht vorhanden.

## Dritte Ansicht: Power-User 2

Der vorhandene Darstellungsdialog bietet **Standard**, **Power-User 1** (bisherige Ansicht) und **Power-User 2**. Die Wahl wird sofort angewendet und dauerhaft gespeichert, ohne Film, Tonspur oder Schnittplan zurückzusetzen. Power-User 2 verschiebt dieselben vier Schnittbuttons als kompakte Reihe unter die Playersteuerung; die Suche folgt daneben oder darunter. Der offene Anfang erscheint fett rot/grün bei der Frame-Lupe. Rechts steht mehr Höhe für die Tabelle bereit. Bei Platzmangel erfolgt Standard mit späterer Wiederherstellung der Wunschansicht. Die bestehenden Ansichten behalten ihre Anordnung. [Umsetzung, echte WPF-Aufnahmen und Prüfgrenzen](POWER-USER-2-IMPLEMENTIERUNG-20261103.md).

## Zwei Schnittarbeitsweisen

Unter **Einstellungen → Darstellung und Schnittbearbeitung …** dauerhaft wählen:

- **Das kann weg – Rot:** markierte Teile entfernen; Standard auch bei alten Einstellungen.
- **Das soll bleiben – Grün:** ausschließlich markierte Teile behalten.

Die Wahl gilt erst für den nächsten neu geöffneten Film. Ein laufender Plan bleibt unverändert. Nach dem ersten Punkt zeigt die Timeline eine feste Startlinie und eine halbtransparente Vorschau bis zur aktuellen Sliderposition, auch rückwärts. Erst das gültige Ende übernimmt einen Bereich. **Esc** verwirft nur den offenen Anfang. Bestehende Tabelle, Kantenbearbeitung, Auswahl, Löschen und Frame-Lupe bleiben gemeinsame Bedienelemente.

Vor grüner Ausgabe nennt eine Bestätigung Anzahl und Dauer der Behaltebereiche und erklärt das Weglassen aller unmarkierten Teile, auch noch nicht bearbeiteter Teile. Offene Anfänge verhindern das Speichern in beiden Modi. Vollständige Zwischenstände können im bestehenden Cutlist-Format gespeichert und weiterbearbeitet werden; keine neue Projektdatei oder Fertigstellungsverwaltung. Die Motoren erhalten weiterhin den kanonischen Schnittauftrag. MP4Box, otr-can 0.1.6, Klebecenter und libmpv 0.41.0 wurden für diese Erweiterung nicht geändert.

## Prüfung und persönliche Rückmeldung

Release-Build ohne Warnungen/Fehler, **1.728/1.728 Tests** bestanden: Core 200, Cutlists 80, Media 390, App 1.058. Power-User 2 ergänzt 15 automatisierte Fälle und zwölf separate native Prüffallgruppen; die anschließende Buttonausrichtung ergänzt vier DPI-Fälle und eine gezielte native Prüfung. +10 Bilder und beide Keyframe-Buttons haben gleiche Ober-/Unterkanten, bei Rückwechsel bleiben die bisherigen Layoutmaße erhalten. Echte WPF-Prüfung für Tabelle/Einstellungen, Timeline mit Slider, Transparenz, Vorwärts-/Rückwärtsbewegung, Abbruch/Übernahme und beide Layouts. Vorschau aktualisiert eine eigene Zeichenebene; 10.000 Keyframes werden während 200 zusätzlicher Positionsänderungen je Fall nicht neu durchlaufen.

Der Anwender bestätigt die gemeinsame Tabelle, Dauer und Downloadbeschriftung mit NCIS Sydney. Chicago Fire S13E09 wurde in der vorherigen lokalen 1.0.3-Prüffassung mit otr-can 0.1.6 persönlich für Schnitt, Bild, beide Tonspuren und Drei-Sekunden-Vorschau positiv geprüft. Die späteren Screenshots zeigen den grünen Behaltebereich beziehungsweise die roten komplementären Bereiche; beide Ansichten planen **00:39:28.320**. Kein zusätzlicher grüner Filmschnitt und keine persönliche Wiederaufnahmeprüfung daraus abgeleitet.

**The Christmas Cookbook** ist anschließend in Power-User 2 mit otr-can persönlich für Ton, Bild, Schnitt und Drei-Sekunden-Vorschau bestätigt; die Bild-/Tonmengen entsprechen **01:21:08.960**. Nach dem angekündigten Versionswechsel bestätigt die letzte Aufnahme die korrigierte Buttonausrichtung positiv. [Praxisnachweis](CHRISTMAS-COOKBOOK-PRAXISTEST-20261103.md).

## Offen und erhalten

**Santa Tell Me** ist inzwischen im grünen Modus mit vier Bereichen und **01:21:24.120** persönlich für Ton, Bild, Schnitt und Drei-Sekunden-Vorschau positiv gemeldet. MP4Box-/otr-can-Protokolle, gespeicherte Cutlist und Servereintrag liegen vor. [Praxisnachweis](SANTA-TELL-ME-GRUENER-PRAXISTEST-20261103.md). Die persönliche Wiederaufnahme gespeicherter begonnener Arbeit bleibt offen. Bestehende AVI-/Zeitstempel-/MP4Box-Grenzen bleiben gültig. Keine neue Setup-/Portable-/VM-Abnahme. Der anschließende Nutzerauftrag friert den korrigierten Stand ein und beauftragt Commit und Push. Kein Versions-/Buildsprung, Paketbau oder Release.

[Bedienung](NUTZERANLEITUNG.md) · [Projektstand](AKTUELLER-STAND.md) · [Testplan](TESTPLAN.md) · [Tabelle: Umsetzung und Nachweise](CUTLIST-TABELLE-IMPLEMENTIERUNG-20261103.md) · [Schnittmodi: Umsetzung und Nachweise](SCHNITTMODI-TIMELINE-20261103.md)
