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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MinistryInvestment.Mvc.Controllers
{
    public class OrganizationController : BaseController
    {
        public OrganizationController(
            IMinistryInvestmentConfig ministryInvestmentConfig,
            IMinistryInvestmentService ministryInvestmentService,
            ILog log,
            IAccessTokenClaimAuthorizationService authorizationService
        )
            : base(ministryInvestmentConfig, ministryInvestmentService, log, authorizationService) { }
        [ClaimAuthorize(CheckAuthorization.READ)]
        public async Task<ActionResult> Index(string message, int OrganizationID = 0, int RequestID = 0, int GiftID = 0, int AddressID = 0, int ContactID = 0, int FinancialInformationID = 0)
        {
            var vm = new OrganizationViewModel(
                MinistryInvestmentConfig,
                MinistryInvestmentService.GetOrganizationDetails(OrganizationID),
                MinistryInvestmentService.GetCategories(),
                MinistryInvestmentService.GetPartnerTypes(),
                MinistryInvestmentService.GetCountries(),
                MinistryInvestmentService.GetContactTypes(),
                MinistryInvestmentService.GetProjectTypes(),
                MinistryInvestmentService.GetRegions(),
                MinistryInvestmentService.GetRequestStatuses(),
                MinistryInvestmentConfig.ContentServerFolderURL,
                MinistryInvestmentConfig.CategoryOtherID,
                MinistryInvestmentConfig.ProjectTypeOtherID,
                RequestID,
                GiftID,
                AddressID,
                ContactID,
                FinancialInformationID,
                await GetMenuPermissions(),
                MinistryInvestmentService.GetVoteTeams(),
                message
            );
            return View(vm);
        }
        [ClaimAuthorize(CheckAuthorization.EDIT)]
        [HttpPost]
        public async Task<JsonResult> SaveOrganizationDetails([FromForm] OrganizationDetail organizationDetails)
        {
            //TODO:
            // Something to explore at a later date,
            // currently this is forcing this method to fail because not all of the fields are present in the object

            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values
            //            .SelectMany(v => v.Errors)
            //            .Select(e => e.ErrorMessage);
            //    Log.Error($"Validation failed: {string.Join(", ", errors)}");
            //    return Json(new
            //    {
            //        success = false,
            //        errors = errors,
            //    });
            //}

            int OrganizationId = organizationDetails.Organization.OrganizationID;
            string message = "";
            try
            {
                organizationDetails.dateStringToInt();
                //var file = Request.Form.Files["organizationDetails.Organization.OrganizationLogo"];
                var files = Request.Form.Files;
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.ContentType.Contains("image"))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await file.CopyToAsync(memoryStream);
                                organizationDetails.Organization.OrganizationLogo = memoryStream.ToArray();
                            }
                        }
                        else if (organizationDetails.SaveFinancialInformation)
                        {
                            var d = await MinistryInvestmentService.UploadFinancialDocument(organizationDetails, file);
                            organizationDetails.FinancialInformation.DocumentID = d.Id.HasValue ? d.Id.Value : 0;
                            organizationDetails.FinancialInformation.FolderID = d.ParentId.HasValue ? d.Id.Value : 0;
                            organizationDetails.FinancialInformation.DocumentName = d.Name;
                        }
                        else
                        {
                            message = "Unsupported file type uploaded.";
                        }
                    }
                }
                OrganizationId = await MinistryInvestmentService.SaveOrganizationDetails(organizationDetails, GetUserName());
            }
            catch (Exception exc)
            {
                Log.Error($"{exc}");
            }
            return Json
            (new
            {
                url = Url.Action("Index",
                    new
                    {
                        OrganizationID = OrganizationId,
                        organizationDetails.RequestID,
                        organizationDetails.GiftID,
                        organizationDetails.Address.AddressID,
                        organizationDetails.Contact.ContactID,
                        organizationDetails.FinancialInformation.FinancialInformationID,
                        organizationDetails.Assessment.AssessmentID,
                    })
            });
            //return Json(new { message = "Organization Saved Successfully", OrganizationId });
        }

        [ClaimAuthorize(CheckAuthorization.EDIT)]
        [HttpPost]
        [Consumes("application/json")]
        public JsonResult SaveAddress([FromBody] string address)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                Log.Error($"Validation failed: {string.Join(", ", errors)}");
                return Json(new
                {
                    success = false,
                    errors = errors,
                });
            }
            return Json(0);
            //return Json(new { addressId = MinistryInvestmentService.SaveAddress((address) });
        }
        [ClaimAuthorize(CheckAuthorization.EDIT)]
        [HttpPost]
        public async Task<JsonResult> SaveRequestDetail([FromForm] RequestDetail requestDetail)
        {
            requestDetail.Request.LastChangeUser = GetUserName();
            int OrganizationId = requestDetail.Request.OrganizationID;
            int RequestID = requestDetail.Request.RequestID;
            string message = "";
            try
            {

                var files = Request.Form.Files;
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var d = await MinistryInvestmentService.UploadOnePager(requestDetail, file);
                        requestDetail.Request.OnePagerDocumentID = d.Id.HasValue ? d.Id.Value : 0;
                    }
                }
                RequestID = await MinistryInvestmentService.SaveRequestDetail(requestDetail);
                return Json(new { message = "Gift Saved Successfully", RequestID });
            }
            catch (Exception exc)
            {
                Log.Error($"{exc}");
            }
            return Json(new { message = "Gift Saved Successfully", RequestID });
        }
        [ClaimAuthorize(CheckAuthorization.EDIT)]
        public async Task<JsonResult> SaveGiftForm([FromForm] GiftDetail giftDetail)
        {
            giftDetail.Gift.LastChangeUser = GetUserName();
            int GiftID = giftDetail.Gift.GiftID;

            string message = "";
            try
            {
                GiftID = MinistryInvestmentService.SaveGiftDetail(giftDetail);
            }
            catch (Exception exc)
            {
                Log.Error($"{exc}");
            }

            //return Json(new { message = "Gift Saved Successfully", url = Url.Action("LoadGiftPartial", "Organization", new { giftID = giftDetail.Gift.GiftID, requestID = giftDetail.RequestID }), requestID = giftDetail.RequestID });
            return Json(new { message = "Gift Saved Successfully", giftDetail.RequestID, GiftID });
        }

        [ClaimAuthorize(CheckAuthorization.READ)]
        public IActionResult GetContacts(int organizationID)
        {
            return Json(MinistryInvestmentService.GetContacts(organizationID));
        }
        [ClaimAuthorize(CheckAuthorization.READ)]
        public IActionResult GetAddresses(int organizationID)
        {
            return Json(MinistryInvestmentService.GetAddresses(organizationID));
        }

        [ClaimAuthorize(CheckAuthorization.READ)]
        public IActionResult GetFinancials(int organizationID)
        {
            return Json(MinistryInvestmentService.GetFinancials(organizationID));
        }

        [ClaimAuthorize(CheckAuthorization.READ)]
        public IActionResult GetFinancialDocuments(int financialInformationID)
        {
            return Json(MinistryInvestmentService.GetFinancialDocuments(financialInformationID));
        }

        [ClaimAuthorize(CheckAuthorization.READ)]
        public async Task<ActionResult> RequestFormPartial(int requestID, int organizationID)
        {
            var model = new RequestPartialViewModel(MinistryInvestmentConfig,
                MinistryInvestmentService.GetRequestDetail(requestID, organizationID),
                MinistryInvestmentService.GetRequestStatuses(),
                MinistryInvestmentService.GetProjectTypes(),
                MinistryInvestmentService.GetVoteTeams(),
                await GetMenuPermissions()
            );
            return PartialView("_RequestPartial", model);
        }
        [ClaimAuthorize(CheckAuthorization.READ)]
        public async Task<ActionResult> LoadGiftPartial(int giftID, int requestID, int organizationID = 0)
        {
            var model = new GiftPartialViewModel(MinistryInvestmentConfig,
                MinistryInvestmentService.GetGiftDetail(giftID, requestID, organizationID),
                await GetMenuPermissions(),
                requestID);
            return PartialView("_GiftPartial", model);
        }

        [ClaimAuthorize(CheckAuthorization.READ)]
        public IActionResult GetAssessments(int organizationID)
        {
            return Json(MinistryInvestmentService.GetAssessments(organizationID));
        }

        [ClaimAuthorize(CheckAuthorization.READ)]
        public IActionResult GetAssessmentDetails(int assessmentID, int organizationID)
        {
            try
            {
                var detail = MinistryInvestmentService.GetAssessmentDetail(assessmentID, organizationID);
                return Json(detail);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get assessment detail: {ex.Message}", ex);
                return BadRequest("Failed to load assessment");
            }
        }
        [ClaimAuthorize(CheckAuthorization.EDIT)]
        public IActionResult NewAssessmentForm(int organizationID)
        {
            try
            {
                var detail = MinistryInvestmentService.NewAssessmentForm(organizationID);
                return Json(detail);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to create new assessment form: {ex.Message}", ex);
                return BadRequest("Failed to load assessment form");
            }
        }

        [ClaimAuthorize(CheckAuthorization.EDIT)]
        [HttpPost]
        public async Task<JsonResult> SaveAssessment([FromForm] AssessmentScoreSubmission submission)
        {
            try
            {
                int assessmentId = MinistryInvestmentService.SaveAssessment(submission, GetUserName());
                return Json(new
                {
                    success = true,
                    assessmentID = assessmentId
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save assessment: {ex.Message}", ex);
                return Json(new
                {
                    success = false,
                    error = ex.Message.Contains("This request already has an assessment.") ? ex.Message : "Failed to save assessment"
                });
            }
        }


        [ClaimAuthorize(CheckAuthorization.EDIT)]
        public async Task<JsonResult> DeleteAssessment(int assessmentID)
        {
            try
            {
                MinistryInvestmentService.DeleteAssessment(assessmentID);
                return Json(new
                { success = true });
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to delete assessment: {ex.Message}", ex);
                return Json(new
                {
                    success = true,
                    error = "Failed to delete assessment"
                });
            }
        }
    }
}