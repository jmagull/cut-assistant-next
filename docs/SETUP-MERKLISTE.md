# Setup-Merkliste

**Stand 27.10.2026: CAN 1.0.2 Build 41, otr-can 0.1.6, libmpv 0.41.0.** [Versionen](VERSIONEN.md), [Paketbericht](RELEASE-1.0.2-BUILD41-RESULT.md).

## Erledigt

- [x] Filmteile/Reihenfolge im Klebecenter wählen, danach Tonspur je Teil. Keine Vorbereitungskopie.
- [x] Technische Prüfung, Abschlussdialog und Kontrollplayer mit drei Sekunden Vorlauf.
- [x] Eigener John-Wick-Lauf AC-3/AAC persönlich positiv bestätigt.
- [x] Zweiter eigener Lauf AAC/AAC persönlich positiv bestätigt.
- [x] Build-40-Setup, Portable, Quellen, Motor-/libmpv-Quellen, Relink und Prüfsummen erstellt/geprüft.
- [x] Voraussetzungstext auf 0.1.6 korrigiert, Version beim Paketbau abgeglichen. Motor war bereits 0.1.6.
- [x] Setup-Screenshot zeigt den korrigierten Build-40-Hinweis.

## Weitere Paketvarianten

- [x] Arbeits-PC-Funktionslauf nach Umstieg auf Setup Build 40: beide Furiosa-Schnitte mit otr-can und Klebecenter samt allen Prüfstellen positiv gemeldet.
- [ ] Weitere Installationsvarianten gesondert prüfen, soweit gewünscht.
- [x] Erster VM-Setup-Fall: Die Teufelsbrigade AVI mit MP4Box, Ton/Schnitt/Bild/Drei-Sekunden-Vorschau positiv.
- [x] Zweiter VM-Setup-Fall: Die Teufelsbrigade AVI mit otr-can 0.1.6, MP3→AAC; Ton/Bild/Schnitt/Drei-Sekunden-Vorschau positiv.
- [x] Dritter VM-Setup-Fall: Grey’s Anatomy MP4 mit MP4Box, drei Abschnitte; Ton/Bild/Schnitt/Drei-Sekunden-Vorschau positiv.
- [x] Vierter VM-Setup-Fall: Grey’s Anatomy MP4 mit otr-can 0.1.6; Ton/Bild/Schnitt/Drei-Sekunden-Vorschau positiv. AVI und MP4 damit jeweils mit beiden Motoren bestätigt.
- [x] Fünfter VM-Setup-Fall: Furiosa HD mit MP4Box, fünf Abschnitte und Zuordnung der kleinen Überlappung an Übergang 1; Bild/Ton/Schnitt/Drei-Sekunden-Vorschau positiv.
- [x] Sechster VM-Setup-Fall: Furiosa HD mit otr-can 0.1.6, AAC/AC-3 technisch geprüft; Bild/Schnitt/Ton/Drei-Sekunden-Vorschau positiv. AVI, HQ und HD damit jeweils mit beiden Motoren bestätigt.
- [x] Siebter VM-Setup-Fall: Furiosa Teil 2 HD mit MP4Box, zwei Abschnitte; Bild/Schnitt/Ton/Drei-Sekunden-Vorschau positiv.
- [x] Achter VM-Setup-Fall: Furiosa Teil 2 HD mit otr-can 0.1.6; Bild/Schnitt/Ton/Drei-Sekunden-Vorschau positiv, beide Tonspuren technisch geprüft.
- [x] Neunter VM-Setup-Fall: Furiosa-Klebecenter mit AAC/AAC, Bild/Ton/Klebestelle/Drei-Sekunden-Vorschau positiv.
- [x] VM-Deinstallation persönlich bestätigt; Diagnoseordner mit protocol.txt im Filmordner als Rest beobachtet.
- [x] Build 41: nach Erfolg Protokoll und leeren Arbeitsordner bereinigen, bei Fehler/Abbruch Diagnose erhalten; vier Regressionstests und 21 native Fälle positiv.
- [x] Build-41-Setup auf dem Arbeitsrechner mit John Wick: temporäre Datei weg; Bild/Ton/Schnitt/Drei-Sekunden-Vorschau und Klebecenter positiv.
- [ ] Neue Build-41-ZIP-/VM-Bereinigung getrennt prüfen, soweit vorgesehen.
- [x] Erster VM-Portable-Fall nach Setup-Deinstallation: Grey’s Anatomy mit MP4Box, Bild/Ton/Schnitt/Drei-Sekunden-Vorschau positiv.
- [x] Zweiter VM-Portable-Fall: Teufelsbrigade AVI mit otr-can 0.1.6, sechs Abschnitte, Bild/Ton/Schnitt/Drei-Sekunden-Vorschau positiv.
- [x] Dritter VM-Portable-Fall: Furiosa-Klebecenter AAC/AAC; Bild/Ton/Klebestelle/Drei-Sekunden-Vorschau positiv.
- [x] Persönliche ZIP-Prüfrunde Build 40 mit „damit zip check“ ausdrücklich abgeschlossen.
- [ ] Persönliche neue Fehler-/Abbruchfälle nach Bedarf.

