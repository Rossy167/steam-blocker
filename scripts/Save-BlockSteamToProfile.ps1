Add-Content -Path $PROFILE -Value "
function Block-Steam {New-NetFirewallRule -Action block -Program 'C:\Program Files (x86)\Common Files\Steam\SteamService.exe' -Profile any -Direction Outbound -Displayname 'Block-Steam Rossy' | Out-Null
New-NetFirewallRule -Action block -Program 'C:\Program Files (x86)\Steam\bin\cef\cef.win7x64\steamwebhelper.exe' -Profile any -Direction Outbound -Displayname 'Block-Steam Rossy' | Out-Null
New-NetFirewallRule -Action block -Program 'C:\program files (x86)\steam\steam.exe' -Profile any -direction Outbound -Displayname 'Block-Steam Rossy' | Out-Null
New-NetFirewallRule -Action block -Program 'C:\Program Files (x86)\Common Files\Steam\SteamService.exe' -Profile any -Direction Inbound -Displayname 'Block-Steam Rossy' | Out-Null
New-NetFirewallRule -Action block -Program 'C:\Program Files (x86)\Steam\bin\cef\cef.win7x64\steamwebhelper.exe' -Profile any -Direction Inbound -Displayname 'Block-Steam Rossy' | Out-Null
New-NetFirewallRule -Action block -Program 'C:\program files (x86)\steam\steam.exe' -Profile any -direction Inbound -Displayname 'Block-Steam Rossy'}
function Unblock-Steam {Get-NetFirewallRule | Where-Object DisplayName -eq 'Block-Steam Rossy' | Remove-NetFirewallRule | Out-Null}"