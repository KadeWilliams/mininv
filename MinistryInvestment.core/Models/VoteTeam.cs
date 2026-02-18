using System;
using System.ComponentModel.DataAnnotations;

namespace MinistryInvestment.Core.Models
{
    public class VoteTeam
    {
        [Required(ErrorMessage = "VoteTeamID is required")]
        public int VoteTeamID { get; set; }


        [Display(Name = "Vote Team Name")]
        [Required(ErrorMessage = "VoteTeamName is required")]
        public string VoteTeamName { get; set; }
    }
}
