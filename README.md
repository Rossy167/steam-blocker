# steam-blocker

Quick scripts to block Steam on Windows.

## Can has .exe

Download the .exe files here, you can run these as Admin to block and unblock Steam.
https://github.com/Rossy167/steam-blocker/releases/tag/executable

## What do?
A quick set of scripts to block the Steam on the Windows Firewall, and then unblock it afterwards. I'm sure there's many reasons to do this, but for me it's just so that multiple users can use a family sharing at the same time, without Steam blocking it. It gets around this by stopping Steam from accessing the internet preventing it from knowing who is playing what.

## TL;DR

Block-Steam blocks Steam, Unblock-Steam deletes the Firewall rules set from Block-Steam, Save-BlockSteamToProfile saves it to our Powershell profile so you can do it directly from the shell rather than running the PS1 files. 

To run it: 
* Press the Windows key
* Type "powershell"
* Right click on "powershell" and select "run as administrator"
* Paste the following: `Set-ExecutionPolicy unrestricted`
* Run `Block-Steam.ps1`
* Paste the following: `Set-ExecutionPolicy restricted`

## Resources Used
* https://gallery.technet.microsoft.com/scriptcenter/PS2EXE-GUI-Convert-e7cb69d5
* https://docs.microsoft.com/en-us/powershell/module/netsecurity/new-netfirewallrule?view=win10-ps