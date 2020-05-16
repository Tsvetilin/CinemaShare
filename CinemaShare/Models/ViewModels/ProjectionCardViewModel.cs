using Data.Enums;
using System;

namespace CinemaShare.Models.ViewModels
{
    public class ProjectionCardViewModel
    {
        public string Id { get; set; }
       
        public string FilmTitle { get; set; }

        public DateTime Date { get; set; }

        public ProjectionType ProjectionType { get; set; }

        public string CinemaName { get; set; }

        public string CinemaCity { get; set; }

        public string CinemaCountry { get; set; }
    }
}
