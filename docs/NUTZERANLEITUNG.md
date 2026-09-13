# Nutzeranleitung – Cut Assistant Next

Stand: 13.09.2026. Diese Anleitung beschreibt den aktuellen Entwicklungsstand für Windows 11 x64. Die Vorbereitung von AVI und anderen Containern ist experimentell.

Im Fenstertitel und oben in der Oberfläche stehen Version und Buildnummer, zum Beispiel **Cut Assistant Next · 0.2.0 · Build 1**. Bitte diese Angaben bei Fehlermeldungen mit angeben.

## 1. Einmalig einrichten

Starte die aktuelle Programmversion aus ihrem vollständigen Ausgabeordner. Die Datei `libmpv-2.dll` gehört neben das Programm; kopiere nicht nur die EXE allein. Auf dem Rechner muss die passende .NET-10-Desktop-Laufzeit vorhanden sein. Ein Installer ist noch nicht Teil dieses Standes.

Öffne die Einstellungen und hinterlege die benötigten Werkzeuge:

| Menü | Einstellung | Wofür? |
|---|---|---|
| Einstellungen → FFmpeg-Werkzeuge … | `ffprobe.exe` | Analyse der Original- und Arbeitsdatei |
| Einstellungen → FFmpeg-Werkzeuge … | `ffmpeg.exe` | Experimentelles verlustfreies Umpacken vor dem Schnitt |
| Einstellungen → Schnittanwendung … | Programmdatei `mp4box.exe` | Schneiden und Zusammenfügen |
| Einstellungen → Cutlist-Einstellungen … | Persönliche Server-URL | Cutlists suchen, herunterladen und hochladen |
| Einstellungen → Cutlist-Einstellungen … | Standardautor und Kommentarbausteine | Neue Cutlists vorbereiten |
| Einstellungen → Namensmaske … | Standard-Namensmaske | Namen der geschnittenen Ausgabe bilden |

Wähle jeweils die ausführbare Datei, nicht nur ihren Ordner. Der aktuelle Schnittablauf erwartet MP4Box als Schnittanwendung. Ein beliebiges anderes Programm wird durch Eintragen seines Pfades nicht zu einem unterstützten Schnittmotor.

Ohne Server-Konfiguration kannst du Videos weiterhin bearbeiten und lokale Cutlists verwenden. Ohne MP4Box kannst du Schnittmarken vorbereiten und Cutlists speichern, aber keine Videoausgabe schneiden.

## 2. Video laden

Klicke oben auf **Videodatei laden** und wähle das Originalvideo. Der Dateidialog bietet unter anderem MP4, AVI, MKV, MOV, TS, MPEG und WMV an. „Alle Dateien“ erlaubt die Auswahl weiterer Endungen. Die Auswahlmöglichkeit ist keine Zusage, dass jeder enthaltene Codec verarbeitet werden kann.

CAN analysiert die Datei und lädt sie in den Player. Unter **Video-Informationen …** findest du Container, Video- und Audiowerte. Maßgeblich ist der erkannte Inhalt: Eine Datei mit `.avi` am Ende kann bereits MP4-Inhalt enthalten.

Beim Laden wird noch nichts umgewandelt. Die Originaldatei bleibt Grundlage der Bearbeitung.

## 3. Eine Cutlist wählen oder selbst beginnen

Ist eine persönliche Server-URL eingerichtet, sucht CAN nach dem Laden automatisch mit dem unveränderten Originaldateinamen. Der Treffer-Dialog zeigt die verfügbaren Cutlists sowie Informationen zur Auswahl, darunter Autor, Bewertung, Format, vorgeschlagener Name und Kommentar.

- **Cutlist laden** übernimmt die ausgewählte Server-Cutlist.
- **Ohne Cutlist fortfahren** schließt den Dialog. Du kannst eigene Schnittmarken setzen.
- **Cutlist vom Server laden** in der Hauptoberfläche öffnet die Suche später erneut. So kannst du eine andere Cutlist ausprobieren, ohne das Video neu zu laden.
- **Lokale Cutlist laden** öffnet eine bereits vorhandene `.cutlist`-Datei auf deinem Rechner.

