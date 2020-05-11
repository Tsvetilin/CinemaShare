using System;
using System.ComponentModel.DataAnnotations;

namespace CinemaShare.Models.InputModels
{
    public class FilmInputModel: FilmUpdateInputModel
    {

        [MaxLength(50)]
        [Required]
        public string Title { get; set; }

        [Required]
        [Range(0,5,ErrorMessage ="Rating can be between 0 and 5")]
        public int Rating { get; set; }
        
        public string Error { get; set; }
    }
}
