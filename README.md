# Cut Assistant Next

Lokaler Entwicklungsstand vom 12.10.2026: **0.2.1 · Build 16 · RC3**, optionaler Motor **otr-can 0.1.5**. Experimentelle H.264-AVI-Vorbereitung kopiert Video und MP3 nach MP4; der Motor kodiert MP3 nach dem Schnitt **einmal nach AAC-LC**. 793 CAN-/44 Rust-Tests und unabhängige PCM-Prüfungen bestanden. Neuer Teufelsbrigade-Schnitt persönlich abgenommen: Anfang, Ende, alle Schnittübergänge sowie Bild und Ton als einwandfrei bestätigt. Die bereits im vorherigen Ergebnis vorhandene technische Video-PTS-Auffälligkeit bleibt gesondert offen. [Umsetzung und Prüfbericht](docs/AVI-OTR-CAN-MP3-20261012.md). Motorpfad lokal auf 0.1.5 gestellt, vorherige Einstellung gesichert; öffentliche Releasepakete unverändert. Paketbau auf 0.1.5 umgestellt und mit je zehn Fällen unter PowerShell 7/5.1 geprüft; neuer Setup-/Portable-Paketsatz noch nicht erstellt. [Paketbau-Prüfbericht](docs/PAKETBAU-OTR-CAN-015-20261012.md). Feste Quellen für CAN Build 16 und otr-can 0.1.5 sind lokal gesichert und geprüft; [Quellstand](docs/QUELLSTAND-BUILD16-RC3-20261012.md).

Cut Assistant Next (CAN) ist ein Windows-Programm zum Abspielen von Videos, Bearbeiten klassischer Cutlists und Schneiden mit MP4Box. Die Oberfläche verwendet WPF und mpv/libmpv, ohne DirectShow oder installierte Windows-Codec-Pakete vorauszusetzen.

CAN ist weiterhin ein Proof of Concept mit experimenteller Unterstützung weiterer Eingangscontainer. Die [öffentliche Vorabversion 0.2.1 Build 13 – RC3](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.1-build13-rc3) bietet Setup und Portable-ZIP mit dem geprüften libmpv-0.41.0-Stack und dem optionalen Motor otr-can 0.1.4.

Der veröffentlichte Stand **0.2.1 · Build 13 · RC3** ergänzt Frame-Lupe, Schnittkantenkorrektur, Halbierungssuche und den optionalen OTR-CAN-CPU-Motor. MP4Box bleibt die Vorgabe. Direkt unter der Schnittliste wählst du **Schneiden MP4Box (schnell)** oder **Schneiden otr-can (framegenau, langsam)** für diesen Auftrag. CAN steuert Original, Keep-Segmente, Ziel, einmalige Indexierung, Abbruch und Aufräumen. Release-Build ohne Warnungen oder Fehler, **785/785 Tests bestanden**. Der lokale Build-12-RC3-Kandidat mit Setup, Portable-ZIP und optionalem Motor ist automatisch geprüft und persönlich abgenommen: Überinstallation, Windows-11-VM, beide Schnittmotoren, Bild/Ton, Deinstallation und separater ZIP-Test bestätigt; [Prüfbericht](docs/RELEASE-BUILD12-RC3-RESULT.md). Build 13 ergänzt den KI-Hinweis im Credits-Dialog und den Abgleich alter Aufgabenlisten; diese Ergänzung ist noch nicht in den abgenommenen Build-12-Paketen enthalten. Der neue Build-13-RC3-Satz mit Setup, Portable und Quellen ist ebenfalls automatisch geprüft; persönliche Paketbestätigung noch offen. [Neuer Prüfbericht](docs/RELEASE-BUILD13-RC3-RESULT.md). Build 13 RC3 wurde am 11.10.2026 als öffentliche Vorabversion freigegeben; [Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-BUILD13-RC3.md).

Der reguläre optionale Motor **otr-can 0.1.4** kombiniert die geprüfte Frame-/Zeitstempelkorrektur mit **vollständiger Audio-Neukodierung jeder Tonspur**. Codec, Kanallayout, Samplerate und Zielbitrate bleiben erhalten; zusätzliche Kodierverluste und Laufzeit sind bewusst akzeptiert. 40 Rust-Tests bestanden. Der reguläre Pines-Auftrag mit Motor 0.1.3 über CAN benötigte rund **3:33 Minuten**; die vollständige Ausgabe ist bytegleich zum persönlich als synchron bestätigten Testfilm. CAN zeigt eigene Video-/Audio-/Mux-Phasen. Motor 0.1.4 erhält zusätzlich den Audiofilterzustand bei wechselnden AC-3-Begleitdaten; CAN bleibt auch bei großen Warnprotokollen bedienbar. Der lokale Motorpfad ist auf 0.1.4 eingestellt. Details im [Radical-Korrekturbericht](docs/OTR-CAN-RADICAL-KORREKTUR-20261011.md). Ältere EXEs bleiben erhalten. Nachweise und Unterstützungsgrenzen stehen im [0.1.3-Prüfbericht](docs/OTR-CAN-0.1.3-PRUEFBERICHT.md). Weitere reale Formate und getrennte Tonspurkontrollen folgen vor Version 1.0.

