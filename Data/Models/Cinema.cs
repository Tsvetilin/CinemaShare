using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Models
{
    public class Cinema
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        [ForeignKey(nameof(Manager))]
        public Guid ManagerId { get; set; }
        public virtual CinemaUser Manager { get; set; }
        
    }
}
