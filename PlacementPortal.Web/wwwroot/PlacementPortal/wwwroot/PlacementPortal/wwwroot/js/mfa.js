$(document).ready(function () {
    const userEmailProtected = GetUserEmailFromQuery();
    console.log(userEmailProtected)
    if (userEmailProtected != undefined || userEmailProtected != null) {
        showSpinner();
        RenderMFAData(userEmailProtected);
    }
    UpdateEnableDisableMFAButton();
})

$(document).on('click', "#enableMFABtn", function () {
    GetUserAuthForm();
})

$(document).on("click", "#AuthenticateUser", function (e) {
    e.preventDefault();
    const enableButton = $("#enableMFABtn").attr("data-disablemfa") == "false";
    let userAuthForm = $("#userAuthForm");
    $.validator.unobtrusive.parse(userAuthForm);
    let userAuthFormData = new FormData(userAuthForm[0]);

    if (userAuthForm.valid()) {
        showSpinner();
        if(enableButton){
            ValidateUserCredentials(userAuthFormData);
        }
        else{
            ValidateUserCredentialsAndDisableMFA(userAuthFormData);
        }
    }
});

$(document).on('click', "#verifyOTPSubmit", function (e) {
    e.preventDefault();
    const email = $("#mfaInputEmail").val();
    const otp = $("#otp").val();
    VerifyOtp(email, otp)
})

function ValidateUserCredentialsAndDisableMFA(formData) {
    $.ajax({
        url: "/User/DisableMFA",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            console.log(response);
            $("#userAuthModal").modal("hide");
            if (response.success) {
                toastr.success(response.message);
                setTimeout(() => {
                    hideSpinner();
                    window.location.href = `/Authentication/Logout`;
                }, 1500);

            } else {
                toastr.error(response.message);
                hideSpinner();
            }
        },
        error: function (xhr, status, error) {
            alert("An error occurred: " + error);
            hideSpinner();
        },
    });
}
function ValidateUserCredentials(formData) {
    $.ajax({
        url: "/User/ValidateUserCredentials",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            console.log(response);
            $("#userAuthModal").modal("hide");
            if (response.success) {
                toastr.success(response.message);
                setTimeout(() => {
                    hideSpinner();
                    window.location.href = `/User/RegisterMFA?user=${response.data}`;
                }, 1500);

            } else {
                toastr.error(response.message);
                hideSpinner();
            }
        },
        error: function (xhr, status, error) {
            alert("An error occurred: " + error);
            hideSpinner();
        },
    });
}

function GetUserAuthForm() {
    $.ajax({
        url: "/User/GetUserAuthForm",
        type: "GET",
        success: function (response) {
            $("#userAuthModalMain").html(response);
        },
        error: function (xhr, status, error) {
            console.warn(status);
            console.warn(xhr);
            console.warn(error);
        },
    });
}

function RenderMFAData(protectedEmail) {
    $.ajax({
        url: "/User/GetMFAData",
        type: "Post",
        data: {
            protectedEmail: protectedEmail
        },
        success: function (response) {
            $("#authenticatorDataMain").html(response);
            hideSpinner();
        },
        error: function (xhr, status, error) {
            console.warn(status);
            console.warn(xhr);
            console.warn(error);
        },
    });
}

function VerifyOtp(email, otp) {
    $.ajax({
        url: "/Authentication/VerifyOtp",
        type: "Post",
        data: {
            email: email,
            otp: otp
        },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                setTimeout(() => {
                    window.location.href = "/Dashboard/Dashboard";
                }, 1200);
            }
            else {
                toastr.error(response.message);
                setTimeout(() => {
                    window.location.href = "/Authentication/Logout";
                }, 1200);
                
            }
        },
        error: function (xhr, status, error) {
            console.warn(status);
            console.warn(xhr);
            console.warn(error);
        },
    });
}

function GetUserEmailFromQuery() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get("user");
}

function CheckIfMFAEnabled() {
    return new Promise((resolve, reject) => {
      $.ajax({
        url: "/User/CheckIfMFAEnabled",
        type: "Get",
        success: function (data) {
          resolve(data);
        },
        error: function (xhr, status, error) {
          reject(0);
        },
      });
    });
}

async function GetCurrentUserMFAEnabledData(){
    const data = await CheckIfMFAEnabled();
    console.log(data);
    return data;
}

async function UpdateEnableDisableMFAButton(){
    const data = await GetCurrentUserMFAEnabledData();
    const enableDisableButton = $("#enableMFABtn");
    if(data.success){
        enableDisableButton.text("Disable").attr("data-disablemfa", true);
    }
}

// Shows Spinner
function showSpinner() {
    $("#loader").show();
}

// Hides Spinner
function hideSpinner() {
    $("#loader").hide();
}