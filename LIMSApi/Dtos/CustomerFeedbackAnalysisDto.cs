namespace LIMSApi.Dtos
{
    public class CustomerFeedbackAnalysisDto
    {
        public long CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CompanyName { get; set; }

        public string? Address { get; set; }
        public string? ContactPerson { get; set; }

        public string? Designation { get; set; }
        public DateTime? FeedbackDate { get; set; }

        public string? Email { get; set; }

        public string? MobileNo { get; set; }
        public string? Suggestions{ get; set; }
        public string? NewRequirement{ get; set; }
        public decimal AverageRating { get; set; }

        public List<FeedbackRatingDto> Ratings { get; set; }
    }
   
    public class FeedbackRatingDto
    {
        public string Parameter { get; set; }
        public int? Rating { get; set; }
    }
}
