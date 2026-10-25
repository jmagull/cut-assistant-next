# Projektstatus

## 25.10.2026 – Cutlist-Motorangabe nach otr-can-Schnitt korrigiert

Beim erneuten Speichern einer Peppermint-Cutlist trug CAN trotz otr-can-Schnitt weiterhin MP4Box, dessen Pfad und dessen Version ein. Der Cutlist-Dialog bietet jetzt eine sichtbare Motorwahl. Vorbelegt wird der zuletzt erfolgreich für die aktuelle Aufnahme verwendete Motor, andernfalls der Motor einer geladenen Cutlist; ohne beides bleibt MP4Box die Vorgabe. Für otr-can werden Name und EXE-Dateiname sowie die tatsächlich abgefragte Version geschrieben. Eine fehlgeschlagene Versionsabfrage lässt die Version leer. Die Peppermint-Cutlist wurde lokal berichtigt. Release-Build ohne Warnungen oder Fehler; 1416/1416 Solution-Tests bestanden. Keine Paket- oder Veröffentlichungsänderung. Der Quellfix gehört nicht zum eingefrorenen Build 35.

## 22.10.2026 – CAN 1.0.2 Build 35 paketiert und technisch geprüft

Setup, Portable, passende CAN-/Motor-/libmpv-Quellen und Prüfsummen aus Freeze-Quellcommit **c9e6ebf1368a47211bc0ca22243d6e6138e20959** erstellt. **1411/1411 Pakettests**, zusätzlich **40/40 Diagnosetests**, **893/893 App-Tests gegen finale Paket-DLLs**, elf Motor-Paketprüfungen und drei native künstliche Übergangsprüfungen erfolgreich; Build/Publish ohne Warnungen/Fehler. 250 Portable-Dateien und 623 Quellen-ZIP-Dateien verifiziert, 146 erfasste Bestandsdateien erhalten. Buildzähler nach Prüfung auf 35 übernommen.

Neue persönliche Setup-/VM-/Deinstallations-/Portable-Abnahme steht aus. **Keine Veröffentlichung: frühestens 25.10.2026 nach erneuter ausdrücklicher Freigabe**, keine automatische Veröffentlichung oder neuer Release-Tag. Frühere Systemhänger bleiben ungeklärt. [Paketbericht](RELEASE-1.0.2-BUILD35-RESULT.md), [Prüfübergabe](UEBERGABE-CAN-1.0.2-BUILD35-20261022.txt).

## 22.10.2026 – Freeze und Paketauftrag für 1.0.2 Build 35

Nutzer beauftragt Freeze, Setup/Portable und zugehörige Quellen/Prüfsummen sowie Commit und Push. Neuer Stand **1.0.2 · Build 35**; bestehende Pakete bleiben erhalten. **Veröffentlichung frühestens Sonntag, 25.10.2026, nach zusätzlicher ausdrücklicher Freigabe.** Kein automatischer Termin und kein Release-Tag. Paketprüfungen folgen separat; bekannte Grenzen und offene Freeze-Ursache bleiben dokumentiert. [Freeze](FREEZE-1.0.2-20261022.md), [Release Notes](RELEASE-NOTES-1.0.2.md).

## 22.10.2026 – Zwei weitere Furiosa-Läufe mit otr-can positiv bestätigt

Der Nutzer meldet **zwei weitere erfolgreiche otr-can-Läufe mit Mad Max/Furiosa, „check bei allen parametern“**. Damit sind in der aktuellen Rückmeldeserie drei erfolgreiche Läufe einschließlich Bild, Ton, Schnitt und Drei-Sekunden-Vorlauf gemeldet: ein vollständig protokollierter Lauf und zwei zusätzliche persönliche Bestätigungen ohne weitere Einzelprotokolle. Frühere VM-Erfolge werden separat gehalten; die Ursache der früheren Systemhänger bleibt offen.