Das Laden einer anderen Cutlist übernimmt deren Schnittplan. Sichere eigene Änderungen vorher mit „Cutlist erzeugen“, wenn du sie behalten möchtest.

### Warnungen beim Einlesen

Eine andere Dateigröße kann auf eine andere Aufnahme oder Qualitätsstufe hinweisen. CAN zeigt die Abweichung und fragt, ob du trotzdem laden möchtest. Bestätige bewusst und kontrolliere die Schnittstellen; eine passende Laufzeit allein beweist nicht, dass jede Schnittmarke passt.

Cutlists aus anderen Formaten können dieselbe Timeline beschreiben. Deshalb werden sie nicht allein aufgrund des Formats ausgefiltert.

Leere Bereiche mit exakt null Dauer werden beim Import ignoriert. Die übrigen gültigen Bereiche bleiben erhalten; die Cutlist-Datei auf der Festplatte wird dabei nicht verändert. Widersprüchliche oder negative Angaben bleiben Fehler. Schnittpositionen hinter dem tatsächlichen Videoende werden geprüft; erkannte kurze Endfragmente können über den vorhandenen Korrekturdialog behandelt werden.

## 4. Schnittmarken bearbeiten

CAN markiert **Bereiche, die entfernt werden sollen**. Die roten Abschnitte der Schnitt-Timeline stehen für Vorlauf, Werbung, Nachlauf oder andere unerwünschte Teile.

1. Gehe im Player an den Anfang eines zu entfernenden Abschnitts.
2. Klicke **Schnittanfang setzen**.
3. Gehe an das Ende dieses Abschnitts und klicke **Schnittende setzen**.
4. Wiederhole das für weitere Abschnitte.

Mit **Videoanfang** bzw. **Videoende** kannst du die entsprechende Dateigrenze verwenden. Bestehende Bereiche lassen sich in der Tabelle oder Timeline auswählen und korrigieren. **Bereich löschen** entfernt die ausgewählte Markierung aus dem Schnittplan; es löscht keine Videodatei.

Play/Pause, Zeitleiste, **−10 Bilder**, **Bild zurück**, **Bild vor** und **+10 Bilder** helfen bei der Positionierung. Einzelbildschritte sind im pausierten Zustand verfügbar. Tastatur: Leertaste für Play/Pause, Pfeil links/rechts für ein Bild, Strg+Pfeil links/rechts für zehn Bilder.

Die Anzeige **Geschnitten** ist eine Vorschau der geplanten Ausgabelaufzeit. Prüfe das tatsächliche Schnittergebnis im Player; Schnittmotor und Videostruktur können die exakten Grenzen beeinflussen.

## 5. Nur eine Cutlist erstellen

Mit **Cutlist erzeugen** öffnest du den Erstellungsdialog. Prüfe die Metadaten, den vorgeschlagenen Namen und den Kommentar und speichere die `.cutlist` lokal.

Dafür muss das Video nicht geschnitten oder nach MP4 vorbereitet werden. CAN kann somit auch ausschließlich zum Erstellen und Korrigieren von Cutlists verwendet werden.

Das klassische Cutlist-Format beschreibt die zu behaltenden Abschnitte. CAN rechnet seine Entfernbereiche beim Erzeugen entsprechend um. Der Bezug zur Originaldatei bleibt bestehen.

## 6. Video schneiden

Klicke **Schneiden** in der Hauptoberfläche.

### Wenn bereits MP4-Inhalt vorliegt

CAN verwendet den bestehenden MP4Box-Ablauf. Das gilt auch für MP4-Inhalt mit einer anderen Dateiendung.

### Wenn AVI oder ein anderer Container erkannt wurde

