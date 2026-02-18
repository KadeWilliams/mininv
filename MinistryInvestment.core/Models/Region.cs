using System.ComponentModel.DataAnnotations;

namespace MinistryInvestment.Core.Models
{
    public class Region
    {
        public int RegionId { get; set; }

        [Display(Name = "Region Name"), Required()]
        public string RegionName { get; set; }
    }
}