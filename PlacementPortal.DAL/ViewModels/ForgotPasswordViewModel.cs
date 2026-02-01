using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.DAL.ViewModels;

public class ForgotPasswordViewModel
{
     [Required]
     [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")]
     public string Email { get; set; } 
}

