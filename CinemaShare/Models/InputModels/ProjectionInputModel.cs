using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.InputModels
{
    public class ProjectionInputModel
    {
        [Required]
        [Display(Name = "Film title")]
        public string FilmTitle { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Projection type")]
        public ProjectionType ProjectionType { get; set; }

        [Required]
        [Display(Name ="Total tickets")]
        [Range(1,500, ErrorMessage ="{0} must be between {1} and {2}")]
        public int TotalTickets { get; set; }
    }
}
