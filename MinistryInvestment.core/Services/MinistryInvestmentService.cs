using HobbyLobby.DocumentManagement;
using HobbyLobby.Logging;
using Microsoft.AspNetCore.Http;
using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.DocumentManagement;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Core.Repositories;
using Category = MinistryInvestment.Core.Models.Category;

namespace MinistryInvestment.Core.Services
{
    public class MinistryInvestmentService : IMinistryInvestmentService
    {
        private readonly IMinistryInvestmentRepository _ministryInvestmentRepository;
        private readonly IDocumentManagementHelper _documentManagementHelper;
        private readonly ILog _log;
        private readonly IMinistryInvestmentConfig _ministryInvestmentConfig;

        public MinistryInvestmentService(IMinistryInvestmentRepository ministryInvestmentRepository, IDocumentManagementHelper documentManagementHelper, ILog log, IMinistryInvestmentConfig ministryInvestmentConfig)
        {
            _ministryInvestmentRepository = ministryInvestmentRepository;
            _documentManagementHelper = documentManagementHelper;
            _log = log;
            _ministryInvestmentConfig = ministryInvestmentConfig;
        }

        // Gets
        public OrganizationDetail GetOrganizationDetails(int OrganizationId)
        {
            var OrgDetails = new OrganizationDetail();
            if (OrganizationId < 0)
            {
                OrganizationId = OrganizationId * -1;
                OrgDetails.SaveConflict = true;
            }
            OrgDetails.Organization = _ministryInvestmentRepository.GetOrganization(OrganizationId);
            OrgDetails.Contacts = _ministryInvestmentRepository.GetOrganizationContacts(OrganizationId);
            OrgDetails.Requests = _ministryInvestmentRepository.GetOrganizationRequests(OrganizationId);
            OrgDetails.Financials = _ministryInvestmentRepository.GetFinancialInformation(OrganizationId);
            //OrgDetails.Assessments = _ministryInvestmentRepository.GetAssessments(OrganizationId);
            IEnumerable<OrganizationRegion> orgRegion = _ministryInvestmentRepository.GetOrganizationRegions(OrganizationId);
            OrgDetails.OrganizationRegions = new String[orgRegion.Count()];
            int i = 0;
            foreach (var x in orgRegion)
            {
                OrgDetails.OrganizationRegions[i] = x.RegionID.ToString();
                i++;
            }
            return OrgDetails;
        }
        public IEnumerable<Contact> GetContacts(int organizationID)
        {
            return _ministryInvestmentRepository.GetOrganizationContacts(organizationID);
        }
        public IEnumerable<FinancialInformation> GetFinancials(int organizationID)
        {
            return _ministryInvestmentRepository.GetFinancialInformation(organizationID);
        }
        public IEnumerable<FinancialDocument> GetFinancialDocuments(int financialInformationID)
        {
            var fd = _ministryInvestmentRepository.GetFinancialDocuments(financialInformationID);
            foreach (var d in fd)
            {
                d.Link = string.Format(_ministryInvestmentConfig.ContentServerDocumentURL, d.DocumentID, d.FolderID);
            }
            return fd;
        }
        public IEnumerable<Category> GetCategories()
        {
            return _ministryInvestmentRepository.GetCategories();
        }
        public IEnumerable<VoteTeam> GetVoteTeams()
        {
            return _ministryInvestmentRepository.GetVoteTeams();
        }

