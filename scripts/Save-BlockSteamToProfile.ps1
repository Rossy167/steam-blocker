$enginePath = Join-Path $PSScriptRoot 'Steam-Blocker.ps1'
if (-not (Test-Path $enginePath)) {
    throw "Steam-Blocker.ps1 was not found in '$PSScriptRoot'."
}

$profileDirectory = Split-Path -Parent $PROFILE
if (-not (Test-Path $profileDirectory)) {
    New-Item -Path $profileDirectory -ItemType Directory -Force | Out-Null
}

$escapedEnginePath = $enginePath.Replace("'", "''")
$profileFunctions = @"
function Block-Steam {
    & '$escapedEnginePath' -Mode Block
}
function Unblock-Steam {
    & '$escapedEnginePath' -Mode Unblock
}
function Test-SteamBlocker {
    & '$escapedEnginePath' -Mode Validate
}
"@

Add-Content -Path $PROFILE -Value $profileFunctions
Write-Output "Steam Blocker commands added to $PROFILE"
