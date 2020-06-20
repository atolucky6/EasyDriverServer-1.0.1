namespace EasyDriver.Client.Models
{
    public class WriteResponse
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
        public WriteCommand WriteCommand { get; set; }
    }
}