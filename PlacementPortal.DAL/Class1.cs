namespace PlacementPortal.DAL;

public class Class1
{

}
// STEP 1: Install SignalR Packages via NuGet
// In both your Web project and any shared libraries:
// Install-Package Microsoft.AspNetCore.SignalR

// STEP 2: Create the Notification Hub

// public class NotificationHub : Hub
// {
//     public async Task SendNotification(string userId, string message)
//     {
//         await Clients.User(userId).SendAsync("ReceiveNotification", message);
//     }
// }

















// // STEP 3: Configure SignalR in Program.cs
// builder.Services.AddSignalR();

// // Add this in your endpoints section:
// app.MapHub<NotificationHub>("/notificationHub");

// // STEP 4: Create a NotificationSender Service
// using Microsoft.AspNetCore.SignalR;

// public interface INotificationSender
// {
//     Task NotifyUserAsync(string userId, string message);
//     Task NotifyRoleAsync(string role, string message);
// }

// public class NotificationSender : INotificationSender
// {
//     private readonly IHubContext<NotificationHub> _hubContext;
//     private readonly IUserConnectionManager _connectionManager;

//     public NotificationSender(IHubContext<NotificationHub> hubContext, IUserConnectionManager connectionManager)
//     {
//         _hubContext = hubContext;
//         _connectionManager = connectionManager;
//     }

//     public async Task NotifyUserAsync(string userId, string message)
//     {
//         await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
//     }

//     public async Task NotifyRoleAsync(string role, string message)
//     {
//         var users = await _connectionManager.GetUserIdsByRole(role);
//         foreach (var userId in users)
//         {
//             await NotifyUserAsync(userId, message);
//         }
//     }
// }

// // STEP 5: Track User Connections (Optional)
// public interface IUserConnectionManager
// {
//     Task<IEnumerable<string>> GetUserIdsByRole(string role);
// }

// public class UserConnectionManager : IUserConnectionManager
// {
//     private readonly YourDbContext _context;

//     public UserConnectionManager(YourDbContext context)
//     {
//         _context = context;
//     }

//     public async Task<IEnumerable<string>> GetUserIdsByRole(string role)
//     {
//         return await _context.Users
//             .Where(u => u.Role == role)
//             .Select(u => u.Id.ToString())
//             .ToListAsync();
//     }
// }

// // STEP 6: Register Services
// builder.Services.AddScoped<INotificationSender, NotificationSender>();
// builder.Services.AddScoped<IUserConnectionManager, UserConnectionManager>();

// // STEP 7: Use Notification in Your Service
// public async Task<OperationStatusDTO> SaveResumeComment(ResumeCommentViewModel resumeComment)
// {
//     OperationStatusDTO result = new();

//     try
//     {
//         Resume resume = await _unitOfWork.ResumeRepository.GetFirstOrDefault(r => r.Id == resumeComment.ResumeId)!;

//         if (resume == null)
//         {
//             result.Message = Consts.NotFound(Consts.EntityConst.Resume);
//             return result;
//         }

//         if (resume.Comment != null && resume.Comment.ToLower().Trim().Equals(resumeComment.ResumeComment?.ToLower().Trim()))
//         {
//             result.Message = Consts.SavedMessage(Consts.EntityConst.Resume);
//             return result;
//         }

//         resume.Comment = resumeComment.ResumeComment;
//         GenericReturnDTO genericReturn = _unitOfWork.ResumeRepository.Update(resume);
//         await _unitOfWork.SaveChangesAsync();

//         result.Success = Consts.GeneralConst.StatusSuccess;
//         result.Message = Consts.SavedMessage(Consts.EntityConst.Resume);

//         // Notify all students
//         await _notificationSender.NotifyRoleAsync("Student", $"Resume Comment Updated: {resumeComment.ResumeComment}");

//         return result;
//     }
//     catch (Exception)
//     {
//         result.Message = Consts.FailToSaveMessage(Consts.EntityConst.Resume);
//         return result;
//     }
// }

// // STEP 8: Frontend JS (in _Layout.cshtml or specific View)
// <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.5/signalr.min.js"></script>
// <script>
//     const connection = new signalR.HubConnectionBuilder()
//         .withUrl("/notificationHub")
//         .build();

//     connection.on("ReceiveNotification", function (message) {
//         toastr.info(message);
//     });

//     connection.start().catch(function (err) {
//         return console.error(err.toString());
//     });
// </script>
