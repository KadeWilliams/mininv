using System.ComponentModel.DataAnnotations;

namespace MinistryInvestment.Core.Models
{
    public class Request
    {
        [Required(ErrorMessage = "RequestID is required")]
        public int RequestID { get; set; }


        [Display(Name = "Organization"), Required()]
        public int OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public byte[]? OrganizationLogo { get; set; }

        [Display(Name = "Year"), Required()]
        public int Year { get; set; }


        [Display(Name = "Month"), Required()]
        public int Month { get; set; }

        [Display(Name = "Vote Team"), Required()]
        public int VoteTeamID { get; set; }

        [Display(Name = "Vote Date"), Required()]
        public DateTime VoteDate { get; set; }
        [Display(Name = "Requested Amount"), Required(), Range(0, double.MaxValue), DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public double RequestedAmount { get; set; }
        public DateTime? OldestDisbursementDate { get; set; }
        public DateTime? LatestDisbursementDate { get; set; }
        public string? DisbursementRange
        {
            get
            {
                var oldestDate = OldestDisbursementDate.HasValue ? OldestDisbursementDate.Value.Month.ToString() + "/" + OldestDisbursementDate.Value.Year.ToString() : null;
                var latestDate = LatestDisbursementDate.HasValue ? LatestDisbursementDate.Value.Month.ToString() + "/" + LatestDisbursementDate.Value.Year.ToString() : null;
                if (oldestDate == null && latestDate == null)
                {
                    return null;
                }
                else if (oldestDate != null && latestDate == null)
                {
                    return oldestDate;
                }
                else if (oldestDate == latestDate)
                {
                    return oldestDate;
                }
                return oldestDate + "-" + latestDate;
            }
        }


        [Display(Name = "Project Type"), Required()]
        public int ProjectTypeID { get; set; }
        [Display(Name = "Project Type Name")]
        public string ProjectTypeName { get; }

        [Display(Name = "Project Type Other"), Required()]
        public string? ProjectTypeOther { get; set; }

        [Display(Name = "Status"), Required()]
        public int RequestStatusID { get; set; }


        [Display(Name = "Description"), Required()]
        public string Description { get; set; }


        [Display(Name = "Response")]
        public string? Response { get; set; }
        public string VoteTeamName { get; set; }
        public int ApprovedGiftAmount { get; set; }
        public IEnumerable<Gift>? Gifts { get; set; }

        [Display(Name = "LastChangeUser: ")]
        public string LastChangeUser { get; set; }
        [Display(Name = "LastChangeDttm: ")]
        public DateTime LastChangeDttm { get; set; }
        public int GiftCount { get; set; }
        public DateTime MaxDisbursementDate { get; set; }
        public double RemainingGiftAmount { get; set; }
        public int OnePagerDocumentID { get; set; }
        public int CSFolderId { get; set; }
        public int PartnerTypeID { get; set; }
        public byte[]? OnePagerDocument { get; set; }
    }
}