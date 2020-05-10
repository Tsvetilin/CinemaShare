using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.InputModels
{
    public class CinemaInputModel
    {
        [MaxLength(50)]
        [Required]
        [Display(Name="Name")]
        public string Name { get; set; }

        [MaxLength(50)]
        [Required]
        [Display(Name="Country")]
        public string Country { get; set; }

        [MaxLength(50)]
        [Required]
        [Display(Name="City")]
        public string City { get; set; }

        public string Error { get; set; }
    }
}
