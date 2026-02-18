using System.ComponentModel.DataAnnotations;


namespace MinistryInvestment.Core.Models
{
    public class Country
    {
        public int CountryID { get; set; }
        public string CountryCD { get; set; }
        [Display(Name = "Country Name"), Required()]
        public string CountryName { get; set; }
    }
}