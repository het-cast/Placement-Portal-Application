const jobListingFormTemplate = `
<div class="row row-cols-5 mb-3 job-profile-row d-flex flex-wrap align-items-center justify-content-center">
                    
                    <div class="col-12 col-md mb-2 mb-md-0 job-listing-field">
                        <div class="form-floating mb-1">
                                <select class="form-select" name="jobDomain">
                                    <option value="">Select Job Domain / Industry</option>
                                    <option value="IT">Computer Science</option>
                                    <option value="ECE">Electronics and Comm.</option>
                                    <option value="MECH">Mechanical Engg.</option>
                                    <option value="ELECTRICAL">Electical Engg.</option>
                                    <option value="AUTOMOBILE">Automobile</option>
                                    <option value="PRINTING">Printing</option>
                                    <option value="CIVIL">Civil</option>
                                    <option value="TEXTILE">Textile</option>

                                </select>
                                <label for="selectSSCBoardStudent">Domain</label>
                                <span class="text-danger"></span>
                            </div>
                    </div>
                    <div class="col-12 col-md mb-2 mb-md-0 job-listing-field">
                        <div class="form-floating mb-1">
                            <input type="text" class="form-control job-profile" name="jobProfile"
                                placeholder="Job Profile : " autocomplete="off">
                            <ul class="list-group position-absolute suggestion-list d-none" style="z-index: 999; width:100%;"></ul>

                            <label for="jobProfile">Job Profile</label>
                            <span class="text-danger"></span>
                        </div>
                    </div>
                    <div class="col-12 col-md mb-2 mb-md-0 job-listing-field">
                        <div class="form-floating mb-1">
                            <input type="text" class="form-control" name="minSalary" placeholder=">Min. Salary : ">
                            <label for="minSalary">Min. Salary</label>
                            <span class="text-danger"></span>
                        </div>
                    </div>
                    <div class="col-12 col-md mb-2 mb-md-0 job-listing-field">
                        <div class="form-floating mb-1">
                            <input type="text" class="form-control" name="maxSalary" placeholder="Max. Salary : ">
                            <label for="maxSalary">Max. Salary</label>
                            <span class="text-danger"></span>
                        </div>
                    </div>
                    <div class="col-12 col-md mb-2 mb-md-0 job-listing-field">
                        <div class="form-floating mb-1">
                            <input type="text" class="form-control pe-none" name="salaryUnit"
                                placeholder="Salary Unit : " value="LPA">
                            <label for="salaryUnit">Salary Unit</label>
                            <span class="text-danger"></span>
                        </div>
                    </div>
                    <div class=" d-flex align-items-center" style="width: 50px;">
                        <button class="btn delete-job-profile" type="button">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </div>
                    </div>
`;
const jobListingRowsContainer = $("#companyProfileFormRows");
let CompanyProfileAddedId = 0;
let uniqueKeys = [];
let currentlyUpdatingCompanyId;
let currentlyUpdatedJobListingCompanyId;
let distinctJobProfiles;

$(document).ready(async function () {
  let today = new Date().toISOString().slice(0, 10);
  $("#applicationStartDate").attr("min", today);
  $("#applicationEndDate").attr("min", today);
  $("#applicationStartDate").on("change", function () {
    let fromDateValue = $("#applicationStartDate").val();
    $("#applicationEndDate").attr("min", fromDateValue);
  });
  // return;
  let updatePage = getUpdateJobListingsPresentFromQuery();
  if (updatePage) {
    const companyID = getCompanyIdFromQuery();
    RenderJobListingFormWithData(companyID);
  }

  distinctJobProfiles = await GetDistinctProfiles();
});


// Event Listeners and other document related things

$(document).on("click", "#SaveCompanyAndNavigateJob", function (e) {
  e.preventDefault();
  let companyProfileForm = $("#CompanyProfileForm");
  $.validator.unobtrusive.parse(companyProfileForm);
  let companyProfileFormData = new FormData(companyProfileForm[0]);

  if (companyProfileForm.valid()) {
    showSpinner();
    SaveCompanyProfileDetails(companyProfileFormData);
  }
});

$(document).on("click", "#updateCompanyProfileSubmit", function (e) {
  e.preventDefault();
  const companyId = $(this).attr("data-companyid");
  let companyProfileForm = $("#CompanyProfileForm");
  $.validator.unobtrusive.parse(companyProfileForm);
  let companyProfileFormData = new FormData(companyProfileForm[0]);
  companyProfileFormData.append("companyId", companyId);

  if (companyProfileForm.valid()) {
    showSpinner();
    UpdateCompanyProfileDetails(companyProfileFormData);
  }
});

$(document).on("click", "#addNewJobListing", function () {
  $("#companyProfileFormRows").append(jobListingFormTemplate);
});

