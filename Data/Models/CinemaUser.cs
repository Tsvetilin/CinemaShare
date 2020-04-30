using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class CinemaUser:IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }
        
        public virtual IEnumerable<Film> AddedFilms { get; set; }
        public virtual IEnumerable<ProjectionTicket> ProjectionTickets { get; set; }
        public virtual IEnumerable<FilmReview> FilmReviews { get; set; }
        public virtual Cinema Cinema { get; set; }
    }
}
