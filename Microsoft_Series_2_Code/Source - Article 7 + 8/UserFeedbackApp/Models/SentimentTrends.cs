namespace UserFeedbackApp.Models
{
    public class SentimentTrends
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public float PositiveValue { get; set; }
        public float NeutralValue { get; set; }
        public float NegativeValue { get; set; }
    }
}
