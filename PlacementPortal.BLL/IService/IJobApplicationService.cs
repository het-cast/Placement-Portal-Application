using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IService;

public interface IJobApplicationService
{
    Task<OperationStatusDTO> SubmitJobApplicationAsync(int jobListingId);

    Task<PaginationDTO<StudentJobApplicationDTO>> GetApplicationsForTpoPaginated(PaginationRequestDTO paginationRequest, int statusFilter);

    Task<JobApplicationDataDTO> GetApplicationDetails(int applicationId);

    Task<PaginationDTO<StudentJobApplicationDTO>> GetPersonalApplicationsPaginatedEnhanced(PaginationRequestDTO paginationRequest);

    Task<OperationStatusDTO> UpdateApplicationStatus(int applicationId, int applyStatus);

    Task<ApiResponseDTO<List<int>>> GetJobApplicationsByStudentForCompanyId(int companyId);
}
