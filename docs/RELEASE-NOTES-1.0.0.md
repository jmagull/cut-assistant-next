# Cut Assistant Next 1.0.0 · Build 32

Stabile Erstveröffentlichung am **18.10.2026** ausdrücklich freigegeben; persönliche Setup-/Deinstallations-/Portable-Abnahme abgeschlossen. Die unveränderten Pakete werden als stabile Latest-Ausgabe veröffentlicht; [Ausführung und Uploadnachweise](RELEASE-PUBLICATION-1.0.0.md).

## Neu gegenüber 0.3.0 · Build 31 · RC3

- Videodatei beim Programmstart übergeben, beispielsweise `CutAssistantNext.App.exe "D:\Videos\Film.mp4"`.
- Videodateien direkt in den Player ziehen und über den gewohnten Ladeweg öffnen.
- Unter **Einstellungen → Darstellung** zwischen Standard- und Power-User-Ansicht wechseln. Die Power-User-Ansicht bietet den großen Player links und Schnittwerkzeuge rechts. Wiedergabeposition und Schnittzustand bleiben beim Wechsel erhalten.
- Rechts klar gegliederte Bereiche für Schnittpunkte/-liste, **Film schneiden**, **Cutlists** und **Namensmaske**. Standardansicht vollständig erhalten.
- Stabile Versionskennung **1.0.0** ohne RC-Zusatz.

Weiterhin enthalten: MP4Box und optionales otr-can 0.1.5, präzise Frame-/Keyframe-Navigation, Frame-Lupe, Halbierungssuche, Cutlists, Namensmasken und Kontrollplayer. AVI-Schutz und otr-can-Zeitstrukturprüfung bleiben bestehen.

## Voraussetzungen und bekannte Grenzen

Windows 11 x64 und .NET 10 Desktop Runtime x64. Externe FFmpeg/ffprobe-, GPAC/MP4Box- und FFMS2-Werkzeuge müssen eingerichtet sein. Für otr-can wird zusätzlich die passende Microsoft-VC++-Laufzeit benötigt. Laufzeiten und externe Medienwerkzeuge sind nicht im Paket enthalten; der geprüfte libmpv-Satz und otr-can 0.1.5 sind enthalten.

AVI ist experimentell; Frame-Lupe, Hauptplayer-Keyframes und Halbierungssuche sind für erkannte AVI-Container gesperrt. Ungeeignete Bildzeiten und nicht unterstützte Zeitstrukturen werden vor dem otr-can-Schnitt abgelehnt. Keine automatische Reparatur oder Bildratenumrechnung.

MP4Box arbeitet schnell über kopierte Abschnitte und garantiert keine framegenauen Übergänge. Bei **Radical – Eine Klasse für sich (HD)** ist ein Werbeblitzer bei **57:27.314** belegt; die otr-can-0.1.5-Ausgabe ist persönlich nach Bild, Ton und allen Übergängen als blitzfrei bestätigt. Auch der bestehende MP4Box-Befund bei zwei gleichartigen AAC-Spuren bleibt dokumentiert. Nicht sämtliche Medienformate sind fehlerfrei unterstützt.

Auch bei **Toxic Folge 1 und 4** wurden MP4Box-Anfangsblitzer gemeldet; otr-can ist dort persönlich positiv geprüft. Bei **Toxic Folge 5** war MP4Box nach persönlicher Lupen-/Keyframe-Korrektur am Anfang blitzfrei. Das ist keine allgemeine Garantie. Cutlist vollständig einschließlich Schlussbereich erstellen: otr-can benötigt vorhandenen Originalton für alle Behaltebereiche und füllt fehlende Samples nicht automatisch mit Stille auf. Der zunächst fehlgeschlagene Doc-Zwischenlauf mit unfertiger Cutlist ist nach deren Fertigstellung mit beiden Motoren erfolgreich. [Einzelne Praxistests und Grenzen](RELEASE-1.0.0-BUILD32-RESULT.md).

## Prüfung und Pakete

Persönlich bestätigt: Kommandozeilenstart, Drag & Drop, Power-User-Layout, Frame-Lupe, Frame-Liste sowie erfolgreiche Schnitte mit beiden Motoren. Vorbereitungsstand: **1347/1347 automatisierte Tests**, Release-Build ohne Warnungen/Fehler. Die abschließenden 1.0-Paketnachweise und SHA-256-Prüfsummen stehen im [Paketprüfbericht](RELEASE-1.0.0-BUILD32-RESULT.md). Am **18.10.2026** sind VM-Setup, Darstellungswechsel, AVI-Schnitte mit beiden Motoren samt Bild/Ton und Schutzsperren, MP4-Frame-Lupe sowie Deinstallation persönlich bestätigt. **Portable/ZIP ist anschließend ebenfalls persönlich positiv bestätigt:** anderer otr-can-Pfad, MP4-Schnitte mit beiden Motoren samt Bild/Ton und Halbierungssprung sowie AVI-Schnitte mit beiden Motoren. Setup-/Deinstallations-/Portable-Paketabnahme abgeschlossen; stabile Veröffentlichung am 18.10.2026 ausdrücklich freigegeben.

Setup, Portable-ZIP, passende Quellen-/Lizenzpakete und Prüfsummen werden gemeinsam bereitgestellt. Ältere RC-Releases bleiben unverändert verfügbar. [Benutzeranleitung](NUTZERANLEITUNG.md), [Freeze-Umfang](FREEZE-1.0.0-20261016.md).
