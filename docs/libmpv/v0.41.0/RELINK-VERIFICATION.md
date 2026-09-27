# Relink-Prüfung des CAN-libmpv-0.41.0-Kandidaten

Prüfumgebung: Windows x64, PowerShell 5.1, MSYS2 CLANG64 Clang/LLD 22.1.8.
Die Testwerkzeuge mussten innerhalb der Codex-Umgebung in einen erlaubten
Arbeitsordner kopiert werden; ihre Version und die zugrunde liegenden
CLANG64-Laufzeitdateien blieben unverändert. Das Paket selbst enthält diese
Kopien der Werkzeuge nicht.

1. Die 216 mpv-Objektdateien und die 63 benötigten Archive wurden aus dem
   zum Referenz-DLL-Build gehörenden Build-Baum übernommen. Die gesicherte
   `build.ninja` aus dem Quellen-Prüfpaket ist mit dessen aktuellem Stand
   bytegleich. Die Referenz-DLL hatte danach weiter den dokumentierten Hash
   `95A37097C6C7EADF7098A0247AC9028143DA26E14081A76A7F6575C1A5AB22EA`.
2. `relink.ps1` verknüpfte die unveränderten Eingaben erfolgreich. Die neue
   DLL meldete Client-API `0x20005`; `mpv_create()` und `mpv_initialize()`
   waren erfolgreich, `mpv_terminate_destroy()` lief anschließend.
3. `verify-replacement.ps1` erzeugte eine veränderte Kopie von
   `libfribidi.a`: Original-SHA-256
   `0E0C296C7809172DDE58D0A549AE4DFA5354AA88611149947961EB3227D72743`,
   Testkopie-SHA-256
   `251E279D57714EFD329419B1460DF4DF8D1BE21FB0669D8A1D27FBB9AD8923A7`.
   Der erneute Link mit dieser Kopie gelang. `llvm-readobj` fand den neuen
   Export `can_relink_probe_marker`; der aufgerufene Marker gab `4242`
   zurück. Auch diese DLL meldete API `0x20005` und initialisierte mpv
   erfolgreich.
4. Die vier weiteren LGPL-Link-Eingänge `libplacebo.a`, `libglib-2.0.a`,
   `libiconv.a` und `libintl.a` wurden jeweils einzeln über die
   Ersatzarchiv-Option verknüpft. Mit einer bytegleichen Kopie des jeweiligen
   Archivs gelang der Link und der mpv-Initialisierungstest in allen vier
   Fällen. Die inhaltlich geänderte Archivprobe wurde bei FriBidi durchgeführt.

Der Test belegt, dass ein geändertes LGPL-Archiv anstelle des Originals in
die DLL gelangt. Er beansprucht keine Bitgleichheit der neu verknüpften DLL
mit dem Ausgangsbuild und ersetzt keine gesonderte Prüfung jeder denkbaren
Änderung an den fünf Bibliotheken. Die ausführbaren Prüfschritte stehen in
`relink.ps1`, `verify-replacement.ps1` und `tests/marker.c`.
