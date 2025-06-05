namespace VoiceChatMVC.Models
{
    public class CallLog
    {
        public int Id { get; set; }
        public string Caller { get; set; } = string.Empty;
        public string Receiver { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}