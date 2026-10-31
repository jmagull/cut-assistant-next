# Testplan – aktueller Funktionsstand

**Stand 31.10.2026: geprüfter und eingefrorener Build 47 zur Veröffentlichung als v1.0.2-build47 freigegeben; frühere Releases/Pakete bleiben erhalten, Motor 0.1.6.** [Versionen](VERSIONEN.md), [Paketbericht](RELEASE-1.0.2-BUILD47-RESULT.md).

## Freigabe zum Einfrieren und Veröffentlichen am 31.10.2026

Der Nutzer beauftragt ausdrücklich, den bestätigten Stand einzufrieren, README/Dokumentation abzugleichen, zu committen, zu pushen und zu veröffentlichen. Veröffentlicht wird der unveränderte geprüfte Build-47-Paketsatz unter **v1.0.2-build47**; ältere Releases bleiben erhalten. Die bisherigen Aussagen über noch fehlende Veröffentlichungsfreigaben beschreiben den damaligen Paketierungsstand und sind durch diesen Auftrag abgelöst. Kein bisher nicht gemeldeter persönlicher Test wird dadurch als bestanden ergänzt. [Veröffentlichungsnachweis](RELEASE-1.0.2-BUILD47-PUBLISHED-20261031.md).

## Aktuelle Praxistests am 31.10.2026