Keine Veröffentlichung ohne Freigabe. Dokumentationsabgleich ändert weder Pakete noch Zähler 40. Die folgenden historischen Planungen sind keine aktuelle Aufgabenliste.

## Historische Planung und Ausgangspunkt vom 26.10.2026

## Klebecenter – vorgemerkt am 26.10.2026

**Fortsetzung geplant für morgen, 27.10.2026.** Als erster Praxisfall dienen **John Wick: Kapitel 4, Teil 1 und Teil 2**. Die Einbindung des Zusammenfügens in CAN ist noch offen; ein separat mit FFmpeg erzeugter Referenzfilm ist bereits erfolgreich geprüft.

- [x] **Erster Baustein: Tonspurkopie für weitere Filmteile.** In Build 38 umgesetzt: bereits geschnittenen Filmteil laden, unter **Tonspuren → Tonspur für Filmteile auswählen …** eine Spur bewusst wählen und eine zusätzliche MP4-Kopie ohne Neukodierung speichern. Der Nutzer hat den Ablauf mit John Wick Teil 1 samt Erfolgsprotokoll und Abschlussmeldung persönlich bestätigt. Die neue Kopie enthält AAC/Deutsch/Stereo; der automatische Notfallweg für Teil 2 ist ebenfalls persönlich bestätigt. [Umsetzung und Grenzen](TONSPURAUSWAHL-FILMTEILE-20261026.md).
- [ ] **Teile auswählen:** bereits geschnittene Filmteile für einen gemeinsamen Film auswählen und ihre Reihenfolge festlegen.
- [ ] **Tonspuren auswählen:** pro Teil die passenden Spuren wählen und deren Eignung für das Zusammenfügen prüfen; insbesondere Sprache, Codec, Kanalzahl und Samplerate. Die vorhandene bewusste Tonspurauswahl bildet die Grundlage.
- [ ] **Kleben:** die vorbereiteten Teile zu einer neuen Datei zusammenfügen und anschließend den Übergang sowie Bild und Ton kontrollieren. Vorhandene Teile erhalten.

**Aufhänger für die nächste Sitzung:** Teil 1 wurde mit Build 38 als **John Wick Kapitel 4 Teil 1 otr-can - AAC.mp4** gespeichert. Teil 2 ist die erfolgreiche AAC-Notfallausgabe **John Wick Kapitel 4 Teil 2 mit Audio Korrektur.mp4**. Beide haben eine passende AAC-Stereospur mit 48000 Hz. Beide Dateien wurden anschließend separat mit FFmpeg zu **John Wick Kapitel 4 mit Audio Korrektur.mp4** zusammengefügt; der Nutzer bestätigt den sauberen Übergang. Laufzeit **02:38:49.160**, Klebestelle **01:57:37.120**, Drei-Sekunden-Vorlauf **01:57:34.120**. Bild vollständig unverändert, Ton unter Beachtung des AAC-Paddings einmal durchgehend kodiert. [Geprüftes Muster und Zeitstempelfallstrick](JOHN-WICK-4-KLEBECENTER-REFERENZ-20261026.md).

## Historische Setup- und Abnahmeplanung

Die folgenden Abschnitte beschreiben ihre jeweils datierten früheren Stände. Den aktuellen Stand und die nächsten Schritte nennen der Überblick und der Klebecenter-Eintrag oben.

