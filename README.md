## steam-blocker

Quick scripts to block Steam on Windows.

### Download

https://github.com/Rossy167/steam-blocker/releases/tag/ui

https://github.com/Rossy167/steam-blocker/releases/tag/scripts

**These programs will not work unless ran as Admin.**
Follow the instructions in TL;DR

### Usage
A quick set of scripts to block the Steam on the Windows Firewall, and then unblock it afterwards. I'm sure there's many reasons to do this, but for me it's just so that multiple users can use a family sharing at the same time, without Steam blocking it. It gets around this by stopping Steam from accessing the internet preventing it from knowing who is playing what.

### TL;DR

Block-Steam blocks Steam, Unblock-Steam deletes the Firewall rules set from Block-Steam, Save-BlockSteamToProfile saves it to our Powershell profile so you can do it directly from the shell rather than running the PS1 files. 

To run the UI Program: 
* Extract the .zip you downloaded
* Run the .exe as adminstrator (right click + run as administrator)
* Click the button for first time use
* Click the block button to block
* Click the unblock button to unblock :O
* Close when done

To run the scripts: 
* Extract the .zip you downloaded
* Press the Windows key
* Type "powershell"
* Right click on "powershell" and select "run as administrator"
* cd into the directory you downloaded the .ps1 to e.g `cd C:\Users\louis\Downloads\`
* Paste the following: `Set-ExecutionPolicy unrestricted`
* Run `Block-Steam.ps1`
* Paste the following: `Set-ExecutionPolicy restricted`

### Resources Used
* https://gallery.technet.microsoft.com/scriptcenter/PS2EXE-GUI-Convert-e7cb69d5
* https://docs.microsoft.com/en-us/powershell/module/netsecurity/new-netfirewallrule?view=win10-ps
