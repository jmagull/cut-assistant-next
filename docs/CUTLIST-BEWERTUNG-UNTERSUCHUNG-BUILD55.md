# Cutlist-Bewertung – Untersuchung für Build 55

**Historischer Untersuchungsstand vor dem Folgeauftrag:** HTTP 200 wurde danach ausdrücklich freigegeben. Die Bewertung ist inzwischen implementiert; [aktueller Entwicklungsbericht](CUTLIST-BEWERTUNG-BUILD55.md). Die nachstehende frühere Zurückstellung gilt nicht mehr.

Stand: 10.11.2026. Grundlage: bestehende lokale Entwicklungsfassung **Cut Assistant Next 1.0.3 Build 55**, getrennte Arbeitskopie .build/worktrees/community-feedback.

## Ergebnis und Grenze der Umsetzung

**Phase A abgeschlossen; Phase B gemäß der ausdrücklichen Bedingung des Auftrags zurückgestellt.** Keine Bewertungsübermittlung, kein Bewertungsdialog und keine Wechsel-/Beenden-Abfrage eingebaut. Der gegenwärtige Antwortvertrag ist nicht ausreichend bestätigt, um eine Serverannahme eindeutig zu erkennen. Das bedeutet nicht, dass der alte Client nicht mehr funktioniert: Die persönliche Funktionsbestätigung des Anwenders bleibt bestehen.

Die aktuelle öffentliche Serveroberfläche bestätigt einen Bewertungsaufruf, verwendet aber einen anderen Weg als der historische Client und wertet dabei nur HTTP 200 aus. Der Auftrag schließt HTTP-Erfolg als alleinigen Annahmenachweis ausdrücklich aus. Einen Antwortinhalt zu erfinden oder die alte Sonderregel „fehlendes error bedeutet Erfolg“ zu übernehmen wäre daher nicht zulässig.

Keine echten Bewertungen, keine Aufrufe von rate.php, set-rating oder set-rating-ignore, keine Uploads, Registrierung oder Profiländerungen. Nur öffentliche Quellen und die lesende Serverinformation wurden abgerufen; keine persönliche Serveradresse oder Benutzerkennung für diese Untersuchung benutzt.

## Historische Referenz

