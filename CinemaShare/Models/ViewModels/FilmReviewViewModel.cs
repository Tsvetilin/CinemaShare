using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.ViewModels
{
    public class FilmReviewViewModel
    {
        public GenderType UserGender { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Username { get; set; }

        public string Content { get; set; }
    }
}
