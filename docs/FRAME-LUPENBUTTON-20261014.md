# Sichtbarer Frame-Lupenbutton – 14.10.2026

Lokale Prüffassung auf Basis **CAN 0.3.0 · Build 29 · RC3**. Ausschließlich Priorität 1 der Übergabe vom 14.10.2026 bearbeitet. Persönliche Abnahme des Lupenbuttons am 14.10.2026 positiv bestätigt; öffentliche Build-29-Pakete bleiben eingefroren.

## Änderung

- `src/CutAssistantNext.App/MainWindow.xaml`: **🔍 Frame-Lupe** unmittelbar rechts des gesamten Anzeigebereichs **Vorgemerkter Anfang**. Die Zeit hat eine feste Breite von 150 WPF-Einheiten; 10 Einheiten Abstand zum Button. Damit passen auch neunstellige Gesamtstunden mit Minuten, Sekunden und Millisekunden. Der Wechsel zwischen leerer und gefüllter Zeit verschiebt den Button nicht.
- Anzeige und Button bilden eine horizontale Gruppe innerhalb des bestehenden WrapPanel. Bei ausreichender Breite bleibt alles in einer Zeile; kleine Fenster verwenden weiterhin den vorhandenen automatischen Umbruch. Anzeige und Button werden dabei gemeinsam umgebrochen. Vier Schnittbuttons, deren Größen, Abstände und Funktionen bleiben unverändert; keine neue feste Zeile.
- `src/CutAssistantNext.App/MainWindow.xaml.cs`: Der neue Click-Handler ruft ausschließlich die vorhandene `OpenFrameLoupeAsync()`-Routine auf. Tooltip: **Frame-Lupe öffnen (Shift+F)**. Das Tastenkürzel bleibt unverändert. Ohne gültige Schnittkante erscheint derselbe bestehende Hinweis. Die Erweiterung auf freie Videopositionen gehört zu Priorität 3 und ist hier nicht umgesetzt.

## Nachweise

C#-Formatierung und Diffprüfung erfolgreich. Paketwiederherstellung mit `RestorePackagesWithLockFile=false` und separatem `NuGetLockFilePath`, ohne die vorhandenen Lockdateien zu beschreiben. Vollständiger Release-Rebuild in separatem Ausgabeordner mit explizitem `CanBuildNumber=29`: **0 Warnungen, 0 Fehler**. Das reguläre `tools/build.ps1` wurde wegen seiner automatischen Zählererhöhung nicht verwendet.

**1076/1076 CAN-Tests bestanden**, keine übersprungen: Core 127, Cutlists 72, App 726, Media 151. Ergebnisse unter `.build/frame-loupe-button-20261014/test-results`.

**72 unterschiedliche WPF-Layoutfälle** gegen den tatsächlich kompilierten Hauptfenster-Bedienbereich:

- Zeilenbreiten 1200, 900 und 700 WPF-Einheiten; Skalierungen 100, 125, 150 und 200 Prozent.
- Leere Anzeige, `01:24:35.280`, 99 Stunden, 1000 Stunden, `256204778:48:05.477` und Rückkehr zur leeren Anzeige.
- Zeit vollständig sichtbar, unveränderter horizontaler Buttonstand beim Wertwechsel, 10 Einheiten Abstand und unveränderte Positionen der vier Schnittbuttons. Bei 1200 Einheiten alles in einer Zeile, bei kleinen Breiten gemeinsamer vorhandener Umbruch.
- Gerenderte Ansichten bei 100 und 200 Prozent visuell geprüft. Keine WPF-Bindungsfehler.

Vier getrennte modale Prüfungen: echter Button-Click und direkte Zielroutine von Shift+F zeigen ohne Kante denselben Hinweis; bei simuliertem pausiertem Player, Medienanalyse und gültiger Kante öffnen beide den echten Lupendialog. Die Lade-Sperre greift in beiden Pfaden; Öffnungssperre wird freigegeben, Schnittwerte bleiben erhalten. Der Test hat keine globale Tastatureingabe simuliert und keine echte Videoanalyse oder Kantenübernahme erneut durchgeführt. Der Tastaturhandler wurde nicht geändert.

