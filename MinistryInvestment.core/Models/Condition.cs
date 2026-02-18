using System;
using System.ComponentModel.DataAnnotations;
namespace MinistryInvestment.Core.Models
{
    public class Condition
    {
        [Required(ErrorMessage = "ConditionID is required")]
        public int ConditionID { get; set; }
        [Required(ErrorMessage = "GiftID is required")]
        public int GiftID { get; set; }
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Display(Name = "Deadline")]
        public DateTime? Deadline { get; set; }
        [Display(Name = "Completed")]
        public bool Completed { get; set; }
    }
}
