var isInitialLoginStudent = false;

$(document).ready(function () {
  IsInitialLoginStudent();
});

$(document).on("click", ".form-tab-student", function () {
  $(".nav-link").removeClass("active");
  $(this).addClass("active");

  const selectedTab = $(this).data("tab");
  loadTabContent(selectedTab);
});

$(document).on("click", "#personalDetailsSubmit", function (e) {
  e.preventDefault();
  let personalDetailsStudentForm = $("#PersonalDetailsStudentForm");
  $.validator.unobtrusive.parse(personalDetailsStudentForm);
  let personalDetailsStudentFormData = new FormData(
    personalDetailsStudentForm[0]
  );

  if (personalDetailsStudentForm.valid()) {
    showSpinner();
    AddStudentPersonalDetails(personalDetailsStudentFormData);
  }
});

$(document).on("click", "#StudentAcadmicDetailsFormSubmit", function (e) {
  e.preventDefault();

  let sscPassingYear = parseInt($("#sscPassingYear").val(), 10);
  let hscPassingYear = parseInt($("#hscPassingYear").val(), 10);
  let collegePassingYear = parseInt($("#passoutYear").val(), 10);
  console.log(collegePassingYear)
  if (hscPassingYear < sscPassingYear + 2) {
    toastr.error("Enter valid Hsc / Ssc Passing Year");
    // $("#StudentAcadmicDetailsForm")
    //     .find("input[id='sscPassingYear'], input[id='hscPassingYear']")
    //     .addClass("is-invalid");
    return;
  }

  if(collegePassingYear < (hscPassingYear + 4) || collegePassingYear < (sscPassingYear))
  {
    toastr.error("Enter a valid college passout year");
    return;
  }



  let studentAcademicDetailsStudentForm = $("#StudentAcadmicDetailsForm");
  $.validator.unobtrusive.parse(studentAcademicDetailsStudentForm);
  let studentAcademicDetailsStudentFormData = new FormData(
    studentAcademicDetailsStudentForm[0]
  );

  if (studentAcademicDetailsStudentForm.valid()) {
    showSpinner();
    AddStudentAcademicDetails(studentAcademicDetailsStudentFormData);
  }
});

$(document).on("click", "#navigateToUpdateStudent", function (e) {
  e.preventDefault();
  RenderUpdateStudentDetailsEditPartialView();
});

$(document).on("click", "#navigateToStudentDetails", function (e) {
  e.preventDefault();
  RenderStudentDetailsPartialView();
});

$(document).on("click", "#addResumeUpload", function (e) {
  e.preventDefault();
  let resumeForm = $("#addResumeForm");
  $.validator.unobtrusive.parse(resumeForm);
  let resumeFormData = new FormData(resumeForm[0]);

  if (resumeForm.valid()) {
    showSpinner()
    AddResume(resumeFormData);
  }
})

function AddResume(resumeFormData) {
  $.ajax({
    url: "/Student/UploadResume",
    type: "POST",
    data: resumeFormData,
    processData: false,
    contentType: false,
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        if (response.data == true) {
          setTimeout(() => {
            hideSpinner();
            window.location.reload();
          }, 1500);
        }
        else {
          hideSpinner();
        }

        // response.data == true ? window.location.reload() : "";
      } else {
        GetResumeInfo();
        toastr.error(response.message);
        hideSpinner();
      }
    },
    error: function (xhr, status, error) {
      alert("An error occurred: " + error);
    },
  });
}

function AddStudentPersonalDetails(personalDetailsStudentFormData) {
  $.ajax({
    url: "/Student/AddPersonalDetails",
    type: "POST",
    data: personalDetailsStudentFormData,
    processData: false,
    contentType: false,
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        if (response.data == true) {
          setTimeout(() => {
            hideSpinner();
            window.location.reload();
          }, 1500);
        }
        else {
          setTimeout(() => {
            hideSpinner();
            loadTabContent(academic);
          }, 1500);
        }


      } else {
        toastr.error(response.message);
        hideSpinner();
      }
    },
    error: function (xhr, status, error) {
      alert("An error occurred: " + error);
    },
  });
}

function AddStudentAcademicDetails(academicDetailsFormData) {
  $.ajax({
    url: "/Student/UpdateAcademicDetails",
    type: "POST",
    data: academicDetailsFormData,
    processData: false,
    contentType: false,
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        if (response.data == true) {
          setTimeout(() => {
            hideSpinner()
            window.location.reload();
          }, 1500);
        }
        else {
          hideSpinner();
        }
      } else {
        toastr.error(response.message);
        hideSpinner();
      }
    },
    error: function (xhr, status, error) {
      toastr.error("Some Error Occurred");
    },
  });
}

