using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.ViewModels
{
    public class ProjectionIndexViewModel
    {
        public int CurrentPage { get; set; }
        public int PagesCount { get; set; }
        public List<ProjectionCardViewModel> Projectitons { get; set; }
    }
}
