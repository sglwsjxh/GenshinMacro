using GenshinMacro.Input;
using GenshinMacro.Services;

namespace GenshinMacro.MacroEngine;

public class MacroCoordinator
{
    public event Action<string>? OnWorkerError;

    private readonly RotationWorker _rotation = new();
    private readonly DoubleClickWorker _doubleClick = new();
    private readonly IInputSimulator _inputSim;
    private readonly IButtonStateProvider _buttonState;
    private readonly IKeyStateProvider _keyState;
    private readonly SettingsService _settings;

    public MacroCoordinator(
        IInputSimulator inputSim,
        IButtonStateProvider buttonState,
        IKeyStateProvider keyState,
        SettingsService settings)
    {
        _inputSim = inputSim;
        _buttonState = buttonState;
        _keyState = keyState;
        _settings = settings;
    }

    public bool IsRotationRunning => _rotation.IsRunning;
    public bool IsDoubleClickRunning => _doubleClick.IsRunning;
    public bool AnyRunning => _rotation.IsRunning || _doubleClick.IsRunning;

    public void StartAll()
    {
        var s = _settings.Current;

        // Configure rotation worker from settings
        _rotation.Enabled = s.AutoRotation.Enabled;
        _rotation.TriggerKey = s.AutoRotation.TriggerKey;
        _rotation.Speed = s.AutoRotation.Speed;
        _rotation.PollIntervalMs = s.AutoRotation.IntervalMs;

        // Configure double-click worker from settings
        _doubleClick.Enabled = s.DoubleMacro.Enabled;
        _doubleClick.TriggerKey = s.DoubleMacro.TriggerKey;

        _rotation.OnError += OnWorkerError;
        _doubleClick.OnError += OnWorkerError;

        if (_rotation.Enabled)
            _rotation.Start(_buttonState, _inputSim, _keyState);

        if (_doubleClick.Enabled)
            _doubleClick.Start(_buttonState, _inputSim, _keyState);
    }

    public void StopAll()
    {
        _rotation.OnError -= OnWorkerError;
        _doubleClick.OnError -= OnWorkerError;
        _rotation.Stop();
        _doubleClick.Stop();
    }
}