Nur Dokumentation ergänzt; kein neuer Medien-/Build-/Testlauf. Buildnummer 34 und Release-Status unverändert. [Wiederholungsbestätigung](OTR-CAN-FURIOSA-PRAXIS-20261022.md#zwei-weitere-positive-nutzerläufe-am-22102026).

## 22.10.2026 – Furiosa auch mit otr-can erfolgreich abgeschlossen und persönlich geprüft

Neues Nutzerprotokoll **Furiosa Teil 1/2, HD** mit **otr-can 0.1.6**: fünf Behalteabschnitte, **171458** geprüfte Videopakete, je **329199360** dekodierte Samples für AAC- und AC3-Stereo bei 48 kHz, finale native MP4-/CAN-Prüfung erfolgreich. Der Nutzer bestätigt **Bild, Ton, Schnitt und Drei-Sekunden-Vorlauf**. Beginn 583,200 s; kein identischer Wiederholungstest des letzten geschützten Freeze-Auftrags ab 579,960 s. Der positive Lauf klärt die früheren Systemhänger nicht auf.

Nur Prüfbericht, Freeze-Nachtrag, Status und Tagebuch ergänzt; kein eigener Medienlauf oder neuer Build/Test. Buildnummer 34 und Release-Status unverändert. [Erfolg, Prüfumfang und Abgrenzung](OTR-CAN-FURIOSA-PRAXIS-20261022.md).

## 22.10.2026 – Fire Country HQ mit MP4Box persönlich erfolgreich geprüft

**Fire Country – Todesfalle, S3E14** (HQ, 3plus, Aufnahme 21.10.2026): Nutzer bestätigt **Schnitt, Bild, Ton und Drei-Sekunden-Sprung**. Protokoll mit drei Segmenten, gleichen Bildzeiten an beiden inneren Übergängen, jeweiliger Decoderanfangsprüfung und **„Fertig.“** Zusätzlicher erfolgreicher HQ-Praxisfall des bestehenden Prüfwegs nach der Erweiterung; keine kleine Überlappung gemeldet.

Nur Dokumentation ergänzt, kein neuer Build/Testlauf. Testnachweise, Buildnummer 34 und Release-Status unverändert. [Praxisfall](MP4BOX-UEBERGANGSINTEGRATION-20261022.md#weiterer-praxisfall-fire-country--todesfalle-s3e14).

## 22.10.2026 – S.W.A.T. auch mit otr-can persönlich erfolgreich geprüft

**S.W.A.T. – Der Schatz der Santa Clara** mit **otr-can 0.1.6**: Nutzer bestätigt **Bild, Ton, Drei-Sekunden-Vorlauf und Schnitt**. Das Protokoll zeigt einmalige FFMS2-Indexierung, 172492 geprüfte Quellbildzeiten, drei Behalteabschnitte, **59632** geprüfte Videopakete und **114493440** dekodierte Samples der Stereo-AAC-Spur bei 48 kHz. Finale native MP4-Prüfung und CAN-Abschluss erfolgreich. Zusätzlicher positiver Realfall neben der zuvor bestätigten MP4Box-Ausgabe; die früheren Systemhänger bleiben ungeklärt.

Nur Prüfbericht, Status und Tagebuch ergänzt; kein neuer Build/Testlauf, keine Versions-/Releaseänderung. Vorhandener CAN-Testnachweis unverändert. [Protokollauswertung und persönlicher Prüfumfang](OTR-CAN-SWAT-PRAXIS-20261022.md).

## 22.10.2026 – Weiterer MP4Box-Praxisfall S.W.A.T. persönlich erfolgreich

**S.W.A.T. – Der Schatz der Santa Clara**, HD-Aufnahme vom 21.10.2026: Nutzer bestätigt **„Schnitt, Ton, 3 Sekunden check“**. Protokoll mit drei Segmenten, gleicher Bildzeit am zweiten Übergang, Prüfung aller Abschnitte und **„Fertig.“** Damit ist zusätzlich der bestehende Prüfweg für gleiche Zeitstempel nach der Erweiterung persönlich positiv erprobt. Kein neuer Überlappungsfall; keine gesonderte Bildabnahme aus der Rückmeldung abgeleitet.

Nur Dokumentation ergänzt, kein neuer Build/Testlauf. Nachweis weiterhin **1411/1411 Solution-Tests, 40/40 isolierte Tests, vier von vier native Furiosa-Prüfungen**. Build 34 und Release-Status unverändert. [Praxisfall und Umfang](MP4BOX-UEBERGANGSINTEGRATION-20261022.md#weiterer-praxisfall-swat--der-schatz-der-santa-clara).

## 22.10.2026 – Integrierte Furiosa-Prüfung persönlich bestätigt

Der Nutzer bestätigt nach eigenem Schnitt im regulären CAN **„Bild, Ton, Schnitt, 3 Sekunden check“**. Das beigefügte Furiosa-MP4Box-Protokoll zeigt fünf Segmente, die neue Zuordnung der kleinen Überlappung am ersten Übergang, die Prüfung aller Abschnitte und **„Fertig.“** Damit ist zusätzlich zur technischen Integrationsprüfung auch die persönliche Schnitt-/Bild-/Ton-/Vorlaufkontrolle positiv bestätigt.

Nur Prüfbericht, Status und Tagebuch ergänzt; kein neuer Build/Testlauf. Nachweis weiterhin **1411/1411 Solution-Tests, 40/40 isolierte Tests, vier von vier native Prüfungen**. Der bekannte Bildankerbefund und die ungeklärte Systemfreeze-Ursache bleiben erhalten. Build 34 und Release-Status unverändert. [Persönliche Prüfung](MP4BOX-UEBERGANGSINTEGRATION-20261022.md#persönliche-prüfung-im-regulären-can-am-22102026).

## 22.10.2026 – Begrenzte MP4Box-Zuordnung produktiv integriert und mit neuem Furiosa-Schnitt geprüft

Nach persönlicher Schnitt-/Bild-/Ton-/Vorlaufbestätigung und ausdrücklichem Folgeauftrag die kleine H.264-Überlappung unter strengen Paket-/DTS-/Decoderbedingungen im bestehenden Ausgabenanalysator zugeordnet. Der reguläre MP4Box-Service liefert dafür `Located` mit Dateiidentität und verständlichem Überlappungshinweis. Abschluss und Kontrollplayer verwenden diese tatsächlichen Ausgabezeiten; unklare Fälle behalten den bisherigen berechneten Rückfall.

**1411/1411 Solution-Tests, 40/40 isolierte Tests**, Release ohne Warnungen/Fehler. Neuer echter Furiosa-Schnitt über den produktiven App-Service: vier korrekte Prüfpositionen und **vier von vier nativen Kontrollen bestanden**, einschließlich Drei-Sekunden-Auftrag, fortgesetzter Wiedergabe und passendem ersten neuen Originalbild. Letzte alte Referenz am ersten Übergang weiterhin unbeobachtet. Acht vorhandene Lockdateien und Buildzähler erhalten; Version/Build 1.0.1/34 unverändert, keine Pakete oder Veröffentlichung. Die Systemfreeze-Ursache bleibt offen. [Änderung, Nachweis und Grenzen](MP4BOX-UEBERGANGSINTEGRATION-20261022.md).

## 22.10.2026 – Furiosa-Prototyp: Schnitt, Bild, Ton und Drei-Sekunden-Vorlauf persönlich bestätigt

Nach dem separaten libmpv-Prüflauf bestätigt der Nutzer ausdrücklich **„Schnitt, Bild, Ton und 3 Sekunden check“**. Damit liegt zusätzlich zu den vier technischen Bildankerprüfungen eine positive persönliche Bewertung des Schnitts, Bildes, Tons und Vorlaufs vor. Der Befund zur letzten alten Referenz am ersten Übergang bleibt separat erhalten; keine vollständige Bildzählung oder Einzeltonspur-/Synchronitätsmessung daraus abgeleitet.

Nur Prüfbericht, Status und Tagebuch ergänzt; kein neuer Build/Testlauf. Nachweis unverändert **40/40 Prototyptests, 1385/1385 bestehende Tests**, Release ohne Warnungen/Fehler. Keine Produktintegration, neue Version oder Releasefreigabe. Build 34 bleibt Draft; Systemfreeze-Ursache offen. [Persönliche Prüfung und Grenzen](MP4BOX-LIBMPV-KONTROLLE-20261022.md#persönliche-prüfung-am-22102026).

## 22.10.2026 – Vier Furiosa-Kandidaten im separaten libmpv-Prüflauf technisch bestätigt

Neuer isolierter Prüfplayer mit unveränderten aktuellen CAN-Playerquellen: **vier reale Übergangsprüfungen bestanden**, tatsächliche Seek-Aufträge jeweils Kandidat minus drei Sekunden, Wiedergabe über die Grenze und pixelgleiche neue Originalbilder im Schrittfenster. Beim ersten Übergang bleibt die letzte alte Bildreferenz im Schrittfenster unbeobachtet; daraus keine vollständige Bildverlust- oder Wiedergabeaussage abgeleitet. Kandidaten bleiben als unbestätigte produktive Prüfstellen gekennzeichnet; die zunächst offene persönliche Kontrolle ist inzwischen im oben genannten Umfang positiv bestätigt.

**40/40 Prototyptests und 1385/1385 bestehende Tests**, eigener und vollständiger Release-Build ohne Warnungen/Fehler. Acht Lockdateien und Buildzähler per SHA-256 unverändert. Keine produktive Codeänderung, neue Schnitte oder Releaseänderung. Build 34 bleibt Draft, Systemfreeze-Ursache offen. [Nachweis, Einschränkungen und Bedienung](MP4BOX-LIBMPV-KONTROLLE-20261022.md).

## 22.10.2026 – Isolierter MP4Box-Diagnoseprototyp, Furiosa-Zeitabweichung nachvollzogen

Nach ausdrücklich freigegebener Bestandsaufnahme separat implementiert und an der vorhandenen Furiosa-MP4Box-Ausgabe geprüft: alle **171218 Ausgabepakete** dem Original und fünf Behaltebereichen zugeordnet, 109 wiederholte Nutzdaten durch exakt passende beidseitig verankerte Folgen aufgelöst. Erster Übergang mit **8,270833 ms rückläufigen Präsentationszeiten**, weitere drei lokale Decoderfenster mit jeweils einem beobachteten Wechsel. Ausgabeanfänge gegenüber dem Plan **1,117 / 4,910 / 4,695 / 9,300 s früher**. Diagnosekandidaten bleiben ausdrücklich unbestätigte CAN-Prüfpositionen.

**29/29 Prototyptests und 1385/1385 bestehende Tests**, Release-Build ohne Warnungen/Fehler. Keine Änderung am produktiven Schnitt-/Playercode und keine neuen Filmschnitte. Buildnummer 34 und vorhandene Lockdateien erhalten; keine Paketierung oder Veröffentlichung. **1.0.0 Build 32 bleibt Stable/Latest, 1.0.1 Build 34 Draft; frühere Systemhänger weiterhin ungeklärt.** Die positiven Praxistests vom 21.10. sind als Anwenderbefunde aus der Übergabe übernommen, kein neuer Stabilitätsnachweis. [Messung, Grenzen und nächster Schritt](MP4BOX-UEBERGANGSDIAGNOSE-20261022.md).

## 18.10.2026 – CAN 1.0.0 als erste stabile Latest-Version veröffentlicht

**Cut Assistant Next 1.0.0 · Build 32** auf ausdrücklichen Nutzerauftrag **öffentlich und stabil als Latest** veröffentlicht, kein Entwurf/keine Vorabversion. Annotierter Tag **v1.0.0** am tatsächlich gebauten Quellcommit **3e2ac03dfe4a701ecbc95c80cc06ff2527309b9f**, Motor otr-can 0.1.5 unverändert. Persönliche Produktiv-/VM-Setup-/Deinstallations-/Portable-Abnahme vor Veröffentlichung im jeweils gemeldeten Umfang dokumentiert.

**Alle sieben Uploads vor/nach Freigabe** über Namen, Bytezahl und SHA-256-Digest geprüft; Release Notes, Quellpin und Latest-Status bestätigt. Acht frühere öffentliche Releases, bestehende Tags und 70 ältere lokale Release-Dateien unverändert. Neue Pakete identisch zum eingefrorenen abgenommenen Satz. Acht lokale Lockdateien und Buildzähler 32 erhalten, vier bestehende Produktions-Lockänderungen ausgeschlossen. Kein neuer Build/Versionswechsel/Neuverpacken, Paketnachweis weiterhin **1347/1347 Tests**, Release ohne Warnungen/Fehler. [Stabiles Release](https://github.com/jmagull/cut-assistant-next/releases/tag/v1.0.0), [Veröffentlichungsnachweis](RELEASE-PUBLICATION-1.0.0.md).

## 18.10.2026 – Stabile Veröffentlichung CAN 1.0.0 ausdrücklich freigegeben

Nach persönlich abgeschlossener Setup-/Deinstallations-/Portable-Abnahme in der VM beauftragt der Nutzer ausdrücklich **Veröffentlichung**. Die seit dem letzten Push ergänzten persönlichen Prüfungen werden gesichert. Veröffentlichung unveränderter sieben Dateien aus `artifacts/release-1.0.0-build32` als **stabile Version 1.0.0 · Build 32**, keine Vorabversion, als Latest vorgesehen. Annotierter Tag **v1.0.0** muss auf den tatsächlich gebauten Quellcommit **3e2ac03dfe4a701ecbc95c80cc06ff2527309b9f** zeigen.

Vor Freigabe im Entwurf vollständiger Abgleich aller GitHub-Assetnamen, Größen und SHA-256-Digests; anschließend erneut öffentlich prüfen. Bestehende RC-Releases/Tags/Archive bleiben erhalten. Vier vorbestehende Produktions-Lockänderungen bleiben außerhalb des Commits; kein Versions-/Buildwechsel, Neuverpacken oder neuer Motor. Technischer Paketnachweis unverändert 1347/1347 Tests, Release ohne Warnungen/Fehler; kein erneuter Build für den Veröffentlichungsschritt. [Veröffentlichungsnachweis](RELEASE-PUBLICATION-1.0.0.md).

## 18.10.2026 – Portable/ZIP in der VM persönlich erfolgreich; Paketabnahme abgeschlossen

Nach erfolgreichem VM-Setup und Deinstallation bestätigt der Nutzer jetzt **Portable/ZIP**: **Umstellen auf einen anderen otr-can-Pfad check**; **MP4 mit MP4Box und otr-can check, Bild und Ton check, Halbierungssprung check**; **AVI mit MP4Box und otr-can check**. Damit sind die vorgesehenen persönlichen **Setup-/Deinstallations-/Portable-Praxistests in der VM** für **CAN 1.0.0 · Build 32** gemeldet.

Die persönliche Bild-/Tonbestätigung steht im neuen Bericht unter MP4; keine zusätzliche getrennte AVI-Bild-/Ton- oder Schutzsperrenprüfung für Portable daraus abgeleitet. Die entsprechenden AVI-Prüfungen des Setup-Laufs bleiben separat dokumentiert. Bekannte AVI-/MP4Box-Grenzen bleiben transparent erhalten.

Abnahmestand in README, Anleitung, Release-/Freeze-Unterlagen und Übergabe geschlossen. Nur Repository-Dokumentation geändert; eingefrorene Pakete samt beigelegten Dokumenten, Quellpin, Code, acht Lockdateien und Buildzähler erhalten. Kein neuer CAN-Build/Testlauf; Paketnachweis weiterhin **1347/1347 Tests**, Release ohne Warnungen/Fehler. **Tag und stabile Veröffentlichung bleiben gesondert freizugeben.** [Paket-/Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 18.10.2026 – VM-Setup und Deinstallation persönlich erfolgreich; Portable folgt

Der Nutzer meldet die **VM-Setup-Prüfung** für **CAN 1.0.0 · Build 32**: **Darstellungswechsel check**; bei **AVI MP4Box und otr-can check, Bild und Ton check, Frame-Lupe geblockt check, Halbierungssprung geblockt check**. Bei **MP4 „Frame Lupe doppel check“**; erfolgreiche Schnittprotokolle mit beiden Motoren liegen vor. **Deinstallation: check. Jetzt kommt ZIP** – Portable ist angekündigt und noch nicht persönlich abgenommen.

AVI-Fall **Die Teufelsbrigade**, sechs Behalteabschnitte: beide Motoren erfolgreich; otr-can 0.1.5 nach einmaliger Indexierung von 263694 Frames mit **182574** geprüften Videopaketen und **350542080** geprüften Audiosamples bei 48 kHz, MP3 → AAC-LC Stereo 192 kbit/s. Die FFmpeg-Vorbereitung enthält weiterhin Zeitstempelwarnungen; diese bleiben transparent dokumentiert. MP4-Fall **Grey’s Anatomy – Durchs Feuer**, drei Behalteabschnitte: MP4Box fertig, otr-can mit **60651** geprüften Videopaketen und **116449920** geprüften Stereo-AAC-Samples, Endmux erfolgreich. Keine zusätzliche aufgeschlüsselte persönliche MP4-Bild-/Tonabnahme aus der reinen Logausgabe abgeleitet.

**VM-Setup und Deinstallation sind damit persönlich bestätigt. Portable/ZIP bleibt offen.** Abnahmestand in README, Anleitung, Release-/Freeze-Unterlagen und VM-Übergabe aktualisiert. Nur Dokumentation geändert; eingefrorene Pakete, Code, Lockdateien und Buildzähler erhalten. Kein neuer CAN-Build/Testlauf; technischer Paketnachweis weiterhin 1347/1347 Tests und Release ohne Warnungen/Fehler. Tag und Veröffentlichung bleiben gesondert freizugeben. [Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 18.10.2026 – Toxic Folge 6: Keyframe-Grenzen sowie Bild und Ton beider Ausgaben geprüft

**Toxic Folge 6 S1E6 (HQ, Aufnahme 17.10.2026)**: Zwei MP4Box-Läufe mit je einem Behalteabschnitt und Abschluss **„Fertig.“**. Zuerst **`-splitx 653.02:3449.48`**, anschließend **`-splitx 653.82:3446.74`**. Der Nutzer bestätigt **„Anfang und Ende auf Key-Frames gesetzt :-)“**. Die zweite Anforderung beginnt 0,800 s später und endet 2,740 s früher; angeforderte Behaltedauer **2792,920 s = 00:46:32.920**, keine gemessene Ausgabedauer.

Keyframe-Auswahl an beiden Grenzen und technischer Abschluss beider Läufe dokumentiert. Anschließend bestätigt der Nutzer ausdrücklich **„Bild und Ton in beiden Fällen check :-)“**: Bild und Ton beider MP4Box-Ausgaben persönlich positiv geprüft. Ein otr-can-Ergebnis für Folge 6 wurde hier nicht mitgeliefert. Keine zusätzliche technische Keyframe-/IDR-Diagnose durchgeführt. Nur Abnahmedokumentation ergänzt; eingefrorene Pakete, Code, Lockdateien und Buildzähler erhalten. Kein neuer CAN-Build/Testlauf; Paketnachweis weiterhin 1347/1347 Tests. [Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 18.10.2026 – Toxic Folge 5: MP4Box mit Lupe korrigiert, gewählte Ausgabe blitzfrei

**Toxic Folge 5 S1E5 (HQ, Aufnahme 17.10.2026)**: MP4Box zunächst mit Werbeblitzer am Anfang. Der Nutzer setzt den Vorspannbeginn anschließend mit der **Frame-Lupe auf Keyframe 28.258** und bestätigt das neue MP4Box-Ergebnis als **werbeblitzerfrei**. Er wählt ausdrücklich diesen MP4Box-Schnitt, da laut seiner Bewertung nur der Vorspann betroffen ist. **otr-can in beiden Varianten persönlich in Ordnung**. Das nachgereichte positive MP4Box-Ergebnis beruht auf der persönlichen Rückmeldung; der gezeigte MP4Box-Aufruf `565.18:3042.58` belegt nicht den exakten Aufruf nach der Korrektur.

Vorliegendes OTR-Protokoll: eine Indexierung, **209972** gültige Quellframes, Behalteabschnitt **565.180–3042.600 s**, **123871** verifizierte Videopakete; Stereo-AAC, 48 kHz, **118916160** verifizierte dekodierte Samples. Abschließende MP4-Prüfung erfolgreich. Konsistente Sollausgabe **2477,420 s = 00:41:17.420**. Ein technischer OTR-Lauf protokolliert; die zweite Variante ist persönlich bestätigt. Keine allgemeine MP4Box-/Keyframe-Garantie daraus abgeleitet.

Nur Abnahmedokumentation und Benutzerhinweis ergänzt; Programm/Motoren, eingefrorene Pakete, Lockdateien und Buildzähler erhalten. Kein neuer CAN-Build/Testlauf, Paketnachweis weiterhin 1347/1347 Tests. [Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 18.10.2026 – Toxic Folge 4: MP4Box-Anfangsblitzer, otr-can am Anfang blitzfrei

**Toxic Folge 4 S1E4 (HQ, Aufnahme 17.10.2026)**: MP4Box beendet den einzelnen Behalteabschnitt **`-splitx 570.12:3202.3`** mit „Fertig.“; persönliche Rückmeldung **„Werbeblitzer am Anfang“**. Der otr-can-0.1.5-Lauf endet samt abschließender MP4-Prüfung erfolgreich; der Nutzer bestätigt **„Keine Blitzer am Anfang“**.

OTR-Protokoll: eine Indexierung, **223494** gültige Quellframes, ein Behalteabschnitt **570.120–3202.320 s**, **131610** verifizierte Videopakete; Stereo-AAC, 48 kHz, **126345600** verifizierte dekodierte Samples. Konsistente Sollausgabe **2632,200 s = 00:43:52.200**. OTR-Endzeit 0,020 s später als die MP4Box-Anforderung; kein vollständig identischer A/B-Plan behauptet. Die persönliche Qualitätsbestätigung bezieht sich ausdrücklich auf den Anfang.

Weitere persönliche MP4Box-Präzisionsgrenze dokumentiert; keine zusätzliche Bild-/Paketdiagnose durchgeführt. Nur Abnahmedokumentation und Benutzerhinweis ergänzt; eingefrorene Pakete, Code, Lockdateien und Buildzähler erhalten. Kein neuer CAN-Build/Testlauf; Paketnachweis weiterhin 1347/1347 Tests. VM-/Portable-/Deinstallationsabnahme bleibt gesondert. [Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 17.10.2026 – Heutige Praxistestrunde beendet; Commit und Push freigegeben

Der Nutzer beendet die heutige Testrunde und beauftragt ausdrücklich **Commit und Push**. Gesichert werden der vorbereitete 1.0-Quellstand sowie die persönlichen Abnahmen: Produktiv-Setup, Drag & Drop, beide Ansichten und problemloser Wechsel, Halbierungssuche, Frame-Lupe, Schutzdialog und die dokumentierten Filmtests. Der Doc-Zwischenstand mit unfertiger Cutlist ist aufgelöst; die fertige Cutlist läuft mit beiden Motoren persönlich problemlos. Bekannte Radical-/Toxic-Folge-1-MP4Box-Befunde bleiben dokumentiert.

Paketquellpin bleibt **3e2ac03dfe4a701ecbc95c80cc06ff2527309b9f**, **1.0.0 · Build 32**, otr-can 0.1.5. Eingefrorene Archive/Prüfsummen, Buildzähler und acht lokale Lockdateien erhalten; vier vorbestehende Produktions-Lockänderungen ausdrücklich außerhalb des Commits. Abschließender Git-Ausführungsnachweis lokal unter `.build/commit-push-20261017`. Kein neuer Build/Testlauf für die reine Abnahmedokumentation; bestehender Paketnachweis **1347/1347 Tests**, Release ohne Warnungen/Fehler. **Neue VM-/Setup-/Deinstallations-/Portable-Abnahme sowie Tag und Veröffentlichung bleiben gesonderte Schritte.**

## 17.10.2026 – Brilliant Minds mit drei Abschnitten und beiden Motoren persönlich erfolgreich

**Brilliant Minds (HQ, Aufnahme 16.10.2026)**: MP4Box schneidet drei Abschnitte und fügt sie erfolgreich zusammen. Persönlich bestätigt **„ohne blitzer“**. Der anschließende otr-can-0.1.5-Lauf ist vollständig erfolgreich; Rückmeldung **„auc alle prima“** als positive Gesamtbewertung dokumentiert.

OTR-Protokoll: eine Indexierung, **179998** gültige Quellframes, **60103** verifizierte Videopakete; eine Stereo-AAC-Spur, 48 kHz, **115397760** verifizierte dekodierte Samples. Abschließende MP4-Prüfung erfolgreich. Konsistente Sollausgabe **2404,120 s = 00:40:04.120**. OTR-Endzeiten jeweils 0,040 s später als die MP4Box-Anforderungen; kein vollständig identischer A/B-Plan behauptet.

Nur Abnahmedokumentation ergänzt; eingefrorene Pakete, Code, Lockdateien und Buildzähler erhalten. Kein neuer CAN-Build/Testlauf; technischer Paketnachweis weiterhin 1347/1347 Tests. Keine weitere VM-/Portable-/Deinstallationsabnahme gemeldet. [Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 17.10.2026 – Doc S02E16 mit fertiger Cutlist: beide Motoren persönlich erfolgreich

Der Nutzer stellt klar, dass die vorherige Cutlist noch nicht fertig war. Mit dem nun gesetzten Schluss-Schnitt enden die beiden Behaltebereiche für otr-can bei **2180.000 s** und **3737.160 s**, statt den zweiten Abschnitt bis ans Videoende laufen zu lassen. MP4Box-Protokoll mit `582.96:2179.96` und `2842.36:3737.12` samt Zusammenfügen erfolgreich. Der Nutzer bestätigt **„Beide ohne probleme“**.

otr-can **0.1.5**: eine Indexierung, **157446** gültige Quellframes, zwei Behalteabschnitte, **62296** verifizierte Videopakete; Stereo-AAC, 48 kHz, **119608320** verifizierte dekodierte Samples. Abschließende MP4-/Zeitstempel-/Padding-/Decoderkonfigurationsprüfung erfolgreich. Konsistente Sollausgabe **2491,840 s = 00:41:31.840**. Die MP4Box-Endanforderungen liegen jeweils 0,040 s früher; kein identischer A/B-Plan behauptet.

Die frühere Abbruchdiagnose betrifft die **unfertige Cutlist**, nicht den jetzt erfolgreichen finalen Filmtest. Merksatz: **Cutlist einschließlich Schlussbereich vollständig erstellen; alle behaltenen Bereiche müssen von den Originaltonspuren abgedeckt sein.** Bei dieser Aufnahme war der Schluss-Schnitt nötig; kein allgemeiner Zwang zu einem Schluss-Schnitt bei jeder Datei. Kein Programm-/Motorfehler im finalen Doc-Test nachgewiesen. Diagnosebericht, Anleitung, README und Abnahmestand entsprechend korrigiert.

Nur Dokumentation geändert; eingefrorene Pakete, Code, acht Lockdateien und Buildzähler erhalten. Kein neuer CAN-Build/Testlauf, technischer Paketnachweis weiterhin 1347/1347 Tests. VM-/Portable-/Deinstallationsabnahme bleibt gesondert. [Finaler Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md), [historischer Zwischenstand und Auflösung](DOC-S02E16-AUDIOENDE-20261017.md).

## 17.10.2026 – Historischer Doc-Zwischenstand: unfertige Cutlist bis zum Videoende

Der nachgereichte otr-can-Lauf scheitert nach erfolgreicher Videoverifikation an **242520960 erwarteten / 242483328 vorhandenen PCM-Samples**. Das unveränderte Original hat **6297.840 s Video**, aber nur **6297.056 s Stereo-AAC-Ton bei 48 kHz**: **0,784 s = 37632 Samples fehlen am Schluss**. Unabhängige Dekodierung mit dem unveränderten Motorfilter reproduziert die Samplezahl exakt, ohne Warnungen. Die strenge PCM-Prüfung stoppt den Auftrag; kein erfolgreiches otr-can-Endergebnis für den ursprünglichen Plan.

Rein diagnostisch geprüfte Variante: Behalte-Ende **01:44:57.040** liefert exakt die erwarteten **242482560** Samples. Dafür würden **20 Videobilder / 0,8 s Schluss** entfallen; keine Cutliständerung und kein vollständiger neuer Filmschnitt durchgeführt. Kein automatisches Auffüllen oder Lockern der Prüfung. MP4Box bleibt persönlich mit „Alles prima“ bestätigt.

Nachträglich vom Nutzer als **unfertige Cutlist** eingeordnet; der anschließende finale Doc-Test mit Schluss-Schnitt ist mit beiden Motoren persönlich erfolgreich. Dieser Eintrag bewahrt ausschließlich den historischen Zwischenstand und seine technische Erklärung. Original, Anwendung/Motor, eingefrorene Pakete, acht Lockdateien und Buildzähler erhalten. Keine neue Release-/VM-Abnahme; technischer Paketnachweis weiterhin 1347/1347 Tests, kein neuer CAN-Build/Testlauf für die Dokumentation. [Ursache, Audioprüfungen und begrenzte Variante](DOC-S02E16-AUDIOENDE-20261017.md).

## 17.10.2026 – Doc S02E16 mit MP4Box persönlich positiv geprüft

**Doc S02E16 (HQ, Aufnahme 16.10.2026)**: Das bereitgestellte MP4Box-Protokoll zeigt zwei Behalteabschnitte **582.96–2179.96 s** und **2842.36–6297.8 s**, anschließendes Zusammenfügen und Abschluss **„Fertig.“**. Der Nutzer bestätigt anschließend ausdrücklich **„Alles prima“**. MP4Box damit für diesen Praxisfall persönlich positiv geprüft.

Der Nutzer kündigt **„Jetzt kommt otr-cn“** an; der otr-can-Test dieses Falls steht zum Zeitpunkt dieses Eintrags noch aus. Nur Abnahmedokumentation ergänzt; eingefrorene Pakete, Programmcode, Lockdateien und Buildzähler erhalten. Kein neuer Build/Testlauf; technischer Paketnachweis weiterhin 1347/1347 Tests. [Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 17.10.2026 – Toxic Folge 3: MP4Box persönlich positiv, otr-can technisch erfolgreich

**Toxic Folge 3 S1E3 (HQ, Aufnahme 16.10.2026)**: MP4Box beendet den einzelnen Behalteabschnitt **`-splitx 747.3:3222.76`** mit „Fertig.“; persönliche Rückmeldung **„MP4Box prima“**. Der otr-can-0.1.5-Lauf ist laut Protokoll ebenfalls erfolgreich abgeschlossen; eine zusätzliche persönliche Bewertung dieser otr-can-Ausgabe wurde in dieser Nachricht noch nicht ausdrücklich genannt.

OTR-Protokoll: eine Indexierung, **239975** gültige Quellframes, Behalteabschnitt **747.300–3222.780 s**, **123774** verifizierte Videopakete; Stereo-AAC, 48 kHz, **118823040** geprüfte dekodierte Samples. Konsistente Sollausgabe aus Protokollwerten: **2475,480 s = 00:41:15.480**. OTR-Endzeit **0,020 s** später als im MP4Box-Aufruf; kein identischer A/B-Plan behauptet.

Nur Abnahmedokumentation ergänzt; eingefrorene Pakete, Code, Lockdateien und Buildzähler erhalten. Kein neuer Build/Testlauf, technischer Paketnachweis weiterhin 1347/1347 Tests. Bekannte frühere MP4Box-Befunde und offene Paket-/VM-Prüfpunkte unverändert. [Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 17.10.2026 – Toxic Folge 2 mit beiden Motoren persönlich erfolgreich geprüft

**Toxic Folge 2 S1E2 (HQ, Aufnahme 16.10.2026)**: MP4Box beendet den einzelnen Behalteabschnitt **`-splitx 511.84:2931.26`** mit „Fertig.“; der Nutzer bestätigt **„Keine Blitzer“**. Der anschließende otr-can-0.1.5-Lauf ist ebenfalls erfolgreich, persönliche Rückmeldung **„auch alles prima“**.

OTR-Protokoll: eine Indexierung, **194968** gültige Quellframes, ein Behalteabschnitt **511.840–2931.280 s**, **120972** verifizierte Videopakete; Stereo-AAC, 48 kHz, **116133120** geprüfte dekodierte Samples. Konsistente Sollausgabe aus Protokollwerten: **2419,440 s = 00:40:19.440**. Die OTR-Endzeit liegt **0,020 s** nach dem MP4Box-Aufruf; kein vollständig identischer A/B-Plan behauptet. Der frühere Blitzer bei **Toxic Folge 1** ist ein anderer Testfall und bleibt dokumentiert.

Nur Abnahmedokumentation ergänzt; eingefrorene Pakete, Programmcode, Lockdateien und Buildzähler erhalten. Kein neuer Build/Testlauf; technischer Paketnachweis weiterhin 1347/1347 Tests. Keine weitere VM-/Portable-/Deinstallationsabnahme gemeldet. [Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 17.10.2026 – Beide Ansichten und problemloser Wechsel persönlich bestätigt

Der Nutzer bestätigt **„sieht beides gut aus, umschalten klappt ohne probleme“** mit zwei Screenshots aus **CAN 1.0.0 · Build 32**. Standard- und Power-User-Ansicht zeigen **Winter Spring Summer or Fall** mit Frame **47.524**, Position **00:31:40**, sechs identischen Schnittbereichen und angezeigter Schnittdauer **01:28:50.160**. Auch Lautstärke **74 %** und Halbierungssuchweite **1 Frame** stimmen sichtbar überein. Beide Darstellungen und der Ansichtswechsel sind damit persönlich positiv geprüft.

Nur Abnahmedokumentation ergänzt; eingefrorene Pakete, Programmcode, Lockdateien und Buildzähler erhalten. Technischer Paketnachweis weiterhin 1347/1347 Tests und Release ohne Warnungen/Fehler; kein neuer Build/Testlauf für diesen Nachtrag. Die gesonderte Windows-11-VM-/Portable-/Deinstallationsabnahme bleibt offen. [Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 17.10.2026 – Halbierungssuche und Frame-Lupe persönlich bestätigt

Der Nutzer meldet **„halbierungssuche check“** und **„Framelupe check“**. Damit sind **Halbierungssuche und Frame-Lupe** für den aktuellen CAN-1.0.0-Build-32-Praxislauf persönlich positiv geprüft.

Nur Abnahmedokumentation ergänzt; Programmcode, eingefrorene Pakete, Prüfsummen, Lockdateien und Buildzähler erhalten. Technischer Paketnachweis weiterhin **1347/1347 Tests**, Release **0 Warnungen/0 Fehler**; kein neuer Build oder Testlauf für diesen Dokumentationsnachtrag. [Paket-/Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 17.10.2026 – Winter Spring Summer or Fall mit beiden Motoren persönlich positiv geprüft

Weiterer realer HQ-Filmtest: **Winter Spring Summer or Fall**, Aufnahme vom 16.10.2026. MP4Box-Protokoll enthält fünf `-splitx`-Aufrufe und anschließendes Zusammenfügen bis **„Fertig.“**. Der Nutzer bestätigt **„Alles in Ordnung keine Blitzer“**, außerdem **„ort-can auch alles ok“** (otr-can).

otr-can **0.1.5**: eine FFMS2-Indexierung, **194972** gültige Quellframes, fünf Behalteabschnitte, **133254** geprüfte Videopakete. Eine Stereo-AAC-Spur mit 48 kHz und **255847680** geprüften dekodierten Samples, abschließende MP4-Paket-/Zeitstempelprüfung erfolgreich. Aus Protokollwerten berechnet: **5330,160 s (01:28:50.160)** Sollausgabe. Die fünf OTR-Endzeiten liegen jeweils 0,040 s hinter den im MP4Box-Aufruf übergebenen Endzeiten; daher kein vollständig identischer A/B-Schnittplan behauptet.

Persönlicher positiver Filmtest dokumentiert; keine zusätzliche vollständige Decoder-, Tonspur-Einzel- oder VM-/Portable-/Deinstallationsabnahme daraus abgeleitet. Bekannte Radical-/Toxic-MP4Box-Befunde bleiben bestehen. Nur Dokumentation geändert; eingefrorene Pakete, Code, Lockdateien und Buildzähler erhalten. Kein neuer Build oder Testlauf; technischer Paketnachweis weiterhin 1347/1347 Tests. Der Nutzer testet weiter. [Abnahmebericht und Abschnittsvergleich](RELEASE-1.0.0-BUILD32-RESULT.md).

## 17.10.2026 – Schutzdialog beim Videowechsel persönlich geprüft

Der Nutzer bestätigt **„Prüfdialog check“** mit Screenshot des Dialogs **„Videodatei wechseln“** in CAN **1.0.0 · Build 32**. Sichtbar sind die Rückfrage zum Laden einer anderen Datei und Verwerfen der aktuellen Schnittarbeit, der Hinweis auf vorheriges Speichern der Cutlist sowie **Ja/Nein**. Damit ist die angezeigte Schutzrückfrage persönlich geprüft. Eine tatsächlich gewählte Antwort oder das nachfolgende Verhalten wurde in dieser Rückmeldung nicht getrennt bestätigt.

Paketbericht, Status und Tagebuch ergänzt. Technischer Nachweis der Ja-/Nein-/Zustandserhaltung bleibt der vorhandene automatisierte/native Prüfstand; kein neuer Build oder Testlauf für diesen Dokumentationsnachtrag. Eingefrorene Pakete, Programmcode, Lockdateien und Buildzähler erhalten; VM-/Portable-/Deinstallationsabnahme weiterhin offen. [Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 17.10.2026 – Toxic in der Power-User-Ansicht geprüft; Drag & Drop erneut bestätigt

Persönlicher Praxislauf mit **Toxic Folge 1 S1E1 (HQ, Aufnahme 16.10.2026)** in der Profi-/Power-User-Darstellung. MP4Box-Aufruf mit **`-splitx 746.3:3104.8`** beendet den einzigen Behalteabschnitt erfolgreich; der Nutzer meldet jedoch **einen Blitzer am Filmanfang**. Mit **otr-can 0.1.5 kein Blitzer am Anfang, alles prima**. Im vom Nutzer bereitgestellten Cutlist-Server-Screenshot sind die otr-can-Ausgabe als blitzfrei und **Bild, Ton und alle Schnittübergänge als geprüft** vermerkt. Einzelne Tonspuren sind nicht getrennt beschrieben; keine zusätzliche technische Bild-/Paketdiagnose durchgeführt.

Zusätzlich ausdrücklich bestätigt: **„video in Player gezogen check :-)“**. Drag & Drop damit erneut im persönlichen Praxislauf positiv gemeldet. Die gemeldeten Prüfungen sind nicht als VM-/Portable-/Deinstallationsabnahme bezeichnet; diese Prüfpunkte bleiben offen. Der erfolgreiche MP4Box-Prozesslauf und seine Bildqualität bleiben getrennt bewertet, die bestehende Präzisionsgrenze ist um den konkreten Toxic-Fall ergänzt.

Nur Dokumentation ergänzt; eingefrorene Pakete, Prüfsummen, Programmcode, Lockdateien und Buildzähler erhalten. Technischer Paketnachweis weiterhin **1347/1347 Tests**, Release **0 Warnungen/0 Fehler**; kein erneuter Build oder Testlauf für diesen Dokumentationsnachtrag. [Paket-/Abnahmebericht](RELEASE-1.0.0-BUILD32-RESULT.md), [Hinweise zu MP4Box](NUTZERANLEITUNG.md#mp4box-erfolgreichen-schnitt-auf-werbeblitzer-prüfen).

## 16.10.2026 – CAN 1.0.0 Build 32: Setup auf dem Produktivrechner persönlich bestätigt

Der Nutzer meldet ausdrücklich **„setup produktiv rechner check“**. Damit ist das Setup von **CAN 1.0.0 · Build 32** auf dem Produktivrechner persönlich erfolgreich geprüft. Die vorherigen Screenshots zeigen die stabile Versionsanzeige sowie Standard- und Power-User-Ansicht mit geladenem Film und gleicher Position/Schnittliste.

Die persönliche Windows-11-VM-Abnahme des neuen Setup-Pakets, der Deinstallation und der Portable-Ausgabe steht weiterhin aus. Bekannte AVI-/MP4Box-Grenzen gelten weiter. Nur Abnahmedokumentation ergänzt; eingefrorene Pakete, Prüfsummen, gebauter Quellcommit, Programmcode, lokale Lockdateien und Buildzähler bleiben erhalten. Technischer Paketnachweis weiterhin **1347/1347 Tests**, Build **0 Warnungen/0 Fehler**; kein neuer Build oder Testlauf für diesen reinen Dokumentationsnachtrag. [Paketbericht](RELEASE-1.0.0-BUILD32-RESULT.md).

## 16.10.2026 – CAN 1.0.0 Build 32: stabile Pakete lokal fertig und eingefroren

**Setup, Portable-ZIP, CAN-/Motor-/libmpv-Quellen, Relink und SHA-256** aus lokalem Quellcommit **3e2ac03dfe4a701ecbc95c80cc06ff2527309b9f** erstellt. Anzeige **1.0.0 · Build 32** ohne RC; otr-can 0.1.5 unverändert. Endgültiger isolierter Locked Restore, Release **0 Warnungen/0 Fehler**, **1347/1347 Pakettests**, keine übersprungen. **250 Portable-Dateien**, **558 Quellen-ZIP-Dateien**, **13 native Fallgruppen**, keine Bindungsfehler; endgültige Helfer-DLLs bytegleich mit den Paketen. Persönliche Einstellungen erhalten.

Sieben Auslieferungsdateien unter `artifacts/release-1.0.0-build32` schreibgeschützt eingefroren. Buildzähler 32; alle acht lokalen Lockdateien einschließlich der vier vorbestehenden Änderungen erhalten; 70 ältere Release-Dateien unverändert. Ergebnisdokumentation folgt dem gebauten Quellpin, ohne Archive nachträglich zu ändern. **Neue persönliche Windows-11-VM-/Setup-/Portable-/Deinstallationsabnahme steht aus. Kein Push, Tag oder GitHub-Release.** Bekannte AVI-/MP4Box-Grenzen bleiben dokumentiert. [Paketprüfung und SHA-256](RELEASE-1.0.0-BUILD32-RESULT.md), [VM-Übergabe](UEBERGABE-CAN-1.0.0-BUILD32-20261016.txt).

## 16.10.2026 – Funktions-Freeze und lokale Pakete für CAN 1.0.0 beauftragt

Nach der persönlichen Funktionsabnahme erklärt der Nutzer den Stand für eingefroren und „1.0 Release ready“; anschließend Auftrag **Setup, ZIP und zugehörige Artefakte**. Ziel **1.0.0 · Build 32** ohne RC-Kennung, unverändertes otr-can 0.1.5. Keine weiteren größeren Funktionen ohne ausdrückliche Entscheidung. Versionsdatei, Paketbau und Setup für stabile Kennungen angepasst; README, Anleitung, Freeze-Umfang und Release Notes aktualisiert.

Der bekannte Radical-Werbeblitzer bei MP4Box bleibt dokumentiert. Neue Pakete werden aus isolierten Quellen erstellt und mit vollständiger Testsuite, Inhalts-/Herkunftsprüfung und SHA-256 verifiziert. Persönliche Windows-11-VM-Abnahme der neuen Pakete steht anschließend aus. Der lokale Quellcommit wurde anschließend ausdrücklich freigegeben. Push, Tag und GitHub-Veröffentlichung bleiben gesondert freizugeben; bestehende RC-Archive und lokale Lockänderungen bleiben erhalten. [Freeze](FREEZE-1.0.0-20261016.md), [Release Notes](RELEASE-NOTES-1.0.0.md), [Paketprüfung](RELEASE-1.0.0-BUILD32-RESULT.md).

## 16.10.2026 – Persönliche Prüfung von Layout, Frame-Werkzeugen und beiden Schnittmotoren bestätigt

Der Nutzer meldet für die aktuelle lokale Prüffassung:

- **Frame-Lupe geprüft.**
- **Frame-Liste geprüft.**
- **MP4Box-Schnitt erfolgreich.**
- **otr-can-Schnitt erfolgreich.**
- **Power-User-Layout persönlich geprüft.**

Kommandozeilenübergabe mit realer HD-Aufnahme und Drag & Drop waren bereits zuvor persönlich bestätigt. Damit sind die drei neuen Arbeitspakete im persönlichen Praxislauf positiv gemeldet. Die Formulierung „Frame-Liste“ wird entsprechend der Nutzerrückmeldung übernommen.

Der erfolgreiche MP4Box-Schnitt bestätigt den Ablauf; er hebt den bereits nachgewiesenen **Radical-Werbeblitzer bei 57:27.314** nicht auf. Eine Behebung wurde nicht gemeldet. Die persönliche Radical-Prüfung mit otr-can 0.1.5 bleibt positiv. Echte DPI-/Mehrmonitorwechsel, weitere Release-Regressionen und die Paket-/Windows-11-VM-Prüfung dieses Arbeitsstands bleiben gesonderte Prüfpunkte; die frühere Build-31-Paketabnahme wird nicht übertragen.

README, Benutzeranleitung und Layout-Prüfbericht auf diesen persönlichen Prüfstand gebracht; ausschließlich Dokumentation geändert. Kein endgültiger Freeze, Versions-/Buildnummerwechsel oder Veröffentlichungsauftrag daraus abgeleitet. [Aktueller Layout-Prüfbericht](POWER-USER-FEINSCHLIFF-20261016.md), [offener MP4Box-Befund](RADICAL-MP4BOX-WERBEBLITZER-20261016.md).

Nach dem Dokumentationsnachtrag erneut geprüft: Release-Build **0 Warnungen/0 Fehler**, **1347/1347 Tests** erfolgreich, keine übersprungen. Alle übrigen Projektdateien einschließlich Quellcode und Lockdateien sowie der Buildzähler unverändert; Nachweise unter `.build/personal-acceptance-20261016`.

## 16.10.2026 – Radical HD mit otr-can persönlich abgenommen; MP4Box-Blitzer eingegrenzt

Nutzerprotokoll bestätigt erfolgreichen otr-can-0.1.5-Schnitt mit genau einer Indexierung, **464866** geprüften Quellframes, **350453** Ausgabebildern und **7009,060 s** Ergebnis. AAC-Stereo und AC-3 mit sechs Kanälen, je **336434880** geprüfte Samples bei 48 kHz; Muxprüfung erfolgreich. Nutzer meldet keine Werbeblitzer und bestätigt im Cutlist-Server-Screenshot **Bild, Ton und alle Schnittübergänge geprüft**. Cutlist online sichtbar; kein Upload durch diese Untersuchung und keine pauschale Format-/Tonspurfreigabe daraus abgeleitet.

MP4Box läuft durch, aber Nutzerblitzer bei **57:27.314** bestätigt: Videopaket-/Bildvergleich ordnet das dort dekodierte Bild eindeutig Original **4814,400 s** zu, vor dem vorgesehenen zweiten Behaltebeginn **4814,560 s**. Der kopierte zweite Abschnitt beginnt bereits am Random Access Point **4813,940 s**, **0,620 s früher**; passt zum bestehenden `-splitx`-Verhalten. Aktuelle MP4Box-Videodauer 7009,697333 s. Die ältere gespeicherte Cutlist unterscheidet sich am ersten Beginn um ein Bild vom neuen OTR-Protokoll, weshalb kein vollständig identischer A/B-Plan behauptet wird; zweite Sollgrenze stimmt überein.

Nur lesende Diagnose und Dokumentation; Originale/Ausgaben erhalten. Bekannte MP4Box-Präzisionsgrenze offen, keine Motor-/Layoutkorrektur oder allgemeine Release-Abnahme behauptet. Benutzeranleitung um konkreten Hinweis ergänzt. Anwendung, acht Lockdateien, Version/Buildzähler und eingefrorene Pakete unverändert. [Bildnachweis, technische Details und Grenzen](RADICAL-MP4BOX-WERBEBLITZER-20261016.md).

## 16.10.2026 – Power-User-Werkzeuge gemäß Konzeptskizze geordnet

Der Nutzer bestätigt **Video in Player ziehen erfolgreich** und bewertet die grundsätzliche Zweispaltenanordnung positiv. Auf ausdrücklichen Feinschliff-Auftrag rechts umgesetzt: kurzer Hinweis, vier gleich breite Schnittpunktbuttons, feste vollständige Zeit-/Lupenzeile, Tabelle mit eigenem Bildlauf und Löschen rechts darunter, danach **Film schneiden**, **Cutlists** und breite **Namensmaske**. Originale Ereignisse/Bindings weiterverwendet; Standardreihenfolge und -darstellung beim Rückwechsel wiederhergestellt. Nur rechte Werkzeuge werden umgeordnet, Player und Tabelle behalten ihre Elternstruktur. Anfangsbreite rechts 460 DIP, Minimum 430; Splitter und Rückfall für schmale Fenster erhalten.

Release-Rebuild **0 Warnungen/0 Fehler**, **1347/1347 Tests**, keine übersprungen, einschließlich vier neuer tatsächlicher WPF-Tests. **Zwölf native Fallgruppen** mit echtem libmpv: bisherige Ansicht-/Drop-/AVI-/Navigationsregression sowie leere/40-zeilige Tabelle, feste Lupe bei neunstelligen Stunden, tatsächliche Schnitt-/Löschklicks und Dialogaufrufe. Motor-/Erzeugungs-/Namensmaskendialoge und Dateiauswahl abgebrochen; Upload-/Server-Schutz geprüft, kein Schnitt/Upload/Speichern ausgeführt. Keine Bindungsfehler; rechte Werkzeuge separat gerendert und visuell geprüft. Echte DPI-/Mehrmonitor- und persönliche Feinschliff-Abnahme offen.

README, Anleitung, Status und Tagebuch aktualisiert. Acht Lockdateien, vier bestehende Produktions-Lockänderungen, Version.props und Buildzähler erhalten; **0.3.0/31/RC3**. Keine Änderungen an eingefrorenen Paketen, kein Commit/Push/Tag/Release. [Änderungen, Prüfgrenzen und neue lokale Prüffassung](POWER-USER-FEINSCHLIFF-20261016.md).

## 15.10.2026 – Drei Arbeitspakete für Zielversion 1.0 technisch umgesetzt

Optionaler einzelner Startpfad nach Fenster-/Playerbereitschaft; derselbe Medienladeweg wie Dateidialog und Drop. Absolute/relative/Unicode-/UNC-Pfade berücksichtigt, Existenzprüfung im Hintergrund. WPF- und natives OLE-Dropziel am bestehenden Video-HWND, ohne Overlay; einzelne unterstützte Dateiendung, keine Ordner/Mehrfachablage. Vor dem Verwerfen von Schnittbereichen, vorgemerktem Anfang oder geladener Cutlist gemeinsame Bestätigung; Abbruch erhält Film, Position und Schnittzustand.

**Einstellungen → Darstellung** speichert Standard-/Power-User-Präferenz separat. Sofortiger Wechsel verändert ausschließlich Grid-Positionen/-Größen; kein Reparenting, neuer Player oder HWND. Großer Player links, gesamter Schnittbereich rechts mit Splitter und Bildlauf. Unter 1100 DIP Inhaltsbreite vorübergehend Standardanordnung mit Hinweis, Präferenz erhalten. Bei geringer Höhe größere scrollbare Arbeitsfläche, kein Abschneiden von Funktionen. In 1920×1080 Testfenster Videofläche von 348 auf ca. 615 DIP Höhe vergrößert.

Vollständiger isolierter Release-Rebuild **0 Warnungen/0 Fehler**; **1343/1343 Tests**, keine übersprungen (Core 196, Cutlists 72, Media 194, App 881; 35 neue Fälle). Native Prüfungen bestätigen stabile HWND-/Playeridentität, Pausenposition/Schnittdaten, laufende Wiedergabe, Drop-Laden, Abbruch eines Dateiwechsels, AVI-/MP4-Wechsel, Keyframes, Halbierung und Lupenvorschau in beiden Ansichten. Zusätzlich **36 Kontrollplayer-Prüfungen** und drei synthetische AAC/AC3-Schnittläufe mit MP4Box/otr-can sowie Cutlist-Rundlauf, Namensbildung und vollständiger Stream-Dekodierung bestanden; keine Bindungsfehler.

Alle acht Lockdateien bytegleich, insbesondere vier vorbestehende Produktionsänderungen erhalten. **0.3.0/31/RC3**, Version.props und Buildzähler unverändert. Keine Commits, Pushes, Tags, Paketierung oder Veröffentlichung. Reale Explorer-Gesten, HD-Film-/Tonabnahme, echte DPI-/Mehrmonitorwechsel und neue Windows-11-VM-/Installations-/Portable-Abnahme bleiben persönlich offen. Größere Funktionen für 1.0 damit technisch umgesetzt; vor endgültigem Freeze/Versionswechsel weiterhin persönliche Prüfung und gesonderte Nutzerfreigabe. [Konzept, Nachweise, Grenzen und Startpfad](ZWEI-ANSICHTEN-START-DROP-20261015.md).

## 15.10.2026 – Build 31 RC3 öffentlich veröffentlicht

Nach persönlicher VM-/Setup-/Portable-/Deinstallations- und Schnittabnahme beauftragt der Nutzer ausdrücklich die Veröffentlichung. **0.3.0 · Build 31 · RC3** unverändert als [öffentliche Vorabversion](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build31-rc3) freigegeben, kein Entwurf und keine stabile Latest-Ausgabe. Quelltag **v0.3.0-build31-rc3** auf **de8ec0d598b095d85e4559e7fd83012534959637**. Alle sieben Uploads vor/nach Freigabe mit eindeutigen Namen, Status, Größe und SHA-256-Digest geprüft, Release-Text identisch.

Build 30/29/16 samt Assets unverändert, lokale eingefrorene Dateien und alle acht Lockdateien erhalten, Buildzähler weiter 31. Kein neuer Build, Testlauf oder Motorwechsel; technischer Nachweis weiterhin **1308/1308 Tests**. [Veröffentlichung und Download-Prüfsummen](RELEASE-PUBLICATION-BUILD31-RC3.md), [Paketbericht und Grenzen](RELEASE-BUILD31-RC3-RESULT.md).

## 15.10.2026 – Build 31 persönlich in der VM und mit beiden Motoren abgenommen

Nach Paketierung bestätigt der Nutzer **Setup in der VM, Deinstallation und Portable-ZIP in der VM erfolgreich**, außerdem **Videoschnitt mit MP4Box und otr-can**. Die zuvor offenen persönlichen Paketprüfungen sind damit gemeldet. Keine zusätzliche vollständige Film-/Tonspurprüfung aller Ausgaben oder getrennte Setup-/Portable-Zuordnung jedes Motorlaufs daraus abgeleitet. [Abnahme und Grenzen](RELEASE-BUILD31-RC3-RESULT.md).

Nur Dokumentation ergänzt. Eingefrorene Pakete und Prüfsummen, Quellcommit **de8ec0d598b095d85e4559e7fd83012534959637**, Tag **v0.3.0-build31-rc3**, Buildzähler 31 und lokale Lockänderungen erhalten. Technischer Paketnachweis weiterhin **1308/1308 Tests**, kein neuer Build für diese Dokumentation; keine neue GitHub-Release-Veröffentlichung.

## 15.10.2026 – Build 31 RC3 eingefroren, Pakete fertig und Quellstand gepusht

**0.3.0 · Build 31 · RC3**, Quellcommit **de8ec0d598b095d85e4559e7fd83012534959637**, annotierter Remote-Tag **v0.3.0-build31-rc3**. Setup, Portable, CAN-/Motor-/libmpv-Quellen, Relink und Prüfsummen lokal unter `artifacts/release-build31-RC3`, sechs Archive plus SHA256SUMS schreibgeschützt. Herkunft aus festem Commit, Locked Restore, Release **0 Warnungen/0 Fehler**, **1308/1308 Pakettests**, keine übersprungen. **249 Portable-Dateien**, **539 Quellen-ZIP-Dateien**, vier native UI-Wechselfälle, sechs Indexvergleiche und drei Produktionsdienst-Fälle mit echtem FFMS2 geprüft; keine Bindungsfehler. Motor 0.1.5 unverändert.

Alle acht lokalen Lockdateien bytegleich, vier vorbestehende Produktionsänderungen erhalten und nicht committet. Lokaler Buildzähler nach bestandenem Paketlauf auf **31**. Build-30-/Altartefakte erhalten. Keine frische Setup-/VM-/Deinstallations- oder vollständige Film-/Tonabnahme. Keine neue GitHub-Release-Veröffentlichung; diese bleibt gesondert. [Abschlussbericht und Prüfsummen](RELEASE-BUILD31-RC3-RESULT.md), [Übergabe](UEBERGABE-BUILD31-RC3-20261015.txt).

## 15.10.2026 – Build 31 RC3 zum Einfrieren und Paketieren freigegeben

Der Nutzer beauftragt nach den erfolgreichen Praxistests das Einfrieren, Setup, ZIP und zugehörige Artefakte sowie Commit und Push. Neuer Paketsatz **0.3.0 · Build 31 · RC3**, unveränderter Motor **otr-can 0.1.5**. Enthalten: Hauptplayer-Halbierungssuche, AVI-Ausblendung/Lupensperre und zweistufige Zeitstrukturvorprüfung vor otr-can. AVI/MP4-Umschaltung und tatsächliche Schutzmeldungen für No Turning Back/Navy CIS zuletzt persönlich bestätigt. Öffentliche Build-30-/Altartefakte bleiben erhalten. [Freeze-Auftrag](FREEZE-BUILD31-RC3-20261015.md), [Paketprüfung](RELEASE-BUILD31-RC3-RESULT.md).

Vier vorbestehende Produktions-Lockänderungen bleiben erhalten und außerhalb des Quellcommits; Quellen aus festem Commit, isolierter Locked Restore und vollständige Paketprüfung geplant. Kein frischer VM-/Installationsnachweis behauptet. Die GitHub-Release-Veröffentlichung bleibt ein gesonderter Schritt.

## 15.10.2026 – OTR-CAN-Vorprüfung um Nullstart und CFR-Raster erweitert

Neuer Nutzerfall **Navy CIS**, AVI unter `.avi.mp4`: 112423 eindeutige aufsteigende FFMS2-Zeiten, aber Motorabbruch wegen nicht unterstützter Zeitstruktur. Reale separate Diagnosekopie beginnt bei **0, 840, 880, 920 ms**. Vorprüfung übernimmt die exakten Nullstart-/25-/50-fps-Rasterbedingungen samt Mikrosekundentoleranzen aus dem unveränderten Motor 0.1.5. Navy wird nun vor dem Schnittaufruf mit eigener verständlicher Meldung gestoppt. No Turning Back bleibt gesperrt, Teufelsbrigade freigegeben. Keine Bildratenumrechnung, keine Änderung an Vorbereitung, MP4Box oder Motor.

Release-Rebuild **0 Warnungen/0 Fehler**, **1308/1308 Tests**, 0 übersprungen; sechs reale Indexvergleiche und Produktionsschnittdienst mit echtem FFMS2-Indexer an drei Quellen bestanden, positive Motorfreigabe im Helfer abgefangen. Navy-Original-SHA-256, alle acht Lockdateien und Version/Buildzähler unverändert. Vorhandene Testziele erhalten, Arbeitsordner aufgeräumt. **0.3.0/30/RC3**, kein Restore, Commit, Push, Paket oder Release. Keine neue vollständige Film-/Tonabnahme; Navy-MP4Box-Erfolg noch nicht behauptet. [Befund, Grenzen und neue Prüffassung](OTR-CAN-ZEITSTEMPEL-VORPRUEFUNG-20261015.md).

## 15.10.2026 – Halbierungssuche für AVI ausgeblendet

Auf Nutzerwunsch ist die gesamte Hauptplayer-Halbierungssuche bei **erkanntem AVI-Container** ausgeblendet, ohne leere Zusatzzeile. Auch AVI unter `.mp4`/`.avi.mp4` fällt darunter. Die Navigation wird beim Laden nicht eingerichtet und der gemeinsame Suchaufruf ist zusätzlich gesperrt; kein später Warnungsdialog wegen fehlender Original-Zeitstempel. Der gelbe Hinweis erklärt die vorläufige Nichtverfügbarkeit. Nach Wechsel zu echtem MP4 erscheint die Suche wieder und bleibt bedienbar. Die Lupe bleibt bei AVI deaktiviert; Schnittmotoren einschließlich der gezielten otr-can-Vorprüfung erhalten.

Geändert: Hauptfenster-XAML/Code, Hauptfenster-ViewModel, AVI-Warnung, zugehörige bestehende Tests und Dokumentation. Release-Rebuild **0 Warnungen/0 Fehler**, **1288/1288 Tests**, 0 übersprungen. Vier echte WPF/mpv-Fälle mit AVI/MP4 und vertauschten Endungen: AVI-Zeile verborgen ohne Platzverbrauch, direkte Aufrufe wirkungslos und Wiedergabe ununterbrochen; MP4-Zeile wieder sichtbar, tatsächlicher Suchsprung und Reset erfolgreich. Drei Fensterbreiten, keine Bindungsfehler. Alle acht Lockdateien und Version/Buildzähler unverändert, **0.3.0/30/RC3**, kein Restore, Commit, Push, Paket oder Release. [Aktueller Startpfad und Nachweis](HALBIERUNGSSUCHE-HAUPTFENSTER-20261015.md).

## 15.10.2026 – OTR-CAN-Schnittaufruf bei ungeeigneten Bildzeiten verhindern

Nach FFMS2-Indexierung werden die Bildzeiten der tatsächlichen Schnittquelle geprüft, bevor `cut-can` startet. Doppelte, rückläufige oder ungültige Werte sperren den Motor mit verständlichem Hinweis auf MP4Box und gesonderte Ergebnisprüfung. Gültige AVI bleiben zugelassen; dieselbe Schutzprüfung gilt für MP4. Kein automatischer Motorwechsel, keine neue Indexierung oder Neukodierung, keine Änderung am Motor oder MP4Box.

Release-Rebuild **0 Warnungen/0 Fehler**, **1288/1288 Tests**, 0 übersprungen. Fünf reale Zeitcodevergleiche sowie zwei Fälle im Produktionsdienst mit echtem FFMS2-Indexer: No Turning Back blockiert ohne Motoraufruf, Teufelsbrigade freigegeben (Motor im positiven Prüfhelferfall bewusst abgefangen, kein neuer Filmschnitt). Arbeitsordner aufgeräumt, vorhandene Testziele unverändert. Alle acht Lockdateien und Version/Buildzähler erhalten, **0.3.0/30/RC3**; kein Restore, Commit, Push, Paket oder Release. [Umsetzung, Grenzen und separate Prüffassung](OTR-CAN-ZEITSTEMPEL-VORPRUEFUNG-20261015.md).

## 15.10.2026 – AVI-Lupe vorläufig gesperrt; otr-can-AVI persönlich abgenommen

Auf Nutzerauftrag wird die Frame-Lupe bei erkanntem AVI-Container für Button und Shift+F gesperrt, vor Pause oder Dialoginitialisierung. Gelber Hinweis und deaktivierter Button-Tooltip: **„Die Frame-Lupe steht für AVI-Container derzeit nicht zur Verfügung.“** Container statt Dateiendung; MP4 nach Medienwechsel wieder verfügbar. Widersprüchliche Verweise auf die AVI-Lupe entfernt. MP4Box, otr-can und Framezuordnung unverändert.

Separater Release-Rebuild **0 Warnungen/0 Fehler**, **1253/1253 Tests**, 0 übersprungen. Vier native Medien-/Containerwechsel-Fälle einschließlich irreführender Endungen; AVI-Öffnungsweg und Button starten keinen Dialog und pausieren nicht, Tooltips verfügbar, Hinweis in drei Fensterbreiten umgebrochen, keine WPF-Bindungsfehler. Alle acht Lockdateien, Version und Buildzähler erhalten. **0.3.0/30/RC3**, kein Restore, Commit, Push, Paket oder Release. [Änderungen, Nachweise und separater Startpfad](AVI-FRAME-LUPE-SPERRE-20261015.md).

Neues Nutzerprotokoll bestätigt erfolgreichen otr-can-0.1.5-Schnitt der **ursprünglichen Teufelsbrigade-AVI**, einschließlich verlustfreier Vorbereitung: 263694 Quellvideopakete, 182574 Ausgabevideopakete, 350542080 Audiosamples geprüft. Anschließende persönliche Rückmeldung: **Ton, Bild und Übergänge geprüft und einwandfrei**. Der zuvor gemeldete MP4Box-Lauf war technisch erfolgreich. Dieser positive AVI-Fall bleibt vom problematischen **No Turning Back** getrennt; keine allgemeine AVI-Reparatur nachgewiesen.
## 15.10.2026 – AVI-Ursachenanalyse und persönliche Halbierungssuche-Abnahme

Der Nutzer bestätigt die Hauptfenster-Halbierungssuche als prächtig funktionierend, einschließlich aller Richtungen; größere Bilder werden als langsam beschrieben. Persönliche Funktionsabnahme positiv festgehalten, keine eigene Laufzeitmessung oder Bestätigung jedes vorgeschlagenen Einzeltests daraus abgeleitet. [Abnahmebericht](HALBIERUNGSSUCHE-HAUPTFENSTER-20261015.md).

Separater reiner Untersuchungsauftrag **No Turning Back**: beide Fehler an derselben gehashten Aufnahme reproduziert. **1149 PTS-Rücksprünge/FFMS2-Doppelzeiten**, auch nach regulärem Umpacken unverändert; vorhandene technisch erfolgreiche MP4Box-Ausgabe weiterhin mit **978 Doppelzeiten**. Kein Reparatur- oder Bild-/Ton-/Übergangsnachweis. H.264-Picture-Timing-SEI als Auslöser eingegrenzt: kurze kontrollierte SEI-Entfernung auf einer Diagnosekopie ergibt eindeutige Zeiten bei **598 identischen Bildhashes**, aber keine freigegebene Reparatur. Teufelsbrigade-AVI und MP4-Referenz bleiben im realen Lupenvergleich nutzbar. [Diagnose, Identitätsgrenzen und sichere Folgeoptionen](AVI-ZEITSTEMPEL-NO-TURNING-BACK-20261015.md).

Unveränderter Produktstand separat Release gebaut: **0 Warnungen/0 Fehler**, **1244/1244 Tests**, 0 übersprungen; drei reale Lupenmodell-Fälle. Original-SHA-256, acht Lockdateien, bestehender Produktdiff und Halbierungssuche erhalten. Nur Dokumentation/ignorierte Diagnosehilfen ergänzt, keine Motor-/Framezuordnungsänderung und keine Prüfung abgeschaltet. Version/Buildzähler weiter **0.3.0/30/RC3**, kein Restore, Commit, Push, Paket oder Release.

## 15.10.2026 – Halbierungssuche im Hauptfenster zur persönlichen Prüfung

Kompakte zweite Playerzeile **Halbierungssuche · ← Frames / Frames → / Zurücksetzen** ergänzt. Bestehende Core-Halbierungslogik und Lupen-Frameanalyse/Vorschau wiederverwendet; Start an der erreichten Playerposition, unabhängiger Suchzustand, neue Suche nach bewusster manueller Navigation. Obere Bedienzeile, Keyframe-Weg, Schnittmotoren und Frame-Lupe erhalten. Kein Schnittpunkt wird durch die Suche verändert.

Vollständiger separater Release-Rebuild **0 Warnungen/0 Fehler**, **1244/1244 Tests**, 0 übersprungen (29 neue Fälle), **30 native Integrationsfälle** einschließlich echter MP4-/VFR-Zielbilder, drei AVI-Schutzfälle und zwölf Layout-/Vergrößerungsfälle; keine Bindungsfehler. Native Prüfung deckte verspätete Positionshinweise und eine falsche AVI-50-Bildzuordnung mit rekonstruierten Zeiten auf. Korrektur und Regressionstests: bewusste Navigation explizit melden; für die neue Hauptplayersuche ausschließlich Original-Zeitstempel akzeptieren. AVI ohne diese Informationen verweist vor einer Positionsänderung auf die bisherige unveränderte Lupe.

[Bedienung, Prüfungen, Grenzen und Startpfad](HALBIERUNGSSUCHE-HAUPTFENSTER-20261015.md). Persönliche HD-/Workflow-Abnahme offen. **0.3.0/30/RC3** und Buildzähler unverändert; alle acht Lockdateien bytegleich, vier vorbestehende Produktionsänderungen erhalten. Kein Restore, Commit, Push, Paketbau oder Release; veröffentlichter Build 30 bleibt eingefroren.

## 14.10.2026 – Build 30 RC3 öffentlich eingefroren

**0.3.0 · Build 30 · RC3** mit unverändertem **otr-can 0.1.5** auf Nutzerauftrag gebaut und [veröffentlicht](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build30-rc3). Quellcommit **57878bd90b3e044218f8f157b6922552a170f1d4**, annotierter Tag **v0.3.0-build30-rc3** auf genau diesem Stand. Setup, Portable, CAN-/Motor-/libmpv-Quellen/Relink und Prüfsummen: sieben Dateien vor/nach Freigabe mit GitHub-Digests geprüft, lokal eingefroren/schreibgeschützt.

Isolierter Locked Restore, Release-Build **0 Warnungen/0 Fehler**, **1215/1215 Tests**, 0 übersprungen. **249 Portable-Dateien**, **527 Quellen-ZIP-Dateien**, 18 native Integrationsprüfungen gegen das entpackte Paket sowie K-Anzeige/Legende erfolgreich. Neue Setup-/VM-Abnahme nicht behauptet. Versionszähler nach erfolgreicher Paketprüfung von 29 auf **30** gesetzt; öffentliche Build-29-/Build-16-Ausgaben und lokale Build-29-Pakete unverändert. Alle acht Lockdateien bytegleich, vier vorbestehende Produktionsänderungen nicht committet. Keine Motor-/Abhängigkeitsänderung. [Paketprüfbericht](RELEASE-BUILD30-RC3-RESULT.md), [Veröffentlichungsnachweis](RELEASE-PUBLICATION-BUILD30-RC3.md), [Übergabetext](UEBERGABE-BUILD30-RC3-20261014.txt). Keine weitere Funktionsänderung für diesen eingefrorenen Stand.
## 14.10.2026 – Build 30 RC3 zum Einfrieren und Veröffentlichen freigegeben

Der Nutzer gibt den aktuellen Stand ausdrücklich für Setup, Portable-ZIP, Quellen, Prüfsummen, Dokumentation, Commit, Push und Veröffentlichung frei. Neuer Paketsatz **0.3.0 · Build 30 · RC3**, Tag **v0.3.0-build30-rc3**; bisherige Releases einschließlich Build 29 bleiben unverändert. **otr-can 0.1.5** und der geprüfte Vier-DLL-libmpv-Satz werden unverändert wiederverwendet.

Enthalten sind Prioritäten 1–4 mit Filmregel, Untersuchungsmodus und ausdrücklich ausgenommener AVI-Keyframe-Navigation, außerdem K vor dem Schrägstrich und **„Blaue Bereiche: Keyframes“**. Die persönliche Abschlussrückmeldung bestätigt den Gesamtstand; keine frische Setup-/VM-Abnahme oder erneuten realen Schnittauftrag daraus abgeleitet. Letzter Entwicklungsnachweis: **1215/1215 Tests**, 0 Warnungen/Fehler. Paketprüfung folgt aus festem Quellcommit in isoliertem Export. Vier vorbestehende Produktions-Lockänderungen bleiben bytegleich und aus dem Commit ausgeschlossen. [Paketprüfbericht](RELEASE-BUILD30-RC3-RESULT.md).
## 14.10.2026 – Hinweistext „Blaue Bereiche: Keyframes“

Auf Nutzerwunsch im Keyframe-Status/Tooltip **„Blaue Striche: Keyframes“** durch **„Blaue Bereiche: Keyframes“** ersetzt. Anleitung und aktueller Prüfbericht angepasst. Zeichenlogik, K-Position vor dem Schrägstrich, Navigation und AVI-Ausschluss unverändert.

Separater vollständiger Release-Rebuild **0 Warnungen/0 Fehler**, **1215/1215 Tests**, keine übersprungen. Neue Prüffassung unter `.build/keyframe-label-20261014/compiled/bin/CutAssistantNext.App/release`; [Startpfad](KEYFRAME-NAVIGATION-20261014.md). Kein Restore, Version/Zähler 0.3.0/29/RC3 und vier vorbestehende Lockänderungen erhalten. Keine Commits, Pushes oder Veröffentlichung.

## 14.10.2026 – K-Kennzeichen direkt hinter aktueller Framenummer

Auf Nutzerwunsch steht das blaue **(K)** jetzt vor dem Schrägstrich: **Frame 36.206 (K) / ca. 210.000**. Aktuelle Framenummer und Gesamtzahl werden getrennt angezeigt und bei Frameänderungen gemeinsam aktualisiert. Keyframe-Erkennung, Navigation und AVI-Ausschluss unverändert.

Separater vollständiger Release-Rebuild **0 Warnungen/0 Fehler**, **1215/1215 Tests**, keine übersprungen. Kompilierte WPF-Anzeige mit K, ohne K und unbekannten Framezahlen geprüft; tatsächliche Text-/Geometriereihenfolge korrekt, AVI weiterhin ohne K, keine Bindungsfehler. [Nachtrag und neuer Startpfad](KEYFRAME-NAVIGATION-20261014.md). Persönliche Prüfung offen. Weiterhin **0.3.0 · Build 29 · RC3**, vier vorbestehende Lockänderungen erhalten; kein Restore, Commit, Push oder Release.

## 14.10.2026 – Keyframe-Navigation zur Prüfung, AVI ausdrücklich ausgenommen

Priorität 4 mit kompakten **◀ K / K ▶**, blauen Strichen oben im bestehenden Schnittstreifen und pausierter **(K)**-Anzeige umgesetzt. Rote Entfernbereiche und Klickverhalten erhalten. Hintergrundindex aus ausdrücklich gekennzeichneten Keyframes mit Original-PTS; bei langen Aufnahmen bis zum Abschluss deaktivierte Sprungbuttons mit Status im Tooltip. Keine zusätzliche feste Bedienzeile.

**Anschließende ausdrückliche Nutzerentscheidung:** Bei AVI keine Keyframe-Analyse, -Buttons, -Marker oder `(K)`. Erklärung im bestehenden gelben AVI-Infoblock, bisherige AVI-Frame-Lupe erhalten. Experimentelle AVI-Player-/Ursprungsanpassungen vollständig entfernt; produktiver Playback-Service unverändert.

Separater Release-Rebuild **0 Warnungen/0 Fehler**, **1215/1215 Tests**, keine übersprungen (47 zusätzliche Fälle). **18 Integrationsprüfungen** mit tatsächlichen mpv-Bildern/K-Buttons, AVI-Ausschluss und intakter AVI-Lupe, drei Fenstergrößen, roten/blauen Pixeln und Abbruch; keine Bindungsfehler. Lange HD-MP4 zusätzlich vollständig analysiert: **3448 Keyframes in 26,6 s**. [Prüfbericht, Grenzen und Startpfad](KEYFRAME-NAVIGATION-20261014.md).

Persönliche Prüfung offen. Kein Restore, weiter **0.3.0 · Build 29 · RC3**, vier vorbestehende Produktions-Lockänderungen erhalten, Prioritäten 1–3 enthalten. Keine Motor-/Namensmasken-/Releaseänderung, Commits oder Pushes.

## 14.10.2026 – Untersuchungsmodus der Frame-Lupe persönlich abgenommen

Nutzer bestätigt die Priorität-3-Prüffassung mit **„check funktioniert einwandfrei :-)“**. Die gemeldete persönliche Funktionsabnahme der Frame-Lupe ohne Schnittkante ist damit positiv abgeschlossen. Keine gesonderte Bestätigung jedes vorgeschlagenen Einzeltests oder aller Medienformate daraus abgeleitet. [Prüfbericht und Abnahme](FRAME-LUPE-UNTERSUCHUNGSMODUS-20261014.md).

Nur Dokumentation aktualisiert, kein erneuter Build/Testlauf. Letzter technischer Stand **1168/1168 Tests**, **46 Integrationsprüfungen**, Release-Rebuild ohne Warnungen/Fehler. Code, Lockdateien, Version und öffentliche Veröffentlichung unverändert; keine Commits oder Pushes. Priorität 4 bleibt zurückgestellt.

## 14.10.2026 – Frame-Lupe ohne Schnittkante zur persönlichen Prüfung

Priorität 3 ausdrücklich freigegeben und umgesetzt: **Frame-Lupe** und **Shift+F** öffnen ohne ausgewählte Kante den **Untersuchungsmodus** an der vom Hauptplayer gemeldeten Position nach dem Pausieren. Leere Liste, abgewählte Bereiche, reine Zeilenauswahl und vorgemerkter Anfang werden unterstützt. Bestehende Analyse/Vorschau/Navigation gemeinsam verwendet; keine künstliche Kante. **Schnittkante übernehmen** dauerhaft gesperrt und zusätzlich im Dialog abgesichert. Mit ausgewählter Kante bleibt die bisherige Bearbeitung erhalten.

Kein Restore, separater vollständiger Release-Rebuild **0 Warnungen/0 Fehler**, **1168/1168 Tests**, keine übersprungen (34 zusätzliche Fälle). **46 Integrationsprüfungen** mit echten nummerierten MP4-/AVI-Bildern, kompilierten Dialogen in Normal-/Kleinansicht, wirkungslosem Untersuchungs-Übernahmeklick, erfolgreicher Kantenübernahme und Hauptfenster-Öffnungen bei simuliertem erreichtem Playerstand/abweichendem Sliderziel. Keine Bindungsfehler. [Prüfbericht, Grenzen und Startpfad](FRAME-LUPE-UNTERSUCHUNGSMODUS-20261014.md).

Persönliche Prüfung offen. Lokale Fassung weiter **0.3.0 · Build 29 · RC3**, vier Produktions-Lockänderungen bytegleich, Prioritäten 1–2 erhalten. Keine Motor-/Namensmasken-/Releaseänderung, Commits oder Pushes. Priorität 4 bleibt zurückgestellt.

## 14.10.2026 – Reiner Filmname in allen drei Namensdialogen bestätigt

Nutzer bestätigt mit **„cutlist erzeugen check“** und Screenshot auch die Vorschau im Dialog **Cutlist erzeugen**: Staffel, Folge und Folgentitel leer, Vorschau ausschließlich **Die Hart 2**. Zusammen mit den vorigen MP4Box-/otr-can-Rückmeldungen ist die Filmnamenanzeige in allen drei Dialogen persönlich positiv geprüft. Weitere persönliche Prüfungen und Gesamtabschluss bleiben offen; das Bild zeigt noch keinen Speichervorgang.

Nur Dokumentation aktualisiert, Diffprüfung erfolgreich; letzter technischer Stand **1134/1134 Tests**, Release-Rebuild ohne Warnungen/Fehler. Kein neuer Build/Testlauf. Code, Lockdateien, Version und öffentliche Release-Dateien unverändert.

## 14.10.2026 – Reiner Filmname auch im otr-can-Dialog bestätigt

Nutzer bestätigt per Screenshot und **„otr-can check, teste weiter“**: Im Dialog **Schneiden – otr-can** sind die drei Serienfelder leer; der Ausgabedateiname lautet ausschließlich **Die Hart 2**. Damit ist die persönliche Namensprüfung in beiden Schneidedialogen positiv dokumentiert. Weitere persönliche Tests laufen; der Gesamtabschluss bleibt offen.

Nur Dokumentation geändert, Diffprüfung erfolgreich; letzter technischer Stand **1134/1134 Tests**, Release-Rebuild ohne Warnungen/Fehler. Kein neuer Build/Testlauf und keine Code-, Lockdatei-, Versions- oder Releaseänderung.

## 14.10.2026 – Filmname nach Neuerzeugen im MP4Box-Dialog bestätigt

Nutzer bestätigt per Screenshot: Im Dialog **Schneiden – MP4Box** sind Staffel, Folge und Folgentitel leer; nach **Aus obigen Eingaben neu erzeugen** steht als Ausgabedateiname ausschließlich **Die Hart 2**. Erfolgreiche persönliche Teilprüfung der korrigierten Filmregel. Weitere persönliche Tests laufen; die Gesamtprüfung bleibt offen. Daraus wird kein bereits ausgeführter Schnitt oder geprüfter Dateiname auf dem Datenträger abgeleitet.

Nur Dokumentation aktualisiert, kein neuer Build/Testlauf. Letzter technischer Stand: **1134/1134 Tests**, Release-Rebuild ohne Warnungen/Fehler. Prioritäten 3–4 bleiben zurückgestellt.

## 14.10.2026 – Filmregel präzisiert und Screenshot-Fall korrigiert

Nutzer meldet **Die Hart 2- [12.10.2026]** trotz leerer Serienfelder und stellt ausdrücklich klar: **Alles hinter dem Filmnamen muss weg**, wenn Staffel, Folge und Folgentitel leer sind. Damit ersetzt die Klarstellung die vorherige Annahme, Zusatzfelder wie Datum bei Filmen beizubehalten. Der zentrale Renderer beendet die automatische Filmmaske nun am Ende des `%Name%`-Bausteins; bei Serienangaben bleibt die vollständige Maske aktiv. Gespeicherte Masken, manuelle Ausgabenamen und geschützte Vorschläge bleiben erhalten; bewusste Neuerzeugung verwendet die neue Regel.

Bildfall vorab exakt als Fehler reproduziert, dann korrigiert. Vollständiger separater Release-Rebuild **0 Warnungen/0 Fehler**, **1134/1134 Tests**, keine übersprungen. Gespeicherte Bildmaske, Cutlist-Datei und beide Schnittdienste mit simulierten Werkzeugen geprüft. **Echter WPF-Cutlist-Dialog** bei 720×720 und 600×400: tatsächlicher Neuerzeugen-Klick → **Die Hart 2**, Serienvorschau weiterhin vollständig, zurückgeleerte Serienfelder wieder nur Titel; keine Bindungsfehler. [Aktuelle Regel, Prüfbild und Startpfad](NAMENSMASKEN-LEERE-SERIENFELDER-20261014.md). Persönliche Wiederholungsprüfung noch offen.

Lokale Prüffassung weiterhin **0.3.0 · Build 29 · RC3**. Vier Produktions-Lockänderungen bytegleich, öffentliche Pakete/Tags/Prüfsummen und Motoren unverändert. Keine neue Abhängigkeit, Commits, Pushes oder Veröffentlichungen. Prioritäten 3–4 bleiben zurückgestellt.

## 14.10.2026 – Filmnamen ohne leere Serientrennzeichen zur persönlichen Prüfung

Priorität 2 ausdrücklich freigegeben und zentral im Core-`NameTemplateRenderer` umgesetzt: Sind **Staffel, Folge und Folgentitel sämtlich leer** (auch Whitespace), entfallen leere Serienbausteine nach dem Name-Baustein samt ihren Trennzeichen. **Ghostbusters - - → Ghostbusters**, vollständige Serie weiterhin **Kimi - S01E03 - Der Besuch**. Zusätzliche Datums-, Sender-, Originalnamensfelder und sinnvoller Literaltext bleiben erhalten; gespeicherte Masken, manuelle Ausgabenamen und geschützte Cutlist-Vorschläge werden nicht umgeschrieben. Teilweise ausgefüllte Serienfelder behalten das bisherige Verhalten.

Separater vollständiger Release-Rebuild **0 Warnungen/0 Fehler**, **1127/1127 Tests**, keine übersprungen (51 zusätzliche Fälle). Cutlist-Dateien tatsächlich gespeichert/wieder gelesen und beide Schnittdienste mit simulierten Werkzeugen ausgeführt, einschließlich exakter Dateiendung und manueller Namen. Kein erneuter echter Videoschnitt behauptet. [Regel, Prüfungen und Startpfad](NAMENSMASKEN-LEERE-SERIENFELDER-20261014.md). Persönliche Abnahme noch offen.

Weiterhin lokale Prüffassung **0.3.0 · Build 29 · RC3**, vier Produktions-Lockänderungen bytegleich erhalten; öffentliche Pakete, MP4Box und otr-can 0.1.5 unverändert. Keine neue Abhängigkeit, Versionserhöhung, Commits, Pushes oder Veröffentlichungen. Abgenommener Lupenbutton und Strg-Klick-Tooltips enthalten; Prioritäten 3–4 bleiben zurückgestellt.

## 14.10.2026 – Lupenbutton persönlich bestätigt, Strg-Klick-Hinweise ergänzt

Nutzer bestätigt Darstellung und Funktion des neuen Lupenbuttons als sehr gut, einschließlich Hinweis ohne angeklickte Schnittkante und Tooltip mit Shift+F. Persönliche Abnahme des Lupenbuttons damit positiv festgehalten; keine zusätzliche getrennte Skalierungs- oder Tastaturprüfung daraus abgeleitet.

Auf ausdrücklichen Folgeauftrag in `MainWindow.xaml` zwei Tooltips ergänzt: **−20 Bilder (Strg+Klick)** bei **−10 Bilder**, **+20 Bilder (Strg+Klick)** bei **+10 Bilder**. Vorhandene Strg-Klick-Funktion und dynamische Buttonbeschriftung unverändert. XAML und Click-Zuordnungen geprüft, Formatierung/Diffprüfung erfolgreich; separater Release-Rebuild **0 Warnungen/0 Fehler**, **1076/1076 Tests**, keine übersprungen. Persönliche Tooltip-Abnahme am 14.10.2026 mit „funktioniert tip top :-)“ bestätigt; [Startpfad und Nachtrag](FRAME-LUPENBUTTON-20261014.md).

Weiterhin **0.3.0 · Build 29 · RC3**, vier Produktions-Lockänderungen bytegleich erhalten, öffentliche Release-Pakete und Motoren unverändert. Keine Commits, Pushes oder Veröffentlichungen; Prioritäten 2–4 warten weiterhin auf gesonderte Freigabe.

## 14.10.2026 – Sichtbarer Frame-Lupenbutton zur persönlichen Prüfung

Ausschließlich Priorität 1 der Shoutbox-Rückmeldungen umgesetzt: **🔍 Frame-Lupe** in der Schnittliste unmittelbar nach dem vollständigen Anzeigebereich **Vorgemerkter Anfang**, mit Tooltip **Frame-Lupe öffnen (Shift+F)**. Fester Zeitbereich von 150 WPF-Einheiten für Gesamtstunden bis zur `TimeSpan`-Grenze; Button und Anzeige bleiben zusammen. Die vier Schnittbuttons und das vorhandene Umbruchverhalten bleiben erhalten, keine zusätzliche feste Layoutzeile. Button und unverändertes Shift+F verwenden dieselbe Öffnungsroutine samt Kanten-/Medien-/Ladeschutz.

Lokale Prüffassung **0.3.0 · Build 29 · RC3** in separatem Ausgabeordner: Release-Rebuild **0 Warnungen/0 Fehler**, **1076/1076 Tests**, keine übersprungen. **72 WPF-Layoutfälle** bei drei Zeilenbreiten und 100/125/150/200 Prozent Skalierung; Zeit bis zu neunstelligen Gesamtstunden vollständig, Button-/Schnittbuttonpositionen beim Wertwechsel stabil. Öffnungsprüfung ohne Kante und mit simulierter gültiger Kante für Button und Shift+F-Zielroutine erfolgreich, Ladesperre und unveränderte Schnittwerte bestätigt, keine Bindungsfehler. [Prüfbericht und Startpfad](FRAME-LUPENBUTTON-20261014.md).

Persönliche Prüfung mit echter Aufnahme noch offen. Die vier vorbestehenden Produktions-Lockänderungen bytegleich erhalten; Version, Zähler 29, eingefrorene öffentliche Pakete und Motoren unverändert. Keine Commits, Pushes oder Veröffentlichungen. Prioritäten 2–4 warten auf gesonderte Freigabe.

## 13.10.2026 – 0.3.0 Build 29 RC3 öffentlich veröffentlicht

Nach positiver Produktiv-/VM-/Portable-Abnahme auf Nutzerauftrag als [öffentliche Vorabversion](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.3.0-build29-rc3) freigegeben. Annotierter Tag am gebauten Quellcommit **ce4bc15b118d8080a78d7db5396db3ccf830ca89**, unveränderter **otr-can 0.1.5**. Setup, Portable, CAN-/Motorquellen, libmpv-Prüf-/Relink-Pakete und SHA256SUMS: **sieben unveränderte Uploads** im Entwurf und nach Freigabe anhand von GitHub-Größen/SHA-256-Digests geprüft. Release-Text und Remote-Quelltag stimmen. [Veröffentlichungsnachweis](RELEASE-PUBLICATION-BUILD29-RC3.md).

Alte Build-16-Veröffentlichung, alle acht lokalen Lockdateien und sieben eingefrorenen Dateien per Vergleich/Hash erhalten. Vier Produktions-Lockänderungen bleiben uncommittet. Buildzähler 29, RC3 und bekannte AVI-/MP4Box-Grenzen unverändert. Veröffentlichung und Dokumentation ohne neuen Build/Testlauf oder Funktionsänderung; letzter Paketprüfstand **1076/1076 CAN-Tests**, Release ohne Warnungen/Fehler. Abschlussdokumentation separat auf origin/main gesichert; Quelltag bleibt fest.

## 13.10.2026 – VM-Setup und Portable mit beiden Motoren bestätigt

Nutzer bestätigt für **0.3.0 · Build 29 · RC3** in der VM: **Setup-Installation**, **Grey’s Anatomy / Durchs Feuer (MP4)** mit MP4Box und otr-can sowie **Deinstallation** jeweils „check“. Anschließend **Portable-Appstart**, **Pfade einstellen** und **Die Teufelsbrigade (AVI)** mit MP4Box und otr-can ebenfalls „check“. Gemeldete praktische Setup-/VM-/Portable-Paketabnahme damit positiv abgeschlossen; Produktivabnahme zusätzlich erhalten. [Paketprüfbericht](RELEASE-BUILD29-RC3-RESULT.md).

Keine zusätzlichen nativen Messwerte oder ausdrücklich getrennten Bild-/Ton-/Tonspurkontrollen in dieser Nachricht. Installationstyp und frisches Profil nicht gesondert unterschieden. Eingefrorener Quell-/Binärsatz, Prüfsummen und Build 29 unverändert; vier lokale Lockänderungen erhalten. Nur Abnahmedokumentation, kein neuer Build, Paket oder GitHub-Release.

## 13.10.2026 – Produktivinstallation und Schnittprüfung bestätigt

Nutzer bestätigt **Produktivinstallation des Build-29-Paketsatzes** und anschließenden **Schnitt** als „check“. Cutlist-Bild: **Torchwood / Sünden der Vergangenheit / S4E7** mit MP4Box/GPAC 26.07, Werbung vollständig entfernt; **Torchwood / Befreiungsaktion / S4E6** mit otr-can 0.1.5 erfolgreich nach genauer Lupenbestimmung des Übergangs. MP4Box-Doppelszene bei **15:05** im S4E6-Fall separat dokumentiert. [Paketabnahme](RELEASE-BUILD29-RC3-RESULT.md).

Keine frische Installation/Überinstallation eigens unterschieden und keine neuen nativen Messwerte oder getrennte Torchwood-Tonspurprüfung behauptet. Separate Portable-/VM-/Deinstallationsprüfung noch offen. Nur Abnahmedokumentation ergänzt; **0.3.0 · Build 29 · RC3**, feste Quellen/Pakete/Prüfsummen und vier lokale Lockänderungen erhalten. Letzter technischer Stand 1076/1076 Tests, kein neuer Build oder Motorumbau.

## 13.10.2026 – Build 29 RC3 gesichert und als Paketsatz eingefroren

CAN-Quellcommit **ce4bc15b118d8080a78d7db5396db3ccf830ca89** auf `origin/main` gesichert. Aus genau diesem Commit ohne Arbeitsbaumüberlagerung gebaut: **0.3.0 · Build 29 · RC3**, unveränderter **otr-can 0.1.5**. Setup, Portable-ZIP, CAN-/Motorquellen, libmpv-Quellen-/Relink-Archive und Prüfsummen fertig. Sechs Archive plus Prüfsummen schreibgeschützt; Quellcommit und Artefakte bleiben fest, Ergebnisdokumentation wird separat gesichert.

Gesperrter Windows-x64-Restore, Release/Publish und Inno-Kompilierung erfolgreich, **0 Warnungen/0 Fehler**, **1076/1076 Tests** aus eingefrorenen Quellen, keine übersprungen. **249 Portable-Dateien**, **509 Quellen-ZIP-Dateien**, alle Artefakthashes, App-/Kandidaten-/Motorversionen und Herkunft geprüft. Tatsächliche WPF-Versionsanzeige und sechs Masken-/Cutlist-Prüfungen mit entpacktem Paketcode erfolgreich. Alle acht lokalen Lockdateien, persönliche Einstellungen und bisherige Build-16-Artefakte erhalten; vier vorbestehende Produktions-Lockänderungen weiterhin außerhalb des Commits. [Paketprüfbericht](RELEASE-BUILD29-RC3-RESULT.md).

Neue persönliche Setup-/VM-/Portable-Paketabnahme noch offen; bisherige Entwicklungsabnahmen und bekannte AVI-/MP4Box-Grenzen gelten weiter. Kein neues GitHub-Release, öffentliche Build-16-Ausgabe unverändert. Nächster Schritt: die fertigen Pakete persönlich installieren/entpacken und prüfen.

## 13.10.2026 – Build 29 RC3 zum Einfrieren und Paketbau freigegeben

Nutzer beauftragt **Commit, Push, Setup, ZIP und zugehörige Artefakte**, danach aktuellen Stand einfrieren. CAN **0.3.0 · Build 29 · RC3** mit unverändertem **otr-can 0.1.5** festhalten; Quellen aus festem Commit exportieren, vollständiger Paketbau im exportierten Baum. Vier lokale Produktions-Lockänderungen erhalten und vom Commit/Snapshot ausschließen. Gesperrten Restore mit Version 0.3.0 und Original-Lockdateien vorab erfolgreich geprüft. Paketvoraussetzungen um vorhandene FFmpeg-Versionsregel ergänzt; kein Funktionsumbau. [Freeze-Auftrag und Prüfplan](FREEZE-BUILD29-RC3-20261013.md).

Letzter Entwicklungsprüfstand: **1076/1076 Tests**, Release ohne Warnungen/Fehler, persönliche Maskenwechsel-/Bedienabnahme bestätigt. Neue Installations-/Portable-Prüfung folgt nach dem Paketbau; öffentliche Build-16-Ausgabe bleibt unverändert.

## 13.10.2026 – Gespeicherte Namensmaske ohne Neustart anwenden

Gemeldeten Fall S01E01 → 01x01 bei schon geladener Datei korrigiert: im Zustandspfad des Cutlist-Dialogs aktuelle gespeicherte Standardmaske zusätzlich weitergeben. Vorschau und geschützte Namensvorschläge zunächst erhalten; **Aus obigen Eingaben neu erzeugen** verwendet die aktuelle Maske und aktualisiert den weitergegebenen Namenszustand. Schnittdialog übernimmt nach Neuerzeugen ebenfalls die tatsächlich verwendete Maske. Name, Staffel, Folge, Folgentitel und Datum bleiben erhalten; kein Neustart oder erneutes Videoladen nötig.

Lokaler **0.3.0 · Build 29 · RC3**, **0 Warnungen/0 Fehler**, **1076/1076 Tests bestanden**, keine übersprungen; sieben neue Workflow-Fälle. **Sechs echte WPF-/Speicherprüfungen**: Speichern/Neuerzeugen, beide Maskenrichtungen, normale/kleine Cutlist-Ansicht und Cutlist-Wiederladen in einer Sitzung. Keine Bindungsfehler, Prüfbilder gesichtet, nur isolierte Einstellungen geschrieben. Alle acht Lockdateien und persönliche Einstellungen unverändert. [Nachweis und Grenzen](NAMENSMASKE-OHNE-NEUSTART-20261013.md).

Persönliche Wiederholung am 13.10.2026 positiv bestätigt: S13E07 → 13x07, neue ungewöhnliche Maske S01xxE01 speichern, bisherigen Vorschlag zunächst erhalten und erst nach Neuerzeugen S13xxE07 verwenden – ohne Neustart. Fünf Nutzerbilder und „check“ dokumentiert. Vier App-Produktionsdateien plus Tests/Dokumentation geändert; keine Motor-/Parser-/Zeitberechnungsänderung oder neue Abhängigkeit. Vorherige Nutzeränderungen und Abnahmen erhalten. Kein Commit, Push, Paket oder Release.

## 13.10.2026 – Chicago Fire: Bild, Ton und alle Übergänge bestätigt

Nutzerbild der eigenen Server-Cutlist ergänzt den Chicago-Fire-/Unantastbar-Vergleich: **MP4Box-Werbeblitzer bei 00:00.00 auch nach Lupenkorrektur**, **otr-can 0.1.5 blitzerfrei**, **Bild, Ton und alle Schnittübergänge geprüft**. Persönliche Abnahme damit ausdrücklich um Ton und sämtliche Übergänge erweitert. Keine einzelne Tonspurprüfung oder neue technische Messung behauptet. [Nachweis](BEDIENABNAHME-VORSCHAUHINWEIS-20261013.md). Nur Dokumentation und lokale Bildsicherung; Stand **0.3.0 · Build 28 · RC3**, letzter technischer Prüfstand 1069/1069 Tests unverändert. Kein Motorumbau, Serverzugriff, neuer Build, Commit, Push oder Release.

## 13.10.2026 – Bedienung bestätigt, Beispielhinweis ergänzt

Nutzer bestätigt **FFmpeg-Versioncheck**, **Richtungswechsel-Halbsprung** und **alternative Folgennomenklatur**; Essentials-Git-Build vom 12.10.2026 verwendet. Ausgabevorschau „Chicago Fire 13x07 Unantastbar [12.10.2026]“ persönlich positiv geprüft. In `NamingSettingsDialog.xaml` ausschließlich Vorschauüberschrift ergänzt: **„Vorschau (Beispiel: Staffel 1, Episode 1 – von Hand eingetragen)“**. Staffel/Folge bleiben ausdrücklich Beispieldaten, keine Parser-/Maskenänderung.

Lokaler **0.3.0 · Build 28 · RC3**, **0 Warnungen/0 Fehler**, **1069/1069 Tests**, keine übersprungen. Zunächst separater vollständiger Build während des otr-can-Laufs, nach „can geschlossen“ derselbe Build 28 im üblichen Ordner erstellt und geprüft. Zwei echte WPF-Ansichten: Hinweis vollständig, Vorschau unverändert, keine Bindungsfehler, keine persönlichen Einstellungen gespeichert. Alle acht Lockdateien bytegleich erhalten. [Nachweis](BEDIENABNAHME-VORSCHAUHINWEIS-20261013.md).

**Chicago Fire / Unantastbar:** MP4Box-Protokoll mit einem Bereich 724.84:3100.06 endet erfolgreich, Nutzer meldet Blitzer; Senderblende im Startbild sichtbar. Neues otr-can-Ausgabebild mit NBC-Intro vom Nutzer als **„Alles prima :-)“** bestätigt. Erneut eingefügter Text weiterhin MP4Box-Protokoll; keine zusätzlichen nativen otr-can-Messwerte behauptet. MP4Box-Bildbefund dokumentiert, kein Motorumbau. Öffentlicher Build 16 RC3 erhalten; kein Commit, Push, Paket oder Release.

## 13.10.2026 – Richtungsabhängige Lupensuche und FFmpeg-Versionen geprüft

Auf Nutzerauftrag Halbierung nur beim Suchrichtungswechsel umgesetzt, bereits für den Sprung in die Gegenrichtung. Gleiche Richtung behält die Weite; normale Bildschritte, Fehler/Abbruch und bewegungslose Sprünge verbrauchen keine Halbierung. Richtung erst nach erfolgreicher Bildvorschau merken; Neustart setzt Weite und Richtung zurück. Beide Buttons zeigen ihre nächste Weite. Beispiel **+2000 → +2000 → −1000 → −1000 → +500**.

Unter FFmpeg-Werkzeuge **Prüfen** ergänzt, mit gemeinsamer Versionskontrolle auch im vorhandenen OTR-CAN-Prüfweg. Nutzer bestätigt **größer als 5.1 erforderlich, ab 9.0.2 empfohlen**. FFmpeg/ffprobe getrennt prüfen, numerische Releasegrenzen, unbekannte Versionen nicht bestätigen. Moderner Git-Build anhand des Bibliotheksstands erkannt, keine genaue 9.0.2-Patchversion behauptet. Hintergrundprüfung, Prozess-Zeitlimit, Abbruch beim Schließen, scrollbar bei kleinen Fenstern; keine automatische Installation oder Speicherung durch die Prüfung.

Lokaler **0.3.0 · Build 27 · RC3**, **0 Warnungen/0 Fehler**, **1069/1069 CAN-Tests bestanden** (51 zusätzliche), keine übersprungen. **57 echte AVI-/Werkzeug-/WPF-Integrationsfälle** gegen den Abschlussbuild bestanden; nummerierte Bilder, wiederholte Richtung und Gegenrichtung, echtes Prüfen, aktuelle Git-Werkzeuge und OTR-CAN-Vorprüfung, simulierte alte/empfohlene Releases, Fenstergrößen und Abbruch. Keine Bindungsfehler; alle acht Lockdateien und persönliche Einstellungsdateien per Hash erhalten. [Prüfbericht](LUPEN-SUCHRICHTUNG-FFMPEG-20261013.md).

Persönliche Prüfung der neuen Such-/Versionsbedienung noch offen; bisherige positive AVI-/Schnittabnahme von Build 26 erhalten. Keine Änderung an Schnittmotoren oder Player, keine neue Abhängigkeit. Öffentlicher Build 16 RC3 mit otr-can 0.1.5 unverändert; kein Setup, Portable-Paket, Release, Commit oder Push.

## 13.10.2026 – AVI-Frame-Lupe: Vorschau und Kantenübernahme ergänzt

Auf Nutzerauftrag „ok dann avi lupe :-)“ den fehlenden Original-PTS-Fall reproduziert und gezielt ergänzt: nur bei tatsächlich erkanntem AVI einen vorhandenen rekonstruierten Bildzeitstempel für den bestehenden, auf Bild/Zeiten/Stream-Zeitbasis geprüften Vorschauweg verwenden. Original-PTS behalten Vorrang und bleiben im Modell getrennt; Status, Details und Übernahmeprotokoll kennzeichnen den AVI-Weg. Kantenübernahme nach erfolgreicher Vorschau ermöglicht, Abbruch/Fehler-/Mehrdeutigkeits- und Schnittplanprüfungen erhalten. Zwei Produktionsdateien (`FrameLoupeViewModel.cs`, `MainWindow.xaml.cs`) angepasst, keine Motoränderung oder neue Abhängigkeit.

Lokaler **0.3.0 · Build 26 · RC3**, **0 Warnungen/0 Fehler**, **1018/1018 CAN-Tests bestanden**, keine übersprungen; darunter 19 neue Lupenfälle. **32 echte Analyse-/Vorschau-/WPF-Integrationsfälle** mit nummerierten 25-/50-fps-AVI-Varianten, MP4-Regressionsquelle und Original-Teufelsbrigade bestanden. Tatsächliches Bild bei 582,240 s, ±1/±10, halbierte Suche, normale/kleine WPF-Ansicht und Übernahmeklick geprüft; keine Bindungsfehler oder Speicherung persönlicher Einstellungen. [Prüfbericht und Grenzen](AVI-FRAME-LUPE-20261013.md).

Persönliche AVI-Lupenabnahme am 13.10.2026 positiv bestätigt: „Lupe funktioniert bei avi jetzt bestens“, Schnitt der Teufelsbrigade mit MP4Box und otr-can ebenfalls erfolgreich und jeweils ohne Blitzer. Beide übermittelten Protokolle zeigen sechs erfolgreich verarbeitete Behaltebereiche; otr-can 0.1.5 mit einmaliger Indexierung und MP3→AAC-LC. Der gewünschte Suchsprung nur bei Richtungswechsel bleibt eine separate offene Bedienänderung auf der Merkliste. Fehlende rekonstruierte oder mehrdeutige Zeitstempel bleiben gesperrt; keine allgemeine AVI-/Zeitstempelreparatur oder vollständige Gleichheit zu mpv/Schnittmotoren behauptet. Untaugliche alte Aufnahme nicht untersucht. Alle acht Lockdateien bytegleich erhalten; vier vorbestehende lokale Produktionsänderungen bleiben separat. Öffentlicher Build 16 RC3 mit otr-can 0.1.5 und vorheriger Versionswechsel erhalten. Kein Setup, Portable-Paket, Release, Commit oder Push.

## 13.10.2026 – Entwicklungsstand auf 0.3.0 angehoben

Versionswechsel von **0.2.1 · Build 25 · RC3** auf **0.3.0 · Build 25 · RC3** zur Würdigung der abgeschlossenen Erweiterungen: zwei Schnittmotoren, Kontrollplayer, verbesserte Schnittbearbeitung und Bedienung. Nur die zentrale Version in `Version.props` geändert; alle Projekte, Assembly-/Dateiversionen, Fenstertitel, Hauptüberschrift sowie numerische Cutlist-/Upload-Version übernehmen sie automatisch. Buildnummer **25** und Kennung **RC3** bleiben erhalten; README und aktuelle Statusangaben angepasst. Keine funktionalen Änderungen.

Validierung abgeschlossen: Restore ohne Lockdateischreibung erfolgreich; Release-Rebuild mit `-p:CanBuildNumber=25` und `--no-restore` mit **0 Warnungen/0 Fehlern**, **999/999 CAN-Tests bestanden**, keine übersprungen. Alle vier Produktionsassemblies: Assemblyversion **0.3.0.0**, Dateiversion **0.3.0.25**, Informationsversion **0.3.0+build.25**, Kandidat **RC3**. App-EXE-Dateiversion, numerische Cutlist-/Upload-Version **0.3.0** sowie tatsächlicher kompilierter WPF-Fenstertitel und Hauptüberschrift **Cut Assistant Next · 0.3.0 · Build 25 · RC3** geprüft. Hauptfenster nicht angezeigt, keine persönlichen Einstellungen gespeichert. SHA-256 aller acht Lockdateien und des lokalen Buildzählers unverändert. Das reguläre `tools/build.ps1` wurde für diesen Versionswechsel nicht verwendet, da es den Zähler auf 26 erhöhen würde. Kein erneuter Film-/Motor- oder AVI-Lupen-Praxistest; Belege lokal unter `.build/version-030-20261013`.

Die vier vorbestehenden lokalen Produktionsänderungen an `packages.lock.json` in App, Core, Cutlists und Media bleiben außerhalb des Versionswechsels erhalten. Öffentliche Veröffentlichung **0.2.1 · Build 16 · RC3 mit otr-can 0.1.5** unverändert; kein Setup, Portable-Paket, Release, Commit oder Push. **AVI-Frame-Lupe bleibt eine eigenständige Folgeaufgabe.**

## 12.10.2026 – Quellstand zur Sicherung freigegeben

Nutzerauftrag: heute ausschließlich Commit und Push zur Sicherung der fertig geprüften Bedienrunde einschließlich Anfang-/Ende-Kontrolle. Keine weitere Implementierung und kein Paket-/Releasebau. AVI-Frame-Lupe auf **13.10.2026** zurückgestellt.

Gesicherter Arbeitsstand: lokaler Build 25 RC3, 999/999 CAN-Tests, 36 WPF-/Native-Kontrollen sowie persönliche positive Bedien- und Anfang-/Ende-Abnahme. Die vier vorbestehenden Produktions-Lockänderungen bleiben ausdrücklich außerhalb des Commits erhalten. Der künstliche MP4Box-Sonderfall bleibt separat dokumentiert.

## 12.10.2026 – Kontrollplayer: Anfang und Ende ergänzt

Auf Nutzerwunsch Prüfen auch für Anfang und Ende ergänzt, bei einem oder mehreren Behalteabschnitten: Anfang direkt bei 0:00, Ende drei Sekunden vor der tatsächlich geladenen Dauer, kurze Dateien auf 0:00 begrenzt. Automatisches Play nach Seek; Navigation als Vorherige/Nächste Prüfstelle über Anfang → Übergänge → Ende. Vorhandene unabhängige Instanz, Fehler/Abbruch/Dispose und Original-/Schnittzustand erhalten.

Lokaler **Build 25 RC3**, **0 Warnungen/Fehler**, **999/999 CAN-Tests**, **36 WPF-/Native-Kontrollen** bestanden, ohne Bindungsfehler. [Prüfbericht](KONTROLLPLAYER-ANFANG-ENDE-20261012.md). Der Nutzer bestätigt anschließend „prüfen: Anfang und Ende super :-)“. Neue Erweiterung persönlich positiv geprüft. Zwei Kimi-Bilder zeigen den gut erkennbaren Unterschied zwischen Senderblende bei MP4Box und Filmszene bei otr-can; illustrativer Vergleich bei unterschiedlichen Playerpositionen, kein identischer Frameabgleich.

Die Bedienrunde aus Build 24 wurde inzwischen vom Nutzer positiv geprüft: direktes Cutlist-/Filmspeichern, freie Namen, x-Maske auch im Cutlist-Dialog, beide Motoren einschließlich Bild/Ton, Übergangsplayer und Original-AVI mit beiden Motoren. Erster Maskenverdacht vom Nutzer korrigiert. **AVI-Frame-Lupe weiterhin ohne Bildvorschau bei fehlenden Original-PTS; separat offen, hier unverändert.** Künstlichen MP4Box-Testbefund separat behandeln; keine nachgewiesene Regression des bewährten Radical-/AVI-Ablaufs.

Acht Lockdateien, zwölf persönliche Einstellungen und sieben Build-16-Artefakte unverändert. Kein Commit, Push, Paket oder Release.

## 12.10.2026 – Schritt 8: Gesamtabnahme durchgeführt, MP4Box-Befund offen

Auf Auftrag „8) go“ Bedienrunde zusammen geprüft und zwei Verbundtests für alte/neue Masken, manuellen Filmnamen, unabhängige Cutlist-Namen, Speichern/Wiederladen, Behaltebereiche und Ausgabeübergänge ergänzt. Keine Produktionsfunktion verändert; vorhandene Kurztexte bleiben ausdrücklich anwählbar und persönliche Vorlagen erhalten.

Lokaler **Build 24 RC3**, **0 Warnungen/Fehler**, **987/987 CAN-Tests** bestanden. Kompilierte WPF-Prüfungen gegen Build 24 wiederholt, einschließlich 25 Kontrollplayer-Prüfungen. Acht reale synthetische CAN-Schnittaufträge erfolgreich: AAC/AC3 und geeignete AVI/MP3 mit beiden Motoren, zwei AAC-Spuren mit otr-can, jeweils auch wieder geladene Cutlists. Alle acht Ausgaben streng dekodiert und im unabhängigen Player geprüft; Hauptposition, Auswahl, Lupenziel und Schnittdaten erhalten.

**Offener Befund:** MP4Box 26.07 bricht bei der synthetischen Datei mit zwei gleichartigen AAC-Spuren beim Zusammenfügen mit Bitstream-/I/O-Fehler ab, auch direkt ohne CAN reproduziert. Quelle erhalten, keine fertige Ausgabe veröffentlicht, Arbeitsdateien entfernt. Keine pauschale Aussage über alle Mehrspurdateien; AAC/AC3-Prüfung erfolgreich. Negativer AVI-ohne-B-Frames/CFR-Vorlauf dokumentiert. [Vollständige Ergebnisse und Grenzen](GESAMTABNAHME-BEDIENUNG-20261012.md). Technische Prüfrunde durchgeführt, uneingeschränkte Gesamt-/Releasefreigabe und persönliche Filmabnahme noch offen.

Acht Lockdateien, zwölf persönliche Einstellungen, sieben öffentliche Build-16-Artefakte erhalten. Kein Motorumbau, Commit, Push, Paket oder Release.

## 12.10.2026 – Schritt 7: Unabhängiger Kontrollplayer für fertige Übergänge

Auf Auftrag „ok dann 7) los :-)“ Prüfen-Aktionen für innere Übergänge ergänzt. Ein unabhängiges, wiederverwendbares libmpv-Fenster lädt die tatsächliche Ausgabe, startet nach erfolgreichem Laden/Seek drei Sekunden vor dem Übergang und bietet vorherigen/nächsten Schnitt, Play/Pause, Position und Lautstärke. Eigene Ausgabezeiten pro Abschluss, Originalplayer und Schnittliste erhalten; fehlende/defekte Dateien verständlich behandeln. Abbruch/Dispose vor HWND-Zerstörung, Wiederöffnung, Hauptfensterende und Navigation nach natürlichem Filmende abgesichert.

Lokaler **Build 23 RC3**, **0 Warnungen/Fehler**, **985/985 CAN-Tests** (22 zusätzliche Fälle) bestanden. Zwei echte libmpv-Kontexte unabhängig geprüft, anschließend **25 WPF-/Native-Kontrollen** mit synthetischem H.264/AAC-Video bestanden, ohne Bindungsfehler. [Prüfbericht und Grenzen](SCHNITTUEBERGAENGE-KONTROLLPLAYER-20261012.md). Persönliche Film-/Tonabnahme noch offen; keine neuen realen Motor-Schnittaufträge für diesen Schritt. Acht Lockdateien, zwölf persönliche Einstellungen und sieben öffentliche Build-16-Artefakte unverändert. Kein Commit, Push, Paket oder Release. Schritt 8 offen.

## 12.10.2026 – Schritt 6: Rechtsklick-Kontrolle und SVG

Auf Auftrag „6) gogogo“ Rechtsklick auf Anfang/Ende ergänzt: Originalposition −3/+3 Sekunden, auf Datei und unmittelbar angrenzende Filmstücke begrenzt; bei innen direkt berührenden roten Bereichen Hinweis statt ungültigem Sprung. Auswahl, Lupenkante und Schnittzeiten erhalten; keine automatische Wiedergabe. Linksklick und Text-/Editierfokus unverändert. SVG unter docs/images/schnittkontrolle-3sek.svg erstellt, vor heller/dunkler Umgebung geprüft und mit dem vorgegebenen Erklärungstext in README eingebunden.

Lokaler **Build 22 RC3**, **0 Warnungen/Fehler**, **963/963 CAN-Tests** (25 zusätzliche Fälle) bestanden; kompilierte Tabelle mit Rechts-/Linksklick-Ereignissen und simuliertem Player geprüft, einschließlich kurzer Filmstücke, Dateigrenzen, erhaltenem Zustand und vorhandener Tastaturnavigation. [Prüfbericht und Grenzen](SCHNITTKONTROLLE-3SEK-20261012.md). Kein realer Film-/Lupe-Praxistest oder automatisierte Bestätigung des Hinweisdialogs behauptet. Acht Lockdateihashes erhalten; keine persönlichen Einstellungen geändert. Kein Restore, Commit, Push, Setup oder Release. Schritt 7 noch nicht begonnen.

## 12.10.2026 – Schritt 5: Tabellen-/Timeline-Auswahl vereinheitlicht

Auf Auftrag „also los mit 5)“ erneuten Zeilenklick auch auf Anfang/Ende zur Abwahl gemacht. Grenzsprung bleibt unabhängig von Auswahl/Abwahl erhalten; gemeinsame Auswahlquelle und Kantenfreigabe beibehalten. Bisherige Nicht-Kanten-Fokusbehandlung erhalten, Text-/Editierquellen ausgenommen; Timeline-Bindung bei interaktiven Wertwechseln erhalten. Zeitkorrektur, Löschen und Frame-Lupe unverändert.

Lokaler **Build 21 RC3**, **0 Warnungen/Fehler**, **938/938 CAN-Tests** (14 zusätzliche Fälle) bestanden. Kompilierte Tabelle/Timeline mit simuliertem Player und echten WPF-Ereignissen geprüft: Start/Ende/Dauer, Wiederklick, Zeilenwechsel, exakte Sprünge, Bindung, Textfokus und native Tastaturnavigation; keine Schnittzeitänderung oder Bindungsfehler. [Prüfbericht und Grenzen](SCHNITTBEREICH-AUSWAHL-20261012.md). Acht lokale Lockdateihashes erhalten, keine persönlichen Einstellungen gespeichert. Kein Restore, Commit, Push, Setup oder Release. Persönliche Abnahme noch offen; Schritt 6 noch nicht begonnen.

## 12.10.2026 – Schritt 4: Getrennte Namensmasken-Bausteine

Auf Auftrag „4) gogogo“ S, Staffel, E, Folge und x einzeln in der Palette angeboten; führende Nullen erhalten. S/E nur mit zugehörigem Wert, x nur mit beiden Werten. Neue Standardmaske erzeugt **Ghostbusters S01E01 Hexenprobleme [19.09.2026]**; Datum aus OTR 26.09.19, Serienangaben ausdrücklich ergänzte Beispieldaten. Filmfall ohne Serienwerte bleibt ohne SE. Alte Masken und individuelle Präfixe unterstützt; bekannte Legacy-Paare im Editor geteilt, unveränderte Paare wieder in originaler Syntax gespeichert. Keine ungefragte Änderung gespeicherter Masken.

Lokaler **Build 20 RC3**, **0 Warnungen/Fehler**, **924/924 CAN-Tests** (43 zusätzliche Fälle) bestanden. Echter WPF-Editor mit Palette, Vorschau, synthetischen Drop-/Move-Ereignissen, Entf, Standard/Verwerfen und modalem Speichern/Abbrechen geprüft; Normal-/Mindestgröße, keine Bindungsfehler, ausschließlich isolierte Einstellungen. [Prüfbericht](NAMENSMASKEN-EINZELBAUSTEINE-20261012.md). Alle acht lokalen Lockdateihashes erhalten, keine persönlichen Einstellungen geändert. Kein Restore, Commit, Push, Setup oder Release. Persönliche Bedienabnahme noch offen; Schritt 5 noch nicht begonnen.

## 12.10.2026 – Schritt 3: Cutlist automatisch speichern

Auf Nutzerauftrag „ok dann 3)“ den zusätzlichen Cutlist-Dateispeicherdialog entfernt. Ausschließlich eingerichteten Ordner **Eigene Cutlists** verwenden; Originaldateiname plus .cutlist unverändert, kein Fallback und keine Ordneranlage. Überschreibfrage mit Nein als Vorgabe; geschützter temporärer Schreibweg erhält bestehende Dateien bei Fehlern. Schreibarbeiten im Hintergrund; Uploadpfad und Erfolgsmeldung erst nach erfolgreicher Speicherung. Ordnerprüfung aus Schritt 2 gemeinsam verwendet; Einstellungen-Hilfetexte angepasst, Lade-Dialoge und Motoren unverändert.

Lokaler Entwicklungsbuild **0.2.1 · Build 19 · RC3**, **0 Warnungen/Fehler**, **881/881 CAN-Tests** (21 zusätzliche Fälle) bestanden. Bytegleichheit, Original-/Schnittzuordnung, Rechte/Dateisperren, Konflikte und Aufräumen geprüft; kompilierten Cutlist- und Ordnerdialog mit isolierten Einstellungen geprüft. [Prüfbericht und Grenzen](CUTLIST-AUTOSPEICHERN-20261012.md). Erster Dateisperrtest erwartete eine andere Windows-Fehlerklasse; Erwartung korrigiert, Schutzprüfungen erhalten. Acht lokale Lockdateihashes erhalten. Kein Restore, Commit, Push, Setup oder Release. Persönliche Speicherabnahme noch offen; Schritt 4 noch nicht begonnen.

## 12.10.2026 – Schritt 2: Filmname editierbar, direktes Ausgabeziel

Auf Nutzerauftrag „2) go“ vollständigen Filmnamen im gemeinsamen Dialog frei editierbar gemacht; exakter Hilfetext und bewusste Neuerzeugung erhalten. Film-Dateispeicherdialog entfernt: ausschließlich konfiguriertes Verzeichnis „Geschnittene Videos“, einmal .mp4, strenge Namens-/Pfad-/Schreibprüfung, Originalschutz und ausdrückliche Überschreibfrage mit Nein als Vorgabe. Manueller Filmname bleibt getrennt von Cutlist-Namenszustand und gespeicherter Maske. Cutlist-Speicherdialog und beide Motorimplementierungen unverändert.

Lokaler Entwicklungsbuild **0.2.1 · Build 18 · RC3**, ohne Warnungen/Fehler, **860/860 CAN-Tests** (67 zusätzliche Fälle) bestanden; beide CAN-Motorservices mit simulierten Werkzeugen einschließlich Konflikten geprüft. Kompilierter WPF-Dialog mit echten Bindungen/Buttons bei Normal-/Mindestgröße geprüft, keine Bindungsfehler, isolierte Fenstereinstellungen. [Umsetzung und Prüfgrenzen](AUSGABENAME-DIREKTSPEICHERN-20261012.md). Initiale Testdaten-/ACL-Prüffehler korrigiert, keine Tests entfernt oder übersprungen. Acht lokale Lockdateihashes erhalten; kein Restore, Commit, Push, Setup oder Release. Persönliche neue Filmprobe noch offen; Schritt 3 noch nicht begonnen.

## 12.10.2026 – Schritt 1: Schaltflächen neu angeordnet

Auf Nutzerauftrag **„1) go“** die acht Schaltflächen exakt umgeordnet: Bereich löschen → Lokale Cutlist laden → Cutlist vom Server laden → MP4Box → otr-can → Cutlist erzeugen → Upload → Namensmaske. Nur MainWindow.xaml funktional geändert; vorhandene Beschriftungen, Click-Handler, Bindings und Aktivierungsregeln erhalten. WrapPanel beibehalten, zwei Abstände angepasst. Cutlist-Speichern bleibt eine eigenständige Aktion, keine neue zeitliche Voraussetzung.

Lokaler Entwicklungsbuild **0.2.1 · Build 17 · RC3**, **0 Warnungen/Fehler**, **793/793 CAN-Tests** bestanden, keine übersprungen. Gebautes MainWindow bei 960×780 und Mindestgröße 760×600 vermessen; alle acht Buttons vollständig angeordnet, visuelle Reihenfolge und Bindings geprüft. Kompilierte Buttons im unsichtbaren Vorschaufenster gerendert; keine Hauptplayerinitialisierung oder Speicherung von Benutzereinstellungen. Keine neue persönliche Klick-/Filmabnahme behauptet. [Plan und Nachweise](ENTWICKLUNGSPLAN-BEDIENUNG-20261012.md#umsetzungsstand--schritt-1-abgeschlossen). Alle acht Lockdateihashes erhalten. Kein Restore, Commit, Push, Setup oder Release; veröffentlichter Build 16 bleibt unverändert. Schritt 2 noch nicht begonnen.

## 12.10.2026 – Nächste Bedienungsrunde geordnet und analysiert

Gesammelte Nutzerwünsche in acht Schritten geordnet: Schaltflächen; freier Filmname und direktes Ausgabeziel; automatisches Cutlist-Speichern; Namensmasken; gemeinsame Auswahl; Rechtsklickkontrolle samt SVG; unabhängiger Kontrollplayer; Gesamtabnahme und gesonderter Kurztextabgleich. Gemeinsamer Motor-/Namensablauf, bisherige Dateinamen-Sanierung und Verzeichnis-Fallbacks, Cutlist-Schreibschutzbedarf, Anfang-/Ende-Auswahlpfad sowie libmpv-Instanzlebenszyklus untersucht. [Entwicklungsplan mit Bestandsaufnahme und konkreten Vorschlägen](ENTWICKLUNGSPLAN-BEDIENUNG-20261012.md). Noch keine funktionalen Änderungen; keine Builds, Pakete, Commits oder Veröffentlichung für diesen Planungsschritt. Vier lokale Produktions-Lockänderungen erhalten.

## 12.10.2026 – Build 16 RC3 veröffentlicht

Auf ausdrücklichen Nutzerauftrag **5) go :-)** um **12.10.2026 15:26:56 Uhr MESZ** als öffentliche Vorabversion veröffentlicht. CAN-Tag **v0.2.1-build16-rc3** verweist auf den gebauten Quellcommit **603d64d**; Motor **0.1.5 / 2a47462**. Alle **sieben Uploads**, Größen, GitHub-SHA-256-Digests, Release-Text und Quellziel vor und nach Freigabe geprüft. **793/793 CAN-Tests**, **44/44 Rust-Tests**, 249 Portable-Dateien geprüft; gemeldete persönliche Setup-/VM-/Portable-Prüfungen positiv. Bekannter PTS-Befund und weitere Prüfgrenzen bleiben erhalten. [Öffentliches Release](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.1-build16-rc3), [Veröffentlichungsnachweis](RELEASE-PUBLICATION-BUILD16-RC3.md). Pakete und Prüfsummen unverändert, vier vorbestehende Produktions-Lockdateiänderungen nicht übertragen.

## 12.10.2026 – Portable-Prüfung in der VM bestätigt

Nutzer bestätigt **„VM und zip: MP4Box check / otr-can check“**. Portable-ZIP mit beiden Motoren positiv geprüft; vorherige frische Setup-VM-Installation, beide Motoren und Deinstallation ebenfalls bestätigt. Beide neuen Anhänge sind identisch und enthalten MP4Box: eigene Arbeits-/Segmentdateien, sechs Bereiche, Zusammenfügen, „Fertig.“. otr-can aus ausdrücklicher Nutzerbestätigung übernommen, keine neuen nativen Messwerte behauptet. [Paketprüfbericht](RELEASE-BUILD16-RC3-RESULT.md). Praktische gemeldete Paketprüfungen positiv abgeschlossen; Überinstallation, frisches Portable-Profil und strenger Video-Nachweis nicht daraus abgeleitet. Bekannte Grenzen erhalten. Nächster separater Schritt: Veröffentlichung. Pakete/Prüfsummen und Build 16 unverändert.

## 12.10.2026 – Frische VM-Setup-Prüfung bestätigt

Nutzer bestätigt **frische Setup-Installation in der VM**, **MP4Box**, **otr-can** und **Deinstallation**. Beigefügtes MP4Box-Protokoll zeigt erfolgreiche Stream-Copy-Vorbereitung der Teufelsbrigade-AVI, sechs Bereichsschnitte, Zusammenfügen und „Fertig.“. otr-can und Deinstallation ausdrücklich als „check“ bestätigt; keine neuen nativen Messwerte oder zusätzliche detaillierte VM-Bild-/Tonabnahme daraus abgeleitet. [Paketprüfbericht](RELEASE-BUILD16-RC3-RESULT.md). Vorherige detaillierte Setup-Filmabnahme bleibt gültig, PTS-Befund erhalten. Nächster Pakettest: Portable-ZIP; Überinstallation nicht gesondert bestätigt. Kein neuer Build oder Release.

## 12.10.2026 – Setup Build 16: AVI/MP3 persönlich bestätigt

Nutzer bestätigt neuen Teufelsbrigade-Lauf aus **Setup Build 16**: **„Anfang, Ende, Schnitte, Bild, Ton alles top :-)“**. Protokoll: verlustfreie AVI-Vorbereitung, eine Indexierung, Motor 0.1.5, sechs Bereiche, 182.574 Videopakete und 350.542.080 Tonsamples/48 kHz, alle Motor-/CAN-Prüfungen abgeschlossen. Alle 249 erklärten installierten Dateien nachträglich gegen Paketmanifest bytegenau geprüft. Motorpfad vom verbliebenen Entwicklungspfad auf die bytegleiche installierte EXE korrigiert und gesichert; kein neuer Schnitt nach dieser Umstellung behauptet. [Aktualisierter Paketprüfbericht](RELEASE-BUILD16-RC3-RESULT.md). Installationstyp noch nicht ausdrücklich bestätigt; Portable, MP4Box-Paketlauf, weitere Installations-/Deinstallationsprüfungen und Veröffentlichung bleiben offen. Bekannte PTS-Warnungen bleiben dokumentiert.

