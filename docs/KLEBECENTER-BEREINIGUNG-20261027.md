# Klebecenter: Arbeitsordner nach Erfolg bereinigen

Stand 27.10.2026, CAN 1.0.2 Build 41; Motor unverändert otr-can 0.1.6.

## Problem und Änderung

Bei den persönlich erfolgreichen Build-40-VM-Läufen blieb nach dem Zusammenfügen ein .can-join-Ordner mit protocol.txt zurück. Die bisherige Bereinigung behielt das Protokoll absichtlich auch bei Erfolg. Die Deinstallation räumt keine Arbeitsordner im Filmverzeichnis auf.

Build 41 entfernt nach vollständig geprüfter und gespeicherter Ausgabe zuerst die bekannten Arbeitsdateien, danach das geschlossene Erfolgsprotokoll und den leeren Arbeitsordner. Fehler und Abbruch behalten protocol.txt mit Diagnosepfad. Scheitert die Bereinigung einer Arbeitsdatei oder enthält der Ordner fremde Einträge, bleiben Ordner und Diagnose bestehen und eine Meldung nennt den Grund. Es gibt keine rekursive Löschung, keine Suche nach alten Ordnern und keine Änderung an Originalen oder fertigem Film. Das Protokoll bleibt im laufenden Dialog weiterhin verfügbar.

Geändert: Mp4JoinService.cs und Mp4JoinTests.cs. Keine Motor-, Medienverarbeitungs-, Audio- oder Videoänderung und keine neue Abhängigkeit.

## Prüfung

Release-Build 41: 0 Warnungen/Fehler; 1489 Entwicklungstests bestanden (200 Core, 72 Cutlists, 292 Media, 925 App). Vier neue Regressionstests prüfen Erfolg, Fehler, gesperrte Arbeitsdatei und fremde Unterordner. Zusätzlich 1489 Pakettests und 1217 Tests gegen finale Produktbibliotheken bestanden. 21 native Klebecenter-Fälle erfolgreich: Bei Erfolg kein Arbeitsordner, bei Fehler/Abbruch ausschließlich protocol.txt; Originale und bestehende Ziele unverändert. Quellen, Portable und Prüfsummen geprüft. Der Arbeitsbericht ergänzt die abschließenden Paketdateien. Build 40 und seine persönliche Setup-/ZIP-Abnahme bleiben als früherer Stand erhalten. Die neue Bereinigung ist jetzt persönlich im Setup auf dem Arbeitsrechner mit John Wick bestätigt: temporäre Datei weg; Bild, Ton, Schnitt, Drei-Sekunden-Vorschau und Klebecenter positiv. Daraus wird keine neue ZIP-/VM- oder persönliche Fehler-/Abbruchprüfung abgeleitet.

Keine Veröffentlichung ohne Freigabe. [Paketbericht](RELEASE-1.0.2-BUILD41-RESULT.md), [Klebecenter](KLEBECENTER-20261027.md).

Beim ersten Paketversuch wurde ein Restorefehler erkannt: Der vorangegangene lokale Restore hatte die Laufzeitabschnitte zuvor unveränderter Test-Lockdateien entfernt. Ausschließlich diese erzeugten Änderungen wurden auf die bytegenauen bisherigen Dateien zurückgeführt. Der danach gesperrte Restore war erfolgreich; die vier vorhandenen Produktions-Lockänderungen sind bytegleich erhalten. Kein Fehler wurde unterdrückt.
