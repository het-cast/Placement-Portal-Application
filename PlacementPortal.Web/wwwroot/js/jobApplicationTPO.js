$(document).ready(function () {
  GetJobApplications();
});

let searchFilterJobApp = $("#searchFilterJobApp").val();
let pageSizeJobApp = 5;
let pageNumberJobApp = 1;
let sortColumnJobApp;
let sortOrderJobApp;
let statusFilterJobApp = -1;

$(document).on("keyup", "#searchFilterJobApp", function (e) {
  e.preventDefault();
  e.stopPropagation();
  searchFilterJobApp = $(this).val();
  pageNumberJobApp = 1;
  GetJobApplications();
});

$(document).on("change", "#pageSizeJobApp", function () {
  pageNumberJobApp = 1;
  pageSizeJobApp = $(this).val();
  GetJobApplications();
});

$(document).on("change", "#jobApplicationStatus", function () {
  statusFilterJobApp = Number($(this).val());
  GetJobApplications();
});

$(document).on("click", "#nextJobApp", function () {
  pageNumberJobApp = pageNumberJobApp + 1;
  GetJobApplications();
});

$(document).on("click", "#previousJobApp", function () {
  pageNumberJobApp = pageNumberJobApp - 1;
  GetJobApplications();
});

$(document).on("click", ".application-view", function () {
  const jobApplicationId = $(this).attr("data-applicationid");
  GetApplicationDetails(jobApplicationId);
});

$(document).on("click", ".sort-order-jobappstudent", function () {
  sortOrderJobApp = $(this).attr("data-sortorder");
  sortColumnJobApp = $(this).attr("data-sortcolumn");
  GetJobApplications();
});

$(document).on("click", "#clearFiltersJobApp", function () {
  searchFilterJobApp = "";
  statusFilterJobApp = "";
  $("#searchFilterJobApp").val("");
  $("#jobApplicationStatus").val("");
  GetJobApplications();
});

$(document).on("click", ".application-status-edit", async function(){
  const jobApplicationId = $(this).attr("data-applicationid");
  const data = await RetrieveApplicationDetails(jobApplicationId);
  $("#jobAppUpdateSubmit").attr("data-applicationid", jobApplicationId);
  const applyStatus = data.applyStatus;
  $("#jobApplicationStatusEdit").val(applyStatus);
})

$(document).on("click", "#jobAppUpdateSubmit", function(e){
  e.preventDefault();
  const jobApplicationId = $(this).attr("data-applicationid");
  const applyStatus = $("#jobApplicationStatusEdit").val();
  UpdateJobApplication(jobApplicationId, applyStatus)
})

function GetJobApplications() {
  $.ajax({
    url: "/JobApplication/ViewApplicationsPaginated",
    type: "GET",
    data: {
      CurrentPage: pageNumberJobApp,
      PageSize: pageSizeJobApp,
      SearchFilter: searchFilterJobApp,
      SortColumn: sortColumnJobApp,
      SortOrder: sortOrderJobApp,
      StatusFilter: statusFilterJobApp,
    },
    success: function (response) {
      $("#allApplicationsContainer").html(response);
    },
    error: function (xhr, status, error) {
      console.warn(status);
      console.warn(xhr);
      console.warn(error);
    },
  });
}

function GetApplicationDetails(jobApplicationId) {
  $.ajax({
    url: "/JobApplication/GetJobApplicationDetails",
    type: "GET",
    data: {
      applicationId: jobApplicationId,
    },
    success: function (response) {
      $("#applicationViewContainer").html(response);
    },
    error: function (xhr, status, error) {
      console.warn(status);
      console.warn(xhr);
      console.warn(error);
    },
  });
}

function GetApplicationDetailsJson(jobApplicationId) {
  return new Promise((resolve, reject) => {
    $.ajax({
      url: "/JobApplication/GetJobApplicationDetailsJson",
      type: "Get",
      data: {
        applicationId: jobApplicationId,
      },
      success: function (data) {
        resolve(data);
      },
      error: function (xhr, status, error) {
        reject(0);
      },
    });
  });
}

async function RetrieveApplicationDetails(jobApplicationId) {
  const jsonData = await GetApplicationDetailsJson(jobApplicationId);
  return jsonData.data;
}

function UpdateJobApplication(applicationId, applyStatus) {
  $.ajax({
    url: "/JobApplication/UpdateJobApplicationStatus",
    type: "POST",
    data: {
      applicationId,
      applyStatus
    },
    success: function (response) {
      if(response.success){
        toastr.success(response.message);
        $("#editApplicationStatusModal").modal("hide");
        GetJobApplications();
      }
      else{
        toastr.error(response.message);
        $("#editApplicationStatusModal").modal("hide");
        GetJobApplications();
      }
    },
    error: function (xhr, status, error) {
      console.warn(status);
      console.warn(xhr);
      console.warn(error);
    },
  });
}