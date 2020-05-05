using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class FilmReview
    {
        public FilmReview()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        public string FilmId { get; set; }

        public string UserId { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual Film Film { get; set; }

        public virtual CinemaUser User { get; set; }
    }
}
