using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinistryInvestment.Core.Models
{
    public class RequestDetail
    {
        public Request Request { get; set; } = new Request();
        public IEnumerable<Gift> Gifts { get; set; } = new List<Gift>();
        public Gift? Gift { get; set; } = new Gift();
        public bool SaveRequest { get; set; }
        public bool SaveGift { get; set; }
        public bool DeleteGift { get; set; }
    }
}