## Eingefrorener Paketsatz – 13.10.2026

- [x] **Öffentliche Veröffentlichung Build 29 RC3:** auf Nutzerauftrag freigegeben, sieben unveränderte Downloads über GitHub-Größen und SHA-256-Digests geprüft. [Nachweis](RELEASE-PUBLICATION-BUILD29-RC3.md). Ältere Build-16-Ausgabe erhalten.

- [x] **0.3.0 · Build 29 · RC3** aus festem, gepushtem Quellcommit gebaut; Setup, Portable-ZIP, CAN-/Motorquellen und libmpv-Quellen-/Relink-Pakete samt Prüfsummen erstellt. **1076/1076 Tests**, 249 Portable-Dateien und vollständiger Quellsnapshot geprüft. [Paketnachweis](RELEASE-BUILD29-RC3-RESULT.md).
- [x] Vier lokale Produktions-Lockänderungen und Altartefakte erhalten; Quell-/Binärsatz und Prüfsummen eingefroren.
- [x] **Produktivinstallation und Schnittprüfung Build 29:** vom Nutzer am 13.10.2026 positiv bestätigt; Torchwood S4E7 mit MP4Box und S4E6 mit otr-can nach Lupenkontrolle. MP4Box-Doppelszene bei 15:05 für S4E6 separat dokumentiert.
- [x] **Separate Portable-/VM-Paketabnahme Build 29:** Setup-Installation, Greys Anatomy / Durchs Feuer mit MP4Box und otr-can sowie Deinstallation in der VM bestätigt. Portable-Appstart, Pfade einrichten und Teufelsbrigade-AVI mit beiden Motoren ebenfalls bestätigt. [Persönliche Paketprüfung](RELEASE-BUILD29-RC3-RESULT.md). Installationstyp/frisches Profil nicht gesondert unterschieden.

## Offener Bedienwunsch – Frame-Lupe, 13.10.2026

