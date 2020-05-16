using Data.Enums;
using Data.Models;
using System;

namespace CinemaShare.Models.ViewModels
{
    public class TicketCardViewModel
    {
        public string Id { get; set; }

        public string CinemaName { get; set; }

        public string FilmTitle { get; set; }

        public ProjectionType ProjectionType { get; set; }

        public DateTime ProjectionDate { get; set; }

        public TicketType Type { get; set; }

        public int Seat { get; set; }

        public double Price { get; set; }
    }
}
