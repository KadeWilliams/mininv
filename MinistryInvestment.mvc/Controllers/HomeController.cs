using HobbyLobby.Avatar.ApplicationSecurity.Authorization;
using HobbyLobby.Avatar.AspNetCore.Authorization;
using HobbyLobby.Logging;
using Microsoft.AspNetCore.Mvc;
using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Core.Services;
using MinistryInvestment.Mvc.Security;
using MinistryInvestment.Mvc.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MinistryInvestment.Mvc.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(
            IMinistryInvestmentConfig ministryInvestmentConfig,
            IMinistryInvestmentService ministryInvestmentService,
            ILog log,
            IAccessTokenClaimAuthorizationService authorizationService
        )
            : base(ministryInvestmentConfig, ministryInvestmentService, log, authorizationService) { }
        public async Task<ActionResult> Index()
        {
            var permissions = await GetMenuPermissions();
            if (permissions.HasEditPermissions || permissions.HasAdminPermissions)
            {
                var vm = new HomeViewModel(
                    MinistryInvestmentConfig,
                    await MinistryInvestmentService.GetRequests(),
                    MinistryInvestmentService.GetRequestStatuses(),
                    MinistryInvestmentService.GetProjectTypes(),
                    MinistryInvestmentService.GetOrganizations(),
                    await GetMenuPermissions()
                );
                return View(vm);
            }
            return RedirectToAction("Index", "Search");
        }
    }
}