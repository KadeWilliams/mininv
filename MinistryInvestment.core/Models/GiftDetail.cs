using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinistryInvestment.Core.Models
{
    public class GiftDetail
    {
        public Gift Gift { get; set; } = new Gift();
        public IEnumerable<GiftSchedule> GiftSchedules { get; set; } = new List<GiftSchedule>();
        public IEnumerable<Condition> Conditions { get; set; } = new List<Condition>();
        public GiftSchedule? GiftSchedule { get; set; } = new GiftSchedule();
        public Condition? Condition { get; set; } = new Condition();
        public bool SaveGift { get; set; }
        public bool SaveGiftSchedule { get; set; }
        public bool SaveCondition { get; set; }
        public bool GenerateSchedules { get; set; }
        public bool DeleteGiftSchedule { get; set; }
        public bool DeleteCondition { get; set; }
        public int RequestID { get; set; }
    }
}
