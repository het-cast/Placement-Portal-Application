$(document).ready(function () {
  const userEmailProtected = GetUserEmailFromQuery();
  console.log(userEmailProtected)
  if (userEmailProtected != undefined || userEmailProtected != null) {
    $("#mfaInputEmail").val(userEmailProtected);
  }
})

function GetUserEmailFromQuery() {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get("user");
}

$(document).on("click", "#PasswordToggle", function () {
  $(this).toggleClass("fa-eye-slash fa-eye");
  let passwordInputElement = document.getElementById("LoginPasswordInput");
  if (passwordInputElement.type == "password") {
    passwordInputElement.type = "text";
  } else if (passwordInputElement.type == "text") {
    passwordInputElement.type = "password";
  }
});

$(document).on("click", "#LoginSubmit", function (e) {
  e.preventDefault();
  let rememberMe = CheckRememberMe();

  let loginForm = $("#loginSubmitForm");
  $.validator.unobtrusive.parse(loginForm);
  let formData = new FormData(loginForm[0]);

  formData.append("rememberMe", rememberMe);

  if (loginForm.valid()) {
    showSpinner();
    AuthenticateUserMFA(formData);
  }
});

$(document).on("click", "#addStudentSubmit", function (e) {
  e.preventDefault();

  let addStudentForm = $("#addStudentForm");
  $.validator.unobtrusive.parse(addStudentForm);
  let addStudentFormData = new FormData(addStudentForm[0]);

  if (addStudentForm.valid()) {
    showSpinner();
    AddStudentCredentials(addStudentFormData);
  }
});

$(document).on('click', "#verifyOTPSubmitAndLogin", function (e) {
  e.preventDefault();

  const email = $("#mfaInputEmail").val();
  const otpInput = $("#otp");
  const otp = otpInput.val().trim();

  otpInput.removeClass("is-invalid");

  if (otp === "") {
    toastr.error("Please enter OTP to proceed.");
    otpInput.addClass("is-invalid").focus();
    return;
  }

  showSpinner();
  VerifyOTPandLogin(email, otp);
});

$(document).on('input', '#otp', function () {
  if ($(this).val().trim() === "") {
    $(this).addClass("is-invalid").focus();
  }
  $(this).removeClass('is-invalid');
});



function VerifyOTPandLogin(email, otp) {
  $.ajax({
    url: "/Authentication/VerifyOTPandLogin",
    type: "Post",
    data: {
      email: email,
      otp: otp
    },
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        setTimeout(() => {
          hideSpinner();
          window.location.href = "/Dashboard/Dashboard";
        }, 1200);
      }
      else {
        toastr.error(response.message);
        setTimeout(() => {
          hideSpinner();
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

$(document).on("click", ".add-user", function (e) {
  e.preventDefault();
  const roleId = $(this).attr("data-usertype");
  let addUserForm = $("#addUserForm");
  $.validator.unobtrusive.parse(addUserForm);
  let addUserFormData = new FormData(addUserForm[0]);

  addUserFormData.append("RoleId", roleId);

  if (addUserForm.valid()) {
    showSpinner();
    AddUserCredentials(addUserFormData);
  }
})

function CheckRememberMe() {
  let rememberMe = $("#RememberMe").prop("checked");
  return rememberMe;
}

function AuthenticateUser(formData) {
  $.ajax({
    url: "/Authentication/AuthenticateUser",
    type: "POST",
    data: formData,
    processData: false,
    contentType: false,
    success: function (response) {
      if (response.success) {
        window.location.href = response.redirectToUrl;
        toastr.success(response.message);
        hideSpinner();
      } else {
        hideSpinner();
        toastr.error(response.message);
        console.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      alert("An error occurred: " + error);
    },
  });
}

function AuthenticateUserMFA(formData) {
  $.ajax({
    url: "/Authentication/AuthenticateUserMFA",
    type: "POST",
    data: formData,
    processData: false,
    contentType: false,
    success: function (response) {
      if (response.mfaEnabled === "") {
        AuthenticateUser(formData);
      }
      else {
        if (response.success) {
          toastr.success(response.message);
          setTimeout(() => {
            hideSpinner();
            window.location.href = `/Authentication/MFAOtpVerify?user=${response.data}`;;
          }, 1200);
        } else {
          hideSpinner();
          toastr.error(response.message);
        }
      }
    },
    error: function (xhr, status, error) {
      alert("An error occurred: " + error);
    },
  });
}

function AddStudentCredentials(formData) {
  $.ajax({
    url: "/Authentication/AddStudentCredentials",
    type: "POST",
    data: formData,
    processData: false,
    contentType: false,
    success: function (response) {
      console.log(response);
      if (response.success) {
        toastr.success(response.message);
        setTimeout(() => {
          hideSpinner();
          window.location.href = response.data;
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

function AddUserCredentials(formData) {
  $.ajax({
    url: "/Authentication/AddUserCredentials",
    type: "POST",
    data: formData,
    processData: false,
    contentType: false,
    success: function (response) {
      console.log(response);
      if (response.success) {
        toastr.success(response.message);
        setTimeout(() => {
          hideSpinner();
          window.location.href = response.data;
        }, 1000);

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


function showSpinner() {
  $("#loader").show();
}

function hideSpinner() {
  $("#loader").hide();
}