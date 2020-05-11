using System;

namespace CinemaShare.Models.ViewModels
{
    public class ExtendedFilmCardViewModel: FilmCardViewModel
    {
        public string Cast { get; set; }
        public string Director { get; set; }
        public int Runtime { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
