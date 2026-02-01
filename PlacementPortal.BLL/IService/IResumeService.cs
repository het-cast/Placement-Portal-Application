using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IService;

public interface IResumeService
{
    Task<ResumeUploadDTO> GetResumeForStudent();
    
    Task<PaginationDTO<ResumeUploadDTO>> GetResumesPaginated(PaginationRequestDTO paginatedData);

    Task<ResumeCommentViewModel> GetResumeCommentData(int resumeId);

    Task<OperationStatusDTO> SaveResumeComment(ResumeCommentViewModel resumeComment);

    GenericReturnDTO GetPdfFileIfExists(string fileName);

    GenericReturnDTO DownloadResume(string fileName);
}  
