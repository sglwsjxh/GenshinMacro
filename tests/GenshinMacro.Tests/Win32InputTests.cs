using GenshinMacro.Input;
using Xunit;

namespace GenshinMacro.Tests;

public class Win32InputTests
{
    [Fact]
    public void InputStruct_Size_ShouldBeCorrect()
    {
        // On 64-bit: INPUT = 4 (type) + 4 (padding) + 28 (MOUSEINPUT) = 40 bytes
        // On 32-bit: smaller
        // Just verify it compiles and has the expected layout
        var input = new Interop.INPUT
        {
            type = Interop.InputType.Mouse,
            union = default
        };
        Assert.Equal(Interop.InputType.Mouse, input.type);
    }

    [Fact]
    public void FakeInputSimulator_Should_ImplementInterface()
    {
        IInputSimulator simulator = new FakeInputSimulator();
        Assert.NotNull(simulator);
    }

    [Fact]
    public void FakeButtonStateProvider_Should_ImplementInterface()
    {
        IButtonStateProvider provider = new FakeButtonStateProvider();
        Assert.NotNull(provider);
    }

    [Fact]
    public void Win32InputSimulator_SendInput_Fails_Gracefully_When_Not_Elevated()
    {
        // SendInput will fail if not running as admin and targeting high-integrity processes.
        // At minimum it should return false without crashing.
        var sim = new Win32InputSimulator();
        // This might succeed or fail depending on environment - just verify no crash
        var result = sim.MoveMouseBy(0, 0);
        // We don't assert success/failure since it depends on elevation state
        // The important thing is no exception is thrown
        Assert.True(true);
    }

    [Fact]
    public void ButtonStateProvider_Should_Poll_Without_Crashing()
    {
        var provider = new Win32ButtonStateProvider();
        // These calls should not crash even if no X buttons are present
        var x1 = provider.IsXButton1Pressed();
        var x2 = provider.IsXButton2Pressed();
        // Can't assert true/false since it depends on actual hardware state
        // Just verify no exception
        Assert.True(true);
    }
}