CAN zeigt zuerst einen Hinweis mit dem erkannten Container und fragt, ob es die experimentelle Vorbereitung durchführen soll:

- **Ja:** Weiter zum Schnittdialog; vor dem eigentlichen Schnitt wird eine temporäre MP4 erzeugt.
- **Nein:** Zurück zur Bearbeitung. Originalvideo und Schnittmarken bleiben geladen.

Die Vorbereitung kopiert die Streams mit FFmpeg ohne Neukodierung. Das Original bleibt unverändert. Falls ein Stream nicht in MP4 übernommen werden kann, erscheint ein Fehler; CAN startet nicht selbstständig eine verlustbehaftete Konvertierung.

### Namen und Ziel wählen

Im Schnittdialog prüfst du Name, gegebenenfalls Staffel, Folge und Folgentitel sowie die Vorschau. Ein Namensvorschlag aus der Cutlist kann bereits übernommen sein. **Aus obigen Eingaben neu erzeugen** bildet die Vorschau erneut aus den Eingaben und der Namensmaske.

Klicke im Dialog auf **Schneiden …** und wähle einen Zielnamen für die MP4-Datei. Verwende einen anderen Pfad als den der Originaldatei. Eine bereits vorhandene Ausgabe wird nur nach der entsprechenden Bestätigung ersetzt.

### Vorbereitung und Fortschritt

Der Dialog **Video vorbereiten und schneiden** zeigt den aktuellen Schritt und darunter das standardmäßig geöffnete **Protokoll**. Dieses lässt sich einklappen und über **In Zwischenablage** kopieren.

Bei experimenteller Vorbereitung geschieht Folgendes:

1. FFmpeg kopiert die Streams in eine temporäre MP4.
2. CAN prüft, ob die Medieninformationen im Wesentlichen zum Original passen.
3. Kleine Unterschiede, beispielsweise zwei Frames, werden im Protokoll vermerkt. Deutliche Abweichungen stoppen den Ablauf.
4. MP4Box schneidet die bereits gewählten Bereiche und erstellt die Ausgabe.
5. Die temporäre Arbeitsdatei wird nach dem Vorgang entfernt. Bei einem weiteren Schnittversuch wird sie erneut erzeugt.

Die Prüfungen sind keine vollständige Kontrolle aller Bildzeitstempel. Kontrolliere Anfang, Ende, Werbegrenzen und Ton-Synchronität der fertigen Datei.

**Abbrechen** fordert den Abbruch an. Warte, bis er abgeschlossen ist. Nach einem Fehler bleibt das Fenster offen und zeigt den Grund direkt oberhalb des Protokolls. Bei Erfolg meldet es **Fertig.** und bietet einen Schließen-Countdown an.

## 7. Cutlist auf den Server hochladen

Speichere eine selbst erstellte oder überarbeitete Cutlist zunächst lokal. Danach kannst du **Cutlist auf Server hochladen** verwenden. Auch eine bewusst lokal geladene Cutlist kann als Upload-Kandidat dienen. Prüfe vor der Bestätigung, ob sie zur aktuell geladenen Originaldatei gehört und sinnvoll kommentiert ist.

Eine frisch vom Server heruntergeladene temporäre Cutlist wird nicht unmittelbar als eigener Upload-Kandidat behandelt. Bearbeite und speichere sie zuerst als eigene Fassung. Beim Laden eines anderen Videos wird der bisherige Upload-Kandidat verworfen.

Teile Cutlists für die regulär verfügbaren Originalaufnahmen. Die von CAN intern erzeugte MP4-Arbeitsdatei dient nur dem lokalen Schnitt; ihre temporäre Identität wird nicht zur Grundlage einer neuen Cutlist gemacht. Eine allgemeine Herkunftserkennung für jede extern erstellte und manuell geladene Remux-Datei ist damit nicht zugesichert.

## 8. Fenster, Einstellungen und Protokolle

