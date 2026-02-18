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
    public class CategoriesController : BaseController
    {
        public CategoriesController(IMinistryInvestmentConfig ministryInvestmentConfig, IMinistryInvestmentService ministryInvestmentService, ILog log, IAccessTokenClaimAuthorizationService authorizationService)
            : base(ministryInvestmentConfig, ministryInvestmentService, log, authorizationService) { }
        [ClaimAuthorize(CheckAuthorization.ADMIN)]
        public async Task<ActionResult> Index(bool error = false)
        {
            var vm = new CategoryViewModel(MinistryInvestmentConfig, MinistryInvestmentService.GetCategories(), error, await GetMenuPermissions());

            return View(vm);
        }

        [ClaimAuthorize(CheckAuthorization.ADMIN)]
        public IActionResult SaveCategory(Category category)
        {
            try
            {
                MinistryInvestmentService.SaveCategory(category);
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