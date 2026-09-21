# Steam Blocker

A Windows desktop utility and PowerShell toolkit for temporarily controlling Steam's network access through Windows Firewall rules.

Steam Blocker began as a small household automation script for Steam Family Sharing. It has since been organized into a discoverable PowerShell engine and an elevated Tauri desktop interface. There are two ways to use it: the PowerShell cmdlet module, or the desktop application.

> **Important:** This tool changes local Windows Firewall configuration. Steam may enforce Family Sharing rules server-side, so blocking network access cannot guarantee that multiple accounts can use a shared library simultaneously.

## Highlights

- Finds Steam through the Windows registry and environment-based installation paths.
- Supports current and legacy `steamwebhelper.exe` layouts.
- Uses Windows-provided common-files locations instead of localized folder names.
- Creates grouped, application-specific inbound and outbound firewall rules.
- Removes only rules created by Steam Blocker.
- Requires and declares Administrator access explicitly through a Windows application manifest.
- Reports PowerShell output, errors, and exit codes in the desktop UI.
- Bundles the PowerShell engine as a Tauri resource inside the desktop application.
- Keeps command-line entry points available for automation and scripting.

## User Experience

The desktop application provides three operations:

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

- Windows with Visual Studio Build Tools or MSVC C++ Build Tools
- Node.js LTS
- Rust stable-msvc toolchain

The repository can be inspected and PowerShell syntax-checked on other platforms, but the application itself is Windows-specific. Windows Firewall, the Windows registry, UAC, and a native Windows build of the desktop application cannot be fully exercised on macOS.

## Quick Start

### Desktop application

1. Install [Node.js LTS](https://nodejs.org/) and the [Rust stable-msvc toolchain](https://www.rust-lang.org/tools/install) on Windows.
2. From `UI/Steam-Blocker-Tauri/`, run `npm install`.
3. Run `npm run tauri dev` to launch the application, or `npm run tauri build` to produce a Windows release, then approve the Administrator prompt when launching it.
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
Tauri desktop application (Rust + React)
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

The Tauri project bundles `Steam-Blocker.ps1` as a Tauri resource, resolved at runtime through Tauri's resource directory API, so an installed copy does not depend on the repository or a separate script download.

## Project Structure

```text
scripts/
  Steam-Blocker.ps1              Shared discovery and firewall engine
  Block-Steam.ps1                Compatibility wrapper for blocking
  Unblock-Steam.ps1              Compatibility wrapper for unblocking
  Save-BlockSteamToProfile.ps1   Adds profile convenience commands

UI/Steam-Blocker-Tauri/
  src/App.tsx                    Desktop interface (three operations, status area)
  src-tauri/src/lib.rs           Hardcoded check_installation/block_steam/unblock_steam commands
  src-tauri/build.rs             Embeds the requireAdministrator Windows manifest
  src-tauri/windows-app-manifest.xml   UAC elevation declaration
  src-tauri/capabilities/default.json  Locked-down capability set (no shell-execute)
  src-tauri/tauri.conf.json      Bundles scripts/Steam-Blocker.ps1 as a resource
```

## Build

### Desktop application (Tauri)

From a Windows machine with Node.js and the Rust stable-msvc toolchain installed:

```powershell
cd UI\Steam-Blocker-Tauri
npm install
npm run tauri build
```

The Windows executable, with `Steam-Blocker.ps1` bundled as a resource, is produced under:

```text
UI\Steam-Blocker-Tauri\src-tauri\target\release\
```

## Limitations and Safety Notes

- Steam controls some Family Sharing behavior remotely; local firewall rules cannot override every server-side policy.
- Blocking Steam while it is running may leave the client in an offline or partially connected state. Unblock Steam before normal online use.
- The tool removes rules in the `Steam Blocker` firewall group. Do not reuse that group name for unrelated firewall rules on the same machine.
- Review the detected executable list before relying on the block operation in a production or shared environment.
- An unsigned build of the desktop application will trigger Windows SmartScreen on first run. Code signing removes that warning but is a recurring cost and is out of scope for this project.

## Credits

The original project was created as a practical attempt to solve a real household workflow. The current implementation preserves that goal while adding installation discovery, modern Steam layout support, explicit elevation, reliable process handling, release packaging, and clearer operational feedback.
