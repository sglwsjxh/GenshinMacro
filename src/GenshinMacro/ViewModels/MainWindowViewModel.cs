using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using GenshinMacro.Input;
using GenshinMacro.MacroEngine;

namespace GenshinMacro.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly MacroCoordinator _coordinator;
    private bool _isRunning;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (_isRunning == value) return;
            _isRunning = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StatusText));
        }
    }

    public string StatusText => IsRunning ? "运行中" : "已停止";

    public ICommand ToggleCommand { get; }

    public MainWindowViewModel()
    {
        var inputSim = new Win32InputSimulator();
        var buttonState = new Win32ButtonStateProvider();
        _coordinator = new MacroCoordinator(inputSim, buttonState);
        ToggleCommand = new RelayCommand(Toggle);
    }

    public void Toggle()
    {
        if (IsRunning)
        {
            _coordinator.StopAll();
            IsRunning = false;
            return;
        }

        _coordinator.StartAll();
        IsRunning = true;
    }

    public void Shutdown()
    {
        if (_coordinator.AnyRunning)
            _coordinator.StopAll();

        IsRunning = false;
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
