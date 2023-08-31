namespace UserFeedbackApp.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ReviewText { get; set; }
        public string PostDate { get; set; }

        public string Sentiment { get; set; }
        public float PositiveValue { get; set; }
        public float NeutralValue { get; set; }
        public float NegativeValue { get; set; }
    }
}
