# Setup-Merkliste

Stand: 13.09.2026. Vereinbarter Umfang für die Veröffentlichung nach der weiteren Testphase. Diese Punkte sind geplant und noch nicht umgesetzt.

## Rundum-sorglos-Paket

- [ ] CAN mit gemeinsam getesteten Versionen von FFmpeg, ffprobe, MP4Box und libmpv ausliefern.
- [ ] Konkrete Werkzeugversionen, Downloadquellen und Prüfsummen für reproduzierbare Pakete festhalten.
- [ ] Werkzeuge im CAN-Verzeichnis ablegen und automatisch über relative Pfade finden; die von libmpv benötigte Platzierung neben der Anwendung berücksichtigen.
- [ ] Eigene Werkzeugpfade als optionale Einstellung weiterhin ermöglichen; das Paket soll ohne manuelle Pfadeinrichtung starten.
- [ ] Weitergabebedingungen der tatsächlich verwendeten Builds prüfen und erforderliche Lizenztexte, Hinweise und gegebenenfalls Quellcode-Angebote beilegen.
- [ ] Bereitstellung der .NET-Desktop-Laufzeit klären: im Paket enthalten oder durch das Setup bereitgestellt.
- [ ] Installer oder portable Ausgabe festlegen und auf einem Windows-System ohne zuvor eingerichtete Entwicklungswerkzeuge testen.
- [ ] Programmsymbol und finale Gestaltung ergänzen.
- [ ] Paketversion und enthaltene Werkzeugversionen dokumentieren; Updates als erneut getestetes Gesamtpaket bereitstellen.
- [ ] Installation beziehungsweise Entpacken, Werkzeugerkennung, Wiedergabe, Analyse und Schnitt gemeinsam prüfen.

Zusätzliche Werkzeug-Downloadlinks unter Hilfe sind für dieses Konzept nicht vorgesehen. Die Projektlinks in den Credits bleiben erhalten. Der Menüpunkt Update/GitHub öffnet derzeit nur die Projektseite; ein automatischer Updater ist damit nicht umgesetzt.

## Vorher

Jörg nimmt sich in dieser Woche Zeit für weitere Praxistests. Erkenntnisse und Fehler werden mit CAN-Version, Buildnummer und gegebenenfalls Schnittprotokoll festgehalten. Die Veröffentlichung folgt nach Auswertung dieser Tests und Freigabe.

## V2

- Klebezentrum (experimentell): mehrteilige Aufnahmen zusammenfügen.
- Schnittbereiche beim Schneiden gemäß den Hinweisen von chrisdude feinabstimmen; konkrete Regeln vor der Umsetzung festlegen.

Stapelverarbeitung und Smart Rendering bleiben weitere spätere Wünsche ohne feste Versionszuordnung.
