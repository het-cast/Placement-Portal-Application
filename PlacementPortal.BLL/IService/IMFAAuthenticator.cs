namespace PlacementPortal.BLL.IService;

public interface IMFAAuthenticator
{
    public string GenerateSecretKey();
    public string GenerateTQrCodeUrl(string email, string secretKey);
    public byte[] GenerateQrCode(string uri);
    public bool ValidateOTP(string secretKey, string otp);
}
