[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$NoRestore,
    [string[]]$AdditionalArguments = @()
)

$ErrorActionPreference = 'Stop'
$repository = Split-Path -Parent $PSScriptRoot
$stateDirectory = Join-Path $repository '.build'
$counterPath = Join-Path $stateDirectory 'build-number.txt'
[IO.Directory]::CreateDirectory($stateDirectory) | Out-Null

# A second full build must not reserve the same number while this one runs.
$buildLock = [IO.File]::Open((Join-Path $stateDirectory 'build.lock'),
    [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
try {
    $previous = 0
    if (Test-Path -LiteralPath $counterPath) {
        if (-not [int]::TryParse([IO.File]::ReadAllText($counterPath).Trim(), [ref]$previous) -or $previous -lt 0) {
            throw 'Die gespeicherte Buildnummer ist ungültig.'
        }
    }
    $next = $previous + 1
    if ($next -gt 65534) {
        throw 'Die Buildnummer überschreitet den Wertebereich der Windows-Dateiversion.'
    }

    $arguments = @('build', (Join-Path $repository 'CutAssistantNext.sln'),
        '--configuration', $Configuration, '-t:Rebuild')
    if ($NoRestore) { $arguments += '--no-restore' }
    $arguments += $AdditionalArguments
    $arguments += "-p:CanBuildNumber=$next"

    Write-Host "Vollständiger Build $next wird erstellt ..."
    & dotnet @arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Build fehlgeschlagen. Die gespeicherte Buildnummer bleibt $previous."
    }

    $pendingPath = Join-Path $stateDirectory 'build-number.new'
    [IO.File]::WriteAllText($pendingPath, [string]$next)
    if (Test-Path -LiteralPath $counterPath) {
        [IO.File]::Replace($pendingPath, $counterPath, (Join-Path $stateDirectory 'build-number.previous'))
    } else {
        [IO.File]::Move($pendingPath, $counterPath)
    }
    Write-Host "Build $next erfolgreich abgeschlossen."
} finally {
    $buildLock.Dispose()
}
