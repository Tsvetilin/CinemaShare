using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.ViewModels
{
    public class HomePageViewModel
    {
        public List<FilmCardViewModel> RecentFilms { get; set; }
        public List<FilmCardViewModel> TopFilms { get; set; }
    }
}
