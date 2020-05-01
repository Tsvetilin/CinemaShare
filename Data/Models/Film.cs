using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Film
    {
        public Film()
        {
            this.Id = Guid.NewGuid().ToString();
            this.FilmReviews = new HashSet<FilmReview>();
        }

        [Key]
        public string Id { get; set; }

        public byte Rating { get; set; }

        public virtual CinemaUser AddedByUser { get; set; }

        public virtual FilmData FilmData { get; set; }

        public virtual FilmProjection FilmProjection { get; set; }

        public virtual IEnumerable<FilmReview> FilmReviews { get; set; }

    }
}
