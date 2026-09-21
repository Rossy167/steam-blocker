fn main() {
    // Embed a Windows application manifest requesting `requireAdministrator`,
    // equivalent to the WPF app's app.manifest. Windows Firewall
    // administration (New-NetFirewallRule / Remove-NetFirewallRule in
    // scripts/Steam-Blocker.ps1) requires elevation, so the exe itself
    // should trigger the UAC prompt rather than relying on the invoked
    // PowerShell process to elevate on its own.
    let windows_attributes = tauri_build::WindowsAttributes::new()
        .app_manifest(include_str!("windows-app-manifest.xml"));

    tauri_build::try_build(tauri_build::Attributes::new().windows_attributes(windows_attributes))
        .expect("failed to run tauri build script");
}
