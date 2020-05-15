using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Cinema
    {
        public Cinema()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string Country { get; set; }

        public string ManagerId { get; set; }

        public virtual CinemaUser Manager { get; set; }
        
        public virtual IEnumerable<FilmProjection> FilmProjections { get; set; }
    }
}
