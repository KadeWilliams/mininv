using System;
using System.ComponentModel.DataAnnotations;

namespace MinistryInvestment.Core.Models
{
    public class Address
    {
        [Required(ErrorMessage = "AddressID is required")]
        public int? AddressID { get; set; }
        public int OrganizationID { get; set; }
        [Display(Name = "Attn Line")]
        public string? AttnLine { get; set; }
        [Display(Name = "Address 1"), Required()]
        public string? Address1 { get; set; }
        [Display(Name = "Address 2")]
        public string? Address2 { get; set; }
        [Display(Name = "City")]
        public string? City { get; set; }
        [Display(Name = "State")]
        public string? State { get; set; }
        [Display(Name = "Zip Code")]
        public string? ZipCode { get; set; }
        [Display(Name = "Country"), Required()]
        public int? CountryID { get; set; }
        public string? CountryName { get; set; }
        [Display(Name = "Active")]
        public bool Active { get; set; }
        [Display(Name = "Last Change User")]
        public string LastChangeUser { get; set; }
        [Display(Name = "Last Change Dttm")]
        public DateTime? LastChangeDttm { get; set; }
    }
}
