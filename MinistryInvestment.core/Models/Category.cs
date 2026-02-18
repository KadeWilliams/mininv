using System.ComponentModel.DataAnnotations;


namespace MinistryInvestment.Core.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Display(Name = "Category Name"), Required()]
        public string CategoryName { get; set; }
    }
}