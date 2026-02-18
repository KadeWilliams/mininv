namespace MinistryInvestment.Core.Models
{
    public class Assessment
    {
        public int AssessmentID { get; set; }
        public int OrganizationID { get; set; }
        public int RequestID { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string? Notes { get; set; }
        public string LastChangeUser { get; set; }
        public DateTime LastChangeDttm { get; set; }

        public DateTime VoteDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public string ProjectTypeName { get; set; }
        public string RequestStatusName { get; set; }
        public decimal Score { get; set; }
        public IEnumerable<AssessmentScore>? Scores { get; set; }
    }
}
