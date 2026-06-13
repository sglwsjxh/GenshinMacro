using System.Windows.Input;
using GenshinMacro.Interop;

namespace GenshinMacro.Input;

public class Win32KeyStateProvider : IKeyStateProvider
{
    public bool IsKeyPressed(string keyName)
    {
        if (string.IsNullOrEmpty(keyName))
            return false;

        var vk = ParseVirtualKey(keyName);
        if (vk < 0) return false;

        short state = NativeMethods.GetAsyncKeyState(vk);
        return (state & 0x8000) != 0;
    }

    private static int ParseVirtualKey(string keyName)
    {
        // Mouse side buttons
        if (string.Equals(keyName, "XButton1", StringComparison.OrdinalIgnoreCase))
            return (int)VirtualKey.XBUTTON1;
        if (string.Equals(keyName, "XButton2", StringComparison.OrdinalIgnoreCase))
            return (int)VirtualKey.XBUTTON2;

        // Try to parse as a WPF Key enum value
        if (Enum.TryParse<Key>(keyName, ignoreCase: true, out var key))
        {
            return KeyInterop.VirtualKeyFromKey(key);
        }

        return -1;
    }
}
