[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][ValidateRange(0, 65534)][int]$BuildNumber,
    [Parameter(Mandatory = $true)][string]$NativeSourceDirectory,
    [Parameter(Mandatory = $true)][string]$SourceArchivePath,
    [Parameter(Mandatory = $true)][string]$OutputDirectory,
    [Parameter(Mandatory = $true)][string]$InnoCompilerPath,
    [ValidatePattern('^[A-Za-z0-9][A-Za-z0-9.-]*$')][string]$Candidate = 'RC1'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
$utf8 = New-Object System.Text.UTF8Encoding($false)
$sourceArchive = (Resolve-Path -LiteralPath $SourceArchivePath).Path
$compiler = (Resolve-Path -LiteralPath $InnoCompilerPath).Path
$output = [System.IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $output) { throw 'OutputDirectory must be new; existing results are preserved.' }
if (-not (Test-Path -LiteralPath (Join-Path $repo 'SOURCE-FILES.sha256'))) { throw 'Run this script from the extracted CAN source snapshot.' }
$snapshot = Get-Content -LiteralPath (Join-Path $repo 'SOURCE-SNAPSHOT.json') -Encoding UTF8 -Raw | ConvertFrom-Json
if ($snapshot.BuildNumber -ne $BuildNumber) { throw 'Build number differs from the source snapshot.' }
[xml]$versionXml = Get-Content -LiteralPath (Join-Path $repo 'Version.props') -Encoding UTF8 -Raw
$version = [string]$versionXml.Project.PropertyGroup.Version
if ($snapshot.Version -ne $version) { throw 'Version differs from the source snapshot.' }

