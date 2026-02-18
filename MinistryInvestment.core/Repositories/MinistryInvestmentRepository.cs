using Dapper;
using Microsoft.Data.SqlClient;
using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using System.Data;
using System.Data.Common;

namespace MinistryInvestment.Core.Repositories
{
    public class MinistryInvestmentRepository : IMinistryInvestmentRepository
    {
        private readonly IMinistryInvestmentConfig _ministryInvestmentConfig;
        public MinistryInvestmentRepository(IMinistryInvestmentConfig ministryInvestmentConfig)
        {
            _ministryInvestmentConfig = ministryInvestmentConfig;
        }

        // gets
        public Organization GetOrganization(int OrganizationId)
        {
            try
            {
                Organization organization = new Organization();
                if (OrganizationId != 0)
                {
                    using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
                    {
                        organization = conn.QuerySingle<Organization>(
                            "usp_GetOrganization",
                            new
                            {
                                OrganizationId
                            },
                            commandType: CommandType.StoredProcedure);
                    }
                }
                return organization;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IEnumerable<OrganizationRegion> GetOrganizationRegions(int OrganizationId)
        {
            IEnumerable<OrganizationRegion> organizationRegions;
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                organizationRegions = conn.Query<OrganizationRegion>(
                    "usp_GetOrganizationRegions",
                    new
                    {
                        OrganizationId
                    },
                    commandType: CommandType.StoredProcedure);
            }
            return organizationRegions;
        }
        public IEnumerable<Organization> GetOrganizations()
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.Query<Organization>(
                    "usp_GetOrganizations",
                    commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<Category> GetCategories()
        {
            IEnumerable<Category> categories;

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                categories = conn.Query<Category>(
                    "usp_GetCategories",
                    commandType: CommandType.StoredProcedure);
            }

            return categories;
        }
        public IEnumerable<VoteTeam> GetVoteTeams()
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.Query<VoteTeam>(
                    "usp_GetVoteTeams",
                    commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<RequestStatus> GetRequestStatuses()
        {
            IEnumerable<RequestStatus> requestStatuses;

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                requestStatuses = conn.Query<RequestStatus>(
                    "usp_GetRequestStatuses",
                    commandType: CommandType.StoredProcedure);
            }
            return requestStatuses;
        }

        public IEnumerable<Region> GetRegions()
        {
            IEnumerable<Region> regions;

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                regions = conn.Query<Region>(
                    "usp_GetRegions",
                    commandType: CommandType.StoredProcedure);
            }

            return regions;
        }
        public IEnumerable<Assessment> GetAssessmentsByOrganization(int organizationID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                var d = conn.Query<Assessment>(
                    "usp_GetAssessments",
                    new
                    {
                        OrganizationID = organizationID
                    },
                    commandType: CommandType.StoredProcedure);
                return d;
            }
        }

