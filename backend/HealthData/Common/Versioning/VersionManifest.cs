namespace Common.Versioning;

public sealed class VersionManifest
{
    public int SchemaVersion { get; init; }
    public VersionEntry Suite { get; init; } = new();
    public ServiceVersions Services { get; init; } = new();
    public ReleaseInfo Release { get; init; } = new();
}

public class VersionEntry
{
    public string Version { get; init; } = string.Empty;
}

public sealed class AndroidVersionEntry : VersionEntry
{
    public int VersionCode { get; init; }
}

public sealed class ServiceVersions
{
    public AndroidVersionEntry Android { get; init; } = new();
    public VersionEntry Backend { get; init; } = new();
    public VersionEntry Frontend { get; init; } = new();
}

public sealed class ReleaseInfo
{
    public string TagPrefix { get; init; } = "v";
}