# Cut Assistant Next – Proof of Concept

Moderner Nachfolger des Cut Assistant für Windows 11 – ohne DirectShow und ohne Abhängigkeit von installierten Windows-Codecs.

## Aktuelle Projektphase

**Proof of Concept 0.1: Medienwiedergabe und Navigation**

Der erste Prototyp soll nachweisen, dass typische OTR-MP4-Dateien mit mpv/libmpv zuverlässig geöffnet, abgespielt und präzise navigiert werden können.

## Technische Grundlage

- C# und .NET 10
- WPF
- mpv/libmpv für Videowiedergabe
- ffprobe für Medieninformationen
- Git/GitHub für Versionsverwaltung
- Codex als Programmierwerkstatt
- ChatGPT für Architektur, Planung, Testauswertung und Dokumentation

## POC-0.1-Funktionsumfang

- MP4-Datei öffnen
- eingebettete Videowiedergabe
- Play/Pause
- Positionsanzeige und Zeitleiste
- Einzelbild vorwärts und rückwärts
- Lautstärkeregelung
- ffprobe-Medieninformationen
- verständliche Protokolldatei

Noch nicht enthalten: Cutlists, Cutlist-Server, MP4Box-/FFmpeg-Schnitt, Stapelverarbeitung und automatische Umbenennung.

## Repository-Struktur

```text
cut-assistant-next/
├── AGENTS.md
├── README.md
├── docs/
├── src/
├── tests/
├── samples/cutlists/
└── tools/
```

Die Visual-Studio-Solution und die Projekte werden im nächsten Entwicklungsschritt angelegt.

## Arbeitsgrundsatz

> Eine Funktion planen, umsetzen, testen, dokumentieren und erst danach den nächsten Schritt beginnen.
