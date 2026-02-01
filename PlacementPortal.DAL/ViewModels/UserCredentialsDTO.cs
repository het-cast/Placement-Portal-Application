namespace PlacementPortal.DAL.ViewModels;

public class UserCredentialsDTO
{
    public int? Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}
