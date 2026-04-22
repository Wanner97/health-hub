namespace Common.Versioning;

public interface IVersionManifestProvider
{
    VersionManifest Current { get; }

    string SuiteVersion { get; }
    string BackendVersion { get; }
    string FrontendVersion { get; }
    string AndroidVersion { get; }
    int AndroidVersionCode { get; }
}