## 12.10.2026 – Setup/Portable Build 16 RC3 erstellt

Schritt 3 abgeschlossen: fester CAN-Quellcommit **603d64d**, Motor **0.1.5 / 2a47462**. Gesperrter Restore ausschließlich im exportierten Quellbaum, Build/Publish erfolgreich, **0 Warnungen/Fehler**, **793/793 Tests**. Setup und Portable sowie vier passende Quellen-/Relink-Archive und Prüfsummen erstellt. Alle **249 Portable-Dateien**, App-Version 0.2.1.16, RC3-Assemblymetadaten, Motorversion/MP3-Fähigkeit und Artefakthashes geprüft. Getestete Motor-EXE erhalten; lokale und exportierte Lockdateien sowie Quellmanifest unverändert, Zähler bleibt 16. [Paketprüfbericht](RELEASE-BUILD16-RC3-RESULT.md). Noch keine Installation, persönliche Paketabnahme oder Veröffentlichung; nächster Schritt 4: Installations-/Portable- und Schnittprüfungen mit tatsächlichen Paketkomponenten.

## 12.10.2026 – Quellstand Build 16 RC3 gesichert

Schritt 2 abgeschlossen: CAN-Quellcommit **603d64d**, Motor-Quellcommit **2a47462** lokal gesichert. Feste CAN-/Motorquellenarchive ohne Arbeitsbaumüberlagerung und ein vollständiges 0.1.5-Motorpaket mit Quellen, Lizenzbeständen und Provenienz vorbereitet. Exportierter Motor offline gebaut, **44/44 Tests**, Archive/Manifeste vollständig geprüft; echter Motorblock des eingefrorenen CAN-Paketskripts akzeptiert das Paket. Getestete EXE erhalten; Kontrollbuild nicht bytegleich, keine bytegenaue Reproduzierbarkeit behauptet. Alle acht lokalen CAN-Lockdateien unverändert; vier vorbestehende Produktionsänderungen bleiben offen. CAN-Prüfstand weiterhin Build 16, 793/793 Tests. [Quellen-/Prüfbericht](QUELLSTAND-BUILD16-RC3-20261012.md). Kein Setup/Portable, Push, Tag oder Release. Nächster Schritt: Paketbau aus genau diesen Quellen.

