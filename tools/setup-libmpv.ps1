[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$releaseTag =
    "2026-07-30-74356c0fc6"

$archiveName =
    "mpv-dev-lgpl-x86_64-20260730-git-74356c0fc6.7z"

$expectedArchiveSha256 =
    "a0a74229523685ba364d0c93168fa3a03e1af5b84a5bca592833b28aa9fe0023"

$downloadUrl =
    "https://github.com/zhongfly/mpv-winbuild/releases/download/" +
    "$releaseTag/$archiveName"

$repositoryRoot =
    Split-Path -Parent $PSScriptRoot

$destinationDirectory = Join-Path `
    $repositoryRoot `
    "external\libmpv\win-x64"

$destinationDll = Join-Path `
    $destinationDirectory `
    "libmpv-2.dll"

function Find-SevenZip
{
    $candidates = @(
        (
            Get-Command `
                7z.exe `
                -ErrorAction SilentlyContinue |
                Select-Object `
                    -ExpandProperty Source `
                    -First 1
        ),
        "$env:ProgramFiles\7-Zip\7z.exe",
        "${env:ProgramFiles(x86)}\7-Zip\7z.exe"
    ) |
        Where-Object {
            $_ -and (Test-Path $_ -PathType Leaf)
        } |
        Select-Object -Unique

    $sevenZip = $candidates |
        Select-Object -First 1

    if (-not $sevenZip)
    {
        throw (
            "7-Zip wurde nicht gefunden. " +
            "Erwartet wird insbesondere " +
            "'C:\Program Files\7-Zip\7z.exe'."
        )
    }

    return $sevenZip
}

$tempDirectory = Join-Path `
    ([System.IO.Path]::GetTempPath()) `
    (
        "cut-assistant-libmpv-" +
        [Guid]::NewGuid().ToString("N")
    )

try
{
    [System.IO.Directory]::CreateDirectory(
        $tempDirectory) |
        Out-Null

    $archivePath = Join-Path `
        $tempDirectory `
        $archiveName

    $extractDirectory = Join-Path `
        $tempDirectory `
        "extracted"

    [System.IO.Directory]::CreateDirectory(
        $extractDirectory) |
        Out-Null

    Write-Host "Lade fest versioniertes libmpv-Archiv herunter:"
    Write-Host "  Release: $releaseTag"
    Write-Host "  Archiv:  $archiveName"

    Invoke-WebRequest `
        -Uri $downloadUrl `
        -OutFile $archivePath `
        -Headers @{
            "User-Agent" = "CutAssistantNext-PoC"
        }

    $actualArchiveSha256 = (
        Get-FileHash `
            -Path $archivePath `
            -Algorithm SHA256
    ).Hash.ToLowerInvariant()

    if ($actualArchiveSha256 -ne $expectedArchiveSha256)
    {
        throw (
            "Die SHA-256-Prüfsumme stimmt nicht überein." +
            [Environment]::NewLine +
            "Erwartet: $expectedArchiveSha256" +
            [Environment]::NewLine +
            "Ermittelt: $actualArchiveSha256"
        )
    }

    Write-Host "SHA-256-Prüfung erfolgreich."

    $sevenZip = Find-SevenZip

    Write-Host "Entpacke mit:"
    Write-Host "  $sevenZip"

    & $sevenZip `
        "x" `
        $archivePath `
        "-o$extractDirectory" `
        "-y" |
        Out-Host

    if ($LASTEXITCODE -ne 0)
    {
        throw (
            "7-Zip wurde mit Exit-Code " +
            "$LASTEXITCODE beendet."
        )
    }

    $dllCandidates = @(
        Get-ChildItem `
            -Path $extractDirectory `
            -Recurse `
            -File `
            -Filter "libmpv-2.dll"
    )

    if ($dllCandidates.Count -eq 0)
    {
        throw (
            "Im Archiv wurde keine " +
            "'libmpv-2.dll' gefunden."
        )
    }

    if ($dllCandidates.Count -gt 1)
    {
        $candidatePaths = $dllCandidates.FullName -join `
            [Environment]::NewLine

        throw (
            "Im Archiv wurden mehrere libmpv-DLLs gefunden:" +
            [Environment]::NewLine +
            $candidatePaths
        )
    }

    [System.IO.Directory]::CreateDirectory(
        $destinationDirectory) |
        Out-Null

    Copy-Item `
        -Path $dllCandidates[0].FullName `
        -Destination $destinationDll `
        -Force

    if (-not (
        Test-Path `
            $destinationDll `
            -PathType Leaf))
    {
        throw (
            "Die libmpv-DLL wurde nicht am erwarteten " +
            "Ziel abgelegt."
        )
    }

    $destinationFile = Get-Item $destinationDll

    Write-Host
    Write-Host "libmpv wurde erfolgreich bereitgestellt:"
    Write-Host "  Datei: $($destinationFile.FullName)"
    Write-Host "  Größe: $($destinationFile.Length) Bytes"
    Write-Host "  Quelle: $downloadUrl"
    Write-Host "  Archiv-SHA-256: $expectedArchiveSha256"
}
finally
{
    if (Test-Path $tempDirectory)
    {
        Remove-Item `
            -Path $tempDirectory `
            -Recurse `
            -Force `
            -ErrorAction SilentlyContinue
    }
}