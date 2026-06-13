using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using GenshinMacro.Models;
using GenshinMacro.Services;

namespace GenshinMacro.ViewModels;

public partial class DoubleMacroViewModel : ObservableObject
{
    private readonly SettingsService _settings;
    private bool _suppressSave;

    [ObservableProperty]
    private bool _enabled;

    [ObservableProperty]
    private string _triggerKey = "XButton2";

    public List<string> AvailableKeys { get; } =
    [
        "XButton2", "XButton1",
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

    public DoubleMacroViewModel(SettingsService settings)
    {
        _settings = settings;
        LoadFromSettings();
    }

    private void LoadFromSettings()
    {
        _suppressSave = true;
        var s = _settings.Current.DoubleMacro;
        Enabled = s.Enabled;
        TriggerKey = s.TriggerKey;
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

    private void Save()
    {
        var s = _settings.Current.DoubleMacro;
        s.Enabled = Enabled;
        s.TriggerKey = TriggerKey;
        _settings.Save();
    }

    public void Reload()
    {
        LoadFromSettings();
    }
}
