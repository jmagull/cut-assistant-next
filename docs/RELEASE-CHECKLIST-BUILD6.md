# Freigabecheckliste: CAN 0.2.0 Build 6 und CAN-libmpv 0.41.0
## Aktueller Stand: RC1 lokal bereit (27.09.2026)

Die unten genannten Vorbereitungsarbeiten 1 und 2 sind für RC1 abgeschlossen: Der eindeutige CAN-Quellstand wurde archiviert, daraus wurden ZIP und Setup gebaut und geprüft. 484/484 Tests bestanden; 148 ZIP-Dateien wurden gegen ihre Prüfsummen abgeglichen. Der vollständige lokale Release-Satz liegt unter `artifacts/release-build6-RC1/`. Details, Dateigrößen und Hashes stehen im [RC1-Prüfbericht](RELEASE-BUILD6-RC1-RESULT.md).

Dieser Abschnitt entstand nach dem Build; das RC1-Quellarchiv bleibt unverändert. Die folgenden Vorschau- und VM-Ergebnisse dokumentieren den bisherigen Verlauf. Das neue RC1-Setup bestand in der VM einen dreiteiligen HQ-Schnitt; Bild und Ton der fertigen Datei wurden als einwandfrei bestätigt. Die RC1-Setup-Deinstallation lief erfolgreich. Auch der separate RC1-ZIP-Test schnitt und verband drei Segmente vollständig; Bild und Ton der fertigen MP4 wurden geprüft und sind okay. Die praktische RC1-Paketabnahme ist damit abgeschlossen. Noch ausstehend ist die gemeinsame öffentliche Bereitstellung mit Quellen, Relink-Paket und Prüfsummen. Es wurde nichts veröffentlicht.

**Arbeitsstand, noch keine Veröffentlichung.** Die Tests vom 27.09.2026 umfassen die frühere selbstenthaltene Ausgabe sowie Setup und ZIP der frameworkabhängigen Vorschau. Für die öffentliche Ausgabe ist eine frameworkabhängige Variante mit separat installierter .NET-10-Desktop-Laufzeit gewählt.

## Bereits überprüft

- CANs eigener Lizenzweg ist GPL-3.0-or-later; der vollständige Text steht in `LICENSE`.
- Der Projektverantwortliche bestätigte am 27.09.2026 die eigene Erstellung von `assets/can.ico` und die Neuimplementierung des CAN-Codes ohne Übernahme aus dem alten Pascal-Cut-Assistant oder anderen Projekten. Die Git-Historie weist einen Autor aus; dies ergänzt, ersetzt aber nicht diese Rechteauskunft.
- Die .NET-/WPF-Einbindung ist für die unveränderte, separat installierte Desktop Runtime begründet bewertet; siehe [DOTNET-LICENSING.md](DOTNET-LICENSING.md). Auf dieser Grundlage wird die Release-Vorbereitung fortgesetzt. Dies ist eine dokumentierte Auslegung, keine verbindliche Rechtsberatung oder Garantie.
- Vier libmpv-DLLs, Herkunft, Build, Lizenzen und Relink-Probe sind in `docs/libmpv/v0.41.0/` dokumentiert.
- Das Quellen-Prüfpaket `CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip` ist lokal vorhanden: SHA-256 `01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B`.
- Das Relink-Paket `CAN-libmpv-v0.41.0-relink-2026-09-26.zip` ist lokal vorhanden: SHA-256 `00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342`.
- Die selbstenthaltene interne Build-6-Ausgabe bestand HD- und VM-Praxistests einschließlich Installation, dreiteiligem Schnitt und Deinstallation; siehe `docs/libmpv/v0.41.0/INTEGRATION-TEST.md`.
- Die frameworkabhängige Vorschau wurde lokal gebaut. Ihr Publish enthält die vier festgelegten DLLs, CANs GPL-Text und Drittanbieterhinweise, aber weder `coreclr.dll` noch `wpfgfx_cor3.dll`, `D3DCompiler_47_cor3.dll` oder `createdump.exe`. Der Laufzeitkonfigurationsdatei zufolge benötigt sie Microsoft.NETCore.App und Microsoft.WindowsDesktop.App 10.0.0 oder kompatible Updates.

## Geprüfte laufzeitabhängige Portable-Vorschau

Die lokale Datei `artifacts/CutAssistantNext-0.2.0-Build6-CAN-libmpv-v041-framework-PREVIEW.zip` ist ein **interner Kandidat** mit 23.058.509 Bytes und SHA-256 `A39768C59DF9DB81C8F5E0BCF118D6081D8EFA8C3EC680542C5239941EFECA73`. Sie enthält 145 Dateien, darunter die vier geprüften DLLs, `LICENSE`, libmpv-Hinweise und die NuGet-/Laufzeithinweise. Die separat installierte [.NET 10 Desktop Runtime für Windows x64](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) ist Voraussetzung; Microsofts Downloadseite nennt aktuell 10.0.12. Die einfache .NET Runtime ohne Desktop-Komponenten genügt für WPF nicht.

