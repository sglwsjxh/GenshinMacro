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
    private const int PyAutoGuiPauseMs = 100;

    protected override void Run(IButtonStateProvider buttonState, IInputSimulator inputSim)
    {
        while (!_cts.IsCancellationRequested)
        {
            if (buttonState.IsXButton2Pressed())
            {
                lock (InputLock.SyncRoot)
                {
                    if (!ExecuteDoubleClickSequence(inputSim))
                        return;
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

    private bool ExecuteDoubleClickSequence(IInputSimulator inputSim)
    {
        if (_cts.IsCancellationRequested) return false;

        // Cycle 1
        if (!inputSim.LeftButtonDown()) return Fail("LeftButtonDown");
        Thread.Sleep(LeftHoldMs);
        if (!inputSim.RightButtonDown()) return Fail("RightButtonDown");
        if (!inputSim.RightButtonUp()) return Fail("RightButtonUp");
        Thread.Sleep(PyAutoGuiPauseMs);
        Thread.Sleep(PostRightClickMs);
        if (!inputSim.LeftButtonUp()) return Fail("LeftButtonUp");
        Thread.Sleep(PostRightClickMs);

        // Cycle 2
        if (!inputSim.LeftButtonDown()) return Fail("LeftButtonDown");
        Thread.Sleep(LeftHoldMs);
        if (!inputSim.RightButtonDown()) return Fail("RightButtonDown");
        if (!inputSim.RightButtonUp()) return Fail("RightButtonUp");
        Thread.Sleep(PyAutoGuiPauseMs);
        Thread.Sleep(PostRightClickMs);
        if (!inputSim.LeftButtonUp()) return Fail("LeftButtonUp");
        Thread.Sleep(InterCycleMs);

        return true;
    }

    private bool Fail(string action)
    {
        ReportError($"双马头宏：{action} 模拟失败，请检查是否以管理员权限运行");
        return false;
    }
}
