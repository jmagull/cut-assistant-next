# CAN 0.2.0 Build 6 – RC2-Prüfbericht

Stand: 29.09.2026. Lokale Paketprüfung; noch kein Veröffentlichungsnachweis.

## Quellstand und Änderungen

Gebaut aus Commit `178ba0a3c41f17800648636eb10a76961b3d96b4`, ohne Arbeitsbaumänderungen, aus einem frisch entpackten CAN-Quellarchiv.

- Namensmasken-Editor: „Änderungen verwerfen“ stellt den Stand beim Öffnen wieder her; „CAN-Standardmaske“ lädt die ursprüngliche CAN-Maske.
- Dauerhafte Übernahme erfolgt erst mit „Speichern“; „Abbrechen“ erhält den gespeicherten Stand.
- Nutzeranleitung ergänzt.
- RC-Kennung wird beim Paketieren an Build und Publish übergeben und in Fenstertitel, Hauptüberschrift und Setup-Versionsbezeichnung verwendet.

## Automatische Prüfung

- Gesperrter Restore erfolgreich.
- Release-Build: 0 Warnungen, 0 Fehler.
- 484/484 Tests bestanden: Core 40, Cutlists 65, Media 91, App 288.
- Frameworkabhängiges Publish, Portable-ZIP und Setup erfolgreich erstellt.
- Vier native libmpv-DLLs, erforderliche Lizenzhinweise und Quellmanifest durch den Paketierer geprüft.
- Alle fünf Release-Pakete gegen SHA-256 geprüft.

## Praktische Prüfung

Die folgenden Ergebnisse wurden vom Projektverantwortlichen bestätigt:

- RC2-Anzeige in Titel und Hauptüberschrift des lokal gebauten Release-Programms sichtbar.
- Namensmasken-Editor: Bearbeitung, beide Reset-Aktionen, Speichern und Abbrechen einschließlich erneutem Öffnen erfolgreich.
- Setup: Installation über die vorhandene Version erfolgreich.
- Setup: Eigene Maske speichern, erneut öffnen und anschließend auf CAN-Standard zurücksetzen erfolgreich.
- Setup: „Black Adam“ als HD-MP4 mit sechs Keep-Segmenten geschnitten; Bild und Ton einwandfrei.
- Setup: Deinstallation erfolgreich.
- Portable-ZIP: Separater HD-Schnitt von „Black Adam“ erfolgreich. Das bereitgestellte Protokoll zeigt sechs abgeschlossene Segment-Schnitte, vollständiges Zusammenfügen und „Fertig.“. Bild und Ton der Ausgabe wurden als einwandfrei bestätigt.

Die RC-Anzeige und die Reset-Funktionen wurden für das Portable-ZIP nicht nochmals separat bestätigt. Die genannten Praxistests sind keine allgemeine Garantie für sämtliche Eingangsformate.

## Pakete und Prüfsummen

Maßgeblich ist der neue Paketsatz unter `artifacts/release-build6-RC2/`. Der frühere lokale RC2-Satz ohne sichtbare RC-Kennung wurde ersetzt.

| Datei | Bytes | SHA-256 |
|---|---:|---|
| CutAssistantNext-0.2.0-Build6-source-RC2.zip | 2588172 | EFF78C9548F47E11A8F80AA36F0B67E90FB97CF8D736E8BB818F366EA0915D21 |
| CutAssistantNext-0.2.0-Build6-win-x64-RC2.zip | 22468654 | 1D0272DB52C1D5D3887FC93A52DDAB5859B89AFE20B6C952EE83630F369D84E0 |
| CutAssistantNext-0.2.0-Build6-win-x64-Setup-RC2.exe | 19124255 | 77E1C4B30ED939DEDFD77497DEE18996DF3AF5F1DCC1D850BDC56FCF331C506F |
| CAN-libmpv-v0.41.0-pruefpaket-2026-09-26.zip | 405926830 | 01546ADB0BCCB15588456CAA2E90BE805B6A55804618E0A4EECA08B01AB1FE8B |
| CAN-libmpv-v0.41.0-relink-2026-09-26.zip | 47273515 | 00EE11B824D7E28C30D62096C4C1BC5BFD6052FEE393956980FC14152661A342 |

Dieser Bericht entstand nach dem Build und ist nicht Bestandteil des unveränderten CAN-Quell-ZIPs. RELEASE-PROVENANCE.json im Programmordner dokumentiert die Zuordnung zum Quellarchiv.
