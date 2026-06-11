global using System.Windows;
global using System.Diagnostics;
global using System.Security.Principal;
global using GenshinMacro.Services;

namespace GenshinMacro;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        if (!IsAdministrator())
        {
            var proc = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = Environment.ProcessPath,
                Verb = "runas",
                Arguments = string.Join(" ", e.Args)
            };
            try
            {
                Process.Start(proc);
            }
            catch
            {
                // User cancelled UAC or elevation failed - exit
            }
            Environment.Exit(0);
            return;
        }

        base.OnStartup(e);

        new ThemeService().Initialize();
    }

    private static bool IsAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
