namespace PlacementPortal.DAL.ViewModels;

public class AuthenticationResultDTO
{
  public bool Success { get ; set ;} = false;

  public string Message { get ; set ; } = string.Empty;
 
  public string Token { get ; set ; } = string.Empty;

  public string Role { get ; set ;} = string.Empty;

  public string Email { get ; set ; } = string.Empty;

  public int UserId { get ; set ; } = -1; 
}
