namespace PlacementPortal.DAL.ViewModels;

public class TokenDataDTO
{
    public int UserId { get ; set ; } = -1;

    public string Email { get ; set ; } ="";

    public string Role { get ; set ; } = "";

    public bool IsTokenPresent { get ; set ; } = true;
}
