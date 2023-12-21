using System.Text.Json.Serialization;

namespace LipUI.Models;

internal class ServerInstance
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "undefined";

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("working_directory")]
    public string WorkingDirectory { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    public static bool operator ==(ServerInstance? left, ServerInstance? right)
        => left is not null && left.Equals(right);

    public static bool operator !=(ServerInstance? left, ServerInstance? right)
        => !(left == right);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        if (obj is ServerInstance instance)
            return WorkingDirectory == instance.WorkingDirectory;
        else
            return false;
    }

    public override int GetHashCode()
        => WorkingDirectory.GetHashCode();
}
