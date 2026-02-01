using System.Buffers;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Ocsp;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.Hubs;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Service;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IGlobalService _globalService;

    public NotificationService(IHubContext<NotificationHub> hubContext, IUnitOfWork unitOfWork, IGlobalService globalService)
    {
        _hubContext = hubContext;
        _unitOfWork = unitOfWork;
        _globalService = globalService;
    }

    public async Task<List<UserAccountDTO>> GetStudentsList()
    {
        (int totalCount, List<UserAccountDTO> studentsList) = await _unitOfWork.AuthenticationRepository
                                                                                .GetListAllDTOs(ua => ua.RoleId == (int)RoleEnum.Student,
                                                                                    ua => new UserAccountDTO
                                                                                    {
                                                                                        Email = ua.Email,
                                                                                        Id = ua.Id,
                                                                                        RoleId = ua.RoleId
                                                                                    }
                                                                                );
        return studentsList;
    }

    public async Task<List<UserAccountDTO>> GetTPOsList()
    {
        (int totalCount, List<UserAccountDTO> tposList) = await _unitOfWork.AuthenticationRepository
                                                                                .GetListAllDTOs(ua => ua.RoleId == (int)RoleEnum.TPO,
                                                                                    ua => new UserAccountDTO
                                                                                    {
                                                                                        Email = ua.Email,
                                                                                        Id = ua.Id,
                                                                                        RoleId = ua.RoleId
                                                                                    }
                                                                                );
        return tposList;
    }

    public async Task<OperationStatusDTO> SaveNotificationForSpecificUser(int userId, string message)
    {
        OperationStatusDTO result = new();

        Notification notification = new()
        {
            Message = message,
        };

        NotificationUserMapping notificationMapping = new()
        {
            UserId = userId,
            Notification = notification
        };

        GenericReturnDTO addResultNotification = await _unitOfWork.NotificationRepository.Add(notification);
        GenericReturnDTO addResultNotificationMapping = await _unitOfWork.NotificationMappingRepository.Add(notificationMapping);
        await _unitOfWork.SaveChangesAsync();

        result.Success = Consts.GeneralConst.StatusSuccess;
        result.Message = Consts.AddedMessage(Consts.EntityConst.Notfication);

        await NotifyUserAsync(userId.ToString(), message);

        return result;
    }

    public async Task<OperationStatusDTO> SaveNotificationsForStudentsEnhancedMessage(int senderId, string message)
    {
        OperationStatusDTO result = new();

        List<UserAccountDTO> students = await GetStudentsList();

        UserAccount sender = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(u => u.Id == senderId)!;

        string senderEmail = sender.Email;

        string modifiedMessage = $"{senderEmail} {message}";

        Notification notification = new()
        {
            Message = modifiedMessage,
        };

        await _unitOfWork.NotificationRepository.Add(notification);
        await _unitOfWork.SaveChangesAsync();

        List<NotificationUserMapping> notificationMappings = new();

        foreach (var student in students)
        {
            NotificationUserMapping notificationMapping = new()
            {
                Notification = notification,
                UserId = student.Id,
            };

            notificationMappings.Add(notificationMapping);
        }


        await _unitOfWork.NotificationMappingRepository.AddRange(notificationMappings);
        result.Success = true;
        result.Message = Consts.AddedMessage(Consts.EntityConst.Notfication);
        await NotifyAllUsersAsync(message);
        return result;
    }

    public async Task<OperationStatusDTO> SaveNotificationAsyncsForAllStudents(string message)
    {
        OperationStatusDTO result = new();

        List<UserAccountDTO> students = await GetStudentsList();
        Notification notification = new()
        {
            Message = message,
        };

        await _unitOfWork.NotificationRepository.Add(notification);
        await _unitOfWork.SaveChangesAsync();

        List<NotificationUserMapping> notificationMappings = new();

        foreach (var student in students)
        {
            NotificationUserMapping notificationMapping = new()
            {
                Notification = notification,
                UserId = student.Id,
            };

            notificationMappings.Add(notificationMapping);
        }


        await _unitOfWork.NotificationMappingRepository.AddRange(notificationMappings);
        result.Success = true;
        result.Message = Consts.AddedMessage(Consts.EntityConst.Notfication);
        return result;
    }

    public async Task<OperationStatusDTO> SaveNotificationAsyncsForAllTPOs(string message)
    {
        OperationStatusDTO result = new();

        List<UserAccountDTO> tpos = await GetTPOsList();
        Notification notification = new()
        {
            Message = message,
        };

        await _unitOfWork.NotificationRepository.Add(notification);
        await _unitOfWork.SaveChangesAsync();

        List<NotificationUserMapping> notificationMappings = new();

        foreach (var tpo in tpos)
        {
            NotificationUserMapping notificationMapping = new()
            {
                Notification = notification,
                UserId = tpo.Id,
            };

            notificationMappings.Add(notificationMapping);
        }


        await _unitOfWork.NotificationMappingRepository.AddRange(notificationMappings);
        result.Success = true;
        result.Message = Consts.AddedMessage(Consts.EntityConst.Notfication);
        await NotifyAllUsersAsync(message);
        return result;
    }


    public async Task NotifyUserAsync(string userId, string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", message, userId);
    }

    public async Task NotifyAllUsersAsync(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
    }

    public async Task NotifyRoleAsync(string role, string message)
    {
        List<UserAccount> users = await _unitOfWork.AuthenticationRepository.GetList(u => u.RoleId == (int)Enum.Parse(typeof(RoleEnum), role));

        foreach (var user in users)
        {
            await NotifyUserAsync(user.Email, message);
        }
    }

    public async Task<List<NotificationDTO>> RetrieveNotificationsForCurrentUser()
    {
        try
        {
            TokenDataDTO tokenData = _globalService.GetTokenData();
            (int totalCount, List<NotificationDTO> notifications) = await _unitOfWork.NotificationMappingRepository
                                                             .GetListAllDTOs(
                                                                n => !n.IsRead && n.UserId == tokenData.UserId && !n.IsDeleted,
                                                                n => new NotificationDTO
                                                                {
                                                                    CreatedAt = n.CreatedAt ?? DateTime.Now,
                                                                    Message = n.Notification.Message,
                                                                    IsRead = n.IsRead,
                                                                    Id = n.Id
                                                                }
                                                                );

            return notifications;
        }
        catch (Exception)
        {
            return new List<NotificationDTO>();
        }
    }

    public async Task<OperationStatusDTO> ReadAllNotificationsOfCurrentUser()
    {
        OperationStatusDTO result = new();
        try
        {
            TokenDataDTO tokenData = _globalService.GetTokenData();
            List<NotificationUserMapping> notificationMappings = await _unitOfWork.NotificationMappingRepository.GetList(n => n.UserId == tokenData.UserId && !n.IsRead && !n.IsDeleted);
            foreach (var notification in notificationMappings)
            {
                notification.IsRead = true;
            }

            _unitOfWork.NotificationMappingRepository.UpdateRange(notificationMappings);
            result.Success = Consts.GeneralConst.StatusSuccess;
            result.Message = Consts.NotificationConst.MarkedAsReadAll;
        }
        catch (Exception)
        {
            result.Message = Consts.NotificationConst.NotMarkedAsRead;
        }
        return result;
    }

    public async Task<OperationStatusDTO> ReadSingularNotifications(int notificationMapId)
    {
        OperationStatusDTO result = new();
        try
        {
            NotificationUserMapping notificationMapping = await _unitOfWork.NotificationMappingRepository.GetFirstOrDefault(n => n.Id == notificationMapId)!;

            notificationMapping.IsRead = true;
            _unitOfWork.NotificationMappingRepository.Update(notificationMapping);
            await _unitOfWork.SaveChangesAsync();
            result.Success = Consts.GeneralConst.StatusSuccess;
            result.Message = Consts.NotificationConst.MarkedAsRead;
        }
        catch (Exception)
        {
            result.Message = Consts.NotificationConst.NotMarkedAsRead;
        }
        return result;
    }

}
