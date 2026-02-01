using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.DAL.ViewModels;

public class PasswordResetDTO
{

    [Required]
    public string Email { get ; set ; }

    [Required(ErrorMessage = "Please enter a Password")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""{}|<>]).{8,}$", ErrorMessage = "Please enter a valid Password Ex: User@1234")]
    public string? PasswordReset { get ; set ; }

    [Required(ErrorMessage = "Please enter a Password")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""{}|<>]).{8,}$", ErrorMessage = "Please enter a valid Password Ex: User@1234")]
    [Compare("PasswordReset", ErrorMessage = "Passwords do not match.")]
    public string? PasswordResetConfirm { get ; set ;}
}
