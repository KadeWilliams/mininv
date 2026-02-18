using HobbyLobby.Avatar.AspNetCore.Authorization;
using HobbyLobby.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Services;
using MinistryInvestment.Mvc.ViewModels;
using System.Threading.Tasks;

namespace MinistryInvestment.Mvc.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IMinistryInvestmentConfig ministryInvestmentConfig, IMinistryInvestmentService ministryInvestmentService, ILog log, IAccessTokenClaimAuthorizationService authorizationService) : base(ministryInvestmentConfig, ministryInvestmentService, log, authorizationService)
        {
        }

        [HttpPost]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            });

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        public async Task<IActionResult> AccessDenied()
        {
            return View(new AccountViewModel(MinistryInvestmentConfig, await GetMenuPermissions()));
        }
    }
}