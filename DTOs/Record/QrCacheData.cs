namespace Together.DTOs.Record
{
    public class QrCacheData
    {
        public int ProjectId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string ActionType { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}