        public string GetAssessmentNote(int assessmentID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.QueryFirstOrDefault<string>(
                    "usp_GetAssessmentNote",
                    new
                    {
                        AssessmentID = assessmentID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public int? GetAssessmentRequestID(int assessmentID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                var d = conn.QueryFirstOrDefault<int?>(
                    "usp_GetAssessmentRequestID",
                    new
                    {
                        AssessmentID = assessmentID
                    },
                    commandType: CommandType.StoredProcedure);
                return d;
            }
        }
        public IEnumerable<RequestForAssessment> GetRequestsForAssessment(int organizationID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                var d = conn.Query<RequestForAssessment>(
                    "usp_GetAvailableRequests",
                    new
                    {
                        OrganizationID = organizationID
                    },
                    commandType: CommandType.StoredProcedure);
                return d;
            }
        }

        public IEnumerable<AssessmentScore> GetAssessmentDetail(int assessmentID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.Query<AssessmentScore>(
                    "usp_GetAssessment",
                    new
                    {
                        AssessmentID = assessmentID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<AssessmentCategory> GetAssessmentCategories()
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.Query<AssessmentCategory>(
                    "usp_GetAssessmentCategories",
                    commandType: CommandType.StoredProcedure);
            }
        }
        public int SaveAssessment(int assessmentID, int organizationID, int requestID, string notes, string user)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.ExecuteScalar<int>(
                    "usp_SaveAssessment",
                    new
                    {
                        AssessmentID = assessmentID,
                        OrganizationID = organizationID,
                        RequestID = requestID,
                        Notes = notes,
                        LastChangeUser = user
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void SaveAssessmentScores(int assessmentID, IEnumerable<ScoreEntry> scores, string user)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var score in scores)
                        {
                            conn.Execute(
                                "usp_SaveAssessmentScore",
                                new
                                {
                                    AssessmentID = assessmentID,
                                    score.AssessmentCategoryID,
                                    score.Score,
                                    ScoreNotes = score.Notes,
                                    LastChangeUser = user
                                },
                                transaction: tran,
                                commandType: CommandType.StoredProcedure);
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        public void DeleteAssessment(int assessmentID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Execute(
                    "usp_DeleteAssessment",
                    new
                    {
                        AssessmentID = assessmentID,
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        /*
        public int SaveAssessment(Assessment a)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.QueryFirstOrDefault<int>(
                    "usp_SaveAssessment",
                    new
                    {
                        a.AssessmentID,
                        a.OrganizationID,
                        a.Strategy,
                        a.Governance,
                        a.Leadership,
                        a.Donations,
                        a.Financials,
                        a.Impact,
                        a.Notes,
                        a.LastChangeUser
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
         */
        public IEnumerable<FinancialInformation> GetFinancialInformation(int OrganizationId)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.Query<FinancialInformation>(
                     "usp_GetFinancials",
                     new
                     {
                         OrganizationID = OrganizationId
                     },
                     commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<FinancialDocument> GetFinancialDocuments(int FinancialInformationID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.Query<FinancialDocument>(
                     "usp_GetFinancialDocuments",
                     new
                     {
                         FinancialInformationID
                     },
                     commandType: CommandType.StoredProcedure);
            }

        }
        public int SaveFinancialInformation(FinancialInformation fi)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.QueryFirstOrDefault<int>(
                    "usp_SaveFinancialInformation",
                    new
                    {
                        fi.FinancialInformationID,
                        fi.OrganizationID,
                        fi.FiscalYear,
                        fi.Revenue,
                        fi.Programs,
                        fi.Administration,
                        fi.Fundraising,
                        fi.Liabilities,
                        fi.TotalExpenses,
                        fi.Cash,
                        fi.DocumentID,
                        fi.Notes,
                        fi.LastChangeUser,
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public async Task SaveFinancialDocument(FinancialInformation fi, string user)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                await conn.ExecuteAsync(
                     "usp_SaveFinancialDocument",
                     new
                     {
                         fi.FinancialInformationID,
                         fi.FolderID,
                         fi.DocumentID,
                         fi.DocumentName,
                         LastChangeUser = user
                     },
                     commandType: CommandType.StoredProcedure);
            }

        }


        public IEnumerable<PartnerType> GetPartnerTypes()
        {
            IEnumerable<PartnerType> partnerTypes;

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                partnerTypes = conn.Query<PartnerType>(
                    "usp_GetPartnerTypes",
                    commandType: CommandType.StoredProcedure);
            }

            return partnerTypes;
        }

        public IEnumerable<ContactType> GetContactTypes()
        {
            IEnumerable<ContactType> contactTypes;

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                contactTypes = conn.Query<ContactType>(
                    "usp_GetContactTypes",
                    commandType: CommandType.StoredProcedure);
            }

            return contactTypes;
        }
        public IEnumerable<ProjectType> GetProjectTypes()
        {
            IEnumerable<ProjectType> projectTypes;

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                projectTypes = conn.Query<ProjectType>(
                    "usp_GetProjectTypes",
                    commandType: CommandType.StoredProcedure);
            }

            return projectTypes;
        }
        public IEnumerable<Country> GetCountries()
        {
            IEnumerable<Country> countries;

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                countries = conn.Query<Country>(
                    "usp_GetCountryList",
                    commandType: CommandType.StoredProcedure);
            }

            return countries;
        }
        public IEnumerable<Address> GetAddresses(int OrganizationID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.Query<Address>(
                    "usp_GetAddresses",
                    new
                    {
                        OrganizationID
                    },
                    commandType: CommandType.StoredProcedure); ;
            }
        }
        public IEnumerable<Contact> GetOrganizationContacts(int OrganizationId)
        {
            IEnumerable<Contact> contacts;

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                contacts = conn.Query<Contact>(
                    "usp_GetOrganizationContacts",
                    new
                    {
                        OrganizationId,
                    },
                    commandType: CommandType.StoredProcedure); ;
            }

            return contacts;
        }
        public IEnumerable<Request> GetOrganizationRequests(int OrganizationId)
        {
            IEnumerable<Request> requests;

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                requests = conn.Query<Request>(
                    "usp_GetOrganizationRequests",
                    new
                    {
                        OrganizationId,
                    },
                    commandType: CommandType.StoredProcedure); ;
            }

            return requests;
        }

