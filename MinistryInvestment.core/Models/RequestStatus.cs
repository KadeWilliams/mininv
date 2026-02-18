using System.ComponentModel.DataAnnotations;

namespace MinistryInvestment.Core.Models
{
    public class RequestStatus
    {
        [Required(ErrorMessage = "RequestStatusID is required")]
        public int RequestStatusID { get; set; }


        [Display(Name = "Request Status Name")]
        [Required(ErrorMessage = "RequestStatusName is required")]
        public string RequestStatusName { get; set; }
    }
}