## 12.10.2026 – Paketbau-Vorbereitung für otr-can 0.1.5

Schritt 1 abgeschlossen: Paketskript erwartet 0.1.5 samt passendem Snapshot/Quellenarchiv und prüft zusätzlich die MP3-Fähigkeit der kopierten EXE. Installationshinweise aktualisiert. Je 10 PowerShell-Prüffälle unter PowerShell 7 und Windows PowerShell 5.1 bestanden. Validierungsbuild **0.2.1 · Build 16 · RC3**, ohne Warnungen/Fehler, **793/793 CAN-Tests**. Anwendungscode/Motor unverändert; Filmabnahme betrifft den zuvor geprüften Build-15-Ablauf. Kein neuer Paketsatz erstellt. [Prüfbericht](PAKETBAU-OTR-CAN-015-20261012.md). Nächster Schritt: Quellstand sichern und passende feste Quellen vorbereiten; lokale Lockdateiänderungen erhalten.

## 12.10.2026 – Experimentelle AVI-/MP3-Unterstützung für otr-can

Lokaler Stand **CAN 0.2.1 · Build 15 · RC3 / otr-can 0.1.5**. Vorhandenes verlustfreies Umpacken wiederverwendet, Arbeitsquelle statt Original indexiert/geschnitten, MP3 erst nach PCM-Schnitt einmal nach AAC-LC kodiert. 793/793 CAN- und 44/44 Rust-Tests; vier synthetische Aufträge mit sechs bytegenau geprüften PCM-Tonspuren sowie bytegleicher AAC-/AC3-Regressionsausgabe bestanden. MP4Box-AVI und MP4-Multispurablauf weiterhin erfolgreich.

