using GenshinMacro.Input;

namespace GenshinMacro.Tests;

public class FakeInputSimulator : IInputSimulator
{
    public bool MoveMouseBy(int deltaX, int deltaY) => true;
    public bool RightClick() => true;
    public bool LeftButtonDown() => true;
    public bool LeftButtonUp() => true;
    public bool RightButtonDown() => true;
    public bool RightButtonUp() => true;
}