        public IEnumerable<Region> GetRegions()
        {
            return _ministryInvestmentRepository.GetRegions();
        }
        public IEnumerable<PartnerType> GetPartnerTypes()
        {
            return _ministryInvestmentRepository.GetPartnerTypes();
        }
        public IEnumerable<ContactType> GetContactTypes()
        {
            return _ministryInvestmentRepository.GetContactTypes();
        }
        public IEnumerable<ProjectType> GetProjectTypes()
        {
            return _ministryInvestmentRepository.GetProjectTypes();
        }
        public IEnumerable<Country> GetCountries()
        {
            return _ministryInvestmentRepository.GetCountries();
        }
        public IEnumerable<Address> GetAddresses(int OrganizationID)
        {
            return _ministryInvestmentRepository.GetAddresses(OrganizationID);
        }
        public IEnumerable<Contact> GetOrganizationContacts(int OrganizationId)
        {
            return _ministryInvestmentRepository.GetOrganizationContacts(OrganizationId);
        }
        public IEnumerable<Request> GetOrganizationRequests(int OrganizationId)
        {
            return _ministryInvestmentRepository.GetOrganizationRequests(OrganizationId);
        }
        public async Task<IEnumerable<Request>> GetRequests()
        {
            return await _ministryInvestmentRepository.GetRequests();
        }
        public IEnumerable<Organization> GetOrganizations()
        {
            return _ministryInvestmentRepository.GetOrganizations();
        }
        public IEnumerable<RequestStatus> GetRequestStatuses()
        {
            return _ministryInvestmentRepository.GetRequestStatuses();
        }
        public IEnumerable<OrganizationRegion> GetOrganizationRegions(int OrganizationId)
        {
            return _ministryInvestmentRepository.GetOrganizationRegions(OrganizationId);
        }
        public IEnumerable<Organization> GetPendingRequestOranizationName(IEnumerable<Request> requests)
        {
            List<Organization> organizations = new List<Organization>();
            foreach (var r in requests)
            {
                organizations.Add(_ministryInvestmentRepository.GetOrganization(r.OrganizationID));
            }
            return organizations;
        }
        public RequestDetail GetRequestDetail(int requestID, int organizationID)
        {
            var rd = new RequestDetail();
            rd.Request.OrganizationID = organizationID;

            if (requestID == 0)
                return rd;

            rd.Request = _ministryInvestmentRepository.GetRequest(requestID);
            rd.Gifts = _ministryInvestmentRepository.GetGifts(requestID);
            return rd;
        }
        public GiftDetail GetGiftDetail(int giftID, int requestID, int organizationID = 0)
        {
            var gd = new GiftDetail();
            gd.RequestID = requestID;
            gd.Gift.OrganizationID = organizationID;
            if (giftID == 0)
                return gd;

            gd.Gift = _ministryInvestmentRepository.GetGift(giftID);
            gd.Gift.GiftSchedules = _ministryInvestmentRepository.GetGiftSchedules(giftID);
            return gd;
        }

        // Saves
        public async Task<int> SaveOrganizationDetails(OrganizationDetail organizationDetails, string lastChangeUser)
        {
            DateTime time = DateTime.Now;
            DateTime? PreviousChangeDttm = organizationDetails.Organization.LastChangeDttm;
            organizationDetails.Organization.LastChangeUser = lastChangeUser;
            organizationDetails.Organization.LastChangeDttm = time;
            int organizationID;
            if (organizationDetails.Organization.OrganizationID != 0)
            {
                if (organizationDetails.ContentServer)
                {
                    var d = await _documentManagementHelper.CreateOrganizationFolder(organizationDetails.Organization);
                    organizationDetails.Organization.CSFolderId = d.FolderID;
                }

                organizationID = _ministryInvestmentRepository.SaveOrganization(organizationDetails.Organization, PreviousChangeDttm);

                if (organizationDetails.ForDeletion)
                {
                    _ministryInvestmentRepository.DeleteOrganizationDetail(organizationDetails);
                    organizationDetails.SaveRequest = organizationDetails.SaveContact = false;
                }
                else if (organizationDetails.SaveContact)
                {
                    organizationDetails.Contact.LastChangeUser = lastChangeUser;
                    organizationDetails.Contact.LastChangeDttm = time;
                    organizationDetails.Contact.OrganizationID = organizationID;
                    organizationDetails.Contact.ContactID = _ministryInvestmentRepository.SaveContact(organizationDetails.Contact);
                }
                else if (organizationDetails.SaveRequest)
                {
                    organizationDetails.Request.LastChangeUser = lastChangeUser;
                    organizationDetails.Request.LastChangeDttm = time;
                    _ministryInvestmentRepository.SaveRequest(organizationDetails.Request);
                }
                else if (organizationDetails.SaveAddress)
                {
                    organizationDetails.Address.LastChangeUser = lastChangeUser;
                    organizationDetails.Address.LastChangeDttm = time;
                    organizationDetails.Address.OrganizationID = organizationID;
                    organizationDetails.Address.AddressID = _ministryInvestmentRepository.SaveAddress(organizationDetails.Address);
                }
                else if (organizationDetails.SaveFinancialInformation)
                {
                    organizationDetails.FinancialInformation.LastChangeUser = lastChangeUser;
                    organizationDetails.FinancialInformation.OrganizationID = organizationID;
                    organizationDetails.FinancialInformation.FinancialInformationID = _ministryInvestmentRepository.SaveFinancialInformation(organizationDetails.FinancialInformation);
                    if (organizationDetails.FinancialInformation.DocumentID > 0)
                    {
                        await _ministryInvestmentRepository.SaveFinancialDocument(organizationDetails.FinancialInformation, lastChangeUser);
                    }
                }
                else if (organizationDetails.SaveAssessment)
                {
                    organizationDetails.Assessment.LastChangeUser = lastChangeUser;
                    organizationDetails.Assessment.OrganizationID = organizationID;
                    //organizationDetails.Assessment.AssessmentID = _ministryInvestmentRepository.SaveAssessment(organizationDetails.Assessment);
                }

                if (organizationDetails.OrganizationRegions != null)
                {
                    string x = "";
                    foreach (var r in organizationDetails.OrganizationRegions) { x = x + r + ","; }
                    _ministryInvestmentRepository.SaveOrganizationRegions(organizationDetails.Organization.OrganizationID, x.Trim(','));
                }
                if (organizationDetails.OrganizationRegions == null)
                {
                    _ministryInvestmentRepository.SaveOrganizationRegions(organizationDetails.Organization.OrganizationID, null);
                }
                if (organizationDetails.DeleteContact)
                {
                    _ministryInvestmentRepository.DeleteContact(organizationDetails);
                }
                if (organizationDetails.DeleteAddress)
                {
                    _ministryInvestmentRepository.DeleteAddress(organizationDetails);
                }
                if (organizationDetails.DeleteFinancialInformation)
                {
                    _ministryInvestmentRepository.DeleteFinancialInformation(organizationDetails);
                }
                if (organizationDetails.DeleteAssessment)
                {
                    _ministryInvestmentRepository.DeleteAssessment(organizationDetails);
                }
                return organizationID;
            }
            else
            {
                PreviousChangeDttm = null;
                organizationID = _ministryInvestmentRepository.SaveOrganization(organizationDetails.Organization, PreviousChangeDttm);
                if (organizationDetails.OrganizationRegions != null)
                {
                    string x = "";
                    foreach (var r in organizationDetails.OrganizationRegions) { x = x + r + ","; }
                    _ministryInvestmentRepository.SaveOrganizationRegions(organizationID, x.Trim(','));
                }
                return organizationID;
            }
        }

