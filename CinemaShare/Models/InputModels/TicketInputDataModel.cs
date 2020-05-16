using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CinemaShare.Models.InputModels
{
    public class TicketInputDataModel
    {
        [Display(Name = "Available seats")]
        public SelectList AvailableSeats { get; set; }

        public double ChildrenPrice { get; set; }
        public double StudentPrice { get; set; }
        public double AdultPrice { get; set; }
    }
}
