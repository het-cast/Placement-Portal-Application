using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.Helper;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Service;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IAuditLogger _auditLogger;

    private readonly IGlobalService _globalService;
    public StudentService(IUnitOfWork unitOfWork, IGlobalService globalService, IAuditLogger auditLogger)
    {
        _unitOfWork = unitOfWork;
        _globalService = globalService;
        _auditLogger = auditLogger;
    }

    public async Task<bool> CheckIfUserPerformInititalLogin()
    {
        TokenDataDTO tokenData = _globalService.GetTokenData();

        string emailOfUserLoggedIn = tokenData.Email;

        UserAccount? LoggedInUser = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(u => u.Email.ToLower().Trim() == emailOfUserLoggedIn.ToLower().Trim())!;

        if (LoggedInUser != null && LoggedInUser.IsInitialLogin == true)
        {
            return true;
        }
        return false;
    }

    public async Task<UserDetailsDTO> GetUserDetails(bool IsInitialLogin)
    {
        TokenDataDTO tokenData = _globalService.GetTokenData();
        UserDetail? studentPersonalDetails = await _unitOfWork.UserDetailsRepository.GetFirstOrDefault(ud => ud.User.Email.ToLower().Trim() == tokenData.Email.ToLower().Trim())!;
        UserDetailsDTO userDetails = new();

        if (IsInitialLogin)
        {
            if (studentPersonalDetails == null)
            {
                return new UserDetailsDTO()
                {
                    Email = tokenData.Email.ToLower().Trim()
                };
            }

            userDetails = new UserDetailsDTO
            {
                FirstName = studentPersonalDetails.FirstName ?? "",
                MiddleName = studentPersonalDetails.MiddleName ?? "",
                LastName = studentPersonalDetails.LastName ?? "",
                Email = tokenData.Email.ToLower().Trim(),
                Address = studentPersonalDetails.Address ?? "",
                Phone = studentPersonalDetails.Phone ?? 0
            };
        }
        else
        {
            userDetails = new UserDetailsDTO
            {
                FirstName = studentPersonalDetails?.FirstName,
                MiddleName = studentPersonalDetails?.MiddleName,
                LastName = studentPersonalDetails?.LastName,
                Phone = studentPersonalDetails?.Phone ?? null,
                Email = tokenData.Email.ToLower().Trim(),
                Address = studentPersonalDetails?.Address
            };
        }
        return userDetails;
    }

    public async Task<ResumeDataDTO> GetResumeDetails()
    {
        TokenDataDTO tokenData = _globalService.GetTokenData();
        Resume? resume = await _unitOfWork.ResumeRepository.GetFirstOrDefault(r => r.Student.Email == tokenData.Email.ToLower().Trim())!;
        if (resume == null)
        {
            return new ResumeDataDTO();
        }

        ResumeDataDTO resumeData = new()
        {
            OriginalFileName = resume.ResumeName,
            ResumePath = resume.ResumeFileUrl,
            StudentEmail = tokenData.Email.ToLower().Trim()
        };

        return resumeData;
    }

    public async Task<StudentAcademicDetailsDTO> GetStudentDetails()
    {
        try
        {
            TokenDataDTO tokenData = _globalService.GetTokenData();
            UserAccount? existingStudent = await _unitOfWork.AuthenticationRepository
                .GetFirstOrDefault(sd => sd.Email.ToLower().Trim() == tokenData.Email.ToLower().Trim())!;



            if (existingStudent == null)
            {
                return new StudentAcademicDetailsDTO();
            }

            StudentDetail? existingAcademicDetails = await _unitOfWork.StudentDetailRepository
                                                                .GetFirstOrDefaultInclude(a => existingStudent.Id == a.StudentId, a => a.BranchNavigation)!;

            if (existingAcademicDetails == null)
            {
                return new StudentAcademicDetailsDTO();
            }

            StudentAcademicDetailsDTO studentDetails = new()
            {
                EnrollementNumber = existingAcademicDetails.EnrollmentNumber ?? 0,
                EnrollmentYear = existingAcademicDetails.EnrollmentYear,
                SscBoard = existingAcademicDetails.SscBoard,
                HscBoard = existingAcademicDetails.HscBoard,
                Cpi = existingAcademicDetails.Cpi ?? 0,
                Cgpa = existingAcademicDetails.Cgpa ?? 0,
                SscPassingYear = existingAcademicDetails.SscPassingYear,
                SscPercentage = existingAcademicDetails.SscPercentage,
                HscPassingYear = existingAcademicDetails.HscPassingYear,
                HscPercentage = existingAcademicDetails.HscPercentage,
                DepartmentId = existingAcademicDetails.Branch,
                PendingBacklog = existingAcademicDetails.PendingBacklog ?? 0,
                CpiDiploma = existingAcademicDetails.CpiDiploma,
                BranchName = existingAcademicDetails.BranchNavigation.Name,
                StudentId = existingAcademicDetails.Id
            };

            return studentDetails;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<DepartmentDTO>> GetDepartments()
    {
        List<Department> departments = await _unitOfWork.DepartmentRepository.GetList(d => d.IsDeleted == false);

        List<DepartmentDTO> departmentDTOs = departments.Select(d => new DepartmentDTO
        {
            Id = d.Id,
            Name = d.Name,
            IsDeleted = d.IsDeleted
        }).ToList();

        return departmentDTOs;
    }

    public async Task<GenericReturnDTO> UpdateAcademicDetails(StudentAcademicDetailsDTO dto)
    {
        GenericReturnDTO result = new()
        {
            Success = false,
            Message = Consts.StudentConst.AcademicDetailsNotUpdated
        };

        TokenDataDTO tokenData = _globalService.GetTokenData();

        UserAccount? existingStudent = await _unitOfWork.AuthenticationRepository
                                                  .GetFirstOrDefault(sd => sd.Email.ToLower().Trim() == tokenData.Email.ToLower().Trim())!;

        if (existingStudent == null)
        {
            result.Message = Consts.StudentConst.StudentNotFound;
            return result;
        }



        try
        {

            StudentDetail duplicateDetail = await _unitOfWork.StudentDetailRepository
                                                                .GetFirstOrDefault(a => a.EnrollmentNumber == dto.EnrollementNumber
                                                                                    && existingStudent.Id != a.StudentId)!;
            if (duplicateDetail != null)
            {
                result.Message = Consts.ExistsMessage(Consts.EntityConst.Student);
                return result;
            }

            StudentDetail? existingAcademicDetails = await _unitOfWork.StudentDetailRepository
                                                                .GetFirstOrDefault(a => existingStudent.Id == a.StudentId)!;


            if (existingAcademicDetails == null)
            {
                StudentDetail newAcademicDetails = new()
                {
                    StudentId = existingStudent.Id,
                    SscPercentage = dto.SscPercentage,
                    EnrollmentNumber = dto.EnrollementNumber,
                    Branch = dto.DepartmentId,
                    SscBoard = dto.SscBoard,
                    SscPassingYear = dto.SscPassingYear,
                    HscPercentage = dto.HscPercentage,
                    HscBoard = dto.HscBoard,
                    HscPassingYear = dto.HscPassingYear,
                    CpiDiploma = dto.CpiDiploma,
                    EnrollmentYear = dto.EnrollmentYear,
                    PendingBacklog = dto.PendingBacklog,
                    Cgpa = dto.Cgpa,
                    Cpi = dto.Cpi
                };

                GenericReturnDTO addResult = await _unitOfWork.StudentDetailRepository.Add(newAcademicDetails);
                await _unitOfWork.SaveChangesAsync();

                if (addResult.Success)
                {
                    result.Success = true;
                    result.Message = Consts.StudentConst.AcademicDetailsSaved;
                }
                else
                {
                    result.Message = addResult.Message;
                    return result;
                }
            }
            else
            {
                // Update existing academic details
                existingAcademicDetails.SscPercentage = dto.SscPercentage;
                existingAcademicDetails.EnrollmentNumber = dto.EnrollementNumber;
                existingAcademicDetails.SscBoard = dto.SscBoard;
                existingAcademicDetails.SscPassingYear = dto.SscPassingYear;
                existingAcademicDetails.HscPercentage = dto.HscPercentage;
                existingAcademicDetails.HscBoard = dto.HscBoard;
                existingAcademicDetails.HscPassingYear = dto.HscPassingYear;
                existingAcademicDetails.CpiDiploma = dto.CpiDiploma;
                existingAcademicDetails.EnrollmentYear = dto.EnrollmentYear;
                existingAcademicDetails.Branch = dto.DepartmentId;
                existingAcademicDetails.PendingBacklog = dto.PendingBacklog;
                existingAcademicDetails.Cgpa = dto.Cgpa;
                existingAcademicDetails.Cpi = dto.Cpi;

                GenericReturnDTO updateResult = _unitOfWork.StudentDetailRepository.Update(existingAcademicDetails);
                await _unitOfWork.SaveChangesAsync();

                if (updateResult.Success)
                {
                    result.Success = true;
                    result.Message = Consts.StudentConst.AcademicDetailsSaved;
                    await _auditLogger.LogAsync(Consts.OperationsForLog.Modified, Consts.EntityConst.Profile, existingStudent.Id.ToString());
                }
                else
                {
                    result.Message = updateResult.Message;
                    return result;
                }
            }

            bool isEverythingReady = await _globalService.IsEverythingSubmittedByStudent();

            if (isEverythingReady)
            {
                bool valueChanged = await ChangeInitialLoginValueOfStudent(tokenData.Email);
                result.Data = StudentConst.RegistrationCompleted;
            }

        }
        catch (Exception ex)
        {
            result.Message = Consts.StudentConst.AcademicDetailsNotSaved;
        }

        return result;
    }

    public async Task<StudentViewModel> GetCompleteStudentDetails()
    {
        StudentAcademicDetailsDTO studentAcademicDetails = await GetStudentDetails();
        UserDetailsDTO personalDetails = await GetUserDetails(false);
        ResumeDataDTO resumeData = await GetResumeDetails();
        StudentViewModel student = new()
        {
            StudentPersonalDetails = personalDetails,
            StudentAcademicDetails = studentAcademicDetails,
            ResumeData = resumeData
        };

        return student;
    }

    public async Task<GenericReturnDTO> AddPersonalDetails(UserDetailsDTO userDetailsDTO)
    {
        GenericReturnDTO result = new();

        TokenDataDTO tokenData = _globalService.GetTokenData();

        UserAccount? userAccount = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(ua => ua.Email == tokenData.Email)!;

        UserDetail? existingUser = await _unitOfWork.UserDetailsRepository.GetFirstOrDefault(ud => ud.User.Email.ToLower().Trim() == userDetailsDTO.Email!.ToLower().Trim())!;

        UserDetail duplicateDetail = await _unitOfWork.UserDetailsRepository.GetFirstOrDefault(ud =>
                                                            (ud.FirstName == userDetailsDTO.FirstName &&
                                                             ud.MiddleName == userDetailsDTO.MiddleName &&
                                                             ud.LastName == userDetailsDTO.LastName) || ud.User.Email == userDetailsDTO.Email)!;
        if (duplicateDetail != null)
        {
            result.Success = Consts.GeneralConst.StatusFailed;
            result.Message = Consts.ExistsMessage(Consts.EntityConst.Student);
        }

        GenericReturnDTO status = new();

        if (existingUser == null)
        {
            UserDetail newUserDetail = new()
            {
                Address = userDetailsDTO.Address,
                FirstName = userDetailsDTO.FirstName,
                MiddleName = userDetailsDTO.MiddleName,
                LastName = userDetailsDTO.LastName,
                Phone = userDetailsDTO.Phone,
                User = userAccount
            };

            status = await _unitOfWork.UserDetailsRepository.Add(newUserDetail);
        }
        else
        {
            existingUser.Address = userDetailsDTO.Address;
            existingUser.Phone = userDetailsDTO.Phone;
            existingUser.FirstName = userDetailsDTO.FirstName;
            existingUser.MiddleName = userDetailsDTO.MiddleName;
            existingUser.LastName = userDetailsDTO.LastName;

            status = _unitOfWork.UserDetailsRepository.Update(existingUser);
        }

        await _unitOfWork.SaveChangesAsync();

        bool isEverythingReady = await _globalService.IsEverythingSubmittedByStudent();

        if (isEverythingReady)
        {
            bool valueChanged = await ChangeInitialLoginValueOfStudent(tokenData.Email);
            result.Data = StudentConst.RegistrationCompleted;
        }

        if (!status.Success)
        {
            result.Success = false;
            result.Message = Consts.UserConst.UserDetailsNotSaved;
            return result;
        }

        result.Success = true;
        result.Message = Consts.UserConst.UserDetailsSaved;
        return result;
    }

    public async Task<GenericReturnDTO> AddResume(ResumeUploadDTO resumeUpload)
    {
        GenericReturnDTO result = new();

        try
        {
            TokenDataDTO tokenData = _globalService.GetTokenData();

            UserAccount? student = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(u => tokenData.Email.ToLower().Trim() == u.Email.ToLower().Trim());

            var isStudentRegistrationComplete = await _globalService.IsStudentRegistrationComplete();

            if (!isStudentRegistrationComplete)
            {
                result.Message = StudentConst.ShouldCompleteRegistration;
                return result;
            };

            if (student == null)
            {
                result.Message = Consts.ResumeConst.ResumeNotSaved;
            }

            Resume? existingResumeForStudent = await _unitOfWork.ResumeRepository.GetFirstOrDefault(r => r.StudentId == student.Id);


            if (resumeUpload.ResumeFile == null)
            {
                result.Message = Consts.ResumeConst.NoFileSelected;
                return result;
            }

            (string resumePath, string originalFileName) = await FileHelper.SaveResumeAsync(resumeUpload.ResumeFile);

            GenericReturnDTO taskResult = new();

            if (existingResumeForStudent == null)
            {
                var resumeToBeAdded = new Resume
                {
                    StudentId = student!.Id,
                    ResumeFileUrl = resumePath,
                    ResumeName = originalFileName,
                };

                taskResult = await _unitOfWork.ResumeRepository.Add(resumeToBeAdded);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                existingResumeForStudent.ResumeFileUrl = resumePath;
                existingResumeForStudent.StudentId = student!.Id;
                existingResumeForStudent.ResumeName = originalFileName;

                taskResult = _unitOfWork.ResumeRepository.Update(existingResumeForStudent);

                await _unitOfWork.SaveChangesAsync();
            }

            bool isEverythingReady = await _globalService.IsEverythingSubmittedByStudent();

            if (isEverythingReady)
            {
                bool valueChanged = await ChangeInitialLoginValueOfStudent(tokenData.Email);
                result.Data = StudentConst.RegistrationCompleted;
            }

            if (taskResult.Success)
            {
                result.Success = true;
                result.Message = Consts.ResumeConst.ResumeSaved;
                return result;
            }
        }
        catch (Exception)
        {

            result.Message = Consts.ResumeConst.ResumeNotSaved;
        }
        return result;
    }

    public async Task<bool> ChangeInitialLoginValueOfStudent(string email)
    {
        try
        {
            UserAccount? userAccount = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(u => u.Email == email)!;
            userAccount.IsInitialLogin = false;

            GenericReturnDTO result = _unitOfWork.AuthenticationRepository.Update(userAccount);
            await _unitOfWork.SaveChangesAsync();
            return result.Success;

        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<UserCredentialsDTO>> GetStudentsList()
    {
        List<UserAccount> users = await _unitOfWork.AuthenticationRepository.GetList(ua => ua.RoleId == (int)RoleEnum.Student);

        List<UserCredentialsDTO> userCredentials = users.Select(u => new UserCredentialsDTO
        {
            Email = u.Email
        }).ToList();

        return userCredentials;
    }
}
