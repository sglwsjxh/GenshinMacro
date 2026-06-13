using System.Threading;
using GenshinMacro.Input;
using GenshinMacro.MacroEngine;
using GenshinMacro.Services;
using Xunit;

namespace GenshinMacro.Tests;

public class MacroEngineTests
{
    [Fact]
    public void RotationWorker_StartStop_Should_Lifecycle()
    {
        var worker = new RotationWorker();
        var btn = new FakeButtonStateProvider();
        var sim = new FakeInputSimulator();
        var keyState = new FakeKeyStateProvider();

        Assert.False(worker.IsRunning);
        worker.Start(btn, sim, keyState);
        Assert.True(worker.IsRunning);
        Thread.Sleep(60);
        worker.Stop();
        Assert.False(worker.IsRunning);
    }

    [Fact]
    public void DoubleClickWorker_StartStop_Should_Lifecycle()
    {
        var worker = new DoubleClickWorker();
        var btn = new FakeButtonStateProvider();
        var sim = new FakeInputSimulator();
        var keyState = new FakeKeyStateProvider();

        Assert.False(worker.IsRunning);
        worker.Start(btn, sim, keyState);
        Assert.True(worker.IsRunning);
        Thread.Sleep(60);
        worker.Stop();
        Assert.False(worker.IsRunning);
    }

    [Fact]
    public void MacroCoordinator_Should_StartAndStopBoth()
    {
        var settings = new SettingsService();
        var keyState = new FakeKeyStateProvider();
        var coord = new MacroCoordinator(
            new FakeInputSimulator(),
            new FakeButtonStateProvider(),
            keyState,
            settings);

        Assert.False(coord.AnyRunning);
        coord.StartAll();
        Assert.True(coord.AnyRunning);
        Thread.Sleep(60);
        coord.StopAll();
        Assert.False(coord.AnyRunning);
    }

    [Fact]
    public void DoubleClickWorker_Should_EmitCorrectSequence()
    {
        var btn = new FakeButtonStateProvider();
        var sim = new FakeInputSimulator();
        var keyState = new FakeKeyStateProvider();
        var worker = new DoubleClickWorker();
        worker.TriggerKey = "XButton2";
        keyState.Overrides["XButton2"] = true;

        worker.Start(btn, sim, keyState);
        Thread.Sleep(150);
        worker.Stop();

        var log = sim.CallLog;
        Assert.Contains(log, c => c == "LeftButtonDown");
        Assert.Contains(log, c => c == "RightButtonDown");
        Assert.Contains(log, c => c == "RightButtonUp");
        Assert.Contains(log, c => c == "LeftButtonUp");
    }

    [Fact]
    public void InputLock_Should_BeSingleton()
    {
        Assert.Same(InputLock.SyncRoot, InputLock.SyncRoot);
    }

    [Fact]
    public void RotationWorker_SendInputFailure_Should_StopWorker()
    {
        var btn = new FakeButtonStateProvider();
        var sim = new FakeInputSimulator();
        sim.ReturnValue = false;
        var keyState = new FakeKeyStateProvider();
        var worker = new RotationWorker();
        worker.TriggerKey = "XButton1";
        keyState.Overrides["XButton1"] = true;

        worker.Start(btn, sim, keyState);
        Thread.Sleep(100);
        worker.Stop();
        Assert.False(worker.IsRunning);
    }
}
