using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.ViewModels
{
    public class CinemaDataViewModel: CinemaCardViewModel
    {
        public List<FilmProjection> FilmProjections { get; set; }
    }
}
