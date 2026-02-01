// Companies List Pagination
let companyIdForJobsFetching;
let searchFilter = $("#searchFilterProfile").val();
let pageSize = 5;
let pageNumber = 1;
let jobFiltersObj = {
  JobDomain: "",
  MaxPackage: -1,
  MinPackage: -1,
  MaxBacklogsAllowed: -1,
  MinCgpaRequired: -1,
  StudentCGPA: -1,
  StudentBacklog: -1,
};

$(document).ready(function () {
  GetCompaniesList();
});

function GetStudentDetails() {
  return new Promise((resolve, reject) => {
    $.ajax({
      url: "/Student/GetCompleteStudentDetailsJson",
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

async function RetrieveStudentDetails() {
  const jsonData = await GetStudentDetails();
  return jsonData.data;
}

$(document).on("click", "#viewEligibleCompanies", async function (e) {
  e.preventDefault();
  clearJobFilters();
  ResetJobListingVariables();
  searchFilter = "";
  $("#searchFilterProfile").val("");
  const studentData = await RetrieveStudentDetails();
  jobFiltersObj = {
    StudentBacklog: studentData.studentAcademicDetails.pendingBacklog,
    StudentCGPA: studentData.studentAcademicDetails.cgpa,
  };
  GetCompaniesList();
});

$(document).on("keyup", "#searchFilterProfile", function (e) {
  e.preventDefault();
  e.stopPropagation();
  searchFilter = $(this).val();
  pageNumber = 1;
  GetCompaniesList();
});

$(document).on("change", "#pageSize", function () {
  pageNumber = 1;
  pageSize = $(this).val();
  GetCompaniesList();
});

$(document).on("click", "#next", function () {
  pageNumber = pageNumber + 1;
  GetCompaniesList();
});

$(document).on("click", "#previous", function () {
  pageNumber = pageNumber - 1;
  GetCompaniesList();
});

let searchFilterJobListing = $("#searchFunctionalityJobListing").val();
let pageSizeJobListing = 5;
let pageNumberJobListing = 1;

function ResetJobListingVariables() {
  pageNumberJobListing = 1;
  pageSizeJobListing = 5;
  searchFilterJobListing = "";
  searchFilter = "";
}

$(document).on("keyup", "#searchFunctionalityJobListing", function () {
  searchFilter = $(this).val();
  pageNumberJobListing = 1;
  RenderJobsByCompany();
});

$(document).on("change", "#pageSizeJobListing", function () {
  pageNumberJobListing = 1;
  pageSizeJobListing = $(this).val();
  RenderJobsByCompany();
});

$(document).on("click", "#nextJobListing", function () {
  pageNumberJobListing = pageNumberJobListing + 1;
  RenderJobsByCompany();
});

$(document).on("click", "#previousJobListing", function () {
  pageNumberJobListing = pageNumberJobListing - 1;
  RenderJobsByCompany();
});

$(document).on("click", ".job-listing-apply-btn", function () {
  const companyId = $(this).attr("data-companyid");
  const companyJobId = $(this).attr("data-joblistingid");
  showSpinner();
  ApplyForJob(companyJobId);
});

function ApplyForJob(jobListingId) {
  $.ajax({
    url: "/JobApplication/ApplyForJob",
    type: "POST",
    data: {
      jobListingId: jobListingId,
    },
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        setTimeout(() => {
          RenderJobsByCompany();
          hideSpinner();
        }, 1300);
      }
      else {
        toastr.error(response.message);
        hideSpinner();
      }
    },
    error: function (xhr, status, error) {
      hideSpinner();
      toastr.error("Job Not Applied Successfully");
    },
  });
}

function GetCompaniesList() {
  $.ajax({
    url: "/Company/GetCompaniesList",
    type: "Post",
    data: {
      pageSize: pageSize,
      currentPage: pageNumber,
      SearchFilter: searchFilter,
      searchKeyword: searchFilter,
      jobSearchFilters: jobFiltersObj,
    },
    success: function (response) {
      if (response.success == false) {
        toastr.error(response.message);
      }
      $("#companiesFilteredListContainer").html(response);
    },
    error: function (xhr, status, error) {
      alert("An error occurred: " + error);
      console.warn(error);
    },
  });
}

function ResetStorageandRenderInitialView() {
  localStorage.removeItem("PartialViewFunctionName");
  localStorage.removeItem("PartialViewFunctionParams");
  GetCompaniesList();
}

$(document).on("click", "#companyListingsNavigation", function () { });

$(document).on("click", "#companyJobsListing", function () {
  const companyId = $(this).attr("data-companyid");
  companyIdForJobsFetching = companyId;
  ResetJobListingVariables();
  RenderJobsByCompany();
});

$(document).on("click", "#clearJobFilters", function () {
  jobFiltersObj = {
    JobDomain: "",
    MaxPackage: -1,
    MinPackage: -1,
    MaxBacklogsAllowed: -1,
    MinCgpaRequired: -1,
    StudentCGPA: -1,
    StudentBacklog: -1,
  };
  searchFilter = "";
  $("#searchFilterProfile").val("");
  clearJobFilters();
  GetCompaniesList();
});

$(document).on("click", "#submitJobFilters", function (e) {
  e.preventDefault();

  const minPackageFilter = parseFloat($('input[name="MinPackage"]').val());
  const maxPackageFilter = parseFloat($('input[name="MaxPackage"]').val());
  const searchFilter = $('input[name="SearchTermJob"]').val();
  const jobDomainFilter = $('select[name="JobDomainFilter"]').val();
  const minCGPAFilter = parseFloat($('input[name="MinCGPAFilter"]').val());
  const maxBacklogsFilter = parseInt(
    $('input[name="MaxBacklogsFilter"]').val()
  );

  jobFiltersObj = {
    JobDomain: jobDomainFilter,
    MaxBacklogsAllowed: maxBacklogsFilter,
    MaxPackage: maxPackageFilter,
    MinCgpaRequired: minCGPAFilter,
    MinPackage: minPackageFilter,
    StudentBacklog: -1,
    StudentCGPA: -1,
  };

  if (
    minPackageFilter &&
    maxPackageFilter &&
    minPackageFilter > maxPackageFilter
  ) {
    toastr.error("Min package cannot be greater than Max Package");
    return;
  }

  GetCompaniesList();
});

$(document).on('click', ".btn-applied", function (e) {
  e.preventDefault();
  // toastr.warning("Already applied")
  toastr.open({ type: 'info', message: 'Already Applied to this Job Posting' });
})

function clearJobFilters() {
  $("#filterForm")[0].reset();
  $("#resultInfo").hide();
}

function RenderJobsByCompany() {
  debugger;
  $.ajax({
    url: "/Company/GetJobListingsByCompany",
    type: "Post",
    data: {
      companyId: companyIdForJobsFetching,
      pageSize: pageSizeJobListing,
      currentPage: pageNumberJobListing,
      searchKeyword: searchFilterJobListing,
      jobSearchFilters: jobFiltersObj,
    },
    success: function (response) {
      if (response.success == false) {
        toastr.error(response.message);
      }
      $("#jobModalBody").html(response);
      HasAppliedToggle();
    },
    error: function (xhr, status, error) {
      console.warn(status);
      console.warn(xhr);
      console.warn(error);
    },
  });
}

function HasAppliedToggle() {
  const companyid = parseInt($(".job-listing-apply-btn").first().data("companyid"));
  CheckListingAssociation(companyid);
  // jobListingIds.push(parseInt($(this).data("joblistingid")));

  // console.log(studentDetails);
}

function CheckListingAssociation(companyid) {
  $.ajax({
    url: '/JobApplication/GetJobApplicationsByCompanyAndStudent',
    type: 'GET',
    data: {
      companyid: companyid
    },
    success: function (response) {
      $(".job-listing-apply-btn").each(function () {
        const jobId = parseInt($(this).data("joblistingid"));
        const appliedJobIds = response.data;
        console.log(appliedJobIds)

        if (appliedJobIds.includes(jobId)) {
          $(this).text("Applied")
            .addClass("btn-applied")
            .removeClass("btn-apply job-listing-apply-btn");
          // .prop("disabled", true);
        }
      });
    },
    error: function () {
      console.error("Error checking applied job listings.");
    }
  });
}

$(document).on("click", ".delete-company-profile", function () {
  const companyId = $(this).attr("data-companyid");
  $(".company-delete-btn").attr("data-companyid", companyId);
})

$(document).on("click", ".company-delete-btn", function () {
  const companyId = $(this).attr("data-companyid");
  showSpinner();
  DeleteComapnyProfile(companyId)
})

$(document).on("click", "#updateCompanyJobListings", async function (e) {
  e.preventDefault();
  let companyEncryptedId = "";
  const companyIdToBeUpdated = parseInt($(this).attr("data-companyid"));
  CompanyProfileAddedId = companyIdToBeUpdated;
  $.ajax({
    url: "/Company/GetCompanyId",
    type: "GET",
    data: { companyId: companyIdToBeUpdated },
    success: function (response) {
      if (response.success) {
        companyEncryptedId = response.data;
        window.location.href = `/Company/JobListings?updateListings=${true}&companyId=${companyEncryptedId}`;
      } else {
      }
    },
    error: function (xhr, status, error) {
      alert("An error occurred: " + error);
    },
  });
});

function DeleteComapnyProfile(companyId) {
  $.ajax({
    url: "/Company/DeleteCompanyProfile",
    method: "POST",
    data: { companyId: companyId },
    success: function (response) {
      $("#deleteCompanyModal").modal('hide');
      if (response.success) {
        toastr.success(response.message)
        GetCompaniesList();
      } else {
        toastr.error(response.message);
      }
      hideSpinner();
    },
    error: function () {
      toastr.error("Company Profile not deleted successfully");
    },
  });
}

function showSpinner() {
  $("#loader").show();
}

function hideSpinner() {
  $("#loader").hide();
}