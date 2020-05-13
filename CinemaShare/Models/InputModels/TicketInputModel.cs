using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.InputModels
{
    public class TicketInputModel
    {
        [Required]
        [Display(Name ="Ticket type")]
        public TicketType TicketType { get; set; }

        [Required]
        [Display(Name ="Available seats")]
        public List<int> Seat { get; set; }
    }
}
