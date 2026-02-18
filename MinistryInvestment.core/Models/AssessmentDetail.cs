namespace MinistryInvestment.Core.Models
{
    public class AssessmentDetail
    {
        public int AssessmentID { get; set; }
        public int OrganizationID { get; set; }
        public int RequestID { get; set; }
        public string? Notes { get; set; }
        public string LastChangeUser { get; set; }
        public DateTime? LastChangeDttm { get; set; }

        public IEnumerable<AssessmentHeaderGroup> HeaderGroups { get; set; }
        public IEnumerable<RequestForAssessment> Requests { get; set; }
        public int? SelectedRequestID { get; set; }
    }

    public class RequestForAssessment
    {
        public int RequestID { get; set; }
        public DateTime VoteDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public string ProjectTypeName { get; set; }
        public string RequestStatusName { get; set; }
        public bool HasAssessment { get; set; }
        public int? AssessmentID { get; set; }
    }

    public class AssessmentHeaderGroup
    {
        public string Header { get; set; }
        public IEnumerable<AssessmentScore> Questions { get; set; }
        public decimal? HeaderScore { get; set; }
    }

    public class AssessmentScoreSubmission
    {
        public int AssessmentID { get; set; }
        public int OrganizationID { get; set; }
        public int RequestID { get; set; }
        public string Notes { get; set; }
        public List<ScoreEntry> Scores { get; set; }
    }

    public class ScoreEntry
    {
        public int AssessmentCategoryID { get; set; }
        public decimal Score { get; set; }
        public string Notes { get; set; }
    }
}
