using GenshinMacro.Input;

namespace GenshinMacro.MacroEngine;

public class DoubleClickWorker : MacroWorkerBase
{
    // Python polling interval: 0.05 (50ms)
    private const int PollIntervalMs = 50;

    // Timing constants (in milliseconds) matching Python exactly
    private const int LeftHoldMs = 100;
    private const int PostRightClickMs = 30;
    private const int InterCycleMs = 80;

    protected override void Run(IButtonStateProvider buttonState, IInputSimulator inputSim)
    {
        while (!_cts.IsCancellationRequested)
        {
            if (buttonState.IsXButton2Pressed())
            {
                lock (InputLock.SyncRoot)
                {
                    ExecuteDoubleClickSequence(inputSim);
                }
            }
            try
            {
                Task.Delay(PollIntervalMs, _cts.Token).Wait(_cts.Token);
            }
            catch (OperationCanceledException) { break; }
            catch (AggregateException) { break; }
        }
    }

    private void ExecuteDoubleClickSequence(IInputSimulator inputSim)
    {
        if (_cts.IsCancellationRequested) return;

        // Cycle 1
        if (!inputSim.LeftButtonDown()) return;
        Thread.Sleep(LeftHoldMs);
        if (!inputSim.RightButtonDown()) return;
        if (!inputSim.RightButtonUp()) return;
        Thread.Sleep(PostRightClickMs);
        if (!inputSim.LeftButtonUp()) return;
        Thread.Sleep(PostRightClickMs);

        // Cycle 2
        if (!inputSim.LeftButtonDown()) return;
        Thread.Sleep(LeftHoldMs);
        if (!inputSim.RightButtonDown()) return;
        if (!inputSim.RightButtonUp()) return;
        Thread.Sleep(PostRightClickMs);
        if (!inputSim.LeftButtonUp()) return;
        Thread.Sleep(InterCycleMs);
    }
}
