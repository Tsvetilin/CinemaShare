﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;

namespace Data.Models
{
    public class Film
    {
        public Film()
        {
            this.Id = Guid.NewGuid().ToString();
            this.FilmReviews = new HashSet<FilmReview>();
            this.Ratings = new HashSet<FilmRating>();
        }

        [Key]
        public string Id { get; set; }

        public double Rating { get; set; }

        public string AddedByUserId { get; set; }

        public virtual CinemaUser AddedByUser { get; set; }

        public virtual FilmData FilmData { get; set; }

        public virtual IEnumerable<FilmRating> Ratings { get; set; }

        public virtual IEnumerable<FilmProjection> FilmProjection { get; set; }

        public virtual IEnumerable<FilmReview> FilmReviews { get; set; }

        public virtual IEnumerable<CinemaUser> OnWatchlistUsers { get; set; }

    }
}
