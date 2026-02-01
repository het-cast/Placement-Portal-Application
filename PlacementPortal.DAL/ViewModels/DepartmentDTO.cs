namespace PlacementPortal.DAL.ViewModels;

public class DepartmentDTO
{
 
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsDeleted { get; set; }
}
