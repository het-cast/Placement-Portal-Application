using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.DAL.ViewModels;

public class StudentAcademicDetailsDTO
{
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Please enter SSC %")]
    [Range(35, 100, ErrorMessage = "SSC% must be between 35 and 100.")]
    public decimal SscPercentage { get; set; }

    [Required]
    [RegularExpression("^[1-9][0-9]{11}$", ErrorMessage ="Invalid / Enrollment number should be of 12 digits.")]
    public long EnrollementNumber { get ; set ; }

    public int DepartmentId { get ; set ; }

    [Required(ErrorMessage = "Please select a SSC Board")]
    public string SscBoard { get; set; }

    [Required(ErrorMessage = "Please enter SSC Passing Year")]
    [Range(1900, 2030, ErrorMessage = "Please enter a valid SSC passing year")]
    public int SscPassingYear { get; set; }

    [Required(ErrorMessage = "Please enter HSC %")]
    [Range(60, 100, ErrorMessage = "HSC% must be between 60 and 100.")]
    public decimal HscPercentage { get; set; }

    [Required(ErrorMessage = "Please select a SSC Board")]
    public string HscBoard { get; set; }

    [Required(ErrorMessage = "Please enter SSC Passing Year")]
    [Range(1900, 2030, ErrorMessage = "Please enter a valid SSC passing year")]
    public short HscPassingYear { get; set; }

    [Range(0, 10, ErrorMessage = "CPI Diploma must be between 0 and 10.")]
    public decimal? CpiDiploma { get; set; }

    [Required(ErrorMessage = "Please enter Passout Year")]
    [Range(2018, 2050, ErrorMessage = "Please enter a valid Passout year")]
    public int? EnrollmentYear { get; set; }

    [Required(ErrorMessage = "Birthdate is required")]
    public DateOnly Birthdate { get; set; }

    public int Branch { get; set; }

    public string? BranchName { get ; set ; }

    [Range(0, 5, ErrorMessage = "Pending backlog must be <= 5 / Or write 5 if more.")]
    public int PendingBacklog { get; set; }

    [Required(ErrorMessage = "Please enter CGPA")]
    [Range(0, 10, ErrorMessage = "CGPA must be between 0 and 10.")]
    public decimal Cgpa { get; set; }

    [Required(ErrorMessage = "Please enter CPI")]
    [Range(0, 10, ErrorMessage = "CPI must be between 0 and 10.")]
    public decimal Cpi { get; set; }
}
