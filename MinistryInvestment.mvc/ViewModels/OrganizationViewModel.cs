using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Mvc.Security;
using System.Collections.Generic;

namespace MinistryInvestment.Mvc.ViewModels
{
    public class OrganizationViewModel : BaseViewModel
    {
        private readonly OrganizationDetail _organizationDetails;
        private readonly IEnumerable<Category> _categories;
        private readonly IEnumerable<RequestStatus> _requestStatuses;
        private readonly IEnumerable<Country> _countries;
        private readonly IEnumerable<PartnerType> _partnerTypes;
        private readonly IEnumerable<Contact> _contacts;
        private readonly IEnumerable<Request> _requests;
        private readonly IEnumerable<ContactType> _contactTypes;
        private readonly IEnumerable<ProjectType> _projectTypes;
        private readonly IEnumerable<Region> _regions;
        private readonly string _contentServerFolderURL;
        private readonly int _CategoryOtherID;
        private readonly int _ProjectTypeOtherID;
        private readonly int _RequestID;
        private readonly int _GiftID;
        private readonly int _AddressID;
        private readonly int _ContactID;
        private readonly int _FinancialInformationID;
        private readonly bool _saveConfict;
        private readonly string _message;

        public OrganizationViewModel
        (
            IMinistryInvestmentConfig ministryInvestmentConfig,
            OrganizationDetail organizationDetail,
            IEnumerable<Category> categories,
            IEnumerable<PartnerType> partnerTypes,
            IEnumerable<Country> countries,
            IEnumerable<ContactType> contactTypes,
            IEnumerable<ProjectType> projectTypes,
            IEnumerable<Region> regions,
            IEnumerable<RequestStatus> requestStatuses,
            string contentServerFolderURL,
            int categoryOtherID,
            int projectTypeOtherID,
            int requestID,
            int giftID,
            int addressID,
            int contactID,
            int financialInformationID,
            MenuPermissions menuPermissions,
            IEnumerable<VoteTeam> voteTeams,
            string message = null
        )
            : base(ministryInvestmentConfig, categories, requestStatuses, countries, regions, partnerTypes, contactTypes, projectTypes, menuPermissions: menuPermissions, voteTeams: voteTeams)
        {
            _organizationDetails = organizationDetail;
            _categories = categories;
            _partnerTypes = partnerTypes;
            _countries = countries;
            _contactTypes = contactTypes;
            _requestStatuses = requestStatuses;
            _regions = regions;
            _contentServerFolderURL = contentServerFolderURL;
            _CategoryOtherID = categoryOtherID;
            _ProjectTypeOtherID = projectTypeOtherID;
            _RequestID = requestID;
            _GiftID = giftID;
            _AddressID = addressID;
            _ContactID = contactID;
            _FinancialInformationID = financialInformationID;
            _projectTypes = projectTypes;
            _message = message;
        }
        public OrganizationDetail OrganizationDetails { get { return _organizationDetails; } }
        public IEnumerable<Category> Categories { get { return _categories; } }
        public IEnumerable<Country> Countrys { get { return _countries; } }
        public IEnumerable<PartnerType> PartnerTypes { get { return _partnerTypes; } }
        public IEnumerable<Contact> Contacts { get { return _contacts; } }
        public IEnumerable<Request> Requests { get { return _requests; } }
        public IEnumerable<ContactType> ContactTypes { get { return _contactTypes; } }
        public IEnumerable<ProjectType> ProjectType { get { return _projectTypes; } }
        public IEnumerable<RequestStatus> RequestStatuses { get { return _requestStatuses; } }
        public IEnumerable<Region> Regions { get { return _regions; } }
        public string ContentServerFolderURL { get { return _contentServerFolderURL; } }
        public int CategoryOtherID { get { return _CategoryOtherID; } }
        public int ProjectTypeOtherID { get { return _ProjectTypeOtherID; } }
        public int RequestID { get { return _RequestID; } }
        public int GiftID { get { return _GiftID; } }
        public int AddressID => _AddressID;
        public int ContactID => _ContactID;
        public int FinancialInformationID => _FinancialInformationID;
        public string Message { get { return _message; } }
    }
}