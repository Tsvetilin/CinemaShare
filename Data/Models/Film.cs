using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Models
{
    public class Film
    {
        [Key]
        public string Id { get; set; }

        public byte Rating { get; set; }

        public virtual CinemaUser AddedByUser { get; set; }
        public virtual FilmData FilmData { get; set; }
        public virtual FilmProjection FilmProjection { get; set; }

        public virtual IEnumerable<FilmReview> FilmReviews { get; set; }

    }
}
