# Community-Feedback vom 09.11.2026 – ursprünglicher Build-55-Stand

**Nachtrag zur Videoende-Logik:** Die frühere Sondermarkierung „letztes Behaltebild“ ist ersetzt. **Schnitt bis Videoende abschließen** beendet jetzt ausschließlich einen offenen Entfernen-Bereich an der tatsächlichen Mediendauer. Aktueller Stand und Prüfergebnisse im [EOF-Korrekturbericht](SPOTLIGHT-VIDEOENDE-BUILD55.md). Die nachstehenden Angaben zur früheren Videoende-Sondermarkierung sind historisch.

**Korrektur nach persönlicher Prüfung:** Die Sekundensteuerung wurde aus Spotlight entfernt und in die CAN-Hauptplayerleiste verschoben. Videoende bleibt erhalten. Aktuelle Bedienung, Prüfungen und EXE-Pfad im [Korrekturbericht](PLAYER-SEKUNDEN-NAVIGATION-BUILD55.md). Die folgenden Navigations-/Datei-/Testangaben beschreiben die Erstfassung.

## Grundlage und Abgrenzung

Grundlage ist der bereits zusammengeführte CAN-Stand a7ba850 (1.0.3 Build 54), einschließlich Spotlight und Audioreparatur. Die ursprüngliche Arbeitskopie steht noch auf 22f4042 und enthält Nutzeränderungen; sie wurde nicht umgestellt oder bearbeitet. Entwicklung ausschließlich auf codex/community-feedback-20261109 in der getrennten Arbeitskopie .build/worktrees/community-feedback. Die vorhandene Solution und alle Produkt-/Projekt-/EXE-Namen bleiben bestehen. Kein neuer Produktname, keine neue Anwendung, keine neue Produktionsabhängigkeit, keine Umstrukturierung.

Der vorgeschriebene nummerierende Build erzeugt lokal Build 55; kein neuer Paketsatz. Installation, Release, Merge nach main und Push wurden nicht ausgeführt. Die Spotlight-Änderungen liegen derzeit als lokale Änderungen in der bestehenden Unterkomponente vor; ein späterer freigegebener Quellstand muss diese Änderungen und die CAN-Anbindung zusammen sichern.

## Videoende: Befund und Umsetzung

Im vorliegenden Spotlight-Quellstand 6973b37 gibt es keine separate Filmende-Markierung mehr. MarkEnd schließt ausschließlich einen begonnenen Werbeblock beziehungsweise einen Korrekturentwurf. Der komplementäre Behalteplan läuft anschließend grundsätzlich bis zur Mediendauer weiter. Das erklärt den reproduzierbaren Nachlauf; eine frühere Implementierung oder deren Entfernung ist damit historisch noch nicht nachgewiesen. Das bisher dokumentierte Zwei-Sekunden-Bildraster trifft das tatsächliche Dateiende nicht zuverlässig. Es war keine fehlerhafte CAN-Übergabe nachweisbar: Der übergebene Plan enthielt den Nachlauf schlicht nicht als Entfernen-Bereich.

Neu: Ein Bild oder eine Zeitposition wählen und **Gewähltes Bild als Videoende** drücken. Der Videoanfang bleibt automatisch null; kein zusätzliches Anfangsfeld. Das gewählte Bild bleibt vollständig erhalten, danach wird bis zur Dateidauer entfernt. **Videoende aufheben** stellt den bisherigen Plan wieder her. Eine Markierung lässt sich neu setzen. Bestehende Werbeblöcke bleiben separat erhalten; sie werden nicht überschrieben. Ein letzter gewünschter Frame innerhalb eines Werbeblocks wird mit verständlichem Hinweis abgewiesen. Offene Anfänge und unbestätigte Korrekturen sind zuerst abzuschließen oder zu verwerfen.

