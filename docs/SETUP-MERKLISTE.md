# Setup-Merkliste

Stand: 11.10.2026. Die ursprüngliche Planung vom September ist mit dem aktuellen Projektstand abgeglichen. Öffentliche Vorabversion: **0.2.1 Build 13 RC3 mit otr-can 0.1.4**, auf Nutzerauftrag nach den technischen Prüfungen veröffentlicht. Persönliche vollständige Paketabnahme: **Build 12 RC3**, dessen Pakete unverändert separat erhalten bleiben.

Aktuelle Nachweise: [Projektstatus](STATUS.md), [Build-13-Prüfung](RELEASE-BUILD13-RC3-RESULT.md), [RC3-Veröffentlichung](RELEASE-PUBLICATION-BUILD13-RC3.md), [persönliche Build-12-Abnahme](RELEASE-BUILD12-RC3-RESULT.md).

## Eingefrorener Paketsatz – 13.10.2026

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

## Aktueller Installer und portable Ausgabe

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

Build 13 RC3 ist veröffentlicht. Weitere Rückmeldungen mit konkreter Versions-/Buildkennung festhalten; eine zusätzliche persönliche Build-13-Paketprüfung kann nachgetragen werden. Nächste Funktionsaufgabe getrennt festlegen. Version 1.0 ist weiterhin nicht festgelegt.

## V2

- Klebezentrum (experimentell): mehrteilige Aufnahmen zusammenfügen.
- Schnittbereiche beim Schneiden gemäß den Hinweisen von chrisdude feinabstimmen; konkrete Regeln vor der Umsetzung festlegen.
- Auswahl der Tonspur in CANs Player anbieten, wenn eine Aufnahme mehrere Tonspuren enthält; Sprachen und Kennzeichnungen verständlich anzeigen.

Stapelverarbeitung und Smart Rendering bleiben weitere spätere Wünsche ohne feste Versionszuordnung.