Teufelsbrigade: sechs unveränderte Keep-Bereiche, 182.574 Videopakete, 350.542.080 Audio-Samples; unabhängiges PCM vor Kodierung bytegleich zur Originalauswahl. Persönliche Filmabnahme am 12.10.2026 bestätigt: Anfang, Ende, alle Schnittübergänge, Ton und Bild einwandfrei. Strenger zusätzlicher Videodekodierlauf zeigt vorbestehende PTS-Auffälligkeit; Videostream neuer und früherer Ausgabe bytegleich. Keine allgemeine AVI-Freigabe oder Video-Reparatur daraus abgeleitet. [Prüfbericht](AVI-OTR-CAN-MP3-20261012.md). Vier vorbestehende src-Lockänderungen erhalten; kein Commit oder Release. Lokaler Motorpfad auf die separat geprüfte 0.1.5-EXE gestellt; frühere Einstellung gesichert, Indexer und alte EXE unverändert.

Die folgenden Angaben dokumentieren den vorherigen, veröffentlichten Build-13-Stand.

Stand: 11.10.2026. Dieser Überblick beschreibt den aktuellen Feature-Stand; ältere Projektdefinitionen und Architekturentscheidungen dokumentieren teilweise frühere Entwicklungsphasen.

Lokaler Entwicklungsstand nach Credits-Ergänzung und Dokumentationsabgleich: **0.2.1 · Build 13 · RC3**, Release-Build ohne Warnungen/Fehler und **785/785 CAN-Tests**. Der KI-Absatz wurde im echten WPF-Dialog bei normaler und kleiner Ansicht geprüft. Der persönlich abgenommene Paketsatz bleibt **Build 12 RC3**; dessen eingefrorene Quellen-/Binärpakete enthalten diese Ergänzung noch nicht. Der aktualisierte Build-13-Paketsatz ist erstellt und technisch geprüft; persönliche Paketbestätigung noch offen. Motor otr-can 0.1.4 unverändert, letzter nativer Prüfstand 40 Tests.

