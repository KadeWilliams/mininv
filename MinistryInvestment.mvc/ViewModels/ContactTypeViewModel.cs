using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Mvc.Security;
using System.Collections.Generic;

namespace MinistryInvestment.Mvc.ViewModels
{
    public class ContactTypeViewModel : BaseViewModel
    {
        private readonly IEnumerable<ContactType> _contactTypes;
        private readonly ContactType _contactType;
        private readonly bool _error;

        public ContactTypeViewModel(IMinistryInvestmentConfig ministryInvestmentConfig, IEnumerable<ContactType> contactTypes, bool error, MenuPermissions menuPermissions)
            : base(ministryInvestmentConfig, menuPermissions: menuPermissions)
        {
            _contactTypes = contactTypes;
            _error = error;
        }
        public IEnumerable<ContactType> ContactTypes { get { return _contactTypes; } }
        public ContactType ContactType { get { return _contactType; } }
        public bool Error { get { return _error; } }
    }
}