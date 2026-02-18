//using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace MinistryInvestment.Core.Models
{
    public class OrganizationDetail
    {
        public Organization Organization { get; set; }
        [Display(Name = "Regions")]
        public string[]? OrganizationRegions { get; set; }
        [Display(Name = "Date"), Required()]
        public string RequestDate { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
        public IEnumerable<Request> Requests { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<FinancialInformation> Financials { get; set; }
        public IEnumerable<Assessment> Assessments { get; set; } // for viewing? 
        public Contact? Contact { get; set; }
        public Request? Request { get; set; }
        public Address? Address { get; set; }
        public FinancialInformation? FinancialInformation { get; set; }
        public Assessment? Assessment { get; set; }
        public int RequestID { get; set; }
        public int GiftID { get; set; }
        public bool SaveContact { get; set; }
        public bool SaveRequest { get; set; }
        public bool SaveFinancialInformation { get; set; }
        public bool SaveAssessment { get; set; }
        public bool ForDeletion { get; set; }
        public bool ContentServer { get; set; }
        public bool SaveConflict { get; set; } = false;
        public bool SaveAddress { get; set; }
        public bool DeleteAddress { get; set; }
        public bool DeleteContact { get; set; }
        public bool DeleteFinancialInformation { get; set; }
        public bool DeleteAssessment { get; set; }
        public string? FinancialDocumentPath { get; set; }
        public string FinancialDocumentType { get; set; } = "application/pdf";
        public void dateStringToInt()
        {
            if (RequestDate != null)
            {
                string[] token = RequestDate.Split('/');
                Request.VoteDate = DateTime.Parse($"{token[0]}/{token[1]}");
            }
        }
    }
}