## Aktueller Umfang

Der aktualisierte **Build-13-RC3-Paketsatz ist erstellt, automatisch geprüft und veröffentlicht**: fester Quellcommit c98041a, 785/785 Tests, 249 vollständig geprüfte Portable-Dateien, Credits-/WPF-Prüfung mit entpackten Komponenten und künstlicher Zwei-Tonspur-Schnitt bestanden. Die Schnittausgabe ist bytegleich zum Build-12-Pakettest. [Neuer Prüfbericht](RELEASE-BUILD13-RC3-RESULT.md). Eine neue persönliche Installations-/Portable-Bestätigung für Build 13 ist nicht dokumentiert; der Nutzer beauftragte die sofortige Veröffentlichung. [Öffentliches RC3-Release](RELEASE-PUBLICATION-BUILD13-RC3.md).

CAN verwendet C#, .NET 10 und WPF unter Windows 11 x64. ffprobe liefert Medieninformationen, mpv/libmpv übernimmt die Wiedergabe, MP4Box den Schnitt. FFmpeg bereitet bei Bedarf eine MP4-Arbeitsdatei vor.

Umgesetzt sind Medienanalyse und Wiedergabe, Frame-Navigation, Frame-Lupe mit Schnittkantenübernahme und Halbierungssuche, manuelle Schnittplanung, lokale Cutlists, Server-Suche/-Download/-Upload, Namensmasken, Herkunftsübernahme bei Fremd-Cutlists, MP4Box-Schnitt und experimentelle Vorbereitung anderer Container. Die Bedienung ist in der [Nutzeranleitung](NUTZERANLEITUNG.md) beschrieben.

