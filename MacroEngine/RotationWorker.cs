using GenshinMacro.Input;

namespace GenshinMacro.MacroEngine;

public class RotationWorker : MacroWorkerBase
{
    /// <summary>Pixels per sub-step. Higher = faster rotation.</summary>
    public int Speed { get; set; } = 96;

    /// <summary>Poll interval in milliseconds.</summary>
    public int PollIntervalMs { get; set; } = 20;

    protected override void Run(IButtonStateProvider buttonState, IInputSimulator inputSim, IKeyStateProvider keyState)
    {
        var subSteps = 20;
        var subStepDelayMs = PollIntervalMs / subSteps;

        while (!_cts.IsCancellationRequested)
        {
            if (keyState.IsKeyPressed(TriggerKey))
            {
                lock (InputLock.SyncRoot)
                {
                    for (int i = 0; i < subSteps; i++)
                    {
                        if (_cts.IsCancellationRequested) return;
                        if (!inputSim.MoveMouseBy(Speed, 0))
                        {
                            ReportError("旋转宏：输入模拟失败，请检查是否以管理员权限运行");
                            return;
                        }
                        Thread.Sleep(subStepDelayMs);
                    }
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
}
