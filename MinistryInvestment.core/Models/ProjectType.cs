using System.ComponentModel.DataAnnotations;

namespace MinistryInvestment.Core.Models
{
    public class ProjectType
    {
        public int ProjectTypeId { get; set; }

        [Display(Name = "Project Type"), Required()]
        public string ProjectTypeName { get; set; }
    }
}