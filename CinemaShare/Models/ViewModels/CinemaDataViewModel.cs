using Data.Models;
using System.Collections.Generic;

namespace CinemaShare.Models.ViewModels
{
    public class CinemaDataViewModel: CinemaCardViewModel
    {
        public List<ProjectionCardViewModel> FilmProjections { get; set; }
    }
}
