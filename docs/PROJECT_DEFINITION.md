# Projektdefinition

## Ausgangslage

Der bestehende Cut Assistant 2018 basiert auf Delphi/VCL, DirectShow und installierten Windows-Codecs. Der vorhandene Quellcode dient als Referenz für bewährte Funktionen, Abläufe und Dateiformate. Geplant ist ein moderner Neuaufbau.

## Ziel

Der POC soll nachweisen, dass typische OTR-MP4-Dateien ohne DirectShow zuverlässig wiedergegeben und präzise navigiert werden können. Darauf soll später ein vollständiger Schnittassistent mit Cutlist-Unterstützung, MP4Box, FFmpeg und Dateiumbenennung aufbauen.

## Grundentscheidungen

- C# und .NET 10
- Windows 11 x64
- WPF
- mpv/libmpv
- ffprobe
- Git/GitHub
- modularer Aufbau
- kein DirectShow, kein DSPack

## Erfolgskriterien

- mindestens drei repräsentative OTR-MP4-Dateien funktionieren
- Bild und Ton laufen zuverlässig
- kein separates Active-Movie-Window
- keine DirectShow-Codecs notwendig
- Frame-Schritte und Zeitsprünge funktionieren stabil
- Fehler werden verständlich protokolliert
- reproduzierbarer Build