- [x] **Suchsprung nur bei Richtungswechsel halbieren.** In Build 27 umgesetzt und technisch geprüft; am 13.10.2026 vom Nutzer als „Halb frame sprung check“ persönlich bestätigt. Erster Suchsprung mit dem eingestellten Startwert. Weitere Suchsprünge in derselben Richtung behalten diesen Wert. Erst beim Wechsel nach links/rechts halbiert sich der Wert; der Sprung in die neue Richtung verwendet bereits den halbierten Wert. Beispiel bei Startwert 2000: **+2000 → +2000 → −1000 → −1000 → +500**. Mindestwert 1 Frame beibehalten; **Suche neu starten** setzt Startwert und gemerkte Suchrichtung zurück. Normale ±1/±10-/±20-Schritte bleiben erhalten.
- [x] **AVI-Lupe und Schnitt mit beiden Verfahren:** Nutzer bestätigt am 13.10.2026 die funktionierende AVI-Lupe und Teufelsbrigade-Schnitte mit MP4Box sowie otr-can, jeweils ohne Blitzer. [Abnahmenachweis](AVI-FRAME-LUPE-20261013.md#persönliche-abnahme--13102026). AVI-Abnahme für **0.3.0 · Build 26 · RC3**; Richtungswechsel-Regel und FFmpeg-Versionsprüfung anschließend in **Build 27** umgesetzt. [Prüfnachweis](LUPEN-SUCHRICHTUNG-FFMPEG-20261013.md).

## Bedienung – Ergänzung vom 11.10.2026

- [x] Timeline-Klick und Reglerziehen: am 05.10.2026 umgesetzt, technisch geprüft und persönlich bestätigt; Nachweis im [Tagebuch](TAGEBUCH.md#05102026--timeline-per-mausklick-positionieren).
- [x] Fenstergrößen von Schneiden, Fortschritt/Protokoll, Schneiden abgeschlossen und Cutlist erzeugen getrennt speichern und beim nächsten Aufruf sowie nach einem Programmneustart wiederherstellen.
- [x] Besonders **Video vorbereiten und schneiden**: gewählte Breite/Höhe und Maximierung beibehalten, damit auch der Protokollbereich beim nächsten Schnitt wieder ausreichend Platz hat.
- [x] **Cutlist erzeugen**: vorhandene Größen-Datei weiterverwenden und Speicherung um Maximierung ergänzen.

Fenstergrößen am 11.10.2026 unter Build 11 umgesetzt, gebaut und geprüft: damals 767 CAN-Tests und echter WPF-Neustarttest, anschließend persönlich bestätigt. Details im [Prüfbericht](DIALOG-FENSTERGROESSEN-PRUEFBERICHT-20261011.md).

## Installer und portable Ausgabe – historischer Stand vom 11.10.2026

- [x] Frameworkabhängige Ausgabe mit separat installierter .NET 10 Desktop Runtime; geprüfter CAN-libmpv-0.41.0-Stack enthalten. FFmpeg/ffprobe und GPAC/MP4Box separat einrichten.
- [x] Build-12-RC3-Setup und Portable-ZIP mit optionalem otr-can 0.1.4, passenden Quellen, Lizenztexten und Prüfsummen gebaut und automatisch geprüft: **785 CAN-/40 Rust-Tests**.
- [x] Persönliche Überinstallation, Windows-11-VM-Installation, beide Schnittmotoren, Bild/Ton, Setup-Deinstallation und separater Portable-ZIP-Test bestätigt; [Prüfbericht](RELEASE-BUILD12-RC3-RESULT.md).
- [x] Werkzeug-/Quellzuordnung, SHA-256-Werte und Verfahren für erneuten Paketbau dokumentiert; [Paketbau](RELEASE-BUILD12-RC3-RESULT.md#reproduzierbarer-lokaler-paketbau).
- [x] Juli-libmpv-Kandidat abgelöst: der geprüfte September-Stack 0.41.0 mit vier DLLs, Quellen-/Relink-Paketen und Lizenzhinweisen wurde bereits mit RC2 veröffentlicht; [Veröffentlichungsnachweis](RELEASE-PUBLICATION-BUILD7-RC2.md). Keine Wiederbeschaffung des alten Juli-Archivs erforderlich.

Der KI-Absatz im Credits-Dialog wurde nach dieser Paketabnahme ergänzt. Der dafür neu erstellte **Build-13-RC3-Satz aus Commit c98041a** enthält zusammengehörige Quellen-/Binärpakete und ist automatisch geprüft: 785/785 Tests, 249 Portable-Dateien, entpackte WPF-/Credits-Komponenten und künstlicher Zwei-Tonspur-Schnitt. [Prüfbericht](RELEASE-BUILD13-RC3-RESULT.md). Persönliche Paketbestätigung für Build 13 noch offen.

## Historische TEST2-Prüfungen vom 20.09.2026

Die folgenden Punkte beschreiben ausschließlich die damaligen internen, selbstenthaltenen Testpakete. Ihr Paketkonzept mit gebündelter .NET-Laufzeit und Werkzeugen gilt nicht für die aktuelle frameworkabhängige Ausgabe.

- [x] Windows-Installer TEST2: CAN 0.2.0 Build 6, selbstenthaltene .NET-Laufzeit, libmpv sowie FFmpeg und ffprobe 8.1.2 enthalten.
- [x] GPAC 26.07: Originalinstaller separat eingebunden. Fehlende Installation wird nach ausdrücklicher Zustimmung durchgeführt; eine vorhandene Installation wird übersprungen. Beide Fälle auf VM 108 erfolgreich getestet.
- [x] Portable ZIP TEST2: CAN, selbstenthaltene .NET-Laufzeit und libmpv enthalten; FFmpeg und GPAC werden nicht mitgeliefert. Entpacken und Programmstart auf dem Entwicklungsrechner erfolgreich getestet.

### Technische Prüfung und Freigabe

- [x] Werkzeugerkennung: gebündelte FFmpeg-Pfade und separat installiertes GPAC werden automatisch gefunden; eigene Werkzeugpfade bleiben konfigurierbar.
- [x] Selbstenthaltene .NET-Laufzeit und libmpv sind in beiden Ausgaben enthalten.
- [x] Programmsymbol, deutscher Installationsassistent und Desktop-Verknüpfung umgesetzt und getestet.
- [x] Installer auf VM 108 getestet: Installation, Wiedergabe, Analyse, MP4Box-Schnitt und Deinstallation.
- [x] Beide GPAC-Szenarien getestet: Nachinstallation mit Zustimmung sowie Überspringen einer vorhandenen Installation.
- [x] Portable ZIP: Inhalt, Entpacken und Programmstart auf dem Entwicklungsrechner getestet.

Die damaligen offenen Aufgaben zu Quellen, Lizenzen und Paketbau wurden durch die oben verlinkten späteren Prüfungen und Veröffentlichungsnachweise abgelöst. Die damalige Forderung nach einem Portable-Test ohne vorhandene CAN-Konfiguration ist kein offener TEST2-Releaseblocker mehr; ein ausdrücklich neuer Benutzerprofiltest kann als zusätzliche Prüfung erfolgen. Aus der persönlichen ZIP-Abnahme wird kein gesonderter Nachweis eines frischen Profils abgeleitet.

Zusätzliche Werkzeug-Downloadlinks unter Hilfe sind für dieses Konzept nicht vorgesehen. Die Projektlinks in den Credits bleiben erhalten. Der Menüpunkt Update/GitHub öffnet derzeit nur die Projektseite; ein automatischer Updater ist damit nicht umgesetzt.

## Projektlizenz und Entstehung

- [x] Projektlizenz **GPL-3.0-or-later** gewählt und in [LICENSE](../LICENSE), README und Veröffentlichung aufgenommen. Die Bedingungen fremder Komponenten bleiben gesondert zugeordnet.
- [x] Lizenztexte und Komponentenhinweise beigelegt; [.NET-/NuGet-Bestandsaufnahme](DOTNET-LICENSING.md), [libmpv-Hinweise](libmpv/v0.41.0/THIRD-PARTY-NOTICES.md) und [Paketprüfbericht](RELEASE-BUILD12-RC3-RESULT.md) dokumentieren den jeweiligen Umfang.
- [x] KI-Hinweis in [Danksagung](DANKSAGUNG.md) vorhanden und im aktuellen Credits-Dialog ergänzt. In den unveränderten Build-12-Paketen fehlt der Dialogabsatz noch. Wortlaut nach Angabe des Projektverantwortlichen:

  „Cut Assistant Next entstand mit Unterstützung von ChatGPT/Codex unter Einsatz von GPT-5.6 Sol (Denkaufwand: hoch) und GPT-6 Astra (Denkaufwand: mittel). Die abschließende Entscheidung über Vorschläge, Änderungen und deren Übernahme lag stets beim Nutzer und Projektverantwortlichen Jörg.“

## Nächster Schritt

Am 27.10.2026 die CAN-Integration des Klebecenters auf Basis des persönlich bestätigten John-Wick-Referenzfalls beginnen. Vorhandene Dateien schützen, passende Tonspuren bewusst wählen und die fertige Bild-/Tonzeitachse prüfen. Separate neue Portable-/VM-/Deinstallationsprüfungen für den aktuellen Paketsatz bleiben offen. Veröffentlichung nur nach ausdrücklicher Freigabe.

## Weitere Planung

- **Klebecenter (experimentell):** Teile auswählen, Tonspuren auswählen und zusammenfügen. Nächster Praxisfall am 27.10.2026: John Wick Kapitel 4, Teil 1 und Teil 2; siehe den Eintrag vom 26.10.2026 oben.
- Schnittbereiche beim Schneiden gemäß den Hinweisen von chrisdude feinabstimmen; konkrete Regeln vor der Umsetzung festlegen.
- Auswahl der Tonspur in CANs Player anbieten, wenn eine Aufnahme mehrere Tonspuren enthält; Sprachen und Kennzeichnungen verständlich anzeigen.

Stapelverarbeitung und Smart Rendering bleiben weitere spätere Wünsche ohne feste Versionszuordnung.

## Korrigierter Voraussetzungstext – Build 40

Hinweis nennt jetzt korrekt Motor 0.1.6; tatsächlicher Motor war auch in Build 39 bereits 0.1.6. Neue Pakete und Quellen geprüft; Klebecenter unverändert. Setup-Test auf dem Arbeits-PC vom Nutzer begonnen, Abschluss noch nicht bestätigt. Neue Build-40-/Portable-/VM-Abnahme offen.
