namespace PlacementPortal.DAL.ViewModels;

public class PaginationDTO<T> where T : class
{
    public List<T>? Items { get; set; }

    public int StartIndex { get; set; }

    public int EndIndex { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }
}
