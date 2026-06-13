using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GenshinMacro.Models;
using GenshinMacro.Services;

namespace GenshinMacro.ViewModels;

public partial class AutoRotationViewModel : ObservableObject
{
    private readonly SettingsService _settings;
    private bool _suppressSave;

    [ObservableProperty]
    private bool _enabled;

    [ObservableProperty]
    private string _triggerKey = "XButton1";

    [ObservableProperty]
    private int _speed = 96;

    [ObservableProperty]
    private int _intervalMs = 20;

    public List<string> AvailableKeys { get; } =
    [
        "XButton1", "XButton2",
        "F1", "F2", "F3", "F4", "F5", "F6",
        "F7", "F8", "F9", "F10", "F11", "F12",
        "Q", "E", "R", "F", "G",
        "LeftShift", "RightShift",
        "LeftCtrl", "RightCtrl",
        "LeftAlt", "RightAlt",
        "Space",
        "1", "2", "3", "4", "5",
        "CapsLock", "Tab", "V",
    ];

    public AutoRotationViewModel(SettingsService settings)
    {
        _settings = settings;
        LoadFromSettings();
    }

    private void LoadFromSettings()
    {
        _suppressSave = true;
        var s = _settings.Current.AutoRotation;
        Enabled = s.Enabled;
        TriggerKey = s.TriggerKey;
        Speed = s.Speed;
        IntervalMs = s.IntervalMs;
        _suppressSave = false;
    }

    partial void OnEnabledChanged(bool value)
    {
        if (!_suppressSave) Save();
    }

    partial void OnTriggerKeyChanged(string value)
    {
        if (!_suppressSave) Save();
    }

    partial void OnSpeedChanged(int value)
    {
        if (!_suppressSave) Save();
    }

    partial void OnIntervalMsChanged(int value)
    {
        if (!_suppressSave) Save();
    }

    private void Save()
    {
        var s = _settings.Current.AutoRotation;
        s.Enabled = Enabled;
        s.TriggerKey = TriggerKey;
        s.Speed = Speed;
        s.IntervalMs = IntervalMs;
        _settings.Save();
    }

    public void Reload()
    {
        LoadFromSettings();
    }
}
