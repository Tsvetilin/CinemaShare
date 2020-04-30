using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class FilmData
    {
        [Key]
        public string FilmId { get; set; }

        public string Poster { get; set; }

        public string Description { get; set; }

        public string Genre { get; set; }

        public string Title { get; set; }

        public string Director { get; set; }

        public string Cast { get; set; }

        public ushort Runtime { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string TargetAudience { get; set; }

        public virtual Film Film { get; set; }
    }
}
