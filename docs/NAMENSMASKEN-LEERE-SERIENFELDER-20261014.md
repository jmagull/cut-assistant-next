# Filmnamen ohne Maskenzusätze – 14.10.2026

**Aktueller Stand nach ausdrücklicher Klarstellung und Fehlerbild.** Priorität 2, lokale Prüffassung **CAN 0.3.0 · Build 29 · RC3**. Persönliche Filmnamenanzeige in beiden Schneidedialogen und im Cutlist-Dialog bestätigt; weitere persönliche Tests laufen. Öffentliches Release unverändert.

## Verbindliche Regel

Sind **Staffel, Folge und Folgentitel sämtlich leer**, endet der automatisch erzeugte Vorschlag am Ende des **Name**-Bausteins. **Alles dahinter entfällt**, einschließlich Datum, Sender, Originalname, Trennzeichen und freier Zusätze. Null, leere Strings und Whitespace gelten als leer. Der Name selbst und ausdrücklich davor stehende Maskenpräfixe werden nicht abgeschnitten.

Diese ausdrückliche Klarstellung ersetzt die anfängliche Annahme, Datums-/Zusatzfelder bei Filmen beizubehalten. Bei mindestens einem gefüllten Serienfeld wird die vollständige vorhandene Maske verwendet.

| Fall mit der Bildmaske | Neuer Vorschlag |
|---|---|
| Die Hart 2; drei Serienfelder leer | `Die Hart 2` |
| Kimi; Staffel 01, Folge 03, Folgentitel Der Besuch | `Kimi S01E03- Der Besuch- [12.10.2026]` |
| Alle Serienwerte danach wieder leeren | `Die Hart 2` |

Exakte Maske aus dem gemeldeten Bildfall:

    %Name% %S%%Staffel%%E%%Folge%-%Folgentitel: %- [%Tag%.%Monat%.%YYYY%]

## Gemeinsame Umsetzung und Erhaltung

Einzige geänderte Produktionsdatei dieser Aufgabe: `src/CutAssistantNext.Core/Naming/NameTemplateRenderer.cs`. Die Cutlist-Erstellung und beide Schnittwege verwenden diesen Renderer bereits gemeinsam. Die frühere Trennzeichen-Bereinigung wurde durch die einfache Begrenzung nach `%Name%` ersetzt. Die vollständige Maske wird weiterhin validiert, sodass unbekannte Variablen nicht durch die Filmregel verdeckt werden.

Die Präfixtoken-Verarbeitung liest vollständige Tokens statt an der schließenden Prozentmarke einer anderen Variablen zu starten. Dadurch wird ein normaler Doppelpunkt zwischen Bausteinen nicht als unbekannte Präfixvariable fehlinterpretiert.

Gespeicherte Masken, Datums-/Serienmetadaten, Dateiendungsregeln, Cutlist-Dateinamensregeln, Originaldatei und Schnittwerte bleiben erhalten. Manuelle Ausgabenamen und geschützte übernommene Cutlist-Vorschläge werden weiterhin erst durch bewusstes Neuerzeugen ersetzt. Keine neue Maskensyntax, Produktionsabhängigkeit oder Motoränderung.

## Nachweise

