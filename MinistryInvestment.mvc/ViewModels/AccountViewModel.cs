using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Mvc.Security;

namespace MinistryInvestment.Mvc.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        public AccountViewModel(IMinistryInvestmentConfig ministryInvestmentConfig, MenuPermissions menuPermissions)
            : base(ministryInvestmentConfig, menuPermissions: menuPermissions)
        {

        }
    }
}