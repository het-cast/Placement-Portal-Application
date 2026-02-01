namespace PlacementPortal.DAL.ViewModels;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
public class ResumeUploadDTO
{
    public int? ResumeId { get ; set ; }

    public int? StudentId { get ; set ; }

    public string? StudentEmail { get ; set ; }

    public string? StudentName { get ; set ;}

    [Required]
    public IFormFile? ResumeFile { get; set; }

    public string? ResumePath { get; set; }

    public string? OriginalFileName { get ; set ; }

    public string? Branch { get; set; }

    public DateTime? UploadDate { get; set; }
}
