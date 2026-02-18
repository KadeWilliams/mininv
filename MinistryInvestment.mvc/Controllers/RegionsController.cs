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
using System.Threading.Tasks;

namespace MinistryInvestment.Mvc.Controllers
{
    public class RegionsController : BaseController
    {
        public RegionsController(IMinistryInvestmentConfig ministryInvestmentConfig, IMinistryInvestmentService ministryInvestmentService, ILog log, IAccessTokenClaimAuthorizationService authorizationService)
            : base(ministryInvestmentConfig, ministryInvestmentService, log, authorizationService) { }
        [ClaimAuthorize(CheckAuthorization.ADMIN)]
        public async Task<ActionResult> Index(bool error = false)
        {
            var vm = new RegionViewModel(MinistryInvestmentConfig, MinistryInvestmentService.GetRegions(), error, await GetMenuPermissions());

            return View(vm);
        }

        [ClaimAuthorize(CheckAuthorization.ADMIN)]
        public IActionResult SaveRegion(Region region)
        {
            try
            {
                MinistryInvestmentService.SaveRegion(region);
            }
            catch (Exception exc)
            {
                Log.Error($"{exc}");
                return RedirectToAction("Index", new { error = true });
            }
            return RedirectToAction("Index");
        }
    }
}