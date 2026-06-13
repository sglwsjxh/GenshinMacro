using System.Text.Json.Serialization;

namespace GenshinMacro.Models;

public class AppSettings
{
    [JsonPropertyName("autoRotation")]
    public AutoRotationSettings AutoRotation { get; set; } = new();

    [JsonPropertyName("doubleMacro")]
    public DoubleMacroSettings DoubleMacro { get; set; } = new();
}

public class AutoRotationSettings
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("triggerKey")]
    public string TriggerKey { get; set; } = "XButton1";

    [JsonPropertyName("speed")]
    public int Speed { get; set; } = 96;

    [JsonPropertyName("intervalMs")]
    public int IntervalMs { get; set; } = 20;
}

public class DoubleMacroSettings
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("triggerKey")]
    public string TriggerKey { get; set; } = "XButton2";
}
