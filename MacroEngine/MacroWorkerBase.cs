using GenshinMacro.Input;

namespace GenshinMacro.MacroEngine;

public abstract class MacroWorkerBase
{
    public event Action<string>? OnError;

    /// <summary>The configured trigger key name (e.g. "XButton1", "F6").</summary>
    public string TriggerKey { get; set; } = "XButton1";

    /// <summary>Whether this worker should run.</summary>
    public bool Enabled { get; set; } = true;

    protected readonly CancellationTokenSource _cts = new();
    protected Thread? _thread;
    protected bool _running;

    public bool IsRunning => _running;
    public string? LastError { get; protected set; }

    public void Start(IButtonStateProvider buttonState, IInputSimulator inputSim)
    {
        if (_running) return;
        _running = true;
        _thread = new Thread(() => Run(buttonState, inputSim, new Win32KeyStateProvider()))
        {
            IsBackground = true,
            Name = GetType().Name
        };
        _thread.Start();
    }

    public void Start(IButtonStateProvider buttonState, IInputSimulator inputSim, IKeyStateProvider keyState)
    {
        if (_running) return;
        _running = true;
        _thread = new Thread(() => Run(buttonState, inputSim, keyState))
        {
            IsBackground = true,
            Name = GetType().Name
        };
        _thread.Start();
    }

    protected void ReportError(string message)
    {
        LastError = message;
        OnError?.Invoke(message);
    }

    public void Stop()
    {
        if (!_running) return;
        _running = false;
        _cts.Cancel();
        _thread?.Join(TimeSpan.FromSeconds(2));
        _thread = null;
    }

    /// <summary>
    /// Old override point (no key state). Workers should upgrade to the 3-param overload.
    /// </summary>
    protected virtual void Run(IButtonStateProvider buttonState, IInputSimulator inputSim)
    {
        Run(buttonState, inputSim, new Win32KeyStateProvider());
    }

    /// <summary>New override point with key state support.</summary>
    protected abstract void Run(IButtonStateProvider buttonState, IInputSimulator inputSim, IKeyStateProvider keyState);
}