Wiederverwendung: CANs vorhandener FfprobeFrameRunner und dessen Original-PTS/Zeitbasis, Spotlight-CutPlan-Validierung, RuntimeSummary, derselbe atomare Transfer über SpotlightMediaSession.TryApply und CutPlanViewModel.LoadCutPlan sowie die bestehenden Cutlist-Schreib-/Lesefunktionen. Die durchschnittliche Bildrate dient nicht als Ersatz für tatsächliche Framegrenzen. Die folgende Frame-PTS beziehungsweise eine nachgewiesene Dauer des letzten Frames bildet die exklusive Endgrenze. Containerstart und Rundung auf 100-ns-Ticks werden berücksichtigt. Fehlende/mehrdeutige Original-PTS führen zu einem Hinweis statt zu einer erfundenen Grenze. In solchen Dateien bleibt die manuelle Bearbeitung mit CAN/Frame-Lupe erforderlich.

Der vollständige Plan vereinigt Nachlauf und bereits markierte spätere Werbeblöcke ohne Überschneidungen. Laufzeitanzeige, Übernahme und optionaler bestehender Export verwenden denselben Plan. Ohne Videoende bleibt die bisherige Schnittsemantik erhalten. Die gemeinsame Markierungsliste enthält weiterhin die manuell erfassten Werbeblöcke; der gesondert sichtbare Filmende-Hinweis beschreibt den zusätzlichen Nachlauf.

## Tastaturnavigation

Spotlight: unmodifizierte Pfeiltasten links/rechts sowie die beiden sichtbaren Pfeilbuttons verschieben die gewählte Position um die eingegebene Sekundenanzahl. Standard 10; Schnellauswahl 1, 5, 10, 30, 60; positive Dezimalwerte mit Punkt oder Komma möglich. Grenzen werden auf null und Dateidauer begrenzt. CANs vorhandenes PlaybackViewModel.SeekToAsync steuert denselben libmpv-Player, einschließlich Pause-Erhalt; kein zusätzlicher Player. Die zuvor ausgewählte Bildkarte wird nach einem Fenstersprung abgewählt, damit sie nicht als Bild der neuen Position erscheint. Neue Vorschaubilder werden bei Bedarf weiterhin ausdrücklich mit Details erzeugen geladen; die Anzeige der gewählten Position ist maßgeblich.

Textfelder, editierbare Auswahlfelder, Slider, Tabellen, Scrollbars und Trenner behalten ihre Pfeiltasten. Kombinationen mit Strg/Alt/Umschalt/Windows werden nicht übernommen. Die bestehenden CAN- und Frame-Lupen-Tastenkombinationen bleiben unverändert. Während einer laufenden Operation werden weitere Sprünge nicht aufgestaut. Fehler verändern keine Schnittmarkierung.

Die Sprungweite wird im bestehenden CAN-Einstellungsverzeichnis unter spotlight-navigation-settings.json gespeichert, auch bei Schließen ohne vorherigen Sprung. Beschädigte, fehlende oder ungültige Werte ergeben 10 Sekunden; Schreibfehler werden protokolliert und unterbrechen die Schnittarbeit nicht. Die native Prüfroutine verwendet ein isoliertes Einstellungsverzeichnis.

## Cutlist-Bewertung: Machbarkeit und Grenze

