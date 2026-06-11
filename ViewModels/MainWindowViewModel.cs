using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenshinMacro.Input;
using GenshinMacro.MacroEngine;
using GenshinMacro.Services;
using Wpf.Ui.Appearance;

namespace GenshinMacro.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly MacroCoordinator _coordinator;
    private readonly ThemeService _themeService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    private bool _isRunning;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowError))]
    private string _errorMessage = "";

    public string StatusText => IsRunning ? "运行中" : "已停止";

    public bool ShowError => !string.IsNullOrEmpty(ErrorMessage);

    public string ThemeToggleIcon =>
        _themeService.CurrentTheme == ApplicationTheme.Dark ? "☀" : "🌙";

    public MainWindowViewModel()
        : this(new Win32InputSimulator(), new Win32ButtonStateProvider(), new ThemeService())
    {
    }

    public MainWindowViewModel(IInputSimulator inputSim, IButtonStateProvider buttonState, ThemeService themeService)
    {
        _themeService = themeService;
        _coordinator = new MacroCoordinator(inputSim, buttonState);
        _coordinator.OnWorkerError += OnWorkerError;
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
    private void ToggleTheme()
    {
        _themeService.ToggleTheme();
        OnPropertyChanged(nameof(ThemeToggleIcon));
    }

    public void Shutdown()
    {
        _coordinator.OnWorkerError -= OnWorkerError;

        if (_coordinator.AnyRunning)
            _coordinator.StopAll();

        IsRunning = false;
    }
}
