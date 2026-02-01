using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IService;

public interface IJWTService
{
  string GenerateJwtToken(int userId, string email, string role, int? expiryMinutes = null);

  (string, string, int) ValidateToken(string token);

}
