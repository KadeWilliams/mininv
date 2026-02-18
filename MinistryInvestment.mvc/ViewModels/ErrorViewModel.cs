namespace MinistryInvestment.Mvc.ViewModels
{
    public class ErrorViewModel
    {
        public ErrorViewModel(string requestId)
        {
            RequestId = requestId;
        }

        public string RequestId { get; private set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}