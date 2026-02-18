using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Mvc.Security;
using System.Collections;
using System.Collections.Generic;

namespace MinistryInvestment.Mvc.ViewModels
{
    public class RequestPartialViewModel : BaseViewModel
    {
        private readonly RequestDetail _requestDetail;

        public RequestPartialViewModel
            (
                IMinistryInvestmentConfig ministryInvestmentConfig,
                RequestDetail requestDetail,
                IEnumerable<RequestStatus> requestStatuses,
                IEnumerable<ProjectType> projectTypes,
                IEnumerable<VoteTeam> voteTeams,
                MenuPermissions menuPermissions
            ) : base(ministryInvestmentConfig, requestStatuses: requestStatuses, projectTypes: projectTypes, voteTeams: voteTeams, menuPermissions: menuPermissions)
        {
            _requestDetail = requestDetail;
        }

        public RequestDetail RequestDetail => _requestDetail;
    }
}
