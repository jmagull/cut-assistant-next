# Prüfhinweise – Cut Assistant Next 1.0.3 Build 58

Diese lokale Prüffassung trägt die Dateiversion **1.0.3.58**. Die öffentliche GitHub-Version ist Build 54. Die Portable-ZIP vollständig in einen neuen Ordner entpacken und **CutAssistantNext.App.exe** starten. Die Voraussetzungen für Setup und ZIP stehen in **VORAUSSETZUNGEN.txt**. Beide enthalten den Motor **0.1.6-audio-diag.1** unter `tools/otr-can/otr_can.exe` mit Quellen und Lizenzhinweisen.

## Persönlich bestätigter Funktionsstand

- **MP4Box:** Wild Card mit zwei AAC-Tonspuren, Long Lost Christmas und NCIS: Sydney – Mango Madness wurden in Build 56 erfolgreich geschnitten. Die MP4Box-Schnittgrenzen hängen von den Keyframes ab. Bei Wild Card traten noch zwei kurze Werbeblitzer auf. [Technischer Nachweis](MP4BOX-WILD-CARD-ZUSAMMENFUEHRUNG-BUILD56.md)
- **Cutlist-Bewertung:** Fünf Sterne wurden auf dem Server gespeichert. CAN zeigte „Mit 5 bewertet“ dauerhaft an und entfernte den Eintrag aus den unbewerteten Downloads. [Bewertungsablauf](CUTLIST-BEWERTUNG-BUILD55.md)
- **Schnittshuffle und Filmende:** Der Anwender bestätigte den Realfilm-Test mit Build 58. Die Pfeile springen zwischen den Grenzen des ausgewählten Bereichs. Am tatsächlichen Filmende bleibt der Hauptplayer pausiert und bedienbar. Nach einem Slider-Rücksprung kann die Wiedergabe fortgesetzt werden. [Schnittshuffle](SCHNITTSHUFFLE-BUILD57.md) · [Playerkorrektur](PLAYER-FILMENDE-BUILD58.md)
- **Spotlight-CAN:** „Schnitt bis Videoende abschließen“ schließt einen offenen Entfernen-Bereich an der vollen Mediendauer. [Videoende](SPOTLIGHT-VIDEOENDE-BUILD55.md)

## Persönliche Abnahme der Pakete

Mit dem vollständig entpackten ZIP oder dem Setup in einer isolierten Windows-11-Umgebung die Version im Fenstertitel prüfen und einen Film laden. Einen vollständigen roten und grünen Schnittbereich auswählen, zwischen den Grenzen springen und Auswahl, Bearbeitungskante sowie Zeitleiste prüfen. Den Film bis zum tatsächlichen Ende abspielen, Play betätigen und den Slider anschließend zurückbewegen. Play und Bild-, Sekunden- und Keyframe-Navigation erneut prüfen.

Einen Mehrsegment-Schnitt mit MP4Box einschließlich Tonspuren und Übergängen prüfen, die mitgelieferte otr-can-Version unter den Werkzeugeinstellungen prüfen und Spotlight-CAN öffnen. CAN normal schließen und erneut öffnen. Die automatisierten Prüfungen des konkreten Paketsatzes stehen im separaten Paketbericht.
