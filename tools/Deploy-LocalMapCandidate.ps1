[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$GameDirectory,

    [Parameter(Mandatory = $true)]
    [string]$SourcePluginDll,

    [Parameter(Mandatory = $true)]
    [string]$SourceBundle,

    [Parameter(Mandatory = $true)]
    [string]$PluginFolderName,

    [string]$GameExecutableName = "OPERATOR.exe",

    [string]$BundleFileName = "your_map_bundle",

    [string]$BuildNotesPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Require-File([string]$Path, [string]$Label) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "$Label does not exist: $Path"
    }
}

function Require-LeafName([string]$Value, [string]$Label) {
    if ([string]::IsNullOrWhiteSpace($Value) -or [IO.Path]::GetFileName($Value) -ne $Value) {
        throw "$Label must be a file name, not a path: $Value"
    }
}

Require-File $SourcePluginDll "Plugin DLL"
Require-File $SourceBundle "Bundle"
Require-LeafName $GameExecutableName "GameExecutableName"
Require-LeafName $BundleFileName "BundleFileName"

if ([IO.Path]::IsPathRooted($PluginFolderName) -or $PluginFolderName -match "(^|[\\/])\.\.([\\/]|$)") {
    throw "PluginFolderName must be a relative folder below BepInEx\\plugins."
}

$resolvedGameDirectory = (Resolve-Path -LiteralPath $GameDirectory).Path
$gameExecutablePath = Join-Path $resolvedGameDirectory $GameExecutableName
Require-File $gameExecutablePath "Game executable"

$resolvedPluginDll = (Resolve-Path -LiteralPath $SourcePluginDll).Path
$resolvedBundle = (Resolve-Path -LiteralPath $SourceBundle).Path
$pluginDirectory = Join-Path (Join-Path $resolvedGameDirectory "BepInEx\\plugins") $PluginFolderName
$pluginDestination = Join-Path $pluginDirectory ([IO.Path]::GetFileName($resolvedPluginDll))
$bundleDestination = Join-Path $pluginDirectory $BundleFileName

$runningGame = Get-CimInstance Win32_Process | Where-Object {
    $_.Name -ieq $GameExecutableName -and
    $_.ExecutablePath -and
    ([IO.Path]::GetFullPath($_.ExecutablePath) -ieq $gameExecutablePath)
}
if ($runningGame) {
    throw "The game is still running. Close it before deploying files."
}

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupDirectory = Join-Path $resolvedGameDirectory ("MapModBackups\\" + $PluginFolderName + "_" + $timestamp)
New-Item -ItemType Directory -Force -Path $backupDirectory | Out-Null
New-Item -ItemType Directory -Force -Path $pluginDirectory | Out-Null

foreach ($destination in @($pluginDestination, $bundleDestination)) {
    if (Test-Path -LiteralPath $destination -PathType Leaf) {
        Copy-Item -LiteralPath $destination -Destination $backupDirectory -Force
    }
}

if ($BuildNotesPath) {
    Require-File $BuildNotesPath "Build notes"
    $resolvedBuildNotes = (Resolve-Path -LiteralPath $BuildNotesPath).Path
    $buildNotesDestination = $bundleDestination + ".build.txt"
    if (Test-Path -LiteralPath $buildNotesDestination -PathType Leaf) {
        Copy-Item -LiteralPath $buildNotesDestination -Destination $backupDirectory -Force
    }
}

Copy-Item -LiteralPath $resolvedPluginDll -Destination $pluginDestination -Force
Copy-Item -LiteralPath $resolvedBundle -Destination $bundleDestination -Force
if ($BuildNotesPath) {
    Copy-Item -LiteralPath $resolvedBuildNotes -Destination $buildNotesDestination -Force
}

$sourcePluginHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $resolvedPluginDll).Hash
$destinationPluginHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $pluginDestination).Hash
$sourceBundleHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $resolvedBundle).Hash
$destinationBundleHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $bundleDestination).Hash

if ($sourcePluginHash -ne $destinationPluginHash -or $sourceBundleHash -ne $destinationBundleHash) {
    throw "Hash verification failed. The previous files are in: $backupDirectory"
}

Write-Host "Deployment complete."
Write-Host "Backup: $backupDirectory"
Write-Host "Plugin SHA256: $destinationPluginHash"
Write-Host "Bundle SHA256: $destinationBundleHash"
