global using Xunit;
using GenshinMacro.Input;
using GenshinMacro.MacroEngine;

namespace GenshinMacro.Tests;

public class MacroEngineTests
{
    [Fact]
    public void RotationWorker_StartStop_Should_NotCrash()
    {
        var worker = new RotationWorker();
        var btnState = new FakeButtonStateProvider();
        var inputSim = new FakeInputSimulator();

        worker.Start(btnState, inputSim);
        Assert.True(worker.IsRunning);
        Thread.Sleep(50);
        worker.Stop();
        Assert.False(worker.IsRunning);
    }

    [Fact]
    public void DoubleClickWorker_StartStop_Should_NotCrash()
    {
        var worker = new DoubleClickWorker();
        var btnState = new FakeButtonStateProvider();
        var inputSim = new FakeInputSimulator();

        worker.Start(btnState, inputSim);
        Assert.True(worker.IsRunning);
        Thread.Sleep(50);
        worker.Stop();
        Assert.False(worker.IsRunning);
    }

    [Fact]
    public void MacroCoordinator_StartStop_Should_NotCrash()
    {
        var coord = new MacroCoordinator(new FakeInputSimulator(), new FakeButtonStateProvider());
        coord.StartAll();
        Assert.True(coord.AnyRunning);
        Thread.Sleep(50);
        coord.StopAll();
        Assert.False(coord.AnyRunning);
    }

    [Fact]
    public void InputLock_Should_BeShared()
    {
        var lock1 = GenshinMacro.MacroEngine.InputLock.SyncRoot;
        var lock2 = GenshinMacro.MacroEngine.InputLock.SyncRoot;
        Assert.Same(lock1, lock2);
    }
}
