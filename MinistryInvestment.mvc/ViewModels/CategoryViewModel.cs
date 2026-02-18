using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Mvc.Security;
using System.Collections.Generic;

namespace MinistryInvestment.Mvc.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        private readonly IEnumerable<Category> _categories;
        private readonly Category _category;
        private readonly bool _error;

        public CategoryViewModel(IMinistryInvestmentConfig ministryInvestmentConfig, IEnumerable<Category> categories, bool error, MenuPermissions menuPermissions)
            : base(ministryInvestmentConfig, menuPermissions: menuPermissions)
        {
            _categories = categories;
            _error = error;
        }
        public IEnumerable<Category> Categories { get { return _categories; } }
        public Category Category { get { return _category; } }
        public bool Error { get { return _error; } }
    }
}