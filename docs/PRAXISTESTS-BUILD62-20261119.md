# Cut Assistant Next 1.0.3 Build 62 – persönliche Teilabnahme

**Datum:** 19.11.2026. **Umgebung:** Windows-11-Arbeitsrechner. **Quelle:** ausdrückliche Rückmeldung des Anwenders zu der ersten Build-62-Prüffassung. Die Ergebnisse bleiben nach den Ergänzungen dokumentiert und werden nicht als persönliche Prüfung der aktualisierten Pakete ausgegeben.

## Persönlich bestätigte Ergebnisse

| Prüfung | Ergebnis und Grenze |
| --- | --- |
| Power-User 1 | Neue Anordnung und Ausrichtung, Schnittliste und Frame-Lupe persönlich einwandfrei. Standard, Power-User 2 und vollständige Rot-/Grün-Funktion noch offen. |
| Tastenkürzel-Dialog | Vier Funktionen sichtbar; A, E, Umschalt+A und Umschalt+E individuell eingerichtet. Die damalige Standardspalte war unbelegt. Noch keine vollständige persönliche Funktions-/Wiederholungs-/Neustartprüfung. |
| TEMP-Auswahl | Alle drei Optionen vorhanden. Windows-TEMP ausgewählt, gespeichert und nach erneutem Öffnen erhalten. |
| MP4Box / Windows-TEMP | Wetterfee sucht Weihnachtswunder (2024), fünf Behalten-Segmente verarbeitet und zusammengefügt, sichere Zielablage und technische Übergangsprüfung erfolgreich. **Bild, Ton und Schnittübergänge persönlich einwandfrei.** Arbeitsordner laut Protokoll bereinigt. |
| EAC3 / MP4Box-Ausweichschnitt | Technischer Ablauf einschließlich zwei Arbeitsordnern und deren protokollierter Bereinigung bestanden; persönliche Sicht-/Hör-/Synchronitätsprüfung der gesonderten Fallback-Ausgabe noch nicht ausdrücklich bestätigt. |
| Protokoll | Konkrete EAC3-Ursache, technische Details, Messwerte vor/nach Fehler, Arbeitsordner und Bereinigung nachvollziehbar; vollständiges Protokoll kopierbar. Vollständige persönliche UI-Prüfung von Scrollbarkeit und Fehlerhervorhebung noch offen. |
| Setup-Upgrade | Upgrade auf dem Windows-11-Arbeitsrechner persönlich bestanden. VM-Installation/-Deinstallation noch offen. |

## MP4Box-Praxistest: Wetterfee sucht Weihnachtswunder (2024)

Windows-TEMP wurde tatsächlich verwendet, MP4Box erhielt den Arbeitsordner ausdrücklich über **-tmp**. Die Speicherplatzprüfung lief erfolgreich; fünf Segmente wurden zusammengefügt und korrekt im Zielverzeichnis abgelegt. Der Anwender bestätigt Bild, Ton und Schnittübergänge als einwandfrei. Die Bereinigung des Auftragsordners ist laut übergebenem Protokoll erfolgt; eine davon unabhängige Dateisystemprüfung wird nicht behauptet.

| Messung | Protokollierter Wert |
| --- | ---: |
| Frei vor Schnittbeginn | 164,75 GiB |
| Geschätzter Arbeitsbedarf | 7,46 GiB |
| Geschätzter Zielbedarf | 3,67 GiB |
| Arbeits- und Ziellaufwerk | Identischer Datenträger |

## EAC3 und bestätigter MP4Box-Neustart

FFMS2-Indexierung und Bildzeitenprüfung waren erfolgreich. otr-can verarbeitete **126.689 Videopakete** und lehnte anschließend die nicht unterstützte EAC3-Tonspur in der strengen Audioverarbeitung ab. Der erste Arbeitsordner wurde laut Protokoll bereinigt. CAN bot den bestätigungspflichtigen Wechsel an; der Anwender bestätigte ihn ausdrücklich.

MP4Box begann einen vollständigen neuen Schnitt mit denselben fünf Behalten-Bereichen, fügte alle Segmente erfolgreich zusammen und übernahm die fertige Ausgabe sicher ins Zielverzeichnis. Der zweite Arbeitsordner wurde laut Protokoll ebenfalls bereinigt. Die erneute Speicherplatzprüfung meldete **163,87 GiB frei**, ohne absehbaren Platzmangel.

**Technischer Ausweichablauf bestanden.** Eine EAC3-Reparatur durch otr-can wird nicht behauptet. Die persönliche Bild-/Ton-/Synchronitätsabnahme dieser gesonderten Ausgabe bleibt offen.

## Nachprüfung der aktualisierten Prüffassung

Die ergänzend beauftragten Werkseinstellungen und die TEMP-Speicheranzeige sind eigene Änderungen und müssen noch persönlich nachgeprüft werden. Offen bleiben insbesondere:

- Standardspalte, vier tatsächliche Tastenfunktionen, Gedrückthalten, Neustart, alle Ansichten und beide Schnittmodi.
- Anzeige von tatsächlichem Arbeitsordner, Datenträgerkapazität, freiem Platz und Ordnergröße einschließlich Aktualisieren und fehlender Zugriffsrechte.
- Benutzerdefinierter Arbeitsordner und tatsächlicher Transfer zwischen verschiedenen Laufwerken.
- Kontrollierte Abbrüche und Schutz vorhandener Zieldateien, soweit noch nicht persönlich geprüft.
- Bild, Ton und Synchronität der Fallback-Ausgabe.
- Aktualisiertes Setup/Portable sowie VM-Installation/-Deinstallation.

**Gesamtabnahme offen. Build 61 bleibt die stabile GitHub-Ausgabe.** Diese Teilabnahme erteilt keine Veröffentlichungs-, Merge-, Installations- oder Release-Freigabe für Build 62.
