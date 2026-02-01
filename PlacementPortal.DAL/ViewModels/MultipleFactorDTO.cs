namespace PlacementPortal.DAL.ViewModels;

public class MultipleFactorDTO
{
    public bool Success { get; set; } = false;

    public string? Message { get; set; }

    public string? Email { get; set; }

    public string? SecretKey { get; set; } = string.Empty;

    public string? QRCode { get; set; } = string.Empty;

    public string? OTP { get; set; }
}
