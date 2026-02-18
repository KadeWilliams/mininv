using System.ComponentModel.DataAnnotations;

namespace MinistryInvestment.Core.Models
{
    public class Contact
    {
        [Required(ErrorMessage = "Contact ID is required")]
        public int ContactID { get; set; }


        [Required(ErrorMessage = "Organization ID is required")]
        public int OrganizationID { get; set; }


        [Display(Name = "Contact Name"), Required()]
        public string ContactName { get; set; }


        [Display(Name = "Email")]
        public string? Email { get; set; }


        [Display(Name = "Cell")]
        public string? Cell { get; set; }


        [Display(Name = "Office")]
        public string? Office { get; set; }

        [Display(Name = "Contact Type"), Required()]
        public int ContactTypeID { get; set; }
        public string ContactTypeName { get; set; }

        [Display(Name = "Contact Notes")]
        public string? ContactNotes { get; set; }

        [Display(Name = "LastChangeUser: ")]
        public string LastChangeUser { get; set; }

        [Display(Name = "LastChangeDttm: ")]
        public DateTime LastChangeDttm { get; set; }
    }
}