# Klebecenter – klarerer Tonspurhinweis, 29.10.2026

**Entwicklungsbuild 45, CAN 1.0.2.45.** Nutzer wünscht eine Erklärung unmittelbar am Haken. Beschriftung jetzt **„Dieses Tonspurpaar übernehmen (nur bei übereinstimmendem Tonformat)“**, mit Textumbruch. Zusätzlich **„Jede Tonspur darf nur einmal zugeordnet werden. Nicht benötigte Paare abwählen.“** Nur JoinDialog.xaml geändert; Paarvorschläge, Bestätigungspflicht, Doppelbelegungsschutz und beide Medienverfahren unverändert. otr-can bleibt 0.1.6.

Vorheriger Build-44-Dialog nativ mit AAC/AC-3 in Teil 1 und nur AAC in Teil 2 nachgestellt: automatisch nur AAC aktiviert, AC-3 nicht ausgewählt. Mit beiden Spuren in beiden Teilen werden beide Paare korrekt vorgeschlagen. Die im Nutzerscreenshot doppelte aktivierte AAC-Zuordnung wird bewusst abgelehnt; nach Abwahl des zweiten Paars und erneuter Bestätigung ist QUICK AND COPY freigegeben. Nutzer bestätigt anschließend diese Auswahl. Kein belegter Fehler der automatischen Zuordnung in diesen Fällen; die Beschriftung wurde verständlicher gemacht.

## Prüfung

Release-Rebuild 0 Warnungen/Fehler, **1552 Tests bestanden**, keine übersprungen. Vier native WPF-Fälle: John Wick nur AAC und Furiosa AAC+AC-3, jeweils bei 680 und 900 DIP Breite. Tatsächlich erzeugte Checkboxtexte und Textumbruch geprüft; gültige Paare nach Bestätigung freigegeben. Absichtlich doppelte AAC-Zuordnung bleibt gesperrt; nach Abwahl erneute Bestätigung erforderlich. Prüfbild visuell kontrolliert. Alle übrigen Produkt-/Testdateien, ursprüngliche Locks und bisherige Pakete unverändert.

Nachweise unter **.build/quick-pair-hint-20261029/**; Nachstellung des unveränderten Build 44 unter **.build/quick-pair-dialog-20261029/**. Keine neuen Setup-/ZIP-Dateien, kein Commit/Push oder Release. Spätere Rückmeldung am 29.10.2026: Screenshot mit AAC aktiviert und AC-3 ohne Gegenstück abgewählt; Nutzer bestätigt „ich glaube das passt so“. Neuer Hinweis damit persönlich bestätigt. Anschließend Stand einfrieren und Setup/ZIP/Quellen als Build 45 ausdrücklich beauftragt; frühere Build-44-Pakete bleiben erhalten.

## Laufende Setup-Prüfrunde Build 44

Auf dem Arbeitsrechner acht HD-Schnittfälle persönlich positiv: Ring, The Game, S.W.A.T. S08E15 und Fire Country S03E15 jeweils mit MP4Box und otr-can. Bei Ring Fremdsprachen sprachlich ungeprüft; alle vier AAC-Spuren technisch verifiziert, deutscher Ton persönlich positiv. Die Klebecenter-Prüfung läuft: Auswahl für John Wick AAC und Furiosa AAC+AC-3 erläutert. Anschließend John Wick mit QUICK AND COPY technisch abgeschlossen und vom Nutzer auf Ton und Übergang positiv bestätigt; damit neun bestätigte Setup-Funktionsfälle. Noch kein daraus abgeleiteter Furiosa-Erfolg oder Gesamtabschluss der Setup-/Portable-Prüfung.

## John Wick – QUICK AND COPY persönlich bestätigt

29.10.2026: Originale **John Wick Kapitel 4 Teil 1.mp4** und **John Wick Kapitel 4 Teil 2.mp4**, jeweils AAC-Spur 1 / Deutsch / Stereo / 48000 Hz. Nur die gemeinsame AAC-Spur übernommen. **446682 unveränderte Tonpakete**, Referenz **457399680**, Ausgabe **457401216 Samples je Kanal**, erklärte Abweichung **+1536 Samples / 32 ms** durch nicht angewendete innere Randtrimmung. AAC-DTS an der Naht von 338740736 auf 338741249 Sampleticks korrigiert, vorherige DTS 338741248. Vollständige Ausgabe-Tondekodierung und technische Nahtprüfung mit drei Sekunden Vor-/Nachlauf abgeschlossen. Gesamtzeit **00:01:20.34**. Nutzer ausdrücklich **„ton und Übergang check“**; keine zusätzliche persönliche Bild-/Anfang-/Ende- oder einzelne Drei-Sekunden-Abnahme unterstellt.

Beide Anhänge (a3f4aace-732c-41a4-b1d1-1824e751539e und ac075fb5-29fb-478f-b328-da45c9431614) sind bytegleich, SHA-256 **26b2b582ce2c97e3551ee0ff9f7b48ba946bb4b5542eae7eb5c0e74a92d5b866**, daher ein Lauf. Kein Nachweis eines zweiten Films. Nur Dokumentationsnachtrag; Code, Zähler 45 und Build-44-Pakete unverändert.

## John Wick – AUDIO-ENTHUSIAST persönlich bestätigt

29.10.2026, weitere Rückmeldung aus der Build-44-Setup-Prüfrunde vor dem angekündigten Wechsel auf Entwicklungsbuild 45: **John Wick Kapitel 4 Teil 1.mp4** und **John Wick Kapitel 4 Teil 2.mp4**, je AAC-Spur 1 / Deutsch / Stereo / 48000 Hz. Bild unverändert, Ton samplegenau verbunden und einmal durchgehend als AAC kodiert. Technische Prüfung abgeschlossen: **457399680 Tonsamples**, Klebestelle **01:57:37.120**, Gesamtdauer **02:38:49.160**. Gesamtzeit AUDIO-ENTHUSIAST **00:05:30.21**. Nutzer bestätigt ausdrücklich **Bild, Ton und Übergang**.

Anhänge 8b31ac7b-8272-444b-8ca0-0934f19b5d28 und 43612c4f-4b08-4177-9a0f-0cd9d402f9b3 bytegleich, SHA-256 **b7316603ff5aed509680e6cff20ab98850824adb802ef5f0f317885e1e9b18ad**; ein Lauf. Mit acht HD-Schnittfällen und John Wick in beiden Klebeverfahren sind **zehn Setup-Funktionsfälle auf dem Arbeitsrechner positiv**. Keine zusätzliche Furiosa-, Portable-/VM- oder Build-45-Abnahme unterstellt. Build 45 ist lokal unter src/CutAssistantNext.App/bin/Release/net10.0-windows/CutAssistantNext.App.exe gebaut; bisherige Setup-/ZIP-Pakete bleiben Build 44.
