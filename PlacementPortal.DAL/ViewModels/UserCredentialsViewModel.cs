using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.DAL.ViewModels;

public class UserCredentialsViewModel
{
    public int? Id { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.[a-zA-Z]{2,10}$", ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Please enter a Password")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""{}|<>]).{8,}$", ErrorMessage = "Please enter a valid Password Ex: User@1234")]
    public string Password { get; set; } = null!;

}
