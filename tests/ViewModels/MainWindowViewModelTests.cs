using System.Threading;
using System.Windows.Input;
using GenshinMacro.Services;
using GenshinMacro.ViewModels;
using Xunit;

namespace GenshinMacro.Tests.ViewModels;

public class MainWindowViewModelTests
{
    private static (SettingsService, FakeKeyStateProvider) CreateServices()
    {
        var settings = new SettingsService();
        var keyState = new FakeKeyStateProvider();
        return (settings, keyState);
    }

    [Fact]
    public void Constructor_WithFakes_DoesNotThrow()
    {
        var (settings, keyState) = CreateServices();
        var vm = new MainWindowViewModel(settings, keyState);
        Assert.NotNull(vm);
        Assert.False(vm.IsRunning);
        Assert.False(vm.ShowError);
    }

    [Fact]
    public void Toggle_FlipsIsRunning()
    {
        var (settings, keyState) = CreateServices();
        var vm = new MainWindowViewModel(settings, keyState);

        Assert.False(vm.IsRunning);
        vm.ToggleCommand.Execute(null);
        Assert.True(vm.IsRunning);
        vm.ToggleCommand.Execute(null);
        Assert.False(vm.IsRunning);
    }

    [Fact]
    public void SelectedSection_DefaultsToAutoRotation()
    {
        var (settings, keyState) = CreateServices();
        var vm = new MainWindowViewModel(settings, keyState);

        Assert.Equal(SelectedSection.AutoRotation, vm.SelectedSection);
        Assert.NotNull(vm.RotationVM);
        Assert.NotNull(vm.DoubleMacroVM);
    }

    [Fact]
    public void NavigateCommand_SwitchesSection()
    {
        var (settings, keyState) = CreateServices();
        var vm = new MainWindowViewModel(settings, keyState);

        vm.NavigateToCommand.Execute("DoubleMacro");
        Assert.Equal(SelectedSection.DoubleMacro, vm.SelectedSection);

        vm.NavigateToCommand.Execute("AutoRotation");
        Assert.Equal(SelectedSection.AutoRotation, vm.SelectedSection);
    }

    [Fact]
    public void DismissErrorCommand_ClearsErrorMessage()
    {
        var (settings, keyState) = CreateServices();
        var vm = new MainWindowViewModel(settings, keyState);

        // Set error via reflection to simulate
        vm.GetType().GetProperty("ErrorMessage")?.SetValue(vm, "test error");
        Assert.True(vm.ShowError);

        var cmd = vm.GetType().GetProperty("DismissErrorCommand")?.GetValue(vm) as ICommand;
        cmd?.Execute(null);
        Assert.Equal("", vm.ErrorMessage);
        Assert.False(vm.ShowError);
    }
}
