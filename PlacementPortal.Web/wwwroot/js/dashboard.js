$(document).ready(function(){
    GetJobApplicationsByStudent(dateSelectFilter);
})

let dateSelectFilter = $("#dateFilterDashboard").val();

$(document).on("change", "#dateFilterDashboard", function(){
    dateSelectFilter = $(this).val();
    GetJobApplicationsByStudent(dateSelectFilter);
})


function GetJobApplicationsByStudent(dateSelectFilter) {
    $.ajax({
      url: "/Dashboard/GetDashboardDetails",
      type: "GET",
      data: {
        dateSelect : dateSelectFilter
      },
      success: function (response) {
        if(response.success == false){
          toastr.error(response.message);
        }
        $("#dashboardDetails").html(response);
      },
      error: function (xhr, status, error) {
        console.warn(status);
        console.warn(xhr);
        console.warn(error);
      },
    });
  }
  