$(document).on("click", ".delete-job-profile", function () {
  $(this).closest(".job-profile-row").remove();
});

$(document).on("click", ".delete-job-profile-update", function () {
  $(this).closest(".job-profile-row").remove();
});

$(document).on("click", "#saveJobProfiles", function (e) {
  e.preventDefault();

  uniqueKeys = [];
  let isValid = true;
  const jobListings = [];

  const companyId = getCompanyIdFromQuery();

  const applicationStart = $("#applicationStartDate").val();
  const applicationEnd = $("#applicationEndDate").val();

  if (applicationStart == "0001-01-01" || applicationEnd == "0001-01-01") {
    toastr.error("Please enter valid date(s)");
    return;
  }

  console.log($("#applicationStartDate").val() == "0001-01-01");
  console.log(`Application Start date is : ${$("#applicationStartDate").val()}`);
  const applicationStartDate = new Date($("#applicationStartDate").val());
  const applicationEndDate = new Date($("#applicationEndDate").val());
  const minCgpaRequired = parseFloat($("#minimumCGPAReq").val());
  const maxBacklogsAllowed = parseInt($("#maximumBacksAllowed").val());

  if (
    !applicationStartDate ||
    !applicationEndDate ||
    !minCgpaRequired
  ) {
    toastr.error("Please fill out all required company details.");
    return;
  }

  if (isNaN(minCgpaRequired) || isNaN(maxBacklogsAllowed)) {
    toastr.error("Min CGPA and Max Backlogs must be a number");
    return;
  }


  if (!applicationStartDate || applicationStartDate === "Mon Jan 01 0001 05:53:28 GMT+0553 (India Standard Time)") {
    toastr.error("Application end date cannot be empty or invalid.");
    return;
  }

  if (!applicationStartDate || !applicationEndDate) {
    toastr.error("Application start date or end date cannot be empty");
    return;
  }

  if (applicationStartDate >= applicationEndDate) {
    toastr.error("Application start date must be before end date.");
    return;
  }

  if (minCgpaRequired < 0 || minCgpaRequired > 10) {
    toastr.error("Minimum CGPA should be between 0 and 10.");
    return;
  }

  if (maxBacklogsAllowed < 0 && maxBacklogsAllowed > 5) {
    toastr.error("Max Backlogs should be between 0 and 5");
    return
  }

  const rows = $(".job-profile-row");
  for (let i = 0; i < rows.length; i++) {
    const $row = $(rows[i]);
    const jobProfile = $row.find("input[name='jobProfile']").val().trim();
    const jobDomain = $row.find("select[name='jobDomain']").val().trim();
    const minSalary = parseFloat($row.find("input[name='minSalary']").val());
    const maxSalary = parseFloat($row.find("input[name='maxSalary']").val());
    const salaryUnit = $row.find("input[name='salaryUnit']").val().trim();
    const positionId = $row.data("position-id") || 0;

    $row.find("input, select").removeClass("is-invalid");

    // Empty or invalid value check
    if (
      !jobProfile ||
      !jobDomain ||
      isNaN(minSalary) ||
      isNaN(maxSalary) ||
      !salaryUnit
    ) {
      toastr.error("Please fill out all fields in each job listing properly.");
      $row.find("input, select").each(function () {
        if (!$(this).val().trim()) {
          $(this).addClass("is-invalid");
        }
      });
      isValid = false;
      break;
    }

    if (jobProfile.length < 2 || jobProfile.length > 90) {
      toastr.error("Job Profile should have length > 2 and < 90");
      isValid = false;
      break;
    }

    console.log(`Check this 0.5 thing`)
    console.log(0.5 < 0.211212);

    if (minSalary < 0.5 || maxSalary < 0.5) {
      toastr.error("Salaries must be non-negative numbers > 0.5.");
      $row
        .find("input[name='minSalary'], input[name='maxSalary']")
        .addClass("is-invalid");
      isValid = false;
      break;
    }

    if (minSalary > 99 || maxSalary > 99) {
      toastr.error("Salary should be below 100");
      $row
        .find("input[name='minSalary'], input[name='maxSalary']")
        .addClass("is-invalid");
      isValid = false;
      break;
    }

    if (minSalary > maxSalary) {
      toastr.error("Minimum salary cannot be greater than maximum salary.");
      $row
        .find("input[name='minSalary'], input[name='maxSalary']")
        .addClass("is-invalid");
      isValid = false;
      break;
    }

    const uniqueKey = `${jobProfile.toLowerCase()}-${jobDomain.toLowerCase()}`;
    if (uniqueKeys.includes(uniqueKey)) {
      toastr.error(`Duplicate job profile found: ${jobProfile} (${jobDomain})`);
      $row.find("input[name='jobProfile']").addClass("is-invalid");
      isValid = false;
      break;
    }

    uniqueKeys.push(uniqueKey);
    $row.attr("data-uniquekey", uniqueKey);

    jobListings.push({
      UniqueGeneratedKey: uniqueKey,
      JobDomain: jobDomain,
      JobProfie: jobProfile,
      MinimumSalary: minSalary,
      MaximumSalary: maxSalary,
    });
  }

  if (!isValid) return;

  if (jobListings.length === 0) {
    toastr.error("Please enter at least one valid job listing.");
    return;
  }

  const jobListingCommonData = {
    CompanyId: companyId,
    ApplicationStartDate: $("#applicationStartDate").val(),
    ApplicationEndDate: $("#applicationEndDate").val(),
    MinCgpaRequired: minCgpaRequired,
    MaxBacklogsAllowed: maxBacklogsAllowed,
    SalaryUnit: "LPA",
  };

  const JobListingData = {
    JobListingCommon: jobListingCommonData,
    JobListings: jobListings,
  };

  const JobProfileData = {
    companyId: parseInt(companyId),
    JobListings: jobListings,
  };

  showSpinner();
  AddJobProfiles(JobListingData);
});

