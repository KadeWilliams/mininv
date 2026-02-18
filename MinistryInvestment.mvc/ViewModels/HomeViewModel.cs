using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Mvc.Security;
using System.Collections.Generic;

namespace MinistryInvestment.Mvc.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {

        private readonly IEnumerable<Request> _requests;
        private readonly IEnumerable<Category> _categories;
        private readonly IEnumerable<ProjectType> _projectTypes;
        private readonly IEnumerable<RequestStatus> _requestStatuses;
        private readonly OrganizationDetail _organization;
        private readonly IEnumerable<Organization> _organizations;
        private readonly Request _request;
        public HomeViewModel(
            IMinistryInvestmentConfig ministryInvestmentConfig,
            IEnumerable<Request> requests,
            IEnumerable<RequestStatus> requestStatuses,
            IEnumerable<ProjectType> projectTypes,
            IEnumerable<Organization> organizations,
            MenuPermissions menuPermissions
        )
            : base(ministryInvestmentConfig, requestStatuses: requestStatuses, projectTypes: projectTypes, menuPermissions: menuPermissions)
        {
            _requests = requests;
            _requestStatuses = requestStatuses;
            _projectTypes = projectTypes;
            _organizations = organizations;
        }
        public IEnumerable<Request> Requests { get { return _requests; } }
        public IEnumerable<Category> Categories { get { return _categories; } }
        public IEnumerable<RequestStatus> RequestStatuses { get { return _requestStatuses; } }
        public IEnumerable<ProjectType> ProjectTypes { get { return _projectTypes; } }
        public Request Request { get { return _request; } }
        public OrganizationDetail OrganizationDetails { get { return _organization; } }
        public IEnumerable<Organization> Organizations { get { return _organizations; } }
    }
}