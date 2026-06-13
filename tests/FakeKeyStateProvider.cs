using GenshinMacro.Input;

namespace GenshinMacro.Tests;

public class FakeKeyStateProvider : IKeyStateProvider
{
    /// <summary>If set, all IsKeyPressed calls return this value.</summary>
    public bool DefaultReturn { get; set; }

    /// <summary>Per-key overrides. Key = keyName.</summary>
    public Dictionary<string, bool> Overrides { get; set; } = new();

    public bool IsKeyPressed(string keyName)
    {
        if (Overrides.TryGetValue(keyName, out var result))
            return result;
        return DefaultReturn;
    }
}
