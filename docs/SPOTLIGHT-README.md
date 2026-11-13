# Spotlight-CAN – kurze Bedienung

Spotlight-CAN ist Bestandteil von Cut Assistant Next 1.0.3 Build 58.

Spotlight hilft, Werbung anhand von Vorschaubildern grob zu finden und Schnittbereiche vorzubereiten. Die genaue Kontrolle und der eigentliche Filmschnitt erfolgen anschließend in Cut Assistant Next.

## Vom Überblick zum fertigen Schnitt

1. Film in CAN öffnen und **Werkzeuge → Spotlight-CAN – Visuelle Werbesuche** wählen.
2. **Übersicht erzeugen**, einen interessanten Bereich auswählen und **Details erzeugen**.
3. Mit **Schnittbereich Beginn setzen** und **Schnittbereich Ende setzen** grobe Entfernen-Bereiche markieren. Die Laufzeitanzeige nennt Originaldauer, markierte Entfernungsdauer und voraussichtlich verbleibende Dauer.
4. **Schnittbereiche an CAN übernehmen**. Vor dem Ersetzen vorhandener Schnittarbeit wird gefragt.
5. In CAN die Grenzen mit Player und Frame-Lupe präzisieren, das Dateiende kontrollieren, Cutlist speichern und mit dem gewünschten Schnittmotor schneiden. Danach Bild, Ton, Schnittübergänge und Drei-Sekunden-Vorschau prüfen.

## Schnitt bis Videoende abschließen

1. Das erste zu entfernende Bild auswählen.
2. **Schnittbereich Beginn setzen** drücken. Der Beginn ist vorgemerkt.
3. **Schnitt bis Videoende abschließen** drücken. Das Ende wird automatisch auf die tatsächliche Mediendauer gesetzt; kein weiteres Bild auswählen.

Der vollständige Entfernen-Bereich erscheint sofort in der Schnittbereichsliste und in der Laufzeitberechnung. Ohne offenen Beginn ist der Button deaktiviert. Bereits abgeschlossene Werbeblöcke bleiben erhalten; Überschneidungen werden abgewiesen. Der Bereich lässt sich wie jeder andere korrigieren oder mit **Bereich löschen** entfernen. Eine zusätzliche Behaltebild-Markierung oder ein separates „Videoende aufheben“ gibt es nicht mehr.

Beispiel: **02:00:03.560 bis 02:09:58.000** entfernt **00:09:54.440**. Die Übernahme nach CAN berücksichtigt wie bisher Rot/Entfernen und Grün/Behalten. [Umsetzung und Tests](SPOTLIGHT-VIDEOENDE-BUILD55.md).

Die Sekunden-Sprungsteuerung ist nach der persönlichen Prüfung aus Spotlight entfernt worden. Sie liegt jetzt direkt in der CAN-Hauptplayerleiste. Spotlight behält seine Bildauswahl und Detailsuche. Pfeiltasten werden hier nicht mehr speziell abgefangen. Mehr im [Korrekturbericht zu Build 55](PLAYER-SEKUNDEN-NAVIGATION-BUILD55.md).

## Vorschaubilder und Dateigrenzen

Die Detailsuche zeigt standardmäßig alle zwei Sekunden ein Vorschaubild. **Schnitt bis Videoende abschließen** verwendet die tatsächliche Dateidauer, unabhängig vom letzten Rasterbild. **Videoanfang** setzt die CAN-Grenze auf **00:00:00.000**. Nach der Übernahme kannst du die übrigen Schnittgrenzen im CAN-Player und mit der Frame-Lupe präzisieren.

## Fenster und Werkzeuge

Fenstergröße, Bildschirmposition und Maximierung werden beim Schließen gespeichert und auch nach einem CAN-Neustart wiederhergestellt. Das funktioniert auch für weitere Monitore; eine nicht mehr passende Position wird auf eine verfügbare Bildschirm-Arbeitsfläche begrenzt.

Spotlight verwendet den bereits in CAN geöffneten Film sowie die unter **Einstellungen → FFmpeg-Werkzeuge** eingerichteten Programme. Es besitzt keinen eigenen Player und keinen eigenen Schnittmotor. Für einen Filmwechsel zuerst Spotlight schließen.

Die Bedienung gilt für die lokale Prüffassung 1.0.3 Build 58. [Videoende und Prüfungen](SPOTLIGHT-VIDEOENDE-BUILD55.md)
