using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IService;

public interface IGlobalService
{
    public TokenDataDTO GetTokenData();

    public Task<bool> IsStudentRegistrationComplete();

    public Task<bool> IsEverythingSubmittedByStudent();

    public void SendEmailToAllStudents();

    int RoleIdOfLoggedInUser();

    int IdOfLoggedInUser();
}
