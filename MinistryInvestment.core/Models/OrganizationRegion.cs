using System.ComponentModel.DataAnnotations;

namespace MinistryInvestment.Core.Models
{
    [Display(Name = "Regions")]
    public class OrganizationRegion
    {
        [Required(ErrorMessage = "OrganizationID is required")]
        public int OrganizationID { get; set; }


        [Required(ErrorMessage = "RegionID is required")]
        public int RegionID { get; set; }
    }
}