**Brilliant Minds S01E11 – Die andere Frau, 5plus HQ – Build 46 / Setup / Arbeitsrechner:** Cutlist mit 25 fps, MP4Box 26.07 und drei Behaltebereichen. Protokoll bestätigt drei geschnittene und zusammengefügte Segmente, alle drei Schnittstellenprüfungen und Abschluss „Fertig.“; an Übergang 1 gleicher Bildzeitpunkt, dekodierter neuer Anfang zusätzlich geprüft. Nutzer bestätigt **Bild, Ton, Schnitt und Drei-Sekunden-Vorschau**. Dritter aktueller persönlicher Praxisfall nach Rocky Horror und Doc. [Nachweis](PRAXISTESTS-20261031.md#fall-3-brilliant-minds-s01e11--die-andere-frau-5plus-hq-25-fps).

**Doc S02E18 – Orientierung, 5plus HQ – Build 46, Setup auf dem Arbeitsrechner (laufende Umgebung fortgeführt):** Cutlist mit 25 fps, vorgesehenem Motor MP4Box 26.07 und zwei Behaltebereichen **584,200–1925,240 s** / **2618,560–3768,720 s**. Geplante Gesamtdauer **00:41:31.200**, rechnerischer innerer Übergang **00:22:21.040**; keine gemessenen Ausgabezeiten. Nutzer bestätigt **Bild, Ton, Schnitt und Drei-Sekunden-Vorschau**. Cutlist-Kommentar vermerkt vollständig entfernte Werbung. [Praxisbericht](PRAXISTESTS-20261031.md#fall-2-doc-s02e18--orientierung-5plus-hq-25-fps).

**The Rocky Horror Picture Show, arte HD – Build 46, Setup auf dem Arbeitsrechner (vom Nutzer präzisiert):** Cutlist nennt CAN 1.0.2, 50 fps und vorgesehenen Motor otr-can 0.1.6, einen Behaltebereich **374,300–6094,960 s** mit **5720,660 s** Dauer. Geplant 286033 Bilder bei 50 fps, keine gemessene Ausgabebildanzahl. Nutzer bestätigt **Ton, Bild, Schnitt und Drei-Sekunden-Vorschau**. Laut Cutlist-Kommentar keine Werbung und arte-Vorspann-Blitzer nicht mehr sichtbar. Getrennt von Build-47-VM/Portable dokumentiert; keine vollständige Paket-Endabnahme. [Praxisbericht](PRAXISTESTS-20261031.md#fall-1-the-rocky-horror-picture-show-arte-hd-50-fps).

## Portable-ZIP Build 47 in der VM begonnen

**Weitere Nachweise:** Furiosa-MP4Box-Sperre persönlich per Screenshot bestätigt. Neuer QUICK-Lauf nach ausdrücklicher Nutzerklärung aus **otr-can-Teilen**: AAC und AC-3 erhalten, **00:00:41.93**, Ton/Bild/Schnitt/Drei-Sekunden-Vorschau persönlich bestanden; Codec-Framegrößen- und Audio-DTS-Warnung in der Meckerecke erhalten. Angehängter eigener Furiosa-otr-can-AUDIO-ENTHUSIAST-Lauf mit AAC/AAC technisch erfolgreich in **00:05:26.37**, **387448320 Samples**, Klebestelle **01:54:18.320**, Dauer **02:14:31.840**. Persönliche Kontrolle des letzten Laufs nicht separat zugeordnet. Drei Portable-Läufe dokumentiert, ZIP-Endabnahme weiter offen. [Fälle und Abnahmestand](VM-PORTABLE-BUILD47-20261031.md#abnahmestand).

Nutzer wechselt mit „jetzt zip“ zur getrennten Portable-Prüfung. Erster Fall John Wick Kapitel 4 aus zwei MP4Box-Teilen: Sperrhinweis für AUDIO-ENTHUSIAST im Screenshot sichtbar, QUICK mit AAC und AC-3 ohne Neukodierung erfolgreich in **28,6188598 s** (Anzeige **00:00:28.61**). Ausgabeübernahme und Bereinigung erfolgreich; Warnung/Laufzeitunterschiede in der Meckerecke erhalten. **Ton, Bild und Drei-Sekunden-Vorschau persönlich bestanden.** Weitere Portable-Fälle und ZIP-Endabnahme offen, keine Setup-Deinstallation daraus abgeleitet. [Portable-Bericht](VM-PORTABLE-BUILD47-20261031.md).

## VM-Setup Build 47: drei bestätigte Verfahrenswege

**Fortschreibung: drei Verarbeitungsläufe persönlich positiv.** Der Nutzer hält ausdrücklich fest: **MP4Box → QUICK AND COPY; otr-can → QUICK AND COPY sowie AUDIO-ENTHUSIAST.** Neuer otr-can-QUICK-Lauf mit AAC und AC-3 in **00:00:42.79**, Bild/Ton/Drei-Sekunden-Vorschau bestätigt; Codec-Framegrößen- und Audio-DTS-Warnung in der Meckerecke erhalten. Furiosa-otr-can mit AUDIO-ENTHUSIAST und AAC/AAC in **00:05:37.95**: **387448320 Samples**, Klebestelle **01:54:18.320**, Dauer **02:14:31.840**, technische Prüfungen vollständig positiv, Bild/Ton persönlich bestätigt. Deinstallation, Portable und vollständige Endabnahme bleiben separat offen. [Fälle und Zuordnung](VM-SETUP-BUILD47-20261031.md#vom-nutzer-festgehaltene-verfahrenszuordnung).

Zusätzlicher Dialogcheck mit Furiosa: Wechsel von zwei MP4Box-Teilen zu zwei otr-can-Teilen lässt den Sperrhinweis wie vorgesehen verschwinden, mit zwei Screenshots persönlich bestätigt. Kein neuer Verarbeitungslauf gezählt. Die eigene Tonspurauswahl für AUDIO-ENTHUSIAST bleibt eine zusätzliche Startvoraussetzung. [Dialognachweis](VM-SETUP-BUILD47-20261031.md#zusätzlicher-dialogcheck-von-mp4box-zu-otr-can-wechseln).

MP4Box-Sperre für AUDIO-ENTHUSIAST persönlich bestätigt. QUICK AND COPY mit AAC und AC-3 (jeweils Deutsch/Stereo/48000 Hz) technisch erfolgreich, Gesamt **27,8317790 s** (Anzeige **00:00:27.83**). Nutzer bestätigt QUICK, Bild, Ton und Drei-Sekunden-Vorschau; Ausgabeübernahme und Bereinigung erfolgreich, Werkzeugwarnung und Laufzeitunterschiede in der Meckerecke erhalten. Weitere VM-Fälle und Endabnahme sowie Deinstallation/Portable-ZIP offen. [Laufender VM-Bericht](VM-SETUP-BUILD47-20261031.md).

## Build-47-Paketprüfung

1593 Pakettests und zusätzlich 1321 Tests gegen finale Produkt-DLLs (934 App/387 Media) positiv, keine übersprungen. 705 Quellenmanifest-Dateien/706 Quellen-ZIP-Dateien und 250 Portable-Nutzdateien vollständig geprüft, Release ohne Warnungen/Fehler, native Herkunfts-/Dialogprüfung und zwei Fenstergrößen positiv. Persönlicher John-Wick-MP4Box-Dialogcheck bestanden. Nächster Schritt: neue Setup-Installation, Portable-ZIP und VM-Funktionsprüfung getrennt bestätigen. Die neue Paketabnahme ist durch den Entwicklungsdialogcheck noch nicht abgeschlossen. [Paketbericht](RELEASE-1.0.2-BUILD47-RESULT.md).

## Entwicklungsbuild 47: MP4Box-Sperre für AUDIO-ENTHUSIAST

**1593 Tests bestanden:** 200 Core, 72 Cutlists, 387 Media, 934 App. 17 neue Erkennungs-/Dienst-/Dialogfälle, native Prüfung realer Furiosa-MP4Box-Teile und otr-can-Vergleich sowie visuelle Prüfung zweier Dialoggrößen positiv. Persönlich noch zu prüfen: MP4Box in beiden Positionen und gemischte Paare sperren AUDIO-ENTHUSIAST samt Tonspurauswahl; QUICK bleibt nach bestätigter Zuordnung nutzbar. Beim Tauschen bleibt die Sperre richtig zugeordnet, beim Ersetzen durch unmarkierte Dateien wird sie aufgehoben. Hinweis muss auch im kleinen Fenster sichtbar bleiben. Herkunft ohne GPAC-Kennung unbekannt, bestehende Medienprüfungen bleiben aktiv. [Umsetzung und Prüfgrenzen](MP4BOX-AUDIO-ENTHUSIAST-SPERRE-20261031.md).

## VM-Setup Build 46 am 31.10.2026

**Separater lokaler Furiosa-Nachweis:** AUDIO-ENTHUSIAST lehnt die im neuen Screenshot benannten MP4Box-Dateien schon an der Bildvorprüfung ab. Metadaten direkt gelesen: nominell 25 fps, mittlere Raten ≈ 24,999561/24,999216; zusätzlich AAC-Dauerabweichungen ≈ 21,271/14,083 ms und unterschiedliche H.264-Konfigurationshashes. Kein zusätzlicher positiver VM-Fall. [Details und Prüfgrenzen](FURIOSA-MP4BOX-AUDIO-ENTHUSIAST-20261031.md).

**Offener Befund:** Zwei MP4Box-Teile werden vor dem AUDIO-ENTHUSIAST-Start mit der blauen Meldung zu identischen Bild-/Tonzeiträumen ab null abgewiesen. Nutzer bestätigt, dass kein Lauf startet. Exakte Start-/Endzeiten des konkreten Dateipaars noch zu ermitteln; die früher gemeldeten 40-ms-Unterschiede der MP4Box-Teile sind ein Hinweis, kein neuer Dateinachweis. Drei bisherige Erfolge bleiben erhalten, kein vierter Erfolg gezählt. [Diagnosestand](VM-SETUP-BUILD46-20261031.md#offener-befund-zwei-mp4box-teile-mit-audio-enthusiast).

**Dritter Funktionsfall: QUICK AND COPY mit einer AAC-Spur**, John Wick Kapitel 4 Teil 1/2, jeweils Deutsch/Stereo/48000 Hz. Vorbereitung beider gewählter Spuren und anschließendes Concat ohne Neukodierung erfolgreich, Gesamt **00:00:28.88**. Ausgabeübernahme und Bereinigung erfolgreich. FFmpeg korrigiert einen nicht monotonen Audio-DTS; Warnung und Laufzeitunterschied bleiben in der Meckerecke. Nutzer bestätigt **Ton, Bild und Drei-Sekunden-Vorschau**. Drei persönliche Funktionsfälle und Installation positiv, VM-Prüfrunde weiterhin laufend. [Nachweis](VM-SETUP-BUILD46-20261031.md#fall-3-quick-and-copy-mit-einer-aac-spur).

**Zweiter Funktionsfall: AUDIO-ENTHUSIAST**, John Wick Kapitel 4 Teil 1/2, jeweils AAC/Deutsch/Stereo/48000 Hz. Bild unverändert, Ton samplegenau verbunden und einmal durchgehend als AAC-LC kodiert; alle technischen Prüfungen einschließlich Klebestellendekodierung, Ausgabeübernahme und Bereinigung erfolgreich. **457399680 Tonsamples**, Klebestelle **01:57:37.120**, Gesamtdauer **02:38:49.160**, Verarbeitung **00:05:54.41**. Nutzer bestätigt **Bild, Ton und Drei-Sekunden-Vorschau**. Damit zwei persönliche Funktionsfälle; VM-Prüfrunde weiterhin laufend. [Nachweis](VM-SETUP-BUILD46-20261031.md#fall-2-audio-enthusiast-mit-john-wick-kapitel-4).

Setup-Installation persönlich bestanden; Screenshot zeigt CAN 1.0.2 Build 46. Erster Funktionsfall: QUICK AND COPY mit bestätigter AAC-/AC-3-Mehrspurzuordnung (jeweils Deutsch/Stereo/48000 Hz), keine entfallenden Spuren, keine Neukodierung. Gesamt **30,0369156 s**, Anzeige **00:00:30.03**. Ausgabeprüfung, Übernahme und Bereinigung laut Protokoll erfolgreich; Meckerecke bewahrt Werkzeugwarnung und Laufzeitunterschiede. **Ton, Bild, Schnitt/Klebestelle und Drei-Sekunden-Vorschau persönlich bestanden.** Nutzer testet weiter; weitere VM-Fälle, Deinstallation und Portable-ZIP nicht vorweggenommen. [Laufender Bericht und Messwerte](VM-SETUP-BUILD46-20261031.md).

## Persönliche Schnittprüfung am 30.10.2026

Der Nutzer bestätigt ausdrücklich: **Bild, Ton, Schnittübergänge und Drei-Sekunden-Vorschau bei OTR-CAN und MP4Box erfolgreich geprüft.**

| Schnittmotor | Persönliches Ergebnis | Technischer Nachweis |
|---|---|---|
| otr-can 0.1.6 | Alle vier Prüfpunkte bestanden | Übermitteltes Erfolgsprotokoll zu „Das Weihnachtsschloss“, HQ-MP4 vom 29.10.2026: fünf Behaltebereiche, 134128 Videopakete; AAC-Stereo, 48000 Hz, 257525760 verifizierte Tonsamples. Finale MP4 einschließlich Bild-/Tonpaketen, PTS/DTS, Padding und Decoderkonfigurationen verifiziert. |
| MP4Box | Dieselben vier Prüfpunkte bestanden | Persönliche Bestätigung; kein separates MP4Box-Protokoll übermittelt. |

Vor dem otr-can-Aufruf wurden 194955 eindeutige, aufsteigende Bildzeiten und die 25/50-fps-Zeitstruktur geprüft. CAN-Buildnummer und Installationsvariante sind im übermittelten Protokoll nicht angegeben. Das Ergebnis bestätigt die gemeldeten Schnittprüfungen; eine vollständige Setup-/Portable-/VM-Abnahme oder Veröffentlichung ist damit nicht beauftragt.

### Weiterer Fall: John Wick – Kapitel 3

HD-MP4 vom 20.10.2026, sechs Behaltebereiche. Der Nutzer bestätigt für **beide Schnittmotoren Schnitt, Ton, Bild und Drei-Sekunden-Vorschau als bestanden**.

| Schnittmotor | Technischer Nachweis aus den übermittelten Protokollen |
|---|---|
| otr-can 0.1.6 | 352507 eindeutige, aufsteigende Bildzeiten mit bestätigter 25/50-fps-Zeitstruktur; 183126 Videopakete einschließlich roher PTS/DTS verifiziert. Zwei vollständige Tonspuren einmal durchgehend kodiert: AAC und AC-3, jeweils Stereo/48000 Hz und 351601920 verifizierte decodierte Samples mit kontinuierlichen rohen PTS. Finale MP4 einschließlich aller Bild-/Tonpakete, PTS/DTS, Padding und Decoderkonfigurationen verifiziert. |
| MP4Box | Sechs Abschnitte geschnitten und zusammengefügt; Schnittstellenprüfung aller sechs Abschnitte mit „Fertig.“ abgeschlossen. An Übergang 2 und 3 jeweils gleiche Bildzeit am Abschnittswechsel gemeldet und der dekodierte neue Anfang zusätzlich geprüft. |

Eigenständiger weiterer Praxisfall nach „Das Weihnachtsschloss“. CAN-Buildnummer und Installationsvariante sind weiterhin nicht angegeben. Die persönliche Bestätigung nennt Ton allgemein; eine gesonderte Hörbewertung jeder einzelnen Tonspur wird daraus nicht abgeleitet.

## Aktuelle Build-46-Prüfung

- Entwicklung: 1576 Tests, 27 native QUICK- und 21 bisherige Enthusiast-Fälle bestanden. Paket-DLLs nach Publish erneut gezielt prüfen.
- QUICK: beide kompatiblen AAC-/AC-3-Spuren automatisch vorschlagen, bestätigen und erhalten; unterschiedliche Streamanordnung und bewusste Einzelwahl prüfen. Fehlende/doppelte Zuordnung, unpassende Formate und Überschreibversuche verhindern.
- QUICK-Warnungen einschließlich unbekannter FFmpeg-Meldungen und vorhandener Zeitstempel-/H.264-/Laufzeitabweichungen nur dokumentieren. Meckerecke vorhanden; keine zusätzlichen Vollprüfungen und keine Neukodierung. Erfolgreiche lesbare Ausgabe trotz Hinweisen akzeptieren.
- Echte Prozessfehler, leere/unlesbare oder falsch zugeordnete Ausgabe und Abbruch weiterhin negativ testen. Originale erhalten, bei Erfolg aufräumen, bei Fehler Diagnose behalten.
- AUDIO-ENTHUSIAST: bisherige strenge Prüfungen, RF64 und AAC-Neukodierung unverändert; neun erfolgreiche Ausgaben bytegleich zur Build-45-Referenz. Keine automatische Umstellung.
- Kompiliertes Menü, Paarbestätigung/-änderung/-tausch, Textumbruch, beide Methoden und Kontrollplayer mit tatsächlicher Tonspurumschaltung prüfen.
- Persönlich positiv: Furiosa und John Wick aus jeweils neuen MP4Box-Teilen, beide AAC+AC-3, Bild/Ton/Klebestelle/Drei-Sekunden-Vorschau; 23,74 bzw. 16,44 s. Weitere automatisch erzeugte Referenzen und neue Setup-/ZIP-/VM-Abnahme getrennt halten.

[Entwicklung und persönliche Praxis](KLEBECENTER-QUICK-PRAGMATISCH-20261029.md), [Freeze](FREEZE-1.0.2-BUILD46-20261029.md).

## Historische technische Basis Build 41

**1489/1489 Pakettests**, zusätzlich **925 App-/292 Medientests** gegen finale DLLs, 0 Warnungen/Fehler. Build 41 ergänzt nur die Bereinigung nach Erfolg und vier Regressionstests. 21 native Klebecenter-Fälle mit finalen DLLs prüfen Erfolg, Fehler und Abbruch samt Aufräumregel; zehn frühere Notfall-/Kopiefälle stammen aus Build 39 und wurden für diese Korrektur nicht erneut ausgeführt.

## Persönliche Abnahme

| Bereich | Bestätigt | Noch offen |
|---|---|---|
| AAC-Notfallweg | eigener John-Wick-Teil-2-Schnitt nach Rückfrage | neue Fehler-/Abbruchvarianten |
| Klebecenter AC-3/AAC | vollständiger eigener Lauf, Prüfdialog, Bild/Ton/Übergang | keine allgemeine Formatfreigabe |
| Klebecenter AAC/AAC | zweiter eigener Lauf mit Erfolgsprotokoll und „Auch hier alles prima“ | kein Schluss auf alle Installationsvarianten |
| Arbeits-PC-Setup Build 40 | nach Umstieg beide Furiosa-Schnitte und Klebecenter samt allen Prüfstellen positiv | zusätzliche Installations-/Deinstallationsvarianten |
| Furiosa, große Aufnahme | zwei otr-can-Schnitte, AAC/AAC-Kleben und alle Prüfstellen positiv | kein Nachweis einer behobenen Freeze-Ursache |
| Arbeits-PC-Setup Build 41 | John Wick: temporäre Datei weg, Bild/Ton/Schnitt/Drei-Sekunden-Vorschau und Klebecenter positiv | neue ZIP-/VM- und persönliche Fehler-/Abbruchfälle separat |
| Build-40-Hinweis | Screenshot zeigt korrekt otr-can 0.1.6 | kein Beleg für alle Installationsschritte |
| VM-Setup, Fall 1 | Die Teufelsbrigade AVI mit MP4Box, Ton/Schnitt/Bild/3-Sekunden-Vorschau positiv | Fall abgeschlossen; anschließende Deinstallation bestätigt |
| VM-Setup, Fall 2 | gleiche AVI mit otr-can 0.1.6, MP3→AAC und Ton/Bild/Schnitt/3-Sekunden-Vorschau positiv | Fall abgeschlossen; anschließende Deinstallation bestätigt |
| VM-Setup, Fall 3 | Grey’s Anatomy MP4, MP4Box mit drei Abschnitten; Ton/Bild/Schnitt/3-Sekunden-Vorschau positiv | Fall abgeschlossen; anschließende Deinstallation bestätigt |
| VM-Setup, Fall 4 | gleiche MP4 mit otr-can 0.1.6, drei Abschnitte; Ton/Bild/Schnitt/3-Sekunden-Vorschau positiv | Fall abgeschlossen; anschließende Deinstallation bestätigt |
| VM-Setup, Fall 5 | Furiosa HD mit MP4Box, fünf Abschnitte, kleine Bildzeitüberlappung am ersten Übergang zugeordnet; Bild/Ton/Schnitt/3-Sekunden-Vorschau positiv | Fall abgeschlossen; anschließende Deinstallation bestätigt |
| VM-Setup, Fall 6 | Furiosa HD mit otr-can 0.1.6, fünf Abschnitte, AAC und AC-3 technisch geprüft; Bild/Ton/Schnitt/3-Sekunden-Vorschau positiv | Fall abgeschlossen; anschließende Deinstallation bestätigt |
| VM-Setup, Fall 7 | Furiosa Teil 2 HD mit MP4Box, zwei Abschnitte; Bild/Schnitt/Ton/3-Sekunden-Vorschau positiv | Fall abgeschlossen; anschließende Deinstallation bestätigt |
| VM-Setup, Fall 8 | Furiosa Teil 2 HD mit otr-can 0.1.6, zwei Abschnitte, AAC/AC-3 technisch geprüft; Bild/Schnitt/Ton/3-Sekunden-Vorschau positiv | Fall abgeschlossen; anschließende Deinstallation bestätigt |
| VM-Setup, Fall 9 | Furiosa-Klebecenter AAC/AAC, Bild/Ton/Klebestelle/3-Sekunden-Vorschau positiv; 387448320 Samples | Fall abgeschlossen; anschließende Deinstallation bestätigt |
| VM-Deinstallation | Nutzer bestätigt Deinstallation; protocol.txt im Klebe-Arbeitsordner bleibt gemäß aktueller Regel | in Build 41 technisch korrigiert; persönliche Prüfung offen |
| Portable Build 40, Fälle 1–3 | Grey’s Anatomy/MP4Box, Teufelsbrigade AVI/otr-can und Furiosa-Klebecenter AAC/AAC; jeweilige Bild-/Ton-/Schnitt-/Klebestellen-/Vorschauchecks positiv; „damit zip check“ | persönliche ZIP-Prüfung abgeschlossen; weitere Fehlerfälle nach Bedarf |

## Klebecenter gezielt prüfen

- Filmteile im Klebecenter wählen; Reihenfolge vor Tonspurwahl. Beim Tausch folgt die Auswahl der Datei.
- Keine Vorauswahl bei mehreren Spuren; entfallende Spuren/AAC-Neukodierung sichtbar. AAC/AAC sowie AC-3/AAC bei passenden Kanälen/Abtastraten prüfen.
- Erfolgsdialog mit Anfang, tatsächlicher Klebestelle und Ende. Drei Sekunden Vorlauf; Bild, Ton und Synchronität kontrollieren.
- Furiosa: **387448320 Tonsamples**, **02:14:31.840** Dauer, **01:54:18.320** Klebestelle, Vorlauf **01:54:15.320**. [Praxisbericht](FURIOSA-KLEBECENTER-PRAXIS-20261027.md).
- John Wick: **238229 Videopakete**, **457399680 Tonsamples**, **02:38:49.160** Dauer, **01:57:37.120** Klebestelle, Vorlauf **01:57:34.120**.
- Negative Fälle: fehlende/unpassende Spur oder Datei, inkompatibles Video, Decoderwarnung, Quelltonlücken/Überlappungen, geänderte Pakete, falsche Samplezahlen, Abbruch, vorhandenes Ziel/Zieldatei-Rennen und Platzmangel. Nur sichere Ablehnung gilt als bestanden.
- Gewählte Spur vollständig prüfen; beschädigte entfallende Spur nicht dekodieren. Originale/Ziele erhalten, große Zwischenstände bereinigen; ab Build 41 Protokoll bei Fehler/Abbruch erhalten und nach Erfolg samt leerem Arbeitsordner entfernen.

## Paket-/Versionsprüfung

App **1.0.2.40**, tatsächliche Motor-EXE **otr_can 0.1.6**, Voraussetzungstext **0.1.6**. Quellen-/Portable-Manifeste, SHA-256, Laufzeit-/Lizenzhinweise gemäß Paketbericht. Die Mindestangabe „MP3 ab 0.1.5“ bleibt richtig.

## Historische Prüfstände bis 13.10.2026

Die nachfolgenden Zahlen, Paketmeldungen und datierten Einführungen beschreiben ihre früheren Prüfstände. Aktuelle Zahlen und offene Aufgaben stehen oben.

Stand des Abgleichs: 11.10.2026. Vollständiger Release-Testlauf für den lokalen **Build 13 RC3: 785/785 CAN-Tests**, 0 Warnungen/0 Fehler. Nativer Motor otr-can 0.1.4 unverändert, letzter nativer Prüfstand: **40/40 Rust-Tests**. Ältere Zahlen weiter unten gehören zu den ausdrücklich datierten Prüfständen.

Timeline-Klick ist seit 05.10.2026 persönlich abgenommen; die vier Schnittdialoge speichern ihre Größen und Maximierung und bestanden am 11.10.2026 den WPF-Neustarttest sowie die persönliche Prüfung. Der lokale Build-12-RC3-Paketsatz bestand Setup-, Überinstallations-, Windows-11-VM-, Schnitt-, Bild-/Ton-, Deinstallations- und separate Portable-Prüfungen; siehe [Paketprüfbericht](RELEASE-BUILD12-RC3-RESULT.md). Diese Punkte sind keine offenen September-Aufgaben mehr. Weitere Format-/Tonspurprüfungen bleiben Ausbauprüfungen mit den dokumentierten Grenzen.

## Credits – KI-Hinweis

- **Credits** im Hauptmenü öffnen und bis zum Abschnitt **KI-unterstützte Entwicklung** scrollen. Vollständiger Wortlaut aus [DANKSAGUNG.md](DANKSAGUNG.md), einschließlich Modellangaben und Entscheidungshoheit von Jörg.
- Normales Fenster und Mindestgröße 540 × 400 prüfen: Text umbrochen, bis zum Ende scrollbar, Schließen erreichbar. Keine persönlichen Einstellungen verändern.
- Diese Dialogergänzung gehört zum neuen lokalen Entwicklungsstand; die bereits abgenommenen Build-12-Pakete enthalten sie noch nicht.
- Automatischer echter WPF-Dialogtest und visuelle Kontrolle am 11.10.2026 bestanden: Absatz exakt mit DANKSAGUNG.md verglichen, 780 × 720 und 540 × 400 gerendert, Schließen erreichbar, keine Bindungsfehler; vorhandene OTR-Projektlinks und Handler ebenfalls geprüft. Belege in .build/credits-ai-20261011; keine neue persönliche Paketabnahme für Build 13.

## Testumgebung

Build 13 RC3 wurde am 11.10.2026 ausdrücklich zur sofortigen Veröffentlichung freigegeben. Sieben Release-Uploads vor/nach Freigabe nach Größe und GitHub-SHA-256-Digest geprüft; Tag und Quellcommit stimmen. [Veröffentlichungsnachweis](RELEASE-PUBLICATION-BUILD13-RC3.md). Dies ergänzt die technischen Paketprüfungen und behauptet keine neue persönliche Installationsabnahme.

Neuer Build-13-Paketsatz am 11.10.2026 technisch geprüft: gesperrter Restore und 785/785 Release-Tests aus Commit c98041a, alle 249 entpackten Portable-Dateien, Credits und Hauptfenster einschließlich libmpv sowie künstlicher Zwei-Bereich-Schnitt mit 90 decodierten Bildern und AAC-Stereo/AC-3-5.1. Schnittausgabe bytegleich zu Build 12, vollständiger Decoderlauf bestanden. Keine neue persönliche Setup-/Portable-Abnahme; [Prüfbericht](RELEASE-BUILD13-RC3-RESULT.md).

- Windows 11 x64
- Release-Build
- repräsentative MP4- und OTR-Dateien, einschließlich MP4-Inhalt mit Dateiendung `.avi`
- mpv/libmpv und ffprobe aus der vorgesehenen Laufzeitstruktur

## Manuelle Kernprüfungen

| Nr. | Prüfung | Erwartetes Ergebnis |
|---|---|---|
| 1 | Anwendung starten | Hauptfenster erscheint ohne Fehler |
| 2 | MP4 öffnen | Video wird im Hauptfenster geladen |
| 3 | Play/Pause | Wiedergabe reagiert zuverlässig |
| 4 | Zeitleiste anklicken und Regler ziehen | Klicks links/mittig/rechts auf Slider-Spur und graue/rote Schnitt-Timeline setzen Position und Slider auf die gewählte Stelle; rote Auswahl bleibt umschaltbar; Reglerziehen funktioniert weiter |
| 5 | Frame vorwärts | einzelner Bildschritt funktioniert |
| 6 | Frame rückwärts | stabiler Rückwärtsschritt |
| 7 | Lautstärke ändern | Ton wird angepasst |
| 8 | Medieninformationen | Dauer, Auflösung und Codecs erscheinen |
| 9 | Datei wechseln | Ressourcen werden sauber freigegeben |
| 10 | Fehlerfall | verständliche Meldung und Logeintrag |
| 11 | Tastatursteuerung | Play/Pause und Frame-Schritte reagieren zuverlässig |
| 12 | Schnittbereich setzen | Anfang und Ende werden korrekt übernommen |
| 13 | Mehrere Bereiche | Bereiche werden chronologisch verwaltet |
| 14 | Timeline-Auswahl | Tabelle und Timeline bleiben synchron |
| 15 | Bereich korrigieren | Anfang und Ende lassen sich korrigieren |
| 16 | Bereich löschen | ausgewählter Bereich wird entfernt |
| 17 | Fenster neu öffnen | Größe bzw. Maximierung wird wiederhergestellt |

## Namensmaskenwechsel ohne Neustart – 13.10.2026, Build 29 RC3

- Video/Cutlist laden, Namensmaske von S/E auf x ändern und speichern; ohne Neustart **Cutlist erzeugen → Aus obigen Eingaben neu erzeugen**. Erwartet aktuelle gespeicherte Maske, führende Null in Folge 07 und bearbeiteter Folgentitel erhalten. Wechsel zurück und wieder zu x ebenfalls prüfen.
- Bereits übernommener fremder/eigener Namensvorschlag bleibt beim Öffnen, Eingabeänderung und Speichern ohne Neuerzeugen erhalten. Bewusstes Neuerzeugen darf ihn ersetzen; manueller Filmname bleibt sonst unabhängig. Nach Filmnamens-Neuerzeugung muss der folgende Cutlist-Dialog dieselbe Maske verwenden.
- Cutlist speichern/wieder laden: Vorschlag, Originalzuordnung und Schnittbereiche unverändert. Bestätigt: **1076/1076 Tests**, **sechs WPF-/Speicherprüfungen**, Release ohne Warnungen/Fehler, keine Bindungsfehler und persönliche Einstellungen erhalten. [Prüfbericht](NAMENSMASKE-OHNE-NEUSTART-20261013.md). Persönliche Wiederholungsprüfung am 13.10.2026 bestanden: S13E07 → 13x07 → S13xxE07 bei gespeichertem Maskenwechsel ohne Neustart; alter Vorschlag bis zum bewussten Neuerzeugen erhalten.

## Richtungsabhängige Lupensuche / FFmpeg-Versionen – 13.10.2026, Build 27 RC3

- Suchfolge bei Startwert 2000: **+2000/+2000/−1000/−1000/+500**. Tatsächliche Bild-/Frameanzahl prüfen, Gegenrichtung verwendet bereits die Hälfte. Normale ±1/±10/±20 verändern Weite/Richtung nicht; Neustart setzt beides zurück. Keine Halbierung nach Fehler, Abbruch oder bewegungslosem Dateigrenzensprung. Beide Buttonwerte zeigen die jeweilige nächste Weite.
- **FFmpeg-Werkzeuge → Prüfen**: FFmpeg und ffprobe getrennt kontrollieren; 5.1/5.1.0 abweisen, 5.1.1 zulassen, 8.1.3 mit Empfehlung, 9.0.2/9.0.10/10.0 empfohlen. Unbekannte Versionsangaben, falsche Programme, fehlende Dateien/ungültige Pfade, Startfehler, Timeout und Abbruch prüfen. Git-Kennung mit passendem neuerem Build-/Laufzeit-Bibliotheksstand gesondert erkennen; keine exakte Releaseversion behaupten.
- Prüfung im Hintergrund, Pfade/Speichern/Prüfen währenddessen deaktiviert, Schließen bricht Prüfung ab; keine Einstellungen speichern. Pfadänderung löscht altes Ergebnis. Dialog bei 720×560/600×420: Status scrollbar, Buttons erreichbar. OTR-CAN-Werkzeugkontrolle verwendet dieselbe Regel.
- Bestätigt: **1069/1069 CAN-Tests**, keine übersprungen, **57 integrierte AVI-/Werkzeug-/WPF-Prüfungen** gegen Build 27, Release ohne Warnungen/Fehler, keine Bindungsfehler, Lockdateien und persönliche Einstellungen erhalten. [Prüfbericht](LUPEN-SUCHRICHTUNG-FFMPEG-20261013.md). FFmpeg-Versioncheck und Richtungswechsel-Halbsprung am 13.10.2026 persönlich positiv bestätigt; Essentials-Git-Build vom 12.10.2026 verwendet.

## AVI-Frame-Lupe – 13.10.2026, Build 26 RC3

- Tatsächlich erkanntes AVI ohne Original-PTS: vorhandenen rekonstruierten Bildzeitstempel prüfen, Bild laden, Status/Details müssen die Herkunft kennzeichnen; Übernahme nur mit passender Zeit-/Bildzuordnung innerhalb der Datei. AVI mit abweichender Endung verwendet denselben Weg; MP4 erhält keinen AVI-Fallback.
- ±1/±10, halbierte Suche mit Abschnittserweiterung, Dateianfang und bekannte Kanten prüfen. Übernahme ändert ausschließlich die gewählte Start-/Endkante, Containerstart weiterhin abziehen; Original-PTS behalten Vorrang. Abbrechen verändert keinen Schnittbereich.
- Fehlende rekonstruierte, doppelte/nicht aufsteigende Zeiten, falsches Bild/Zeitbasis, fehlgeschlagene/leere Vorschau, Bildanzeigefehler und Abbruch müssen Bild/Übernahme sperren. Paket-DTS und Bildrate/Framenummer liefern keinen Ersatzzeitpunkt.
- Automatisch bestätigt: **1018/1018 CAN-Tests**, darunter 19 neue Lupenfälle; **32 echte ffprobe-/FFmpeg-/WPF-Integrationsfälle** mit nummerierten AVI-/MP4-Quellen und Original-Teufelsbrigade, Bild, Navigation, Normal-/Mindestgröße und tatsächlichem Übernahmeklick, ohne Bindungsfehler. [Nachweise und Prüfgrenzen](AVI-FRAME-LUPE-20261013.md).
- Persönliche Bedienabnahme am 13.10.2026 positiv bestätigt: AVI-Lupe funktioniert, Teufelsbrigade-Schnitt mit MP4Box und otr-can jeweils ohne Blitzer. Suchsprung ausschließlich bei Richtungswechsel halbieren bleibt separat vorgemerkt. Rekonstruierte Zeiten bleiben Schätzwerte; keine allgemeine AVI-Reparatur oder vollständiger unabhängiger Bildabgleich zu mpv/Schnittmotoren behauptet. Die als untauglich bestätigte alte Aufnahme weiterhin ausgeschlossen.
## AVI-Originalhinweis – Stand 11.10.2026

- Tatsächlichen AVI-Container mit `.avi`, `.AVI`, `.avi.mp4` und ohne Endung laden: gelber Hinweis direkt unter dem Dateinamen; abweichende Endungen werden erklärt. Analyse bleibt erfolgreich, Wiedergabe und Cutlist-Bearbeitung bleiben verfügbar.
- Echte MP4 mit `.avi.mp4` oder `.avi` im Namen laden: kein AVI-Hinweis. Andere bzw. unbekannte Container erhalten ebenfalls keine AVI-Warnung.
- Nach AVI eine MP4 laden: alter Hinweis bereits während der neuen Analyse entfernt. Auch bei fehlgeschlagener oder abgebrochener Folgeanalyse kein veralteter Hinweis.
- Schmales Hauptfenster: Hinweistext vollständig umbrochen, Bearbeitungsbereich weiterhin über die Scrollleiste erreichbar.
- 18 neue automatisierte Fälle für echte Container, Endungen und Warnungslebenszyklus; **785/785 CAN-Tests** und Release-Build ohne Warnungen/Fehler. Echte WPF-Prüfung mit Testplayer und erfundenen Analysedaten: normale und kleine Ansicht, Dateiwechsel, kein Bindungsfehler und keine persönlichen Einstellungen geschrieben. Die untaugliche alte Aufnahme „Wo die Liebe hinfällt“ nicht für diese Prüfung verwendet.

## Getrennte Standardordner für Videos und Cutlists

- Unter **Einstellungen → Standardordner …** verschiedene vorhandene Ordner für Originalvideos, geschnittene Videos und eigene Cutlists wählen, speichern und CAN neu starten. Alle drei Werte bleiben erhalten.
- **Videodatei laden** beginnt im Originalordner; **Geschnittene Datei speichern** beginnt im Ausgabeordner und behält die Namensvorschau. Eine manuelle abweichende Auswahl ersetzt keine Standardordner-Einstellung.
- **Lokale Cutlist laden** und **Cutlist speichern** starten im Cutlist-Standardordner. Ohne Vorgabe oder bei nicht verfügbarem Ordner: Öffnen mit Windows-Ordnerwahl, Speichern im Ordner des Originalvideos. Der vorgeschlagene Cutlist-Dateiname bleibt erhalten.
- Nur einen Ordner festlegen, alle Felder leeren oder denselben Ordner für mehrere Zwecke verwenden: unabhängig möglich. Bestehende Konfiguration mit nur zwei Videoordnern bleibt gültig.
- Ungültiger, relativer oder nicht vorhandener Pfad: verständliche Meldung, Dialog bleibt offen. Ein nach dem Speichern entfernter Ordner oder nicht angeschlossenes Laufwerk verhindert die Dateiauswahl nicht; gespeicherte Vorgabe bleibt erhalten.
- **×** leert nur das zugehörige Feld. **Abbrechen** und Fensterkreuz speichern keine Änderungen. Speicherfehler sichtbar; erneuter Versuch möglich.
- Kleine Fenster, lange Pfade und Ordnerauswahl prüfen: Inhalt scrollbar und Speichern/Abbrechen erreichbar. Werkzeugdialoge behalten ihre bisherigen Startordner.

## Abschlussfenster nach dem Schneiden

- Erfolgreicher Schnitt: Nach Erstellung der Ausgabe und Aufräumen erscheint **Schneiden abgeschlossen**, ohne Countdown und ohne automatischen Playerstart.
- Entfernte Vor-/Nachläufe und mehrere Werbepausen: Kontrollzeiten beginnen bei 0 und summieren nur die behaltenen Abschnitte. Beispiel: 582,240–1902,960 s und 2551,320–3645,400 s ergeben einen Übergang bei **00:22:00.720** und ein Ende bei **00:40:14.800**.
- **Hinweise kopieren**: Dateipfad, sämtliche Kontrollstellen, Zeitbezug und Kontrollhinweis sind vollständig in der Zwischenablage; sichtbare Rückmeldung. Bei belegter Zwischenablage kann erneut kopiert werden.
- Original und Schnittliste bleiben geöffnet und bearbeitbar. Spätere Korrekturen verändern die Hinweise zum abgeschlossenen Schnitt nicht.
- Viele Übergänge, langer Dateiname und kleines Fenster: Inhalt scrollbar, beide Buttons erreichbar; Protokoll bei Bedarf aufklappbar und mit Strg+C kopierbar.
- Schließen per Button, Escape oder Fensterkreuz; kein automatisches Schließen.
- Abbruch oder Fehler: Fortschrittsfenster mit bisheriger Fehlermeldung, kein Erfolgsfenster.

## Automatisierte Tests

- Video-/Cutlist-Standardordner: unabhängiges Speichern/Laden, Neustart, leere oder beschädigte Einstellungen, fehlende Ordner, relative/ungültige Pfade, Dateien statt Ordnern, Speicherfehler und unveränderte Einstellungen beim Bearbeiten
- ffprobe-Parsing, Medienanalyse und Fehlerabbildung
- Frame-Lupe: Kantenauswahl, Halbierungsfolge, Intervallerweiterung anhand echter Frameanzahl, Dateigrenzen, fehlende/mehrdeutige PTS, Vorschaufehler und Abbruch
- Frame-Lupe-Übernahme: Original-PTS ohne impliziten Frame-/Keyframeversatz, Containerstart bei 0/+2/−2 s, variable Zeitabstände, gesperrter Button während Laden/nach Vorschaufehler; nur die gewählte Kante ändern; unveränderte/ungültige/veraltete Kanten, Überschneidungen und Export der korrigierten Keep-Bereiche
- PTS-Vorschau: absolute Quellzeitstempel, Zeitbasis-Prüfung und Ablehnung mehrdeutiger Bildausgabe
- Startwert der Frame-Lupe: Default 2000, Speichern/Laden, ungültige Werte und Speicherfehler
- `MainWindowViewModel` einschließlich Übernahme der Mediendauer
- `PlaybackViewModel` einschließlich Play/Pause, Seeking, Frames und Lautstärke
- `MpvMediaPlayerService` einschließlich Pausenzustand während des Ladens
- `RemoveSegment` einschließlich Bereichsvalidierung
- `CutPlan` einschließlich Sortierung, Überschneidungsschutz, Ersetzen und Löschen
- `CutPlanViewModel` einschließlich Erfassung, Auswahl und Korrekturmodus
- `CutCompletionViewModel`: Zeiten im geschnittenen Film, kumulierte Behaltezeiten, angrenzende Entfernbereiche, vollständiger Kopiertext, unabhängige Ergebnismomentaufnahme, Stunden über 24 und Millisekunden
- UTF-8-Protokollierung, Größenbegrenzung und Rotation
- `NameTemplateRenderer` einschließlich Namensvariablen, optionaler Präfixe, fehlender Werte und unbekannter Variablen
- `CutlistSettingsStore` einschließlich Standardwerten, Standardautor, Schnelltexten, Laden und Speichern sowie UTF-8 ohne BOM
- `TechnicalNoticeDetector` einschließlich des Sonderfalls `.avi` mit tatsächlich erkanntem MP4/ISO-BMFF-Container
- `CutlistKeepSegmentBuilder` einschließlich der Umrechnung von Remove- in Keep-Bereiche und der Behandlung von Randfällen
- Cutlist-Metadaten einschließlich `NoOfCuts`, Autor, Benutzerkommentar und technischer Hinweise
- `CutlistDocument` einschließlich Konsistenzprüfung zwischen Metadaten und Keep-Bereichen
- `CutlistSerializer` einschließlich klassischer Kompatibilitätszeilen, kulturunabhängiger Zahlenwerte und Golden-Master-Test
- `CutlistFileWriter` einschließlich UTF-8 ohne BOM, CRLF-Zeilenenden und Umlauten
- `CutlistParser` einschließlich des Einlesens klassischer Cutlist-Strukturen
- `CutlistFileReader` einschließlich UTF-8 und historischem Windows-1252-/ANSI-Fallback
- `CutlistGenerationViewModel` einschließlich Herkunftsübernahme, Duplikatschutz, fehlendem beziehungsweise eigenem Autor und Erhalt vorhandener Benutzerkommentare

Historischer vollständiger Testlauf vom 19.09.2026: **450 von 450 Tests erfolgreich** (Debug-Build). Der damalige offene Release-Test ist durch die später dokumentierten Release-Läufe abgelöst; aktueller Prüfstand steht oben.

Zusätzlich wurde die lokale Cutlist-Dateiausgabe in einem Smoke-Test praktisch geprüft. Dabei wurde eine vollständige `.cutlist`-Datei erzeugt und anschließend explizit als UTF-8 eingelesen; auch Umlaute wurden korrekt erhalten.

Cutlist-Reader und Parser sind umgesetzt und automatisiert getestet. Beim Einlesen wird zunächst UTF-8 verwendet; historische Windows-1252-/ANSI-Cutlists werden als Kompatibilitätsfall unterstützt.

## P1 – Cutlist-Bedienung, praktisch abgenommen am 19.09.2026

| Prüfung | Ergebnis |
|---|---|
| Fremde Namensvorschau übernehmen | Ursprünglicher Vorschlag bleibt beim Ändern der Namensfelder erhalten |
| „Aus obigen Eingaben neu erzeugen“ | Bewusste Umschaltung auf die eigene Namensmaske funktioniert; danach aktualisiert sich die Vorschau |
| Herkunftshinweis bei Fremdautor | Wird automatisch ergänzt; eigener Autor bleibt erhalten |
| Wiederholtes Öffnen und fehlender Autor | Keine doppelten Hinweise und keine leeren Herkunftsangaben |
| Zeitdarstellung | Anfang, Ende und Dauer einheitlich mit drei Nachkommastellen |
| Namensvorschau kopieren | Markierung und Strg+C praktisch geprüft; Vorschau aktualisiert sich weiterhin bei Änderungen |

## Integrierter AVI-Ablauf und UI – Stand 19.09.2026

| Prüfung | Ergebnis / Stand |
|---|---|
| Diplomatin AVI, zwei Cutlists, Vorbereitung bis MP4-Ausgabe | Live erfolgreich; Wiedergabe, Anfang/Ende und Ton-Synchronität bestätigt |
| Rubikon AVI, zwei Cutlists, Vorbereitung bis MP4-Ausgabe | Live erfolgreich; Wiedergabe und Ton-Synchronität bestätigt |
| Diplomatin HD-AVI mit HQ-Cutlist | Dateigrößenwarnung bewusst akzeptiert; Schnitt erfolgreich |
| Zwei Frames Unterschied bei Diplomatin | Akzeptiert und protokolliert; 20-ms-Containerverlängerung als Regression abgesichert |
| Fehlender ffprobe-Pfad | Konkreter Fehlergrund direkt im Fortschrittsfenster live bestätigt; ursprünglichen Pfad nach dem Test wiederherstellen |
| Server-Cutlist erneut auswählen | Ohne erneutes Videoladen live bestätigt |
| Schmales/kurzes Fenster | Umbruch, Scrollen, Videobegrenzung und sichtbare Zeitangaben live bestätigt |
| Fenstergröße ohne Videoladen speichern | Live bestätigt |
| Leerer ColdCut-Endmarker | Import-/Schnittplan-/Roundtrip-Tests mit Diplomatin-Werten bestanden; separater Live-Nachweis des konkreten Originaleintrags nicht festgehalten |
| Künstliche AVI ohne Ton, AVI mit MP3 und H.264/AAC-MKV | Remux und Metadatenprüfung technisch ausgeführt und bestanden |
| Vorhandene Remux-Zieldatei / vorab abgebrochener Auftrag | Im technischen Integrationstest abgewiesen |
| Enigma AVI, H.264/MP3, 25 fps | Remux und MP4Box-Schnitt erfolgreich; praktische Filmkontrolle und Decoderlauf mit normalisierten Ausgabezeitstempeln bestanden; PTS-/DTS-Auffälligkeiten dokumentiert |
| Wo die Liebe hinfällt, Full-HD-AVI mit Endung `.avi.mp4` | Bildfehler und fehlerhafte Bildaktualisierung bei Frame-Sprüngen in CAN beobachtet; Tonversatz in MPC-HC berichtet; kein erfolgreicher Schnittnachweis. Nutzer bestätigt am 11.10.2026 das Original als untauglich; keine weitere Untersuchung dieser Aufnahme beauftragt, neue Ausstrahlung abwarten |

Der damalige vollständige automatisierte Testlauf umfasste 450 erfolgreiche Tests. Die frühere Aufschlüsselung auf die vier Testsuiten bezog sich auf 443 Tests; beide Zahlen dokumentieren den September-Stand. Die späteren gezielten WPF- und Medienprüfungen stehen in den jeweiligen Berichten; eine allgemeine automatisierte GUI- oder Video-Synchronitätsabdeckung wird daraus nicht abgeleitet.

### Weitere manuelle Fehler- und Grenzfallprüfungen

- Vorbereitung mit „Nein“ ablehnen: Original und Schnittmarken bleiben verfügbar.
- Während eines längeren FFmpeg-Laufs abbrechen: Prozess endet, MP4Box startet nicht, temporäre Datei wird aufgeräumt.
- Während MP4Box abbrechen: Abbruchmeldung prüfen; keine neue endgültige Ausgabe, vorhandene Ausgabe unverändert und temporäre Dateien aufgeräumt.
- Ungültigen FFmpeg-/MP4Box-Pfad sowie nicht MP4-kompatible Streams prüfen: verständlicher Fehler, keine automatische Neukodierung.
- Ziel bereits vorhanden: Bestätigung und Erhalt der alten Ausgabe bei Fehlschlag vor dem Ersetzen prüfen.
- Fehleranzeige bei eingeklapptem Protokoll prüfen; Kopieren und erneuten Versuch kontrollieren.
- Andere Eingangscontainer, weitere Audio-/Videostreams und verschiedene Bildraten getrennt erproben.
- Bei Erfolg immer Filmgrenzen, Werbeschnitte und Ton-Synchronität der tatsächlichen Ausgabe kontrollieren.

Diese Liste kennzeichnet noch zu vertiefende Prüfungen; sie behauptet keine bereits erfolgte vollständige Abdeckung. Keine realen Filmdateien ins Repository aufnehmen.

Die Review-Korrekturen sind durch Tests für widersprüchliche Cutlist-Bereiche sowie die Ausgabeübernahme nach erfolgreichem Zusammenfügen abgesichert. Fehler, Abbruch, vorhandene Ausgaben und während des Schnitts neu angelegte Zieldateien werden geprüft.

## VM-Setup-Prüfrunde Build 40

Erster AVI-/MP4Box-Fall persönlich positiv. Originalzeitstempelwarnungen sind im Protokoll dokumentiert; Vorbereitungsprüfungen und alle sechs Abschnittsprüfungen abgeschlossen. Weitere Fälle noch laufend. [VM-Bericht](VM-SETUP-BUILD40-20261027.md).

## Eigene Portable-ZIP-Prüfung in der VM

Erster Fall nach Setup-Deinstallation persönlich positiv. Setup- und Portable-Nachweise getrennt halten; aktueller Bericht nennt die einzelnen Fälle. [Portable-Prüfrunde](VM-PORTABLE-BUILD40-20261027.md).

## Build-41-Bereinigung persönlich prüfen

Ein erfolgreiches Kleben darf keinen neuen .can-join-Arbeitsordner zurücklassen. Bei Fehler/Abbruch muss das Diagnoseprotokoll erhalten und sein Pfad lesbar sein. Technisch durch vier Regressionstests und 21 native Fälle geprüft. Persönlicher Setup-Test auf dem Arbeitsrechner mit John Wick positiv: temporäre Datei weg, Bild/Ton/Schnitt/Drei-Sekunden-Vorschau und Klebecenter bestätigt. Neue ZIP-/VM- und persönliche Fehler-/Abbruchprüfung daraus nicht abgeleitet.
