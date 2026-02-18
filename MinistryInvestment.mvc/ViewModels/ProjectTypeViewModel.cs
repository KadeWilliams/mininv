using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Mvc.Security;
using System.Collections.Generic;

namespace MinistryInvestment.Mvc.ViewModels
{
    public class ProjectTypeViewModel : BaseViewModel
    {
        private readonly IEnumerable<ProjectType> _projectTypes;
        private readonly ProjectType _projectType;
        private readonly bool _error;

        public ProjectTypeViewModel(IMinistryInvestmentConfig ministryInvestmentConfig, IEnumerable<ProjectType> projectTypes, bool error, MenuPermissions menuPermissions)
            : base(ministryInvestmentConfig, menuPermissions: menuPermissions)
        {
            _projectTypes = projectTypes;
            _error = error;
        }
        public IEnumerable<ProjectType> ProjectTypes { get { return _projectTypes; } }
        public ProjectType ProjectType { get { return _projectType; } }
        public bool Error { get { return _error; } }
    }
}