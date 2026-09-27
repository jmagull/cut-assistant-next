# .NET- und NuGet-Lizenzen der Windows-Testausgabe

**Stand 27.09.2026: begründete Projektbewertung für die frameworkabhängige Windows-x64-Ausgabe unter GPL-3.0-or-later.** Die Bewertung dient der Release-Vorbereitung; sie ist keine verbindliche Rechtsberatung oder Garantie und keine pauschale Freigabe anderer Paketformen. Die fremden Laufzeitdateien bleiben unter ihren eigenen Bedingungen.

## Windows-Laufzeit

Microsofts [Lizenzübersicht](https://github.com/dotnet/core/blob/main/license-information-windows.md) ordnet einige Windows-Binärdateien der [.NET Library License](https://dotnet.microsoft.com/en-us/dotnet_library_license.htm), eine Datei der [Windows-SDK-Lizenz](https://learn.microsoft.com/en-us/legal/windows-sdk/license) und die übrigen Dateien MIT zu. Im getesteten selbstenthaltenen CAN-Publish liegen:

| Datei | Zuordnung laut Microsoft |
| --- | --- |
| `coreclr.dll` | .NET Library License |
| `Microsoft.DiaSymReader.Native.amd64.dll` | .NET Library License |
| `vcruntime140_cor3.dll` | .NET Library License |
| `wpfgfx_cor3.dll` | .NET Library License |
| `D3DCompiler_47_cor3.dll` | Windows-SDK-Lizenz |

Die Lizenzbedingungen dieser Dateien sind nicht gleichbedeutend mit MIT. Insbesondere enthalten sie eigene Verteilungsbedingungen. Die selbstenthaltene Paketform wird mit diesem Dokument nicht abschließend bewertet. Für die öffentliche Ausgabe hat der Projektverantwortliche die frameworkabhängige Paketform mit separat installierter .NET Desktop Runtime gewählt. Die gebaute und geprüfte Vorschau ist die Grundlage der folgenden Bewertung.

Die für den Build verwendeten Runtime-Pakete `Microsoft.NETCore.App.Runtime.win-x64` und `Microsoft.WindowsDesktop.App.Runtime.win-x64` hatten Version `10.0.11`. Ihre lokal mitgelieferten Texte sind unter [`licenses/dotnet`](licenses/dotnet/) unverändert abgelegt. Der `System.ComponentModel.Annotations`-Pakettext liegt dort ebenfalls. Die Microsoft-Lizenzübersicht ist für die Windows-Sonderfälle maßgeblicher als die allgemeine MIT-Datei eines Runtime-Pakets.

## Anwendungspakete

Die Paketmetadaten des App-Restore (`src/CutAssistantNext.App/obj/project.assets.json`) nennen für diese in der Anwendung verwendeten NuGet-Pakete MIT:

| Paket | Version | Copyright laut Paketmetadaten | Lizenznachweis |
| --- | --- | --- | --- |
| `HanumanInstitute.LibMpv` | 0.10.1 | © 2023–2026 Etienne Charland | [Upstream](https://github.com/mysteryx93/LibMpv-OpenGL/blob/main/LICENSE) |
| `HanumanInstitute.Validators` | 2.1.2 | © 2020–2023 Etienne Charland | [Upstream](https://github.com/mysteryx93/HanumanInstitute.Validators) |
| `JetBrains.Annotations` | 2025.2.4 | © 2016–2025 JetBrains s.r.o. | [Upstream](https://github.com/JetBrains/JetBrains.Annotations/blob/master/license.md) |
| `System.ComponentModel.Annotations` | 5.0.0 | © Microsoft Corporation | [Paketlizenz](licenses/dotnet/System.ComponentModel.Annotations-5.0.0-LICENSE.TXT) |

Im laufzeitabhängigen Vorschau-Publish liegen die DLLs der ersten drei Pakete. Ihre Copyright-Vermerke und der MIT-Text sind in [MIT-NOTICES.txt](licenses/nuget/MIT-NOTICES.txt) zusammengefasst und werden in die Ausgabe kopiert. `System.ComponentModel.Annotations.dll` liegt dort nicht; sein Paket erscheint nur im Restore-Graphen. Vor einer öffentlichen Ausgabe müssen die tatsächlich enthaltenen Versionen erneut geprüft werden. Test-only-Pakete gehören nicht zum App-Publish.

## Geprüfter Paketinhalt

Die frameworkabhängige Vorschau enthält 145 Dateien. Die vier dokumentierten libmpv-Dateien stimmen bytegenau mit den SHA-256-Werten in [libmpv/README.md](libmpv/v0.41.0/README.md) überein. `coreclr.dll`, `Microsoft.DiaSymReader.Native.amd64.dll`, `PresentationNative_cor3.dll`, `vcruntime140_cor3.dll`, `wpfgfx_cor3.dll`, `D3DCompiler_47_cor3.dll` und `createdump.exe` sind nicht enthalten. Die Laufzeitkonfiguration verweist auf die gemeinsam installierten Frameworks `Microsoft.NETCore.App` und `Microsoft.WindowsDesktop.App`, jeweils Version `10.0.0` mit kompatiblen Updates. Auf der VM wurden die Setup-Installation und -Deinstallation sowie Schnitt und Wiedergabe mit Setup und ZIP erfolgreich berichtet; siehe [Freigabecheckliste](RELEASE-CHECKLIST-BUILD6.md).

## Begründete Bewertung und Projektentscheidung

Nach der hier dokumentierten Auslegung ist CAN unter GPL-3.0-or-later mit unveränderter, separat installierter .NET 10 Desktop Runtime einschließlich WPF lizenzrechtlich vertretbar. Der Projektverantwortliche hat am 27.09.2026 zugestimmt, auf dieser Grundlage weiterzuarbeiten. Die .NET-Frage wird für diese Paketform als begründet bewertet geführt, nicht als ungeklärter pauschaler Freigabeblocker.

Die Begründung:

1. [GPLv3 §1](https://www.gnu.org/licenses/gpl-3.0.en.html#section1) definiert Systembibliotheken anhand allgemeiner Kriterien zum üblichen Lieferumfang und zur Funktion. Zu den maßgeblichen Hauptkomponenten zählen auch Compiler und Objektcode-Interpreter. Die [GNU-Erläuterung zur GPLv3](https://www.gnu.org/licenses/quick-guide-gplv3.en.html) bezieht ausdrücklich Standardbibliotheken gängiger Programmiersprachen ein, die separat vom Betriebssystem installiert werden können. Einzelne Produkte müssen dafür nicht namentlich in einer FAQ aufgeführt sein.
2. Microsoft dokumentiert WPF als regulären Bestandteil der [.NET Desktop Runtime](https://learn.microsoft.com/en-us/dotnet/core/install/windows). Eine [WPF-Quellimplementierung unter MIT](https://github.com/dotnet/wpf/blob/main/LICENSE.TXT) ist öffentlich verfügbar. Zusammen mit der unveränderten Nutzung der gemeinsamen Frameworks stützt dies die hier vorgenommene Einordnung der benötigten Laufzeitbibliotheken als Systembibliotheken. Diese Einordnung ist eine Anwendung der Kriterien auf CAN, keine ausdrücklich für CAN erteilte GNU- oder Microsoft-Freigabe. Die MIT-Quelllizenz ersetzt dabei nicht die abweichenden Windows-Binärlizenzen.
3. Die besonders lizenzierten Windows-Laufzeitdateien werden im geprüften CAN-Paket nicht weiterverteilt. Nutzer beziehen die Desktop Runtime separat von Microsoft unter dessen Bedingungen. Das entspricht der technischen Trennung von [frameworkabhängiger und selbstenthaltener Ausgabe](https://learn.microsoft.com/en-us/dotnet/core/deploying/). Die Trennung allein wäre noch kein Beweis der GPL-Verträglichkeit; maßgeblich ist die begründete Einordnung nach §1.

Diese Bewertung setzt voraus, dass CAN die unveränderte gemeinsame Desktop Runtime nutzt, die besonders lizenzierten Laufzeitdateien weiterhin nicht bündelt und die Lizenzhinweise der tatsächlich mitgelieferten Bestandteile erhält. Der mitgelieferte .NET-Startcode wird dadurch nicht zu eigenem CAN-Code; die vorhandenen MIT-Hinweise bleiben erforderlich. Eine andere Paketform oder geänderte Laufzeitbestandteile erfordern eine neue Bewertung. Es wird keine zusätzliche Ausnahme zur GPL für CAN erfunden oder auf fremden Code übertragen.

Eine unabhängige fachkundige Prüfung kann zusätzliche Sicherheit geben; sie ist in dieser Projektentscheidung keine zwingende zusätzliche Freigabestufe. Für die öffentliche Ausgabe bleiben der passende CAN-Quellstand, die libmpv-Quellen-/Relink-Pakete, sämtliche Hinweise sowie abschließende Paket- und Prüfsummenkontrollen erforderlich. Die bisher getesteten internen ZIP-/Setup-Dateien enthalten ältere Fassungen dieser Dokumentation und werden nicht stillschweigend als endgültige Release-Pakete ausgegeben.
