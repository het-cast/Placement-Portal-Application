namespace PlacementPortal.DAL.ViewModels;

public class GenericReturnDTO
{
    public bool Success { get ; set ; } = false;

    public string Message { get ; set ; } = "";

    public Object? Data { get ; set ;}

    public int Id { get ; set ;}
}
