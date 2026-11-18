# Cut Assistant Next 1.0.3 Build 61 – lokaler Releasekandidat

Stand 18.11.2026. Gegenüber dem veröffentlichten Build 60:

- **Gezielte EAC3-Meldung:** Bei der ausdrücklichen otr-can-Ablehnung „Unsupported audio codec eac3“ erscheint „otr-can – EAC3-Tonspur erkannt“ mit „Mit MP4Box schneiden“ und „Abbrechen“. MP4Box startet ausschließlich nach Bestätigung; keine zusätzliche aufwendige Vorprüfung. Auch AAC+EAC3 wird korrekt erkannt. Andere Audiofehler behalten ihre Abläufe. Der Motor und sein EAC3-Unterstützungsumfang bleiben unverändert; MP4Box ist keine allgemeine Audioreparatur.
- Persönliche Tastenkürzel mit Konfliktprüfung, dauerhaftem Speichern und passenden Tooltips. Einzeltasten A bis Z und deutsche OEM-Tasten Ä, Ö und Ü lassen sich zuweisen. Sekundensprünge können auf Buchstabe + Pfeiltaste gelegt werden, etwa U + ←/→. S + ←/→ bleibt die Standardbelegung. Einzeltasten, die zugleich Teil einer Pfeilkombination sind, lösen beim Loslassen aus. In Eingabefeldern hat die Texteingabe Vorrang.
- Wiederholung von Sekunden-, Keyframe- und Einzelbildsprüngen per Maus und Tastatur; Startverzögerung und Abstand einstellbar und gespeichert.
- Fortlaufender Schnittshuffle über alle Bereichsgrenzen mit synchroner Schnittauswahl und unveränderter Reihenfolge nach Einzelbildkontrollen.
- Blaue K-Markierungen an beiden Grenzen und korrigierte Keyframe-Tooltips. Die nach dem ersten Test fehlenden Anfangs-/Endzeittexte sind wieder sichtbar.
- Schnittmotor-Empfehlung für unterstützte H.264-MP4-Konstellationen; manuelle Wahl und bekannte MP4Box-Grenzen bleiben bestehen.
- Optionale Suche nach anderen OTR-Aufnahmequalitäten mit Kennzeichnung und ausdrücklicher Übernahme.
- Erneutes Öffnen der letzten Schnittprüfung ohne neuen Schnitt sowie verbesserte Fokusrückgabe nach Schließen des Kontrollplayers.
- Frei editierbarer endgültiger Ausgabename für MP4Box und otr-can; gemeinsamer Name beim Dialogwechsel. Frei editierbarer vorgeschlagener Filmname beim Cutlist-Erzeugen. Manuelle Titel bleiben bis zur bewussten Neugenerierung erhalten; Originalzuordnung bleibt unverändert.
- Getrennte Versions- und Funktionsprüfungen für FFmpeg/ffprobe und MP4Box einschließlich Streamcopy-Regression und zwei AAC-Tonspuren.

**Abnahme:** Die persönliche Funktions- und Installationsabnahme unter Windows 11 ist abgeschlossen. Bestätigt sind die Realfilmschnitte mit beiden Motoren, der EAC3-Hinweis mit bestätigtem MP4Box-Ausweichschnitt, die oben genannten Build-61-Funktionen, deutsche Tastenkürzel einschließlich U + Pfeiltaste und Neustart, Setup-Upgrade, Portable-Start sowie VM-Installation/-Deinstallation. Die automatisierten Ergebnisse des abschließenden Laufs stehen im Paketbericht.

Die Einschränkung der allgemeinen Empfehlung bei EAC3 und der separate 40-ms-Rasterbefund sind nicht Teil dieser Dialogkorrektur. Der persönlich erfolgreiche MP4Box-Schnitt der EAC3-Aufnahme „Wetterfee sucht Weihnachtswunder“ bleibt Referenz; er wurde für die Korrektur nicht wiederholt. Die abschließende Aktualisierung der Dokumentation übernimmt die abgeschlossene persönliche Windows-11-Abnahme des unveränderten Programmstands.

