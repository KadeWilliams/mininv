using Microsoft.AspNetCore.Mvc.Rendering;
using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Mvc.Security;
using System;
using System.Collections.Generic;

namespace MinistryInvestment.Mvc.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private readonly SearchCriteria _searchCriteria;
        public SearchViewModel
        (
            IMinistryInvestmentConfig ministryInvestmentConfig,
            IEnumerable<Category> categories,
            IEnumerable<RequestStatus> requestStatuses,
            SearchCriteria searchCriteria,
            IEnumerable<Country> countries,
            IEnumerable<Region> regions,
            IEnumerable<PartnerType> partnerType,
            IEnumerable<ContactType> contactype,
            IEnumerable<ProjectType> projectType,
            MenuPermissions menuPermissions,
            IEnumerable<VoteTeam> voteTeams
        )
            : base(ministryInvestmentConfig, categories, requestStatuses, countries, regions, partnerType, contactype, projectType, menuPermissions, voteTeams)
        {
            _searchCriteria = searchCriteria;
        }



        public IEnumerable<SelectListItem> GetsearchData()
        {
            var selectList = new List<SelectListItem>();
            var categories = new SelectListGroup { Name = "Category" };
            var countries = new SelectListGroup { Name = "Country" };
            var partnerTypes = new SelectListGroup { Name = "Partner Type" };
            var regions = new SelectListGroup { Name = "Region" };
            var statuses = new SelectListGroup { Name = "Status" };
            var contactTypes = new SelectListGroup { Name = "Contact Type" };
            var projectTypes = new SelectListGroup { Name = "Project Type" };
            var voteTeams = new SelectListGroup { Name = "Vote Team" };
            var booleans = new SelectListGroup { Name = "Flags", Disabled = true };

            foreach (var x in GetCategories())
            {
                x.Value = "Category," + x.Value;
                x.Group = categories;
                selectList.Add(x);
            }

            foreach (var x in GetRequestStatuses())
            {
                x.Value = "Status," + x.Value;
                x.Group = statuses;
                selectList.Add(x);
            }
            statuses.Disabled = true;

            foreach (var x in GetRegions())
            {
                x.Value = "Region," + x.Value;
                x.Group = regions;
                selectList.Add(x);
            }

            foreach (var x in GetPartnerTypes())
            {
                x.Value = "PartnerType," + x.Value;
                x.Group = partnerTypes;
                selectList.Add(x);
            }

            foreach (var x in GetVoteTeams())
            {
                x.Value = "Team," + x.Value;
                x.Group = voteTeams;
                selectList.Add(x);
            }
            voteTeams.Disabled = true;

            foreach (var x in GetContactTypes())
            {
                x.Value = "ContactTypeID," + x.Value;
                x.Group = contactTypes;
                selectList.Add(x);
            }
            contactTypes.Disabled = true;

            foreach (var x in GetProjectTypes())
            {
                x.Value = "ProjectTypeID," + x.Value;
                x.Group = projectTypes;
                selectList.Add(x);
            }
            projectTypes.Disabled = true;

            foreach (var x in Enum.GetNames(typeof(Flags)))
            {
                var si = new SelectListItem
                {
                    Value = "Flag," + x,
                    Group = booleans,
                    Text = x,
                };
                selectList.Add(si);
            }

            // place all new criteria above countries, tomselect limits the number of items show in the option group
            // so placing it above is the easiest way to get new criteria added to the page
            foreach (var x in GetCountries())
            {
                x.Value = "Country," + x.Value;
                x.Group = countries;
                selectList.Add(x);
            }

            return selectList;
        }
        public IEnumerable<SelectListItem> GetSearchFor()
        {
            var selectList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Search Organizations" },
                new SelectListItem { Value = "1", Text = "Search Requests" },
                new SelectListItem { Value = "2", Text = "Search Contacts" },
                new SelectListItem { Value = "3", Text = "Search Gifts" }
            };
            return selectList;
        }
        public SearchCriteria SearchCriteria { get { return _searchCriteria; } }

        public enum Flags
        {
            Conditional,
            Recurring
        }
    }
}