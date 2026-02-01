using Microsoft.AspNetCore.Http;

namespace PlacementPortal.BLL.Helper;

public class FileHelper
{
    public static async Task<(string, string)> SaveResumeAsync(IFormFile file)
    {
        string resumeFolder = Path.Combine("wwwroot", "resumes");

        if (!Directory.Exists(resumeFolder))
            Directory.CreateDirectory(resumeFolder);

        string[] allowedExtensions = new[] { ".pdf" };

        string extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            throw new Exception("Only PDF files are allowed.");

        string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
        string fileExtension = Path.GetExtension(file.FileName);

        string safeName = string.Concat(originalFileName
                            .Where(c => !Path.GetInvalidFileNameChars().Contains(c)));

        string uniqueFileName = $"{safeName}_{Guid.NewGuid()}{fileExtension}";
        string filePath = Path.Combine(resumeFolder, uniqueFileName);

        using (FileStream stream = new(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        string relativePath = Path.Combine("resumes", uniqueFileName).Replace("\\", "/");

        return (relativePath, originalFileName);
    }
}
