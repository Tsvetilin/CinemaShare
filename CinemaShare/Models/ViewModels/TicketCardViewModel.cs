using Data.Enums;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.ViewModels
{
    public class TicketCardViewModel
    {
        public string Id { get; set; }

        public FilmProjection Projection { get; set; }

        public TicketType Type { get; set; }

        public int Seat { get; set; }

        public double Price { get; set; }
    }
}
