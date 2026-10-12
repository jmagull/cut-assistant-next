# Veröffentlichung: CAN 0.2.1 Build 16 – RC3

Die Vorabversion wurde auf ausdrücklichen Auftrag **5) go :-)** am **12.10.2026 15:26:56 Uhr MESZ** veröffentlicht (GitHub: 2026-10-12T13:26:56Z).

- [GitHub-Release v0.2.1-build16-rc3](https://github.com/jmagull/cut-assistant-next/releases/tag/v0.2.1-build16-rc3)
- [Prüfsummen](https://github.com/jmagull/cut-assistant-next/releases/download/v0.2.1-build16-rc3/SHA256SUMS.txt)
- [Paketprüfbericht](RELEASE-BUILD16-RC3-RESULT.md)

Der annotierte Tag verweist auf den tatsächlich gebauten CAN-Quellcommit **603d64d5c6374eea96ec5920719443fc6d399d81**. Motorquellstand: **2a474629836d52023bf37cbbdb309ccd1c23acf3**, Version **0.1.5**. Beide Quellstände wurden übertragen; der separate Motor bleibt im bestehenden privaten Repository, seine passenden Quellen sind öffentlich als Release-Download und im Paket enthalten. Die nachfolgenden Dokumentationscommits 6f5807d und d0236bb wurden ebenfalls gesichert. Tag und eingefrorenes Quellenarchiv bleiben beim gebauten Stand.

**Sieben Dateien** veröffentlicht: Setup, Portable-ZIP, CAN-Quellen, otr-can-Quellen, libmpv-Quellen-/Prüfpaket, libmpv-Relink-Paket und SHA256SUMS.txt. Vor der Freigabe im Entwurf und anschließend öffentlich wurden alle Größen und GitHub-SHA-256-Digests mit den lokalen Dateien abgeglichen. Release-Text und Quellziel stimmen überein. GitHub bestätigt **draft: false**, **prerelease: true** und Release-ID **410623454**. Nicht als stabile Latest-Ausgabe markiert; frühere Releases nicht verändert.

**Prüfstand:** Build ohne Warnungen/Fehler, **793/793 CAN-Tests**, exportierte Motorquellen gebaut mit **44/44 Rust-Tests**, alle **249 Portable-Dateien** geprüft. Nutzer bestätigt frische Setup-Installation in der VM, beide Motoren und Deinstallation sowie Portable-ZIP in der VM mit beiden Motoren. Ein Setup-AVI-Ergebnis wurde zusätzlich für Anfang, Ende, alle Übergänge, Bild und Ton ausführlich positiv bewertet. Portable-Anhänge belegen MP4Box; otr-can-Erfolg folgt aus ausdrücklicher Nutzerbestätigung. Kein gesonderter Überinstallationstest oder frisches Portable-Profil daraus abgeleitet.

**Bekannte Grenzen:** AVI bleibt experimentell. Teufelsbrigade-Remux meldet fehlende PTS; strenge Video-Dekodierung zeigt einen bereits vorbestehenden Zeitstempelbefund. Die persönliche positive Bewertung ersetzt diesen technischen Nachweis nicht. MP3 wird ohne zusätzliche AAC-Vorkodierung erst nach dem PCM-Schnitt einmal verlustbehaftet nach AAC-LC kodiert. Die geprüfte Motor-EXE bleibt unverändert; Quellen-Kontrollbuild ist nicht bytegleich, keine bytegenaue Reproduzierbarkeit behauptet. Details und Voraussetzungen stehen im Release-Text und [AVI-/MP3-Prüfbericht](AVI-OTR-CAN-MP3-20261012.md).

Nach Freigabe nur Dokumentation aktualisiert. Keine Binärdatei, kein Archiv, keine Prüfsumme oder Buildnummer neu erstellt. Die vier vorbestehenden Produktions-Lockdateiänderungen bleiben lokal erhalten und wurden nicht übertragen. Lokale Belege: .build/release-build16-RC3/GITHUB-DRAFT-PROOF.json, GITHUB-PUBLISHED-PROOF.json, UPLOAD-EXPECTED.json und UPLOAD.log.