Fenstergröße und Maximierung werden beim Schließen gespeichert und beim nächsten Start wiederhergestellt, auch ohne zuvor ein Video zu laden. Die Lautstärke wird ebenfalls gespeichert.

Bei geringer Breite umbrechen Bediengruppen. Bei geringer Höhe erreichst du die unteren Bereiche über die rechte Scrollleiste.

Die lokalen Einstellungen liegen unter `%LOCALAPPDATA%\Cut Assistant Next\Settings`. Das allgemeine Anwendungsprotokoll liegt unter `%LOCALAPPDATA%\Cut Assistant Next\Logs\CutAssistantNext.log`. Das technische Schnittprotokoll kannst du im Fortschrittsdialog kopieren, bevor du ihn schließt.

Über **Hilfe → Nutzeranleitung (GitHub)** öffnest du diese Anleitung im Standardbrowser. **Hilfe → GitHub-Projekt** führt zur Projektseite. Auch **Update/GitHub** öffnet die Projektseite im Browser. Diese Links benötigen eine Internetverbindung.

Unter **Credits** findest du die Danksagung mit Links zu den beteiligten Projekten und Informationsseiten. Der Text lässt sich scrollen; die Links öffnen sich im Standardbrowser.

## 9. Häufige Probleme

| Meldung oder Beobachtung | Nächster Schritt |
|---|---|
| Gültigen Pfad zu ffprobe.exe einstellen | Unter FFmpeg-Werkzeuge die vorhandene Programmdatei auswählen; ffprobe wird auch zur Kontrolle der Arbeitsdatei benötigt. |
| ffmpeg.exe nicht gefunden | FFmpeg-Pfad prüfen. Für direkte MP4-Schnitte ist kein Remux nötig. |
| Keine Schnittanwendung konfiguriert | Unter Schnittanwendung den Pfad zu MP4Box eintragen. |
| Keine Server-Treffer | Originaldateiname und persönliche Server-URL prüfen; lokale Cutlist laden oder selbst markieren. |
| Cutlist enthält überlappende oder falsch sortierte Schnittbereiche | Die Cutlist wurde nicht geladen. Bitte eine andere Cutlist wählen. |
| Dateigröße laut Cutlist weicht ab | Aufnahme/Qualitätsstufe vergleichen; nur bewusst fortfahren und Grenzen kontrollieren. |
| Vorbereitung nicht möglich oder Prüfung gestoppt | Konkreten Fehler lesen und das Protokoll kopieren. Nicht jeder Codec lässt sich verlustfrei nach MP4 übernehmen. |
| Temporäre Datei konnte nicht gelöscht werden | Den im Protokoll genannten Pfad notieren; nach Ende des Vorgangs prüfen, ob die Datei noch von einem Programm verwendet wird. |
| Schnittlauf abgebrochen oder fehlgeschlagen | Die Ausgabe wird erst nach erfolgreichem Zusammenfügen übernommen. Bei Fehler oder Abbruch wird die temporäre Ausgabe aufgeräumt; eine bereits vorhandene Zieldatei bleibt erhalten. |

## 10. Experimenteller Umfang und Testbasis

Der integrierte AVI-Ablauf wurde mit **Die Diplomatin – Tod einer Nonne** und **Rubikon**, jeweils OTR-HD-AVI, mit jeweils zwei Cutlists erfolgreich geprüft. Die ausgegebenen MP4-Dateien enthielten H.264 mit 1280×720 bei 50 fps sowie MP3-Stereoton. Der Nutzer bestätigte Schnittgrenzen und Ton-Synchronität.

Das sind konkrete Beispiele, keine Garantie für alle historischen AVI-, MKV- oder sonstigen Codec-Kombinationen. Weitere technische Prüfergebnisse stehen im [Testplan](TESTPLAN.md), die genauen aktuellen Toleranzen im [Projektstatus](STATUS.md).
