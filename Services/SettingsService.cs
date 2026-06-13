using System.IO;
using System.Text.Json;
using GenshinMacro.Models;

namespace GenshinMacro.Services;

public class SettingsService
{
    private static readonly string SettingsDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GenshinMacro");

    private static readonly string SettingsFile = Path.Combine(SettingsDir, "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AppSettings Current { get; private set; } = new();

    public SettingsService()
    {
        Current = Load();
    }

    public void Save()
    {
        Save(Current);
    }

    public void Save(AppSettings settings)
    {
        try
        {
            if (!Directory.Exists(SettingsDir))
                Directory.CreateDirectory(SettingsDir);

            var tmp = SettingsFile + ".tmp";
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(tmp, json);
            File.Delete(SettingsFile);  // Atomic replace on Windows
            File.Move(tmp, SettingsFile);
        }
        catch
        {
            // Silently fail - settings will be lost on next restart
        }
    }

    private static AppSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsFile))
                return new AppSettings();

            var json = File.ReadAllText(SettingsFile);
            return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }
}
