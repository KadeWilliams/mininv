using System.ComponentModel.DataAnnotations;

namespace MinistryInvestment.Core.Models
{
    public class PartnerType
    {
        public int PartnerTypeId { get; set; }

        [Display(Name = "Partner Type"), Required(ErrorMessage = "Partner Type is required")]
        public string PartnerTypeName { get; set; }
    }
}