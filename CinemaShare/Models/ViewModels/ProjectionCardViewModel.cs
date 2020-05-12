using Data.Enums;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
