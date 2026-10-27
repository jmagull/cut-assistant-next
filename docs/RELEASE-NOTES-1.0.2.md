# Cut Assistant Next 1.0.2 · Build 41

Am 27.10.2026 nach ausdrücklicher Freigabe als **v1.0.2 / Latest veröffentlicht**. CAN 1.0.2 Build 41 enthält **otr-can 0.1.6** und **libmpv 0.41.0**. Setup und Portable-ZIP gehören zum gleichen geprüften Quellenstand.

## Klebecenter

Unter **Klebecenter → Filmteile zusammenfügen …** zwei bereits geschnittene MP4-Dateien auswählen. Zuerst Reihenfolge festlegen, dann pro Teil eine Tonspur wählen. Bei mehreren Spuren gibt es keine Vorauswahl. Die allein gewählte Spur jedes Teils wird verwendet; übrige Spuren entfallen. Beide gewählten Spuren müssen Abtastrate und Kanalbelegung teilen. AAC/AAC und AC-3/AAC sind möglich.

Das Bild bleibt unverändert. Der Ton wird samplegenau verbunden und **einmal durchgehend als AAC-LC neu kodiert** (192 kbit/s bei Mono/Stereo, 384 kbit/s bei Mehrkanalton). Eine Vorbereitungskopie oder erneute Anwendung einer Cutlist ist nicht nötig. Genau zwei passende progressive H.264-MP4-Dateien mit konstanten 25/50 fps werden unterstützt; unpassende Formate oder fehlerhafte Zeitachsen werden abgelehnt.

Nach Abschluss **Anfang, Klebestelle und Ende** über „Prüfen“ kontrollieren. Die Vorschau beginnt drei Sekunden vor der tatsächlichen Klebestelle. Nach Erfolg werden Arbeitsdateien und Erfolgsprotokoll samt leerem Arbeitsordner entfernt. Bei Fehler oder Abbruch bleibt die Diagnose erhalten; gesperrte oder fremde Inhalte werden gemeldet und nicht rekursiv gelöscht.

[Anleitung](NUTZERANLEITUNG.md), [Klebecenter und Grenzen](KLEBECENTER-20261027.md), [Bereinigungsnachweis](KLEBECENTER-BEREINIGUNG-20261027.md).

## Schnitt und Ton

Der otr-can-Schnitt kann bei erkanntem Tonfehler nach ausdrücklicher Auswahl mit einer streng geprüften intakten AAC-Spur fortgesetzt werden. Entfallene Spuren werden angezeigt; Warnungen werden nicht pauschal ignoriert. Gesunde Aufträge behalten ihren normalen Ablauf. Cutlist-Metadaten nennen den vorgesehenen Schnittmotor samt Version.

Der MP4Box-Prüfablauf ordnet begrenzte Bildzeitüberlappungen am tatsächlichen Abschnittswechsel zu. Fehlende Originalbilder werden dadurch nicht wiederhergestellt; Werbeblitzer können bei MP4Box bestehen bleiben. Geeignete ältere H.264-AVI-Dateien werden experimentell ohne Video-Neukodierung für den Schnitt vorbereitet; MP3 wird beim otr-can-Schnitt einmal nach AAC-LC kodiert.

## Richtige Werkzeugversion

**Mitgeliefert: otr-can 0.1.6**, im Programmordner unter **tools/otr-can/otr_can.exe**. Nach einer Überinstallation unter **Einstellungen → OTR-CAN-Werkzeuge** diesen Pfad prüfen und die mitgelieferte EXE wählen. Eine alte gespeicherte Einstellung kann auf eine frühere Motorversion zeigen. Der frühere Hinweis „0.1.5“ wurde korrigiert; die Paketprüfung gleicht Text, tatsächliche EXE und Herkunft ab.

FFmpeg/ffprobe, GPAC/MP4Box und FFMS2/ffmsindex bleiben externe Werkzeuge. Das Klebecenter verwendet die konfigurierten FFmpeg-Werkzeuge. Windows 11 x64, .NET 10 Desktop Runtime x64 und für otr-can die Visual-C++-x64-Laufzeit werden benötigt. Details stehen im mitgelieferten Voraussetzungstext.

## Prüfstand

1489 Pakettests, zusätzlich 1217 Tests gegen finale Produktbibliotheken und 21 native Klebecenter-Fälle bestanden; Release-Build ohne Warnungen/Fehler. John Wick und Furiosa persönlich geprüft. Build-40-Setup-/ZIP-VM-Runde bestätigt; Build-41-Setup auf dem Arbeitsrechner einschließlich verschwundener temporärer Datei und Bild/Ton/Schnitt/Drei-Sekunden-Vorschau/Klebecenter positiv. Eine zusätzliche Build-41-ZIP-/VM-Prüfung wird daraus nicht abgeleitet.

Die Ursache früherer kompletter Furiosa-Rechnerstillstände bleibt ungeklärt; positive spätere Läufe belegen keine gezielte Behebung. Bitte jede Ausgabe auf Bild, Ton, Synchronität und Übergänge kontrollieren.

Die beigelegten Quellenarchive gehören exakt zu den geprüften Binärpaketen und enthalten Dokumentation aus ihrem Erstellungsstand vor der Veröffentlichungsfreigabe. Die aktuellen Repository-Unterlagen und diese Releasebeschreibung ergänzen die spätere Abnahme/Freigabe. [Paketbericht und Prüfsummen](RELEASE-1.0.2-BUILD41-RESULT.md).
