namespace MinistryInvestment.Core.Models
{
    public class DocumentManagementReturn
    {
        public int ReturnCode { get; set; } = 0;
        public int FolderID { get; set; } = 0;
        public int? DocumentID { get; set; } = 0;
        public string Message { get; set; } = "Failed to find or create folders";
    }
}