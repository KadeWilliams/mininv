using MinistryInvestment.Core.Configuration;
using MinistryInvestment.Core.Models;
using MinistryInvestment.Mvc.Security;
using System.Collections;
using System.Collections.Generic;

namespace MinistryInvestment.Mvc.ViewModels
{
    public class GiftPartialViewModel : BaseViewModel
    {
        private readonly GiftDetail _giftDetail;
        private readonly int _requestID;

        public GiftPartialViewModel
            (
                IMinistryInvestmentConfig ministryInvestmentConfig,
                GiftDetail giftDetail,
                MenuPermissions menuPermissions,
                int requestID = 0
            ) : base(ministryInvestmentConfig, menuPermissions: menuPermissions)
        {
            _giftDetail = giftDetail;
            _requestID = requestID;
        }

        public GiftDetail GiftDetail => _giftDetail;
        public int RequestID => _requestID;
    }
}
