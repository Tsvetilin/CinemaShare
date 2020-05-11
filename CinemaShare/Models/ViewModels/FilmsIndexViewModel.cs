using System.Collections.Generic;

namespace CinemaShare.Models.ViewModels
{
    public class FilmsIndexViewModel
    {
        public int CurrentPage { get; set; }
        public int PagesCount { get; set; }
        public List<ExtendedFilmCardViewModel> Films { get; set; }
    }
}
