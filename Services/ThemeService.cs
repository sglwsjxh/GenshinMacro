using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace GenshinMacro.Services;

public class ThemeService
{
    private static readonly string ThemeFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "theme.json");

    public ApplicationTheme CurrentTheme { get; private set; } = ApplicationTheme.Light;

    private static readonly Dictionary<string, (Color Light, Color Dark)> ColorMap = new()
    {
        ["AccentColor"]             = (C(0xD4, 0x94, 0x0A), C(0xF7, 0xB7, 0x69)),
        ["AccentDarkColor"]         = (C(0xB3, 0x7D, 0x08), C(0xD4, 0x94, 0x0A)),
        ["BackgroundColor"]         = (C(0xF5, 0xF5, 0xF5), C(0x1A, 0x1A, 0x2E)),
        ["CardColor"]               = (C(0xFF, 0xFF, 0xFF), C(0x25, 0x25, 0x40)),
        ["TextPrimaryColor"]        = (C(0x21, 0x21, 0x21), C(0xEC, 0xE5, 0xD8)),
        ["TextSecondaryColor"]      = (C(0x75, 0x75, 0x75), C(0x9C, 0xBF, 0xD1)),
        ["BorderColor"]             = (C(0xE0, 0xE0, 0xE0), C(0x3E, 0x4D, 0x60)),
        ["SuccessColor"]            = (C(0x43, 0xA0, 0x47), C(0x66, 0xBB, 0x6A)),
        ["ErrorColor"]              = (C(0xE5, 0x39, 0x35), C(0xEF, 0x53, 0x50)),
        ["SideKey1BgColor"]         = (C(0xF0, 0xFD, 0xF9), C(0x1A, 0x3A, 0x35)),
        ["SideKey1BorderColor"]     = (C(0xD7, 0xEF, 0xE7), C(0x2D, 0x5A, 0x4F)),
        ["SideKey2BgColor"]         = (C(0xFF, 0xF5, 0xF5), C(0x3A, 0x1A, 0x1A)),
        ["SideKey2BorderColor"]     = (C(0xF1, 0xD6, 0xD6), C(0x5A, 0x2D, 0x2D)),
        ["ErrorBannerBgColor"]      = (C(0xFF, 0xF3, 0xE0), C(0x3A, 0x2A, 0x1A)),
        ["ErrorBannerBorderColor"]  = (C(0xFF, 0x98, 0x00), C(0xF7, 0xB7, 0x69)),
        ["ErrorBannerTextColor"]    = (C(0xE6, 0x51, 0x00), C(0xF7, 0xB7, 0x69)),
        ["StatusStoppedBgColor"]    = (C(0xF3, 0xF4, 0xF6), C(0x2A, 0x2D, 0x37)),
        ["StatusRunningBgColor"]    = (C(0xEA, 0xF7, 0xED), C(0x1A, 0x3A, 0x25)),
        ["ShadowColor"]             = (C(0x18, 0x00, 0x00, 0x00), C(0x40, 0x00, 0x00, 0x00)),
    };

    private static Color C(byte r, byte g, byte b, byte a = 255)
        => Color.FromArgb(a, r, g, b);

    public void Initialize()
    {
        var theme = LoadTheme();
        CurrentTheme = theme;

        if (theme == ApplicationTheme.Light)
        {
            ApplyColorResources(ApplicationTheme.Light);
            return;
        }

        ApplyTheme(theme);
    }

    public void ApplyTheme(ApplicationTheme theme)
    {
        CurrentTheme = theme;
        ApplicationThemeManager.Apply(theme);
        ApplyColorResources(theme);
        SaveTheme(theme);
    }

    public void ToggleTheme()
    {
        var next = CurrentTheme == ApplicationTheme.Light
            ? ApplicationTheme.Dark
            : ApplicationTheme.Light;
        ApplyTheme(next);
    }

    private void ApplyColorResources(ApplicationTheme theme)
    {
        var resources = Application.Current.Resources;

        foreach (var (key, (light, dark)) in ColorMap)
            resources[key] = theme == ApplicationTheme.Dark ? dark : light;
    }

    private ApplicationTheme LoadTheme()
    {
        if (!File.Exists(ThemeFilePath))
            return ApplicationTheme.Light;

        try
        {
            var json = File.ReadAllText(ThemeFilePath);
            var data = JsonSerializer.Deserialize<ThemeData>(json);
            return data?.Theme == "Dark" ? ApplicationTheme.Dark : ApplicationTheme.Light;
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
        }
    }

    private class ThemeData
    {
        public string Theme { get; set; } = "Light";
    }
}
