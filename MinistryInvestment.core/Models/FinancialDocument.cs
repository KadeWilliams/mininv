namespace MinistryInvestment.Core.Models
{
    public class FinancialDocument
    {
        public int FinancialDocumentID { get; set; }
        public int FinancialInformationID { get; set; }
        public int FolderID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string LastChangeUser { get; set; } = string.Empty;
        public DateTime LastChangeDttm { get; set; }
        public string Link { get; set; } = string.Empty;
    }
}
