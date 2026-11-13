# Prüfhinweise – Cut Assistant Next 1.0.3 Build 56

Die lokale Prüffassung trägt die Dateiversion **1.0.3.56**. Voraussetzungen und externe Werkzeuge stehen in **VORAUSSETZUNGEN.txt**. Die Portable-ZIP vollständig in einen frischen Ordner entpacken und **CutAssistantNext.App.exe** starten. Beide Programmpakete enthalten otr-can **0.1.6-audio-diag.1** unter **tools/otr-can/otr_can.exe** samt Herkunft, Lizenztexten und Motorquellen.

- **MP4Box:** Wild Card mit zwei AAC-Spuren und fünf Segmenten sowie Long Lost Christmas mit vier und NCIS: Sydney – Mango Madness mit drei Segmenten wurden mit Build 56 persönlich erfolgreich geschnitten. Die Regressionstests decken AAC, AAC+AC-3, AAC+AAC und AC-3+AC-3 ab. Bei Wild Card wurden mit MP4Box zwei bekannte Werbeblitzer beobachtet.
- **otr-can:** Der persönliche Wild-Card-Gegentest mit beiden AAC-Spuren verlief erfolgreich und ohne beobachtete Werbeblitzer.
- **Spotlight:** Einen offenen Entfernen-Bereich mit **Schnitt bis Videoende abschließen** bis zur tatsächlichen Dateidauer schließen. Die CAN-Grenze **Videoanfang** ist **00:00:00.000**.
- **Bewertung und Namen:** Der persönliche Cutlist-Bewertungstest bestätigte Speicherung, dauerhafte Serveranzeige und Entfernen aus der Liste unbewerteter Downloads. **Aus obigen Eingaben neu erzeugen** wendet die aktuelle Standardmaske auf die ausgefüllten Namensfelder an. Der Ausgabename bleibt bearbeitbar.

Für die Sichtung Bild, jede Tonspur, Übergänge und Drei-Sekunden-Vorschau prüfen. Weitere Bedienung in **NUTZERANLEITUNG.md** und **README-Spotlight.md**, technische Details im vollständigen Quellarchiv unter **docs/MP4BOX-WILD-CARD-ZUSAMMENFUEHRUNG-BUILD56.md**.
