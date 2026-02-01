using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.DAL.ViewModels
{
    public class CompanyProfileViewModel
    {
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(50, ErrorMessage = "Company name can't exceed 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Website Url is required")]
        [Url(ErrorMessage = "Please enter a valid website URL.")]
        [Display(Name = "Company Website")]
        public string? CompanyWebsite { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, ErrorMessage = "Description can't exceed 200 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(50, ErrorMessage = "Location can't exceed 100 characters.")]
        public string? Location { get; set; }
    }
}
