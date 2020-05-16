using Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace CinemaShare.Models.InputModels
{
    public class TicketInputModel
    {
        [Required]
        [Display(Name ="Ticket type")]
        public TicketType TicketType { get; set; }

        [Required]
        [Display(Name ="Your seat")]
        public int Seat { get; set; }
    }
}
