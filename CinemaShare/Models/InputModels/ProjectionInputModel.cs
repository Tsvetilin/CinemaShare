using Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CinemaShare.Models.InputModels
{
    public class ProjectionInputModel
    {
        [Required]
        [Display(Name = "Film title")]
        public string FilmTitle { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Projection type")]
        public ProjectionType ProjectionType { get; set; }

        [Required]
        [Display(Name ="Total tickets")]
        [Range(1,500, ErrorMessage ="{0} must be between {1} and {2}")]
        public int TotalTickets { get; set; }

        [Required]
        [Display(Name = "Children ticket price")]
        [Range(1, 200, ErrorMessage = "{0} must be between {1} and {2}")]
        public double ChildrenTicketPrice { get; set; }

        [Required]
        [Display(Name = "Students ticket price")]
        [Range(1, 200, ErrorMessage = "{0} must be between {1} and {2}")]
        public double StudentsTicketPrice { get; set; }

        [Required]
        [Display(Name = "Adults ticket price")]
        [Range(1, 200, ErrorMessage = "{0} must be between {1} and {2}")]
        public double AdultsTicketPrice { get; set; }
    }
}
