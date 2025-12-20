using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public class Form
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime OpenDate { get; set; } = DateTime.UtcNow;
        public DateTime? CloseDate { get; set; }

        public int MaxResponses { get; set; } = 0; 
        public int ResponseCount { get; set; } = 0;

        public virtual ICollection<FormResponse> Responses { get; set; }
        public virtual ICollection<FormQuestion> Questions { get; set; }
    }

    public class FormQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int FormId { get; set; }
        public int DisplayOrder { get; set; }

        [Required]
        [StringLength(500)]
        public string QuestionText { get; set; }

        public QuestionType Type { get; set; } = QuestionType.Text;

        [Column(TypeName = "text")]
        public string Options { get; set; } 

        public bool IsRequired { get; set; } = true;

        [ForeignKey("FormId")]
        public virtual Form Form { get; set; }
    }

    public enum QuestionType
    {
        Text,
        TextArea,
        Number,
        Email,
        Phone,
        Date,
        MultipleChoice,
        Checkbox,
        Rating
    }

    public class FormResponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int FormId { get; set; }
        public int? VolunteerId { get; set; } 

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string IPAddress { get; set; }

        [ForeignKey("FormId")]
        public virtual Form Form { get; set; }

        [ForeignKey("VolunteerId")]
        public virtual Account Volunteer { get; set; }

        public virtual ICollection<FormAnswer> Answers { get; set; }
    }

    public class FormAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ResponseId { get; set; }
        public int QuestionId { get; set; }

        [Column(TypeName = "text")]
        public string AnswerValue { get; set; }

        [ForeignKey("ResponseId")]
        public virtual FormResponse Response { get; set; }

        [ForeignKey("QuestionId")]
        public virtual FormQuestion Question { get; set; }
    }
}
