using Data.Enums;
using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models
{
    public class FilmInputModel
    {

        [MaxLength(50)]
        [Required]
        public string Title { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 20)]
        public string Poster { get; set; }

        [Required]
        [StringLength(maximumLength: 300, 
                      ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                      MinimumLength = 20)]
        public string Description { get; set; }

        [Required]
        public List<GenreType> Genre { get; set; }

        [Required]

        public string Director { get; set; }

        [Required]
        public string Cast { get; set; }
        
        [Required]
        public int Runtime { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public TargetAudience TargetAudience { get; set; }

    }
}
