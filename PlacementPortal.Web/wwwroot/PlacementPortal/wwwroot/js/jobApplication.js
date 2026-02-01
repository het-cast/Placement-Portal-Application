$(document).ready(function () {
  GetJobApplicationsByStudent();
});
let searchFilterJobApplication = $("#searchFilterJobApplication").val();
let pageSizeJobApplication = 5;
let pageNumberJobApplication = 1;
let sortOrderJobApplication;
let sortColumnJobApplication;


$(document).on("keyup", "#searchFilterJobApplication", function (e) {
  e.preventDefault();
  e.stopPropagation();
  searchFilterJobApplication = $(this).val();
  pageNumberJobApplication = 1;
  GetJobApplicationsByStudent();
});

$(document).on("change", "#pageSizeJobApplication", function () {
  pageNumberJobApplication = 1;
  pageSizeJobApplication = $(this).val();
  GetJobApplicationsByStudent();
});

$(document).on("click", "#nextJobApplication", function () {
  pageNumberJobApplication = pageNumberJobApplication + 1;
  GetJobApplicationsByStudent();
});

$(document).on("click", "#previousJobApplication", function () {
  pageNumberJobApplication = pageNumberJobApplication - 1;
  GetJobApplicationsByStudent();
});

$(document).on("click", ".sort-order-jobappstudent", function(){
  sortOrderJobApplication = $(this).attr("data-sortorder");
  sortColumnJobApplication = $(this).attr("data-sortcolumn");
  GetJobApplicationsByStudent();
})

function GetJobApplicationsByStudent() {
  $.ajax({
    url: "/JobApplication/ViewApplicationByStudentPaginated",
    type: "GET",
    data: {
      CurrentPage: pageNumberJobApplication,
      PageSize: pageSizeJobApplication,
      SearchFilter: searchFilterJobApplication,
      SortColumn : sortColumnJobApplication,
      SortOrder : sortOrderJobApplication
    },
    success: function (response) {
      $("#myApplicationsPaginatedData").html(response);
    },
    error: function (xhr, status, error) {
      console.warn(status);
      console.warn(xhr);
      console.warn(error);
    },
  });
}
