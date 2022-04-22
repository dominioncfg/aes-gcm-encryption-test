namespace AesGcmTest.Infrastructure;

public class AwsKeyManagementClientConfiguration
{
    public const string Section = "AwsKms";

    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string RegionName { get; set; } = string.Empty;
    public string ServiceUrl { get; set; } = string.Empty;
    public bool UseHttp { get; set; }
}
