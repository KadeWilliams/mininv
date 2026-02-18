using Microsoft.AspNetCore.Mvc.Rendering;
using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Mvc.Security;
using System.Collections.Generic;
using System.Linq;

namespace MinistryInvestment.Mvc.ViewModels
{
    public abstract class BaseViewModel
    {
        private readonly IMinistryInvestmentConfig _ministryInvestmentConfig;
        private readonly IEnumerable<Category> _categories;
        private readonly IEnumerable<RequestStatus> _requestStatuses;
        private readonly IEnumerable<Region> _regions;
        private readonly IEnumerable<Country> _countries;
        private readonly IEnumerable<PartnerType> _partnerTypes;
        private readonly IEnumerable<ContactType> _contactTypes;
        private readonly IEnumerable<ProjectType> _projectTypes;
        private readonly MenuPermissions _menuPermissions;
        private readonly IEnumerable<VoteTeam> _voteTeams;
        public BaseViewModel
        (
            IMinistryInvestmentConfig ministryInvestmentConfig,
            IEnumerable<Category> categories = null,
            IEnumerable<RequestStatus> requestStatuses = null,
            IEnumerable<Country> countries = null,
            IEnumerable<Region> regions = null,
            IEnumerable<PartnerType> partnerTypes = null,
            IEnumerable<ContactType> contactTypes = null,
            IEnumerable<ProjectType> projectTypes = null,
            MenuPermissions menuPermissions = null,
            IEnumerable<VoteTeam> voteTeams = null
        )
        {
            _regions = regions;
            _countries = countries;
            _ministryInvestmentConfig = ministryInvestmentConfig;
            _categories = categories;
            _requestStatuses = requestStatuses;
            _partnerTypes = partnerTypes;
            _contactTypes = contactTypes;
            _projectTypes = projectTypes;
            _menuPermissions = menuPermissions;
            _voteTeams = voteTeams;
        }
        public IMinistryInvestmentConfig MinistryInvestmentConfig { get { return _ministryInvestmentConfig; } }

        public MenuPermissions MenuPermissions { get { return _menuPermissions; } }

        public IEnumerable<SelectListItem> GetCategories()
        {
            var selectList = new List<SelectListItem>();
            if (_categories != null && _categories.Any())
            {
                foreach (var ca in _categories)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = ca.CategoryId.ToString(),
                        Text = ca.CategoryName
                    });
                }
            }
            return selectList;
        }
        public IEnumerable<SelectListItem> GetRequestStatuses()
        {
            var selectList = new List<SelectListItem>();
            if (_requestStatuses != null && _requestStatuses.Any())
            {
                foreach (var rs in _requestStatuses)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = rs.RequestStatusID.ToString(),
                        Text = rs.RequestStatusName
                    });
                }
            }
            return selectList;
        }
        public IEnumerable<SelectListItem> GetCountries()
        {
            var selectList = new List<SelectListItem>();

            if (_countries != null && _countries.Any())
            {
                foreach (var co in _countries)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = co.CountryID.ToString(),
                        Text = co.CountryName
                    });
                }
            }
            return selectList;
        }
        public IEnumerable<SelectListItem> GetRegions()
        {
            var selectList = new List<SelectListItem>();
            if (_regions != null && _regions.Any())
            {
                foreach (var r in _regions)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = r.RegionId.ToString(),
                        Text = r.RegionName
                    });
                }
            }
            return selectList;
        }
        public IEnumerable<SelectListItem> GetPartnerTypes()
        {
            var selectList = new List<SelectListItem>();
            if (_partnerTypes != null && _partnerTypes.Any())
            {
                foreach (var pt in _partnerTypes)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = pt.PartnerTypeId.ToString(),
                        Text = pt.PartnerTypeName
                    });
                }
            }
            return selectList;
        }
        public IEnumerable<SelectListItem> GetContactTypes()
        {
            var selectList = new List<SelectListItem>();
            if (_contactTypes != null && _contactTypes.Any())
            {
                foreach (var ct in _contactTypes)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = ct.ContactTypeId.ToString(),
                        Text = ct.ContactTypeName
                    });
                }
            }
            return selectList;
        }
        public IEnumerable<SelectListItem> GetProjectTypes()
        {
            var selectList = new List<SelectListItem>();
            if (_projectTypes != null && _projectTypes.Any())
            {
                foreach (var ct in _projectTypes)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = ct.ProjectTypeId.ToString(),
                        Text = ct.ProjectTypeName
                    });
                }
            }
            return selectList;
        }
        public IEnumerable<SelectListItem> GetVoteTeams()
        {
            var selectList = new List<SelectListItem>();
            if (_voteTeams != null && _voteTeams.Any())
            {
                foreach (var vt in _voteTeams)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = vt.VoteTeamID.ToString(),
                        Text = vt.VoteTeamName
                    });
                }
            }
            return selectList;
        }
    }
}