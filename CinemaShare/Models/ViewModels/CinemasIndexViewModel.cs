using System.Collections.Generic;

namespace CinemaShare.Models.ViewModels
{
    public class CinemasIndexViewModel:PageViewModel
    {
        public List<CinemaCardViewModel> Cinemas { get; set; }
    }
}
