using System.ComponentModel.DataAnnotations;
namespace MinistryInvestment.Core.Models
{
    public class Gift
    {
        public int GiftID { get; set; }
        public int RequestID { get; set; }
        [Display(Name = "Conditional")]
        public bool Conditional { get; set; } = false;
        [Display(Name = "Recurring")]
        public bool Recurring { get; set; } = false;
        [Display(Name = "Gift Amount"), Required(), Range(1, double.MaxValue), DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double GiftAmount { get; set; }
        [Display(Name = "Remaining Gift Amount"), DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double RemainingGiftAmount { get; set; }
        [Display(Name = "Payment Memo")]
        public string? PaymentMemo { get; set; }
        [Display(Name = "Gift Note")]
        public string? GiftNote { get; set; }
        public string LastChangeUser { get; set; }
        public DateTime? LastChangeDttm { get; set; }
        public IEnumerable<GiftSchedule>? GiftSchedules { get; set; } = new List<GiftSchedule>();
        public IEnumerable<Condition>? Conditions { get; set; } = new List<Condition>();
        public int OrganizationID { get; set; }
        [Display(Name = "Frequency")]
        public int FrequencyType { get; set; }
        public enum Frequency
        {
            Months,
            Quarters,
        }
        [Range(1, 24)]
        public int SelectedFrequency { get; set; }
        [Display(Name = "Gift Start Date")]
        public DateTime GiftStartDate { get; set; }
        public int GiftScheduleCount { get; set; }
        public int ConditionID { get; set; }
        [Display(Name = "Description")]
        public string ConditionDescription { get; set; }
        [Display(Name = "Deadline")]
        public DateTime? ConditionDeadline { get; set; }
        [Display(Name = "Completed")]
        public bool ConditionCompleted { get; set; }
        public bool Rebalance { get; set; } = false;
        public DateTime VoteDate { get; set; }
        public DateTime? NextDisbursement { get; set; }
    }
}
