using Data.Enums;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.ViewModels
{
    public class ProjectionCardViewModel
    {
        public string Id { get; set; }
       
        public Film film { get; set; }

        public DateTime Date { get; set; }

        public ProjectionType ProjectionType { get; set; }

        public Cinema cinema { get; set; }
    }
}
