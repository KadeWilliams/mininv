using HobbyLobby.Avatar.ApplicationSecurity.Authorization;
using HobbyLobby.Avatar.AspNetCore.Authorization;
using HobbyLobby.Logging;
using Microsoft.AspNetCore.Mvc;
using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Core.Services;
using MinistryInvestment.Mvc.Security;
using MinistryInvestment.Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MinistryInvestment.Mvc.Controllers
{
    public class SearchController : BaseController
    {
        public SearchController(IMinistryInvestmentConfig ministryInvestmentConfig, IMinistryInvestmentService ministryInvestmentService, ILog log, IAccessTokenClaimAuthorizationService authorizationService)
            : base(ministryInvestmentConfig, ministryInvestmentService, log, authorizationService) { }
        [ClaimAuthorize(CheckAuthorization.READ)]
        public async Task<ActionResult> Index()
        {
            var vm = new SearchViewModel
            (
                MinistryInvestmentConfig,
                MinistryInvestmentService.GetCategories(),
                MinistryInvestmentService.GetRequestStatuses(),
                new SearchCriteria(),
                MinistryInvestmentService.GetCountries(),
                MinistryInvestmentService.GetRegions(),
                MinistryInvestmentService.GetPartnerTypes(),
                MinistryInvestmentService.GetContactTypes(),
                MinistryInvestmentService.GetProjectTypes(),
                await GetMenuPermissions(),
                MinistryInvestmentService.GetVoteTeams()
            );
            return View(vm);
        }
        [ClaimAuthorize(CheckAuthorization.READ)]
        public ActionResult Search(SearchCriteria searchCriteria
#nullable enable
        )
        {
            try
            {
                searchCriteria.parseDates();
                searchCriteria.ToDataTables();
                return Json(MinistryInvestmentService.Search<dynamic>(searchCriteria));
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Error Searching.");
            }
            return null;
        }

    }
}