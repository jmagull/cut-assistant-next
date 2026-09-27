[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $SourceDirectory
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Exact tested CAN-libmpv 0.41.0 candidate; never accept a later upstream build silently.
$expectedHashes = [ordered]@{
    'libmpv-2.dll' = '95A37097C6C7EADF7098A0247AC9028143DA26E14081A76A7F6575C1A5AB22EA'
    'libc++.dll' = '7344DAED05388589E9BD691ED1D30C568C374DA4B8B6A12E1502185948C03CD4'
    'libshaderc_shared.dll' = 'A66696E62D2207B259E33BC3E00E02C55B77F74048872596805B8D82BCDABFB6'
    'libspirv-cross-c-shared.dll' = '2AF800CBBD27CEA876227DD3903F7FB662652BBC86E2BA9F92F13E501F1E637A'
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$destinationDirectory = Join-Path $repositoryRoot 'external\libmpv\can-v0.41.0'
$source = (Resolve-Path -LiteralPath $SourceDirectory).Path

foreach ($name in $expectedHashes.Keys) {
    $path = Join-Path $source $name
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Im Quellordner fehlt $name."
    }
    $actual = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
    if ($actual -ne $expectedHashes[$name]) {
        throw "$name hat nicht den geprüften SHA-256-Wert. Erwartet: $($expectedHashes[$name]); gefunden: $actual"
    }
}

[System.IO.Directory]::CreateDirectory($destinationDirectory) | Out-Null
foreach ($name in $expectedHashes.Keys) {
    Copy-Item -LiteralPath (Join-Path $source $name) `
        -Destination (Join-Path $destinationDirectory $name) -Force
}

foreach ($name in $expectedHashes.Keys) {
    $actual = (Get-FileHash -LiteralPath (Join-Path $destinationDirectory $name) -Algorithm SHA256).Hash
    if ($actual -ne $expectedHashes[$name]) {
        throw "Kopierprüfung fehlgeschlagen: $name"
    }
}

Write-Host 'CAN-libmpv 0.41.0: vier verifizierte DLLs bereitgestellt.'
Write-Host "Ziel: $destinationDirectory"
