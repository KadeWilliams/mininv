namespace MinistryInvestment.Core.Models
{
    public class AssessmentScore
    {
        public int AssessmentScoreID { get; set; }
        public int AssessmentID { get; set; }
        public int AssessmentCategoryID { get; set; }
        public decimal Score { get; set; }
        public string? Notes { get; set; }
        public string LastChangeUser { get; set; }
        public DateTime? LastChangeDttm { get; set; }

        public string Header { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Weight { get; set; }
    }
}
