using System.Linq.Expressions;
using Hangfire;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.Helper;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Service;

public class ResumeService : IResumeService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IGlobalService _globalService;

    private readonly INotificationService _notificationService;

    private readonly EmailHelper _emailHelper;

    public ResumeService(IUnitOfWork unitOfWork, IGlobalService globalService, INotificationService notificationService, EmailHelper emailHelper)
    {
        _unitOfWork = unitOfWork;
        _globalService = globalService;
        _notificationService = notificationService;
        _emailHelper = emailHelper;
    }

    public async Task<ResumeUploadDTO> GetResumeForStudent()
    {
        TokenDataDTO tokenData = _globalService.GetTokenData();
        Resume? resumeDetails = await _unitOfWork.ResumeRepository.GetFirstOrDefault(r => r.Student.Email == tokenData.Email)!;

        if (resumeDetails == null)
        {
            return new ResumeUploadDTO();
        }

        ResumeUploadDTO resumeUpload = new()
        {
            OriginalFileName = resumeDetails.ResumeName,
            ResumePath = resumeDetails.ResumeFileUrl,
            ResumeId = resumeDetails.Id,
        };

        return resumeUpload;
    }

    public async Task<PaginationDTO<ResumeUploadDTO>> GetResumesPaginated(PaginationRequestDTO paginationRequest)
    {
        Expression<Func<Resume, bool>> expression;

        if (string.IsNullOrWhiteSpace(paginationRequest.SearchFilter))
        {
            expression = r => true;
        }
        else
        {
            string lowerSearch = paginationRequest.SearchFilter.ToLower().Trim();
            expression = r =>
                r.Student.Email.ToLower().Trim().Contains(lowerSearch) ||
                r.Student.UserDetails.Any(ud =>
                    ud.FirstName.ToLower().Trim().Contains(lowerSearch) ||
                    ud.MiddleName.ToLower().Trim().Contains(lowerSearch) ||
                    ud.LastName.ToLower().Trim().Contains(lowerSearch)) ||
                r.ResumeName.ToLower().Trim().Contains(lowerSearch);
        }

        bool isAscending = paginationRequest.SortOrder.Equals(FilterConst.Ascending);

        Expression<Func<Resume, object>> orderBy = r => r.Id;

        if (!string.IsNullOrEmpty(paginationRequest.SortColumn))
        {
            orderBy = paginationRequest.SortColumn switch
            {
                FilterConst.StudentFirstNameSort => r => r.StudentId,
                FilterConst.DeptIdSort => r => r.Student.StudentDetails.First().Branch,
                FilterConst.DeptNameSort => r => r.Student.StudentDetails.First().BranchNavigation.Name,
                FilterConst.CreatedAtSort => r => r.CreatedAt,
                _ => j => j.Id
            };
        }
        Expression<Func<Resume, int>> orderThenBy = r => r.Id;

        (int totalCount, List<ResumeUploadDTO> resumeDataListing) = await _unitOfWork.ResumeRepository.GetListPaginated(
            expression,
            x => new ResumeUploadDTO
            {
                StudentName = $"{x.Student.UserDetails.Select(ud => ud.FirstName).FirstOrDefault()} {x.Student.UserDetails.Select(ud => ud.MiddleName).FirstOrDefault()} {x.Student.UserDetails.Select(ud => ud.LastName).FirstOrDefault()}",
                OriginalFileName = x.ResumeName,
                ResumePath = x.ResumeFileUrl,
                StudentId = x.StudentId,
                StudentEmail = x.Student.Email,
                ResumeId = x.Id,
                Branch = x.Student.StudentDetails.Select(ud => ud.BranchNavigation.Name).FirstOrDefault(),
                UploadDate = x.CreatedAt
            },
            orderBy,
            r => r.Id,
            isAscending,
            paginationRequest.CurrentPage,
            paginationRequest.PageSize
        );

        int startIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + 1;

        int endIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + resumeDataListing.Count;

        return new PaginationDTO<ResumeUploadDTO>
        {
            StartIndex = resumeDataListing.Count > 0 ? startIndex : 0,
            EndIndex = resumeDataListing.Count > 0 ? endIndex : 0,
            CurrentPage = paginationRequest.CurrentPage,
            TotalPages = (int)Math.Ceiling((double)totalCount / paginationRequest.PageSize),
            Items = resumeDataListing,
            TotalRecords = totalCount
        };
    }

    public async Task<ResumeCommentViewModel> GetResumeCommentData(int resumeId)
    {
        try
        {
            Resume resume = await _unitOfWork.ResumeRepository.GetFirstOrDefault(r => r.Id == resumeId) ?? new Resume() { Id = resumeId };

            ResumeCommentViewModel resumeComment = new()
            {
                ResumeComment = resume.Comment,
                ResumeId = resume.Id
            };

            return resumeComment;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<OperationStatusDTO> SaveResumeComment(ResumeCommentViewModel resumeComment)
    {
        OperationStatusDTO result = new();

        try
        {
            Resume resume = await _unitOfWork.ResumeRepository.GetFirstOrDefault(r => r.Id == resumeComment.ResumeId)!;


            if (resume == null)
            {
                result.Message = Consts.NotFound(Consts.EntityConst.Resume);
                return result;
            }

            UserAccount user = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(ua => ua.Id == resume.StudentId)!;

            if (resumeComment.ResumeComment == null || String.IsNullOrEmpty(resumeComment.ResumeComment))
            {
                result.Message = Consts.NothingToBeAdded(Consts.EntityConst.ResumeComment);
                return result;
            }

            if (resume.Comment != null && resume.Comment.ToLower().Trim() == resumeComment.ResumeComment.ToLower().Trim())
            {
                result.Success = Consts.GeneralConst.StatusSuccess;
                result.Message = Consts.SavedMessage(Consts.EntityConst.Resume);
                return result;
            }

            resume.Comment = resumeComment.ResumeComment;
            GenericReturnDTO genericReturn = _unitOfWork.ResumeRepository.Update(resume);
            await _unitOfWork.SaveChangesAsync();

            result.Success = Consts.GeneralConst.StatusSuccess;
            result.Message = Consts.SavedMessage(Consts.EntityConst.Resume);

            BackgroundJob.Enqueue(() => _emailHelper.SendEmailSync(user.Email, "Resume Related Comment", resume.Comment!));

            await _notificationService.SaveNotificationForSpecificUser(resume.StudentId, $"Resume Comment Updated: {resumeComment.ResumeComment}");

            return result;
        }
        catch (Exception)
        {
            result.Message = Consts.FailToSaveMessage(Consts.EntityConst.Resume);
            return result;
        }
    }

    public GenericReturnDTO GetPdfFileIfExists(string fileName)
    {
        GenericReturnDTO result = new();

        if (string.IsNullOrWhiteSpace(fileName))
        {
            result.Message = Consts.ResumeConst.InValidFileName;
            result.Data = Consts.GeneralConst.BadRequest;
            return result;
        }

        fileName = Uri.UnescapeDataString(fileName);

        string justFileName = Path.GetFileName(fileName);

        string resumeFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resumes");
        string fullPath = Path.Combine(resumeFolder, justFileName);

        if (!File.Exists(fullPath))
        {
            result.Message = Consts.ResumeConst.ResumeNotFound;
            result.Data = Consts.GeneralConst.NotFound;
            return result;
        }
        string ext = Path.GetExtension(justFileName).ToLowerInvariant();
        if (ext != ".pdf")
        {
            result.Message = Consts.ResumeConst.ExtPreviewNotSupported;
            result.Data = Consts.GeneralConst.Content;
            return result;
        }

        result.Message = Consts.ResumeConst.ResumePreviewed;
        result.Data = fullPath;
        return result;
    }

    public GenericReturnDTO DownloadResume(string fileName)
    {
        GenericReturnDTO result = new();
        if (string.IsNullOrWhiteSpace(fileName))
        {
            result.Message = Consts.ResumeConst.InValidFileName;
            result.Data = Consts.GeneralConst.BadRequest;
            return result;
        }

        string sanitizedPath = fileName.Replace("/", Path.DirectorySeparatorChar.ToString());
        string resumeFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        string fullPath = Path.Combine(resumeFolder, sanitizedPath);

        if (!File.Exists(fullPath))
        {
            result.Message = Consts.ResumeConst.ResumeNotFound;
            result.Data = Consts.GeneralConst.NotFound;
            return result;
        }

        byte[] fileBytes = File.ReadAllBytes(fullPath);
        string contentType = "application/octet-stream";
        string downloadFileName = Path.GetFileName(fullPath);


        result.Success = true;
        result.Message = Consts.ResumeConst.ResumeDownloaded;
        result.Data = new ResumeFileData
        {
            FileBytes = fileBytes,
            ContentType = contentType,
            DownloadFileName = downloadFileName
        };
        return result;

    }
}


