namespace GenshinMacro.Input;

/// <summary>
/// Provides state information for configurable trigger keys (both mouse and keyboard).
/// </summary>
public interface IKeyStateProvider
{
    /// <summary>
    /// Returns true if the key identified by <paramref name="keyName"/> is currently held down.
    /// Supported values: "XButton1", "XButton2", or any System.Windows.Input.Key name.
    /// </summary>
    bool IsKeyPressed(string keyName);
}
