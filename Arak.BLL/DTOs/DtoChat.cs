namespace Arak.BLL.DTOs
{
    public class ChatRequest
    {
        public string Message { get; set; }
        public string ConversationId { get; set; }
    }

    public class ChatResponse
    {
        public string Reply { get; set; }
        public string Intent { get; set; }
        public double Confidence { get; set; }
    }
}
