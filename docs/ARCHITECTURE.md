# Architektur – Arbeitsstand

## Zielbild

```text
WPF-Oberfläche
      |
Anwendungslogik / ViewModels
      |
+----------------------+----------------------+
| Medienwiedergabe     | Medienanalyse        |
| mpv/libmpv           | ffprobe              |
+----------------------+----------------------+
      |
Cutlists            | Schnittmotoren | Server | später: Renamer | Stapel
```

## Vorgesehene Projekte

- `CutAssistantNext.App` – WPF-Oberfläche und Startpunkt
- `CutAssistantNext.Core` – Modelle, Verträge und Anwendungslogik
- `CutAssistantNext.Media` – mpv/libmpv und ffprobe
- `CutAssistantNext.Cutlists` – Cutlist-Modell, Metadaten, Parser, Serialisierung sowie Datei-Ein- und -Ausgabe
- passende Testprojekte unter `tests/`

## Architekturregeln

- Die Oberfläche kennt keine DirectShow- oder ffprobe-Details.
- Externe Prozesse und native Bibliotheken werden hinter Schnittstellen gekapselt.
- Fachlogik bleibt unabhängig von WPF testbar.
- Pfade und Laufzeitabhängigkeiten werden nicht fest codiert.
- Fehler werden in verständliche Anwendungsmeldungen und technische Logs getrennt.

## Wiedergabe-POC – umgesetzte Architektur

Der libmpv-Spike verwendet folgende Aufteilung:

```text
CutAssistantNext.App
├── MainWindow und ViewModel
└── MpvVideoHost auf Basis von HwndHost
          |
          | stellt ein Windows-HWND bereit
          v
CutAssistantNext.Media
├── IMediaPlayerService
├── MpvMediaPlayerService
└── HanumanLibMpvClient
          |
          v
libmpv-2.dll
```

- Der WPF-spezifische Video-Host verbleibt in `CutAssistantNext.App`.
- `MpvVideoHost` erstellt ein natives untergeordnetes Windows-Fenster und stellt dessen Handle bereit.
- Die Einbettung erfolgt über die mpv-Option `wid`.
- Player-Zustand und Wiedergabesteuerung sind hinter `IMediaPlayerService` gekapselt.
- `MpvMediaPlayerService` verarbeitet Zustände, Position, Dauer, Fehler und libmpv-Ereignisse.
- `HanumanLibMpvClient` kapselt die konkrete Anbindung über `HanumanInstitute.LibMpv` 0.10.1.
- ViewModels kennen weder WPF-Fensterklassen noch mpv- oder native Typen.
- Die Ereignisschleife wird beim Beenden kontrolliert abgebrochen.
- libmpv wird freigegeben, bevor WPF das native Videofenster zerstört.
- Die native DLL wird beim Build automatisch in den Ausgabe- und Publish-Ordner kopiert.

Die native Laufzeit wird reproduzierbar mit `tools/setup-libmpv.ps1` bereitgestellt:

```text
Release:
  2026-07-30-74356c0fc6

Archiv:
  mpv-dev-lgpl-x86_64-20260730-git-74356c0fc6.7z

Archiv-SHA-256:
  a0a74229523685ba364d0c93168fa3a03e1af5b84a5bca592833b28aa9fe0023

libmpv-2.dll-SHA-256:
  B41C7D9F6499AB4F44978D6B30673EA403A471FD2069A49085C6185AB1CA3D94
```

Die eingebettete Wiedergabe wurde mit einer realen MP4-Datei einschließlich Bild und Ton erfolgreich geprüft. Die Anwendung beendet sich nach dem kontrollierten Freigeben von libmpv vollständig.

Die libmpv-Render-API bleibt eine spätere Alternative, falls HWND-Einbettung, WPF-Airspace oder gewünschte Overlays dies erforderlich machen.

## Schnittplanung – umgesetzte Architektur

- `RemoveSegment` in `CutAssistantNext.Core` beschreibt einen Bereich, der entfernt werden soll.
- `CutPlan` verwaltet Mediendauer und Entfernungsbereiche, sortiert sie chronologisch und verhindert ungültige Überschneidungen.
- `CutPlanViewModel` bildet Erfassung, Auswahl, Korrektur und Löschen für die WPF-Oberfläche ab.
- `CutTimelineTrack` visualisiert die Schnittbereiche, enthält aber keine fachliche Schnittlogik.
- Tabelle und Schnitt-Timeline verwenden dieselbe Auswahl.
- Die Benutzeroberfläche arbeitet bewusst mit Remove-Bereichen.
- Die klassische Cutlist-Ausgabe berechnet daraus die komplementären Keep-Bereiche.

