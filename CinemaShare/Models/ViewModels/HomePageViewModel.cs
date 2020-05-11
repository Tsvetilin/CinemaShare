using System.Collections.Generic;

namespace CinemaShare.Models.ViewModels
{
    public class HomePageViewModel
    {
        public List<FilmCardViewModel> RecentFilms { get; set; }
        public List<FilmCardViewModel> TopFilms { get; set; }
    }
}
