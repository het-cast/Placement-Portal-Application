using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Helper;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Service;

public class JobDeadlineReminderService : IJobDeadlineReminderService
{
    private readonly ICompanyService _companyService;

    private readonly IStudentService _studentService;

    private readonly EmailHelper _emailService;

    public JobDeadlineReminderService(
        ICompanyService companyService,
        IStudentService studentService,
        EmailHelper emailService)
    {
        _companyService = companyService;
        _studentService = studentService;
        _emailService = emailService;
    }

    public async Task SendDeadlineReminderEmailsAsync()
    {
        List<CompanyProfile> closingJobs = await _companyService.GetJobsClosingWithin24HoursAsync();

        foreach (CompanyProfile job in closingJobs)
        {
            List<UserCredentialsDTO> eligibleStudents = await _studentService.GetStudentsList();

            foreach (var student in eligibleStudents)
            {
                await _emailService.SendEmailAsync(student.Email, EmailConsts.DeadlineSoon , EmailConsts.GenerateDeadlineSoonBody(job.Name));
            }
        }
    }

}
