using Data.Models;
using System.Collections.Generic;

namespace CinemaShare.Models.ViewModels
{
    public class CinemaDataViewModel: CinemaCardViewModel
    {
        public List<FilmProjection> FilmProjections { get; set; }
    }
}
