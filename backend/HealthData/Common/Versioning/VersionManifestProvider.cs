using System.Text.Json;

namespace Common.Versioning;

public sealed class VersionManifestProvider : IVersionManifestProvider
{
    public VersionManifest Current { get; }

    public string SuiteVersion => Current.Suite.Version;
    public string BackendVersion => Current.Services.Backend.Version;
    public string FrontendVersion => Current.Services.Frontend.Version;
    public string AndroidVersion => Current.Services.Android.Version;
    public int AndroidVersionCode => Current.Services.Android.VersionCode;

    public VersionManifestProvider(string manifestPath)
    {
        Current = LoadManifest(manifestPath);
    }

    private static VersionManifest LoadManifest(string manifestPath)
    {
        if (string.IsNullOrWhiteSpace(manifestPath))
        {
            throw new InvalidOperationException("The version manifest path is missing.");
        }

        if (!File.Exists(manifestPath))
        {
            throw new FileNotFoundException(
                $"The version manifest could not be found at '{manifestPath}'.",
                manifestPath);
        }

        var json = File.ReadAllText(manifestPath);

        var manifest = JsonSerializer.Deserialize<VersionManifest>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (manifest is null)
        {
            throw new InvalidOperationException("The version manifest could not be deserialized.");
        }

        Validate(manifest);

        return manifest;
    }

    private static void Validate(VersionManifest manifest)
    {
        if (string.IsNullOrWhiteSpace(manifest.Suite.Version))
        {
            throw new InvalidOperationException("suite.version is missing in versions.json.");
        }

        if (string.IsNullOrWhiteSpace(manifest.Services.Backend.Version))
        {
            throw new InvalidOperationException("services.backend.version is missing in versions.json.");
        }

        if (string.IsNullOrWhiteSpace(manifest.Services.Frontend.Version))
        {
            throw new InvalidOperationException("services.frontend.version is missing in versions.json.");
        }

        if (string.IsNullOrWhiteSpace(manifest.Services.Android.Version))
        {
            throw new InvalidOperationException("services.android.version is missing in versions.json.");
        }

        if (manifest.Services.Android.VersionCode <= 0)
        {
            throw new InvalidOperationException("services.android.versionCode must be greater than 0.");
        }
    }
}