// Typing inside input
$(document).on("input", ".job-profile", function () {
  const inputVal = $(this).val().toLowerCase();
  const $suggestionList = $(this).siblings('.suggestion-list'); // FIXED: Use siblings
  let suggestionsHtml = '';

  const matchedProfiles = distinctJobProfiles.filter(profile =>
    profile.toLowerCase().includes(inputVal)
  );

  if (inputVal.length > 0 && matchedProfiles.length > 0) {
    matchedProfiles.forEach(profile => {
      suggestionsHtml += `<li class="list-group-item suggestion-item" style="cursor:pointer;">${profile}</li>`;
    });
    $suggestionList.html(suggestionsHtml).removeClass('d-none').show();
  } else {
    $suggestionList.hide();
  }
});

// Clicking on suggestion
$(document).on('click', '.suggestion-item', function () {
  const selectedText = $(this).text();
  const $input = $(this).closest('.form-floating').find('.job-profile'); // FIXED

  $input.val(selectedText);
  $(this).parent('.suggestion-list').hide(); // Hide current suggestion list
});

// Clicking outside input or suggestions
$(document).on('click', function (e) {
  if (!$(e.target).closest('.form-floating').length) {
    $('.suggestion-list').hide();
  }
});


// Functions Definitions and all

// checks If the page is for update job listings
function getUpdateJobListingsPresentFromQuery() {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get("updateListings");
}

// Extracts hashed / protected Company Id from the query parameter
function getCompanyIdFromQuery() {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get("companyId");
}

// Saves Company Profile Details and navigates to job listings page
function SaveCompanyProfileDetails(companyProfileFormData) {
  $.ajax({
    url: "/Company/AddCompanyProfile",
    type: "POST",
    data: companyProfileFormData,
    processData: false,
    contentType: false,
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        CompanyProfileAddedId = response.data;
        setTimeout(() => {
          hideSpinner();
          window.location.href = `/Company/JobListings?companyId=${CompanyProfileAddedId}`;
        }, 1500);

      } else {
        hideSpinner();
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      alert("An error occurred: " + error);
    },
  });
}

// Updates Company Profile
function UpdateCompanyProfileDetails(companyProfileFormData) {
  $.ajax({
    url: "/Company/UpdateCompanyProfile",
    type: "POST",
    data: companyProfileFormData,
    processData: false,
    contentType: false,
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        setTimeout(() => {
          hideSpinner();
          window.location.href = "/Company/Company";
        }, 1500);
      } else {
        hideSpinner();
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      alert("An error occurred: " + error);
    },
  });
}

// Adds / Updates Job Listings
function AddJobProfiles(JobProfileData) {
  $.ajax({
    url: "/Company/AddJobListingsEnhanced",
    method: "POST",
    data: JobProfileData,
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        if (response.data != null) {
          setTimeout(() => {
            // hideSpinner();
            toastr.error(response.data + "Listings are associated with applications that are currently in In Progress state can't be deleted");
          }, 500);
          setTimeout(() => {
            hideSpinner();
            window.location.href = "/Company/Company";
          }, 4000);
        }
        if (response.data == null) {
          setTimeout(() => {
            hideSpinner();
            window.location.href = "/Company/Company";
          }, 1500);
        }
      } else {
        hideSpinner();
        toastr.error(response.message || "Failed to save listings.");
      }
    },
    error: function () {
      toastr.error("An error occurred while saving job listings.");
    },
  });
}