- Der Screenshot-Fall wurde vor der Korrektur exakt reproduziert: Null/Leer/Whitespace erzeugten jeweils `Die Hart 2- [12.10.2026]`; gewünschtes Ergebnis war `Die Hart 2`. Drei fehlschlagende Regressionen und ein erfolgreicher Serienfall sind unter `.build/movie-title-only-20261014/first-regression` erhalten.
- Betroffene C#-Dateien formatiert. Restore ohne Schreiben bestehender Lockdateien, vollständiger separater Release-Rebuild mit festem `CanBuildNumber=29`: **0 Warnungen, 0 Fehler**. Reguläres Zählerskript wegen unveränderter Buildnummer nicht verwendet.
- **1134/1134 Tests bestanden**, keine übersprungen: Core 169, Cutlists 72, App 742, Media 151. Sieben weitere Regressionen nach dem ersten 1127er-Prüfstand; insgesamt 58 zusätzliche Fälle gegenüber dem öffentlichen Stand.
- Exakte Bildmaske, vollständige Serie, Whitespace und literalem Doppelpunkt geprüft. Gespeicherte Bildmaske, alter Vorschlag, ausdrücklich neue Generierung, frische Cutlist-Factory, gespeicherte/wieder gelesene Cutlist sowie Ausgabe beider Schnittdienste mit simulierten Werkzeugaufrufen abgesichert. Filmvorschlag `Die Hart 2`, Dateiname `Die Hart 2.mp4`, gespeicherte Maske bytegleich.
- Bestehende Film-Datum-/Zusatz-Erwartungen wurden anhand der Klarstellung aktualisiert; allgemeine Datum-/Editor-Prüfungen verwenden weiter einen Serienkontext. Metadaten-Datum bleibt ausdrücklich geprüft. Kein Test entfernt. Ein Vorprüfbuild mit doppeltem Staffel-Testargument sowie der erste vollständige Lauf mit neun alten Zusatz-Erwartungen und dem Doppelpunkt-Befund wurden korrigiert; Erstresultate unter `initial-full-tests` erhalten.
- **Echter kompilierter WPF-Cutlist-Dialog**, 720×720 und 600×400: tatsächlicher Neuerzeugen-Button aktualisiert die gebundene Vorschau auf **Die Hart 2**; vollständige Serie behält das Datum, anschließend geleerte Serienfelder ergeben wieder nur den Titel. Keine Bindungsfehler. Prüfbilder visuell gesichtet; ausschließlich isolierte Fenstereinstellungen geschrieben.
- Vier vorbestehende Produktions-Lockänderungen anhand ihrer gesicherten SHA-256-Werte bytegleich, Buildzähler weiterhin 29. Version, öffentliche Pakete/Tags/Prüfsummen, MP4Box und otr-can 0.1.5 nicht geändert. Keine Commits, Pushes oder Veröffentlichungen.

Die neuen Motorprüfungen verwenden simulierte Werkzeuge; keine zusätzliche echte Filmschnitt- oder persönliche Abnahme wird daraus abgeleitet. Vorherige Lupen-/Tooltip-Abnahmen bleiben erhalten.

## Persönliche Teilprüfung – 14.10.2026

Nach bewusstem Neuerzeugen zeigt der vom Nutzer geprüfte Dialog **Schneiden – MP4Box** bei leeren Staffel-/Folge-/Folgentitelfeldern ausschließlich **Die Hart 2** als Ausgabedateinamen. Per Screenshot und Rückmeldung **„nach neu erzeugen check, ich test weiter“** bestätigt. Weitere persönliche Tests laufen, die Gesamtprüfung bleibt offen. Ein bereits ausgeführter Schnitt und die Ausgabe auf dem Datenträger sind durch diesen Nachweis noch nicht bestätigt.

Anschließend bestätigt der Nutzer mit **„otr-can check, teste weiter“** auch den Dialog **Schneiden – otr-can**: leere Serienfelder und ausschließlich **Die Hart 2** als Ausgabedateiname. Damit sind beide Namensdialoge persönlich geprüft; weitere Tests und Gesamtabschluss bleiben offen.

Mit **„cutlist erzeugen check“** und Screenshot bestätigt der Nutzer anschließend auch den Dialog **Cutlist erzeugen**: leere Serienfelder, Vorschau ausschließlich **Die Hart 2**. Die Filmnamenanzeige ist damit in allen drei Namensdialogen persönlich geprüft. Ein bereits erfolgtes Cutlist-Speichern wird aus diesem Bild nicht abgeleitet; weitere persönliche Prüfungen und Gesamtabschluss bleiben offen.

## Persönlich wiederholen

**Diese neue Prüffassung** verwenden:

`C:\Users\joerg\Documents\GitHub\cut-assistant-next\.build\movie-title-only-20261014\compiled\bin\CutAssistantNext.App\release\CutAssistantNext.App.exe`

1. Die vorhandene Maske beibehalten. Beim Film die drei Serienfelder leer lassen und **Aus obigen Eingaben neu erzeugen** anklicken. Die Vorschau muss ausschließlich **Die Hart 2** zeigen.
2. Bei einer Serie die vollständige Maske einschließlich Datum prüfen.
3. Manuell geänderte Ausgabenamen bleiben ohne bewusste Neuerzeugung erhalten.

Prüfbild: `.build/movie-title-only-20261014/ui-proof/results/movie-720.png`; WPF-Nachweise und Bindungslog im selben Verzeichnis. CAN-Testresultate: `.build/movie-title-only-20261014/test-results`. Vorherige Prüffassungen bleiben erhalten; Prioritäten 3–4 erst nach gesonderter Freigabe.
