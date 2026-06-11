using System.Threading;
using GenshinMacro.Input;
using GenshinMacro.ViewModels;
using Xunit;

namespace GenshinMacro.Tests.ViewModels;

public class MainWindowViewModelTests
{
    [Fact]
    public void Constructor_WithFakes_DoesNotThrow()
    {
        var sim = new FakeInputSimulator();
        var btn = new FakeButtonStateProvider();
        var vm = new MainWindowViewModel(sim, btn);
        Assert.NotNull(vm);
        Assert.False(vm.IsRunning);
        Assert.False(vm.ShowError);
    }

    [Fact]
    public void Toggle_FlipsIsRunning()
    {
        var sim = new FakeInputSimulator();
        var btn = new FakeButtonStateProvider();
        var vm = new MainWindowViewModel(sim, btn);

        Assert.False(vm.IsRunning);
        vm.Toggle();
        Assert.True(vm.IsRunning);
        vm.Toggle();
        Assert.False(vm.IsRunning);
    }

    [Fact]
    public void OnWorkerError_SetsErrorMessageAndStops()
    {
        var sim = new FakeInputSimulator();
        sim.ReturnValue = false; // simulate SendInput failure
        var btn = new FakeButtonStateProvider();
        btn.X1Pressed = true; // trigger rotation worker to execute and fail
        var vm = new MainWindowViewModel(sim, btn);

        vm.Toggle(); // starts workers — rotation will error immediately

        // Wait for worker thread to detect the error and fire OnWorkerError
        Thread.Sleep(200);

        Assert.False(vm.IsRunning);
        Assert.True(vm.ShowError);
        Assert.Contains("旋转宏", vm.ErrorMessage);
    }
}
