using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace GenshinMacro.Services;

public class ThemeService
{
    private static readonly string ThemeFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "theme.json");

    public ApplicationTheme CurrentTheme { get; private set; } = ApplicationTheme.Light;

    private record PaletteEntry(Color Light, Color Dark, string BrushKey);

    private static readonly Dictionary<string, PaletteEntry> Palette = new()
    {
        ["AccentColor"]            = NewEntry(0xD4,0x94,0x0A, 0xF7,0xB7,0x69, "AccentBrush"),
        ["AccentDarkColor"]        = NewEntry(0xB3,0x7D,0x08, 0xD4,0x94,0x0A, "AccentDarkBrush"),
        ["BackgroundColor"]        = NewEntry(0xF5,0xF5,0xF5, 0x1A,0x1A,0x2E, "BackgroundBrush"),
        ["CardColor"]              = NewEntry(0xFF,0xFF,0xFF, 0x25,0x25,0x40, "CardBrush"),
        ["TextPrimaryColor"]       = NewEntry(0x21,0x21,0x21, 0xEC,0xE5,0xD8, "TextPrimaryBrush"),
        ["TextSecondaryColor"]     = NewEntry(0x75,0x75,0x75, 0x9C,0xBF,0xD1, "TextSecondaryBrush"),
        ["BorderColor"]            = NewEntry(0xE0,0xE0,0xE0, 0x3E,0x4D,0x60, "BorderBrush"),
        ["SuccessColor"]           = NewEntry(0x43,0xA0,0x47, 0x66,0xBB,0x6A, "SuccessBrush"),
        ["ErrorColor"]             = NewEntry(0xE5,0x39,0x35, 0xEF,0x53,0x50, "ErrorBrush"),
        ["SideKey1BgColor"]        = NewEntry(0xF0,0xFD,0xF9, 0x1A,0x3A,0x35, "SideKey1BackgroundBrush"),
        ["SideKey1BorderColor"]    = NewEntry(0xD7,0xEF,0xE7, 0x2D,0x5A,0x4F, "SideKey1BorderBrush"),
        ["SideKey2BgColor"]        = NewEntry(0xFF,0xF5,0xF5, 0x3A,0x1A,0x1A, "SideKey2BackgroundBrush"),
        ["SideKey2BorderColor"]    = NewEntry(0xF1,0xD6,0xD6, 0x5A,0x2D,0x2D, "SideKey2BorderBrush"),
        ["ErrorBannerBgColor"]     = NewEntry(0xFF,0xF3,0xE0, 0x3A,0x2A,0x1A, "ErrorBannerBgBrush"),
        ["ErrorBannerBorderColor"] = NewEntry(0xFF,0x98,0x00, 0xF7,0xB7,0x69, "ErrorBannerBorderBrush"),
        ["ErrorBannerTextColor"]   = NewEntry(0xE6,0x51,0x00, 0xF7,0xB7,0x69, "ErrorBannerTextBrush"),
        ["StatusStoppedBgColor"]   = NewEntry(0xF3,0xF4,0xF6, 0x2A,0x2D,0x37, "StatusStoppedBgBrush"),
        ["StatusRunningBgColor"]   = NewEntry(0xEA,0xF7,0xED, 0x1A,0x3A,0x25, "StatusRunningBgBrush"),
        ["ShadowColor"]            = NewEntry(0x18,0x00,0x00, 0x40,0x00,0x00, "ShadowBrush"),
    };

    private static PaletteEntry NewEntry(byte lr, byte lg, byte lb, byte dr, byte dg, byte db, string brushKey)
        => new(Color.FromArgb(255, lr, lg, lb), Color.FromArgb(255, dr, dg, db), brushKey);

    public void Initialize()
    {
        var theme = LoadTheme();
        CurrentTheme = theme;

        if (theme == ApplicationTheme.Dark)
        {
            ApplicationThemeManager.Apply(theme, WindowBackdropType.None);
            ApplyPalette(theme);
        }
    }

    public void ApplyTheme(ApplicationTheme theme)
    {
        CurrentTheme = theme;
        // WindowBackdropType.None prevents Mica/acrylic from turning the
        // plain WPF Window background to pure black.
        ApplicationThemeManager.Apply(theme, WindowBackdropType.None);
        ApplyPalette(theme);
        SaveTheme(theme);
    }

    public void ToggleTheme()
    {
        var next = CurrentTheme == ApplicationTheme.Light
            ? ApplicationTheme.Dark
            : ApplicationTheme.Light;
        ApplyTheme(next);
    }

    private void ApplyPalette(ApplicationTheme theme)
    {
        var resources = Application.Current.Resources;
        var isDark = theme == ApplicationTheme.Dark;

        foreach (var (colorKey, entry) in Palette)
        {
            var color = isDark ? entry.Dark : entry.Light;
            resources[colorKey] = color;
            resources[entry.BrushKey] = new SolidColorBrush(color);
        }
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
