namespace MyTvTime.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public string StatusCode { get; set; }
        public string AdditionalInfo { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public bool ShowStatusCode => !string.IsNullOrEmpty(StatusCode);
        public bool ShowAdditionalInfo => !string.IsNullOrEmpty(AdditionalInfo);
    }
}