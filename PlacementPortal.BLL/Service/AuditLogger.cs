using Microsoft.AspNetCore.Http;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Service;


public class AuditLogger : IAuditLogger
{

    private readonly IGlobalService _globalService;

    private readonly IUnitOfWork _unitOfWork;

    public AuditLogger(IGlobalService globalService, IUnitOfWork unitOfWork)
    {
        _globalService = globalService;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationStatusDTO> LogAsync(string action, string entityName, string entityId, string? details = null)
    {
        OperationStatusDTO result = new();

        try
        {
            TokenDataDTO tokenData = _globalService.GetTokenData();

            AuditLog log = new()
            {
                ActionByEmail = tokenData.Email,
                Role = tokenData.Role,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                Timestamp = DateTime.Now
            };

            await _unitOfWork.AuditLogRepository.Add(log);
            await _unitOfWork.SaveChangesAsync();

            result.Success = Consts.GeneralConst.StatusSuccess;
            result.Message = Consts.AuditLog.LogAdded;
            return result;
        }
        catch (Exception)
        {
            result.Message = Consts.AuditLog.LogNotAdded;
            return result;
        }
    }
}


