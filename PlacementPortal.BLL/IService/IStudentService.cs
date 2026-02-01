using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IService;

public interface IStudentService
{
    Task<bool> CheckIfUserPerformInititalLogin();

    Task<UserDetailsDTO> GetUserDetails(bool IsInitialLogin);

    Task<GenericReturnDTO> AddPersonalDetails(UserDetailsDTO userDetailsDTO);

    Task<List<DepartmentDTO>> GetDepartments();

    Task<GenericReturnDTO> UpdateAcademicDetails(StudentAcademicDetailsDTO dto);

    Task<StudentAcademicDetailsDTO> GetStudentDetails();

    Task<ResumeDataDTO> GetResumeDetails();

    Task<StudentViewModel> GetCompleteStudentDetails();

    Task<GenericReturnDTO> AddResume(ResumeUploadDTO resumeUpload);

    Task<List<UserCredentialsDTO>> GetStudentsList();
}
