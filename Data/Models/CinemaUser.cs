using Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class CinemaUser:IdentityUser
    {
        public CinemaUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.AddedFilms = new HashSet<Film>();
            this.ProjectionTickets = new HashSet<ProjectionTicket>();
            this.FilmReviews = new HashSet<FilmReview>();
            this.WatchList = new HashSet<Film>();
        }

        [PersonalData]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [PersonalData]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public GenderType Gender { get; set; }

        [ForeignKey(nameof(AddedFilms))]
        public virtual IEnumerable<Film> AddedFilms { get; set; }

        public virtual IEnumerable<ProjectionTicket> ProjectionTickets { get; set; }

        public virtual IEnumerable<FilmReview> FilmReviews { get; set; }

        public virtual IEnumerable<Film> WatchList { get; set; }

        public virtual Cinema Cinema { get; set; }
    }
}
