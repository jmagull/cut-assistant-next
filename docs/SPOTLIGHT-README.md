# Spotlight-CAN – kurze Bedienung

Spotlight hilft, Werbung anhand von Vorschaubildern grob zu finden und Schnittbereiche vorzubereiten. Die genaue Kontrolle und der eigentliche Filmschnitt erfolgen anschließend in Cut Assistant Next.

## Vom Überblick zum fertigen Schnitt

1. Film in CAN öffnen und **Werkzeuge → Spotlight-CAN – Visuelle Werbesuche** wählen.
2. **Übersicht erzeugen**, einen interessanten Bereich auswählen und **Details erzeugen**.
3. Mit **Schnittbereich Beginn setzen** und **Schnittbereich Ende setzen** grobe Entfernen-Bereiche markieren. Die Laufzeitanzeige nennt Originaldauer, markierte Entfernungsdauer und voraussichtlich verbleibende Dauer.
4. **Schnittbereiche an CAN übernehmen**. Vor dem Ersetzen vorhandener Schnittarbeit wird gefragt.
5. In CAN die Grenzen mit Player und Frame-Lupe präzisieren, das Dateiende kontrollieren, Cutlist speichern und mit dem gewünschten Schnittmotor schneiden. Danach Bild, Ton, Schnittübergänge und Drei-Sekunden-Vorschau prüfen.

## Wichtig: letzter Bereich und tatsächliches Videoende

**Ein Vorschaubild ist nicht automatisch das letzte Bild des Originalfilms.** Die Detailsuche verwendet standardmäßig **2 Sekunden Bildabstand**. Zwischen den angezeigten Bildern liegen weitere Filmframes; auch am Ende kann hinter dem letzten Rasterbild noch ein Rest liegen. Der Abstand lässt sich unter **Abstand (s)** ändern. Spotlight zeigt keine lückenlose Folge aller Filmframes.

**Wenn ein Schnittbereich bis zum tatsächlichen Dateiende reichen soll, nach der Übernahme in CAN den betreffenden letzten Bereich auswählen und mit „Videoende“ dessen Ende setzen.** Bei einem neu begonnenen Bereich erst den Anfang setzen und anschließend **Videoende** drücken. Dieser Button setzt die Schnittgrenze auf die vollständige Dateidauer, unabhängig vom letzten Spotlight-Vorschaubild.

Im roten Entfernen-Modus betrifft das beispielsweise die Nachlaufwerbung bis zum Ende des Originals. Im grünen Behalten-Modus nur dann bis **Videoende** verlängern, wenn dieser Rest tatsächlich erhalten bleiben soll. Die Farb-/Schnittarbeitsweise beachten; nicht versehentlich Werbung behalten.

Der Zwei-Sekunden-Abstand ist eine normale Eigenschaft der groben Bildauswahl. Die Feinkorrektur einschließlich des exakten Videoendes gehört zum anschließenden CAN-Arbeitsschritt.

## Fenster und Werkzeuge

Fenstergröße, Bildschirmposition und Maximierung werden beim Schließen gespeichert und auch nach einem CAN-Neustart wiederhergestellt. Das funktioniert auch für weitere Monitore; eine nicht mehr passende Position wird auf eine verfügbare Bildschirm-Arbeitsfläche begrenzt.

Spotlight verwendet den bereits in CAN geöffneten Film sowie die unter **Einstellungen → FFmpeg-Werkzeuge** eingerichteten Programme. Es besitzt keinen eigenen Player und keinen eigenen Schnittmotor. Für einen Filmwechsel zuerst Spotlight schließen.

Stand: CAN 1.0.3 Build 50, 08.11.2026. Spotlight zur Grobauswahl und maximierte Fensterwiederherstellung persönlich mit der Setup-Fassung auf dem Arbeitsrechner bestätigt. Die finale Schnittkontrolle bleibt erforderlich.
