using System.Collections.Generic;

namespace CinemaShare.Models.ViewModels
{
    public class CinemasIndexViewModel
    {
        public int CurrentPage { get; set; }
        public int PagesCount { get; set; }
        public List<CinemaCardViewModel> Cinemas { get; set; }
    }
}
