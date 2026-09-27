# CAN 0.2.0 Build 6 – lokaler RC1-Prüfbericht

Stand: 27.09.2026. **Lokal erstellt und technisch geprüft; noch nicht veröffentlicht.**

## Zusammengehöriger Release-Satz

Alle fünf Archive/Pakete liegen unter `artifacts/release-build6-RC1/`. `SHA256SUMS.txt` enthält ihre Prüfsummen. `publish/` enthält den entpackten Programmstand; `logs/` und `test-results/` enthalten die Nachweise.

| Datei | Bytes | SHA-256 |
| --- | ---: | --- |
| `CutAssistantNext-0.2.0-Build6-source-RC1.zip` | 2588116 | `53738CF4D18D9DC7A50C86FA08F5ECCC3B09F329A319E780340692AC3C0A078B` |
| `CutAssistantNext-0.2.0-Build6-win-x64-RC1.zip` | 22463703 | `86C2C625ABFF87B6A84801E22743AC56F0174617295909C2C0208546F77D1990` |
| `CutAssistantNext-0.2.0-Build6-win-x64-Setup-RC1.exe` | 19123309 | `13192823CB91859C2F771D78D6323C7823C8772E7176FD3E0FCD47B01DEAEEF2` |
| `CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip` | 405926830 | `01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B` |
| `CAN-libmpv-v0.41.0-relink-2026-09-26.zip` | 47273515 | `00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342` |

## Quellzuordnung und Build

Das CAN-Quellpaket enthält 373 Projektdateien sowie `SOURCE-FILES.sha256` und `SOURCE-SNAPSHOT.json`. Es erfasst die noch nicht eingecheckten Änderungen auf Basis von Git-Commit `5c144f33e699bea8d2c123e44315d3afae6f8f3e`, einschließlich der Lizenztexte, eigenen Assets, Paketierungsdateien und acht NuGet-Sperrdateien. Native DLLs werden separat aus dem dokumentierten Vier-DLL-Build bereitgestellt.

Gebaut wurde aus einer frischen Entpackung genau dieses Quell-ZIPs. Der Paketierungsablauf prüfte die Quellprüfsummen vor und nach dem Build. Im ausgelieferten Programmordner verknüpft `RELEASE-PROVENANCE.json` die Binärpakete mit dem SHA-256-Wert des Quellarchivs und nennt .NET SDK 10.0.303 sowie die Prüfsumme des Inno-Setup-7-Compilers. Die Anleitung steht in [RELEASE-BUILD6.md](RELEASE-BUILD6.md).

Dieser Ergebnisbericht und die anschließende Aktualisierung der Freigabecheckliste wurden nach dem Build geschrieben. Sie sind ergänzende Nachweise und nicht Bestandteil des unveränderten RC1-Quell-ZIPs. Maßgeblich für den Build ist das oben gehashte Quellarchiv, nicht ein späterer Arbeitsbaumstand. Ein bitidentischer Neuaufbau mit anderen Werkzeugversionen wird nicht behauptet.

## Prüfergebnis

- Release-Build: erfolgreich, 0 Warnungen und 0 Fehler.
- Tests: **484 von 484 bestanden**, 0 fehlgeschlagen, 0 übersprungen (Core 40, Cutlists 65, Media 91, App 288).
- Gesperrter NuGet-Restore erfolgreich; die vier aufgelösten Produktionspaketversionen entsprechen dem zuvor verwendeten Stand.
- Portable-ZIP: sämtliche **148 Dateien** per SHA-256 gegen `PUBLISH-FILES.sha256` geprüft.
- Setup: mit Inno Setup 7 erfolgreich aus demselben geprüften Publish-Verzeichnis erstellt.
- Die vier nativen DLLs stimmen mit den festgelegten CAN-libmpv-Prüfsummen überein.
- Frameworkabhängige Ausgabe: benötigt die separat installierte .NET 10 Desktop Runtime für Windows x64. Keine gebündelten Laufzeitdateien aus der Ausschlussliste; keine externen Programme `ffmpeg.exe`, `ffprobe.exe` oder `mp4box.exe`.
- CAN-GPL-Text, aktuelle .NET-Bewertung, NuGet-Hinweise, libmpv-Hinweise und Lizenztexte sind enthalten. Die Anleitung und der gemeinsame Hinweis mit den Downloadlinks für .NET, FFmpeg und GPAC liegen auch direkt im Programmordner.
- Programm-Dateiversion `0.2.0.6`; lokaler Buildzähler bleibt 6. Kein Commit, Push oder Upload.

## Verbleibender Schritt

Die früheren VM-Tests der Vorschau sind in der [Freigabecheckliste](RELEASE-CHECKLIST-BUILD6.md) dokumentiert. Der Projektverantwortliche testete das **RC1-Setup in der VM** mit einer HQ-MP4: Das bereitgestellte Protokoll zeigt drei vollständig geschnittene Segmente, vollständiges Zusammenfügen und abschließendes Schreiben mit „Fertig.“. Bild und Ton der fertigen Datei wurden als einwandfrei bestätigt. Anschließend bestätigte der Projektverantwortliche die erfolgreiche Deinstallation des RC1-Setups. Der separate Test des **RC1-ZIPs** zeigt ebenfalls drei vollständig geschnittene und zusammengefügte Segmente mit „Fertig.“; die erzeugte MP4 wurde abgespielt, Bild und Ton sind okay. Damit sind die praktischen Pakettests für Setup und ZIP abgeschlossen.

Danach den vollständigen Release-Satz einschließlich CAN-Quellen, libmpv-Quellen-/Relink-Paket, Lizenzhinweisen und Prüfsummen dauerhaft gemeinsam bereitstellen und die hochgeladenen Dateien erneut gegen die Prüfsummen vergleichen. Die Lizenzbewertung bleibt an die dokumentierte Paketform und ihre Voraussetzungen gebunden; siehe [DOTNET-LICENSING.md](DOTNET-LICENSING.md).