function loadTabContent(tabName) {
  let url = "";

  // if (isInitialLoginStudent) {
  //   if (tabName === "personal") {
  //     url = "/Student/PersonalDetails";
  //   } else if (tabName === "academic") {
  //     url = "/Student/AcademicDetails";
  //   }
  // } else {
  if (tabName === "personal") {
    url = "/Student/GetPersonalDetails";
  } else if (tabName === "academic") {
    url = "/Student/GetAcademicDetails";
  } else if (tabName === "resumeupload") {
    url = "/Student/ResumeUploadView"
  }

  $("#tabContent").html('<div class="text-center my-3">Loading...</div>');

  $.ajax({
    url: url,
    data: {
      initialLogin: isInitialLoginStudent
    },
    type: "GET",
    success: function (data) {
      $("#tabContent").html(data);
      if (tabName === "academic") {
        GetDepartmentsList();
      }
      if (tabName == "resumeupload") {
        GetResumeInfo();
      }
    },
    error: function (xhr, status, error) {
      console.error("Error loading " + tabName + " tab:", error);
      $("#tabContent").html(
        '<div class="text-danger">Error loading tab content.</div>'
      );
    },
  });
}

function GetResumeInfo() {
  $.ajax({
    url: "/Student/GetResumeInfo",
    type: "GET",
    data: {
    },
    success: function (data) {
      const resumeDetails = data.data;
      if (resumeDetails && resumeDetails.originalFileName) {
        $('#file-name-display').text(resumeDetails.originalFileName);
      }
      else {
        $('#file-name-display').addClass("text-danger").text('No file uploaded yet');
      }

    },
    error: function (xhr, status, error) {
      console.error("Error checking student progress", error);
    },
  });
}

function IsInitialLoginStudent() {
  $.ajax({
    url: "/Student/CheckStudentProgress",
    type: "GET",
    data: {
    },
    success: function (data) {
      isInitialLoginStudent = data.initialLogin;
      if (isInitialLoginStudent === true) {
        RenderStudentDetailsEditPartialView();
      } else {
        RenderStudentDetailsPartialView();
      }
    },
    error: function (xhr, status, error) {
      console.error("Error checking student progress", error);
    },
  });
}

function RenderStudentDetailsEditPartialView() {
  $.ajax({
    url: "/Student/UpdateStudentDetails",
    type: "GET",
    data: {},
    success: function (data) {
      $("#StudentMain").html(data);
      loadTabContent("personal");
    },
    error: function (xhr, status, error) { },
  });
}

function RenderUpdateStudentDetailsEditPartialView() {
  $.ajax({
    url: "/Student/UpdateStudentDetails",
    type: "GET",
    data: {},
    success: function (data) {
      $("#StudentMain").html(data);
      loadTabContent("personal");
    },
    error: function (xhr, status, error) { },
  });
}

function RenderStudentDetailsPartialView() {
  $.ajax({
    url: "/Student/GetStudentData",
    type: "GET",
    data: {},
    success: function (data) {
      $("#StudentMain").html(data);
    },
    error: function (xhr, status, error) { },
  });
}

function GetDepartmentsList() {
  $.ajax({
    url: "/Student/GetDepartments",
    type: "GET",
    success: function (data) {
      let selectHtml = $("#selectDepartmentStudent");
      selectHtml.empty();

      selectHtml.append('<option value="" selected>Select Department</option>');

      data.departments.forEach((dept) => {
        selectHtml.append(`<option value="${dept.id}">${dept.name}</option>`);
      });
      // if (!isInitialLoginStudent) {
      GetDepartmentIdForStudent();
      // }
    },
    error: function () {
      console.error("Failed to fetch department list");
    },
  });
}

function GetDepartmentIdForStudent() {
  $.ajax({
    url: "/Student/GetDepartmentForStudent",
    type: "GET",
    success: function (data) {
      let selectHtml = $("#selectDepartmentStudent");
      selectHtml.val(data.departmentId);
      console.log(data.departmentId)
      if (data.departmentId == 0) {
        selectHtml.val("")
      }
    },
    error: function () {
      console.error("Failed to fetch department list");
    },
  });
}

function displayFileName(input) {
  const fileName = input.files.length > 0 ? input.files[0].name : 'No file chosen';
  $('#file-name-display').text(fileName);
}

function LoadUpdateStudentsPartialView() {
  $.ajax({
    url: "/Student/GetStudentData",
    type: "GET",
    data: {},
    success: function (data) {
      $("#StudentMain").html(data);
      loadTabContent("personal");
    },
    error: function (xhr, status, error) { },
  });
}

function showSpinner() {
  $("#loader").show();
}

function hideSpinner() {
  $("#loader").hide();
}