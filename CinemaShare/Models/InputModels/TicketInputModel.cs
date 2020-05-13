using Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        [Display(Name ="Your seat")]
        public int Seat { get; set; }
    }
}
