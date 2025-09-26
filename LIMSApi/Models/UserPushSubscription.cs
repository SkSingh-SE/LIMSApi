namespace LIMSApi.Models
{
    public class UserPushSubscription
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public string P256DH { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
    }

}