Die optionale OTR-CAN-Integration ist als Schritt 5 umgesetzt: der gemeinsame unveränderliche Schnittauftrag hält Original-/Zielpfade und Keep-Segmente fest. Die Motorwahl erfolgt auf Benutzerwunsch über **Schneiden MP4Box (schnell)** oder **Schneiden otr-can (framegenau, langsam)** direkt unter der Schnittliste, jeweils für diesen Auftrag. **OTR-CAN-Werkzeuge …** bietet nur Pfade und Prüfungen; ein älterer gespeicherter Motorhaken wird ignoriert. OTR-CAN 0.1.4 erhält die konfigurierten FFmpeg-/ffprobe-Pfade. CAN indexiert einmal im eigenen Arbeitsordner, startet den CPU-Motor am Original, prüft die MP4-/Streamstruktur und übernimmt erst danach das Ergebnis. Abbruch beendet den Prozessbaum; nach Erfolg, Fehler oder Abbruch werden eigene Arbeitsdateien bereinigt. Die lokale Entwicklungskennung ist seit 13.10.2026 **0.3.0 · Build 29 · RC3**; die öffentliche Ausgabe **0.2.1 · Build 16 · RC3 mit otr-can 0.1.5** bleibt unverändert.

Aktueller regulärer Motor **0.1.4**: Frame-/Zeitstempelkorrektur und vollständige Audio-Neukodierung aus 0.1.3, zusätzlich stabiler Audiofilterzustand bei geänderten Downmix-Begleitdaten. CAN verarbeitet große Warnprotokolle gebündelt mit begrenzter Anzeige und vollständiger Kopiermöglichkeit. **40 Rust- und 785 CAN-Tests**, 24 gemeinsame Grenzfälle, echter WPF-Großprotokolltest und unabhängiger Radical-PCM-Vergleich beider Tonspuren bestanden. Radical-Korrektur technisch geprüft und vom Nutzer persönlich bestätigt: sauberer Übergang, synchroner Ton. Stereo wird als lauter als 5.1 wahrgenommen; der Nutzer bestätigt denselben Unterschied bereits im Original. Keine quantitative Pegelmessung vorgenommen. Details und regulärer Abschluss im [Radical-Korrekturbericht](OTR-CAN-RADICAL-KORREKTUR-20261011.md). Pines und die ursprüngliche 0.1.3-Übernahme bleiben im [0.1.3-Prüfbericht](OTR-CAN-0.1.3-PRUEFBERICHT.md) dokumentiert. MP4Box bleibt Standard, ältere EXEs erhalten. Den aktuellen Praxistest-Punkt hat der Nutzer am 11.10.2026 als abgeschlossen bestätigt; Paketvorbereitung und persönliche Installationsabnahme von Build 12 sind anschließend abgeschlossen. Lokaler Build-12-RC3-Paketsatz mit Setup, Portable-ZIP und passendem Quell-/Lizenzbestand vorbereitet; persönliche Überinstallation, Windows-11-VM- und separate Portable-Paketprüfung einschließlich Bild/Ton und Setup-Deinstallation positiv bestätigt. [Paketprüfbericht](RELEASE-BUILD12-RC3-RESULT.md). Build 13 RC3 anschließend auf Nutzerauftrag veröffentlicht; siehe Veröffentlichungsnachweis.

Zusätzlicher persönlicher 0.1.3-Praxistest am 11.10.2026: **Torchwood / Vergiftet (HQ)** mit drei Behaltebereichen. Nutzer bestätigt **„kein Blitzer, ton ist top :-)“**. Das übermittelte Protokoll zeigt eine Indexierung, 77956 Video-Pakete / 25 fps sowie eine AAC-Stereospur / 48 kHz mit 149675520 decodierten Samples; jeweils 51:58.240, erfolgreicher nativer und CAN-Abschluss. Keine Laufzeit mitgeteilt. Ein-Tonspur-Test; weitere getrennte Mehrtonspurkontrollen bleiben offen. Details im [0.1.3-Prüfbericht](OTR-CAN-0.1.3-PRUEFBERICHT.md).

Weiterer direkter Nutzerlauf am 11.10.2026: **Chicago Fire / Ein komischer Vogel (HD)** mit Motor 0.1.4 und einem Behaltebereich erfolgreich abgeschlossen; AAC-Stereo und AC-3/6 Kanäle, beide Samplezahlen/-PTS und finaler Mux geprüft laut übermitteltem Protokoll, Ergebnisdauer 39:37.160. Nutzer bestätigt anschließend den otr-can-Bildanfang als ohne Blitzer; im gezeigten MP4Box-Vergleich steht noch TV-SPONSORING. Nutzer bestätigt abschließend **„ton und bild beide ok :-)“**. **Fenstergrößenübernahme vom Nutzer als „check“ bestätigt**. Details im [Motor-Prüfbericht](OTR-CAN-RADICAL-KORREKTUR-20261011.md) und [Fenstergrößenbericht](DIALOG-FENSTERGROESSEN-PRUEFBERICHT-20261011.md).

Weiterer persönlicher MP4Box-Praxistest am 11.10.2026: **Brilliant Minds (HQ)**, Aufnahme vom 09.10.2026, mit drei Behaltebereichen erfolgreich geschnitten und zusammengefügt laut übermitteltem Protokoll. Nutzer bestätigt **„Einwand frei ohne Blitzer :-)“**. Positive persönliche Schnitt-/Bildbewertung des MP4Box-Ergebnisses; keine gesonderte Tonspurprüfung oder Laufzeit mitgeteilt. Details im [Tagebuch](TAGEBUCH.md).

## Frühere Prüfungen und Entwicklungsschritte

Die folgenden Befunde dokumentieren die jeweiligen älteren Motor- und Versuchsstände. Aktuelle Übernahme und Grenzen stehen oben im 0.1.3-Prüfbericht.

