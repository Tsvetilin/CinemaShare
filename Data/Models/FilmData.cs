using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class FilmData
    {
        public FilmData()
        {
            this.Genre = new HashSet<GenreType>();
        }

        [Key]
        public string FilmId { get; set; }

        public virtual Film Film { get; set; }

        [MaxLength(50)]
        [Required]
        public string Title { get; set; }

        public string Poster { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public virtual IEnumerable<GenreType> Genre { get; set; }

        [MaxLength(50)]
        public string Director { get; set; }

        public string Cast { get; set; }

        public int Runtime { get; set; }

        public DateTime ReleaseDate { get; set; }

        public TargetAudience TargetAudience { get; set; }


    }
}
