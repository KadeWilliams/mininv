using HobbyLobby.DocumentManagement;
using Microsoft.AspNetCore.Http;
using MinistryInvestment.Core.Models;
using Category = MinistryInvestment.Core.Models.Category;

namespace MinistryInvestment.Core.Services
{
    public interface IMinistryInvestmentService
    {
        // Gets
        OrganizationDetail GetOrganizationDetails(int OrganizationId);
        IEnumerable<Contact> GetContacts(int organizationID);
        IEnumerable<FinancialInformation> GetFinancials(int organizationID);
        IEnumerable<FinancialDocument> GetFinancialDocuments(int financialInformationID);
        IEnumerable<Category> GetCategories();
        IEnumerable<VoteTeam> GetVoteTeams();
        IEnumerable<Region> GetRegions();
        IEnumerable<PartnerType> GetPartnerTypes();
        IEnumerable<ContactType> GetContactTypes();
        IEnumerable<ProjectType> GetProjectTypes();
        IEnumerable<Country> GetCountries();
        IEnumerable<Contact> GetOrganizationContacts(int OrganizationId);
        IEnumerable<Request> GetOrganizationRequests(int OrganizationId);
        IEnumerable<OrganizationRegion> GetOrganizationRegions(int OrganizationId);
        Task<IEnumerable<Request>> GetRequests();
        IEnumerable<Organization> GetOrganizations();
        IEnumerable<RequestStatus> GetRequestStatuses();
        IEnumerable<Assessment> GetAssessments(int organizationID);
        IEnumerable<RequestForAssessment> GetRequestsForAssessment(int organizationID);
        AssessmentDetail GetAssessmentDetail(int assessmentID, int organizationID);
        AssessmentDetail NewAssessmentForm(int organizationID);
        int SaveAssessment(AssessmentScoreSubmission submission, string user);
        void DeleteAssessment(int assessmentID);

        IEnumerable<Organization> GetPendingRequestOranizationName(IEnumerable<Request> requests);
        RequestDetail GetRequestDetail(int requestID, int organizationID = 0);
        GiftDetail GetGiftDetail(int giftID, int requestID, int organizationID = 0);
        IEnumerable<Address> GetAddresses(int OrganizationID);

        // Saves
        void SaveCategory(Category category);
        void SaveRegion(Region region);
        void SavePartnerType(PartnerType partnerType);
        void SaveContactType(ContactType contactType);
        void SaveProjectType(ProjectType projectType);
        Task<int> SaveOrganizationDetails(OrganizationDetail organizationDetails, String lastChangeUser);
        Task<int> SaveRequestDetail(RequestDetail requestDetail);
        int SaveGiftDetail(GiftDetail giftDetail);
        Task<Node> UploadFinancialDocument(OrganizationDetail organizationDetail, IFormFile formFile);
        Task<Node> UploadOnePager(RequestDetail requestDetail, IFormFile formFile);
        int SaveAddress(Address address);

        // Search
        IEnumerable<T> Search<T>(SearchCriteria searchCriteria);
    }
}