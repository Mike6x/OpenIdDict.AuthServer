namespace Identity.Domain.Settings;

public class OpenIdDictSettingsConfig
{
    public bool OnlyAllowHttps { get; set; }
    public EncryptionConfig Encryption { get; set; } = new EncryptionConfig();
    public EncryptionConfig Signing { get; set; } = new EncryptionConfig();

    public IEnumerable<ApplicationConfig> ApplicationConfigs { get; set; } = new List<ApplicationConfig>();
}

public class EncryptionConfig
{
    public string Key { get; set; } = string.Empty;
    public CertConfig Cert { get; set; } = new CertConfig();
}

public class CertConfig
{
    public string Path { get; set; } = string.Empty;
    public bool GenerateIfEmpty { get; set; }
    public string Password { get; set; } = string.Empty;
    public int ValidityMonths { get; set; }
    public string Issuer { get; set; } = "OpenIdDict";
}