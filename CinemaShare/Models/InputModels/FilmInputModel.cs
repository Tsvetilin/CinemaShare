using Castle.Components.DictionaryAdapter;
using Data.Enums;
using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.InputModels
{
    public class FilmInputModel: FilmUpdateInputModel
    {

        [MaxLength(50)]
        [Required]
        public string Title { get; set; }

        [Required]
        [Range(1,5,ErrorMessage ="Rating can be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        [StringLength(maximumLength: 100,
                      ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                      MinimumLength = 3)]
        
        public string Error { get; set; }
    }
}