## Lizenz

Der vom CAN-Projekt selbst entwickelte Quellcode steht unter **GPL-3.0-or-later**. Der vollständige Lizenztext steht in [LICENSE](LICENSE). Eingebundene Bibliotheken, NuGet-Pakete und die .NET-Laufzeit behalten ihre eigenen Lizenzen; die [libmpv-Hinweise](docs/libmpv/v0.41.0/THIRD-PARTY-NOTICES.md) und die [.NET-/NuGet-Bestandsaufnahme](docs/DOTNET-LICENSING.md) ordnen sie zu. Die öffentliche RC3-Ausgabe ist als Vorabversion gekennzeichnet; die zugehörigen Quellen und Prüfsummen stehen beim Release.

## Danksagung

CAN baut auf Ideen und Arbeit vieler Menschen und Projekte auf. Die [Danksagung](docs/DANKSAGUNG.md) nennt die Beteiligten und würdigt auch die KI-unterstützte Entwicklung.

## Einstieg

- [Nutzeranleitung](docs/NUTZERANLEITUNG.md): Einrichtung, Bedienung, Cutlists, Schneiden und Hilfe bei Fehlern.
- [Projektstatus](docs/STATUS.md): umgesetzter Umfang, bestätigte Tests und offene Aufgaben.
- [Entwicklungstagebuch](docs/TAGEBUCH.md): aktuelle Änderungen, Prüfergebnisse und offene Schritte.
- [Setup-Merkliste](docs/SETUP-MERKLISTE.md): geplantes Gesamtpaket und vorgemerkte V2-Funktionen.
- [Testplan](docs/TESTPLAN.md): reproduzierbare Prüfungen und Testbasis.
- [Build-13-RC3-Prüfbericht](docs/RELEASE-BUILD13-RC3-RESULT.md): gesicherter Quellcommit, neue Setup-/Portable-Pakete, technische Prüfungen und noch offene persönliche Paketbestätigung.
- [RC2-Prüfbericht](docs/RELEASE-BUILD7-RC2-RESULT.md): Quellstand, Paketprüfsummen, 496 automatisierte Tests, Portable-Starttest und verbleibende Praxistests.
- [RC3-Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-BUILD13-RC3.md): Quelltag, sieben öffentliche Downloads und geprüfte GitHub-Digests.
- [Früherer RC2-Veröffentlichungsnachweis](docs/RELEASE-PUBLICATION-BUILD7-RC2.md): Build 7 und dessen persönliche Paketprüfung.
- [Architektur](docs/ARCHITECTURE.md) und [Entscheidungen](docs/DECISIONS.md): technische Hintergründe und Entwicklungsgeschichte.

## Was CAN derzeit kann

- Videos laden, mit ffprobe analysieren und mit mpv wiedergeben.
- Play/Pause, Seeking, Einzelbildschritte und Lautstärkeregelung.
- Zu entfernende Bereiche markieren, auswählen, korrigieren und löschen.
- Eine angeklickte Schnittkante mit Umschalt+F in der Frame-Lupe prüfen, Framedetails und Keyframes anzeigen und den ausgewählten Frame als neue Kante übernehmen.
- Zusätzlich zur Navigation mit ±1/±10 Frames mit halbierten Suchsprüngen navigieren: standardmäßig 2000, 1000, 500 … bis 1 Frame, mit einstellbarem Startwert und Neustart.
- Cutlists lokal laden, erzeugen und speichern.
- Passende Server-Cutlists automatisch suchen und die Suche erneut per Button öffnen.
- Lokal gespeicherte Cutlists nach Bestätigung auf den konfigurierten Server hochladen.
- Fremde Cutlists als Vorlage verwenden, deren Namensvorschlag bewusst beibehalten oder neu erzeugen und die Herkunft bei abweichendem Autor im Kommentar dokumentieren.
- Schnittzeiten einheitlich anzeigen und die Namensvorschau markieren und kopieren.
- Ausgabenamen über Namensmasken und Cutlist-Namensvorschläge vorbereiten.
- MP4-Inhalte direkt mit MP4Box schneiden, auch bei abweichender Dateiendung.
- Andere Container nach Bestätigung experimentell mit FFmpeg verlustfrei nach MP4 umpacken und anschließend mit MP4Box schneiden.
- Optional mit OTR-CAN direkt aus dem Original schneiden, FFMS2 einmal indexieren und alle Tonspuren übernehmen; CPU-Encoding an den Video-Schnittgrenzen und vollständige Audio-Neukodierung.
- Fortschritt und Protokoll anzeigen, kopieren und den Vorgang abbrechen.
- Hauptfenster und die vier Schnittdialoge (Schneiden, Fortschritt/Protokoll, Abschluss, Cutlist erzeugen) merken Größe und Maximierung; die Player-Lautstärke wird ebenfalls gespeichert. Inhalte bleiben bei kleinen Fenstern scrollbar.

