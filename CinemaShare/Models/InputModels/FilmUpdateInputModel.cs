using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaShare.Models.InputModels
{
    public class FilmUpdateInputModel
    {
        [Required]
        [StringLength(300, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                      MinimumLength = 20)]
        public string Poster { get; set; }

        [Required]
        [StringLength(maximumLength: 300,
                      ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                      MinimumLength = 20)]
        public string Description { get; set; }

        [Required]
        [MinLength(1,
            ErrorMessage = "The {0} must conatains at least {1} genre")]
        public List<Genre> Genre { get; set; }

        [Required]
        [StringLength(maximumLength: 100,
                      ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                      MinimumLength = 3)]
        public string Director { get; set; }

        [Required]
        [StringLength(maximumLength: 300,
                      ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                      MinimumLength = 3)]
        public string Cast { get; set; }

        [Required]
        [Range(0, 300, ErrorMessage = "Can only be between 0 .. 300")]
        public int Runtime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Release date")]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [Display(Name = "Target audience")]
        public TargetAudience TargetAudience { get; set; }
    }
}
