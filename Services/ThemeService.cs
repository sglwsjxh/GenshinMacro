using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace GenshinMacro.Services;

/// <summary>
/// Bootstraps the WPF-UI dark theme on startup.
/// This used to handle light/dark toggling; now it only initializes dark mode.
/// </summary>
public class ThemeService
{
    public void Initialize()
    {
        ApplicationThemeManager.Apply(ApplicationTheme.Dark, WindowBackdropType.None);
    }
}
