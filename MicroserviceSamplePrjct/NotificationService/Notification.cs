namespace NotificationService
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int OrderId { get; set; }
        public string Message { get; set; }
        public string RecipientEmail { get; set; }
    }
}
