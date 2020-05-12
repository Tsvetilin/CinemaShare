using System.Collections.Generic;

namespace CinemaShare.Models.ViewModels
{
    public class FilmsIndexViewModel:PageViewModel
    {
        public List<ExtendedFilmCardViewModel> Films { get; set; }
    }
}
