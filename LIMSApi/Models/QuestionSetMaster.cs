using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIMSApi.Models
{
    public class QuestionSetMaster : AuditProperty
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string QuestionNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public string? QuestionSetJson { get; set; }
        public string? ResultConfigurationsJson { get; set; }
        [NotMapped]
        public List<QuestionSetQuestion> Questions { get; set; } = new();
        [NotMapped]
        public List<ResultConfigurations> ResultConfigurations { get; set; } = new();
        

    }
    [NotMapped]
    public class QuestionSetQuestion
    {
        public int QuestionNo { get; set; }

        public string? Question { get; set; }

        public string? OptionA { get; set; }

        public string? OptionB { get; set; }

        public string? OptionC { get; set; }

        public string? OptionD { get; set; }

        // A / B / C / D
        public string? CorrectAnswer { get; set; }
    }
    [NotMapped]
    public class ResultConfigurations
    {
        public string? Interpretation { get; set; }
        public string? Percentage { get; set; }
        public string? MarksObtained { get; set; }

    }
}
