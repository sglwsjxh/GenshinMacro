using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenshinMacro.Input;
using GenshinMacro.MacroEngine;
using GenshinMacro.Models;
using GenshinMacro.Services;

namespace GenshinMacro.ViewModels;

public enum SelectedSection
{
    AutoRotation,
    DoubleMacro
}

public partial class MainWindowViewModel : ObservableObject
{
    private readonly MacroCoordinator _coordinator;
    private readonly SettingsService _settings;
    private readonly IKeyStateProvider _keyState;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowError))]
    private string _errorMessage = "";

    [ObservableProperty]
    private SelectedSection _selectedSection = SelectedSection.AutoRotation;

    [ObservableProperty]
    private object? _currentPage;

    public AutoRotationViewModel RotationVM { get; }
    public DoubleMacroViewModel DoubleMacroVM { get; }

    public string StatusText => IsRunning ? "运行中" : "已停止";

    public bool ShowError => !string.IsNullOrEmpty(ErrorMessage);

    public MainWindowViewModel()
        : this(new SettingsService(), new Win32KeyStateProvider())
    {
    }

    public MainWindowViewModel(SettingsService settings, IKeyStateProvider keyState)
    {
        _settings = settings;
        _keyState = keyState;

        RotationVM = new AutoRotationViewModel(settings);
        DoubleMacroVM = new DoubleMacroViewModel(settings);

        _coordinator = new MacroCoordinator(
            new Win32InputSimulator(),
            new Win32ButtonStateProvider(),
            _keyState,
            settings);

        _coordinator.OnWorkerError += OnWorkerError;

        CurrentPage = RotationVM;
    }

    partial void OnSelectedSectionChanged(SelectedSection value)
    {
        CurrentPage = value switch
        {
            SelectedSection.AutoRotation => RotationVM,
            SelectedSection.DoubleMacro => DoubleMacroVM,
            _ => RotationVM
        };
    }

    private void OnWorkerError(string message)
    {
        ErrorMessage = message;
        IsRunning = false;
    }

    [RelayCommand]
    private void Toggle()
    {
        if (IsRunning)
        {
            _coordinator.StopAll();
            IsRunning = false;
            return;
        }

        ErrorMessage = "";
        _coordinator.StartAll();
        IsRunning = true;
    }

    [RelayCommand]
    private void DismissError() => ErrorMessage = "";

    [RelayCommand]
    private void NavigateTo(string section)
    {
        if (Enum.TryParse<SelectedSection>(section, out var s))
            SelectedSection = s;
    }

    public void Shutdown()
    {
        _coordinator.OnWorkerError -= OnWorkerError;
        if (_coordinator.AnyRunning)
            _coordinator.StopAll();
        IsRunning = false;
    }
}
