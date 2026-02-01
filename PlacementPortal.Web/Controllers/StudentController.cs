using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Helper;
using PlacementPortal.BLL.IService;
using PlacementPortal.BLL.Services;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.Web.Controllers;


[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class StudentController : Controller
{

  private readonly IStudentService _studentService;

  private readonly IResumeService _resumeService;

  public StudentController(IStudentService studentService, IResumeService resumeService)
  {
    _studentService = studentService;
    _resumeService = resumeService;
  }

  [CustomAuthorize(new string[] {"Admin",  "Student", "TPO" },"/Dashboard/Dashboard", "/Student/Student", "/Company/Company")]
  [HttpGet]
  public IActionResult Student()
  {
    return View();
  }

  [HttpGet]
  public async Task<IActionResult> CheckStudentProgress()
  {
    try
    {
      bool result = await _studentService.CheckIfUserPerformInititalLogin();

      return Json(new { initialLogin = result });
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.GeneralConst.ErrorFetchingData));
    }
  }

  [HttpGet]
  public async Task<IActionResult> GetPersonalDetails(bool initialLogin)
  {
    UserDetailsDTO? userDetails = await _studentService.GetUserDetails(initialLogin);
    return PartialView("_PersonalDetailsStudent", userDetails);
  }

  [HttpGet]
  public async Task<IActionResult> GetAcademicDetails()
  {
    StudentAcademicDetailsDTO? studentDetails = await _studentService.GetStudentDetails();
    return PartialView("_AcademicDetailsStudent", studentDetails);
  }

  [HttpGet]
  public IActionResult ResumeUploadView()
  {
    return PartialView("_ResumeUpload");
  }

  [HttpGet]
  public async Task<IActionResult> GetResumeInfo()
  {
    try
    {
      ResumeUploadDTO resumeInfo = await _resumeService.GetResumeForStudent();
      return Json(new { data = resumeInfo });
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.GeneralConst.ErrorFetchingData));
    }
  }

  [HttpGet]
  public async Task<IActionResult> GetDepartmentForStudent()
  {
    try
    {
      StudentAcademicDetailsDTO studentDetails = await _studentService.GetStudentDetails();
      return Json(new { departmentId = studentDetails.DepartmentId });
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.GeneralConst.ErrorFetchingData));
    }
  }

  [HttpGet]
  public async Task<IActionResult> GetCompleteStudentDetailsJson()
  {
    try
    {
      StudentViewModel studentDetails = await _studentService.GetCompleteStudentDetails();
      return Json(new ApiResponseDTO<object>(true, "Details Successfully fetched", studentDetails));
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.GeneralConst.ErrorFetchingData));
    }
  }

  [HttpGet]
  public async Task<IActionResult> GetStudentData()
  {
    try
    {
      StudentViewModel studentDetails = await _studentService.GetCompleteStudentDetails();
      return PartialView("StudentDetails", studentDetails);
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.GeneralConst.ErrorFetchingData));
    }
  }

  [HttpGet]
  public IActionResult UpdateStudentData()
  {
    return PartialView("UpdateStudentDetails");
  }

  [HttpGet]
  public IActionResult AcademicDetails()
  {
    return PartialView("_AcademicDetailsStudent");
  }

  [HttpGet]
  public IActionResult UpdateStudentDetails()
  {
    return PartialView("_RegisterStudent");
  }

  [HttpPost]
  public async Task<IActionResult> AddPersonalDetails(UserDetailsDTO userDetails)
  {
    try
    {
      GenericReturnDTO result = await _studentService.AddPersonalDetails(userDetails);

      return Json(new ApiResponseDTO<bool>(result.Success, result.Message, result.Data != null));
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.GeneralConst.ErrorFetchingData));
    }
  }

  [HttpGet]
  public async Task<IActionResult> GetDepartments()
  {
    try
    {
      List<DepartmentDTO> departments = await _studentService.GetDepartments();

      return Json(new { departments });
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.GeneralConst.ErrorFetchingData));
    }
  }

  [HttpPost]
  public async Task<IActionResult> UpdateAcademicDetails(StudentAcademicDetailsDTO studentDetails)
  {
    try
    {
      GenericReturnDTO result = await _studentService.UpdateAcademicDetails(studentDetails);

      return Json(new ApiResponseDTO<bool>(result.Success, result.Message, result.Data != null));
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(StudentConst.UpdationFailed));
    }
  }

  [HttpPost]
  public async Task<IActionResult> UploadResume(ResumeUploadDTO resumeUpload)
  {
    try
    {
      GenericReturnDTO result = await _studentService.AddResume(resumeUpload);
      return Json(new ApiResponseDTO<bool>(result.Success, result.Message, result.Data != null));
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(ResumeConst.UpdationFailed));
    }
  }
}
