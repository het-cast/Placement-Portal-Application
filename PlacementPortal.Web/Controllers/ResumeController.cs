using Microsoft.AspNetCore.Mvc;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.IService;
using PlacementPortal.BLL.Services;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.Web.Controllers;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ResumeController : Controller
{
  private readonly IResumeService _resumeService;
  public ResumeController(IResumeService resumeService)
  {
    _resumeService = resumeService;
  }

  [CustomAuthorize(new string[] { "TPO" })]
  [HttpGet]
  public IActionResult Resume()
  {
    return View();
  }

  [HttpPost]
  public async Task<IActionResult> GetResumesPaginated(PaginationRequestDTO paginationRequest)
  {
    try
    {
      PaginationDTO<ResumeUploadDTO> resumes = await _resumeService.GetResumesPaginated(paginationRequest);
      return PartialView("_ResumeList", resumes);
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.ResumeConst.ErrorFetchingList));
    }
  }

  [HttpGet]
  public async Task<IActionResult> RenderResumeCommentPartial(int resumeId)
  {
    try
    {
      ResumeCommentViewModel resumeData = await _resumeService.GetResumeCommentData(resumeId);
      return PartialView("_ResumeComment", resumeData);
    }
    catch (Exception)
    {
      return Json(ApiResponse.Fail<string>(CustomExceptionConst.DataNotFound));
    }
  }

  [HttpPost]
  public async Task<IActionResult> SaveResumeComment(ResumeCommentViewModel commentData){
    try{
      OperationStatusDTO result = await _resumeService.SaveResumeComment(commentData);
      return Json(new ApiResponseDTO<string>(result.Success, result.Message));
    }
    catch(Exception)
    {
      return Json(ApiResponse.Fail<string>(Consts.FailToSaveMessage(Consts.EntityConst.Resume)));
    }
  }


  [HttpGet]
  public IActionResult PreviewResume(string fileName)
  {
    GenericReturnDTO result = _resumeService.GetPdfFileIfExists(fileName);

    return (result.Data as string) switch
    {
      Consts.GeneralConst.BadRequest => BadRequest(result.Message),
      Consts.GeneralConst.NotFound => NotFound(result.Message),
      Consts.GeneralConst.Content => Content(result.Message),
      _ => PhysicalFile(result.Data!.ToString()!, "application/pdf"),
    };
  }

  [HttpGet]
  public IActionResult DownloadResume(string fileName)
  {

    GenericReturnDTO result = _resumeService.DownloadResume(fileName);
    var fileData = result.Data as ResumeFileData;
    return (result.Data as string) switch
    {
      Consts.GeneralConst.BadRequest => BadRequest(result.Message),
      Consts.GeneralConst.NotFound => NotFound(result.Message),
      Consts.GeneralConst.Content => Content(result.Message),
      _ => File(fileData!.FileBytes!, fileData.ContentType!, fileData.DownloadFileName)
    };

  }

}
