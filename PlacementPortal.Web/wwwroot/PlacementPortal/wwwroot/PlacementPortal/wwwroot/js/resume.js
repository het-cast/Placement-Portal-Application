$(document).ready(function () {
  GetResumesPaginated();
});
let searchFilterResumeList = $("#searchFilterResumeList").val();
let pageSizeResumeList = 5;
let pageNumberResumeList = 1;
let sortColumnResumeList;
let sortOrderResumeList;

$(document).on("keyup", "#searchFilterResumeList", function (e) {
  e.preventDefault();
  e.stopPropagation();
  searchFilterResumeList = $(this).val();
  pageNumberResumeList = 1;
  GetResumesPaginated();
});

$(document).on("change", "#pageSizeResumeList", function () {
  pageNumberResumeList = 1;
  pageSizeResumeList = $(this).val();
  GetResumesPaginated();
});

$(document).on("click", "#nextResumeList", function () {
  pageNumberResumeList = pageNumberResumeList + 1;
  GetResumesPaginated(); 
});

$(document).on("click", "#previousResumeList", function () {
  pageNumberResumeList = pageNumberResumeList - 1;
  GetResumesPaginated();
});

$(document).on("click", ".sort-order-resume", function () {
  sortOrderResumeList = $(this).attr("data-sortorder");
  sortColumnResumeList = $(this).attr("data-sortcolumn");
  GetResumesPaginated();
})

$(document).on("click", ".resume-comment-modal-btn", function () {
  const resumeId = $(this).attr("data-resumeid");
  console.log(`Resume Id is ${resumeId}`);
  $("#resumeCommentSubmit").attr("data-resumeid", resumeId);
  RenderResumeCommentPartial(resumeId);
})

$(document).on("click", "#resumeCommentSubmit", function () {
  const resumeId = $(this).attr("data-resumeid");

  let resumeCommentForm = $("#resumeCommentForm");
  $.validator.unobtrusive.parse(resumeCommentForm);
  let resumeCommentFormData = new FormData(resumeCommentForm[0]);

  for(var pari of resumeCommentFormData){
    console.log(pari)
  }
  // return

  if (resumeCommentForm.valid()) {
    SaveResumeComment(resumeCommentFormData);
  }
})

function SaveResumeComment(formData) {
  $.ajax({
    url: "/Resume/SaveResumeComment",
    type: "POST",
    data: formData,
    processData: false,
    contentType: false,
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        $("#resumeCommentAddModal").modal("hide");
      }
      else {
        toastr.error(response.message);
        $("#resumeCommentAddModal").modal("hide");
      }
    },
    error: function (xhr, status, error) {
      console.warn(status);
      console.warn(xhr);
      console.warn(error);
    },
  });
}

function RenderResumeCommentPartial(resumeId) {
  $.ajax({
    url: "/Resume/RenderResumeCommentPartial",
    type: "Get",
    data: {
      resumeId: resumeId
    },
    success: function (response) {
      if (response.success == false) {
        toastr.error(response.message);
      }
      $("#resumeCommentContainer").html(response);
    },
    error: function (xhr, status, error) {
      console.warn(status);
      console.warn(xhr);
      console.warn(error);
    },
  });
}

function GetResumesPaginated() {
  $.ajax({
    url: "/Resume/GetResumesPaginated",
    type: "Post",
    data: {
      CurrentPage: pageNumberResumeList,
      PageSize: pageSizeResumeList,
      SearchFilter: searchFilterResumeList,
      SortOrder: sortOrderResumeList,
      SortColumn: sortColumnResumeList
    },
    success: function (response) {
      if (response.success == false) {
        toastr.error(response.message);
      }
      $("#paginatedResumeList").html(response);
    },
    error: function (xhr, status, error) {
      console.warn(status);
      console.warn(xhr);
      console.warn(error);
    },
  });
}