Die alte Funktion ist fachlich wiederverwendbar; kein Delphi-/GPL-Code wurde übernommen. Die untersuchte Primärquelle [ca2018/Main.pas](https://github.com/abc874/ca2018/blob/master/Main.pas), Funktion SendRating, belegt den historischen GET-Aufruf rate.php mit rate (Server-Cutlist-ID), rating, userid, protocol und Versionsparametern. Der [alte Bewertungsdialog](https://github.com/abc874/ca2018/blob/master/CutlistRate_dialog.pas) verlangt eine bewusste Auswahl/Bestätigung. Die vorhandene CAN-Suche liefert bereits Server-ID und Autor; CANs Bewertung bei Cutlist-Erzeugung ist dagegen die Bewertung des Autors und keine Fremdbewertung.

Am 09.11.2026 ließen sich die cutlist.at-Startseiten über die Rechercheverbindung nur mit Gatewayfehler abrufen. Die [Sniplist-Startseite](https://sniplist.mepaso.net/) war in der Webrecherche erreichbar, lieferte aber keine bestätigte öffentliche Dokumentation des aktuellen Bewertungsvertrags. Daraus folgt keine Aussage über einen allgemeinen Serverausfall oder das Fehlen einer Schnittstelle. Die gegenwärtigen Authentifizierungs-, Protokoll-, Antwort- und Duplikatregeln des historischen Bewertungsendpunkts konnten nicht bestätigt werden. Es wurde keine Testbewertung an einen echten Server gesendet.

**Ergebnis: keine unbestätigte Direktübermittlung eingebaut.** Dies nutzt die im Arbeitsauftrag ausdrücklich vorgesehene Dokumentationsalternative. Bestehende Suche, Download und Upload bleiben unverändert. Auch keine nur scheinbar funktionierende Bewertungsschaltfläche. Abbruch-/Netzwerkfehler-Tests für eine neue Bewertungsübermittlung sind deshalb noch offen und werden nicht als durchgeführt ausgewiesen.

Vorschlag für die anschließende Integration nach Bestätigung des aktuellen Serververtrags:

1. Beim erfolgreichen Übernehmen einer Server-Cutlist eine unveränderliche Herkunft aus ursprünglicher Serveradresse, Cutlist-ID, Autor, Originalfilm und Mediengeneration halten. Lokale Dateien ohne belegte Serveridentität nicht anhand des Dateinamens zuordnen; eigene Cutlists ausnehmen.
2. Nach ausdrücklicher Schnittprüfung ein freiwilliges Angebot anzeigen: Bewerten oder Überspringen. Keine Vorauswahl und keine automatische Bewertung, auch nicht nach Ablauf eines Timers oder bei Fensterende.
3. Dialog zeigt Autor und übernommene Fassung; nur bewusstes Senden löst genau eine abbrechbare Anfrage aus. Kein automatischer Wiederholungsversuch nach unklarem Netzwerkausgang. Währenddessen weiterarbeiten können; Wechsel des Films darf keine neue Fassung bewerten.
4. Netzwerk-/Antwortfehler getrennt anzeigen; Schnittplan und fertiger Film bleiben erhalten. Persönliche Serverkennung nicht protokollieren. Bestätigtes Antwortformat und Duplicate-/Abbruchfälle mit simuliertem HTTP prüfen; ein echter Schreibtest erst nach gesondertem ausdrücklichem Auftrag.

## Geänderte Dateien

CAN:

- src/CutAssistantNext.App/MainWindow.Spotlight.cs – vorhandener Host mit Frameprüfung, Playernavigation und Einstellungen.
- src/CutAssistantNext.App/Services/SpotlightFrameBoundary.cs – neue, testbare Frame-Endgrenzenauflösung mit bestehender Analyse.
- src/CutAssistantNext.App/Settings/SpotlightNavigationSettingsStore.cs – neue Sprungweitenpersistenz.
- tests/CutAssistantNext.App.Tests/Services/SpotlightCommunityFeedbackTests.cs – gezielte Markierungs-/Navigations-/Zeit-/Einstellungsprüfungen.
- tests/CutAssistantNext.App.Tests/Services/SpotlightMediaSessionTests.cs – neun Blöcke mit/ohne Videoende in beiden Schnittmodi.
- tools/prototypes/SpotlightIntegrationChecks/Program.cs – bestehender Prüfhelfer um echten Seek/PTS-Endmarker erweitert; isolierte Navigationseinstellungen.
- README.md und docs/SPOTLIGHT-README.md – aktuelle Bedienhinweise.
- docs/COMMUNITY-FEEDBACK-20261109.md – dieser Bericht.

Vorhandene Spotlight-Unterkomponente, jeweils unter external/spotlight-can:

- src/SpotlightCan.Core/CutPlan.cs – vollständiger Plan mit zusätzlichem Nachlauf.
- src/SpotlightCan.Wpf/MainViewModel.cs – Marker, Navigation und vollständige Laufzeit-/Transfer-/Exportdaten.
- src/SpotlightCan.Wpf/MainWindow.xaml und MainWindow.xaml.cs – vorhandene Oberfläche und geschützte Pfeiltastenbehandlung.
- src/SpotlightCan.Wpf/SpotlightHostOptions.cs – bestehender Hostvertrag erweitert.

Keine neuen Hilfsprogramme außerhalb tools; der einzige angepasste Prüfhelfer liegt weiterhin unter tools/prototypes. tools/otr-can/otr_can.exe bleibt der unveränderte Laufzeitort. MP4Box, otr-can, AAC-Reparatur, Frame-Lupe und Paketierung wurden nicht geändert. Keine neuen Produktionspakete oder fest codierten Benutzerpfade. Vorhandene C#-Projekte bleiben an ihren bisherigen Orten.

## Prüfung und offene Abnahme

Final: Release-Build ohne Warnungen/Fehler; 1888 CAN-Tests (200 Core, 80 Cutlists, 390 Media, 1218 App), 78 bestehende Spotlight-Tests (59 Core/Media, 19 WPF) und 36 echte CAN-/libmpv-/ffprobe-Integrationsprüfungen bestanden. Einschließlich tatsächlichem WPF-Pfeiltastenereignis und Eingabefeldschutz, pausierter/laufender Wiedergabe, neun Blöcken in beiden CAN-Modi, Abbruch und fehlgeschlagener Frameprüfung. dotnet format whitespace für die neuen C#-Dateien sowie git diff --check in CAN und Spotlight erfolgreich. Restore zusätzlich mit --runtime win-x64 --locked-mode erfolgreich; Lockdateien unverändert.

Die erste neue Testrunde fand zwei unpassende Zeit-Fixtures und einen bereits vorhandenen 100-ns-Rundungseffekt im Cutlist-Parser: doppelte PTS im Testdatenpaar korrigiert, exakte Millisekunden-Fixture statt double verwendet; Cutlist-Rundlauf erlaubt höchstens einen Tick, deutlich unter einer Bilddauer. Keine Produktionstoleranz für Bilder oder Audio erweitert und kein Test entfernt. Nachfolgende vollständige Runde erfolgreich.

Nachweise liegen lokal unter .build/test-results-complete, .build/spotlight-test-results-final und .build/native-community-final (results.json und tatsächliche WPF-Aufnahmen). Nur künstlicher 12-Sekunden-Film; keine realen OTR-Dateien hinzugefügt. Darstellung anhand der Aufnahme bei 1320 × 920 geprüft, rechte Bedienleiste scrollbar. Vollständige Windows-10- und Windows-11-Plattformtests, VM-/Setup-/Portable-/DPI-Abnahme und persönlicher komplexer Realfilmtest nicht durchgeführt. Automatisierte Hostläufe auf dem vorhandenen Windows-Arbeitsrechner ersetzen diese Abnahmen nicht. Die Cutlist-Rundlaufprüfung mit neun Werbeblöcken plus Nachlauf erzeugt zehn Behalteintervalle; zusätzlich direkte CAN-Übergabe beider Modi mit und ohne Marker geprüft.

Startbare Entwicklungsdatei: src/CutAssistantNext.App/bin/Release/net10.0-windows/CutAssistantNext.App.exe in dieser Arbeitskopie. Die vier bestehenden libmpv-DLLs sind enthalten. Der vorhandene Build-54-Motor mitsamt Quellen-/Lizenzbeigaben wurde ausschließlich in den lokalen Buildausgabeordner tools/otr-can kopiert, nicht neu gebaut oder verändert. EXE-SHA-256 identisch: 4146ee12f81bb7a4faef2d4f25d742a17649959be8c36491169055f5cf9fa4d1. Kein Setup und keine Release-Paketierung. Vorhandene FFmpeg-/ffprobe-/MP4Box-/FFMS-Einstellungen bleiben erforderlich.

Nächster Schritt: persönliche Prüfung dieser lokalen Entwicklungsfassung, besonders Auswahl des letzten gewünschten Bildes, Pfeiltasten bei Pause, Eingabefeldfokus und ein komplexer Film. Die separate Bewertungsintegration erfordert einen bestätigten aktuellen Serververtrag. Keine Veröffentlichung oder Installation ohne ausdrückliche spätere Freigabe.
