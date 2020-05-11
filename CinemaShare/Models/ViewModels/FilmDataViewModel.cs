﻿using Data.Enums;
using Data.Models;
using System.Collections.Generic;

namespace CinemaShare.Models.ViewModels
{
    public class FilmDataViewModel : ExtendedFilmCardViewModel
    {
        public string CreatedByUserId { get; set; }
        public TargetAudience TargetAudience { get; set; }
        public List<FilmReview> FilmReviews { get; set; }
        public List<FilmProjection> FilmProjections { get; set; }
    }
}
