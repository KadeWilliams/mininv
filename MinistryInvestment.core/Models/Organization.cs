using System.ComponentModel.DataAnnotations;
namespace MinistryInvestment.Core.Models
{
    public class Organization
    {
        [Display(Name = "Organization"), Required()]
        public int OrganizationID { get; set; }

        [Display(Name = "Organization Name"), Required()]
        public string OrganizationName { get; set; }

        [Display(Name = "Category"), Required()]
        public int CategoryID { get; set; }

        [Display(Name = "Category Other"), Required()]
        public string CategoryOther { get; set; }

        [Display(Name = "Partner Type"), Required()]
        public int PartnerTypeID { get; set; }

        public int CSFolderId { get; set; }

        [Display(Name = "Address line 1")]
        public string? Address1 { get; set; }

        [Display(Name = "Address line 2")]
        public string? Address2 { get; set; }

        [Display(Name = "City")]
        public string? City { get; set; }

        [Display(Name = "State")]
        public string? State { get; set; }

        [Display(Name = "Country")]
        public string CountryName { get; set; }

        [Display(Name = "Zip Code")]
        public string? ZipCode { get; set; }


        [Display(Name = "Notes")]
        public string? OrganizationNotes { get; set; }

        [Display(Name = "Total Gifts"), DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TotalGifts { get; set; }

        [Display(Name = "LastChangeUser: ")]
        public string LastChangeUser { get; set; }
        [Display(Name = "LastChangeDttm: ")]
        public DateTime LastChangeDttm { get; set; }

        [Display(Name = "Organization Link ")]
        public string? OrganizationURL { get; set; }
        [Display(Name = "Upload Logo")]
        public byte[]? OrganizationLogo { get; set; }
        public string CategoryName { get; }
        public byte[] VersionStamp { get; set; }
    }
}