Untersucht wurde [abc874/ca2018](https://github.com/abc874/ca2018/tree/405294ef30981afac1caacd4b96201973b82c4ff), Stand 405294ef30981afac1caacd4b96201973b82c4ff. Kein Pascal-Code wurde in CAN übernommen.

| Quelle | Bestätigter historischer Ablauf |
|---|---|
| [Main.pas](https://github.com/abc874/ca2018/blob/405294ef30981afac1caacd4b96201973b82c4ff/Main.pas), SendRating, ab 3284 | GET zu konfigurierter Basis plus rate.php; rate=Server-ID, rating=0..5, userid=Settings.UserID, protocol=1, Anwendungs-/Versionsparameter. RatingSent erst nach interpretierter Erfolgsantwort. |
| Main.pas, CheckResponseProto1, ab 551 | error=0 als Erfolg; beim Bewerten historisch auch fehlendes error als Erfolg. error=1 bereits bewertet, error=2 fehlendes Argument, error=-1 Datenbankfehler. Die aktuelle serverseitige Gleichsetzung von error=1 mit eigener Cutlist ist damit nicht bewiesen. |
| Main.pas, AskForUserRating, ab 3530; CloseCutlist, ab 3783 | Frage vor Schließen der Cutlist; Ja öffnet Bewertung, Nein setzt fort, Abbrechen hält den Zustand. Bestehende Rückfrage zum Verwerfen folgt weiterhin. Eigene Autoren werden vorab ausgeschlossen. |
| [UCutlist.pas](https://github.com/abc874/ca2018/blob/405294ef30981afac1caacd4b96201973b82c4ff/UCutlist.pas), UserShouldSendRating, ab 1267 | IDOnServer und RatingSent steuern das Angebot; keine Bewertung ohne Server-ID oder nach bereits versandter Bewertung. |
| [CutlistRate_dialog.pas](https://github.com/abc874/ca2018/blob/405294ef30981afac1caacd4b96201973b82c4ff/CutlistRate_dialog.pas) und [.dfm](https://github.com/abc874/ca2018/blob/405294ef30981afac1caacd4b96201973b82c4ff/CutlistRate_dialog.dfm) | Qualitätsstufen 0..5 und ausdrückliche Auswahlbestätigung. |
| [Utils.pas](https://github.com/abc874/ca2018/blob/405294ef30981afac1caacd4b96201973b82c4ff/Utils.pas), GetVersionRequestParams, ab 326 | Historische Identität CutAssistant plus damalige Version. CAN darf diese Identität nicht vortäuschen. |
| [Settings_dialog.pas](https://github.com/abc874/ca2018/blob/405294ef30981afac1caacd4b96201973b82c4ff/Settings_dialog.pas), ab 985 und 1112 | UserID ist ein eigenes INI-Feld; bei leerem Wert wird eine zufällige ID erzeugt und gespeichert. **Diese Kennung ist nicht automatisch der FRED der persönlichen Serveradresse.** |

## Aktuelle, nicht verändernde Prüfung

Öffentliche Originalquellen wurden am 10.11.2026 direkt geladen und als Daten gelesen, nicht ausgeführt:

- [Server-FAQ](https://sniplist.mepaso.net/views/faq.html): persönliche Adresse mit 64-stelligem FRED; Hinweise zum alten Cut Assistant und zur abweichenden Antwortverarbeitung. Sie nennt ausdrücklich eine gespeicherte Bewertung trotz Fehlermeldung des alten Clients. Kein eindeutiger aktueller Bewertungs-Antwortkörper dokumentiert.
- [Aktueller Webclient](https://sniplist.mepaso.net/vendor.js): restService.setCutlistRating sendet POST an /api/set-rating mit JSON-Feldern id und rating. Die persönliche Kennung kommt aus dem URL-Pfad und wird als X-Authorization: Bearer <FRED> mitgegeben. Der gemeinsame HTTP-Helfer löst bei Status 200 mit dem Antwortinhalt auf; der Bewertungshandler ignoriert diesen Inhalt. **Damit sind kein bestimmtes Bestätigungsfeld und keine fachliche Fehlerstruktur nachgewiesen.**
- [Lesende Serverinformation](https://sniplist.mepaso.net/api/get-info): antwortete mit name=cutlist, version=1.0.54. Diese Serverversion ist keine CAN-Version und bestätigt keine Bewertungsantwort.
- [Einstieg](https://sniplist.mepaso.net/view/intro.html) und [Links](https://sniplist.mepaso.net/view/links.html) ergänzen die Herkunft. Keine dort gefundene aktuelle Schnittstellenbeschreibung schließt die nachstehenden Lücken.
- HEAD auf http://cutlist.at/ ergab 200 ohne Location-Header. Daraus wird keine automatische Gleichsetzung von Serveridentitäten oder Übertragbarkeit der API abgeleitet.

Lokale Rohbelege liegen ausschließlich ignoriert unter .build/rating-server-*; Größen und SHA-256 stehen in .build/rating-server-evidence.json. SHA-256 des untersuchten Webclients: 81875a45e9049193c479a00885b89ac5d3275e0f26449cd50211f9ba091a2dd1. Fremder JavaScript-Code wird nicht Teil der Anwendung oder ihrer Auslieferung.

### Genau verbleibende Unsicherheiten

1. **Legacy-Aufruf:** Welche Kombination aus persönlichem URL-Pfad, eigenständiger userid und protocol=1 wird aktuell bei rate.php ausgewertet? CAN speichert derzeit einen FRED, keine historische zufällige UserID. Ein Ersetzen des einen durch das andere ist unbelegt.
2. **Annahmebestätigung:** Welcher konkrete Inhalt bestätigt eine Bewertung? Welche aktuellen Antworten bedeuten bereits bewertet, eigene Cutlist, fehlende Parameter oder Datenbankfehler? Historische Tabellen sind dafür keine aktuelle Garantie.
3. **Moderner Aufruf:** POST, JSON und FRED-Header sind im heutigen Webclient belegt. Dessen alleinige HTTP-200-Prüfung erfüllt die Annahmebedingung dieses Auftrags nicht. Erfolgs-/Fehlerkörper und verlässliche Zuordnung zum bewerteten Datensatz fehlen.
4. **Identität und Serverbindung:** Akzeptanz einer ehrlichen CAN-Kennung, genauer Bewertungsstatus pro Benutzer und etwaige Unterschiede der beiden erlaubten Serveradressen sind nicht nachgewiesen. Öffentliche Quellen liefern außerdem keine belastbare aktuelle Eigentümer-ID für einen sicheren Ausschluss eigener Cutlists bei abweichendem lokalem Autorennamen.

Nötiger nächster Nachweis ist eine aktuelle Betreiberbeschreibung oder serverseitiger Quelltext mit Erfolgs-/Fehlerbeispielen und Kennungsregeln. Alternativ kann eine vom Anwender ohnehin bewusst abgegebene Bewertung im alten Client mit anonymisiertem Request/Response belegt werden; FRED, userid und persönliche URL dürfen nicht in Bericht oder Log geraten. Ein einzelner erfolgreicher Mitschnitt belegt noch nicht alle Fehlercodes. Eine echte Testbewertung wurde weder ausgeführt noch als automatische Voraussetzung vorgesehen.

## Bestandsanalyse CAN und spätere Ansatzpunkte

| Vorhandene Komponente | Beobachtung / notwendiger Anschluss nach Protokollklärung |
|---|---|
| Services/CutlistServerClient.cs | Injizierbarer HttpClient; getxml.php-Suche, getfile.php-Download, vorhandener Upload. Kein Bewertungsaufruf. Suche liefert ID, Autor und aggregierte Bewertungen; das ist kein Bewertungsstatus dieses Benutzers. CAN-Identität beim vorhandenen Upload bereits eigenständig. |
| Services/CutlistSearchResult.cs; CutlistResultRow.cs | Servertreffer tragen ID/Autor und ursprüngliche ServerUrl in der Ergebniszeile. Lokale Zeilen separat. Keine Zuordnung nur nach Dateiname. |
| Services/CutlistSelectionReader.cs | Lädt Serverdatei oder prüft lokale Datei erneut; Ergebnis enthält nur Document und LocalFilePath. ID/Serveradresse werden nicht bis zum aktiven Schnittzustand mitgeführt. |
| MainWindow.CutlistSearch.cs | Mediengeneration schützt vor verspäteten Ergebnissen. Download allein ist kein Verwendungsnachweis; erst erfolgreiche Übernahme darf Herkunft aktivieren. |
| MainWindow.xaml.cs, LoadCutlistDocument | Prüft Dateigröße und Endfragment, erstellt Plan, fragt Ersetzen und lädt ihn. Rückgabe derzeit void; später tatsächlichen Übernahmeerfolg eindeutig rückmelden. Ablehnung/Fehler dürfen keinen Bewertungskandidaten erzeugen. |
| MainWindow.MediaInput.cs; SelectMediaFileAsync; LoadMediaFileCoreAsync | Drag-and-drop/Startargument und Dateidialog haben getrennte Eingänge. Bestehende Ersetzen-Rückfragen vor Reset erhalten; Rating-Entscheidung vor Verwerfen. |
| MainWindow.xaml.cs, MainWindow_Closing | Setzt unmittelbar shutdown/isClosed und beginnt Ressourcenfreigabe. Eine spätere abbrechbare Entscheidung muss davor liegen; Cancel darf Player und CancellationTokenSource nicht schließen. Gleichzeitige Übergänge und mehrfaches Schließen sperren. |
| Settings/CutlistSettingsStore.cs | DefaultAuthor ist lokale Autorenangabe, keine serverseitige Eigentümer-ID. Selbsteinschätzung bei Erstellung bleibt von Fremdbewertung getrennt. |
| CutlistServerSettingsStore/Validator | Persönliche Adresse gespeichert/validiert; keine Bewertungshistorie. Späterer Duplikatschutz darf Persistenzfehler nicht still übergehen oder Geheimnisse protokollieren. |

**In diesem Nachtrag wurde keine dieser Komponenten verändert oder um eine unbestätigte Schnittstelle erweitert.**

Nach Klärung muss der aktive Ursprung mindestens ursprünglichen Server/Account, Cutlist-ID, Autor, eindeutige Bezeichnung, Medienkontext und Verwendungsstatus enthalten. Erst nach erfolgreichem Laden setzen, nicht beim Anzeigen/Download. Manuelle Korrekturen erhalten den ursprünglichen Kandidaten. Lokale Dateien bleiben ohne nachgewiesene Herkunft lokal. Eigene/unklare Herkunft, ungültige IDs, nicht verwendete und bereits bewertet bekannte Listen bleiben ausgeschlossen.

Erst an einem bestätigten Wechsel beziehungsweise beim Beenden soll Bewerten / Überspringen / Abbrechen erscheinen. Abbrechen erhält die Arbeit; Überspringen setzt nur den begonnenen Vorgang fort. Der spätere Dialog braucht Autor, Original-ID und Bezeichnung, sechs Stufen 0..5 ohne vorausgewählte Note sowie ausdrückliches Senden. Erfolgreicher Filmschnitt ist keine Voraussetzung. Bestehende Ersetzen-Rückfragen werden weder ersetzt noch als erfolgte Speicherung ausgegeben.

Ein zukünftiger Übertragungszustand muss ursprünglichen Server und ID sowie Accountwechsel berücksichtigen. Bestätigt erfolgreich, serverseitig bereits bewertet und unklar nach möglicher Übermittlung sind zu unterscheiden. Bei Timeout/Abbruch nach Versand keine automatische Wiederholung; ein persistierter unklarer Zustand verhindert versehentliches erneutes Senden auch nach Neustart. Fehler verändern weder Schnittplan noch Ausgabe. Weiterleitungen, Antworttexte und Exception-Meldungen dürfen FRED oder userid nicht offenlegen.

## Bestandssicherung und geänderte Dateien

Vor den Dokumentationsänderungen wurde der bisherige Stand unter .build/20261110-before-cutlist-rating/ gesichert: Patch, Liste und Kopien der 23 geänderten/neuen Dateien, zusätzlich Spotlight-Quellen/Tests sowie SHA-256-Manifest von 498 Quell-/Test-/Werkzeugdateien und vorhandener EXE/Motordatei. Die getrennte Arbeitskopie und ihre bisherigen Änderungen bleiben erhalten. Keine Änderungen an der ursprünglichen Hauptarbeitskopie.

Nur diese Dateien dieses Nachtrags ändern sich:

- docs/CUTLIST-BEWERTUNG-UNTERSUCHUNG-BUILD55.md: Untersuchung, Nachweise, offene Fragen, Ansatz und Prüfergebnis.
- README.md: Verweis auf diesen Bericht mit eindeutigem Status „noch nicht integriert“.

Keine Änderung an Produktionscode, Tests, Solution, Abhängigkeiten, Spotlight, Navigation oder Schnittmotoren. Keine Versions-/Buildnummernerhöhung. Kein Commit, Push, Merge, Setup, Installation oder Release.

## Prüfung

Release-Build mit unveränderter CanBuildNumber=55 erfolgreich: **0 Warnungen, 0 Fehler**. Abhängigkeiten mit gesperrten Versionsständen wiederhergestellt.

- **1942 CAN-Tests bestanden:** 200 Core, 80 Cutlists, 392 Media, 1270 App; keine Fehler oder übersprungenen Tests. Nachweise: .build/cutlist-rating-regression-tests/*.trx.
- **78 Spotlight-Tests bestanden:** 59 Core/Media und 19 WPF; keine Fehler oder übersprungenen Tests. Nachweise: .build/cutlist-rating-spotlight-tests/*.trx.
- SHA-256-Vergleich aller 498 Baseline-Dateien: **keine Abweichung**, einschließlich vorhandener Entwicklungs-EXE und mitgeliefertem otr-can.exe. Keine Quellcodeformatierung erforderlich; nur Dokumentation geändert.
- Native Medien-/Motor-Praxistests wurden bei diesem reinen Dokumentationsnachtrag nicht erneut ausgeführt. Die früheren Ergebnisse werden nicht als neue Prüfung ausgegeben.

Verwendete Befehle: dotnet restore CutAssistantNext.sln --runtime win-x64 --locked-mode; dotnet build CutAssistantNext.sln --configuration Release --no-restore -p:CanBuildNumber=55; dotnet test CutAssistantNext.sln --configuration Release --no-build; zusätzlich die beiden bestehenden Spotlight-Testprojekte mit --configuration Release --no-restore. Der nummernsteigernde Buildhelfer wurde zur geforderten Beibehaltung von Build 55 nicht aufgerufen.

Die gewünschten neuen Bewertungsfälle sind **nicht implementiert und nicht als bestanden zu werten**: kein neuer Dialog, kein Angebot bei Wechsel/Beenden, keine Persistenz und kein Bewertungs-HTTP-Parser. Mocktests gegen einen erfundenen Antwortvertrag würden die offene Serverfrage nicht lösen. Nach Klärung sind alle im Auftrag aufgeführten Angebots-, Abbruch-, Persistenz-, Eigentümer-, Netzwerk- und Antwortfälle gezielt zu ergänzen.

Die vorhandene Entwicklungs-EXE bleibt unter:

C:/Users/joerg/Documents/GitHub/cut-assistant-next/.build/worktrees/community-feedback/src/CutAssistantNext.App/bin/Release/net10.0-windows/CutAssistantNext.App.exe

Sie enthält weiterhin die bereits getesteten Build-55-Verbesserungen, **keine Cutlist-Bewertungsfunktion**. Eine persönliche Abnahme oder eine echte erfolgreiche Serverbewertung wird nicht behauptet.
