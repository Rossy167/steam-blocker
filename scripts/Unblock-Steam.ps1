Get-NetFirewallRule | Where-Object DisplayName -eq 'Block-Steam Rossy' | Remove-NetFirewallRule | Out-Null
