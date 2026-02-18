using System;
using System.ComponentModel.DataAnnotations;
namespace MinistryInvestment.Core.Models
{
    public class GiftSchedule
    {
        [Required(ErrorMessage = "GiftScheduleID is required")]
        public int GiftScheduleID { get; set; }
        [Required(ErrorMessage = "GiftID is required")]
        public int GiftID { get; set; }
        [Display(Name = "Disbursement Date"), Required()]
        public DateTime? DisbursementDate { get; set; }
        [Display(Name = "Amount"), Required(), DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double Amount { get; set; }
        public double PreviousAmount { get; set; }
        [Display(Name = "Include Payment Info")]
        public bool IncludePaymentInfo { get; set; }
        [Display(Name = "Initial Lump Sum"), DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double InitialLumpSum { get; set; }
    }
}