**Status:** ausschließlich lokal vorbereitet. Kein Veröffentlichungsauftrag; öffentliche stabile Version bleibt Build 60.

## Übernommene Funktionen aus Build 60

## Hinweis zu FFmpeg

Für Cut Assistant Next 1.0.3 Build 60 empfehlen wir vorerst den erfolgreich geprüften **FFmpeg-Git-Build vom 12.10.2026 (Gyan.dev, `615b07170e`)**. FFmpeg und ffprobe müssen aus demselben Paket stammen.

Der Gyan.dev-Entwicklungsbuild vom **11.11.2026 (`23c4540053`)** kann beim otr-can-Videokopieren durch ein geändertes Seek-Verhalten unvollständige Segmente erzeugen. otr-can erkennt das und bricht den Schnitt ab. Für diesen Schnittweg bitte den empfohlenen Build verwenden.

Das Seek-Verhalten wurde im FFmpeg-Entwicklungszweig korrigiert. Ein Build eines anderen Anbieters vom **14.11.2026 (`604ef3784d`)** bestand den Praxistest mit dem vorhandenen otr-can-Motor. Ein aktualisierter Gyan.dev-Build kann nach erfolgreicher praktischer Prüfung als neue Referenz dienen.

[Download und Versionsinformationen bei Gyan.dev](https://www.gyan.dev/ffmpeg/builds/)

## Kurze Behaltebereiche am Videoende

- CAN prüft zeitbasierte Cutlists bei fehlender oder ungültiger Bildrate anhand der geladenen Aufnahme. Verlässliche Bildzeitstempel haben Vorrang. Der direkte Import und die lokale/Server-Cutlist-Tabelle nutzen dieselbe fachliche Prüfung.
- Bei einem auffälligen kurzen Endbereich zeigt CAN die Zeiten und, wenn gemessen, die Anzahl der Bildanfänge. Gültige Bereiche lassen sich unverändert laden. Entfernt wird der letzte von mehreren Bereichen erst nach Bestätigung. Bereiche hinter dem Videoende werden abgewiesen.
- Beim direkten MP4Box-Schnitt kann CAN einen auffälligen Endbereich vorab prüfen. Fehlt trotz Exitcode 0 eine Segmentdatei, nennt CAN Segmentnummer, Quellbereich und die verwendeten Werkzeuggrenzen. Die Keyframe-Grenzen von MP4Box gelten weiterhin.

## Usability aus Build 59

- **Sekundensprünge:** Frei einstellbare Sprungweite. `S` halten und `←` oder `→` drücken. Ein kurzer Druck springt einmal. Beim Halten folgen weitere Sprünge nach etwa 350 ms und danach etwa alle 200 ms. Die vorhandenen Sprungbuttons wiederholen bei gehaltener Maustaste genauso. Texteingaben, Einzelbildnavigation und `Strg + ←/→` für Keyframes behalten ihre Bedienung.
- **Schnittshuffle:** Der aktive rote oder grüne Bereich bleibt nach einer Korrektur ausgewählt. Für einen neuen Bereich zuerst die Auswahl aufheben.
- **Lautstärke:** Das Hauptfenster regelt auch den Kontrollplayer. Beim Öffnen und bei späteren Änderungen übernimmt der Kontrollplayer den zentralen Wert.
- **Tooltips:** Belegte Schaltflächen zeigen das jeweilige Tastenkürzel zusätzlich zur Funktionsbeschreibung.

**Aus Build 58 übernommen:** Spotlight-CAN mit „Schnitt bis Videoende abschließen“, Rot-/Grün-Schnittmodus, gemeinsame Suche nach lokalen und Server-Cutlists, Cutlist-Bewertung, Bild- und Schnittvorschau sowie Kontrollplayer. otr-can behält die Audioprüfung, Tonspur-Abwahl und die ausdrücklich zu bestätigende experimentelle AAC-Zeitlückenreparatur. MP4Box fügt Segmente mit gleichartigen AAC-Tonspuren weiterhin über `-add`, `-cat` und `-new` zusammen. Keyframe-bedingte Werbeblitzer können bei MP4Box weiterhin auftreten.

## Installation

Windows 11 x64 und .NET 10 Desktop Runtime x64 vorausgesetzt. Für otr-can wird außerdem Microsoft Visual C++ v14 Redistributable x64 benötigt. Das Setup aktualisiert die vorhandene Installation. Für die portable Ausgabe das ZIP vollständig in ein frisches Verzeichnis entpacken und `CutAssistantNext.App.exe` starten.

Enthalten sind libmpv 0.41.0 und otr-can 0.1.6-audio-diag.1 unter `tools/otr-can/otr_can.exe`. FFmpeg, ffprobe, MP4Box und FFMS2 bleiben externe Werkzeuge. Bezugsquellen und Pfadeinstellungen stehen in `VORAUSSETZUNGEN.txt`. Die vollständigen Quellen, das libmpv-Relink-Archiv und SHA-256-Prüfsummen werden separat bereitgestellt.

## Historische Prüfbasis Build 60

Die persönliche Abnahme unter Windows 11 umfasste das Setup auf dem Arbeitsrechner sowie Setup und frisch entpacktes Portable-ZIP in der VM. Bestätigt sind Schnitte mit MP4Box und otr-can, der AVI-Test „Teufelsbrigade“, die Cutlist-Bewertung, der Kontrollplayer mit zentraler Lautstärke und die Sekundensprünge. Bei der Deinstallation in der VM blieben die Benutzereinstellungen erhalten. Die otr-can-Werkzeugpfade mussten anschließend erneut gesetzt werden. Für Windows 10 liegt keine persönliche Prüfung von Build 60 vor.

<details>
<summary>Build 59 und frühere Fassungen</summary>

# Cut Assistant Next 1.0.3 Build 59

**Neu gegenüber Build 58**

- **Sekundensprünge:** Frei einstellbare Sprungweite. `S` halten und `←` oder `→` drücken. Ein kurzer Druck springt einmal. Beim Halten folgen weitere Sprünge nach etwa 350 ms und danach etwa alle 200 ms. Die vorhandenen Sprungbuttons wiederholen bei gehaltener Maustaste genauso. Texteingaben, Einzelbildnavigation und `Strg + ←/→` für Keyframes behalten ihre Bedienung.
- **Schnittshuffle:** Der aktive rote oder grüne Bereich bleibt nach einer Korrektur ausgewählt. Für einen neuen Bereich zuerst die Auswahl aufheben.
- **Lautstärke:** Das Hauptfenster regelt auch den Kontrollplayer. Beim Öffnen und bei späteren Änderungen übernimmt der Kontrollplayer den zentralen Wert.
- **Tooltips:** Belegte Schaltflächen zeigen das jeweilige Tastenkürzel zusätzlich zur Funktionsbeschreibung.

**Aus Build 58 übernommen:** Spotlight-CAN mit „Schnitt bis Videoende abschließen“, Rot-/Grün-Schnittmodus, gemeinsame Suche nach lokalen und Server-Cutlists, Cutlist-Bewertung, Bild- und Schnittvorschau sowie Kontrollplayer. otr-can behält die Audioprüfung, Tonspur-Abwahl und die ausdrücklich zu bestätigende experimentelle AAC-Zeitlückenreparatur. MP4Box fügt Segmente mit gleichartigen AAC-Tonspuren weiterhin über `-add`, `-cat` und `-new` zusammen. Keyframe-bedingte Werbeblitzer können bei MP4Box weiterhin auftreten.

## Installation

Windows 11 x64 und .NET 10 Desktop Runtime x64 vorausgesetzt. Für otr-can wird außerdem Microsoft Visual C++ v14 Redistributable x64 benötigt. Das Setup aktualisiert die vorhandene Installation. Für die portable Ausgabe das ZIP vollständig in ein frisches Verzeichnis entpacken und `CutAssistantNext.App.exe` starten.

Enthalten sind libmpv 0.41.0 und otr-can 0.1.6-audio-diag.1 unter `tools/otr-can/otr_can.exe`. FFmpeg, ffprobe, MP4Box und FFMS2 bleiben externe Werkzeuge. Bezugsquellen und Pfadeinstellungen stehen in `VORAUSSETZUNGEN.txt`. Die vollständigen Quellen, das libmpv-Relink-Archiv und SHA-256-Prüfsummen werden separat bereitgestellt.

## Prüfbasis

Jörg hat Tastatur- und Mauswiederholung, die erhaltene Schnittauswahl, den zentralen Lautstärkeregler, die Lautstärkeübernahme und die Tooltips mit dem Usability-Entwicklungsstand persönlich unter Windows 11 geprüft. Die persönliche Prüfung der neuen Build-59-Pakete steht aus. Windows 10 wurde nicht persönlich geprüft.

<details>
<summary>Release Notes zu Build 58 und früheren Fassungen</summary>

# Release Notes – Cut Assistant Next 1.0.3 Build 58

## Cut Assistant Next 1.0.3 Build 58 – vorbereiteter Release-Kandidat

[Build 58 wurde am 13.11.2026 veröffentlicht](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.3-build58).

- **MP4Box:** CAN importiert das erste Segment mit `-add`, hängt weitere mit `-cat` an und erzeugt die Ausgabe mit `-new`. Zuvor konnte GPAC bei gleichartigen AAC-Spuren beide Eingabespuren einer Zielspur zuordnen. Geprüft sind AAC, AAC+AC-3, AAC+AAC und AC-3+AC-3. Die bekannten Keyframe-Werbeblitzer können weiterhin auftreten. [Ursache, Paketvergleich und Praxistests](MP4BOX-WILD-CARD-ZUSAMMENFUEHRUNG-BUILD56.md)
- **Spotlight-CAN:** Einen offenen Entfernen-Bereich mit **Schnitt bis Videoende abschließen** bis zur tatsächlichen Dateidauer schließen. **Videoanfang** in CAN setzt die Grenze auf **00:00:00.000**. [Bedienung](SPOTLIGHT-README.md)
- **Cutlist-Bewertung:** Übernommene fremde Cutlists beim Wechsel oder Beenden freiwillig bewerten. Im persönlichen Test wurden die serverseitige Bewertung, ihre dauerhafte Anzeige und das Entfernen aus der Liste unbewerteter Downloads bestätigt. [Bewertungsablauf](CUTLIST-BEWERTUNG-BUILD55.md)
- **Schnittshuffle:** Einen vollständigen roten oder grünen Schnittbereich auswählen und mit **←** oder **→** zu seiner Grenze springen. Auswahl, Bearbeitungskante und gespeicherte Grenzen bleiben erhalten. [Schnittshuffle in den drei Ansichten](SCHNITTSHUFFLE-BUILD57.md)
- **Hauptplayer am Filmende:** Die Datei bleibt am tatsächlichen Ende geladen und der Player pausiert. Mit dem Slider zurück in den Film springen und Play betätigen. Der Anwender hat den Ablauf mit einem Realfilm in Build 58 bestätigt. [Fehleranalyse und Praxistest](PLAYER-FILMENDE-BUILD58.md)
- **Namensmaske:** Bei fremden Cutlists bleibt der übernommene Namensvorschlag zunächst erhalten. **Aus obigen Eingaben neu erzeugen** verwendet die aktuell gespeicherte Standardmaske. Bei eigenen Cutlists die gewünschten Namensbestandteile ausfüllen. Der Ausgabedateiname bleibt im Schneidedialog frei bearbeitbar.
- **Weiter verfügbar:** Rot/Grün-Modus, Power-User 2, lokale und serverseitige Cutlist-Suche, Frame-Lupe, Halbierungssuche sowie otr-can mit den bestehenden Optionen zur Tonspur-Abwahl und AAC-Reparatur.

**Prüfbasis.** Der Anwender hat die MP4Box-Schnitte und Cutlist-Bewertung in Build 56 sowie Schnittshuffle und die Playerkorrektur in Build 58 persönlich bestätigt. Die Paketprüfung wird im separaten Paketbericht dokumentiert. [Prüfhinweise](PRUEFHINWEISE-BUILD58.md)

**Pakete:** Windows-x64-Setup, Portable-ZIP, CAN-Quellen mit vollständigem Spotlight-CAN, passende Motorquellen, libmpv-Quellen und Relink-Archiv sowie Prüfsummen. Windows 11 x64, .NET 10 Desktop Runtime x64 und für otr-can Visual C++ v14 Redistributable x64 separat einrichten. Die Programme FFmpeg, ffprobe, MP4Box und FFMS2 bleiben extern.

<details>
<summary>Veröffentlichter Build 54 und frühere Prüfstände</summary>

# Release Notes – Cut Assistant Next 1.0.3 Build 54

Stabile Version vom 08.11.2026, persönlich freigegeben. Gegenüber 1.0.2 Build 48:

- **Spotlight-CAN integriert:** Werbeblöcke mit Vorschaubildern grob markieren, Original-/Entfernen-/Restdauer sehen und bestätigte Schnittbereiche direkt an CAN übergeben. Feinkontrolle mit Player und Frame-Lupe; den tatsächlichen letzten Frame bei Bedarf mit **Videoende** setzen. Keine automatische oder framegenaue Werbeerkennung.
- **Spotlight-Fenster merken:** Größe, Position und Maximierung bleiben auch nach CAN-Neustart erhalten. Negative Monitorpositionen werden unterstützt; bei fehlendem Monitor, anderer Auflösung oder DPI-Skalierung wird das Fenster sichtbar korrigiert.
- **Audiofehler gezielt behandeln:** Normalmodus weiterhin strikt. Bei einer Mehrspurdatei kann eine nachweislich beschädigte Spur nach Prüfung der verbleibenden Spuren ausdrücklich abgewählt werden. Abbrechen und bestätigter vollständiger MP4Box-Neustart bleiben verfügbar.
- **AAC-Zeitlücken reparieren (experimentell):** Nur für unterstützte, nachgewiesene Lücken und ausschließlich nach ausdrücklicher Zustimmung. Positionsgenaue Stille, vollständiger neuer Schnitt und unabhängige Bild-/Ton-/Containerprüfung. Keine allgemeine Sampletoleranz und keine stillschweigende Korrektur; verlorene Originaltöne werden nicht wiederhergestellt. Bei Fehlschlag kontrollierter Abbruch.
- **Schnittbearbeitung:** Gemeinsame Server-/Lokal-Cutlist-Auswahl mit exaktem Originalnamen; Rot/Entfernen oder Grün/Behalten, dynamische Markierungsvorschau und dauerhaft wählbare Ansichten Standard/Power-User 1/Power-User 2. Bestehende Cutlists und manuelle Feinarbeit bleiben nutzbar.
- **Stabilität und Werkzeuge:** Geprüfte Player-/Drei-Sekunden-Vorschau und Tonspurwechsel aus 1.0.2 bleiben erhalten. Die abgesicherten Audio-Neustarts verwenden bestätigte Schnittbereiche und bestehende Werkzeugkonfiguration. Der passende mitgelieferte Motor wird einheitlich aus **tools/otr-can/otr_can.exe** gewählt, ohne gespeicherte Pfade zu überschreiben; fehlt er, klarer Fehler statt stiller Verwendung einer alten EXE.

## Einrichtung und Grenzen

Windows 11 x64, **.NET 10 Desktop Runtime x64** sowie für otr-can **Visual C++ v14 Redistributable x64** separat erforderlich. Enthalten: libmpv **0.41.0**, otr-can **0.1.6-audio-diag.1** samt Quellen-/Lizenznachweisen. FFmpeg/ffprobe, GPAC/MP4Box und FFMS2/ffmsindex separat einrichten. FFmpeg/ffprobe müssen neuer als 5.1 sein; ab 9.0.2 empfohlen, Git-Entwicklungsbuilds werden anhand ihres Bibliotheksstands geprüft.

Setup oder vollständige Portable-ZIP verwenden; die ZIP in einen frischen Ordner entpacken. Beide Varianten benötigen die externen Laufzeiten und verwenden CANs Einstellungen im lokalen Windows-Benutzerprofil. Bei Überinstallation können alte runtime-Dateien bestehen bleiben; der Motor wird ausschließlich aus tools/otr-can verwendet.

AVI bleibt experimentell, bekannte MP4Box-/Werbeblitzer-Grenzen bleiben bestehen. QUICK AND COPY kann Zeitstempel-/Laufzeithinweise ausgeben und führt keine samplegenaue Tonprüfung aus. Bild, jede Tonspur, Übergänge und Drei-Sekunden-Vorschau persönlich kontrollieren. Windows 10 wurde nicht persönlich validiert.

## Prüfungen und persönliche Abnahme

Build-54-Nachweis: **1857 CAN-Tests, 78 Spotlight-Tests, 32 native Spotlight-, 32 Kontrollplayer-Prüfungen, vier Fensterprozesse, 20 native Audiofälle und 13 Motor-Paketprüfungen**, jeweils erfolgreich; Build ohne Warnungen/Fehler. Schlussprüfungen und Binärzuordnung für die Veröffentlichung werden im Veröffentlichungsnachweis ergänzt; Wiederholungen zählen nicht als zusätzliche unterschiedliche Tests.

**Cut Assistant Next 1.0.3 Build 54 wurde persönlich erfolgreich geprüft.** Setup in der Windows-VM: regulärer Grey’s-Anatomy-Schnitt sowie NCIS Sydney – Breathless, S02E07 mit ausdrücklich bestätigter experimenteller AAC-Reparatur; Bild, Ton und Drei-Sekunden-Vorschau positiv. Die frühere Build-53-Abnahme einschließlich Setup/ZIP, Spotlight-Fenster/-Übergabe, Mehrspur-/Spur-Abwahl und beiden Klebecenter-Wegen bleibt diesem ursprünglichen Paketsatz zugeordnet. Automatisierte Nachweise und persönliche Rückmeldungen sind getrennt dokumentiert.

## Pakete und Quellen

[Release und Downloads](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.3-build54): Setup, Portable-ZIP, vollständiges CAN-Quellarchiv einschließlich angeheftetem Spotlight-CAN und passendem Motorquellarchiv, separates Motorquellarchiv, unveränderte libmpv-Quellen/Relink sowie beide Prüfsummenmanifeste. App-Dateiversion **1.0.3.54**, Tag **v1.0.3-build54**. Die persönlich geprüften Programmdateien bleiben bei der dokumentarischen Veröffentlichung unverändert; Quellenarchiv und Provenienz ordnen den finalen Dokumentationsstand eindeutig zu.

[Anleitung](NUTZERANLEITUNG.md) · [Spotlight](SPOTLIGHT-README.md) · [Herkunft, Abnahme und Veröffentlichung](RELEASE-1.0.3-BUILD54-PUBLISHED-20261108.md) · [Build 48 bleibt erhalten](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.2-build48).

<details>
<summary>Historische Entwicklungs- und Prüfstände vor Veröffentlichung</summary>

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

</details>

</details>

</details>
</details>

Die geplante EAC3-Motorerweiterung, das Batchcenter und Skriptunterstützung in Namensmaske II sind künftige Vorhaben außerhalb des Build-61-Funktionsumfangs.
