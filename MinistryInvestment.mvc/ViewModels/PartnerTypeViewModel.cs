
using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Mvc.Security;
using System.Collections.Generic;

namespace MinistryInvestment.Mvc.ViewModels
{
    public class PartnerTypeViewModel : BaseViewModel
    {
        private readonly IEnumerable<PartnerType> _partnerTypes;
        private readonly PartnerType _partnerType;
        private readonly bool _error;

        public PartnerTypeViewModel(IMinistryInvestmentConfig ministryInvestmentConfig, IEnumerable<PartnerType> partnerTypes, bool error, MenuPermissions menuPermissions)
            : base(ministryInvestmentConfig, menuPermissions: menuPermissions)
        {
            _partnerTypes = partnerTypes;
            _error = error;
        }
        public IEnumerable<PartnerType> PartnerTypes { get { return _partnerTypes; } }
        public PartnerType PartnerType { get { return _partnerType; } }
        public bool Error { get { return _error; } }
    }
}