Der neue Motor unterstützt zunächst genau eine Videospur an Streamposition 0 und Schnittzeiten unter 24 Stunden. CAN übergibt seine Zeitangaben mit sieben Nachkommastellen; die native Mikrosekundenberechnung bleibt erhalten. Die reale Referenz-/Decoderprüfung von Navy CIS und Kimi ist abgeschlossen: jeweils zwei Tonspuren, korrekte Soll-Bildanzahl und bytegleiche Ausgabe zu einem frischen CAN-04-Schnitt. Der Nutzer hat beide fertigen Filme persönlich als perfekt bewertet. Geerbte Zeitstempelauffälligkeiten an Teilübergängen bleiben offen; siehe [Prüfbericht](OTR-CAN-SCHRITT6-PRUEFBERICHT.md). Kein Paket veröffentlicht. Die Cutlist-Erstellung erhält keine neue Werkzeugpflicht; der bisherige ffprobe-Bedarf beim Videoladen bleibt bestehen.

Zusätzlicher direkter CAN-Bedientest am 10.10.2026: Radical mit derselben unveränderten Cutlist über beide Schnittbuttons. Der bei MP4Box sichtbare SRF-zwei-Blitzer am Übergang um 57:26 ist laut Nutzer im otr-can-Ergebnis verschwunden, ohne Lupenkorrektur. Abschlussprotokoll bestätigt eine Indexierung und erfolgreichen nativen Schnitt; die Ausgabe enthält AAC-Stereo und AC-3 mit sechs Kanälen. Details stehen im [Prüfbericht](OTR-CAN-SCHRITT6-PRUEFBERICHT.md).

Weiterer direkter HD-Bedientest am 10.10.2026: The Place Beyond the Pines mit fünf Behaltebereichen. Der Nutzer bestätigt die Schnittstellen beider Ergebnisse als „super“, ohne Blitzer und ohne Verwendung der Lupe. otr-can benötigte laut Nutzer knapp 15 Minuten. Beide Ausgaben enthalten AAC-Stereo und AC-3-Stereo; der native Metadatenbefund von einem zusätzlichen Videoframe/40 ms gegenüber der nominalen Cutlist bleibt im Prüfbericht festgehalten.

Nach persönlicher positiver Prüfung des schnellen Seek-Versuchs direkt in CAN wurde die Optimierung als regulärer Motor **0.1.2** übernommen. Video verwendet kurzen Vorlauf, der Ton behält den bisherigen Dekodierweg. Keine Experimental-Kennzeichnung oder Build-Schalter mehr; der lokale CAN-Motorpfad ist auf den neuen regulären Build gestellt. Vier erneute reale 0.1.2-Ausgaben sind bytegleich: Pines 57,2 s, Navy CIS 23,1 s, Kimi 31,0 s und HQ 24,8 s, jeweils Indexierung plus Motorlauf. 25 Rust- und 743 CAN-Tests bestanden; MP4Box bleibt Vorgabe. Aufbau und Messgrenzen stehen im [Seek-Prüfbericht](OTR-CAN-SEEK-PRUEFBERICHT.md).

Vorbereitung von Version 1.0 am 11.10.2026: Eine nummerierte Bilddiagnose am unveränderten 0.1.2-Motor reproduziert falsche Segmentgrenzen in 12 von 16 Grenzfällen. Gleiche Gesamtbildzahl kann ein fehlendes und ein doppeltes Bild verdecken; rohe PTS und Audio-Übergänge sind weiterhin nicht freigegeben. Die Lupen-Vorschaukette besteht 30 echte Bild-/PTS-/Zeitprüfungen, der Nutzer meldet aktuell keine neue Navigationsauffälligkeit. Vor Übernahme einer Korrektur ist eine separate Versuchsvariante erforderlich; siehe [Diagnose und Vorschlag](OTR-CAN-FRAME-DIAGNOSE-20261011.md).

Die freigegebene getrennte Video-Testvariante korrigiert inzwischen alle 16 ursprünglichen und sieben zusätzlichen nummerierten Bildfälle. Pines hat 137.625 tatsächlich decodierte Bilder mit korrekten Video-PTS/DTS; 84 Originalbildvergleiche ohne Abweichungsbefund, davon 39 wegen ähnlicher Nachbarbilder nicht eindeutig. 31 Rust-Experimenttests bestanden. Ausschließlich Video ohne Ton, keine CAN-Übernahme; Audio-Übergänge bleiben vor Version 1.0 offen. Quellen im Rust-Repository experiments/frame-boundaries, Details im [Korrekturbericht](OTR-CAN-FRAME-KORREKTUR-20261011.md).

Audio-Versuch am 11.10.2026: 35 synthetische Aufträge / 70 Tonspuren und beide Pines-Spuren mit exakten PCM-/Ergebnis-Samplezahlen sowie lückenlosen rohen PTS geprüft. Gemeinsame Pines-Testdatei hat 137.625 Bilder und alle drei Streams bei 5505,000 s; Paket-/Konfigurationsvergleich vor/nach Mux identisch. **Vollständige Audio-Neukodierung**, zusätzliche Verluste und Laufzeit. Der Nutzer bestätigt den gemeinsamen Pines-Testfilm persönlich als einwandfrei synchron; keine ausdrücklich getrennte Tonspur-Abnahme oder CAN-Übernahme daraus abgeleitet. Nachweise und Entscheidung vor Integration im [Audio-Prüfbericht](OTR-CAN-AUDIO-PRUEFBERICHT-20261011.md).

## Frame-Lupe und Schnittkantenkorrektur

- Umschalt+F öffnet die Lupe für eine ausdrücklich angeklickte Anfangs- oder Endkante. Eine unabhängige FFmpeg-Vorschau wird anhand des Original-PTS und der Zeitbasis geprüft. Tabellenwahl, Keyframe-Anzeige und normale Schritte mit ±1/±10 sowie Strg für ±20 sind verfügbar.
- Eigene Suchbuttons und Umschalt+Links/Rechts beginnen standardmäßig mit 2000 Frames und halbieren nach jedem erfolgreich angezeigten Suchsprung bis auf 1. Der Startwert ist einstellbar; normale Schritte verändern die Suchweite nicht.
- **Schnittkante übernehmen** setzt die ausgewählte Grenze vor den angezeigten Frame. Bei einer Startkante ist dieser das erste entfernte Bild, bei einer Endkante das erste anschließend behaltene Bild. Abbrechen verändert den Plan nicht. **Cutlist erzeugen** speichert anschließend die korrigierten Behaltebereiche.
- Derzeit wird eine Datei mit genau einem Videostream vorausgesetzt. Die Frame-Nummer gilt nur für den geladenen Analyseabschnitt; ein absoluter Dateiindex und der unabhängige Abgleich zum Hauptplayerbild bleiben offen.
- Navy-CIS-Praxistests bestätigten Navigation, Keyframe-Auswahl und das Speichern einer korrigierten Cutlist. Der externe CutlistDude-Test zeigte einen Ein-Frame-Versatz bei der Zuordnung der Zeiten, einschließlich eines Werbebildes am zweiten Filmstart. CutlistDude/CL_OFFSET und CANs MP4Box-Modus wurden nicht geändert; die Zuordnung muss vor einer Integration gesondert geklärt werden.

## Oberfläche und Cutlist-Ablauf

- „Videodatei laden“ bietet MP4, AVI, MKV und weitere Dateiendungen an. Entscheidend ist die Containeranalyse, nicht der Dateiname. Tatsächlich erkanntes AVI erhält nach dem Laden einen gelben, nicht blockierenden Hinweis auf mögliche Bild-, Navigations- und Tonprobleme im Original. Abweichende Endungen wie `.avi.mp4` werden erklärt; bei Dateiwechsel, Analysefehler oder Abbruch wird ein vorheriger Hinweis entfernt. Echte MP4-Dateien erhalten keine AVI-Warnung.
- Nach dem Laden wird bei konfiguriertem Server automatisch gesucht. „Cutlist vom Server laden“ öffnet die Suche erneut, ohne die Originaldatei neu laden zu müssen.
- Lokale und heruntergeladene Cutlists verwenden denselben Importweg. Dateigrößenabweichungen lösen eine Warnung aus und können bewusst akzeptiert werden.
- Null-Längen-Einträge werden beim Import ignoriert und NoOfCuts wird intern angepasst. Negative Dauern, widersprüchliche Frame-Dauern und ausschließlich leere Bereiche bleiben Fehler.
- Die Oberfläche zeigt Entfernbereiche; klassische Cutlists speichern die komplementären Behaltebereiche.
- Fremde Cutlists können als Vorlage dienen. Ein vorhandener Namensvorschlag bleibt zunächst erhalten und kann bewusst aus den eigenen Eingaben neu erzeugt werden.
- Bei abweichendem Vorlagenautor ergänzt CAN den Benutzerkommentar automatisch um einen Herkunftshinweis. Fehlende oder eigene Autoren erzeugen keinen Hinweis; doppelte Einträge werden vermieden.
- Anfang, Ende und Dauer erscheinen in der Schnittliste einheitlich mit drei Nachkommastellen. Die Namensvorschau im Dialog „Cutlist erzeugen“ ist markierbar und mit Strg+C kopierbar.
- Technische Medieninformationen stehen im Dialog „Video-Informationen“.
- Bei geringer Breite umbrechen die Bediengruppen, bei geringer Höhe scrollt der Hauptbereich. Das native Videofenster wird auf den sichtbaren Scrollbereich begrenzt.
- Fenstergröße wird auch ohne Videoladen gespeichert; Erfolg oder Fehler wird protokolliert.
- Schneiden, Fortschritt/Protokoll, Abschluss und Cutlist erzeugen merken getrennt Größe und Maximierung über Aufrufe und Neustarts. Bestehende Cutlist-Einstellungen bleiben kompatibel; 767 CAN-Tests und echter Vier-Dialog-Neustarttest bestanden. [Prüfbericht](DIALOG-FENSTERGROESSEN-PRUEFBERICHT-20261011.md).

## Hilfe, Credits und Versionsanzeige

- Hilfe verlinkt Nutzeranleitung und GitHub-Projekt; Update/GitHub öffnet ebenfalls die Projektseite.
- Credits zeigt die abgestimmte Danksagung in einem scrollbaren Dialog mit Projektlinks. Der KI-Hinweis aus der Dokumentation wurde nach der Build-12-Paketabnahme auch im aktuellen Dialog ergänzt; die eingefrorenen Build-12-Pakete enthalten diese Ergänzung noch nicht.
- Version und lokale Buildnummer stehen im Fenstertitel und in der Hauptüberschrift. Aktueller Entwicklungsstand: 0.3.0 · Build 29 · RC3; öffentliche Vorabversion: 0.2.1 · Build 16 · RC3 mit otr-can 0.1.5.
- Vollständige Builds über `tools/build.ps1` erhöhen den Zähler nur nach Erfolg. Fehlgeschlagener Build und anschließendes Weiterzählen wurden geprüft.
- Hilfe-Links, Credits-Dialog und Update/GitHub wurden vom Nutzer live bestätigt.

## Experimentelle Video-Vorbereitung

Originaldatei → Analyse → Server-/lokale Cutlist oder eigene Marken → „Schneiden“ → gegebenenfalls Ja/Nein-Abfrage → FFmpeg-Arbeitsdatei → Plausibilitätsprüfung → MP4Box → MP4-Ausgabe.

Die Originaldatei bleibt geladen und unverändert. Cutlist-Namen und Dateigrößen beziehen sich weiterhin auf das Original. Die temporäre MP4 wird nicht als neue Nutzerdatei geladen und nicht Grundlage einer automatisch erzeugten Server-Cutlist.

FFmpeg verwendet Stream-Copy für alle Streams. Nicht kompatible Streams werden nicht still entfernt oder neu kodiert. Nach Abschluss, Fehler oder Abbruch versucht CAN die temporäre MP4 zu löschen; Fehler beim Aufräumen erscheinen im Protokoll. MP4Box legt seine Schnittsegmente im Ausgabeordner ab und räumt sie am Ende auf.

Geprüft werden Container, Streamanzahl, Codecs, Auflösung, Bildrate, Dauer und verfügbare Start-/Frame-Angaben. Kleine Abweichungen werden nur im Protokoll vermerkt:

| Merkmal | Aktuelle Toleranz |
|---|---|
| Gemeldete Frame-Anzahl | bis zu zwei Frames |
| Mittlere Bildrate | rechnerisch höchstens zwei Frames über die Original-Laufzeit |
| Containerlaufzeit | zwei Frames, mindestens 100 ms |
| Relativer Videostart | zwei Frames |
| Relativer Audiostart | 100 ms |
| Fehlende Start-/Frame-Anzahl-Metadaten | Detailhinweis; übrige Prüfungen gelten weiter |

Dies ist eine Metadaten-Plausibilitätsprüfung, kein vollständiger Nachweis der Timeline oder Ton-Synchronität. Die Diplomatin zeigte 317399 gegenüber 317397 Frames und 6347,98 gegenüber 6348 Sekunden. Dieser akzeptierte Grenzfall ist durch einen Regressionstest abgesichert.

## Fortschritt und Fehler

Das Protokoll ist standardmäßig aufgeklappt, kann eingeklappt und in die Zwischenablage kopiert werden. Es zeigt sowohl FFmpeg als auch MP4Box. Ein konkreter Fehlergrund bleibt im Statusbereich sichtbar, selbst wenn nachgereichte Fortschrittsmeldungen eintreffen. Bei Fehler gibt es kein automatisches Schließen.

## Verifikation

- Historischer vollständiger Testlauf am 10.10.2026: **743/743 Tests bestanden**, Release-Rebuild unter Buildnummer 11 ohne Warnungen oder Fehler; damaliger nativer Release-Build mit **25/25 Rust-Tests**. Aktueller bestätigter Paketprüfstand: 785 CAN-/40 Rust-Tests, siehe oben. Ein künstliches H.264-Video mit zwei AAC-Tonspuren wurde über CAN geschnitten: genau eine Indexierung, 70 Videoframes, 348 identische decodierte Bild-/Ton-Datensätze einschließlich Zeitstempeln und bytegleiche Ausgabe zu CAN 04. Ein einzelner AVI-Abschnitt wurde als tatsächliches MP4 ausgegeben. Die integrierte Werkzeugpfadübergabe funktioniert auch ohne FFmpeg/Indexer im Prozess-PATH. Motorwahl in normaler/kleiner WPF-Ansicht und isolierter Einstellungsdatei geprüft. Die folgenden realen Navy-CIS-/Kimi-Prüfungen bestätigen Referenztreue und decodierte Bildanzahl mit den dokumentierten Zeitstempelvorbehalten.
- P1 abgeschlossen: Schutz übernommener Namensvorschläge, bewusste Neuberechnung aus der Namensmaske, automatische Herkunftsangabe bei Fremd-Cutlists, einheitliche Zeitdarstellung und kopierbare Namensvorschau. Die Herkunftsübernahme ist durch sieben neue Testfälle und praktische Prüfungen abgesichert.
- Diplomatin und Rubikon: integrierter AVI-Ablauf mit jeweils zwei Cutlists erfolgreich; Wiedergabe einschließlich Anfang, Ende und Ton-Synchronität vom Nutzer bestätigt.
- Zusätzlich bestätigter Diplomatin-Schnitt mit HQ-Cutlist trotz unterschiedlicher Quelldateigröße.
- Enigma: AVI mit H.264/MP3 und 25 fps erfolgreich verlustfrei vorbereitet und mit MP4Box geschnitten. Die fertige MP4 enthält 162.246 Videopakete. Anfang, Mitte, Ende, Abspann und Ton wurden praktisch geprüft. Ein vollständiger Decoderlauf mit normalisierten Ausgabezeitstempeln endete ohne Fehlermeldungen.
- Bei Enigma traten beim Remux fehlende PTS-Werte und beim ersten Decoder-Prüflauf zwei DTS-Meldungen auf. Die genaue Ursache ist nicht abschließend geklärt; die praktische Qualitätskontrolle war erfolgreich.
- „Wo die Liebe hinfällt“: großer Full-HD-AVI-Container trotz Endung `.avi.mp4`, durchschnittlich 50 fps und zwei Tonspuren. Bildfehler und Probleme beim framegenauen Zurückspringen in CAN beobachtet; Tonversatz in MPC-HC berichtet. Keine erfolgreiche Schnittabnahme für diese Datei. Am 11.10.2026 bestätigt der Nutzer das Original als bereits untauglich und beauftragt ausdrücklich keine weitere Untersuchung dieser Aufnahme; er wartet auf einen neuen Sendetermin.
- Fehleranzeige mit absichtlich ungültigem ffprobe-Pfad live bestätigt.
- Kleine künstliche AVI-/MKV-Dateien: Prozessaufruf und Ergebnisprüfung, Ablehnung existierender Arbeitsdateien und vorab abgebrochener Aufträge geprüft.
- Leere Cutlist-Endbereiche sind automatisiert mit den Diplomatin-Schnittwerten abgesichert. Ein separater ausdrücklicher Live-Nachweis für genau den zuvor fehlerhaften Originaleintrag ist nicht protokolliert.

Weitere Prüfschritte stehen im [Testplan](TESTPLAN.md). Aus diesen Beispielen folgt keine allgemeine Unterstützung aller AVI-/Codec-Kombinationen.

## Abnahme und nächste Schritte

Am 11.10.2026 bestätigt der Nutzer die Punkte **„1) check / 2) check“** der gemeinsam besprochenen Aufgabenliste.

- [x] **1. Weitere Praxistests:** vom Nutzer für den aktuellen Stand mit otr-can 0.1.4 als abgeschlossen bestätigt. Die einzelnen Filmbeobachtungen und technischen Prüfgrenzen bleiben in den jeweiligen Berichten dokumentiert.
- [x] **2. AVI-Sonderfälle und Warnhinweis:** gelber Originalhinweis umgesetzt und geprüft, vom Nutzer bestätigt. Die als untauglich bestätigte alte Aufnahme „Wo die Liebe hinfällt“ wird ausdrücklich nicht weiter untersucht; der Nutzer wartet auf einen neuen Sendetermin.
- [x] **3. Nächste Veröffentlichung vorbereiten:** lokaler 0.2.1-Build-12-RC3-Paketsatz samt otr-can 0.1.4, Quellen, Lizenztexten und Prüfsummen erstellt und automatisch geprüft. Überinstallation/Versionsanzeige sowie Windows-11-VM-Installation, beide Schnittmotoren, Bild/Ton, Deinstallation und separater Portable-ZIP-Test vom Nutzer bestätigt. Persönliche Paketabnahme abgeschlossen; [Prüfbericht](RELEASE-BUILD12-RC3-RESULT.md). Noch nicht veröffentlicht, Version 1.0 weiterhin nicht festgelegt.
- [x] **4. Ältere Aufgabenlisten abgleichen:** Merkliste und Testplan auf den bestätigten Stand gebracht; ältere Build-6-Releaseunterlagen und libmpv-Lizenzprüfung als historische Nachweise eingeordnet. Timeline-Klick, Fenstergrößen, GPL-3.0-or-later-Lizenzwahl, KI-Dokumentation und Paketprüfung sind erledigt. Der KI-Absatz im Credits-Dialog wurde zusätzlich ergänzt. Der alte Juli-libmpv-Kandidat ist durch den geprüften September-Stack abgelöst, dessen Quellen-/Relink-Pakete bereits mit RC2 veröffentlicht wurden.
- [x] **5. Aktualisierten Build 13 RC3 sichern und paketieren:** Commit c98041a, neues CAN-Quellarchiv, Setup, Portable-ZIP und vollständiger Quellen-/Prüfsummensatz erstellt und technisch geprüft. Vorbestehende Lockdateiänderungen und Build-12-Pakete erhalten; [Prüfbericht](RELEASE-BUILD13-RC3-RESULT.md).

- [x] **6. Build 13 RC3 veröffentlichen:** auf ausdrücklichen Nutzerauftrag am 11.10.2026 um 18:07:42 Uhr MESZ als öffentliche Vorabversion freigegeben. Sieben Uploads, Größen und GitHub-Digests vor/nach Freigabe geprüft; Tag verweist auf c98041a. [Veröffentlichungsnachweis](RELEASE-PUBLICATION-BUILD13-RC3.md). Eine zusätzliche persönliche Build-13-Paketprüfung ist bislang nicht dokumentiert und bleibt von der Veröffentlichung getrennt.

Für V2 vorgemerkt: Klebezentrum (experimentell) für mehrteilige Aufnahmen und Feinabstimmung der Schnittbereiche gemäß chrisdudes Hinweisen. Stapelverarbeitung und Smart Rendering bleiben spätere Wünsche. Eine automatische Neukodierung ist nicht Teil des aktuellen Vorbereitungsablaufs.

Das abschließende Code-Review ist erledigt. Beide Befunde wurden behoben und getestet: widersprüchliche Cutlist-Bereiche werden mit einer verständlichen Meldung abgelehnt (`fde1970`); MP4-Ausgaben werden erst nach erfolgreichem Zusammenfügen übernommen (`cdcff86`). Bei Fehler oder Abbruch wird die temporäre Ausgabe gelöscht, eine vorhandene Zieldatei bleibt erhalten.
