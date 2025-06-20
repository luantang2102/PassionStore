namespace PassionStore.Infrastructure.Settings;

public class InfrastructureSettings
{
    public ConnectionStringsOption ConnectionStringsOption { get; set; } = new ConnectionStringsOption();
    public JwtOption JwtOption { get; set; } = new JwtOption();
    public CloudinaryOption CloudinaryOption { get; set; } = new CloudinaryOption();
    public EmailOption EmailOption { get; set; } = new EmailOption();
}