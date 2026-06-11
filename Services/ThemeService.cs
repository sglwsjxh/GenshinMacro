using System.IO;
using System.Text.Json;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace GenshinMacro.Services;

/// <summary>
/// Manages WPF-UI theme persistence (theme.json) and runtime switching.
/// </summary>
public class ThemeService
{
    private static readonly string ThemeFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "theme.json");

    private static readonly Color LightAccent = Color.FromRgb(0xD4, 0x94, 0x0A);
    private static readonly Color DarkAccent = Color.FromRgb(0xF7, 0xB7, 0x69);

    public ApplicationTheme CurrentTheme { get; private set; } = ApplicationTheme.Light;

    /// <summary>
    /// Loads saved theme from disk and applies it. Defaults to Light on first launch.
    /// </summary>
    public void Initialize()
    {
        var theme = LoadTheme();
        ApplyTheme(theme);
    }

    /// <summary>
    /// Applies the given theme and its accent color, then persists the choice.
    /// </summary>
    public void ApplyTheme(ApplicationTheme theme)
    {
        CurrentTheme = theme;
        ApplicationThemeManager.Apply(theme);

        var accentColor = theme == ApplicationTheme.Dark ? DarkAccent : LightAccent;
        ApplicationAccentColorManager.Apply(accentColor, theme, systemGlassColor: false, systemAccentColor: true);

        SaveTheme(theme);
    }

    /// <summary>
    /// Toggles between Light and Dark themes.
    /// </summary>
    public void ToggleTheme()
    {
        var next = CurrentTheme == ApplicationTheme.Light
            ? ApplicationTheme.Dark
            : ApplicationTheme.Light;
        ApplyTheme(next);
    }

    private ApplicationTheme LoadTheme()
    {
        if (!File.Exists(ThemeFilePath))
            return ApplicationTheme.Light;

        try
        {
            var json = File.ReadAllText(ThemeFilePath);
            var data = JsonSerializer.Deserialize<ThemeData>(json);
            if (data?.Theme == "Dark")
                return ApplicationTheme.Dark;
            return ApplicationTheme.Light;
        }
        catch
        {
            return ApplicationTheme.Light;
        }
    }

    private void SaveTheme(ApplicationTheme theme)
    {
        try
        {
            var data = new ThemeData
            {
                Theme = theme == ApplicationTheme.Dark ? "Dark" : "Light"
            };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ThemeFilePath, json);
        }
        catch
        {
            // Theme persistence is non-critical — silently ignore IO errors
        }
    }

    private class ThemeData
    {
        public string Theme { get; set; } = "Light";
    }
}