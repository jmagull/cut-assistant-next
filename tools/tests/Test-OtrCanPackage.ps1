[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$EngineExecutablePath,
    [Parameter(Mandatory = $true)][string]$ScratchDirectory
)

# Exercise the actual packaging block, without restoring, publishing or running Inno Setup.
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$engine = (Resolve-Path -LiteralPath $EngineExecutablePath).Path
$root = [IO.Path]::GetFullPath($ScratchDirectory)
if (Test-Path -LiteralPath $root) { throw 'Test scratch directory must be new.' }
[IO.Directory]::CreateDirectory($root) | Out-Null
$utf8 = New-Object Text.UTF8Encoding($false)
$tokens = $null
$parseErrors = $null
$ast = [Management.Automation.Language.Parser]::ParseFile((Join-Path $repo 'tools/package-release.ps1'), [ref]$tokens, [ref]$parseErrors)
if ($parseErrors.Count) { throw ($parseErrors | Out-String) }
$blocks = @($ast.FindAll({ param($node)
    $node -is [Management.Automation.Language.IfStatementAst] -and
    $node.Extent.Text.StartsWith('if ($OtrCanPackageDirectory)')
}, $true))
$steps = @($ast.FindAll({ param($node)
    $node -is [Management.Automation.Language.FunctionDefinitionAst] -and $node.Name -eq 'Invoke-Step'
}, $true))
if ($blocks.Count -ne 1 -or $steps.Count -ne 1) { throw 'Cannot locate the unique packaging block and tool runner.' }
$validate = [ScriptBlock]::Create($blocks[0].Extent.Text)
Invoke-Expression ($steps[0].Extent.Text.Replace('function Invoke-Step', 'function Invoke-ActualStep'))

# Only negative cases override probe output. The positive case executes the copied real EXE.
$script:CapabilityOverride = $null
$script:VersionOverride = $null
function Invoke-Step([string]$Name, [string]$Program, [string[]]$Arguments) {
    $override = $null
    if ($Name -eq 'motor-capabilities') { $override = $script:CapabilityOverride }
    if ($Name -eq 'motor-version') { $override = $script:VersionOverride }
    if ($null -ne $override) {
        [IO.File]::WriteAllText((Join-Path $logs ($Name + '.log')), $override, $utf8)
        return
    }
    Invoke-ActualStep $Name $Program $Arguments
}

