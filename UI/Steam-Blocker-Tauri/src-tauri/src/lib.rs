// Thin, hardcoded wrapper around the shared PowerShell engine (scripts/Steam-Blocker.ps1).
//
// The frontend can only ever trigger one of the three fixed operations below
// (check_installation / block_steam / unblock_steam). There is no generic
// "run a command" entry point and no shell-execute capability is granted in
// capabilities/default.json: the mode string sent to powershell.exe always
// comes from this match, never from caller-supplied text.

use std::path::PathBuf;
use std::process::Command;

use serde::Serialize;
use tauri::path::BaseDirectory;
use tauri::Manager;

#[cfg(target_os = "windows")]
use std::os::windows::process::CommandExt;

// Prevents a console window from flashing behind the app when
// powershell.exe is spawned. Windows-only constant for CREATE_NO_WINDOW.
#[cfg(target_os = "windows")]
const CREATE_NO_WINDOW: u32 = 0x0800_0000;

/// Result of running the PowerShell engine, mirroring the WPF app's
/// CommandResult (exit code + captured stdout/stderr).
#[derive(Serialize)]
pub struct ScriptResult {
    #[serde(rename = "exitCode")]
    exit_code: i32,
    stdout: String,
    stderr: String,
}

#[derive(Clone, Copy)]
enum Mode {
    Validate,
    Block,
    Unblock,
}

impl Mode {
    fn as_arg(self) -> &'static str {
        match self {
            Mode::Validate => "Validate",
            Mode::Block => "Block",
            Mode::Unblock => "Unblock",
        }
    }
}

fn resolve_script_path(app: &tauri::AppHandle) -> Result<PathBuf, String> {
    app.path()
        .resolve("scripts/Steam-Blocker.ps1", BaseDirectory::Resource)
        .map_err(|error| format!("Could not resolve the bundled PowerShell engine: {error}"))
}

fn run_script(app: &tauri::AppHandle, mode: Mode) -> Result<ScriptResult, String> {
    let script_path = resolve_script_path(app)?;

    if !script_path.exists() {
        return Err(format!(
            "The PowerShell engine was not found at {}.",
            script_path.display()
        ));
    }

    let mut command = Command::new("powershell.exe");
    command
        .arg("-NoProfile")
        .arg("-ExecutionPolicy")
        .arg("Bypass")
        .arg("-File")
        .arg(&script_path)
        .arg("-Mode")
        .arg(mode.as_arg());

    #[cfg(target_os = "windows")]
    command.creation_flags(CREATE_NO_WINDOW);

    let output = command
        .output()
        .map_err(|error| format!("Failed to launch powershell.exe: {error}"))?;

    Ok(ScriptResult {
        exit_code: output.status.code().unwrap_or(-1),
        stdout: String::from_utf8_lossy(&output.stdout).into_owned(),
        stderr: String::from_utf8_lossy(&output.stderr).into_owned(),
    })
}

/// Runs `Steam-Blocker.ps1 -Mode Validate`: locates Steam and lists the
/// executable paths that can be managed, without changing firewall rules.
#[tauri::command]
fn check_installation(app: tauri::AppHandle) -> Result<ScriptResult, String> {
    run_script(&app, Mode::Validate)
}

/// Runs `Steam-Blocker.ps1 -Mode Block`: creates the Steam Blocker firewall
/// rule group.
#[tauri::command]
fn block_steam(app: tauri::AppHandle) -> Result<ScriptResult, String> {
    run_script(&app, Mode::Block)
}

/// Runs `Steam-Blocker.ps1 -Mode Unblock`: removes the Steam Blocker
/// firewall rule group.
#[tauri::command]
fn unblock_steam(app: tauri::AppHandle) -> Result<ScriptResult, String> {
    run_script(&app, Mode::Unblock)
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .invoke_handler(tauri::generate_handler![
            check_installation,
            block_steam,
            unblock_steam
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
