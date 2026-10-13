# Bedienabnahme und Beispielhinweis – 13.10.2026

## Persönliche Rückmeldung zu Build 27

Der Nutzer bestätigt **FFmpeg-Versioncheck**, **Richtungswechsel-Halbsprung** und **andere Folgennomenklatur** als „check“. Der zuvor gezeigte Versioncheck betrifft den Essentials-Git-Build vom **12.10.2026**, Kennung `2026-10-12-git-615b07170e-essentials_build-www.gyan.dev`; der frühere technische Prüflauf verwendete den Full-Git-Build vom 04.10.2026. Keine neue unabhängige technische Abnahme sämtlicher Essentials-Funktionen daraus abgeleitet.

Bild 1 zeigt „Chicago Fire“, Staffel **13**, Folge **07**, Folgentitel **Unantastbar** und die Ausgabevorschau **Chicago Fire 13x07 Unantastbar [12.10.2026]**. Alternative Folgennomenklatur damit persönlich bestätigt.

## Maskenvorschau – Build 28

Auf Wunsch direkt hinter „Vorschau“ den Hinweis ergänzt:

**Vorschau (Beispiel: Staffel 1, Episode 1 – von Hand eingetragen)**

Einzige Produktionsänderung dieses Schritts: `src/CutAssistantNext.App/Dialogs/NamingSettingsDialog.xaml`. Die vorhandenen ergänzten Beispielwerte bleiben Staffel **01**, Folge **01**; sie stammen weiterhin nicht aus dem Ghostbusters-Dateinamen. Parser, Maske, tatsächliche Eingaben und Ausgabevorschau unverändert. Keine neue Test-/Produktionsabhängigkeit und keine zusätzliche Implementierung.

Der erste nummerierte Release-Build **0.3.0 · Build 28 · RC3** wurde vollständig mit separatem `ArtifactsPath` erstellt, weil CAN noch mit otr-can lief. Nach der ausdrücklichen Rückmeldung **„can geschlossen“** denselben Build 28 mit festgehaltener Nummer im üblichen Ordner neu gebaut, ohne den Zähler erneut zu erhöhen. Beide Release-Builds: **0 Warnungen/0 Fehler**, jeweils **1069/1069 CAN-Tests bestanden**, keine übersprungen. Kompilierten Dialog im üblichen Build 28 bei **1100×760** und **980×700** geprüft: Hinweis vollständig, Footer erreichbar, Vorschau **Ghostbusters S01E01 Hexenprobleme [19.09.2026]** erhalten, keine Bindungsfehler, keine Einstellungen gespeichert. Prüfbild gesichtet; alle acht Lockdateien bytegleich erhalten.

## Chicago Fire / Unantastbar – MP4Box und otr-can

Quelle laut übermitteltem Protokoll: `Chicago_Fire__Unantastbar_S13E7_26.10.12_21-00_sf2_45_TVOON_DE.HD.mp4`. Ein Behalteabschnitt mit MP4Box-Parametern **724.84:3100.06**; Protokoll endet erfolgreich mit **„Fertig.“**. Nutzer meldet für MP4Box einen **Blitzer**. Bild 3 zeigt am Beginn der fertigen MP4Box-Ausgabe die gelb/weiße SRF-zwei-Senderblende.

Nach dem angekündigten otr-can-Lauf zeigt das neue Bild die Ausgabe **Chicago Fire S13E07 Unantastbar otr-can [12.10.2026].mp4** am Beginn mit NBC-Intro. Der Nutzer bewertet sie mit **„Alles prima :-)“** persönlich positiv. Der gleichzeitig erneut gepostete Protokolltext ist weiterhin das MP4Box-Protokoll, kein neuer otr-can-Prozessnachweis. Keine Motor-/Video-/Samplezahlen oder gesonderte Ton-/Synchronitätsprüfung daraus abgeleitet. Die Bilder illustrieren den gemeldeten Unterschied am Anfang; kein identischer Originalframe- oder Hash-/Decodervergleich behauptet.

Ein anschließend übermitteltes Bild der eigenen Server-Cutlist ergänzt ausdrücklich: **„Werbeblitzer bei 00:00.00 mit MP4Box, auch nach Korrektur mit der Frame-Lupe. Mit otr-can 0.1.5 blitzerfrei. Bild, Ton und alle Schnittübergänge geprüft.“** Damit sind auch Bild, Ton und sämtliche Schnittübergänge persönlich geprüft bestätigt; die frühere eingeschränkte Rückmeldung wird durch diesen späteren Nachweis ergänzt. Keine gesonderte Prüfung jeder einzelnen Tonspur oder unabhängige Decoder-/Hashprüfung daraus abgeleitet. Die sichtbare Cutlist ist vom Nutzer bereitgestellt; CAN hat im Rahmen dieser Dokumentation keine Serveraktion ausgeführt. Bild lokal unter `user-proof/cutlist-chicago-fire-acceptance.png` gesichert.

Der MP4Box-Blitzer bleibt ein dokumentierter Bildbefund bei dieser Aufnahme. Erfolgreicher Prozessabschluss bestätigt allein nicht die gewünschte Schnittbildqualität. Keine Ursachenreparatur oder Änderung an einem Motor für diesen Hinweistextauftrag vorgenommen; kein Beleg für einen Zusammenhang mit dem Lupen-Halbsprung oder der Versionsprüfung. Der separate künstliche MP4Box-Zwei-AAC-Befund bleibt erhalten.

Lokale Belege im ignorierten `.build/naming-preview-hint-20261013`: Build-/Testprotokolle, Dialogprüfbilder, `UI-PROOF.json`, Lockdateihashes und Nutzerbilder unter `user-proof`. Öffentliche Build-16-RC3-Pakete unverändert; kein Setup, Portable-Paket, Release, Commit oder Push.
