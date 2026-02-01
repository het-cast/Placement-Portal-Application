$(document).ready(function () {
    GetTPOsPaginated();
  });
  let searchFilterTPO = $("#searchFilterTPO").val();
  let pageSizeTPO = 5;
  let pageNumberTPO = 1;
  let sortColumnTPO;
  let sortOrderTPO;
  
  $(document).on("keyup", "#searchFilterTPOList", function (e) {
    e.preventDefault();
    e.stopPropagation();
    searchFilterTPO = $(this).val();
    pageNumberTPO = 1;
    GetTPOsPaginated();
  });
  
  $(document).on("change", "#pageSizeTPO", function () {
    pageNumberTPO = 1;
    pageSizeTPO = $(this).val();
    GetTPOsPaginated();
  });
  
  $(document).on("click", "#nextTPO", function () {
    pageNumberTPO = pageNumberTPO + 1;
    GetTPOsPaginated(); 
  });
  
  $(document).on("click", "#previousTPO", function () {
    pageNumberTPO = pageNumberTPO - 1;
    GetTPOsPaginated();
  });
  
  $(document).on("click", ".sort-order-resume", function () {
    sortOrderTPO = $(this).attr("data-sortorder");
    sortColumnTPO = $(this).attr("data-sortcolumn");
    GetTPOsPaginated();
  })

  function GetTPOsPaginated() {
    $.ajax({
      url: "/Admin/GetTPOsListPaginated",
      type: "Post",
      data: {
        CurrentPage: pageNumberTPO,
        PageSize: pageSizeTPO,
        SearchFilter: searchFilterTPO,
        SortOrder: sortOrderTPO,
        SortColumn: sortColumnTPO
      },
      success: function (response) {
        if (response.success == false) {
          toastr.error(response.message);
        }
        $("#paginatedTPOList").html(response);
      },
      error: function (xhr, status, error) {
        console.warn(status);
        console.warn(xhr);
        console.warn(error);
      },
    });
  }