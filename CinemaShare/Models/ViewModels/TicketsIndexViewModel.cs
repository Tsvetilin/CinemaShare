using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.ViewModels
{
    public class TicketsIndexViewModel : PageViewModel
    {
        public List<TicketCardViewModel> Tickets { get; set; }
    }
}