Die reine Cutlist-Erstellung benötigt keine Video-Umwandlung. Erst beim Schneiden wird eine gegebenenfalls notwendige MP4-Arbeitsdatei erzeugt. Serversuche und Cutlist-Zuordnung beziehen sich dabei weiterhin auf die Originaldatei.

## Grenzen

MP4 ist ein Container, keine Bezeichnung für einen bestimmten Videocodec. Ob sich ein anderes Format verlustfrei vorbereiten lässt, hängt von seinen Streams ab. Die MP4Box-Vorbereitung verwendet Stream-Copy. Der optionale OTR-CAN-Motor kopiert geeignete IDR-Abschnitte, kodiert Video-Schnittgrenzen und jede vollständige Tonspur auf der CPU neu. Ungeeignete Dateien oder deutliche Abweichungen werden mit einer Fehlermeldung gestoppt.

Die Prüfung der Arbeitsdatei ist eine Plausibilitätsprüfung anhand der Medieninformationen. Sie ersetzt nicht die Kontrolle der Schnittstellen und der Ton-Synchronität. OTR-CANs Video-Smart-Rendering und Audio-Neukodierung sind synthetisch und mit Pines geprüft; Stapelverarbeitung und integrierte mehrteilige Aufnahmen sind noch nicht umgesetzt.

Insbesondere bei historischen AVI-Dateien können bereits im Original Probleme mit Zeitstempeln, Bildaktualisierung oder Ton-Synchronität auftreten. Eine irreführende Dateiendung wie `.avi.mp4` ändert nichts am tatsächlich erkannten Container. CAN zeigt bei tatsächlich erkanntem AVI bereits nach dem Laden einen gelben Hinweis auf mögliche Probleme im Original; eine abweichende Dateiendung wird zusätzlich erklärt. Weitere Praxistests erfolgen mit geeigneten neuen Aufnahmen.

## Für Anwender

Vorgesehene Umgebung ist Windows 11 x64. Die öffentliche RC3-Vorabversion bietet Setup und Portable-ZIP sowie den optionalen Motor otr-can 0.1.4. Die .NET 10 Desktop Runtime muss separat installiert werden; die vier geprüften libmpv-DLLs sind enthalten. ffprobe, FFmpeg und MP4Box werden separat installiert und über die Einstellungen konfiguriert.

Die [Nutzeranleitung](docs/NUTZERANLEITUNG.md) erklärt die Einrichtung ohne Entwicklungswerkzeuge.

## Aus dem Quellcode bauen

Voraussetzungen: Windows 11 x64, .NET-10-SDK und die vier DLLs des [geprüften CAN-libmpv-0.41.0-Kandidaten](docs/libmpv/v0.41.0/README.md) in einem lokalen Ordner. Das Setup-Skript prüft die SHA-256-Werte und kopiert die DLLs in den ignorierten Laufzeitordner. Ein .NET-Restore benötigt die passenden Pakete im lokalen Cache oder Netzwerkzugriff.

Im Repository-Verzeichnis:

```powershell
.\tools\setup-libmpv.ps1 -SourceDirectory 'C:\Pfad\zum\Vier-DLL-Ordner'
dotnet restore .\CutAssistantNext.sln
.\tools\build.ps1 -Configuration Release -NoRestore
dotnet test .\CutAssistantNext.sln --configuration Release --no-build
```

Die Anwendung liegt danach unter `src\CutAssistantNext.App\bin\Release\net10.0-windows\CutAssistantNext.App.exe`. Der Build kopiert die vier vorbereiteten DLLs und die Drittanbieterhinweise in das Ausgabeverzeichnis. Für Analyse, experimentelles Umpacken und Schnitt sind zusätzlich die konfigurierten externen Werkzeuge erforderlich.