function GetDistinctJobProfilesPromise() {
  return new Promise((resolve, reject) => {
    $.ajax({
      url: "/Company/GetDistinctJobProfiles",
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

async function GetDistinctProfiles() {
  const jsonData = await GetDistinctJobProfilesPromise();
  console.log(jsonData.data);
  return jsonData.data;
}

// Retrieves Job Listings data by promise and all by company Id
function GetJobListingsByCompany(companyId) {
  return new Promise((resolve, reject) => {
    $.ajax({
      url: "/Company/GetJobListings",
      type: "Post",
      data: { companyId: companyId },
      success: function (data) {
        resolve(data);
      },
      error: function (xhr, status, error) {
        reject(0);
      },
    });
  });
}

// Extracts jsonData from the promise of GetJobListingsByCompany function
async function GetJobListingByCompanyData(companyId) {
  const jsonData = await GetJobListingsByCompany(companyId);
  return companyId == 0 ? null : jsonData.data;
}

// Retrieves data and calls the Update Page with retrieved job listings data
async function RenderJobListingFormWithData(companyIdToBeUpdated) {
  const data = await GetJobListingByCompanyData(companyIdToBeUpdated);
  await UpdateListingsFormWithData(data);
}

// It Updates Job Listings Form with the data
async function UpdateListingsFormWithData(data) {
  const jobListings = data;
  let areJobListingsEmpty = $.isEmptyObject(jobListings);
  if (!areJobListingsEmpty) {
    $("#applicationEndDate").val(data[0].applicationEndDate);
    $("#applicationStartDate").val(data[0].applicationStartDate);
    $("#applicationStartDate").addClass("pe-none").attr('tabindex', '-1');

    $("#minimumCGPAReq").val(data[0].minCgpaRequired);
    $("#maximumBacksAllowed").val(data[0].maxBacklogsAllowed);

    jobListings.forEach((job, index) => {
      const rowHtml = `
    <div class="row row-cols-5 mb-3 job-profile-row d-flex flex-wrap align-items-center justify-content-center" data-uniquekey="${job.uniqueGeneratedKey
        }"> 
      <div class=" col-12 col-md mb-2 mb-md-0 job-listing-field">
        <div class="form-floating mb-1">
          <select class="form-select" name="jobDomain">
            <option value="">Select Job Domain / Industry</option>
            <option value="IT" ${job.jobDomain === "IT" ? "selected" : ""
        }>Computer Science</option>
            <option value="ECE" ${job.jobDomain === "ECE" ? "selected" : ""
        }>Electronics and Comm.</option>
            <option value="MECH" ${job.jobDomain === "MECH" ? "selected" : ""
        }>Mechanical Engg.</option>
            <option value="ELECTRICAL" ${job.jobDomain === "ELECTRICAL" ? "selected" : ""
        }>Electrical Engg.</option>
            <option value="AUTOMOBILE" ${job.jobDomain === "AUTOMOBILE" ? "selected" : ""
        }>Automobile</option>
            <option value="PRINTING" ${job.jobDomain === "PRINTING" ? "selected" : ""
        }>Printing</option>
            <option value="CIVIL" ${job.jobDomain === "CIVIL" ? "selected" : ""
        }>Civil</option>
            <option value="TEXTILE" ${job.jobDomain === "TEXTILE" ? "selected" : ""
        }>Textile</option>
          </select>
          <label>Domain</label>
        </div>
      </div>

      <div class="col-12 col-md mb-2 mb-md-0 job-listing-field">
        <div class="form-floating mb-1">
          <input type="text" class="form-control job-profile" name="jobProfile" value="${job.jobProfie
        }" autocomplete="off"/>
        <ul class="list-group position-absolute suggestion-list d-none" style="z-index: 999; width:100%;"></ul>
          <label>Job Profile</label>
        </div>
      </div>

      <div class="col-12 col-md mb-2 mb-md-0 job-listing-field">
        <div class="form-floating mb-1">
          <input type="text" class="form-control" name="minSalary" value="${job.minimumSalary
        }" />
          <label>Min. Salary</label>
        </div>
      </div>

      <div class="col-12 col-md mb-2 mb-md-0 job-listing-field">
        <div class="form-floating mb-1">
          <input type="text" class="form-control" name="maxSalary" value="${job.maximumSalary
        }" />
          <label>Max. Salary</label>
        </div>
      </div>

      <div class="col-12 col-md mb-2 mb-md-0 job-listing-field">
        <div class="form-floating mb-1">
          <input type="text" class="form-control pe-none" name="salaryUnit" value="${job.salaryUnit
        }" />
          <label>Salary Unit</label>
        </div>
      </div>

      <div class="d-flex align-items-center" style="width: 50px;">
        <button class="btn delete-job-profile-update" type="button">
          <i class="fas fa-trash-alt"></i>
        </button>
      </div>
    </div>
    `;
      $("#companyProfileFormRows").append(rowHtml);
    });
  }
  $("#saveJobProfiles").attr("data-submittype", "Update");
}

// Shows Spinner
function showSpinner() {
  $("#loader").show();
}

// Hides Spinner
function hideSpinner() {
  $("#loader").hide();
}