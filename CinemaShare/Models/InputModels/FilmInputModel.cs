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
        [Range(0,5,ErrorMessage ="Rating can be between 0 and 5")]
        public int Rating { get; set; }
        
        public string Error { get; set; }
    }
}
