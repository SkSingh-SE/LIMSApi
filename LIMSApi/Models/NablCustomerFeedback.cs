using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    [Table("NablCustomerFeedbacks")]
    public class NablCustomerFeedback : NablFormBase
    {
        public long? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [MaxLength(200)]
        public string? CustomerName { get; set; }

        [MaxLength(200)]
        public string? ContactPerson { get; set; }

        public DateTime? FeedbackDate { get; set; }

        public DateTime? FeedbackPeriodFrom { get; set; }

        public DateTime? FeedbackPeriodTo { get; set; }

        // Dynamic ratings stored as JSON array: [{ parameter, rating }]
        // Matches frontend CustomerFeedback.ratings: FeedbackRating[]
        public string? RatingsJson { get; set; }

        // Individual rating fields (kept for querying/reporting)
        public int? OverallSatisfaction { get; set; } // 1-5

        public int? TurnaroundRating { get; set; }

        public int? AccuracyRating { get; set; }

        public int? CommunicationRating { get; set; }

        public int? ServiceRating { get; set; }

        public string? CommentsSuggestions { get; set; }

        public string? Suggestions { get; set; }

        public bool? WouldRecommend { get; set; }

        [MaxLength(200)]
        public string? CollectedBy { get; set; }
    }
}
