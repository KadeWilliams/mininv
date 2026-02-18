using MinistryInvestment.Core.Models;

namespace MinistryInvestment.Core.Repositories
{
    public interface IMinistryInvestmentRepository
    {
        // Get
        Organization GetOrganization(int OrganizationId);
        IEnumerable<Category> GetCategories();
        IEnumerable<VoteTeam> GetVoteTeams();
        IEnumerable<Region> GetRegions();
        IEnumerable<PartnerType> GetPartnerTypes();
        IEnumerable<ContactType> GetContactTypes();
        IEnumerable<ProjectType> GetProjectTypes();
        IEnumerable<Organization> GetOrganizations();
        IEnumerable<Country> GetCountries();
        IEnumerable<Address> GetAddresses(int OrganizationID);
        IEnumerable<Contact> GetOrganizationContacts(int OrganizationId);
        IEnumerable<Request> GetOrganizationRequests(int OrganizationId);
        IEnumerable<OrganizationRegion> GetOrganizationRegions(int OrganizationId);
        IEnumerable<FinancialInformation> GetFinancialInformation(int OrganizationId);
        IEnumerable<FinancialDocument> GetFinancialDocuments(int FinancialInformationID);
        Task SaveFinancialDocument(FinancialInformation fi, string user);
        Task<IEnumerable<Request>> GetRequests();
        IEnumerable<RequestStatus> GetRequestStatuses();
        IEnumerable<Assessment> GetAssessmentsByOrganization(int organizationID);
        string GetAssessmentNote(int assessmentID);
        IEnumerable<RequestForAssessment> GetRequestsForAssessment(int organizationID);
        int? GetAssessmentRequestID(int assessmentID);
        IEnumerable<AssessmentScore> GetAssessmentDetail(int assessmentID);
        IEnumerable<AssessmentCategory> GetAssessmentCategories();
        int SaveAssessment(int assessmentID, int organizationID, int requestID, string notes, string user);
        void SaveAssessmentScores(int assessmentID, IEnumerable<ScoreEntry> scores, string user);
        void DeleteAssessment(int assessmentID);
        Request GetRequest(int requestID);
        IEnumerable<Gift> GetGifts(int requestID);
        Gift GetGift(int giftID);
        IEnumerable<GiftSchedule> GetGiftSchedules(int requestID);
        IEnumerable<Condition> GetConditions(int requestID);
        // Save
        void SaveCategory(Category category);
        void SaveRegion(Region region);
        int SaveFinancialInformation(FinancialInformation fi);
        //int SaveAssessment(Assessment a);
        void SavePartnerType(PartnerType partnerType);
        void SaveContactType(ContactType contactType);
        void SaveProjectType(ProjectType projectType);
        int SaveOrganization(Organization organization, DateTime? PreviouseChangeDttm);
        int SaveContact(Contact contact);
        void DeleteContact(OrganizationDetail organizationDetail);
        void DeleteAddress(OrganizationDetail organizationDetail);
        void DeleteFinancialInformation(OrganizationDetail organizationDetail);
        void DeleteAssessment(OrganizationDetail organizationDetail);
        Task<int> SaveRequest(Request request);
        int SaveGift(RequestDetail requestDetail);
        int SaveGift(GiftDetail giftDetail);
        void DeleteGift(RequestDetail requestDetail);
        void SaveGiftSchedule(GiftDetail giftDetail);
        void DeleteGiftSchedule(GiftDetail giftDetail);
        void SaveCondition(GiftDetail giftDetail);
        void DeleteCondition(GiftDetail giftDetail);
        void GenerateSchedules(GiftDetail giftDetail);
        void SaveOrganizationRegions(int organizationID, string regionID);
        int SaveAddress(Address address);
        // Search
        IEnumerable<T> GetSearchResults<T>(SearchCriteria searchCriteria);
        // Delete
        void DeleteOrganizationDetail(OrganizationDetail organizationDetail);
        void DeleteRegion(int regionID);
        void DeleteCategory(int CategoryID);
        void DeletePartnerType(int PartnerTypeID);
        void DeleteContactType(int ContactTypeID);
        void DeleteProjectType(int ProjectTypeID);

    }
}