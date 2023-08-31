namespace UserFeedbackApp.Models
{
    public class AIModel
    {
       public List<string> review { get; set; }

       public AIModel() 
       { 
            review = new List<string>();
       }
    }
}
