using System.ComponentModel.DataAnnotations;

namespace MinistryInvestment.Core.Models
{
    public class ContactType
    {
        public int ContactTypeId { get; set; }

        [Display(Name = "Contact Type"), Required()]
        public string ContactTypeName { get; set; }
    }
}