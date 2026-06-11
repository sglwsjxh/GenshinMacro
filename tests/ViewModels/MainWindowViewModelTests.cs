using System.Threading;
using System.Windows.Input;
using GenshinMacro.Input;
using GenshinMacro.Services;
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
        var vm = new MainWindowViewModel(sim, btn, new ThemeService());
        Assert.NotNull(vm);
        Assert.False(vm.IsRunning);
        Assert.False(vm.ShowError);
    }

    [Fact]
    public void Toggle_FlipsIsRunning()
    {
        var sim = new FakeInputSimulator();
        var btn = new FakeButtonStateProvider();
        var vm = new MainWindowViewModel(sim, btn, new ThemeService());

        Assert.False(vm.IsRunning);
        vm.ToggleCommand.Execute(null);
        Assert.True(vm.IsRunning);
        vm.ToggleCommand.Execute(null);
        Assert.False(vm.IsRunning);
    }

    [Fact]
    public void OnWorkerError_SetsErrorMessageAndStops()
    {
        var sim = new FakeInputSimulator();
        sim.ReturnValue = false; // simulate SendInput failure
        var btn = new FakeButtonStateProvider();
        btn.X1Pressed = true; // trigger rotation worker to execute and fail
        var vm = new MainWindowViewModel(sim, btn, new ThemeService());

        vm.ToggleCommand.Execute(null); // starts workers — rotation will error immediately

        // Wait up to 2 seconds for the worker thread to detect the error
        var timeout = DateTime.UtcNow.AddSeconds(2);
        while (vm.IsRunning && DateTime.UtcNow < timeout)
            Thread.Sleep(50);

        Assert.False(vm.IsRunning);
        Assert.True(vm.ShowError);
        Assert.Contains("旋转宏", vm.ErrorMessage);
    }

    [Fact]
    public void DismissErrorCommand_ClearsErrorMessage()
    {
        var sim = new FakeInputSimulator();
        var btn = new FakeButtonStateProvider();
        var vm = new MainWindowViewModel(sim, btn, new ThemeService());
        // Manually set error
        vm.GetType().GetProperty("ErrorMessage")?.SetValue(vm, "test error");
        // Execute dismiss through command
        var cmd = vm.GetType().GetProperty("DismissErrorCommand")?.GetValue(vm) as ICommand;
        cmd?.Execute(null);
        Assert.Equal("", vm.ErrorMessage);
        Assert.False(vm.ShowError);
    }
}