## Cutlist-Erzeugung – umgesetzte Architektur

Die Cutlist-Erzeugung ist als eigenes Modul umgesetzt und von WPF sowie konkreten Schnittmotoren getrennt.

```text
CutAssistantNext.Core
├── Editing
│   ├── RemoveSegment
│   └── CutPlan
├── Naming
│   ├── NameTemplateContext
│   └── NameTemplateRenderer
└── Metadata
    ├── TechnicalNotice
    └── TechnicalNoticeDetector
          |
          v
CutAssistantNext.Cutlists
├── Editing
│   ├── CutlistKeepSegment
│   └── CutlistKeepSegmentBuilder
├── Metadata
│   ├── CutlistGeneralMetadata
│   ├── CutlistInfoMetadata
│   └── CutApplicationInfo
├── Model
│   └── CutlistDocument
├── Serialization
│   ├── CutlistParser
│   └── CutlistSerializer
└── IO
    ├── CutlistFileReader
    └── CutlistFileWriter
```

Architekturregeln:

- `CutAssistantNext.Cutlists` hängt von `CutAssistantNext.Core` ab, nicht von `CutAssistantNext.App` oder `CutAssistantNext.Media`.
- Die Benutzeroberfläche und das Core-Schnittmodell arbeiten mit Remove-Bereichen; `CutlistKeepSegmentBuilder` bildet daraus die komplementären Keep-Bereiche für das klassische Cutlist-Format.
- `NoOfCuts` wird aus der tatsächlichen Anzahl der Keep-Bereiche abgeleitet und gegen das Dokumentmodell validiert.
- `CutlistDocument` bündelt `[General]`, Keep-Bereiche und `[Info]` als typisiertes Modell.
- `CutlistSerializer` erzeugt den Cutlist-Inhalt unabhängig vom Dateisystem.
- `CutlistFileWriter` übernimmt die Dateiausgabe als UTF-8 ohne BOM mit CRLF-Zeilenenden.
- Zeit- und Zahlenwerte werden kulturunabhängig serialisiert.
- `StartFrame` und `DurationFrames` bleiben optionale spätere Kompatibilitätsfelder.
- Technische Hinweise wie eine `.avi`-Dateiendung bei tatsächlich erkanntem MP4/ISO-BMFF-Container entstehen in `CutAssistantNext.Core` und werden erst im Cutlist-Modul auf `OtherError` und `OtherErrorDescription` abgebildet.
- Die Namensbildung liegt in `CutAssistantNext.Core.Naming`, weil derselbe erzeugte Basisname für `SuggestedMovieName` und später für den Ausgabedateinamen verwendet werden soll.
- Namensmaske, Standardautor und Schnelltexte sind Benutzereinstellungen und keine fest codierten Cutlist-Werte.
- Die Identität eines Schnittprogramms wird über `CutApplicationInfo` beschrieben. Der lokale Installationspfad eines Schnittmotors gehört nicht in die Cutlist-Metadaten.
- Cutlist-Reader und Parser sind umgesetzt. Beim Einlesen wird zunächst UTF-8 verwendet; historische Windows-1252-/ANSI-Dateien werden als Kompatibilitätsfall unterstützt.
- Serversuche, Download und direkter Upload sind in `CutAssistantNext.App` gekapselt und verwenden dieselbe konfigurierte persönliche Server-URL.
- Heruntergeladene Server-Cutlists verwenden denselben fachlichen Lade- und Prüfweg wie lokale Cutlists.
- Der direkte Upload arbeitet ausschließlich mit einer zuvor lokal gespeicherten oder bewusst lokal geladenen Cutlist.
- Für den Server-Upload wird aus dem gespeicherten `CutlistDocument` eine separate kompatible Kopie erzeugt; die lokale Original-Cutlist wird nicht verändert.

Die Kompatibilitätsdetails der erzeugten Dateien sind in ADR-007 festgelegt und durch automatisierte Tests abgesichert.

## Offene Entscheidungen

- möglicher späterer Wechsel von der HWND-Einbettung zur libmpv-Render-API
- Strategie für Aktualisierungen der fest versionierten nativen Laufzeit
- MVVM-Hilfsbibliothek oder möglichst wenige externe Pakete
- Installer-/Portable-Konzept
