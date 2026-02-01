using OtpNet;
using PlacementPortal.BLL.IService;
using QRCoder;

namespace PlacementPortal.BLL.Service;

public class MFAAuthenticator : IMFAAuthenticator
{
    public string GenerateSecretKey()
    {
        byte[] secretKey = KeyGeneration.GenerateRandomKey(20);

        return Base32Encoding.ToString(secretKey);
    }

    public string GenerateTQrCodeUrl(string email, string secretKey)
    {
        string issuer = Uri.EscapeDataString("PlacementPortalSystem");

        string userEmail = Uri.EscapeDataString(email);

        return $"otpauth://totp/{issuer}:{userEmail}?secret={secretKey}&issuer={issuer}&algorithm=SHA1&digits=6&period=30";
    }

    public byte[] GenerateQrCode(string uri)
    {
        using QRCodeGenerator qrGenerator = new ();

        using QRCodeData? qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        using PngByteQRCode? qrCode = new(qrCodeData);

        return qrCode.GetGraphic(20);

    }

    public bool ValidateOTP(string secretKey, string otp)
    {
        Totp? totp = new(Base32Encoding.ToBytes(secretKey));
        return totp.VerifyTotp(otp, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}
