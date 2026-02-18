using HobbyLobby.Avatar.AspNetCore.Authorization;
using HobbyLobby.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Services;
using MinistryInvestment.Mvc.Security;
using StackExchange.Profiling;
using System;
using System.Threading.Tasks;
using System.Web;

namespace MinistryInvestment.Mvc.Controllers
{
    public class BaseController : Controller
    {
        private readonly ILog _log;
        private readonly IMinistryInvestmentService _ministryInvestmentService;
        private readonly IMinistryInvestmentConfig _ministryInvestmentConfig;
        private readonly IAccessTokenClaimAuthorizationService _authorizationService;

        public BaseController(IMinistryInvestmentConfig ministryInvestmentConfig,
            IMinistryInvestmentService ministryInvestmentService, ILog log, IAccessTokenClaimAuthorizationService authorizationService)
        {
            _ministryInvestmentService = ministryInvestmentService;
            _ministryInvestmentConfig = ministryInvestmentConfig;
            _log = log;
            _authorizationService = authorizationService;
        }
        protected async Task<MenuPermissions> GetMenuPermissions()
        {
            using (MiniProfiler.Current.Step("GetMenuPermission"))
            {
                var checkAuthorization = new CheckAuthorization(_authorizationService, User);
                var menuPermissions = new MenuPermissions();

                menuPermissions.HasAdminPermissions = await checkAuthorization.CheckAdmin(); // Modify Maintenance Pages
                menuPermissions.HasEditPermissions = await checkAuthorization.CheckEdit(); // See everything, Save non-maintenance pages
                menuPermissions.HasReadPermissions = await checkAuthorization.CheckRead(); // Read everything, save nothing
                return menuPermissions;
            }
        }

        protected string GetUserName()
        {
            return User.Identity.Name.ToLower();
        }

        protected IMinistryInvestmentService MinistryInvestmentService
        {
            get { return _ministryInvestmentService; }
        }

        protected IMinistryInvestmentConfig MinistryInvestmentConfig
        {
            get { return _ministryInvestmentConfig; }
        }

        protected ILog Log
        {
            get { return _log; }
        }
        public ActionResult KeepSessionAlive()
        {
            HttpContext.Session.SetString("Timeout", DateTime.Now.ToString());
            return Json(HttpContext.Session.GetString("Timeout"));
        }
        public ActionResult GetLastSessionTime()
        {
            return Json(HttpContext.Session.GetString("Timeout"));
        }
    }
}