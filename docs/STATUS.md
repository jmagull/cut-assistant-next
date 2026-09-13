# Projektstatus

Stand: 13.09.2026. Dieser Überblick beschreibt den aktuellen Feature-Stand; ältere Projektdefinitionen und Architekturentscheidungen dokumentieren teilweise frühere Entwicklungsphasen.

## Aktueller Umfang

CAN verwendet C#, .NET 10 und WPF unter Windows 11 x64. ffprobe liefert Medieninformationen, mpv/libmpv übernimmt die Wiedergabe, MP4Box den Schnitt. FFmpeg bereitet bei Bedarf eine MP4-Arbeitsdatei vor.

Umgesetzt sind Medienanalyse und Wiedergabe, Frame-Navigation, manuelle Schnittplanung, lokale Cutlists, Server-Suche/-Download/-Upload, Namensmasken, MP4Box-Schnitt und experimentelle Vorbereitung anderer Container. Die Bedienung ist in der [Nutzeranleitung](NUTZERANLEITUNG.md) beschrieben.

## Oberfläche und Cutlist-Ablauf

- „Videodatei laden“ bietet MP4, AVI, MKV und weitere Dateiendungen an. Entscheidend ist die Containeranalyse, nicht der Dateiname.
- Nach dem Laden wird bei konfiguriertem Server automatisch gesucht. „Cutlist vom Server laden“ öffnet die Suche erneut, ohne die Originaldatei neu laden zu müssen.
- Lokale und heruntergeladene Cutlists verwenden denselben Importweg. Dateigrößenabweichungen lösen eine Warnung aus und können bewusst akzeptiert werden.
- Null-Längen-Einträge werden beim Import ignoriert und NoOfCuts wird intern angepasst. Negative Dauern, widersprüchliche Frame-Dauern und ausschließlich leere Bereiche bleiben Fehler.
- Die Oberfläche zeigt Entfernbereiche; klassische Cutlists speichern die komplementären Behaltebereiche.
- Technische Medieninformationen stehen im Dialog „Video-Informationen …“.
- Bei geringer Breite umbrechen die Bediengruppen, bei geringer Höhe scrollt der Hauptbereich. Das native Videofenster wird auf den sichtbaren Scrollbereich begrenzt.
- Fenstergröße wird auch ohne Videoladen gespeichert; Erfolg oder Fehler wird protokolliert.

## Experimentelle Video-Vorbereitung

Originaldatei → Analyse → Server-/lokale Cutlist oder eigene Marken → „Schneiden“ → gegebenenfalls Ja/Nein-Abfrage → FFmpeg-Arbeitsdatei → Plausibilitätsprüfung → MP4Box → MP4-Ausgabe.

Die Originaldatei bleibt geladen und unverändert. Cutlist-Namen und Dateigrößen beziehen sich weiterhin auf das Original. Die temporäre MP4 wird nicht als neue Nutzerdatei geladen und nicht Grundlage einer automatisch erzeugten Server-Cutlist.

FFmpeg verwendet Stream-Copy für alle Streams. Nicht kompatible Streams werden nicht still entfernt oder neu kodiert. Nach Abschluss, Fehler oder Abbruch versucht CAN die temporäre MP4 zu löschen; Fehler beim Aufräumen erscheinen im Protokoll. MP4Box legt seine Schnittsegmente im Ausgabeordner ab und räumt sie am Ende auf.

Geprüft werden Container, Streamanzahl, Codecs, Auflösung, Bildrate, Dauer und verfügbare Start-/Frame-Angaben. Kleine Abweichungen werden nur im Protokoll vermerkt:

| Merkmal | Aktuelle Toleranz |
|---|---|
| Gemeldete Frame-Anzahl | bis zu zwei Frames |
| Mittlere Bildrate | rechnerisch höchstens zwei Frames über die Original-Laufzeit |
| Containerlaufzeit | zwei Frames, mindestens 100 ms |
| Relativer Videostart | zwei Frames |
| Relativer Audiostart | 100 ms |
| Fehlende Start-/Frame-Anzahl-Metadaten | Detailhinweis; übrige Prüfungen gelten weiter |

Dies ist eine Metadaten-Plausibilitätsprüfung, kein vollständiger Nachweis der Timeline oder Ton-Synchronität. Die Diplomatin zeigte 317399 gegenüber 317397 Frames und 6347,98 gegenüber 6348 Sekunden. Dieser akzeptierte Grenzfall ist durch einen Regressionstest abgesichert.

## Fortschritt und Fehler

Das Protokoll ist standardmäßig aufgeklappt, kann eingeklappt und in die Zwischenablage kopiert werden. Es zeigt sowohl FFmpeg als auch MP4Box. Ein konkreter Fehlergrund bleibt im Statusbereich sichtbar, selbst wenn nachgereichte Fortschrittsmeldungen eintreffen. Bei Fehler gibt es kein automatisches Schließen.

## Verifikation

- Release-Build: 0 Fehler, 0 Warnungen; **430/430 Tests bestanden**.
- Diplomatin und Rubikon: integrierter AVI-Ablauf mit jeweils zwei Cutlists erfolgreich; Wiedergabe einschließlich Anfang, Ende und Ton-Synchronität vom Nutzer bestätigt.
- Zusätzlich bestätigter Diplomatin-Schnitt mit HQ-Cutlist trotz unterschiedlicher Quelldateigröße.
- Fehleranzeige mit absichtlich ungültigem ffprobe-Pfad live bestätigt.
- Kleine künstliche AVI-/MKV-Dateien: Prozessaufruf und Ergebnisprüfung, Ablehnung existierender Arbeitsdateien und vorab abgebrochener Aufträge geprüft.
- Leere Cutlist-Endbereiche sind automatisiert mit den Diplomatin-Schnittwerten abgesichert. Ein separater ausdrücklicher Live-Nachweis für genau den zuvor fehlerhaften Originaleintrag ist nicht protokolliert.

Weitere Prüfschritte stehen im [Testplan](TESTPLAN.md). Aus diesen Beispielen folgt keine allgemeine Unterstützung aller AVI-/Codec-Kombinationen.

## Nächste Schritte

1. Abschließendes Review des Feature-Branches.
2. Pull Request und Übernahme nach `main` nach Freigabe.
3. Veröffentlichung vorbereiten: Installer oder portable Ausgabe, Programmsymbol und finale Produktgestaltung.

Spätere Funktionswünsche: mehrteilige Aufnahmen, Stapelverarbeitung und Smart Rendering. Eine automatische Neukodierung ist nicht Teil des aktuellen Vorbereitungsablaufs.

Der Implementierungsstand bis zur Fehleranzeige ist unter `9f04f19` auf `feature/cut-application-configuration` committed und gepusht. Die aktuelle Dokumentationsüberarbeitung wird gesondert abgeschlossen.
