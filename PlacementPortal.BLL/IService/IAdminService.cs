using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IService;

public interface IAdminService
{
    Task<PaginationDTO<TPOListDTO>> GetTPOsPaginated(PaginationRequestDTO paginationRequest);
}
