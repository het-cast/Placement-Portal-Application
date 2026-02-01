// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
GetNotificationsData();
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.on("ReceiveNotification", function (message, id) {
    console.log("Received Signal from the server");
    GetNotificationsData();
});

connection.start().catch(function (err) {
    console.error(err.toString());
});

connection.onclose(() => {
    console.warn("SignalR connection closed");
});

connection.onreconnecting(error => {
    console.warn("Reconnecting to SignalR:", error);
});

connection.onreconnected(() => {
    console.log("SignalR reconnected");
});

// function GetNotificationsData() {
//     $.ajax({
//         url: "/Notification/GetUnreadNotificationsCountForCurrentUser",
//         type: "GET",
//         data: {
//         },
//         success: function (data) {
//             console.log(data)
//             console.log(data.data.length);
//             const notificationsContainerUl = $(".notifications-dropdown-menu");
//             const notificationsArray = data.data;
//             const unreadNotificationsCount = data.data.length;
//             let liElements = `<li id="readAllNotificationBtn" class="w-100 d-flex justify-content-end">
//                                 <span class="dropdown-item btn me-3" style="color: #764ba2; border: 1px solid #764ba2; width:40%;">Read All
//                                 </span>
//                               </li>
//                               <li>
//                                 <hr class="dropdown-divider">
//                               </li>
//                               `;
//             if (unreadNotificationsCount <= 0) {
//                 const noNotificationHtml = `
//                     <li>
//                         <span class="dropdown-item"> No Notifications there to be shown
//                         </span>
//                     </li>
//                 `
//                 notificationsContainerUl.html(noNotificationHtml);
//             }
//             if (unreadNotificationsCount > 0) {
//                 $("#notificationsCount").html(unreadNotificationsCount);
//                 notificationsArray.forEach(notification => {
//                     liElements += `
//                         <li class="">
//                             <div class="d-flex flex-wrap text-wrap">
//                                 <span class="dropdown-item text-wrap">${notification.message}
//                                 </span>
//                                 <button id="readSingularNotificationBtn" data-notificationMapId="${notification.id}" class="w-75 d-flex justify-content-center" style="color: #764ba2; border: 1px solid #764ba2;">
//                                     Read
//                               </button>
//                             </div> 
//                         </li>
//                         <li>
//                             <hr class="dropdown-divider">
//                         </li>
//                     `
//                 });
//                 notificationsContainerUl.html(liElements);
//             }
//         },
//         error: function (xhr, status, error) {

//         },
//     });
// }
function GetNotificationsData() {
    $.ajax({
        url: "/Notification/GetUnreadNotificationsCountForCurrentUser",
        type: "GET",
        success: function (data) {
            const notificationsContainerUl = $(".notifications-container");
            const notificationsArray = data.data || [];
            const unreadNotificationsCount = notificationsArray.length;

            let liElements = "";

            if (unreadNotificationsCount <= 0) {
                $("")
                liElements = `
            <li class="notification-item">
              <span>No notifications to display.</span>
            </li>
          `;
                $("#notificationsCount").removeClass("notifications-count").text(""); // Clear bell badge
            } else {
                $("#notificationsCount").addClass("notifications-count").text(unreadNotificationsCount);

                notificationsArray.forEach((notification) => {
                    liElements += `
              <div class="notification-item">
                <div class="d-flex gap-2 align-items-center justify-content-between">
                  <span class="message">${notification.message}</span>
                  <div class="d-flex justify-content-end">
                   <button title="Mark it read" class="read-btn read-singular-notification d-flex justify-content-center align-items-center" data-notificationmapid="${notification.id}" style="width: 50px; height: 50%;" > 
                    <i class="fa-solid fa-check"></i>
                   </button>
                  </div>
                </div>
              </div>
            `;
                });

                liElements += `
            <div class="read-all">
              <button id="readAllNotificationBtn">Read All</button>
            </div>
          `;
            }

            notificationsContainerUl.html(liElements);
        },
        error: function (xhr, status, error) {
            console.error("Notification fetch error:", error);
        },
    });
}

$(document).on("click", "#readAllNotificationBtn", function (e) {
    e.preventDefault();
    MarkReadAllNotifications();
})

$(document).on("click", ".read-singular-notification", function (e) {
    e.preventDefault();
    e.stopPropagation();
    const notificationMappingId = $(this).attr("data-notificationmapid");
    console.log(notificationMappingId);
    MarkReadNotification(notificationMappingId);
})

function MarkReadAllNotifications() {
    $.ajax({
        url: "/Notification/ReadAllNotificationsOfCurrentUser",
        type: "POST",
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
            }
            else {
                toastr.error(response.message);
            }
            GetNotificationsData();
        },
        error: function (xhr, status, error) {

        },
    });
}

function MarkReadNotification(notificationMappingId) {
    $.ajax({
        url: "/Notification/ReadSingularNotification",
        type: "POST",
        data: {
            notificationMapId: notificationMappingId
        },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
            }
            else {
                toastr.error(response.message);
            }
            GetNotificationsData();
        },
        error: function (xhr, status, error) {

        },
    });
}