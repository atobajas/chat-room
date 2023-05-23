namespace Chat.Application.Models
{
    public class Message
    {
        public DateTime CreatedOn { get; init; }
        public string CreatedBy { get; init; } = string.Empty;
        public string Text { get; init; } = string.Empty;
        public string FormattedText =>
            $"[{CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")}] {CreatedBy} says: {Text}";
    }
}
