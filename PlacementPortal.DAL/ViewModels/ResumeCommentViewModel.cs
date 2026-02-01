using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.DAL.ViewModels;

public class ResumeCommentViewModel
{
    public int? ResumeId { get ; set ; }

    [Required]
    public string? ResumeComment { get ; set ; }
}
