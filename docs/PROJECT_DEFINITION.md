# Projektdefinition

## Aktueller Umfang – 03.11.2026

**03.11.2026 – eingefrorener Entwicklungsstand CAN 1.0.3, Buildzähler 48.** Gemeinsame Server-/Lokal-Cutlist-Tabelle, Rot/Entfernen und Grün/Behalten mit dynamischer Vorschau sowie Standard / Power-User 1 / Power-User 2 einschließlich Buttonausrichtung sind umgesetzt. Letzter technischer Stand: **1.728/1.728 Tests**, Release-Build ohne Warnungen/Fehler. Quellstand und Dokumentation werden auf Nutzerauftrag committet und gepusht; keine neuen Pakete und keine Veröffentlichung ohne gesonderte Freigabe. Öffentlich bleibt der dokumentierte stabile Stand **CAN 1.0.2 Build 48**. [Freeze und Prüfgrenzen](FREEZE-1.0.3-20261103.md).

[Entwicklungsnotizen 1.0.3](RELEASE-NOTES-1.0.3.md) · [Cutlist-Tabelle](CUTLIST-TABELLE-IMPLEMENTIERUNG-20261103.md) · [Schnittmodi und Vorschau](SCHNITTMODI-TIMELINE-20261103.md).

Zusätzlich zum bisherigen Schnittassistenten stehen in der lokalen Entwicklung die gemeinsame Auswahl eigener und Server-Cutlists sowie zwei gleichwertige Bedienweisen zur Verfügung: unerwünschte Teile rot markieren oder gewünschte Teile grün markieren. Die dynamische offene Markierung dient der Orientierung und verändert den Plan erst nach einem gültigen Ende. Kein neues Arbeitsstandsformat; das Fortsetzen basiert auf gespeicherten vollständigen Cutlist-Intervallen. Persönliche Sichtprüfung beider Modi und ein vollständiger grüner Santa-Tell-Me-Filmschnitt sind positiv gemeldet; die persönliche Wiederaufnahmeprüfung bleibt offen. [Praxisnachweis](SANTA-TELL-ME-GRUENER-PRAXISTEST-20261103.md).

Die nachfolgende Einordnung vom 27.10.2026 dokumentiert den früheren Projektumfang.

**Stand 27.10.2026: CAN 1.0.2 Build 41, otr-can 0.1.6, libmpv 0.41.0, .NET 10/WPF.** Das ursprüngliche Wiedergabe-POC-Ziel ist um Schnitt-/Cutlist-Abläufe und das integrierte Klebecenter erweitert. Eigene John-Wick-Kombinationen AC-3/AAC und AAC/AAC positiv bestätigt. Build-40-Setup-/ZIP-VM-Runde und Build-41-Arbeits-PC-Setup einschließlich Bereinigung persönlich bestätigt. CAN 1.0.2 Build 41 ist veröffentlicht; zusätzliche Build-41-ZIP-/VM-Prüfung bleibt separat offen. [Projektstand](AKTUELLER-STAND.md), [Versionen](VERSIONEN.md).

## Ausgangslage

Der bestehende Cut Assistant 2018 basiert auf Delphi/VCL, DirectShow und installierten Windows-Codecs. Der vorhandene Quellcode dient als Referenz für bewährte Funktionen, Abläufe und Dateiformate. Geplant ist ein moderner Neuaufbau.

## Ziel

Der POC soll nachweisen, dass typische OTR-MP4-Dateien ohne DirectShow zuverlässig wiedergegeben und präzise navigiert werden können. Dieser Nachweis ist inzwischen erbracht. Darauf aufbauend wird der POC schrittweise zum Schnittassistenten erweitert. Die manuelle Schnittplanung mit markierten Entfernungsbereichen, die Erzeugung und Verarbeitung klassischer Cutlists, die Anbindung an MP4Box, die Dateinamenslogik sowie der vollständige Cutlist-Server-Workflow mit automatischer Suche, Download und direktem Upload sind inzwischen umgesetzt und praktisch bestätigt. Geeignete klassische H.264-AVI-Dateien werden inzwischen experimentell vorbereitet und geprüft; ungeeignete Zeitstrukturen werden abgelehnt. Keine allgemeine AVI-Freigabe. Zusätzlich sind otr-can mit bestätigtem AAC-Notfallweg und das Klebecenter umgesetzt.

## Erreichter Zwischenstand

- Wiedergabe typischer MP4- und OTR-Dateien ist praktisch bestätigt.
- Play/Pause, Seeking und framegenaue Navigation funktionieren zuverlässig.
- Medienanalyse über ffprobe ist integriert.
- Die erste Schnittplanung mit `RemoveSegment` und `CutPlan` ist umgesetzt.
- Markierte Bereiche können gesetzt, ausgewählt, korrigiert und gelöscht werden; lokal ab 1.0.3 wahlweise Entfernen oder Behalten.
- Eine eigene Schnitt-Timeline visualisiert die markierten Bereiche.
- Aus den Remove-Bereichen werden komplementäre Keep-Bereiche für klassische Cutlists erzeugt.
- Cutlist-Metadaten, Dokumentmodell, Serialisierung und lokale Dateiausgabe sind umgesetzt und automatisiert getestet.

- Bestehende lokale und vom Server geladene Cutlists durchlaufen denselben fachlichen Lade- und Prüfweg.
- Die automatische Cutlist-Serversuche einschließlich Download ist umgesetzt und praktisch bestätigt; die lokale Entwicklung 1.0.3 ergänzt die gemeinsame Tabelle und exakte lokale Suche.
- Lokal gespeicherte oder bewusst lokal wieder geladene Cutlists können nach Benutzerbestätigung direkt auf den persönlichen Cutlist-Server hochgeladen werden.

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
