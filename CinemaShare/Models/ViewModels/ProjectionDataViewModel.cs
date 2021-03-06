﻿using Data.Enums;

namespace CinemaShare.Models.ViewModels
{
    public class ProjectionDataViewModel: ProjectionCardViewModel
    {
        public int TotalTickets { get; set; }
       
        public int FilmRuntime { get; set; }
      
        public TargetAudience FilmTargetAudience { get; set; }
       
        public int TicketsSold { get; set; }

        public int AvailableTickets { get { return TotalTickets - TicketsSold; }  }

        public double ChildrenTicketPrice { get; set; }

        public double StudentsTicketPrice { get; set; }

        public double AdultsTicketPrice { get; set; }

        public string FilmId { get; set; }

        public string CinemaId { get; set; }

    }
}