        public async Task<Node> UploadFinancialDocument(OrganizationDetail organizationDetail, IFormFile formFile)
        {
            if (organizationDetail.Organization.CSFolderId <= 0)
            {
                var d = await _documentManagementHelper.CreateOrganizationFolder(organizationDetail.Organization);
                organizationDetail.Organization.CSFolderId = d.FolderID;
            }
            return await _documentManagementHelper.UploadFinancialDocument(organizationDetail.Organization.CSFolderId, formFile);
        }
        public async Task<Node> UploadOnePager(RequestDetail requestDetail, IFormFile formFile)
        {
            if (requestDetail.Request.CSFolderId <= 0)
            {
                var d = await _documentManagementHelper.CreateOrganizationFolder(requestDetail.Request);
                requestDetail.Request.CSFolderId = d.FolderID;
            }
            return await _documentManagementHelper.UploadOnePagerDocument(requestDetail.Request.CSFolderId, formFile);
        }
        public void SaveCategory(Category category)
        {
            if (category.CategoryName == "-1")
            {
                _ministryInvestmentRepository.DeleteCategory(category.CategoryId);
            }
            else
            {
                _ministryInvestmentRepository.SaveCategory(category);
            }
        }
        public void SaveRegion(Region region)
        {
            if (region.RegionName == "-1")
            {
                _ministryInvestmentRepository.DeleteRegion(region.RegionId);
            }
            else
            {
                _ministryInvestmentRepository.SaveRegion(region);
            }
        }
        public void SavePartnerType(PartnerType partnertype)
        {
            if (partnertype.PartnerTypeName == "-1")
            {
                _ministryInvestmentRepository.DeletePartnerType(partnertype.PartnerTypeId);
            }
            else
            {
                _ministryInvestmentRepository.SavePartnerType(partnertype);
            }
        }
        public void SaveContactType(ContactType contactType)
        {
            if (contactType.ContactTypeName == "-1")
            {
                _ministryInvestmentRepository.DeleteContactType(contactType.ContactTypeId);
            }
            else
            {
                _ministryInvestmentRepository.SaveContactType(contactType);
            }
        }
        public void SaveProjectType(ProjectType projectType)
        {
            if (projectType.ProjectTypeName == "-1")
            {
                _ministryInvestmentRepository.DeleteProjectType(projectType.ProjectTypeId);
            }
            else
            {
                _ministryInvestmentRepository.SaveProjectType(projectType);
            }
        }
        public async Task<int> SaveRequestDetail(RequestDetail requestDetail)
        {
            if (requestDetail.SaveGift)
            {
                _ministryInvestmentRepository.SaveGift(requestDetail);
            }
            if (requestDetail.DeleteGift)
            {
                _ministryInvestmentRepository.DeleteGift(requestDetail);
            }
            return await _ministryInvestmentRepository.SaveRequest(requestDetail.Request);
        }
        public int SaveGiftDetail(GiftDetail giftDetail)
        {
            if (giftDetail.SaveGiftSchedule)
                _ministryInvestmentRepository.SaveGiftSchedule(giftDetail);

            if (giftDetail.GenerateSchedules)
                _ministryInvestmentRepository.GenerateSchedules(giftDetail);

            if (giftDetail.DeleteGiftSchedule)
                _ministryInvestmentRepository.DeleteGiftSchedule(giftDetail);

            giftDetail.Gift.GiftID = _ministryInvestmentRepository.SaveGift(giftDetail);

            if (giftDetail.Gift.Conditional == true)
                _ministryInvestmentRepository.SaveCondition(giftDetail);
            else if (giftDetail.Gift.ConditionID != 0)
                _ministryInvestmentRepository.DeleteCondition(giftDetail);

            return giftDetail.Gift.GiftID;
        }

