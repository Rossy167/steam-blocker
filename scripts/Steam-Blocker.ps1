[CmdletBinding()]
param(
    [ValidateSet('Validate', 'Block', 'Unblock')]
    [string] $Mode = 'Validate'
)

$ErrorActionPreference = 'Stop'
$RuleGroup = 'Steam Blocker'
$RulePrefix = 'Steam Blocker - '

function Assert-Administrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        throw 'Steam Blocker must be run as Administrator.'
    }
}

function Get-RegistrySteamPath {
    $keys = @(
        'HKCU:\Software\Valve\Steam',
        'HKLM:\Software\Valve\Steam',
        'HKLM:\Software\WOW6432Node\Valve\Steam'
    )

    foreach ($key in $keys) {
        if (Test-Path $key) {
            $installPath = (Get-ItemProperty -Path $key -Name InstallPath -ErrorAction SilentlyContinue).InstallPath
            if ($installPath -and (Test-Path $installPath)) {
                return [Environment]::ExpandEnvironmentVariables($installPath)
            }
        }
    }

    return $null
}

function Get-SteamInstallPath {
    $candidates = @(
        (Get-RegistrySteamPath),
        (Join-Path $env:ProgramFiles 'Steam'),
        (Join-Path ${env:ProgramFiles(x86)} 'Steam'),
        (Join-Path $env:LOCALAPPDATA 'Programs\Steam')
    ) | Where-Object { $_ -and (Test-Path $_) } | Select-Object -Unique

    foreach ($candidate in $candidates) {
        if (Test-Path (Join-Path $candidate 'steam.exe')) {
            return (Resolve-Path $candidate).Path
        }
    }

    throw 'Steam was not found. Install Steam or verify its installation is registered in Windows.'
}

function Get-SteamProgramPaths {
    param([string] $InstallPath)

    $paths = New-Object System.Collections.Generic.List[string]
    $knownPaths = @(
        (Join-Path $InstallPath 'steam.exe'),
        (Join-Path $InstallPath 'SteamService.exe'),
        (Join-Path $InstallPath 'steamwebhelper.exe'),
        (Join-Path $InstallPath 'bin\steamwebhelper.exe'),
        (Join-Path $InstallPath 'bin\SteamService.exe'),
        (Join-Path $InstallPath 'bin\cef\steamwebhelper.exe'),
        (Join-Path $InstallPath 'bin\cef\cef.win7x64\steamwebhelper.exe'),
        (Join-Path $env:CommonProgramFiles 'Steam\SteamService.exe'),
        (Join-Path ${env:CommonProgramFiles(x86)} 'Steam\SteamService.exe'),
        (Join-Path $env:ProgramFiles 'Common Files\Steam\SteamService.exe'),
        (Join-Path ${env:ProgramFiles(x86)} 'Common Files\Steam\SteamService.exe')
    )

    foreach ($path in $knownPaths) {
        if ($path -and (Test-Path $path -PathType Leaf)) {
            $paths.Add((Resolve-Path $path).Path)
        }
    }

    $binPath = Join-Path $InstallPath 'bin'
    if (Test-Path $binPath) {
        Get-ChildItem -Path $binPath -Filter steamwebhelper.exe -File -Recurse -ErrorAction SilentlyContinue |
            ForEach-Object { $paths.Add($_.FullName) }
    }

    $paths | Sort-Object -Unique
}

function Get-RuleName {
    param([string] $ProgramPath)
    return $RulePrefix + [IO.Path]::GetFileNameWithoutExtension($ProgramPath)
}

function Remove-SteamBlockRules {
    Get-NetFirewallRule -Group $RuleGroup -ErrorAction SilentlyContinue |
        Remove-NetFirewallRule -ErrorAction SilentlyContinue
}

Assert-Administrator

if ($Mode -eq 'Unblock') {
    Remove-SteamBlockRules
    Write-Output 'Steam firewall rules removed.'
    exit 0
}

$installPath = Get-SteamInstallPath
$programPaths = @(Get-SteamProgramPaths -InstallPath $installPath)

if ($programPaths.Count -eq 0) {
    throw "Steam was found at '$installPath', but no supported Steam programs were found."
}

if ($Mode -eq 'Validate') {
    Write-Output "Steam installation: $installPath"
    Write-Output 'Programs that can be blocked:'
    $programPaths | ForEach-Object { Write-Output "- $_" }
    exit 0
}

Remove-SteamBlockRules
foreach ($programPath in $programPaths) {
    $ruleName = Get-RuleName -ProgramPath $programPath
    New-NetFirewallRule -DisplayName $ruleName -Group $RuleGroup -Direction Outbound -Action Block -Program $programPath -Profile Any -Enabled True -ErrorAction Stop | Out-Null
    New-NetFirewallRule -DisplayName "$ruleName - Inbound" -Group $RuleGroup -Direction Inbound -Action Block -Program $programPath -Profile Any -Enabled True -ErrorAction Stop | Out-Null
}

Write-Output "Steam blocked using $($programPaths.Count) discovered program(s)."
Write-Output "Installation: $installPath"
