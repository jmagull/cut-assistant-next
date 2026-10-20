[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$PrototypeExecutable,
    [Parameter(Mandatory)][string]$Ffmpeg,
    [Parameter(Mandatory)][string]$Ffprobe,
    [Parameter(Mandatory)][string]$Mp4Box,
    [Parameter(Mandatory)][string]$OutputDirectory
)
$ErrorActionPreference = 'Stop'
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $OutputDirectory) { throw 'Der neue Prüfungsordner darf noch nicht existieren.' }
[IO.Directory]::CreateDirectory($OutputDirectory) | Out-Null
foreach ($fixtureTool in @($PrototypeExecutable, $Ffmpeg, $Ffprobe, $Mp4Box)) {
    if (-not (Test-Path -LiteralPath $fixtureTool -PathType Leaf)) { throw "Werkzeug fehlt: $fixtureTool" }
}
function Invoke-FixtureTool([string]$tool, [string[]]$toolArguments, [string]$name) {
    & $tool @toolArguments > (Join-Path $OutputDirectory "$name.stdout") 2> (Join-Path $OutputDirectory "$name.stderr")
    if ($LASTEXITCODE -ne 0) { throw "$name ist fehlgeschlagen: $LASTEXITCODE" }
}
$fixtureSource = Join-Path $OutputDirectory 'source.mp4'
Invoke-FixtureTool $Ffmpeg @('-n','-v','error','-f','lavfi','-i','testsrc2=size=320x180:rate=25',
    '-t','20','-an','-c:v','libx264','-g','50','-keyint_min','50','-sc_threshold','0','-bf','2','-pix_fmt','yuv420p',$fixtureSource) 'generate'
$fixtureRanges = @('1.2:4.16','6.8:9.16','12:14.96','16.8:19.36')
$fixtureSegments = @(1..4 | ForEach-Object { Join-Path $OutputDirectory "segment-$_.mp4" })
for ($fixtureIndex = 0; $fixtureIndex -lt 4; $fixtureIndex++) {
    Invoke-FixtureTool $Mp4Box @('-p','0',$fixtureSource,'-splitx',$fixtureRanges[$fixtureIndex],'-out',$fixtureSegments[$fixtureIndex]) "split-$fixtureIndex"
}
$fixtureOutput = Join-Path $OutputDirectory 'output.mp4'
$concatArguments = @('-p','0')
foreach ($fixtureSegment in $fixtureSegments) { $concatArguments += @('-cat',$fixtureSegment) }
Invoke-FixtureTool $Mp4Box ($concatArguments + @($fixtureOutput)) 'concat'
$request = @{ FfprobePath = [IO.Path]::GetFullPath($Ffprobe); OutputFilePath = $fixtureOutput; SegmentFilePaths = $fixtureSegments }
$requestPath = Join-Path $OutputDirectory 'request.json'
$resultPath = Join-Path $OutputDirectory 'result.json'
$request | ConvertTo-Json | Set-Content -LiteralPath $requestPath
Invoke-FixtureTool $PrototypeExecutable @('--analyze',$requestPath,$resultPath) 'analyze'
$result = Get-Content -LiteralPath $resultPath -Raw | ConvertFrom-Json
$expected = @(2.2,5.4,8.4)
if ($result.Transitions.Count -ne 3) { throw 'Drei Übergänge erwartet.' }
for ($fixtureIndex = 0; $fixtureIndex -lt 3; $fixtureIndex++) {
    if ([decimal]$result.Transitions[$fixtureIndex].Seconds -ne [decimal]$expected[$fixtureIndex]) { throw 'Unerwarteter Übergang.' }
}
# A one-segment output must work without introducing a transition.
$singleOutput = Join-Path $OutputDirectory 'single-output.mp4'
Copy-Item -LiteralPath $fixtureSegments[0] -Destination $singleOutput
@{ FfprobePath=$request.FfprobePath; OutputFilePath=$singleOutput; SegmentFilePaths=@($fixtureSegments[0]) } |
    ConvertTo-Json | Set-Content -LiteralPath (Join-Path $OutputDirectory 'single-request.json')
Invoke-FixtureTool $PrototypeExecutable @('--analyze',(Join-Path $OutputDirectory 'single-request.json'),(Join-Path $OutputDirectory 'single-result.json')) 'single'
$single = Get-Content -LiteralPath (Join-Path $OutputDirectory 'single-result.json') -Raw | ConvertFrom-Json
if ($single.Transitions.Count -ne 0) { throw 'Ein Segment darf keinen Übergang erzeugen.' }
# Reordered segments must be rejected without publishing a measured result.
$request.SegmentFilePaths = @($fixtureSegments[1],$fixtureSegments[0],$fixtureSegments[2],$fixtureSegments[3])
$request | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $OutputDirectory 'wrong-order-request.json')
& $PrototypeExecutable --analyze (Join-Path $OutputDirectory 'wrong-order-request.json') (Join-Path $OutputDirectory 'must-not-exist.json') > (Join-Path $OutputDirectory 'wrong-order.stdout') 2> (Join-Path $OutputDirectory 'wrong-order.stderr')
if ($LASTEXITCODE -eq 0 -or (Test-Path -LiteralPath (Join-Path $OutputDirectory 'must-not-exist.json'))) { throw 'Falsche Segmentreihenfolge wurde akzeptiert.' }
@{ Passed=$true; ExpectedTransitions=$expected; SingleSegment=$true; WrongOrderRejected=$true;
    PlannedTransitions=@(3,5.4,8.4); Note='Unterschiedliche positive/negative Startverschiebungen, kein fester Korrekturwert.' } |
    ConvertTo-Json | Set-Content -LiteralPath (Join-Path $OutputDirectory 'FIXTURES-PROOF.json')
Write-Output 'PASS: gemischte Segmentverschiebungen, Einzelabschnitt und abgelehnte falsche Reihenfolge.'
