using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.ViewModels
{
    public class FilmCardViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Poster { get; set; }

        public string Rating { get; set; }

        public string Genres { get; set; }
    }
}
