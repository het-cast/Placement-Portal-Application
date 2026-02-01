using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Strategies
{
    public class DashboardStrategyFactory
    {
        private readonly Dictionary<RoleEnum, IDashboardStrategy> _strategyMap;
        private readonly IGlobalService _globalService;

        public DashboardStrategyFactory(IEnumerable<IDashboardStrategy> strategies, IGlobalService globalService)
        {
            _strategyMap = strategies.ToDictionary(s => s.Role); 
            _globalService = globalService;
        }

        public IDashboardStrategy GetStrategy()
        {
            TokenDataDTO tokenData = _globalService.GetTokenData();

            if (!Enum.TryParse<RoleEnum>(tokenData.Role, ignoreCase: true, out var role))
            {
                throw new InvalidOperationException(CustomExceptionConst.InvalidRoleInToken);
            }

            if (!_strategyMap.TryGetValue(role, out var strategy))
            {
                throw new InvalidOperationException(CustomExceptionConst.GetMessageOfMissingStrategy(role.ToString()));
            }

            return strategy;
        }
    }
}
