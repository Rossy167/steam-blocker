# Steam Blocker

A Windows desktop utility and PowerShell toolkit for temporarily controlling Steam's network access through Windows Firewall rules.

Steam Blocker began as a small household automation script for Steam Family Sharing. It has since been organized into a discoverable PowerShell engine, an elevated WPF interface, and an MSI packaging workflow.

> **Important:** This tool changes local Windows Firewall configuration. Steam may enforce Family Sharing rules server-side, so blocking network access cannot guarantee that multiple accounts can use a shared library simultaneously.

## Highlights

- Finds Steam through the Windows registry and environment-based installation paths.
- Supports current and legacy `steamwebhelper.exe` layouts.
- Uses Windows-provided common-files locations instead of localized folder names.
- Creates grouped, application-specific inbound and outbound firewall rules.
- Removes only rules created by Steam Blocker.
- Requires and declares Administrator access explicitly through a Windows application manifest.
- Reports PowerShell output, errors, and exit codes in the desktop UI.
- Ships the PowerShell engine with the WPF application and MSI installer.
- Keeps command-line entry points available for automation and scripting.

## User Experience

The WPF application provides three operations:

| Operation | Purpose |
| --- | --- |
| **Check installation** | Locates Steam and lists executable paths that can be managed. |
| **Block Steam** | Creates inbound and outbound Windows Firewall block rules. |
| **Unblock Steam** | Removes the Steam Blocker rule group. |

The application must run elevated because Windows Firewall administration requires Administrator privileges. The application manifest requests elevation when the program starts.

## Requirements

### Runtime

- Windows 10 or later
- Windows PowerShell 5.1
- Steam installed for the current Windows user or registered machine-wide
- Administrator approval for firewall changes

### Development

- Windows with Visual Studio or MSBuild
- .NET Framework 4.7.2 developer targeting pack
- Advanced Installer, if building the MSI project

The repository can be inspected and PowerShell syntax-checked on other platforms, but the application itself is Windows-specific. WPF, Windows Firewall, the Windows registry, UAC, and Advanced Installer cannot be fully exercised on macOS.

## Quick Start

### Desktop application

1. Build or download a Windows release.
2. Install or extract the application.
3. Launch **Steam Blocker** and approve the Administrator prompt.
4. Select **Check installation** to confirm that Steam was found.
5. Select **Block Steam** when a local offline window is needed.
6. Select **Unblock Steam** when normal Steam connectivity should be restored.

### PowerShell

Open Windows PowerShell as Administrator, change to the `scripts` directory, and run:

```powershell
Set-ExecutionPolicy -Scope Process Bypass

.\Steam-Blocker.ps1 -Mode Validate
.\Steam-Blocker.ps1 -Mode Block
.\Steam-Blocker.ps1 -Mode Unblock
```

`Validate` reports the detected Steam installation and managed executable paths without changing firewall rules.

The legacy-compatible entry points call the same engine:

```powershell
.\Block-Steam.ps1
.\Unblock-Steam.ps1
```

To add profile commands for the current PowerShell user:

```powershell
.\Save-BlockSteamToProfile.ps1
```

This adds:

- `Block-Steam`
- `Unblock-Steam`
- `Test-SteamBlocker`

## How It Works

```text
WPF application
      |
      | starts elevated Windows PowerShell
      v
Steam-Blocker.ps1
      |
      +-- discovers Steam from registry and known Windows locations
      +-- identifies Steam client, web helper, and service executables
      +-- creates or removes the "Steam Blocker" firewall rule group
      +-- returns output and a meaningful process exit code
```

The WPF project includes `Steam-Blocker.ps1` as build content under `Scripts\`. The Advanced Installer project packages that file beside the executable so an installed copy does not depend on the repository or a separate script download.

## Project Structure

```text
scripts/
  Steam-Blocker.ps1              Shared discovery and firewall engine
  Block-Steam.ps1                Compatibility wrapper for blocking
  Unblock-Steam.ps1              Compatibility wrapper for unblocking
  Save-BlockSteamToProfile.ps1   Adds profile convenience commands

UI/Steam-Blocker-WPF/
  MainWindow.xaml                Desktop interface
  MainWindow.xaml.cs             Process execution and status handling
  app.manifest                   UAC elevation declaration
  Steam-Blocker-WPF.csproj       .NET Framework 4.7.2 WPF project

UI/Steam-Blocker-Setup/
  Steam-Blocker-Setup.aip        Advanced Installer MSI definition
```

## Build

### WPF application

From a Windows developer command prompt:

```powershell
msbuild .\UI\Steam-Blocker-WPF\Steam-Blocker-WPF.csproj /p:Configuration=Release
```

The executable and the copied PowerShell engine are produced under:

```text
UI\Steam-Blocker-WPF\bin\Release\
```

### MSI installer

Open `UI\Steam-Blocker-WPF\Steam-Blocker-WPF.sln` on Windows with Advanced Installer integration available. The installer is configured to consume the Release output and package `Scripts\Steam-Blocker.ps1` beside the application executable.

## Limitations and Safety Notes

- Steam controls some Family Sharing behavior remotely; local firewall rules cannot override every server-side policy.
- Blocking Steam while it is running may leave the client in an offline or partially connected state. Unblock Steam before normal online use.
- The tool removes rules in the `Steam Blocker` firewall group. Do not reuse that group name for unrelated firewall rules on the same machine.
- Review the detected executable list before relying on the block operation in a production or shared environment.

## Credits

The original project was created as a practical attempt to solve a real household workflow. The current implementation preserves that goal while adding installation discovery, modern Steam layout support, explicit elevation, reliable process handling, release packaging, and clearer operational feedback.
