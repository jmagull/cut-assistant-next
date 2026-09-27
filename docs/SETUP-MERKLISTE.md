# Setup-Merkliste

Stand: 20.09.2026. Interne Testphase für Cut Assistant Next 0.2.0, Build 6.
Windows-Installer TEST2 und Portable ZIP TEST2 wurden erfolgreich getestet.
Eine öffentliche Veröffentlichung ist noch nicht freigegeben; insbesondere
Lizenz- und Weitergabebedingungen sind noch zu klären.

## Windows-Installer und portable Ausgabe

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
- [ ] Portable ZIP zusätzlich auf einem Rechner ohne vorhandene CAN-Konfiguration testen.
- [ ] Werkzeugversionen, Downloadquellen und SHA-256-Prüfsummen vollständig für einen reproduzierbaren Paketbau dokumentieren.
- [ ] libmpv-Juli-Build `2026-07-30-74356c0fc6`: Originalarchiv wiederbeschaffen und dauerhaft sichern (hinterlegte Download-URL am 20.09.2026 mit HTTP 404 geprüft). Passende Quellcodes, Buildinformationen und Lizenzpflichten für genau diesen Build klären. Der September-Build ist nur ein Untersuchungskandidat und ersetzt die getestete Juli-DLL nicht.
- [ ] Lizenztexte, Weitergaberechte und gegebenenfalls erforderliche Quellcode-Angebote prüfen und beilegen.
- [ ] Release-Verfahren für künftige, gemeinsam getestete Paketversionen dokumentieren.

Zusätzliche Werkzeug-Downloadlinks unter Hilfe sind für dieses Konzept nicht vorgesehen. Die Projektlinks in den Credits bleiben erhalten. Der Menüpunkt Update/GitHub öffnet derzeit nur die Projektseite; ein automatischer Updater ist damit nicht umgesetzt.

## Projektlizenz und Entstehung

- [ ] Allgemeine Lizenzbedingungen für CAN festlegen: GPL 3.0 oder eine geeignete Alternative prüfen. Die endgültige Auswahl und Freigabe stehen noch aus; dieser Merkpunkt ändert die derzeitige Lizenz nicht.
- [ ] Nach der Entscheidung die Projektlizenz und zugehörigen Hinweise in Repository, Dokumentation und Veröffentlichung einheitlich aufnehmen; die Bedingungen mitgelieferter Komponenten gesondert berücksichtigen.
- [ ] Einen Hinweis zur KI-unterstützten Entwicklung in Credits und Dokumentation aufnehmen. Vorgesehener Wortlaut nach Angabe des Projektverantwortlichen:

  „Cut Assistant Next entstand mit Unterstützung von ChatGPT/Codex unter Einsatz von GPT-5.6 Sol (Denkaufwand: hoch) und GPT-6 Astra (Denkaufwand: mittel). Die abschließende Entscheidung über Vorschläge, Änderungen und deren Übernahme lag stets beim Nutzer und Projektverantwortlichen Jörg.“

## Vorher

Jörg nimmt sich in dieser Woche Zeit für weitere Praxistests. Erkenntnisse und Fehler werden mit CAN-Version, Buildnummer und gegebenenfalls Schnittprotokoll festgehalten. Die Veröffentlichung folgt nach Auswertung dieser Tests und Freigabe.

## V2

- Klebezentrum (experimentell): mehrteilige Aufnahmen zusammenfügen.
- Schnittbereiche beim Schneiden gemäß den Hinweisen von chrisdude feinabstimmen; konkrete Regeln vor der Umsetzung festlegen.
- Auswahl der Tonspur in CANs Player anbieten, wenn eine Aufnahme mehrere Tonspuren enthält; Sprachen und Kennzeichnungen verständlich anzeigen.

Stapelverarbeitung und Smart Rendering bleiben weitere spätere Wünsche ohne feste Versionszuordnung.
