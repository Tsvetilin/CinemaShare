using System;

namespace Data.Models
{
    public class FilmRating
    {
        public FilmRating()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }

        public string UserId { get; set; }

        public virtual CinemaUser User { get; set; }

        public int Rating { get; set; }
    }
}