Beim Setup-Test wurde der Start zunächst ohne installierte Desktop Runtime geprüft. Nach deren Installation wurden Setup und ZIP praktisch erprobt; Schnitt und Wiedergabe waren erfolgreich. Die Deinstallation wurde für das Setup bestätigt. Das Entfernen des entpackten ZIP-Testordners ist noch nicht bestätigt; es bleibt eine lokale Aufräumaufgabe und ist kein eigenständiger Lizenz- oder Veröffentlichungsblocker.

## VM-Zwischenstand der laufzeitabhängigen Ausgabe (27.09.2026)

Der erste Start ohne installierte .NET-Desktop-Laufzeit zeigte wie erwartet den .NET-Startdialog mit fehlendem `Microsoft.NETCore.App` 10.0.0. Dessen automatischer Downloadlink benennt nur diese Basislaufzeit; der kombinierte Installationshinweis verweist deshalb ausdrücklich auf die .NET 10 **Desktop Runtime** für Windows x64.

Der Projektverantwortliche bestätigte anschließend einen neuen Schnittlauf mit dem laufzeitabhängigen CAN-Paket: Drei Behaltebereiche (`1044.44:1698.04`, `2364.56:3398.36`, `4034.64:4746.96`) wurden vollständig geschnitten und zusammengefügt; MP4Box beendete das Schreiben mit „Fertig.“. Die temporären Dateinamen unterscheiden sich vom früheren VM-Test. Der Projektverantwortliche prüfte die fertige MP4 im Player und bestätigte ein einwandfreies Ergebnis. Anschließend bestätigte der Projektverantwortliche, dass sich die Setup-Version gut deinstallieren ließ. Der separate ZIP-Test startete nach der Installation der .NET 10 Desktop Runtime ohne erneute Laufzeitabfrage. Sein neues Schnittprotokoll zeigt alle drei fertig geschnittenen Segmente, das vollständige Zusammenfügen und das abschließende Schreiben der MP4 mit „Fertig.“. Der Projektverantwortliche prüfte die fertige ZIP-MP4 im Player und bestätigte Bild und Ton als einwandfrei. Auch die Wiedergabe im CAN-Player wurde erfolgreich geprüft.

## Separates laufzeitabhängiges Test-Setup

`installer/CutAssistantNext-CAN-libmpv-v041-framework-PREVIEW.iss` zeigt nach der Installation einen gemeinsamen Hinweis mit Downloadlinks für .NET 10 Desktop Runtime, FFmpeg und GPAC und packt denselben geprüften Vorschau-Publish. Der Inno-Compiler beendete den Build erfolgreich. Die lokale Datei `artifacts/installer-output/CutAssistantNext-0.2.0-Build6-CAN-libmpv-v041-framework-PREVIEW.exe` umfasst 19.150.505 Bytes; SHA-256 `B64427A9A50F88A5A89C9D0B272466249DA557BB2215526F4C9FAAFD732F15BF`. Nach dem VM-Test bestätigte der Projektverantwortliche die erfolgreiche Deinstallation der Setup-Version. Der separate ZIP-Schnittlauf und die Wiedergabeprüfung sind erfolgreich abgeschlossen. Das Setup bleibt ein interner Kandidat.

## Vor öffentlicher Ausgabe zu erledigen

1. Den endgültigen CAN-Quellstand einschließlich der neuen Lizenzhinweise passend zur Binärausgabe eindeutig festhalten. Der Arbeitsbaum enthält noch nicht eingecheckte Änderungen. Ein Quellpaket muss auch die zum Bauen erforderlichen Projektdateien, Skripte und eigenen Assets enthalten.
2. Aus diesem Quellstand endgültige frameworkabhängige Installer-/Portable-Pakete erstellen, Dateiinhalt und Lizenzhinweise prüfen und Hashes dokumentieren. Die besonders lizenzierten .NET-/WPF-Laufzeitdateien dürfen für die hier bewertete Paketform nicht hineingeraten. Die bisherigen internen Testpakete enthalten ältere Dokumentationsstände und werden nicht stillschweigend als endgültige Pakete ausgegeben.
3. Quellen-Prüfpaket und Relink-Paket zusammen mit exakt den vier DLLs, CAN-Quellstand, Hinweisen und Binärpaketen dauerhaft öffentlich erreichbar machen. Prüfsummen nach dem Hochladen erneut vergleichen.

Quellen-Prüfpaket und Relink-Paket wurden am 27.09.2026 erneut gegen die oben dokumentierten SHA-256-Werte geprüft. Das Relink-Paket enthält 215 `.obj`-Dateien und eine Ressourcen-Objektdatei `.o`, zusammen 216 Objektdateien, sowie 63 Archive. Die Quellen-/Relink-Archive bleiben unverändert; die aktuellen Freigabehinweise werden daneben und in den endgültigen CAN-Paketen bereitgestellt.

Bis die Veröffentlichungsarbeiten erledigt sind, bleiben die vorhandenen Binärpakete interne Kandidaten.