function Write-Provenance($package, $value) {
    [IO.File]::WriteAllText((Join-Path $package 'ENGINE-PROVENANCE.json'), ($value | ConvertTo-Json), $utf8)
}
function Write-Manifest($package) {
    $lines = @(Get-ChildItem -LiteralPath $package -File -Recurse | Where-Object { $_.Name -ne 'ENGINE-FILES.sha256' } | Sort-Object FullName | ForEach-Object {
        '{0}  {1}' -f (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash, $_.FullName.Substring($package.Length + 1).Replace('\', '/')
    })
    [IO.File]::WriteAllLines((Join-Path $package 'ENGINE-FILES.sha256'), [string[]]$lines, $utf8)
}
function New-Fixture($caseDirectory) {
    $package = Join-Path $caseDirectory 'motor package'
    [IO.Directory]::CreateDirectory((Join-Path $package 'sources')) | Out-Null
    Copy-Item -LiteralPath $engine -Destination (Join-Path $package 'otr_can.exe')
    foreach ($name in @('LICENSE', 'THIRD-PARTY-NOTICES.md')) {
        [IO.File]::WriteAllText((Join-Path $package $name), 'Synthetic test fixture, not a distribution package.', $utf8)
    }
    # A source checksum fixture; no real sources/release archives are prepared here.
    [IO.File]::WriteAllText((Join-Path $package 'sources/otr-can-0.1.6-source.zip'), 'source checksum fixture', $utf8)
    $provenance = [PSCustomObject]@{
        Version = '0.1.6'
        ExecutableSha256 = (Get-FileHash -LiteralPath (Join-Path $package 'otr_can.exe') -Algorithm SHA256).Hash
        SourceArchiveSha256 = (Get-FileHash -LiteralPath (Join-Path $package 'sources/otr-can-0.1.6-source.zip') -Algorithm SHA256).Hash
    }
    Write-Provenance $package $provenance
    Write-Manifest $package
    return $package
}

$cases = @('no-motor', 'valid', 'old-snapshot', 'old-provenance', 'missing-source', 'changed-exe', 'wrong-source-hash', 'escape-manifest', 'missing-capability', 'missing-overlap-capability', 'wrong-cli-version')
$results = @()
foreach ($case in $cases) {
    $caseDirectory = Join-Path $root $case
    [IO.Directory]::CreateDirectory($caseDirectory) | Out-Null
    $logs = Join-Path $caseDirectory 'logs'
    $publish = Join-Path $caseDirectory 'publish'
    [IO.Directory]::CreateDirectory($logs) | Out-Null
    [IO.Directory]::CreateDirectory($publish) | Out-Null
    $snapshot = [PSCustomObject]@{ OtrCanVersion = '0.1.6' }
    $OtrCanPackageDirectory = if ($case -eq 'no-motor') { '' } else { New-Fixture $caseDirectory }
    $script:CapabilityOverride = $null
    $script:VersionOverride = $null
    $expectedError = $null
    switch ($case) {
        'old-snapshot' { $snapshot.OtrCanVersion = '0.1.5'; $expectedError = 'source snapshot declares a different motor version' }
        'old-provenance' {
            $provenance = Get-Content -LiteralPath (Join-Path $OtrCanPackageDirectory 'ENGINE-PROVENANCE.json') -Raw | ConvertFrom-Json
            $provenance.Version = '0.1.5'
            Write-Provenance $OtrCanPackageDirectory $provenance
            Write-Manifest $OtrCanPackageDirectory
            $expectedError = 'Unexpected motor package version'
        }
        'missing-source' {
            Remove-Item -LiteralPath (Join-Path $OtrCanPackageDirectory 'sources/otr-can-0.1.6-source.zip')
            $expectedError = 'Missing motor package file'
        }
        'changed-exe' {
            [IO.File]::AppendAllText((Join-Path $OtrCanPackageDirectory 'otr_can.exe'), 'changed', $utf8)
            $expectedError = 'Motor package changed'
        }
        'wrong-source-hash' {
            $provenance = Get-Content -LiteralPath (Join-Path $OtrCanPackageDirectory 'ENGINE-PROVENANCE.json') -Raw | ConvertFrom-Json
            $provenance.SourceArchiveSha256 = '0' * 64
            Write-Provenance $OtrCanPackageDirectory $provenance
            Write-Manifest $OtrCanPackageDirectory
            $expectedError = 'Motor sources differ from provenance'
        }
        'escape-manifest' {
            [IO.File]::WriteAllText((Join-Path $OtrCanPackageDirectory 'ENGINE-FILES.sha256'), ('0' * 64) + '  ../outside.txt', $utf8)
            $expectedError = 'Motor manifest entry escapes package'
        }
        'missing-capability' { $script:CapabilityOverride = 'not-mp3-aac-v1'; $expectedError = 'does not report the required MP3-to-AAC capability' }
        'missing-overlap-capability' { $script:CapabilityOverride = 'mp3-aac-v1'; $expectedError = 'does not report the required AAC-overlap capability' }
        'wrong-cli-version' { $script:VersionOverride = 'otr_can 0.1.5'; $expectedError = 'Bundled motor version check failed' }
    }
    $errorMessage = $null
    try { & $validate | Out-Null }
    catch { $errorMessage = $_.Exception.Message }
    if ($expectedError) {
        if (!$errorMessage -or $errorMessage -notlike ('*' + $expectedError + '*')) { throw "Case ${case}: expected '$expectedError', got '$errorMessage'." }
    } elseif ($errorMessage) { throw "Case $case unexpectedly failed: $errorMessage" }
    if ($case -eq 'valid') {
        foreach ($file in (Get-ChildItem -LiteralPath $OtrCanPackageDirectory -File -Recurse)) {
            $relative = $file.FullName.Substring($OtrCanPackageDirectory.Length + 1)
            $copy = Join-Path (Join-Path $publish 'tools/otr-can') $relative
            if ((Get-FileHash -LiteralPath $copy -Algorithm SHA256).Hash -ne (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash) { throw 'Copy changed fixture contents.' }
        }
        if (@((Get-Content -LiteralPath (Join-Path $logs 'motor-capabilities.log') -Raw) -split '\s+') -notcontains 'mp3-aac-v1') { throw 'Real capability probe failed.' }
        if (@((Get-Content -LiteralPath (Join-Path $logs 'motor-capabilities.log') -Raw) -split '\s+') -notcontains 'aac-overlap-v1') { throw 'Real AAC-overlap capability probe failed.' }
    }
    if ($case -eq 'no-motor' -and ((Test-Path -LiteralPath (Join-Path $publish 'tools/otr-can')) -or @(Get-ChildItem -LiteralPath $logs).Count)) { throw 'Optional motor was unexpectedly used.' }
    $results += [PSCustomObject]@{ Case = $case; Passed = $true; ExpectedRejection = [bool]$expectedError; Message = $errorMessage }
    Write-Host "PASS $case"
}
[IO.File]::WriteAllText((Join-Path $root 'RESULT.json'), ($results | ConvertTo-Json -Depth 4), $utf8)
Write-Host "All $($results.Count) motor packaging checks passed. No setup or release package created."