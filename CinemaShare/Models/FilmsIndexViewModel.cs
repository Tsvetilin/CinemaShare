using CinemaShare.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models
{
    public class FilmsIndexViewModel
    {
        public int CurrentPage { get; set; }
        public int PagesCount { get; set; }
        public List<ExtendedFilmCardViewModel> Films { get; set; }
    }
}
