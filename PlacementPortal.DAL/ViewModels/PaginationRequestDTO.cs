namespace PlacementPortal.DAL.ViewModels;

public class PaginationRequestDTO
{
    public string SearchFilter { get ; set ; } = "";

    public int CurrentPage { get ; set ; } = 1;

    public int PageSize { get ; set ; } = 5;

    public string SortColumn { get ; set ; } = "Id";

    public string SortOrder { get ; set ; } = "asc";
}
