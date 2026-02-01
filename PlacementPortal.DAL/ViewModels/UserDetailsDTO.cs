using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.DAL.ViewModels;

public class UserDetailsDTO
{
    [Required(ErrorMessage = "Please enter First Name")]
    [RegularExpression(@"^\S+$", ErrorMessage = "First Name cannot contain spaces")]
    public string? FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Please enter Middle Name")]
    [RegularExpression(@"^\S+$", ErrorMessage = "Middle Name cannot contain spaces")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Please enter Last Name")]
    [RegularExpression(@"^\S+$", ErrorMessage = "Last Name cannot contain spaces")]
    public string? LastName { get; set; } = null!;

    [Required]
    [Phone]
    // [RegularExpression("^[6-9][0-9]{9}$", ErrorMessage = "Phone number must start with 6 or 7 and should be 10 digits.")]
    [RegularExpression(@"^\s*[6-9]\d{9}\s*$", ErrorMessage = "Phone number must be 10 digits, start with 6-9, and contain no spaces.")]

    public long? Phone { get; set; } 

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.[a-zA-Z]{2,10}$", ErrorMessage = "Please enter a valid email address")]
    public string? Email { get ; set; } = null!;

    [Required]
    public string? Address { get ; set ; }
}
