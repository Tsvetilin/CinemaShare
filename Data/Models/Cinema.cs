using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey(nameof(Manager))]
        public string ManagerId { get; set; }

        public virtual CinemaUser Manager { get; set; }
        
    }
}
