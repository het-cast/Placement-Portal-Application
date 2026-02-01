using System.Linq.Expressions;
using LinqKit;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Service;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationDTO<TPOListDTO>> GetTPOsPaginated(PaginationRequestDTO paginationRequest)
    {
        Expression<Func<UserAccount, bool>> expression;

        if (string.IsNullOrWhiteSpace(paginationRequest.SearchFilter))
        {
            expression = u => u.RoleId == (int)RoleEnum.TPO;
        }
        else
        {
            string lowerSearch = paginationRequest.SearchFilter.ToLower().Trim();
            expression = u =>
                u.RoleId == (int)RoleEnum.TPO && u.Email.ToLower().Trim().Contains(lowerSearch);
        }

        bool isAscending = paginationRequest.SortOrder.Equals(FilterConst.Ascending);

        Expression<Func<UserAccount, object>> orderBy = r => r.Id;

        if (!string.IsNullOrEmpty(paginationRequest.SortColumn))
        {
            orderBy = paginationRequest.SortColumn switch
            {
                FilterConst.IdSort => u => u.Id,
                FilterConst.EmailSort => u => u.Email,
                FilterConst.CreatedAtSort => u => u.CreatedAt,
                _ => u => u.Id
            };
        }
        Expression<Func<UserAccount, int>> orderThenBy = r => r.Id;

        (int totalCount, List<TPOListDTO> tpoListings) = await _unitOfWork.AuthenticationRepository.GetListPaginated(
            expression,
            x => new TPOListDTO
            {
                TPOId = x.Id,
                TPOEmail = x.Email,
                CreatedAt = x.CreatedAt
            },
            orderBy,
            u => u.Id,
            isAscending,
            paginationRequest.CurrentPage,
            paginationRequest.PageSize
        );

        int startIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + 1;

        int endIndex = (paginationRequest.CurrentPage - 1) * paginationRequest.PageSize + tpoListings.Count;

        return new PaginationDTO<TPOListDTO>
        {
            StartIndex = tpoListings.Count > 0 ? startIndex : 0,
            EndIndex = tpoListings.Count > 0 ? endIndex : 0,
            CurrentPage = paginationRequest.CurrentPage,
            TotalPages = (int)Math.Ceiling((double)totalCount / paginationRequest.PageSize),
            Items = tpoListings,
            TotalRecords = totalCount
        };
    } 

}