## Version und Buildnummer

Aktuelle öffentliche Vorabversion vom **11.10.2026: 0.2.1 · Build 13 · RC3**, Release-Build ohne Warnungen/Fehler und **785/785 Tests bestanden**. Setup, Portable, CAN-/Motorquellen, libmpv-Quellen-/Relink-Pakete und Prüfsummen stehen beim [Release](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.1-build13-rc3) bereit. Alle sieben Uploads stimmen mit den lokalen Größen und SHA-256-Werten überein. Die vollständige persönliche Paketabnahme betrifft Build 12; Build 13 wurde nach den technischen Prüfungen ausdrücklich zur Veröffentlichung freigegeben. Frühere Releases bleiben unverändert verfügbar.

Öffentliche Vorabversion vom 01.10.2026: **0.2.1 · Build 7 · RC2** mit korrigierter Upload-Identität und überarbeiteten Cutlist-Einstellungen. Der Release-Build ist ohne Warnungen oder Fehler abgeschlossen; **496/496 Tests bestanden**. Setup, Portable-ZIP, Quellen und Prüfsummen stehen beim Release bereit. Die frühere Ausgabe **0.2.0 Build 6 – RC2** bleibt unverändert verfügbar.

Die inhaltliche Version steht zentral in `Version.props`: Patch für Korrekturen, Minor für neue Funktionen, Major für einen größeren Versionssprung. Sie wird bewusst gepflegt.

Die RC-Kennung steht ebenfalls in `Version.props` (`CanCandidate`, derzeit `RC3`) und erscheint auch bei normalen lokalen Builds im Fenstertitel und in der Hauptüberschrift. Eine ausdrücklich übergebene Build-Eigenschaft `CanCandidate` hat Vorrang. Die numerische Version für Cutlists und HTTP-Uploads enthält diese Kennung nicht.

`tools/build.ps1` baut die gesamte Lösung neu und erhöht den lokalen Zähler in `.build/build-number.txt` genau einmal bei erfolgreichem Abschluss. Fehlgeschlagene Builds erhöhen ihn nicht. Titel, Hauptüberschrift und Dateieigenschaften verwenden dieselbe Version und Buildnummer. Einzelne Projekt-Builds und direkte IDE-/dotnet-Builds verwenden die zuletzt erfolgreiche Nummer (bei einem neuen Checkout zunächst 0); für eine neue nummerierte Ausgabe das Skript verwenden. Der Zähler ist lokal, wird nicht mit Git synchronisiert und identifiziert keine weltweit eindeutige Veröffentlichung. Gleichzeitige Skript-Builds im selben Checkout werden durch eine Dateisperre verhindert.

## Projektstruktur

| Bereich | Aufgabe |
|---|---|
| `src/CutAssistantNext.App` | WPF, Dialoge, ViewModels, Einstellungen und Ablaufsteuerung |
| `src/CutAssistantNext.Core` | Medienmodelle, Schnittplan, Namensbildung und Verträge |
| `src/CutAssistantNext.Media` | ffprobe, mpv und Prozessausführung für FFmpeg/MP4Box |
| `src/CutAssistantNext.Cutlists` | Cutlist-Import, Export, Metadaten und Bereichsumrechnung |
| `tests` | vier automatisierte Testsuiten |
| `samples` | kleine Referenzdaten |
| `tools` | Einrichtung von libmpv und versionierte Builds |
| `docs` | Anleitung, Status, Testplan und technische Dokumentation |

Historischer RC2-Release-Stand (01.10.2026): **0.2.1 Build 7 – RC2**, Release-Build ohne Warnungen oder Fehler und **496/496 automatisierte Tests bestanden**. Alle 148 Dateien des Portable-ZIPs wurden geprüft; Start, Versionsanzeige und reguläres Beenden waren erfolgreich. Die Installation in einer VM und ein dreiteiliger HQ-MP4-Schnitt von „Grey’s Anatomy – Durchs Feuer“ wurden bestätigt; Bild und Ton der fertigen Datei waren einwandfrei. Die Setup-Deinstallation wurde ebenfalls als erfolgreich bestätigt. Der separate Portable-ZIP-Schnitt mit drei Behaltebereichen wurde ebenfalls erfolgreich abgeschlossen; Bild und Ton der fertigen Datei wurden vom Benutzer als einwandfrei bestätigt. Details stehen im [damaligen Prüfbericht](docs/RELEASE-BUILD7-RC2-RESULT.md); die früheren Pakettests bleiben im [Build-6-Prüfbericht](docs/RELEASE-BUILD6-RC2-RESULT.md) dokumentiert.