        public int SaveAddress(Address address)
        {
            return _ministryInvestmentRepository.SaveAddress(address);
        }

        public IEnumerable<Assessment> GetAssessments(int organizationID)
        {
            return _ministryInvestmentRepository.GetAssessmentsByOrganization(organizationID);
        }
        public IEnumerable<RequestForAssessment> GetRequestsForAssessment(int organizationID)
        {
            return _ministryInvestmentRepository.GetRequestsForAssessment(organizationID);
        }

        public AssessmentDetail GetAssessmentDetail(int assessmentID, int organizationID)
        {
            var assessmentNote = _ministryInvestmentRepository.GetAssessmentNote(assessmentID);
            var scores = _ministryInvestmentRepository.GetAssessmentDetail(assessmentID);
            var requests = _ministryInvestmentRepository.GetRequestsForAssessment(organizationID);

            var selectedRequestID = _ministryInvestmentRepository.GetAssessmentRequestID(assessmentID);

            return BuildAssessmentDetail(assessmentID, scores, requests, organizationID, assessmentNote, selectedRequestID);
        }
        public AssessmentDetail NewAssessmentForm(int organizationID)
        {
            var categories = _ministryInvestmentRepository.GetAssessmentCategories();
            var requests = _ministryInvestmentRepository.GetRequestsForAssessment(organizationID);

            var scores = categories.Select(c => new AssessmentScore
            {
                AssessmentCategoryID = c.AssessmentCategoryID,
                Header = c.Header,
                Category = c.Category,
                Description = c.Description,
                Weight = c.Weight,
                Score = 0,
                Notes = null
            });
            return BuildAssessmentDetail(0, scores, requests, organizationID);
        }
        private AssessmentDetail BuildAssessmentDetail(int assessmentID, IEnumerable<AssessmentScore> scores, IEnumerable<RequestForAssessment> requests, int organizationID = 0, string? notes = null, int? selectedRequestID = null)
        {
            var scoreList = scores.ToList();
            var headerGroups = scoreList
                .GroupBy(c => c.Header)
                .Select(g => new AssessmentHeaderGroup
                {
                    Header = g.Key,
                    Questions = g.ToList(),
                    HeaderScore = CalculateHeaderScore(g.ToList())
                })
                .OrderBy(g => g.Header)
                .ToList();

            return new AssessmentDetail
            {
                AssessmentID = assessmentID,
                OrganizationID = organizationID,
                Notes = notes,
                HeaderGroups = headerGroups,
                Requests = requests,
                SelectedRequestID = selectedRequestID
            };
        }

        public int SaveAssessment(AssessmentScoreSubmission submission, string user)
        {
            int assessmentID = _ministryInvestmentRepository.SaveAssessment(submission.AssessmentID, submission.OrganizationID, submission.RequestID, submission.Notes, user);

            _ministryInvestmentRepository.SaveAssessmentScores(assessmentID, submission.Scores, user);

            return assessmentID;
        }

        public void DeleteAssessment(int assessmentID)
        {
            _ministryInvestmentRepository.DeleteAssessment(assessmentID);
        }
        private decimal? CalculateHeaderScore(List<AssessmentScore> questions)
        {
            var answered = questions.Where(q => q.Score > 0).ToList();
            if (answered.Count == 0) return null;

            decimal score = answered.Sum(q => q.Score);
            return score;
        }
        // Search
        public IEnumerable<T> Search<T>(SearchCriteria searchCriteria)
        {
            var d = _ministryInvestmentRepository.GetSearchResults<T>(searchCriteria);
            return d;
        }
    }
}