        public async Task<IEnumerable<Request>> GetRequests()
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return await conn.QueryAsync<Request>(
                   "usp_GetRequests",
                   commandType: CommandType.StoredProcedure);
            }
        }
        public Request GetRequest(int RequestID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.QuerySingle<Request>(
                        "usp_GetRequest",
                        new
                        {
                            RequestID
                        },
                        commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<Gift> GetGifts(int RequestID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.Query<Gift>(
                    "usp_GetGifts",
                    new
                    {
                        RequestID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public Gift GetGift(int GiftID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.QuerySingle<Gift>(
                    "usp_GetGift",
                    new
                    {
                        GiftID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<GiftSchedule> GetGiftSchedules(int GiftID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.Query<GiftSchedule>(
                    "usp_GetGiftSchedules",
                    new
                    {
                        GiftID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<Condition> GetConditions(int GiftID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.Query<Condition>(
                    "usp_GetConditions",
                    new
                    {
                        GiftID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        // saves
        public int SaveOrganization(Organization organization, DateTime? PreviousChangeDttm)
        {
            int organizationId;
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                organizationId = conn.ExecuteScalar<int>(
                    "usp_SaveOrganization",
                    new
                    {
                        organization.OrganizationID,
                        organization.OrganizationName,
                        organization.CategoryID,
                        organization.CategoryOther,
                        organization.PartnerTypeID,
                        organization.CSFolderId,
                        organization.OrganizationNotes,
                        organization.LastChangeUser,
                        organization.LastChangeDttm,
                        PreviousChangeDttm,
                        organization.OrganizationURL,
                        organization.OrganizationLogo,
                        organization.VersionStamp
                    },
                    commandType: CommandType.StoredProcedure);
            }
            return organizationId;
        }
        public void SaveCategory(Category category)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                    "usp_SaveCategory",
                    new
                    {
                        category.CategoryId,
                        category.CategoryName
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<int> SaveRequest(Request request)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return await conn.QueryFirstOrDefaultAsync<int>(
                "usp_SaveRequest",
                    new
                    {
                        request.RequestID,
                        request.OrganizationID,
                        request.VoteDate,
                        request.RequestedAmount,
                        request.VoteTeamID,
                        request.ProjectTypeID,
                        request.ProjectTypeOther,
                        request.RequestStatusID,
                        request.Description,
                        request.Response,
                        request.OnePagerDocumentID,
                        request.LastChangeUser
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void DeleteGift(RequestDetail requestDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                "usp_DeleteGift",
                    new
                    {
                        requestDetail.Gift.GiftID,
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void DeleteContact(OrganizationDetail organizationDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                    "usp_DeleteContact",
                    new
                    {
                        organizationDetail.Contact.ContactID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void DeleteAddress(OrganizationDetail organizationDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                    "usp_DeleteAddress",
                    new
                    {
                        organizationDetail.Address.AddressID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void DeleteFinancialInformation(OrganizationDetail organizationDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                    "usp_DeleteFinancialInformation",
                    new
                    {
                        organizationDetail.FinancialInformation.FinancialInformationID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void DeleteAssessment(OrganizationDetail organizationDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                    "usp_DeleteAssessment",
                    new
                    {
                        organizationDetail.Assessment.AssessmentID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public int SaveGift(RequestDetail requestDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.QueryFirstOrDefault<int>(
                "usp_SaveGift",
                    new
                    {
                        requestDetail.Request.RequestID,
                        requestDetail.Gift.GiftID,
                        requestDetail.Gift.Conditional,
                        requestDetail.Gift.Recurring,
                        requestDetail.Gift.GiftAmount,
                        requestDetail.Gift.PaymentMemo,
                        requestDetail.Gift.GiftNote,
                        requestDetail.Request.LastChangeUser
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public int SaveGift(GiftDetail giftDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.QueryFirstOrDefault<int>(
                "usp_SaveGift",
                    new
                    {
                        giftDetail.Gift.RequestID,
                        giftDetail.Gift.GiftID,
                        giftDetail.Gift.Conditional,
                        giftDetail.Gift.Recurring,
                        giftDetail.Gift.GiftAmount,
                        giftDetail.Gift.PaymentMemo,
                        giftDetail.Gift.GiftNote,
                        giftDetail.Gift.LastChangeUser
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void DeleteGiftSchedule(GiftDetail giftDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                "usp_DeleteGiftSchedule",
                    new
                    {
                        giftDetail.GiftSchedule.GiftScheduleID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void DeleteCondition(GiftDetail giftDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                "usp_DeleteCondition",
                    new
                    {
                        giftDetail.Gift.ConditionID
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void GenerateSchedules(GiftDetail giftDetail)
        {
            var FrequencyType = Enum.GetName(typeof(Gift.Frequency), giftDetail.Gift.FrequencyType);
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                "usp_GenerateSchedules",
                    new
                    {
                        giftDetail.Gift.GiftID,
                        FrequencyType,
                        giftDetail.Gift.SelectedFrequency,
                        giftDetail.Gift.GiftStartDate,
                        giftDetail.GiftSchedule.InitialLumpSum,
                        giftDetail.Gift.LastChangeUser
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void SaveGiftSchedule(GiftDetail giftDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                "usp_SaveGiftSchedule",
                    new
                    {
                        giftDetail.Gift.GiftID,
                        giftDetail.GiftSchedule.GiftScheduleID,
                        giftDetail.GiftSchedule.DisbursementDate,
                        giftDetail.GiftSchedule.Amount,
                        giftDetail.GiftSchedule.IncludePaymentInfo,
                        giftDetail.Gift.Rebalance,
                        giftDetail.Gift.LastChangeUser
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void SaveCondition(GiftDetail giftDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                "usp_SaveCondition",
                    new
                    {
                        giftDetail.Gift.GiftID,
                        giftDetail.Gift.ConditionID,
                        giftDetail.Gift.ConditionDescription,
                        giftDetail.Gift.ConditionDeadline,
                        giftDetail.Gift.ConditionCompleted,
                        giftDetail.Gift.LastChangeUser
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void SaveRegion(Region region)
        {

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                    "usp_SaveRegion",
                    new
                    {
                        region.RegionId,
                        region.RegionName,
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void SaveOrganizationRegions(int organizationID, string regionID)
        {

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                    "usp_SaveOrganizationRegions",
                    new
                    {
                        organizationID,
                        regionID,
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void SavePartnerType(PartnerType partnerType)
        {

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                    "usp_SavePartnerType",
                    new
                    {
                        partnerType.PartnerTypeId,
                        partnerType.PartnerTypeName,
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void SaveContactType(ContactType contactType)
        {

            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                    "usp_SaveContactType",
                    new
                    {
                        contactType.ContactTypeId,
                        contactType.ContactTypeName,
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public void SaveProjectType(ProjectType projectType)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                conn.Query(
                    "usp_SaveProjectType",
                    new
                    {
                        projectType.ProjectTypeId,
                        projectType.ProjectTypeName,
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public int SaveContact(Contact contact)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.QueryFirstOrDefault<int>(
                    "usp_SaveContact",
                    new
                    {
                        contact.ContactID,
                        contact.OrganizationID,
                        contact.ContactName,
                        contact.Email,
                        contact.Cell,
                        contact.Office,
                        contact.ContactTypeID,
                        contact.ContactNotes,
                        contact.LastChangeUser,
                        contact.LastChangeDttm
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public int SaveAddress(Address address)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.QueryFirstOrDefault<int>(
                    "usp_SaveAddress",
                    new
                    {
                        address.OrganizationID,
                        address.AddressID,
                        address.Address1,
                        address.Address2,
                        address.AttnLine,
                        address.City,
                        address.State,
                        address.CountryID,
                        address.ZipCode,
                        address.Active,
                        address.LastChangeUser,
                        address.LastChangeDttm
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }
        // Search
        public IEnumerable<T> GetSearchResults<T>(SearchCriteria searchCriteria)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                return conn.Query<T>(
                    "usp_Search",
                         new
                         {
                             searchCriteria.SearchType,
                             UserOrganizationNamesTable = searchCriteria.SearchOrganizationNamesTable.AsTableValuedParameter(),
                             UserCategoriesTable = searchCriteria.SearchCategoriesTable.AsTableValuedParameter(),
                             UserCountryCDsTable = searchCriteria.SearchCountryCDsTable.AsTableValuedParameter(),
                             UserOrganizationRegionsTable = searchCriteria.SearchOrganizationRegionsTable.AsTableValuedParameter(),
                             UserRequestStatusesTable = searchCriteria.SearchRequestStatusesTable.AsTableValuedParameter(),
                             UserVoteTeamTable = searchCriteria.SearchVoteTeamTable.AsTableValuedParameter(),
                             UserContactNamesTable = searchCriteria.SearchContactNamesTable.AsTableValuedParameter(),
                             UserContactTypesIDTable = searchCriteria.SearchContactTypesIDTable.AsTableValuedParameter(),
                             UserPartnerTypesTable = searchCriteria.SearchPartnerTypesTable.AsTableValuedParameter(),
                             UserProjectTypesTable = searchCriteria.SearchProjectTypesTable.AsTableValuedParameter(),
                             UserFlagsTable = searchCriteria.SearchFlagsTable.AsTableValuedParameter(),
                             searchCriteria.BeginDate,
                             searchCriteria.EndDate,
                             LowerDonationAmount = searchCriteria.LowerDonationAmount.HasValue ? searchCriteria.LowerDonationAmount.Value : 0,
                             UpperDonationAmount = searchCriteria.UpperDonationAmount.HasValue ? searchCriteria.UpperDonationAmount.Value : 0,
                         },
                    commandType: CommandType.StoredProcedure);

            }
        }
        public void DeleteOrganizationDetail(OrganizationDetail organizationDetail)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {
                if (organizationDetail.SaveContact)
                {
                    conn.Query(
                       "usp_DeleteContact",
                       new
                       {
                           organizationDetail.Contact.ContactID
                       },
                       commandType: CommandType.StoredProcedure);
                }
                else if (organizationDetail.SaveRequest)
                {
                    conn.Query(
                       "usp_DeleteRequest",
                       new
                       {
                           organizationDetail.Request.RequestID
                       },
                       commandType: CommandType.StoredProcedure); ;
                }
            }
        }
        public void DeleteRegion(int regionID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {

                conn.Query(
                   "usp_DeleteRegion",
                   new
                   {
                       regionID
                   },
                   commandType: CommandType.StoredProcedure);

            }
        }
        public void DeleteCategory(int CategoryID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {

                conn.Query(
                   "usp_DeleteCategory",
                   new
                   {
                       CategoryID
                   },
                   commandType: CommandType.StoredProcedure);

            }
        }
        public void DeletePartnerType(int PartnerTypeID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {

                conn.Query(
                   "usp_DeletePartnerType",
                   new
                   {
                       PartnerTypeID
                   },
                   commandType: CommandType.StoredProcedure);

            }
        }
        public void DeleteContactType(int ContactTypeID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {

                conn.Query(
                   "usp_DeleteContactType",
                   new
                   {
                       ContactTypeID
                   },
                   commandType: CommandType.StoredProcedure);

            }
        }
        public void DeleteProjectType(int ProjectTypeID)
        {
            using (DbConnection conn = new SqlConnection(_ministryInvestmentConfig.DataAccessMinistryInvestment))
            {

                conn.Query(
                   "usp_DeleteProjectType",
                   new
                   {
                       ProjectTypeID
                   },
                   commandType: CommandType.StoredProcedure);

            }
        }
    }
}