Lokale Hilfsprogrammfehler wurden vor den abschließenden erfolgreichen Prüfläufen korrigiert. Belege und Prüfbilder: `.build/frame-loupe-button-20261014/proof`; Testhelfer: `.build/frame-loupe-button-20261014/qa`. Persönliche Einstellungen wurden nicht gespeichert. Die vier vorbestehenden Produktions-Lockänderungen sind anhand ihrer vorher gesicherten SHA-256-Werte bytegleich erhalten. Version und lokaler Zähler bleiben **0.3.0 / 29 / RC3**. Keine Motoränderung, neue Produktionsabhängigkeit, Git-Sicherung oder Veröffentlichung.

## Persönlich prüfen

Prüffassung starten:

`C:\Users\joerg\Documents\GitHub\cut-assistant-next\.build\frame-loupe-button-20261014\compiled\bin\CutAssistantNext.App\release\CutAssistantNext.App.exe`

1. Aufnahme und Schnittliste laden, Start- oder Endkante in der Tabelle anklicken. Button und Shift+F jeweils öffnen und vergleichen.
2. Ohne gültige Kante den gleichen Hinweis prüfen.
3. Vorgemerkten Anfang setzen/abschließen: vollständige Zeit und unveränderte Buttonposition beobachten.
4. Gewohnte Fenstergröße und Windows-Skalierung prüfen. Bei kleinen Fenstern dürfen Anzeige und Button gemeinsam automatisch umbrechen.

Erst nach persönlicher Rückmeldung und gesonderter Freigabe mit Priorität 2 fortfahren.

## Nachtrag: persönliche Abnahme und 20-Bilder-Tooltips

Der Nutzer bestätigt am 14.10.2026 sehr gute Darstellung und Funktion des Lupenbuttons, den Hinweis ohne angeklickte Schnittkante sowie den Tooltip beim Überfahren mit der Maus. Persönliche Abnahme positiv festgehalten. Eine zusätzliche getrennte Prüfung sämtlicher Fenstergrößen/Skalierungen oder des Tastaturkürzels ist in dieser Rückmeldung nicht ausdrücklich enthalten.

Auf Folgeauftrag zwei Tooltiptexte in `MainWindow.xaml` ergänzt:

- **−10 Bilder:** `−20 Bilder (Strg+Klick)`
- **+10 Bilder:** `+20 Bilder (Strg+Klick)`

Strg-Klick für 20 Bilder und die Beschriftungsumschaltung bei gedrückter Strg-Taste waren bereits implementiert. Nur die Hinweise wurden ergänzt. XAML mit XML-Parser und unveränderte Click-Zuordnungen geprüft, Formatierung und Diffprüfung erfolgreich. Neuer separater vollständiger Release-Rebuild **0 Warnungen/0 Fehler** und **1076/1076 Tests**, keine übersprungen. Keine neuen Unit-Tests für die statischen Texte; persönliche Abnahme der Tooltip-Ergänzung anschließend am 14.10.2026 mit „funktioniert tip top :-)“ bestätigt.

Neue lokale Prüffassung einschließlich Lupenbutton und beider Hinweise:

`C:\Users\joerg\Documents\GitHub\cut-assistant-next\.build\frame-step-tooltips-20261014\compiled\bin\CutAssistantNext.App\release\CutAssistantNext.App.exe`

Vorherige Prüffassung bleibt erhalten. Version **0.3.0**, Buildzähler **29**, RC3, vier lokale Produktions-Lockänderungen, öffentliche Release-Pakete und Motoren unverändert. Testbelege unter `.build/frame-step-tooltips-20261014/test-results`. Keine Commits, Pushes oder Veröffentlichungen. Prioritäten 2–4 weiterhin zurückgestellt.
