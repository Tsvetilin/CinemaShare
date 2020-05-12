using Data.Enums;
using Data.Models;
using System;

namespace CinemaShare.Models.ViewModels
{
    public class ProjectionDataViewModel: ProjectionCardViewModel
    {
        public int TotalTickets { get; set; }
        public int FilmRuntime { get; set; }
        public TargetAudience FilmTargetAudience { get; set; }
        public int TicketsSold { get; set; }
    }
}
