using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Mvc.Security;
using System.Collections.Generic;

namespace MinistryInvestment.Mvc.ViewModels
{
    public class RegionViewModel : BaseViewModel
    {
        private readonly IEnumerable<Region> _regions;
        private readonly Region _region;
        private readonly bool _error;

        public RegionViewModel(IMinistryInvestmentConfig ministryInvestmentConfig, IEnumerable<Region> regions, bool error, MenuPermissions menuPermissions)
            : base(ministryInvestmentConfig, menuPermissions: menuPermissions)
        {
            _regions = regions;
            _error = error;
        }
        public IEnumerable<Region> Regions { get { return _regions; } }
        public Region Region { get { return _region; } }
        public bool Error { get { return _error; } }
    }
}