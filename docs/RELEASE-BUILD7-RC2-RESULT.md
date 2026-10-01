# CAN 0.2.1 Build 7 – RC2-Prüfbericht

Stand: 01.10.2026.

## Quellstand

Gebaut aus Commit `dcf4123bd663aa7a61cb22b35764716471e7624b`, aus einem frisch entpackten Quellarchiv ohne Arbeitsbaumänderungen. Das Quellarchiv enthält SOURCE-SNAPSHOT.json und SOURCE-FILES.sha256. RELEASE-PROVENANCE.json in den Binärpaketen verweist auf dieses Archiv und dessen Prüfsumme.

## Änderungen

- Uploads übernehmen Autor, Programmname und Version aus der gespeicherten Cutlist.
- Die HTTP-Anfrage verwendet `app=CutAssistantNext` und die numerische Version der laufenden CAN-Anwendung.
- Überarbeitete Cutlist-Einstellungen mit URL-Hilfe erst nach fehlgeschlagenem Verbindungstest und einem höheren, weiter oben platzierten Fenster.
- Versionsanzeige 0.2.1 · Build 7 · RC2.

## Automatische Prüfung

- Windows-x64-Restore mit gesperrten Paketversionen erfolgreich.
- Release-Build: **0 Warnungen, 0 Fehler**.
- **496/496 Tests bestanden**: Core 40, Cutlists 65, Media 91, App 300.
- Frameworkabhängiges Publish, Portable-ZIP und Inno-Setup erfolgreich erstellt.
- Vier native libmpv-DLLs und erforderliche Lizenzhinweise geprüft; keine .NET-Laufzeit und keine separaten Schnittwerkzeuge im Paket enthalten.
- Alle **148 Dateien** des frisch entpackten Portable-ZIPs gegen PUBLISH-FILES.sha256 geprüft.
- libmpv-Quellen-Prüfpaket und Relink-Paket gegenüber der bisherigen Veröffentlichung unverändert und erneut anhand ihrer SHA-256-Werte geprüft.
- Portable-Start erfolgreich: Fenstertitel „Cut Assistant Next · 0.2.1 · Build 7 · RC2“. Reguläres Beenden mit Exitcode 0.

## Praktische Nachweise und Grenzen

Die Upload-Korrektur wurde vor der Versionsanhebung mit echten cutlist.at-Uploads bestätigt (IDs 2079433, 2079437 und 2079440). Die letzte dieser Prüfungen verwendete HTTP-Version 0.2.0. Der Benutzer bestätigte anschließend die Löschung aller Test-Cutlists. Die neue Fensterdarstellung wurde am lokalen Entwicklungsstand bestätigt.

Nach der Veröffentlichung bestätigte der Benutzer die Installation in einer VM einschließlich GPAC, .NET 10 und FFmpeg. Mit „Grey’s Anatomy – Durchs Feuer“ (HQ-MP4) wurden drei Behaltebereiche geschnitten und zusammengefügt. Das bereitgestellte Protokoll zeigt alle drei abgeschlossenen Segment-Schnitte, vollständiges Zusammenfügen, Dateischreiben bis 100 % und „Fertig.“. Anschließend bestätigte der Benutzer Bild und Ton der fertigen Datei als einwandfrei.

Die zugehörige Cutlist enthält `Application=Cut Assistant Next`, `Version=0.2.1`, `Author=Joerg` und drei Bereiche. Die MP4Box-Aufrufe `592.56:1759.12`, `2314.2:3005.44` und `3565.52:4133.64` stimmen mit diesen Bereichen und der vorhandenen CAN-Umrechnung (Ende = Start + Dauer − ein Bild bei 25 fps) überein. Dies ist kein zusätzlicher Upload-Nachweis.

Ein gesonderter Server-Praxistest mit HTTP-Version 0.2.1, ein echter Sniplist-Test, die Setup-Deinstallation und ein separater Videoschnitt mit dem Portable-ZIP stehen noch aus. Die früheren Pakettests von 0.2.0 Build 6 RC2 gelten nicht als erneute Abnahme dieser Ausgabe.

## Pakete und Prüfsummen

Lokaler Paketsatz: `artifacts/release-build7-RC2/`.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |
| CutAssistantNext-0.2.1-Build7-source-RC2.zip | 2598680 | C22E8DC7996A36D7BED6A801CFFC2358BE7D172BF197BEBF413DB8CEFFB6FF1E |
| CutAssistantNext-0.2.1-Build7-win-x64-RC2.zip | 23157375 | 553B51D55DFB35C770C12EBA5AB89E1F55A5AD34ED34D7F049A74C12C7C41B38 |
| CutAssistantNext-0.2.1-Build7-win-x64-Setup-RC2.exe | 19245113 | DE8E478FC62B5A2648F0223EC4A7E1F42C1FD01BBB3135FC7BB8600B87EFAA10 |

Dieser Bericht entstand nach dem Paketbau und ist nicht Bestandteil des unveränderten CAN-Quellarchivs.
