using System.ComponentModel.DataAnnotations;

namespace CinemaShare.Models.InputModels
{
    public class FilmReviewInputModel
    {
        [Required]
        [Display(Name = "Review")]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Content { get; set; }
    }
}
