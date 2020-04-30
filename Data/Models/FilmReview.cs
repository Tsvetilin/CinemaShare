using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class FilmReview
    {
        [Key]
        public string Id { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual Film Film { get; set; }
        public virtual CinemaUser User { get; set; }
    }
}