function Assert-SourceSnapshot {
    foreach ($line in (Get-Content -LiteralPath (Join-Path $repo 'SOURCE-FILES.sha256') -Encoding UTF8)) {
        if ($line -notmatch '^([0-9A-Fa-f]{64})  (.+)$') { throw 'Invalid source manifest entry.' }
        $expected = $Matches[1]
        $path = [System.IO.Path]::GetFullPath((Join-Path $repo $Matches[2]))
        if (-not $path.StartsWith($repo.TrimEnd('\') + '\', [System.StringComparison]::OrdinalIgnoreCase)) { throw 'Source entry escapes snapshot.' }
        if ((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $expected) { throw "Source changed: $path" }
    }
}
Assert-SourceSnapshot
Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::OpenRead($sourceArchive)
try {
    $entries = @($archive.Entries | Where-Object { $_.FullName -match '/SOURCE-FILES.sha256$' })
    if ($entries.Count -ne 1) { throw 'Source ZIP has no unique source manifest.' }
    $stream = $entries[0].Open()
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try { $archivedManifestHash = [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-', '') }
    finally { $sha.Dispose(); $stream.Dispose() }
} finally { $archive.Dispose() }
if ($archivedManifestHash -ne (Get-FileHash -LiteralPath (Join-Path $repo 'SOURCE-FILES.sha256') -Algorithm SHA256).Hash) { throw 'Source ZIP does not match this snapshot.' }

[System.IO.Directory]::CreateDirectory($output) | Out-Null
$logs = Join-Path $output 'logs'
[System.IO.Directory]::CreateDirectory($logs) | Out-Null
$publish = Join-Path $output 'publish'
$dotnet = (Get-Command dotnet -ErrorAction Stop).Source
function Invoke-Step([string]$Name, [string]$Program, [string[]]$Arguments) {
    Write-Host "Starting $Name"
    $log = Join-Path $logs ($Name + '.log')
    $savedPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        & $Program @Arguments 2>&1 | Out-File -LiteralPath $log -Encoding UTF8
        $code = $LASTEXITCODE
    } finally { $ErrorActionPreference = $savedPreference }
    Get-Content -LiteralPath $log -Encoding UTF8 -Tail 12 | Write-Host
    if ($code -ne 0) { throw "$Name failed with exit code $code; see $log" }
}
& (Join-Path $PSScriptRoot 'setup-libmpv.ps1') -SourceDirectory $NativeSourceDirectory
$solution = Join-Path $repo 'CutAssistantNext.sln'
$app = Join-Path $repo 'src\CutAssistantNext.App\CutAssistantNext.App.csproj'
Invoke-Step 'restore' $dotnet @('restore', $solution, '-r', 'win-x64', '--locked-mode')
Invoke-Step 'build' $dotnet @('build', $solution, '-c', 'Release', '--no-restore', "-p:CanBuildNumber=$BuildNumber", "-p:CanCandidate=$Candidate")
Invoke-Step 'tests' $dotnet @('test', $solution, '-c', 'Release', '--no-build', '--no-restore', '--logger', 'trx', '--results-directory', (Join-Path $output 'test-results'))
Invoke-Step 'publish' $dotnet @('publish', $app, '-c', 'Release', '-r', 'win-x64', '--self-contained', 'false', "-p:CanBuildNumber=$BuildNumber", "-p:CanCandidate=$Candidate", '-p:RestoreLockedMode=true', '-o', $publish)

$forbidden = @(Get-ChildItem -LiteralPath $publish -File -Recurse | Where-Object {
    $_.Name -match '^(coreclr|hostfxr|hostpolicy|PresentationNative_cor3|wpfgfx_cor3|vcruntime140_cor3|D3DCompiler_47_cor3|Microsoft\.DiaSymReader\.Native\..+|ffms2)\.dll$|^(createdump|ffmpeg|ffprobe|mp4box|ffmsindex)\.exe$'
})
if ($forbidden.Count -gt 0) { throw ('Unexpected bundled runtime/tool files: ' + ($forbidden.Name -join ', ')) }
$runtime = Get-Content -LiteralPath (Join-Path $publish 'CutAssistantNext.App.runtimeconfig.json') -Encoding UTF8 -Raw | ConvertFrom-Json
if ($runtime.runtimeOptions.PSObject.Properties['includedFrameworks']) { throw 'Self-contained runtime configuration found.' }
$frameworkNames = @($runtime.runtimeOptions.frameworks | ForEach-Object { $_.name })
if ('Microsoft.NETCore.App' -notin $frameworkNames -or 'Microsoft.WindowsDesktop.App' -notin $frameworkNames) { throw 'Required shared frameworks missing.' }
foreach ($name in @('libmpv-2.dll', 'libc++.dll', 'libshaderc_shared.dll', 'libspirv-cross-c-shared.dll')) {
    if ((Get-FileHash -LiteralPath (Join-Path $publish $name) -Algorithm SHA256).Hash -ne (Get-FileHash -LiteralPath (Join-Path $NativeSourceDirectory $name) -Algorithm SHA256).Hash) { throw "Native DLL differs: $name" }
}
foreach ($name in @('LICENSE', 'third-party\DOTNET-LICENSING.md', 'third-party\licenses\nuget\MIT-NOTICES.txt', 'third-party\libmpv\v0.41.0\THIRD-PARTY-NOTICES.md')) {
    if (-not (Test-Path -LiteralPath (Join-Path $publish $name))) { throw "Missing notice: $name" }
}
$fileVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo((Join-Path $publish 'CutAssistantNext.App.exe')).FileVersion
if ($fileVersion -ne "$version.$BuildNumber") { throw "Unexpected file version: $fileVersion" }
Copy-Item -LiteralPath (Join-Path $repo 'docs\NUTZERANLEITUNG.md') -Destination (Join-Path $publish 'NUTZERANLEITUNG.md')
Copy-Item -LiteralPath (Join-Path $repo 'installer\HINWEIS-DOTNET-UND-WERKZEUGE.txt') -Destination (Join-Path $publish 'VORAUSSETZUNGEN.txt')
$sourceHash = (Get-FileHash -LiteralPath $sourceArchive -Algorithm SHA256).Hash
$provenance = [ordered]@{
    Version = $version; BuildNumber = $BuildNumber; Candidate = $Candidate
    SourceArchive = [System.IO.Path]::GetFileName($sourceArchive); SourceArchiveSha256 = $sourceHash
    SourceFilesManifestSha256 = $archivedManifestHash; BaseGitCommit = $snapshot.BaseGitCommit
    IncludesWorkingTreeChanges = $snapshot.IncludesWorkingTreeChanges
    DotnetSdk = (& $dotnet --version | Out-String).Trim()
    InnoCompilerSha256 = (Get-FileHash -LiteralPath $compiler -Algorithm SHA256).Hash
}
[System.IO.File]::WriteAllText((Join-Path $publish 'RELEASE-PROVENANCE.json'), ($provenance | ConvertTo-Json), $utf8)
Assert-SourceSnapshot
$fileLines = @(Get-ChildItem -LiteralPath $publish -File -Recurse | Sort-Object FullName | ForEach-Object {
    '{0}  {1}' -f (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash, $_.FullName.Substring($publish.Length + 1).Replace('\', '/')
})
[System.IO.File]::WriteAllLines((Join-Path $output 'PUBLISH-FILES.sha256'), $fileLines, $utf8)
$baseName = "CutAssistantNext-$version-Build$BuildNumber-win-x64"
$zipPath = Join-Path $output "$baseName-$Candidate.zip"
[System.IO.Compression.ZipFile]::CreateFromDirectory($publish, $zipPath, [System.IO.Compression.CompressionLevel]::Optimal, $false)
Invoke-Step 'installer' $compiler @("--define=CanPublishDir=$publish", "--define=CanOutputDir=$output", "--define=CanVersion=$version", "--define=CanBuildNumber=$BuildNumber", "--define=CanCandidate=$Candidate", (Join-Path $repo 'installer\CutAssistantNext-RELEASE.iss'))
$setupPath = Join-Path $output "$baseName-Setup-$Candidate.exe"
if (-not (Test-Path -LiteralPath $setupPath)) { throw 'Installer output missing.' }
Copy-Item -LiteralPath $sourceArchive -Destination (Join-Path $output ([System.IO.Path]::GetFileName($sourceArchive)))
$artifactLines = @($sourceArchive, $zipPath, $setupPath) | ForEach-Object { '{0}  {1}' -f (Get-FileHash -LiteralPath $_ -Algorithm SHA256).Hash, [System.IO.Path]::GetFileName($_) }
[System.IO.File]::WriteAllLines((Join-Path $output 'SHA256SUMS.txt'), [string[]]$artifactLines, $utf8)
Assert-SourceSnapshot
Write-Host "Release candidate ready: $output"
