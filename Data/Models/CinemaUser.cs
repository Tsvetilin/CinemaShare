using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        
        public virtual IEnumerable<Film> AddedFilms { get; set; }

        public virtual IEnumerable<ProjectionTicket> ProjectionTickets { get; set; }

        public virtual IEnumerable<FilmReview> FilmReviews { get; set; }

        public virtual Cinema Cinema { get; set; }
    }
}
